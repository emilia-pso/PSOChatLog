using IWshRuntimeLibrary;
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
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace PSOChatLog
{
    public partial class frmSearchIniFile : Form
    {
        public frmSearchIniFile()
        {
            InitializeComponent();
        }
        private void frmSearchIniFile_Load(object sender, EventArgs e)
        {
            var myCheck = Regex.IsMatch(Value.strEnvironment, ".*TexTra.*", RegexOptions.Singleline);
            do
            {
                if (myCheck == true)
                {
                    //TexTra
                    Variable.strExeType = "TexTra";
                    break;
                }
                if (myCheck == false)
                {
                    //従来Exe
                    Variable.strExeType = "";
                    break;
                }
            } while (false);

            listBox1.Items.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\");
            listBox1.Items.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\");
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                listBox1.Items.Add(drive);
            }
        }
        private bool SerachFolderIni(string folder)
        {
            var bRet = false;
            do
            {
                if (System.IO.Directory.Exists(folder) == false)
                {
                    break;
                }
                if (System.IO.File.Exists(folder + "\\PSOChatLog" + Variable.strExeType + ".ini") == false)
                {
                    break;
                }
                bRet = true;
            } while (false);
            return bRet;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            stop();
            listBox2.Items.Clear();
            List<string> strBuf = new List<string>();
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                var strDir = "";
                strDir = listBox1.SelectedItems[i].ToString();
                if (SerachFolderIni(strDir))
                {
                    strDir = listBox1.SelectedItems[i].ToString() + "\\PSOChatLog" + Variable.strExeType + ".ini";
                    strBuf.Add(strDir);
                }
                try
                {
                    string[] subFolders = System.IO.Directory.GetDirectories(strDir, "*", System.IO.SearchOption.AllDirectories);
                    for (int j = 0; j < subFolders.Length - 1; j++)
                    {
                        strDir = subFolders[j];
                        if (SerachFolderIni(strDir))
                        {
                            strDir = subFolders[j] + "\\PSOChatLog" + Variable.strExeType + ".ini";
                            strBuf.Add(strDir);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            for (int i = 0; i < strBuf.Count; i++)
            {
                listBox2.Items.Add(strBuf[i]);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var strSrc = listBox2.Items[listBox2.SelectedIndex].ToString();
            var strDst = System.Environment.CurrentDirectory + "\\PSOChatLog" + Variable.strExeType + ".ini";
            System.IO.File.Copy(strSrc, strDst, true);

            MessageBox.Show("設定ファイルをコピーしました！");
        }
        public void stop()
        {
            //System.Diagnostics.Debugger.Break();
        }
    }
    public static class Variable
    {
        public static string strExeType = "";
    }

}
