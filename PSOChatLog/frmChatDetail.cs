using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSOChatLog
{
    public partial class frmChatDetail : Form
    {
        public frmChatDetail()
        {
            InitializeComponent();
            Label.initializeChatDetail(this);
        }
        private String _strParam;
        public String strParam
        {
            get
            {
                return _strParam;
            }
            set
            {
                _strParam = value;
            }
        }
        public struct stChatData
        {
            public string strDateTime;
            public string strMemberID;
            public string strMember;
            public string strChatLog;
        }
        public struct stChatCount
        {
            public string strMemberID;
            public string strMemberName;
            public DateTime dtmLogDate;
            public int numChatCount;
        }
        [System.Security.Permissions.UIPermission(
            System.Security.Permissions.SecurityAction.Demand,
            Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        private void ChatDetail_Load(object sender, EventArgs e)
        {
            var myCheck = Regex.IsMatch(Value.strEnvironment, ".*TexTra.*", RegexOptions.Singleline);
            do
            {
                if (myCheck == true)
                {
                    //TexTra
                    this.Text = this.Text + " TexTra® Powered by NICT";
                    this.NICT.Visible = true;
                    break;
                }
                if (myCheck == false)
                {
                    //従来Exe
                    this.NICT.Visible = false;
                    break;
                }
            } while (false);


            radioButton1.Checked = true;
            txtRow.Text = (Int32.Parse(strParam) + 1).ToString();
            lstCharacterList.UseCustomTabOffsets = true;
            lstCharacterList.CustomTabOffsets.Clear();
            //lstCharacterList.CustomTabOffsets.AddRange(new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190 });
            //lstCharacterList.CustomTabOffsets.AddRange(new int[] { 50, 115, 180 });
            lstCharacterList.CustomTabOffsets.AddRange(new int[] { 80, 120, 160, 170, 180, 190 });
            var strLangTarget = Language_GP.myLanguage_GP();
            do
            {
                if (strLangTarget.Equals("JA") == true)
                {
                    List<string> lt = new List<string> { "前回の解析後","今日のログ", "昨日からのログ", "今週のログ", "今月のログ", "全てのログ" };
                    cboSelectAnalysis.Text = "前回の解析後";
                    cboSelectAnalysis.Items.AddRange(lt.ToArray());
                    break;
                }
                if (strLangTarget.Equals("EN") == true)
                {
                    List<string> lt = new List<string> { "Since last", "Today's Log", "from yesterday", "This week's", "This month's", "ALL" };
                    cboSelectAnalysis.Text = "Since last";
                    cboSelectAnalysis.Items.AddRange(lt.ToArray());
                    break;
                }
                if (strLangTarget.Equals("") == false)
                {
                    List<string> lt = new List<string> { "Since last", "Today's Log", "from yesterday", "This week's", "This month's", "ALL" };
                    cboSelectAnalysis.Text = "Since last";
                    cboSelectAnalysis.Items.AddRange(lt.ToArray());
                }
            }
            while (false);

            var strIniFileName = ".\\" + ".\\" + Value.strEnvironment + ".ini" + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strBackColorRed = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "BackColorRed", strIniFileName);
            var strBackColorGreen = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "BackColorGreen", strIniFileName);
            var strBackColorBlue = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "BackColorBlue", strIniFileName);
            var strFontName = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "FontName", strIniFileName);
            var strFontSize = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "FontSize", strIniFileName);

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
            txtRow.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtALL.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtID.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtName.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtText.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtInputID.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            cboSelectAnalysis.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtNickName.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            lstCharacterList.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));
            txtRemarks.BackColor = Color.FromArgb(Int32.Parse(strBackColorRed), Int32.Parse(strBackColorGreen), Int32.Parse(strBackColorBlue));

            gfViewUpdate();
        }
        private void ChatDetail_Close()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        protected override void OnLoad(EventArgs e)
        {
            SetTopMost();
            base.OnLoad(e);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
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
            var strFloating = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "Floating", strIniFileName);
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

        private void btnUpMax_Click(object sender, EventArgs e)
        {
            var strBTType = "UpMax";
            btnMoveClick(strBTType);
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            var strBTType = "Up";
            btnMoveClick(strBTType);
        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            var strBTType = "Down";
            btnMoveClick(strBTType);
        }
        private void btnDownMax_Click(object sender, EventArgs e)
        {
            var strBTType = "DownMax";
            btnMoveClick(strBTType);
        }
        private void btnMoveClick(string strBTType)
        {
            var strReturn = "";
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strViewControl = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "ViewControl", strIniFileName);

            var strRetListView = "";
            var strRetListBox = "";
            stop();
            ((frmMain)this.Owner).gfChangeSelectListView(strBTType, ref strRetListView);
            ((frmMain)this.Owner).gfChangeSelectListBox(strBTType, ref strRetListBox);
            do
            {
                if (strViewControl == "ListView")
                {
                    strReturn = strRetListView;
                    break;
                }
                if (strViewControl == "ListBox")
                {
                    strReturn = strRetListBox;
                    break;
                }
                strReturn = strRetListView;
                break;
            } while (false);
            txtRow.Text = (Int32.Parse(strReturn) + 1).ToString();
            radioButton1.Checked = true;
            gfViewUpdate();
        }
        private void gfViewUpdate()
        {
            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strViewControl = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "ViewControl", strIniFileName);

            var strRow = "";
            var strALL = "";
            var strID = "";
            var strName = "";
            var strText = "";
            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            string strDBSourceMain = "Provider=Microsoft.Jet.OLEDB.4.0;"    //プロバイダ (32bitのみ)
                + "Data Source=" + strSourceFileName + ";"                  //ファイル名の指定
                + "Jet OLEDB:Engine Type=5";                                //Engine Type は 5 

            //Logger logger = Logger.GetInstance();
            //logger.print("gfViewUpdate 消去");
            stop();
            do
            {
                strRow = (Int32.Parse(txtRow.Text) - 1).ToString();
                do
                {
                    if (strViewControl == "ListView")
                    {
                        ((frmMain)this.Owner).GetListViewRow(strRow, ref strALL, ref strID, ref strName, ref strText);
                        break;
                    }
                    if (strViewControl == "ListBox")
                    {
                        ((frmMain)this.Owner).GetListBoxRow(strRow, ref strALL, ref strID, ref strName, ref strText);
                        break;
                    }
                    ((frmMain)this.Owner).GetListViewRow(strRow, ref strALL, ref strID, ref strName, ref strText);
                    break;
                } while (false);
                txtRow.Text = (Int32.Parse(strRow) + 1).ToString();
                txtALL.Text = strALL;
                txtID.Text = strID;
                txtName.Text = strName;
                txtText.Text = strText;
                txtNickName.Text = "";
                txtRemarks.Text = "";
                if (File.Exists(strSourceFileName) == false)
                {
                    break;
                }
                gfViewMember(strDBSourceMain, strSourceFileName);
            }
            while (false);
        }
        private void cmdALL_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtALL.Text, true);
        }
        private void cmdID_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtID.Text, true);
        }
        private void cmdName_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtName.Text, true);
        }
        private void cmdText_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txtText.Text, true);
        }
        private void btnIDSerch_Click(object sender, EventArgs e)
        {
            var strRow = "";
            var strALL = "";
            var strID = "";
            var strName = "";
            var strText = "";
            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            string strDBSourceMain = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                "Data Source=" + strSourceFileName + ";" +     //ファイル名の指定
                                "Jet OLEDB:Engine Type=5";            //Engine Type は 5 
            var myCheck = false;
            do
            {
                strRow = (Int32.Parse(txtRow.Text) - 1).ToString();
                ((frmMain)this.Owner).GetListBoxRow(strRow, ref strALL, ref strID, ref strName, ref strText);
                txtALL.Text = strALL;
                txtID.Text = strID;
                txtName.Text = strName;
                txtText.Text = strText;
                txtNickName.Text = "";
                txtRemarks.Text = "";
                if (File.Exists(strSourceFileName) == false)
                {
                    break;
                }

                myCheck = Regex.IsMatch(txtInputID.Text, "\\d{8}", RegexOptions.Singleline);
                if (myCheck == false)
                {
                    break;
                }
                radioButton2.Checked = true;
                gfViewMember(strDBSourceMain, strSourceFileName);
            }
            while (false);
        }
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private HandlingMDB handlingMDB = new HandlingMDB();
        private async void btnLogAnalysis_Click(object sender, EventArgs e)
        {

            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            string strDBSourceMain = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                "Data Source=" + strSourceFileName + ";" +     //ファイル名の指定
                                "Jet OLEDB:Engine Type=5";            //Engine Type は 5 
            string strDBSourceCD = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                    "Data Source=" + strSourceFileName + ".CompactDatabase" + ";" +     //ファイル名の指定
                                    "Jet OLEDB:Engine Type=5";            //Engine Type は 5 
            //表示切替
            clsStopFlg.bStopFlg = false;
            btnLogAnalysis.Visible = false;
            cboSelectAnalysis.Visible = false;
            progressBar1.Visible = true;
            btnSTOP.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
            progressBar1.Visible = true;
            progressBar1.Maximum = 100;
            timer1.Interval = 100;
            timer1.Start();

            //キャンセルトークン取得
            CancellationToken cancelToken = new();
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            cancelToken = cancellationTokenSource.Token;

            //実処理
            IntPtr hWindowHandleOwnerWindow = this.Owner.Handle;
            IntPtr hWindowHandleCallWindow = this.Handle;
            handlingMDB.SetAwaitParameter(cboSelectAnalysis.Text, hWindowHandleOwnerWindow, hWindowHandleCallWindow);
            await handlingMDB.DoAnalysisAsync(cancelToken);

            //キャンセルトークンの後始末
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;

            ////ニックネーム、備考、キャラクターリストを再表示する
            gfViewMember(strDBSourceMain, strSourceFileName);

            //表示切替
            timer1.Stop();
            progressBar1.Visible = false;
            Cursor.Current = Cursors.Default;
            btnSTOP.Visible = false;
            progressBar1.Visible = false;
            cboSelectAnalysis.Visible = true;
            btnLogAnalysis.Visible = true;
        }
        private void btnSTOP_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void gfMDBFileCreate(string strDBSource)
        {
            ADOX.Catalog catMakeMDB = new ADOX.Catalog();
            catMakeMDB.Create(strDBSource);
            catMakeMDB = null;
        }
        private void gfMDBTableCreate(string strDBSource)
        {
            //MDBファイルに接続する
            ADOX.Catalog catMakeTable = new ADOX.Catalog();
            catMakeTable.let_ActiveConnection(strDBSource);

            //テーブルの作成
            ADOX.Table tblMember = new ADOX.Table();
            tblMember.ParentCatalog = catMakeTable;
            tblMember.Name = "tblMember";                              //テーブル名

            ADOX.Column tblMemberColumnId = new ADOX.Column();
            tblMemberColumnId.ParentCatalog = catMakeTable;
            tblMemberColumnId.Type = ADOX.DataTypeEnum.adInteger;                        //事前に設定しないと"AutoIncrement"でエラーとなる
            tblMemberColumnId.Name = "ID";                                               //列名
            tblMemberColumnId.Properties["Jet OLEDB:Allow Zero Length"].Value = false;   //空白を許可しない
            tblMemberColumnId.Properties["AutoIncrement"].Value = true;                  //自動採番とする
            tblMember.Columns.Append(tblMemberColumnId, ADOX.DataTypeEnum.adInteger);        //形を指定して追加
                                                                                             //列名２
            ADOX.Column tblMemberColumnMemberID = new ADOX.Column();
            tblMemberColumnMemberID.ParentCatalog = catMakeTable;
            tblMemberColumnMemberID.Type = ADOX.DataTypeEnum.adInteger;
            tblMemberColumnMemberID.Name = "MemberID";                                           //列名
            tblMemberColumnMemberID.Properties["Jet OLEDB:Allow Zero Length"].Value = false;  //空白を許可しない
            tblMember.Columns.Append(tblMemberColumnMemberID, ADOX.DataTypeEnum.adInteger);   //形を指定して追加

            ADOX.Column tblMemberColumnMemberNickName = new ADOX.Column();
            tblMemberColumnMemberNickName.ParentCatalog = catMakeTable;
            tblMemberColumnMemberNickName.Name = "MemberNickName";                                           //列名
            tblMemberColumnMemberNickName.Properties["Jet OLEDB:Allow Zero Length"].Value = true;  //空白を許可する
            tblMember.Columns.Append(tblMemberColumnMemberNickName, ADOX.DataTypeEnum.adWChar, 255);   //形を指定して追加 (255文字)

            ADOX.Column tblMemberColumnMemberData = new ADOX.Column();
            tblMemberColumnMemberData.ParentCatalog = catMakeTable;
            tblMemberColumnMemberData.Name = "MemberData";                                           //列名
            tblMemberColumnMemberData.Properties["Jet OLEDB:Allow Zero Length"].Value = true;  //空白を許可する
            tblMember.Columns.Append(tblMemberColumnMemberData, ADOX.DataTypeEnum.adWChar, 255);   //形を指定して追加 (255文字)

            catMakeTable.Tables.Append(tblMember);

            //テーブルの作成
            ADOX.Table tblChatCount = new ADOX.Table();
            tblChatCount.ParentCatalog = catMakeTable;
            tblChatCount.Name = "tblChatCount";                              //テーブル名

            //列名１　Id(整数値)の作成
            /*
                        ADOX.Column columnId = new ADOX.Column();
                        columnId.ParentCatalog = catMakeTable;
                        columnId.Type = ADOX.DataTypeEnum.adInteger;                        //事前に設定しないと"AutoIncrement"でエラーとなる
                        columnId.Name = "ID";                                               //列名
                        columnId.Properties["Jet OLEDB:Allow Zero Length"].Value = false;   //空白を許可しない
                        columnId.Properties["AutoIncrement"].Value = true;                  //自動採番とする
                        tblChatCount.Columns.Append(columnId, ADOX.DataTypeEnum.adInteger);        //形を指定して追加
             */                                                                                      //列名２
            ADOX.Column columnMemberID = new ADOX.Column();
            columnMemberID.ParentCatalog = catMakeTable;
            columnMemberID.Type = ADOX.DataTypeEnum.adInteger;
            columnMemberID.Name = "MemberID";                                           //列名
            columnMemberID.Properties["Jet OLEDB:Allow Zero Length"].Value = false;  //空白を許可しない
            tblChatCount.Columns.Append(columnMemberID, ADOX.DataTypeEnum.adInteger);   //形を指定して追加

            ADOX.Column columnMemberName = new ADOX.Column();
            columnMemberName.ParentCatalog = catMakeTable;
            columnMemberName.Name = "MemberName";                                           //列名
            columnMemberName.Properties["Jet OLEDB:Allow Zero Length"].Value = false;  //空白を許可しない
            tblChatCount.Columns.Append(columnMemberName, ADOX.DataTypeEnum.adWChar, 255);   //形を指定して追加 (255文字)
                                                                                             //主キーの設定
            ADOX.Column columnLogDate = new ADOX.Column();
            columnLogDate.ParentCatalog = catMakeTable;
            columnLogDate.Type = ADOX.DataTypeEnum.adDate;
            columnLogDate.Name = "LogDate";                                     //列名
            columnLogDate.Properties["Jet OLEDB:Allow Zero Length"].Value = false; //空白を許可する
            //table.Columns.Append(columnFirstTime, ADOX.DataTypeEnum.adDBTimeStamp); //形を指定して追加
            tblChatCount.Columns.Append(columnLogDate, ADOX.DataTypeEnum.adDate); //形を指定して追加

            ADOX.Column columnChatCount = new ADOX.Column();
            columnChatCount.ParentCatalog = catMakeTable;
            columnChatCount.Type = ADOX.DataTypeEnum.adInteger;
            columnChatCount.Name = "ChatCount";                                         //列名
            //columnChatCount.Properties["Jet OLEDB:Allow Zero Length"].Value = false;    //空白を許可しない
            tblChatCount.Columns.Append(columnChatCount, ADOX.DataTypeEnum.adInteger);         //形を指定して追加

            //tblChatCount.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "ID", "", "");
            //複数設定する場合
            ADOX.Key key = new ADOX.Key();
            key.Name = "PrimaryKey";
            key.Type = ADOX.KeyTypeEnum.adKeyPrimary;
            key.Columns.Append("MemberID");
            key.Columns.Append("MemberName");
            key.Columns.Append("LogDate");
            tblChatCount.Keys.Append(key);

            //テーブルを追加
            catMakeTable.Tables.Append(tblChatCount);
            //接続を閉じる
            catMakeTable.ActiveConnection.Close();
            catMakeTable = null;
        }
        private void gfMDBTableUpdateOrInsert(string strDBSource, string strSQL, ref int iReturn, ref bool bErrFlg)
        {
            OleDbConnection oleConn = new OleDbConnection(strDBSource);
            OleDbCommand oleCmd = new OleDbCommand();

            iReturn = 0;
            for (int i = 0; i <= 10; i++)
            {
                bErrFlg = false;
                try
                {
                    oleConn.Open();
                }
                catch (Exception ex)
                {
                    //Logger logger = Logger.GetInstance();
                    //logger.br();
                    //logger.print(i + "回目");
                    //logger.print("\"" + strSQL + "\"");
                    //logger.print("\"" + ex.Message + "\"");
                    //logger.br();
                    //logger.print(ex.StackTrace);
                    //logger.br();
                    //logger.br();

                    //stop();

                    if (ex.Message.Equals("データベース '' を開くことができません。アプリケーションで認識できないデータベースであるか、またはファイルが破損しています。 "))
                    {
                        bErrFlg = true;
                    }
                    if (ex.Message.Equals("接続が閉じられていません。現在の接続の状態は 'オープン\" です。。"))
                    {
                        bErrFlg = true;
                        Thread.Sleep(100);
                    }
                    bErrFlg = true;
                }
                if (bErrFlg == false)
                {
                    break;
                }
            }
            for (int i = 0; i <= 10; i++)
            {
                bErrFlg = false;
                try
                {
                    oleCmd.Connection = oleConn;
                    oleCmd.CommandText = strSQL;
                    iReturn = oleCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Logger logger = Logger.GetInstance();
                    //logger.br();
                    //logger.print(i + "回目");
                    //logger.print("\"" + strSQL + "\"");
                    //logger.print("\"" + ex.Message + "\"");
                    //logger.br();
                    //logger.print(ex.StackTrace);
                    //logger.br();
                    //logger.br();

                    //stop();

                    if (ex.Message.Equals("インデックス、主キー、またはリレーションシップで値が重複しているので、テーブルを変更できませんでした。重複する値のあるフィールドの値を変更するか、インデックスを削除してください。または重複する値を使用できるように再定義してください。"))
                    {
                        bErrFlg = false;
                        break;
                    }
                    if (ex.Message.Equals("更新可能なクエリであることが必要です。"))
                    {
                        bErrFlg = false;
                        break;
                    }
                    //
                    bErrFlg = true;
                }
                if (bErrFlg == false)
                {
                    break;
                }
                Thread.Sleep(100);
            }
            oleCmd.Dispose();
            oleConn.Close();
        }
        private void gfViewMember(string strDBSource, string strSourceFileName)
        {
            var myCheck = false;
            var bGoFlg = false;
            var numID = 0;
            Logger logger = Logger.GetInstance();
            logger.print("gfViewMember 表示");
            do
            {
                if (radioButton1.Checked == true)
                {
                    myCheck = Regex.IsMatch(txtID.Text, "\\d{8}", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bGoFlg = true;
                        numID = Int32.Parse(txtID.Text);
                    }
                }
                if (radioButton2.Checked == true)
                {
                    myCheck = Regex.IsMatch(txtInputID.Text, "\\d{8}", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bGoFlg = true;
                        numID = Int32.Parse(txtInputID.Text);
                    }
                }
                if (bGoFlg == false)
                {
                    txtNickName.Text = "";
                    txtRemarks.Text = "";
                    lstCharacterList.Items.Clear();
                    break;
                }
                if (File.Exists(strSourceFileName) == false)
                {
                    break;
                }

                OleDbConnection conn = new OleDbConnection();
                OleDbCommand comm1 = new OleDbCommand();
                OleDbCommand comm2 = new OleDbCommand();

                conn.ConnectionString = strDBSource; // MDB名など

                 //接続します。
                conn.Open();

                 //SELECT文を設定します。
                comm1.CommandText = "SELECT MemberName, FirstTime, LastTime, ChatCount FROM tblChatCount WHERE MemberID = " + numID.ToString() + " ORDER BY ChatCount DESC";

                /*
                SELECT tblChatCount.MemberID, tblChatCount.MemberName, Min(tblChatCount.LogDate) AS FirstTime, Max(tblChatCount.LogDate) AS LastTime, Sum(tblChatCount.ChatCount) AS SumChatCount
                FROM tblChatCount
                GROUP BY tblChatCount.MemberID, tblChatCount.MemberName
                HAVING (((tblChatCount.MemberID)=42068383))
                ORDER BY Sum(tblChatCount.ChatCount) DESC;
                */

                comm1.CommandText = "";
                comm1.CommandText += " SELECT";
                comm1.CommandText += " tblChatCount.MemberName";
                comm1.CommandText += ",Min(tblChatCount.LogDate) AS FirstTime";
                comm1.CommandText += ",Max(tblChatCount.LogDate) AS LastTime";
                comm1.CommandText += ",Sum(tblChatCount.ChatCount) AS SumChatCount";
                comm1.CommandText += " FROM tblChatCount";
                comm1.CommandText += " GROUP BY tblChatCount.MemberID";
                comm1.CommandText += ",tblChatCount.MemberName";
                comm1.CommandText += " HAVING(((tblChatCount.MemberID) = " + numID.ToString() + "))";
                comm1.CommandText += " ORDER BY Sum(tblChatCount.ChatCount) DESC;";

                comm1.Connection = conn;
                OleDbDataReader reader1 = comm1.ExecuteReader();
                var strMemberName = "";
                DateTime dtmFirstTime;
                DateTime dtmLastTime;
                var strChatCount = "";

                 //結果を表示します。
                lstCharacterList.Items.Clear();
                //lstCharacterList.Items.Add("\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10\t11\t12\t13\t14\t15\t16\t17\t18\t19");
                while (reader1.Read())
                {
                    //lstCharacterList.Items.Add(reader.GetValue(0).ToString() + "\t" + reader.GetValue(1).ToString() + "\t" + reader.GetValue(2).ToString() + "\t" + String.Format("{0, 10}", Int32.Parse(reader.GetValue(3).ToString())));
                    strMemberName = reader1.GetValue(0).ToString();
                    dtmFirstTime = (DateTime)reader1.GetValue(1);
                    dtmLastTime = (DateTime)reader1.GetValue(2);
                    strChatCount = String.Format("{0:D10}", Int32.Parse(reader1.GetValue(3).ToString()));
                    //lstCharacterList.Items.Add(strMemberName + "\t" + dtmFirstTime.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + dtmLastTime.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + strChatCount);
                    lstCharacterList.Items.Add(strMemberName + "\t" + dtmFirstTime.ToString("yyyy/MM/dd") + "\t" + dtmLastTime.ToString("yyyy/MM/dd") + "\t" + strChatCount);
                }

                comm2.CommandText = "SELECT MemberNickName, MemberData FROM tblMember WHERE MemberID = " + numID.ToString() + ";";
                comm2.Connection = conn;
                OleDbDataReader reader2 = comm2.ExecuteReader();
                while (reader2.Read())
                {
                    var strMemberNickName = "";
                    var strMemberData = "";
                    strMemberNickName = reader2.GetValue(0).ToString();
                    strMemberData = reader2.GetValue(1).ToString();
                    txtNickName.Text = strMemberNickName;
                    txtRemarks.Text = strMemberData;
                }

                //接続を解除します。
                conn.Close();
            }
            while (false);
        }
        private void gfMemberUpdate()
        {
            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            string strDBSourceMain = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                "Data Source=" + strSourceFileName + ";" +     //ファイル名の指定
                                "Jet OLEDB:Engine Type=5";            //Engine Type は 5 

            var strSQL = "";
            var iReturn = 0;

            var myCheck = false;
            var bGoFlg = false;
            var numID = 0;
            var bErrFlg = false;
            //Logger logger = Logger.GetInstance();
            //logger.print("gfMemberUpdate 登録");
            do
            {
                if (radioButton1.Checked == true)
                {
                    myCheck = Regex.IsMatch(txtID.Text, "\\d{8}", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bGoFlg = true;
                        numID = Int32.Parse(txtID.Text);
                    }
                }
                if (radioButton2.Checked == true)
                {
                    myCheck = Regex.IsMatch(txtInputID.Text, "\\d{8}", RegexOptions.Singleline);
                    if (myCheck == true)
                    {
                        bGoFlg = true;
                        numID = Int32.Parse(txtInputID.Text);
                    }
                }
                var strLangTarget = Language_GP.myLanguage_GP();
                if (System.Text.Encoding.GetEncoding(932).GetByteCount(txtNickName.Text) > 255)
                {
                    if (strLangTarget.Equals("JA"))
                    {
                        MessageBox.Show("ニックネーム欄の文字数をオーバーしています");
                    }
                    if (strLangTarget.Equals("EN"))
                    {
                        MessageBox.Show("The number of characters in the nickname field has been exceeded");
                    }
                    break;
                }
                if (System.Text.Encoding.GetEncoding(932).GetByteCount(txtRemarks.Text) > 255)
                {
                    if (strLangTarget.Equals("JA"))
                    {
                        MessageBox.Show("備考欄の文字数をオーバーしています");
                    }
                    if (strLangTarget.Equals("EN"))
                    {
                        MessageBox.Show("The number of characters in the remarks column is exceeded");
                    }
                    break;
                }
                if (bGoFlg == false)
                {
                    break;
                }
                do
                {
                    if (File.Exists(strSourceFileName) == true)
                    {
                        break;
                    }
                    //MDBファイルが無かったら作成する(MDBファイル名は、日本語版と英語版で同一とする)
                    gfMDBFileCreate(strDBSourceMain);
                    //テーブルを作成する
                    gfMDBTableCreate(strDBSourceMain);
                }
                while (false);
                if (txtNickName.Text.Equals("") && txtRemarks.Text.Equals(""))
                {
                    //delete分を発行する
                    strSQL = "";
                    iReturn = 0;
                    strSQL += "delete from tblMember";
                    strSQL += " where MemberID = " + numID.ToString() + "";
                    strSQL += ";";
                    gfMDBTableUpdateOrInsert(strDBSourceMain, strSQL, ref iReturn, ref bErrFlg);
                    break;
                }
                //update文を発行する
                strSQL = "";
                iReturn = 0;
                strSQL += "update tblMember";
                strSQL += " set";
                strSQL += " MemberNickName = \"" + txtNickName.Text + "\"";
                strSQL += ",MemberData = \"" + txtRemarks.Text + "\"";
                strSQL += " where MemberID = " + numID.ToString() + "";
                strSQL += ";";
                gfMDBTableUpdateOrInsert(strDBSourceMain, strSQL, ref iReturn, ref bErrFlg);
                if (iReturn != 0)
                {
                    break;
                }
                if (bErrFlg == true)
                {
                    break;
                }
                //update件数が0だったらinsertする
                strSQL = "";
                iReturn = 0;
                //INSERT INTO tblMember ( MemberID, MemberNickName, MemberData )
                //VALUES(42068383, "aaa", "bbb");

                strSQL += "insert into tblMember";
                strSQL += "(MemberID";
                strSQL += ",MemberNickName";
                strSQL += ",MemberData";
                strSQL += ")VALUES(";
                if (radioButton1.Checked == true)
                {
                    strSQL += " " + txtID.Text + "";
                }
                if (radioButton2.Checked == true)
                {
                    strSQL += " " + txtInputID.Text + "";
                }
                strSQL += ",\"" + txtNickName.Text + "\"";
                strSQL += ",\"" + txtRemarks.Text + "\"";
                strSQL += ");";
                gfMDBTableUpdateOrInsert(strDBSourceMain, strSQL, ref iReturn, ref bErrFlg);
            }
            while (false);
        }
        public class ClsCallApiGetPrivateProfile
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

        private void txtNickName_Leave(object sender, EventArgs e)
        {
            //Logger logger = Logger.GetInstance();
            //logger.print("txtNickName_Leave");
            gfMemberUpdate();
        }
        private void txtRemarks_Leave(object sender, EventArgs e)
        {
            //Logger logger = Logger.GetInstance();
            //logger.print("txtRemarks_Leave");
            gfMemberUpdate();
        }
        private void ChatDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Logger logger = Logger.GetInstance();
            //logger.print("ChatDetail_FormClosing");
            gfMemberUpdate();
        }
        private void ChatDetail_Resize(object sender, EventArgs e)
        {
            DoResize();
        }
        private void ChatDetail_ResizeEnd(object sender, EventArgs e)
        {
            DoResize();
        }
        private void DoResize()
        {
            //800, 600
            //782, 520
            lstCharacterList.Height = this.Height - 392;//600時208
            txtRemarks.Height = this.Height - 392;//600時208
            //708, 281
            //708, 486
            btnDown.Top = this.Height - 319;//600時281
            btnDownMax.Top = this.Height - 114;//600時486
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value = handlingMDB.AwaitProgressBar;
        }
        public new void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        public void stop()
        {
            //System.Diagnostics.Debugger.Break();
        }
    }
    public static class clsStopFlg
    {
        //public static readonly string[] strFileName = new string[MaxArray.numMaxArrayClsListBoxPreWriteBuffer];
        public static bool bStopFlg = false;
    }

}