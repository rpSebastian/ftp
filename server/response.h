//
// Created by jiahua on 19-6-18.
//

#pragma once
#include <string>

namespace FtpResponse {
    constexpr char hello[] = "220 Welcome!";
    constexpr char passwd[] = "331 Please specify the password.";
    constexpr char passwdOk[] = "230 Login successful.";
    constexpr char passwdNotOk[] = "530 Not logged in, user or password incorrect!";
    constexpr char startFileTransfer[] = "150 Opening BINARY mode data connection for file transfer.";
    constexpr char stopFileTransfer[] = "226 Transfer complete";
    constexpr char startListTransfer[] = "150 Opening ASCII mode data connection for file transfer.";
    constexpr char stopListTransfer[] = "226 Transfer complete";
    constexpr char startRecvFile[] = "150 Opening BINARY mode data connection for file transfer.";
}
