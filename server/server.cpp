#include <utility>
#include <netdb.h>
#include <iostream>
#include <vector>
#include <sstream>
#include <sys/stat.h>
#include <cassert>
#include <fcntl.h>
#include <dirent.h>
#include "server.h"
#include "response.h"

int Server::freePort = 40000;

int Server::InitServer(int type, const struct sockaddr *addr, socklen_t alen, int qlen) {
    int fd, err;
    int reuse = 1;

    if ((fd = socket(addr->sa_family, type, 0)) < 0)
        return (-1);
    if (setsockopt(fd, SOL_SOCKET, SO_REUSEADDR, &reuse,
                   sizeof(int)) < 0)
        goto errout;
    if (bind(fd, addr, alen) < 0)
        goto errout;
    if (type == SOCK_STREAM || type == SOCK_SEQPACKET)
        if (listen(fd, qlen) < 0)
            goto errout;
    return (fd);

    errout:
    err = errno;
    close(fd);
    errno = err;
    return (-1);
}

void Server::Log(const std::string &msg) {
    std::cerr << msg << std::endl;
}

Server::Server(const std::string &host, int port, std::string filepath) :
        socket_(-1), path_(std::move(filepath)), host_(host), port_(port) {
    struct addrinfo *ailist, *aip;
    int fd, err;

    if ((err = getaddrinfo(host.c_str(),
                           std::to_string(port).c_str(),
                           nullptr, &ailist)) != 0) {
        std::cerr << "Server: getaddrinfo error: "
                  << gai_strerror(err) << std::endl;
        return;
    }
    for (aip = ailist; aip != nullptr; aip = aip->ai_next) {
        if ((fd = InitServer(SOCK_STREAM, aip->ai_addr,
                             aip->ai_addrlen, kQLen)) >= 0) {
            socket_.Reset(fd);
        }
    }
    signal(SIGPIPE, Socket::SigHandler);
}

bool Server::IsSockOk() const {
    return socket_.IsOk();
}

Server::~Server() noexcept = default;

void Server::TestServe() {
    Log("Beginning Test Service");
    for (;;) {
        Socket clsk = socket_.Accept();
        if (!clsk.IsOk())
            continue;
        clsk.Send("Hello, nice to meet you!");
        std::string msg;
        msg = clsk.Recv();
        Log("Recv: " + msg);
        while (!msg.empty()) {
            if (msg == "end all")
                return;
            int n = clsk.Send(msg);
            if (n == EPIPE)
                Log("Error: Client shutdown the connection");
            msg = clsk.Recv();
            Log("Recv: " + msg);
        }
    }
}

std::vector<std::string> parseCommand(const std::string &str) {
    std::istringstream is(str);
    std::vector<std::string> ret;
    std::string s;
    while ((is >> s)) {
        ret.push_back(s);
    }
    return ret;
}

size_t getFileSize(const std::string &filename) {
    struct stat s;
    if (stat(filename.c_str(), &s) < 0) {
        return 0;
    }
    return s.st_size;
}


