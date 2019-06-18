#include <iostream>
#include "server.h"

using std::cout;
using std::endl;
using std::cerr;

int main(int argc, const char *argv[]) {
    cout << "Hello" << endl;
    Server server("127.0.0.1", 9090);
}