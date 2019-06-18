#include <iostream>
#include <vector>
#include <map>
#include <cassert>
#include <pwd.h>
#include "server.h"

using std::cout;
using std::endl;
using std::cerr;


std::map<std::string, std::string> parseFlags(int argc, const char *argv[]);

std::string getHomePath();

int main(int argc, const char *argv[]) {
    int port = 9090;
    std::string host = "127.0.0.1";
    std::string path = getHomePath();

    auto mp = parseFlags(argc, argv);

    if (mp.find("port") != mp.end())
        port = std::stoi(mp["port"]);

    if (mp.find("host") != mp.end())
        host = mp["host"];

    if (mp.find("prefix") != mp.end())
        path = mp["prefix"];


    Server server(host, port, path);
    assert(server.IsSockOk());
    server.ServeFtp();
}

std::map<std::string, std::string> parseFlags(int argc, const char *argv[]) {
    std::map<std::string, std::string> mp;
    for (int i = 1; i < argc; i++) {
        std::string arg(argv[i]);
        assert(!arg.empty());
        if (arg[0] != '-') continue;
        size_t deli = arg.find('=');
        if (deli == std::string::npos) continue;
        std::string key = arg.substr(1, deli - 1);
        std::string val = arg.substr(deli + 1);
        mp[key] = val;
    }
    return mp;
}

std::string getHomePath() {
    struct passwd *pw = getpwuid(getuid());
    const char *homedir = pw->pw_dir;
    return std::string(homedir);
}
