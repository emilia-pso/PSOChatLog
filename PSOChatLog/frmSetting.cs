//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//PSO Chat Log Write By sakura and emilia@tanuki
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//
//Form2.cs  Write By emilia@tanuki
//
//since 2021/06/06
//
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Value;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Web.Util;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Web.UI;
using Microsoft.VisualBasic.FileIO;

namespace PSOChatLog
{
    public partial class frmSetting : Form
    {
        public frmSetting()
        {
            InitializeComponent();
            Label.initializeForm2(this);
        }
        private void frmSetting_Load(object sender, EventArgs e)
        {
            gfTabControlSetting();
            gfComboBoxFont();
            ShortTextRegistrationLoad();
            gfIniFileLoad();
            gfRadioButtonEnableCheck();
        }
        private void frmSetting_DragDrop(object sender, DragEventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            var myCheck = false;
            // ドロップされたファイルを順に確認する
            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                // ファイルの拡張子を取得
                string extension = Path.GetExtension(file);
                // ファイルへのショートカットは拡張子".lnk"
                if (".lnk" == extension)
                {
                    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                    // ショートカットオブジェクトの取得
                    IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(file);

                    // ショートカットのリンク先の取得
                    string targetPath = shortcut.TargetPath.ToString();
                    //myCheck = Regex.IsMatch(targetPath, "{.*online\\.exe}", RegexOptions.Singleline);
                    myCheck = Regex.IsMatch(targetPath, ".*online\\.exe", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        textBox1.Text = targetPath;
                        //MessageBox.Show(targetPath);
                    }
                }
                // ファイル"online.exe"だった場合
                if (Path.GetFileName(file).Equals("online.exe"))
                {
                    // ショートカットのリンク先の取得
                    string targetPath = Path.GetDirectoryName(file) + "\\" + Path.GetFileName(file);
                    //myCheck = Regex.IsMatch(targetPath, "{.*online\\.exe}", RegexOptions.Singleline);
                    myCheck = Regex.IsMatch(targetPath, ".*online\\.exe", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        textBox1.Text = targetPath;
                        //MessageBox.Show(targetPath);
                    }
                }
            }

        }
        private void frmSetting_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            SetTopMost();
            base.OnLoad(e);
        }
        // 常に最前面に表示
        private void SetTopMost()
        {
            if (getFloatSetting())
            {
                IntPtr HWND_TOPMOST = new IntPtr(-1);
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, 0x0010 | 0x0002 | 0x0400 | 0x0001 | 0x0040);
            }
            else
            {
                IntPtr HWND_NPTOPMOST = new IntPtr(-2);
                SetWindowPos(this.Handle, HWND_NPTOPMOST, 0, 0, 0, 0, 0x0010 | 0x0002 | 0x0400 | 0x0001 | 0x0040);
            }
        }

        // Form#Show()でアクティブにしない
        protected override bool ShowWithoutActivation
        {
            get
            {
                return getFloatSetting();
            }
        }
        private bool getFloatSetting()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strFloating = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Floating", strIniFileName);
            var bRet = false;
            do
            {
                if (strFloating.Equals("true") != false)
                {
                    bRet = true;
                }
                if (strFloating.Equals("false") != false)
                {
                    bRet = false;
                }
            }
            while (false);
            return bRet;
        }

        private void gfTabControlSetting()
        {
            var myCheck = Regex.IsMatch(Value.strEnvironment, ".*TexTra.*", RegexOptions.Singleline);
            do
            {
                if (myCheck == true)
                {
                    //TexTra
                    this.tabControl1.TabPages.Remove(this.tabPage5);
                    this.Text = this.Text + " TexTra® Powered by NICT";
                    this.NICT.Visible = true;
                    break;
                }
                if (myCheck == false)
                {
                    //従来Exe
                    this.tabControl1.TabPages.Remove(this.tabPage6);
                    this.NICT.Visible = false;
                    break;
                }
            } while (false);
        }
        private void gfIniFileLoad()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strUseIMEZhToJa = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "UseIMEZhToJa", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);
            var strBaiduID = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu ID", strIniFileName);
            var strBaiduPASS = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu PASS", strIniFileName);
            var strDeepLUsePython = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepLUsePython", strIniFileName);
            var strBackColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorRed", strIniFileName);
            var strBackColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorGreen", strIniFileName);
            var strBackColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorBlue", strIniFileName);
            var strForeColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorRed", strIniFileName);
            var strForeColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorGreen", strIniFileName);
            var strForeColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorBlue", strIniFileName);
            var strFontName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontName", strIniFileName);
            var strFontSize = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontSize", strIniFileName);
            var strGuildChatSoundCheck = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundCheck", strIniFileName);
            var strGuildChatSoundFile = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundFile", strIniFileName);
            var strSendTextMode = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SendTextMode", strIniFileName);
            var strWaitSetKeyDelayDelay = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSetKeyDelayDelay", strIniFileName);
            var strWaitSetKeyDelayPressDuration = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSetKeyDelayPressDuration", strIniFileName);
            var strWaitSpacekeyChatDelay = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSpacekeyChatDelay", strIniFileName);
            var strWaitInputModeChangeWait = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitInputModeChangeWait", strIniFileName);
            var strWaitCharToCharTransmissionWaitBoth = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitCharToCharTransmissionWaitBoth", strIniFileName);
            var strWaitCharToCharTransmissionWaitChatLogTool = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitCharToCharTransmissionWaitChatLogTool", strIniFileName);
            var strLangJudge = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "LangJudge", strIniFileName);
            var strCyrillicConvert = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "CyrillicConvert", strIniFileName);
            var strFloating = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Floating", strIniFileName);
            var strSaveScreenPos = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveScreenPos", strIniFileName);
            var strScreenPosStX = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosStX", strIniFileName);
            var strScreenPosStY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosStY", strIniFileName);
            var strScreenPosEdX = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosEdX", strIniFileName);
            var strScreenPosEdY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosEdY", strIniFileName);
            var strSaveChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChat", strIniFileName);
            var strSaveChatRow = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChatRow", strIniFileName);
            var strCheckMsgOmitGoToLobby = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "CheckMsgOmitGoToLobby", strIniFileName);

            var strTexTraAPIName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIName", strIniFileName);
            var strTexTraAPIKEY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIKEY", strIniFileName);
            var strTexTraAPISecret = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPISecret", strIniFileName);

            var strTranslationOperationMode = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TranslationOperationMode", strIniFileName);

            var strViewControl = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ViewControl", strIniFileName);

            /*
                        { "checkBox4", "終了時に画面の位置を保存する。" },
                        { "checkBox5", "終了時にチャットを保存する" },
                        { "label25", "保存するチャットの行数(Max9999行)" },
            */

            textBox1.Text = strInstallPath;
            if (strSpaceChat.Equals("true"))
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
            if (strUseIMEZhToJa.Equals("true"))
            {
                checkBox7.Checked = true;
            }
            else
            {
                checkBox7.Checked = false;
            }
            if (strGuildChatSoundCheck.Equals("true"))
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
            do
            {
                if (strTranslation.Equals("DeepL API Free") && strDeepLAPIFreeKey != "")
                {
                    radioButton2.Checked = true;
                    break;
                }
                if (strTranslation.Equals("DeepL API Pro") && strDeepLAPIProKey != "")
                {
                    radioButton3.Checked = true;
                    break;
                }
                if (strTranslation.Equals("Google Apps Scripts") && strGoogleAppsScriptsURL != "")
                {
                    radioButton4.Checked = true;
                    break;
                }
                if (strTranslation.Equals("DeepL API and Google Apps Scripts") && strDeepLAPIFreeKey != "" && strGoogleAppsScriptsURL != ""
                    || strTranslation.Equals("DeepL API and Google Apps Scripts") && strDeepLAPIProKey != "" && strGoogleAppsScriptsURL != "")
                {
                    radioButton5.Checked = true;
                    break;
                }
                if (strTranslation.Equals("Baidu Trans Web API") && strBaiduID != "" && strBaiduPASS != "")
                {
                    radioButton9.Checked = true;
                    break;
                }
                if (strTranslation.Equals("TexTra API") && strTexTraAPIName != "" && strTexTraAPIKEY != "" && strTexTraAPISecret != "")
                {
                    radioButton15.Checked = true;
                    break;
                }
                radioButton1.Checked = true;
                radioButton14.Checked = true;
                break;
            }while (false);
            do
            {
                if (strDeepLUsePython.Equals("true"))
                {
                    checkBox8.Checked = true;
                    break;
                }
                checkBox8.Checked = false;
                break;
            }while (false);
            do
            {
                if (strSendTextMode.Equals("MemorySharp"))
                {
                    radioButton13.Checked = true;
                    break;
                }
                if (strSendTextMode.Equals("AHK"))
                {
                    radioButton6.Checked = true;
                    break;
                }
                if (strSendTextMode.Equals("Both"))
                {
                    radioButton7.Checked = true;
                    break;
                }
                if (strSendTextMode.Equals("ChatLogTool"))
                {
                    radioButton8.Checked = true;
                    break;
                }
                if (strSendTextMode.Equals("PowerShell"))
                {
                    radioButton12.Checked = true;
                    break;
                }
                radioButton13.Checked = true;
                break;
            }while (false);
            do
            {
                if (strTranslationOperationMode.Equals("Async"))
                {
                    radioButton16.Checked = true;
                    break;
                }
                if (strTranslationOperationMode.Equals("Sync"))
                {
                    radioButton17.Checked = true;
                    break;
                }
                radioButton16.Checked = true;
                break;
            } while (false);
            do
            {
                if (strViewControl.Equals("ListView"))
                {
                    radioButton18.Checked = true;
                    break;
                }
                if (strViewControl.Equals("ListBox"))
                {
                    radioButton19.Checked = true;
                    break;
                }
                radioButton18.Checked = true;
                break;
            } while (false);
            do
            {
                if (strLangJudge.Equals("LDJAVA2014"))
                {
                    radioButton10.Checked = true;
                    break;
                }
                if (strLangJudge.Equals("LDRECJKChk"))
                {
                    radioButton11.Checked = true;
                    break;
                }
                radioButton11.Checked = true;
            }
            while (false);
            do
            {
                if (strCyrillicConvert.Equals("true"))
                {
                    checkBox6.Checked = true;
                    break;
                }
                checkBox6.Checked = false;
                break;
            }
            while (false);
            do
            {
                if (strFloating.Equals("true"))
                {
                    checkBox3.Checked = true;
                    break;
                }
                checkBox3.Checked = false;
                break;
            }
            while (false);
            do
            {
                if (strSaveScreenPos.Equals("true"))
                {
                    checkBox4.Checked = true;
                    break;
                }
                checkBox4.Checked = false;
                break;
            }
            while (false);
            do
            {
                if (strSaveChat.Equals("true"))
                {
                    checkBox5.Checked = true;
                    break;
                }
                checkBox5.Checked = false;
                break;
            }
            while (false);
            if (strSaveChatRow.Equals(""))
            {
                textBox15.Text = "0";
            }
            else
            {
                textBox15.Text = strSaveChatRow;
            }

            do
            {
                if (strCheckMsgOmitGoToLobby.Equals("true"))
                {
                    checkBox9.Checked = true;
                    break;
                }
                checkBox9.Checked = false;
                break;
            }
            while (false);

            textBox2.Text = strDeepLAPIFreeKey;
            textBox3.Text = strDeepLAPIProKey;
            textBox4.Text = strGoogleAppsScriptsURL;
            textBox13.Text = strBaiduID;
            textBox14.Text = strBaiduPASS;
            textBox22.Text = strTexTraAPIName;
            textBox23.Text = strTexTraAPIKEY;
            textBox24.Text = strTexTraAPISecret;
            if (strFontName.Equals(""))
            {
                comboBox1.Text = "MS UI Gothic";
            }
            else
            {
                comboBox1.Text = strFontName;
            }
            if (strFontSize.Equals(""))
            {
                comboBox2.Text = "9";
            }
            else
            {
                comboBox2.Text = strFontSize;
            }

            //Console.WriteLine($"KEY1={stringValue} KEY2={intValue}");
            if (strGuildChatSoundFile.Equals(""))
            {
                textBox5.Text = "guild.wav";
            }
            else
            {
                textBox5.Text = strGuildChatSoundFile;
            }

            if (strWaitSetKeyDelayDelay.Equals(""))
            {
                textBox7.Text = "100";
            }
            else
            {
                textBox7.Text = strWaitSetKeyDelayDelay;
            }

            if (strWaitSetKeyDelayPressDuration.Equals(""))
            {
                textBox8.Text = "100";
            }
            else
            {
                textBox8.Text = strWaitSetKeyDelayPressDuration;
            }

            if (strWaitSpacekeyChatDelay.Equals(""))
            {
                textBox9.Text = "1000";
            }
            else
            {
                textBox9.Text = strWaitSpacekeyChatDelay;
            }

            if (strWaitInputModeChangeWait.Equals(""))
            {
                textBox10.Text = "1000";
            }
            else
            {
                textBox10.Text = strWaitInputModeChangeWait;
            }

            if (strWaitCharToCharTransmissionWaitBoth.Equals(""))
            {
                textBox11.Text = "10";
            }
            else
            {
                textBox11.Text = strWaitCharToCharTransmissionWaitBoth;
            }

            if (strWaitCharToCharTransmissionWaitChatLogTool.Equals(""))
            {
                textBox12.Text = "10";
            }
            else
            {
                textBox12.Text = strWaitCharToCharTransmissionWaitChatLogTool;
            }

            if (strForeColorRed.Equals(""))
            {
                textBox19.Text = "0";
            }
            else
            {
                textBox19.Text = strForeColorRed;
            }

            if (strForeColorGreen.Equals(""))
            {
                textBox20.Text = "0";
            }
            else
            {
                textBox20.Text = strForeColorGreen;
            }

            if (strForeColorBlue.Equals(""))
            {
                textBox21.Text = "0";
            }
            else
            {
                textBox21.Text = strForeColorBlue;
            }

            if (strBackColorRed.Equals(""))
            {
                textBox16.Text = "255";
            }
            else
            {
                textBox16.Text = strBackColorRed;
            }

            if (strBackColorGreen.Equals(""))
            {
                textBox17.Text = "255";
            }
            else
            {
                textBox17.Text = strBackColorGreen;
            }

            if (strBackColorBlue.Equals(""))
            {
                textBox18.Text = "255";
            }
            else
            {
                textBox18.Text = strBackColorBlue;
            }
        }
        private void gfIniFileSave()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var strSpaceChat = "";
            var strUseIMEZhToJa = "";
            var strTranslation = "";
            var strInstallPath = "";
            var strDeepLAPIFreeKey = "";
            var strDeepLAPIProKey = "";
            var strGoogleAppsScriptsURL = "";
            var strFontName = "";
            var strFontSize = "";
            var strGuildChatSoundCheck = "";
            var strGuildChatSoundFile = "";
            var strSendTextMode = "";
            var strWaitSetKeyDelayDelay = "";
            var strWaitSetKeyDelayPressDuration = "";
            var strWaitSpacekeyChatDelay = "";
            var strWaitInputModeChangeWait = "";
            var strWaitCharToCharTransmissionWaitBoth = "";
            var strWaitCharToCharTransmissionWaitChatLogTool = "";
            var strForeColorRed = "";
            var strForeColorGreen = "";
            var strForeColorBlue = "";
            var strBackColorRed = "";
            var strBackColorGreen = "";
            var strBackColorBlue = "";
            var strCyrillicConvert = "";
            var numVal = 0;
            //var strTmp = "";
            var strBaiduID = "";
            var strBaiduPASS = "";
            var strLangJudge = "";
            var strFloating = "";
            var myCheck = false;
            var strSaveScreenPos = "";
            //var strScreenPosStX = "";
            //var strScreenPosStY = "";
            //var strScreenPosEdX = "";
            //var strScreenPosEdY = "";
            var strSaveChat = "";
            var strSaveChatRow = "";
            var strCheckMsgOmitGoToLobby = "";
            var strDeepLUsePython = "";

            var strTexTraAPIName = "";
            var strTexTraAPIKEY = "";
            var strTexTraAPISecret = "";

            var strTranslationOperationMode = "";
            var strViewControl = "";
            var strLangTarget = Language_GP.myLanguage_GP();

            if (checkBox1.Checked == true)
            {
                strSpaceChat = "true";
            }
            else
            {
                strSpaceChat = "false";
            }
            if (checkBox7.Checked == true)
            {
                strUseIMEZhToJa = "true";
            }
            else
            {
                strUseIMEZhToJa = "false";
            }
            if (checkBox2.Checked == true)
            {
                strGuildChatSoundCheck = "true";
            }
            else
            {
                strGuildChatSoundCheck = "false";
            }
            do
            {
                if (radioButton2.Checked == true && radioButton2.Enabled == true)
                {
                    strTranslation = "DeepL API Free";
                    break;
                }
                if (radioButton3.Checked == true && radioButton3.Enabled == true)
                {
                    strTranslation = "DeepL API Pro";
                    break;
                }
                if (radioButton4.Checked == true && radioButton4.Enabled == true)
                {
                    strTranslation = "Google Apps Scripts";
                    break;
                }
                if (radioButton5.Checked == true && radioButton5.Enabled == true)
                {
                    strTranslation = "DeepL API and Google Apps Scripts";
                    break;
                }
                if (radioButton9.Checked == true && radioButton9.Enabled == true)
                {
                    strTranslation = "Baidu Trans Web API";
                    break;
                }
                if (radioButton15.Checked == true && radioButton15.Enabled == true)
                {
                    strTranslation = "TexTra API";
                    break;
                }
                strTranslation = "none";
                break;
            }
            while (false);
            do
            {
                if (checkBox8.Checked == true)
                {
                    strDeepLUsePython = "true";
                    break;
                }
                strDeepLUsePython = "false";
                break;
            }
            while (false);
            do
            {
                if (radioButton13.Checked == true)
                {
                    strSendTextMode = "MemorySharp";
                    break;
                }
                if (radioButton6.Checked == true)
                {
                    strSendTextMode = "AHK";
                    break;
                }
                if (radioButton7.Checked == true)
                {
                    strSendTextMode = "Both";
                    break;
                }
                if (radioButton8.Checked == true)
                {
                    strSendTextMode = "ChatLogTool";
                    break;
                }
                if (radioButton12.Checked == true)
                {
                    strSendTextMode = "PowerShell";
                    break;
                }
                strSendTextMode = "MemorySharp";
            }
            while (false);

            do
            {
                if (radioButton10.Checked == true)
                {
                    strLangJudge = "LDJAVA2014";
                    break;
                }
                if (radioButton11.Checked == true)
                {
                    strLangJudge = "LDRECJKChk";
                    break;
                }
            }
            while (false);
            do
            {
                if (radioButton16.Checked == true)
                {
                    strTranslationOperationMode = "Async";
                    break;
                }
                if (radioButton17.Checked == true)
                {
                    strTranslationOperationMode = "Sync";
                    break;
                }
            } while (false);
            do
            {
                if (radioButton18.Checked == true)
                {
                    strViewControl = "ListView";
                    break;
                }
                if (radioButton19.Checked == true)
                {
                    strViewControl = "ListBox";
                    break;
                }
            } while (false);
            do
            {
                if (checkBox6.Checked == true)
                {
                    strCyrillicConvert = "true";
                    break;
                }
                strCyrillicConvert = "false";
                break;
            }
            while (false);

            do
            {
                if (checkBox3.Checked == true)
                {
                    strFloating = "true";
                    break;
                }
                strFloating = "false";
                break;
            }
            while (false);
            do
            {
                if (checkBox4.Checked == true)
                {
                    strSaveScreenPos = "true";
                    break;
                }
                strSaveScreenPos = "false";
                break;
            }
            while (false);
            do
            {
                if (checkBox5.Checked == true)
                {
                    strSaveChat = "true";
                    break;
                }
                strSaveChat = "false";
                break;
            }
            while (false);
            try
            {
                numVal = Int32.Parse(textBox15.Text);
            }
            catch
            {
                numVal = 1000;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strSaveChatRow = numVal.ToString();
            }
            else
            {
                strSaveChatRow = "9999";
            }

            do
            {
                if (checkBox9.Checked == true)
                {
                    strCheckMsgOmitGoToLobby = "true";
                    break;
                }
                strCheckMsgOmitGoToLobby = "false";
                break;
            }
            while (false);


            try
            {
                numVal = Int32.Parse(textBox7.Text);
            }
            catch
            {
                numVal = 100;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitSetKeyDelayDelay = numVal.ToString();
            }
            else
            {
                strWaitSetKeyDelayDelay = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox8.Text);
            }
            catch
            {
                numVal = 100;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitSetKeyDelayPressDuration = numVal.ToString();
            }
            else
            {
                strWaitSetKeyDelayPressDuration = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox9.Text);
            }
            catch
            {
                numVal = 1000;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitSpacekeyChatDelay = numVal.ToString();
            }
            else
            {
                strWaitSpacekeyChatDelay = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox10.Text);
            }
            catch
            {
                numVal = 1000;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitInputModeChangeWait = numVal.ToString();
            }
            else
            {
                strWaitInputModeChangeWait = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox11.Text);
            }
            catch
            {
                numVal = 10;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitCharToCharTransmissionWaitBoth = numVal.ToString();
            }
            else
            {
                strWaitCharToCharTransmissionWaitBoth = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox12.Text);
            }
            catch
            {
                numVal = 10;
            }
            if (0 <= numVal && numVal <= 9999)
            {
                strWaitCharToCharTransmissionWaitChatLogTool = numVal.ToString();
            }
            else
            {
                strWaitCharToCharTransmissionWaitChatLogTool = "9999";
            }

            try
            {
                numVal = Int32.Parse(textBox19.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strForeColorRed = numVal.ToString();
            }
            else
            {
                strForeColorRed = "255";
            }

            try
            {
                numVal = Int32.Parse(textBox20.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strForeColorGreen = numVal.ToString();
            }
            else
            {
                strForeColorGreen = "255";
            }

            try
            {
                numVal = Int32.Parse(textBox21.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strForeColorBlue = numVal.ToString();
            }
            else
            {
                strForeColorBlue = "255";
            }

            try
            {
                numVal = Int32.Parse(textBox16.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strBackColorRed = numVal.ToString();
            }
            else
            {
                strBackColorRed = "255";
            }

            try
            {
                numVal = Int32.Parse(textBox17.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strBackColorGreen = numVal.ToString();
            }
            else
            {
                strBackColorGreen = "255";
            }

            try
            {
                numVal = Int32.Parse(textBox18.Text);
            }
            catch
            {
                numVal = 0;
            }
            if (0 <= numVal && numVal <= 255)
            {
                strBackColorBlue = numVal.ToString();
            }
            else
            {
                strBackColorBlue = "255";
            }

            do
            {
                strInstallPath = textBox1.Text;
                strDeepLAPIFreeKey = textBox2.Text;
                strDeepLAPIProKey = textBox3.Text;
                strBaiduID = textBox13.Text;
                strBaiduPASS = textBox14.Text;
                //https://script.google.com/macros/s/
                myCheck = Regex.IsMatch(textBox4.Text, "https://script.google.com/macros/s/.*/exec", RegexOptions.Singleline);
                if (myCheck == true || textBox4.Text.Equals(""))
                {
                    strGoogleAppsScriptsURL = textBox4.Text;
                }
                else
                {
                    if (strLangTarget.Equals("JA") != false)
                    {
                        MessageBox.Show("Google Apps Scripts のURLが不正です。" + "\n設定できませんでした。", "設定");
                    }
                    else
                    {
                        MessageBox.Show("The URL for Google Apps Scripts is invalid." + "\nCould not configure.", "Configuration");
                    }
                    break;
                }
                strFontName = comboBox1.Text;
                strFontSize = comboBox2.Text;
                strGuildChatSoundFile = textBox5.Text;
                strTexTraAPIName = textBox22.Text;
                strTexTraAPIKEY = textBox23.Text;
                strTexTraAPISecret= textBox24.Text;

                var clsSetIniFile = new clsSetIniFile();
                clsSetIniFile.SetValue(strEnvironment, "InstallPath", strInstallPath, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "SpaceChat", strSpaceChat, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "UseIMEZhToJa", strUseIMEZhToJa, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "Translation", strTranslation, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "DeepL API Free Key", strDeepLAPIFreeKey, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "DeepL API Pro Key", strDeepLAPIProKey, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "Google Apps Scripts URL", strGoogleAppsScriptsURL, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "Baidu ID", strBaiduID, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "Baidu PASS", strBaiduPASS, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "DeepLUsePython", strDeepLUsePython, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "ForeColorRed", strForeColorRed, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "ForeColorGreen", strForeColorGreen, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "ForeColorBlue", strForeColorBlue, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "BackColorRed", strBackColorRed, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "BackColorGreen", strBackColorGreen, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "BackColorBlue", strBackColorBlue, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "FontName", strFontName, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "FontSize", strFontSize, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "GuildChatSoundCheck", strGuildChatSoundCheck, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "GuildChatSoundFile", strGuildChatSoundFile, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "SendTextMode", strSendTextMode, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitSetKeyDelayDelay", strWaitSetKeyDelayDelay, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitSetKeyDelayPressDuration", strWaitSetKeyDelayPressDuration, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitSpacekeyChatDelay", strWaitSpacekeyChatDelay, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitInputModeChangeWait", strWaitInputModeChangeWait, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitCharToCharTransmissionWaitBoth", strWaitCharToCharTransmissionWaitBoth, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "WaitCharToCharTransmissionWaitChatLogTool", strWaitCharToCharTransmissionWaitChatLogTool, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "LangJudge", strLangJudge, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "CyrillicConvert", strCyrillicConvert, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "Floating", strFloating, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "SaveScreenPos", strSaveScreenPos, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "SaveChat", strSaveChat, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "SaveChatRow", strSaveChatRow, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "CheckMsgOmitGoToLobby", strCheckMsgOmitGoToLobby, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "TexTraAPIName", strTexTraAPIName, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "TexTraAPIKEY", strTexTraAPIKEY, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "TexTraAPISecret", strTexTraAPISecret, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "TranslationOperationMode", strTranslationOperationMode, strIniFileName);
                clsSetIniFile.SetValue(strEnvironment, "ViewControl", strViewControl, strIniFileName);
                this.Close();
            }
            while (false);
        }
        private void ShortTextRegistrationLoad()
        {
            var strFileName = System.Environment.CurrentDirectory + "\\ShortTextRegistration.csv";

            //this.ClientSize = new Size(650, 250); // クライアント領域のサイズ

            dataGridView1.AllowUserToAddRows = false;

            DataGridViewTextBoxColumn textColumn0 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn textColumn1 = new DataGridViewTextBoxColumn();

            stop();
            do
            {
                var strFirstLanguage_GP = Language_GP.myLanguage_GP();
                if (strFirstLanguage_GP.Equals("JA"))
                {
                    //名前とヘッダーを設定する
                    textColumn0.Name = "No";
                    textColumn0.HeaderText = "No";
                    textColumn0.Width = 40;
                    //列を追加する
                    dataGridView1.Columns.Add(textColumn0);
                    //名前とヘッダーを設定する
                    textColumn1.Name = "ShortTextRegistration";
                    textColumn1.HeaderText = "短文登録";
                    textColumn1.Width = 673;
                    //列を追加する
                    dataGridView1.Columns.Add(textColumn1);
                }
                else
                {
                    //名前とヘッダーを設定する
                    textColumn0.Name = "No";
                    textColumn0.HeaderText = "No";
                    textColumn0.Width = 40;
                    //列を追加する
                    dataGridView1.Columns.Add(textColumn0);
                    //名前とヘッダーを設定する
                    textColumn1.Name = "ShortTextRegistration";
                    textColumn1.HeaderText = "ShortTextRegistration";
                    textColumn1.Width = 673;
                    //列を追加する
                    dataGridView1.Columns.Add(textColumn1);
                }

                stop();

                if (File.Exists(strFileName) == false)
                {
                    break;
                }
                FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                // テキストエンコーディングにUTF-8を用いてstreamの読み込みを行うStreamReaderを作成する
                var sr = new StreamReader(fs);
                //streamから文字列を読み込み
                string strBuf = sr.ReadToEnd();
                strBuf = strBuf.Replace("\r\n\r\n", "\r\n");
                strBuf = strBuf.TrimEnd('\r', '\n');
                var strLine = Regex.Split(strBuf, "\r\n");

                for (int n = 0; n < strLine.Length; n++)
                {
                    if (strLine[n] == "")
                    {
                        continue;
                    }
                    var strCol = Regex.Split(strLine[n], ",");
                    var values = new string[]
                    {
                    strCol[0], strCol[1]
                    };
                    //strLogData[n] = strLogData[n].Replace(",", "\",\"");
                    //dataGridView1.Rows.Add("\"" + strLogData[n] + "\"");
                    dataGridView1.Rows.Add(values);
                }
                //listView1.View = System.Windows.Forms.View.Details;
                //var strFirstLanguage_GP = Language_GP.myLanguage_GP();
                //if (strFirstLanguage_GP.Equals("JA"))
                //{
                //    listView1.Columns.Add("No", 20, HorizontalAlignment.Left);
                //    listView1.Columns.Add("短文登録", 480, HorizontalAlignment.Left);
                //}
                //else
                //{
                //    listView1.Columns.Add("No", 20, HorizontalAlignment.Left);
                //    listView1.Columns.Add("ShortTextRegistration", 480, HorizontalAlignment.Left);
                //}
                //var strFileName = System.Environment.CurrentDirectory + "\\ShortTextRegistration.csv";
                //do
                //{
                //    if (File.Exists(strFileName) == false)
                //    {
                //        break;
                //    }
                //    //<listview>
                //    listView1.Items.Clear();
                //    string[] data = System.IO.File.ReadAllText(strFileName).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (string line in data)
                //    {
                //        //string[] items = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //        string[] items = line.Split(new string[] { "," }, StringSplitOptions.None);
                //        listView1.Items.Add(new ListViewItem(items));
                //    }
                //    //listView1.ListItems(listView1.ListItems.Count).EnsureVisible;
                //    if (listView1.Items.Count == 0)
                //    {
                //        break;
                //    }
                //    //listView1.Items[listView1.Items.Count - 1].EnsureVisible();
                //} while (false);
            } while (false);
        }
        private void ShortTextRegistrationSave()
        {
            var strFileName = System.Environment.CurrentDirectory + "\\ShortTextRegistration.csv";

            string saveData = "";
            int iRowStart = 0;
            string strLine = "";

            stop();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                strLine = "";
                do
                {
                    for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                    {
                        strLine += dataGridView1.Rows[i].Cells[j].Value + ",";
                    }
                    strLine = strLine.Substring(0, strLine.Length - 1);
                    saveData += strLine;
                    saveData += Environment.NewLine;
                } while (false);
            }
            Encoding encoding = Encoding.UTF8;
            System.IO.File.WriteAllText(strFileName, saveData, encoding);

        }
        private void gfRadioButtonEnableCheck()
        {
            //翻訳をしない
            radioButton1.Enabled = true;

            if (textBox2.Text != "")
            {
                radioButton2.Enabled = true;
            }
            else
            {
                radioButton2.Enabled = false;
            }
            if (textBox3.Text != "")
            {
                radioButton3.Enabled = true;
            }
            else
            {
                radioButton3.Enabled = false;
            }
            if (textBox4.Text != "")
            {
                radioButton4.Enabled = true;
            }
            else
            {
                radioButton4.Enabled = false;
            }
            if (textBox2.Text != "" && textBox4.Text != "" || textBox3.Text != "" && textBox4.Text != "")
            {
                radioButton5.Enabled = true;
            }
            else
            {
                radioButton5.Enabled = false;
            }
            if (textBox13.Text != "" && textBox14.Text != "")
            {
                radioButton9.Enabled = true;
            }
            else
            {
                radioButton9.Enabled = false;
            }
            radioButton14.Enabled = true;
            if (textBox22.Text != "" && textBox23.Text != "" && textBox24.Text != "")
            {
                radioButton15.Enabled = true;
            }
            else
            {
                radioButton15.Enabled = false;
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            var strLangTarget = Language_GP.myLanguage_GP();
            // 操作するレジストリ・キーの名前
            //            string rKeyName = @"HKEY_CURRENT_USER\SOFTWARE\EphineaPSO";
            string rKeyName = @"SOFTWARE\EphineaPSO";
            // 取得処理を行う対象となるレジストリの値の名前
            string rGetValueName = "Install_Dir";

            // レジストリの取得
            try
            {
                // レジストリ・キーのパスを指定してレジストリを開く
                RegistryKey rKey = Registry.CurrentUser;
                rKey = rKey.OpenSubKey(@"SOFTWARE\EphineaPSO");

                // レジストリの値を取得
                string strLocation = (string)rKey.GetValue(rGetValueName);

                // 開いたレジストリを閉じる
                rKey.Close();

                // コンソールに取得したレジストリの値を表示
                textBox1.Text = strLocation + "\\online.exe";
            }
            catch (NullReferenceException)
            {
                // レジストリ・キーまたは値が存在しない
                if (strLangTarget.Equals("JA") != false)
                {
                    MessageBox.Show("レジストリ［" + rKeyName + "］の［" + rGetValueName + "］がありません！");
                }
                else
                {
                    MessageBox.Show("Registry key［" + rGetValueName + "］of［" + rKeyName + "］not found");
                }
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            var strStartMenu = "";
            var strDesktop = "";
            var strPrograms = "";
            var strEphineaPSOBB = "";
            var strLinkPath = "";
            var myCheck = false;

            do
            {
                strStartMenu = System.Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                strPrograms = "Programs";
                strEphineaPSOBB = "Ephinea PSOBB";
                strLinkPath = strStartMenu + "\\" + strPrograms + "\\" + strEphineaPSOBB + "\\" + "Launch Ephinea PSOBB.lnk";
                //C:\Users\admin\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Ephinea PSOBB
                IWshRuntimeLibrary.WshShell shell1 = new IWshRuntimeLibrary.WshShell();
                // スタートメニューのショートカットオブジェクトの取得
                IWshRuntimeLibrary.IWshShortcut shortcut1 = (IWshRuntimeLibrary.IWshShortcut)shell1.CreateShortcut(strLinkPath);

                // スタートメニューのショートカットのリンク先の取得
                string targetPath1 = shortcut1.TargetPath.ToString();
                //myCheck = Regex.IsMatch(targetPath, "{.*online\\.exe}", RegexOptions.Singleline);
                myCheck = Regex.IsMatch(targetPath1, ".*online\\.exe", RegexOptions.Singleline);
                if (myCheck == true)
                {
                    textBox1.Text = targetPath1;
                    break;
                    //MessageBox.Show(targetPath);
                }
                strDesktop = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                //strPrograms = "Programs";
                //strEphineaPSOBB = "Ephinea PSOBB";
                strLinkPath = strDesktop + "\\" + "Ephinea PSOBB.lnk";

                //C:\Users\admin\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Ephinea PSOBB
                IWshRuntimeLibrary.WshShell shell2 = new IWshRuntimeLibrary.WshShell();
                // スタートメニューのショートカットオブジェクトの取得
                IWshRuntimeLibrary.IWshShortcut shortcut2 = (IWshRuntimeLibrary.IWshShortcut)shell2.CreateShortcut(strLinkPath);

                // スタートメニューのショートカットのリンク先の取得
                string targetPath2 = shortcut2.TargetPath.ToString();
                //myCheck = Regex.IsMatch(targetPath, "{.*online\\.exe}", RegexOptions.Singleline);
                myCheck = Regex.IsMatch(targetPath2, ".*online\\.exe", RegexOptions.Singleline);
                if (myCheck == true)
                {
                    textBox1.Text = targetPath2;
                    break;
                    //MessageBox.Show(targetPath);
                }
            }
            while (false);
        }
        private void gfComboBoxFont()
        {
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.ItemHeight = 20;
            comboBox1.MaxDropDownItems = 20;
            comboBox1.IntegralHeight = false;
            foreach (FontFamily item in FontFamily.Families)
            {
                if (item.IsStyleAvailable(FontStyle.Regular))
                {
                    comboBox1.Items.Add(item.Name);
                }
            }

            comboBox2.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox2.ItemHeight = 20;
            comboBox2.MaxDropDownItems = 20;
            comboBox2.IntegralHeight = false;
            //List<string> lt = new List<string> { "6", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };
            //comboBox2.Items.AddRange(lt.ToArray());
            comboBox2.Items.Add("6");
            comboBox2.Items.Add("8");
            comboBox2.Items.Add("9");
            comboBox2.Items.Add("10");
            comboBox2.Items.Add("11");
            comboBox2.Items.Add("12");
            comboBox2.Items.Add("14");
            comboBox2.Items.Add("16");
            comboBox2.Items.Add("18");
            comboBox2.Items.Add("20");
            comboBox2.Items.Add("22");
            comboBox2.Items.Add("24");
            comboBox2.Items.Add("26");
            comboBox2.Items.Add("28");
            comboBox2.Items.Add("36");
            comboBox2.Items.Add("48");
            comboBox2.Items.Add("72");
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            ShortTextRegistrationSave();
            gfIniFileSave();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void textBox24_TextChanged(object sender, EventArgs e)
        {
            gfRadioButtonEnableCheck();
        }
        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }
            try
            {
                e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), new Font(comboBox1.Items[e.Index].ToString(), 12), new SolidBrush(Color.Black), e.Bounds.X, e.Bounds.Y);
            }
            catch
            {

            }
        }
        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }
            try
            {
                e.Graphics.DrawString(comboBox2.Items[e.Index].ToString(), new Font(comboBox2.Items[e.Index].ToString(), 12), new SolidBrush(Color.Black), e.Bounds.X, e.Bounds.Y);
            }
            catch
            {

            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "MS UI Gothic";
            comboBox2.Text = "9";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            //https://www.deepl.com/pro-api?cta=header-pro
            Process.Start("https://www.deepl.com/pro-api?cta=header-pro");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //https://script.google.com/home
            Process.Start("https://script.google.com/home");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            var strScript = "";
            strScript += "function doPost(e) {\n";
            strScript += "  var params = JSON.parse(e.postData.getDataAsString());\n";
            //            strScript += "  // リクエストパラメータを取得する\n";
            //            strScript += "  //  Language_GPAppクラスを用いて翻訳を実行\n";
            strScript += "  var translatedText = LanguageApp.translate(params.value, params.source, params.target);\n";
            //            strScript += "  // レスポンスボディの作成\n";
            strScript += "  var body;\n";
            strScript += "  if (translatedText) {\n";
            strScript += "    body = {\n";
            strScript += "      code: 200,\n";
            strScript += "      text: translatedText\n";
            strScript += "    };\n";
            strScript += "  } else {\n";
            strScript += "    body = {\n";
            strScript += "      code: 400,\n";
            strScript += "      text: \"Bad Request\"\n";
            strScript += "    };\n";
            strScript += "  }\n";
            //            strScript += "  // レスポンスの作成\n";
            strScript += "  var response = ContentService.createTextOutput();\n";
            //            strScript += "  // Mime TypeをJSONに設定\n";
            strScript += "  response.setMimeType(ContentService.MimeType.JSON);\n";
            //            strScript += "  // JSONテキストをセットする\n";
            strScript += "  response.setContent(JSON.stringify(body));\n";
            strScript += "  console.log(translatedText);\n";
            strScript += "  Logger.log(translatedText);\n";
            strScript += "\n";
            strScript += "  return response;\n";
            strScript += "}\n";
            Clipboard.SetDataObject(strScript, true);
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            var numVolume = Int32.Parse(textBox6.Text);
            if (100 >= numVolume && numVolume >= 0)
            {
                trackBar1.Value = numVolume;
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox6.Text = trackBar1.Value.ToString();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            var strWavFile = textBox5.Text;
            do
            {
                if (File.Exists(strWavFile) == false)
                {
                    break;
                }
                var player = new System.Media.SoundPlayer(strWavFile);
                //非同期再生する
                player.Play();
            }
            while (false);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            textBox5.Text = "guild.wav";
            textBox6.Text = "100";
            trackBar1.Value = 100;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox7.Text = "100";
            textBox8.Text = "100";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox9.Text = "1000";
            textBox10.Text = "1000";
            textBox11.Text = "1";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox12.Text = "1";
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            DoResize();
        }
        private void Form2_ResizeEnd(object sender, EventArgs e)
        {
            DoResize();
        }
        private void DoResize()
        {
            //800, 600
            //782, 520
            tabControl1.Top = 0;
            tabControl1.Left = 2;
            tabControl1.Width = this.Width - 18;//800時782
            tabControl1.Height = this.Height - 80;//600時520
            NICT.Top = this.Height - 69;//600時531
            //580, 526
            Button1.Left = this.Width - 220;//800-580
            Button1.Top = this.Height - 74;//600-526
        }

        private void button12_Click(object sender, EventArgs e)
        {
            do
            {
                string filePath = ".\\" + "Chat" + ".Log";
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                //manual_gas.html
                Process.Start("manual_gas.html");
            }
            while (false);
        }
        private void button14_Click(object sender, EventArgs e)
        {
            //https://mt-auto-minhon-mlt.ucri.jgn-x.jp/
            Process.Start("https://mt-auto-minhon-mlt.ucri.jgn-x.jp/");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //https://mt-auto-minhon-mlt.ucri.jgn-x.jp/content/setting/user/edit/
            Process.Start("https://mt-auto-minhon-mlt.ucri.jgn-x.jp/content/setting/user/edit/");

        }
        private void button16_Click(object sender, EventArgs e)
        {

        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
        private class clsSetIniFile
        {
            // DLL関数の定義です。
            [DllImport("KERNEL32.DLL")]
            private static extern int WritePrivateProfileString(
                string lpAppName,
                string lpKeyName,
                string lpString,
                string lpFileName
            );

            // DLL関数をラップしたメソッドです。
            public void SetValue(string section, string key, string value, string fileName)
            {
                WritePrivateProfileString(section, key, value, fileName);
            }
        }
        private class ClsCallApiGetPrivateProfile
        {
            // DLL関数の定義です。
            [DllImport("KERNEL32.DLL")]
            public static extern uint GetPrivateProfileString(
                string lpAppName,
                string lpKeyName,
                string lpDefault,
                StringBuilder lpReturnedString,
                uint nSize,
                string lpFileName);

            [DllImport("KERNEL32.DLL")]
            public static extern uint GetPrivateProfileInt(
                string lpAppName,
                string lpKeyName,
                int nDefault,
                string lpFileName);

            // DLL関数をラップしたメソッドです。
            public string CallApiGetValueString(string section, string key, string fileName)
            {
                var sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), fileName);
                return sb.ToString();
            }

            public int CallApiGetPrivateProfileInt(string section, string key, string fileName)
            {
                var sb = new StringBuilder(1024);
                return (int)GetPrivateProfileInt(section, key, 0, fileName);
            }
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {

        }
        private void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            //インデックスが0のアイテムは編集できないようにする
            if (e.Item == 0)
            {
                e.CancelEdit = true;
            }
        }
        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            System.Windows.Forms.ListView listView1 = (System.Windows.Forms.ListView)sender;
            //F2キーが離されたときは、フォーカスのあるアイテムの編集を開始
            if (e.KeyCode == Keys.F2 && listView1.FocusedItem != null && listView1.LabelEdit)
            {
                listView1.FocusedItem.BeginEdit();
            }
        }
        public void stop()
        {
            //System.Diagnostics.Debugger.Break();
        }
    }
}