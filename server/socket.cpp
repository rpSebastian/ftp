//
// Created by jiahua on 19-6-18.
//

#include <errno.h>
#include "socket.h"

Socket::Socket(int sockfd): sockfd_(sockfd) {
    memset(buf_, 0, sizeof(buf_));
}

Socket::~Socket() noexcept {
    if (sockfd_ != -1)
        close(sockfd_);
}

int Socket::Send(const std::string &msg, int flags) {
    const char *ps = msg.c_str();
    int remain = msg.size();
    int n;

    n = send(sockfd_, ps, remain, flags);

    if (n == EPIPE)
        return EPIPE;

    if (n == -1)
        return (-1);

    return remain - n;
}

std::string Socket::Recv() {
    int n;
    n = recv(sockfd_, buf_, sizeof(buf_), 0);
    if (n == -1)
        return std::string();
    buf_[n] = '\0';
    return std::string(buf_);
}

bool Socket::IsOk() const {
    return sockfd_ != -1;
}

int Socket::Reset(int sockfd) {
    int ret = sockfd_;
    sockfd_ = sockfd;
    return ret;
}

Socket Socket::Accept() {
    return Socket(accept(sockfd_, nullptr, nullptr));
}

Socket::Socket(Socket &&rhs) noexcept {
    this->sockfd_ = rhs.sockfd_;
    rhs.sockfd_= -1;
}

Socket &Socket::operator=(Socket &&rhs) noexcept {
    this->sockfd_ = rhs.sockfd_;
    rhs.sockfd_ = -1;
    return *this;
}

int Socket::SendBuf(const char *buf, size_t size, int flags) {
    return send(sockfd_, buf, size, flags);
}

Socket Socket::Release() {
    int ret = sockfd_;
    sockfd_ = -1;
    return std::move(Socket(ret));
}

void Socket::SigHandler(int signo) {
    errno = EPIPE;
}

int Socket::RecvBuf(char *buf, size_t size) {
    return recv(sockfd_, buf, size, 0);
}
