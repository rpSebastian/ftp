using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ftpClient
{
    class DataSocket
    {
        private static Byte[] buffer = new Byte[1024 * 1024 * 2];
        private int bufferLength;
        private Socket socket;
        public DataSocket(IPAddress ip, int port)
        {
            IPEndPoint ipe = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
        }
        ~DataSocket()
        { 
            socket.Close();
        }
        public void RECV()
        {
            bufferLength = socket.Receive(buffer);
        }
        public int Size()
        { 
            return bufferLength;
        }
        public void writeFileStream(FileStream fs)
        {
            fs.Write(buffer, 0, bufferLength);
        }
    }
}
