#include <cerrno>
#include <string>
#include <sys/socket.h>
#include <unistd.h>

class Server {
private:
    static constexpr int kHostNameMax = 256;
    static constexpr int kQLen = 10;
private:
    int sockfd_;

    static int InitServer(int type, const struct sockaddr *addr, socklen_t alen, int qlen);
public:
    Server(const std::string &host, size_t port);
    ~Server();

    int Accept();

    bool IsSockOk() const;
};