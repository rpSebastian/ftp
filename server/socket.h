//
// Created by jiahua on 19-6-18.
//

#pragma once
#include <cstddef>
#include <string>
#include <cstring>
#include <unistd.h>
#include <sys/socket.h>

class Socket {
private:
    constexpr static size_t kDefaultBufSize = 4096;
    int sockfd_;
    char buf_[kDefaultBufSize]{};
public:
    explicit Socket(int sockfd);
    ~Socket() noexcept;

    int Send(const std::string &msg, int flags = 0);
    int SendBuf(const char *buf, size_t size, int flags = 0);
    std::string Recv();
    Socket Accept();

    int Reset(int sockfd);

    // No copying
    Socket(const Socket &) = delete;
    Socket &operator=(const Socket &) = delete;

    // Can be moved
    Socket(Socket &&rhs) noexcept ;
    Socket &operator=(Socket &&rhs) noexcept ;

    Socket Release();

    bool IsOk() const;
};


