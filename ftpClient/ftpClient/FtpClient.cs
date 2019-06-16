using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;

namespace ftpClient
{
    class FtpClient
    {
        private ControlSocket cs;
        private IPAddress serverIp;
        private String user;
        private String pass;
        public FtpClient(String serverIp = "127.0.0.1", int serverPort = 12345, String user = "xh", String pass = "123456")
        {
            this.serverIp = IPAddress.Parse(serverIp);
            this.user = user;
            this.pass = pass;
            cs = new ControlSocket(this.serverIp, serverPort);
        }
        public void login()
        {
            cs.USER(user);
            cs.PASS(pass);
        }
        private void downloadFile(String fname)
        {
            int fileSize = cs.SIZE(fname);
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.RETR(fname);
            string path = $"C:\\Users\\rpSebastian\\Desktop\\{fname}";
            using (FileStream fs = File.Create(path))
            {
                int currentSize = 0;
                while (currentSize < fileSize)
                {
                    ds.RECV();
                    currentSize += ds.Size();
                    ds.writeFileStream(fs);
                }
            }
            cs.RETR_END();
        }
    }
}
