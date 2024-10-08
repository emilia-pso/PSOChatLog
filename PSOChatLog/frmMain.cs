﻿//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//PSO Chat Log Write By sakura and emilia@tanuki
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//
//Form1.cs  Write By emilia@tanuki
//
//since 2021/06/06
//
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
using ADOX;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Assembly.CallingConvention;
using LanguageDetection;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mail;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static Value;

namespace PSOChatLog
{
    public partial class frmMain : Form
    {
        //************************************************************************************************************************************************************************************************
        //クラス宣言開始
        //************************************************************************************************************************************************************************************************
        MessageFilter messageFilter;
        Logger logger = Logger.GetInstance();
        // 構造体型用の List 作成
        List<tyDictionarySlang> stDictionarySlang = new List<tyDictionarySlang>();
        List<tyShortTextRegistration> stShortTextRegistration = new List<tyShortTextRegistration>();

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private System.IO.FileSystemWatcher watcherNormal = null;
        private System.IO.FileSystemWatcher watcherAddons1 = null;
        private System.IO.FileSystemWatcher watcherAddons2 = null;
        private System.IO.FileSystemWatcher watcherAddons3 = null;
        private System.IO.FileSystemWatcher watcherAddons4 = null;
        private LanguageDetection.LanguageDetector LDJAVA2014 = null;
        private WebClient webClientDeepLBuiltin = new WebClient();
        static readonly HttpClient httpClient = new HttpClient();
        //************************************************************************************************************************************************************************************************
        //クラス宣言終了
        //************************************************************************************************************************************************************************************************



        public frmMain()
        //************************************************************************************************************************************************************************************************
        //初期化開始
        //************************************************************************************************************************************************************************************************
        {
            InitializeComponent();
        }
        [System.STAThreadAttribute()]
        //************************************************************************************************************************************************************************************************
        //初期化終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //開始処理開始
        //************************************************************************************************************************************************************************************************
        private void frmMain_Load(object sender, EventArgs e)
        {
            messageFilter = new MessageFilter();
            System.Windows.Forms.Application.AddMessageFilter(messageFilter);

            button6.Enabled = false;
            button6.Visible = false;

            //設定を読込
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strSaveScreenPos = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveScreenPos", strIniFileName);
            var strScreenPosStX = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosStX", strIniFileName);
            var strScreenPosStY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosStY", strIniFileName);
            var strScreenPosEdX = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosEdX", strIniFileName);
            var strScreenPosEdY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ScreenPosEdY", strIniFileName);
            var strSaveChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChat", strIniFileName);
            var strSaveChatRow = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChatRow", strIniFileName);

            //設定を読めなかったらiniファイルを検索する。
            if (File.Exists(strIniFileName) == false)
            {
                frmSearchIniFile frmSearchIniFile = new frmSearchIniFile();
                frmSearchIniFile.ShowDialog();
            }

            Label.initializeForm1(this);

            // 言語設定コンボボックスの初期化です
            Combobox1SettingLanguage_GP();
            if (string.IsNullOrEmpty(comboBox2.Text) != false)
            {
                comboBox2.Text = "English";
            }

            DictionarySlangLoad();
            ShortTextRegistrationLoad();
            //TexTra対応(自分が通常版としてふるまうのか、TexTra版としてふるまうのか確認)
            var strFileNameLog = "";
            var strFileNameLogLV = "";
            var myCheck = Regex.IsMatch(Value.strEnvironment, ".*TexTra.*", RegexOptions.Singleline);
            do
            {
                if (myCheck == true)
                {
                    //TexTra
                    this.NICT.Visible = true;
                    this.Text = this.Text.Replace("PSO Chat Log", "PSO Chat Log TexTra®") + " Powered by NICT";
                    strFileNameLog = System.Environment.CurrentDirectory + "\\" + "ChatTexTra" + ".Log";
                    strFileNameLogLV = System.Environment.CurrentDirectory + "\\" + "ChatTexTra_LV" + ".Log";
                    break;
                }
                if (myCheck == false)
                {
                    //従来Exe
                    this.NICT.Visible = false;
                    strFileNameLog = System.Environment.CurrentDirectory + "\\" + "Chat" + ".Log";
                    strFileNameLogLV = System.Environment.CurrentDirectory + "\\" + "Chat_LV" + ".Log";
                    break;
                }
            } while (false);

            //HttpConnectionの初期化（HttpConnection.InitializeConnection呼び出し）
            //Proxy設定と通信タイムアウトの設定です。はじめに一回だけ呼べばOK。
            HttpConnection.InitializeConnection(30, HttpConnection.ProxyType.IE, "", 0, (0).ToString(), "");

            EnableDoubleBuffering(listBox1);
            EnableDoubleBuffering(listView1);

            LDJAVA2014 = new LanguageDetection.LanguageDetector();
            LDJAVA2014.AddLanguages(Language_GP.getLowerdCodes().ToArray());

            ListControlUpdate();
            ListBoxSetting();
            ListViewSetting();

            doVerCheckPSOChatLog();

            do
            {
                if (strSaveScreenPos.Equals("true") == false)
                {
                    break;
                }
                if (string.IsNullOrEmpty(strScreenPosStX) != false)
                {
                    break;
                }
                if (string.IsNullOrEmpty(strScreenPosStY) != false)
                {
                    break;
                }
                if (string.IsNullOrEmpty(strScreenPosEdX) != false)
                {
                    break;
                }
                if (string.IsNullOrEmpty(strScreenPosEdY) != false)
                {
                    break;
                }
                if (strScreenPosStX.Equals("-32000") != false)
                {
                    break;
                }
                if (strScreenPosStY.Equals("-32000") != false)
                {
                    break;
                }
                this.Left = Int32.Parse(strScreenPosStX);
                this.Top = Int32.Parse(strScreenPosStY);
                this.Width = Int32.Parse(strScreenPosEdX);
                this.Height = Int32.Parse(strScreenPosEdY);
            }
            while (false);
            do
            {
                if (strSaveChat.Equals("true") == false)
                {
                    break;
                }
                ListBoxTextLoad(strFileNameLog);
                ListViewTextLoad(strFileNameLogLV);
            }
            while (false);

            gfWatcherStart();
            this.timer1.Start();
        }
        //************************************************************************************************************************************************************************************************
        //開始処理終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //Watcher設定開始
        //************************************************************************************************************************************************************************************************
        private void gfWatcherStart()
        {
            try
            {
                //監視するディレクトリを指定
                var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
                var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
                var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
                //var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
                //var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
                //var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
                //var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
                //var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

                WatcherStartNormal(strInstallPath);
                WatcherStartAddonsChatlog(strInstallPath);
                WatcherStartAddonsPSOChatLogSupport(strInstallPath);
                WatcherStartAddonsSimpleMailReader(strInstallPath);
                WatcherStartAddonsSimpleMaillog(strInstallPath);

                // ロギングのサンプルだよ。
                //logger.print(strInstallPath);
                //logger.print(new string[] { strInstallPath });
                //logger.print("インストールフォルダは{0}です", strInstallPath);

            }
            catch (Exception ex)
            {
                //logger.print(ex.StackTrace);
            }
        }
        private void WatcherStartNormal(string strInstallPath)
        {
            var strLogPath = "";
            if (watcherNormal != null) return;
            watcherNormal = new System.IO.FileSystemWatcher();
            do
            {
                string filePath = strInstallPath;
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "log";
                if (Directory.Exists(strLogPath) == false)
                {
                    break;
                }
                //watcher.Path = @"C:\Users\admin\EphineaPSO\log";
                watcherNormal.Path = strLogPath;
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                /*
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                */
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                //すべてのファイルを監視
                watcherNormal.Filter = "";
                //UIのスレッドにマーシャリングする
                //コンソールアプリケーションでの使用では必要ない
                watcherNormal.SynchronizingObject = this;

                //バッファサイズを標準の4kbから、最大の64kbに増やす
                //watcherNormal.InternalBufferSize = 65536;

                //イベントハンドラの追加
                watcherNormal.Changed += new System.IO.FileSystemEventHandler(watcher_ChangedNormal);
                watcherNormal.Created += new System.IO.FileSystemEventHandler(watcher_ChangedNormal);
                //watcherNormal.Deleted += new System.IO.FileSystemEventHandler(watcher_ChangedNormal);
                //watcher.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watcherNormal.EnableRaisingEvents = true;
                //Console.WriteLine("監視を開始しました");
            }
            while (false);
        }
        private void WatcherStartAddonsChatlog(string strInstallPath)
        {
            var strLogPath = "";
            if (watcherAddons1 != null) return;
            watcherAddons1 = new System.IO.FileSystemWatcher();
            do
            {
                string filePath = strInstallPath;
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "addons\\Chatlog\\log";
                if (Directory.Exists(strLogPath) == false)
                {
                    break;
                }
                //C:\Users\admin\EphineaPSO\addons\Chatlog\log
                watcherAddons1.Path = strLogPath;
                //watcher.Path = @"C:\Users\admin\EphineaPSO\log";
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                /*
                watcherAddons1.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                */
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                //すべてのファイルを監視
                watcherAddons1.Filter = "";
                //UIのスレッドにマーシャリングする
                //コンソールアプリケーションでの使用では必要ない
                watcherAddons1.SynchronizingObject = this;

                //バッファサイズを標準の4kbから、最大の64kbに増やす
                //watcherAddons1.InternalBufferSize = 65536;

                //イベントハンドラの追加
                watcherAddons1.Changed += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                watcherAddons1.Created += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcherAddons1.Deleted += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcher.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watcherAddons1.EnableRaisingEvents = true;
                //Console.WriteLine("監視を開始しました");
            }
            while (false);
        }
        private void WatcherStartAddonsPSOChatLogSupport(string strInstallPath)
        {
            var strLogPath = "";
            if (watcherAddons2 != null) return;
            watcherAddons2 = new System.IO.FileSystemWatcher();
            do
            {
                string filePath = strInstallPath;
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "addons\\PSOChatLogSupport\\log";
                if (Directory.Exists(strLogPath) == false)
                {
                    break;
                }
                //C:\Users\admin\EphineaPSO\addons\Chatlog\log
                watcherAddons2.Path = strLogPath;
                //watcher.Path = @"C:\Users\admin\EphineaPSO\log";
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                /*
                watcherAddons2.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                */
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                //すべてのファイルを監視
                watcherAddons2.Filter = "";
                //UIのスレッドにマーシャリングする
                //コンソールアプリケーションでの使用では必要ない
                watcherAddons2.SynchronizingObject = this;

                //バッファサイズを標準の4kbから、最大の64kbに増やす
                //watcherAddons2.InternalBufferSize = 65536;

                //イベントハンドラの追加
                watcherAddons2.Changed += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                watcherAddons2.Created += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcherAddons2.Deleted += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcher.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watcherAddons2.EnableRaisingEvents = true;
                //Console.WriteLine("監視を開始しました");
            }
            while (false);
        }
        private void WatcherStartAddonsSimpleMailReader(string strInstallPath)
        {
            var strLogPath = "";
            if (watcherAddons3 != null) return;
            watcherAddons3 = new System.IO.FileSystemWatcher();
            do
            {
                string filePath = strInstallPath;
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "addons\\SimpleMailReader\\log";
                if (Directory.Exists(strLogPath) == false)
                {
                    break;
                }
                //C:\Users\admin\EphineaPSO\addons\Chatlog\log
                watcherAddons3.Path = strLogPath;
                //watcher.Path = @"C:\Users\admin\EphineaPSO\log";
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                /*
                watcherAddons3.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                */
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                //すべてのファイルを監視
                watcherAddons3.Filter = "";
                //UIのスレッドにマーシャリングする
                //コンソールアプリケーションでの使用では必要ない
                watcherAddons3.SynchronizingObject = this;

                //バッファサイズを標準の4kbから、最大の64kbに増やす
                //watcherAddons3.InternalBufferSize = 65536;

                //イベントハンドラの追加
                watcherAddons3.Changed += new System.IO.FileSystemEventHandler(watcher_ChangedSimpleMail);
                watcherAddons3.Created += new System.IO.FileSystemEventHandler(watcher_ChangedSimpleMail);
                //watcherAddons3.Deleted += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcher.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watcherAddons3.EnableRaisingEvents = true;
                //Console.WriteLine("監視を開始しました");
            }
            while (false);
        }
        private void WatcherStartAddonsSimpleMaillog(string strInstallPath)
        {
            var strLogPath = "";
            if (watcherAddons4 != null) return;
            watcherAddons4 = new System.IO.FileSystemWatcher();
            do
            {
                string filePath = strInstallPath;
                if (File.Exists(filePath) == false)
                {
                    break;
                }
                strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "addons\\SimpleMaillog\\log";
                if (Directory.Exists(strLogPath) == false)
                {
                    break;
                }
                //C:\Users\admin\EphineaPSO\addons\Chatlog\log
                watcherAddons4.Path = strLogPath;
                //watcher.Path = @"C:\Users\admin\EphineaPSO\log";
                //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
                /*
                watcherAddons4.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                */
                watcherNormal.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
                //すべてのファイルを監視
                watcherAddons4.Filter = "";
                //UIのスレッドにマーシャリングする
                //コンソールアプリケーションでの使用では必要ない
                watcherAddons4.SynchronizingObject = this;

                //バッファサイズを標準の4kbから、最大の64kbに増やす
                //watcherAddons4.InternalBufferSize = 65536;

                //イベントハンドラの追加
                watcherAddons4.Changed += new System.IO.FileSystemEventHandler(watcher_ChangedSimpleMail);
                watcherAddons4.Created += new System.IO.FileSystemEventHandler(watcher_ChangedSimpleMail);
                //watcherAddons4.Deleted += new System.IO.FileSystemEventHandler(watcher_ChangedAddons);
                //watcher.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

                //監視を開始する
                watcherAddons4.EnableRaisingEvents = true;
                //Console.WriteLine("監視を開始しました");
            }
            while (false);
        }
        private void WatcherEnd()
        {
            //監視を終了
            watcherNormal.EnableRaisingEvents = false;
            watcherNormal.Dispose();
            watcherNormal = null;
            watcherAddons1.EnableRaisingEvents = false;
            watcherAddons1.Dispose();
            watcherAddons1 = null;
            watcherAddons2.EnableRaisingEvents = false;
            watcherAddons2.Dispose();
            watcherAddons2 = null;
            watcherAddons3.EnableRaisingEvents = false;
            watcherAddons3.Dispose();
            watcherAddons3 = null;
            //Console.WriteLine("監視を終了しました");
        }
        private void watcher_ChangedNormal(System.Object source, System.IO.FileSystemEventArgs e)
        {
            var strFileName = "";
            strFileName = Path.GetFileName(e.FullPath);
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が変更されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "Normal", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が作成されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "Normal", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が削除されました");
                    break;
            }
        }
        private void watcher_ChangedAddons(System.Object source, System.IO.FileSystemEventArgs e)
        {
            var strFileName = "";
            strFileName = Path.GetFileName(e.FullPath);
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が変更されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "Addons", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が作成されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "Addons", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が削除されました");
                    break;
            }
        }
        private void watcher_ChangedSimpleMail(System.Object source, System.IO.FileSystemEventArgs e)
        {
            var strFileName = "";
            strFileName = Path.GetFileName(e.FullPath);
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が変更されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "SimpleMail", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が作成されました");
                    ArrayClsWaitingToTranslateAddnew(e.FullPath, "SimpleMail", strFileName, false);
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    //Console.WriteLine("ファイル 「" + e.FullPath + "」が削除されました");
                    break;
            }
        }
        //************************************************************************************************************************************************************************************************
        //Watcher設定終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //終了処理開始
        //************************************************************************************************************************************************************************************************

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            WatcherEnd();
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            //var clsSetIniFile = new clsSetIniFile();
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strFileNameLog = "";
            var strFileNameLogLV = "";
            var strLineText = "";
            var myCheck = Regex.IsMatch(Value.strEnvironment, ".*TexTra.*", RegexOptions.Singleline);
            do
            {
                if (myCheck == true)
                {
                    //TexTra
                    strFileNameLog = ".\\" + "ChatTexTra" + ".Log";
                    strFileNameLogLV = ".\\" + "ChatTexTra_LV" + ".Log";
                    break;
                }
                if (myCheck == false)
                {
                    //従来Exe
                    strFileNameLog = ".\\" + "Chat" + ".Log";
                    strFileNameLogLV = ".\\" + "Chat_LV" + ".Log";
                    break;
                }
            } while (false);

            var strSaveScreenPos = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveScreenPos", strIniFileName);
            if (strSaveScreenPos.Equals("true") != false)
            {
                FileUtil.SetValue(strEnvironment, "ScreenPosStX", this.Left.ToString(), FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosStY", this.Top.ToString(), FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosEdX", this.Width.ToString(), FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosEdY", this.Height.ToString(), FileUtil.strIniFileName);
            }
            else
            {
                FileUtil.SetValue(strEnvironment, "ScreenPosStX", "-7", FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosStY", "0", FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosEdX", "520", FileUtil.strIniFileName);
                FileUtil.SetValue(strEnvironment, "ScreenPosEdY", "480", FileUtil.strIniFileName);
            }
            //stop();
            
            var strSaveChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChat", strIniFileName);
            var strSaveChatRow = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SaveChatRow", strIniFileName);
            if (strSaveChat.Equals("true") != false)
            {
                UnicodeEncoding unicode = new UnicodeEncoding(true, false);

                System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileNameLog, false, System.Text.Encoding.GetEncoding("UTF-8"));
                if (string.IsNullOrEmpty(strSaveChatRow) != false)
                {
                    strSaveChatRow = "0";
                }
                var numStRow = listBox1.Items.Count - Int32.Parse(strSaveChatRow);
                if (numStRow < 0)
                {
                    numStRow = 0;
                }
                for (int i = numStRow; i < listBox1.Items.Count; i++)
                {
                    strLineText = "";
                    strLineText = listBox1.Items[i].ToString();
                    if (Regex.IsMatch(strLineText, "^Waiting", RegexOptions.Singleline) == false)
                    {
                        sw.WriteLine(strLineText);
                    }
                }
                //閉じる
                sw.Close();
            }
            //stop();
            if (strSaveChat.Equals("true") != false)
            {
                ListViewTextSave(strFileNameLogLV, strSaveChatRow);
            }
            
            do
            {
                //stop();
                if (automaticUpdate.AwaitUpDateBTText == null)
                {
                    break;
                }
                if (automaticUpdate.AwaitUpDateBTText.Equals("Success") == false)
                {
                    break;
                }
                var pathFrom = System.Environment.CurrentDirectory + "\\Extract\\" + Value.strPSOChatLogVersionCurrent + "\\PSOChatLogUpdater.exe";
                var pathTo = System.Environment.CurrentDirectory;

                MessageBox.Show("アップデーターを起動します。");

                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.Arguments = automaticUpdate.AwaitFileNameDownload + " " + automaticUpdate.AwaitCheckVerResult;
                pInfo.FileName = "PSOChatLogUpdater.exe";
                Process.Start(pInfo);
            } while (false);
        }
        //************************************************************************************************************************************************************************************************
        //終了処理終了
        //************************************************************************************************************************************************************************************************



        void ListViewTextSave(string strFileName, string strSaveChatRow)
        {
            string saveData = "";
            int iSaveChatRow = Int32.Parse(strSaveChatRow);
            int iRowStart = 0;
            string strLine = "";

            //<listview>
            //listView1.Columns.Add("翻訳", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("数", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("日付", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("原文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("訳文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("時刻", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("種別", 40, HorizontalAlignment.Left);
            //listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("名前", 120, HorizontalAlignment.Left);
            //listView1.Columns.Add("文章", 1000, HorizontalAlignment.Left);
            
            iRowStart = listView1.Items.Count - iSaveChatRow;
            if (iRowStart < 0) {
                iRowStart = 0;
            }
            for (int i = iRowStart; i < listView1.Items.Count - 1; i++)
            {
                strLine = "";
                do
                {
                    try
                    {
                        if (Regex.IsMatch(listView1.Items[i].SubItems[3].Text, "Waiting", RegexOptions.Singleline))
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    try
                    {
                        if (Regex.IsMatch(listView1.Items[i].SubItems[5].Text, "<........>", RegexOptions.Singleline))
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    for (int j = 0; j < listView1.Items[i].SubItems.Count; j++)
                    {
                        strLine += listView1.Items[i].SubItems[j].Text + "\t";
                    }
                    //strLine = strLine.Substring(0, strLine.Length - 1);
                    saveData += strLine;
                    saveData += Environment.NewLine;
                } while (false);
            }
            if (saveData != "")
            {
                //stop();
                System.IO.File.WriteAllText(strFileName, saveData);
            }
        }
        void ListBoxTextLoad(string strFileName)
        {
            do
            {
                if (File.Exists(strFileName) == false)
                {
                    break;
                }
                if (listBox1.Items.Count > 0)
                {
                    break;
                }
                FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
                // テキストエンコーディングにUTF-8を用いてstreamの読み込みを行うStreamReaderを作成する
                var sr = new StreamReader(fs);

                //streamから文字列を読み込み
                string strBuf = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();
                strBuf = strBuf.Replace("\r\n\r\n", "\r\n");
                strBuf = strBuf.TrimEnd('\r', '\n');
                var strLogData = Regex.Split(strBuf, "\r\n");
                for (int i = 0; i < strLogData.Length; i++)
                {
                    if (strLogData[i].Equals(""))
                    {
                        continue;
                    }
                    listBox1.Items.Add(strLogData[i]);
                }
                listBox1.TopIndex = listBox1.Items.Count - 1;
            } while (false);
        }
        void ListViewTextLoad(string strFileName)
        {
            do
            {
                if (File.Exists(strFileName) == false)
                {
                    break;
                }
                //<listview>
                listView1.Items.Clear();
                string[] data = System.IO.File.ReadAllText(strFileName).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                //stop();
                foreach (string line in data)
                {
                    //string[] items = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var RegMatch = Regex.Match(line, "\\t");
                    if (RegMatch.Success == false)
                    {
                        break;
                    }
                    string[] items = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    listView1.Items.Add(new ListViewItem(items));
                }
                //listView1.ListItems(listView1.ListItems.Count).EnsureVisible;
                if (listView1.Items.Count == 0)
                {
                    break;
                }
                listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            } while (false);
        }
        public static void EnableDoubleBuffering(Control control)
        {
            control.GetType().InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               control,
               new object[] { true });
        }
        private void Combobox1SettingLanguage_GP()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(Language_GP.LANGUAGES_GP.ToArray());

            // コンボボックスの初期位置です。
            comboBox1.SelectedItem = Language_GP.REVERSED_DECTIONARY[Language_GP.myLanguage_GP()];
            // コンボボックスを入力しないようにする。
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void ListControlUpdate()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strViewControl = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ViewControl", strIniFileName);
            do
            {
                if (strViewControl == "ListView")
                {
                    listView1.BringToFront();
                    break;
                }
                if (strViewControl == "ListBox")
                {
                    listBox1.BringToFront();
                    break;
                }
                listView1.BringToFront();
                break;
            } while (false);
        }
        private void ListBoxSetting()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strForeColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorRed", strIniFileName);
            var strForeColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorGreen", strIniFileName);
            var strForeColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorBlue", strIniFileName);
            var strBackColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorRed", strIniFileName);
            var strBackColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorGreen", strIniFileName);
            var strBackColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorBlue", strIniFileName);
            var strFontName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontName", strIniFileName);
            var strFontSize = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontSize", strIniFileName);

            if (strForeColorRed.Equals(""))
            {
                strForeColorRed = "0";
            }
            if (strForeColorGreen.Equals(""))
            {
                strForeColorGreen = "0";
            }
            if (strForeColorBlue.Equals(""))
            {
                strForeColorBlue = "0";
            }
            if (strBackColorRed.Equals(""))
            {
                strBackColorRed = "255";
            }
            if (strBackColorGreen.Equals(""))
            {
                strBackColorGreen = "255";
            }
            if (strBackColorBlue.Equals(""))
            {
                strBackColorBlue = "255";
            }
            if (strFontName.Equals(""))
            {
                strFontName = "MS UI Gothic";
            }
            if (strFontSize.Equals(""))
            {
                strFontSize = "9";
            }
            listBox1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            textBox1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            textBox2.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            comboBox1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));

            listBox1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            textBox1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            textBox2.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            comboBox1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));

            listBox1.Font = new Font(strFontName, Int32.Parse(strFontSize));
            listBox1.UseCustomTabOffsets = true;
            listBox1.CustomTabOffsets.Clear();
            //listBox1.CustomTabOffsets.AddRange(new int[] { 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190 });
            //listBox1.CustomTabOffsets.AddRange(new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190 });
            listBox1.CustomTabOffsets.AddRange(new int[] { 30, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190 });
            //listBox1.Items.Add("\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10\t11\t12");
            //listBox1.UseCustomTabOffsets = true;
            //listBox1.DrawMode = DrawMode.OwnerDrawFixed;
        }
        private void ListViewSetting()
        {
            //listView1 = Single;
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strForeColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorRed", strIniFileName);
            var strForeColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorGreen", strIniFileName);
            var strForeColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "ForeColorBlue", strIniFileName);
            var strBackColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorRed", strIniFileName);
            var strBackColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorGreen", strIniFileName);
            var strBackColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "BackColorBlue", strIniFileName);
            var strFontName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontName", strIniFileName);
            var strFontSize = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "FontSize", strIniFileName);

            if (strForeColorRed.Equals(""))
            {
                strForeColorRed = "0";
            }
            if (strForeColorGreen.Equals(""))
            {
                strForeColorGreen = "0";
            }
            if (strForeColorBlue.Equals(""))
            {
                strForeColorBlue = "0";
            }
            if (strBackColorRed.Equals(""))
            {
                strBackColorRed = "255";
            }
            if (strBackColorGreen.Equals(""))
            {
                strBackColorGreen = "255";
            }
            if (strBackColorBlue.Equals(""))
            {
                strBackColorBlue = "255";
            }
            if (strFontName.Equals(""))
            {
                strFontName = "MS UI Gothic";
            }
            if (strFontSize.Equals(""))
            {
                strFontSize = "9";
            }
            listView1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            textBox1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            textBox2.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));
            comboBox1.ForeColor = Color.FromArgb(Int32.Parse(strForeColorRed), Int32.Parse(strForeColorGreen), Int32.Parse(strForeColorBlue));

            listView1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            textBox1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            textBox2.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            comboBox1.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));

            listView1.Font = new Font(strFontName, Int32.Parse(strFontSize));

            listView1.View = System.Windows.Forms.View.Details;
            listView1.FullRowSelect = true;

            var strFirstLanguage_GP = Language_GP.myLanguage_GP();
            if (strFirstLanguage_GP.Equals("JA"))
            {
                listView1.Columns.Add("翻訳", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("数", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("日付", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("原文", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("訳文", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("時刻", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("種別", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("名前", 120, HorizontalAlignment.Left);
                listView1.Columns.Add("文章", 1000, HorizontalAlignment.Left);
            }
            else
            {
                listView1.Columns.Add("Trans", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("num", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("Date", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("OrgMsg", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("TraMsg", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("Time", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("Type", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
                listView1.Columns.Add("Name", 120, HorizontalAlignment.Left);
                listView1.Columns.Add("Text", 1000, HorizontalAlignment.Left);
            }
            //<listview>
        }
        private void listViewHeaderReset()
        {
            var strFirstLanguage_GP = Language_GP.myLanguage_GP();
            do
            {
                if (listView1.Columns.Count == 0)
                {
                    break;
                }
                if (strFirstLanguage_GP.Equals("JA"))
                {
                    listView1.Columns[0].Text = "翻訳";
                    listView1.Columns[1].Text = "数";
                    listView1.Columns[2].Text = "LogType";
                    listView1.Columns[3].Text = "MsgType";
                    listView1.Columns[4].Text = "日付";
                    listView1.Columns[5].Text = "原文";
                    listView1.Columns[6].Text = "訳文";
                    listView1.Columns[7].Text = "時刻";
                    listView1.Columns[8].Text = "種別";
                    listView1.Columns[9].Text = "ID";
                    listView1.Columns[10].Text = "名前";
                    listView1.Columns[11].Text = "文章";
                }
                else
                {
                    listView1.Columns[0].Text = "Trans";
                    listView1.Columns[1].Text = "num";
                    listView1.Columns[2].Text = "LogType";
                    listView1.Columns[3].Text = "MsgType";
                    listView1.Columns[4].Text = "Date";
                    listView1.Columns[5].Text = "OrgMsg";
                    listView1.Columns[6].Text = "TraMsg";
                    listView1.Columns[7].Text = "Time";
                    listView1.Columns[8].Text = "Type";
                    listView1.Columns[9].Text = "ID";
                    listView1.Columns[10].Text = "Name";
                    listView1.Columns[11].Text = "Text";
                }
            } while (false);
        }
        public string GetButtonLabel()
        {
            var strFirstLanguage_GP = Language_GP.myLanguage_GP();
            var strChatLang = gfGetChatLang();
            var strReturn = "";

            do
            {
                if (strFirstLanguage_GP.Equals("JA"))
                {
                    if (strChatLang.Equals("EN"))
                    {
                        strReturn = "英";
                        break;
                    }
                    if (strChatLang.Equals("DE"))
                    {
                        strReturn = "独";
                        break;
                    }
                    if (strChatLang.Equals("ES"))
                    {
                        strReturn = "西";
                        break;
                    }
                    if (strChatLang.Equals("FR"))
                    {
                        strReturn = "仏";
                        break;
                    }
                    if (strChatLang.Equals("RU"))
                    {
                        strReturn = "露";
                        break;
                    }
                    if (strChatLang.Equals("ZH-CN"))
                    {
                        strReturn = "中";
                        break;
                    }
                    if (strChatLang.Equals("ZH-TW"))
                    {
                        strReturn = "台";
                        break;
                    }
                    if (strChatLang.Equals("ZH"))
                    {
                        strReturn = "中";
                        break;
                    }
                    if (strChatLang.Equals("JA"))
                    {
                        strReturn = "和";
                        break;
                    }
                    if (strChatLang.Equals("KO"))
                    {
                        strReturn = "韓";
                        break;
                    }
                    break;
                }
                if (strChatLang.Equals("EN"))
                {
                    strReturn = "E";
                    break;
                }
                if (strChatLang.Equals("DE"))
                {
                    strReturn = "D";
                    break;
                }
                if (strChatLang.Equals("ES"))
                {
                    strReturn = "E";
                    break;
                }
                if (strChatLang.Equals("FR"))
                {
                    strReturn = "F";
                    break;
                }
                if (strChatLang.Equals("RU"))
                {
                    strReturn = "R";
                    break;
                }
                if (strChatLang.Equals("ZH-CN"))
                {
                    strReturn = "C";
                    break;
                }
                if (strChatLang.Equals("ZH-TW"))
                {
                    strReturn = "T";
                    break;
                }
                if (strChatLang.Equals("ZH"))
                {
                    strReturn = "C";
                    break;
                }
                if (strChatLang.Equals("JA"))
                {
                    strReturn = "J";
                    break;
                }
                if (strChatLang.Equals("KO"))
                {
                    strReturn = "K";
                    break;
                }
            }
            while (false);
            return strReturn;
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
        private void DoResize()
        {
            //画面描画
            //リサイズ終了後、再描画とレイアウトロジックを実行する
            var lNormalHeight =480;
            var lNormalWidth = 520;

            NICT.Left = this.Width - (lNormalWidth - 397);

            label1.Left = this.Width - (lNormalWidth - 337);
            comboBox1.Left = this.Width - (lNormalWidth - 401);

            listBox1.Width = this.Width - (lNormalWidth - 480);
            listBox1.Height = this.Height - (listBox1.Top + (lNormalHeight - listBox1.Top - 280));
            listView1.Width = this.Width - (lNormalWidth - 480);
            listView1.Height = this.Height - (listView1.Top + (lNormalHeight - listView1.Top - 280));

            button7.Top = this.Height - (lNormalHeight - 352);
            button8.Top = this.Height - (lNormalHeight - 352);
            button9.Top = this.Height - (lNormalHeight - 352);
            button10.Top = this.Height - (lNormalHeight - 352);
            button11.Top = this.Height - (lNormalHeight - 352);
            button12.Top = this.Height - (lNormalHeight - 352);
            button13.Top = this.Height - (lNormalHeight - 352);
            button14.Top = this.Height - (lNormalHeight - 352);
            button15.Top = this.Height - (lNormalHeight - 352);
            button16.Top = this.Height - (lNormalHeight - 352);

            label2.Top = this.Height - (lNormalHeight - 354);
            label2.Left = this.Width - (lNormalWidth - 337);
            comboBox2.Top = this.Height - (lNormalHeight - 351);
            comboBox2.Left = this.Width - (lNormalWidth - 401);

            textBox1.Top = this.Height - (lNormalHeight - 382);
            //textBox1.Left = 12;
            textBox1.Width = this.Width - (lNormalWidth - 362);
            //textBox1.Height = 19;//19
            textBox2.Top = this.Height - (lNormalHeight - 409);
            //textBox2.Left = 12;
            textBox2.Width = this.Width - (lNormalWidth - 362);
            //textBox2.Height = 19;//19

            Button2.Top = this.Height - (lNormalHeight - 381);
            Button2.Left = this.Width - (lNormalWidth - 383);
            Button4.Top = this.Height - (lNormalHeight - 381);
            Button4.Left = this.Width - (lNormalWidth - 441);
            Button1.Top = this.Height - (lNormalHeight - 408);
            Button1.Left = this.Width - (lNormalWidth - 383);
            Button3.Top = this.Height - (lNormalHeight - 408);
            Button3.Left = this.Width - (lNormalWidth - 441);

            this.Invalidate();
            this.PerformLayout();
            ListViewScrollCheckBottom(listView1);
            ListBoxScrollCheckBottom(listBox1);
        }
        private AutomaticUpdate automaticUpdate = new AutomaticUpdate();
        private async void doVerCheckPSOChatLog()
        {
            //キャンセルトークン取得
            CancellationToken cancelToken = new();
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            cancelToken = cancellationTokenSource.Token;

            //実処理
            IntPtr hWindowHandleOwnerWindow = this.Handle;
            IntPtr hWindowHandleCallWindow = this.Handle;

            automaticUpdate.SetAwaitParameter("CheckVer", hWindowHandleOwnerWindow, hWindowHandleCallWindow);
            await automaticUpdate.DoAutomaticUpdateAsync(cancelToken);

            //キャンセルトークンの後始末
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;

            //stop();
            do
            {
                if (automaticUpdate.AwaitCheckVerResult == null)
                {
                    break;
                }
                if (automaticUpdate.AwaitCheckVerResult.Equals(""))
                {
                    break;
                }
                if (decimal.Parse(Value.strPSOChatLogVersionCurrent) >= decimal.Parse(automaticUpdate.AwaitCheckVerResult))
                {
                    break;
                }
                this.button6.Visible = true;
                this.button6.Enabled = automaticUpdate.AwaitUpDateBTEnable;
                this.button6.Text = automaticUpdate.AwaitUpDateBTText;
            } while (false);
        }
        private async void doDownloadPSOChatLog()
        {
            //キャンセルトークン取得
            CancellationToken cancelToken = new();
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            cancelToken = cancellationTokenSource.Token;

            //実処理
            IntPtr hWindowHandleOwnerWindow = this.Handle;
            IntPtr hWindowHandleCallWindow = this.Handle;
            automaticUpdate.SetAwaitParameter("Download", hWindowHandleOwnerWindow, hWindowHandleCallWindow);
            await automaticUpdate.DoAutomaticUpdateAsync(cancelToken);

            //キャンセルトークンの後始末
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
        private void SendTextChatLogToPSOBB(string strImput)
        {
            var strData = "";
            var strExecPath = "";
            var strTmp = "";
            var intChar = 0;

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            //var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            //var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            //var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            //var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            //var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            var strSendTextMode = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SendTextMode", strIniFileName);
            var strWaitSetKeyDelayDelay = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSetKeyDelayDelay", strIniFileName);
            var strWaitSetKeyDelayPressDuration = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSetKeyDelayPressDuration", strIniFileName);
            var strWaitSpacekeyChatDelay = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitSpacekeyChatDelay", strIniFileName);
            var strWaitInputModeChangeWait = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitInputModeChangeWait", strIniFileName);
            var strWaitCharToCharTransmissionWaitBoth = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitCharToCharTransmissionWaitBoth", strIniFileName);
            var strWaitCharToCharTransmissionWaitChatLogTool = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "WaitCharToCharTransmissionWaitChatLogTool", strIniFileName);


            if (strSendTextMode.Equals("AHK") == true)
            {
                do//空ループ
                {
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //文字列が入っていなかったら終了2バイト文字が入っていたら終了
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    if (string.IsNullOrWhiteSpace(strImput))
                    {
                        //Console.WriteLine("IsNullOrWhiteSpace");
                        break;
                    }
                    if (isOneByteChar(strImput) == false)
                    {
                        //Console.WriteLine("isOneByteChar");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //AHKがインストールされているかチェック
                    //C:\Program Files\AutoHotkey\AutoHotkey.exe
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    string filePath = @"C:\Program Files\AutoHotkey\AutoHotkey.exe";
                    //string filePath = @"C:\Program Files\AHK\AutoHotkey.exe";
                    if (File.Exists(filePath) == false)
                    {
                        //Console.WriteLine("AutoHotkey is not found");
                        var strLangTarget = Language_GP.myLanguage_GP();
                        if (strLangTarget.Equals("JA") != false)
                        {
                            ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "送信機能を実行するには、AHKをインストールしてください。");
                        }
                        else
                        {
                            ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Please install AHK to perform the send function.");
                        }
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLogのパスを取得する
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string path = myAssembly.Location;
                    strExecPath = GetFileNameFullPathToPathName(path);
                    //Console.WriteLine("path = " + strExecPath);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLog.ahkファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    strData = "#SingleInstance force\n";
                    strData += "SetKeyDelay, " + strWaitSetKeyDelayDelay + ", " + strWaitSetKeyDelayPressDuration + "\n";
                    strData += "Process,Exist,psobb.exe\n";
                    strData += "If (ErrorLevel != 0)\n";
                    strData += "{\n";
                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\n";
                    if (strSpaceChat.Equals("true"))
                    {
                        strData += "\tSend, {Space}\t;Start Chat\n";
                    }
                    strData += "\tIME_SET_COPY(0)\n";
                    for (int i = 0; i <= strImput.Length - 1; i++)
                    {
                        strTmp = strImput.Substring(i, 1);
                        intChar = gfGetChar(strTmp);
                        //strData += "\tsleep " + r1.Next(100, 200) + "\n";
                        //strData += "\tSend, {Alt}\n";
                        strData += "\tSend, {asc " + (intChar) + "}\t;" + strTmp + "\n";
                        //strData += "\tSend, {Alt}\n";
                    }
                    strData += "\tExitApp\n";
                    strData += "}\n";
                    strData += "ExitApp\n";
                    strData += "\n";
                    strData += ";http://www6.atwiki.jp/eamat/\n";
                    strData += ";https://w.atwiki.jp/eamat/pages/17.html\n";
                    strData += "IME_SET_COPY(SetSts, WinTitle=\"A\"){\n";
                    strData += "    ControlGet,hwnd,HWND,,,%WinTitle%\n";
                    strData += "    if	(WinActive(WinTitle))	{\n";
                    strData += "        ptrSize := !A_PtrSize ? 4 : A_PtrSize\n";
                    strData += "        VarSetCapacity(stGTI, cbSize:=4+4+(PtrSize*6)+16, 0)\n";
                    strData += "        NumPut(cbSize, stGTI,  0, \"UInt\")   ;	DWORD   cbSize;\n";
                    strData += "        hwnd := DllCall(\"GetGUIThreadInfo\", Uint,0, Uint,&stGTI)\n";
                    strData += "                 ? NumGet(stGTI,8+PtrSize,\"UInt\") : hwnd\n";
                    strData += "    }\n";
                    strData += "\n";
                    strData += "    return DllCall(\"SendMessage\"\n";
                    strData += "          , UInt, DllCall(\"imm32\\ImmGetDefaultIMEWnd\", Uint,hwnd)\n";
                    strData += "          , UInt, 0x0283  ;Message : WM_IME_CONTROL\n";
                    strData += "          ,  Int, 0x006   ;wParam  : IMC_SETOPENSTATUS\n";
                    strData += "          ,  Int, SetSts) ;lParam  : 0 or 1\n";
                    strData += "}\n";
                    strData += "\n";
                    strData += "Pause::ExitApp\n";
                    strData += "Break::ExitApp\n";
                    strData += "Esc::ExitApp\n";
                    File.WriteAllText(strExecPath + strEnvironment + ".ahk", strData);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //AHKにPSOChatLog.ahkファイルを実行させる
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + strEnvironment + ".ahk");
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                }
                while (false);//空ループなので、必ず一回で処理を抜ける
            }

            if (strSendTextMode.Equals("Both") == true)
            {
                do//空ループ
                {
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //文字列が入っていなかったら終了
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    if (string.IsNullOrWhiteSpace(strImput))
                    {
                        //Console.WriteLine("IsNullOrWhiteSpace");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //AHKがインストールされているかチェック
                    //C:\Program Files\AutoHotkey\AutoHotkey.exe
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    string filePath = @"C:\Program Files\AutoHotkey\AutoHotkey.exe";
                    if (File.Exists(filePath) == false)
                    {
                        //Console.WriteLine("AutoHotkey is not found");
                        ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "送信機能を実行するには、AHKをインストールしてください。");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLogのパスを取得する
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string path = myAssembly.Location;
                    strExecPath = GetFileNameFullPathToPathName(path);
                    //Console.WriteLine("path = " + strExecPath);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //ime_enable.ahkファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    strData = "#SingleInstance force\n";
                    //strData += "SetKeyDelay, 100, 100\n";
                    strData += "Process,Exist,psobb.exe\n";
                    strData += "If (ErrorLevel != 0)\n";
                    strData += "{\n";
                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\n";
                    strData += "\tIME_SET_COPY(1)\n";
                    strData += "\tExitApp\n";
                    strData += "}\n";
                    strData += "ExitApp\n";
                    strData += "\n";
                    strData += ";http://www6.atwiki.jp/eamat/\n";
                    strData += ";https://w.atwiki.jp/eamat/pages/17.html\n";
                    strData += "IME_SET_COPY(SetSts, WinTitle=\"A\"){\n";
                    strData += "    ControlGet,hwnd,HWND,,,%WinTitle%\n";
                    strData += "    if	(WinActive(WinTitle))	{\n";
                    strData += "        ptrSize := !A_PtrSize ? 4 : A_PtrSize\n";
                    strData += "        VarSetCapacity(stGTI, cbSize:=4+4+(PtrSize*6)+16, 0)\n";
                    strData += "        NumPut(cbSize, stGTI,  0, \"UInt\")   ;	DWORD   cbSize;\n";
                    strData += "        hwnd := DllCall(\"GetGUIThreadInfo\", Uint,0, Uint,&stGTI)\n";
                    strData += "                 ? NumGet(stGTI,8+PtrSize,\"UInt\") : hwnd\n";
                    strData += "    }\n";
                    strData += "\n";
                    strData += "    return DllCall(\"SendMessage\"\n";
                    strData += "          , UInt, DllCall(\"imm32\\ImmGetDefaultIMEWnd\", Uint,hwnd)\n";
                    strData += "          , UInt, 0x0283  ;Message : WM_IME_CONTROL\n";
                    strData += "          ,  Int, 0x006   ;wParam  : IMC_SETOPENSTATUS\n";
                    strData += "          ,  Int, SetSts) ;lParam  : 0 or 1\n";
                    strData += "}\n";
                    strData += "\n";
                    strData += "Pause::ExitApp\n";
                    strData += "Break::ExitApp\n";
                    strData += "Esc::ExitApp\n";
                    File.WriteAllText(strExecPath + "ime_enable" + ".ahk", strData);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //ime_disable.ahkファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    strData = "#SingleInstance force\n";
                    //strData += "SetKeyDelay, 100, 100\n";
                    strData += "Process,Exist,psobb.exe\n";
                    strData += "If (ErrorLevel != 0)\n";
                    strData += "{\n";
                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\n";
                    strData += "\tIME_SET_COPY(0)\n";
                    strData += "\tExitApp\n";
                    strData += "}\n";
                    strData += "ExitApp\n";
                    strData += "\n";
                    strData += ";http://www6.atwiki.jp/eamat/\n";
                    strData += ";https://w.atwiki.jp/eamat/pages/17.html\n";
                    strData += "IME_SET_COPY(SetSts, WinTitle=\"A\"){\n";
                    strData += "    ControlGet,hwnd,HWND,,,%WinTitle%\n";
                    strData += "    if	(WinActive(WinTitle))	{\n";
                    strData += "        ptrSize := !A_PtrSize ? 4 : A_PtrSize\n";
                    strData += "        VarSetCapacity(stGTI, cbSize:=4+4+(PtrSize*6)+16, 0)\n";
                    strData += "        NumPut(cbSize, stGTI,  0, \"UInt\")   ;	DWORD   cbSize;\n";
                    strData += "        hwnd := DllCall(\"GetGUIThreadInfo\", Uint,0, Uint,&stGTI)\n";
                    strData += "                 ? NumGet(stGTI,8+PtrSize,\"UInt\") : hwnd\n";
                    strData += "    }\n";
                    strData += "\n";
                    strData += "    return DllCall(\"SendMessage\"\n";
                    strData += "          , UInt, DllCall(\"imm32\\ImmGetDefaultIMEWnd\", Uint,hwnd)\n";
                    strData += "          , UInt, 0x0283  ;Message : WM_IME_CONTROL\n";
                    strData += "          ,  Int, 0x006   ;wParam  : IMC_SETOPENSTATUS\n";
                    strData += "          ,  Int, SetSts) ;lParam  : 0 or 1\n";
                    strData += "}\n";
                    strData += "\n";
                    strData += "Pause::ExitApp\n";
                    strData += "Break::ExitApp\n";
                    strData += "Esc::ExitApp\n";
                    File.WriteAllText(strExecPath + "ime_disable" + ".ahk", strData);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //space.ahkファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    strData = "#SingleInstance force\n";
                    strData += "SetKeyDelay, 100, 100\n";
                    strData += "Process,Exist,psobb.exe\n";
                    strData += "If (ErrorLevel != 0)\n";
                    strData += "{\n";
                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\n";
                    strData += "\tSend, {Space}\t;Start Chat\n";
                    strData += "\tExitApp\n";
                    strData += "}\n";
                    strData += "ExitApp\n";
                    strData += "\n";
                    strData += "Pause::ExitApp\n";
                    strData += "Break::ExitApp\n";
                    strData += "Esc::ExitApp\n";
                    File.WriteAllText(strExecPath + "space" + ".ahk", strData);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //IMEZhToJa.ahkファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    strData = "#SingleInstance force\r\n";
                    strData += "SetKeyDelay, 100, 100\r\n";
                    strData += "Process,Exist,psobb.exe\r\n";
                    strData += "If(ErrorLevel != 0)\r\n";
                    strData += "{\r\n";
                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\r\n";
                    strData += "\tSleep, 100\r\n";
                    strData += "\tSend { ctrl down}{ shift}{ ctrl up}\t;IME Zh to Ja\r\n";
                    strData += "\tSleep, 100\r\n";
                    strData += "\tExitApp\r\n";
                    strData += "}\r\n";
                    strData += "ExitApp\r\n";
                    strData += "\r\n";
                    strData += "Pause::ExitApp\n";
                    strData += "Break::ExitApp\n";
                    strData += "Esc::ExitApp\n";
                    File.WriteAllText(strExecPath + "IMEZhToJa" + ".ahk", strData);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    var bFlgNoZhToJa = false;
                    do
                    {
                        if (comboBox1.Text.Equals("簡体字") == false && comboBox1.Text.Equals("繁体字") == false)
                        {
                            break;
                        }
                        if (comboBox2.Text.Equals("русский язык") == false && comboBox2.Text.Equals("日本語") == false && comboBox2.Text.Equals("조선어") == false)
                        {
                            break;
                        }
                        var strUseIMEZhToJa = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "UseIMEZhToJa", strIniFileName);
                        if (strUseIMEZhToJa.Equals("true") == false)
                        {
                            break;
                        }
                        bFlgNoZhToJa = true;
                    }
                    while (false);

                    //ウィンドウのタイトルに「Ephinea: Phantasy Star Online Blue Burst」を含むプロセスをすべて取得する
                    Process[] ps = GetProcessesByWindowTitle("Ephinea: Phantasy Star Online Blue Burst");
                    //アクティブウィンドウにする。
                    foreach (Process p in ps)
                    {
                        Microsoft.VisualBasic.Interaction.AppActivate(p.MainWindowTitle);
                        //SendKeys.Send(textBox1.Text);
                    }

                    if (strSpaceChat.Equals("true") != false)
                    {
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //AHKにspace.ahkファイルを実行させる
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "space" + ".ahk");
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Thread.Sleep(Int32.Parse(strWaitSpacekeyChatDelay));
                    }
                    if (bFlgNoZhToJa != false)
                    {
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //AHKにIMEZhToJa.ahkファイルを実行させる
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "IMEZhToJa" + ".ahk");
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Thread.Sleep(Int32.Parse(strWaitSpacekeyChatDelay));
                    }

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //AHKにime_disable.ahkファイルを実行させる
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "ime_disable" + ".ahk");
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Thread.Sleep(Int32.Parse(strWaitInputModeChangeWait));
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //AHKにime_enable.ahkファイルを実行させる
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "ime_enable" + ".ahk");
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Thread.Sleep(Int32.Parse(strWaitInputModeChangeWait));

                    var numBackMode = 0;
                    for (int i = 0; i <= strImput.Length - 1; i++)
                    {
                        strTmp = strImput.Substring(i, 1);
                        if (isOneByteChar(strTmp) == false)
                        {
                            //2バイトキャラ
                            if (numBackMode != 2)
                            {
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                //AHKにime_enable.ahkファイルを実行させる
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "ime_enable" + ".ahk");
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                Thread.Sleep(Int32.Parse(strWaitInputModeChangeWait));
                                numBackMode = 2;
                            }
                            SendKeys.SendWait(strTmp);
                            Thread.Sleep(Int32.Parse(strWaitCharToCharTransmissionWaitBoth));
                        }
                        else
                        {
                            //1バイトキャラ
                            if (numBackMode != 1)
                            {
                                SendKeys.SendWait("{ENTER}");
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                //AHKにime_disable.ahkファイルを実行させる
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "ime_disable" + ".ahk");
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                Thread.Sleep(Int32.Parse(strWaitInputModeChangeWait));
                                numBackMode = 1;
                            }
                            try
                            {
                                SendKeys.SendWait(strTmp);
                            }
                            catch
                            {
                                if (isOneByteChar(strTmp) == false)
                                {
                                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "\"" + strTmp + "\"" + " Can not Send This character");
                                }
                                else
                                {
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                    //PSOChatLog.ahkファイルの生成
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                    strData = "#SingleInstance force\n";
                                    strData += "SetKeyDelay, " + strWaitSetKeyDelayDelay + ", " + strWaitSetKeyDelayPressDuration + "\n";
                                    strData += "Process,Exist,psobb.exe\n";
                                    strData += "If (ErrorLevel != 0)\n";
                                    strData += "{\n";
                                    strData += "\tWinActivate,ahk_pid %ErrorLevel%\n";
                                    if (strSpaceChat.Equals("true"))
                                    {
                                        strData += "\tSend, {Space}\t;Start Chat\n";
                                    }
                                    strData += "\tIME_SET_COPY(0)\n";
                                    intChar = gfGetChar(strTmp);
                                    strData += "\tSend, {asc " + (intChar) + "}\t;" + strTmp + "\n";
                                    strData += "\tExitApp\n";
                                    strData += "}\n";
                                    strData += "ExitApp\n";
                                    strData += "\n";
                                    strData += ";http://www6.atwiki.jp/eamat/\n";
                                    strData += ";https://w.atwiki.jp/eamat/pages/17.html\n";
                                    strData += "IME_SET_COPY(SetSts, WinTitle=\"A\"){\n";
                                    strData += "    ControlGet,hwnd,HWND,,,%WinTitle%\n";
                                    strData += "    if	(WinActive(WinTitle))	{\n";
                                    strData += "        ptrSize := !A_PtrSize ? 4 : A_PtrSize\n";
                                    strData += "        VarSetCapacity(stGTI, cbSize:=4+4+(PtrSize*6)+16, 0)\n";
                                    strData += "        NumPut(cbSize, stGTI,  0, \"UInt\")   ;	DWORD   cbSize;\n";
                                    strData += "        hwnd := DllCall(\"GetGUIThreadInfo\", Uint,0, Uint,&stGTI)\n";
                                    strData += "                 ? NumGet(stGTI,8+PtrSize,\"UInt\") : hwnd\n";
                                    strData += "    }\n";
                                    strData += "\n";
                                    strData += "    return DllCall(\"SendMessage\"\n";
                                    strData += "          , UInt, DllCall(\"imm32\\ImmGetDefaultIMEWnd\", Uint,hwnd)\n";
                                    strData += "          , UInt, 0x0283  ;Message : WM_IME_CONTROL\n";
                                    strData += "          ,  Int, 0x006   ;wParam  : IMC_SETOPENSTATUS\n";
                                    strData += "          ,  Int, SetSts) ;lParam  : 0 or 1\n";
                                    strData += "}\n";
                                    strData += "\n";
                                    strData += "Pause::ExitApp\n";
                                    strData += "Break::ExitApp\n";
                                    strData += "Esc::ExitApp\n";
                                    File.WriteAllText(strExecPath + strEnvironment + ".ahk", strData);
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                                    Thread.Sleep(500);
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                    //AHKにPSOChatLog.ahkファイルを実行させる
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                    Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + strEnvironment + ".ahk");
                                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                    Thread.Sleep(500);
                                }
                            }
                            Thread.Sleep(Int32.Parse(strWaitCharToCharTransmissionWaitBoth));
                        }
                    }
                    Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "ime_enable" + ".ahk");
                    if (bFlgNoZhToJa != false)
                    {
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //AHKにIMEZhToJa.ahkファイルを実行させる
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Process.Start(@"C:\Program Files\AutoHotkey\AutoHotkey.exe", strExecPath + "IMEZhToJa" + ".ahk");
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        Thread.Sleep(Int32.Parse(strWaitSpacekeyChatDelay));
                    }
                }
                while (false);//空ループなので、必ず一回で処理を抜ける
            }

            if (strSendTextMode.Equals("ChatLogTool") == true)
            {
                do//空ループ
                {
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //文字列が入っていなかったら終了
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    if (string.IsNullOrWhiteSpace(strImput))
                    {
                        //Console.WriteLine("IsNullOrWhiteSpace");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLogのパスを取得する
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string path = myAssembly.Location;
                    strExecPath = GetFileNameFullPathToPathName(path);
                    //Console.WriteLine("path = " + strExecPath);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //ウィンドウのタイトルに「Ephinea: Phantasy Star Online Blue Burst」を含むプロセスをすべて取得する
                    Process[] ps = GetProcessesByWindowTitle("Ephinea: Phantasy Star Online Blue Burst");
                    //Process[] ps = GetProcessesByWindowTitle("记事本");
                    //アクティブウィンドウにする。
                    foreach (Process pPSOBB in ps)
                    {
                        Microsoft.VisualBasic.Interaction.AppActivate(pPSOBB.MainWindowTitle);
                    }

                    UnicodeEncoding unicode = new UnicodeEncoding(true, false);
                    string input = strImput;
                    Encoding EncordSource = Encoding.GetEncoding("gb2312");
                    Encoding EncordDestination = Encoding.GetEncoding("UTF-8");
                    var output = EncordDestination.GetString(EncordSource.GetBytes(input));
                    strImput = output;

                    for (int i = 0; i <= strImput.Length - 1; i++)
                    {
                        strTmp = strImput.Substring(i, 1);
                        try
                        {
                            SendKeys.SendWait(strTmp);
                        }
                        catch
                        {
                            ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "\"" + strTmp + "\"" + " Can not Send This character");
                        }
                        Thread.Sleep(Int32.Parse(strWaitCharToCharTransmissionWaitChatLogTool));
                    }
                }
                while (false);//空ループなので、必ず一回で処理を抜ける
            }

            if (strSendTextMode.Equals("PowerShell") == true)
            {
                do//空ループ
                {
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //文字列が入っていなかったら終了
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    if (string.IsNullOrWhiteSpace(strImput))
                    {
                        //Console.WriteLine("IsNullOrWhiteSpace");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLogのパスを取得する
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string path = myAssembly.Location;
                    strExecPath = GetFileNameFullPathToPathName(path);
                    //Console.WriteLine("path = " + strExecPath);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLog.batファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    var strCallPowerShell = "";
                    strCallPowerShell += "@chcp\r\n";
                    strCallPowerShell += "@chcp 65001\r\n";
                    strCallPowerShell += "@PSOChatLog.bat\r\n";
                    File.WriteAllText(strExecPath + strEnvironment + "_call" + ".bat", strCallPowerShell);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    UnicodeEncoding unicode = new UnicodeEncoding(true, false);
                    string input = strImput;
                    Encoding encSource = Encoding.GetEncoding("UTF-8");
                    Encoding encDest = Encoding.GetEncoding("UTF-8");
                    var output = encDest.GetString(encSource.GetBytes(input));
                    strImput = output;

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //PSOChatLog.batファイルの生成
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    var strPowerShell = "";
                    strPowerShell += "@powershell -NoProfile -ExecutionPolicy Unrestricted \"&([ScriptBlock]::Create((cat -encoding utf8 \\\"%~f0\\\" | ? {$_.ReadCount -gt 2}) -join \\\"`n\\\"))\" %*\r\n";
                    strPowerShell += "@exit /b\r\n";
                    strPowerShell += "\r\n";
                    strPowerShell += "add-type -AssemblyName System.Windows.Forms\r\n";
                    strPowerShell += "add-type -AssemblyName Microsoft.VisualBasic\r\n";
                    strPowerShell += "\r\n";
                    strPowerShell += "function activate_window([string]$title)\r\n";
                    strPowerShell += "{\r\n";
                    strPowerShell += "  [Microsoft.VisualBasic.Interaction]::AppActivate($title)\r\n";
                    strPowerShell += "}\r\n";
                    strPowerShell += "function send([string]$key)\r\n";
                    strPowerShell += "{\r\n";
                    strPowerShell += "[System.Windows.Forms.SendKeys]::SendWait($key)\r\n";
                    strPowerShell += "  sleep 0.5\r\n";
                    strPowerShell += "}\r\n";
                    strPowerShell += "\r\n";
                    strPowerShell += "chcp\r\n";
                    strPowerShell += "\r\n";
                    strPowerShell += "activate_window \"Ephinea: Phantasy Star Online Blue Burst\"\r\n";
                    strPowerShell += "sleep 1\r\n";
                    strPowerShell += "send \"" + strImput + "\"\r\n";

                    File.WriteAllText(strExecPath + strEnvironment + ".bat", strPowerShell);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Thread.Sleep(500);
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //cmd.exeにPSOChatLog.batファイルを実行させる
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //string program = "cmd.exe";
                    var param1 = strExecPath + strEnvironment + "_call" + ".bat";
                    string param = param1;
                    //プログラムを実行し、戻ってきた結果を出力
                    //var result = "";
                    CallBatchFile(param, "");
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    Thread.Sleep(500);
                }
                while (false);//空ループなので、必ず一回で処理を抜ける
            }
            var PSOBBServer = "";
            if (strSendTextMode.Equals("MemorySharp") == true)
            {
                do//空ループ
                {
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //文字列が入っていなかったら終了
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    if (string.IsNullOrWhiteSpace(strImput))
                    {
                        //Console.WriteLine("IsNullOrWhiteSpace");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    //MemorySharp.dllがインストールされているかチェック
                    //C:\Program Files\AutoHotkey\AutoHotkey.exe
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    string filePath = strExecPath + "MemorySharp.dll";
                    if (File.Exists(filePath) == false)
                    {
                        //Console.WriteLine("MemorySharp.dll is not found");
                        ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "can not found MemorySharp.dll");
                        break;
                    }
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    do
                    {
                        //ウィンドウのタイトルに「Ephinea: Phantasy Star Online Blue Burst」を含むプロセスをすべて取得する
                        Process[] ps = GetProcessesByWindowTitle("Ephinea: Phantasy Star Online Blue Burst");

                        if (ps.Length == 0)
                        {
                            break;
                        }
                        PSOBBServer = "Ephinea";
                        //Process[] ps = GetProcessesByWindowTitle("记事本");
                        //アクティブウィンドウにする。
                        foreach (Process pPSOBB in ps)
                        {
                            Microsoft.VisualBasic.Interaction.AppActivate(pPSOBB.MainWindowTitle);
                        }

                        WriteChat("Ephinea: Phantasy Star Online Blue Burst", strImput);
                        //WriteChat("psobb.exe", strImput);
                    } while (false);
                    if (PSOBBServer.Equals(""))
                    {
                        {
                            //ウィンドウのタイトルに「PHANTASY STAR ONLINE Blue Burst」を含むプロセスをすべて取得する
                            Process[] ps = GetProcessesByWindowTitle("PHANTASY STAR ONLINE Blue Burst");

                            if (ps.Length == 0)
                            {
                                break;
                            }
                            PSOBBServer = "UnConfirmed";
                            //アクティブウィンドウにする。
                            foreach (Process pPSOBB in ps)
                            {
                                Microsoft.VisualBasic.Interaction.AppActivate(pPSOBB.MainWindowTitle);
                            }

                            WriteChat("PHANTASY STAR ONLINE Blue Burst", strImput);
                        } while (false) ;
                    }
                }
                while (false);//空ループなので、必ず一回で処理を抜ける
            }
        }
        private void SendTextShortTextRegistration(int iSTRNum)
        {
            var strRet = "";
            do
            {
                for (int i = 0; i < stShortTextRegistration.Count - 1; i++)
                {
                    if (stShortTextRegistration[i].iNum == iSTRNum)
                    {
                        strRet = stShortTextRegistration[i].ShortText;
                        break;
                    }
                }
                if (strRet.Equals(""))   
                {
                    break;
                }
                SendTextChatLogToPSOBB(strRet);
            } while (false);
        }
        private void WriteChat(string processName, string text)
        {
            //stop();
            //text = gfGetCharacterLimit(text, 56);
            text = gfGetCharacterLimit(text, 70);//1バイトキャラのみの場合、最大で73バイト
            //text = gfGetCharacterLimit(text, 90);//2バイトキャラのみの場合、最大で55文字110バイト
            //text += '\0';
            text = "\tE" + text + '\0';
            Process[] ps = GetProcessesByWindowTitle(processName);
            MemorySharp ms = new(ps[0]);

            IntPtr address = ms.Read<IntPtr>(new IntPtr(0x00A98478), false);
            IntPtr addressChat = ms.Read<IntPtr>(address + 0x10, false);

            do
            {
                if (address == IntPtr.Zero)
                {
                    break;
                }
                if (addressChat == IntPtr.Zero)
                {
                    break;
                }
                ushort uiState = ms.Read<ushort>(addressChat + 0x0000001E, false);
                if (uiState == 66 || uiState == 67 || uiState == 70)
                {
                }
                else
                {
                    break;
                }

                try
                {
                    ms.WriteString(address + 0x000000A4, text, Encoding.Unicode, false);
                }
                catch
                {
                    MessageBox.Show("MemorySharp error code 01");
                }

                try
                {
                    uiState = 82;
                    ms.Write(addressChat + 0x0000001E, uiState, false);
                }
                catch
                {
                    MessageBox.Show("MemorySharp error code 02");
                }
            }
            while (false);
        }
        private string GetFileNameFullPathToPathName(string strFullPath)
        {
            var strExecPath = "";
            var strTmp = "";
            var bFlg = false;

            for (int i = strFullPath.Length - 1; 0 <= i; i--)
            {
                strTmp = strFullPath.Substring(i, 1);
                if (Regex.IsMatch(strTmp, "\\\\", RegexOptions.Singleline))
                {
                    bFlg = true;
                }
                //最後の\から、最初までを抜き出す
                if (bFlg == true)
                {
                    strExecPath = strTmp + strExecPath;
                }
            }
            return strExecPath;
        }
        private bool GetFileNameFileNameToDateTime(string strFileName, ref string strYYYY, ref string strMM, ref string strDD)
        {
            var strTmp = "";
            var strNum = "";
            var myCheck = false;
            var bBreakFlg = false;
            for (int i = 0; i < strFileName.Length; i++)
            {
                strTmp = strFileName.Substring(i, 1);
                myCheck = Regex.IsMatch(strTmp, "[0-9]", RegexOptions.Singleline);
                if (myCheck == true)
                {
                    strNum = strNum + strTmp;
                }
                while (false) ;
            }
            do
            {
                if (strNum.Length != 8)
                {
                    bBreakFlg = true;
                    break;
                }
                strYYYY = strNum.Substring(0, 4);
                strMM = strNum.Substring(4, 2);
                strDD = strNum.Substring(6, 2);
            }
            while (false);
            if (bBreakFlg == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private string gfGetCharacterLimit(string strData, int iLimit)
        {
            var strTmp = "";
            var numCount = 0;
            var strRet = "";
            do
            {
                for (int i = 0; i < strData.Length; i++)
                {
                    strTmp = strData.Substring(i, 1);
                    if (isOneByteChar(strTmp) == false)
                    {
                        numCount += 2;
                    }
                    else
                    {
                        numCount += 1;
                    }
                    if (numCount > iLimit)
                    {
                        break;
                    }
                    strRet += strTmp;
                }
            }
            while (false);
            return strRet;
        }
        private bool isOneByteChar(string strData)
        {
            var bRet = false;
            byte[] byte_data = System.Text.Encoding.GetEncoding(932).GetBytes(strData);
            if (byte_data.Length == strData.Length)
            {
                bRet = true;
            }
            return bRet;
        }
        private char gfGetChar(string strData)
        {
            byte[] byte_data = System.Text.Encoding.GetEncoding(932).GetBytes(strData);
            return (char)byte_data[0];
        }
        private bool CallPythonExistenceCheck(DateTime dtmAddLogDate, string strLogType)
        {
            var result = "";
            var program = "";
            var param = "-V";
            var myCheck = false;
            foreach (string line in CallPythonFile(program, true, dtmAddLogDate, strLogType, param))
            {
                result += line;
            }
            //MessageBox.Show(result);
            myCheck = Regex.IsMatch(result, "Python 3.*", RegexOptions.Singleline);
            return myCheck;
        }
        public IEnumerable<string> CallPythonFile(string program, bool bCheck, DateTime dtmAddLogDate, string strLogType, string args = "")
        {


            //環境変数にて、Pythonの標準コードページをUTF-8に指定(Python 3.7以降で有効)
            Environment.SetEnvironmentVariable("PYTHONUTF8", "1");

            ProcessStartInfo psInfo = new ProcessStartInfo();
            Process p = new Process();
            //コードページをUTF-8に指定
            psInfo.StandardOutputEncoding = Encoding.UTF8;

            // エラー出力をリダイレクトする
            psInfo.RedirectStandardError = true;

            // 標準入力をリダイレクトする
            psInfo.RedirectStandardInput = true;

            // 標準出力をリダイレクトする
            psInfo.RedirectStandardOutput = true;

            // コンソール・ウィンドウを開かない
            psInfo.CreateNoWindow = true;

            // シェル機能を使用しない
            psInfo.UseShellExecute = false;

            // 実行するファイルをセット
            psInfo.FileName = "Python";

            //引数があればセット
            //psInfo.Arguments = (args.Equals("")) ? "" : string.Format("{0} {1}", program, args);
            psInfo.Arguments = (args.Equals("") ? "" : string.Format("{0} {1}", program, args));

            // プロセスを開始
            try
            {
                p = Process.Start(psInfo);
            }
            catch
            {
                psInfo.FileName = "cmd";
                p = Process.Start(psInfo);
            }
            // アプリのコンソール出力結果を全て受け取る
            string lineErr;
            string lineOutput;

            lineErr = p.StandardError.ReadToEnd();
            if (string.IsNullOrEmpty(lineErr) == false)
            {
                if (bCheck == false)
                {
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Python Call Error\t\t" + "\"" + lineErr + "\"");
                }
                //Console.WriteLine = lineErr;
            }

            while ((lineOutput = p.StandardOutput.ReadLine()) != null)
            {
                yield return lineOutput + Environment.NewLine;
            }
        }
        public IEnumerable<string> CallCommandLine(string program, string args = "")
        {
            ProcessStartInfo psInfo = new ProcessStartInfo();
            Process p = new Process();
            //コードページをUTF-8に指定
            psInfo.StandardOutputEncoding = Encoding.UTF8;

            // エラー出力をリダイレクトする
            psInfo.RedirectStandardError = true;

            // 標準入力をリダイレクトする
            psInfo.RedirectStandardInput = true;

            // 標準出力をリダイレクトする
            psInfo.RedirectStandardOutput = true;

            // コンソール・ウィンドウを開かない
            psInfo.CreateNoWindow = false;

            // シェル機能を使用しない
            psInfo.UseShellExecute = false;

            // 実行するファイルをセット
            psInfo.FileName = "cmd.exe";

            //引数があればセット
            //psInfo.Arguments = (args.Equals("")) ? "" : string.Format("{0} {1}", program, args);
            psInfo.Arguments = (args.Equals("") ? "" : string.Format("{0} {1}", program, args));

            // プロセスを開始
            try
            {
                p = Process.Start(psInfo);
            }
            catch
            {
                psInfo.FileName = "cmd";
                p = Process.Start(psInfo);
            }
            // アプリのコンソール出力結果を全て受け取る
            //string lineErr;
            string lineOutput;

            while ((lineOutput = p.StandardOutput.ReadLine()) != null)
            {
                yield return lineOutput + Environment.NewLine;
            }
        }
        private int CallBatchFile(string strFileName, string param)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.Arguments = string.Format(@"/c " + strFileName + " {0}", param);
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            return process.ExitCode;
        }
        public IEnumerable<string> GetPythonEnvironmentVariable()
        {
            //環境変数にて、Pythonの標準コードページをUTF-8に指定(Python 3.7以降で有効)
            Environment.SetEnvironmentVariable("PYTHONUTF8", "1");

            ProcessStartInfo psInfo = new ProcessStartInfo();
            Process p = new Process();
            //コードページをUTF-8に指定
            psInfo.StandardOutputEncoding = Encoding.UTF8;

            // エラー出力をリダイレクトする
            psInfo.RedirectStandardError = true;

            // 標準入力をリダイレクトする
            psInfo.RedirectStandardInput = true;

            // 標準出力をリダイレクトする
            psInfo.RedirectStandardOutput = true;

            // コンソール・ウィンドウを開かない
            psInfo.CreateNoWindow = true;

            // シェル機能を使用しない
            psInfo.UseShellExecute = false;

            // 実行するファイルをセット
            psInfo.FileName = "chcp";

            //引数があればセット
            //psInfo.Arguments = (args.Equals("")) ? "" : string.Format("{0} {1}", program, args);
            //psInfo.Arguments = (args.Equals("") ? "" : string.Format("{0} {1}", program, args));

            // プロセスを開始
            try
            {
                p = Process.Start(psInfo);
            }
            catch
            {
                psInfo.FileName = "cmd";
                p = Process.Start(psInfo);
            }
            // アプリのコンソール出力結果を全て受け取る
            string lineErr;
            string lineOutput;

            lineErr = p.StandardError.ReadToEnd();
            if (string.IsNullOrEmpty(lineErr) == false)
            {
                //if (bCheck == false)
                //{
                //    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Python Call Error\t\t" + "\"" + lineErr + "\"");
                //}
                //Console.WriteLine = lineErr;
            }

            while ((lineOutput = p.StandardOutput.ReadLine()) != null)
            {
                yield return lineOutput + Environment.NewLine;
            }
        }
        public void GetListBoxRow(string strRow, ref string strALL, ref string strID, ref string strName, ref string strText)
        {
            var strTmp = "";
            var numColumn = 0;
            var strText1 = "";
            var strText2 = "";

            strALL = listBox1.Items[Int32.Parse(strRow)].ToString();
            for (int i = 0; i < strALL.Length; i++)
            {
                strTmp = strALL.Substring(i, 1);
                do
                {
                    if (strTmp.Equals("\t"))
                    {
                        numColumn++;
                        break;
                    }
                    if (numColumn == 0)
                    {
                        strID = strID + strTmp;
                        break;
                    }
                    if (numColumn == 1)
                    {
                        strName = strName + strTmp;
                        break;
                    }
                    if (numColumn == 2)
                    {
                        strText1 = strText1 + strTmp;
                        break;
                    }
                    if (numColumn == 3)
                    {
                        strText2 = strText2 + strTmp;
                        break;
                    }
                }
                while (false);
            }
            if (String.IsNullOrEmpty(strText2))
            {
                strText = strText1;
            }
            else
            {
                strText = strText2;
            }
        }
        public void GetListViewRow(string strRow, ref string strALL, ref string strID, ref string strName, ref string strText)
        {
            for (var j = 0; j < listView1.Items[Int32.Parse(strRow)].SubItems.Count - 1; j++)
            {
                Console.WriteLine(j + "\t" + listView1.Items[Int32.Parse(strRow)].SubItems[j].Text);
            }
            try
            {
                strID = listView1.Items[Int32.Parse(strRow)].SubItems[9].Text;
            }
            catch
            {
                strID = "";
            }
            try
            {
                strName = listView1.Items[Int32.Parse(strRow)].SubItems[10].Text;
            }
            catch
            {
                strName = "";
            }
            try
            {
                strText = listView1.Items[Int32.Parse(strRow)].SubItems[11].Text;
            }
            catch
            {
                strText = "";
            }
            strALL = strID + "\t" + strName + "\t" + strText;
            for (var i = 0; i < listView1.Items.Count - 1; i++)
            {
                for (var j = 0; j < listView1.Items[i].SubItems.Count - 1; j++)
                {
                    Console.WriteLine(listView1.Items[i].SubItems[j].Text);
                }
            }
            //<listview>
            //listView1.Columns.Add("翻訳", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("数", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("日付", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("原文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("訳文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("時刻", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("種別", 40, HorizontalAlignment.Left);
            //listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("名前", 120, HorizontalAlignment.Left);
            //listView1.Columns.Add("文章", 1000, HorizontalAlignment.Left);
        }
        public void gfChangeSelectListBox(string strUpDown, ref string strSelectedIndices)
        {
            //stop();
            var strRet = "";
            do
            {
                if (listBox1.SelectedIndices.Count == 0)
                {
                    break;
                }
                if (strUpDown.Equals("Up") && listBox1.SelectedIndices[0] == 0)
                {
                    //モードがUpかつ先頭行だったら無視
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Down") && listBox1.SelectedIndices[0] == listBox1.Items.Count - 1)
                {
                    //モードがDownかつ最終行だったら無視
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("UpMax"))
                {
                    listBox1.SetSelected(0, true);
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Up"))
                {
                    listBox1.SetSelected(listBox1.SelectedIndices[0] - 1, true);
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Down"))
                {
                    listBox1.SetSelected(listBox1.SelectedIndices[0] + 1, true);
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("DownMax"))
                {
                    listBox1.SetSelected(listBox1.Items.Count - 1, true);
                    strRet = listBox1.SelectedIndices[0].ToString();
                    break;
                }
            }
            while (false);
            strSelectedIndices = strRet;
        }
        public void gfChangeSelectListView(string strUpDown, ref string strSelectedIndices)
        {
            //stop();
            var strRet = "";
            do
            {
                if (listView1.SelectedIndices.Count == -1)
                {
                    break;
                }
                if (listView1.SelectedIndices.Count == 0)
                {
                    break;
                }
                if (strUpDown.Equals("Up") && listView1.SelectedIndices[0] == 0)
                {
                    //モードがUpかつ先頭行だったら無視
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Down") && listView1.SelectedIndices[0] == listView1.Items.Count - 1)
                {
                    //モードがDownかつ最終行だったら無視
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("UpMax"))
                {
                    listView1.Items[0].Selected = true;
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Up"))
                {
                    listView1.Items[listView1.SelectedItems[0].Index - 1].Selected = true;
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("Down"))
                {
                    listView1.Items[listView1.SelectedItems[0].Index + 1].Selected = true;
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
                if (strUpDown.Equals("DownMax"))
                {
                    listView1.Items[listView1.Items.Count - 1].Selected = true;
                    strRet = listView1.SelectedIndices[0].ToString();
                    break;
                }
            }
            while (false);
            strSelectedIndices = strRet;
        }
        private Boolean CheckOpenForms(string strCheckFormName)
        {
            var bRet = false;
            for (int i = 0; i < System.Windows.Forms.Application.OpenForms.Count; i++)
            {
                if (System.Windows.Forms.Application.OpenForms[i].Name == strCheckFormName)
                {
                    bRet = true;
                    break;
                }
            }
            return bRet;
        }
        private void FormsActivate(string strActivateFormName)
        {
            for (int i = 0; i < System.Windows.Forms.Application.OpenForms.Count; i++)
            {
                if (System.Windows.Forms.Application.OpenForms[i].Name == strActivateFormName)
                {
                    System.Windows.Forms.Application.OpenForms[i].Activate();
                    break;
                }
            }
        }
        private string gfGetChatLang()
        {
            var strReturn = "";
            do
            {
                if (comboBox2.Text.Equals("English"))
                {
                    strReturn = "EN";
                    break;
                }
                if (comboBox2.Text.Equals("Deutsch"))
                {
                    strReturn = "DE";
                    break;
                }
                if (comboBox2.Text.Equals("español"))
                {
                    strReturn = "ES";
                    break;
                }
                if (comboBox2.Text.Equals("français"))
                {
                    strReturn = "FR";
                    break;
                }
                if (comboBox2.Text.Equals("русский язык"))
                {
                    strReturn = "RU";
                    break;
                }
                if (comboBox2.Text.Equals("Cipher"))
                {
                    strReturn = "RU";
                    break;
                }
                if (comboBox2.Text.Equals("簡体字"))
                {
                    strReturn = "ZH-CN";
                    break;
                }
                if (comboBox2.Text.Equals("繁体字"))
                {
                    strReturn = "ZH-TW";
                    break;
                }
                if (comboBox2.Text.Equals("日本語"))
                {
                    strReturn = "JA";
                    break;
                }
                if (comboBox2.Text.Equals("조선어"))
                {
                    strReturn = "KO";
                    break;
                }
            }
            while (false);
            return strReturn;
        }

        public System.Diagnostics.Process[] GetProcessesByWindowTitle(string windowTitle)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            //すべてのプロセスを列挙する
            foreach (System.Diagnostics.Process p
                in System.Diagnostics.Process.GetProcesses())
            {
                //指定された文字列がメインウィンドウのタイトルに含まれているか調べる
                if (0 <= p.MainWindowTitle.IndexOf(windowTitle))
                {
                    //含まれていたら、コレクションに追加
                    list.Add(p);
                }
            }

            //コレクションを配列にして返す
            return (System.Diagnostics.Process[])
                list.ToArray(typeof(System.Diagnostics.Process));
        }

        private void frmMain_Scroll(object sender, ScrollEventArgs e)
        {
            ListViewScrollCheckBottom(listView1);
            ListBoxScrollCheckBottom(listBox1);
        }


        //************************************************************************************************************************************************************************************************
        //翻訳を含む様々なログをリストボックスに表示
        //************************************************************************************************************************************************************************************************
        private void ListboxDisplayUpdateTranceAsync(string strIndexNo, string strTranslator, string strLogType, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strGuildChatSoundCheck = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundCheck", strIniFileName);
            var strGuildChatSoundFile = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundFile", strIniFileName);

            var strLangTarget = Language_GP.myLanguage_GP();
            var ListboxTopIndex = listBox1.TopIndex;
            var strEmoji = "";
            var i = 0;
            //var listBoxScrollCheckBottom = ListBoxScrollCheckBottom(listBox1);
            var listboxtopindex = 0;
            //stop();
            for (i = listBox1.Items.Count - 1; i >= 0; i--)
            {
                strEmoji = "";
                do
                {
                    if (strLogType.Equals("Normal") == true)
                    {
                        strEmoji = "🗣";
                        break;
                    }
                    if (strLogType.Equals("Addons") == true)
                    {
                        strEmoji = "🗣";
                        break;
                    }
                    if (strLogType.Equals("Guild") == true)
                    {
                        strEmoji = "🧙‍";
                        break;
                    }
                    if (strLogType.Equals("SimpleMail") == true)
                    {
                        strEmoji = "📩";
                        break;
                    }
                } while (false);
                var strLineText = listBox1.Items[i].ToString();
                //if (strLineText == "Waiting\t\t>\t" + strIndexNo)
                if (Regex.IsMatch(strLineText, "Waiting.*" + strIndexNo, RegexOptions.Singleline) == true)
                {
                    if (strTranslator.Equals("Trans DeepL") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑DeepL Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑DeepL Trans", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑DeepL 翻訳\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑DeepL 翻訳", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑DeepL Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑DeepL Trans", strEmoji, strAfterTranslation);
                        } while (false);
                        break;
                    }
                    if (strTranslator.Equals("Trans DeepL U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑DeepL Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑DeepL Trans U", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑DeepL 翻訳 U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑DeepL 翻訳 U", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑DeepL Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑DeepL Trans U", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }
                    if (strTranslator.Equals("Trans Google") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Google Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Google Trans", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Google 翻訳\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Google 翻訳", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑Google Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "\"↑Google Trans", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }
                    if (strTranslator.Equals("Trans Baidu") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Baidu Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Baidu Trans", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Baidu 翻訳\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Baidu 翻訳", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑Baidu Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑Baidu Trans", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }
                    if (strTranslator.Equals("Trans Baidu U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Baidu Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Baidu Trans U", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑Baidu 翻訳 U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑Baidu 翻訳 U", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑Baidu Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑Baidu Trans U", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }

                    //stop();

                    if (strTranslator.Equals("Trans TexTra") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑TexTra Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑TexTra Trans", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑TexTra 翻訳\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑TexTra 翻訳", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑TexTra Trans\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑TexTra Trans", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }
                    if (strTranslator.Equals("Trans TexTra U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑TexTra Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑TexTra Trans U", strEmoji, strAfterTranslation);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listboxtopindex = listBox1.TopIndex;
                                listBox1.Items[i] = ("↑TexTra 翻訳 U\t" + strEmoji + ">\t" + strAfterTranslation);
                                listBox1.TopIndex = listboxtopindex;
                                listViewItemsEditAsync(strIndexNo, "↑TexTra 翻訳 U", strEmoji, strAfterTranslation);
                                break;
                            }
                            listboxtopindex = listBox1.TopIndex;
                            listBox1.Items[i] = ("↑TexTra Trans U\t" + strEmoji + ">\t" + strAfterTranslation);
                            listBox1.TopIndex = listboxtopindex;
                            listViewItemsEditAsync(strIndexNo, "↑TexTra Trans U", strEmoji, strAfterTranslation);
                        }
                        while (false);
                        break;
                    }
                    break;
                }
            }
            //if (listBoxScrollCheckBottom == true)
            //{
            //    listBox1.TopIndex = ListboxTopIndex;
            //}
        }
        private void ListboxDisplayUpdate()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strGuildChatSoundCheck = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundCheck", strIniFileName);
            var strGuildChatSoundFile = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "GuildChatSoundFile", strIniFileName);
            var strLangTarget = Language_GP.myLanguage_GP();
            var strEmoji = "";
            var listBoxScrollCheckBottom = ListBoxScrollCheckBottom(listBox1);
            for (int i = 0; i <= ClsListBoxPreWriteBuffer.numMax; i++)
            {
                var strLogType = ClsListBoxPreWriteBuffer.strLogType[i];
                strEmoji = "";
                do
                {
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("HBR Counts") == true)
                    {
                        strEmoji = "💯";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("Material") == true)
                    {
                        strEmoji = "🧋";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("Welcome") == true)
                    {
                        strEmoji = "😀";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("RareDrop") == true)
                    {
                        strEmoji = "📦";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("Lv200") == true)
                    {
                        strEmoji = "🎉";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("Date changed") == true)
                    {
                        strEmoji = "🌞";
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMemberID[i].Equals("Anguish") == true)
                    {
                        strEmoji = "😧";
                        break;
                    }
                    if (strLogType.Equals("Normal") == true)
                    {
                        strEmoji = "🗣";
                        break;
                    }
                    if (strLogType.Equals("Addons") == true)
                    {
                        strEmoji = "🗣";
                        break;
                    }
                    if (strLogType.Equals("Guild") == true)
                    {
                        strEmoji = "🧙‍";
                        break;
                    }
                    if (strLogType.Equals("SimpleMail") == true)
                    {
                        strEmoji = "📩";
                        break;
                    }
                } while (false);
                do
                {
                    if (ClsListBoxPreWriteBuffer.numListBox[i] != 0)//既に表示されていたら次の行へ
                    {
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Waiting") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("Waiting\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "Waiting", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("Waiting\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "Waiting", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("System\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "System", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("SysMsg") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("System\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "System", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("システム\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "システム", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("System\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "System", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("ErrMsg") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("error\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "error", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("エラー\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "エラー", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("error\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "error", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Dic") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑Dic\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Dic", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑辞書\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑辞書", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑Dic\t\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑Dic", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans DeepL") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑DeepL Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑DeepL Trans", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑DeepL 翻訳\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑DeepL 翻訳", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑DeepL Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑DeepL Trans", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans DeepL U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑DeepL Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑DeepL Trans U", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑DeepL 翻訳 U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑DeepL 翻訳 U", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑DeepL Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑DeepL Trans U", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans Google") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑Google Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Google Trans", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑Google 翻訳\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Google 翻訳", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑Google Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑Google Trans", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans Baidu") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑Baidu Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Baidu Trans", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑Baidu 翻訳\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Baidu 翻訳", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑Baidu Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑Baidu Trans", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans Baidu U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑Baidu Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Baidu Trans U", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑Baidu 翻訳 U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑Baidu 翻訳 U", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑Baidu Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑Baidu Trans U", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    //stop();
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans TexTra") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑TexTra Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑TexTra Trans", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑TexTra 翻訳\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑TexTra 翻訳", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑TexTra Trans\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑TexTra Trans", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strMsgType[i].Equals("Trans TexTra U") == true)
                    {
                        do
                        {
                            if (strLangTarget.Equals("EN"))
                            {
                                listBox1.Items.Add("↑TexTra Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑TexTra Trans U", strEmoji);
                                break;
                            }
                            if (strLangTarget.Equals("JA"))
                            {
                                listBox1.Items.Add("↑TexTra 翻訳 U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                                listViewItemsAdd(i, "↑TexTra 翻訳 U", strEmoji);
                                break;
                            }
                            listBox1.Items.Add("↑TexTra Trans U\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                            listViewItemsAdd(i, "↑TexTra Trans U", strEmoji);
                        }
                        while (false);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    if (ClsListBoxPreWriteBuffer.strLogType[i].Equals("Guild") == true)
                    {
                        //ギルチャの時は、ファイルが存在すれば、音を鳴らす。
                        var strWavFile = strGuildChatSoundFile;
                        do
                        {
                            if (strGuildChatSoundCheck.Equals("false"))
                            {
                                break;
                            }
                            if (File.Exists(strWavFile) == false)
                            {
                                break;
                            }
                            var player = new System.Media.SoundPlayer(strWavFile);
                            //非同期再生する
                            player.Play();
                        }
                        while (false);
                        listBox1.Items.Add("Guild" + "\t" + ClsListBoxPreWriteBuffer.strMemberName[i] + "\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                        listViewItemsAdd(i, "Guild", strEmoji);
                        ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                        break;
                    }
                    //listBox1.Items.Add(ClsListBoxPreWriteBuffer.strDateTime[i].Trim() + "\t" + ClsListBoxPreWriteBuffer.strMemberID[i].Trim() + "\t" + ClsListBoxPreWriteBuffer.strMemberName[i] + "\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                    listBox1.Items.Add(ClsListBoxPreWriteBuffer.strMemberID[i].Trim() + "\t" + ClsListBoxPreWriteBuffer.strMemberName[i] + "\t" + strEmoji + ">\t" + ClsListBoxPreWriteBuffer.strMessage[i]);
                    listViewItemsAdd(i, "Chat", strEmoji);
                    ClsListBoxPreWriteBuffer.numListBox[i] = listBox1.Items.Count;
                }
                while (false);
            }
            if (listBoxScrollCheckBottom == true)
            {
                listBox1.TopIndex++;
            }
        }
        private bool ListBoxScrollCheckBottom(System.Windows.Forms.ListBox listBox)
        {
            var Ret = false;
            var strBottom = "";
            int listBoxItem = 0;
            listBoxItem = ListBoxItemGetBottom(listBox1);
            int LastItem = 0;
            do
            {
                if (listBox1 == null)
                {
                    break;
                }
                if (listBox1.Items.Count == 0)
                {
                    break;
                }
                //LastItem = listBox1.Items[listBox1.Items.Count - 1];
                LastItem = listBox1.Items.Count - 1;
                if (listBoxItem != LastItem)
                {
                    break;
                }
                Ret = true;
                strBottom = "最終行です";

            } while (false);
            //this.label3.Text = strBottom;
            return Ret;
        }
        public int ListBoxItemGetBottom(System.Windows.Forms.ListBox listBox)
        {
            int i = 0;
            var Ret = 0;
            var TopPixelCurrrentRow = 0;
            int offset = 0;
            offset = 12;
            var LoopCount = 0;
            do
            {
                if (listBox.Items.Count == 0)
                {
                    Ret = 0;
                    break;
                }
                if (listBox.Items.Count <= LoopCount)
                {
                    Ret = LoopCount;
                    break;
                }
                TopPixelCurrrentRow = TopPixelCurrrentRow + listBox.GetItemHeight(LoopCount);
                if (TopPixelCurrrentRow + offset > listBox.ClientSize.Height)
                {
                    Ret = LoopCount + listBox.TopIndex;
                    if (Ret > (listBox.Items.Count - 1))
                    {
                        Ret = listBox.Items.Count - 1;
                    }
                    break;
                }
                LoopCount++;
            } while (true);
            return Ret;
        }
        //************************************************************************************************************************************************************************************************
        //翻訳を含む様々なログをリストボックスに表示終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //翻訳を含む様々なログをリストビューに表示開始
        //************************************************************************************************************************************************************************************************
        private void listViewItemsAdd(long i, string strView, string strEmoji)
        {
            bool ScrollListViewFlg = ListViewScrollCheckBottom(listView1);
            do
            {
                //<listview>
                //listView1.Columns.Add("翻訳", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("数", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("日付", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("原文", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("訳文", 0, HorizontalAlignment.Left);
                //listView1.Columns.Add("時刻", 60, HorizontalAlignment.Left);
                //listView1.Columns.Add("種別", 40, HorizontalAlignment.Left);
                //listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
                //listView1.Columns.Add("名前", 120, HorizontalAlignment.Left);
                //listView1.Columns.Add("文章", 1000, HorizontalAlignment.Left);

                //<listview>
                string[] strListViewAddText =
                {
                ClsListBoxPreWriteBuffer.bDoTranslationFlg[i].ToString()
                , ClsListBoxPreWriteBuffer.numListBox[i].ToString()
                , ClsListBoxPreWriteBuffer.strLogType[i]
                , ClsListBoxPreWriteBuffer.strMsgType[i]
                , ClsListBoxPreWriteBuffer.dtmLogDate[i].ToString()
                , ClsListBoxPreWriteBuffer.strOriginalMessage[i]
                , ClsListBoxPreWriteBuffer.strTranslatedMessage[i]
                , ClsListBoxPreWriteBuffer.strDateTime[i]
                , strView + " "+ strEmoji
                , ClsListBoxPreWriteBuffer.strMemberID[i]
                , ClsListBoxPreWriteBuffer.strMemberName[i]
                , ClsListBoxPreWriteBuffer.strMessage[i]
                };
                listView1.Items.Add(new ListViewItem(strListViewAddText));


                if (ScrollListViewFlg == false)
                {
                    break;
                }
                listView1.EnsureVisible(listView1.Items.Count - 1);
            } while (false);
        }
        private bool ListViewScrollCheckBottom(System.Windows.Forms.ListView listView)
        {
            var Ret = false;
            var strBottom = "";
            ListViewItem LastViewItem = null;
            LastViewItem = ListViewItemGetBottom(listView1);
            ListViewItem LastItem = null;
            //stop();
            do
            {
                if (listView1 == null)
                {
                    break;
                }
                if (listView1.Items.Count == 0)
                {
                    break;
                }
                LastItem = listView1.Items[listView1.Items.Count - 1];
                if (LastViewItem == null)
                {
                    break;
                }
                if (LastViewItem.Index != LastItem.Index)
                {
                    break;
                }
                Ret = true;
                strBottom = "最終行です";

            } while (false);
            //this.label3.Text = strBottom;
            return Ret;
        }
        public static ListViewItem ListViewItemGetBottom(System.Windows.Forms.ListView listView)
        {
            if (listView.TopItem == null)
            {
                return null;
                //return listView.Items[0];
            }
            for (int i = listView.TopItem.Index; i < listView.Items.Count; i++)
            {
                ListViewItem testItem = listView.Items[i];

                // アイテムの位置がリストビューの高さ以上の場合（完全に表示されているもの）
                if (listView.LargeImageList != null && testItem.Position.Y + listView.LargeImageList.ImageSize.Height > listView.ClientSize.Height ||
                    listView.LargeImageList == null && testItem.Position.Y + 16 > listView.ClientSize.Height)
                {
                    // 表示されていない場合（ヘッダのみとか1行目が途中までしか表示されていないとか）
                    if (i == 0)
                    {
                        return listView.Items[0];
                    }
                    // 表示されている場合
                    else
                    {
                        int lastVisibleIndex = i - 1;

                        return listView.Items[lastVisibleIndex];
                    }
                }
            }

            return listView.Items.OfType<ListViewItem>().LastOrDefault();
        }
        private void listViewItemsEditAsync(string strIndexNo, string strView, string strEmoji, string strTranslatedText)
        {
            //for (int j = 0; j < listView1.Items.Count; j++)
            for (int j = listView1.Items.Count - 1; j >= 0; j--)
            {
                if (listView1.Items[j].SubItems[11].Text == strIndexNo)
                {
                    listView1.Items[j].SubItems[8].Text = strView;
                    listView1.Items[j].SubItems[11].Text = strTranslatedText;
                    break;
                    //<listview>
                }
            }
            ////<listview>
            //listView1.Columns.Add("翻訳", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("数", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("LogType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("MsgType", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("日付", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("原文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("訳文", 0, HorizontalAlignment.Left);
            //listView1.Columns.Add("時刻", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("種別", 40, HorizontalAlignment.Left);
            //listView1.Columns.Add("ID", 60, HorizontalAlignment.Left);
            //listView1.Columns.Add("名前", 120, HorizontalAlignment.Left);
            //listView1.Columns.Add("文章", 1000, HorizontalAlignment.Left);
        }
        //************************************************************************************************************************************************************************************************
        //翻訳を含む様々なログをリストビューに表示終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //タイマーで待ちnormalログとaddonsログが揃った頃に翻訳
        //************************************************************************************************************************************************************************************************
        private void timer1_Tick(object sender, EventArgs e)
        {
            var strLastLine = "";
            var strMember = "";
            var strMemberID = "";
            var strDateTime = "";
            var strSource = "";
            var bBreakFlg = false;
            var strMemberLog = "";
            var strAfterSlang = "";
            var strAfterTranslation = "";
            var bHitFlg = false;
            var strTranslator = "";
            var numMyWaitNo = 0;
            var strFileName = "";
            var bDoTranslationFlg = false;

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);
            timer1.Stop();
            //ListBoxScrollCheckBottom(listBox1);
            //ListViewScrollCheckBottom(listView1);
            do
            {
                if (automaticUpdate.AwaitCheckVerResult == null)
                {
                    break;
                }
                if (automaticUpdate.AwaitCheckVerResult.Equals(""))
                {
                    break;
                }
                if (decimal.Parse(Value.strPSOChatLogVersionCurrent) >= decimal.Parse(automaticUpdate.AwaitCheckVerResult))
                {
                    break;
                }
                this.button6.Visible = true;
                this.button6.Enabled = automaticUpdate.AwaitUpDateBTEnable;
                this.button6.Text = automaticUpdate.AwaitUpDateBTText;
            } while (false);


            ArrayClsWaitingToTranslateAddnew("", "Normal", "", true);
            ArrayClsWaitingToTranslateAddnew("", "Addons", "", true);
            ArrayClsWaitingToTranslateAddnew("", "Guild", "", true);
            ArrayClsWaitingToTranslateAddnew("", "SimpleMail", "", true);
            DateTime dtmAddLogDate;
            var strLogType = "";

            //MessageBox.Show("timer1");
            do//空ループ
            {
                //if (ClsWaitingToTranslate.bExclusiveFlg == true)
                //{
                //    //配列占有フラグが立っている＝書込不可
                //    break;
                //}

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //待ち行列をソートし、先頭から処理をする。(有効データを変形バブルソートする)
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //ClsWaitingToTranslate.bExclusiveFlg = true;

                for (var i = 0; i < MaxArray.numMaxClsWaitingToTranslate - 2; i++)
                {
                    // 下から上に順番に比較します
                    for (var j = MaxArray.numMaxClsWaitingToTranslate - 2; j > i; j--)
                    {
                        do
                        {
                            //比較データが無効の場合は抜ける
                            if (ClsWaitingToTranslate.numTaskNo[j] == 0)
                            {
                                break;
                            }
                            //上の方が大きいときは互いに入れ替えます
                            //if (ClsWaitingToTranslate.dtmLogDate[j] < ClsWaitingToTranslate.dtmLogDate[j - 1])
                            if (DateTime.Compare(ClsWaitingToTranslate.dtmLogDate[j], ClsWaitingToTranslate.dtmLogDate[j - 1]) < 0)
                            {
                                DateTime dtmTmpSort = ClsWaitingToTranslate.dtmLogDate[j];
                                int numTmpSort = ClsWaitingToTranslate.numTaskNo[j];
                                string strTmpTypeSort = ClsWaitingToTranslate.strLogType[j];
                                string strTmpLineSort = ClsWaitingToTranslate.strLogLine[j];
                                string strTmpDataOriginal = ClsWaitingToTranslate.strSource[j];
                                string strTmpFileName = ClsWaitingToTranslate.strFileName[j];

                                ClsWaitingToTranslate.dtmLogDate[j] = ClsWaitingToTranslate.dtmLogDate[j - 1];
                                ClsWaitingToTranslate.numTaskNo[j] = ClsWaitingToTranslate.numTaskNo[j - 1];
                                ClsWaitingToTranslate.strLogType[j] = ClsWaitingToTranslate.strLogType[j - 1];
                                ClsWaitingToTranslate.strLogLine[j] = ClsWaitingToTranslate.strLogLine[j - 1];
                                ClsWaitingToTranslate.strSource[j] = ClsWaitingToTranslate.strSource[j - 1];
                                ClsWaitingToTranslate.strFileName[j] = ClsWaitingToTranslate.strFileName[j - 1];

                                ClsWaitingToTranslate.dtmLogDate[j - 1] = dtmTmpSort;
                                ClsWaitingToTranslate.numTaskNo[j - 1] = numTmpSort;
                                ClsWaitingToTranslate.strLogType[j - 1] = strTmpTypeSort;
                                ClsWaitingToTranslate.strSource[j - 1] = strTmpDataOriginal;
                                ClsWaitingToTranslate.strLogLine[j - 1] = strTmpLineSort;
                                ClsWaitingToTranslate.strFileName[j - 1] = strTmpFileName;
                            }
                            //日付が一緒の場合はノーマルログとアドオンの比較に移る
                            if (DateTime.Compare(ClsWaitingToTranslate.dtmLogDate[j], ClsWaitingToTranslate.dtmLogDate[j - 1]) != 0)
                            {
                                break;
                            }
                            // アドオンの方が先の時は互いに入れ替えます
                            if (ClsWaitingToTranslate.strLogType[j].Equals("Normal") && ClsWaitingToTranslate.strLogType[j - 1].Equals("Addons"))
                            {
                                DateTime dtmTmpSort = ClsWaitingToTranslate.dtmLogDate[j];
                                int numTmpSort = ClsWaitingToTranslate.numTaskNo[j];
                                string strTmpTypeSort = ClsWaitingToTranslate.strLogType[j];
                                string strTmpSource = ClsWaitingToTranslate.strSource[j];
                                string strTmpLineSort = ClsWaitingToTranslate.strLogLine[j];
                                string strTmpFileName = ClsWaitingToTranslate.strFileName[j];

                                ClsWaitingToTranslate.dtmLogDate[j] = ClsWaitingToTranslate.dtmLogDate[j - 1];
                                ClsWaitingToTranslate.numTaskNo[j] = ClsWaitingToTranslate.numTaskNo[j - 1];
                                ClsWaitingToTranslate.strLogType[j] = ClsWaitingToTranslate.strLogType[j - 1];
                                ClsWaitingToTranslate.strSource[j] = ClsWaitingToTranslate.strSource[j - 1];
                                ClsWaitingToTranslate.strLogLine[j] = ClsWaitingToTranslate.strLogLine[j - 1];
                                ClsWaitingToTranslate.strFileName[j] = ClsWaitingToTranslate.strFileName[j - 1];

                                ClsWaitingToTranslate.dtmLogDate[j - 1] = dtmTmpSort;
                                ClsWaitingToTranslate.numTaskNo[j - 1] = numTmpSort;
                                ClsWaitingToTranslate.strLogType[j - 1] = strTmpTypeSort;
                                ClsWaitingToTranslate.strSource[j - 1] = strTmpSource;
                                ClsWaitingToTranslate.strLogLine[j - 1] = strTmpLineSort;
                                ClsWaitingToTranslate.strFileName[j - 1] = strTmpFileName;
                            }
                            //削除コード
                            for (var k = 0; k < MaxArray.numMaxClsWaitingToTranslate - 3; k++)
                            {
                                do
                                {
                                    //有効フラグを見に行かないとエラー
                                    if (ClsWaitingToTranslate.numTaskNo[k] == 0)
                                    {
                                        break;
                                    }
                                    if (ClsWaitingToTranslate.strSource[k] != ClsWaitingToTranslate.strSource[k + 1])
                                    {
                                        break;
                                    }
                                    if (ClsWaitingToTranslate.strLogType[k].Equals("Normal") == false)
                                    {
                                        break;
                                    }
                                    if (ClsWaitingToTranslate.strLogType[k + 1].Equals("Addons") == false)
                                    {
                                        break;
                                    }
                                    //翻訳対象文字列が同一で、NormalとAddonsでNormalが先に存在した場合、Addonsの方を削除する。
                                    ClsWaitingToTranslate.numTaskNo[k + 1] = ClsWaitingToTranslate.numTaskNo[k + 2];
                                    ClsWaitingToTranslate.dtmLogDate[k + 1] = ClsWaitingToTranslate.dtmLogDate[k + 2];
                                    ClsWaitingToTranslate.strLogType[k + 1] = ClsWaitingToTranslate.strLogType[k + 2];
                                    ClsWaitingToTranslate.strSource[k + 1] = ClsWaitingToTranslate.strSource[k + 2];
                                    ClsWaitingToTranslate.strLogLine[k + 1] = ClsWaitingToTranslate.strLogLine[k + 2];
                                    ClsWaitingToTranslate.strFileName[k + 1] = ClsWaitingToTranslate.strFileName[k + 2];

                                } while (false);
                            }
                        }
                        while (false);
                    }
                }
                if (ClsWaitingToTranslate.numTaskNo[0] != 0)
                {
                    var strPrint = "";
                    strPrint += "再読込待ち行列ソート後 numMyWaitNo = " + numMyWaitNo + "\n";
                    for (var i = 0; i < MaxArray.numMaxClsWaitingToTranslate - 1; i++)
                    {
                        if (String.IsNullOrEmpty(ClsWaitingToTranslate.strLogLine[i]))
                        {
                            break;
                        }
                        strPrint += "\tLogDate[" + i + "] = " + ClsWaitingToTranslate.dtmLogDate[i];
                        strPrint += "\tTaskNo[" + i + "] = " + ClsWaitingToTranslate.numTaskNo[i];
                        strPrint += "\tLogType[" + i + "] = " + ClsWaitingToTranslate.strLogType[i];
                        strPrint += "\tLogDataOriginal[" + i + "] = " + ClsWaitingToTranslate.strSource[i];
                        strPrint += "\tLogLine[" + i + "] = " + ClsWaitingToTranslate.strLogLine[i];
                        strPrint += "\n";
                    }
                    //logger.print(strPrint);
                }

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //ClsWaitingToTranslate.bExclusiveFlg = false;

                if (ClsWaitingToTranslate.numTaskNo[0] == 0)
                {
                    break;
                }

                numMyWaitNo = ClsWaitingToTranslate.numTaskNo[0];
                dtmAddLogDate = ClsWaitingToTranslate.dtmLogDate[0];
                strLogType = ClsWaitingToTranslate.strLogType[0];
                //strSource = ClsWaitingToTranslate.strSource[0];
                strLastLine = ClsWaitingToTranslate.strLogLine[0];
                strFileName = ClsWaitingToTranslate.strFileName[0];

                if (strLogType == "Normal")
                {
                    strSource = ExtractTargetTranslationNormal(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                }
                if (strLogType == "Addons")
                {
                    strSource = ExtractTargetTranslationAddons(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                }
                if (strLogType == "Guild")
                {
                    strSource = ExtractTargetTranslationGuild(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                }
                if (strLogType == "SimpleMail")
                {
                    strSource = ExtractTargetTranslationSimpleMail(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                }

                //ClsWaitingToTranslate.bExclusiveFlg = true;
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //実処理
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //stop();
                do
                {
                    strMemberLog = strSource;
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, strFileName, "Log", bDoTranslationFlg, strLogType, strDateTime, strMemberID, strMember, strMemberLog);
                    //if (strLogType == "Addons")
                    //{
                    //    break;
                    //}
                    if (bDoTranslationFlg == false)
                    {
                        break;
                    }
                    var strLangTarget = Language_GP.myLanguage_GP();
                    //if (strLangTarget.Equals("JA"))
                    //{
                        strAfterSlang = ConvertDictionarySlang(strSource, ref bHitFlg);
                        if (bHitFlg == true)
                        {
                            ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "Dic", false, strLogType, strDateTime, strMemberID, strMember, strAfterSlang);
                        }
                        else
                        {
                            strAfterSlang = strSource;
                        }
                    //}
                    /*
                    if (strLangTarget.Equals("EN"))
                    {
                        strAfterSlang = strSource;
                    }
                    //gfCheckAutoTranslation(strAfterSlang, strLangTarget, strLangSource, ref bBreakFlg);
                    gfCheckAutoTranslation(dtmAddLogDate, strSource, strLangTarget, strLangSource, strLogType, ref bBreakFlg);
                    if (bBreakFlg == true)
                    {
                        break;
                    }
                    bBreakFlg = false;
                    //strAfterTranslation = gfAutoTransration(strAfterSlang, ref bBreakFlg);
                    //strAfterTranslation = gfCallAutoTranslation(dtmAddLogDate, strAfterSlang, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
                    */

                    strAfterTranslation = CallTransrationChatlog(dtmAddLogDate, strAfterSlang, strLangTarget, ref strTranslator, ref bBreakFlg, strLogType, strDateTime, strMemberID, strMember, strAfterTranslation);
                    //stop();
                    if (bBreakFlg == true)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(strTranslator) != false)
                    {
                        break;
                    }
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "Trans " + strTranslator, bDoTranslationFlg, strLogType, strDateTime, strMemberID, strMember, strAfterTranslation);
                    break;
                }
                while (false);
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //処理が終了したら待ち行列から自分を削除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //ClsWaitingToTranslate.bExclusiveFlg = true;
                for (var i = 0; i <= MaxArray.numMaxClsWaitingToTranslate - 2; i++)
                {
                    //削除対象のデータが無効ならば、それ以上削除しない。
                    if (ClsWaitingToTranslate.numTaskNo[i] == 0)
                    {
                        break;
                    }
                    //<todo>
                    ClsWaitingToTranslate.numTaskNo[i] = ClsWaitingToTranslate.numTaskNo[i + 1];
                    ClsWaitingToTranslate.dtmLogDate[i] = ClsWaitingToTranslate.dtmLogDate[i + 1];
                    ClsWaitingToTranslate.strLogType[i] = ClsWaitingToTranslate.strLogType[i + 1];
                    ClsWaitingToTranslate.strSource[i] = ClsWaitingToTranslate.strSource[i + 1];
                    ClsWaitingToTranslate.strLogLine[i] = ClsWaitingToTranslate.strLogLine[i + 1];
                    ClsWaitingToTranslate.strFileName[i] = ClsWaitingToTranslate.strFileName[i + 1];
                }
                //配列占有フラグを寝せて、処理を終了
                //ClsWaitingToTranslate.bExclusiveFlg = false;

                if (ClsWaitingToTranslate.numTaskNo[0] != 0)
                {
                    var strPrint = "";
                    strPrint += "再読込待ち行列削除後 numMyWaitNo = " + numMyWaitNo + "\n";
                    for (var i = 0; i < MaxArray.numMaxClsWaitingToTranslate - 1; i++)
                    {
                        if (String.IsNullOrEmpty(ClsWaitingToTranslate.strLogLine[i]))
                        {
                            break;
                        }
                        strPrint += "\tLogDate[" + i + "] = " + ClsWaitingToTranslate.dtmLogDate[i];
                        strPrint += "\tTaskNo[" + i + "] = " + ClsWaitingToTranslate.numTaskNo[i];
                        strPrint += "\tLogType[" + i + "] = " + ClsWaitingToTranslate.strLogType[i];
                        strPrint += "\tLogType[" + i + "] = " + ClsWaitingToTranslate.strSource[i];
                        strPrint += "\tLogLine[" + i + "] = " + ClsWaitingToTranslate.strLogLine[i];
                        strPrint += "\n";
                    }
                    //logger.print(strPrint);
                }

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);
            timer1.Interval = 100;
            timer1.Start();
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }
        //************************************************************************************************************************************************************************************************
        //タイマーで待ちnormalログとaddonsログが揃った頃に翻訳
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //翻訳関数群開始
        //************************************************************************************************************************************************************************************************
        private string CallTransrationChatlog(DateTime dtmAddLogDate, string strSource, string strLangTarget, ref string strTranslator, ref bool bBreakFlg, string strLogType, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            /*
            入力
            ↓
            言語自動判別(JAVA2014)
            ↓			        ↓(NULL)
            DeepL or GAS		1バイトor2バイト
                                ↓
                                ALL1バイト = 英語
                                一部2バイト = 正規表現CJKチェック(AutoJudgementCJKCheckRegex)
                                ↓
                                DeepL or GAS

            入力
            ↓
		1バイトor2バイト
            ↓
        ALL1バイト = 言語自動判別(JAVA2014)
        一部2バイト = 正規表現CJKチェック(AutoJudgementCJKCheckRegex)
            ↓                  ↓
        DeepL or GAS            EN
                                ↓
                        言語自動判別(JAVA2014)

            "EN",
            "DE",
            "ES",
            "FR",
            "RU",
            "ZH-CN",
            "ZH-TW",
            "JA",
            "KO"
             */

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strLangJudge = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "LangJudge", strIniFileName);
            var strTranslationOperationMode = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TranslationOperationMode", strIniFileName);

            if (string.IsNullOrEmpty(strLangJudge) != false)
            {
                strLangJudge = "LDJAVA2014";
            }

            var strLD = strLangJudge;

            var strRet1 = "";
            var strRet3 = "";
            var strReturn = "";

            if (strLangTarget.Equals("ZH-CN") != false)
            {
                strLangTarget = "ZH";
            }
            if (strLangTarget.Equals("ZH-TW") != false)
            {
                strLangTarget = "ZH";
            }

            do
            {
                if (strLD == "LDJAVA2014")
                {
                    if (string.IsNullOrWhiteSpace(strSource.Trim()))
                    {
                        //翻訳する対象の文字列が空白
                        break;
                    }
                    strRet1 = AutoJudgementCJKCheckLanguageDetector2014(strSource);
                    if (String.IsNullOrEmpty(strRet1) != false)
                    {
                        if (isOneByteChar(strSource) == false)
                        {
                            //2バイトを含んでいる
                            strRet1 = AutoJudgementCJKCheckRegex(strSource);
                            //翻訳実行
                            strRet3 = strRet1.ToUpper();
                            break;
                        }
                        else
                        {
                            //1バイト
                            //ENと解釈
                            //1バイト
                            if (AutoJudgementCheckShouldEtoJ(strSource) == false)
                            {
                                break;
                            }
                            //翻訳実行
                            strRet3 = "EN";
                            break;
                        }
                    }
                    if (strRet1.Length == 2)
                    {
                        //翻訳実行
                        strRet3 = strRet1.ToUpper();
                        break;
                    }
                    if (strRet1.Length == 5)
                    {
                        //翻訳実行
                        strRet3 = strRet1.ToUpper();
                        break;
                    }
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "LD2014 error");
                    break;
                }
                if (strLD == "LDRECJKChk")
                {
                    //入力
                    //    ↓
                    //1バイトor2バイト
                    //    ↓
                    //ALL1バイト = 言語自動判別(2014)
                    //一部2バイト = 正規表現CJKチェック(AutoJudgementCJKCheckRegex)
                    //    ↓                  ↓
                    //DeepL or GAS EN
                    //                        ↓

                    if (string.IsNullOrWhiteSpace(strSource.Trim()))
                    {
                        //翻訳する対象の文字列が空白
                        break;
                    }
                    if (isOneByteChar(strSource) == false)
                    {
                        //2バイトを含んでいる
                        strRet3 = AutoJudgementCJKCheckRegex(strSource);
                        if (strRet3.Equals("EN") != false)
                        {
                            strRet3 = AutoJudgementCJKCheckLanguageDetector2014(strSource);
                        }
                        if (strRet3.Equals("en") != false)
                        {
                            strRet3 = AutoJudgementCJKCheckLanguageDetector2014(strSource);
                        }
                    }
                    else
                    {
                        //1バイト
                        if (AutoJudgementCheckShouldEtoJ(strSource) == false)
                        {
                            break;
                        }
                        strRet3 = AutoJudgementCJKCheckLanguageDetector2014(strSource);
                    }
                    break;
                }
            }
            while (false);

            do
            {
                if (string.IsNullOrWhiteSpace(strSource.Trim()))
                {
                    //翻訳する対象の文字列が空白
                    break;
                }
                if (string.IsNullOrWhiteSpace(strRet3) != false)
                {
                    break;
                }
                var strLangSource = strRet3.ToUpper();
                if (strLangTarget.Equals(strLangSource))
                {
                    //同じ言語同士を翻訳しようとしている
                    break;
                }
                if (strTranslationOperationMode.Equals("Async") == true)
                {
                    //非同期で呼び出し
                    AsyncCallTranslation(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, ref bBreakFlg, strLogType, strDateTime, strMemberID, strMember, strAfterTranslation);
                    break;
                }
                //同期で呼び出し
                
                strReturn = SyncCallTranslation(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
            }
            while (false);
            //stop();
            return strReturn;
        }
        public void AsyncCallTranslation(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, ref bool bBreakFlg, string strLogType, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strIndexNo = "<" + Value.lIndexNoCurrent.ToString("D8") + ">";
            Value.lIndexNoCurrent++;

            ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "Waiting" + strTranslator, true, strLogType, strDateTime, strMemberID, strMember, strIndexNo);
            do
            {
                if (strTranslation.Equals("DeepL API Free") || strTranslation.Equals("DeepL API Pro") || strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    ASyncCallTranslationDeepL(dtmAddLogDate, strSource, strLangTarget, strLangSource, strLogType, strIndexNo, strDateTime, strMemberID, strMember, strAfterTranslation);
                }
                if (strTranslation.Equals("Google Apps Scripts") || strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    ASyncCallTranslationGAS(dtmAddLogDate, strSource, strLangTarget, strLangSource, strLogType, strIndexNo, strDateTime, strMemberID, strMember, strAfterTranslation);
                }
                if (strTranslation.Equals("Baidu Trans Web API"))
                {
                    ASyncCallTranslationBaidu(dtmAddLogDate, strSource, strLangTarget, strLangSource, strLogType, strIndexNo, strDateTime, strMemberID, strMember, strAfterTranslation);
                }
                if (strTranslation.Equals("TexTra API"))
                {
                    ASyncCallTranslationTexTra(dtmAddLogDate, strSource, strLangTarget, strLangSource, strLogType, strIndexNo, strDateTime, strMemberID, strMember, strAfterTranslation);
                }
            }
            while (false);
            //ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            //stop();
        }
        private async void ASyncCallTranslationDeepL(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, string strLogType, string strIndexNo, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strTranslator = "DeepL";
            var strJSON = "";
            var strMessageTarget = "";
            var strMessageError = "";
            var strCode = "";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);

            var param1EndPoint = "";
            var param2Key = "";

            //stop();

            do//空ループ
            {
                if (strTranslation.Equals("DeepL API Free"))
                {
                    param1EndPoint = "https://api-free.deepl.com/v2/translate";
                    param2Key = strDeepLAPIFreeKey;
                    break;
                }
                if (strTranslation.Equals("DeepL API Pro"))
                {
                    param1EndPoint = "https://api.deepl.com/v2/translate";
                    param2Key = strDeepLAPIProKey;
                    break;
                }
                if (strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    if (strDeepLAPIFreeKey != "")
                    {
                        param1EndPoint = "https://api-free.deepl.com/v2/translate";
                        param2Key = strDeepLAPIFreeKey;
                        break;
                    }
                    if (strDeepLAPIProKey != "")
                    {
                        param1EndPoint = "https://api.deepl.com/v2/translate";
                        param2Key = strDeepLAPIProKey;
                        break;
                    }
                }
            } while (false);
            do
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    break;
                }
                if (strLangSource.Equals("KO") != false)
                {
                    break;
                }
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "auth_key", param2Key },
                    { "text", strSource},
                    { "source_lang", strLangSource },
                    { "target_lang", strLangTarget }
                });

                //var res1 = await httpClient.PostAsync(param1EndPoint, content);
                //strJSON = res1.ToString();

                //例外キャッチ
                try
                {
                    {
                        var res = await httpClient.PostAsync(param1EndPoint, content);
                        strJSON = await res.Content.ReadAsStringAsync();
                    }
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                //例外キャッチ終了

                //リターンコードを取得する。
                //"StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:\r\n{\r\n  access-control-allow-origin: *\r\n  strict-transport-security: max-age=63072000; includeSubDomains; preload\r\n  server-timing: l7_lb_tls;dur=556, l7_lb_idle;dur=0, l7_lb_receive;dur=5, l7_lb_total;dur=601\r\n  access-control-expose-headers: Server-Timing\r\n  Date: Sun, 26 Mar 2023 02:36:28 GMT\r\n  Server: nginx\r\n  Content-Length: 104\r\n  Content-Type: application/json\r\n}"
                //{"translations":[{"detected_source_language":"EN","text":"これはテストメッセージです。"}]}
                //if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "StatusCode..", ",") == false)
                //{
                //    break;
                //}
                //if (strCode.Equals("200") == false)
                //{
                //    //200以外ならエラーメッセージを取得する。
                //    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                //    {
                //        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                //        break;
                //    }
                //    break;
                //}
                //リターンコードが200なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。

                //var res2 = await res1.Content.ReadAsStringAsync();
                //strJSON = res2.ToString();

                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }

                strTranslator = "Trans " + strTranslator;

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
                //stop();
                ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            } while (false);
        }
        private async void ASyncCallTranslationGAS_bak(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, string strLogType, string strIndexNo, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strTranslator = "Google";
            var strJSON = "";
            var strMessageTarget = "";
            var strMessageError = "";
            var strSendJson = "";
            var strCode = "";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            //stop();

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                if ((strTranslation.Equals("Google Apps Scripts") || strTranslation.Equals("DeepL API and Google Apps Scripts")) == false)
                {
                    break;
                }
            } while (false);
            do
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    break;
                }
                if (strLangSource.Equals("KO") != false)
                {
                    break;
                }

                strSendJson = "{ \"value\" : \"" + strSource.Trim() + "\", \"source\" : \"" + strLangSource + "\", \"target\" : \"" + strLangTarget + "\"  }";
                var content = new StringContent(strSendJson, Encoding.UTF8, "application/x-www-form-urlencoded");

                //取得
                var res1 = await httpClient.PostAsync(strGoogleAppsScriptsURL, content);
                strJSON = res1.ToString();

                //リターンコードを取得する。
                //"StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:\r\n{\r\n  X-Content-Type-Options: nosniff\r\n  Access-Control-Allow-Origin: *\r\n  Pragma: no-cache\r\n  X-Frame-Options: SAMEORIGIN\r\n  Content-Security-Policy: frame-ancestors 'self'\r\n  X-XSS-Protection: 1; mode=block\r\n  Alt-Svc: h3=\":443\"; ma=2592000,h3-29=\":443\"; ma=2592000\r\n  Vary: Sec-Fetch-Dest\r\n  Vary: Sec-Fetch-Mode\r\n  Vary: Sec-Fetch-Site\r\n  Vary: Accept-Encoding\r\n  Transfer-Encoding: chunked\r\n  Accept-Ranges: none\r\n  Cache-Control: no-store, must-revalidate, no-cache, max-age=0\r\n  Date: Sun, 26 Mar 2023 02:23:27 GMT\r\n  Server: GSE\r\n  Content-Type: application/json; charset=utf-8\r\n  Expires: Mon, 01 Jan 1990 00:00:00 GMT\r\n}"
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "StatusCode..", ",") == false)
                {
                    break;
                }
                if (strCode.Equals("200") == false)
                {
                    //200以外ならエラーメッセージを取得する。
                    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                    {
                        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                        break;
                    }
                    break;
                }

                //リターンコードが200なら正常終了
                var res2 = await res1.Content.ReadAsStringAsync();
                strJSON = res2.ToString();

                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }
                strTranslator = "Trans " + strTranslator;

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }

                ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            } while (false);
        }
        private async void ASyncCallTranslationGAS(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, string strLogType, string strIndexNo, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strTranslator = "Google";
            var strJSON = "";
            var strMessageTarget = "";
            var strMessageError = "";
            var strSendJson = "";
            var strCode = "";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            //stop();

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                if ((strTranslation.Equals("Google Apps Scripts") || strTranslation.Equals("DeepL API and Google Apps Scripts")) == false)
                {
                    break;
                }
            } while (false);
            do
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    break;
                }
                if (strLangSource.Equals("KO") != false)
                {
                    break;
                }

                strSendJson = "{ \"value\" : \"" + strSource.Trim() + "\", \"source\" : \"" + strLangSource + "\", \"target\" : \"" + strLangTarget + "\"  }";
                var content = new StringContent(strSendJson, Encoding.UTF8, "application/x-www-form-urlencoded");

                //例外キャッチ
                try
                {
                    {
                        var res = await httpClient.PostAsync(strGoogleAppsScriptsURL, content);
                        strJSON = await res.Content.ReadAsStringAsync();
                    }
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                //例外キャッチ終了

                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }
                strTranslator = "Trans " + strTranslator;

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }

                ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            } while (false);
        }
        private async void ASyncCallTranslationBaidu(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, string strLogType, string strIndexNo, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strTranslator = "Baidu";
            var strJSON = "";
            var strMessageTarget = "";
            var strCode = "";
            var strMessageError = "";
            var strSendJson = "";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strBaiduID = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu ID", strIniFileName);
            var strBaiduPASS = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu PASS", strIniFileName);

            var param1EndPoint = "";
            var param2Key = "";

            //stop();

            do
            {
                if (strTranslation.Equals("Baidu Trans Web API") == false)
                {
                    break;
                }

                //ES = spa
                //FR = fra
                //JA = jp
                //KO = kor
                if (strLangSource.Equals("ES") != false)
                {
                    strLangSource = "spa";
                }
                if (strLangTarget.Equals("ES") != false)
                {
                    strLangTarget = "spa";
                }
                if (strLangSource.Equals("FR") != false)
                {
                    strLangSource = "fra";
                }
                if (strLangTarget.Equals("FR") != false)
                {
                    strLangTarget = "fra";
                }
                if (strLangSource.Equals("JA") != false)
                {
                    strLangSource = "jp";
                }
                if (strLangTarget.Equals("JA") != false)
                {
                    strLangTarget = "jp";
                }
                if (strLangSource.Equals("KO") != false)
                {
                    strLangSource = "kor";
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    strLangTarget = "kor";
                }

                // 原文
                string q = strSource;
                // 源语言
                string from = strLangSource.ToLower();
                // 目标语言
                string to = strLangTarget.ToLower();
                // 改成您的APP ID
                string appId = strBaiduID;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                // 改成您的密钥
                string secretKey = strBaiduPASS;
                string sign = EncryptString(appId + q + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                url += "q=" + HttpUtility.UrlEncode(q);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 6000;

                string retString = "";

                strJSON = retString;

                strTranslator = "Baidu";

                //strSendJson = "{" +
                //    "\"q\":" + strSource + "," +
                //    "\"from\":" + from + "," +
                //    "\"to\":" + to + "," +
                //    "\"appid\":" + appId + "," +
                //    "\"salt\":" + salt + "," +
                //    "\"sign\":" + sign + "," +
                //"}";
                //var content = new StringContent(strSendJson, Encoding.UTF8, "text/html;charset=UTF-8");

                //var content = new FormUrlEncodedContent(new Dictionary<string, string>
                //{
                //    { "q", strSource },
                //    { "from", from},
                //    { "to", to },
                //    { "appid", appId },
                //    { "salt", salt },
                //    { "sign", sign }
                //});
                //var res1 = await httpClient.PostAsync(url, content);
                
                //var res1 = await httpClient.GetAsync(url);
                //strJSON = res1.ToString();

                //例外キャッチ
                try
                {
                    {
                        var res = await httpClient.GetAsync(url);
                        strJSON = await res.Content.ReadAsStringAsync();
                    }
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                //例外キャッチ終了

                //リターンコードを取得する。
                //"StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:\r\n{\r\n  Tracecode: 07931630100403871498032611\r\n  Date: Sun, 26 Mar 2023 03:13:13 GMT\r\n  P3P: CP=\" OTI DSP COR IVA OUR IND COM \"\r\n  Set-Cookie: BAIDUID=0BFC7F53979BAF430F0C8FEEA0267EF1:FG=1; expires=Mon, 25-Mar-24 03:13:13 GMT; max-age=31536000; path=/; domain=.baidu.com; version=1\r\n  Server: Apache\r\n  Content-Length: 160\r\n  Content-Type: application/json\r\n}"
                //if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "StatusCode..", ",") == false)
                //{
                //    break;
                //}
                //if (strCode.Equals("200") == false)
                //{
                //    //200以外ならエラーメッセージを取得する。
                //    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                //    {
                //        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                //        break;
                //    }
                //    break;
                //}
                //リターンコードが200なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"

                ////翻訳文字列を取得する。
                //var res2 = await res1.Content.ReadAsStringAsync();
                //strJSON = res2.ToString();

                //"{\"from\":\"en\",\"to\":\"jp\",\"trans_result\":[{\"src\":\"this is a test message\",\"dst\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\"}]}"
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, "dst\\x22.\\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }

                strTranslator = "Trans " + strTranslator;

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }

                ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            } while (false);
        }
        private async void ASyncCallTranslationTexTra(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, string strLogType, string strIndexNo, string strDateTime, string strMemberID, string strMember, string strAfterTranslation)
        {
            var strTranslator = "TexTra";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strTexTraAPIName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIName", strIniFileName);
            var strTexTraAPIKEY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIKEY", strIniFileName);
            var strTexTraAPISecret = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPISecret", strIniFileName);

            var bEncordedEscapeSeuenceUnicode = false;

            var strSendJson = "";
            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageResponse = "";
            var strMessageTarget = "";


            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    break;
                }
                strLangSource = strLangSource.ToLower();
                strLangTarget = strLangTarget.ToLower();
                if (strLangSource == "zh-cn")
                {
                    strLangSource = "zh-CN";
                }
                if (strLangSource == "zh-tw")
                {
                    strLangSource = "zh-TW";
                }
                if (strLangTarget == "zh-cn")
                {
                    strLangTarget = "zh-CN";
                }
                if (strLangTarget == "zh-tw")
                {
                    strLangTarget = "zh-TW";
                }
                if (strTranslation.Equals("TexTra API") == false)
                {
                    break;
                }
                string URL_VAL = "https://mt-auto-minhon-mlt.ucri.jgn-x.jp/api/mt/generalNT" + "_" + strLangSource + "_" + strLangTarget + "/";
                string TYPE = "json";
                string TEXT = strSource;

                //stop();

                //HttpConnectionOAuthのインスタンス化
                HttpConnection httpCon = new HttpConnection();

                //stop();

                //初期化（コンシューマーキーの設定など）
                httpCon.Initialize(strTexTraAPIKEY, strTexTraAPISecret, "", "", "");

                //stop();

                //取得
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("key", strTexTraAPIKEY);
                param.Add("name", strTexTraAPIName);
                param.Add("type", TYPE);
                param.Add("text", TEXT);
                // その他のパラメータについては、各APIのリクエストパラメータに従って設定してください。

                //HttpStatusCode statusCode = httpCon.GetContent("POST", new Uri(URL_VAL), param, ref strJSON, null);
                httpCon.SetAwaitParameter("POST", new Uri(URL_VAL), param, ref strJSON, null);
                await httpCon.GetContentAsync();
                httpCon.GetAwaitParameter(ref strJSON);

                //Console.WriteLine(result);

                //リターンコードを取得する。
                //"{\"resultset\":{\"code\":0,\"message\":\"\",\"request\":{\"url\":\"https:\\/\\/mt-auto-minhon-mlt.ucri.jgn-x.jp\\/api\\/mt\\/generalNT_en_ja\\/\",\"text\":\"this is a pen\",\"split\":0,\"history\":0,\"xml\":null,\"term_id\":\"all\",\"bilingual_id\":\"all\",\"log_use\":2,\"editor_use\":1,\"data\":null},\"result\":{\"text\":\"\\u3053\\u308c\\u306f\\u30da\\u30f3\\u3067\\u3059\",\"blank\":0,\"information\":{\"text-s\":\"this is a pen\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30da\\u30f3\\u3067\\u3059\",\"sentence\":[{\"text-s\":\"this is a pen\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30da\\u30f3\\u3067\\u3059\",\"split\":[{\"text-s\":\"this is a pen\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30da\\u30f3\\u3067\\u3059\",\"process\":{\"regex\":[],\"replace-before\":[],\"short-before\":[],\"preprocess\":[],\"translate\":{\"reverse\":[],\"specification\":[],\"text-s\":\"this is a pen\",\"src-token\":[\"this\",\"is\",\"a\",\"pen\"],\"text-t\":\"\\u3053\\u308c\\u306f\\u30da\\u30f3\\u3067\\u3059\",\"associate\":[[],[],[[[\"this\"],[\"\\u3053\\u308c\"]],[[\"is\"],[\"\\u3067\\u3059\"]],[[\"a\"],[\"\\u306f\"]],[[\"pen\"],[\"\\u30da\\u30f3\"]]]],\"oov\":null,\"exception\":\"\",\"associates\":[[[],[],[[[\"this\"],[\"\\u3053\\u308c\"]],[[\"is\"],[\"\\u3067\\u3059\"]],[[\"a\"],[\"\\u306f\"]],[[\"pen\"],[\"\\u30da\\u30f3\"]]]]]},\"short-after\":[],\"replace-after\":[]}}]}]}}}}"
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "code..", ",") == false)
                {
                    break;
                }
                if (strCode.Equals("0") == false)
                {
                    //200以外ならエラーメッセージを取得する。
                    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                    {
                        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                        break;
                    }
                    break;
                }
                //リターンコードが200なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。

                //var res2 = await res1.Content.ReadAsStringAsync();
                //strJSON = res2.ToString();

                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageResponse, "result\\x22:{\\x22", "}") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }
                if (ConvertJsonToStringRegexGeneralPurpose(strMessageResponse, ref strMessageTarget, "text\\x22.\\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                }
                strTranslator = "Trans " + strTranslator;

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }

                ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
            } while (false);
        }
        public string SyncCallTranslation(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            var strRet = "";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            do
            {
                if (strTranslation.Equals("DeepL API Free") || strTranslation.Equals("DeepL API Pro") || strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    strRet = SyncCallTranslationDeepL(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
                }
                if (strRet.Equals("") == false)
                {
                    break;
                }
                if (strTranslation.Equals("Google Apps Scripts") || strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    strRet = SyncCallTranslationGAS(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
                }
                if (strRet.Equals("") == false)
                {
                    break;
                }
                if (strTranslation.Equals("Baidu Trans Web API"))
                {
                    strRet = SyncCallTranslationBaidu(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
                }
                if (strTranslation.Equals("TexTra API"))
                {
                    strRet = SyncCallTranslationTexTra(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
                }
            }
            while (false);
            //stop();
            return strRet;
        }
        public string SyncCallTranslationDeepL(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            var strRet = "";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strDeepLUsePython = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepLUsePython", strIniFileName);
            if (strDeepLUsePython.Equals("true") == false)
            {
                strRet = SyncCallTranslationDeepLBuiltin(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
            }
            else
            {
                strRet = SyncCallTranslationDeepLPython(dtmAddLogDate, strSource, strLangTarget, strLangSource, ref strTranslator, strLogType, ref bBreakFlg);
            }
            return strRet;
        }
        public string SyncCallTranslationDeepLBuiltin(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            bBreakFlg = false;
            strTranslator = "DeepL";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);

            var param1EndPoint = "";
            var param2Key = "";

            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageTarget = "";

            var strRet = "";

            do
            {
                if (strTranslation.Equals("DeepL API Free"))
                {
                    param1EndPoint = "https://api-free.deepl.com/v2/translate";
                    param2Key = strDeepLAPIFreeKey;
                    break;
                }
                if (strTranslation.Equals("DeepL API Pro"))
                {
                    param1EndPoint = "https://api.deepl.com/v2/translate";
                    param2Key = strDeepLAPIProKey;
                    break;
                }
                if (strTranslation.Equals("DeepL API and Google Apps Scripts"))
                {
                    if (strDeepLAPIFreeKey != "")
                    {
                        param1EndPoint = "https://api-free.deepl.com/v2/translate";
                        param2Key = strDeepLAPIFreeKey;
                        break;
                    }
                    if (strDeepLAPIProKey != "")
                    {
                        param1EndPoint = "https://api.deepl.com/v2/translate";
                        param2Key = strDeepLAPIProKey;
                        break;
                    }
                }
            }
            while (false);

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangSource.Equals("KO") != false)
                {
                    bBreakFlg = true;
                    break;
                }
                //var param3TagetLang = "\"" + strLangTarget + "\"";
                //var param4SourceLang = "\"" + strLangSource + "\"";
                var param3TagetLang = "" + strLangTarget + "";
                var param4SourceLang = "" + strLangSource + "";
                var param5Text = strSource.Trim();
                //string param = param1EndPoint + " " + param2Key + " " + param3TagetLang + " " + param4SourceLang + " " + param5Text;

                //WebClient webClientDeepLBuiltin = new WebClient();
                webClientDeepLBuiltin.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClientDeepLBuiltin.Encoding = Encoding.UTF8;

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "auth_key", param2Key },
                    { "text", param5Text },
                    { "source_lang", param4SourceLang },
                    { "target_lang", param3TagetLang }
                };

                string data_string = "";
                foreach (KeyValuePair<string, string> kvp in data)
                {
                    if (data_string != "")
                    {
                        data_string += "&";
                    }
                    data_string += kvp.Key + "=";

                    data_string += HttpUtility.UrlEncode(kvp.Value, Encoding.UTF8);
                }
                try
                {
                    String result = webClientDeepLBuiltin.UploadString(new Uri(param1EndPoint), "POST", data_string);
                    strJSON = result.ToString();
                }
                catch (Exception e)
                {
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
                }

                ////リターンコードを取得する。
                //if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "{\\x22code\\x22.", ",") == false)
                //{
                //    bBreakFlg = true;
                //    break;
                //}
                //if (strCode.Equals("200") == false)
                //{
                //    //200以外ならエラーメッセージを取得する。
                //    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                //    {
                //        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                //        bBreakFlg = true;
                //        break;
                //    }
                //    bBreakFlg = true;
                //    break;
                //}
                ////リターンコードが200なら正常終了
                ////"result":{"text":"\u30c6\u30b9\u30c8"
                ////翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                    bBreakFlg = true;
                    break;
                }

                //strTranslator = "DeepL";

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
                strRet = strMessageTarget;
#if DEBUG
                ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + "Builtin API call MSG = " + strSource);
#endif
            }
            while (false);

            return strRet;
        }
        private string SyncCallTranslationDeepLPython(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            bBreakFlg = false;
            strTranslator = "DeepL";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            var strRet = "";


            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageTarget = "";

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangSource.Equals("KO") != false)
                {
                    bBreakFlg = true;
                    break;
                }
                if (Regex.IsMatch(strTranslation, "DeepL API.*", RegexOptions.Singleline) == false)
                {
                    bBreakFlg = true;
                    break;
                }
                var bPyCheck = CallPythonExistenceCheck(dtmAddLogDate, strLogType);
                if (bPyCheck == false)
                {
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Python install error");
                    bBreakFlg = true;
                    break;
                }
                var strPythonFileName = ".\\" + strEnvironment + ".py";
                if (File.Exists(strPythonFileName) == false)
                {
                    var pySource = "";
                    pySource += "import argparse\n";
                    pySource += "import json\n";
                    pySource += "import os\n";
                    pySource += "import sys\n";
                    pySource += "import urllib.parse\n";
                    pySource += "import urllib.request\n";
                    pySource += "\n";
                    pySource += "argvs = sys.argv\n";
                    pySource += "DEEPL_TRANSLATE_EP = argvs[1]\n";
                    pySource += "AUTH_KEY = argvs[2]\n";
                    pySource += "t_lang = argvs[3]\n";
                    pySource += "s_lang = argvs[4]\n";
                    pySource += "text = argvs[5]\n";
                    pySource += "\n";
                    pySource += "T_LANG_CODES = [\"DE\", \"EN\", \"FR\", \"IT\", \"JA\", \"ES\", \"NL\", \"PL\", \"PT-PT\", \"PT-BR\", \"PT\", \"RU\", \"ZH\"]\n";
                    pySource += "S_LANG_CODES = [\"DE\", \"EN\", \"FR\", \"IT\", \"JA\", \"ES\", \"NL\", \"PL\", \"PT\", \"RU\", \"ZH\"]\n";
                    pySource += "\n";
                    pySource += "def translate(text, s_lang='', t_lang='JA'):\n";
                    pySource += "    headers = {\n";
                    pySource += "        'Content-Type': 'application/x-www-form-urlencoded; utf-8'\n";
                    pySource += "    }\n";
                    pySource += "\n";
                    pySource += "    params = {\n";
                    pySource += "        'auth_key': AUTH_KEY,\n";
                    pySource += "        'text': text,\n";
                    pySource += "        'target_lang': t_lang\n";
                    pySource += "    }\n";
                    pySource += "\n";
                    pySource += "    if s_lang != '':\n";
                    pySource += "        params['source_lang'] = s_lang\n";
                    pySource += "\n";
                    pySource += "    req = urllib.request.Request(\n";
                    pySource += "        DEEPL_TRANSLATE_EP,\n";
                    pySource += "        method='POST',\n";
                    pySource += "        data=urllib.parse.urlencode(params).encode('utf-8'),\n";
                    pySource += "        headers=headers\n";
                    pySource += "    )\n";
                    pySource += "\n";
                    pySource += "    try:\n";
                    pySource += "        with urllib.request.urlopen(req) as res:\n";
                    pySource += "            res_json = json.loads(res.read().decode('utf-8'))\n";
                    pySource += "            print(json.dumps(res_json, indent=2))\n";
                    pySource += "    except urllib.error.HTTPError as e:\n";
                    pySource += "        print(e)\n";
                    pySource += "\n";
                    pySource += "\n";
                    pySource += "if __name__ == '__main__':\n";
                    pySource += "\n";
                    pySource += "    if t_lang not in T_LANG_CODES:\n";
                    pySource += "        print((\n";
                    pySource += "            f'ERROR: Invalid target Language_GP code \"{t_lang}\". \\n'\n";
                    pySource += "            f'Alloed lang code are following. \\n{str(T_LANG_CODES)}'\n";
                    pySource += "        ))\n";
                    pySource += "        sys.exit(1)\n";
                    pySource += "\n";
                    pySource += "    if s_lang != '' and s_lang not in S_LANG_CODES:\n";
                    pySource += "        print((\n";
                    pySource += "            f'WARNING: Invalid source Language_GP code \"{s_lang}\". \\n'\n";
                    pySource += "            'The source Language_GP is automatically determined in this request. \\n'\n";
                    pySource += "            f'Allowed source lang code are following. \\n{str(S_LANG_CODES)} \\n\\n'\n";
                    pySource += "        ))\n";
                    pySource += "        s_lang = ''\n";
                    pySource += "\n";
                    pySource += "    translate(text, t_lang=t_lang, s_lang=s_lang)\n";

                    //Pythonプログラムソースをファイルに保存
                    File.WriteAllText(strPythonFileName, pySource, Encoding.GetEncoding("utf-8"));
                }
                //Pythonプログラムのパスとプログラムに渡す引数を準備
                string program = strPythonFileName;
                var param1EndPoint = "";
                var param2Key = "";
                do
                {
                    if (strTranslation.Equals("DeepL API Free"))
                    {
                        param1EndPoint = "\"https://api-free.deepl.com/v2/translate\"";
                        param2Key = "\"" + strDeepLAPIFreeKey + "\"";
                        break;
                    }
                    if (strTranslation.Equals("DeepL API Pro"))
                    {
                        param1EndPoint = "\"https://api.deepl.com/v2/translate\"";
                        param2Key = "\"" + strDeepLAPIProKey + "\"";
                        break;
                    }
                    if (strTranslation.Equals("DeepL API and Google Apps Scripts"))
                    {
                        if (strDeepLAPIFreeKey != "")
                        {
                            param1EndPoint = "\"https://api-free.deepl.com/v2/translate\"";
                            param2Key = "\"" + strDeepLAPIFreeKey + "\"";
                            break;
                        }
                        if (strDeepLAPIProKey != "")
                        {
                            param1EndPoint = "\"https://api.deepl.com/v2/translate\"";
                            param2Key = "\"" + strDeepLAPIProKey + "\"";
                            break;
                        }
                    }
                }
                while (false);
                var param3TagetLang = "\"" + strLangTarget + "\"";
                var param4SourceLang = "\"" + strLangSource + "\"";
                var param5Text = "\"" + strSource.Trim() + "\"";
                string param = param1EndPoint + " " + param2Key + " " + param3TagetLang + " " + param4SourceLang + " " + param5Text;
                //MessageBox.Show("program = " + program + "\nparam = " + param);
                //Pythonプログラムを実行し、戻ってきた結果を出力
                strJSON = "";
                foreach (string line in CallPythonFile(program, false, dtmAddLogDate, strLogType, param))
                {
                    strJSON += line;
                }
                //var strInput = " " + Microsoft.VisualBasic.Strings.StrConv(strData, VbStrConv.Lowercase) + " ";
                //{"code":200,"text":"これはテストメッセージです"}
                /*
                    {
                        "translations": [
                        {
                            "detected_source_Language_GP": "EN",
                            "text": "これはテストメッセージです"
                        }
                        ]
                    }
                */
                ////リターンコードを取得する。
                //if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "{\\x22code\\x22.", ",") == false)
                //{
                //    bBreakFlg = true;
                //    break;
                //}
                //if (strCode.Equals("200") == false)
                //{
                //    //200以外ならエラーメッセージを取得する。
                //    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                //    {
                //        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "TexTra API call error " + strCode + " = " + strMessageError);
                //        bBreakFlg = true;
                //        break;
                //    }
                //    bBreakFlg = true;
                //    break;
                //}
                ////リターンコードが200なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                //"{\r\n  \"translations\": [\r\n    {\r\n      \"detected_source_language\": \"EN\",\r\n      \"text\": \"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\\u3002\"\r\n    }\r\n  ]\r\n}\r\n"
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22: \x22", "\\x22") == false)
                {
#if DEBUG
                    Assembly myAssembly = Assembly.GetEntryAssembly();
                    string strSystemPath = myAssembly.Location;
                    var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    //Console.WriteLine("path = " + strExecPath);
                    File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
                    //Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
#endif
                    bBreakFlg = true;
                    break;
                }
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                //strTranslator = "DeepL";
                //strJSON = ConvertJsonToStringRegex(strJSON);
                var bEncordedEscapeSequenceUnicode = false;
                strRet = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
#if DEBUG
                ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + "Python API call MSG = " + strSource);
#endif
            }
            while (false);//空ループなので、一回処理をしたら必ず抜ける
            return strRet;
        }
        private string SyncCallTranslationGAS(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            bBreakFlg = false;
            strTranslator = "Google";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            var strSendJson = "";

            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageTarget = "";

            var strRet = "";

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    bBreakFlg = true;
                    break;
                }
                if ((strTranslation.Equals("Google Apps Scripts") || strTranslation.Equals("DeepL API and Google Apps Scripts")) == false)
                {
                    bBreakFlg = true;
                    break;
                }

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //jsonに変換する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strGoogleAppsScriptsURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    strSendJson = "{\"value\":\"" + strSource.Trim() + "\"," +
                                    "\"source\":\"" + strLangSource + "\"," +
                                    "\"target\":\"" + strLangTarget + "\"}";
                    streamWriter.Write(strSendJson);
                    //Console.WriteLine(json);
                }
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //Google Apps ScriptのWEBAPIに投げ、受け取ったjsonから翻訳後の文字列を抜き出す
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        strJSON = streamReader.ReadToEnd();
                    }
                }
                catch
                {
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Apps Scripts call error " + strSource);
#endif
                }
                //{"code":200,"text":"これはテストメッセージです"}
                //リターンコードを取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "{\\x22code\\x22.", ",") == false)
                {
                    bBreakFlg = true;
                    break;
                }
                if (strCode.Equals("200") == false)
                {
                    //200以外ならエラーメッセージを取得する。
                    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                    {
                        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Apps Scripts call error " + strCode + " = " + strMessageError);
                        bBreakFlg = true;
                        break;
                    }
                    bBreakFlg = true;
                    break;
                }
                //リターンコードが200なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Apps Scripts call error");
                    //Assembly myAssembly = Assembly.GetEntryAssembly();
                    //string strSystemPath = myAssembly.Location;
                    //var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    ////Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", result);
                    ////Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    //ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " error\t" + j + "\t" + "\"" + strSource + "\"");
#endif
                    bBreakFlg = true;
                    break;
                }

                //strTranslator = "Google";
                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
                strRet = strMessageTarget;
#if DEBUG
                ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " call MSG = " + strSource);
#endif
            }
            while (false);//空ループなので、一回処理をしたら必ず抜ける
            return strRet;
        }
        public string SyncCallTranslationBaidu(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            bBreakFlg = false;
            strTranslator = "Baidu";
            var strRet = "";

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strBaiduID = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu ID", strIniFileName);
            var strBaiduPASS = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Baidu PASS", strIniFileName);

            var strAfterTranslation = "";

            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageTarget = "";

            do
            {
                if (strTranslation.Equals("Baidu Trans Web API") == false)
                {
                    break;
                }

                //ES = spa
                //FR = fra
                //JA = jp
                //KO = kor
                if (strLangSource.Equals("ES") != false)
                {
                    strLangSource = "spa";
                }
                if (strLangTarget.Equals("ES") != false)
                {
                    strLangTarget = "spa";
                }
                if (strLangSource.Equals("FR") != false)
                {
                    strLangSource = "fra";
                }
                if (strLangTarget.Equals("FR") != false)
                {
                    strLangTarget = "fra";
                }
                if (strLangSource.Equals("JA") != false)
                {
                    strLangSource = "jp";
                }
                if (strLangTarget.Equals("JA") != false)
                {
                    strLangTarget = "jp";
                }
                if (strLangSource.Equals("KO") != false)
                {
                    strLangSource = "kor";
                }
                if (strLangTarget.Equals("KO") != false)
                {
                    strLangTarget = "kor";
                }

                // 原文
                string q = strSource;
                // 源语言
                string from = strLangSource.ToLower();
                // 目标语言
                string to = strLangTarget.ToLower();
                // 改成您的APP ID
                string appId = strBaiduID;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                // 改成您的密钥
                string secretKey = strBaiduPASS;
                string sign = EncryptString(appId + q + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                url += "q=" + HttpUtility.UrlEncode(q);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 6000;

                string retString = "";
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
                catch
                {
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Translation time out");
                }

                strJSON = retString;

                strTranslator = "Baidu";
                //result = ConvertJsonToStringBaidu(result);

                ////リターンコードを取得する。
                //if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "{\\x22code\\x22.", ",") == false)
                //{
                //    bBreakFlg = true;
                //    break;
                //}
                //if (strCode.Equals("200") == false)
                //{
                //    //200以外ならエラーメッセージを取得する。
                //    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                //    {
                //        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Apps Scripts call error " + strCode + " = " + strMessageError);
                //        bBreakFlg = true;
                //        break;
                //    }
                //    bBreakFlg = true;
                //    break;
                //}
                ////リターンコードが200なら正常終了

                //"{\"from\":\"en\",\"to\":\"jp\",\"trans_result\":[{\"src\":\"this is a test message\",\"dst\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\"}]}"
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, ".*dst\x22:\x22", "\\x22") == false)
                {
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " Apps Scripts call error");
                    //Assembly myAssembly = Assembly.GetEntryAssembly();
                    //string strSystemPath = myAssembly.Location;
                    //var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
                    ////Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", result);
                    ////Console.WriteLine("path = " + strExecPath);
                    //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
                    //ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + j + "\t" + "\"" + strSource + "\"");
#endif
                    bBreakFlg = true;
                    break;
                }

                var bEncordedEscapeSequenceUnicode = false;
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
#if DEBUG
                ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call MSG = " + strSource);
#endif
                strRet = strMessageTarget;
            }
            while (false);
            return strRet;
        }
        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        public string SyncCallTranslationTexTra(DateTime dtmAddLogDate, string strSource, string strLangTarget, string strLangSource, ref string strTranslator, string strLogType, ref bool bBreakFlg)
        {
            strTranslator = "TexTra";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strTexTraAPIName = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIName", strIniFileName);
            var strTexTraAPIKEY = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPIKEY", strIniFileName);
            var strTexTraAPISecret = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "TexTraAPISecret", strIniFileName);


            var bEncordedEscapeSequenceUnicode = false;

            var strJSON = "";
            var strCode = "";
            var strMessageError = "";
            var strMessageTarget = "";

            bBreakFlg = false;

            do//空ループ
            {
                if (strTranslation.Equals("none") || strTranslation.Equals(""))
                {
                    bBreakFlg = true;
                    break;
                }
                if (strLangTarget.Equals(strLangSource))
                {
                    bBreakFlg = true;
                    break;
                }
                strLangSource = strLangSource.ToLower();
                strLangTarget = strLangTarget.ToLower();
                if (strLangSource == "zh-cn")
                {
                    strLangSource = "zh-CN";
                }
                if (strLangSource == "zh-tw")
                {
                    strLangSource = "zh-TW";
                }
                if (strLangTarget == "zh-cn")
                {
                    strLangTarget = "zh-CN";
                }
                if (strLangTarget == "zh-tw")
                {
                    strLangTarget = "zh-TW";
                }
                if (strTranslation.Equals("TexTra API") == false)
                {
                    bBreakFlg = true;
                    break;
                }
                string URL_VAL = "https://mt-auto-minhon-mlt.ucri.jgn-x.jp/api/mt/generalNT" + "_" + strLangSource + "_" + strLangTarget + "/";
                string TYPE = "json";
                string TEXT = strSource;

                //stop();

                //HttpConnectionOAuthのインスタンス化
                HttpConnection httpCon = new HttpConnection();

                //stop();

                //初期化（コンシューマーキーの設定など）
                httpCon.Initialize(strTexTraAPIKEY, strTexTraAPISecret, "", "", "");

                //stop();

                //取得
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("key", strTexTraAPIKEY);
                param.Add("name", strTexTraAPIName);
                param.Add("type", TYPE);
                param.Add("text", TEXT);
                // その他のパラメータについては、各APIのリクエストパラメータに従って設定してください。

                //stop();

                HttpStatusCode statusCode = httpCon.GetContent("POST", new Uri(URL_VAL), param, ref strJSON, null);

                //{"resultset":{"code":0,"message":"","request":{"url":"https:\/\/mt-auto-minhon-mlt.ucri.jgn-x.jp\/api\/mt\/generalNT_en_ja\/","text":"test","split":0,"history":0,"xml":null,"data":null},"result":{"text":"\u30c6\u30b9\u30c8","information":{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","sentence":[{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","split":[{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","process":{"regex":[],"replace-before":[],"short-before":[],"preprocess":[],"translate":{"reverse":[],"specification":[],"text-s":"test","src-token":["test"],"text-t":"\u30c6\u30b9\u30c8","associate":[[],[],[[["test"],["\u30c6\u30b9\u30c8"]]]],"oov":null,"exception":"","associates":[[[],[],[[["test"],["\u30c6\u30b9\u30c8"]]]]]},"short-after":[],"replace-after":[]}}]}]}}}}
                //リターンコードを取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strCode, "resultset.*code\x22.", ",") == false)
                {
                    bBreakFlg = true;
                    break;
                }
                if (strCode.Equals("0") == false)
                {
                    //0以外ならエラーメッセージを取得する。
                    if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageError, "resultset.*code.*message\x22:\x22", "\\x22") == false)
                    {
                        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                        bBreakFlg = true;
                        break;
                    }
                    bBreakFlg = true;
                    break;
                }
                //リターンコードが0なら正常終了
                //"result":{"text":"\u30c6\u30b9\u30c8"
                //翻訳文字列を取得する。
                if (ConvertJsonToStringRegexGeneralPurpose(strJSON, ref strMessageTarget, "result.*text\x22:\x22", "\\x22") == false)
                {
                    bBreakFlg = true;
                    break;
                }
                strTranslator = "TexTra";
                strMessageTarget = ConvertUnicodeEscapeSequenceToText(strMessageTarget, ref bEncordedEscapeSequenceUnicode);
                if (bEncordedEscapeSequenceUnicode != false)
                {
                    strTranslator += " U";
                }
#if DEBUG
                ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call MSG = " + strSource);
#endif
            } while (false);//空ループなので、一回処理をしたら必ず抜ける
            return strMessageTarget;
        }
        private string ConvertJsonToStringDeepL(string strData)
        {
            //Newtonsoft.Json.Linqを利用したが、エラーで落ちるので、正規表現を利用した自力パース関数に戻す
            var strReturn = "";
            do
            {
                if (strData.Equals(""))
                {
                    break;
                }
                JObject jsonObj = JObject.Parse(strData);
                foreach (JObject key1Item in jsonObj["translations"])
                {
                    strReturn = key1Item["text"].ToString();
                }
            }
            while (false);
            return strReturn;
        }
        private string ConvertJsonToStringGAS(string strData)
        {
            //Newtonsoft.Json.Linqを利用したが、エラーで落ちるので、正規表現を利用した自力パース関数に戻す
            var strReturn = "";
            do
            {
                if (strData.Equals(""))
                {
                    break;
                }
                JObject jsonObj = JObject.Parse(strData);
                strReturn = jsonObj["text"].ToString();
            }
            while (false);
            return strReturn;
        }
        private string ConvertJsonToStringBaidu(string strData)
        {
            //Newtonsoft.Json.Linqを利用したが、エラーで落ちるので、正規表現を利用した自力パース関数に戻す
            var strReturn = "";
            do
            {
                if (strData.Equals(""))
                {
                    break;
                }

                JObject jsonObj = JObject.Parse(strData);
                foreach (JObject key1Item in jsonObj["trans_result"])
                {
                    strReturn = key1Item["dst"].ToString();
                }
            }
            while (false);
            return strReturn;
        }
        private Boolean ConvertJsonToStringRegexGeneralPurpose(string strJSON, ref string strParse, string strRegexHeader, string strRegexFooter)
        {
            var bRet = false;
            strParse = "";
            do
            {
                var IsMatchRegexHeader = Regex.Match(strJSON, strRegexHeader);
                if (IsMatchRegexHeader.Success == false)
                {
                    //ヘッダが正規表現で見つからない
                    break;
                }
                //ヘッダが正規表現で取得できた
                var strRegexHeaderAfter = strJSON.Substring(IsMatchRegexHeader.Index + IsMatchRegexHeader.Length);

                //フッタを正規表現で探す
                var IsMatchRegexFooter = Regex.Match(strRegexHeaderAfter, strRegexFooter);
                if (IsMatchRegexFooter.Success == false)
                {
                    //フッタが正規表現で見つからない
                    break;
                }
                //ヘッダとフッタがそろった
                strParse = strRegexHeaderAfter.Substring(0, IsMatchRegexFooter.Index);
                bRet = true;
            } while (false);
            return bRet;
        }
        private string ConvertJsonToStringRegex(string strData)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //受け取ったjsonからデータ部分を抜き出す
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            var strTmp2 = "";
            var strTmp = "";
            var bFlg = false;
            for (int i = strData.Length - 1; 0 <= i; i--)
            {
                strTmp = strData.Substring(i, 1);
                if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline))
                {
                    if (bFlg == true)
                    {
                        strTmp2 = strTmp + strTmp2;
                        break;
                    }
                    else
                    {
                        bFlg = true;
                    }
                }
                if (bFlg == true)
                {
                    //最後のダブルクォートから、一つ前のダブルクォートまでを抜き出す
                    strTmp2 = strTmp + strTmp2;
                }
            }
            strTmp = "";
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //抜き出した文字列の先頭と最後のダブルクォートを外す
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Console.WriteLine(strData);
            var strReturn = "";
            for (int i = 1; i < strTmp2.Length - 1; i++)
            {
                strTmp = strTmp2.Substring(i, 1);
                strReturn = strReturn + strTmp;
            }
            return strReturn;
        }
        private string ConvertJsonToStringTexTra(string strData)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //受け取ったjsonからデータ部分を抜き出す
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            var strTmp2 = "";
            var strTmp = "";
            //"{\"resultset\":{\"code\":0,\"message\":\"\",\"request\":{\"url\":\"https:\\/\\/mt-auto-minhon-mlt.ucri.jgn-x.jp\\/api\\/mt\\/generalNT_en_ja\\/\",\"text\":\"this is a test message\",\"split\":0,\"history\":0,\"xml\":null,\"term_id\":\"all\",\"bilingual_id\":\"all\",\"log_use\":2,\"editor_use\":1,\"data\":null},\"result\":{\"text\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\",\"blank\":0,\"information\":{\"text-s\":\"this is a test message\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\",\"sentence\":[{\"text-s\":\"this is a test message\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\",\"split\":[{\"text-s\":\"this is a test message\",\"text-t\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\",\"process\":{\"regex\":[],\"replace-before\":[],\"short-before\":[],\"preprocess\":[],\"translate\":{\"reverse\":[],\"specification\":[],\"text-s\":\"this is a test message\",\"src-token\":[\"this\",\"is\",\"a\",\"test\",\"message\"],\"text-t\":\"\\u3053\\u308c\\u306f\\u30c6\\u30b9\\u30c8\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\\u3067\\u3059\",\"associate\":[[],[],[[[\"this\"],[\"\\u3053\\u308c\"]],[[\"is\"],[\"\\u3067\\u3059\"]],[[\"a\"],[\"\\u306f\"]],[[\"test\"],[\"\\u30c6\\u30b9\\u30c8\"]],[[\"message\"],[\"\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\"]]]],\"oov\":null,\"exception\":\"\",\"associates\":[[[],[],[[[\"this\"],[\"\\u3053\\u308c\"]],[[\"is\"],[\"\\u3067\\u3059\"]],[[\"a\"],[\"\\u306f\"]],[[\"test\"],[\"\\u30c6\\u30b9\\u30c8\"]],[[\"message\"],[\"\\u30e1\\u30c3\\u30bb\\u30fc\\u30b8\"]]]]]},\"short-after\":[],\"replace-after\":[]}}]}]}}}}"
            //{"resultset":{"code":0,"message":"","request":{"url":"https:\/\/mt-auto-minhon-mlt.ucri.jgn-x.jp\/api\/mt\/generalNT_en_ja\/","text":"test","split":0,"history":0,"xml":null,"data":null},"result":{"text":"\u30c6\u30b9\u30c8","information":{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","sentence":[{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","split":[{"text-s":"test","text-t":"\u30c6\u30b9\u30c8","process":{"regex":[],"replace-before":[],"short-before":[],"preprocess":[],"translate":{"reverse":[],"specification":[],"text-s":"test","src-token":["test"],"text-t":"\u30c6\u30b9\u30c8","associate":[[],[],[[["test"],["\u30c6\u30b9\u30c8"]]]],"oov":null,"exception":"","associates":[[[],[],[[["test"],["\u30c6\u30b9\u30c8"]]]]]},"short-after":[],"replace-after":[]}}]}]}}}}
            //stop();

            var m = Regex.Match(strData, "\\x22result\\x22:{\\x22text\\x22:\\x22");
            if (m.Success)
            {
                var n = Regex.Match(strData.Substring(m.Index + m.Length), "\\x22");
                do
                {
                    // マッチする箇所があった場合、マッチした部分の文字列と、インデックス・長さを表示する
                    Console.WriteLine("{0,-10} ({1}, {2})", m.Value, m.Index, m.Length);
                    for (int i = m.Index + m.Length; i < strData.Length - 1; i++)
                    {
                        strTmp = strData.Substring(i, 1);
                        if (strTmp.Equals("\\x22"))
                        {
                            break;
                        }
                        strTmp2 += strTmp;
                        if (strTmp2.Length == n.Index)
                        {
                            //                            MessageBox.Show(strTmp2);
                            break;
                        }
                    }
                }
                while (false);
                //                MessageBox.Show(strTmp2);
            }
            else
            {
                // マッチする箇所がなかった場合
                Console.WriteLine("?");
            }
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //抜き出した文字列の先頭と最後のダブルクォートを外す
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Console.WriteLine(strData);
            return strTmp2;
        }
        private void DictionarySlangLoad()
        {
            List<string> strSlangDictionary = new List<string>();

            var pathFrom = System.IO.Directory.GetFiles(System.Environment.CurrentDirectory, "Slang*.csv");
            for (int i = 0; i < pathFrom.Count(); i++)
            {
                string strFileName = pathFrom[i].Replace(System.Environment.CurrentDirectory + "\\", "");
                strSlangDictionary.Add(strFileName);
            }

            for (int i = 0;
                i < strSlangDictionary.Count;
                i++)
            {
                if (strSlangDictionary[i].Equals("") == true)
                {
                    continue;
                }
                // 外部ファイルからスラングのリストを取得
                StreamReader sr = new StreamReader(strSlangDictionary[i], Encoding.GetEncoding("UTF-8"));
                string buf = sr.ReadToEnd();
                // 改行コードで分割する
                string[] list = Regex.Split(buf, "\r\n|\n");
                for (var j = 0; j < list.Length; j++)
                {
                    do
                    {
                        if (list[j].Contains(",") == false)
                        {
                            // 区切り文字がなければスキップ
                            continue;
                        }
                        // 構造体型の変数作成
                        tyDictionarySlang SD = new tyDictionarySlang();
                        // 値の代入
                        //stop();
                        SD.Slang = " " + list[j].Trim().Substring(0, list[j].IndexOf(",")) + " ";
                        SD.Dictionary = " " + list[j].Trim().Substring(list[j].IndexOf(",") + 1) + " ";
                        // List への追加
                        stDictionarySlang.Add(SD);
                    } while (false);
                }
            }
            int sn = stDictionarySlang.Count;
            for (int i = 0; i < sn; i++)
            {
                //配列の回数分回す
                for (int j = i; j < sn; j++)
                {
                    //比較元より大きければ入れ替え
                    if (stDictionarySlang[i].Slang.CompareTo(stDictionarySlang[j].Slang) == -1)
                    {
                        tyDictionarySlang x;
                        x = stDictionarySlang[j];
                        stDictionarySlang[j] = stDictionarySlang[i];
                        stDictionarySlang[i] = x;
                    }
                }
            }
        }
        private void ShortTextRegistrationLoad()
        {
            stShortTextRegistration.Clear();
            var strFileName = System.Environment.CurrentDirectory + "\\ShortTextRegistration.csv";
            do
            {
                if (strFileName.Equals(""))
                {
                    break; ;
                }
                if (File.Exists(strFileName) == false)
                {
                    break; ;
                }
                // 外部ファイルからスラングのリストを取得
                StreamReader sr = new StreamReader(strFileName, Encoding.GetEncoding("UTF-8"));
                string buf = sr.ReadToEnd();
                // 改行コードで分割する
                string[] list = Regex.Split(buf, "\r\n|\n");
                for (var j = 0; j < list.Length; j++)
                {
                    do
                    {
                        if (list[j].Contains(",") == false)
                        {
                            // 区切り文字がなければスキップ
                            continue;
                        }
                        // 構造体型の変数作成
                        tyShortTextRegistration STR = new tyShortTextRegistration();
                        // 値の代入
                        STR.iNum = Int32.Parse(list[j].Substring(0, list[j].IndexOf(",")));
                        STR.ShortText = list[j].Substring(list[j].IndexOf(",") + 1);
                        // List への追加
                        stShortTextRegistration.Add(STR);
                    } while (false);
                }
            } while (false);
        }
        private void ShortTextRegistrationWrite(int iSTRNo, string strShortTextRegistration)
        {
            do
            {
                for (int i = 0; i < stShortTextRegistration.Count - 1; i++)
                {
                    if (stShortTextRegistration[i].iNum == iSTRNo)
                    {
                        // 構造体型の変数作成
                        tyShortTextRegistration STR = new tyShortTextRegistration();
                        // 値の代入
                        STR.iNum = iSTRNo;
                        STR.ShortText = strShortTextRegistration;
                        stShortTextRegistration[i] = STR;
                    }
                }
            } while (false);
        }
        private void ShortTextRegistrationSave()
        {
            var strFileName = System.Environment.CurrentDirectory + "\\ShortTextRegistration.csv";

            var strLine = "";
            var saveData = "";
            do
            {
                for (int i = 0; i < stShortTextRegistration.Count; i++)
                {
                    saveData += stShortTextRegistration[i].iNum + "," + stShortTextRegistration[i].ShortText + Environment.NewLine;
                }
            } while (false);
            //stop();
            System.IO.File.WriteAllText(strFileName, saveData, Encoding.GetEncoding("UTF-8"));
        }
        private string ConvertDictionarySlang(string strData, ref bool bHitFlg)
        {
            bHitFlg = false;
            var strInput = " " + Microsoft.VisualBasic.Strings.StrConv(strData, VbStrConv.Lowercase) + " ";
            var strExt = strInput;

            var strFirstLanguage_GP = Language_GP.myLanguage_GP();
            var strLangSource = "";
            do
            {
                if (strFirstLanguage_GP.ToUpper() == "JA")
                {
                    //JP
                    for (int i = 0; i < stDictionarySlang.Count; i++)
                    {
                        strExt = strExt.Replace(stDictionarySlang[i].Slang, stDictionarySlang[i].Dictionary);
                    }
                    if (strExt.Equals(strInput) == false)
                    {
                        bHitFlg = true;
                    }
                    strExt = strExt.Trim();
                    break;
                }
                //stop();
                //JP以外
                for (int i = 0; i < stDictionarySlang.Count - 1; i++)
                {
                    if (isOneByteChar(stDictionarySlang[i].Dictionary) == false)
                    {
                        continue;
                    }    
                    strExt = strExt.Replace(stDictionarySlang[i].Slang, stDictionarySlang[i].Dictionary);
                }
                if (strExt.Equals(strInput) == false)
                {
                    bHitFlg = true;
                }
                strExt = strExt.Trim();
            } while (false);
            return strExt;
        }
        private string ConvertCipherToCyrillic(DateTime dtmAddLogDate, string strLogType, string strSource)
        {
            var strRet = "";


            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strCyrillicConvert = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "CyrillicConvert", strIniFileName);

            string strGarbledCharacters = @"\p{IsLatin-1Supplement}|\p{IsLatinExtended-A}|\p{IsLatinExtended-B}";
            Regex GarbledCharacters = new Regex(strGarbledCharacters);
            do
            {
                if (strCyrillicConvert.Equals("true") == false)
                {
                    strRet = strSource;
                    break;
                }
                // キリル文字が一定以上の割合で存在しない場合は変換しない。
                var numCyrillicCount = 0;
                var n = System.Math.Min(strSource.Length, 100);
                for (var i = 0; i < n; i++)
                {
                    var t = strSource.Substring(i, 1);
                    if (GarbledCharacters.IsMatch(t))
                    {
                        numCyrillicCount++;
                    }
                }
                double dblCyrillicCount = numCyrillicCount;
                double dblALL = strSource.Length;
                if ((dblCyrillicCount / dblALL) < 0.8)
                {
                    strRet = strSource;
                    break;
                }
                UnicodeEncoding unicode = new UnicodeEncoding(true, false);
                string input = strSource;
                Encoding EncordSource = Encoding.GetEncoding("windows-1252");
                Encoding EncordDestination = Encoding.GetEncoding("windows-1251");
                var output = EncordDestination.GetString(EncordSource.GetBytes(input));

                //U+0400-U+04FF U+0500-U+052F U+2DE0-U+2DFF U+A640-U+A69F
                string strCyrillic = @"\p{IsCyrillic}";
                Regex Cyrillic = new Regex(strCyrillic);
                if (Cyrillic.IsMatch(output))
                {
                    //ok
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Is Cyrillic = " + strSource);
#endif
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Is Cyrillic = " + output);
#endif
                    strRet = output;
                }
                else
                {
                    //文字化け
                    strRet = strSource;
                    //stop();
                }
            }
            while (false);

            return strRet;
        }
        private string ConvertCyrillicToCipher(DateTime dtmAddLogDate, string strLogType, string strSource)
        {
            var strRet = "";

            //stop();
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strCyrillicConvert = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "CyrillicConvert", strIniFileName);

            string strGarbledCharacters = @"\p{IsLatin-1Supplement}|\p{IsLatinExtended-A}|\p{IsLatinExtended-B}";
            Regex GarbledCharacters = new Regex(strGarbledCharacters);
            do
            {
                //if (strCyrillicConvert.Equals("true") == false)
                //{
                //    strRet = strSource;
                //    break;
                //}
                //// キリル文字が一定以上の割合で存在しない場合は変換しない。
                //var numCyrillicCount = 0;
                //var n = System.Math.Min(strSource.Length, 100);
                //for (var i = 0; i < n; i++)
                //{
                //    var t = strSource.Substring(i, 1);
                //    if (GarbledCharacters.IsMatch(t))
                //    {
                //        numCyrillicCount++;
                //    }
                //}
                //double dblCyrillicCount = numCyrillicCount;
                //double dblALL = strSource.Length;
                //if ((dblCyrillicCount / dblALL) < 0.8)
                //{
                //    strRet = strSource;
                //    break;
                //}
                UnicodeEncoding unicode = new UnicodeEncoding(true, false);
                string input = strSource;
                Encoding EncordSource = Encoding.GetEncoding("windows-1251");
                Encoding EncordDestination = Encoding.GetEncoding("windows-1252");
                var output = EncordDestination.GetString(EncordSource.GetBytes(input));

                //U+0400-U+04FF U+0500-U+052F U+2DE0-U+2DFF U+A640-U+A69F
                string strCyrillic = @"\p{IsCyrillic}";
                Regex Cyrillic = new Regex(strCyrillic);
                //if (Cyrillic.IsMatch(output))
                //{
                    //ok
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Is Cyrillic = " + strSource);
#endif
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "Is Cyrillic = " + output);
#endif
                    strRet = output;
                //}
                //else
                //{
                //    //文字化け
                //    strRet = strSource;
                //    //stop();
                //}
            }
            while (false);

            return strRet;
        }
        private string ConvertUnicodeEscapeSequenceToText(string strData, ref bool bEncordedEscapeSequenceUnicode)
        {
            var strTmp = "";
            var strTmp2 = "";

            for (int i = 0; i < strData.Length; i++)
            {
                if (strData.Substring(i).Length > 2)
                {
                    if (strData.Substring(i, 2).Equals("\\u"))
                    {
                        //"\uを発見。もしかしたらunicodeエンコードされた文字かも知れない。"
                        strTmp = strData.Substring(i + 2, 4);
                        try
                        {
                            //続く4文字を16進数とみなし、数値型に変換できるかトライ
                            int charCode16 = Convert.ToInt32(strTmp, 16);  // 16進数文字列 -> 数値
                            char c = Convert.ToChar(charCode16);  // 数値(文字コード) -> 文字
                            string newChar = c.ToString();    // "" という「文字列」}
                            strTmp2 = strTmp2 + newChar;
                            bEncordedEscapeSequenceUnicode = true;
                            i = i + 5;
                        }
                        catch
                        {
                            //偶然の一致。普通の文字。バッファに文字を登録し、次の文字へ。
                            strTmp2 = strTmp2 + strData.Substring(i, 1);
                        }
                    }
                    else
                    {
                        //普通の文字。バッファに文字を登録し、次の文字へ。
                        strTmp2 = strTmp2 + strData.Substring(i, 1);
                    }
                }
                else
                {
                    //普通の文字。バッファに文字を登録し、次の文字へ。
                    strTmp2 = strTmp2 + strData.Substring(i, 1);
                }
            }
            return strTmp2;

        }
        public static string AutoJudgementCJKCheckRegex(string strSource)
        {
            string strJapaneseKana = @"\p{IsHiragana}|\p{IsKatakana}";
            string strKanji = @"\p{IsCJKUnifiedIdeographs}";
            // ↓Pythonスクリプトで生成したやつをコピペ
            string strSimplifiedChinese = @"\u3437|\u3439|\u343D|\u3447|\u3448|\u3454|\u3469|\u347A|\u34E5|\u3509|\u358A|\u359E|\u360E|\u36AF|\u36C0|\u36DF|\u36E0|\u36E3|\u36E4|\u36FF|\u3766|\u37C6|\u37DC|\u37E5|\u384E|\u3916|\u3918|\u392D|\u393D|\u396A|\u3988|\u39CF|\u39D0|\u39D1|\u39DB|\u39DF|\u39F0|\u3A2B|\u3B4E|\u3B4F|\u3B63|\u3B64|\u3B74|\u3BA0|\u3C69|\u3C6E|\u3CBF|\u3CD4|\u3CD5|\u3CE0|\u3CE1|\u3CE2|\u3CFD|\u3D0B|\u3D89|\u3DB6|\u3DBD|\u3DEA|\u3E8D|\u3EC5|\u3ECF|\u3ED8|\u3EEA|\u3FA1|\u4025|\u4056|\u40B5|\u40C5|\u4149|\u415F|\u416A|\u41DA|\u41F2|\u4264|\u4336|\u4337|\u4338|\u4339|\u433A|\u433B|\u433C|\u433D|\u433E|\u433F|\u4340|\u4341|\u43AC|\u43DD|\u442A|\u44D3|\u44D5|\u45BC|\u45D6|\u461B|\u461E|\u464A|\u464C|\u4653|\u46D3|\u4723|\u4724|\u4725|\u4727|\u4729|\u4759|\u478C|\u478D|\u478E|\u4790|\u47E2|\u4880|\u4881|\u4882|\u497A|\u497D|\u497E|\u497F|\u4980|\u4981|\u4982|\u4983|\u4985|\u4986|\u49B6|\u49B7|\u4A44|\u4B6A|\u4BC3|\u4BC4|\u4BC5|\u4C9D|\u4C9E|\u4C9F|\u4CA0|\u4CA1|\u4CA2|\u4CA3|\u4CA4|\u4D13|\u4D14|\u4D15|\u4D16|\u4D17|\u4D18|\u4D19|\u4DAE|\u4E07|\u4E0E|\u4E11|\u4E13|\u4E1A|\u4E1B|\u4E1C|\u4E1D|\u4E22|\u4E24|\u4E25|\u4E27|\u4E2A|\u4E30|\u4E34|\u4E3A|\u4E3D|\u4E3E|\u4E48|\u4E49|\u4E4C|\u4E50|\u4E54|\u4E60|\u4E61|\u4E66|\u4E70|\u4E71|\u4E89|\u4E8E|\u4E8F|\u4E91|\u4E9A|\u4EA7|\u4EA9|\u4EB2|\u4EB5|\u4EB8|\u4EBF|\u4EC5|\u4EC6|\u4ECE|\u4ED1|\u4ED3|\u4EEA|\u4EEC|\u4EF7|\u4F17|\u4F18|\u4F1A|\u4F1B|\u4F1E|\u4F1F|\u4F20|\u4F21|\u4F23|\u4F24|\u4F25|\u4F26|\u4F27|\u4F2A|\u4F2B|\u4F53|\u4F59|\u4F63|\u4F65|\u4FA0|\u4FA3|\u4FA5|\u4FA6|\u4FA7|\u4FA8|\u4FA9|\u4FAA|\u4FAC|\u4FE3|\u4FE6|\u4FE8|\u4FE9|\u4FEA|\u4FEB|\u4FED|\u503A|\u503E|\u506C|\u507B|\u507E|\u507F|\u50A5|\u50A7|\u50A8|\u50A9|\u513F|\u514B|\u5151|\u5156|\u515A|\u5170|\u5173|\u5174|\u5179|\u517B|\u517D|\u5181|\u5185|\u5188|\u518C|\u5199|\u519B|\u519C|\u51AF|\u51B2|\u51B3|\u51B5|\u51BB|\u51C0|\u51C6|\u51C9|\u51CF|\u51D1|\u51DB|\u51E0|\u51E4|\u51EB|\u51ED|\u51EF|\u51FA|\u51FB|\u51FF|\u520D|\u5212|\u5218|\u5219|\u521A|\u521B|\u5220|\u522B|\u522C|\u522D|\u522E|\u5236|\u5239|\u523D|\u523E|\u523F|\u5240|\u5242|\u5250|\u5251|\u5265|\u5267|\u529D|\u529E|\u52A1|\u52A2|\u52A8|\u52B1|\u52B2|\u52B3|\u52BF|\u52CB|\u52DA|\u5300|\u5326|\u532E|\u533A|\u533B|\u534E|\u534F|\u5355|\u5356|\u5362|\u5364|\u536B|\u5374|\u5382|\u5385|\u5386|\u5389|\u538B|\u538C|\u538D|\u5390|\u5395|\u5398|\u53A2|\u53A3|\u53A6|\u53A8|\u53A9|\u53AE|\u53BF|\u53C1|\u53C2|\u53C6|\u53C7|\u53CC|\u53D1|\u53D8|\u53D9|\u53E0|\u53EA|\u53F0|\u53F6|\u53F7|\u53F9|\u53FD|\u540C|\u540E|\u5411|\u5413|\u5415|\u5417|\u5423|\u5428|\u542C|\u542F|\u5434|\u5450|\u5452|\u5453|\u5455|\u5456|\u5457|\u5458|\u5459|\u545B|\u545C|\u548F|\u5499|\u549B|\u549D|\u54A4|\u54B8|\u54CD|\u54D1|\u54D2|\u54D3|\u54D4|\u54D5|\u54D7|\u54D9|\u54DC|\u54DD|\u54DF|\u551B|\u551D|\u5520|\u5521|\u5522|\u5524|\u5567|\u556C|\u556D|\u556E|\u556F|\u5570|\u5574|\u5578|\u55B7|\u55BD|\u55BE|\u55EB|\u55F3|\u5618|\u5624|\u5631|\u565C|\u56A3|\u56E2|\u56ED|\u56F0|\u56F1|\u56F4|\u56F5|\u56FD|\u56FE|\u5706|\u5723|\u5739|\u573A|\u5742|\u574F|\u5757|\u575A|\u575B|\u575C|\u575D|\u575E|\u575F|\u5760|\u5784|\u5785|\u5786|\u5792|\u57A6|\u57A9|\u57AB|\u57AD|\u57AF|\u57B1|\u57B2|\u57B4|\u57D8|\u57D9|\u57DA|\u57EF|\u5811|\u5815|\u5846|\u5899|\u58EE|\u58F0|\u58F3|\u58F6|\u58F8|\u5904|\u5907|\u590D|\u591F|\u5934|\u5938|\u5939|\u593A|\u5941|\u5942|\u594B|\u5956|\u5965|\u5968|\u5978|\u5986|\u5987|\u5988|\u59A9|\u59AA|\u59AB|\u59D7|\u59F9|\u5A04|\u5A05|\u5A06|\u5A07|\u5A08|\u5A31|\u5A32|\u5A34|\u5A73|\u5A74|\u5A75|\u5A76|\u5AAA|\u5AAD|\u5AD2|\u5AD4|\u5AF1|\u5B37|\u5B59|\u5B66|\u5B6A|\u5B81|\u5B9D|\u5B9E|\u5BA0|\u5BA1|\u5BAA|\u5BAB|\u5BBD|\u5BBE|\u5BDD|\u5BF9|\u5BFB|\u5BFC|\u5BFF|\u5C06|\u5C14|\u5C18|\u5C1D|\u5C27|\u5C34|\u5C38|\u5C3D|\u5C42|\u5C43|\u5C49|\u5C4A|\u5C5E|\u5C61|\u5C66|\u5C7F|\u5C81|\u5C82|\u5C96|\u5C97|\u5C98|\u5C99|\u5C9A|\u5C9B|\u5CAD|\u5CBD|\u5CBF|\u5CC4|\u5CE1|\u5CE3|\u5CE4|\u5CE5|\u5CE6|\u5D02|\u5D03|\u5D04|\u5D2D|\u5D58|\u5D5A|\u5D5D|\u5DC5|\u5DE9|\u5DEF|\u5E01|\u5E05|\u5E08|\u5E0F|\u5E10|\u5E18|\u5E1C|\u5E26|\u5E27|\u5E2E|\u5E31|\u5E3B|\u5E3C|\u5E42|\u5E72|\u5E76|\u5E7F|\u5E84|\u5E86|\u5E90|\u5E91|\u5E93|\u5E94|\u5E99|\u5E9E|\u5E9F|\u5EEA|\u5F00|\u5F02|\u5F03|\u5F11|\u5F20|\u5F25|\u5F2A|\u5F2F|\u5F39|\u5F3A|\u5F52|\u5F53|\u5F55|\u5F5D|\u5F5F|\u5F66|\u5F68|\u5F7B|\u5F81|\u5F84|\u5F95|\u5FA1|\u5FC6|\u5FCF|\u5FD7|\u5FE7|\u5FFE|\u6000|\u6001|\u6002|\u6003|\u6004|\u6005|\u6006|\u601C|\u603B|\u603C|\u603F|\u604B|\u6052|\u6073|\u6076|\u6078|\u6079|\u607A|\u607B|\u607C|\u607D|\u60A6|\u60AB|\u60AC|\u60AD|\u60AE|\u60AF|\u60CA|\u60E7|\u60E8|\u60E9|\u60EB|\u60EC|\u60ED|\u60EE|\u60EF|\u6120|\u6124|\u6126|\u613F|\u6151|\u616D|\u61D1|\u61D2|\u61D4|\u6206|\u620B|\u620F|\u6217|\u6218|\u622C|\u622F|\u6237|\u6251|\u6267|\u6269|\u626A|\u626B|\u626C|\u6270|\u629A|\u629B|\u629F|\u62A0|\u62A1|\u62A2|\u62A4|\u62A5|\u62C5|\u62DF|\u62E2|\u62E3|\u62E5|\u62E6|\u62E7|\u62E8|\u62E9|\u6302|\u6319|\u631A|\u631B|\u631C|\u631D|\u631E|\u631F|\u6320|\u6321|\u6322|\u6323|\u6324|\u6325|\u6326|\u633D|\u635D|\u635E|\u635F|\u6361|\u6362|\u6363|\u636E|\u63B3|\u63B4|\u63B7|\u63B8|\u63BA|\u63BC|\u63FD|\u63FE|\u63FF|\u6400|\u6401|\u6402|\u6405|\u643A|\u6444|\u6445|\u6446|\u6447|\u6448|\u644A|\u6484|\u6491|\u64B5|\u64B7|\u64B8|\u64BA|\u64DC|\u64DE|\u6512|\u654C|\u655B|\u6569|\u6570|\u658B|\u6593|\u6597|\u65A9|\u65AD|\u65E0|\u65E7|\u65F6|\u65F7|\u65F8|\u6619|\u663C|\u663D|\u663E|\u664B|\u6652|\u6653|\u6654|\u6655|\u6656|\u6682|\u6685|\u66A7|\u66F2|\u672F|\u6734|\u673A|\u6740|\u6742|\u6743|\u6746|\u6761|\u6765|\u6768|\u6769|\u6770|\u677E|\u677F|\u6781|\u6784|\u679E|\u67A2|\u67A3|\u67A5|\u67A7|\u67A8|\u67AA|\u67AB|\u67AD|\u67DC|\u67E0|\u67FD|\u6800|\u6805|\u6807|\u6808|\u6809|\u680A|\u680B|\u680C|\u680E|\u680F|\u6811|\u6816|\u6817|\u6837|\u683E|\u6860|\u6861|\u6862|\u6863|\u6864|\u6865|\u6866|\u6867|\u6868|\u6869|\u686A|\u68A6|\u68BC|\u68BE|\u68BF|\u68C0|\u68C1|\u68C2|\u6901|\u691D|\u691F|\u6920|\u6922|\u6924|\u692B|\u692D|\u692E|\u697C|\u6984|\u6985|\u6987|\u6988|\u6989|\u69DA|\u69DB|\u69DF|\u69E0|\u6A2A|\u6A2F|\u6A31|\u6A65|\u6A71|\u6A79|\u6A7C|\u6AA9|\u6B22|\u6B24|\u6B27|\u6B7C|\u6B81|\u6B87|\u6B8B|\u6B92|\u6B93|\u6B9A|\u6BA1|\u6BB4|\u6BC1|\u6BC2|\u6BD5|\u6BD9|\u6BE1|\u6BF5|\u6BF6|\u6C07|\u6C14|\u6C22|\u6C29|\u6C32|\u6C47|\u6C49|\u6C64|\u6C79|\u6C88|\u6C9F|\u6CA1|\u6CA3|\u6CA4|\u6CA5|\u6CA6|\u6CA7|\u6CA8|\u6CA9|\u6CAA|\u6CDE|\u6CE8|\u6CEA|\u6CF6|\u6CF7|\u6CF8|\u6CFA|\u6CFB|\u6CFC|\u6CFD|\u6CFE|\u6D01|\u6D12|\u6D3C|\u6D43|\u6D45|\u6D46|\u6D47|\u6D48|\u6D49|\u6D4A|\u6D4B|\u6D4D|\u6D4E|\u6D4F|\u6D50|\u6D51|\u6D52|\u6D53|\u6D54|\u6D55|\u6D82|\u6D9B|\u6D9D|\u6D9E|\u6D9F|\u6DA0|\u6DA1|\u6DA2|\u6DA3|\u6DA4|\u6DA6|\u6DA7|\u6DA8|\u6DA9|\u6DC0|\u6E0A|\u6E0C|\u6E0D|\u6E0E|\u6E10|\u6E11|\u6E14|\u6E16|\u6E17|\u6E29|\u6E7E|\u6E7F|\u6E81|\u6E83|\u6E85|\u6E86|\u6E87|\u6ED7|\u6EDA|\u6EDE|\u6EDF|\u6EE0|\u6EE1|\u6EE2|\u6EE4|\u6EE5|\u6EE6|\u6EE8|\u6EE9|\u6EEA|\u6F13|\u6F24|\u6F46|\u6F47|\u6F4B|\u6F4D|\u6F5C|\u6F74|\u6F9B|\u6F9C|\u6FD1|\u6FD2|\u704F|\u706D|\u706F|\u7075|\u707E|\u707F|\u7080|\u7089|\u709C|\u709D|\u70B9|\u70BC|\u70BD|\u70C1|\u70C2|\u70C3|\u70DB|\u70DF|\u70E6|\u70E7|\u70E8|\u70E9|\u70EB|\u70EC|\u70ED|\u7115|\u7116|\u7118|\u7174|\u7231|\u7237|\u724D|\u7266|\u7275|\u727A|\u728A|\u72B6|\u72B7|\u72B8|\u72B9|\u72C8|\u72DD|\u72DE|\u72EC|\u72ED|\u72EE|\u72EF|\u72F0|\u72F1|\u72F2|\u7303|\u730E|\u7315|\u7321|\u732A|\u732B|\u732C|\u732E|\u736D|\u7391|\u7399|\u739A|\u739B|\u73AE|\u73AF|\u73B0|\u73B1|\u73BA|\u73D0|\u73D1|\u73F0|\u73F2|\u740E|\u740F|\u7410|\u743C|\u7476|\u7477|\u7478|\u748E|\u74D2|\u74EF|\u7535|\u753B|\u7545|\u7574|\u7596|\u7597|\u759F|\u75A0|\u75A1|\u75AC|\u75AD|\u75AE|\u75AF|\u75B1|\u75B4|\u75C7|\u75C8|\u75C9|\u75D2|\u75D6|\u75E8|\u75EA|\u75EB|\u7605|\u7606|\u7617|\u7618|\u762A|\u762B|\u763E|\u763F|\u765E|\u7663|\u766B|\u7691|\u76B1|\u76B2|\u76CF|\u76D0|\u76D1|\u76D6|\u76D7|\u76D8|\u770D|\u7726|\u772C|\u7740|\u7741|\u7750|\u7751|\u7786|\u7792|\u77A9|\u77EB|\u77F6|\u77FE|\u77FF|\u7800|\u7801|\u7816|\u7817|\u781A|\u781C|\u783A|\u783B|\u783E|\u7840|\u7841|\u7855|\u7856|\u7857|\u7859|\u785A|\u786E|\u7875|\u7877|\u788D|\u789B|\u789C|\u793C|\u7943|\u794E|\u7962|\u796F|\u7977|\u7978|\u7980|\u7984|\u7985|\u79BB|\u79C3|\u79C6|\u79CD|\u79EF|\u79F0|\u79FD|\u79FE|\u7A06|\u7A0E|\u7A23|\u7A33|\u7A51|\u7A77|\u7A83|\u7A8D|\u7A8E|\u7A91|\u7A9C|\u7A9D|\u7AA5|\u7AA6|\u7AAD|\u7AD6|\u7ADE|\u7B03|\u7B0B|\u7B14|\u7B15|\u7B3A|\u7B3C|\u7B3E|\u7B51|\u7B5A|\u7B5B|\u7B5C|\u7B5D|\u7B79|\u7B7C|\u7B7E|\u7B80|\u7B93|\u7BA6|\u7BA7|\u7BA8|\u7BA9|\u7BAA|\u7BAB|\u7BD1|\u7BD3|\u7BEE|\u7BEF|\u7BF1|\u7C16|\u7C41|\u7C74|\u7C7B|\u7C7C|\u7C9C|\u7C9D|\u7CA4|\u7CAA|\u7CAE|\u7CC1|\u7CC7|\u7CFB|\u7D27|\u7D2F|\u7D77|\u7E9F|\u7EA0|\u7EA1|\u7EA2|\u7EA3|\u7EA4|\u7EA5|\u7EA6|\u7EA7|\u7EA8|\u7EA9|\u7EAA|\u7EAB|\u7EAC|\u7EAD|\u7EAE|\u7EAF|\u7EB0|\u7EB1|\u7EB2|\u7EB3|\u7EB4|\u7EB5|\u7EB6|\u7EB7|\u7EB8|\u7EB9|\u7EBA|\u7EBB|\u7EBC|\u7EBD|\u7EBE|\u7EBF|\u7EC0|\u7EC1|\u7EC2|\u7EC3|\u7EC4|\u7EC5|\u7EC6|\u7EC7|\u7EC8|\u7EC9|\u7ECA|\u7ECB|\u7ECC|\u7ECD|\u7ECE|\u7ECF|\u7ED0|\u7ED1|\u7ED2|\u7ED3|\u7ED4|\u7ED5|\u7ED6|\u7ED7|\u7ED8|\u7ED9|\u7EDA|\u7EDB|\u7EDC|\u7EDD|\u7EDE|\u7EDF|\u7EE0|\u7EE1|\u7EE2|\u7EE3|\u7EE4|\u7EE5|\u7EE6|\u7EE7|\u7EE8|\u7EE9|\u7EEA|\u7EEB|\u7EEC|\u7EED|\u7EEE|\u7EEF|\u7EF0|\u7EF1|\u7EF2|\u7EF3|\u7EF4|\u7EF5|\u7EF6|\u7EF7|\u7EF8|\u7EF9|\u7EFA|\u7EFB|\u7EFC|\u7EFD|\u7EFE|\u7EFF|\u7F00|\u7F01|\u7F02|\u7F03|\u7F04|\u7F05|\u7F06|\u7F07|\u7F08|\u7F09|\u7F0A|\u7F0B|\u7F0C|\u7F0D|\u7F0E|\u7F0F|\u7F10|\u7F11|\u7F12|\u7F13|\u7F14|\u7F15|\u7F16|\u7F17|\u7F18|\u7F19|\u7F1A|\u7F1B|\u7F1C|\u7F1D|\u7F1E|\u7F1F|\u7F20|\u7F21|\u7F22|\u7F23|\u7F24|\u7F25|\u7F26|\u7F27|\u7F28|\u7F29|\u7F2A|\u7F2B|\u7F2C|\u7F2D|\u7F2E|\u7F2F|\u7F30|\u7F31|\u7F32|\u7F33|\u7F34|\u7F35|\u7F42|\u7F51|\u7F57|\u7F5A|\u7F62|\u7F74|\u7F81|\u7F9F|\u7FD8|\u7FD9|\u7FDA|\u8022|\u8027|\u8038|\u803B|\u8042|\u804B|\u804C|\u804D|\u8054|\u8069|\u806A|\u8083|\u80A0|\u80A4|\u80AE|\u80B4|\u80BE|\u80BF|\u80C0|\u80C1|\u80C6|\u80DC|\u80E1|\u80E7|\u80E8|\u80EA|\u80EB|\u80F6|\u8109|\u810D|\u810F|\u8110|\u8111|\u8113|\u8114|\u811A|\u8131|\u8136|\u8138|\u814A|\u8158|\u816D|\u817B|\u817C|\u817D|\u817E|\u8191|\u81DC|\u81F4|\u8206|\u820D|\u8223|\u8230|\u8231|\u823B|\u8270|\u8273|\u827A|\u8282|\u8288|\u8297|\u829C|\u82A6|\u82B8|\u82C1|\u82C7|\u82C8|\u82CB|\u82CC|\u82CD|\u82CE|\u82CF|\u82E7|\u82F9|\u8303|\u830E|\u830F|\u8311|\u8314|\u8315|\u8327|\u8346|\u8350|\u8359|\u835A|\u835B|\u835C|\u835D|\u835E|\u835F|\u8360|\u8361|\u8363|\u8364|\u8365|\u8366|\u8367|\u8368|\u8369|\u836A|\u836B|\u836C|\u836D|\u836E|\u836F|\u8385|\u83B1|\u83B2|\u83B3|\u83B4|\u83B6|\u83B7|\u83B8|\u83B9|\u83BA|\u83BC|\u841A|\u841D|\u8424|\u8425|\u8426|\u8427|\u8428|\u8471|\u8487|\u8489|\u848B|\u848C|\u84DD|\u84DF|\u84E0|\u84E3|\u84E5|\u84E6|\u8502|\u8537|\u8539|\u853A|\u853C|\u8570|\u8572|\u8574|\u85AE|\u85D3|\u8616|\u864F|\u8651|\u865A|\u866B|\u866C|\u866E|\u867D|\u867E|\u867F|\u8680|\u8681|\u8682|\u8683|\u8695|\u86AC|\u86CA|\u86CE|\u86CF|\u86EE|\u86F0|\u86F1|\u86F2|\u86F3|\u86F4|\u8715|\u8717|\u8721|\u8747|\u8748|\u8749|\u877C|\u877E|\u8780|\u87A8|\u87CF|\u8845|\u8854|\u8865|\u8868|\u886C|\u886E|\u8884|\u8885|\u8886|\u889C|\u88AD|\u88AF|\u88C5|\u88C6|\u88C8|\u88E2|\u88E3|\u88E4|\u88E5|\u891B|\u891D|\u8934|\u8955|\u89C1|\u89C2|\u89C3|\u89C4|\u89C5|\u89C6|\u89C7|\u89C8|\u89C9|\u89CA|\u89CB|\u89CC|\u89CD|\u89CE|\u89CF|\u89D0|\u89D1|\u89DE|\u89E6|\u89EF|\u8A1A|\u8A5F|\u8A89|\u8A8A|\u8BA0|\u8BA1|\u8BA2|\u8BA3|\u8BA4|\u8BA5|\u8BA6|\u8BA7|\u8BA8|\u8BA9|\u8BAA|\u8BAB|\u8BAC|\u8BAD|\u8BAE|\u8BAF|\u8BB0|\u8BB1|\u8BB2|\u8BB3|\u8BB4|\u8BB5|\u8BB6|\u8BB7|\u8BB8|\u8BB9|\u8BBA|\u8BBB|\u8BBC|\u8BBD|\u8BBE|\u8BBF|\u8BC0|\u8BC1|\u8BC2|\u8BC3|\u8BC4|\u8BC5|\u8BC6|\u8BC7|\u8BC8|\u8BC9|\u8BCA|\u8BCB|\u8BCC|\u8BCD|\u8BCE|\u8BCF|\u8BD0|\u8BD1|\u8BD2|\u8BD3|\u8BD4|\u8BD5|\u8BD6|\u8BD7|\u8BD8|\u8BD9|\u8BDA|\u8BDB|\u8BDC|\u8BDD|\u8BDE|\u8BDF|\u8BE0|\u8BE1|\u8BE2|\u8BE3|\u8BE4|\u8BE5|\u8BE6|\u8BE7|\u8BE8|\u8BE9|\u8BEA|\u8BEB|\u8BEC|\u8BED|\u8BEE|\u8BEF|\u8BF0|\u8BF1|\u8BF2|\u8BF3|\u8BF4|\u8BF5|\u8BF6|\u8BF7|\u8BF8|\u8BF9|\u8BFA|\u8BFB|\u8BFC|\u8BFD|\u8BFE|\u8BFF|\u8C00|\u8C01|\u8C02|\u8C03|\u8C04|\u8C05|\u8C06|\u8C07|\u8C08|\u8C09|\u8C0A|\u8C0B|\u8C0C|\u8C0D|\u8C0E|\u8C0F|\u8C10|\u8C11|\u8C12|\u8C13|\u8C14|\u8C15|\u8C16|\u8C17|\u8C18|\u8C19|\u8C1A|\u8C1B|\u8C1C|\u8C1D|\u8C1E|\u8C1F|\u8C20|\u8C21|\u8C22|\u8C23|\u8C24|\u8C25|\u8C26|\u8C27|\u8C28|\u8C29|\u8C2A|\u8C2B|\u8C2C|\u8C2D|\u8C2E|\u8C2F|\u8C30|\u8C31|\u8C32|\u8C33|\u8C34|\u8C35|\u8C36|\u8C37|\u8C6E|\u8D1D|\u8D1E|\u8D1F|\u8D20|\u8D21|\u8D22|\u8D23|\u8D24|\u8D25|\u8D26|\u8D27|\u8D28|\u8D29|\u8D2A|\u8D2B|\u8D2C|\u8D2D|\u8D2E|\u8D2F|\u8D30|\u8D31|\u8D32|\u8D33|\u8D34|\u8D35|\u8D36|\u8D37|\u8D38|\u8D39|\u8D3A|\u8D3B|\u8D3C|\u8D3D|\u8D3E|\u8D3F|\u8D40|\u8D41|\u8D42|\u8D43|\u8D44|\u8D45|\u8D46|\u8D47|\u8D48|\u8D49|\u8D4A|\u8D4B|\u8D4C|\u8D4D|\u8D4E|\u8D4F|\u8D50|\u8D51|\u8D52|\u8D53|\u8D54|\u8D55|\u8D56|\u8D57|\u8D58|\u8D59|\u8D5A|\u8D5B|\u8D5C|\u8D5D|\u8D5E|\u8D5F|\u8D60|\u8D61|\u8D62|\u8D63|\u8D6A|\u8D75|\u8D76|\u8D8B|\u8DB1|\u8DB8|\u8DC3|\u8DC4|\u8DDE|\u8DF5|\u8DF6|\u8DF7|\u8DF8|\u8DF9|\u8DFB|\u8E0A|\u8E0C|\u8E2A|\u8E2C|\u8E2F|\u8E51|\u8E52|\u8E70|\u8E7F|\u8E8F|\u8E9C|\u8EAF|\u8F66|\u8F67|\u8F68|\u8F69|\u8F6A|\u8F6B|\u8F6C|\u8F6D|\u8F6E|\u8F6F|\u8F70|\u8F71|\u8F72|\u8F73|\u8F74|\u8F75|\u8F76|\u8F77|\u8F78|\u8F79|\u8F7A|\u8F7B|\u8F7C|\u8F7D|\u8F7E|\u8F7F|\u8F80|\u8F81|\u8F82|\u8F83|\u8F84|\u8F85|\u8F86|\u8F87|\u8F88|\u8F89|\u8F8A|\u8F8B|\u8F8C|\u8F8D|\u8F8E|\u8F8F|\u8F90|\u8F91|\u8F92|\u8F93|\u8F94|\u8F95|\u8F96|\u8F97|\u8F98|\u8F99|\u8F9A|\u8F9E|\u8F9F|\u8FA9|\u8FAB|\u8FB9|\u8FBD|\u8FBE|\u8FC1|\u8FC7|\u8FC8|\u8FD0|\u8FD8|\u8FD9|\u8FDB|\u8FDC|\u8FDD|\u8FDE|\u8FDF|\u8FE9|\u8FF3|\u8FF9|\u9002|\u9009|\u900A|\u9012|\u9026|\u903B|\u9057|\u9065|\u9093|\u909D|\u90AC|\u90AE|\u90B9|\u90BA|\u90BB|\u90C1|\u90CF|\u90D0|\u90D1|\u90D3|\u90E6|\u90E7|\u90F8|\u9142|\u915D|\u9166|\u9171|\u917D|\u917E|\u917F|\u91C7|\u91CA|\u91CC|\u9274|\u92AE|\u933E|\u9485|\u9486|\u9487|\u9488|\u9489|\u948A|\u948B|\u948C|\u948D|\u948E|\u948F|\u9490|\u9491|\u9492|\u9493|\u9494|\u9495|\u9496|\u9497|\u9498|\u9499|\u949A|\u949B|\u949C|\u949D|\u949E|\u949F|\u94A0|\u94A1|\u94A2|\u94A3|\u94A4|\u94A5|\u94A6|\u94A7|\u94A8|\u94A9|\u94AA|\u94AB|\u94AC|\u94AD|\u94AE|\u94AF|\u94B0|\u94B1|\u94B2|\u94B3|\u94B4|\u94B5|\u94B6|\u94B7|\u94B8|\u94B9|\u94BA|\u94BB|\u94BC|\u94BD|\u94BE|\u94BF|\u94C0|\u94C1|\u94C2|\u94C3|\u94C4|\u94C5|\u94C6|\u94C7|\u94C8|\u94C9|\u94CA|\u94CB|\u94CC|\u94CD|\u94CE|\u94CF|\u94D0|\u94D1|\u94D2|\u94D3|\u94D4|\u94D5|\u94D6|\u94D7|\u94D8|\u94D9|\u94DA|\u94DB|\u94DC|\u94DD|\u94DE|\u94DF|\u94E0|\u94E1|\u94E2|\u94E3|\u94E4|\u94E5|\u94E6|\u94E7|\u94E8|\u94E9|\u94EA|\u94EB|\u94EC|\u94ED|\u94EE|\u94EF|\u94F0|\u94F1|\u94F2|\u94F3|\u94F4|\u94F5|\u94F6|\u94F7|\u94F8|\u94F9|\u94FA|\u94FB|\u94FC|\u94FD|\u94FE|\u94FF|\u9500|\u9501|\u9502|\u9503|\u9504|\u9505|\u9506|\u9507|\u9508|\u9509|\u950A|\u950B|\u950C|\u950D|\u950E|\u950F|\u9510|\u9511|\u9512|\u9513|\u9514|\u9515|\u9516|\u9517|\u9518|\u9519|\u951A|\u951B|\u951C|\u951D|\u951E|\u951F|\u9520|\u9521|\u9522|\u9523|\u9524|\u9525|\u9526|\u9527|\u9528|\u9529|\u952A|\u952B|\u952C|\u952D|\u952E|\u952F|\u9530|\u9531|\u9532|\u9533|\u9534|\u9535|\u9536|\u9537|\u9538|\u9539|\u953A|\u953B|\u953C|\u953D|\u953E|\u953F|\u9540|\u9541|\u9542|\u9543|\u9544|\u9545|\u9546|\u9547|\u9548|\u9549|\u954A|\u954B|\u954C|\u954D|\u954E|\u954F|\u9550|\u9551|\u9552|\u9553|\u9554|\u9555|\u9556|\u9557|\u9558|\u9559|\u955A|\u955B|\u955C|\u955D|\u955E|\u955F|\u9560|\u9561|\u9562|\u9563|\u9564|\u9565|\u9566|\u9567|\u9568|\u9569|\u956A|\u956B|\u956C|\u956D|\u956E|\u956F|\u9570|\u9571|\u9572|\u9573|\u9574|\u9575|\u9576|\u957F|\u95E8|\u95E9|\u95EA|\u95EB|\u95EC|\u95ED|\u95EE|\u95EF|\u95F0|\u95F1|\u95F2|\u95F3|\u95F4|\u95F5|\u95F6|\u95F7|\u95F8|\u95F9|\u95FA|\u95FB|\u95FC|\u95FD|\u95FE|\u95FF|\u9600|\u9601|\u9602|\u9603|\u9604|\u9605|\u9606|\u9607|\u9608|\u9609|\u960A|\u960B|\u960C|\u960D|\u960E|\u960F|\u9610|\u9611|\u9612|\u9613|\u9614|\u9615|\u9616|\u9617|\u9618|\u9619|\u961A|\u961B|\u961F|\u9633|\u9634|\u9635|\u9636|\u9645|\u9646|\u9647|\u9648|\u9649|\u9655|\u9666|\u9667|\u9668|\u9669|\u968F|\u9690|\u96B6|\u96BD|\u96BE|\u96CF|\u96E0|\u96F3|\u96FE|\u9701|\u9721|\u972D|\u9753|\u9759|\u9762|\u9765|\u9791|\u9792|\u97AF|\u97E6|\u97E7|\u97E8|\u97E9|\u97EA|\u97EB|\u97EC|\u97F5|\u9875|\u9876|\u9877|\u9878|\u9879|\u987A|\u987B|\u987C|\u987D|\u987E|\u987F|\u9880|\u9881|\u9882|\u9883|\u9884|\u9885|\u9886|\u9887|\u9888|\u9889|\u988A|\u988B|\u988C|\u988D|\u988E|\u988F|\u9890|\u9891|\u9892|\u9893|\u9894|\u9895|\u9896|\u9897|\u9898|\u9899|\u989A|\u989B|\u989C|\u989D|\u989E|\u989F|\u98A0|\u98A1|\u98A2|\u98A3|\u98A4|\u98A5|\u98A6|\u98A7|\u98CE|\u98CF|\u98D0|\u98D1|\u98D2|\u98D3|\u98D4|\u98D5|\u98D6|\u98D7|\u98D8|\u98D9|\u98DA|\u98DE|\u98E8|\u990D|\u9963|\u9964|\u9965|\u9966|\u9967|\u9968|\u9969|\u996A|\u996B|\u996C|\u996D|\u996E|\u996F|\u9970|\u9971|\u9972|\u9973|\u9974|\u9975|\u9976|\u9977|\u9978|\u9979|\u997A|\u997B|\u997C|\u997D|\u997E|\u997F|\u9980|\u9981|\u9982|\u9983|\u9984|\u9985|\u9986|\u9987|\u9988|\u9989|\u998A|\u998B|\u998C|\u998D|\u998E|\u998F|\u9990|\u9991|\u9992|\u9993|\u9994|\u9995|\u9A6C|\u9A6D|\u9A6E|\u9A6F|\u9A70|\u9A71|\u9A72|\u9A73|\u9A74|\u9A75|\u9A76|\u9A77|\u9A78|\u9A79|\u9A7A|\u9A7B|\u9A7C|\u9A7D|\u9A7E|\u9A7F|\u9A80|\u9A81|\u9A82|\u9A83|\u9A84|\u9A85|\u9A86|\u9A87|\u9A88|\u9A89|\u9A8A|\u9A8B|\u9A8C|\u9A8D|\u9A8E|\u9A8F|\u9A90|\u9A91|\u9A92|\u9A93|\u9A94|\u9A95|\u9A96|\u9A97|\u9A98|\u9A99|\u9A9A|\u9A9B|\u9A9C|\u9A9D|\u9A9E|\u9A9F|\u9AA0|\u9AA1|\u9AA2|\u9AA3|\u9AA4|\u9AA5|\u9AA6|\u9AA7|\u9AC5|\u9ACB|\u9ACC|\u9B13|\u9B36|\u9B47|\u9B49|\u9C7C|\u9C7D|\u9C7E|\u9C7F|\u9C80|\u9C81|\u9C82|\u9C83|\u9C84|\u9C85|\u9C86|\u9C87|\u9C88|\u9C89|\u9C8A|\u9C8B|\u9C8C|\u9C8D|\u9C8E|\u9C8F|\u9C90|\u9C91|\u9C92|\u9C93|\u9C94|\u9C95|\u9C96|\u9C97|\u9C98|\u9C99|\u9C9A|\u9C9B|\u9C9C|\u9C9D|\u9C9E|\u9C9F|\u9CA0|\u9CA1|\u9CA2|\u9CA3|\u9CA4|\u9CA5|\u9CA6|\u9CA7|\u9CA8|\u9CA9|\u9CAA|\u9CAB|\u9CAC|\u9CAD|\u9CAE|\u9CAF|\u9CB0|\u9CB1|\u9CB2|\u9CB3|\u9CB4|\u9CB5|\u9CB6|\u9CB7|\u9CB8|\u9CB9|\u9CBA|\u9CBB|\u9CBC|\u9CBD|\u9CBE|\u9CBF|\u9CC0|\u9CC1|\u9CC2|\u9CC3|\u9CC4|\u9CC5|\u9CC6|\u9CC7|\u9CC8|\u9CC9|\u9CCA|\u9CCB|\u9CCC|\u9CCD|\u9CCE|\u9CCF|\u9CD0|\u9CD1|\u9CD2|\u9CD3|\u9CD4|\u9CD5|\u9CD6|\u9CD7|\u9CD8|\u9CD9|\u9CDA|\u9CDB|\u9CDC|\u9CDD|\u9CDE|\u9CDF|\u9CE0|\u9CE1|\u9CE2|\u9CE3|\u9CE4|\u9E1F|\u9E20|\u9E21|\u9E22|\u9E23|\u9E24|\u9E25|\u9E26|\u9E27|\u9E28|\u9E29|\u9E2A|\u9E2B|\u9E2C|\u9E2D|\u9E2E|\u9E2F|\u9E30|\u9E31|\u9E32|\u9E33|\u9E34|\u9E35|\u9E36|\u9E37|\u9E38|\u9E39|\u9E3A|\u9E3B|\u9E3C|\u9E3D|\u9E3E|\u9E3F|\u9E40|\u9E41|\u9E42|\u9E43|\u9E44|\u9E45|\u9E46|\u9E47|\u9E48|\u9E49|\u9E4A|\u9E4B|\u9E4C|\u9E4D|\u9E4E|\u9E4F|\u9E50|\u9E51|\u9E52|\u9E53|\u9E54|\u9E55|\u9E56|\u9E57|\u9E58|\u9E59|\u9E5A|\u9E5B|\u9E5C|\u9E5D|\u9E5E|\u9E5F|\u9E60|\u9E61|\u9E62|\u9E63|\u9E64|\u9E65|\u9E66|\u9E67|\u9E68|\u9E69|\u9E6A|\u9E6B|\u9E6C|\u9E6D|\u9E6E|\u9E6F|\u9E70|\u9E71|\u9E72|\u9E73|\u9E74|\u9E7E|\u9EA6|\u9EB8|\u9EB9|\u9EC4|\u9EC9|\u9EE1|\u9EE9|\u9EEA|\u9EFE|\u9F0B|\u9F0D|\u9F17|\u9F39|\u9F50|\u9F51|\u9F7F|\u9F80|\u9F81|\u9F82|\u9F83|\u9F84|\u9F85|\u9F86|\u9F87|\u9F88|\u9F89|\u9F8A|\u9F8B|\u9F8C|\u9F99|\u9F9A|\u9F9B|\u9F9F|\u9FCE|\u9FCF|\u9FD3|\u9FD4|\u9FD5|\u9FED|\u2003E|\u200B2|\u200D3|\u201B2|\u201BF|\u201D0|\u201F9|\u20242|\u20257|\u206B3|\u206C5|\u206C6|\u206FE|\u20860|\u20B24|\u20BDF|\u20BE0|\u20C37|\u20C5E|\u20CA5|\u20CDE|\u20D22|\u20D78|\u20D7E|\u2121B|\u21291|\u212C0|\u212D7|\u212E4|\u213C6|\u21484|\u21760|\u2178B|\u217B1|\u2181F|\u21847|\u21967|\u21B5C|\u21B6C|\u21CC3|\u21CD2|\u21D5D|\u21DB4|\u21E03|\u21E83|\u21ED8|\u22016|\u222C8|\u224C5|\u225D3|\u22619|\u2261D|\u2261E|\u22650|\u22651|\u22652|\u22653|\u226EF|\u227FC|\u229D0|\u22A93|\u22A97|\u22ACA|\u22AD8|\u22ADE|\u22AEC|\u22B0D|\u22B26|\u22B4F|\u22DA3|\u22F7E|\u230C1|\u23190|\u23223|\u2327C|\u23368|\u2336F|\u23370|\u23391|\u233E2|\u23415|\u23424|\u2345D|\u23476|\u2348C|\u23497|\u234FF|\u23572|\u235CA|\u235CB|\u23610|\u23613|\u23634|\u23637|\u2363E|\u23665|\u2369A|\u2378E|\u23A3C|\u23B64|\u23BE3|\u23C5D|\u23C97|\u23CC6|\u23DA9|\u23DAB|\u23E23|\u23EBC|\u23EBD|\u23F77|\u23F8D|\u241A1|\u241A2|\u241C3|\u241C4|\u241ED|\u241FB|\u24236|\u24237|\u24280|\u242CF|\u243BA|\u243BB|\u2466F|\u24735|\u24762|\u24783|\u247A4|\u2480B|\u24980|\u24A7D|\u24CC4|\u24D8A|\u24DA7|\u24E7A|\u24ECA|\u24F6F|\u24F80|\u24FF2|\u25062|\u25158|\u25174|\u251A7|\u251E2|\u2539D|\u2541F|\u2542F|\u25430|\u2543B|\u25564|\u257A6|\u257C2|\u259C2|\u25A7A|\u25B00|\u25B08|\u25B1E|\u25B20|\u25B49|\u25B8B|\u25B9C|\u25BBE|\u25C54|\u25E65|\u25E85|\u25E87|\u26208|\u26209|\u2620B|\u2620C|\u2620E|\u2620F|\u26210|\u26211|\u26212|\u26213|\u26214|\u26215|\u26216|\u26217|\u26218|\u26219|\u2621A|\u2621B|\u2621C|\u2621D|\u2621E|\u2621F|\u26220|\u26221|\u26360|\u266E8|\u2677C|\u2678C|\u267D7|\u26A29|\u26B19|\u26C34|\u26D07|\u26ED5|\u27234|\u2723F|\u27250|\u2725E|\u273D6|\u273D7|\u2744F|\u274AD|\u27721|\u2772D|\u2775D|\u27924|\u27945|\u27BAA|\u27CD5|\u27E51|\u27E52|\u27E53|\u27E54|\u27E55|\u27E56|\u27E57|\u27EA3|\u27FC8|\u27FDB|\u28001|\u28031|\u28074|\u280BA|\u28104|\u2815B|\u2816B|\u2816C|\u28257|\u28405|\u28406|\u28407|\u28408|\u28409|\u2840A|\u28479|\u28755|\u287F3|\u28828|\u28859|\u2887A|\u288B8|\u28930|\u289EE|\u28C3E|\u28C3F|\u28C40|\u28C41|\u28C42|\u28C43|\u28C44|\u28C45|\u28C46|\u28C47|\u28C48|\u28C49|\u28C4A|\u28C4B|\u28C4C|\u28C4D|\u28C4E|\u28C4F|\u28C50|\u28C51|\u28C52|\u28C53|\u28C54|\u28C55|\u28C56|\u28DFF|\u28E00|\u28E01|\u28E02|\u28E03|\u28E04|\u28E05|\u28E06|\u28E07|\u28E09|\u28E0A|\u28E0B|\u28E0C|\u28E0E|\u28E18|\u28E1F|\u28EF9|\u293FC|\u293FD|\u293FE|\u293FF|\u29400|\u29595|\u29596|\u29597|\u29665|\u29666|\u29667|\u29668|\u29669|\u2966A|\u2966B|\u2966C|\u2966D|\u2966E|\u2966F|\u29670|\u297FF|\u29800|\u29801|\u29802|\u29803|\u29805|\u29806|\u29807|\u29808|\u29809|\u2980A|\u2980B|\u2980C|\u2980E|\u2980F|\u29820|\u29856|\u2985A|\u299E6|\u299E8|\u299E9|\u299EA|\u299EB|\u299EC|\u299ED|\u299EE|\u299EF|\u299F0|\u299F1|\u299F2|\u299F3|\u299F4|\u299F5|\u299F6|\u299F8|\u299FA|\u299FB|\u299FC|\u299FF|\u29A00|\u29A01|\u29A03|\u29A04|\u29A05|\u29A06|\u29A07|\u29A08|\u29A09|\u29A0A|\u29A0B|\u29A0C|\u29A0D|\u29A0E|\u29A0F|\u29A10|\u29A48|\u29B23|\u29B24|\u29B3E|\u29B79|\u29BD2|\u29C30|\u29C92|\u29D0C|\u29F79|\u29F7A|\u29F7B|\u29F7C|\u29F7D|\u29F7E|\u29F7F|\u29F81|\u29F82|\u29F83|\u29F84|\u29F85|\u29F86|\u29F87|\u29F88|\u29F8A|\u29F8B|\u29F8C|\u29F8E|\u2A242|\u2A243|\u2A244|\u2A245|\u2A246|\u2A248|\u2A249|\u2A24A|\u2A24B|\u2A24C|\u2A24D|\u2A24E|\u2A24F|\u2A250|\u2A251|\u2A252|\u2A254|\u2A255|\u2A388|\u2A389|\u2A38A|\u2A38B|\u2A38C|\u2A445|\u2A52D|\u2A68F|\u2A690|\u2A70E|\u2A73A|\u2A79D|\u2A7CE|\u2A7DD|\u2A7F2|\u2A800|\u2A803|\u2A80F|\u2A81F|\u2A821|\u2A833|\u2A835|\u2A838|\u2A83D|\u2A840|\u2A843|\u2A84B|\u2A84F|\u2A85B|\u2A85E|\u2A87A|\u2A888|\u2A88B|\u2A88C|\u2A890|\u2A892|\u2A895|\u2A8A0|\u2A8AE|\u2A8C6|\u2A8D2|\u2A8FB|\u2A904|\u2A905|\u2A91A|\u2A960|\u2A96B|\u2A970|\u2A97F|\u2A9C0|\u2A9D8|\u2AA07|\u2AA0A|\u2AA17|\u2AA27|\u2AA29|\u2AA36|\u2AA37|\u2AA39|\u2AA47|\u2AA4E|\u2AA58|\u2AA5B|\u2AA78|\u2AA91|\u2AA9E|\u2AAB4|\u2AACC|\u2AAE1|\u2AAF7|\u2AAF8|\u2AAFA|\u2AB1A|\u2AB2F|\u2AB5D|\u2AB62|\u2AB67|\u2AB6F|\u2AB75|\u2AB7E|\u2AB83|\u2AB8B|\u2AB96|\u2ABB3|\u2ABB6|\u2ABCB|\u2AC36|\u2AC65|\u2AC77|\u2AC8E|\u2AC94|\u2AC9B|\u2ACAE|\u2ACCD|\u2AD19|\u2AD2F|\u2AD47|\u2AD51|\u2AD63|\u2AD71|\u2AD84|\u2AD92|\u2ADAE|\u2ADCD|\u2ADFD|\u2AE15|\u2AE29|\u2AE40|\u2AE60|\u2AE73|\u2AE79|\u2AEA3|\u2AEAA|\u2AEAD|\u2AEB7|\u2AEB8|\u2AEBB|\u2AEBD|\u2AED0|\u2AEE8|\u2AEF2|\u2AEFA|\u2AF0B|\u2AF34|\u2AF42|\u2AF5D|\u2AF6A|\u2AF6D|\u2AF6E|\u2AF74|\u2AF77|\u2AF94|\u2AFA2|\u2AFA6|\u2AFB8|\u2AFCA|\u2AFDE|\u2AFEB|\u2AFF5|\u2B00C|\u2B013|\u2B028|\u2B02C|\u2B02E|\u2B042|\u2B05F|\u2B061|\u2B072|\u2B073|\u2B077|\u2B07A|\u2B083|\u2B086|\u2B088|\u2B096|\u2B0BF|\u2B0D7|\u2B119|\u2B11A|\u2B11B|\u2B11C|\u2B11D|\u2B11E|\u2B11F|\u2B120|\u2B121|\u2B122|\u2B123|\u2B124|\u2B125|\u2B126|\u2B127|\u2B128|\u2B129|\u2B12A|\u2B12B|\u2B12C|\u2B12D|\u2B12E|\u2B12F|\u2B130|\u2B131|\u2B132|\u2B133|\u2B134|\u2B135|\u2B136|\u2B137|\u2B138|\u2B139|\u2B145|\u2B157|\u2B165|\u2B16D|\u2B17C|\u2B18F|\u2B19D|\u2B1AB|\u2B1D8|\u2B1EA|\u2B1ED|\u2B1F4|\u2B1FD|\u2B209|\u2B20E|\u2B21F|\u2B235|\u2B241|\u2B244|\u2B2AA|\u2B2AE|\u2B2B1|\u2B2B8|\u2B2B9|\u2B2BB|\u2B2C7|\u2B2CC|\u2B2F2|\u2B2F7|\u2B2F9|\u2B2FB|\u2B300|\u2B307|\u2B30B|\u2B328|\u2B329|\u2B32A|\u2B32B|\u2B32C|\u2B32D|\u2B32F|\u2B34F|\u2B350|\u2B359|\u2B35A|\u2B35B|\u2B35C|\u2B35E|\u2B35F|\u2B360|\u2B361|\u2B362|\u2B363|\u2B364|\u2B365|\u2B366|\u2B367|\u2B368|\u2B369|\u2B36A|\u2B36B|\u2B36C|\u2B36D|\u2B36E|\u2B36F|\u2B370|\u2B371|\u2B372|\u2B373|\u2B374|\u2B375|\u2B376|\u2B377|\u2B378|\u2B379|\u2B37A|\u2B37B|\u2B37C|\u2B37D|\u2B37E|\u2B37F|\u2B386|\u2B38C|\u2B3A6|\u2B3A7|\u2B3A8|\u2B3A9|\u2B3AA|\u2B3AB|\u2B3AC|\u2B3AD|\u2B3B1|\u2B3B3|\u2B3B8|\u2B3BA|\u2B3C3|\u2B3C6|\u2B3CB|\u2B3CC|\u2B3D0|\u2B3D1|\u2B3D5|\u2B3DE|\u2B3E8|\u2B404|\u2B405|\u2B406|\u2B407|\u2B408|\u2B409|\u2B40A|\u2B40B|\u2B40C|\u2B40D|\u2B40E|\u2B40F|\u2B410|\u2B411|\u2B412|\u2B413|\u2B414|\u2B415|\u2B416|\u2B417|\u2B418|\u2B419|\u2B437|\u2B458|\u2B461|\u2B477|\u2B4E5|\u2B4E6|\u2B4E7|\u2B4E8|\u2B4E9|\u2B4EA|\u2B4EB|\u2B4EC|\u2B4ED|\u2B4EE|\u2B4EF|\u2B4F0|\u2B4F1|\u2B4F2|\u2B4F3|\u2B4F4|\u2B4F5|\u2B4F6|\u2B4F7|\u2B4F8|\u2B4F9|\u2B4FA|\u2B4FB|\u2B4FC|\u2B4FD|\u2B4FE|\u2B4FF|\u2B500|\u2B501|\u2B502|\u2B503|\u2B504|\u2B505|\u2B506|\u2B507|\u2B508|\u2B509|\u2B50A|\u2B50B|\u2B50C|\u2B50D|\u2B50E|\u2B50F|\u2B510|\u2B511|\u2B512|\u2B513|\u2B514|\u2B515|\u2B516|\u2B52D|\u2B52F|\u2B530|\u2B531|\u2B532|\u2B534|\u2B535|\u2B536|\u2B53D|\u2B55A|\u2B565|\u2B568|\u2B583|\u2B585|\u2B587|\u2B591|\u2B592|\u2B593|\u2B594|\u2B595|\u2B596|\u2B5AA|\u2B5AB|\u2B5AC|\u2B5AD|\u2B5AE|\u2B5AF|\u2B5B0|\u2B5B1|\u2B5B2|\u2B5B3|\u2B5B4|\u2B5B5|\u2B5B6|\u2B5B7|\u2B5B8|\u2B5B9|\u2B5BA|\u2B5C7|\u2B5C8|\u2B5C9|\u2B5CA|\u2B5CB|\u2B5DA|\u2B5DE|\u2B5DF|\u2B5E0|\u2B5E1|\u2B5E2|\u2B5E3|\u2B5E4|\u2B5E5|\u2B5E6|\u2B5E7|\u2B5E8|\u2B5E9|\u2B5EA|\u2B5EB|\u2B5EC|\u2B5ED|\u2B5EE|\u2B5EF|\u2B5F0|\u2B5F1|\u2B5F2|\u2B5F3|\u2B5F4|\u2B5F5|\u2B61B|\u2B61C|\u2B61D|\u2B61E|\u2B61F|\u2B620|\u2B621|\u2B623|\u2B624|\u2B625|\u2B626|\u2B627|\u2B628|\u2B629|\u2B62A|\u2B62B|\u2B62C|\u2B62D|\u2B62E|\u2B62F|\u2B630|\u2B631|\u2B63D|\u2B642|\u2B688|\u2B689|\u2B68A|\u2B68B|\u2B68C|\u2B68D|\u2B68E|\u2B68F|\u2B690|\u2B691|\u2B692|\u2B693|\u2B694|\u2B695|\u2B696|\u2B697|\u2B698|\u2B699|\u2B69A|\u2B69B|\u2B69C|\u2B69D|\u2B69E|\u2B69F|\u2B6A0|\u2B6A1|\u2B6A2|\u2B6A3|\u2B6A4|\u2B6A5|\u2B6A6|\u2B6A7|\u2B6A8|\u2B6A9|\u2B6AA|\u2B6AB|\u2B6AC|\u2B6AD|\u2B6DA|\u2B6DB|\u2B6DC|\u2B6DD|\u2B6DE|\u2B6DF|\u2B6E0|\u2B6E1|\u2B6E2|\u2B6E3|\u2B6E4|\u2B6E5|\u2B6E6|\u2B6E7|\u2B6E8|\u2B6E9|\u2B6EA|\u2B6EB|\u2B6EC|\u2B6ED|\u2B6EE|\u2B6EF|\u2B6F0|\u2B6F1|\u2B6F2|\u2B6F3|\u2B6F4|\u2B6F5|\u2B6F6|\u2B6F7|\u2B6F8|\u2B6F9|\u2B6FA|\u2B6FB|\u2B6FC|\u2B6FD|\u2B6FE|\u2B700|\u2B701|\u2B702|\u2B703|\u2B704|\u2B705|\u2B70A|\u2B711|\u2B712|\u2B713|\u2B714|\u2B715|\u2B719|\u2B71F|\u2B728|\u2B729|\u2B72A|\u2B72B|\u2B72C|\u2B72D|\u2B72E|\u2B72F|\u2B730|\u2B732|\u2B733|\u2B748|\u2B74B|\u2B761|\u2B766|\u2B767|\u2B768|\u2B769|\u2B76A|\u2B76B|\u2B76C|\u2B76D|\u2B76E|\u2B775|\u2B785|\u2B797|\u2B79A|\u2B79B|\u2B79D|\u2B7A0|\u2B7A1|\u2B7A2|\u2B7A3|\u2B7A5|\u2B7A6|\u2B7A7|\u2B7A8|\u2B7A9|\u2B7B7|\u2B7C3|\u2B7C4|\u2B7C5|\u2B7C6|\u2B7C7|\u2B7D1|\u2B7D5|\u2B7DE|\u2B7DF|\u2B7E0|\u2B7E1|\u2B7E2|\u2B7E4|\u2B7E5|\u2B7E6|\u2B7EB|\u2B7EC|\u2B7F2|\u2B7F3|\u2B7F4|\u2B7F5|\u2B7F6|\u2B7F7|\u2B7F8|\u2B7F9|\u2B7FA|\u2B7FB|\u2B7FC|\u2B7FD|\u2B7FE|\u2B7FF|\u2B800|\u2B801|\u2B802|\u2B805|\u2B806|\u2B807|\u2B808|\u2B80A|\u2B80B|\u2B80C|\u2B80F|\u2B810|\u2B811|\u2B812|\u2B816|\u2B81C|\u2B86C|\u2B876|\u2B892|\u2B894|\u2B898|\u2B899|\u2B89C|\u2B89F|\u2B8A8|\u2B8AA|\u2B8AC|\u2B8AD|\u2B8B2|\u2B8B8|\u2B8B9|\u2B8BA|\u2B8C9|\u2B8CA|\u2B8DB|\u2B8EB|\u2B938|\u2B93D|\u2B94D|\u2B954|\u2B973|\u2B975|\u2B97A|\u2B97C|\u2B97D|\u2B981|\u2B985|\u2B989|\u2B98B|\u2B98C|\u2B9A9|\u2B9B0|\u2B9B3|\u2B9C3|\u2B9EE|\u2B9EF|\u2B9F7|\u2B9FF|\u2BA06|\u2BA55|\u2BA56|\u2BA5A|\u2BA5B|\u2BA64|\u2BA65|\u2BA69|\u2BA6B|\u2BA6F|\u2BA73|\u2BA78|\u2BA7A|\u2BA80|\u2BA81|\u2BA82|\u2BA83|\u2BA84|\u2BA85|\u2BA91|\u2BA98|\u2BA9A|\u2BAA7|\u2BAAA|\u2BABA|\u2BABD|\u2BAC7|\u2BACF|\u2BAE6|\u2BAF5|\u2BAFE|\u2BB10|\u2BB19|\u2BB1F|\u2BB5E|\u2BB5F|\u2BB62|\u2BB68|\u2BB6A|\u2BB6E|\u2BB6F|\u2BB72|\u2BB7C|\u2BB83|\u2BB85|\u2BB9C|\u2BBD2|\u2BBE5|\u2BBF6|\u2BC02|\u2BC0D|\u2BC1B|\u2BC20|\u2BC21|\u2BC22|\u2BC23|\u2BC28|\u2BC30|\u2BC39|\u2BC55|\u2BC7F|\u2BC97|\u2BCB8|\u2BCC3|\u2BD3C|\u2BD75|\u2BD76|\u2BD77|\u2BD78|\u2BD79|\u2BD84|\u2BD85|\u2BD87|\u2BD8A|\u2BD95|\u2BDB2|\u2BDC5|\u2BDC8|\u2BDC9|\u2BDCC|\u2BDD8|\u2BDEC|\u2BDEE|\u2BDF7|\u2BDF9|\u2BDFE|\u2BE29|\u2BE6E|\u2BE74|\u2BE77|\u2BE7C|\u2BE7D|\u2BE81|\u2BE82|\u2BE86|\u2BE8A|\u2BE8C|\u2BE92|\u2BE93|\u2BE98|\u2BEAA|\u2BEAB|\u2BEB7|\u2BEB9|\u2BEC1|\u2BEC7|\u2BF17|\u2BF1D|\u2BF23|\u2BF24|\u2BF25|\u2BF27|\u2BF2B|\u2BF2E|\u2BF31|\u2BF32|\u2BF35|\u2BF36|\u2BF3D|\u2BF3E|\u2BF40|\u2BF41|\u2BF47|\u2BF4A|\u2BF4B|\u2BF50|\u2BF54|\u2BF59|\u2BF62|\u2BF63|\u2BF65|\u2BF67|\u2BF6B|\u2BF6E|\u2BF72|\u2BF73|\u2BF81|\u2BF83|\u2BF89|\u2BF8F|\u2BFB2|\u2BFB3|\u2BFC2|\u2BFD7|\u2C025|\u2C029|\u2C02A|\u2C02E|\u2C031|\u2C051|\u2C058|\u2C073|\u2C075|\u2C078|\u2C07A|\u2C07D|\u2C080|\u2C082|\u2C085|\u2C089|\u2C0A0|\u2C0A9|\u2C0AE|\u2C0B0|\u2C0B1|\u2C0BB|\u2C0C0|\u2C0CA|\u2C0CF|\u2C0D8|\u2C0DB|\u2C0E6|\u2C0EB|\u2C0EE|\u2C0F2|\u2C0F3|\u2C11E|\u2C129|\u2C12C|\u2C162|\u2C165|\u2C16B|\u2C182|\u2C199|\u2C1A6|\u2C1AE|\u2C1BE|\u2C1C3|\u2C1C4|\u2C1C7|\u2C1D5|\u2C1D8|\u2C1D9|\u2C1EC|\u2C1F0|\u2C1F9|\u2C1FC|\u2C201|\u2C20F|\u2C215|\u2C227|\u2C231|\u2C23E|\u2C242|\u2C247|\u2C24B|\u2C260|\u2C27C|\u2C282|\u2C288|\u2C289|\u2C28D|\u2C28E|\u2C296|\u2C297|\u2C29C|\u2C2A4|\u2C2A6|\u2C2B5|\u2C2B6|\u2C2BA|\u2C2BE|\u2C2C3|\u2C2CD|\u2C31D|\u2C320|\u2C32E|\u2C334|\u2C335|\u2C337|\u2C359|\u2C35B|\u2C361|\u2C364|\u2C386|\u2C391|\u2C3A7|\u2C3AC|\u2C3DC|\u2C3DF|\u2C3E4|\u2C3E6|\u2C3EB|\u2C3EE|\u2C3F7|\u2C420|\u2C446|\u2C447|\u2C44D|\u2C44F|\u2C452|\u2C453|\u2C455|\u2C457|\u2C459|\u2C467|\u2C484|\u2C486|\u2C487|\u2C488|\u2C48A|\u2C48D|\u2C48E|\u2C493|\u2C495|\u2C497|\u2C4E0|\u2C4EB|\u2C4F1|\u2C4F8|\u2C4FC|\u2C52F|\u2C539|\u2C542|\u2C544|\u2C54A|\u2C55B|\u2C566|\u2C56C|\u2C583|\u2C591|\u2C596|\u2C598|\u2C59E|\u2C59F|\u2C5A0|\u2C5AE|\u2C5BA|\u2C613|\u2C614|\u2C615|\u2C616|\u2C617|\u2C618|\u2C619|\u2C61A|\u2C61B|\u2C61C|\u2C61D|\u2C61E|\u2C61F|\u2C620|\u2C621|\u2C622|\u2C623|\u2C624|\u2C625|\u2C626|\u2C627|\u2C628|\u2C629|\u2C62A|\u2C62B|\u2C62C|\u2C62D|\u2C62E|\u2C62F|\u2C630|\u2C631|\u2C632|\u2C633|\u2C634|\u2C635|\u2C636|\u2C637|\u2C638|\u2C639|\u2C63A|\u2C63B|\u2C63C|\u2C63D|\u2C63E|\u2C63F|\u2C640|\u2C641|\u2C642|\u2C643|\u2C644|\u2C645|\u2C646|\u2C647|\u2C648|\u2C649|\u2C64A|\u2C64B|\u2C64E|\u2C64F|\u2C65D|\u2C66A|\u2C66B|\u2C66D|\u2C684|\u2C6F9|\u2C6FC|\u2C714|\u2C725|\u2C727|\u2C728|\u2C72C|\u2C72F|\u2C738|\u2C73A|\u2C73E|\u2C73F|\u2C741|\u2C743|\u2C74A|\u2C74B|\u2C756|\u2C760|\u2C76F|\u2C774|\u2C78B|\u2C795|\u2C798|\u2C79F|\u2C7A3|\u2C7AB|\u2C7C1|\u2C7EA|\u2C7FA|\u2C7FD|\u2C803|\u2C805|\u2C808|\u2C820|\u2C831|\u2C837|\u2C847|\u2C84D|\u2C84E|\u2C852|\u2C853|\u2C854|\u2C855|\u2C860|\u2C866|\u2C871|\u2C877|\u2C87B|\u2C887|\u2C888|\u2C889|\u2C88A|\u2C88B|\u2C88C|\u2C88D|\u2C88E|\u2C88F|\u2C890|\u2C891|\u2C892|\u2C893|\u2C894|\u2C895|\u2C8AA|\u2C8AF|\u2C8B3|\u2C8C0|\u2C8D9|\u2C8DA|\u2C8DB|\u2C8DC|\u2C8DD|\u2C8DE|\u2C8DF|\u2C8E0|\u2C8E1|\u2C8E2|\u2C8E3|\u2C8E4|\u2C8E5|\u2C8E6|\u2C8E7|\u2C8E8|\u2C8E9|\u2C8EA|\u2C8EB|\u2C8EC|\u2C8ED|\u2C8EE|\u2C8EF|\u2C8F0|\u2C8F1|\u2C8F2|\u2C8F3|\u2C8F4|\u2C8F5|\u2C8F6|\u2C8F7|\u2C8F8|\u2C8F9|\u2C8FA|\u2C8FB|\u2C8FC|\u2C8FD|\u2C8FE|\u2C8FF|\u2C900|\u2C901|\u2C902|\u2C903|\u2C904|\u2C905|\u2C906|\u2C907|\u2C908|\u2C909|\u2C90A|\u2C90B|\u2C90C|\u2C90D|\u2C90E|\u2C90F|\u2C910|\u2C911|\u2C912|\u2C913|\u2C914|\u2C915|\u2C916|\u2C917|\u2C918|\u2C919|\u2C91A|\u2C91B|\u2C91C|\u2C91D|\u2C91E|\u2C91F|\u2C920|\u2C921|\u2C922|\u2C923|\u2C924|\u2C925|\u2C926|\u2C927|\u2C928|\u2C929|\u2C92A|\u2C92B|\u2C92C|\u2C92D|\u2C92E|\u2C92F|\u2C930|\u2C931|\u2C937|\u2C944|\u2C948|\u2C973|\u2C974|\u2C975|\u2C976|\u2C977|\u2C978|\u2C979|\u2C97A|\u2C97B|\u2C97C|\u2C97D|\u2C97E|\u2C97F|\u2C980|\u2C985|\u2C986|\u2C9A3|\u2C9A5|\u2C9A7|\u2C9A9|\u2C9AB|\u2C9AF|\u2C9B4|\u2C9B5|\u2C9B9|\u2C9BB|\u2C9BE|\u2C9C0|\u2C9C3|\u2C9D1|\u2C9D4|\u2C9DA|\u2C9DB|\u2C9E2|\u2C9E4|\u2C9E9|\u2CA01|\u2CA02|\u2CA03|\u2CA04|\u2CA05|\u2CA06|\u2CA07|\u2CA08|\u2CA09|\u2CA0B|\u2CA0C|\u2CA0D|\u2CA0E|\u2CA0F|\u2CA10|\u2CA11|\u2CA12|\u2CA13|\u2CA14|\u2CA4E|\u2CA7D|\u2CA7E|\u2CA8D|\u2CAA7|\u2CAA8|\u2CAA9|\u2CAAB|\u2CAAF|\u2CABA|\u2CB07|\u2CB27|\u2CB28|\u2CB29|\u2CB2A|\u2CB2B|\u2CB2C|\u2CB2D|\u2CB2E|\u2CB2F|\u2CB30|\u2CB31|\u2CB32|\u2CB33|\u2CB34|\u2CB35|\u2CB36|\u2CB37|\u2CB38|\u2CB39|\u2CB3A|\u2CB3B|\u2CB3C|\u2CB3D|\u2CB3E|\u2CB3F|\u2CB40|\u2CB41|\u2CB42|\u2CB43|\u2CB45|\u2CB46|\u2CB47|\u2CB48|\u2CB49|\u2CB4A|\u2CB4B|\u2CB4C|\u2CB4D|\u2CB4E|\u2CB4F|\u2CB50|\u2CB51|\u2CB53|\u2CB54|\u2CB55|\u2CB56|\u2CB57|\u2CB58|\u2CB59|\u2CB5A|\u2CB5B|\u2CB5C|\u2CB5D|\u2CB5E|\u2CB5F|\u2CB60|\u2CB61|\u2CB62|\u2CB63|\u2CB64|\u2CB65|\u2CB66|\u2CB68|\u2CB69|\u2CB6A|\u2CB6B|\u2CB6C|\u2CB6D|\u2CB6F|\u2CB70|\u2CB71|\u2CB72|\u2CB73|\u2CB74|\u2CB75|\u2CB76|\u2CB77|\u2CB78|\u2CB79|\u2CB7A|\u2CB7B|\u2CB7C|\u2CB7D|\u2CB7E|\u2CB7F|\u2CB80|\u2CB81|\u2CB82|\u2CB83|\u2CB84|\u2CB98|\u2CB99|\u2CB9C|\u2CB9D|\u2CB9F|\u2CBA0|\u2CBA1|\u2CBA2|\u2CBA3|\u2CBA4|\u2CBA5|\u2CBA7|\u2CBA8|\u2CBA9|\u2CBAA|\u2CBAC|\u2CBAD|\u2CBAE|\u2CBAF|\u2CBB0|\u2CBB1|\u2CBB2|\u2CBB3|\u2CBB4|\u2CBB5|\u2CBB8|\u2CBB9|\u2CBBA|\u2CBBB|\u2CBBF|\u2CBC0|\u2CBC5|\u2CBCA|\u2CBCE|\u2CC21|\u2CC23|\u2CC24|\u2CC25|\u2CC31|\u2CC32|\u2CC33|\u2CC34|\u2CC35|\u2CC36|\u2CC37|\u2CC38|\u2CC3A|\u2CC53|\u2CC54|\u2CC55|\u2CC56|\u2CC57|\u2CC58|\u2CC59|\u2CC5A|\u2CC5B|\u2CC5C|\u2CC5D|\u2CC5E|\u2CC5F|\u2CC60|\u2CC61|\u2CC62|\u2CC63|\u2CC64|\u2CC65|\u2CC66|\u2CC67|\u2CC68|\u2CC69|\u2CC6A|\u2CC6B|\u2CC6C|\u2CC6D|\u2CC6E|\u2CC6F|\u2CC70|\u2CC71|\u2CC72|\u2CC73|\u2CC75|\u2CC77|\u2CC78|\u2CC7A|\u2CC7C|\u2CC7D|\u2CC7E|\u2CC7F|\u2CC80|\u2CC85|\u2CC86|\u2CC95|\u2CCA5|\u2CCA6|\u2CCA7|\u2CCA8|\u2CCA9|\u2CCAA|\u2CCAB|\u2CCAC|\u2CCAD|\u2CCAE|\u2CCAF|\u2CCB0|\u2CCB1|\u2CCB2|\u2CCB3|\u2CCB4|\u2CCB5|\u2CCB6|\u2CCB7|\u2CCB8|\u2CCB9|\u2CCBA|\u2CCBB|\u2CCBC|\u2CCBD|\u2CCBE|\u2CCBF|\u2CCC0|\u2CCC1|\u2CCC2|\u2CCC3|\u2CCC4|\u2CCC5|\u2CCC6|\u2CCC7|\u2CCC8|\u2CCC9|\u2CCCA|\u2CCCB|\u2CCCC|\u2CCCD|\u2CCCE|\u2CCD0|\u2CCD1|\u2CCD2|\u2CCD3|\u2CCD4|\u2CCD9|\u2CCDF|\u2CCF3|\u2CCF4|\u2CCF5|\u2CCF6|\u2CCF7|\u2CCF8|\u2CCF9|\u2CCFA|\u2CCFB|\u2CCFC|\u2CCFD|\u2CCFE|\u2CCFF|\u2CD00|\u2CD01|\u2CD02|\u2CD03|\u2CD04|\u2CD05|\u2CD06|\u2CD07|\u2CD08|\u2CD09|\u2CD0A|\u2CD0B|\u2CD0C|\u2CD0D|\u2CD0E|\u2CD0F|\u2CD10|\u2CD28|\u2CD29|\u2CD80|\u2CD81|\u2CD82|\u2CD83|\u2CD84|\u2CD85|\u2CD86|\u2CD87|\u2CD88|\u2CD89|\u2CD8A|\u2CD8B|\u2CD8C|\u2CD8D|\u2CD8E|\u2CD8F|\u2CD90|\u2CD91|\u2CD92|\u2CD93|\u2CD94|\u2CD95|\u2CD96|\u2CD97|\u2CD99|\u2CD9A|\u2CD9B|\u2CD9C|\u2CD9D|\u2CD9E|\u2CD9F|\u2CDA0|\u2CDA1|\u2CDA2|\u2CDA3|\u2CDA4|\u2CDA5|\u2CDA6|\u2CDA7|\u2CDA8|\u2CDA9|\u2CDAA|\u2CDAB|\u2CDAC|\u2CDAD|\u2CDAE|\u2CDAF|\u2CDB0|\u2CDB1|\u2CDB2|\u2CDB3|\u2CDB4|\u2CDB5|\u2CDB6|\u2CDB7|\u2CDB8|\u2CDB9|\u2CDBA|\u2CDBB|\u2CDD5|\u2CDFB|\u2CDFC|\u2CDFD|\u2CDFE|\u2CDFF|\u2CE00|\u2CE01|\u2CE02|\u2CE03|\u2CE04|\u2CE05|\u2CE06|\u2CE08|\u2CE09|\u2CE0A|\u2CE0B|\u2CE0C|\u2CE0D|\u2CE0E|\u2CE0F|\u2CE10|\u2CE11|\u2CE12|\u2CE13|\u2CE14|\u2CE15|\u2CE16|\u2CE17|\u2CE18|\u2CE19|\u2CE1A|\u2CE1B|\u2CE1C|\u2CE1D|\u2CE1E|\u2CE1F|\u2CE20|\u2CE21|\u2CE22|\u2CE23|\u2CE24|\u2CE25|\u2CE26|\u2CE27|\u2CE28|\u2CE29|\u2CE2A|\u2CE2B|\u2CE2C|\u2CE2D|\u2CE2E|\u2CE2F|\u2CE30|\u2CE31|\u2CE35|\u2CE36|\u2CE37|\u2CE38|\u2CE39|\u2CE3E|\u2CE45|\u2CE46|\u2CE47|\u2CE48|\u2CE49|\u2CE4A|\u2CE4B|\u2CE4C|\u2CE4D|\u2CE4E|\u2CE55|\u2CE56|\u2CE57|\u2CE58|\u2CE63|\u2CE64|\u2CE6D|\u2CE7A|\u2CE7B|\u2CE7C|\u2CE7D|\u2CE7E|\u2CE7F|\u2CE80|\u2CE81|\u2CE82|\u2CE83|\u2CE84|\u2CE85|\u2CE86|\u2CE87|\u2CE88|\u2CE89|\u2CE8A|\u2CE8B|\u2CE8C|\u2CE8D|\u2CE8E|\u2CE8F|\u2CE90|\u2CE91|\u2CE92|\u2CE93|\u2CE94|\u2CE95|\u2CE96|\u2CE9B|\u2CE9C|\u2CE9D|\u2CE9F|\u2CEEE|\u2CF96|\u2CFA3|\u2D11B|\u2D1C0|\u2D1C9|\u2D1D9|\u2D1DC|\u2D1E1|\u2D1EF|\u2D1F4|\u2D208|\u2D209|\u2D21C|\u2D21F|\u2D22E|\u2D257|\u2D268|\u2D27C|\u2D2B8|\u2D382|\u2D39C|\u2D3E6|\u2D3F8|\u2D478|\u2D479|\u2D4C0|\u2D546|\u2D613|\u2D61A|\u2D6A6|\u2D74B|\u2D76B|\u2D784|\u2D819|\u2D83D|\u2D846|\u2D85C|\u2D875|\u2D88B|\u2D895|\u2D89D|\u2D8C7|\u2D8E7|\u2D90E|\u2D930|\u2D953|\u2D9CB|\u2DA5A|\u2DA5B|\u2DA70|\u2DA86|\u2DAC0|\u2DAD9|\u2DADD|\u2DB48|\u2DC0E|\u2DC12|\u2DC17|\u2DC25|\u2DC40|\u2DC4A|\u2DCAB|\u2DD0A|\u2DD33|\u2DE5C|\u2DECD|\u2DED4|\u2E021|\u2E024|\u2E02A|\u2E032|\u2E14E|\u2E18F|\u2E1D4|\u2E1E4|\u2E260|\u2E261|\u2E262|\u2E263|\u2E264|\u2E265|\u2E267|\u2E268|\u2E269|\u2E26A|\u2E26B|\u2E26C|\u2E26D|\u2E26E|\u2E26F|\u2E30C|\u2E38D|\u2E3C0|\u2E3FA|\u2E41A|\u2E428|\u2E502|\u2E505|\u2E50A|\u2E581|\u2E583|\u2E5B1|\u2E64A|\u2E64B|\u2E6D7|\u2E736|\u2E774|\u2E775|\u2E777|\u2E778|\u2E779|\u2E77A|\u2E81E|\u2E833|\u2E8F2|\u2E8F3|\u2E8F4|\u2E8F5|\u2E8F6|\u2E92B|\u2E92C|\u2E92D|\u2E92E|\u2E92F|\u2E932|\u2E933|\u2E936|\u2E937|\u2E938|\u2E985|\u2E99A|\u2E9F4|\u2E9F5|\u2EA34|\u2EA35|\u2EA5B|\u2EA5C|\u2EA5D|\u2EA5E|\u2EAA1|\u2EAA2|\u2EAA3|\u2EAA4|\u2EAA5|\u2EAC2|\u2EB1B|\u2EB1C|\u2EB1D|\u2EB1E|\u2EB1F|\u2EB20|\u2EB21|\u2EB22|\u2EB23|\u2EB24|\u2EB61|\u2EB62|\u2EB64|\u2EB65|\u2EB66|\u2EB68|\u2EB6A|\u2EB70|\u2EB85|\u2EB87|\u2EBD9|\u30048|\u30067|\u30078|\u3007E|\u30081|\u30083|\u3008B|\u3008E|\u3008F|\u30097|\u3009C|\u300A6|\u300AD|\u300BB|\u300C6|\u300F3|\u300F6|\u300F7|\u300FB|\u300FF|\u30101|\u3011E|\u3012D|\u30154|\u30165|\u30166|\u3017B|\u30195|\u30199|\u3019A|\u301C0|\u301CA|\u301CE|\u301D5|\u301D6|\u301D8|\u301E0|\u301E1|\u301E3|\u301E5|\u301F2|\u301FC|\u30206|\u30207|\u3020A|\u3020D|\u30213|\u3022E|\u3022F|\u30236|\u30241|\u30244|\u30258|\u30259|\u3025A|\u30263|\u30265|\u30269|\u3026A|\u30271|\u3027D|\u30282|\u30285|\u30288|\u30291|\u3029B|\u3029F|\u302A1|\u302A2|\u302D6|\u302F8|\u302F9|\u302FD|\u302FE|\u30300|\u30302|\u30306|\u30307|\u30309|\u30319|\u30326|\u30337|\u3038C|\u3038E|\u3038F|\u30390|\u30391|\u30394|\u30396|\u3039B|\u3039D|\u3039E|\u303A0|\u303A2|\u303A6|\u303AB|\u303B4|\u303B7|\u303B9|\u303C1|\u303D3|\u303D5|\u303DC|\u303DF|\u303F2|\u303F6|\u303FC|\u303FD|\u3041A|\u3043E|\u30441|\u30444|\u30445|\u30454|\u30455|\u30459|\u3045F|\u30465|\u30467|\u3046A|\u3046B|\u3046C|\u30475|\u30478|\u3047F|\u30486|\u30492|\u30496|\u304C4|\u304C6|\u304D4|\u304D5|\u304D7|\u304D9|\u304DC|\u304DD|\u304DF|\u304E4|\u304E7|\u304EC|\u304F1|\u304F7|\u304FB|\u304FC|\u30507|\u3050B|\u30532|\u30536|\u30541|\u30545|\u30548|\u30550|\u3056D|\u30588|\u3058F|\u3059A|\u305A0|\u305A9|\u305C5|\u305C6|\u305D3|\u305D6|\u305D8|\u305D9|\u305DA|\u305DB|\u305DC|\u305E1|\u305E2|\u305E6|\u305E8|\u305EC|\u305F5|\u305F9|\u305FA|\u30600|\u30605|\u30608|\u30613|\u30620|\u30623|\u30629|\u30633|\u30636|\u30638|\u3064B|\u3064E|\u30651|\u30655|\u3068D|\u30694|\u306A6|\u306AA|\u306AC|\u306B1|\u306C9|\u306CA|\u306CF|\u306D2|\u306DB|\u306E1|\u306E3|\u306E4|\u306E5|\u306E6|\u306E8|\u306E9|\u306EA|\u306EE|\u306F1|\u306F2|\u306F5|\u306FA|\u306FB|\u306FD|\u30710|\u3071C|\u3071D|\u30722|\u30728|\u30733|\u30745|\u3074B|\u3074D|\u30757|\u3075C|\u3075E|\u3075F|\u30764|\u3077E|\u30787|\u30789|\u3078D|\u307A4|\u307B2|\u307B3|\u307B7|\u307BB|\u307C4|\u307D8|\u3081B|\u3082B|\u30832|\u30834|\u30839|\u30844|\u30849|\u3084A|\u3084B|\u3084E|\u3084F|\u30850|\u30854|\u3085E|\u30862|\u30869|\u30870|\u30875|\u3087B|\u3087D|\u30884|\u308A2|\u308A4|\u308A6|\u308E2|\u308E6|\u308E9|\u308EB|\u308EC|\u308EF|\u308F6|\u308FC|\u308FD|\u30913|\u30915|\u30928|\u3092B|\u3092C|\u3093D|\u3094A|\u30952|\u3095B|\u3095E|\u30960|\u30962|\u30963|\u30968|\u3096A|\u3096D|\u30979|\u30994|\u3099C|\u309A6|\u309A8|\u309AD|\u309B0|\u309B4|\u309B7|\u309BE|\u309BF|\u309C3|\u309C7|\u309C8|\u309C9|\u309CE|\u309D4|\u309D8|\u309F0|\u309FB|\u309FE|\u30A16|\u30A1C|\u30A26|\u30A33|\u30A45|\u30A4F|\u30A53|\u30A67|\u30A6E|\u30A72|\u30A78|\u30A79|\u30A7A|\u30A7B|\u30A8A|\u30A8F|\u30AA3|\u30AAA|\u30AAB|\u30AAD|\u30AB6|\u30ABC|\u30ABF|\u30ACB|\u30AD6|\u30AFC|\u30AFD|\u30AFF|\u30B00|\u30B01|\u30B02|\u30B03|\u30B05|\u30B06|\u30B07|\u30B08|\u30B09|\u30B0B|\u30B0C|\u30B0D|\u30B0E|\u30B0F|\u30B10|\u30B11|\u30B12|\u30B13|\u30B14|\u30B16|\u30B17|\u30B18|\u30B19|\u30B1A|\u30B1B|\u30B1C|\u30B1D|\u30B1E|\u30B1F|\u30B20|\u30B21|\u30B22|\u30B23|\u30B24|\u30B25|\u30B26|\u30B27|\u30B28|\u30B29|\u30B2A|\u30B2B|\u30B2C|\u30B2D|\u30B2E|\u30B2F|\u30B30|\u30B31|\u30B32|\u30B33|\u30B34|\u30B35|\u30B36|\u30B37|\u30B38|\u30B39|\u30B3A|\u30B3B|\u30B3C|\u30B3D|\u30B3E|\u30B3F|\u30B40|\u30B41|\u30B44|\u30B54|\u30B57|\u30B5A|\u30B62|\u30B63|\u30B79|\u30B85|\u30B87|\u30B99|\u30B9D|\u30BAD|\u30BB2|\u30BC2|\u30BCB|\u30BCE|\u30C06|\u30C0B|\u30C0C|\u30C0F|\u30C11|\u30C20|\u30C22|\u30C24|\u30C28|\u30C2E|\u30C31|\u30C33|\u30C34|\u30C35|\u30C36|\u30C37|\u30C39|\u30C3A|\u30C3E|\u30C3F|\u30C40|\u30C47|\u30C48|\u30C49|\u30C4A|\u30C4C|\u30C4D|\u30C50|\u30C51|\u30C5B|\u30C5D|\u30C5F|\u30C66|\u30C69|\u30C6E|\u30C6F|\u30C71|\u30C72|\u30C7E|\u30C81|\u30C82|\u30C92|\u30C96|\u30C9F|\u30CA0|\u30CAB|\u30CAC|\u30CAE|\u30CAF|\u30CB0|\u30CB2|\u30CB3|\u30CB4|\u30CB5|\u30CB6|\u30CB8|\u30CB9|\u30CBA|\u30CBB|\u30CC1|\u30CC2|\u30CC4|\u30CC6|\u30CCA|\u30CD7|\u30CDA|\u30CF2|\u30CF5|\u30CF8|\u30CF9|\u30CFA|\u30CFB|\u30CFC|\u30D02|\u30D15|\u30D16|\u30D17|\u30D18|\u30D19|\u30D1A|\u30D1B|\u30D1C|\u30D1D|\u30D1E|\u30D22|\u30D23|\u30D24|\u30D25|\u30D2F|\u30D4A|\u30D4C|\u30D4D|\u30D4E|\u30D4F|\u30D50|\u30D51|\u30D52|\u30D53|\u30D54|\u30D56|\u30D57|\u30D58|\u30D59|\u30D5A|\u30D5B|\u30D5C|\u30D5D|\u30D5E|\u30D60|\u30D61|\u30D62|\u30D63|\u30D64|\u30D65|\u30D66|\u30D67|\u30D68|\u30D69|\u30D6A|\u30D6B|\u30D6C|\u30D6D|\u30D6E|\u30D6F|\u30D70|\u30D71|\u30D72|\u30D73|\u30D74|\u30D75|\u30D76|\u30D77|\u30D78|\u30D79|\u30D7A|\u30D7B|\u30D7C|\u30D7D|\u30D7E|\u30D7F|\u30D80|\u30D81|\u30D82|\u30D83|\u30D84|\u30D85|\u30D86|\u30D87|\u30D88|\u30D89|\u30D8A|\u30D8B|\u30D8C|\u30D8D|\u30D8E|\u30D8F|\u30D91|\u30D94|\u30DA8|\u30DAC|\u30DDE|\u30DE0|\u30DE1|\u30DE2|\u30DE4|\u30DE5|\u30DE6|\u30DE7|\u30DE8|\u30DE9|\u30DEA|\u30DEB|\u30DEC|\u30DED|\u30DEE|\u30DF4|\u30DF5|\u30DF6|\u30DF8|\u30E07|\u30E08|\u30E0A|\u30E0E|\u30E10|\u30E14|\u30E1A|\u30E1B|\u30E1E|\u30E26|\u30E40|\u30E6F|\u30E71|\u30E72|\u30E73|\u30E74|\u30E75|\u30E76|\u30E77|\u30E78|\u30E7A|\u30E7B|\u30E7C|\u30E7D|\u30E7E|\u30E7F|\u30E80|\u30E81|\u30E82|\u30E83|\u30E84|\u30E85|\u30E86|\u30E87|\u30E88|\u30E89|\u30E8A|\u30E8B|\u30E8C|\u30E8D|\u30E8E|\u30E8F|\u30E90|\u30E91|\u30E92|\u30E93|\u30E94|\u30E95|\u30E96|\u30E97|\u30E98|\u30E99|\u30E9A|\u30E9B|\u30E9C|\u30E9D|\u30E9E|\u30E9F|\u30EA0|\u30EA1|\u30EA2|\u30EA3|\u30EA4|\u30EA8|\u30EAD|\u30EB2|\u30EB7|\u30EC6|\u30EDD|\u30EE1|\u30EE6|\u30EE8|\u30EEE|\u30EF3|\u30F05|\u30F0B|\u30F0F|\u30F11|\u30F3B|\u30F55|\u30F56|\u30F57|\u30F58|\u30F5A|\u30F5B|\u30F5C|\u30F5D|\u30F5E|\u30F60|\u30F61|\u30F62|\u30F63|\u30F65|\u30F66|\u30F67|\u30F68|\u30F69|\u30F6B|\u30F6C|\u30F6D|\u30F6E|\u30F6F|\u30F70|\u30F71|\u30F72|\u30F73|\u30F74|\u30F75|\u30F76|\u30F77|\u30F78|\u30F79|\u30F7A|\u30F7B|\u30F7C|\u30F7D|\u30F7E|\u30F7F|\u30F80|\u30F81|\u30F83|\u30F84|\u30F85|\u30F86|\u30F87|\u30F88|\u30F89|\u30F8B|\u30F8C|\u30F8D|\u30F8E|\u30F8F|\u30F90|\u30F91|\u30F92|\u30F93|\u30F95|\u30F96|\u30F97|\u30F98|\u30F99|\u30F9A|\u30F9B|\u30F9C|\u30F9D|\u30F9E|\u30F9F|\u30FA1|\u30FA2|\u30FA3|\u30FA4|\u30FA5|\u30FA6|\u30FA7|\u30FA8|\u30FA9|\u30FAA|\u30FAB|\u30FAC|\u30FAD|\u30FAE|\u30FAF|\u30FB0|\u30FB1|\u30FB2|\u30FB3|\u30FB4|\u30FB5|\u30FB6|\u30FB7|\u30FB8|\u30FB9|\u30FBA|\u30FBB|\u30FBC|\u30FBD|\u30FBE|\u30FBF|\u30FC0|\u30FC1|\u30FC2|\u30FC3|\u30FC4|\u30FC5|\u30FC6|\u30FC7|\u30FC8|\u30FC9|\u30FCA|\u30FD6|\u30FE5|\u30FE6|\u30FE7|\u30FE8|\u30FE9|\u30FEA|\u30FEB|\u30FEC|\u30FED|\u30FEF|\u30FF0|\u30FF3|\u30FF4|\u30FF5|\u30FF8|\u30FF9|\u30FFA|\u30FFB|\u30FFE|\u31011|\u31021|\u31052|\u3105E|\u31071|\u31073|\u31074|\u31076|\u31077|\u31079|\u3107A|\u3107D|\u3107E|\u31083|\u31084|\u31085|\u31086|\u31087|\u31088|\u31089|\u3108A|\u3108B|\u3108C|\u3108D|\u3108E|\u31090|\u310A0|\u310A1|\u310A2|\u310A3|\u310A4|\u310A5|\u310A6|\u310A7|\u310A8|\u310A9|\u310AB|\u310AC|\u310AD|\u310AE|\u310AF|\u310B0|\u310B1|\u310B2|\u310B3|\u310B4|\u310B5|\u310B6|\u310B7|\u310B8|\u310BA|\u310BB|\u310D4|\u310D5|\u310D6|\u310D7|\u310D8|\u310D9|\u310DA|\u310DB|\u310DC|\u310DD|\u310DE|\u310DF|\u310E0|\u310F1|\u310F2|\u310F3|\u310F4|\u310F5|\u310F7|\u310F8|\u310F9|\u310FA|\u310FC|\u310FD|\u310FE|\u310FF|\u31100|\u31101|\u31102|\u31103|\u31104|\u31105|\u31106|\u31107|\u31108|\u31109|\u3110A|\u3113C|\u3113D|\u3113E|\u3113F|\u31140|\u31141|\u31142|\u31143|\u31144|\u31145|\u31147|\u31148|\u31149|\u3114A|\u3114B|\u3114D|\u3114E|\u3114F|\u31150|\u31152|\u31153|\u31154|\u31155|\u31156|\u31157|\u31158|\u31159|\u3115A|\u3115B|\u3115C|\u3115D|\u3115E|\u3115F|\u31160|\u31161|\u31162|\u31163|\u31164|\u31165|\u31166|\u31167|\u31168|\u31169|\u3116A|\u3116B|\u3116C|\u3116E|\u31180|\u31181|\u31183|\u31184|\u31185|\u31186|\u31188|\u3118C|\u3118D|\u31196|\u31199|\u3119A|\u3119B|\u311CD|\u311CE|\u311CF|\u311D0|\u311D1|\u311D2|\u311D3|\u311D4|\u311D5|\u311D6|\u311D7|\u311D8|\u311D9|\u311DA|\u311DB|\u311DC|\u311DD|\u311DE|\u311DF|\u311E0|\u311E1|\u311E2|\u311E3|\u311E4|\u311E5|\u311E6|\u311E7|\u311E8|\u311E9|\u311EA|\u311EB|\u311EC|\u311ED|\u311EE|\u311EF|\u311F0|\u311F1|\u311F2|\u311F3|\u311F4|\u311F5|\u311F6|\u311F7|\u311F8|\u311F9|\u311FA|\u311FB|\u311FC|\u311FD|\u311FE|\u311FF|\u31200|\u31201|\u31202|\u31203|\u31204|\u31205|\u31206|\u31207|\u31208|\u31209|\u3120A|\u3120B|\u3120C|\u3120D|\u3120E|\u3120F|\u31210|\u31211|\u31212|\u31213|\u31214|\u31215|\u31216|\u31217|\u31218|\u31219|\u3121A|\u3121B|\u3121C|\u31247|\u31248|\u31249|\u3124A|\u3124B|\u3124C|\u3124D|\u3124E|\u3124F|\u31250|\u31251|\u31252|\u31253|\u31254|\u31255|\u31256|\u31257|\u31258|\u31259|\u3125A|\u3125B|\u3125C|\u3125D|\u3125E|\u3125F|\u31260|\u31261|\u31262|\u31263|\u31264|\u31265|\u31266|\u31267|\u31268|\u31269|\u3126A|\u3126B|\u3126C|\u3126D|\u3126E|\u3126F|\u31270|\u31271|\u31272|\u31273|\u31274|\u31275|\u31276|\u31277|\u31278|\u31279|\u3127A|\u3127B|\u3127C|\u3127D|\u3127E|\u3127F|\u31280|\u31281|\u31282|\u31283|\u31284|\u31285|\u31286|\u31287|\u31288|\u31289|\u3128A|\u3128B|\u3128C|\u3128D|\u3128E|\u3128F|\u31290|\u31291|\u31292|\u31293|\u31294|\u31295|\u31296|\u31297|\u31298|\u31299|\u3129A|\u3129B|\u3129C|\u3129D|\u3129E|\u3129F|\u312A0|\u312A1|\u312A2|\u312A3|\u312A4|\u312A5|\u312A6|\u312A7|\u312A8|\u312A9|\u312AA|\u312AB|\u312AC|\u312AD|\u312AE|\u312AF|\u312B0|\u312B1|\u312B2|\u312B3|\u312B4|\u312B5|\u312BA|\u312BB|\u312BC|\u312BD|\u312C2|\u312C4|\u312C5|\u312C6|\u312C7|\u312C8|\u312C9|\u312CA|\u312CB|\u312CC|\u312CD|\u312CE|\u312D0|\u312D1|\u312D3|\u312D4|\u312D5|\u312D6|\u312D7|\u312D8|\u312D9|\u312DA|\u312DC|\u312DD|\u312DF|\u312E0|\u312E1|\u312E2|\u312E3|\u312E4|\u312E5|\u312E6|\u312E8|\u312EA|\u312EB|\u312EC|\u312ED|\u312EE|\u312F1|\u312F4|\u312F6|\u312FE|\u312FF|\u31300|\u31301|\u31303|\u31304|\u31305|\u31306|\u31307|\u31308|\u31309|\u3130A|\u3130F|\u31315|\u31316|\u31317|\u31318|\u31319|\u3132B|\u3132C|\u3132D|\u3132E|\u3132F|\u31330|\u31331|\u31332|\u31333|\u31334|\u31335|\u31336|\u31337|\u31338|\u31339|\u3133A|\u3133C|\u3133D|\u31341|\u31342|\u31344|\u31345|\u31346|\u31347|\u31348|\u31349";
            string strTraditionalChinese = @"\u346E|\u346F|\u3473|\u3476|\u3493|\u349C|\u34A3|\u34BF|\u34C4|\u34D6|\u34E8|\u3503|\u3505|\u350B|\u351D|\u3522|\u3552|\u3562|\u35A6|\u35AE|\u35D9|\u35E2|\u35E3|\u35F0|\u35F2|\u35F6|\u35FB|\u35FC|\u35FF|\u3613|\u3614|\u3616|\u3619|\u361A|\u3624|\u3654|\u3661|\u3662|\u366C|\u367A|\u367E|\u36DD|\u3704|\u370F|\u3710|\u3717|\u371E|\u3722|\u3725|\u372D|\u372E|\u3737|\u373A|\u375E|\u375F|\u379E|\u37FA|\u3801|\u380F|\u3820|\u3823|\u3853|\u385E|\u3897|\u389D|\u3932|\u396E|\u3977|\u398A|\u398E|\u3996|\u399B|\u399E|\u39A6|\u39AC|\u39AD|\u3A1B|\u3A1F|\u3A25|\u3A3B|\u3A47|\u3A4B|\u3A4C|\u3A5C|\u3A63|\u3A6D|\u3A73|\u3A75|\u3A77|\u3A79|\u3A8E|\u3AB9|\u3B23|\u3B2E|\u3B93|\u3B9D|\u3BB2|\u3BC2|\u3BC6|\u3BE4|\u3BF8|\u3BFC|\u3C02|\u3C05|\u3C0D|\u3C30|\u3C33|\u3CAF|\u3CB0|\u3CB2|\u3D38|\u3D3F|\u3D4D|\u3D51|\u3D52|\u3D57|\u3D64|\u3D7E|\u3D86|\u3D8C|\u3D8D|\u3D8F|\u3D92|\u3D95|\u3DC3|\u3DCD|\u3DF2|\u3DF6|\u3DFB|\u3DFF|\u3E05|\u3E0A|\u3E10|\u3E53|\u3E7D|\u3E8F|\u3E91|\u3E9C|\u3EF6|\u3EFD|\u3F06|\u3F08|\u3F3B|\u3FB5|\u3FBA|\u3FC9|\u3FCE|\u3FD6|\u3FD7|\u3FE7|\u3FF9|\u4009|\u400D|\u4034|\u4039|\u405D|\u406A|\u4071|\u407B|\u408E|\u4093|\u40C1|\u40D5|\u40D8|\u40E2|\u40E3|\u40E4|\u40EE|\u40F4|\u4150|\u4158|\u4173|\u4185|\u4189|\u41D3|\u424D|\u4250|\u4251|\u4259|\u426C|\u4271|\u4272|\u4276|\u429C|\u429F|\u42AD|\u42B2|\u42B5|\u42B7|\u42BA|\u42C3|\u42C6|\u42CD|\u42CE|\u42CF|\u42D0|\u42D1|\u42D4|\u42D9|\u42DA|\u42E6|\u42EB|\u42F9|\u42FA|\u42FB|\u42FC|\u42FD|\u42FE|\u42FF|\u4301|\u4307|\u4308|\u430B|\u430C|\u4310|\u4316|\u431D|\u431E|\u431F|\u4325|\u432A|\u4330|\u4364|\u4377|\u437D|\u4398|\u4399|\u43B1|\u43CA|\u4422|\u4423|\u4437|\u4439|\u443D|\u4457|\u447C|\u44E3|\u4507|\u4508|\u4521|\u4561|\u4564|\u4573|\u4579|\u457C|\u4580|\u4585|\u459A|\u45C3|\u45C5|\u45E5|\u45FB|\u45FD|\u45FF|\u4654|\u4661|\u4671|\u467C|\u4686|\u4689|\u4695|\u469E|\u46A9|\u46B3|\u46B5|\u46BD|\u46C0|\u46C4|\u46CC|\u46CD|\u46D8|\u46DB|\u46DE|\u46E0|\u46E4|\u46EC|\u46ED|\u46F3|\u46FD|\u46FF|\u4700|\u4704|\u4709|\u470B|\u470D|\u470E|\u470F|\u4712|\u4716|\u471A|\u471D|\u474F|\u4755|\u476D|\u476F|\u477B|\u477C|\u4780|\u4781|\u4782|\u4788|\u4789|\u478B|\u4793|\u47B6|\u47C3|\u47C6|\u47CF|\u47D0|\u47FA|\u4806|\u481F|\u4820|\u4829|\u482E|\u4831|\u4841|\u4845|\u4847|\u484A|\u4850|\u4857|\u4858|\u485D|\u485F|\u4866|\u4869|\u4870|\u4874|\u4875|\u4876|\u4877|\u487B|\u487E|\u4888|\u48A8|\u490C|\u490D|\u4920|\u4924|\u4925|\u4928|\u4929|\u492A|\u492C|\u4935|\u4938|\u493B|\u493C|\u4944|\u4947|\u4951|\u4955|\u4956|\u4957|\u495B|\u495D|\u495E|\u4969|\u496F|\u4971|\u4974|\u4976|\u4977|\u4978|\u498C|\u498E|\u4998|\u499B|\u499D|\u499F|\u49AA|\u49AF|\u49B1|\u49B3|\u49DE|\u49E2|\u4A34|\u4A6B|\u4A8A|\u4A8D|\u4A8F|\u4A90|\u4A93|\u4A97|\u4A98|\u4A9C|\u4A9D|\u4AA5|\u4AB4|\u4ABC|\u4ABE|\u4AC0|\u4AC2|\u4AC8|\u4AC9|\u4ACC|\u4ACF|\u4AD0|\u4ADC|\u4ADF|\u4AE0|\u4AE5|\u4AE9|\u4AF4|\u4AF6|\u4AFB|\u4AFC|\u4AFE|\u4B00|\u4B02|\u4B05|\u4B0D|\u4B0E|\u4B10|\u4B13|\u4B14|\u4B18|\u4B1D|\u4B1E|\u4B1F|\u4B23|\u4B27|\u4B2A|\u4B2B|\u4B2C|\u4B2F|\u4B32|\u4B33|\u4B36|\u4B39|\u4B3E|\u4B40|\u4B43|\u4B45|\u4B47|\u4B48|\u4B49|\u4B51|\u4B52|\u4B53|\u4B54|\u4B55|\u4B58|\u4B5E|\u4B61|\u4B62|\u4B63|\u4B6D|\u4B7F|\u4B82|\u4B84|\u4B88|\u4B97|\u4B9D|\u4B9E|\u4BA0|\u4BA7|\u4BAB|\u4BB0|\u4BB2|\u4BB3|\u4BB8|\u4BBD|\u4BBE|\u4BBF|\u4BC0|\u4BE4|\u4C0E|\u4C10|\u4C16|\u4C2B|\u4C32|\u4C37|\u4C3B|\u4C3D|\u4C3E|\u4C40|\u4C41|\u4C42|\u4C45|\u4C47|\u4C4C|\u4C4D|\u4C4E|\u4C50|\u4C52|\u4C53|\u4C57|\u4C59|\u4C5A|\u4C5B|\u4C5C|\u4C5F|\u4C61|\u4C64|\u4C65|\u4C67|\u4C6C|\u4C6D|\u4C70|\u4C71|\u4C74|\u4C75|\u4C77|\u4C78|\u4C79|\u4C7B|\u4C7D|\u4C7E|\u4C81|\u4C85|\u4C89|\u4C8F|\u4C95|\u4C96|\u4C97|\u4C98|\u4C99|\u4C9A|\u4C9B|\u4CA8|\u4CB0|\u4CB8|\u4CB9|\u4CBC|\u4CC5|\u4CC7|\u4CCD|\u4CCF|\u4CD2|\u4CD3|\u4CD5|\u4CDA|\u4CDC|\u4CDF|\u4CE2|\u4CE4|\u4CE7|\u4CE8|\u4CEB|\u4CED|\u4CEE|\u4CF2|\u4CFA|\u4D07|\u4D08|\u4D09|\u4D0B|\u4D1A|\u4D1D|\u4D2C|\u4D2D|\u4D2E|\u4D31|\u4D32|\u4D33|\u4D34|\u4D35|\u4D37|\u4D38|\u4D39|\u4D3A|\u4D3D|\u4D42|\u4D43|\u4D46|\u4D50|\u4D58|\u4D73|\u4D74|\u4D76|\u4D77|\u4D95|\u4D97|\u4DA2|\u4DA3|\u4DA6|\u4DA7|\u4DA8|\u4DAA|\u4DB1|\u4DB2|\u4E1F|\u4E26|\u4E7E|\u4E82|\u4E9E|\u4F47|\u4F75|\u4F86|\u4F96|\u4FB6|\u4FC1|\u4FC2|\u4FD3|\u4FD4|\u4FE0|\u4FE5|\u5000|\u5006|\u5008|\u5009|\u500B|\u5011|\u502B|\u5032|\u5049|\u5051|\u5069|\u5074|\u5075|\u507D|\u508C|\u5091|\u5096|\u5098|\u5099|\u50AA|\u50AD|\u50AF|\u50B1|\u50B3|\u50B4|\u50B5|\u50B7|\u50BE|\u50C0|\u50C2|\u50C5|\u50C6|\u50C9|\u50CD|\u50D1|\u50D3|\u50D5|\u50D7|\u50DE|\u50E4|\u50E5|\u50E8|\u50E9|\u50F4|\u50F9|\u50FE|\u5100|\u5101|\u5102|\u5104|\u5105|\u5108|\u5109|\u5110|\u5114|\u5115|\u5116|\u5118|\u511F|\u5122|\u5123|\u5125|\u5129|\u512A|\u5130|\u5131|\u5132|\u5137|\u5138|\u5139|\u513A|\u513B|\u513C|\u514C|\u5152|\u5157|\u5167|\u5169|\u518A|\u51AA|\u51C8|\u51CD|\u51D4|\u51D9|\u51DC|\u51DF|\u51F1|\u5225|\u522A|\u5244|\u5247|\u524B|\u524E|\u5257|\u525B|\u525D|\u526E|\u5274|\u5275|\u5278|\u527E|\u5283|\u5287|\u5289|\u528A|\u528C|\u528D|\u528F|\u5291|\u5297|\u529A|\u52C1|\u52D1|\u52D5|\u52D9|\u52DB|\u52DD|\u52DE|\u52E2|\u52E3|\u52E9|\u52F1|\u52F4|\u52F5|\u52F8|\u52FB|\u532D|\u532F|\u5330|\u5331|\u5335|\u5340|\u5354|\u5368|\u537B|\u5399|\u53AD|\u53B1|\u53B2|\u53B4|\u53C3|\u53C4|\u53E2|\u5412|\u5433|\u5436|\u5442|\u54BC|\u54E1|\u54EF|\u5504|\u550A|\u5513|\u551A|\u553B|\u554F|\u555E|\u555F|\u5562|\u558E|\u559A|\u55AA|\u55AC|\u55AE|\u55B2|\u55C6|\u55C7|\u55CA|\u55CE|\u55DA|\u55E7|\u55E9|\u55F6|\u55F9|\u55FF|\u5604|\u5606|\u5607|\u560D|\u5613|\u5614|\u5616|\u5617|\u561C|\u5629|\u562A|\u562E|\u562F|\u5630|\u5633|\u5635|\u5638|\u563A|\u563D|\u5641|\u5645|\u5653|\u565A|\u565D|\u565E|\u5660|\u5665|\u5666|\u566F|\u5672|\u5674|\u5678|\u5679|\u5680|\u5682|\u5687|\u5688|\u568C|\u568D|\u5690|\u5695|\u5699|\u569B|\u569D|\u56A0|\u56A6|\u56A7|\u56A8|\u56A9|\u56AA|\u56AB|\u56AC|\u56B1|\u56B2|\u56B3|\u56B4|\u56B6|\u56B8|\u56BD|\u56BF|\u56C0|\u56C1|\u56C2|\u56C5|\u56C7|\u56C8|\u56C9|\u56CB|\u56D0|\u56D1|\u56D2|\u56D5|\u56EA|\u5707|\u570B|\u570D|\u5712|\u5713|\u5716|\u5718|\u571E|\u57B5|\u57B7|\u57C9|\u57E1|\u57E8|\u57EC|\u57F0|\u57F7|\u5805|\u5808|\u580A|\u5816|\u581A|\u581D|\u582F|\u5831|\u5834|\u584A|\u584B|\u584F|\u5852|\u5857|\u5862|\u5864|\u5875|\u5878|\u5879|\u587C|\u587F|\u5886|\u588A|\u588B|\u588F|\u589C|\u589D|\u58A0|\u58A2|\u58A7|\u58AE|\u58B3|\u58B6|\u58B7|\u58BE|\u58BF|\u58C7|\u58C8|\u58CB|\u58CD|\u58CF|\u58D0|\u58D2|\u58D3|\u58D4|\u58D7|\u58D8|\u58D9|\u58DA|\u58DB|\u58DD|\u58DE|\u58DF|\u58E0|\u58E2|\u58E3|\u58E7|\u58E9|\u58EA|\u58EF|\u58FA|\u58FC|\u58FD|\u5920|\u5922|\u593E|\u5950|\u5967|\u5969|\u596A|\u596B|\u596C|\u596E|\u596F|\u5972|\u597C|\u599D|\u59CD|\u59E6|\u5A19|\u5A1B|\u5A41|\u5A61|\u5A66|\u5A6D|\u5A78|\u5A81|\u5A88|\u5A9C|\u5AA7|\u5AAF|\u5AB0|\u5ABC|\u5ABD|\u5AC8|\u5AD7|\u5AE2|\u5AE5|\u5AE7|\u5AF5|\u5AFB|\u5AFF|\u5B03|\u5B05|\u5B07|\u5B08|\u5B0B|\u5B0C|\u5B10|\u5B12|\u5B19|\u5B21|\u5B23|\u5B24|\u5B26|\u5B2A|\u5B2E|\u5B30|\u5B38|\u5B3B|\u5B3E|\u5B44|\u5B46|\u5B47|\u5B4B|\u5B4C|\u5B4E|\u5B6B|\u5B72|\u5B78|\u5B7B|\u5B7E|\u5B7F|\u5BAE|\u5BE0|\u5BE2|\u5BE6|\u5BE7|\u5BE9|\u5BEA|\u5BEB|\u5BEC|\u5BEF|\u5BF5|\u5BF6|\u5BF7|\u5C07|\u5C08|\u5C0B|\u5C0D|\u5C0E|\u5C35|\u5C37|\u5C46|\u5C4D|\u5C53|\u5C5C|\u5C62|\u5C64|\u5C68|\u5C69|\u5C6C|\u5CA1|\u5CF4|\u5CF6|\u5CFD|\u5D0D|\u5D17|\u5D19|\u5D20|\u5D22|\u5D2C|\u5D31|\u5D35|\u5D50|\u5D77|\u5D78|\u5D7C|\u5D7D|\u5D7E|\u5D81|\u5D84|\u5D87|\u5D88|\u5D94|\u5D97|\u5DA0|\u5DA2|\u5DA4|\u5DA7|\u5DA9|\u5DAA|\u5DAE|\u5DB4|\u5DB8|\u5DB9|\u5DBA|\u5DBC|\u5DBD|\u5DC3|\u5DC6|\u5DCA|\u5DCB|\u5DD1|\u5DD2|\u5DD4|\u5DD6|\u5DD7|\u5DD8|\u5DDA|\u5DE0|\u5DF0|\u5E25|\u5E2B|\u5E33|\u5E34|\u5E36|\u5E40|\u5E43|\u5E53|\u5E57|\u5E58|\u5E5F|\u5E60|\u5E63|\u5E69|\u5E6B|\u5E6C|\u5E70|\u5E71|\u5E79|\u5E7A|\u5E7E|\u5EAB|\u5EB2|\u5EC1|\u5EC2|\u5EC4|\u5EC8|\u5ED4|\u5ED5|\u5ED7|\u5EDA|\u5EDD|\u5EDE|\u5EDF|\u5EE0|\u5EE1|\u5EE2|\u5EE3|\u5EE5|\u5EE7|\u5EE9|\u5EEC|\u5EEE|\u5EF3|\u5F12|\u5F33|\u5F35|\u5F37|\u5F44|\u5F48|\u5F4C|\u5F4D|\u5F4E|\u5F59|\u5F5E|\u5F60|\u5F65|\u5F72|\u5F8C|\u5F91|\u5F9E|\u5FA0|\u5FA9|\u5FB5|\u5FB9|\u5FBF|\u6046|\u6065|\u6085|\u608F|\u609E|\u60B5|\u60B6|\u60C0|\u60E1|\u60F1|\u60F2|\u60FB|\u6107|\u611B|\u611C|\u6128|\u6129|\u6134|\u6137|\u613E|\u6144|\u614B|\u614D|\u6150|\u6158|\u6159|\u615A|\u615F|\u6163|\u616A|\u616B|\u616E|\u616F|\u6171|\u6172|\u6173|\u6176|\u6178|\u6179|\u617A|\u6182|\u618A|\u618D|\u6190|\u6191|\u6192|\u6196|\u619A|\u61A2|\u61A4|\u61A6|\u61AA|\u61AB|\u61AE|\u61B2|\u61B4|\u61B6|\u61B8|\u61B9|\u61C0|\u61C7|\u61C9|\u61CC|\u61CD|\u61D3|\u61D5|\u61D8|\u61D9|\u61DC|\u61DF|\u61E0|\u61E3|\u61E4|\u61E7|\u61E8|\u61E9|\u61EB|\u61ED|\u61F0|\u61F2|\u61F6|\u61F7|\u61F8|\u61FA|\u61FC|\u61FE|\u6200|\u6201|\u6203|\u6207|\u6214|\u6227|\u6229|\u6230|\u6231|\u6232|\u6236|\u62CB|\u6329|\u633E|\u6368|\u636B|\u6381|\u6383|\u6384|\u6386|\u6397|\u6399|\u639A|\u639B|\u63A1|\u63C0|\u63DA|\u63DB|\u63EE|\u640A|\u640D|\u640E|\u6416|\u6417|\u6435|\u6436|\u6440|\u6443|\u644B|\u6450|\u6451|\u6455|\u6459|\u645C|\u645F|\u646A|\u646B|\u646F|\u6472|\u6473|\u6476|\u647B|\u647C|\u6488|\u648A|\u648B|\u648C|\u648F|\u6490|\u6493|\u649D|\u649F|\u64A3|\u64A5|\u64A7|\u64AB|\u64B2|\u64B3|\u64B6|\u64BB|\u64BE|\u64BF|\u64C1|\u64C3|\u64C4|\u64C7|\u64C8|\u64CA|\u64CB|\u64D3|\u64D4|\u64DA|\u64DF|\u64E0|\u64E3|\u64E5|\u64E7|\u64EA|\u64EB|\u64EC|\u64EF|\u64F0|\u64F1|\u64F2|\u64F3|\u64F4|\u64F7|\u64FA|\u64FB|\u64FC|\u64FD|\u64FE|\u6504|\u6506|\u650B|\u650E|\u650F|\u6511|\u6514|\u6516|\u6519|\u651B|\u651C|\u651D|\u651E|\u6522|\u6523|\u6524|\u6526|\u6527|\u6529|\u652A|\u652C|\u6533|\u6557|\u6558|\u6575|\u6578|\u657A|\u657F|\u6581|\u6582|\u6583|\u6584|\u6585|\u6586|\u6595|\u65AC|\u65B7|\u65B8|\u65BC|\u65DD|\u65DF|\u661C|\u6642|\u6649|\u665B|\u665D|\u6688|\u6689|\u6690|\u6698|\u669F|\u66A2|\u66AB|\u66C4|\u66C6|\u66C7|\u66C9|\u66CA|\u66CF|\u66D6|\u66E0|\u66E5|\u66E8|\u66EC|\u66ED|\u66EE|\u66F8|\u6703|\u6725|\u6727|\u6771|\u67F5|\u6871|\u687F|\u6894|\u6896|\u6898|\u689C|\u689D|\u689F|\u68B2|\u68C4|\u68C6|\u68D6|\u68D7|\u68DF|\u68E1|\u68E7|\u68F2|\u68F6|\u690F|\u691A|\u6932|\u6947|\u694A|\u694E|\u6953|\u6968|\u696D|\u6975|\u699D|\u69AA|\u69AE|\u69AF|\u69B2|\u69BF|\u69CB|\u69CD|\u69E4|\u69E7|\u69E8|\u69EB|\u69EE|\u69F3|\u69F6|\u69FB|\u69FC|\u6A01|\u6A02|\u6A05|\u6A13|\u6A19|\u6A1E|\u6A20|\u6A22|\u6A23|\u6A2B|\u6A32|\u6A33|\u6A38|\u6A39|\u6A3A|\u6A3B|\u6A3F|\u6A43|\u6A45|\u6A48|\u6A4B|\u6A5A|\u6A5F|\u6A62|\u6A68|\u6A6B|\u6A6F|\u6A81|\u6A82|\u6A89|\u6A8B|\u6A92|\u6A94|\u6A9B|\u6A9C|\u6A9F|\u6AA1|\u6AA2|\u6AA3|\u6AA5|\u6AAD|\u6AAE|\u6AAF|\u6AB0|\u6AB2|\u6AB3|\u6AB5|\u6AB8|\u6ABB|\u6ABE|\u6ABF|\u6AC3|\u6AC5|\u6ACD|\u6ACE|\u6ACF|\u6AD3|\u6ADA|\u6ADB|\u6ADD|\u6ADE|\u6ADF|\u6AE0|\u6AE2|\u6AE5|\u6AE7|\u6AE8|\u6AE9|\u6AEA|\u6AEB|\u6AEC|\u6AEF|\u6AF1|\u6AF3|\u6AF4|\u6AF6|\u6AF8|\u6AF9|\u6AFB|\u6AFD|\u6B04|\u6B07|\u6B0A|\u6B0D|\u6B0F|\u6B10|\u6B11|\u6B12|\u6B13|\u6B16|\u6B18|\u6B1E|\u6B3D|\u6B44|\u6B4D|\u6B50|\u6B55|\u6B57|\u6B5B|\u6B5E|\u6B5F|\u6B61|\u6B72|\u6B77|\u6B78|\u6B7F|\u6B98|\u6B9E|\u6BA2|\u6BA4|\u6BA8|\u6BAB|\u6BAE|\u6BAF|\u6BB0|\u6BB2|\u6BBA|\u6BBC|\u6BC0|\u6BC4|\u6BC6|\u6BCA|\u6BFF|\u6C00|\u6C02|\u6C08|\u6C0C|\u6C23|\u6C2B|\u6C2C|\u6C2D|\u6C33|\u6C7A|\u6C92|\u6C96|\u6CC1|\u6D36|\u6D79|\u6D7F|\u6D87|\u6DB7|\u6DBC|\u6DDA|\u6DE5|\u6DEA|\u6DF5|\u6DF6|\u6DFA|\u6E19|\u6E1B|\u6E22|\u6E26|\u6E2C|\u6E3E|\u6E4A|\u6E4B|\u6E5E|\u6E6F|\u6E88|\u6E96|\u6E9D|\u6EA1|\u6EA4|\u6EAB|\u6EAE|\u6EB0|\u6EB3|\u6EC4|\u6EC5|\u6ECC|\u6ECE|\u6EEC|\u6EED|\u6EEF|\u6EF2|\u6EF7|\u6EF8|\u6EFB|\u6EFE|\u6EFF|\u6F01|\u6F0A|\u6F0D|\u6F0E|\u6F10|\u6F19|\u6F1A|\u6F22|\u6F23|\u6F2C|\u6F32|\u6F35|\u6F38|\u6F3F|\u6F41|\u6F51|\u6F54|\u6F5A|\u6F5B|\u6F63|\u6F64|\u6F6C|\u6F6F|\u6F70|\u6F77|\u6F7F|\u6F80|\u6F85|\u6F86|\u6F87|\u6F92|\u6F96|\u6F97|\u6FA0|\u6FA2|\u6FA4|\u6FA6|\u6FA9|\u6FAB|\u6FAC|\u6FAE|\u6FB0|\u6FB1|\u6FBE|\u6FC1|\u6FC3|\u6FC4|\u6FC6|\u6FC7|\u6FCA|\u6FD5|\u6FD8|\u6FDA|\u6FDC|\u6FDF|\u6FE4|\u6FE7|\u6FEB|\u6FF0|\u6FF1|\u6FFA|\u6FFC|\u6FFE|\u6FFF|\u7001|\u7002|\u7003|\u7004|\u7005|\u7006|\u7007|\u7008|\u7009|\u700B|\u700F|\u7015|\u7018|\u7019|\u701D|\u701F|\u7020|\u7022|\u7026|\u7027|\u7028|\u702F|\u7030|\u7032|\u7033|\u7034|\u7035|\u703E|\u7043|\u7044|\u704D|\u7051|\u7052|\u7053|\u7055|\u7058|\u7059|\u705D|\u705F|\u7060|\u7061|\u7063|\u7064|\u7066|\u7067|\u707D|\u70BA|\u70CF|\u70F4|\u711B|\u7121|\u7147|\u7149|\u7152|\u7159|\u7162|\u7165|\u7169|\u716C|\u7171|\u717C|\u7182|\u7185|\u7189|\u718C|\u7192|\u7193|\u7195|\u7197|\u719E|\u71A1|\u71B0|\u71B1|\u71B2|\u71BE|\u71C0|\u71C1|\u71C8|\u71CC|\u71D2|\u71D6|\u71D8|\u71D9|\u71DC|\u71DF|\u71E1|\u71E6|\u71ED|\u71F0|\u71F4|\u71F5|\u71F6|\u71FC|\u71FD|\u71FE|\u7201|\u7203|\u7204|\u720D|\u7210|\u7213|\u7216|\u721B|\u7223|\u7225|\u7227|\u722D|\u723A|\u723E|\u7246|\u724B|\u7258|\u727C|\u727D|\u7285|\u7293|\u7296|\u729E|\u72A2|\u72A4|\u72A7|\u72C0|\u72F9|\u72FD|\u730C|\u730D|\u7319|\u7327|\u7336|\u733B|\u7341|\u7344|\u7345|\u734A|\u734E|\u7351|\u7356|\u735F|\u7362|\u7368|\u7369|\u736A|\u736B|\u736E|\u7370|\u7371|\u7372|\u7375|\u7377|\u7378|\u7379|\u737A|\u737B|\u737C|\u7380|\u7381|\u7382|\u73FC|\u73FE|\u7416|\u743A|\u743F|\u744B|\u7452|\u7459|\u7463|\u7464|\u7469|\u746A|\u7472|\u747B|\u747D|\u7489|\u748A|\u7495|\u7497|\u749B|\u749D|\u74A1|\u74A3|\u74A6|\u74AB|\u74AF|\u74B0|\u74B5|\u74B8|\u74B9|\u74BC|\u74BD|\u74BE|\u74C4|\u74C5|\u74CA|\u74CF|\u74D0|\u74D3|\u74D4|\u74D5|\u74DA|\u74DB|\u750A|\u750C|\u7512|\u7516|\u7522|\u755D|\u7562|\u756B|\u7570|\u7576|\u7587|\u758A|\u75D9|\u75EE|\u75FE|\u7602|\u760B|\u760D|\u7611|\u7612|\u7613|\u761E|\u7621|\u7627|\u762E|\u7631|\u7632|\u763A|\u7642|\u7646|\u7647|\u7648|\u7649|\u764E|\u7650|\u7658|\u765F|\u7660|\u7662|\u7664|\u7665|\u7667|\u7669|\u766A|\u766C|\u766D|\u766E|\u7670|\u7671|\u7672|\u7674|\u767C|\u769A|\u769F|\u76AA|\u76B0|\u76B8|\u76BA|\u76BE|\u76DC|\u76DE|\u76E1|\u76E3|\u76E4|\u76E7|\u76E8|\u76EA|\u7725|\u773E|\u774D|\u774F|\u7754|\u775C|\u775E|\u776A|\u7774|\u7793|\u7798|\u779B|\u779C|\u779E|\u77A1|\u77A4|\u77AF|\u77B1|\u77B6|\u77B7|\u77BC|\u77C9|\u77CA|\u77D1|\u77D3|\u77D5|\u77D6|\u77D8|\u77DA|\u77EF|\u77F2|\u785C|\u7864|\u7868|\u786F|\u7899|\u78A2|\u78A9|\u78AD|\u78B8|\u78BA|\u78BC|\u78BD|\u78D1|\u78D2|\u78DA|\u78E0|\u78E3|\u78E7|\u78EF|\u78F1|\u78F5|\u78FD|\u78FE|\u7904|\u7906|\u790B|\u790E|\u790F|\u7910|\u7912|\u7919|\u791A|\u791B|\u7925|\u7926|\u7929|\u792A|\u792B|\u792C|\u792E|\u7930|\u7931|\u7932|\u7939|\u797F|\u798D|\u798E|\u7993|\u7995|\u799C|\u79A1|\u79A6|\u79AA|\u79AC|\u79AE|\u79AF|\u79B0|\u79B1|\u79B5|\u79BF|\u79C8|\u7A05|\u7A08|\u7A0F|\u7A1F|\u7A2E|\u7A31|\u7A40|\u7A47|\u7A4C|\u7A4D|\u7A4E|\u7A56|\u7A60|\u7A61|\u7A62|\u7A67|\u7A68|\u7A69|\u7A6B|\u7A6C|\u7A6D|\u7AA9|\u7AAA|\u7AAE|\u7AAF|\u7AB1|\u7AB5|\u7AB6|\u7ABA|\u7AC0|\u7AC4|\u7AC5|\u7AC7|\u7AC9|\u7ACA|\u7AF1|\u7AF6|\u7B46|\u7B4D|\u7B67|\u7B74|\u7B8B|\u7B8F|\u7BB9|\u7BC0|\u7BC4|\u7BC9|\u7BCB|\u7BD4|\u7BD8|\u7BE2|\u7BE4|\u7BE9|\u7BF3|\u7BF5|\u7BF8|\u7BFF|\u7C00|\u7C02|\u7C0D|\u7C1C|\u7C1E|\u7C21|\u7C22|\u7C23|\u7C25|\u7C2B|\u7C35|\u7C39|\u7C3B|\u7C3D|\u7C3E|\u7C43|\u7C4B|\u7C4C|\u7C54|\u7C59|\u7C5A|\u7C5B|\u7C5C|\u7C5F|\u7C60|\u7C63|\u7C66|\u7C69|\u7C6A|\u7C6B|\u7C6C|\u7C6D|\u7C6E|\u7C6F|\u7CAF|\u7CB5|\u7CBB|\u7CDD|\u7CDE|\u7CE7|\u7CEE|\u7CF0|\u7CF2|\u7CF4|\u7CF6|\u7CF7|\u7CF9|\u7CFA|\u7CFD|\u7CFE|\u7D00|\u7D02|\u7D03|\u7D04|\u7D05|\u7D06|\u7D07|\u7D08|\u7D09|\u7D0B|\u7D0C|\u7D0D|\u7D10|\u7D11|\u7D12|\u7D13|\u7D14|\u7D15|\u7D16|\u7D17|\u7D18|\u7D19|\u7D1A|\u7D1B|\u7D1C|\u7D1D|\u7D1E|\u7D1F|\u7D21|\u7D28|\u7D29|\u7D2C|\u7D2D|\u7D30|\u7D31|\u7D32|\u7D33|\u7D35|\u7D36|\u7D38|\u7D39|\u7D3A|\u7D3C|\u7D3D|\u7D3E|\u7D3F|\u7D40|\u7D41|\u7D42|\u7D43|\u7D44|\u7D45|\u7D46|\u7D47|\u7D4D|\u7D4E|\u7D50|\u7D51|\u7D53|\u7D55|\u7D56|\u7D58|\u7D59|\u7D5A|\u7D5B|\u7D5D|\u7D5E|\u7D5F|\u7D60|\u7D61|\u7D62|\u7D63|\u7D64|\u7D65|\u7D66|\u7D67|\u7D68|\u7D6A|\u7D6F|\u7D70|\u7D71|\u7D72|\u7D73|\u7D78|\u7D79|\u7D7A|\u7D7B|\u7D7C|\u7D7D|\u7D7E|\u7D7F|\u7D80|\u7D81|\u7D83|\u7D84|\u7D85|\u7D86|\u7D87|\u7D88|\u7D8A|\u7D8B|\u7D8C|\u7D8D|\u7D8E|\u7D8F|\u7D90|\u7D93|\u7D95|\u7D96|\u7D9C|\u7D9D|\u7D9E|\u7D9F|\u7DA0|\u7DA1|\u7DA2|\u7DA3|\u7DA7|\u7DAA|\u7DAC|\u7DAD|\u7DAF|\u7DB0|\u7DB1|\u7DB2|\u7DB4|\u7DB5|\u7DB7|\u7DB8|\u7DB9|\u7DBA|\u7DBB|\u7DBC|\u7DBD|\u7DBE|\u7DBF|\u7DC0|\u7DC1|\u7DC2|\u7DC4|\u7DC5|\u7DC6|\u7DC7|\u7DC9|\u7DCA|\u7DCB|\u7DCC|\u7DCD|\u7DCE|\u7DCF|\u7DD2|\u7DD3|\u7DD4|\u7DD7|\u7DD8|\u7DD9|\u7DDA|\u7DDB|\u7DDD|\u7DDE|\u7DDF|\u7DE0|\u7DE1|\u7DE2|\u7DE3|\u7DE4|\u7DE6|\u7DE7|\u7DE8|\u7DE9|\u7DEA|\u7DEB|\u7DEC|\u7DEE|\u7DEF|\u7DF0|\u7DF1|\u7DF2|\u7DF4|\u7DF5|\u7DF6|\u7DF7|\u7DF8|\u7DF9|\u7DFA|\u7DFB|\u7E08|\u7E09|\u7E0A|\u7E0B|\u7E0C|\u7E0D|\u7E0E|\u7E10|\u7E11|\u7E12|\u7E13|\u7E15|\u7E16|\u7E17|\u7E1A|\u7E1B|\u7E1C|\u7E1D|\u7E1E|\u7E1F|\u7E21|\u7E23|\u7E27|\u7E29|\u7E2A|\u7E2B|\u7E2C|\u7E2D|\u7E2E|\u7E2F|\u7E30|\u7E31|\u7E32|\u7E33|\u7E34|\u7E35|\u7E36|\u7E37|\u7E38|\u7E39|\u7E3A|\u7E3C|\u7E3D|\u7E3E|\u7E3F|\u7E40|\u7E42|\u7E43|\u7E45|\u7E46|\u7E48|\u7E4E|\u7E4F|\u7E50|\u7E51|\u7E52|\u7E53|\u7E54|\u7E55|\u7E56|\u7E57|\u7E58|\u7E59|\u7E5A|\u7E5C|\u7E5E|\u7E5F|\u7E61|\u7E62|\u7E63|\u7E68|\u7E69|\u7E6A|\u7E6B|\u7E6C|\u7E6D|\u7E6E|\u7E6F|\u7E70|\u7E72|\u7E73|\u7E75|\u7E76|\u7E77|\u7E78|\u7E79|\u7E7B|\u7E7C|\u7E7D|\u7E7E|\u7E7F|\u7E80|\u7E81|\u7E83|\u7E86|\u7E87|\u7E88|\u7E8A|\u7E8B|\u7E8C|\u7E8D|\u7E8F|\u7E91|\u7E93|\u7E95|\u7E96|\u7E97|\u7E98|\u7E9A|\u7E9C|\u7F3D|\u7F46|\u7F48|\u7F4C|\u7F4F|\u7F70|\u7F75|\u7F77|\u7F7C|\u7F82|\u7F85|\u7F86|\u7F88|\u7F8B|\u7FA5|\u7FA9|\u7FB5|\u7FD2|\u7FDC|\u7FEC|\u7FF9|\u7FFD|\u7FFF|\u802C|\u802E|\u8056|\u805E|\u806F|\u8070|\u8072|\u8073|\u8075|\u8076|\u8077|\u8079|\u807B|\u807D|\u807E|\u8085|\u8105|\u8108|\u811B|\u8125|\u812B|\u8139|\u814E|\u8156|\u8161|\u8166|\u816A|\u816B|\u8173|\u8178|\u8183|\u8192|\u8195|\u819A|\u819E|\u81A0|\u81A2|\u81A9|\u81AE|\u81B4|\u81B6|\u81B7|\u81B9|\u81BD|\u81BE|\u81BF|\u81C7|\u81C9|\u81CD|\u81CF|\u81D7|\u81D8|\u81DA|\u81DF|\u81E0|\u81E1|\u81E2|\u81E8|\u81FA|\u8207|\u8208|\u8209|\u820A|\u8259|\u825B|\u825C|\u8264|\u8266|\u826B|\u826D|\u8271|\u8277|\u82BB|\u82E7|\u8332|\u834A|\u838A|\u8396|\u83A2|\u83A7|\u83D5|\u83EF|\u8407|\u840A|\u842C|\u842F|\u8434|\u8435|\u8449|\u8452|\u8457|\u845D|\u8464|\u8466|\u8477|\u847B|\u848D|\u8492|\u8494|\u849E|\u84AD|\u84B3|\u84B6|\u84BC|\u84C0|\u84CB|\u84EE|\u84EF|\u84F2|\u84F4|\u84FD|\u8504|\u850E|\u851E|\u8520|\u8523|\u8525|\u8526|\u852A|\u852D|\u852E|\u852F|\u8531|\u8541|\u8544|\u8546|\u854E|\u8551|\u8552|\u8553|\u8555|\u8558|\u855D|\u855F|\u8561|\u8562|\u8567|\u8569|\u856A|\u856D|\u8573|\u8577|\u857D|\u8580|\u8586|\u8588|\u8589|\u858A|\u858B|\u858C|\u8594|\u8596|\u8598|\u859F|\u85A0|\u85A6|\u85A9|\u85B1|\u85B2|\u85B3|\u85B4|\u85B5|\u85BA|\u85C7|\u85CD|\u85CE|\u85D6|\u85D8|\u85DA|\u85DD|\u85E3|\u85E5|\u85EA|\u85EC|\u85F0|\u85F6|\u85F7|\u85F9|\u85FA|\u85FE|\u8600|\u8604|\u8606|\u8607|\u8608|\u860A|\u860B|\u861A|\u861E|\u861F|\u8621|\u8622|\u862B|\u862C|\u862D|\u8631|\u8635|\u8639|\u863A|\u863F|\u8645|\u8646|\u8649|\u8655|\u865B|\u865C|\u865F|\u8666|\u8667|\u866F|\u86F5|\u86FA|\u86FB|\u86FC|\u8706|\u8726|\u8738|\u873D|\u8740|\u8741|\u8755|\u875C|\u875F|\u8766|\u8778|\u8784|\u8798|\u879E|\u87A2|\u87AE|\u87B4|\u87B9|\u87BB|\u87BF|\u87C2|\u87C4|\u87C8|\u87CE|\u87D8|\u87DC|\u87E1|\u87E3|\u87E6|\u87EC|\u87EF|\u87F1|\u87F2|\u87F3|\u87F6|\u87F7|\u87FB|\u87FD|\u8800|\u8801|\u8805|\u8806|\u8808|\u880C|\u8810|\u8811|\u8812|\u8819|\u881E|\u881F|\u8823|\u8826|\u8828|\u882A|\u8831|\u8833|\u8836|\u883B|\u883E|\u8853|\u8855|\u885A|\u885B|\u885D|\u889E|\u88CA|\u88CC|\u88DC|\u88DD|\u88E1|\u88F2|\u88FD|\u8907|\u890C|\u8918|\u892D|\u8932|\u8933|\u8938|\u893A|\u893B|\u8940|\u8942|\u8947|\u894C|\u894F|\u8953|\u8956|\u8957|\u8958|\u895B|\u895D|\u8960|\u8964|\u8968|\u896A|\u896C|\u896D|\u896F|\u8970|\u8971|\u8972|\u8974|\u8975|\u8978|\u8979|\u897C|\u8986|\u898B|\u898E|\u898F|\u8992|\u8993|\u8995|\u8996|\u8997|\u8998|\u899B|\u899C|\u899F|\u89A0|\u89A1|\u89A2|\u89A4|\u89A5|\u89A6|\u89A9|\u89AA|\u89AC|\u89AD|\u89AF|\u89B0|\u89B2|\u89B4|\u89B6|\u89B7|\u89B8|\u89B9|\u89BA|\u89BB|\u89BC|\u89BD|\u89BF|\u89C0|\u89F4|\u89F6|\u89F7|\u89F8|\u89F9|\u89FB|\u89FD|\u8A01|\u8A02|\u8A03|\u8A06|\u8A08|\u8A0A|\u8A0C|\u8A0E|\u8A0F|\u8A10|\u8A11|\u8A12|\u8A13|\u8A15|\u8A16|\u8A17|\u8A18|\u8A1B|\u8A1C|\u8A1D|\u8A1E|\u8A1F|\u8A22|\u8A23|\u8A25|\u8A26|\u8A27|\u8A28|\u8A29|\u8A2A|\u8A2C|\u8A2D|\u8A30|\u8A31|\u8A34|\u8A36|\u8A38|\u8A39|\u8A3A|\u8A3B|\u8A3D|\u8A40|\u8A41|\u8A43|\u8A44|\u8A45|\u8A46|\u8A47|\u8A49|\u8A4A|\u8A4C|\u8A4D|\u8A4E|\u8A4F|\u8A50|\u8A51|\u8A52|\u8A53|\u8A54|\u8A55|\u8A56|\u8A57|\u8A58|\u8A5B|\u8A5C|\u8A5D|\u8A5E|\u8A60|\u8A61|\u8A62|\u8A63|\u8A65|\u8A66|\u8A68|\u8A69|\u8A6A|\u8A6B|\u8A6C|\u8A6D|\u8A6E|\u8A6F|\u8A70|\u8A71|\u8A72|\u8A73|\u8A74|\u8A75|\u8A76|\u8A77|\u8A7A|\u8A7B|\u8A7C|\u8A7F|\u8A82|\u8A83|\u8A84|\u8A85|\u8A86|\u8A87|\u8A8B|\u8A8C|\u8A8D|\u8A8E|\u8A8F|\u8A90|\u8A91|\u8A92|\u8A94|\u8A95|\u8A97|\u8A98|\u8A99|\u8A9A|\u8A9C|\u8A9E|\u8AA0|\u8AA1|\u8AA3|\u8AA4|\u8AA5|\u8AA6|\u8AA7|\u8AA8|\u8AAA|\u8AAB|\u8AB0|\u8AB2|\u8AB3|\u8AB4|\u8AB6|\u8AB7|\u8AB9|\u8ABA|\u8ABB|\u8ABC|\u8ABD|\u8ABE|\u8ABF|\u8AC1|\u8AC2|\u8AC3|\u8AC4|\u8AC6|\u8AC7|\u8AC8|\u8AC9|\u8ACB|\u8ACD|\u8ACE|\u8ACF|\u8AD1|\u8AD2|\u8AD3|\u8AD4|\u8AD5|\u8AD6|\u8AD7|\u8ADB|\u8ADC|\u8ADD|\u8ADE|\u8ADF|\u8AE0|\u8AE2|\u8AE3|\u8AE4|\u8AE5|\u8AE6|\u8AE7|\u8AE9|\u8AEB|\u8AED|\u8AEE|\u8AEF|\u8AF0|\u8AF1|\u8AF2|\u8AF3|\u8AF4|\u8AF6|\u8AF7|\u8AF8|\u8AF9|\u8AFA|\u8AFB|\u8AFC|\u8AFE|\u8B00|\u8B01|\u8B02|\u8B04|\u8B05|\u8B06|\u8B09|\u8B0A|\u8B0B|\u8B0C|\u8B0D|\u8B0E|\u8B0F|\u8B10|\u8B11|\u8B14|\u8B16|\u8B17|\u8B19|\u8B1A|\u8B1B|\u8B1C|\u8B1D|\u8B1E|\u8B1F|\u8B20|\u8B23|\u8B25|\u8B28|\u8B2B|\u8B2C|\u8B2D|\u8B2F|\u8B30|\u8B31|\u8B32|\u8B33|\u8B34|\u8B35|\u8B38|\u8B39|\u8B3B|\u8B3C|\u8B3E|\u8B40|\u8B42|\u8B44|\u8B45|\u8B46|\u8B47|\u8B48|\u8B49|\u8B4A|\u8B4C|\u8B4E|\u8B4F|\u8B50|\u8B51|\u8B53|\u8B54|\u8B56|\u8B58|\u8B59|\u8B5A|\u8B5C|\u8B5E|\u8B5F|\u8B60|\u8B61|\u8B68|\u8B69|\u8B6B|\u8B6F|\u8B70|\u8B73|\u8B74|\u8B77|\u8B78|\u8B79|\u8B7A|\u8B7B|\u8B7C|\u8B7D|\u8B7E|\u8B7F|\u8B80|\u8B82|\u8B85|\u8B86|\u8B87|\u8B89|\u8B8A|\u8B8B|\u8B8C|\u8B8E|\u8B91|\u8B92|\u8B93|\u8B94|\u8B95|\u8B96|\u8B98|\u8B99|\u8B9A|\u8B9B|\u8B9C|\u8B9D|\u8B9E|\u8B9F|\u8C44|\u8C45|\u8C48|\u8C4E|\u8C50|\u8C6C|\u8C75|\u8C76|\u8C93|\u8C97|\u8C99|\u8C9D|\u8C9E|\u8C9F|\u8CA0|\u8CA1|\u8CA2|\u8CA3|\u8CA4|\u8CA6|\u8CA7|\u8CA8|\u8CA9|\u8CAA|\u8CAB|\u8CAC|\u8CAF|\u8CB0|\u8CB1|\u8CB2|\u8CB3|\u8CB4|\u8CB6|\u8CB7|\u8CB8|\u8CBA|\u8CBB|\u8CBC|\u8CBD|\u8CBE|\u8CBF|\u8CC0|\u8CC1|\u8CC2|\u8CC3|\u8CC4|\u8CC5|\u8CC7|\u8CC8|\u8CCA|\u8CD1|\u8CD2|\u8CD3|\u8CD5|\u8CD7|\u8CD9|\u8CDA|\u8CDC|\u8CDD|\u8CDE|\u8CDF|\u8CE0|\u8CE1|\u8CE2|\u8CE3|\u8CE4|\u8CE5|\u8CE6|\u8CE7|\u8CE8|\u8CEA|\u8CEC|\u8CED|\u8CEE|\u8CF0|\u8CF4|\u8CF5|\u8CF6|\u8CF8|\u8CF9|\u8CFA|\u8CFB|\u8CFC|\u8CFD|\u8CFE|\u8D03|\u8D04|\u8D05|\u8D06|\u8D07|\u8D08|\u8D09|\u8D0A|\u8D0B|\u8D0D|\u8D0F|\u8D10|\u8D11|\u8D13|\u8D14|\u8D15|\u8D16|\u8D17|\u8D19|\u8D1A|\u8D1B|\u8D6C|\u8D95|\u8D99|\u8DA8|\u8DAB|\u8DAC|\u8DB2|\u8DE1|\u8E10|\u8E1A|\u8E34|\u8E4C|\u8E54|\u8E55|\u8E5B|\u8E61|\u8E63|\u8E64|\u8E65|\u8E6A|\u8E73|\u8E7A|\u8E7B|\u8E80|\u8E82|\u8E89|\u8E8A|\u8E8B|\u8E8D|\u8E8E|\u8E91|\u8E92|\u8E93|\u8E95|\u8E98|\u8E9A|\u8E9D|\u8EA1|\u8EA5|\u8EA6|\u8EA7|\u8EAA|\u8EC0|\u8EC2|\u8EC3|\u8EC7|\u8EC9|\u8ECA|\u8ECB|\u8ECC|\u8ECD|\u8ECE|\u8ECF|\u8ED1|\u8ED2|\u8ED3|\u8ED4|\u8ED5|\u8ED6|\u8ED7|\u8ED8|\u8EDB|\u8EDC|\u8EDD|\u8EDE|\u8EDF|\u8EE4|\u8EE5|\u8EE7|\u8EE8|\u8EEB|\u8EEC|\u8EEE|\u8EEF|\u8EF1|\u8EF2|\u8EF3|\u8EF5|\u8EF7|\u8EF8|\u8EF9|\u8EFA|\u8EFB|\u8EFC|\u8EFE|\u8EFF|\u8F00|\u8F01|\u8F02|\u8F03|\u8F04|\u8F05|\u8F06|\u8F07|\u8F08|\u8F09|\u8F0A|\u8F0B|\u8F10|\u8F11|\u8F12|\u8F13|\u8F14|\u8F15|\u8F16|\u8F17|\u8F18|\u8F19|\u8F1A|\u8F1B|\u8F1C|\u8F1D|\u8F1E|\u8F1F|\u8F20|\u8F21|\u8F22|\u8F23|\u8F24|\u8F25|\u8F26|\u8F28|\u8F29|\u8F2A|\u8F2B|\u8F2C|\u8F2E|\u8F2F|\u8F32|\u8F33|\u8F34|\u8F35|\u8F36|\u8F37|\u8F38|\u8F39|\u8F3B|\u8F3E|\u8F3F|\u8F40|\u8F42|\u8F43|\u8F44|\u8F45|\u8F46|\u8F47|\u8F48|\u8F49|\u8F4A|\u8F4D|\u8F4E|\u8F4F|\u8F50|\u8F51|\u8F52|\u8F53|\u8F54|\u8F55|\u8F56|\u8F57|\u8F58|\u8F59|\u8F5A|\u8F5B|\u8F5D|\u8F5E|\u8F5F|\u8F60|\u8F61|\u8F62|\u8F63|\u8F64|\u8F65|\u8FA6|\u8FAD|\u8FAE|\u8FAF|\u8FB2|\u9015|\u9019|\u9023|\u9032|\u903F|\u904B|\u904E|\u9054|\u9055|\u9059|\u905C|\u905E|\u9060|\u9069|\u9070|\u9071|\u9072|\u9076|\u9077|\u9078|\u907A|\u907C|\u9081|\u9084|\u9087|\u908A|\u908F|\u9090|\u90DF|\u90F2|\u90F5|\u9106|\u9109|\u9112|\u9114|\u9116|\u911F|\u9121|\u9126|\u9127|\u9129|\u912A|\u912C|\u912D|\u912E|\u9130|\u9132|\u9133|\u9134|\u9136|\u913A|\u9147|\u9148|\u9186|\u919C|\u919E|\u91A6|\u91A7|\u91AB|\u91AC|\u91B1|\u91B2|\u91B3|\u91B6|\u91C0|\u91C1|\u91C3|\u91C5|\u91CB|\u91D0|\u91D2|\u91D3|\u91D4|\u91D5|\u91D7|\u91D8|\u91D9|\u91DA|\u91DB|\u91DD|\u91DF|\u91E3|\u91E4|\u91E5|\u91E6|\u91E7|\u91E8|\u91E9|\u91EA|\u91EB|\u91EC|\u91ED|\u91F1|\u91F2|\u91F3|\u91F4|\u91F5|\u91F7|\u91F9|\u91FA|\u91FD|\u91FE|\u91FF|\u9200|\u9201|\u9202|\u9203|\u9204|\u9206|\u9207|\u9208|\u9209|\u920B|\u920D|\u920E|\u920F|\u9210|\u9211|\u9212|\u9213|\u9214|\u9215|\u9216|\u9217|\u921A|\u921B|\u921C|\u921E|\u9220|\u9223|\u9224|\u9225|\u9226|\u9227|\u922A|\u922E|\u922F|\u9230|\u9232|\u9233|\u9234|\u9235|\u9236|\u9237|\u9238|\u9239|\u923A|\u923C|\u923D|\u923E|\u923F|\u9240|\u9241|\u9245|\u9248|\u9249|\u924A|\u924B|\u924C|\u924D|\u924E|\u924F|\u9250|\u9251|\u9252|\u9254|\u9255|\u9257|\u9258|\u9259|\u925A|\u925B|\u925C|\u925D|\u925E|\u925F|\u9260|\u9261|\u9264|\u9265|\u9266|\u9267|\u9268|\u926C|\u926D|\u926E|\u9272|\u9275|\u9276|\u9277|\u9278|\u9279|\u927A|\u927B|\u927C|\u927D|\u927E|\u927F|\u9280|\u9281|\u9282|\u9283|\u9285|\u9288|\u928A|\u928B|\u928D|\u928F|\u9291|\u9293|\u9294|\u9296|\u9297|\u9298|\u9299|\u929A|\u929B|\u929C|\u92A0|\u92A1|\u92A3|\u92A5|\u92A6|\u92A7|\u92A8|\u92A9|\u92AA|\u92AB|\u92AC|\u92B1|\u92B2|\u92B3|\u92B6|\u92B7|\u92B8|\u92B9|\u92BB|\u92BC|\u92BE|\u92C1|\u92C2|\u92C3|\u92C5|\u92C7|\u92C9|\u92CA|\u92CB|\u92CC|\u92CD|\u92CF|\u92D0|\u92D2|\u92D7|\u92D8|\u92D9|\u92DC|\u92DD|\u92DF|\u92E0|\u92E1|\u92E3|\u92E4|\u92E5|\u92E6|\u92E7|\u92E8|\u92E9|\u92EA|\u92EE|\u92EF|\u92F0|\u92F1|\u92F6|\u92F8|\u92F9|\u92FC|\u92FE|\u9300|\u9301|\u9302|\u9304|\u9306|\u9307|\u9308|\u930B|\u930D|\u930F|\u9310|\u9311|\u9312|\u9314|\u9315|\u9317|\u9318|\u9319|\u931A|\u931B|\u931C|\u931D|\u931E|\u931F|\u9320|\u9321|\u9322|\u9323|\u9324|\u9325|\u9326|\u9327|\u9328|\u9329|\u932A|\u932B|\u932D|\u932E|\u932F|\u9333|\u9336|\u9338|\u933D|\u9340|\u9341|\u9342|\u9343|\u9344|\u9346|\u9347|\u9348|\u9349|\u934A|\u934B|\u934D|\u934F|\u9350|\u9351|\u9352|\u9354|\u9356|\u9358|\u935A|\u935B|\u935C|\u935D|\u935F|\u9360|\u9361|\u9363|\u9364|\u9365|\u9366|\u9367|\u9368|\u9369|\u936C|\u936D|\u936E|\u936F|\u9370|\u9371|\u9374|\u9375|\u9376|\u937A|\u937C|\u937E|\u9382|\u9384|\u9385|\u9387|\u9388|\u9389|\u938A|\u938B|\u938C|\u938D|\u9391|\u9392|\u9393|\u9394|\u9395|\u9396|\u9397|\u9398|\u9399|\u939A|\u939B|\u939D|\u939E|\u93A1|\u93A2|\u93A3|\u93A6|\u93A7|\u93A9|\u93AA|\u93AC|\u93AE|\u93AF|\u93B0|\u93B2|\u93B3|\u93B5|\u93B6|\u93B7|\u93BF|\u93C1|\u93C2|\u93C3|\u93C6|\u93C7|\u93C8|\u93C9|\u93CC|\u93CD|\u93CF|\u93D0|\u93D1|\u93D2|\u93D3|\u93D4|\u93D5|\u93D7|\u93D8|\u93D9|\u93DA|\u93DC|\u93DD|\u93DE|\u93DF|\u93E1|\u93E2|\u93E4|\u93E5|\u93E6|\u93E8|\u93E9|\u93F0|\u93F5|\u93F7|\u93F8|\u93F9|\u93FA|\u93FB|\u93FD|\u93FE|\u9400|\u9401|\u9403|\u9404|\u9407|\u9408|\u9409|\u940A|\u940B|\u940D|\u940E|\u940F|\u9410|\u9412|\u9413|\u9414|\u9415|\u9416|\u9418|\u9419|\u941A|\u941D|\u9420|\u9424|\u9425|\u9426|\u9427|\u9428|\u9429|\u942A|\u942B|\u942C|\u942E|\u942F|\u9432|\u9433|\u9434|\u9435|\u9436|\u9438|\u9439|\u943A|\u943C|\u943D|\u943F|\u9440|\u9444|\u9447|\u9448|\u9449|\u944A|\u944B|\u944C|\u944F|\u9450|\u9451|\u9452|\u9454|\u9455|\u9456|\u9458|\u9459|\u945B|\u945E|\u9460|\u9461|\u9462|\u9463|\u9465|\u9468|\u946A|\u946D|\u946E|\u946F|\u9470|\u9471|\u9472|\u9474|\u9477|\u9478|\u9479|\u947C|\u947D|\u947E|\u947F|\u9480|\u9481|\u9483|\u9577|\u9580|\u9582|\u9583|\u9584|\u9585|\u9586|\u9588|\u9589|\u958B|\u958C|\u958D|\u958E|\u958F|\u9590|\u9591|\u9592|\u9593|\u9594|\u9595|\u9597|\u9598|\u959B|\u959C|\u959D|\u959E|\u959F|\u95A1|\u95A3|\u95A4|\u95A5|\u95A6|\u95A7|\u95A8|\u95A9|\u95AB|\u95AC|\u95AD|\u95AF|\u95B1|\u95B5|\u95B6|\u95B7|\u95B9|\u95BB|\u95BC|\u95BD|\u95BE|\u95BF|\u95C3|\u95C4|\u95C6|\u95C7|\u95C8|\u95C9|\u95CA|\u95CB|\u95CC|\u95CD|\u95D0|\u95D1|\u95D2|\u95D3|\u95D4|\u95D5|\u95D6|\u95DA|\u95DB|\u95DC|\u95DE|\u95DF|\u95E0|\u95E1|\u95E2|\u95E4|\u95E5|\u962A|\u9658|\u965D|\u9663|\u9670|\u9673|\u9678|\u967D|\u967F|\u9689|\u968A|\u968E|\u9691|\u9695|\u9696|\u969B|\u96A4|\u96A8|\u96AA|\u96AB|\u96AE|\u96AF|\u96B1|\u96B2|\u96B4|\u96B8|\u96BB|\u96CB|\u96D6|\u96D9|\u96DB|\u96DC|\u96DE|\u96E2|\u96E3|\u96F2|\u96FB|\u9722|\u9723|\u9727|\u973C|\u973D|\u9742|\u9744|\u9745|\u9746|\u9748|\u9749|\u975A|\u975C|\u9766|\u9767|\u9768|\u9780|\u978F|\u979D|\u97B8|\u97BB|\u97BC|\u97BD|\u97BE|\u97C1|\u97C3|\u97C6|\u97C7|\u97C9|\u97CA|\u97CB|\u97CC|\u97CD|\u97CF|\u97D0|\u97D2|\u97D3|\u97D4|\u97D7|\u97D8|\u97D9|\u97DA|\u97DB|\u97DC|\u97DD|\u97DE|\u97E0|\u97E1|\u97E2|\u97E3|\u97FB|\u97FF|\u9801|\u9802|\u9803|\u9804|\u9805|\u9806|\u9807|\u9808|\u980A|\u980C|\u980D|\u980E|\u980F|\u9810|\u9811|\u9812|\u9813|\u9814|\u9815|\u9816|\u9817|\u9818|\u981B|\u981C|\u981E|\u981F|\u9820|\u9821|\u9822|\u9824|\u9826|\u9829|\u982A|\u982B|\u982D|\u982E|\u982F|\u9830|\u9832|\u9834|\u9835|\u9837|\u9838|\u9839|\u983B|\u9840|\u9841|\u9843|\u9844|\u9845|\u9846|\u9847|\u9849|\u984A|\u984B|\u984C|\u984D|\u984E|\u984F|\u9850|\u9851|\u9852|\u9853|\u9856|\u9857|\u9858|\u9859|\u985B|\u985C|\u985D|\u985E|\u9860|\u9862|\u9863|\u9864|\u9865|\u9866|\u9867|\u9869|\u986A|\u986B|\u986C|\u986E|\u986F|\u9870|\u9871|\u9873|\u9874|\u98A8|\u98A9|\u98AC|\u98AD|\u98AE|\u98AF|\u98B0|\u98B1|\u98B2|\u98B3|\u98B4|\u98B6|\u98B7|\u98B8|\u98B9|\u98BA|\u98BB|\u98BC|\u98BD|\u98BE|\u98BF|\u98C0|\u98C1|\u98C2|\u98C4|\u98C6|\u98C7|\u98C8|\u98C9|\u98CB|\u98CD|\u98DB|\u98E0|\u98E2|\u98E3|\u98E4|\u98E5|\u98E6|\u98E9|\u98EA|\u98EB|\u98ED|\u98EF|\u98F0|\u98F2|\u98F4|\u98F5|\u98F6|\u98F7|\u98FC|\u98FD|\u98FE|\u98FF|\u9900|\u9902|\u9903|\u9904|\u9905|\u9909|\u990A|\u990C|\u990E|\u990F|\u9911|\u9912|\u9913|\u9914|\u9915|\u9916|\u9917|\u9918|\u991A|\u991B|\u991C|\u991E|\u991F|\u9921|\u9922|\u9923|\u9924|\u9926|\u9927|\u9928|\u9929|\u992A|\u992B|\u992C|\u992D|\u992F|\u9930|\u9931|\u9932|\u9933|\u9934|\u9935|\u9936|\u9937|\u9938|\u9939|\u993A|\u993C|\u993E|\u993F|\u9940|\u9941|\u9943|\u9945|\u9946|\u9947|\u9948|\u9949|\u994A|\u994B|\u994C|\u994E|\u9950|\u9952|\u9957|\u9958|\u9959|\u995B|\u995C|\u995E|\u995F|\u9960|\u9961|\u9962|\u99A9|\u99AC|\u99AD|\u99AE|\u99AF|\u99B1|\u99B2|\u99B3|\u99B4|\u99B5|\u99B9|\u99BA|\u99BC|\u99BD|\u99C1|\u99C2|\u99C3|\u99C9|\u99CA|\u99CD|\u99CE|\u99CF|\u99D0|\u99D1|\u99D2|\u99D3|\u99D4|\u99D5|\u99D7|\u99D8|\u99D9|\u99DA|\u99DB|\u99DC|\u99DD|\u99DE|\u99DF|\u99E2|\u99E3|\u99E4|\u99E5|\u99E7|\u99E9|\u99EA|\u99EB|\u99EC|\u99ED|\u99EE|\u99F0|\u99F1|\u99F4|\u99F6|\u99F7|\u99F8|\u99F9|\u99FA|\u99FB|\u99FC|\u99FD|\u99FE|\u99FF|\u9A00|\u9A01|\u9A02|\u9A03|\u9A04|\u9A05|\u9A07|\u9A09|\u9A0A|\u9A0B|\u9A0C|\u9A0D|\u9A0E|\u9A0F|\u9A11|\u9A14|\u9A15|\u9A16|\u9A17|\u9A19|\u9A1A|\u9A1C|\u9A1D|\u9A1E|\u9A1F|\u9A20|\u9A22|\u9A23|\u9A24|\u9A25|\u9A27|\u9A29|\u9A2A|\u9A2B|\u9A2C|\u9A2D|\u9A2E|\u9A2F|\u9A30|\u9A31|\u9A32|\u9A33|\u9A34|\u9A35|\u9A36|\u9A37|\u9A38|\u9A39|\u9A3A|\u9A3B|\u9A3C|\u9A3D|\u9A3E|\u9A40|\u9A41|\u9A42|\u9A43|\u9A44|\u9A45|\u9A48|\u9A49|\u9A4A|\u9A4B|\u9A4C|\u9A4D|\u9A4E|\u9A4F|\u9A50|\u9A52|\u9A53|\u9A54|\u9A55|\u9A56|\u9A57|\u9A59|\u9A5A|\u9A5B|\u9A5E|\u9A5F|\u9A60|\u9A61|\u9A62|\u9A64|\u9A65|\u9A66|\u9A68|\u9A69|\u9A6A|\u9A6B|\u9AAF|\u9ACF|\u9AD0|\u9AD2|\u9AD4|\u9AD5|\u9AD6|\u9AEE|\u9B06|\u9B0D|\u9B16|\u9B17|\u9B1A|\u9B1C|\u9B1D|\u9B1E|\u9B20|\u9B21|\u9B22|\u9B25|\u9B27|\u9B29|\u9B2E|\u9B31|\u9B39|\u9B3A|\u9B4E|\u9B57|\u9B58|\u9B5A|\u9B5B|\u9B5C|\u9B5D|\u9B5F|\u9B60|\u9B61|\u9B62|\u9B63|\u9B65|\u9B66|\u9B67|\u9B68|\u9B6A|\u9B6B|\u9B6C|\u9B6D|\u9B6E|\u9B6F|\u9B71|\u9B74|\u9B75|\u9B76|\u9B77|\u9B7A|\u9B7B|\u9B7C|\u9B7D|\u9B7E|\u9B80|\u9B81|\u9B82|\u9B83|\u9B84|\u9B85|\u9B86|\u9B87|\u9B88|\u9B8A|\u9B8B|\u9B8C|\u9B8D|\u9B8E|\u9B8F|\u9B90|\u9B91|\u9B92|\u9B93|\u9B98|\u9B9A|\u9B9B|\u9B9C|\u9B9E|\u9B9F|\u9BA0|\u9BA1|\u9BA3|\u9BA4|\u9BA5|\u9BA6|\u9BA7|\u9BA8|\u9BAA|\u9BAB|\u9BAC|\u9BAD|\u9BAE|\u9BAF|\u9BB0|\u9BB3|\u9BB5|\u9BB6|\u9BB7|\u9BB8|\u9BB9|\u9BBA|\u9BBB|\u9BBF|\u9BC0|\u9BC1|\u9BC4|\u9BC5|\u9BC6|\u9BC7|\u9BC8|\u9BC9|\u9BCA|\u9BCC|\u9BD2|\u9BD4|\u9BD5|\u9BD6|\u9BD7|\u9BDA|\u9BDB|\u9BDD|\u9BDE|\u9BE0|\u9BE1|\u9BE2|\u9BE4|\u9BE5|\u9BE6|\u9BE7|\u9BE8|\u9BE9|\u9BEA|\u9BEB|\u9BEC|\u9BEE|\u9BF0|\u9BF1|\u9BF4|\u9BF6|\u9BF7|\u9BF8|\u9BF9|\u9BFB|\u9BFC|\u9BFD|\u9BFE|\u9BFF|\u9C01|\u9C02|\u9C03|\u9C05|\u9C06|\u9C07|\u9C08|\u9C09|\u9C0A|\u9C0B|\u9C0C|\u9C0D|\u9C0F|\u9C10|\u9C11|\u9C12|\u9C13|\u9C15|\u9C17|\u9C1C|\u9C1D|\u9C1F|\u9C20|\u9C21|\u9C23|\u9C24|\u9C25|\u9C26|\u9C27|\u9C28|\u9C29|\u9C2B|\u9C2C|\u9C2D|\u9C2E|\u9C2F|\u9C31|\u9C32|\u9C33|\u9C34|\u9C35|\u9C36|\u9C37|\u9C39|\u9C3A|\u9C3B|\u9C3C|\u9C3D|\u9C3E|\u9C3F|\u9C40|\u9C41|\u9C42|\u9C43|\u9C44|\u9C45|\u9C46|\u9C47|\u9C48|\u9C49|\u9C4A|\u9C4B|\u9C4C|\u9C4D|\u9C4E|\u9C4F|\u9C50|\u9C51|\u9C52|\u9C53|\u9C54|\u9C55|\u9C56|\u9C57|\u9C58|\u9C5A|\u9C5D|\u9C5E|\u9C5F|\u9C60|\u9C62|\u9C63|\u9C64|\u9C65|\u9C66|\u9C67|\u9C68|\u9C6C|\u9C6D|\u9C6E|\u9C6F|\u9C72|\u9C74|\u9C75|\u9C77|\u9C78|\u9C79|\u9C7A|\u9C7B|\u9CE5|\u9CE6|\u9CE7|\u9CE9|\u9CED|\u9CF1|\u9CF2|\u9CF3|\u9CF4|\u9CF6|\u9CF7|\u9CF8|\u9CFA|\u9CFB|\u9CFC|\u9CFD|\u9CFE|\u9CFF|\u9D00|\u9D01|\u9D02|\u9D03|\u9D05|\u9D06|\u9D07|\u9D09|\u9D0D|\u9D10|\u9D12|\u9D13|\u9D14|\u9D15|\u9D17|\u9D18|\u9D19|\u9D1A|\u9D1B|\u9D1C|\u9D1D|\u9D1E|\u9D1F|\u9D20|\u9D21|\u9D22|\u9D23|\u9D25|\u9D26|\u9D28|\u9D29|\u9D2E|\u9D2F|\u9D30|\u9D31|\u9D32|\u9D33|\u9D34|\u9D36|\u9D37|\u9D38|\u9D39|\u9D3A|\u9D3B|\u9D3D|\u9D3E|\u9D3F|\u9D40|\u9D41|\u9D42|\u9D43|\u9D44|\u9D45|\u9D4A|\u9D4B|\u9D4C|\u9D4E|\u9D4F|\u9D50|\u9D51|\u9D52|\u9D53|\u9D54|\u9D55|\u9D56|\u9D57|\u9D59|\u9D5A|\u9D5B|\u9D5C|\u9D5D|\u9D5F|\u9D60|\u9D61|\u9D67|\u9D69|\u9D6A|\u9D6B|\u9D6C|\u9D6E|\u9D6F|\u9D70|\u9D71|\u9D72|\u9D73|\u9D74|\u9D75|\u9D76|\u9D77|\u9D78|\u9D79|\u9D7B|\u9D7C|\u9D7D|\u9D7E|\u9D80|\u9D82|\u9D83|\u9D84|\u9D85|\u9D86|\u9D87|\u9D89|\u9D8A|\u9D8B|\u9D8C|\u9D92|\u9D93|\u9D94|\u9D95|\u9D96|\u9D97|\u9D98|\u9D99|\u9D9A|\u9D9B|\u9D9D|\u9D9E|\u9D9F|\u9DA0|\u9DA1|\u9DA2|\u9DA3|\u9DA4|\u9DA5|\u9DA6|\u9DA8|\u9DA9|\u9DAA|\u9DAC|\u9DAD|\u9DAF|\u9DB0|\u9DB1|\u9DB2|\u9DB4|\u9DB5|\u9DB6|\u9DB7|\u9DB9|\u9DBA|\u9DBB|\u9DBC|\u9DBD|\u9DC0|\u9DC1|\u9DC2|\u9DC3|\u9DC5|\u9DC7|\u9DC8|\u9DC9|\u9DCA|\u9DCB|\u9DCE|\u9DCF|\u9DD0|\u9DD1|\u9DD2|\u9DD3|\u9DD4|\u9DD5|\u9DD6|\u9DD7|\u9DD9|\u9DDA|\u9DDB|\u9DDC|\u9DDE|\u9DDF|\u9DE2|\u9DE3|\u9DE4|\u9DE5|\u9DE6|\u9DE7|\u9DE8|\u9DE9|\u9DEB|\u9DED|\u9DEE|\u9DEF|\u9DF0|\u9DF2|\u9DF3|\u9DF5|\u9DF6|\u9DF7|\u9DF8|\u9DF9|\u9DFA|\u9DFD|\u9DFE|\u9DFF|\u9E00|\u9E01|\u9E02|\u9E03|\u9E04|\u9E05|\u9E06|\u9E07|\u9E09|\u9E0A|\u9E0B|\u9E0C|\u9E0E|\u9E0F|\u9E10|\u9E11|\u9E12|\u9E13|\u9E15|\u9E16|\u9E17|\u9E18|\u9E19|\u9E1A|\u9E1B|\u9E1C|\u9E1D|\u9E1E|\u9E75|\u9E79|\u9E7A|\u9E7C|\u9E7D|\u9E97|\u9EA1|\u9EA5|\u9EA7|\u9EA8|\u9EA9|\u9EAC|\u9EAE|\u9EAF|\u9EB0|\u9EB1|\u9EB2|\u9EB3|\u9EB4|\u9EB5|\u9EB7|\u9EBC|\u9EBD|\u9EC2|\u9EC3|\u9ECC|\u9EDE|\u9EE8|\u9EF2|\u9EF6|\u9EF7|\u9EF8|\u9EFD|\u9EFF|\u9F00|\u9F01|\u9F04|\u9F05|\u9F06|\u9F08|\u9F09|\u9F0A|\u9F1A|\u9F32|\u9F34|\u9F48|\u9F4A|\u9F4B|\u9F4C|\u9F4D|\u9F4E|\u9F4F|\u9F52|\u9F54|\u9F55|\u9F56|\u9F57|\u9F58|\u9F59|\u9F5A|\u9F5C|\u9F5D|\u9F5E|\u9F5F|\u9F60|\u9F61|\u9F63|\u9F64|\u9F65|\u9F66|\u9F67|\u9F69|\u9F6A|\u9F6C|\u9F6D|\u9F6E|\u9F6F|\u9F70|\u9F71|\u9F72|\u9F73|\u9F74|\u9F75|\u9F76|\u9F77|\u9F78|\u9F79|\u9F7A|\u9F7B|\u9F7C|\u9F7D|\u9F7E|\u9F8D|\u9F8E|\u9F8F|\u9F90|\u9F91|\u9F93|\u9F94|\u9F95|\u9F96|\u9F9C|\u9F9D|\u9F9E|\u9FA5|\u9FAD|\u9FAF|\u9FB2|\u9FBD|\u9FC1|\u9FD0|\u9FD2|\u20054|\u2005E|\u20325|\u20385|\u20392|\u203E2|\u203EE|\u20407|\u2040A|\u2040D|\u2042E|\u2043D|\u20447|\u20459|\u20472|\u205AB|\u205FF|\u20625|\u20732|\u2077F|\u20786|\u207AD|\u207EA|\u2080E|\u2080F|\u2081D|\u2082B|\u20A58|\u20A6C|\u20B19|\u20D54|\u20D58|\u20D79|\u20DB8|\u20DB9|\u20DCC|\u20DCF|\u20E5B|\u20E96|\u20EAE|\u20F17|\u20F24|\u20F2E|\u20F48|\u20F78|\u20FAC|\u20FD5|\u20FD8|\u20FFF|\u21020|\u2103F|\u2105A|\u2106F|\u21092|\u210A1|\u210BF|\u210C4|\u210C8|\u210E4|\u21114|\u21116|\u21123|\u21124|\u21129|\u2114F|\u21158|\u21165|\u21167|\u2136B|\u2144D|\u2144E|\u2146D|\u2146F|\u214B6|\u214C1|\u214D7|\u214E6|\u214FE|\u215C6|\u217B5|\u217EB|\u2181A|\u21839|\u21883|\u21898|\u218BF|\u218E8|\u21920|\u21921|\u2192B|\u21B89|\u21BA3|\u21BA4|\u21CF3|\u21DE8|\u21E17|\u21E6C|\u21EA0|\u21EA8|\u21F31|\u21F3E|\u21F57|\u21F73|\u21F75|\u21F86|\u21FB1|\u21FD6|\u22113|\u2213C|\u22161|\u22163|\u2227F|\u22283|\u22370|\u22417|\u22569|\u22595|\u226D4|\u2272D|\u22830|\u2283C|\u22880|\u228CF|\u228D0|\u228DA|\u228ED|\u2290C|\u2291C|\u22927|\u22929|\u22931|\u2293F|\u22960|\u22BE6|\u22BE9|\u22BF7|\u22C61|\u22C90|\u22CA9|\u22CAB|\u22CB8|\u22CBE|\u22CC2|\u22CDA|\u22D26|\u22D29|\u22D63|\u22D91|\u22D92|\u22DAB|\u22DC3|\u22DCF|\u22DDE|\u22DEE|\u22E01|\u22E14|\u22E19|\u22E33|\u22E34|\u22E38|\u22E4F|\u22E65|\u22E7C|\u22E7F|\u22E8E|\u22EB3|\u22FD3|\u22FE1|\u23018|\u23037|\u2303B|\u23138|\u23236|\u232AF|\u232CB|\u232DE|\u23302|\u23350|\u23384|\u2339C|\u2353F|\u2364E|\u2367F|\u23699|\u236E3|\u23755|\u23781|\u23790|\u237BB|\u23815|\u23829|\u23832|\u2384C|\u23876|\u2390B|\u2393F|\u23A55|\u23AD2|\u23BE9|\u23BF4|\u23BF6|\u23C1B|\u23C28|\u23D07|\u23DAF|\u23ECF|\u23ED1|\u23F0A|\u23F29|\u23F4F|\u23FB7|\u23FC9|\u2402A|\u24063|\u2406A|\u24119|\u24137|\u24159|\u24169|\u24177|\u24356|\u2435C|\u243A4|\u243B1|\u243D0|\u24473|\u24479|\u2448E|\u244A6|\u244BB|\u244CC|\u244CE|\u244D3|\u24600|\u246EE|\u246F1|\u24706|\u247E4|\u24814|\u2482E|\u24872|\u2489F|\u248CE|\u248E4|\u2496D|\u24A42|\u24ABA|\u24AE9|\u24B05|\u24BA6|\u24C93|\u24CA2|\u24CF7|\u24CF8|\u24DC3|\u24DFD|\u24E2B|\u24E89|\u24E94|\u24EDC|\u24EDD|\u24EF2|\u24F08|\u24F89|\u2502C|\u25032|\u250AB|\u250B8|\u251D4|\u25278|\u252DD|\u25303|\u2531A|\u253DD|\u25502|\u25565|\u25585|\u2558F|\u255A9|\u255B2|\u255C7|\u255F4|\u255F9|\u255FA|\u255FD|\u25603|\u25710|\u25730|\u257B5|\u2588A|\u258A2|\u258B6|\u258B7|\u25A10|\u25A82|\u25BE4|\u25C78|\u25CCA|\u25D28|\u25D3C|\u25D43|\u25D4A|\u25D5B|\u25D5C|\u25D5D|\u25E20|\u25EBC|\u25EE4|\u25EE6|\u25EF5|\u25F36|\u25F3D|\u25F56|\u25F6D|\u25F7D|\u25F82|\u25F9D|\u25FAF|\u25FC9|\u25FCA|\u25FEF|\u2600E|\u26016|\u26044|\u26055|\u26067|\u26085|\u2608B|\u260C4|\u260D2|\u260D8|\u260E9|\u2610B|\u2610D|\u26127|\u2613C|\u26147|\u26148|\u2614B|\u26158|\u26177|\u26186|\u26188|\u261B2|\u261CE|\u261DB|\u2633E|\u26346|\u263B9|\u263D1|\u26480|\u26516|\u26627|\u26716|\u2679B|\u267D0|\u267FC|\u26805|\u2684F|\u26856|\u2685D|\u26867|\u26876|\u26888|\u268C7|\u268CE|\u269F4|\u269FA|\u26AAD|\u26ABD|\u26C4C|\u26CDD|\u26D55|\u26D86|\u26E37|\u26EA3|\u26F52|\u26F8F|\u26FB5|\u26FB6|\u26FCD|\u2707F|\u27085|\u270FD|\u27355|\u273FB|\u27410|\u27431|\u27496|\u274AF|\u27525|\u2755F|\u27566|\u275A6|\u276F8|\u27701|\u27702|\u27717|\u27723|\u27735|\u27736|\u2775E|\u27785|\u27794|\u277A3|\u277AB|\u277B6|\u277CC|\u27808|\u27825|\u27835|\u2784D|\u2786A|\u27874|\u27878|\u27883|\u27884|\u2788D|\u278A2|\u278F4|\u27963|\u2797A|\u2799D|\u279A6|\u279A7|\u279AD|\u279DD|\u279ED|\u279F5|\u279F8|\u27A0A|\u27A1D|\u27A33|\u27A3E|\u27A55|\u27A59|\u27A66|\u27A67|\u27A6A|\u27A7C|\u27A9E|\u27AA1|\u27AA6|\u27AAA|\u27AAE|\u27ADA|\u27ADD|\u27B01|\u27B05|\u27B07|\u27B0C|\u27B24|\u27B28|\u27B2A|\u27B2E|\u27B2F|\u27B3B|\u27B48|\u27B79|\u27B86|\u27B87|\u27B88|\u27B93|\u27C06|\u27C7B|\u27CDF|\u27D2A|\u27D4A|\u27D73|\u27D84|\u27D94|\u27D9F|\u27DA7|\u27DB2|\u27DCE|\u27DDB|\u27E16|\u27E18|\u27E26|\u27E2A|\u27E2B|\u27E48|\u27F62|\u27F6F|\u27F75|\u27FA5|\u28042|\u28090|\u280D8|\u280DC|\u28109|\u28123|\u28130|\u2814D|\u28185|\u28189|\u281AA|\u281B1|\u281C1|\u281CD|\u281D7|\u281DE|\u281E4|\u281EF|\u281F0|\u281FD|\u28200|\u28206|\u28207|\u2820A|\u2820C|\u28256|\u28279|\u282A0|\u282B0|\u282B8|\u282B9|\u282BB|\u282C1|\u282DA|\u282E2|\u282EE|\u28304|\u28308|\u28348|\u2834F|\u28350|\u28352|\u28370|\u28379|\u2838C|\u283A9|\u283AA|\u283AE|\u283D2|\u283D4|\u283E0|\u283E5|\u28436|\u2844A|\u2860C|\u287A8|\u287BA|\u287CA|\u288BF|\u288C3|\u288C8|\u288C9|\u288DE|\u288E7|\u288E8|\u2890B|\u28921|\u2893B|\u2895B|\u2895C|\u2895F|\u28966|\u2897A|\u289A1|\u289AB|\u289C0|\u289D0|\u289DA|\u289DC|\u289EB|\u289F0|\u289F1|\u28A0F|\u28A1B|\u28A1D|\u28A22|\u28A2F|\u28A39|\u28A68|\u28A70|\u28A85|\u28A8B|\u28A95|\u28AC0|\u28AD2|\u28AFC|\u28B02|\u28B12|\u28B16|\u28B1E|\u28B1F|\u28B43|\u28B46|\u28B4C|\u28B4E|\u28B50|\u28B56|\u28B57|\u28B5A|\u28B5B|\u28B65|\u28B78|\u28B81|\u28B82|\u28B85|\u28BB0|\u28BB3|\u28BC5|\u28BDF|\u28BF5|\u28C03|\u28C0B|\u28C20|\u28C25|\u28C2D|\u28C32|\u28C35|\u28C37|\u28C39|\u28C65|\u28CAD|\u28CB3|\u28CCC|\u28CD0|\u28CD1|\u28CD2|\u28CD5|\u28CD9|\u28CDA|\u28CE8|\u28CF8|\u28CFF|\u28D11|\u28D17|\u28D24|\u28D39|\u28D46|\u28D4C|\u28D57|\u28D64|\u28D66|\u28D69|\u28D6C|\u28D78|\u28D80|\u28D8F|\u28D91|\u28DAE|\u28DAF|\u28DB0|\u28DB2|\u28DBB|\u28DBF|\u28DC8|\u28DF2|\u28DFB|\u28F33|\u28F48|\u28F4F|\u29028|\u29159|\u29166|\u2917E|\u291C9|\u2924D|\u29259|\u292CC|\u292F0|\u2935C|\u29392|\u29395|\u29396|\u2939F|\u293A0|\u293A2|\u293C2|\u293CC|\u293E0|\u293EA|\u293F4|\u293F7|\u2940C|\u29443|\u29452|\u29454|\u29461|\u29463|\u29466|\u2948E|\u2949C|\u2949D|\u294B2|\u294BA|\u294BC|\u294E3|\u294E5|\u294F8|\u294F9|\u29507|\u29508|\u2950A|\u29511|\u29523|\u29533|\u2954A|\u29570|\u29581|\u295B0|\u295BF|\u295C0|\u295D3|\u295DB|\u295E1|\u295F4|\u29600|\u2961A|\u2961D|\u29639|\u2963A|\u2963B|\u29648|\u29685|\u2969A|\u2969B|\u296A5|\u296A9|\u296B5|\u296C6|\u296CC|\u296CE|\u296DE|\u296E1|\u296E9|\u296F2|\u29707|\u29720|\u29726|\u2972F|\u29730|\u29735|\u29736|\u29751|\u29754|\u29760|\u29761|\u29763|\u29767|\u2977D|\u29783|\u29784|\u29786|\u29789|\u297A1|\u297A6|\u297A7|\u297AC|\u297AF|\u297C0|\u297C2|\u297D0|\u297D7|\u297E0|\u29834|\u29863|\u29864|\u2987A|\u2988D|\u298A1|\u298B0|\u298B2|\u298B4|\u298B8|\u298BC|\u298BE|\u298CA|\u298CB|\u298CF|\u298D1|\u298D4|\u298E1|\u298EB|\u298F5|\u298FA|\u2990A|\u29919|\u29932|\u29935|\u29938|\u29943|\u29944|\u29945|\u29947|\u29949|\u2994E|\u29951|\u29972|\u2997C|\u29983|\u2999A|\u299A0|\u299BA|\u299C6|\u299C9|\u299D0|\u299E2|\u29B59|\u29B6F|\u29BC1|\u29BC3|\u29BC6|\u29BF3|\u29C00|\u29C39|\u29C48|\u29CE4|\u29D06|\u29D35|\u29D5A|\u29D66|\u29D69|\u29D71|\u29D79|\u29D7A|\u29D80|\u29D81|\u29D98|\u29DAF|\u29DB0|\u29DB1|\u29DD2|\u29DF0|\u29DF6|\u29E03|\u29E04|\u29E06|\u29E21|\u29E23|\u29E24|\u29E26|\u29E29|\u29E2C|\u29E42|\u29E4A|\u29E5D|\u29E7D|\u29E7E|\u29E9D|\u29E9E|\u29ED7|\u29EDB|\u29EE7|\u29EEC|\u29EEE|\u29EF0|\u29EF1|\u29F14|\u29F36|\u29F45|\u29F47|\u29F48|\u29F54|\u29F77|\u29F90|\u29F92|\u29F9D|\u29FC5|\u29FCA|\u29FE4|\u29FE7|\u29FEA|\u29FF1|\u29FFA|\u2A009|\u2A016|\u2A017|\u2A01A|\u2A01B|\u2A026|\u2A03B|\u2A03E|\u2A048|\u2A04F|\u2A050|\u2A051|\u2A056|\u2A05B|\u2A05C|\u2A071|\u2A07F|\u2A086|\u2A088|\u2A0A9|\u2A0AB|\u2A0C3|\u2A0CD|\u2A0CF|\u2A0D2|\u2A0E6|\u2A0E7|\u2A0EE|\u2A0FF|\u2A105|\u2A106|\u2A115|\u2A120|\u2A132|\u2A142|\u2A143|\u2A156|\u2A15C|\u2A17E|\u2A183|\u2A1AB|\u2A1B0|\u2A1B4|\u2A1B7|\u2A1C4|\u2A1D6|\u2A1D8|\u2A1F0|\u2A1F3|\u2A20F|\u2A214|\u2A217|\u2A23C|\u2A256|\u2A25C|\u2A263|\u2A268|\u2A26E|\u2A271|\u2A278|\u2A27F|\u2A289|\u2A2C8|\u2A2FC|\u2A2FD|\u2A2FF|\u2A310|\u2A312|\u2A317|\u2A318|\u2A31C|\u2A323|\u2A328|\u2A32C|\u2A32D|\u2A32E|\u2A32F|\u2A330|\u2A33D|\u2A33E|\u2A33F|\u2A340|\u2A347|\u2A34D|\u2A351|\u2A352|\u2A353|\u2A358|\u2A35A|\u2A35E|\u2A360|\u2A363|\u2A364|\u2A36C|\u2A374|\u2A376|\u2A377|\u2A37F|\u2A382|\u2A45A|\u2A473|\u2A4AC|\u2A4BF|\u2A4DB|\u2A4EC|\u2A4F0|\u2A4F9|\u2A4FD|\u2A535|\u2A563|\u2A5A8|\u2A5CB|\u2A5DC|\u2A5DD|\u2A5EA|\u2A5ED|\u2A5F3|\u2A5FB|\u2A5FD|\u2A600|\u2A605|\u2A613|\u2A61E|\u2A625|\u2A627|\u2A628|\u2A629|\u2A62C|\u2A62F|\u2A632|\u2A649|\u2A64D|\u2A64F|\u2A651|\u2A655|\u2A65E|\u2A664|\u2A685|\u2A694|\u2A6A3|\u2A6AD|\u2A6AE|\u2A6B0|\u2A6D5|\u2A756|\u2A775|\u2A7D6|\u2A88D|\u2A8A5|\u2ABB0|\u2ABC2|\u2ACF7|\u2AD25|\u2AD62|\u2ADC8|\u2B0D0|\u2B0D1|\u2B0DE|\u2B0E5|\u2B0F7|\u2B107|\u2B1E0|\u2B239|\u2B24D|\u2B2D0|\u2B2E7|\u2B319|\u2B358|\u2B49E|\u2B4A1|\u2B4A2|\u2B4B7|\u2B518|\u2B521|\u2B59E|\u2B5D1|\u2B5D5|\u2B5FB|\u2B726|\u2B75C|\u2B8F4|\u2B95D|\u2B994|\u2B999|\u2B9B8|\u2B9DD|\u2BA11|\u2BA9B|\u2BB06|\u2BB31|\u2BBD3|\u2BCB4|\u2BDA6|\u2BED1|\u2BFA1|\u2C11D|\u2C189|\u2C264|\u2C326|\u2C341|\u2C3F2|\u2C461|\u2C492|\u2C4E1|\u2C5CF|\u2C5FA|\u2C654|\u2C6D5|\u2C810|\u2C8CD|\u2C8D8|\u2C972|\u2C9D9|\u2CB87|\u2CB8D|\u2CBD8|\u2CC42|\u2CC48|\u2CC9A|\u2CC9B|\u2CD42|\u2CD43|\u2CD6E|\u2CDBC|\u2CE42|\u2D096|\u2D27E|\u2D459|\u2D5E1|\u2D892|\u2D9D2|\u2D9D6|\u2DA21|\u2DC58|\u2DD99|\u2E717|\u2E7FD|\u2E848|\u2E90F|\u2E912|\u2E997|\u2EA2D|\u2EA3B|\u300A0|\u300B4|\u300F4|\u3021D|\u30240|\u302C6|\u303BC|\u30520|\u3052B|\u3053A|\u305BB|\u3062F|\u30682|\u306A3|\u30762|\u307EB|\u30853|\u30AC6|\u30ACF|\u30ADB|\u30AF3|\u30D0F|\u30D26|\u30D3D|\u30E48|\u30EDE|\u30FE2|\u310E1|\u310E2|\u310EA|\u311A5|\u311CB";

            Regex JapaneseKana = new Regex(strJapaneseKana);
            Regex Kanji = new Regex(strKanji);
            // ↓Pythonスクリプトで生成したやつをコピペ
            Regex SimplifiedChinese = new Regex(strSimplifiedChinese);
            Regex TraditionalChinese = new Regex(strTraditionalChinese);

            // ほんとはここでひらがなカタカナだけじゃなくて、日本語にしか使われてない漢字も含めたほうが精度あがる
            if (JapaneseKana.IsMatch(strSource))
            {
                return "ja";
            }
            //var SimplifiedChinese = "\u3437|\u3439|\u343D|\u3447|\u3448|\u3454|\u3469|\u347A|\u34E5|\u3509|\u358A|\u359E|\u360E|\u36AF|\u36C0|\u36DF|\u36E0|\u36E3|\u36E4|\u36FF|\u3766|\u37C6|\u37DC|\u37E5|\u384E|\u3916|\u3918|\u392D|\u393D|\u396A|\u3988|\u39CF|\u39D0|\u39D1|\u39DB|\u39DF|\u39F0|\u3A2B|\u3B4E|\u3B4F|\u3B63|\u3B64|\u3B74|\u3BA0|\u3C69|\u3C6E|\u3CBF|\u3CD4|\u3CD5|\u3CE0|\u3CE1|\u3CE2|\u3CFD|\u3D0B|\u3D89|\u3DB6|\u3DBD|\u3DEA|\u3E8D|\u3EC5|\u3ECF|\u3ED8|\u3EEA|\u3FA1|\u4025|\u4056|\u40B5|\u40C5|\u4149|\u415F|\u416A|\u41DA|\u41F2|\u4264|\u4336|\u4337|\u4338|\u4339|\u433A|\u433B|\u433C|\u433D|\u433E|\u433F|\u4340|\u4341|\u43AC|\u43DD|\u442A|\u44D3|\u44D5|\u45BC|\u45D6|\u461B|\u461E|\u464A|\u464C|\u4653|\u46D3|\u4723|\u4724|\u4725|\u4727|\u4729|\u4759|\u478C|\u478D|\u478E|\u4790|\u47E2|\u4880|\u4881|\u4882|\u497A|\u497D|\u497E|\u497F|\u4980|\u4981|\u4982|\u4983|\u4985|\u4986|\u49B6|\u49B7|\u4A44|\u4B6A|\u4BC3|\u4BC4|\u4BC5|\u4C9D|\u4C9E|\u4C9F|\u4CA0|\u4CA1|\u4CA2|\u4CA3|\u4CA4|\u4D13|\u4D14|\u4D15|\u4D16|\u4D17|\u4D18|\u4D19|\u4DAE|\u4E07|\u4E0E|\u4E11|\u4E13|\u4E1A|\u4E1B|\u4E1C|\u4E1D|\u4E22|\u4E24|\u4E25|\u4E27|\u4E2A|\u4E30|\u4E34|\u4E3A|\u4E3D|\u4E3E|\u4E48|\u4E49|\u4E4C|\u4E50|\u4E54|\u4E60|\u4E61|\u4E66|\u4E70|\u4E71|\u4E89|\u4E8E|\u4E8F|\u4E91|\u4E9A|\u4EA7|\u4EA9|\u4EB2|\u4EB5|\u4EB8|\u4EBF|\u4EC5|\u4EC6|\u4ECE|\u4ED1|\u4ED3|\u4EEA|\u4EEC|\u4EF7|\u4F17|\u4F18|\u4F1A|\u4F1B|\u4F1E|\u4F1F|\u4F20|\u4F21|\u4F23|\u4F24|\u4F25|\u4F26|\u4F27|\u4F2A|\u4F2B|\u4F53|\u4F59|\u4F63|\u4F65|\u4FA0|\u4FA3|\u4FA5|\u4FA6|\u4FA7|\u4FA8|\u4FA9|\u4FAA|\u4FAC|\u4FE3|\u4FE6|\u4FE8|\u4FE9|\u4FEA|\u4FEB|\u4FED|\u503A|\u503E|\u506C|\u507B|\u507E|\u507F|\u50A5|\u50A7|\u50A8|\u50A9|\u513F|\u514B|\u5151|\u5156|\u515A|\u5170|\u5173|\u5174|\u5179|\u517B|\u517D|\u5181|\u5185|\u5188|\u518C|\u5199|\u519B|\u519C|\u51AF|\u51B2|\u51B3|\u51B5|\u51BB|\u51C0|\u51C6|\u51C9|\u51CF|\u51D1|\u51DB|\u51E0|\u51E4|\u51EB|\u51ED|\u51EF|\u51FA|\u51FB|\u51FF|\u520D|\u5212|\u5218|\u5219|\u521A|\u521B|\u5220|\u522B|\u522C|\u522D|\u522E|\u5236|\u5239|\u523D|\u523E|\u523F|\u5240|\u5242|\u5250|\u5251|\u5265|\u5267|\u529D|\u529E|\u52A1|\u52A2|\u52A8|\u52B1|\u52B2|\u52B3|\u52BF|\u52CB|\u52DA|\u5300|\u5326|\u532E|\u533A|\u533B|\u534E|\u534F|\u5355|\u5356|\u5362|\u5364|\u536B|\u5374|\u5382|\u5385|\u5386|\u5389|\u538B|\u538C|\u538D|\u5390|\u5395|\u5398|\u53A2|\u53A3|\u53A6|\u53A8|\u53A9|\u53AE|\u53BF|\u53C1|\u53C2|\u53C6|\u53C7|\u53CC|\u53D1|\u53D8|\u53D9|\u53E0|\u53EA|\u53F0|\u53F6|\u53F7|\u53F9|\u53FD|\u540C|\u540E|\u5411|\u5413|\u5415|\u5417|\u5423|\u5428|\u542C|\u542F|\u5434|\u5450|\u5452|\u5453|\u5455|\u5456|\u5457|\u5458|\u5459|\u545B|\u545C|\u548F|\u5499|\u549B|\u549D|\u54A4|\u54B8|\u54CD|\u54D1|\u54D2|\u54D3|\u54D4|\u54D5|\u54D7|\u54D9|\u54DC|\u54DD|\u54DF|\u551B|\u551D|\u5520|\u5521|\u5522|\u5524|\u5567|\u556C|\u556D|\u556E|\u556F|\u5570|\u5574|\u5578|\u55B7|\u55BD|\u55BE|\u55EB|\u55F3|\u5618|\u5624|\u5631|\u565C|\u56A3|\u56E2|\u56ED|\u56F0|\u56F1|\u56F4|\u56F5|\u56FD|\u56FE|\u5706|\u5723|\u5739|\u573A|\u5742|\u574F|\u5757|\u575A|\u575B|\u575C|\u575D|\u575E|\u575F|\u5760|\u5784|\u5785|\u5786|\u5792|\u57A6|\u57A9|\u57AB|\u57AD|\u57AF|\u57B1|\u57B2|\u57B4|\u57D8|\u57D9|\u57DA|\u57EF|\u5811|\u5815|\u5846|\u5899|\u58EE|\u58F0|\u58F3|\u58F6|\u58F8|\u5904|\u5907|\u590D|\u591F|\u5934|\u5938|\u5939|\u593A|\u5941|\u5942|\u594B|\u5956|\u5965|\u5968|\u5978|\u5986|\u5987|\u5988|\u59A9|\u59AA|\u59AB|\u59D7|\u59F9|\u5A04|\u5A05|\u5A06|\u5A07|\u5A08|\u5A31|\u5A32|\u5A34|\u5A73|\u5A74|\u5A75|\u5A76|\u5AAA|\u5AAD|\u5AD2|\u5AD4|\u5AF1|\u5B37|\u5B59|\u5B66|\u5B6A|\u5B81|\u5B9D|\u5B9E|\u5BA0|\u5BA1|\u5BAA|\u5BAB|\u5BBD|\u5BBE|\u5BDD|\u5BF9|\u5BFB|\u5BFC|\u5BFF|\u5C06|\u5C14|\u5C18|\u5C1D|\u5C27|\u5C34|\u5C38|\u5C3D|\u5C42|\u5C43|\u5C49|\u5C4A|\u5C5E|\u5C61|\u5C66|\u5C7F|\u5C81|\u5C82|\u5C96|\u5C97|\u5C98|\u5C99|\u5C9A|\u5C9B|\u5CAD|\u5CBD|\u5CBF|\u5CC4|\u5CE1|\u5CE3|\u5CE4|\u5CE5|\u5CE6|\u5D02|\u5D03|\u5D04|\u5D2D|\u5D58|\u5D5A|\u5D5D|\u5DC5|\u5DE9|\u5DEF|\u5E01|\u5E05|\u5E08|\u5E0F|\u5E10|\u5E18|\u5E1C|\u5E26|\u5E27|\u5E2E|\u5E31|\u5E3B|\u5E3C|\u5E42|\u5E72|\u5E76|\u5E7F|\u5E84|\u5E86|\u5E90|\u5E91|\u5E93|\u5E94|\u5E99|\u5E9E|\u5E9F|\u5EEA|\u5F00|\u5F02|\u5F03|\u5F11|\u5F20|\u5F25|\u5F2A|\u5F2F|\u5F39|\u5F3A|\u5F52|\u5F53|\u5F55|\u5F5D|\u5F5F|\u5F66|\u5F68|\u5F7B|\u5F81|\u5F84|\u5F95|\u5FA1|\u5FC6|\u5FCF|\u5FD7|\u5FE7|\u5FFE|\u6000|\u6001|\u6002|\u6003|\u6004|\u6005|\u6006|\u601C|\u603B|\u603C|\u603F|\u604B|\u6052|\u6073|\u6076|\u6078|\u6079|\u607A|\u607B|\u607C|\u607D|\u60A6|\u60AB|\u60AC|\u60AD|\u60AE|\u60AF|\u60CA|\u60E7|\u60E8|\u60E9|\u60EB|\u60EC|\u60ED|\u60EE|\u60EF|\u6120|\u6124|\u6126|\u613F|\u6151|\u616D|\u61D1|\u61D2|\u61D4|\u6206|\u620B|\u620F|\u6217|\u6218|\u622C|\u622F|\u6237|\u6251|\u6267|\u6269|\u626A|\u626B|\u626C|\u6270|\u629A|\u629B|\u629F|\u62A0|\u62A1|\u62A2|\u62A4|\u62A5|\u62C5|\u62DF|\u62E2|\u62E3|\u62E5|\u62E6|\u62E7|\u62E8|\u62E9|\u6302|\u6319|\u631A|\u631B|\u631C|\u631D|\u631E|\u631F|\u6320|\u6321|\u6322|\u6323|\u6324|\u6325|\u6326|\u633D|\u635D|\u635E|\u635F|\u6361|\u6362|\u6363|\u636E|\u63B3|\u63B4|\u63B7|\u63B8|\u63BA|\u63BC|\u63FD|\u63FE|\u63FF|\u6400|\u6401|\u6402|\u6405|\u643A|\u6444|\u6445|\u6446|\u6447|\u6448|\u644A|\u6484|\u6491|\u64B5|\u64B7|\u64B8|\u64BA|\u64DC|\u64DE|\u6512|\u654C|\u655B|\u6569|\u6570|\u658B|\u6593|\u6597|\u65A9|\u65AD|\u65E0|\u65E7|\u65F6|\u65F7|\u65F8|\u6619|\u663C|\u663D|\u663E|\u664B|\u6652|\u6653|\u6654|\u6655|\u6656|\u6682|\u6685|\u66A7|\u66F2|\u672F|\u6734|\u673A|\u6740|\u6742|\u6743|\u6746|\u6761|\u6765|\u6768|\u6769|\u6770|\u677E|\u677F|\u6781|\u6784|\u679E|\u67A2|\u67A3|\u67A5|\u67A7|\u67A8|\u67AA|\u67AB|\u67AD|\u67DC|\u67E0|\u67FD|\u6800|\u6805|\u6807|\u6808|\u6809|\u680A|\u680B|\u680C|\u680E|\u680F|\u6811|\u6816|\u6817|\u6837|\u683E|\u6860|\u6861|\u6862|\u6863|\u6864|\u6865|\u6866|\u6867|\u6868|\u6869|\u686A|\u68A6|\u68BC|\u68BE|\u68BF|\u68C0|\u68C1|\u68C2|\u6901|\u691D|\u691F|\u6920|\u6922|\u6924|\u692B|\u692D|\u692E|\u697C|\u6984|\u6985|\u6987|\u6988|\u6989|\u69DA|\u69DB|\u69DF|\u69E0|\u6A2A|\u6A2F|\u6A31|\u6A65|\u6A71|\u6A79|\u6A7C|\u6AA9|\u6B22|\u6B24|\u6B27|\u6B7C|\u6B81|\u6B87|\u6B8B|\u6B92|\u6B93|\u6B9A|\u6BA1|\u6BB4|\u6BC1|\u6BC2|\u6BD5|\u6BD9|\u6BE1|\u6BF5|\u6BF6|\u6C07|\u6C14|\u6C22|\u6C29|\u6C32|\u6C47|\u6C49|\u6C64|\u6C79|\u6C88|\u6C9F|\u6CA1|\u6CA3|\u6CA4|\u6CA5|\u6CA6|\u6CA7|\u6CA8|\u6CA9|\u6CAA|\u6CDE|\u6CE8|\u6CEA|\u6CF6|\u6CF7|\u6CF8|\u6CFA|\u6CFB|\u6CFC|\u6CFD|\u6CFE|\u6D01|\u6D12|\u6D3C|\u6D43|\u6D45|\u6D46|\u6D47|\u6D48|\u6D49|\u6D4A|\u6D4B|\u6D4D|\u6D4E|\u6D4F|\u6D50|\u6D51|\u6D52|\u6D53|\u6D54|\u6D55|\u6D82|\u6D9B|\u6D9D|\u6D9E|\u6D9F|\u6DA0|\u6DA1|\u6DA2|\u6DA3|\u6DA4|\u6DA6|\u6DA7|\u6DA8|\u6DA9|\u6DC0|\u6E0A|\u6E0C|\u6E0D|\u6E0E|\u6E10|\u6E11|\u6E14|\u6E16|\u6E17|\u6E29|\u6E7E|\u6E7F|\u6E81|\u6E83|\u6E85|\u6E86|\u6E87|\u6ED7|\u6EDA|\u6EDE|\u6EDF|\u6EE0|\u6EE1|\u6EE2|\u6EE4|\u6EE5|\u6EE6|\u6EE8|\u6EE9|\u6EEA|\u6F13|\u6F24|\u6F46|\u6F47|\u6F4B|\u6F4D|\u6F5C|\u6F74|\u6F9B|\u6F9C|\u6FD1|\u6FD2|\u704F|\u706D|\u706F|\u7075|\u707E|\u707F|\u7080|\u7089|\u709C|\u709D|\u70B9|\u70BC|\u70BD|\u70C1|\u70C2|\u70C3|\u70DB|\u70DF|\u70E6|\u70E7|\u70E8|\u70E9|\u70EB|\u70EC|\u70ED|\u7115|\u7116|\u7118|\u7174|\u7231|\u7237|\u724D|\u7266|\u7275|\u727A|\u728A|\u72B6|\u72B7|\u72B8|\u72B9|\u72C8|\u72DD|\u72DE|\u72EC|\u72ED|\u72EE|\u72EF|\u72F0|\u72F1|\u72F2|\u7303|\u730E|\u7315|\u7321|\u732A|\u732B|\u732C|\u732E|\u736D|\u7391|\u7399|\u739A|\u739B|\u73AE|\u73AF|\u73B0|\u73B1|\u73BA|\u73D0|\u73D1|\u73F0|\u73F2|\u740E|\u740F|\u7410|\u743C|\u7476|\u7477|\u7478|\u748E|\u74D2|\u74EF|\u7535|\u753B|\u7545|\u7574|\u7596|\u7597|\u759F|\u75A0|\u75A1|\u75AC|\u75AD|\u75AE|\u75AF|\u75B1|\u75B4|\u75C7|\u75C8|\u75C9|\u75D2|\u75D6|\u75E8|\u75EA|\u75EB|\u7605|\u7606|\u7617|\u7618|\u762A|\u762B|\u763E|\u763F|\u765E|\u7663|\u766B|\u7691|\u76B1|\u76B2|\u76CF|\u76D0|\u76D1|\u76D6|\u76D7|\u76D8|\u770D|\u7726|\u772C|\u7740|\u7741|\u7750|\u7751|\u7786|\u7792|\u77A9|\u77EB|\u77F6|\u77FE|\u77FF|\u7800|\u7801|\u7816|\u7817|\u781A|\u781C|\u783A|\u783B|\u783E|\u7840|\u7841|\u7855|\u7856|\u7857|\u7859|\u785A|\u786E|\u7875|\u7877|\u788D|\u789B|\u789C|\u793C|\u7943|\u794E|\u7962|\u796F|\u7977|\u7978|\u7980|\u7984|\u7985|\u79BB|\u79C3|\u79C6|\u79CD|\u79EF|\u79F0|\u79FD|\u79FE|\u7A06|\u7A0E|\u7A23|\u7A33|\u7A51|\u7A77|\u7A83|\u7A8D|\u7A8E|\u7A91|\u7A9C|\u7A9D|\u7AA5|\u7AA6|\u7AAD|\u7AD6|\u7ADE|\u7B03|\u7B0B|\u7B14|\u7B15|\u7B3A|\u7B3C|\u7B3E|\u7B51|\u7B5A|\u7B5B|\u7B5C|\u7B5D|\u7B79|\u7B7C|\u7B7E|\u7B80|\u7B93|\u7BA6|\u7BA7|\u7BA8|\u7BA9|\u7BAA|\u7BAB|\u7BD1|\u7BD3|\u7BEE|\u7BEF|\u7BF1|\u7C16|\u7C41|\u7C74|\u7C7B|\u7C7C|\u7C9C|\u7C9D|\u7CA4|\u7CAA|\u7CAE|\u7CC1|\u7CC7|\u7CFB|\u7D27|\u7D2F|\u7D77|\u7E9F|\u7EA0|\u7EA1|\u7EA2|\u7EA3|\u7EA4|\u7EA5|\u7EA6|\u7EA7|\u7EA8|\u7EA9|\u7EAA|\u7EAB|\u7EAC|\u7EAD|\u7EAE|\u7EAF|\u7EB0|\u7EB1|\u7EB2|\u7EB3|\u7EB4|\u7EB5|\u7EB6|\u7EB7|\u7EB8|\u7EB9|\u7EBA|\u7EBB|\u7EBC|\u7EBD|\u7EBE|\u7EBF|\u7EC0|\u7EC1|\u7EC2|\u7EC3|\u7EC4|\u7EC5|\u7EC6|\u7EC7|\u7EC8|\u7EC9|\u7ECA|\u7ECB|\u7ECC|\u7ECD|\u7ECE|\u7ECF|\u7ED0|\u7ED1|\u7ED2|\u7ED3|\u7ED4|\u7ED5|\u7ED6|\u7ED7|\u7ED8|\u7ED9|\u7EDA|\u7EDB|\u7EDC|\u7EDD|\u7EDE|\u7EDF|\u7EE0|\u7EE1|\u7EE2|\u7EE3|\u7EE4|\u7EE5|\u7EE6|\u7EE7|\u7EE8|\u7EE9|\u7EEA|\u7EEB|\u7EEC|\u7EED|\u7EEE|\u7EEF|\u7EF0|\u7EF1|\u7EF2|\u7EF3|\u7EF4|\u7EF5|\u7EF6|\u7EF7|\u7EF8|\u7EF9|\u7EFA|\u7EFB|\u7EFC|\u7EFD|\u7EFE|\u7EFF|\u7F00|\u7F01|\u7F02|\u7F03|\u7F04|\u7F05|\u7F06|\u7F07|\u7F08|\u7F09|\u7F0A|\u7F0B|\u7F0C|\u7F0D|\u7F0E|\u7F0F|\u7F10|\u7F11|\u7F12|\u7F13|\u7F14|\u7F15|\u7F16|\u7F17|\u7F18|\u7F19|\u7F1A|\u7F1B|\u7F1C|\u7F1D|\u7F1E|\u7F1F|\u7F20|\u7F21|\u7F22|\u7F23|\u7F24|\u7F25|\u7F26|\u7F27|\u7F28|\u7F29|\u7F2A|\u7F2B|\u7F2C|\u7F2D|\u7F2E|\u7F2F|\u7F30|\u7F31|\u7F32|\u7F33|\u7F34|\u7F35|\u7F42|\u7F51|\u7F57|\u7F5A|\u7F62|\u7F74|\u7F81|\u7F9F|\u7FD8|\u7FD9|\u7FDA|\u8022|\u8027|\u8038|\u803B|\u8042|\u804B|\u804C|\u804D|\u8054|\u8069|\u806A|\u8083|\u80A0|\u80A4|\u80AE|\u80B4|\u80BE|\u80BF|\u80C0|\u80C1|\u80C6|\u80DC|\u80E1|\u80E7|\u80E8|\u80EA|\u80EB|\u80F6|\u8109|\u810D|\u810F|\u8110|\u8111|\u8113|\u8114|\u811A|\u8131|\u8136|\u8138|\u814A|\u8158|\u816D|\u817B|\u817C|\u817D|\u817E|\u8191|\u81DC|\u81F4|\u8206|\u820D|\u8223|\u8230|\u8231|\u823B|\u8270|\u8273|\u827A|\u8282|\u8288|\u8297|\u829C|\u82A6|\u82B8|\u82C1|\u82C7|\u82C8|\u82CB|\u82CC|\u82CD|\u82CE|\u82CF|\u82E7|\u82F9|\u8303|\u830E|\u830F|\u8311|\u8314|\u8315|\u8327|\u8346|\u8350|\u8359|\u835A|\u835B|\u835C|\u835D|\u835E|\u835F|\u8360|\u8361|\u8363|\u8364|\u8365|\u8366|\u8367|\u8368|\u8369|\u836A|\u836B|\u836C|\u836D|\u836E|\u836F|\u8385|\u83B1|\u83B2|\u83B3|\u83B4|\u83B6|\u83B7|\u83B8|\u83B9|\u83BA|\u83BC|\u841A|\u841D|\u8424|\u8425|\u8426|\u8427|\u8428|\u8471|\u8487|\u8489|\u848B|\u848C|\u84DD|\u84DF|\u84E0|\u84E3|\u84E5|\u84E6|\u8502|\u8537|\u8539|\u853A|\u853C|\u8570|\u8572|\u8574|\u85AE|\u85D3|\u8616|\u864F|\u8651|\u865A|\u866B|\u866C|\u866E|\u867D|\u867E|\u867F|\u8680|\u8681|\u8682|\u8683|\u8695|\u86AC|\u86CA|\u86CE|\u86CF|\u86EE|\u86F0|\u86F1|\u86F2|\u86F3|\u86F4|\u8715|\u8717|\u8721|\u8747|\u8748|\u8749|\u877C|\u877E|\u8780|\u87A8|\u87CF|\u8845|\u8854|\u8865|\u8868|\u886C|\u886E|\u8884|\u8885|\u8886|\u889C|\u88AD|\u88AF|\u88C5|\u88C6|\u88C8|\u88E2|\u88E3|\u88E4|\u88E5|\u891B|\u891D|\u8934|\u8955|\u89C1|\u89C2|\u89C3|\u89C4|\u89C5|\u89C6|\u89C7|\u89C8|\u89C9|\u89CA|\u89CB|\u89CC|\u89CD|\u89CE|\u89CF|\u89D0|\u89D1|\u89DE|\u89E6|\u89EF|\u8A1A|\u8A5F|\u8A89|\u8A8A|\u8BA0|\u8BA1|\u8BA2|\u8BA3|\u8BA4|\u8BA5|\u8BA6|\u8BA7|\u8BA8|\u8BA9|\u8BAA|\u8BAB|\u8BAC|\u8BAD|\u8BAE|\u8BAF|\u8BB0|\u8BB1|\u8BB2|\u8BB3|\u8BB4|\u8BB5|\u8BB6|\u8BB7|\u8BB8|\u8BB9|\u8BBA|\u8BBB|\u8BBC|\u8BBD|\u8BBE|\u8BBF|\u8BC0|\u8BC1|\u8BC2|\u8BC3|\u8BC4|\u8BC5|\u8BC6|\u8BC7|\u8BC8|\u8BC9|\u8BCA|\u8BCB|\u8BCC|\u8BCD|\u8BCE|\u8BCF|\u8BD0|\u8BD1|\u8BD2|\u8BD3|\u8BD4|\u8BD5|\u8BD6|\u8BD7|\u8BD8|\u8BD9|\u8BDA|\u8BDB|\u8BDC|\u8BDD|\u8BDE|\u8BDF|\u8BE0|\u8BE1|\u8BE2|\u8BE3|\u8BE4|\u8BE5|\u8BE6|\u8BE7|\u8BE8|\u8BE9|\u8BEA|\u8BEB|\u8BEC|\u8BED|\u8BEE|\u8BEF|\u8BF0|\u8BF1|\u8BF2|\u8BF3|\u8BF4|\u8BF5|\u8BF6|\u8BF7|\u8BF8|\u8BF9|\u8BFA|\u8BFB|\u8BFC|\u8BFD|\u8BFE|\u8BFF|\u8C00|\u8C01|\u8C02|\u8C03|\u8C04|\u8C05|\u8C06|\u8C07|\u8C08|\u8C09|\u8C0A|\u8C0B|\u8C0C|\u8C0D|\u8C0E|\u8C0F|\u8C10|\u8C11|\u8C12|\u8C13|\u8C14|\u8C15|\u8C16|\u8C17|\u8C18|\u8C19|\u8C1A|\u8C1B|\u8C1C|\u8C1D|\u8C1E|\u8C1F|\u8C20|\u8C21|\u8C22|\u8C23|\u8C24|\u8C25|\u8C26|\u8C27|\u8C28|\u8C29|\u8C2A|\u8C2B|\u8C2C|\u8C2D|\u8C2E|\u8C2F|\u8C30|\u8C31|\u8C32|\u8C33|\u8C34|\u8C35|\u8C36|\u8C37|\u8C6E|\u8D1D|\u8D1E|\u8D1F|\u8D20|\u8D21|\u8D22|\u8D23|\u8D24|\u8D25|\u8D26|\u8D27|\u8D28|\u8D29|\u8D2A|\u8D2B|\u8D2C|\u8D2D|\u8D2E|\u8D2F|\u8D30|\u8D31|\u8D32|\u8D33|\u8D34|\u8D35|\u8D36|\u8D37|\u8D38|\u8D39|\u8D3A|\u8D3B|\u8D3C|\u8D3D|\u8D3E|\u8D3F|\u8D40|\u8D41|\u8D42|\u8D43|\u8D44|\u8D45|\u8D46|\u8D47|\u8D48|\u8D49|\u8D4A|\u8D4B|\u8D4C|\u8D4D|\u8D4E|\u8D4F|\u8D50|\u8D51|\u8D52|\u8D53|\u8D54|\u8D55|\u8D56|\u8D57|\u8D58|\u8D59|\u8D5A|\u8D5B|\u8D5C|\u8D5D|\u8D5E|\u8D5F|\u8D60|\u8D61|\u8D62|\u8D63|\u8D6A|\u8D75|\u8D76|\u8D8B|\u8DB1|\u8DB8|\u8DC3|\u8DC4|\u8DDE|\u8DF5|\u8DF6|\u8DF7|\u8DF8|\u8DF9|\u8DFB|\u8E0A|\u8E0C|\u8E2A|\u8E2C|\u8E2F|\u8E51|\u8E52|\u8E70|\u8E7F|\u8E8F|\u8E9C|\u8EAF|\u8F66|\u8F67|\u8F68|\u8F69|\u8F6A|\u8F6B|\u8F6C|\u8F6D|\u8F6E|\u8F6F|\u8F70|\u8F71|\u8F72|\u8F73|\u8F74|\u8F75|\u8F76|\u8F77|\u8F78|\u8F79|\u8F7A|\u8F7B|\u8F7C|\u8F7D|\u8F7E|\u8F7F|\u8F80|\u8F81|\u8F82|\u8F83|\u8F84|\u8F85|\u8F86|\u8F87|\u8F88|\u8F89|\u8F8A|\u8F8B|\u8F8C|\u8F8D|\u8F8E|\u8F8F|\u8F90|\u8F91|\u8F92|\u8F93|\u8F94|\u8F95|\u8F96|\u8F97|\u8F98|\u8F99|\u8F9A|\u8F9E|\u8F9F|\u8FA9|\u8FAB|\u8FB9|\u8FBD|\u8FBE|\u8FC1|\u8FC7|\u8FC8|\u8FD0|\u8FD8|\u8FD9|\u8FDB|\u8FDC|\u8FDD|\u8FDE|\u8FDF|\u8FE9|\u8FF3|\u8FF9|\u9002|\u9009|\u900A|\u9012|\u9026|\u903B|\u9057|\u9065|\u9093|\u909D|\u90AC|\u90AE|\u90B9|\u90BA|\u90BB|\u90C1|\u90CF|\u90D0|\u90D1|\u90D3|\u90E6|\u90E7|\u90F8|\u9142|\u915D|\u9166|\u9171|\u917D|\u917E|\u917F|\u91C7|\u91CA|\u91CC|\u9274|\u92AE|\u933E|\u9485|\u9486|\u9487|\u9488|\u9489|\u948A|\u948B|\u948C|\u948D|\u948E|\u948F|\u9490|\u9491|\u9492|\u9493|\u9494|\u9495|\u9496|\u9497|\u9498|\u9499|\u949A|\u949B|\u949C|\u949D|\u949E|\u949F|\u94A0|\u94A1|\u94A2|\u94A3|\u94A4|\u94A5|\u94A6|\u94A7|\u94A8|\u94A9|\u94AA|\u94AB|\u94AC|\u94AD|\u94AE|\u94AF|\u94B0|\u94B1|\u94B2|\u94B3|\u94B4|\u94B5|\u94B6|\u94B7|\u94B8|\u94B9|\u94BA|\u94BB|\u94BC|\u94BD|\u94BE|\u94BF|\u94C0|\u94C1|\u94C2|\u94C3|\u94C4|\u94C5|\u94C6|\u94C7|\u94C8|\u94C9|\u94CA|\u94CB|\u94CC|\u94CD|\u94CE|\u94CF|\u94D0|\u94D1|\u94D2|\u94D3|\u94D4|\u94D5|\u94D6|\u94D7|\u94D8|\u94D9|\u94DA|\u94DB|\u94DC|\u94DD|\u94DE|\u94DF|\u94E0|\u94E1|\u94E2|\u94E3|\u94E4|\u94E5|\u94E6|\u94E7|\u94E8|\u94E9|\u94EA|\u94EB|\u94EC|\u94ED|\u94EE|\u94EF|\u94F0|\u94F1|\u94F2|\u94F3|\u94F4|\u94F5|\u94F6|\u94F7|\u94F8|\u94F9|\u94FA|\u94FB|\u94FC|\u94FD|\u94FE|\u94FF|\u9500|\u9501|\u9502|\u9503|\u9504|\u9505|\u9506|\u9507|\u9508|\u9509|\u950A|\u950B|\u950C|\u950D|\u950E|\u950F|\u9510|\u9511|\u9512|\u9513|\u9514|\u9515|\u9516|\u9517|\u9518|\u9519|\u951A|\u951B|\u951C|\u951D|\u951E|\u951F|\u9520|\u9521|\u9522|\u9523|\u9524|\u9525|\u9526|\u9527|\u9528|\u9529|\u952A|\u952B|\u952C|\u952D|\u952E|\u952F|\u9530|\u9531|\u9532|\u9533|\u9534|\u9535|\u9536|\u9537|\u9538|\u9539|\u953A|\u953B|\u953C|\u953D|\u953E|\u953F|\u9540|\u9541|\u9542|\u9543|\u9544|\u9545|\u9546|\u9547|\u9548|\u9549|\u954A|\u954B|\u954C|\u954D|\u954E|\u954F|\u9550|\u9551|\u9552|\u9553|\u9554|\u9555|\u9556|\u9557|\u9558|\u9559|\u955A|\u955B|\u955C|\u955D|\u955E|\u955F|\u9560|\u9561|\u9562|\u9563|\u9564|\u9565|\u9566|\u9567|\u9568|\u9569|\u956A|\u956B|\u956C|\u956D|\u956E|\u956F|\u9570|\u9571|\u9572|\u9573|\u9574|\u9575|\u9576|\u957F|\u95E8|\u95E9|\u95EA|\u95EB|\u95EC|\u95ED|\u95EE|\u95EF|\u95F0|\u95F1|\u95F2|\u95F3|\u95F4|\u95F5|\u95F6|\u95F7|\u95F8|\u95F9|\u95FA|\u95FB|\u95FC|\u95FD|\u95FE|\u95FF|\u9600|\u9601|\u9602|\u9603|\u9604|\u9605|\u9606|\u9607|\u9608|\u9609|\u960A|\u960B|\u960C|\u960D|\u960E|\u960F|\u9610|\u9611|\u9612|\u9613|\u9614|\u9615|\u9616|\u9617|\u9618|\u9619|\u961A|\u961B|\u961F|\u9633|\u9634|\u9635|\u9636|\u9645|\u9646|\u9647|\u9648|\u9649|\u9655|\u9666|\u9667|\u9668|\u9669|\u968F|\u9690|\u96B6|\u96BD|\u96BE|\u96CF|\u96E0|\u96F3|\u96FE|\u9701|\u9721|\u972D|\u9753|\u9759|\u9762|\u9765|\u9791|\u9792|\u97AF|\u97E6|\u97E7|\u97E8|\u97E9|\u97EA|\u97EB|\u97EC|\u97F5|\u9875|\u9876|\u9877|\u9878|\u9879|\u987A|\u987B|\u987C|\u987D|\u987E|\u987F|\u9880|\u9881|\u9882|\u9883|\u9884|\u9885|\u9886|\u9887|\u9888|\u9889|\u988A|\u988B|\u988C|\u988D|\u988E|\u988F|\u9890|\u9891|\u9892|\u9893|\u9894|\u9895|\u9896|\u9897|\u9898|\u9899|\u989A|\u989B|\u989C|\u989D|\u989E|\u989F|\u98A0|\u98A1|\u98A2|\u98A3|\u98A4|\u98A5|\u98A6|\u98A7|\u98CE|\u98CF|\u98D0|\u98D1|\u98D2|\u98D3|\u98D4|\u98D5|\u98D6|\u98D7|\u98D8|\u98D9|\u98DA|\u98DE|\u98E8|\u990D|\u9963|\u9964|\u9965|\u9966|\u9967|\u9968|\u9969|\u996A|\u996B|\u996C|\u996D|\u996E|\u996F|\u9970|\u9971|\u9972|\u9973|\u9974|\u9975|\u9976|\u9977|\u9978|\u9979|\u997A|\u997B|\u997C|\u997D|\u997E|\u997F|\u9980|\u9981|\u9982|\u9983|\u9984|\u9985|\u9986|\u9987|\u9988|\u9989|\u998A|\u998B|\u998C|\u998D|\u998E|\u998F|\u9990|\u9991|\u9992|\u9993|\u9994|\u9995|\u9A6C|\u9A6D|\u9A6E|\u9A6F|\u9A70|\u9A71|\u9A72|\u9A73|\u9A74|\u9A75|\u9A76|\u9A77|\u9A78|\u9A79|\u9A7A|\u9A7B|\u9A7C|\u9A7D|\u9A7E|\u9A7F|\u9A80|\u9A81|\u9A82|\u9A83|\u9A84|\u9A85|\u9A86|\u9A87|\u9A88|\u9A89|\u9A8A|\u9A8B|\u9A8C|\u9A8D|\u9A8E|\u9A8F|\u9A90|\u9A91|\u9A92|\u9A93|\u9A94|\u9A95|\u9A96|\u9A97|\u9A98|\u9A99|\u9A9A|\u9A9B|\u9A9C|\u9A9D|\u9A9E|\u9A9F|\u9AA0|\u9AA1|\u9AA2|\u9AA3|\u9AA4|\u9AA5|\u9AA6|\u9AA7|\u9AC5|\u9ACB|\u9ACC|\u9B13|\u9B36|\u9B47|\u9B49|\u9C7C|\u9C7D|\u9C7E|\u9C7F|\u9C80|\u9C81|\u9C82|\u9C83|\u9C84|\u9C85|\u9C86|\u9C87|\u9C88|\u9C89|\u9C8A|\u9C8B|\u9C8C|\u9C8D|\u9C8E|\u9C8F|\u9C90|\u9C91|\u9C92|\u9C93|\u9C94|\u9C95|\u9C96|\u9C97|\u9C98|\u9C99|\u9C9A|\u9C9B|\u9C9C|\u9C9D|\u9C9E|\u9C9F|\u9CA0|\u9CA1|\u9CA2|\u9CA3|\u9CA4|\u9CA5|\u9CA6|\u9CA7|\u9CA8|\u9CA9|\u9CAA|\u9CAB|\u9CAC|\u9CAD|\u9CAE|\u9CAF|\u9CB0|\u9CB1|\u9CB2|\u9CB3|\u9CB4|\u9CB5|\u9CB6|\u9CB7|\u9CB8|\u9CB9|\u9CBA|\u9CBB|\u9CBC|\u9CBD|\u9CBE|\u9CBF|\u9CC0|\u9CC1|\u9CC2|\u9CC3|\u9CC4|\u9CC5|\u9CC6|\u9CC7|\u9CC8|\u9CC9|\u9CCA|\u9CCB|\u9CCC|\u9CCD|\u9CCE|\u9CCF|\u9CD0|\u9CD1|\u9CD2|\u9CD3|\u9CD4|\u9CD5|\u9CD6|\u9CD7|\u9CD8|\u9CD9|\u9CDA|\u9CDB|\u9CDC|\u9CDD|\u9CDE|\u9CDF|\u9CE0|\u9CE1|\u9CE2|\u9CE3|\u9CE4|\u9E1F|\u9E20|\u9E21|\u9E22|\u9E23|\u9E24|\u9E25|\u9E26|\u9E27|\u9E28|\u9E29|\u9E2A|\u9E2B|\u9E2C|\u9E2D|\u9E2E|\u9E2F|\u9E30|\u9E31|\u9E32|\u9E33|\u9E34|\u9E35|\u9E36|\u9E37|\u9E38|\u9E39|\u9E3A|\u9E3B|\u9E3C|\u9E3D|\u9E3E|\u9E3F|\u9E40|\u9E41|\u9E42|\u9E43|\u9E44|\u9E45|\u9E46|\u9E47|\u9E48|\u9E49|\u9E4A|\u9E4B|\u9E4C|\u9E4D|\u9E4E|\u9E4F|\u9E50|\u9E51|\u9E52|\u9E53|\u9E54|\u9E55|\u9E56|\u9E57|\u9E58|\u9E59|\u9E5A|\u9E5B|\u9E5C|\u9E5D|\u9E5E|\u9E5F|\u9E60|\u9E61|\u9E62|\u9E63|\u9E64|\u9E65|\u9E66|\u9E67|\u9E68|\u9E69|\u9E6A|\u9E6B|\u9E6C|\u9E6D|\u9E6E|\u9E6F|\u9E70|\u9E71|\u9E72|\u9E73|\u9E74|\u9E7E|\u9EA6|\u9EB8|\u9EB9|\u9EC4|\u9EC9|\u9EE1|\u9EE9|\u9EEA|\u9EFE|\u9F0B|\u9F0D|\u9F17|\u9F39|\u9F50|\u9F51|\u9F7F|\u9F80|\u9F81|\u9F82|\u9F83|\u9F84|\u9F85|\u9F86|\u9F87|\u9F88|\u9F89|\u9F8A|\u9F8B|\u9F8C|\u9F99|\u9F9A|\u9F9B|\u9F9F|\u9FCE|\u9FCF|\u9FD3|\u9FD4|\u9FD5|\u9FED|\u2003E|\u200B2|\u200D3|\u201B2|\u201BF|\u201D0|\u201F9|\u20242|\u20257|\u206B3|\u206C5|\u206C6|\u206FE|\u20860|\u20B24|\u20BDF|\u20BE0|\u20C37|\u20C5E|\u20CA5|\u20CDE|\u20D22|\u20D78|\u20D7E|\u2121B|\u21291|\u212C0|\u212D7|\u212E4|\u213C6|\u21484|\u21760|\u2178B|\u217B1|\u2181F|\u21847|\u21967|\u21B5C|\u21B6C|\u21CC3|\u21CD2|\u21D5D|\u21DB4|\u21E03|\u21E83|\u21ED8|\u22016|\u222C8|\u224C5|\u225D3|\u22619|\u2261D|\u2261E|\u22650|\u22651|\u22652|\u22653|\u226EF|\u227FC|\u229D0|\u22A93|\u22A97|\u22ACA|\u22AD8|\u22ADE|\u22AEC|\u22B0D|\u22B26|\u22B4F|\u22DA3|\u22F7E|\u230C1|\u23190|\u23223|\u2327C|\u23368|\u2336F|\u23370|\u23391|\u233E2|\u23415|\u23424|\u2345D|\u23476|\u2348C|\u23497|\u234FF|\u23572|\u235CA|\u235CB|\u23610|\u23613|\u23634|\u23637|\u2363E|\u23665|\u2369A|\u2378E|\u23A3C|\u23B64|\u23BE3|\u23C5D|\u23C97|\u23CC6|\u23DA9|\u23DAB|\u23E23|\u23EBC|\u23EBD|\u23F77|\u23F8D|\u241A1|\u241A2|\u241C3|\u241C4|\u241ED|\u241FB|\u24236|\u24237|\u24280|\u242CF|\u243BA|\u243BB|\u2466F|\u24735|\u24762|\u24783|\u247A4|\u2480B|\u24980|\u24A7D|\u24CC4|\u24D8A|\u24DA7|\u24E7A|\u24ECA|\u24F6F|\u24F80|\u24FF2|\u25062|\u25158|\u25174|\u251A7|\u251E2|\u2539D|\u2541F|\u2542F|\u25430|\u2543B|\u25564|\u257A6|\u257C2|\u259C2|\u25A7A|\u25B00|\u25B08|\u25B1E|\u25B20|\u25B49|\u25B8B|\u25B9C|\u25BBE|\u25C54|\u25E65|\u25E85|\u25E87|\u26208|\u26209|\u2620B|\u2620C|\u2620E|\u2620F|\u26210|\u26211|\u26212|\u26213|\u26214|\u26215|\u26216|\u26217|\u26218|\u26219|\u2621A|\u2621B|\u2621C|\u2621D|\u2621E|\u2621F|\u26220|\u26221|\u26360|\u266E8|\u2677C|\u2678C|\u267D7|\u26A29|\u26B19|\u26C34|\u26D07|\u26ED5|\u27234|\u2723F|\u27250|\u2725E|\u273D6|\u273D7|\u2744F|\u274AD|\u27721|\u2772D|\u2775D|\u27924|\u27945|\u27BAA|\u27CD5|\u27E51|\u27E52|\u27E53|\u27E54|\u27E55|\u27E56|\u27E57|\u27EA3|\u27FC8|\u27FDB|\u28001|\u28031|\u28074|\u280BA|\u28104|\u2815B|\u2816B|\u2816C|\u28257|\u28405|\u28406|\u28407|\u28408|\u28409|\u2840A|\u28479|\u28755|\u287F3|\u28828|\u28859|\u2887A|\u288B8|\u28930|\u289EE|\u28C3E|\u28C3F|\u28C40|\u28C41|\u28C42|\u28C43|\u28C44|\u28C45|\u28C46|\u28C47|\u28C48|\u28C49|\u28C4A|\u28C4B|\u28C4C|\u28C4D|\u28C4E|\u28C4F|\u28C50|\u28C51|\u28C52|\u28C53|\u28C54|\u28C55|\u28C56|\u28DFF|\u28E00|\u28E01|\u28E02|\u28E03|\u28E04|\u28E05|\u28E06|\u28E07|\u28E09|\u28E0A|\u28E0B|\u28E0C|\u28E0E|\u28E18|\u28E1F|\u28EF9|\u293FC|\u293FD|\u293FE|\u293FF|\u29400|\u29595|\u29596|\u29597|\u29665|\u29666|\u29667|\u29668|\u29669|\u2966A|\u2966B|\u2966C|\u2966D|\u2966E|\u2966F|\u29670|\u297FF|\u29800|\u29801|\u29802|\u29803|\u29805|\u29806|\u29807|\u29808|\u29809|\u2980A|\u2980B|\u2980C|\u2980E|\u2980F|\u29820|\u29856|\u2985A|\u299E6|\u299E8|\u299E9|\u299EA|\u299EB|\u299EC|\u299ED|\u299EE|\u299EF|\u299F0|\u299F1|\u299F2|\u299F3|\u299F4|\u299F5|\u299F6|\u299F8|\u299FA|\u299FB|\u299FC|\u299FF|\u29A00|\u29A01|\u29A03|\u29A04|\u29A05|\u29A06|\u29A07|\u29A08|\u29A09|\u29A0A|\u29A0B|\u29A0C|\u29A0D|\u29A0E|\u29A0F|\u29A10|\u29A48|\u29B23|\u29B24|\u29B3E|\u29B79|\u29BD2|\u29C30|\u29C92|\u29D0C|\u29F79|\u29F7A|\u29F7B|\u29F7C|\u29F7D|\u29F7E|\u29F7F|\u29F81|\u29F82|\u29F83|\u29F84|\u29F85|\u29F86|\u29F87|\u29F88|\u29F8A|\u29F8B|\u29F8C|\u29F8E|\u2A242|\u2A243|\u2A244|\u2A245|\u2A246|\u2A248|\u2A249|\u2A24A|\u2A24B|\u2A24C|\u2A24D|\u2A24E|\u2A24F|\u2A250|\u2A251|\u2A252|\u2A254|\u2A255|\u2A388|\u2A389|\u2A38A|\u2A38B|\u2A38C|\u2A445|\u2A52D|\u2A68F|\u2A690|\u2A70E|\u2A73A|\u2A79D|\u2A7CE|\u2A7DD|\u2A7F2|\u2A800|\u2A803|\u2A80F|\u2A81F|\u2A821|\u2A833|\u2A835|\u2A838|\u2A83D|\u2A840|\u2A843|\u2A84B|\u2A84F|\u2A85B|\u2A85E|\u2A87A|\u2A888|\u2A88B|\u2A88C|\u2A890|\u2A892|\u2A895|\u2A8A0|\u2A8AE|\u2A8C6|\u2A8D2|\u2A8FB|\u2A904|\u2A905|\u2A91A|\u2A960|\u2A96B|\u2A970|\u2A97F|\u2A9C0|\u2A9D8|\u2AA07|\u2AA0A|\u2AA17|\u2AA27|\u2AA29|\u2AA36|\u2AA37|\u2AA39|\u2AA47|\u2AA4E|\u2AA58|\u2AA5B|\u2AA78|\u2AA91|\u2AA9E|\u2AAB4|\u2AACC|\u2AAE1|\u2AAF7|\u2AAF8|\u2AAFA|\u2AB1A|\u2AB2F|\u2AB5D|\u2AB62|\u2AB67|\u2AB6F|\u2AB75|\u2AB7E|\u2AB83|\u2AB8B|\u2AB96|\u2ABB3|\u2ABB6|\u2ABCB|\u2AC36|\u2AC65|\u2AC77|\u2AC8E|\u2AC94|\u2AC9B|\u2ACAE|\u2ACCD|\u2AD19|\u2AD2F|\u2AD47|\u2AD51|\u2AD63|\u2AD71|\u2AD84|\u2AD92|\u2ADAE|\u2ADCD|\u2ADFD|\u2AE15|\u2AE29|\u2AE40|\u2AE60|\u2AE73|\u2AE79|\u2AEA3|\u2AEAA|\u2AEAD|\u2AEB7|\u2AEB8|\u2AEBB|\u2AEBD|\u2AED0|\u2AEE8|\u2AEF2|\u2AEFA|\u2AF0B|\u2AF34|\u2AF42|\u2AF5D|\u2AF6A|\u2AF6D|\u2AF6E|\u2AF74|\u2AF77|\u2AF94|\u2AFA2|\u2AFA6|\u2AFB8|\u2AFCA|\u2AFDE|\u2AFEB|\u2AFF5|\u2B00C|\u2B013|\u2B028|\u2B02C|\u2B02E|\u2B042|\u2B05F|\u2B061|\u2B072|\u2B073|\u2B077|\u2B07A|\u2B083|\u2B086|\u2B088|\u2B096|\u2B0BF|\u2B0D7|\u2B119|\u2B11A|\u2B11B|\u2B11C|\u2B11D|\u2B11E|\u2B11F|\u2B120|\u2B121|\u2B122|\u2B123|\u2B124|\u2B125|\u2B126|\u2B127|\u2B128|\u2B129|\u2B12A|\u2B12B|\u2B12C|\u2B12D|\u2B12E|\u2B12F|\u2B130|\u2B131|\u2B132|\u2B133|\u2B134|\u2B135|\u2B136|\u2B137|\u2B138|\u2B139|\u2B145|\u2B157|\u2B165|\u2B16D|\u2B17C|\u2B18F|\u2B19D|\u2B1AB|\u2B1D8|\u2B1EA|\u2B1ED|\u2B1F4|\u2B1FD|\u2B209|\u2B20E|\u2B21F|\u2B235|\u2B241|\u2B244|\u2B2AA|\u2B2AE|\u2B2B1|\u2B2B8|\u2B2B9|\u2B2BB|\u2B2C7|\u2B2CC|\u2B2F2|\u2B2F7|\u2B2F9|\u2B2FB|\u2B300|\u2B307|\u2B30B|\u2B328|\u2B329|\u2B32A|\u2B32B|\u2B32C|\u2B32D|\u2B32F|\u2B34F|\u2B350|\u2B359|\u2B35A|\u2B35B|\u2B35C|\u2B35E|\u2B35F|\u2B360|\u2B361|\u2B362|\u2B363|\u2B364|\u2B365|\u2B366|\u2B367|\u2B368|\u2B369|\u2B36A|\u2B36B|\u2B36C|\u2B36D|\u2B36E|\u2B36F|\u2B370|\u2B371|\u2B372|\u2B373|\u2B374|\u2B375|\u2B376|\u2B377|\u2B378|\u2B379|\u2B37A|\u2B37B|\u2B37C|\u2B37D|\u2B37E|\u2B37F|\u2B386|\u2B38C|\u2B3A6|\u2B3A7|\u2B3A8|\u2B3A9|\u2B3AA|\u2B3AB|\u2B3AC|\u2B3AD|\u2B3B1|\u2B3B3|\u2B3B8|\u2B3BA|\u2B3C3|\u2B3C6|\u2B3CB|\u2B3CC|\u2B3D0|\u2B3D1|\u2B3D5|\u2B3DE|\u2B3E8|\u2B404|\u2B405|\u2B406|\u2B407|\u2B408|\u2B409|\u2B40A|\u2B40B|\u2B40C|\u2B40D|\u2B40E|\u2B40F|\u2B410|\u2B411|\u2B412|\u2B413|\u2B414|\u2B415|\u2B416|\u2B417|\u2B418|\u2B419|\u2B437|\u2B458|\u2B461|\u2B477|\u2B4E5|\u2B4E6|\u2B4E7|\u2B4E8|\u2B4E9|\u2B4EA|\u2B4EB|\u2B4EC|\u2B4ED|\u2B4EE|\u2B4EF|\u2B4F0|\u2B4F1|\u2B4F2|\u2B4F3|\u2B4F4|\u2B4F5|\u2B4F6|\u2B4F7|\u2B4F8|\u2B4F9|\u2B4FA|\u2B4FB|\u2B4FC|\u2B4FD|\u2B4FE|\u2B4FF|\u2B500|\u2B501|\u2B502|\u2B503|\u2B504|\u2B505|\u2B506|\u2B507|\u2B508|\u2B509|\u2B50A|\u2B50B|\u2B50C|\u2B50D|\u2B50E|\u2B50F|\u2B510|\u2B511|\u2B512|\u2B513|\u2B514|\u2B515|\u2B516|\u2B52D|\u2B52F|\u2B530|\u2B531|\u2B532|\u2B534|\u2B535|\u2B536|\u2B53D|\u2B55A|\u2B565|\u2B568|\u2B583|\u2B585|\u2B587|\u2B591|\u2B592|\u2B593|\u2B594|\u2B595|\u2B596|\u2B5AA|\u2B5AB|\u2B5AC|\u2B5AD|\u2B5AE|\u2B5AF|\u2B5B0|\u2B5B1|\u2B5B2|\u2B5B3|\u2B5B4|\u2B5B5|\u2B5B6|\u2B5B7|\u2B5B8|\u2B5B9|\u2B5BA|\u2B5C7|\u2B5C8|\u2B5C9|\u2B5CA|\u2B5CB|\u2B5DA|\u2B5DE|\u2B5DF|\u2B5E0|\u2B5E1|\u2B5E2|\u2B5E3|\u2B5E4|\u2B5E5|\u2B5E6|\u2B5E7|\u2B5E8|\u2B5E9|\u2B5EA|\u2B5EB|\u2B5EC|\u2B5ED|\u2B5EE|\u2B5EF|\u2B5F0|\u2B5F1|\u2B5F2|\u2B5F3|\u2B5F4|\u2B5F5|\u2B61B|\u2B61C|\u2B61D|\u2B61E|\u2B61F|\u2B620|\u2B621|\u2B623|\u2B624|\u2B625|\u2B626|\u2B627|\u2B628|\u2B629|\u2B62A|\u2B62B|\u2B62C|\u2B62D|\u2B62E|\u2B62F|\u2B630|\u2B631|\u2B63D|\u2B642|\u2B688|\u2B689|\u2B68A|\u2B68B|\u2B68C|\u2B68D|\u2B68E|\u2B68F|\u2B690|\u2B691|\u2B692|\u2B693|\u2B694|\u2B695|\u2B696|\u2B697|\u2B698|\u2B699|\u2B69A|\u2B69B|\u2B69C|\u2B69D|\u2B69E|\u2B69F|\u2B6A0|\u2B6A1|\u2B6A2|\u2B6A3|\u2B6A4|\u2B6A5|\u2B6A6|\u2B6A7|\u2B6A8|\u2B6A9|\u2B6AA|\u2B6AB|\u2B6AC|\u2B6AD|\u2B6DA|\u2B6DB|\u2B6DC|\u2B6DD|\u2B6DE|\u2B6DF|\u2B6E0|\u2B6E1|\u2B6E2|\u2B6E3|\u2B6E4|\u2B6E5|\u2B6E6|\u2B6E7|\u2B6E8|\u2B6E9|\u2B6EA|\u2B6EB|\u2B6EC|\u2B6ED|\u2B6EE|\u2B6EF|\u2B6F0|\u2B6F1|\u2B6F2|\u2B6F3|\u2B6F4|\u2B6F5|\u2B6F6|\u2B6F7|\u2B6F8|\u2B6F9|\u2B6FA|\u2B6FB|\u2B6FC|\u2B6FD|\u2B6FE|\u2B700|\u2B701|\u2B702|\u2B703|\u2B704|\u2B705|\u2B70A|\u2B711|\u2B712|\u2B713|\u2B714|\u2B715|\u2B719|\u2B71F|\u2B728|\u2B729|\u2B72A|\u2B72B|\u2B72C|\u2B72D|\u2B72E|\u2B72F|\u2B730|\u2B732|\u2B733|\u2B748|\u2B74B|\u2B761|\u2B766|\u2B767|\u2B768|\u2B769|\u2B76A|\u2B76B|\u2B76C|\u2B76D|\u2B76E|\u2B775|\u2B785|\u2B797|\u2B79A|\u2B79B|\u2B79D|\u2B7A0|\u2B7A1|\u2B7A2|\u2B7A3|\u2B7A5|\u2B7A6|\u2B7A7|\u2B7A8|\u2B7A9|\u2B7B7|\u2B7C3|\u2B7C4|\u2B7C5|\u2B7C6|\u2B7C7|\u2B7D1|\u2B7D5|\u2B7DE|\u2B7DF|\u2B7E0|\u2B7E1|\u2B7E2|\u2B7E4|\u2B7E5|\u2B7E6|\u2B7EB|\u2B7EC|\u2B7F2|\u2B7F3|\u2B7F4|\u2B7F5|\u2B7F6|\u2B7F7|\u2B7F8|\u2B7F9|\u2B7FA|\u2B7FB|\u2B7FC|\u2B7FD|\u2B7FE|\u2B7FF|\u2B800|\u2B801|\u2B802|\u2B805|\u2B806|\u2B807|\u2B808|\u2B80A|\u2B80B|\u2B80C|\u2B80F|\u2B810|\u2B811|\u2B812|\u2B816|\u2B81C|\u2B86C|\u2B876|\u2B892|\u2B894|\u2B898|\u2B899|\u2B89C|\u2B89F|\u2B8A8|\u2B8AA|\u2B8AC|\u2B8AD|\u2B8B2|\u2B8B8|\u2B8B9|\u2B8BA|\u2B8C9|\u2B8CA|\u2B8DB|\u2B8EB|\u2B938|\u2B93D|\u2B94D|\u2B954|\u2B973|\u2B975|\u2B97A|\u2B97C|\u2B97D|\u2B981|\u2B985|\u2B989|\u2B98B|\u2B98C|\u2B9A9|\u2B9B0|\u2B9B3|\u2B9C3|\u2B9EE|\u2B9EF|\u2B9F7|\u2B9FF|\u2BA06|\u2BA55|\u2BA56|\u2BA5A|\u2BA5B|\u2BA64|\u2BA65|\u2BA69|\u2BA6B|\u2BA6F|\u2BA73|\u2BA78|\u2BA7A|\u2BA80|\u2BA81|\u2BA82|\u2BA83|\u2BA84|\u2BA85|\u2BA91|\u2BA98|\u2BA9A|\u2BAA7|\u2BAAA|\u2BABA|\u2BABD|\u2BAC7|\u2BACF|\u2BAE6|\u2BAF5|\u2BAFE|\u2BB10|\u2BB19|\u2BB1F|\u2BB5E|\u2BB5F|\u2BB62|\u2BB68|\u2BB6A|\u2BB6E|\u2BB6F|\u2BB72|\u2BB7C|\u2BB83|\u2BB85|\u2BB9C|\u2BBD2|\u2BBE5|\u2BBF6|\u2BC02|\u2BC0D|\u2BC1B|\u2BC20|\u2BC21|\u2BC22|\u2BC23|\u2BC28|\u2BC30|\u2BC39|\u2BC55|\u2BC7F|\u2BC97|\u2BCB8|\u2BCC3|\u2BD3C|\u2BD75|\u2BD76|\u2BD77|\u2BD78|\u2BD79|\u2BD84|\u2BD85|\u2BD87|\u2BD8A|\u2BD95|\u2BDB2|\u2BDC5|\u2BDC8|\u2BDC9|\u2BDCC|\u2BDD8|\u2BDEC|\u2BDEE|\u2BDF7|\u2BDF9|\u2BDFE|\u2BE29|\u2BE6E|\u2BE74|\u2BE77|\u2BE7C|\u2BE7D|\u2BE81|\u2BE82|\u2BE86|\u2BE8A|\u2BE8C|\u2BE92|\u2BE93|\u2BE98|\u2BEAA|\u2BEAB|\u2BEB7|\u2BEB9|\u2BEC1|\u2BEC7|\u2BF17|\u2BF1D|\u2BF23|\u2BF24|\u2BF25|\u2BF27|\u2BF2B|\u2BF2E|\u2BF31|\u2BF32|\u2BF35|\u2BF36|\u2BF3D|\u2BF3E|\u2BF40|\u2BF41|\u2BF47|\u2BF4A|\u2BF4B|\u2BF50|\u2BF54|\u2BF59|\u2BF62|\u2BF63|\u2BF65|\u2BF67|\u2BF6B|\u2BF6E|\u2BF72|\u2BF73|\u2BF81|\u2BF83|\u2BF89|\u2BF8F|\u2BFB2|\u2BFB3|\u2BFC2|\u2BFD7|\u2C025|\u2C029|\u2C02A|\u2C02E|\u2C031|\u2C051|\u2C058|\u2C073|\u2C075|\u2C078|\u2C07A|\u2C07D|\u2C080|\u2C082|\u2C085|\u2C089|\u2C0A0|\u2C0A9|\u2C0AE|\u2C0B0|\u2C0B1|\u2C0BB|\u2C0C0|\u2C0CA|\u2C0CF|\u2C0D8|\u2C0DB|\u2C0E6|\u2C0EB|\u2C0EE|\u2C0F2|\u2C0F3|\u2C11E|\u2C129|\u2C12C|\u2C162|\u2C165|\u2C16B|\u2C182|\u2C199|\u2C1A6|\u2C1AE|\u2C1BE|\u2C1C3|\u2C1C4|\u2C1C7|\u2C1D5|\u2C1D8|\u2C1D9|\u2C1EC|\u2C1F0|\u2C1F9|\u2C1FC|\u2C201|\u2C20F|\u2C215|\u2C227|\u2C231|\u2C23E|\u2C242|\u2C247|\u2C24B|\u2C260|\u2C27C|\u2C282|\u2C288|\u2C289|\u2C28D|\u2C28E|\u2C296|\u2C297|\u2C29C|\u2C2A4|\u2C2A6|\u2C2B5|\u2C2B6|\u2C2BA|\u2C2BE|\u2C2C3|\u2C2CD|\u2C31D|\u2C320|\u2C32E|\u2C334|\u2C335|\u2C337|\u2C359|\u2C35B|\u2C361|\u2C364|\u2C386|\u2C391|\u2C3A7|\u2C3AC|\u2C3DC|\u2C3DF|\u2C3E4|\u2C3E6|\u2C3EB|\u2C3EE|\u2C3F7|\u2C420|\u2C446|\u2C447|\u2C44D|\u2C44F|\u2C452|\u2C453|\u2C455|\u2C457|\u2C459|\u2C467|\u2C484|\u2C486|\u2C487|\u2C488|\u2C48A|\u2C48D|\u2C48E|\u2C493|\u2C495|\u2C497|\u2C4E0|\u2C4EB|\u2C4F1|\u2C4F8|\u2C4FC|\u2C52F|\u2C539|\u2C542|\u2C544|\u2C54A|\u2C55B|\u2C566|\u2C56C|\u2C583|\u2C591|\u2C596|\u2C598|\u2C59E|\u2C59F|\u2C5A0|\u2C5AE|\u2C5BA|\u2C613|\u2C614|\u2C615|\u2C616|\u2C617|\u2C618|\u2C619|\u2C61A|\u2C61B|\u2C61C|\u2C61D|\u2C61E|\u2C61F|\u2C620|\u2C621|\u2C622|\u2C623|\u2C624|\u2C625|\u2C626|\u2C627|\u2C628|\u2C629|\u2C62A|\u2C62B|\u2C62C|\u2C62D|\u2C62E|\u2C62F|\u2C630|\u2C631|\u2C632|\u2C633|\u2C634|\u2C635|\u2C636|\u2C637|\u2C638|\u2C639|\u2C63A|\u2C63B|\u2C63C|\u2C63D|\u2C63E|\u2C63F|\u2C640|\u2C641|\u2C642|\u2C643|\u2C644|\u2C645|\u2C646|\u2C647|\u2C648|\u2C649|\u2C64A|\u2C64B|\u2C64E|\u2C64F|\u2C65D|\u2C66A|\u2C66B|\u2C66D|\u2C684|\u2C6F9|\u2C6FC|\u2C714|\u2C725|\u2C727|\u2C728|\u2C72C|\u2C72F|\u2C738|\u2C73A|\u2C73E|\u2C73F|\u2C741|\u2C743|\u2C74A|\u2C74B|\u2C756|\u2C760|\u2C76F|\u2C774|\u2C78B|\u2C795|\u2C798|\u2C79F|\u2C7A3|\u2C7AB|\u2C7C1|\u2C7EA|\u2C7FA|\u2C7FD|\u2C803|\u2C805|\u2C808|\u2C820|\u2C831|\u2C837|\u2C847|\u2C84D|\u2C84E|\u2C852|\u2C853|\u2C854|\u2C855|\u2C860|\u2C866|\u2C871|\u2C877|\u2C87B|\u2C887|\u2C888|\u2C889|\u2C88A|\u2C88B|\u2C88C|\u2C88D|\u2C88E|\u2C88F|\u2C890|\u2C891|\u2C892|\u2C893|\u2C894|\u2C895|\u2C8AA|\u2C8AF|\u2C8B3|\u2C8C0|\u2C8D9|\u2C8DA|\u2C8DB|\u2C8DC|\u2C8DD|\u2C8DE|\u2C8DF|\u2C8E0|\u2C8E1|\u2C8E2|\u2C8E3|\u2C8E4|\u2C8E5|\u2C8E6|\u2C8E7|\u2C8E8|\u2C8E9|\u2C8EA|\u2C8EB|\u2C8EC|\u2C8ED|\u2C8EE|\u2C8EF|\u2C8F0|\u2C8F1|\u2C8F2|\u2C8F3|\u2C8F4|\u2C8F5|\u2C8F6|\u2C8F7|\u2C8F8|\u2C8F9|\u2C8FA|\u2C8FB|\u2C8FC|\u2C8FD|\u2C8FE|\u2C8FF|\u2C900|\u2C901|\u2C902|\u2C903|\u2C904|\u2C905|\u2C906|\u2C907|\u2C908|\u2C909|\u2C90A|\u2C90B|\u2C90C|\u2C90D|\u2C90E|\u2C90F|\u2C910|\u2C911|\u2C912|\u2C913|\u2C914|\u2C915|\u2C916|\u2C917|\u2C918|\u2C919|\u2C91A|\u2C91B|\u2C91C|\u2C91D|\u2C91E|\u2C91F|\u2C920|\u2C921|\u2C922|\u2C923|\u2C924|\u2C925|\u2C926|\u2C927|\u2C928|\u2C929|\u2C92A|\u2C92B|\u2C92C|\u2C92D|\u2C92E|\u2C92F|\u2C930|\u2C931|\u2C937|\u2C944|\u2C948|\u2C973|\u2C974|\u2C975|\u2C976|\u2C977|\u2C978|\u2C979|\u2C97A|\u2C97B|\u2C97C|\u2C97D|\u2C97E|\u2C97F|\u2C980|\u2C985|\u2C986|\u2C9A3|\u2C9A5|\u2C9A7|\u2C9A9|\u2C9AB|\u2C9AF|\u2C9B4|\u2C9B5|\u2C9B9|\u2C9BB|\u2C9BE|\u2C9C0|\u2C9C3|\u2C9D1|\u2C9D4|\u2C9DA|\u2C9DB|\u2C9E2|\u2C9E4|\u2C9E9|\u2CA01|\u2CA02|\u2CA03|\u2CA04|\u2CA05|\u2CA06|\u2CA07|\u2CA08|\u2CA09|\u2CA0B|\u2CA0C|\u2CA0D|\u2CA0E|\u2CA0F|\u2CA10|\u2CA11|\u2CA12|\u2CA13|\u2CA14|\u2CA4E|\u2CA7D|\u2CA7E|\u2CA8D|\u2CAA7|\u2CAA8|\u2CAA9|\u2CAAB|\u2CAAF|\u2CABA|\u2CB07|\u2CB27|\u2CB28|\u2CB29|\u2CB2A|\u2CB2B|\u2CB2C|\u2CB2D|\u2CB2E|\u2CB2F|\u2CB30|\u2CB31|\u2CB32|\u2CB33|\u2CB34|\u2CB35|\u2CB36|\u2CB37|\u2CB38|\u2CB39|\u2CB3A|\u2CB3B|\u2CB3C|\u2CB3D|\u2CB3E|\u2CB3F|\u2CB40|\u2CB41|\u2CB42|\u2CB43|\u2CB45|\u2CB46|\u2CB47|\u2CB48|\u2CB49|\u2CB4A|\u2CB4B|\u2CB4C|\u2CB4D|\u2CB4E|\u2CB4F|\u2CB50|\u2CB51|\u2CB53|\u2CB54|\u2CB55|\u2CB56|\u2CB57|\u2CB58|\u2CB59|\u2CB5A|\u2CB5B|\u2CB5C|\u2CB5D|\u2CB5E|\u2CB5F|\u2CB60|\u2CB61|\u2CB62|\u2CB63|\u2CB64|\u2CB65|\u2CB66|\u2CB68|\u2CB69|\u2CB6A|\u2CB6B|\u2CB6C|\u2CB6D|\u2CB6F|\u2CB70|\u2CB71|\u2CB72|\u2CB73|\u2CB74|\u2CB75|\u2CB76|\u2CB77|\u2CB78|\u2CB79|\u2CB7A|\u2CB7B|\u2CB7C|\u2CB7D|\u2CB7E|\u2CB7F|\u2CB80|\u2CB81|\u2CB82|\u2CB83|\u2CB84|\u2CB98|\u2CB99|\u2CB9C|\u2CB9D|\u2CB9F|\u2CBA0|\u2CBA1|\u2CBA2|\u2CBA3|\u2CBA4|\u2CBA5|\u2CBA7|\u2CBA8|\u2CBA9|\u2CBAA|\u2CBAC|\u2CBAD|\u2CBAE|\u2CBAF|\u2CBB0|\u2CBB1|\u2CBB2|\u2CBB3|\u2CBB4|\u2CBB5|\u2CBB8|\u2CBB9|\u2CBBA|\u2CBBB|\u2CBBF|\u2CBC0|\u2CBC5|\u2CBCA|\u2CBCE|\u2CC21|\u2CC23|\u2CC24|\u2CC25|\u2CC31|\u2CC32|\u2CC33|\u2CC34|\u2CC35|\u2CC36|\u2CC37|\u2CC38|\u2CC3A|\u2CC53|\u2CC54|\u2CC55|\u2CC56|\u2CC57|\u2CC58|\u2CC59|\u2CC5A|\u2CC5B|\u2CC5C|\u2CC5D|\u2CC5E|\u2CC5F|\u2CC60|\u2CC61|\u2CC62|\u2CC63|\u2CC64|\u2CC65|\u2CC66|\u2CC67|\u2CC68|\u2CC69|\u2CC6A|\u2CC6B|\u2CC6C|\u2CC6D|\u2CC6E|\u2CC6F|\u2CC70|\u2CC71|\u2CC72|\u2CC73|\u2CC75|\u2CC77|\u2CC78|\u2CC7A|\u2CC7C|\u2CC7D|\u2CC7E|\u2CC7F|\u2CC80|\u2CC85|\u2CC86|\u2CC95|\u2CCA5|\u2CCA6|\u2CCA7|\u2CCA8|\u2CCA9|\u2CCAA|\u2CCAB|\u2CCAC|\u2CCAD|\u2CCAE|\u2CCAF|\u2CCB0|\u2CCB1|\u2CCB2|\u2CCB3|\u2CCB4|\u2CCB5|\u2CCB6|\u2CCB7|\u2CCB8|\u2CCB9|\u2CCBA|\u2CCBB|\u2CCBC|\u2CCBD|\u2CCBE|\u2CCBF|\u2CCC0|\u2CCC1|\u2CCC2|\u2CCC3|\u2CCC4|\u2CCC5|\u2CCC6|\u2CCC7|\u2CCC8|\u2CCC9|\u2CCCA|\u2CCCB|\u2CCCC|\u2CCCD|\u2CCCE|\u2CCD0|\u2CCD1|\u2CCD2|\u2CCD3|\u2CCD4|\u2CCD9|\u2CCDF|\u2CCF3|\u2CCF4|\u2CCF5|\u2CCF6|\u2CCF7|\u2CCF8|\u2CCF9|\u2CCFA|\u2CCFB|\u2CCFC|\u2CCFD|\u2CCFE|\u2CCFF|\u2CD00|\u2CD01|\u2CD02|\u2CD03|\u2CD04|\u2CD05|\u2CD06|\u2CD07|\u2CD08|\u2CD09|\u2CD0A|\u2CD0B|\u2CD0C|\u2CD0D|\u2CD0E|\u2CD0F|\u2CD10|\u2CD28|\u2CD29|\u2CD80|\u2CD81|\u2CD82|\u2CD83|\u2CD84|\u2CD85|\u2CD86|\u2CD87|\u2CD88|\u2CD89|\u2CD8A|\u2CD8B|\u2CD8C|\u2CD8D|\u2CD8E|\u2CD8F|\u2CD90|\u2CD91|\u2CD92|\u2CD93|\u2CD94|\u2CD95|\u2CD96|\u2CD97|\u2CD99|\u2CD9A|\u2CD9B|\u2CD9C|\u2CD9D|\u2CD9E|\u2CD9F|\u2CDA0|\u2CDA1|\u2CDA2|\u2CDA3|\u2CDA4|\u2CDA5|\u2CDA6|\u2CDA7|\u2CDA8|\u2CDA9|\u2CDAA|\u2CDAB|\u2CDAC|\u2CDAD|\u2CDAE|\u2CDAF|\u2CDB0|\u2CDB1|\u2CDB2|\u2CDB3|\u2CDB4|\u2CDB5|\u2CDB6|\u2CDB7|\u2CDB8|\u2CDB9|\u2CDBA|\u2CDBB|\u2CDD5|\u2CDFB|\u2CDFC|\u2CDFD|\u2CDFE|\u2CDFF|\u2CE00|\u2CE01|\u2CE02|\u2CE03|\u2CE04|\u2CE05|\u2CE06|\u2CE08|\u2CE09|\u2CE0A|\u2CE0B|\u2CE0C|\u2CE0D|\u2CE0E|\u2CE0F|\u2CE10|\u2CE11|\u2CE12|\u2CE13|\u2CE14|\u2CE15|\u2CE16|\u2CE17|\u2CE18|\u2CE19|\u2CE1A|\u2CE1B|\u2CE1C|\u2CE1D|\u2CE1E|\u2CE1F|\u2CE20|\u2CE21|\u2CE22|\u2CE23|\u2CE24|\u2CE25|\u2CE26|\u2CE27|\u2CE28|\u2CE29|\u2CE2A|\u2CE2B|\u2CE2C|\u2CE2D|\u2CE2E|\u2CE2F|\u2CE30|\u2CE31|\u2CE35|\u2CE36|\u2CE37|\u2CE38|\u2CE39|\u2CE3E|\u2CE45|\u2CE46|\u2CE47|\u2CE48|\u2CE49|\u2CE4A|\u2CE4B|\u2CE4C|\u2CE4D|\u2CE4E|\u2CE55|\u2CE56|\u2CE57|\u2CE58|\u2CE63|\u2CE64|\u2CE6D|\u2CE7A|\u2CE7B|\u2CE7C|\u2CE7D|\u2CE7E|\u2CE7F|\u2CE80|\u2CE81|\u2CE82|\u2CE83|\u2CE84|\u2CE85|\u2CE86|\u2CE87|\u2CE88|\u2CE89|\u2CE8A|\u2CE8B|\u2CE8C|\u2CE8D|\u2CE8E|\u2CE8F|\u2CE90|\u2CE91|\u2CE92|\u2CE93|\u2CE94|\u2CE95|\u2CE96|\u2CE9B|\u2CE9C|\u2CE9D|\u2CE9F|\u2CEEE|\u2CF96|\u2CFA3|\u2D11B|\u2D1C0|\u2D1C9|\u2D1D9|\u2D1DC|\u2D1E1|\u2D1EF|\u2D1F4|\u2D208|\u2D209|\u2D21C|\u2D21F|\u2D22E|\u2D257|\u2D268|\u2D27C|\u2D2B8|\u2D382|\u2D39C|\u2D3E6|\u2D3F8|\u2D478|\u2D479|\u2D4C0|\u2D546|\u2D613|\u2D61A|\u2D6A6|\u2D74B|\u2D76B|\u2D784|\u2D819|\u2D83D|\u2D846|\u2D85C|\u2D875|\u2D88B|\u2D895|\u2D89D|\u2D8C7|\u2D8E7|\u2D90E|\u2D930|\u2D953|\u2D9CB|\u2DA5A|\u2DA5B|\u2DA70|\u2DA86|\u2DAC0|\u2DAD9|\u2DADD|\u2DB48|\u2DC0E|\u2DC12|\u2DC17|\u2DC25|\u2DC40|\u2DC4A|\u2DCAB|\u2DD0A|\u2DD33|\u2DE5C|\u2DECD|\u2DED4|\u2E021|\u2E024|\u2E02A|\u2E032|\u2E14E|\u2E18F|\u2E1D4|\u2E1E4|\u2E260|\u2E261|\u2E262|\u2E263|\u2E264|\u2E265|\u2E267|\u2E268|\u2E269|\u2E26A|\u2E26B|\u2E26C|\u2E26D|\u2E26E|\u2E26F|\u2E30C|\u2E38D|\u2E3C0|\u2E3FA|\u2E41A|\u2E428|\u2E502|\u2E505|\u2E50A|\u2E581|\u2E583|\u2E5B1|\u2E64A|\u2E64B|\u2E6D7|\u2E736|\u2E774|\u2E775|\u2E777|\u2E778|\u2E779|\u2E77A|\u2E81E|\u2E833|\u2E8F2|\u2E8F3|\u2E8F4|\u2E8F5|\u2E8F6|\u2E92B|\u2E92C|\u2E92D|\u2E92E|\u2E92F|\u2E932|\u2E933|\u2E936|\u2E937|\u2E938|\u2E985|\u2E99A|\u2E9F4|\u2E9F5|\u2EA34|\u2EA35|\u2EA5B|\u2EA5C|\u2EA5D|\u2EA5E|\u2EAA1|\u2EAA2|\u2EAA3|\u2EAA4|\u2EAA5|\u2EAC2|\u2EB1B|\u2EB1C|\u2EB1D|\u2EB1E|\u2EB1F|\u2EB20|\u2EB21|\u2EB22|\u2EB23|\u2EB24|\u2EB61|\u2EB62|\u2EB64|\u2EB65|\u2EB66|\u2EB68|\u2EB6A|\u2EB70|\u2EB85|\u2EB87|\u2EBD9|\u30048|\u30067|\u30078|\u3007E|\u30081|\u30083|\u3008B|\u3008E|\u3008F|\u30097|\u3009C|\u300A6|\u300AD|\u300BB|\u300C6|\u300F3|\u300F6|\u300F7|\u300FB|\u300FF|\u30101|\u3011E|\u3012D|\u30154|\u30165|\u30166|\u3017B|\u30195|\u30199|\u3019A|\u301C0|\u301CA|\u301CE|\u301D5|\u301D6|\u301D8|\u301E0|\u301E1|\u301E3|\u301E5|\u301F2|\u301FC|\u30206|\u30207|\u3020A|\u3020D|\u30213|\u3022E|\u3022F|\u30236|\u30241|\u30244|\u30258|\u30259|\u3025A|\u30263|\u30265|\u30269|\u3026A|\u30271|\u3027D|\u30282|\u30285|\u30288|\u30291|\u3029B|\u3029F|\u302A1|\u302A2|\u302D6|\u302F8|\u302F9|\u302FD|\u302FE|\u30300|\u30302|\u30306|\u30307|\u30309|\u30319|\u30326|\u30337|\u3038C|\u3038E|\u3038F|\u30390|\u30391|\u30394|\u30396|\u3039B|\u3039D|\u3039E|\u303A0|\u303A2|\u303A6|\u303AB|\u303B4|\u303B7|\u303B9|\u303C1|\u303D3|\u303D5|\u303DC|\u303DF|\u303F2|\u303F6|\u303FC|\u303FD|\u3041A|\u3043E|\u30441|\u30444|\u30445|\u30454|\u30455|\u30459|\u3045F|\u30465|\u30467|\u3046A|\u3046B|\u3046C|\u30475|\u30478|\u3047F|\u30486|\u30492|\u30496|\u304C4|\u304C6|\u304D4|\u304D5|\u304D7|\u304D9|\u304DC|\u304DD|\u304DF|\u304E4|\u304E7|\u304EC|\u304F1|\u304F7|\u304FB|\u304FC|\u30507|\u3050B|\u30532|\u30536|\u30541|\u30545|\u30548|\u30550|\u3056D|\u30588|\u3058F|\u3059A|\u305A0|\u305A9|\u305C5|\u305C6|\u305D3|\u305D6|\u305D8|\u305D9|\u305DA|\u305DB|\u305DC|\u305E1|\u305E2|\u305E6|\u305E8|\u305EC|\u305F5|\u305F9|\u305FA|\u30600|\u30605|\u30608|\u30613|\u30620|\u30623|\u30629|\u30633|\u30636|\u30638|\u3064B|\u3064E|\u30651|\u30655|\u3068D|\u30694|\u306A6|\u306AA|\u306AC|\u306B1|\u306C9|\u306CA|\u306CF|\u306D2|\u306DB|\u306E1|\u306E3|\u306E4|\u306E5|\u306E6|\u306E8|\u306E9|\u306EA|\u306EE|\u306F1|\u306F2|\u306F5|\u306FA|\u306FB|\u306FD|\u30710|\u3071C|\u3071D|\u30722|\u30728|\u30733|\u30745|\u3074B|\u3074D|\u30757|\u3075C|\u3075E|\u3075F|\u30764|\u3077E|\u30787|\u30789|\u3078D|\u307A4|\u307B2|\u307B3|\u307B7|\u307BB|\u307C4|\u307D8|\u3081B|\u3082B|\u30832|\u30834|\u30839|\u30844|\u30849|\u3084A|\u3084B|\u3084E|\u3084F|\u30850|\u30854|\u3085E|\u30862|\u30869|\u30870|\u30875|\u3087B|\u3087D|\u30884|\u308A2|\u308A4|\u308A6|\u308E2|\u308E6|\u308E9|\u308EB|\u308EC|\u308EF|\u308F6|\u308FC|\u308FD|\u30913|\u30915|\u30928|\u3092B|\u3092C|\u3093D|\u3094A|\u30952|\u3095B|\u3095E|\u30960|\u30962|\u30963|\u30968|\u3096A|\u3096D|\u30979|\u30994|\u3099C|\u309A6|\u309A8|\u309AD|\u309B0|\u309B4|\u309B7|\u309BE|\u309BF|\u309C3|\u309C7|\u309C8|\u309C9|\u309CE|\u309D4|\u309D8|\u309F0|\u309FB|\u309FE|\u30A16|\u30A1C|\u30A26|\u30A33|\u30A45|\u30A4F|\u30A53|\u30A67|\u30A6E|\u30A72|\u30A78|\u30A79|\u30A7A|\u30A7B|\u30A8A|\u30A8F|\u30AA3|\u30AAA|\u30AAB|\u30AAD|\u30AB6|\u30ABC|\u30ABF|\u30ACB|\u30AD6|\u30AFC|\u30AFD|\u30AFF|\u30B00|\u30B01|\u30B02|\u30B03|\u30B05|\u30B06|\u30B07|\u30B08|\u30B09|\u30B0B|\u30B0C|\u30B0D|\u30B0E|\u30B0F|\u30B10|\u30B11|\u30B12|\u30B13|\u30B14|\u30B16|\u30B17|\u30B18|\u30B19|\u30B1A|\u30B1B|\u30B1C|\u30B1D|\u30B1E|\u30B1F|\u30B20|\u30B21|\u30B22|\u30B23|\u30B24|\u30B25|\u30B26|\u30B27|\u30B28|\u30B29|\u30B2A|\u30B2B|\u30B2C|\u30B2D|\u30B2E|\u30B2F|\u30B30|\u30B31|\u30B32|\u30B33|\u30B34|\u30B35|\u30B36|\u30B37|\u30B38|\u30B39|\u30B3A|\u30B3B|\u30B3C|\u30B3D|\u30B3E|\u30B3F|\u30B40|\u30B41|\u30B44|\u30B54|\u30B57|\u30B5A|\u30B62|\u30B63|\u30B79|\u30B85|\u30B87|\u30B99|\u30B9D|\u30BAD|\u30BB2|\u30BC2|\u30BCB|\u30BCE|\u30C06|\u30C0B|\u30C0C|\u30C0F|\u30C11|\u30C20|\u30C22|\u30C24|\u30C28|\u30C2E|\u30C31|\u30C33|\u30C34|\u30C35|\u30C36|\u30C37|\u30C39|\u30C3A|\u30C3E|\u30C3F|\u30C40|\u30C47|\u30C48|\u30C49|\u30C4A|\u30C4C|\u30C4D|\u30C50|\u30C51|\u30C5B|\u30C5D|\u30C5F|\u30C66|\u30C69|\u30C6E|\u30C6F|\u30C71|\u30C72|\u30C7E|\u30C81|\u30C82|\u30C92|\u30C96|\u30C9F|\u30CA0|\u30CAB|\u30CAC|\u30CAE|\u30CAF|\u30CB0|\u30CB2|\u30CB3|\u30CB4|\u30CB5|\u30CB6|\u30CB8|\u30CB9|\u30CBA|\u30CBB|\u30CC1|\u30CC2|\u30CC4|\u30CC6|\u30CCA|\u30CD7|\u30CDA|\u30CF2|\u30CF5|\u30CF8|\u30CF9|\u30CFA|\u30CFB|\u30CFC|\u30D02|\u30D15|\u30D16|\u30D17|\u30D18|\u30D19|\u30D1A|\u30D1B|\u30D1C|\u30D1D|\u30D1E|\u30D22|\u30D23|\u30D24|\u30D25|\u30D2F|\u30D4A|\u30D4C|\u30D4D|\u30D4E|\u30D4F|\u30D50|\u30D51|\u30D52|\u30D53|\u30D54|\u30D56|\u30D57|\u30D58|\u30D59|\u30D5A|\u30D5B|\u30D5C|\u30D5D|\u30D5E|\u30D60|\u30D61|\u30D62|\u30D63|\u30D64|\u30D65|\u30D66|\u30D67|\u30D68|\u30D69|\u30D6A|\u30D6B|\u30D6C|\u30D6D|\u30D6E|\u30D6F|\u30D70|\u30D71|\u30D72|\u30D73|\u30D74|\u30D75|\u30D76|\u30D77|\u30D78|\u30D79|\u30D7A|\u30D7B|\u30D7C|\u30D7D|\u30D7E|\u30D7F|\u30D80|\u30D81|\u30D82|\u30D83|\u30D84|\u30D85|\u30D86|\u30D87|\u30D88|\u30D89|\u30D8A|\u30D8B|\u30D8C|\u30D8D|\u30D8E|\u30D8F|\u30D91|\u30D94|\u30DA8|\u30DAC|\u30DDE|\u30DE0|\u30DE1|\u30DE2|\u30DE4|\u30DE5|\u30DE6|\u30DE7|\u30DE8|\u30DE9|\u30DEA|\u30DEB|\u30DEC|\u30DED|\u30DEE|\u30DF4|\u30DF5|\u30DF6|\u30DF8|\u30E07|\u30E08|\u30E0A|\u30E0E|\u30E10|\u30E14|\u30E1A|\u30E1B|\u30E1E|\u30E26|\u30E40|\u30E6F|\u30E71|\u30E72|\u30E73|\u30E74|\u30E75|\u30E76|\u30E77|\u30E78|\u30E7A|\u30E7B|\u30E7C|\u30E7D|\u30E7E|\u30E7F|\u30E80|\u30E81|\u30E82|\u30E83|\u30E84|\u30E85|\u30E86|\u30E87|\u30E88|\u30E89|\u30E8A|\u30E8B|\u30E8C|\u30E8D|\u30E8E|\u30E8F|\u30E90|\u30E91|\u30E92|\u30E93|\u30E94|\u30E95|\u30E96|\u30E97|\u30E98|\u30E99|\u30E9A|\u30E9B|\u30E9C|\u30E9D|\u30E9E|\u30E9F|\u30EA0|\u30EA1|\u30EA2|\u30EA3|\u30EA4|\u30EA8|\u30EAD|\u30EB2|\u30EB7|\u30EC6|\u30EDD|\u30EE1|\u30EE6|\u30EE8|\u30EEE|\u30EF3|\u30F05|\u30F0B|\u30F0F|\u30F11|\u30F3B|\u30F55|\u30F56|\u30F57|\u30F58|\u30F5A|\u30F5B|\u30F5C|\u30F5D|\u30F5E|\u30F60|\u30F61|\u30F62|\u30F63|\u30F65|\u30F66|\u30F67|\u30F68|\u30F69|\u30F6B|\u30F6C|\u30F6D|\u30F6E|\u30F6F|\u30F70|\u30F71|\u30F72|\u30F73|\u30F74|\u30F75|\u30F76|\u30F77|\u30F78|\u30F79|\u30F7A|\u30F7B|\u30F7C|\u30F7D|\u30F7E|\u30F7F|\u30F80|\u30F81|\u30F83|\u30F84|\u30F85|\u30F86|\u30F87|\u30F88|\u30F89|\u30F8B|\u30F8C|\u30F8D|\u30F8E|\u30F8F|\u30F90|\u30F91|\u30F92|\u30F93|\u30F95|\u30F96|\u30F97|\u30F98|\u30F99|\u30F9A|\u30F9B|\u30F9C|\u30F9D|\u30F9E|\u30F9F|\u30FA1|\u30FA2|\u30FA3|\u30FA4|\u30FA5|\u30FA6|\u30FA7|\u30FA8|\u30FA9|\u30FAA|\u30FAB|\u30FAC|\u30FAD|\u30FAE|\u30FAF|\u30FB0|\u30FB1|\u30FB2|\u30FB3|\u30FB4|\u30FB5|\u30FB6|\u30FB7|\u30FB8|\u30FB9|\u30FBA|\u30FBB|\u30FBC|\u30FBD|\u30FBE|\u30FBF|\u30FC0|\u30FC1|\u30FC2|\u30FC3|\u30FC4|\u30FC5|\u30FC6|\u30FC7|\u30FC8|\u30FC9|\u30FCA|\u30FD6|\u30FE5|\u30FE6|\u30FE7|\u30FE8|\u30FE9|\u30FEA|\u30FEB|\u30FEC|\u30FED|\u30FEF|\u30FF0|\u30FF3|\u30FF4|\u30FF5|\u30FF8|\u30FF9|\u30FFA|\u30FFB|\u30FFE|\u31011|\u31021|\u31052|\u3105E|\u31071|\u31073|\u31074|\u31076|\u31077|\u31079|\u3107A|\u3107D|\u3107E|\u31083|\u31084|\u31085|\u31086|\u31087|\u31088|\u31089|\u3108A|\u3108B|\u3108C|\u3108D|\u3108E|\u31090|\u310A0|\u310A1|\u310A2|\u310A3|\u310A4|\u310A5|\u310A6|\u310A7|\u310A8|\u310A9|\u310AB|\u310AC|\u310AD|\u310AE|\u310AF|\u310B0|\u310B1|\u310B2|\u310B3|\u310B4|\u310B5|\u310B6|\u310B7|\u310B8|\u310BA|\u310BB|\u310D4|\u310D5|\u310D6|\u310D7|\u310D8|\u310D9|\u310DA|\u310DB|\u310DC|\u310DD|\u310DE|\u310DF|\u310E0|\u310F1|\u310F2|\u310F3|\u310F4|\u310F5|\u310F7|\u310F8|\u310F9|\u310FA|\u310FC|\u310FD|\u310FE|\u310FF|\u31100|\u31101|\u31102|\u31103|\u31104|\u31105|\u31106|\u31107|\u31108|\u31109|\u3110A|\u3113C|\u3113D|\u3113E|\u3113F|\u31140|\u31141|\u31142|\u31143|\u31144|\u31145|\u31147|\u31148|\u31149|\u3114A|\u3114B|\u3114D|\u3114E|\u3114F|\u31150|\u31152|\u31153|\u31154|\u31155|\u31156|\u31157|\u31158|\u31159|\u3115A|\u3115B|\u3115C|\u3115D|\u3115E|\u3115F|\u31160|\u31161|\u31162|\u31163|\u31164|\u31165|\u31166|\u31167|\u31168|\u31169|\u3116A|\u3116B|\u3116C|\u3116E|\u31180|\u31181|\u31183|\u31184|\u31185|\u31186|\u31188|\u3118C|\u3118D|\u31196|\u31199|\u3119A|\u3119B|\u311CD|\u311CE|\u311CF|\u311D0|\u311D1|\u311D2|\u311D3|\u311D4|\u311D5|\u311D6|\u311D7|\u311D8|\u311D9|\u311DA|\u311DB|\u311DC|\u311DD|\u311DE|\u311DF|\u311E0|\u311E1|\u311E2|\u311E3|\u311E4|\u311E5|\u311E6|\u311E7|\u311E8|\u311E9|\u311EA|\u311EB|\u311EC|\u311ED|\u311EE|\u311EF|\u311F0|\u311F1|\u311F2|\u311F3|\u311F4|\u311F5|\u311F6|\u311F7|\u311F8|\u311F9|\u311FA|\u311FB|\u311FC|\u311FD|\u311FE|\u311FF|\u31200|\u31201|\u31202|\u31203|\u31204|\u31205|\u31206|\u31207|\u31208|\u31209|\u3120A|\u3120B|\u3120C|\u3120D|\u3120E|\u3120F|\u31210|\u31211|\u31212|\u31213|\u31214|\u31215|\u31216|\u31217|\u31218|\u31219|\u3121A|\u3121B|\u3121C|\u31247|\u31248|\u31249|\u3124A|\u3124B|\u3124C|\u3124D|\u3124E|\u3124F|\u31250|\u31251|\u31252|\u31253|\u31254|\u31255|\u31256|\u31257|\u31258|\u31259|\u3125A|\u3125B|\u3125C|\u3125D|\u3125E|\u3125F|\u31260|\u31261|\u31262|\u31263|\u31264|\u31265|\u31266|\u31267|\u31268|\u31269|\u3126A|\u3126B|\u3126C|\u3126D|\u3126E|\u3126F|\u31270|\u31271|\u31272|\u31273|\u31274|\u31275|\u31276|\u31277|\u31278|\u31279|\u3127A|\u3127B|\u3127C|\u3127D|\u3127E|\u3127F|\u31280|\u31281|\u31282|\u31283|\u31284|\u31285|\u31286|\u31287|\u31288|\u31289|\u3128A|\u3128B|\u3128C|\u3128D|\u3128E|\u3128F|\u31290|\u31291|\u31292|\u31293|\u31294|\u31295|\u31296|\u31297|\u31298|\u31299|\u3129A|\u3129B|\u3129C|\u3129D|\u3129E|\u3129F|\u312A0|\u312A1|\u312A2|\u312A3|\u312A4|\u312A5|\u312A6|\u312A7|\u312A8|\u312A9|\u312AA|\u312AB|\u312AC|\u312AD|\u312AE|\u312AF|\u312B0|\u312B1|\u312B2|\u312B3|\u312B4|\u312B5|\u312BA|\u312BB|\u312BC|\u312BD|\u312C2|\u312C4|\u312C5|\u312C6|\u312C7|\u312C8|\u312C9|\u312CA|\u312CB|\u312CC|\u312CD|\u312CE|\u312D0|\u312D1|\u312D3|\u312D4|\u312D5|\u312D6|\u312D7|\u312D8|\u312D9|\u312DA|\u312DC|\u312DD|\u312DF|\u312E0|\u312E1|\u312E2|\u312E3|\u312E4|\u312E5|\u312E6|\u312E8|\u312EA|\u312EB|\u312EC|\u312ED|\u312EE|\u312F1|\u312F4|\u312F6|\u312FE|\u312FF|\u31300|\u31301|\u31303|\u31304|\u31305|\u31306|\u31307|\u31308|\u31309|\u3130A|\u3130F|\u31315|\u31316|\u31317|\u31318|\u31319|\u3132B|\u3132C|\u3132D|\u3132E|\u3132F|\u31330|\u31331|\u31332|\u31333|\u31334|\u31335|\u31336|\u31337|\u31338|\u31339|\u3133A|\u3133C|\u3133D|\u31341|\u31342|\u31344|\u31345|\u31346|\u31347|\u31348|\u31349";
            //var TraditionalChinese = "\u346E|\u346F|\u3473|\u3476|\u3493|\u349C|\u34A3|\u34BF|\u34C4|\u34D6|\u34E8|\u3503|\u3505|\u350B|\u351D|\u3522|\u3552|\u3562|\u35A6|\u35AE|\u35D9|\u35E2|\u35E3|\u35F0|\u35F2|\u35F6|\u35FB|\u35FC|\u35FF|\u3613|\u3614|\u3616|\u3619|\u361A|\u3624|\u3654|\u3661|\u3662|\u366C|\u367A|\u367E|\u36DD|\u3704|\u370F|\u3710|\u3717|\u371E|\u3722|\u3725|\u372D|\u372E|\u3737|\u373A|\u375E|\u375F|\u379E|\u37FA|\u3801|\u380F|\u3820|\u3823|\u3853|\u385E|\u3897|\u389D|\u3932|\u396E|\u3977|\u398A|\u398E|\u3996|\u399B|\u399E|\u39A6|\u39AC|\u39AD|\u3A1B|\u3A1F|\u3A25|\u3A3B|\u3A47|\u3A4B|\u3A4C|\u3A5C|\u3A63|\u3A6D|\u3A73|\u3A75|\u3A77|\u3A79|\u3A8E|\u3AB9|\u3B23|\u3B2E|\u3B93|\u3B9D|\u3BB2|\u3BC2|\u3BC6|\u3BE4|\u3BF8|\u3BFC|\u3C02|\u3C05|\u3C0D|\u3C30|\u3C33|\u3CAF|\u3CB0|\u3CB2|\u3D38|\u3D3F|\u3D4D|\u3D51|\u3D52|\u3D57|\u3D64|\u3D7E|\u3D86|\u3D8C|\u3D8D|\u3D8F|\u3D92|\u3D95|\u3DC3|\u3DCD|\u3DF2|\u3DF6|\u3DFB|\u3DFF|\u3E05|\u3E0A|\u3E10|\u3E53|\u3E7D|\u3E8F|\u3E91|\u3E9C|\u3EF6|\u3EFD|\u3F06|\u3F08|\u3F3B|\u3FB5|\u3FBA|\u3FC9|\u3FCE|\u3FD6|\u3FD7|\u3FE7|\u3FF9|\u4009|\u400D|\u4034|\u4039|\u405D|\u406A|\u4071|\u407B|\u408E|\u4093|\u40C1|\u40D5|\u40D8|\u40E2|\u40E3|\u40E4|\u40EE|\u40F4|\u4150|\u4158|\u4173|\u4185|\u4189|\u41D3|\u424D|\u4250|\u4251|\u4259|\u426C|\u4271|\u4272|\u4276|\u429C|\u429F|\u42AD|\u42B2|\u42B5|\u42B7|\u42BA|\u42C3|\u42C6|\u42CD|\u42CE|\u42CF|\u42D0|\u42D1|\u42D4|\u42D9|\u42DA|\u42E6|\u42EB|\u42F9|\u42FA|\u42FB|\u42FC|\u42FD|\u42FE|\u42FF|\u4301|\u4307|\u4308|\u430B|\u430C|\u4310|\u4316|\u431D|\u431E|\u431F|\u4325|\u432A|\u4330|\u4364|\u4377|\u437D|\u4398|\u4399|\u43B1|\u43CA|\u4422|\u4423|\u4437|\u4439|\u443D|\u4457|\u447C|\u44E3|\u4507|\u4508|\u4521|\u4561|\u4564|\u4573|\u4579|\u457C|\u4580|\u4585|\u459A|\u45C3|\u45C5|\u45E5|\u45FB|\u45FD|\u45FF|\u4654|\u4661|\u4671|\u467C|\u4686|\u4689|\u4695|\u469E|\u46A9|\u46B3|\u46B5|\u46BD|\u46C0|\u46C4|\u46CC|\u46CD|\u46D8|\u46DB|\u46DE|\u46E0|\u46E4|\u46EC|\u46ED|\u46F3|\u46FD|\u46FF|\u4700|\u4704|\u4709|\u470B|\u470D|\u470E|\u470F|\u4712|\u4716|\u471A|\u471D|\u474F|\u4755|\u476D|\u476F|\u477B|\u477C|\u4780|\u4781|\u4782|\u4788|\u4789|\u478B|\u4793|\u47B6|\u47C3|\u47C6|\u47CF|\u47D0|\u47FA|\u4806|\u481F|\u4820|\u4829|\u482E|\u4831|\u4841|\u4845|\u4847|\u484A|\u4850|\u4857|\u4858|\u485D|\u485F|\u4866|\u4869|\u4870|\u4874|\u4875|\u4876|\u4877|\u487B|\u487E|\u4888|\u48A8|\u490C|\u490D|\u4920|\u4924|\u4925|\u4928|\u4929|\u492A|\u492C|\u4935|\u4938|\u493B|\u493C|\u4944|\u4947|\u4951|\u4955|\u4956|\u4957|\u495B|\u495D|\u495E|\u4969|\u496F|\u4971|\u4974|\u4976|\u4977|\u4978|\u498C|\u498E|\u4998|\u499B|\u499D|\u499F|\u49AA|\u49AF|\u49B1|\u49B3|\u49DE|\u49E2|\u4A34|\u4A6B|\u4A8A|\u4A8D|\u4A8F|\u4A90|\u4A93|\u4A97|\u4A98|\u4A9C|\u4A9D|\u4AA5|\u4AB4|\u4ABC|\u4ABE|\u4AC0|\u4AC2|\u4AC8|\u4AC9|\u4ACC|\u4ACF|\u4AD0|\u4ADC|\u4ADF|\u4AE0|\u4AE5|\u4AE9|\u4AF4|\u4AF6|\u4AFB|\u4AFC|\u4AFE|\u4B00|\u4B02|\u4B05|\u4B0D|\u4B0E|\u4B10|\u4B13|\u4B14|\u4B18|\u4B1D|\u4B1E|\u4B1F|\u4B23|\u4B27|\u4B2A|\u4B2B|\u4B2C|\u4B2F|\u4B32|\u4B33|\u4B36|\u4B39|\u4B3E|\u4B40|\u4B43|\u4B45|\u4B47|\u4B48|\u4B49|\u4B51|\u4B52|\u4B53|\u4B54|\u4B55|\u4B58|\u4B5E|\u4B61|\u4B62|\u4B63|\u4B6D|\u4B7F|\u4B82|\u4B84|\u4B88|\u4B97|\u4B9D|\u4B9E|\u4BA0|\u4BA7|\u4BAB|\u4BB0|\u4BB2|\u4BB3|\u4BB8|\u4BBD|\u4BBE|\u4BBF|\u4BC0|\u4BE4|\u4C0E|\u4C10|\u4C16|\u4C2B|\u4C32|\u4C37|\u4C3B|\u4C3D|\u4C3E|\u4C40|\u4C41|\u4C42|\u4C45|\u4C47|\u4C4C|\u4C4D|\u4C4E|\u4C50|\u4C52|\u4C53|\u4C57|\u4C59|\u4C5A|\u4C5B|\u4C5C|\u4C5F|\u4C61|\u4C64|\u4C65|\u4C67|\u4C6C|\u4C6D|\u4C70|\u4C71|\u4C74|\u4C75|\u4C77|\u4C78|\u4C79|\u4C7B|\u4C7D|\u4C7E|\u4C81|\u4C85|\u4C89|\u4C8F|\u4C95|\u4C96|\u4C97|\u4C98|\u4C99|\u4C9A|\u4C9B|\u4CA8|\u4CB0|\u4CB8|\u4CB9|\u4CBC|\u4CC5|\u4CC7|\u4CCD|\u4CCF|\u4CD2|\u4CD3|\u4CD5|\u4CDA|\u4CDC|\u4CDF|\u4CE2|\u4CE4|\u4CE7|\u4CE8|\u4CEB|\u4CED|\u4CEE|\u4CF2|\u4CFA|\u4D07|\u4D08|\u4D09|\u4D0B|\u4D1A|\u4D1D|\u4D2C|\u4D2D|\u4D2E|\u4D31|\u4D32|\u4D33|\u4D34|\u4D35|\u4D37|\u4D38|\u4D39|\u4D3A|\u4D3D|\u4D42|\u4D43|\u4D46|\u4D50|\u4D58|\u4D73|\u4D74|\u4D76|\u4D77|\u4D95|\u4D97|\u4DA2|\u4DA3|\u4DA6|\u4DA7|\u4DA8|\u4DAA|\u4DB1|\u4DB2|\u4E1F|\u4E26|\u4E7E|\u4E82|\u4E9E|\u4F47|\u4F75|\u4F86|\u4F96|\u4FB6|\u4FC1|\u4FC2|\u4FD3|\u4FD4|\u4FE0|\u4FE5|\u5000|\u5006|\u5008|\u5009|\u500B|\u5011|\u502B|\u5032|\u5049|\u5051|\u5069|\u5074|\u5075|\u507D|\u508C|\u5091|\u5096|\u5098|\u5099|\u50AA|\u50AD|\u50AF|\u50B1|\u50B3|\u50B4|\u50B5|\u50B7|\u50BE|\u50C0|\u50C2|\u50C5|\u50C6|\u50C9|\u50CD|\u50D1|\u50D3|\u50D5|\u50D7|\u50DE|\u50E4|\u50E5|\u50E8|\u50E9|\u50F4|\u50F9|\u50FE|\u5100|\u5101|\u5102|\u5104|\u5105|\u5108|\u5109|\u5110|\u5114|\u5115|\u5116|\u5118|\u511F|\u5122|\u5123|\u5125|\u5129|\u512A|\u5130|\u5131|\u5132|\u5137|\u5138|\u5139|\u513A|\u513B|\u513C|\u514C|\u5152|\u5157|\u5167|\u5169|\u518A|\u51AA|\u51C8|\u51CD|\u51D4|\u51D9|\u51DC|\u51DF|\u51F1|\u5225|\u522A|\u5244|\u5247|\u524B|\u524E|\u5257|\u525B|\u525D|\u526E|\u5274|\u5275|\u5278|\u527E|\u5283|\u5287|\u5289|\u528A|\u528C|\u528D|\u528F|\u5291|\u5297|\u529A|\u52C1|\u52D1|\u52D5|\u52D9|\u52DB|\u52DD|\u52DE|\u52E2|\u52E3|\u52E9|\u52F1|\u52F4|\u52F5|\u52F8|\u52FB|\u532D|\u532F|\u5330|\u5331|\u5335|\u5340|\u5354|\u5368|\u537B|\u5399|\u53AD|\u53B1|\u53B2|\u53B4|\u53C3|\u53C4|\u53E2|\u5412|\u5433|\u5436|\u5442|\u54BC|\u54E1|\u54EF|\u5504|\u550A|\u5513|\u551A|\u553B|\u554F|\u555E|\u555F|\u5562|\u558E|\u559A|\u55AA|\u55AC|\u55AE|\u55B2|\u55C6|\u55C7|\u55CA|\u55CE|\u55DA|\u55E7|\u55E9|\u55F6|\u55F9|\u55FF|\u5604|\u5606|\u5607|\u560D|\u5613|\u5614|\u5616|\u5617|\u561C|\u5629|\u562A|\u562E|\u562F|\u5630|\u5633|\u5635|\u5638|\u563A|\u563D|\u5641|\u5645|\u5653|\u565A|\u565D|\u565E|\u5660|\u5665|\u5666|\u566F|\u5672|\u5674|\u5678|\u5679|\u5680|\u5682|\u5687|\u5688|\u568C|\u568D|\u5690|\u5695|\u5699|\u569B|\u569D|\u56A0|\u56A6|\u56A7|\u56A8|\u56A9|\u56AA|\u56AB|\u56AC|\u56B1|\u56B2|\u56B3|\u56B4|\u56B6|\u56B8|\u56BD|\u56BF|\u56C0|\u56C1|\u56C2|\u56C5|\u56C7|\u56C8|\u56C9|\u56CB|\u56D0|\u56D1|\u56D2|\u56D5|\u56EA|\u5707|\u570B|\u570D|\u5712|\u5713|\u5716|\u5718|\u571E|\u57B5|\u57B7|\u57C9|\u57E1|\u57E8|\u57EC|\u57F0|\u57F7|\u5805|\u5808|\u580A|\u5816|\u581A|\u581D|\u582F|\u5831|\u5834|\u584A|\u584B|\u584F|\u5852|\u5857|\u5862|\u5864|\u5875|\u5878|\u5879|\u587C|\u587F|\u5886|\u588A|\u588B|\u588F|\u589C|\u589D|\u58A0|\u58A2|\u58A7|\u58AE|\u58B3|\u58B6|\u58B7|\u58BE|\u58BF|\u58C7|\u58C8|\u58CB|\u58CD|\u58CF|\u58D0|\u58D2|\u58D3|\u58D4|\u58D7|\u58D8|\u58D9|\u58DA|\u58DB|\u58DD|\u58DE|\u58DF|\u58E0|\u58E2|\u58E3|\u58E7|\u58E9|\u58EA|\u58EF|\u58FA|\u58FC|\u58FD|\u5920|\u5922|\u593E|\u5950|\u5967|\u5969|\u596A|\u596B|\u596C|\u596E|\u596F|\u5972|\u597C|\u599D|\u59CD|\u59E6|\u5A19|\u5A1B|\u5A41|\u5A61|\u5A66|\u5A6D|\u5A78|\u5A81|\u5A88|\u5A9C|\u5AA7|\u5AAF|\u5AB0|\u5ABC|\u5ABD|\u5AC8|\u5AD7|\u5AE2|\u5AE5|\u5AE7|\u5AF5|\u5AFB|\u5AFF|\u5B03|\u5B05|\u5B07|\u5B08|\u5B0B|\u5B0C|\u5B10|\u5B12|\u5B19|\u5B21|\u5B23|\u5B24|\u5B26|\u5B2A|\u5B2E|\u5B30|\u5B38|\u5B3B|\u5B3E|\u5B44|\u5B46|\u5B47|\u5B4B|\u5B4C|\u5B4E|\u5B6B|\u5B72|\u5B78|\u5B7B|\u5B7E|\u5B7F|\u5BAE|\u5BE0|\u5BE2|\u5BE6|\u5BE7|\u5BE9|\u5BEA|\u5BEB|\u5BEC|\u5BEF|\u5BF5|\u5BF6|\u5BF7|\u5C07|\u5C08|\u5C0B|\u5C0D|\u5C0E|\u5C35|\u5C37|\u5C46|\u5C4D|\u5C53|\u5C5C|\u5C62|\u5C64|\u5C68|\u5C69|\u5C6C|\u5CA1|\u5CF4|\u5CF6|\u5CFD|\u5D0D|\u5D17|\u5D19|\u5D20|\u5D22|\u5D2C|\u5D31|\u5D35|\u5D50|\u5D77|\u5D78|\u5D7C|\u5D7D|\u5D7E|\u5D81|\u5D84|\u5D87|\u5D88|\u5D94|\u5D97|\u5DA0|\u5DA2|\u5DA4|\u5DA7|\u5DA9|\u5DAA|\u5DAE|\u5DB4|\u5DB8|\u5DB9|\u5DBA|\u5DBC|\u5DBD|\u5DC3|\u5DC6|\u5DCA|\u5DCB|\u5DD1|\u5DD2|\u5DD4|\u5DD6|\u5DD7|\u5DD8|\u5DDA|\u5DE0|\u5DF0|\u5E25|\u5E2B|\u5E33|\u5E34|\u5E36|\u5E40|\u5E43|\u5E53|\u5E57|\u5E58|\u5E5F|\u5E60|\u5E63|\u5E69|\u5E6B|\u5E6C|\u5E70|\u5E71|\u5E79|\u5E7A|\u5E7E|\u5EAB|\u5EB2|\u5EC1|\u5EC2|\u5EC4|\u5EC8|\u5ED4|\u5ED5|\u5ED7|\u5EDA|\u5EDD|\u5EDE|\u5EDF|\u5EE0|\u5EE1|\u5EE2|\u5EE3|\u5EE5|\u5EE7|\u5EE9|\u5EEC|\u5EEE|\u5EF3|\u5F12|\u5F33|\u5F35|\u5F37|\u5F44|\u5F48|\u5F4C|\u5F4D|\u5F4E|\u5F59|\u5F5E|\u5F60|\u5F65|\u5F72|\u5F8C|\u5F91|\u5F9E|\u5FA0|\u5FA9|\u5FB5|\u5FB9|\u5FBF|\u6046|\u6065|\u6085|\u608F|\u609E|\u60B5|\u60B6|\u60C0|\u60E1|\u60F1|\u60F2|\u60FB|\u6107|\u611B|\u611C|\u6128|\u6129|\u6134|\u6137|\u613E|\u6144|\u614B|\u614D|\u6150|\u6158|\u6159|\u615A|\u615F|\u6163|\u616A|\u616B|\u616E|\u616F|\u6171|\u6172|\u6173|\u6176|\u6178|\u6179|\u617A|\u6182|\u618A|\u618D|\u6190|\u6191|\u6192|\u6196|\u619A|\u61A2|\u61A4|\u61A6|\u61AA|\u61AB|\u61AE|\u61B2|\u61B4|\u61B6|\u61B8|\u61B9|\u61C0|\u61C7|\u61C9|\u61CC|\u61CD|\u61D3|\u61D5|\u61D8|\u61D9|\u61DC|\u61DF|\u61E0|\u61E3|\u61E4|\u61E7|\u61E8|\u61E9|\u61EB|\u61ED|\u61F0|\u61F2|\u61F6|\u61F7|\u61F8|\u61FA|\u61FC|\u61FE|\u6200|\u6201|\u6203|\u6207|\u6214|\u6227|\u6229|\u6230|\u6231|\u6232|\u6236|\u62CB|\u6329|\u633E|\u6368|\u636B|\u6381|\u6383|\u6384|\u6386|\u6397|\u6399|\u639A|\u639B|\u63A1|\u63C0|\u63DA|\u63DB|\u63EE|\u640A|\u640D|\u640E|\u6416|\u6417|\u6435|\u6436|\u6440|\u6443|\u644B|\u6450|\u6451|\u6455|\u6459|\u645C|\u645F|\u646A|\u646B|\u646F|\u6472|\u6473|\u6476|\u647B|\u647C|\u6488|\u648A|\u648B|\u648C|\u648F|\u6490|\u6493|\u649D|\u649F|\u64A3|\u64A5|\u64A7|\u64AB|\u64B2|\u64B3|\u64B6|\u64BB|\u64BE|\u64BF|\u64C1|\u64C3|\u64C4|\u64C7|\u64C8|\u64CA|\u64CB|\u64D3|\u64D4|\u64DA|\u64DF|\u64E0|\u64E3|\u64E5|\u64E7|\u64EA|\u64EB|\u64EC|\u64EF|\u64F0|\u64F1|\u64F2|\u64F3|\u64F4|\u64F7|\u64FA|\u64FB|\u64FC|\u64FD|\u64FE|\u6504|\u6506|\u650B|\u650E|\u650F|\u6511|\u6514|\u6516|\u6519|\u651B|\u651C|\u651D|\u651E|\u6522|\u6523|\u6524|\u6526|\u6527|\u6529|\u652A|\u652C|\u6533|\u6557|\u6558|\u6575|\u6578|\u657A|\u657F|\u6581|\u6582|\u6583|\u6584|\u6585|\u6586|\u6595|\u65AC|\u65B7|\u65B8|\u65BC|\u65DD|\u65DF|\u661C|\u6642|\u6649|\u665B|\u665D|\u6688|\u6689|\u6690|\u6698|\u669F|\u66A2|\u66AB|\u66C4|\u66C6|\u66C7|\u66C9|\u66CA|\u66CF|\u66D6|\u66E0|\u66E5|\u66E8|\u66EC|\u66ED|\u66EE|\u66F8|\u6703|\u6725|\u6727|\u6771|\u67F5|\u6871|\u687F|\u6894|\u6896|\u6898|\u689C|\u689D|\u689F|\u68B2|\u68C4|\u68C6|\u68D6|\u68D7|\u68DF|\u68E1|\u68E7|\u68F2|\u68F6|\u690F|\u691A|\u6932|\u6947|\u694A|\u694E|\u6953|\u6968|\u696D|\u6975|\u699D|\u69AA|\u69AE|\u69AF|\u69B2|\u69BF|\u69CB|\u69CD|\u69E4|\u69E7|\u69E8|\u69EB|\u69EE|\u69F3|\u69F6|\u69FB|\u69FC|\u6A01|\u6A02|\u6A05|\u6A13|\u6A19|\u6A1E|\u6A20|\u6A22|\u6A23|\u6A2B|\u6A32|\u6A33|\u6A38|\u6A39|\u6A3A|\u6A3B|\u6A3F|\u6A43|\u6A45|\u6A48|\u6A4B|\u6A5A|\u6A5F|\u6A62|\u6A68|\u6A6B|\u6A6F|\u6A81|\u6A82|\u6A89|\u6A8B|\u6A92|\u6A94|\u6A9B|\u6A9C|\u6A9F|\u6AA1|\u6AA2|\u6AA3|\u6AA5|\u6AAD|\u6AAE|\u6AAF|\u6AB0|\u6AB2|\u6AB3|\u6AB5|\u6AB8|\u6ABB|\u6ABE|\u6ABF|\u6AC3|\u6AC5|\u6ACD|\u6ACE|\u6ACF|\u6AD3|\u6ADA|\u6ADB|\u6ADD|\u6ADE|\u6ADF|\u6AE0|\u6AE2|\u6AE5|\u6AE7|\u6AE8|\u6AE9|\u6AEA|\u6AEB|\u6AEC|\u6AEF|\u6AF1|\u6AF3|\u6AF4|\u6AF6|\u6AF8|\u6AF9|\u6AFB|\u6AFD|\u6B04|\u6B07|\u6B0A|\u6B0D|\u6B0F|\u6B10|\u6B11|\u6B12|\u6B13|\u6B16|\u6B18|\u6B1E|\u6B3D|\u6B44|\u6B4D|\u6B50|\u6B55|\u6B57|\u6B5B|\u6B5E|\u6B5F|\u6B61|\u6B72|\u6B77|\u6B78|\u6B7F|\u6B98|\u6B9E|\u6BA2|\u6BA4|\u6BA8|\u6BAB|\u6BAE|\u6BAF|\u6BB0|\u6BB2|\u6BBA|\u6BBC|\u6BC0|\u6BC4|\u6BC6|\u6BCA|\u6BFF|\u6C00|\u6C02|\u6C08|\u6C0C|\u6C23|\u6C2B|\u6C2C|\u6C2D|\u6C33|\u6C7A|\u6C92|\u6C96|\u6CC1|\u6D36|\u6D79|\u6D7F|\u6D87|\u6DB7|\u6DBC|\u6DDA|\u6DE5|\u6DEA|\u6DF5|\u6DF6|\u6DFA|\u6E19|\u6E1B|\u6E22|\u6E26|\u6E2C|\u6E3E|\u6E4A|\u6E4B|\u6E5E|\u6E6F|\u6E88|\u6E96|\u6E9D|\u6EA1|\u6EA4|\u6EAB|\u6EAE|\u6EB0|\u6EB3|\u6EC4|\u6EC5|\u6ECC|\u6ECE|\u6EEC|\u6EED|\u6EEF|\u6EF2|\u6EF7|\u6EF8|\u6EFB|\u6EFE|\u6EFF|\u6F01|\u6F0A|\u6F0D|\u6F0E|\u6F10|\u6F19|\u6F1A|\u6F22|\u6F23|\u6F2C|\u6F32|\u6F35|\u6F38|\u6F3F|\u6F41|\u6F51|\u6F54|\u6F5A|\u6F5B|\u6F63|\u6F64|\u6F6C|\u6F6F|\u6F70|\u6F77|\u6F7F|\u6F80|\u6F85|\u6F86|\u6F87|\u6F92|\u6F96|\u6F97|\u6FA0|\u6FA2|\u6FA4|\u6FA6|\u6FA9|\u6FAB|\u6FAC|\u6FAE|\u6FB0|\u6FB1|\u6FBE|\u6FC1|\u6FC3|\u6FC4|\u6FC6|\u6FC7|\u6FCA|\u6FD5|\u6FD8|\u6FDA|\u6FDC|\u6FDF|\u6FE4|\u6FE7|\u6FEB|\u6FF0|\u6FF1|\u6FFA|\u6FFC|\u6FFE|\u6FFF|\u7001|\u7002|\u7003|\u7004|\u7005|\u7006|\u7007|\u7008|\u7009|\u700B|\u700F|\u7015|\u7018|\u7019|\u701D|\u701F|\u7020|\u7022|\u7026|\u7027|\u7028|\u702F|\u7030|\u7032|\u7033|\u7034|\u7035|\u703E|\u7043|\u7044|\u704D|\u7051|\u7052|\u7053|\u7055|\u7058|\u7059|\u705D|\u705F|\u7060|\u7061|\u7063|\u7064|\u7066|\u7067|\u707D|\u70BA|\u70CF|\u70F4|\u711B|\u7121|\u7147|\u7149|\u7152|\u7159|\u7162|\u7165|\u7169|\u716C|\u7171|\u717C|\u7182|\u7185|\u7189|\u718C|\u7192|\u7193|\u7195|\u7197|\u719E|\u71A1|\u71B0|\u71B1|\u71B2|\u71BE|\u71C0|\u71C1|\u71C8|\u71CC|\u71D2|\u71D6|\u71D8|\u71D9|\u71DC|\u71DF|\u71E1|\u71E6|\u71ED|\u71F0|\u71F4|\u71F5|\u71F6|\u71FC|\u71FD|\u71FE|\u7201|\u7203|\u7204|\u720D|\u7210|\u7213|\u7216|\u721B|\u7223|\u7225|\u7227|\u722D|\u723A|\u723E|\u7246|\u724B|\u7258|\u727C|\u727D|\u7285|\u7293|\u7296|\u729E|\u72A2|\u72A4|\u72A7|\u72C0|\u72F9|\u72FD|\u730C|\u730D|\u7319|\u7327|\u7336|\u733B|\u7341|\u7344|\u7345|\u734A|\u734E|\u7351|\u7356|\u735F|\u7362|\u7368|\u7369|\u736A|\u736B|\u736E|\u7370|\u7371|\u7372|\u7375|\u7377|\u7378|\u7379|\u737A|\u737B|\u737C|\u7380|\u7381|\u7382|\u73FC|\u73FE|\u7416|\u743A|\u743F|\u744B|\u7452|\u7459|\u7463|\u7464|\u7469|\u746A|\u7472|\u747B|\u747D|\u7489|\u748A|\u7495|\u7497|\u749B|\u749D|\u74A1|\u74A3|\u74A6|\u74AB|\u74AF|\u74B0|\u74B5|\u74B8|\u74B9|\u74BC|\u74BD|\u74BE|\u74C4|\u74C5|\u74CA|\u74CF|\u74D0|\u74D3|\u74D4|\u74D5|\u74DA|\u74DB|\u750A|\u750C|\u7512|\u7516|\u7522|\u755D|\u7562|\u756B|\u7570|\u7576|\u7587|\u758A|\u75D9|\u75EE|\u75FE|\u7602|\u760B|\u760D|\u7611|\u7612|\u7613|\u761E|\u7621|\u7627|\u762E|\u7631|\u7632|\u763A|\u7642|\u7646|\u7647|\u7648|\u7649|\u764E|\u7650|\u7658|\u765F|\u7660|\u7662|\u7664|\u7665|\u7667|\u7669|\u766A|\u766C|\u766D|\u766E|\u7670|\u7671|\u7672|\u7674|\u767C|\u769A|\u769F|\u76AA|\u76B0|\u76B8|\u76BA|\u76BE|\u76DC|\u76DE|\u76E1|\u76E3|\u76E4|\u76E7|\u76E8|\u76EA|\u7725|\u773E|\u774D|\u774F|\u7754|\u775C|\u775E|\u776A|\u7774|\u7793|\u7798|\u779B|\u779C|\u779E|\u77A1|\u77A4|\u77AF|\u77B1|\u77B6|\u77B7|\u77BC|\u77C9|\u77CA|\u77D1|\u77D3|\u77D5|\u77D6|\u77D8|\u77DA|\u77EF|\u77F2|\u785C|\u7864|\u7868|\u786F|\u7899|\u78A2|\u78A9|\u78AD|\u78B8|\u78BA|\u78BC|\u78BD|\u78D1|\u78D2|\u78DA|\u78E0|\u78E3|\u78E7|\u78EF|\u78F1|\u78F5|\u78FD|\u78FE|\u7904|\u7906|\u790B|\u790E|\u790F|\u7910|\u7912|\u7919|\u791A|\u791B|\u7925|\u7926|\u7929|\u792A|\u792B|\u792C|\u792E|\u7930|\u7931|\u7932|\u7939|\u797F|\u798D|\u798E|\u7993|\u7995|\u799C|\u79A1|\u79A6|\u79AA|\u79AC|\u79AE|\u79AF|\u79B0|\u79B1|\u79B5|\u79BF|\u79C8|\u7A05|\u7A08|\u7A0F|\u7A1F|\u7A2E|\u7A31|\u7A40|\u7A47|\u7A4C|\u7A4D|\u7A4E|\u7A56|\u7A60|\u7A61|\u7A62|\u7A67|\u7A68|\u7A69|\u7A6B|\u7A6C|\u7A6D|\u7AA9|\u7AAA|\u7AAE|\u7AAF|\u7AB1|\u7AB5|\u7AB6|\u7ABA|\u7AC0|\u7AC4|\u7AC5|\u7AC7|\u7AC9|\u7ACA|\u7AF1|\u7AF6|\u7B46|\u7B4D|\u7B67|\u7B74|\u7B8B|\u7B8F|\u7BB9|\u7BC0|\u7BC4|\u7BC9|\u7BCB|\u7BD4|\u7BD8|\u7BE2|\u7BE4|\u7BE9|\u7BF3|\u7BF5|\u7BF8|\u7BFF|\u7C00|\u7C02|\u7C0D|\u7C1C|\u7C1E|\u7C21|\u7C22|\u7C23|\u7C25|\u7C2B|\u7C35|\u7C39|\u7C3B|\u7C3D|\u7C3E|\u7C43|\u7C4B|\u7C4C|\u7C54|\u7C59|\u7C5A|\u7C5B|\u7C5C|\u7C5F|\u7C60|\u7C63|\u7C66|\u7C69|\u7C6A|\u7C6B|\u7C6C|\u7C6D|\u7C6E|\u7C6F|\u7CAF|\u7CB5|\u7CBB|\u7CDD|\u7CDE|\u7CE7|\u7CEE|\u7CF0|\u7CF2|\u7CF4|\u7CF6|\u7CF7|\u7CF9|\u7CFA|\u7CFD|\u7CFE|\u7D00|\u7D02|\u7D03|\u7D04|\u7D05|\u7D06|\u7D07|\u7D08|\u7D09|\u7D0B|\u7D0C|\u7D0D|\u7D10|\u7D11|\u7D12|\u7D13|\u7D14|\u7D15|\u7D16|\u7D17|\u7D18|\u7D19|\u7D1A|\u7D1B|\u7D1C|\u7D1D|\u7D1E|\u7D1F|\u7D21|\u7D28|\u7D29|\u7D2C|\u7D2D|\u7D30|\u7D31|\u7D32|\u7D33|\u7D35|\u7D36|\u7D38|\u7D39|\u7D3A|\u7D3C|\u7D3D|\u7D3E|\u7D3F|\u7D40|\u7D41|\u7D42|\u7D43|\u7D44|\u7D45|\u7D46|\u7D47|\u7D4D|\u7D4E|\u7D50|\u7D51|\u7D53|\u7D55|\u7D56|\u7D58|\u7D59|\u7D5A|\u7D5B|\u7D5D|\u7D5E|\u7D5F|\u7D60|\u7D61|\u7D62|\u7D63|\u7D64|\u7D65|\u7D66|\u7D67|\u7D68|\u7D6A|\u7D6F|\u7D70|\u7D71|\u7D72|\u7D73|\u7D78|\u7D79|\u7D7A|\u7D7B|\u7D7C|\u7D7D|\u7D7E|\u7D7F|\u7D80|\u7D81|\u7D83|\u7D84|\u7D85|\u7D86|\u7D87|\u7D88|\u7D8A|\u7D8B|\u7D8C|\u7D8D|\u7D8E|\u7D8F|\u7D90|\u7D93|\u7D95|\u7D96|\u7D9C|\u7D9D|\u7D9E|\u7D9F|\u7DA0|\u7DA1|\u7DA2|\u7DA3|\u7DA7|\u7DAA|\u7DAC|\u7DAD|\u7DAF|\u7DB0|\u7DB1|\u7DB2|\u7DB4|\u7DB5|\u7DB7|\u7DB8|\u7DB9|\u7DBA|\u7DBB|\u7DBC|\u7DBD|\u7DBE|\u7DBF|\u7DC0|\u7DC1|\u7DC2|\u7DC4|\u7DC5|\u7DC6|\u7DC7|\u7DC9|\u7DCA|\u7DCB|\u7DCC|\u7DCD|\u7DCE|\u7DCF|\u7DD2|\u7DD3|\u7DD4|\u7DD7|\u7DD8|\u7DD9|\u7DDA|\u7DDB|\u7DDD|\u7DDE|\u7DDF|\u7DE0|\u7DE1|\u7DE2|\u7DE3|\u7DE4|\u7DE6|\u7DE7|\u7DE8|\u7DE9|\u7DEA|\u7DEB|\u7DEC|\u7DEE|\u7DEF|\u7DF0|\u7DF1|\u7DF2|\u7DF4|\u7DF5|\u7DF6|\u7DF7|\u7DF8|\u7DF9|\u7DFA|\u7DFB|\u7E08|\u7E09|\u7E0A|\u7E0B|\u7E0C|\u7E0D|\u7E0E|\u7E10|\u7E11|\u7E12|\u7E13|\u7E15|\u7E16|\u7E17|\u7E1A|\u7E1B|\u7E1C|\u7E1D|\u7E1E|\u7E1F|\u7E21|\u7E23|\u7E27|\u7E29|\u7E2A|\u7E2B|\u7E2C|\u7E2D|\u7E2E|\u7E2F|\u7E30|\u7E31|\u7E32|\u7E33|\u7E34|\u7E35|\u7E36|\u7E37|\u7E38|\u7E39|\u7E3A|\u7E3C|\u7E3D|\u7E3E|\u7E3F|\u7E40|\u7E42|\u7E43|\u7E45|\u7E46|\u7E48|\u7E4E|\u7E4F|\u7E50|\u7E51|\u7E52|\u7E53|\u7E54|\u7E55|\u7E56|\u7E57|\u7E58|\u7E59|\u7E5A|\u7E5C|\u7E5E|\u7E5F|\u7E61|\u7E62|\u7E63|\u7E68|\u7E69|\u7E6A|\u7E6B|\u7E6C|\u7E6D|\u7E6E|\u7E6F|\u7E70|\u7E72|\u7E73|\u7E75|\u7E76|\u7E77|\u7E78|\u7E79|\u7E7B|\u7E7C|\u7E7D|\u7E7E|\u7E7F|\u7E80|\u7E81|\u7E83|\u7E86|\u7E87|\u7E88|\u7E8A|\u7E8B|\u7E8C|\u7E8D|\u7E8F|\u7E91|\u7E93|\u7E95|\u7E96|\u7E97|\u7E98|\u7E9A|\u7E9C|\u7F3D|\u7F46|\u7F48|\u7F4C|\u7F4F|\u7F70|\u7F75|\u7F77|\u7F7C|\u7F82|\u7F85|\u7F86|\u7F88|\u7F8B|\u7FA5|\u7FA9|\u7FB5|\u7FD2|\u7FDC|\u7FEC|\u7FF9|\u7FFD|\u7FFF|\u802C|\u802E|\u8056|\u805E|\u806F|\u8070|\u8072|\u8073|\u8075|\u8076|\u8077|\u8079|\u807B|\u807D|\u807E|\u8085|\u8105|\u8108|\u811B|\u8125|\u812B|\u8139|\u814E|\u8156|\u8161|\u8166|\u816A|\u816B|\u8173|\u8178|\u8183|\u8192|\u8195|\u819A|\u819E|\u81A0|\u81A2|\u81A9|\u81AE|\u81B4|\u81B6|\u81B7|\u81B9|\u81BD|\u81BE|\u81BF|\u81C7|\u81C9|\u81CD|\u81CF|\u81D7|\u81D8|\u81DA|\u81DF|\u81E0|\u81E1|\u81E2|\u81E8|\u81FA|\u8207|\u8208|\u8209|\u820A|\u8259|\u825B|\u825C|\u8264|\u8266|\u826B|\u826D|\u8271|\u8277|\u82BB|\u82E7|\u8332|\u834A|\u838A|\u8396|\u83A2|\u83A7|\u83D5|\u83EF|\u8407|\u840A|\u842C|\u842F|\u8434|\u8435|\u8449|\u8452|\u8457|\u845D|\u8464|\u8466|\u8477|\u847B|\u848D|\u8492|\u8494|\u849E|\u84AD|\u84B3|\u84B6|\u84BC|\u84C0|\u84CB|\u84EE|\u84EF|\u84F2|\u84F4|\u84FD|\u8504|\u850E|\u851E|\u8520|\u8523|\u8525|\u8526|\u852A|\u852D|\u852E|\u852F|\u8531|\u8541|\u8544|\u8546|\u854E|\u8551|\u8552|\u8553|\u8555|\u8558|\u855D|\u855F|\u8561|\u8562|\u8567|\u8569|\u856A|\u856D|\u8573|\u8577|\u857D|\u8580|\u8586|\u8588|\u8589|\u858A|\u858B|\u858C|\u8594|\u8596|\u8598|\u859F|\u85A0|\u85A6|\u85A9|\u85B1|\u85B2|\u85B3|\u85B4|\u85B5|\u85BA|\u85C7|\u85CD|\u85CE|\u85D6|\u85D8|\u85DA|\u85DD|\u85E3|\u85E5|\u85EA|\u85EC|\u85F0|\u85F6|\u85F7|\u85F9|\u85FA|\u85FE|\u8600|\u8604|\u8606|\u8607|\u8608|\u860A|\u860B|\u861A|\u861E|\u861F|\u8621|\u8622|\u862B|\u862C|\u862D|\u8631|\u8635|\u8639|\u863A|\u863F|\u8645|\u8646|\u8649|\u8655|\u865B|\u865C|\u865F|\u8666|\u8667|\u866F|\u86F5|\u86FA|\u86FB|\u86FC|\u8706|\u8726|\u8738|\u873D|\u8740|\u8741|\u8755|\u875C|\u875F|\u8766|\u8778|\u8784|\u8798|\u879E|\u87A2|\u87AE|\u87B4|\u87B9|\u87BB|\u87BF|\u87C2|\u87C4|\u87C8|\u87CE|\u87D8|\u87DC|\u87E1|\u87E3|\u87E6|\u87EC|\u87EF|\u87F1|\u87F2|\u87F3|\u87F6|\u87F7|\u87FB|\u87FD|\u8800|\u8801|\u8805|\u8806|\u8808|\u880C|\u8810|\u8811|\u8812|\u8819|\u881E|\u881F|\u8823|\u8826|\u8828|\u882A|\u8831|\u8833|\u8836|\u883B|\u883E|\u8853|\u8855|\u885A|\u885B|\u885D|\u889E|\u88CA|\u88CC|\u88DC|\u88DD|\u88E1|\u88F2|\u88FD|\u8907|\u890C|\u8918|\u892D|\u8932|\u8933|\u8938|\u893A|\u893B|\u8940|\u8942|\u8947|\u894C|\u894F|\u8953|\u8956|\u8957|\u8958|\u895B|\u895D|\u8960|\u8964|\u8968|\u896A|\u896C|\u896D|\u896F|\u8970|\u8971|\u8972|\u8974|\u8975|\u8978|\u8979|\u897C|\u8986|\u898B|\u898E|\u898F|\u8992|\u8993|\u8995|\u8996|\u8997|\u8998|\u899B|\u899C|\u899F|\u89A0|\u89A1|\u89A2|\u89A4|\u89A5|\u89A6|\u89A9|\u89AA|\u89AC|\u89AD|\u89AF|\u89B0|\u89B2|\u89B4|\u89B6|\u89B7|\u89B8|\u89B9|\u89BA|\u89BB|\u89BC|\u89BD|\u89BF|\u89C0|\u89F4|\u89F6|\u89F7|\u89F8|\u89F9|\u89FB|\u89FD|\u8A01|\u8A02|\u8A03|\u8A06|\u8A08|\u8A0A|\u8A0C|\u8A0E|\u8A0F|\u8A10|\u8A11|\u8A12|\u8A13|\u8A15|\u8A16|\u8A17|\u8A18|\u8A1B|\u8A1C|\u8A1D|\u8A1E|\u8A1F|\u8A22|\u8A23|\u8A25|\u8A26|\u8A27|\u8A28|\u8A29|\u8A2A|\u8A2C|\u8A2D|\u8A30|\u8A31|\u8A34|\u8A36|\u8A38|\u8A39|\u8A3A|\u8A3B|\u8A3D|\u8A40|\u8A41|\u8A43|\u8A44|\u8A45|\u8A46|\u8A47|\u8A49|\u8A4A|\u8A4C|\u8A4D|\u8A4E|\u8A4F|\u8A50|\u8A51|\u8A52|\u8A53|\u8A54|\u8A55|\u8A56|\u8A57|\u8A58|\u8A5B|\u8A5C|\u8A5D|\u8A5E|\u8A60|\u8A61|\u8A62|\u8A63|\u8A65|\u8A66|\u8A68|\u8A69|\u8A6A|\u8A6B|\u8A6C|\u8A6D|\u8A6E|\u8A6F|\u8A70|\u8A71|\u8A72|\u8A73|\u8A74|\u8A75|\u8A76|\u8A77|\u8A7A|\u8A7B|\u8A7C|\u8A7F|\u8A82|\u8A83|\u8A84|\u8A85|\u8A86|\u8A87|\u8A8B|\u8A8C|\u8A8D|\u8A8E|\u8A8F|\u8A90|\u8A91|\u8A92|\u8A94|\u8A95|\u8A97|\u8A98|\u8A99|\u8A9A|\u8A9C|\u8A9E|\u8AA0|\u8AA1|\u8AA3|\u8AA4|\u8AA5|\u8AA6|\u8AA7|\u8AA8|\u8AAA|\u8AAB|\u8AB0|\u8AB2|\u8AB3|\u8AB4|\u8AB6|\u8AB7|\u8AB9|\u8ABA|\u8ABB|\u8ABC|\u8ABD|\u8ABE|\u8ABF|\u8AC1|\u8AC2|\u8AC3|\u8AC4|\u8AC6|\u8AC7|\u8AC8|\u8AC9|\u8ACB|\u8ACD|\u8ACE|\u8ACF|\u8AD1|\u8AD2|\u8AD3|\u8AD4|\u8AD5|\u8AD6|\u8AD7|\u8ADB|\u8ADC|\u8ADD|\u8ADE|\u8ADF|\u8AE0|\u8AE2|\u8AE3|\u8AE4|\u8AE5|\u8AE6|\u8AE7|\u8AE9|\u8AEB|\u8AED|\u8AEE|\u8AEF|\u8AF0|\u8AF1|\u8AF2|\u8AF3|\u8AF4|\u8AF6|\u8AF7|\u8AF8|\u8AF9|\u8AFA|\u8AFB|\u8AFC|\u8AFE|\u8B00|\u8B01|\u8B02|\u8B04|\u8B05|\u8B06|\u8B09|\u8B0A|\u8B0B|\u8B0C|\u8B0D|\u8B0E|\u8B0F|\u8B10|\u8B11|\u8B14|\u8B16|\u8B17|\u8B19|\u8B1A|\u8B1B|\u8B1C|\u8B1D|\u8B1E|\u8B1F|\u8B20|\u8B23|\u8B25|\u8B28|\u8B2B|\u8B2C|\u8B2D|\u8B2F|\u8B30|\u8B31|\u8B32|\u8B33|\u8B34|\u8B35|\u8B38|\u8B39|\u8B3B|\u8B3C|\u8B3E|\u8B40|\u8B42|\u8B44|\u8B45|\u8B46|\u8B47|\u8B48|\u8B49|\u8B4A|\u8B4C|\u8B4E|\u8B4F|\u8B50|\u8B51|\u8B53|\u8B54|\u8B56|\u8B58|\u8B59|\u8B5A|\u8B5C|\u8B5E|\u8B5F|\u8B60|\u8B61|\u8B68|\u8B69|\u8B6B|\u8B6F|\u8B70|\u8B73|\u8B74|\u8B77|\u8B78|\u8B79|\u8B7A|\u8B7B|\u8B7C|\u8B7D|\u8B7E|\u8B7F|\u8B80|\u8B82|\u8B85|\u8B86|\u8B87|\u8B89|\u8B8A|\u8B8B|\u8B8C|\u8B8E|\u8B91|\u8B92|\u8B93|\u8B94|\u8B95|\u8B96|\u8B98|\u8B99|\u8B9A|\u8B9B|\u8B9C|\u8B9D|\u8B9E|\u8B9F|\u8C44|\u8C45|\u8C48|\u8C4E|\u8C50|\u8C6C|\u8C75|\u8C76|\u8C93|\u8C97|\u8C99|\u8C9D|\u8C9E|\u8C9F|\u8CA0|\u8CA1|\u8CA2|\u8CA3|\u8CA4|\u8CA6|\u8CA7|\u8CA8|\u8CA9|\u8CAA|\u8CAB|\u8CAC|\u8CAF|\u8CB0|\u8CB1|\u8CB2|\u8CB3|\u8CB4|\u8CB6|\u8CB7|\u8CB8|\u8CBA|\u8CBB|\u8CBC|\u8CBD|\u8CBE|\u8CBF|\u8CC0|\u8CC1|\u8CC2|\u8CC3|\u8CC4|\u8CC5|\u8CC7|\u8CC8|\u8CCA|\u8CD1|\u8CD2|\u8CD3|\u8CD5|\u8CD7|\u8CD9|\u8CDA|\u8CDC|\u8CDD|\u8CDE|\u8CDF|\u8CE0|\u8CE1|\u8CE2|\u8CE3|\u8CE4|\u8CE5|\u8CE6|\u8CE7|\u8CE8|\u8CEA|\u8CEC|\u8CED|\u8CEE|\u8CF0|\u8CF4|\u8CF5|\u8CF6|\u8CF8|\u8CF9|\u8CFA|\u8CFB|\u8CFC|\u8CFD|\u8CFE|\u8D03|\u8D04|\u8D05|\u8D06|\u8D07|\u8D08|\u8D09|\u8D0A|\u8D0B|\u8D0D|\u8D0F|\u8D10|\u8D11|\u8D13|\u8D14|\u8D15|\u8D16|\u8D17|\u8D19|\u8D1A|\u8D1B|\u8D6C|\u8D95|\u8D99|\u8DA8|\u8DAB|\u8DAC|\u8DB2|\u8DE1|\u8E10|\u8E1A|\u8E34|\u8E4C|\u8E54|\u8E55|\u8E5B|\u8E61|\u8E63|\u8E64|\u8E65|\u8E6A|\u8E73|\u8E7A|\u8E7B|\u8E80|\u8E82|\u8E89|\u8E8A|\u8E8B|\u8E8D|\u8E8E|\u8E91|\u8E92|\u8E93|\u8E95|\u8E98|\u8E9A|\u8E9D|\u8EA1|\u8EA5|\u8EA6|\u8EA7|\u8EAA|\u8EC0|\u8EC2|\u8EC3|\u8EC7|\u8EC9|\u8ECA|\u8ECB|\u8ECC|\u8ECD|\u8ECE|\u8ECF|\u8ED1|\u8ED2|\u8ED3|\u8ED4|\u8ED5|\u8ED6|\u8ED7|\u8ED8|\u8EDB|\u8EDC|\u8EDD|\u8EDE|\u8EDF|\u8EE4|\u8EE5|\u8EE7|\u8EE8|\u8EEB|\u8EEC|\u8EEE|\u8EEF|\u8EF1|\u8EF2|\u8EF3|\u8EF5|\u8EF7|\u8EF8|\u8EF9|\u8EFA|\u8EFB|\u8EFC|\u8EFE|\u8EFF|\u8F00|\u8F01|\u8F02|\u8F03|\u8F04|\u8F05|\u8F06|\u8F07|\u8F08|\u8F09|\u8F0A|\u8F0B|\u8F10|\u8F11|\u8F12|\u8F13|\u8F14|\u8F15|\u8F16|\u8F17|\u8F18|\u8F19|\u8F1A|\u8F1B|\u8F1C|\u8F1D|\u8F1E|\u8F1F|\u8F20|\u8F21|\u8F22|\u8F23|\u8F24|\u8F25|\u8F26|\u8F28|\u8F29|\u8F2A|\u8F2B|\u8F2C|\u8F2E|\u8F2F|\u8F32|\u8F33|\u8F34|\u8F35|\u8F36|\u8F37|\u8F38|\u8F39|\u8F3B|\u8F3E|\u8F3F|\u8F40|\u8F42|\u8F43|\u8F44|\u8F45|\u8F46|\u8F47|\u8F48|\u8F49|\u8F4A|\u8F4D|\u8F4E|\u8F4F|\u8F50|\u8F51|\u8F52|\u8F53|\u8F54|\u8F55|\u8F56|\u8F57|\u8F58|\u8F59|\u8F5A|\u8F5B|\u8F5D|\u8F5E|\u8F5F|\u8F60|\u8F61|\u8F62|\u8F63|\u8F64|\u8F65|\u8FA6|\u8FAD|\u8FAE|\u8FAF|\u8FB2|\u9015|\u9019|\u9023|\u9032|\u903F|\u904B|\u904E|\u9054|\u9055|\u9059|\u905C|\u905E|\u9060|\u9069|\u9070|\u9071|\u9072|\u9076|\u9077|\u9078|\u907A|\u907C|\u9081|\u9084|\u9087|\u908A|\u908F|\u9090|\u90DF|\u90F2|\u90F5|\u9106|\u9109|\u9112|\u9114|\u9116|\u911F|\u9121|\u9126|\u9127|\u9129|\u912A|\u912C|\u912D|\u912E|\u9130|\u9132|\u9133|\u9134|\u9136|\u913A|\u9147|\u9148|\u9186|\u919C|\u919E|\u91A6|\u91A7|\u91AB|\u91AC|\u91B1|\u91B2|\u91B3|\u91B6|\u91C0|\u91C1|\u91C3|\u91C5|\u91CB|\u91D0|\u91D2|\u91D3|\u91D4|\u91D5|\u91D7|\u91D8|\u91D9|\u91DA|\u91DB|\u91DD|\u91DF|\u91E3|\u91E4|\u91E5|\u91E6|\u91E7|\u91E8|\u91E9|\u91EA|\u91EB|\u91EC|\u91ED|\u91F1|\u91F2|\u91F3|\u91F4|\u91F5|\u91F7|\u91F9|\u91FA|\u91FD|\u91FE|\u91FF|\u9200|\u9201|\u9202|\u9203|\u9204|\u9206|\u9207|\u9208|\u9209|\u920B|\u920D|\u920E|\u920F|\u9210|\u9211|\u9212|\u9213|\u9214|\u9215|\u9216|\u9217|\u921A|\u921B|\u921C|\u921E|\u9220|\u9223|\u9224|\u9225|\u9226|\u9227|\u922A|\u922E|\u922F|\u9230|\u9232|\u9233|\u9234|\u9235|\u9236|\u9237|\u9238|\u9239|\u923A|\u923C|\u923D|\u923E|\u923F|\u9240|\u9241|\u9245|\u9248|\u9249|\u924A|\u924B|\u924C|\u924D|\u924E|\u924F|\u9250|\u9251|\u9252|\u9254|\u9255|\u9257|\u9258|\u9259|\u925A|\u925B|\u925C|\u925D|\u925E|\u925F|\u9260|\u9261|\u9264|\u9265|\u9266|\u9267|\u9268|\u926C|\u926D|\u926E|\u9272|\u9275|\u9276|\u9277|\u9278|\u9279|\u927A|\u927B|\u927C|\u927D|\u927E|\u927F|\u9280|\u9281|\u9282|\u9283|\u9285|\u9288|\u928A|\u928B|\u928D|\u928F|\u9291|\u9293|\u9294|\u9296|\u9297|\u9298|\u9299|\u929A|\u929B|\u929C|\u92A0|\u92A1|\u92A3|\u92A5|\u92A6|\u92A7|\u92A8|\u92A9|\u92AA|\u92AB|\u92AC|\u92B1|\u92B2|\u92B3|\u92B6|\u92B7|\u92B8|\u92B9|\u92BB|\u92BC|\u92BE|\u92C1|\u92C2|\u92C3|\u92C5|\u92C7|\u92C9|\u92CA|\u92CB|\u92CC|\u92CD|\u92CF|\u92D0|\u92D2|\u92D7|\u92D8|\u92D9|\u92DC|\u92DD|\u92DF|\u92E0|\u92E1|\u92E3|\u92E4|\u92E5|\u92E6|\u92E7|\u92E8|\u92E9|\u92EA|\u92EE|\u92EF|\u92F0|\u92F1|\u92F6|\u92F8|\u92F9|\u92FC|\u92FE|\u9300|\u9301|\u9302|\u9304|\u9306|\u9307|\u9308|\u930B|\u930D|\u930F|\u9310|\u9311|\u9312|\u9314|\u9315|\u9317|\u9318|\u9319|\u931A|\u931B|\u931C|\u931D|\u931E|\u931F|\u9320|\u9321|\u9322|\u9323|\u9324|\u9325|\u9326|\u9327|\u9328|\u9329|\u932A|\u932B|\u932D|\u932E|\u932F|\u9333|\u9336|\u9338|\u933D|\u9340|\u9341|\u9342|\u9343|\u9344|\u9346|\u9347|\u9348|\u9349|\u934A|\u934B|\u934D|\u934F|\u9350|\u9351|\u9352|\u9354|\u9356|\u9358|\u935A|\u935B|\u935C|\u935D|\u935F|\u9360|\u9361|\u9363|\u9364|\u9365|\u9366|\u9367|\u9368|\u9369|\u936C|\u936D|\u936E|\u936F|\u9370|\u9371|\u9374|\u9375|\u9376|\u937A|\u937C|\u937E|\u9382|\u9384|\u9385|\u9387|\u9388|\u9389|\u938A|\u938B|\u938C|\u938D|\u9391|\u9392|\u9393|\u9394|\u9395|\u9396|\u9397|\u9398|\u9399|\u939A|\u939B|\u939D|\u939E|\u93A1|\u93A2|\u93A3|\u93A6|\u93A7|\u93A9|\u93AA|\u93AC|\u93AE|\u93AF|\u93B0|\u93B2|\u93B3|\u93B5|\u93B6|\u93B7|\u93BF|\u93C1|\u93C2|\u93C3|\u93C6|\u93C7|\u93C8|\u93C9|\u93CC|\u93CD|\u93CF|\u93D0|\u93D1|\u93D2|\u93D3|\u93D4|\u93D5|\u93D7|\u93D8|\u93D9|\u93DA|\u93DC|\u93DD|\u93DE|\u93DF|\u93E1|\u93E2|\u93E4|\u93E5|\u93E6|\u93E8|\u93E9|\u93F0|\u93F5|\u93F7|\u93F8|\u93F9|\u93FA|\u93FB|\u93FD|\u93FE|\u9400|\u9401|\u9403|\u9404|\u9407|\u9408|\u9409|\u940A|\u940B|\u940D|\u940E|\u940F|\u9410|\u9412|\u9413|\u9414|\u9415|\u9416|\u9418|\u9419|\u941A|\u941D|\u9420|\u9424|\u9425|\u9426|\u9427|\u9428|\u9429|\u942A|\u942B|\u942C|\u942E|\u942F|\u9432|\u9433|\u9434|\u9435|\u9436|\u9438|\u9439|\u943A|\u943C|\u943D|\u943F|\u9440|\u9444|\u9447|\u9448|\u9449|\u944A|\u944B|\u944C|\u944F|\u9450|\u9451|\u9452|\u9454|\u9455|\u9456|\u9458|\u9459|\u945B|\u945E|\u9460|\u9461|\u9462|\u9463|\u9465|\u9468|\u946A|\u946D|\u946E|\u946F|\u9470|\u9471|\u9472|\u9474|\u9477|\u9478|\u9479|\u947C|\u947D|\u947E|\u947F|\u9480|\u9481|\u9483|\u9577|\u9580|\u9582|\u9583|\u9584|\u9585|\u9586|\u9588|\u9589|\u958B|\u958C|\u958D|\u958E|\u958F|\u9590|\u9591|\u9592|\u9593|\u9594|\u9595|\u9597|\u9598|\u959B|\u959C|\u959D|\u959E|\u959F|\u95A1|\u95A3|\u95A4|\u95A5|\u95A6|\u95A7|\u95A8|\u95A9|\u95AB|\u95AC|\u95AD|\u95AF|\u95B1|\u95B5|\u95B6|\u95B7|\u95B9|\u95BB|\u95BC|\u95BD|\u95BE|\u95BF|\u95C3|\u95C4|\u95C6|\u95C7|\u95C8|\u95C9|\u95CA|\u95CB|\u95CC|\u95CD|\u95D0|\u95D1|\u95D2|\u95D3|\u95D4|\u95D5|\u95D6|\u95DA|\u95DB|\u95DC|\u95DE|\u95DF|\u95E0|\u95E1|\u95E2|\u95E4|\u95E5|\u962A|\u9658|\u965D|\u9663|\u9670|\u9673|\u9678|\u967D|\u967F|\u9689|\u968A|\u968E|\u9691|\u9695|\u9696|\u969B|\u96A4|\u96A8|\u96AA|\u96AB|\u96AE|\u96AF|\u96B1|\u96B2|\u96B4|\u96B8|\u96BB|\u96CB|\u96D6|\u96D9|\u96DB|\u96DC|\u96DE|\u96E2|\u96E3|\u96F2|\u96FB|\u9722|\u9723|\u9727|\u973C|\u973D|\u9742|\u9744|\u9745|\u9746|\u9748|\u9749|\u975A|\u975C|\u9766|\u9767|\u9768|\u9780|\u978F|\u979D|\u97B8|\u97BB|\u97BC|\u97BD|\u97BE|\u97C1|\u97C3|\u97C6|\u97C7|\u97C9|\u97CA|\u97CB|\u97CC|\u97CD|\u97CF|\u97D0|\u97D2|\u97D3|\u97D4|\u97D7|\u97D8|\u97D9|\u97DA|\u97DB|\u97DC|\u97DD|\u97DE|\u97E0|\u97E1|\u97E2|\u97E3|\u97FB|\u97FF|\u9801|\u9802|\u9803|\u9804|\u9805|\u9806|\u9807|\u9808|\u980A|\u980C|\u980D|\u980E|\u980F|\u9810|\u9811|\u9812|\u9813|\u9814|\u9815|\u9816|\u9817|\u9818|\u981B|\u981C|\u981E|\u981F|\u9820|\u9821|\u9822|\u9824|\u9826|\u9829|\u982A|\u982B|\u982D|\u982E|\u982F|\u9830|\u9832|\u9834|\u9835|\u9837|\u9838|\u9839|\u983B|\u9840|\u9841|\u9843|\u9844|\u9845|\u9846|\u9847|\u9849|\u984A|\u984B|\u984C|\u984D|\u984E|\u984F|\u9850|\u9851|\u9852|\u9853|\u9856|\u9857|\u9858|\u9859|\u985B|\u985C|\u985D|\u985E|\u9860|\u9862|\u9863|\u9864|\u9865|\u9866|\u9867|\u9869|\u986A|\u986B|\u986C|\u986E|\u986F|\u9870|\u9871|\u9873|\u9874|\u98A8|\u98A9|\u98AC|\u98AD|\u98AE|\u98AF|\u98B0|\u98B1|\u98B2|\u98B3|\u98B4|\u98B6|\u98B7|\u98B8|\u98B9|\u98BA|\u98BB|\u98BC|\u98BD|\u98BE|\u98BF|\u98C0|\u98C1|\u98C2|\u98C4|\u98C6|\u98C7|\u98C8|\u98C9|\u98CB|\u98CD|\u98DB|\u98E0|\u98E2|\u98E3|\u98E4|\u98E5|\u98E6|\u98E9|\u98EA|\u98EB|\u98ED|\u98EF|\u98F0|\u98F2|\u98F4|\u98F5|\u98F6|\u98F7|\u98FC|\u98FD|\u98FE|\u98FF|\u9900|\u9902|\u9903|\u9904|\u9905|\u9909|\u990A|\u990C|\u990E|\u990F|\u9911|\u9912|\u9913|\u9914|\u9915|\u9916|\u9917|\u9918|\u991A|\u991B|\u991C|\u991E|\u991F|\u9921|\u9922|\u9923|\u9924|\u9926|\u9927|\u9928|\u9929|\u992A|\u992B|\u992C|\u992D|\u992F|\u9930|\u9931|\u9932|\u9933|\u9934|\u9935|\u9936|\u9937|\u9938|\u9939|\u993A|\u993C|\u993E|\u993F|\u9940|\u9941|\u9943|\u9945|\u9946|\u9947|\u9948|\u9949|\u994A|\u994B|\u994C|\u994E|\u9950|\u9952|\u9957|\u9958|\u9959|\u995B|\u995C|\u995E|\u995F|\u9960|\u9961|\u9962|\u99A9|\u99AC|\u99AD|\u99AE|\u99AF|\u99B1|\u99B2|\u99B3|\u99B4|\u99B5|\u99B9|\u99BA|\u99BC|\u99BD|\u99C1|\u99C2|\u99C3|\u99C9|\u99CA|\u99CD|\u99CE|\u99CF|\u99D0|\u99D1|\u99D2|\u99D3|\u99D4|\u99D5|\u99D7|\u99D8|\u99D9|\u99DA|\u99DB|\u99DC|\u99DD|\u99DE|\u99DF|\u99E2|\u99E3|\u99E4|\u99E5|\u99E7|\u99E9|\u99EA|\u99EB|\u99EC|\u99ED|\u99EE|\u99F0|\u99F1|\u99F4|\u99F6|\u99F7|\u99F8|\u99F9|\u99FA|\u99FB|\u99FC|\u99FD|\u99FE|\u99FF|\u9A00|\u9A01|\u9A02|\u9A03|\u9A04|\u9A05|\u9A07|\u9A09|\u9A0A|\u9A0B|\u9A0C|\u9A0D|\u9A0E|\u9A0F|\u9A11|\u9A14|\u9A15|\u9A16|\u9A17|\u9A19|\u9A1A|\u9A1C|\u9A1D|\u9A1E|\u9A1F|\u9A20|\u9A22|\u9A23|\u9A24|\u9A25|\u9A27|\u9A29|\u9A2A|\u9A2B|\u9A2C|\u9A2D|\u9A2E|\u9A2F|\u9A30|\u9A31|\u9A32|\u9A33|\u9A34|\u9A35|\u9A36|\u9A37|\u9A38|\u9A39|\u9A3A|\u9A3B|\u9A3C|\u9A3D|\u9A3E|\u9A40|\u9A41|\u9A42|\u9A43|\u9A44|\u9A45|\u9A48|\u9A49|\u9A4A|\u9A4B|\u9A4C|\u9A4D|\u9A4E|\u9A4F|\u9A50|\u9A52|\u9A53|\u9A54|\u9A55|\u9A56|\u9A57|\u9A59|\u9A5A|\u9A5B|\u9A5E|\u9A5F|\u9A60|\u9A61|\u9A62|\u9A64|\u9A65|\u9A66|\u9A68|\u9A69|\u9A6A|\u9A6B|\u9AAF|\u9ACF|\u9AD0|\u9AD2|\u9AD4|\u9AD5|\u9AD6|\u9AEE|\u9B06|\u9B0D|\u9B16|\u9B17|\u9B1A|\u9B1C|\u9B1D|\u9B1E|\u9B20|\u9B21|\u9B22|\u9B25|\u9B27|\u9B29|\u9B2E|\u9B31|\u9B39|\u9B3A|\u9B4E|\u9B57|\u9B58|\u9B5A|\u9B5B|\u9B5C|\u9B5D|\u9B5F|\u9B60|\u9B61|\u9B62|\u9B63|\u9B65|\u9B66|\u9B67|\u9B68|\u9B6A|\u9B6B|\u9B6C|\u9B6D|\u9B6E|\u9B6F|\u9B71|\u9B74|\u9B75|\u9B76|\u9B77|\u9B7A|\u9B7B|\u9B7C|\u9B7D|\u9B7E|\u9B80|\u9B81|\u9B82|\u9B83|\u9B84|\u9B85|\u9B86|\u9B87|\u9B88|\u9B8A|\u9B8B|\u9B8C|\u9B8D|\u9B8E|\u9B8F|\u9B90|\u9B91|\u9B92|\u9B93|\u9B98|\u9B9A|\u9B9B|\u9B9C|\u9B9E|\u9B9F|\u9BA0|\u9BA1|\u9BA3|\u9BA4|\u9BA5|\u9BA6|\u9BA7|\u9BA8|\u9BAA|\u9BAB|\u9BAC|\u9BAD|\u9BAE|\u9BAF|\u9BB0|\u9BB3|\u9BB5|\u9BB6|\u9BB7|\u9BB8|\u9BB9|\u9BBA|\u9BBB|\u9BBF|\u9BC0|\u9BC1|\u9BC4|\u9BC5|\u9BC6|\u9BC7|\u9BC8|\u9BC9|\u9BCA|\u9BCC|\u9BD2|\u9BD4|\u9BD5|\u9BD6|\u9BD7|\u9BDA|\u9BDB|\u9BDD|\u9BDE|\u9BE0|\u9BE1|\u9BE2|\u9BE4|\u9BE5|\u9BE6|\u9BE7|\u9BE8|\u9BE9|\u9BEA|\u9BEB|\u9BEC|\u9BEE|\u9BF0|\u9BF1|\u9BF4|\u9BF6|\u9BF7|\u9BF8|\u9BF9|\u9BFB|\u9BFC|\u9BFD|\u9BFE|\u9BFF|\u9C01|\u9C02|\u9C03|\u9C05|\u9C06|\u9C07|\u9C08|\u9C09|\u9C0A|\u9C0B|\u9C0C|\u9C0D|\u9C0F|\u9C10|\u9C11|\u9C12|\u9C13|\u9C15|\u9C17|\u9C1C|\u9C1D|\u9C1F|\u9C20|\u9C21|\u9C23|\u9C24|\u9C25|\u9C26|\u9C27|\u9C28|\u9C29|\u9C2B|\u9C2C|\u9C2D|\u9C2E|\u9C2F|\u9C31|\u9C32|\u9C33|\u9C34|\u9C35|\u9C36|\u9C37|\u9C39|\u9C3A|\u9C3B|\u9C3C|\u9C3D|\u9C3E|\u9C3F|\u9C40|\u9C41|\u9C42|\u9C43|\u9C44|\u9C45|\u9C46|\u9C47|\u9C48|\u9C49|\u9C4A|\u9C4B|\u9C4C|\u9C4D|\u9C4E|\u9C4F|\u9C50|\u9C51|\u9C52|\u9C53|\u9C54|\u9C55|\u9C56|\u9C57|\u9C58|\u9C5A|\u9C5D|\u9C5E|\u9C5F|\u9C60|\u9C62|\u9C63|\u9C64|\u9C65|\u9C66|\u9C67|\u9C68|\u9C6C|\u9C6D|\u9C6E|\u9C6F|\u9C72|\u9C74|\u9C75|\u9C77|\u9C78|\u9C79|\u9C7A|\u9C7B|\u9CE5|\u9CE6|\u9CE7|\u9CE9|\u9CED|\u9CF1|\u9CF2|\u9CF3|\u9CF4|\u9CF6|\u9CF7|\u9CF8|\u9CFA|\u9CFB|\u9CFC|\u9CFD|\u9CFE|\u9CFF|\u9D00|\u9D01|\u9D02|\u9D03|\u9D05|\u9D06|\u9D07|\u9D09|\u9D0D|\u9D10|\u9D12|\u9D13|\u9D14|\u9D15|\u9D17|\u9D18|\u9D19|\u9D1A|\u9D1B|\u9D1C|\u9D1D|\u9D1E|\u9D1F|\u9D20|\u9D21|\u9D22|\u9D23|\u9D25|\u9D26|\u9D28|\u9D29|\u9D2E|\u9D2F|\u9D30|\u9D31|\u9D32|\u9D33|\u9D34|\u9D36|\u9D37|\u9D38|\u9D39|\u9D3A|\u9D3B|\u9D3D|\u9D3E|\u9D3F|\u9D40|\u9D41|\u9D42|\u9D43|\u9D44|\u9D45|\u9D4A|\u9D4B|\u9D4C|\u9D4E|\u9D4F|\u9D50|\u9D51|\u9D52|\u9D53|\u9D54|\u9D55|\u9D56|\u9D57|\u9D59|\u9D5A|\u9D5B|\u9D5C|\u9D5D|\u9D5F|\u9D60|\u9D61|\u9D67|\u9D69|\u9D6A|\u9D6B|\u9D6C|\u9D6E|\u9D6F|\u9D70|\u9D71|\u9D72|\u9D73|\u9D74|\u9D75|\u9D76|\u9D77|\u9D78|\u9D79|\u9D7B|\u9D7C|\u9D7D|\u9D7E|\u9D80|\u9D82|\u9D83|\u9D84|\u9D85|\u9D86|\u9D87|\u9D89|\u9D8A|\u9D8B|\u9D8C|\u9D92|\u9D93|\u9D94|\u9D95|\u9D96|\u9D97|\u9D98|\u9D99|\u9D9A|\u9D9B|\u9D9D|\u9D9E|\u9D9F|\u9DA0|\u9DA1|\u9DA2|\u9DA3|\u9DA4|\u9DA5|\u9DA6|\u9DA8|\u9DA9|\u9DAA|\u9DAC|\u9DAD|\u9DAF|\u9DB0|\u9DB1|\u9DB2|\u9DB4|\u9DB5|\u9DB6|\u9DB7|\u9DB9|\u9DBA|\u9DBB|\u9DBC|\u9DBD|\u9DC0|\u9DC1|\u9DC2|\u9DC3|\u9DC5|\u9DC7|\u9DC8|\u9DC9|\u9DCA|\u9DCB|\u9DCE|\u9DCF|\u9DD0|\u9DD1|\u9DD2|\u9DD3|\u9DD4|\u9DD5|\u9DD6|\u9DD7|\u9DD9|\u9DDA|\u9DDB|\u9DDC|\u9DDE|\u9DDF|\u9DE2|\u9DE3|\u9DE4|\u9DE5|\u9DE6|\u9DE7|\u9DE8|\u9DE9|\u9DEB|\u9DED|\u9DEE|\u9DEF|\u9DF0|\u9DF2|\u9DF3|\u9DF5|\u9DF6|\u9DF7|\u9DF8|\u9DF9|\u9DFA|\u9DFD|\u9DFE|\u9DFF|\u9E00|\u9E01|\u9E02|\u9E03|\u9E04|\u9E05|\u9E06|\u9E07|\u9E09|\u9E0A|\u9E0B|\u9E0C|\u9E0E|\u9E0F|\u9E10|\u9E11|\u9E12|\u9E13|\u9E15|\u9E16|\u9E17|\u9E18|\u9E19|\u9E1A|\u9E1B|\u9E1C|\u9E1D|\u9E1E|\u9E75|\u9E79|\u9E7A|\u9E7C|\u9E7D|\u9E97|\u9EA1|\u9EA5|\u9EA7|\u9EA8|\u9EA9|\u9EAC|\u9EAE|\u9EAF|\u9EB0|\u9EB1|\u9EB2|\u9EB3|\u9EB4|\u9EB5|\u9EB7|\u9EBC|\u9EBD|\u9EC2|\u9EC3|\u9ECC|\u9EDE|\u9EE8|\u9EF2|\u9EF6|\u9EF7|\u9EF8|\u9EFD|\u9EFF|\u9F00|\u9F01|\u9F04|\u9F05|\u9F06|\u9F08|\u9F09|\u9F0A|\u9F1A|\u9F32|\u9F34|\u9F48|\u9F4A|\u9F4B|\u9F4C|\u9F4D|\u9F4E|\u9F4F|\u9F52|\u9F54|\u9F55|\u9F56|\u9F57|\u9F58|\u9F59|\u9F5A|\u9F5C|\u9F5D|\u9F5E|\u9F5F|\u9F60|\u9F61|\u9F63|\u9F64|\u9F65|\u9F66|\u9F67|\u9F69|\u9F6A|\u9F6C|\u9F6D|\u9F6E|\u9F6F|\u9F70|\u9F71|\u9F72|\u9F73|\u9F74|\u9F75|\u9F76|\u9F77|\u9F78|\u9F79|\u9F7A|\u9F7B|\u9F7C|\u9F7D|\u9F7E|\u9F8D|\u9F8E|\u9F8F|\u9F90|\u9F91|\u9F93|\u9F94|\u9F95|\u9F96|\u9F9C|\u9F9D|\u9F9E|\u9FA5|\u9FAD|\u9FAF|\u9FB2|\u9FBD|\u9FC1|\u9FD0|\u9FD2|\u20054|\u2005E|\u20325|\u20385|\u20392|\u203E2|\u203EE|\u20407|\u2040A|\u2040D|\u2042E|\u2043D|\u20447|\u20459|\u20472|\u205AB|\u205FF|\u20625|\u20732|\u2077F|\u20786|\u207AD|\u207EA|\u2080E|\u2080F|\u2081D|\u2082B|\u20A58|\u20A6C|\u20B19|\u20D54|\u20D58|\u20D79|\u20DB8|\u20DB9|\u20DCC|\u20DCF|\u20E5B|\u20E96|\u20EAE|\u20F17|\u20F24|\u20F2E|\u20F48|\u20F78|\u20FAC|\u20FD5|\u20FD8|\u20FFF|\u21020|\u2103F|\u2105A|\u2106F|\u21092|\u210A1|\u210BF|\u210C4|\u210C8|\u210E4|\u21114|\u21116|\u21123|\u21124|\u21129|\u2114F|\u21158|\u21165|\u21167|\u2136B|\u2144D|\u2144E|\u2146D|\u2146F|\u214B6|\u214C1|\u214D7|\u214E6|\u214FE|\u215C6|\u217B5|\u217EB|\u2181A|\u21839|\u21883|\u21898|\u218BF|\u218E8|\u21920|\u21921|\u2192B|\u21B89|\u21BA3|\u21BA4|\u21CF3|\u21DE8|\u21E17|\u21E6C|\u21EA0|\u21EA8|\u21F31|\u21F3E|\u21F57|\u21F73|\u21F75|\u21F86|\u21FB1|\u21FD6|\u22113|\u2213C|\u22161|\u22163|\u2227F|\u22283|\u22370|\u22417|\u22569|\u22595|\u226D4|\u2272D|\u22830|\u2283C|\u22880|\u228CF|\u228D0|\u228DA|\u228ED|\u2290C|\u2291C|\u22927|\u22929|\u22931|\u2293F|\u22960|\u22BE6|\u22BE9|\u22BF7|\u22C61|\u22C90|\u22CA9|\u22CAB|\u22CB8|\u22CBE|\u22CC2|\u22CDA|\u22D26|\u22D29|\u22D63|\u22D91|\u22D92|\u22DAB|\u22DC3|\u22DCF|\u22DDE|\u22DEE|\u22E01|\u22E14|\u22E19|\u22E33|\u22E34|\u22E38|\u22E4F|\u22E65|\u22E7C|\u22E7F|\u22E8E|\u22EB3|\u22FD3|\u22FE1|\u23018|\u23037|\u2303B|\u23138|\u23236|\u232AF|\u232CB|\u232DE|\u23302|\u23350|\u23384|\u2339C|\u2353F|\u2364E|\u2367F|\u23699|\u236E3|\u23755|\u23781|\u23790|\u237BB|\u23815|\u23829|\u23832|\u2384C|\u23876|\u2390B|\u2393F|\u23A55|\u23AD2|\u23BE9|\u23BF4|\u23BF6|\u23C1B|\u23C28|\u23D07|\u23DAF|\u23ECF|\u23ED1|\u23F0A|\u23F29|\u23F4F|\u23FB7|\u23FC9|\u2402A|\u24063|\u2406A|\u24119|\u24137|\u24159|\u24169|\u24177|\u24356|\u2435C|\u243A4|\u243B1|\u243D0|\u24473|\u24479|\u2448E|\u244A6|\u244BB|\u244CC|\u244CE|\u244D3|\u24600|\u246EE|\u246F1|\u24706|\u247E4|\u24814|\u2482E|\u24872|\u2489F|\u248CE|\u248E4|\u2496D|\u24A42|\u24ABA|\u24AE9|\u24B05|\u24BA6|\u24C93|\u24CA2|\u24CF7|\u24CF8|\u24DC3|\u24DFD|\u24E2B|\u24E89|\u24E94|\u24EDC|\u24EDD|\u24EF2|\u24F08|\u24F89|\u2502C|\u25032|\u250AB|\u250B8|\u251D4|\u25278|\u252DD|\u25303|\u2531A|\u253DD|\u25502|\u25565|\u25585|\u2558F|\u255A9|\u255B2|\u255C7|\u255F4|\u255F9|\u255FA|\u255FD|\u25603|\u25710|\u25730|\u257B5|\u2588A|\u258A2|\u258B6|\u258B7|\u25A10|\u25A82|\u25BE4|\u25C78|\u25CCA|\u25D28|\u25D3C|\u25D43|\u25D4A|\u25D5B|\u25D5C|\u25D5D|\u25E20|\u25EBC|\u25EE4|\u25EE6|\u25EF5|\u25F36|\u25F3D|\u25F56|\u25F6D|\u25F7D|\u25F82|\u25F9D|\u25FAF|\u25FC9|\u25FCA|\u25FEF|\u2600E|\u26016|\u26044|\u26055|\u26067|\u26085|\u2608B|\u260C4|\u260D2|\u260D8|\u260E9|\u2610B|\u2610D|\u26127|\u2613C|\u26147|\u26148|\u2614B|\u26158|\u26177|\u26186|\u26188|\u261B2|\u261CE|\u261DB|\u2633E|\u26346|\u263B9|\u263D1|\u26480|\u26516|\u26627|\u26716|\u2679B|\u267D0|\u267FC|\u26805|\u2684F|\u26856|\u2685D|\u26867|\u26876|\u26888|\u268C7|\u268CE|\u269F4|\u269FA|\u26AAD|\u26ABD|\u26C4C|\u26CDD|\u26D55|\u26D86|\u26E37|\u26EA3|\u26F52|\u26F8F|\u26FB5|\u26FB6|\u26FCD|\u2707F|\u27085|\u270FD|\u27355|\u273FB|\u27410|\u27431|\u27496|\u274AF|\u27525|\u2755F|\u27566|\u275A6|\u276F8|\u27701|\u27702|\u27717|\u27723|\u27735|\u27736|\u2775E|\u27785|\u27794|\u277A3|\u277AB|\u277B6|\u277CC|\u27808|\u27825|\u27835|\u2784D|\u2786A|\u27874|\u27878|\u27883|\u27884|\u2788D|\u278A2|\u278F4|\u27963|\u2797A|\u2799D|\u279A6|\u279A7|\u279AD|\u279DD|\u279ED|\u279F5|\u279F8|\u27A0A|\u27A1D|\u27A33|\u27A3E|\u27A55|\u27A59|\u27A66|\u27A67|\u27A6A|\u27A7C|\u27A9E|\u27AA1|\u27AA6|\u27AAA|\u27AAE|\u27ADA|\u27ADD|\u27B01|\u27B05|\u27B07|\u27B0C|\u27B24|\u27B28|\u27B2A|\u27B2E|\u27B2F|\u27B3B|\u27B48|\u27B79|\u27B86|\u27B87|\u27B88|\u27B93|\u27C06|\u27C7B|\u27CDF|\u27D2A|\u27D4A|\u27D73|\u27D84|\u27D94|\u27D9F|\u27DA7|\u27DB2|\u27DCE|\u27DDB|\u27E16|\u27E18|\u27E26|\u27E2A|\u27E2B|\u27E48|\u27F62|\u27F6F|\u27F75|\u27FA5|\u28042|\u28090|\u280D8|\u280DC|\u28109|\u28123|\u28130|\u2814D|\u28185|\u28189|\u281AA|\u281B1|\u281C1|\u281CD|\u281D7|\u281DE|\u281E4|\u281EF|\u281F0|\u281FD|\u28200|\u28206|\u28207|\u2820A|\u2820C|\u28256|\u28279|\u282A0|\u282B0|\u282B8|\u282B9|\u282BB|\u282C1|\u282DA|\u282E2|\u282EE|\u28304|\u28308|\u28348|\u2834F|\u28350|\u28352|\u28370|\u28379|\u2838C|\u283A9|\u283AA|\u283AE|\u283D2|\u283D4|\u283E0|\u283E5|\u28436|\u2844A|\u2860C|\u287A8|\u287BA|\u287CA|\u288BF|\u288C3|\u288C8|\u288C9|\u288DE|\u288E7|\u288E8|\u2890B|\u28921|\u2893B|\u2895B|\u2895C|\u2895F|\u28966|\u2897A|\u289A1|\u289AB|\u289C0|\u289D0|\u289DA|\u289DC|\u289EB|\u289F0|\u289F1|\u28A0F|\u28A1B|\u28A1D|\u28A22|\u28A2F|\u28A39|\u28A68|\u28A70|\u28A85|\u28A8B|\u28A95|\u28AC0|\u28AD2|\u28AFC|\u28B02|\u28B12|\u28B16|\u28B1E|\u28B1F|\u28B43|\u28B46|\u28B4C|\u28B4E|\u28B50|\u28B56|\u28B57|\u28B5A|\u28B5B|\u28B65|\u28B78|\u28B81|\u28B82|\u28B85|\u28BB0|\u28BB3|\u28BC5|\u28BDF|\u28BF5|\u28C03|\u28C0B|\u28C20|\u28C25|\u28C2D|\u28C32|\u28C35|\u28C37|\u28C39|\u28C65|\u28CAD|\u28CB3|\u28CCC|\u28CD0|\u28CD1|\u28CD2|\u28CD5|\u28CD9|\u28CDA|\u28CE8|\u28CF8|\u28CFF|\u28D11|\u28D17|\u28D24|\u28D39|\u28D46|\u28D4C|\u28D57|\u28D64|\u28D66|\u28D69|\u28D6C|\u28D78|\u28D80|\u28D8F|\u28D91|\u28DAE|\u28DAF|\u28DB0|\u28DB2|\u28DBB|\u28DBF|\u28DC8|\u28DF2|\u28DFB|\u28F33|\u28F48|\u28F4F|\u29028|\u29159|\u29166|\u2917E|\u291C9|\u2924D|\u29259|\u292CC|\u292F0|\u2935C|\u29392|\u29395|\u29396|\u2939F|\u293A0|\u293A2|\u293C2|\u293CC|\u293E0|\u293EA|\u293F4|\u293F7|\u2940C|\u29443|\u29452|\u29454|\u29461|\u29463|\u29466|\u2948E|\u2949C|\u2949D|\u294B2|\u294BA|\u294BC|\u294E3|\u294E5|\u294F8|\u294F9|\u29507|\u29508|\u2950A|\u29511|\u29523|\u29533|\u2954A|\u29570|\u29581|\u295B0|\u295BF|\u295C0|\u295D3|\u295DB|\u295E1|\u295F4|\u29600|\u2961A|\u2961D|\u29639|\u2963A|\u2963B|\u29648|\u29685|\u2969A|\u2969B|\u296A5|\u296A9|\u296B5|\u296C6|\u296CC|\u296CE|\u296DE|\u296E1|\u296E9|\u296F2|\u29707|\u29720|\u29726|\u2972F|\u29730|\u29735|\u29736|\u29751|\u29754|\u29760|\u29761|\u29763|\u29767|\u2977D|\u29783|\u29784|\u29786|\u29789|\u297A1|\u297A6|\u297A7|\u297AC|\u297AF|\u297C0|\u297C2|\u297D0|\u297D7|\u297E0|\u29834|\u29863|\u29864|\u2987A|\u2988D|\u298A1|\u298B0|\u298B2|\u298B4|\u298B8|\u298BC|\u298BE|\u298CA|\u298CB|\u298CF|\u298D1|\u298D4|\u298E1|\u298EB|\u298F5|\u298FA|\u2990A|\u29919|\u29932|\u29935|\u29938|\u29943|\u29944|\u29945|\u29947|\u29949|\u2994E|\u29951|\u29972|\u2997C|\u29983|\u2999A|\u299A0|\u299BA|\u299C6|\u299C9|\u299D0|\u299E2|\u29B59|\u29B6F|\u29BC1|\u29BC3|\u29BC6|\u29BF3|\u29C00|\u29C39|\u29C48|\u29CE4|\u29D06|\u29D35|\u29D5A|\u29D66|\u29D69|\u29D71|\u29D79|\u29D7A|\u29D80|\u29D81|\u29D98|\u29DAF|\u29DB0|\u29DB1|\u29DD2|\u29DF0|\u29DF6|\u29E03|\u29E04|\u29E06|\u29E21|\u29E23|\u29E24|\u29E26|\u29E29|\u29E2C|\u29E42|\u29E4A|\u29E5D|\u29E7D|\u29E7E|\u29E9D|\u29E9E|\u29ED7|\u29EDB|\u29EE7|\u29EEC|\u29EEE|\u29EF0|\u29EF1|\u29F14|\u29F36|\u29F45|\u29F47|\u29F48|\u29F54|\u29F77|\u29F90|\u29F92|\u29F9D|\u29FC5|\u29FCA|\u29FE4|\u29FE7|\u29FEA|\u29FF1|\u29FFA|\u2A009|\u2A016|\u2A017|\u2A01A|\u2A01B|\u2A026|\u2A03B|\u2A03E|\u2A048|\u2A04F|\u2A050|\u2A051|\u2A056|\u2A05B|\u2A05C|\u2A071|\u2A07F|\u2A086|\u2A088|\u2A0A9|\u2A0AB|\u2A0C3|\u2A0CD|\u2A0CF|\u2A0D2|\u2A0E6|\u2A0E7|\u2A0EE|\u2A0FF|\u2A105|\u2A106|\u2A115|\u2A120|\u2A132|\u2A142|\u2A143|\u2A156|\u2A15C|\u2A17E|\u2A183|\u2A1AB|\u2A1B0|\u2A1B4|\u2A1B7|\u2A1C4|\u2A1D6|\u2A1D8|\u2A1F0|\u2A1F3|\u2A20F|\u2A214|\u2A217|\u2A23C|\u2A256|\u2A25C|\u2A263|\u2A268|\u2A26E|\u2A271|\u2A278|\u2A27F|\u2A289|\u2A2C8|\u2A2FC|\u2A2FD|\u2A2FF|\u2A310|\u2A312|\u2A317|\u2A318|\u2A31C|\u2A323|\u2A328|\u2A32C|\u2A32D|\u2A32E|\u2A32F|\u2A330|\u2A33D|\u2A33E|\u2A33F|\u2A340|\u2A347|\u2A34D|\u2A351|\u2A352|\u2A353|\u2A358|\u2A35A|\u2A35E|\u2A360|\u2A363|\u2A364|\u2A36C|\u2A374|\u2A376|\u2A377|\u2A37F|\u2A382|\u2A45A|\u2A473|\u2A4AC|\u2A4BF|\u2A4DB|\u2A4EC|\u2A4F0|\u2A4F9|\u2A4FD|\u2A535|\u2A563|\u2A5A8|\u2A5CB|\u2A5DC|\u2A5DD|\u2A5EA|\u2A5ED|\u2A5F3|\u2A5FB|\u2A5FD|\u2A600|\u2A605|\u2A613|\u2A61E|\u2A625|\u2A627|\u2A628|\u2A629|\u2A62C|\u2A62F|\u2A632|\u2A649|\u2A64D|\u2A64F|\u2A651|\u2A655|\u2A65E|\u2A664|\u2A685|\u2A694|\u2A6A3|\u2A6AD|\u2A6AE|\u2A6B0|\u2A6D5|\u2A756|\u2A775|\u2A7D6|\u2A88D|\u2A8A5|\u2ABB0|\u2ABC2|\u2ACF7|\u2AD25|\u2AD62|\u2ADC8|\u2B0D0|\u2B0D1|\u2B0DE|\u2B0E5|\u2B0F7|\u2B107|\u2B1E0|\u2B239|\u2B24D|\u2B2D0|\u2B2E7|\u2B319|\u2B358|\u2B49E|\u2B4A1|\u2B4A2|\u2B4B7|\u2B518|\u2B521|\u2B59E|\u2B5D1|\u2B5D5|\u2B5FB|\u2B726|\u2B75C|\u2B8F4|\u2B95D|\u2B994|\u2B999|\u2B9B8|\u2B9DD|\u2BA11|\u2BA9B|\u2BB06|\u2BB31|\u2BBD3|\u2BCB4|\u2BDA6|\u2BED1|\u2BFA1|\u2C11D|\u2C189|\u2C264|\u2C326|\u2C341|\u2C3F2|\u2C461|\u2C492|\u2C4E1|\u2C5CF|\u2C5FA|\u2C654|\u2C6D5|\u2C810|\u2C8CD|\u2C8D8|\u2C972|\u2C9D9|\u2CB87|\u2CB8D|\u2CBD8|\u2CC42|\u2CC48|\u2CC9A|\u2CC9B|\u2CD42|\u2CD43|\u2CD6E|\u2CDBC|\u2CE42|\u2D096|\u2D27E|\u2D459|\u2D5E1|\u2D892|\u2D9D2|\u2D9D6|\u2DA21|\u2DC58|\u2DD99|\u2E717|\u2E7FD|\u2E848|\u2E90F|\u2E912|\u2E997|\u2EA2D|\u2EA3B|\u300A0|\u300B4|\u300F4|\u3021D|\u30240|\u302C6|\u303BC|\u30520|\u3052B|\u3053A|\u305BB|\u3062F|\u30682|\u306A3|\u30762|\u307EB|\u30853|\u30AC6|\u30ACF|\u30ADB|\u30AF3|\u30D0F|\u30D26|\u30D3D|\u30E48|\u30EDE|\u30FE2|\u310E1|\u310E2|\u310EA|\u311A5|\u311CB";
            var sc = SimplifiedChinese.IsMatch(strSource);
            var tc = TraditionalChinese.IsMatch(strSource);
            if (sc && tc)
            {
                // たまに繁体字・簡体字両方含んでるやつがあるので、そのときは多い方にする
                var scount = 0;
                var tcount = 0;
                var n = System.Math.Min(strSource.Length, 100);
                for (var i = 0; i < n; i++)
                {
                    var t = strSource.Substring(i, 1);
                    if (SimplifiedChinese.IsMatch(t))
                    {
                        scount++;
                    }
                    else if (TraditionalChinese.IsMatch(t))
                    {
                        tcount++;
                    }
                }
                if (scount > tcount)
                {
                    return "zh-cn";
                }
                else
                {
                    return "zh-tw";
                }
            }
            else if (sc)
            {
                return "zh-cn";
            }
            else if (tc)
            {
                return "zh-tw";
            }

            // ここに来た時点ではまだ日本語・繁体字・簡体字どれも可能性あるんだけど、
            // どれにも含まれる漢字しか使われてないので、日本語フォントでも問題なく表示される。
            if (Kanji.IsMatch(strSource))
            {
                return "ja";
            }

            return "en";
        }
        private string AutoJudgementCJKCheckLanguageDetector2014(string strSource)
        {
            //Language_GPDetector detector = new Language_GPDetector();
            ////detector.AddAllLanguage_GPs();
            //detector.AddLanguage_GPs(Language_GP.EN.ToLower(),
            //    Language_GP.ES.ToLower(),
            //    Language_GP.FR.ToLower(),
            //    Language_GP.RU.ToLower(),
            //    Language_GP.ZHCN.ToLower(),
            //    Language_GP.ZHTW.ToLower(),
            //    Language_GP.JA.ToLower(),
            //    Language_GP.KO.ToLower()
            //);
            return LDJAVA2014.Detect(strSource);
        }
        private bool AutoJudgementCheckShouldEtoJ(string strData)
        {
            //連続する2文字がアルファベットの時のみ翻訳する。
            var strTmp = "";
            var myCheck = false;
            var b1stFlg = false;
            var bTransFlg = false;
            for (int i = 0; i < strData.Length; i++)
            {
                strTmp = strData.Substring(i, 1);
                myCheck = Regex.IsMatch(strTmp, "[A-Za-z]", RegexOptions.Singleline);
                do
                {
                    if (myCheck == false)
                    {
                        b1stFlg = false;
                        break;
                    }
                    if (b1stFlg == false)
                    {
                        b1stFlg = true;
                        break;
                    }
                    if (b1stFlg == true)
                    {
                        bTransFlg = true;
                        break;
                    }
                    break;
                }
                while (false);
            }
            return bTransFlg;
        }
        //************************************************************************************************************************************************************************************************
        //翻訳関数群終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //与えられた行データがチャットログであることを確認し、会話の部分を抜き出す開始
        //************************************************************************************************************************************************************************************************
        private string ExtractTargetTranslationNormal(string strLastLine, ref bool bDoTranslationFlg, ref string strDateTime, ref string strMemberID, ref string strMember, ref bool bBreakFlg)
        {
            var myCheck = false;
            var strTmp = "";
            var strTmp1 = "";
            var strTmp2 = "";
            var strTmp3 = "";
            var numCount = 0;
            var strSource = "";
            bBreakFlg = false;
            bDoTranslationFlg = true;
            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //与えられた行データがチャットログであることを確認し、会話の部分を抜き出すまた、ダブルクォートは排除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //myCheck = Regex.IsMatch(lastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t*\\t*", RegexOptions.Singleline);
                var bChatDataFlg = false;
                do
                {
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t.*\\t.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bChatDataFlg = true;
                        break;
                    }
                }
                while (false);
                if (bChatDataFlg == false)
                {
                    //チャットログではないので終了
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "Normal", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this is not chat Data");
#endif
                    bBreakFlg = true;
                    break;
                }



                //Console.WriteLine(lastLine.Length - 1);
                for (int i = strLastLine.Length - 1; 0 <= i; i--)
                {
                    strTmp = strLastLine.Substring(i, 1);
                    if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                    {
                        numCount++;
                        //if (numCount == 2)
                        //{
                        //    //タブが後ろから数えて２つ目
                        //    break;
                        //}
                    }
                    if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numCount == 0)
                        {
                            strSource = strTmp + strSource;
                        }
                        if (numCount == 1)
                        {
                            strTmp1 = strTmp + strTmp1;
                        }
                        if (numCount == 2)
                        {
                            strTmp2 = strTmp + strTmp2;
                        }
                        if (numCount == 3)
                        {
                            strTmp3 = strTmp + strTmp3;
                        }
                    }
                }
                strSource = ConvertCipherToCyrillic(DateTime.Now, "Normal", strSource);
                //Console.WriteLine("strData = " + strData);
                //Console.WriteLine("strMember = " + strMember);
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);
            strMember = strTmp1.Trim();
            strMemberID = strTmp2.Trim();
            strDateTime = strTmp3.Trim();
            return strSource;
        }
        private string ExtractTargetTranslationAddons(string strLastLine, ref bool bDoTranslationFlg, ref string strDateTime, ref string strMemberID, ref string strMember, ref bool bBreakFlg)
        {
            var myCheck = false;
            var strTmp = "";
            var strTmp1 = "";
            var strTmp2 = "";
            var numCount = 0;
            var strSource = "";
            //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:has reached level//6:killcount//7:You have had a change of fortune!//8:anguish JP//9:anguish EN//10:GM Chat or unknown
            //"01:08:59\tbanner:\t注意！ モンスターの強さは\tC625.0％\tC7増加します！注意してください！ "
            //"01:19:41\tbanner:\tAttention!  The monster attack strength is currently boosted by \tC625.0\tC7%!  Be careful out there! "
            var numBannerType = 99;
            var bTotalFlg = false;
            bBreakFlg = false;
            bDoTranslationFlg = true;
            List<string> strBanner = new List<string>();
            //stop();
            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //取得した最後の行がチャットログであることを確認し、会話の部分を抜き出すまた、ダブルクォートは排除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //myCheck = Regex.IsMatch(lastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t*\\t*", RegexOptions.Singleline);
                var bChatDataFlg = false;
                do
                {
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*HBR Counts.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = false;
                        bChatDataFlg = true;
                        numBannerType = 1;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*Character Material Usage.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = false;
                        bChatDataFlg = true;
                        numBannerType = 2;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*Welcome.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 3;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*has found.*after defeating.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = false;
                        bChatDataFlg = true;
                        numBannerType = 4;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*has reached level.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = false;
                        bChatDataFlg = true;
                        numBannerType = 5;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*Inventory Slot.*kills.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = false;
                        bChatDataFlg = true;
                        numBannerType = 6;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*You have had a change of fortune!.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 7;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*モンスターの強さは.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 8;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*Attention!.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 9;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*INFO.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 10;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\tbanner.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 99;
                        break;
                    }
                    myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t.*\\t.*", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bDoTranslationFlg = true;
                        bChatDataFlg = true;
                        numBannerType = 0;
                        break;
                    }
                }
                while (false);
                if (bChatDataFlg == false)
                {
                    //チャットログではないので終了
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "Addons", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this is not chat Data");
#endif
                    bBreakFlg = true;
                    break;
                }


                strLastLine = strLastLine.Replace("\t\tB", "\t");//中華系の人のキャラ名の後ろに"\tB"が入っているので削除する。
                //Console.WriteLine(lastLine.Length - 1);
                if (numBannerType == 0)
                {
                    for (int i = strLastLine.Length - 1; 0 <= i; i--)
                    {
                        strTmp = strLastLine.Substring(i, 1);
                        if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                        {
                            numCount++;
                            //if (numCount == 2)
                            //{
                            //    //タブが後ろから数えて２つ目
                            //    break;
                            //}
                        }
                        if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                        {
                            if (numCount == 0)
                            {
                                strSource = strTmp + strSource;
                            }
                            if (numCount == 1)
                            {
                                strTmp1 = strTmp + strTmp1;
                            }
                            if (numCount == 2)
                            {
                                strTmp2 = strTmp + strTmp2;
                            }
                        }
                    }
                    strSource = ConvertCipherToCyrillic(DateTime.Now, "Addons", strSource);
                    strMember = strTmp1.Trim();
                    strMemberID = "--------";
                    strDateTime = strTmp2.Trim();
                    //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                }
                if (numBannerType != 0)
                {
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "Addons", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this is banner chat data");
#endif
                    for (int i = 0; i < strLastLine.Length; i++)
                    {
                        strTmp = strLastLine.Substring(i, 1);
                        do
                        {
                            if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                            {
                                numCount++;
                                break;
                            }
                            if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                            {
                                if (strBanner.Count <= numCount)
                                {
                                    strBanner.Add("");
                                }
                                strBanner[strBanner.Count - 1] = strBanner[strBanner.Count - 1] + strTmp;
                            }
                        }
                        while (false);
                    }
                    //stop();
                    //strSource = gfCyrillicConvert(DateTime.Now, "Addons", strSource);
                    do
                    {
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        //"15:39:12\tbanner:\t\tC6HBR Counts\tC7 --  スイープアップ作戦 #1: \tC60\tC7,  Silent Afterimage #2: \tC624\tC7,  奪われたヘルパラッシュ: \tC612\tC7,  Total Points: 0 (+\tC60\tC7 to DAR)  Ranking: \tC3NO RANK\tC7 "
                        if (numBannerType == 1)
                        {
                            for (int i = 0; i < strBanner.Count - 1; i++)
                            {
                                strTmp = strBanner[i];
                                do
                                {
                                    if (bTotalFlg == true)
                                    {
                                        break;
                                    }
                                    if (Regex.IsMatch(strTmp, ".*Total Points:.*", RegexOptions.Singleline) == true)
                                    {
                                        bTotalFlg = true;
                                        break;
                                    }
                                    if (Regex.IsMatch(strTmp, "C6.*", RegexOptions.Singleline) == false)
                                    {
                                        break;
                                    }
                                    //strBuf.Add("");
                                    //strBuf[strBuf.Count - 1] = strBuf[strBuf.Count - 1] + strTmp.Substring(2);
                                    if (strSource != "")
                                    {
                                        strSource = strSource + "/";
                                    }
                                    strSource = strSource + strTmp.Substring(2);
                                }
                                while (false);
                            }
                            strSource = "HBR Count[" + strSource + "]";
                            strMember = "HBR Counts";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        //"[Character Material Usage] HP: 125 -- TP: 0 -- Power: 0 -- Mind: 150 -- Evade: 0 -- Defense: 0 -- Luck: 0 -- Total Material Usage (Excluding HP and TP): 150 "
                        //"[Character Material Usage] HP: 0 -- TP: 0 -- Power: 112 -- Mind: 0 -- Evade: 0 -- Defense: 0 -- Luck: 38 -- Total Material Usage (Excluding HP and TP): 150 14:18:59\tbanner:\t[Character Material Usage] HP: 0 -- TP: 0 -- Power: 112 -- Mind: 0 -- Evade: 0 -- Defense: 0 -- Luck: 38 -- Total Material Usage (Excluding HP and TP): 150 "
                        if (numBannerType == 2)
                        {
                            //stop();

                            for (int i = 0; i < strLastLine.Length; i++)
                            {
                                strTmp = strLastLine.Substring(i);
                                do
                                {
                                    if (Regex.IsMatch(strTmp, "^HP.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^TP.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Power.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Mind.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Evade.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Defense.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Luck.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^Total.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^\\x28.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^\\x29.*", RegexOptions.Singleline))
                                    {
                                        numCount++;
                                    }
                                    if (Regex.IsMatch(strTmp, "^\\x20.*", RegexOptions.Singleline))
                                    {
                                        break;
                                    }
                                    if (Regex.IsMatch(strTmp, "^\\x2D.*", RegexOptions.Singleline))
                                    {
                                        break;
                                    }
                                    if (strBanner.Count <= numCount)
                                    {
                                        strBanner.Add("");
                                    }
                                    strBanner[strBanner.Count - 1] = strBanner[strBanner.Count - 1] + strTmp.Substring(0, 1);
                                } while (false);
                            }
                            //stop();
                            strSource = "Mat";
                            strSource += "[";
                            strSource += "HP=" + strBanner[3].Substring(3);
                            strSource += ",TP=" + strBanner[4].Substring(3);
                            strSource += ",Power=" + strBanner[5].Substring(6);
                            strSource += ",Mind=" + strBanner[6].Substring(5);
                            strSource += ",Evade=" + strBanner[7].Substring(6);
                            strSource += ",Defense=" + strBanner[8].Substring(8);
                            strSource += ",Luck=" + strBanner[9].Substring(5);
                            strSource += "]";
                            strMember = "Material";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        //"15:55:01\tbanner:\tWelcome to \tC6Ephinea\tC7!  Ship locations are as follows: \tC3US/Fodra\tC7 -- Chicago, Illinois / \tC3US/Auldrant, US/Devaloka\tC7 -- Seattle, Washington / \tC3EU/Lumireis\tC7 -- Frankfurt, Germany"
                        if (numBannerType == 3)
                        {
                            //stop();
                            for (int i = 0; i < strBanner.Count; i++)
                            {
                                if (Regex.IsMatch(strBanner[i], "^C\\d.*", RegexOptions.Singleline))
                                {
                                    strBanner[i] = strBanner[i].Substring(2);
                                }
                            }
                            //stop();
                            strSource = "";
                            for (int i = 2; i < strBanner.Count; i++)
                            {
                                strSource += strBanner[i];
                            }
                            //stop();
                            strMember = "Welcome";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        //"17:00:49\tbanner:\tUS/Fodra - Block 02 - \tC3Blastoise \tC7 has found \tC6Lame d'Argent\tC7 after defeating \tC4Ul Gibbon\tC7!"
                        if (numBannerType == 4)
                        {
                            strSource = strBanner[5].Substring(2);
                            strMember = strBanner[3].Substring(2);
                            strMemberID = "RareDrop";
                            strDateTime = strBanner[0];
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        //"20:27:03\tbanner:\tUS/Auldrant - Block 01 - \tC6Ralsei\tC7 has reached level \tC9200\tC7!  Congratulations, though this now famous Hunter's hunt is only just beginning!"

                        if (numBannerType == 5)
                        {
                            strSource = "reached level 200! Congratulations";
                            strMember = strBanner[3].Substring(2);
                            strMemberID = "Lv200";
                            strDateTime = strBanner[0];
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        if (numBannerType == 6)
                        {
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        if (numBannerType == 7)
                        {
                            //stop();
                            strSource = strBanner[2];
                            strMember = "Date changed";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        if (numBannerType == 8)
                        {
                            //stop();
                            strSource = strBanner[2] + strBanner[3].Substring(2) + strBanner[4].Substring(2);
                            strMember = "Anguish";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        if (numBannerType == 9)
                        {
                            //stop();
                            strSource = strBanner[2] + strBanner[3].Substring(2) + strBanner[4].Substring(2);
                            strMember = "Anguish";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        if (numBannerType == 10)
                        {
                            //stop();
                            strSource = strLastLine.Substring(18);
                            strSource = strSource.Replace("C3", "");
                            strSource = strSource.Replace("C7", "");
                            strSource = strSource.Replace("C6", "");
                            strSource = strSource.Replace("\t", "");
                            strMember = "INFO";
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                        //0:no banner//1:HBR Counts//2:Character Material Usage//3:Opening Message//4:Rare Drop//5:Lv200//6:killcount//7:You have had a change of fortune!//8:GM Chat or unknown
                        if (numBannerType == 99)
                        {
                            strSource = strBanner[4].Substring(4);
                            strMember = strBanner[3].Substring(1);
                            strMemberID = "--------";
                            strDateTime = strBanner[0];
                        }
                    } while (false);
                }
            }
            while (false);
            return strSource;
        }
        private string ExtractTargetTranslationGuild(string strLastLine, ref bool bDoTranslationFlg, ref string strDateTime, ref string strMemberID, ref string strMember, ref bool bBreakFlg)
        {
            var myCheck = false;
            var strTmp = "";
            var strTmp1 = "";
            var strTmp2 = "";
            var numCount = 0;
            var strSource = "";
            bBreakFlg = false;
            bDoTranslationFlg = true;
            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //取得した最後の行がチャットログであることを確認し、会話の部分を抜き出すまた、ダブルクォートは排除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //myCheck = Regex.IsMatch(lastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t*\\t*", RegexOptions.Singleline);
                //myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t.*\\t.*", RegexOptions.Singleline);
                myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t.*\\t.*", RegexOptions.Singleline);
                if (myCheck == false)
                {
                    //チャットログではないので終了
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "Guild", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this is not chat Data");
#endif
                    bBreakFlg = true;
                    break;
                }
                //Console.WriteLine(lastLine.Length - 1);
                for (int i = strLastLine.Length - 1; 0 <= i; i--)
                {
                    strTmp = strLastLine.Substring(i, 1);
                    if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                    {
                        numCount++;
                        //if (numCount == 2)
                        //{
                        //    //タブが後ろから数えて２つ目
                        //    break;
                        //}
                    }
                    if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numCount == 0)
                        {
                            strSource = strTmp + strSource;
                        }
                        if (numCount == 1)
                        {
                            strTmp1 = strTmp + strTmp1;
                        }
                        if (numCount == 2)
                        {
                            strTmp2 = strTmp + strTmp2;
                        }
                    }
                }
                strSource = ConvertCipherToCyrillic(DateTime.Now, "Guild", strSource);
                //Console.WriteLine("strData = " + strData);
                //Console.WriteLine("strMember = " + strMember);
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);
            strMember = strTmp1.Trim();
            strDateTime = strTmp2.Trim();
            return strSource;
        }
        private string ExtractTargetTranslationSimpleMail(string strLastLine, ref bool bDoTranslationFlg, ref string strDateTime, ref string strMemberID, ref string strMember, ref bool bBreakFlg)
        {
            var myCheck = false;
            var strTmp = "";
            var strTmp1 = "";
            var strTmp2 = "";
            var strTmp3 = "";
            var numCount = 0;
            var strSource = "";
            bBreakFlg = false;
            bDoTranslationFlg = true;
            //stop();
            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //取得した最後の行がチャットログであることを確認し、会話の部分を抜き出すまた、ダブルクォートは排除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //myCheck = Regex.IsMatch(lastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t*\\t*", RegexOptions.Singleline);
                //myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t.*\\t.*", RegexOptions.Singleline);
                myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t.*\\t.*", RegexOptions.Singleline);
                if (myCheck == false)
                {
                    //チャットログではないので終了
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, "SimpleMail", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this is not SimpleMail Data");
#endif
                    bBreakFlg = true;
                    break;
                }
                //Console.WriteLine(lastLine.Length - 1);
                for (int i = strLastLine.Length - 1; 0 <= i; i--)
                {
                    strTmp = strLastLine.Substring(i, 1);
                    if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                    {
                        numCount++;
                        //if (numCount == 2)
                        //{
                        //    //タブが後ろから数えて２つ目
                        //    break;
                        //}
                    }
                    if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numCount == 0)
                        {
                            strSource = strTmp + strSource;
                        }
                        if (numCount == 1)
                        {
                            strTmp1 = strTmp + strTmp1;
                        }
                        if (numCount == 2)
                        {
                            strTmp2 = strTmp + strTmp2;
                        }
                        if (numCount == 3)
                        {
                            strTmp3 = strTmp + strTmp3;
                        }
                    }
                }
                strSource = ConvertCipherToCyrillic(DateTime.Now, "SimpleMail", strSource);
                //Console.WriteLine("strData = " + strData);
                //Console.WriteLine("strMember = " + strMember);
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);
            strMemberID = strTmp2.Trim();
            strMember = strTmp1.Trim();
            strDateTime = strTmp3.Trim(); 
            return strSource;
        }
        //************************************************************************************************************************************************************************************************
        //与えられた行データがチャットログであることを確認し、会話の部分を抜き出す終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //ファイル読み込み関数群開始
        //************************************************************************************************************************************************************************************************
        private void FileReadChatLogFileToArray(string strFullPath, ref string strLogType, ref string[] strLogData, bool bTimerFlg, ref bool bBreakFlg)
        {
            var myCheck = false;
            //var strLastLine = "";
            bBreakFlg = false;
            string[] strTmp = new string[0];
            var numReadedDataMemory = 0;
            var numReadedDataDisk = 0;
            var bHitFlg = false;
            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //更新されたファイルをチャットログかどうかチェックして、チャットログなら全体を取得
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //Console.WriteLine(strFullPath);
                if (strLogType == "Normal" && Regex.IsMatch(strFullPath, "chat\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "Normal" && Regex.IsMatch(strFullPath, "GuildChat\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    strLogType = "Guild";
                    myCheck = true;
                }
                if (strLogType == "Guild" && Regex.IsMatch(strFullPath, "GuildChat\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "Addons" && Regex.IsMatch(strFullPath, "Chatlog\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "Addons" && Regex.IsMatch(strFullPath, "Charlog\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "SimpleMail" && Regex.IsMatch(strFullPath, "SimpleMail\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "SimpleMail" && Regex.IsMatch(strFullPath, "simpleMail\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (strLogType == "SimpleMail" && Regex.IsMatch(strFullPath, "simple_mail\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline))
                {
                    myCheck = true;
                }
                if (myCheck == false)
                {
#if DEBUG
                    ArryClsListBoxPreWriteBufferAddNewLog(DateTime.Now, "", "SysMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", "this file is no chatlog" + "\t" + strFullPath);
#endif
                    bBreakFlg = true;
                    break;
                }
                if (bTimerFlg == false)
                {
                    //ファイルを読んでくる。
                    FileReadChatLogToVariable(strFullPath, strLogType, ref strTmp);
                }
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);

            //logger.print("ファイル読込配列");
            //logger.print("strLogType = " + strLogType + "\n" + "strFullPath = " + strFullPath);
            //logger.print(strTmp);

            do
            {
                if (strTmp.Length == 0)
                {
                    //読み込んだファイルが空。そんなコト有りうるの？
                    break;
                }
                if (ClsFileBufferRead.numMax == -1)
                {
                    //起動して初回のチャット
                    //logger.print("Call ArrayClsFileBufferReadAddData mode = Start ALL");
                    ArrayClsFileBufferReadAddData(strLogType, strFullPath, strTmp[strTmp.Length - 1]);
                    Array.Resize(ref strLogData, 1);
                    strLogData[0] = strTmp[strTmp.Length - 1];
                    break;
                }
                //読込済のログから、現在読んできたログを検索する
                //for (int i = clsFileBufferRead.numMin; i <= clsFileBufferRead.numMax - 1; i++)
                for (int i = ClsFileBufferRead.numMax; ClsFileBufferRead.numMin <= i; i--)
                {
                    //logger.print("clsFileBufferRead[i].strLogType = " + clsFileBufferRead.strLogType[i] + "\tstrLogType = " + strLogType + "\tclsFileBufferRead.strLogLine[i] = " + clsFileBufferRead.strLogLine[i]);
                    for (var j = strTmp.Length - 1; 0 <= j; j--)
                    {
                        //logger.print("clsFileBufferRead[i].strLogType = " + clsFileBufferRead.strLogType[i] + "\tstrLogType = " + strLogType + "\tclsFileBufferRead.strLogLine[i] = " + clsFileBufferRead.strLogLine[i] + " == " + "strTmp[j] = " + strTmp[j]);
                        if (ClsFileBufferRead.strLogType[i].Equals(strLogType) && ClsFileBufferRead.strFileName[i].Equals(strFullPath) && ClsFileBufferRead.strLogLine[i] == strTmp[j])
                        {
                            //ログタイプと、ファイル名(日付を含んでいる)と、ログラインデータ(時刻を含んでいる)が一致
                            numReadedDataMemory = i;
                            numReadedDataDisk = j;
                            bHitFlg = true;
                        }
                    }
                    //上記、ログにヒットしたフラグが、立っていたらループを抜ける
                    if (bHitFlg == true)
                    {
                        break;
                    }
                }
                if (bHitFlg == true)
                {
                    //現在読んできたログを検索して存在したので、次行(！)からログに追加し、同時に読んで来たログを呼び出し元に戻す
                    int j = -1;
                    //for (int i = numReadedDataDisk; i <= strTmp.Length - 1; i++)
                    for (int i = numReadedDataDisk + 1; i <= strTmp.Length - 1; i++)
                    {
                        //logger.print("Call ArrayClsFileBufferReadAddData mode = Add Log");
                        ArrayClsFileBufferReadAddData(strLogType, strFullPath, strTmp[i]);
                        j++;
                        Array.Resize(ref strLogData, j + 1);
                        strLogData[j] = strTmp[i];
                    }
                    break;
                }
                //現在読んできたログを検索して非存在ならば、該当ログの起動してから初回チャットである
                //logger.print("Call ArrayClsFileBufferReadAddData mode = Start Log");
                ArrayClsFileBufferReadAddData(strLogType, strFullPath, strTmp[strTmp.Length - 1]);
                Array.Resize(ref strLogData, 1);
                strLogData[0] = strTmp[strTmp.Length - 1];
            }
            while (false);
        }
        public void FileReadChatLogToVariable(string strFullPath, string strLogType, ref string[] strLogData)
        {
            var i = 0;
            do//読込チャレンジループ
            {
                try
                {
                    //読込チャレンジしてみる
                    //strReadBuf = File.ReadAllLines(strFullPath);
                    //strTextALL = File.ReadAllText(strFullPath, Encoding.GetEncoding("shift-jis"));
                    //FileStream fs = new FileStream(strFullPath,  FileMode.Open, FileAccess.Read);
                    FileStream fs = new FileStream(strFullPath, FileMode.Open, FileAccess.Read);
                    // テキストエンコーディングにUTF-8を用いてstreamの読み込みを行うStreamReaderを作成する
                    var sr = new StreamReader(fs);

                    //streamから文字列を読み込み
                    string strBuf = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    fs.Close();
                    fs.Dispose();
                    strBuf = strBuf.Replace("\r\n\r\n", "\r\n");
                    strBuf = strBuf.TrimEnd('\r', '\n');
                    strLogData = Regex.Split(strBuf, "\r\n");
                }
                //失敗判定
                catch (System.IO.IOException)
                {
                    //失敗していたらスルーして再読込
                }
                if (0 < strLogData.Length)
                {
                    //読み込まれていたらチャレンジ終了
                    break;
                }
                i++;
                Thread.Sleep(10);
            }
            while (i <= 10);
        }
        private void ArrayClsWaitingToTranslateAddnew(string strFullPath, string strLogType, string strFileName, bool bTimerFlg)
        {
            var strLastLine = "";
            var strMember = "";
            var strMemberID = "";
            var strDateTime = "";
            var strSource = "";
            var bBreakFlg = false;
            //var strMemberLog = "";
            //var strAfterSlang = "";
            //var strAfterTranslation = "";
            //var bHitFlg = false;
            //var strTranslator = "";
            //var numMaxWaitNo = 0;
            var numMyWaitNo = 0;
            //var strTmp = "";
            var bMainProcFlg = false;
            //var bSortFlg = false;
            var bDoTranslationFlg = false;

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);
            var strSpaceChat = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "SpaceChat", strIniFileName);
            var strTranslation = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Translation", strIniFileName);
            var strDeepLAPIFreeKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Free Key", strIniFileName);
            var strDeepLAPIProKey = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "DeepL API Pro Key", strIniFileName);
            var strGoogleAppsScriptsURL = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "Google Apps Scripts URL", strIniFileName);

            DateTime dtmAddLogDate;
            var strYYYY = "";
            var strMM = "";
            var strDD = "";
            var strHH = "";
            var strmm = "";
            var strSS = "";

            string[] strLogData = new string[0];
            if (bTimerFlg == true)
            {
                if (strLogType.Equals("Normal"))
                {
                    strFullPath = ClsLastCallVar.strFullPathNormal;
                    strFileName = ClsLastCallVar.strFileNameNormal;
                }
                if (strLogType.Equals("Addons"))
                {
                    strFullPath = ClsLastCallVar.strFullPathAddons;
                    strFileName = ClsLastCallVar.strFileNameAddons;
                }
                if (strLogType.Equals("Guild"))
                {
                    strFullPath = ClsLastCallVar.strFullPathGuild;
                    strFileName = ClsLastCallVar.strFileNameGuild;
                }
                if (strLogType.Equals("SimpleMail"))
                {
                    strFullPath = ClsLastCallVar.strFullPathSimpleMail;
                    strFileName = ClsLastCallVar.strFileNameSimpleMail;
                }
            }
            do//空ループ
            {
                if (strFullPath.Equals(""))
                {
                    break;
                }
                if (strFullPath.Equals(ClsChatDetailReadFile.strFullPathNormal))
                {
                    break;
                }

                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //チャットログが変更されたら開く
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                bBreakFlg = false;
                FileReadChatLogFileToArray(strFullPath, ref strLogType, ref strLogData, bTimerFlg, ref bBreakFlg);
                if (bBreakFlg == true)
                {
                    break;
                }
                //for (int i = strLogData.Length - 1; 0 <= i; i--)
                for (int i = 0; i < strLogData.Length; i++)
                {
                    numMyWaitNo = 0;
                    strLastLine = strLogData[i];
                    if (strLastLine.Equals(""))
                    {
                        break;
                    }
                    do
                    {
                        bBreakFlg = false;
                        if (strLogType.Equals("Normal"))
                        {
                            strSource = ExtractTargetTranslationNormal(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                            ClsLastCallVar.strFullPathNormal = strFullPath;
                            ClsLastCallVar.strFileNameNormal = strFileName;
                        }
                        if (strLogType.Equals("Addons"))
                        {
                            strSource = ExtractTargetTranslationAddons(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                            ClsLastCallVar.strFullPathAddons = strFullPath;
                            ClsLastCallVar.strFileNameAddons = strFileName;
                        }
                        if (strLogType.Equals("Guild"))
                        {
                            strSource = ExtractTargetTranslationGuild(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                            ClsLastCallVar.strFullPathGuild = strFullPath;
                            ClsLastCallVar.strFileNameGuild = strFileName;
                        }
                        if (strLogType.Equals("SimpleMail"))
                        {
                            strSource = ExtractTargetTranslationSimpleMail(strLastLine, ref bDoTranslationFlg, ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                            ClsLastCallVar.strFullPathSimpleMail = strFullPath;
                            ClsLastCallVar.strFileNameSimpleMail = strFileName;
                        }
                        if (bBreakFlg == true)
                        {
                            break;
                        }
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //日付時刻を計算
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        GetFileNameFileNameToDateTime(strFileName, ref strYYYY, ref strMM, ref strDD);
                        try
                        {
                            strHH = strDateTime.Substring(0, 2);
                            strmm = strDateTime.Substring(3, 2);
                            strSS = strDateTime.Substring(6, 2);
                            dtmAddLogDate = DateTime.Parse(strYYYY + "/" + strMM + "/" + strDD + " " + strHH + ":" + strmm + ":" + strSS);
                        }
                        catch
                        {
                            break;
                        }
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //strSource = gfCyrillicConvert(dtmAddLogDate, strLogType, strSource);
#if DEBUG
                        ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, strFullPath, "SysMsg", false, strLogType, strDateTime, strMemberID, strMember, "SysMsg ReadData = " + strSource);
#endif
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //再入待ちウェイト
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        //各タスクが、自分のチャット行を取得したら、再入待ちウェイトをする
                        do//再入待ちウェイトループ
                        {
                            do//空ループ
                            {
                                //if (ClsWaitingToTranslate.bExclusiveFlg == true)
                                //{
                                //    //配列占有フラグが立っている＝書込不可
                                //    break;
                                //}

                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                //配列占有フラグを立て、他のプロセスが配列に触れないようにする
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                                //Application.DoEvents();
                                //ClsWaitingToTranslate.bExclusiveFlg = true;

                                //ウェイトナンバーを取得していない場合、自分のウェイトナンバーを取得し、待ち行列に登録する
                                //最後の配列要素はクリア用に取っておく。
                                if (numMyWaitNo == 0)
                                {
                                    //logger.print("ファイル読込配列に登録されているかチェック");
                                    for (int j = 1; j <= ClsFileBufferRead.numMax; j++)
                                    {
                                        //logger.print(ClsWaitingToTranslate.strLogType[j] + " == " + strLogType + "\t" + ClsWaitingToTranslate.strFileName[j] + " == " + strFileName + "\t" + ClsWaitingToTranslate.strLogLine[j] + " == " + strLastLine);
                                        if (ClsWaitingToTranslate.strLogType[j] == strLogType && ClsWaitingToTranslate.strFileName[j] == strFileName && ClsWaitingToTranslate.strLogLine[j] == strLastLine)
                                        {
                                            //既にファイル読込配列に登録されている。
                                            //logger.print("ファイル読込配列に登録されている");
                                            numMyWaitNo = ClsWaitingToTranslate.numTaskNo[j];
                                            break;
                                        }
                                    }
                                }
                                if (numMyWaitNo == 0)
                                {
                                    var numTmp = 0;
                                    var bOverFlowFlg = true;
                                    //実際に登録する部分
                                    for (var j = 0; j <= MaxArray.numMaxClsWaitingToTranslate - 2; j++)
                                    {
                                        if (ClsWaitingToTranslate.numTaskNo[j] == 0)
                                        {
                                            numTmp++;
                                            numMyWaitNo = numTmp;
                                            ClsWaitingToTranslate.numTaskNo[j] = numMyWaitNo;
                                            ClsWaitingToTranslate.dtmLogDate[j] = dtmAddLogDate;
                                            ClsWaitingToTranslate.strLogType[j] = strLogType;
                                            ClsWaitingToTranslate.strLogLine[j] = strLastLine;
                                            ClsWaitingToTranslate.strSource[j] = strSource;
                                            ClsWaitingToTranslate.strFileName[j] = strFileName;
                                            bOverFlowFlg = false;
                                            break;
                                        }
                                        if (numTmp <= ClsWaitingToTranslate.numTaskNo[j])
                                        {
                                            numTmp = ClsWaitingToTranslate.numTaskNo[j];
                                        }
                                    }
                                    //待ち行列あふれ
                                    if (bOverFlowFlg == true)
                                    {
                                        //MessageBox.Show("バッファ オーバーフロー");
                                    }

                                    var strPrint = "";
                                    strPrint += "再読込待ち行列 numMyWaitNo = " + numMyWaitNo + "\n";
                                    for (var j = 0; j <= MaxArray.numMaxClsWaitingToTranslate - 1; j++)
                                    {
                                        if (String.IsNullOrEmpty(ClsWaitingToTranslate.strLogLine[i]))
                                        {
                                            break;
                                        }
                                        strPrint += "\tnumTaskNo[" + j + "] = " + ClsWaitingToTranslate.numTaskNo[j].ToString();
                                        strPrint += "\tdtmLogDate[" + j + "] = " + ClsWaitingToTranslate.dtmLogDate[j].ToString();
                                        strPrint += "\tstrLogType[" + j + "] = " + ClsWaitingToTranslate.strLogType[j];
                                        strPrint += "\tstrSource[" + j + "] = " + ClsWaitingToTranslate.strSource[j];
                                        strPrint += "\tstrLogLine[" + j + "] = " + ClsWaitingToTranslate.strLogLine[j];
                                        strPrint += "\tstrFileName[" + j + "] = " + ClsWaitingToTranslate.strFileName[j];
                                        strPrint += "\n";
                                    }
                                    //logger.print(strPrint);
                                }
                                //ClsWaitingToTranslate.bExclusiveFlg = false;
                                bMainProcFlg = true;
                                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                            }
                            while (false);
                            if (bMainProcFlg == true)
                            {
                                break;
                            }
                        }
                        while (true);
                        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    }
                    while (false);
                }
            }
            while (false);//空ループなので、必ず一回で処理を抜ける
        }
        //************************************************************************************************************************************************************************************************
        //ファイル読み込み関数群終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //配列操作関数群
        //************************************************************************************************************************************************************************************************
        private bool ArryClsListBoxPreWriteBufferAddNewLog(DateTime dtmAddLogDate, string strAddFileName, string strAddMsgType, bool bDoTranslationFlg, string strAddLogType, string strAddDateTime, string strAddMemberID, string strAddMemberName, string strAddLog)
        {
            var bFlg = true;
            do
            {
                if (strAddLog.Equals("") == true)
                {
                    //空文字列なので終了
                    bFlg = false;
                    break;
                }
                //すでに登録されている文字列かどうか、バックログ検索
                if (ArryClsListBoxPreWriteBufferAddNewLog_BackLogSerch(dtmAddLogDate, strAddFileName, strAddMsgType, bDoTranslationFlg, strAddLogType, strAddDateTime, strAddMemberID, strAddMemberName, strAddLog) == true)
                {
                    //すでに登録されている。終了
                    bFlg = false;
                    break;
                }
                do
                {
                    //配列領域の最後まで使い切っていたら配列をスライド
                    if (ClsListBoxPreWriteBuffer.numMax == MaxArray.numMaxArrayClsListBoxPreWriteBuffer - 1)
                    {
                        ArrayClsListBoxPreWriteBufferSilde();
                        break;
                    }
                    //初回の登録行かチェック。初回なら変数を初期化
                    if (ClsListBoxPreWriteBuffer.numMax == -1)
                    {
                        ClsListBoxPreWriteBuffer.numMin = 0;
                        ClsListBoxPreWriteBuffer.numMax++;
                        break;
                    }
                    ClsListBoxPreWriteBuffer.numMax++;
                }
                while (false);
                ClsListBoxPreWriteBuffer.dtmLogDate[ClsListBoxPreWriteBuffer.numMax] = dtmAddLogDate;
                ClsListBoxPreWriteBuffer.strMsgType[ClsListBoxPreWriteBuffer.numMax] = strAddMsgType;
                ClsListBoxPreWriteBuffer.bDoTranslationFlg[ClsListBoxPreWriteBuffer.numMax] = bDoTranslationFlg;
                ClsListBoxPreWriteBuffer.strLogType[ClsListBoxPreWriteBuffer.numMax] = strAddLogType;
                ClsListBoxPreWriteBuffer.strDateTime[ClsListBoxPreWriteBuffer.numMax] = strAddDateTime;
                ClsListBoxPreWriteBuffer.strMemberID[ClsListBoxPreWriteBuffer.numMax] = strAddMemberID;
                ClsListBoxPreWriteBuffer.strMemberName[ClsListBoxPreWriteBuffer.numMax] = strAddMemberName;
                ClsListBoxPreWriteBuffer.strMessage[ClsListBoxPreWriteBuffer.numMax] = strAddLog;
                ClsListBoxPreWriteBuffer.strOriginalMessage[ClsListBoxPreWriteBuffer.numMax] = strAddLog;
                ClsListBoxPreWriteBuffer.strTranslatedMessage[ClsListBoxPreWriteBuffer.numMax] = "";
                ClsListBoxPreWriteBuffer.numListBox[ClsListBoxPreWriteBuffer.numMax] = 0;

                //public static DateTime[] dtmLogDate = new DateTime[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMsgType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static bool[] bDoTranslationFlg = new bool[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strLogType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strDateTime = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMemberID = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMemberName = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strOriginalMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strTranslatedMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static int[] numListBox = new int[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
            }
            while (false);
            ListboxDisplayUpdate();
            return bFlg;
        }
        private bool ArryClsListBoxPreWriteBufferAddNewLog_BackLogSerch(DateTime dtmAddLogDate, string strAddFileName, string strAddMsgType, bool bDoTranslationFlg, string strLogType, string strAddDateTime, string strAddMemberID, string strAddMemberName, string strAddLog)
        {
            //既に登録されていたらtrueを返す関数
            var bFlg = false;
            do
            {
                if (ClsListBoxPreWriteBuffer.numMax == -1)
                {
                    //最初に登録された行だから、バックログは存在しない。
                    //MessageBox.Show("this is a Fiast log");
                    bFlg = false;
                    break;
                }
                if (strLogType.Equals("Normal") == true || strLogType.Equals("Addons") == true || strLogType.Equals("Guild") == true)
                {
                    //ログなのでチェックに進む。
                    //MessageBox.Show("test");
                }
                else
                {
                    //ログじゃないのでバックログを検索しない。
                    bFlg = false;
                    break;
                }
                for (int i = ClsListBoxPreWriteBuffer.numMax; 0 <= i; i--)
                {
                    do
                    {
                        //日付とログタイプとメンバー名とログ内容でマッチング
                        //まずはメンバー名とログ内容でマッチング
                        //次にログタイプと日付でマッチング
                        if (strAddLog.Equals(ClsListBoxPreWriteBuffer.strMessage[i]) == true && strAddMemberName.Equals(ClsListBoxPreWriteBuffer.strMemberName[i]) == true)
                        {
                            //一致したのでログタイプと日付でマッチングへ進む
                        }
                        else
                        {
                            //一致しなかったので次の配列へ
                            break;
                        }
                        if (strLogType.Equals("Normal") || ClsListBoxPreWriteBuffer.strMsgType[i].Equals("LogNormal"))
                        {
                            //同一のログファイルで同一のログ。
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //同一の時間でバックログ一致
                                bFlg = true;
                                break;
                            }
                        }
                        if (strLogType.Equals("Addons") || ClsListBoxPreWriteBuffer.strMsgType[i].Equals("LogAddons"))
                        {
                            //同一のログファイルで同一のログ。
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //同一の時間でバックログ一致
                                bFlg = true;
                                break;
                            }
                        }
                        if (strLogType.Equals("Guild") || ClsListBoxPreWriteBuffer.strMsgType[i].Equals("LogGuild"))
                        {
                            //同一のログファイルで同一のログ。
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //同一の時間でバックログ一致
                                bFlg = true;
                                break;
                            }
                        }
                        if (strLogType.Equals("Addons") || ClsListBoxPreWriteBuffer.strMsgType[i].Equals("LogNormal"))
                        {
                            //別のログファイルで同一のログ。
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                bFlg = true;
                                break;
                            }
                            dtmAddLogDate = dtmAddLogDate.AddSeconds(-1);
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //マイナス一秒でバックログ一致
                                bFlg = true;
                                break;
                            }
                        }
                        if (strLogType.Equals("Normal") || ClsListBoxPreWriteBuffer.strMsgType[i].Equals("LogAddons"))
                        {
                            //別のログファイルで同一のログ。
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //同一の時間でバックログ一致
                                bFlg = true;
                                break;
                            }
                            dtmAddLogDate = dtmAddLogDate.AddSeconds(+1);
                            if (dtmAddLogDate.CompareTo(ClsListBoxPreWriteBuffer.dtmLogDate[i]) == 0)
                            {
                                //プラス一秒でバックログ一致
                                if (strLogType.Equals("Addons"))
                                    bFlg = true;
                                break;
                            }
                        }
                    }
                    while (false);
                    if (bFlg == true)
                    {
                        break;
                    }
                }
            }
            while (false);
            return bFlg;
        }
        public void ArrayClsFileBufferReadAddData(string strLogType, string strFileName, string strLogLine)
        {
            var strAddMode = "";
            var bRegisteredFlg = false;
            do
            {
                for (int i = 1; i <= ClsFileBufferRead.numMax; i++)
                {
                    if (ClsFileBufferRead.strLogType[i] == strLogType && ClsFileBufferRead.strFileName[i] == strFileName && ClsFileBufferRead.strLogLine[i] == strLogLine)
                    {
                        //既にファイル読込配列に登録されている
                        bRegisteredFlg = true;
                        break;
                    }
                }
                if (bRegisteredFlg == true)
                {
                    break;
                }
                if (ClsFileBufferRead.numMax == -1)
                {
                    //ファイル読込配列は初回登録である
                    strAddMode = "Start";
                    ClsFileBufferRead.numMin++;
                    ClsFileBufferRead.numMax++;
                    ClsFileBufferRead.strLogType[ClsFileBufferRead.numMax] = strLogType;
                    ClsFileBufferRead.strFileName[ClsFileBufferRead.numMax] = strFileName;
                    ClsFileBufferRead.strLogLine[ClsFileBufferRead.numMax] = strLogLine;
                    break;
                }
                if (ClsFileBufferRead.numMax == MaxArray.numMaxClsFileBufferRead - 1)
                {
                    //ファイル読込配列はあふれている
                    strAddMode = "Push Out";
                    var strPrintPushOut = "";
                    strPrintPushOut += "ファイル読込配列 " + strAddMode + "\n";
                    strPrintPushOut += "\tstrLogType[" + 0 + "] = " + ClsFileBufferRead.strLogType[0];
                    strPrintPushOut += "\tstrLogLine[" + 0 + "] = " + ClsFileBufferRead.strLogLine[0];
                    strPrintPushOut += "\tstrFileName[" + 0 + "] = " + ClsFileBufferRead.strFileName[0];
                    strPrintPushOut += "\n";
                    //logger.print(strPrintPushOut);
                    for (int i = 1; i <= ClsFileBufferRead.numMax; i++)
                    {
                        ClsFileBufferRead.strLogType[i - 1] = ClsFileBufferRead.strLogType[i];
                        ClsFileBufferRead.strFileName[i - 1] = ClsFileBufferRead.strFileName[i];
                        ClsFileBufferRead.strLogLine[i - 1] = ClsFileBufferRead.strLogLine[i];
                    }
                    ClsFileBufferRead.strLogType[ClsFileBufferRead.numMax] = strLogType;
                    ClsFileBufferRead.strFileName[ClsFileBufferRead.numMax] = strFileName;
                    ClsFileBufferRead.strLogLine[ClsFileBufferRead.numMax] = strLogLine;
                    break;
                }
                //ファイル読込配列は普通に登録できる。
                strAddMode = "Push";
                ClsFileBufferRead.numMax++;
                ClsFileBufferRead.strLogType[ClsFileBufferRead.numMax] = strLogType;
                ClsFileBufferRead.strFileName[ClsFileBufferRead.numMax] = strFileName;
                ClsFileBufferRead.strLogLine[ClsFileBufferRead.numMax] = strLogLine;
            }
            while (false);

            var strPrint = "";
            strPrint += "ファイル読込配列 " + strAddMode + "\n";
            for (var i = 0; i <= MaxArray.numMaxClsWaitingToTranslate - 1; i++)
            {
                if (String.IsNullOrEmpty(ClsFileBufferRead.strLogLine[i]))
                {
                    break;
                }
                strPrint += "\tstrLogType[" + i + "] = " + ClsFileBufferRead.strLogType[i];
                strPrint += "\tstrLogLine[" + i + "] = " + ClsFileBufferRead.strLogLine[i];
                strPrint += "\tstrFileName[" + i + "] = " + ClsFileBufferRead.strFileName[i];
                strPrint += "\n";
            }
            //logger.print(strPrint);
        }
        public void ArrayClsListBoxPreWriteBufferSilde()
        {
            for (int i = 1; i <= ClsListBoxPreWriteBuffer.numMax; i++)
            {
                ClsListBoxPreWriteBuffer.dtmLogDate[i - 1] = ClsListBoxPreWriteBuffer.dtmLogDate[i];
                ClsListBoxPreWriteBuffer.strMsgType[i - 1] = ClsListBoxPreWriteBuffer.strMsgType[i];
                ClsListBoxPreWriteBuffer.bDoTranslationFlg[i - 1] = ClsListBoxPreWriteBuffer.bDoTranslationFlg[i];
                ClsListBoxPreWriteBuffer.strLogType[i - 1] = ClsListBoxPreWriteBuffer.strLogType[i];
                ClsListBoxPreWriteBuffer.strDateTime[i - 1] = ClsListBoxPreWriteBuffer.strDateTime[i];
                ClsListBoxPreWriteBuffer.strMemberID[i - 1] = ClsListBoxPreWriteBuffer.strMemberID[i];
                ClsListBoxPreWriteBuffer.strMemberName[i - 1] = ClsListBoxPreWriteBuffer.strMemberName[i];
                ClsListBoxPreWriteBuffer.strMessage[i - 1] = ClsListBoxPreWriteBuffer.strMessage[i];
                ClsListBoxPreWriteBuffer.numListBox[i - 1] = ClsListBoxPreWriteBuffer.numListBox[i];
                ClsListBoxPreWriteBuffer.strOriginalMessage[i - 1] = ClsListBoxPreWriteBuffer.strOriginalMessage[i];
                ClsListBoxPreWriteBuffer.strTranslatedMessage[i - 1] = ClsListBoxPreWriteBuffer.strTranslatedMessage[i];
                
                //public static DateTime[] dtmLogDate = new DateTime[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMsgType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static bool[] bDoTranslationFlg = new bool[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strLogType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strDateTime = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMemberID = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMemberName = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strOriginalMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static string[] strTranslatedMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
                //public static int[] numListBox = new int[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];

                ClsListBoxPreWriteBuffer.dtmLogDate[i] = DateTime.Parse("1901/02/19 00:00:00");
                ClsListBoxPreWriteBuffer.strMsgType[i] = "";
                ClsListBoxPreWriteBuffer.bDoTranslationFlg[i] = false;
                ClsListBoxPreWriteBuffer.strLogType[i] = "";
                ClsListBoxPreWriteBuffer.strDateTime[i] = "";
                ClsListBoxPreWriteBuffer.strMemberID[i] = "";
                ClsListBoxPreWriteBuffer.strMemberName[i] = "";
                ClsListBoxPreWriteBuffer.strMessage[i] = "";
                ClsListBoxPreWriteBuffer.numListBox[i] = 0;
                ClsListBoxPreWriteBuffer.strOriginalMessage[i] = "";
                ClsListBoxPreWriteBuffer.strTranslatedMessage[i] = "";
            }
        }
        private void ArrayErasure()
        {
            for (var i = 0; i <= MaxArray.numMaxArrayClsListBoxPreWriteBuffer - 2; i++)
            {
                ////削除対象のデータが無効ならば、それ以上削除しない。
                //if (ClsListBoxPreWriteBuffer.numTaskNo[i] == 0)
                //{
                //    break;
                //}
                ClsListBoxPreWriteBuffer.dtmLogDate[i] = DateTime.Parse("0001/01/01 0:00:00");
                ClsListBoxPreWriteBuffer.strMsgType[i] = "";
                ClsListBoxPreWriteBuffer.bDoTranslationFlg[i] = false;
                ClsListBoxPreWriteBuffer.strLogType[i] = "";
                ClsListBoxPreWriteBuffer.strDateTime[i] = "";
                ClsListBoxPreWriteBuffer.strMemberID[i] = "";
                ClsListBoxPreWriteBuffer.strMemberName[i] = "";
                ClsListBoxPreWriteBuffer.strMessage[i] = "";
                ClsListBoxPreWriteBuffer.strOriginalMessage[i] = "";
                ClsListBoxPreWriteBuffer.strTranslatedMessage[i] = "";
                ClsListBoxPreWriteBuffer.numListBox[i] = 0;
            }
            ClsListBoxPreWriteBuffer.numMin = -1;
            ClsListBoxPreWriteBuffer.numMax = -1;
            for (var i = 0; i <= MaxArray.numMaxClsWaitingToTranslate - 1; i++)
            {
                //削除対象のデータが無効ならば、それ以上削除しない。
                if (ClsWaitingToTranslate.numTaskNo[i] == 0)
                {
                    break;
                }
                //<todo>
                ClsWaitingToTranslate.numTaskNo[i] = ClsWaitingToTranslate.numTaskNo[i + 1];
                ClsWaitingToTranslate.dtmLogDate[i] = ClsWaitingToTranslate.dtmLogDate[i + 1];
                ClsWaitingToTranslate.strLogType[i] = ClsWaitingToTranslate.strLogType[i + 1];
                ClsWaitingToTranslate.strSource[i] = ClsWaitingToTranslate.strSource[i + 1];
                ClsWaitingToTranslate.strLogLine[i] = ClsWaitingToTranslate.strLogLine[i + 1];
                ClsWaitingToTranslate.strFileName[i] = ClsWaitingToTranslate.strFileName[i + 1];
            }
            for (var i = 0; i <= MaxArray.numMaxClsFileBufferRead - 1; i++)
            {
                ////削除対象のデータが無効ならば、それ以上削除しない。
                //if (ClsListBoxPreWriteBuffer.numTaskNo[i] == 0)
                //{
                //    break;
                //}
                ClsFileBufferRead.strLogType[i] = "";
                ClsFileBufferRead.strFileName[i] = "";
                ClsFileBufferRead.strLogLine[i] = "";
            }
            ClsFileBufferRead.bExclusiveFlg = false;
            ClsFileBufferRead.numMin = -1;
            ClsFileBufferRead.numMax = -1;
        }
        //************************************************************************************************************************************************************************************************
        //配列操作関数群終了
        //************************************************************************************************************************************************************************************************


        //************************************************************************************************************************************************************************************************
        //Windows APIコール開始
        //************************************************************************************************************************************************************************************************
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

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
        //[return: MarshalAs(UnmanagedType.Bool)]
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
        //************************************************************************************************************************************************************************************************
        //Windows APIコール終了
        //************************************************************************************************************************************************************************************************



        //************************************************************************************************************************************************************************************************
        //イベントハンドラ
        //************************************************************************************************************************************************************************************************
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileUtil.SetValue(strEnvironment, "Language_GP", Language_GP.DICTIONARY[comboBox1.SelectedItem.ToString()], FileUtil.strIniFileName);
            Label.initializeForm1(this);
            Button1.Text = Button1.Text.Replace("XX", GetButtonLabel());
            Button1.Text = Button1.Text.Replace("X", GetButtonLabel());
            Button2.Text = Button2.Text.Replace("XX", GetButtonLabel());
            Button2.Text = Button2.Text.Replace("X", GetButtonLabel());
            ArrayErasure();
            listViewHeaderReset();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileUtil.SetValue(strEnvironment, "Language_GP", Language_GP.DICTIONARY[comboBox1.SelectedItem.ToString()], FileUtil.strIniFileName);
            Label.initializeForm1(this);
            Button1.Text = Button1.Text.Replace("XX", GetButtonLabel());
            Button1.Text = Button1.Text.Replace("X", GetButtonLabel());
            Button2.Text = Button2.Text.Replace("XX", GetButtonLabel());
            Button2.Text = Button2.Text.Replace("X", GetButtonLabel());
        }
        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // 右クリックされた？
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // マウス座標から選択すべきアイテムのインデックスを取得
                int index = listBox1.IndexFromPoint(e.Location);

                // インデックスが取得できたら
                if (index >= 0)
                {
                    // すべての選択状態を解除してから
                    listBox1.ClearSelected();

                    // アイテムを選択
                    listBox1.SelectedIndex = index;

                    // コンテキストメニューを表示
                    Point pos = listBox1.PointToScreen(e.Location);
                    contextMenuStrip1.Show(pos);
                }
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //var strRow = "";
            var strALL = "";
            var strID = "";
            var strName = "";
            var strText = "";
            GetListBoxRow(listBox1.SelectedIndex.ToString(), ref strALL, ref strID, ref strName, ref strText);
            Clipboard.SetDataObject(strText, true);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            doDownloadPSOChatLog();
        }
        private void frmMain_Resize(object sender, EventArgs e)
        {
            DoResize();
        }
        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            DoResize();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debugger.Break();
            var bBreakFlg = false;
            Button1.Enabled = false;//再入防止兼再翻訳防止
            do
            {
                if (textBox1.Text.Equals(""))
                {
                    break;
                }

                var strFirstLanguage_GP = Language_GP.myLanguage_GP();
                var strLangSource = "";
                do
                {
                    if (strFirstLanguage_GP.ToUpper() == "ZH-CN")
                    {
                        strFirstLanguage_GP = "ZH";
                    }
                    if (strFirstLanguage_GP.ToUpper() == "ZH-TW")
                    {
                        strFirstLanguage_GP = "ZH";
                    }
                    strLangSource = strFirstLanguage_GP;
                }
                while (false);

                var strChatLanguage_GP = gfGetChatLang();
                var strLangTarget = "";
                do
                {
                    if (strChatLanguage_GP.ToUpper() == "ZH-CN")
                    {
                        strChatLanguage_GP = "ZH";
                    }
                    if (strChatLanguage_GP.ToUpper() == "ZH-TW")
                    {
                        strChatLanguage_GP = "ZH";
                    }
                    strLangTarget = strChatLanguage_GP;
                }
                while (false);

                var strTranslator = "";

                var strReturn1 = "";
                var strReturn2 = "";
                var strTmp = "";
                var strSource = "";
                strSource = textBox1.Text.Trim();

                //メイン翻訳
                strReturn1 = SyncCallTranslation(DateTime.Now, strSource, strLangTarget, strLangSource, ref strTranslator, "XtFL", ref bBreakFlg);
                //リバース翻訳
                strTmp = strLangTarget;
                strLangTarget = strLangSource;
                strLangSource = strTmp;
                strSource = strReturn1;

                strReturn2 = SyncCallTranslation(DateTime.Now, strSource, strLangTarget, strLangSource, ref strTranslator, "FLtX", ref bBreakFlg);
                if (comboBox2.Text.Equals("Cipher") == true)
                {
                    strReturn1 = ConvertCyrillicToCipher(DateTime.Now, "Screen", strReturn1);
                }
                textBox1.Text = strReturn1;
                textBox2.Text = strReturn2;
            } while (false);
            Button1.Enabled = true;//再入防止解除
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debugger.Break();
            do
            {
                if (textBox2.Text.Equals(""))
                {
                    break;
                }
                //var bHitFlg = false;
                var bBreakFlg = false;
                Button2.Enabled = false;//再入防止

                var strFirstLanguage_GP = Language_GP.myLanguage_GP();
                var strLangTarget = "";
                do
                {
                    if (strFirstLanguage_GP.ToUpper() == "ZH-CN")
                    {
                        strFirstLanguage_GP = "ZH";
                    }
                    if (strFirstLanguage_GP.ToUpper() == "ZH-TW")
                    {
                        strFirstLanguage_GP = "ZH";
                    }
                    strLangTarget = strFirstLanguage_GP;
                }
                while (false);

                var strChatLanguage_GP = gfGetChatLang();
                var strLangSource = "";
                do
                {
                    if (strChatLanguage_GP.ToUpper() == "ZH-CN")
                    {
                        strChatLanguage_GP = "ZH";
                    }
                    if (strChatLanguage_GP.ToUpper() == "ZH-TW")
                    {
                        strChatLanguage_GP = "ZH";
                    }
                    strLangSource = strChatLanguage_GP;
                }
                while (false);

                var strTranslator = "";

                var strReturn1 = "";
                //var strReturn2 = "";
                //var strTmp = "";
                var strSource = "";
                strSource = textBox2.Text.Trim();

                //メイン翻訳
                strReturn1 = SyncCallTranslation(DateTime.Now, strSource, strLangTarget, strLangSource, ref strTranslator, "XtFL", ref bBreakFlg);
                textBox2.Text = strReturn1;

            } while (false);
            Button2.Enabled = true;//再入防止解除
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            Button3.Enabled = false;//再入防止兼再翻訳防止
            SendTextChatLogToPSOBB(textBox1.Text.Trim());
            textBox1.Text = "";
            Button3.Enabled = true;//再入防止解除
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Array " + ClsListBoxPreWriteBuffer.numMax + " = " + ClsListBoxPreWriteBuffer.strMessage[ClsListBoxPreWriteBuffer.numMax]);
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPathBefore = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);

            frmSetting frmSetting = new frmSetting();
            frmSetting.ShowDialog();
            ListControlUpdate();
            ListBoxSetting();
            ListViewSetting();

            var strInstallPathAfter = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);

            if (strInstallPathBefore.Equals(strInstallPathAfter) == false)
            {
                WatcherEnd();
                gfWatcherStart();
            }
            OnLoad(e);
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            Button5.Enabled = false;//再入防止兼再翻訳防止

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strCheckMsgOmitGoToLobby = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "CheckMsgOmitGoToLobby", strIniFileName);

            var strLangTarget = Language_GP.myLanguage_GP();
            do
            {
                if (strCheckMsgOmitGoToLobby.Equals("true") == false)
                {
                    if (strLangTarget.Equals("JA") == true)
                    {
                        DialogResult dr1 = MessageBox.Show("ロビーに転送します。\r\nよろしいですか？\r\n\r\nフロアにあるアイテム等は消えてしまいます。", "確認", MessageBoxButtons.OKCancel);
                        if (dr1 != System.Windows.Forms.DialogResult.OK)
                        {
                            break;
                        }
                    }
                    else
                    {
                        DialogResult dr2 = MessageBox.Show("Transfer to lobby. \r\nAre you sure ? \r\n\r\nItems on the floor will disappear.", "confirmation", MessageBoxButtons.OKCancel);
                        if (dr2 != System.Windows.Forms.DialogResult.OK)
                        {
                            break;
                        }
                    }
                }
                SendTextChatLogToPSOBB("/lobby");
            }
            while (false);

            Button5.Enabled = true;//再入防止解除
        }
        private void button17_Click(object sender, EventArgs e)
        {
            button17.Enabled = false;//再入防止
            this.Visible = false;

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPathBefore = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);

            frmSearchIniFile frmSearchIniFile = new frmSearchIniFile();
            frmSearchIniFile.ShowDialog();
            ListControlUpdate();
            ListBoxSetting();
            ListViewSetting();

            var strInstallPathAfter = ClsCallApiGetPrivateProfile.CallApiGetValueString(strEnvironment, "InstallPath", strIniFileName);

            if (strInstallPathBefore.Equals(strInstallPathAfter) == false)
            {
                WatcherEnd();
                gfWatcherStart();
            }
            OnLoad(e);

            this.Visible = true;
            button17.Enabled = true;//再入防止解除
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var strParam = "";
            var strTmp = "";
            var strComparison = "";
            var strExt = "";
            var numCount = 0;
            var bErrFlg = false;
            do//空ループ
            {
                if (listBox1.Items.Count == 0)
                {
                    break;
                }
                //Console.WriteLine(strResult);
                if (listBox1.SelectedItem == null)
                {
                    break;
                }
                strTmp = listBox1.SelectedItem.ToString();
                for (int i = strTmp.Length - 1; 0 <= i; i--)
                {
                    strComparison = strTmp.Substring(i, 1);
                    if (Regex.IsMatch(strComparison, "\\t", RegexOptions.Singleline))
                    {
                        //stop();
                        break;
                    }
                    if (Regex.IsMatch(strComparison, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numCount == 0)
                        {
                            strExt = strComparison + strExt;
                        }
                    }
                }
                if (Regex.IsMatch(strTmp, ".*HBR Counts.*", RegexOptions.Singleline))
                {
                    textBox1.Text = strExt;
                    break;
                }
                if (Regex.IsMatch(strTmp, ".*Material.*", RegexOptions.Singleline))
                {
                    textBox1.Text = strExt;
                    break;
                }

                frmChatDetail frmChatDetail = new frmChatDetail();
                //子画面のプロパティに値をセットする
                //frmChatDetail.strParam = listBox1.Items.Count.ToString();
                try
                {
                    strParam = listBox1.SelectedIndices[0].ToString();
                }
                catch
                {
                    bErrFlg = true;
                }
                if (bErrFlg == false)
                {
                    if (CheckOpenForms("ChatDetail") == false)
                    {
                        frmChatDetail.strParam = strParam;
                        frmChatDetail.Owner = this;
                        frmChatDetail.ShowDialog();
                    }
                    else
                    {
                        FormsActivate("ChatDetail");
                    }
                }
            }
            while (false);//空ループなので、必ず一回で処理を抜ける
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var strParam = "";
            var strTmp = "";
            var strComparison = "";
            var strExt = "";
            var numCount = 0;
            var bErrFlg = false;
            do//空ループ
            {
                if (listView1.Items.Count == 0)
                {
                    break;
                }
                //Console.WriteLine(strResult);
                if (listView1.SelectedItems == null)
                {
                    break;
                }
                strTmp = listView1.SelectedItems.ToString();
                for (int i = strTmp.Length - 1; 0 <= i; i--)
                {
                    strComparison = strTmp.Substring(i, 1);
                    if (Regex.IsMatch(strComparison, "\\t", RegexOptions.Singleline))
                    {
                        //stop();
                        break;
                    }
                    if (Regex.IsMatch(strComparison, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numCount == 0)
                        {
                            strExt = strComparison + strExt;
                        }
                    }
                }
                if (Regex.IsMatch(strTmp, ".*HBR Counts.*", RegexOptions.Singleline))
                {
                    textBox1.Text = strExt;
                    break;
                }
                if (Regex.IsMatch(strTmp, ".*Material.*", RegexOptions.Singleline))
                {
                    textBox1.Text = strExt;
                    break;
                }

                frmChatDetail frmChatDetail = new frmChatDetail();
                //子画面のプロパティに値をセットする
                //frmChatDetail.strParam = listView1.Items.Count.ToString();
                try
                {
                    strParam = listView1.SelectedIndices[0].ToString();
                }
                catch
                {
                    bErrFlg = true;
                }
                if (bErrFlg == false)
                {
                    if (CheckOpenForms("ChatDetail") == false)
                    {
                        frmChatDetail.strParam = strParam;
                        frmChatDetail.Owner = this;
                        frmChatDetail.ShowDialog();
                    }
                    else
                    {
                        FormsActivate("ChatDetail");
                    }
                }
            }
            while (false);//空ループなので、必ず一回で処理を抜ける
        }
        private void button7_Click(object sender, EventArgs e)
        {
            var iBtnNum = 0;
            button7.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button7.Enabled = true;
            button7.Focus();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            var iBtnNum = 1;
            button8.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button8.Enabled = true;
            button8.Focus();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            var iBtnNum = 2;
            button9.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button9.Enabled = true;
            button9.Focus();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            var iBtnNum = 3;
            button10.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button10.Enabled = true;
            button10.Focus();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            var iBtnNum = 4;
            button11.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button11.Enabled = true;
            button11.Focus();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            var iBtnNum = 5;
            button12.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button12.Enabled = true;
            button12.Focus();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            var iBtnNum = 6;
            button13.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button13.Enabled = true;
            button13.Focus();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            var iBtnNum = 7;
            button14.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button14.Enabled = true;
            button14.Focus();
        }
        private void button15_Click(object sender, EventArgs e)
        {
            var iBtnNum = 8;
            button15.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button15.Enabled = true;
            button15.Focus();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            var iBtnNum = 9;
            button16.Enabled = false;
            SendTextShortTextRegistration(iBtnNum);
            button16.Enabled = true;
            button16.Focus();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            var strRetNo = "";
            var iRetNo = 0;
            var strRetText = "";
            //stop();
            do
            {
                //キーが押されたか調べる
                if (e.KeyData != System.Windows.Forms.Keys.Enter)
                {
                    break;
                }
                var strCheck = "";
                strCheck = textBox1.Text;
                //stop();
                //「/set_00_msg」
                var myCheck02 = Regex.IsMatch(strCheck, "^\\x2F[sS][eE][tT]\\x20[sS]\\x20[0-9][0-9]\\x20.*", RegexOptions.Singleline);
                //「/set_0_msg」
                var myCheck01 = Regex.IsMatch(strCheck, "^\\x2F[sS][eE][tT]\\x20[sS]\\x20[0-9]\\x20.*", RegexOptions.Singleline);
                if ((myCheck02 || myCheck01) == true)
                {
                    if (myCheck02 == true)
                    {
                        strRetNo = strCheck.Substring(7, 2);
                        strRetText = strCheck.Substring(10);
                    }
                    if (myCheck01 == true)
                    {
                        strRetNo = strCheck.Substring(7, 1);
                        strRetText = strCheck.Substring(9);
                    }
                    try
                    {
                        iRetNo = Int32.Parse(strRetNo);
                    }
                    catch
                    {
                        break;
                    }
                    //stop();
                    ShortTextRegistrationWrite(iRetNo, strRetText);
                    ShortTextRegistrationSave();
                    ShortTextRegistrationLoad();
                    textBox1.Text = "";
                    break;
                }
                //「/s_00」
                var myCheck12 = Regex.IsMatch(strCheck, "^\\x2F[sS]\\x20[0-9][0-9]", RegexOptions.Singleline);
                var myCheck11 = Regex.IsMatch(strCheck, "^\\x2F[sS]\\x20[0-9]", RegexOptions.Singleline);
                if ((myCheck12 || myCheck11) == false)
                {
                    break;
                }
                if (myCheck12 == true)
                {
                    strRetNo = strCheck.Substring(3, 2);
                }
                if (myCheck11 == true)
                {
                    strRetNo = strCheck.Substring(3, 1);
                }
                try
                {
                    iRetNo = Int32.Parse(strRetNo);
                }
                catch
                {
                    break;
                }
                SendTextShortTextRegistration(iRetNo);
                textBox1.Text = "";
            } while (false);
        }
        //************************************************************************************************************************************************************************************************
        //イベントハンドラ終了
        //************************************************************************************************************************************************************************************************
        public void stop()
        {
            System.Diagnostics.Debugger.Break();
        }
    }
    public struct tyTransData
    {
        public DateTime dtmAddLogDate;
        public string strSource;
        public string strLangTarget;
        public string strLangSource;
        public string strTranslator;
        public string strLogType;
        public bool bBreakFlg;
    }
    public struct tyDictionarySlang
    {
        public string Slang;
        public string Dictionary;
    }
    public struct tyShortTextRegistration
    {
        public int iNum;
        public string ShortText;
    }
    public class ClsDictionarySlang
    {
        List<tyDictionarySlang> DictionarySlang = new List<tyDictionarySlang>();
    }
    public static class ClsListBoxPreWriteBuffer
    {
        //public static readonly string[] strFileName = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static DateTime[] dtmLogDate = new DateTime[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strMsgType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static bool[] bDoTranslationFlg = new bool[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strLogType = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strDateTime = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strMemberID = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strMemberName = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strOriginalMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static string[] strTranslatedMessage = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static int[] numListBox = new int[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static int numMin = -1;
        public static int numMax = -1;
    }
    public static class ClsWaitingToTranslate
    {
        public static DateTime[] dtmLogDate = new DateTime[MaxArray.numMaxClsWaitingToTranslate];
        public static int[] numTaskNo = new int[MaxArray.numMaxClsWaitingToTranslate];
        public static string[] strLogType = new string[MaxArray.numMaxClsWaitingToTranslate];
        public static string[] strLogLine = new string[MaxArray.numMaxClsWaitingToTranslate];
        public static string[] strSource = new string[MaxArray.numMaxClsWaitingToTranslate];
        public static string[] strFileName = new string[MaxArray.numMaxClsWaitingToTranslate];
        //public static bool bExclusiveFlg = false;
        //public static bool bReEntryWait = false;
    }
    public static class ClsFileBufferRead
    {
        //public static DateTime[] dtmLogDate = new DateTime[MaxArray.numMaxClsFileBufferRead];
        public static string[] strLogType = new string[MaxArray.numMaxClsFileBufferRead];
        public static string[] strFileName = new string[MaxArray.numMaxClsFileBufferRead];
        public static string[] strLogLine = new string[MaxArray.numMaxClsFileBufferRead];
        public static bool bExclusiveFlg = false;
        public static int numMin = -1;
        public static int numMax = -1;
    }
    static class MaxArray
    {
        public const int numMaxArrayClsListBoxPreWriteBuffer = 100;
        public const int numMaxClsWaitingToTranslate = 100;
        public const int numMaxClsFileBufferRead = 100;
    }
    public static class ClsLastCallVar
    {
        public static string strFullPathNormal = "";
        public static string strFileNameNormal = "";
        public static string strFullPathAddons = "";
        public static string strFileNameAddons = "";
        public static string strFullPathGuild = "";
        public static string strFileNameGuild = "";
        public static string strFullPathSimpleMail = "";
        public static string strFileNameSimpleMail = "";
    }
    public static class ClsChatDetailReadFile
    {
        public static string strFullPathNormal = "";
    }
}
internal class Value
{
    public static string strPSOChatLogVersionCurrent = "0.76";
    public static long lIndexNoCurrent = 0;
    public const string strMDBFileName = "PSOChatLog";
#if TexTra
    public const string strEnvironment = "PSOChatLogTexTra";
#else
    public const string strEnvironment = "PSOChatLog";
#endif
}
