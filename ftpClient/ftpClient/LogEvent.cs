using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftpClient
{
    class LogEvent
    {
        public void WriteLog(string strLog)
        {
            string sFilePath = "E:\\c#\\ftp\\ftpClient\\ftpClient";
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

        public void DeleteLog(string strDelLog)
        {
            string filename = "E:\\c#\\ftp\\ftpClient\\ftpClient\\event.txt";
            var oldLines = File.ReadAllLines(filename);
            var newLines = from oldLine in oldLines where oldLine.Substring(20) != strDelLog select oldLine;
            File.WriteAllLines(filename, newLines);
              
        }





    }
}
