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
        private int serverPort;
        private String user;
        private String pass;
        public FtpClient(String serverIp = "127.0.0.1", int serverPort = 12345, String user = "xh", String pass = "123456")
        {
            this.serverIp = IPAddress.Parse(serverIp);
            this.user = user;
            this.pass = pass;
            this.serverPort = serverPort;
            try
            {
                CallWithTimeout(connect, 2000);
            }
            catch (TimeoutException)
            {
                throw new MyException("服务器Ip或端口号错误");
            }
            catch (SocketException)
            {
                throw new MyException("端口号ip或端口号错误");
            }
        }
        public void connect()
        {
            cs = new ControlSocket(this.serverIp, serverPort);
        }
        public void login()
        {  
            cs.USER(user);
            cs.PASS(pass);
        }
        public void downloadFile(String fname, string saveName, int continueTransport = 0)
        {
            long fileSize = cs.SIZE(fname);
            long currentSize = 0;
            if (continueTransport == 1)
            { 
                using (FileStream fs = File.OpenRead(saveName))
                { 
                    currentSize = fs.Length;
                    cs.REST(currentSize);
                }
            }
            else 
            {
                using (FileStream fs = File.Create(saveName))
                    currentSize = 0;
            }
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.RETR(fname);

            using (FileStream fs = File.OpenWrite(saveName))
            {
                fs.Position = fs.Length;
                while (currentSize < fileSize)
                {
                    ds.RECV();
                    currentSize += ds.Size();
                    ds.writeFileStream(fs);
                    Console.Write(currentSize);
                    Console.Write(" ");
                    Console.WriteLine(fileSize);
                }
            }
            cs.DATA_END();
        }
        public List<string> getNameList(string pathName, char type)
        {
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            cs.LIST(pathName);
            string message = "";
            while (true)
            {
                ds.RECV();
                if (ds.Size() == 0)
                    break;
                message += ds.getMessage();
            }
            message = message.Trim();
            cs.DATA_END();
            string[] infoList = message.Split('\n');
            List<string> nameList = new List<string>();
            for (int i = 0; i < infoList.Length; ++i)
            {
                String info = infoList[i].Trim();
                int id = info.Length - 1;
                while (info[id] != ' ')
                    id--;
                String name = info.Substring(id + 1, info.Length - id - 1);
                if (info[0] == type && name != "." && name != "..")
                    nameList.Add(name);
            }
            foreach (string file in nameList)
            {
                Console.WriteLine(file);
            }
            return nameList;
        }
        public void uploadFile(string saveName, string fname, string path, int continueTransport = 0)
        {
            cs.CWD(path);
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read);
            if (continueTransport == 1)
            {
                long fileSize = cs.SIZE(path + saveName);
                fs.Position = fileSize;
            }
            Console.WriteLine(fs.Position);
            int port = cs.PASV();
            DataSocket ds = new DataSocket(this.serverIp, port);
            if (continueTransport == 1)
                cs.APPE(saveName);
            else
                cs.STOR(saveName);
            ds.readFileStream(fs);
            //cs.DATA_END();
        }
        private void CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }
        //public static void Main(String[] args)
        //{
        //    try
        //    {
        //        FtpClient fc = new FtpClient("192.168.31.164", 9090, "xh", "123456");
        //        fc.login();
        //        fc.downloadFile("1.exe", "F:\\1.exe", 1);
        //        //fc.uploadFile("1.exe", "F:\\1.exe", "/", 1);
        //        //fc.getNameList("/", '-');
        //    }
        //    catch (MyException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}
    }
}