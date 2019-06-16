using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
        public void downloadFile(String fname, string path)
        {
            int fileSize = cs.SIZE(fname);
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.RETR(fname);
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
            cs.DATA_END();
        }
        public List<string> getNameList(string pathName, char type)
        {
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.LIST(pathName);
            ds.RECV();
            cs.DATA_END();
            string message = ds.getMessage().Trim();
            string[] infoList = message.Split('\n');
            Console.WriteLine(message);
            List<string> nameList = new List<string>();
            for (int i = 0; i < infoList.Length; ++i)
            {
                String info = infoList[i].Trim();
                int id = info.Length - 1;
                while (info[id] != ' ')
                    id--;
                String name = info.Substring(id + 1, info.Length - id - 1);
                if (info[0] == type)
                    nameList.Add(name);
            }
            foreach (string file in nameList)
            {
                Console.WriteLine(file);
            }
            return nameList;
        }
        
        public void uploadFile(string fname, string path)
        {
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.STOR(fname);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            ds.readFileStream(fs);
        }
        /*
        public static void Main(String[] args)
        {
            FtpClient fc = new FtpClient();
            fc.login();
            fc.uploadFile("3.txt");
        }
        */
    }
}
