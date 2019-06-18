using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private FtpClient ftpClient;
        private object severTree;

        private void Form1_Load(object sender, EventArgs e)
        {
            TreeNode rootNode = new TreeNode("我的电脑",
            IconIndexs.MyComputer, IconIndexs.MyComputer);       //载入显示 选择显示
            rootNode.Tag = "我的电脑";                            //树节点数据
            rootNode.Text = "我的电脑";                           //树节点标签内容
            this.directoryTree.Nodes.Add(rootNode);              //树中添加根目录

           //循环遍历计算机所有逻辑驱动器名称(盘符)
            foreach (string drive in Environment.GetLogicalDrives())
            {
                var dir = new DriveInfo(drive);
                switch (dir.DriveType)           //判断驱动器类型
                {
                    case DriveType.Fixed:        //仅取固定磁盘盘符 Removable-U盘 
                        {
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
            textBox5.Text = e.Node.Name;
            if (Directory.Exists(e.Node.Name))
                TreeViewItems.Add(e.Node);
        }

        private static bool IsIP(string ip)
        {
            //判断是否为IP
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        private bool isLinked = false;
        private void Button1_Click(object sender, EventArgs e)
        {
            if (!isLinked)
            {
                try
                {
                    bool isip = IsIP(textBox1.Text);
                    int serverPort = int.Parse(textBox4.Text);
                    if (!isip || serverPort < 0 || serverPort > 65535)
                    {
                        MessageBox.Show("IP地址或端口错误!", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return;
                    }
                    String serverIp = textBox1.Text;
                    String user = textBox2.Text;
                    String pass = textBox3.Text;

                    try
                    {
                        ftpClient = new FtpClient(serverIp, serverPort, user, pass);
                        //ftpClient = new FtpClient();
                        ftpClient.login();
                    }
                    catch (MyException exce)
                    {
                        MessageBox.Show(exce.Message, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        return;
                    }
                    MessageBox.Show("登录成功!","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    isLinked = true;

                    List<string> fileList = ftpClient.getNameList("/", '-');
                    List<string> directoryList = ftpClient.getNameList("/", 'd');


                    //实例化TreeNode类 TreeNode(string text,int imageIndex,int selectImageIndex)            
                    TreeNode serverRootNode = new TreeNode("/", IconIndexs.Server, IconIndexs.Server);  //载入显示 选择显示
                    serverRootNode.Name = "/";
                    serverRootNode.Tag = "服务器";                            //树节点数据
                    serverRootNode.Text = "服务器";                           //树节点标签内容
                    this.serverTree.Nodes.Add(serverRootNode);
                }
                catch (System.FormatException exception)
                {
                    MessageBox.Show(exception.Message, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void serverTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox6.Text = e.Node.Name;
            if (e.Node.Name[e.Node.Name.Length - 1] == '/')
                TreeViewItems.serverAdd(e.Node,ftpClient);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLinked)
                {
                    MessageBox.Show("当前与服务器未建立连接！", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    string fname = textBox5.Text;
                    string path = textBox6.Text;
                    fname = Regex.Replace(fname, @"\\", @"\\");
                    int id = fname.Length - 1;
                    while (fname[id] != '\\')
                        id--;
                    String fileName = fname.Substring(id + 1, fname.Length - id - 1);

                    ftpClient.uploadFile(fileName, fname, path);
                    MessageBox.Show("上传完成！", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                return;
            }
            catch (System.NullReferenceException)
            {
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isLinked)
                {
                    MessageBox.Show("当前与服务器未建立连接！", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    string path = textBox5.Text;
                    string fname = textBox6.Text;
                    //Console.WriteLine(path);
                    path = Regex.Replace(path, @"\\", @"\\");
                    //Console.WriteLine(path);
                    int id = fname.Length - 1;
                    while (fname[id] != '/')
                        id--;
                    String fileName = fname.Substring(id + 1, fname.Length - id - 1);
                    path = path + "\\\\" + fileName;
                    ftpClient.downloadFile(fname, path);
                    MessageBox.Show("下载完成！", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
            }
            catch (System.IndexOutOfRangeException exception1)
            {
                return;
            }
        }
       
    }
}
