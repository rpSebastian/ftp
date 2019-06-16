using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftpClient
{
    class TreeViewItems
    {
        public static void Add(TreeNode e)
        {
            //try..catch异常处理
            try
            {
                //判断"我的电脑"Tag 上面加载的该结点没指定其路径
                if (e.Tag.ToString() != "我的电脑")
                {
                    e.Nodes.Clear();                               //清除空节点再加载子节点
                    TreeNode tNode = e;                            //获取选中\展开\折叠结点
                    string path = tNode.Name;                      //路径  

                    //获取"我的文档"路径
                    if (e.Tag.ToString() == "我的文档")
                    {
                        path = Environment.GetFolderPath           //获取计算机我的文档文件夹
                            (Environment.SpecialFolder.MyDocuments);
                    }

                    //获取指定目录中的子目录名称并加载结点
                    string[] dics = Directory.GetDirectories(path);
                    foreach (string dic in dics)
                    {
                        TreeNode subNode = new TreeNode(new DirectoryInfo(dic).Name); //实例化
                        subNode.Name = new DirectoryInfo(dic).FullName;               //完整目录
                        subNode.Tag = subNode.Name;
                        subNode.ImageIndex = IconIndexs.ClosedFolder;       //获取节点显示图片
                        subNode.SelectedImageIndex = IconIndexs.OpenFolder; //选择节点显示图片
                        tNode.Nodes.Add(subNode);

                        subNode.Nodes.Add("");                               //加载空节点 实现+号
                    }

                    DirectoryInfo folder = new DirectoryInfo(path);

                    foreach (FileInfo file in folder.GetFiles())
                    {
                        TreeNode subNode = new TreeNode(file.Name); //实例化
                        subNode.Name = file.FullName;               //完整目录
                        subNode.Tag = subNode.Name;
                        subNode.ImageIndex = IconIndexs.MyDocuments;       //获取节点显示图片
                        subNode.SelectedImageIndex = IconIndexs.MyDocuments; //选择节点显示图片
                        tNode.Nodes.Add(subNode);
             
                        

                    }
                    //Console.WriteLine(path);


                }
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message);                   //异常处理
            }
        }
        
    }
}
