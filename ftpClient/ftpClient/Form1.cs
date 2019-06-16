using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftpClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void directoryTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.Expand();
        }

        private void directoryTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeViewItems.Add(e.Node);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //实例化TreeNode类 TreeNode(string text,int imageIndex,int selectImageIndex)            
            TreeNode rootNode = new TreeNode("我的电脑",
            IconIndexs.MyComputer, IconIndexs.MyComputer);  //载入显示 选择显示
            rootNode.Tag = "我的电脑";                            //树节点数据
            rootNode.Text = "我的电脑";                           //树节点标签内容
            this.directoryTree.Nodes.Add(rootNode);               //树中添加根目录

            //显示MyDocuments(我的文档)结点
            var myDocuments = Environment.GetFolderPath           //获取计算机我的文档文件夹
                (Environment.SpecialFolder.MyDocuments);
            TreeNode DocNode = new TreeNode(myDocuments);
            DocNode.Tag = "我的文档";                            //设置结点名称
            DocNode.Text = "我的文档";

            DocNode.ImageIndex = IconIndexs.MyDocuments;         //设置获取结点显示图片
            DocNode.SelectedImageIndex = IconIndexs.MyDocuments; //设置选择显示图片
            rootNode.Nodes.Add(DocNode);                          //rootNode目录下加载节点
            DocNode.Nodes.Add("");

            //循环遍历计算机所有逻辑驱动器名称(盘符)
            foreach (string drive in Environment.GetLogicalDrives())
            {
                //实例化DriveInfo对象 命名空间System.IO
                var dir = new DriveInfo(drive);
                switch (dir.DriveType)           //判断驱动器类型
                {
                    case DriveType.Fixed:        //仅取固定磁盘盘符 Removable-U盘 
                        {
                            //Split仅获取盘符字母
                            TreeNode tNode = new TreeNode(dir.Name.Split(':')[0]);
                            tNode.Name = dir.Name;
                            tNode.Tag = tNode.Name;
                            tNode.ImageIndex = IconIndexs.FixedDrive;         //获取结点显示图片
                            tNode.SelectedImageIndex = IconIndexs.FixedDrive; //选择显示图片
                            directoryTree.Nodes.Add(tNode);                    //加载驱动节点
                            tNode.Nodes.Add("");

                        }
                        break;
                }
            }
            rootNode.Expand();                  //展开树状视图

        }

        private void directoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeViewItems.Add(e.Node);
        }
    }
}
