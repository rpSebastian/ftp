using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace test
{
    class Program
    {
        public static void WriteLog(string strLog)
        {
            string sFilePath = "E:\\c#\\ftp\\ftpClient\\test";
            string sFileName = "event.txt";
            sFileName = sFilePath + "\\" + sFileName; //文件的绝对路径
            if (!Directory.Exists(sFilePath))//验证路径是否存在
            {
                Directory.CreateDirectory(sFilePath);
                //不存在则创建
            }
            FileStream fs;
            StreamWriter sw;
            
            if (System.IO.File.Exists(sFileName))
            //验证文件是否存在，有则追加，无则创建
            {
                fs = new FileStream(sFileName, FileMode.Append, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(sFileName, FileMode.Create, FileAccess.Write);
            }
            sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "\t"+ strLog);

          
            sw.Close();
            fs.Close();
        }
        static void Main(string[] args)
        {
            string str = "127.0.0.1\t" + "12345\t" + "filename\t" + "0\t" + "true";

            WriteLog(str);

            StreamReader sr = new StreamReader("E:\\c#\\ftp\\ftpClient\\test\\event.txt");
            String line = sr.ReadToEnd();
            Console.WriteLine(line);



        }

    }
}
