#include <netdb.h>
#include <iostream>
#include "server.h"

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

Server::Server(const std::string &host, size_t port) : sockfd_(-1) {
    struct addrinfo *ailist, *aip;
    int fd, err, n;

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
            sockfd_ = fd;
        }
    }
}

bool Server::IsSockOk() const {
    return sockfd_ != -1;
}

Server::~Server() {
    close(sockfd_);
}