void Server::ServeFtp() {
    Log("FTP Server Start running on " + host_ + ": "
        + std::to_string(port_) + " with prefix: " + path_);
    for (;;) {
        std::string msg;
        Socket clsk = socket_.Accept();
        Socket dtsk(-1);

        if (!clsk.IsOk())
            continue;
        FtpSayHello(clsk);

        //---------------------Login Check------------------------
        msg = clsk.Recv();
        if (msg.size() <= 5 || msg.substr(0, 5) != "USER ") {
            Log("user command not correct");
            clsk.Send("user command not correct");
            continue;
        }
        // TODO: check user's username
        clsk.Send(FtpResponse::passwd);
        if (ErrorPipe()) continue;
        msg = clsk.Recv();
        if (msg.size() <= 5 || msg.substr(0, 5) != "PASS ") {
            Log("password command error");
            clsk.Send("password command not correct");
            continue;
        }
        // TODO: check user's password
        clsk.Send(FtpResponse::passwdOk);
        if (ErrorPipe()) continue;
        //---------------------------------------------------------


        //-------------------Interactivity-------------------------
        Log("Start user control");
        msg = clsk.Recv();
        std::string currPath = path_;
        while (!msg.empty()) {
            std::vector<std::string> cmds = parseCommand(msg);
            if (cmds.empty())
                break;
            std::string &cmd = cmds[0];

            if (cmd == "SIZE" && cmds.size() > 1) {
                std::string filename = currPath + cmds[1];
                size_t sz = getFileSize(filename);
                clsk.Send("213 " + std::to_string(sz));
                if (ErrorPipe()) break;
            } else if (cmd == "CWD" && cmds.size() > 1) {
                if (cmds[1].back() != '/') cmds[1].push_back('/');
                std::string pathname = currPath + cmds[1];
                std::ostringstream os;
                os << "250 \"" << pathname << "\" is current directory.";
                clsk.Send(os.str());
                if (ErrorPipe()) break;
                if (pathname.back() != '/') pathname.push_back('/');
                currPath = pathname;
            } else if (cmd == "PASV") {
                int port = getFreePort();
                Server tmpServer(host_, port, path_);
                std::string tmpHost = host_;
                dtsk = tmpServer.ReleaseSocket();
                assert(dtsk.IsOk());

                for (char &ch : tmpHost) if (ch == '.') ch = ',';
                std::ostringstream os;
                os << "227 Entering Passive Mode (" << tmpHost << ","
                   << port / 256 << "," << port % 256 << ").";

                clsk.Send(os.str());
                if (ErrorPipe()) break;
            } else if (cmd == "RETR" && cmds.size() > 1) {
                std::string filename = currPath + cmds[1];
                Socket accSk = dtsk.Release();
                if (!accSk.IsOk()) {
                    Log("PASV should be called first");
                    break;
                }

                clsk.Send(FtpResponse::startFileTransfer);
                if (ErrorPipe()) break;
                Socket wrSk = accSk.Accept();
                assert(wrSk.IsOk());
                Log("New socket for writing");

                int fd;
                fd = open(filename.c_str(), O_RDONLY);
                if (fd > 0) {
                    char buf[kBufSize];
                    int n;
                    while ((n = read(fd, buf, sizeof(buf))) > 0) {
                        wrSk.SendBuf(buf, n);
                        if (ErrorPipe()) break;
                    }
                }
                close(fd);
                clsk.Send(FtpResponse::stopFileTransfer);
                if (ErrorPipe()) break;
            } else if (cmd == "LIST") {
                std::string pathname = currPath;

                if (cmds.size() > 1)
                    pathname += cmds[1];
                if (pathname.back() != '/')
                    pathname.push_back('/');

                Socket accSk = dtsk.Release();
                if (!accSk.IsOk()) {
                    Log("PASV should be called first");
                    break;
                }

                clsk.Send(FtpResponse::startListTransfer);
                if (ErrorPipe()) break;
                Socket wrSk = accSk.Accept();

                DIR *dp = opendir(pathname.c_str());

                while (dp != nullptr) {
                    struct dirent *dr = readdir(dp);
                    if (dr == nullptr) break;
                    auto type = dr->d_type;
                    char typech = '-';
                    if (type == DT_DIR) typech = 'd';
                    else if (type == DT_LNK) typech = 'l';
                    std::ostringstream os;
                    os << typech << ' ' << dr->d_name << '\n';
                    wrSk.Send(os.str());
                    if (ErrorPipe()) break;
                }
                closedir(dp);
                clsk.Send(FtpResponse::stopListTransfer);
                if (ErrorPipe()) break;
            } else if (cmd == "STOR" && cmds.size() > 1) {
                std::string filename = currPath + cmds[1];
                Socket accSk = dtsk.Release();
                if (!accSk.IsOk()) {
                    Log("PASV should be called first");
                    break;
                }

                clsk.Send(FtpResponse::startRecvFile);
                if (ErrorPipe()) break;
                Socket rdSk = accSk.Accept();

                int fd, n;
                char buf[kBufSize];
                fd = open(filename.c_str(), O_RDWR | O_TRUNC | O_CREAT);
                if (fd > 0) {
                    n = rdSk.RecvBuf(buf, kBufSize);
                    while (n > 0) {
                        Log("Receive " + std::to_string(n) + " bytes");
                        write(fd, buf, n);
                        n = rdSk.RecvBuf(buf, kBufSize);
                    }
                    Log("Connection closed by client");
                }
                fchmod(fd, 0600);
                close(fd);
            } else {
                break;
            }

            msg = clsk.Recv();
        }
        //---------------------------------------------------------

    }
}

void Server::FtpSayHello(Socket &socket) {
    socket.Send(FtpResponse::hello);
}

inline bool Server::ErrorPipe() {
    if (errno == EPIPE) {
        Log("Unexpected connection lost");
        return true;
    }
    return false;
}

int Server::getFreePort() {
    return freePort++;
}

Socket Server::ReleaseSocket() {
    return std::move(socket_);
}


