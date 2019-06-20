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
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "\t" + strLog);


            sw.Close();
            fs.Close();
        }

        private void reLinked(string filename, string curInfo)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line1 = sr.ReadLine();
                    if (line1.Substring(line1.Length - 5) == "false")
                    {
                        var infos = line1.Replace("\t", "*").Split('*');
                        Console.WriteLine("{0},{1},{2},{3}", infos[1], infos[2], infos[3], infos[4]);
                    }
                    if (String.Compare(line1.Substring(20), curInfo) == 0)
                    {
                        sr.Close();
                        Console.WriteLine("###");
                        var oldLines = File.ReadAllLines(filename);
                        var newLines = from oldLine in oldLines where oldLine.Substring(20) != curInfo select oldLine;
                        File.WriteAllLines(filename, newLines);
                        break;
                    }

                }

            }
        }

        static void Main(string[] args)
        {
            //String path1 = Directory.GetCurrentDirectory();
            //String path2 = AppDomain.CurrentDomain.BaseDirectory;
            //String path3 = Environment.CurrentDirectory;
            //Console.WriteLine(path1);

            string tempStr = null;
            string str = "127.0.0.1\t" + "12345\t" + "filename\t" + "0\t" + "true" + tempStr;
            string str1 = "192.168.31.2\t" + "1331\t" + "enheng.txt\t" + "0\t" + "false" + tempStr;

            string filename = "E:\\c#\\ftp\\ftpClient\\test\\event.txt";




            //WriteLog(str);
            //WriteLog(str1);

            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line1 = sr.ReadLine();
                    if (line1!=null)
                    {
                        var infos = line1.Replace("\t", "*").Split('*');
                        Console.WriteLine(infos.Length);
                        Console.WriteLine("{0},{1},{2},{3},{4}", infos[1], infos[2], infos[3], infos[4], infos[5]);

                    }


                }

            }


        }
    }
}
