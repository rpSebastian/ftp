using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        public void readFileStream(FileStream fs)
        { 
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            socket.Send(buffer);
        }
        public string getMessage()
        { 
            String message = Encoding.UTF8.GetString(buffer, 0, bufferLength);
            return message;
        }
    }
}
