#pragma once

#include <cerrno>
#include <string>
#include <sys/socket.h>
#include <unistd.h>
#include "socket.h"

class Server {
private:
    static constexpr int kHostNameMax = 256;
    static constexpr int kQLen = 10;
    static int freePort;
    static int getFreePort();
private:
    Socket socket_;
    std::string path_;
    std::string host_;
    int port_;

    static int InitServer(int type, const struct sockaddr *addr, socklen_t alen, int qlen);
    static void Log(const std::string &msg);

    static void FtpSayHello(Socket &socket);

    inline static bool ErrorPipe();
public:
    Server(const std::string &host, int port, std::string filepath);
    ~Server() noexcept ;

    void TestServe();

    void ServeFtp();

    Socket ReleaseSocket();

    bool IsSockOk() const;
};