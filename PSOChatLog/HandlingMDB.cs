using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static PSOChatLog.frmChatDetail;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PSOChatLog
{
    internal class HandlingMDB
    {
        public HttpConnection GetAwaiter()
        {
            return new HttpConnection();
        }
        public bool IsCompleted
        {
            get
            {
                return false;
            }
        }
        public void OnCompleted(Action continuation)
        {
            // awaitの続きを頼む
            continuation();
        }
//        public async Task DoAnalysisAsync(CancellationToken cancelToken)
        public async Task DoAnalysisAsync(CancellationToken cancelToken)
        {
            this.AwaitCancelToken = cancelToken;
            await Task.Run(() => {
                ProcTask();
            });
        }
        public void ProcTask()
        {
            LogAnalysis();
        }
        public void SetAwaitParameter(string strAnalysisType, IntPtr hWindowHandleOwnerWindow, IntPtr hWindowHandleCallWindow)
        {
            this.AwaitAnalysisType = strAnalysisType;
            this.AwaitWindowHandleOwnerWindow = hWindowHandleOwnerWindow;
            this.AwaitWindowHandleCallWindow = hWindowHandleCallWindow;
        }
        public string AwaitAnalysisType { get; set; }
        public int AwaitProgressBar { get; set; }
        public IntPtr AwaitWindowHandleOwnerWindow { get; set; }
        public IntPtr AwaitWindowHandleCallWindow { get; set; }

        public int AwaitProcessId { get; set; }
        public CancellationToken AwaitCancelToken { get; set; }
        private void LogAnalysis()
        {
            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            string strDBSourceMain = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                "Data Source=" + strSourceFileName + ";" +     //ファイル名の指定
                                "Jet OLEDB:Engine Type=5";            //Engine Type は 5 
            string strDBSourceCD = "Provider=Microsoft.Jet.OLEDB.4.0;" + //プロバイダ (32bitのみ)
                                    "Data Source=" + strSourceFileName + ".CompactDatabase" + ";" +     //ファイル名の指定
                                    "Jet OLEDB:Engine Type=5";            //Engine Type は 5
            //stop();
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
            //テーブルの中身をSQLでデリートする
            if (AwaitAnalysisType.Equals("全てのログ") || AwaitAnalysisType.Equals("ALL"))
            {
                //全てのログが対象
                gfMDBTableDelete(strDBSourceMain, "tblChatCount");
            }
            //MDBファイルを最適化する
            var strDestFileName = strSourceFileName + ".CompactDatabase";
            gfMDBCompactDatabase(strSourceFileName, strDestFileName, strDBSourceMain, strDBSourceCD);
            //IDを含んだチャットログファイルを順繰りに読込し、MDBに登録する
            gfCallChatFileToMDB(strDBSourceMain);
        }
        private void gfMDBFileCreate(string strDBSource)
        {
            ADOX.Catalog catMakeMDB = new ADOX.Catalog();
            catMakeMDB.Create(strDBSource);
            catMakeMDB = null;
        }
        private void gfMDBCompactDatabase(string strSourceFileName, string strDestFileName, string strDBSourceMain, string strDBSourceCD)
        {

            var bfalseFlg = false;

            // ファイルパス
            // FileInfoのインスタンスを生成する
            FileInfo fileInfo = new FileInfo(strSourceFileName);
            do
            {
                // 移動後のファイル名（パス）
                if (File.Exists(strDestFileName) == true)
                {
                    File.Delete(strDestFileName);
                }
                // ファイルをリネームする
                for (int i = 0; i < 100; i++)
                {
                    bfalseFlg = false;
                    try
                    {
                        fileInfo.MoveTo(strDestFileName);
                    }
                    catch
                    {
                        bfalseFlg = true;
                    }
                    if (bfalseFlg == false)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                if (bfalseFlg == true)
                {
                    break;
                }
                JRO.JetEngine jroJet = new JRO.JetEngine();
                jroJet.CompactDatabase(strDBSourceCD, strDBSourceMain);

                if (File.Exists(strDestFileName) == true)
                {
                    File.Delete(strDestFileName);
                }

            }
            while (false);
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
        private void gfMDBTableDelete(string strDBSource, string strTableName)
        {
            OleDbConnection oleConn = new OleDbConnection(strDBSource);
            OleDbCommand oleCmd = new OleDbCommand();
            int iRet;

            oleConn.Open();
            oleCmd.Connection = oleConn;
            oleCmd.CommandText = "DELETE FROM " + strTableName + ";";

            iRet = oleCmd.ExecuteNonQuery();
            oleCmd.Dispose();
            oleConn.Close();
            //listBox1.Items.Add(iRet.ToString() + " 件実行完了");

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
        private void gfCallChatFileToMDB(string strDBSource)
        {
            var thisProcess = Process.GetCurrentProcess();
            AwaitProcessId = thisProcess.Id;

            var strIniFileName = ".\\" + Value.strEnvironment + ".ini";
            var ClsCallApiGetPrivateProfile = new ClsCallApiGetPrivateProfile();
            var strInstallPath = ClsCallApiGetPrivateProfile.CallApiGetValueString(Value.strEnvironment, "InstallPath", strIniFileName);

            if (strInstallPath == "")
            {
                MessageBox.Show("strInstallPathが不正です。");
                return;
            }

            var strLogPath = "";
            strLogPath = GetFileNameFullPathToPathName(strInstallPath) + "log";
            string[] filesNormal = System.IO.Directory.GetFiles(strLogPath, "chat*", System.IO.SearchOption.AllDirectories);
            var myCheck = false;
            DateTime dtmNormalTmp;
            double dblLC;
            double dblMax;
            double dblTmp;
            //<todo>
            //progressBar1.Visible = true;
            //progressBar1.Maximum = 100;

            var strSourceFileName = @".\\" + Value.strMDBFileName + ".mdb";
            DateTime dtmPreviousAnalysis = gfGetPreviousAnalysis(strDBSource, strSourceFileName);

            //NormalLogを検索
            for (int i = 0; i < filesNormal.Length; i++)
            {
                if (this.AwaitCancelToken.IsCancellationRequested == true)
                {
                    break;
                }
                if (this.AwaitWindowHandleOwnerWindow == null)
                {
                    break;
                }
                if (this.AwaitWindowHandleCallWindow == null)
                {
                    break;
                }

                dblLC = i;
                dblMax = filesNormal.Length;
                dblTmp = (dblLC / dblMax) * 100;
                //<todo>
                //progressBar1.Value = (int)dblTmp;
                //progressBar1.Refresh();
                AwaitProgressBar = (int)dblTmp;
                //System.Console.WriteLine(i + " = " + gfGetFileName(filesNormal[i]));
                myCheck = Regex.IsMatch(filesNormal[i], "chat\\d\\d\\d\\d\\d\\d\\d\\d.txt", RegexOptions.Singleline);
                do
                {
                    if (myCheck == false)
                    {
                        break;
                    }
                    //Ephineaが始まった年からのログが対象
                    dtmNormalTmp = gfGetDtmFromYYYYMMDD(gfGetFileStrDate(gfGetFileName(filesNormal[i])));
                    if (DateTime.Compare(DateTime.Parse("2015/01/01"), dtmNormalTmp) <= 0)
                    {
                    }
                    else
                    {
                        break;
                    }
                    if (AwaitAnalysisType.Equals("全てのログ") || AwaitAnalysisType.Equals("ALL"))
                    {
                        //全てのログが対象
                    }
                    if (AwaitAnalysisType.Equals("今月のログ") || AwaitAnalysisType.Equals("This month's"))
                    {
                        //今月のログが対象
                        if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToString("yyyy/MM/01")), dtmNormalTmp) <= 0)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (AwaitAnalysisType.Equals("今週のログ") || AwaitAnalysisType.Equals("This week's"))
                    {
                        //7日前からのログが対象
                        if (DateTime.Compare(DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy/MM/dd")), dtmNormalTmp) <= 0)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (AwaitAnalysisType.Equals("昨日からのログ") || AwaitAnalysisType.Equals("from yesterday"))
                    {
                        //全てのログが対象
                        if (DateTime.Compare(DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd")), dtmNormalTmp) <= 0)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (AwaitAnalysisType.Equals("今日のログ") || AwaitAnalysisType.Equals("Today's Log"))
                    {
                        //今日以降のログが対象
                        if (DateTime.Compare(DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")), dtmNormalTmp) <= 0)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (AwaitAnalysisType.Equals("前回の解析後") || AwaitAnalysisType.Equals("Since last"))
                    {
                        //前回の解析以降のログが対象
                        if (DateTime.Compare(dtmPreviousAnalysis, dtmNormalTmp) <= 0)
                        {
                        }
                        else
                        {
                            break;
                        }
                    }

                    //今日までのログが対象
                    if (DateTime.Compare(dtmNormalTmp, DateTime.Parse((DateTime.Now.AddDays(1).ToString("yyyy/MM/dd")))) >= 0)
                    {
                        break;
                    }
                    //ユーザーのストップ指示による停止
                    if (clsStopFlg.bStopFlg == true)
                    {
                        break;
                    }
                    gfChatFileToMDB(filesNormal[i], strDBSource);
                }
                while (false);
            }
            //<todo>
            //progressBar1.Visible = false;
        }
        private void gfChatFileToMDB(string strFullPath, string strDBSource)
        {
            var strDateTime = "";
            var strMemberID = "";
            var strMember = "";
            var bBreakFlg = false;
            var bHitFlg = false;
            var bErrFlg = false;
            string[] strReadBuf = new string[0];
            //List 作成
            List<stChatData> listChatData = new List<stChatData>();
            List<stChatCount> listChatCount = new List<stChatCount>();

            var strYYYY = "";
            var strMM = "";
            var strDD = "";
            GetFileNameFileNameToDateTime(gfGetFileName(strFullPath), ref strYYYY, ref strMM, ref strDD);

            var strSQL = "";
            var iReturn = 0;
            strSQL += "delete from tblChatCount";
            strSQL += " where LogDate = #" + strYYYY + "/" + strMM + "/" + strDD + "#";
            strSQL += ";";
            gfMDBTableUpdateOrInsert(strDBSource, strSQL, ref iReturn, ref bErrFlg);

            FileReadChatLogToVariable(strFullPath, "Normal", ref strReadBuf);
            for (int i = 0; i < strReadBuf.Length - 1; i++)
            {
                if (clsStopFlg.bStopFlg == true)
                {
                    break;
                }
                do
                {
                    bBreakFlg = false;
                    ExtractTargetTranslationNormal(strReadBuf[i], ref strDateTime, ref strMemberID, ref strMember, ref bBreakFlg);
                    if (bBreakFlg == true)
                    {
                        break;
                    }
                    // 構造体型の変数作成
                    stChatData tyChatData = new stChatData();
                    // 値の代入
                    tyChatData.strDateTime = strDateTime;
                    tyChatData.strMemberID = strMemberID;
                    tyChatData.strMember = strMember;
                    tyChatData.strChatLog = strReadBuf[i];
                    // List への追加
                    listChatData.Add(tyChatData);
                }
                while (false);
            }
            for (int i = 0; i < listChatData.Count; i++)
            {
                if (clsStopFlg.bStopFlg == true)
                {
                    break;
                }
                bHitFlg = false;
                for (int j = 0; j < listChatCount.Count; j++)
                {
                    do
                    {
                        if (listChatData[i].strMemberID.Equals(listChatCount[j].strMemberID) == false)
                        {
                            //メンバーIDが異なるので次行へ
                            break;
                        }
                        if (listChatData[i].strMember.Equals(listChatCount[j].strMemberName) == false)
                        {
                            //メンバー名が異なるので次行へ
                            break;
                        }
                        //update
                        // 構造体型の変数作成
                        try
                        {
                            stChatCount tyChatCount = new stChatCount();
                            tyChatCount.strMemberID = listChatCount[j].strMemberID;
                            tyChatCount.strMemberName = listChatCount[j].strMemberName;
                            tyChatCount.dtmLogDate = DateTime.Parse(strYYYY + "/" + strMM + "/" + strDD);
                            tyChatCount.numChatCount = listChatCount[j].numChatCount + 1;
                            listChatCount[j] = tyChatCount;
                        }
                        catch
                        {
                            break;
                        }

                        bHitFlg = true;
                    }
                    while (false);
                }
                if (bHitFlg == false)
                {
                    //add
                    // 構造体型の変数作成
                    // 値の代入

                    stChatCount tyChatCount = new stChatCount();
                    try
                    {
                        tyChatCount.strMemberID = listChatData[i].strMemberID;
                        tyChatCount.strMemberName = listChatData[i].strMember;
                        tyChatCount.dtmLogDate = DateTime.Parse(strYYYY + "/" + strMM + "/" + strDD);
                        tyChatCount.numChatCount = 1;
                    }
                    catch
                    {
                        break;
                    }

                    // List への追加
                    listChatCount.Add(tyChatCount);
                }
            }
            for (int i = 0; i < listChatCount.Count; i++)
            {
                if (clsStopFlg.bStopFlg == true)
                {
                    break;
                }
                do
                {
                    //update文を発行する
                    //strSQL = "";
                    //iReturn = 0;
                    //strSQL += "update tblChatCount";
                    //strSQL += " set";
                    //strSQL += " ChatCount = ChatCount + " + listChatCount[i].numChatCount;
                    //strSQL += " where MemberID = " + listChatCount[i].strMemberID + "";
                    //strSQL += " and MemberName = \"" + listChatCount[i].strMemberName + "\"";
                    //strSQL += " and LogDate = #" + listChatCount[i].dtmLogDate.ToString("yyyy/MM/dd") + "#";
                    //strSQL += ";";
                    //gfMDBTableUpdateOrInsert(strDBSource, strSQL, ref iReturn, ref bErrFlg);
                    //if (iReturn != 0)
                    //{
                    //    break;
                    //}
                    //if (bErrFlg == true)
                    //{
                    //    break;
                    //}
                    //update件数が0だったらinsertする
                    strSQL = "";
                    iReturn = 0;
                    strSQL += "insert into tblChatCount";
                    strSQL += "(MemberID";
                    strSQL += ",MemberName";
                    strSQL += ",LogDate";
                    strSQL += ",ChatCount";
                    strSQL += ")VALUES(";
                    strSQL += " " + listChatCount[i].strMemberID + "";
                    strSQL += ",\"" + listChatCount[i].strMemberName + "\"";
                    strSQL += ",#" + listChatCount[i].dtmLogDate.ToString("yyyy/MM/dd") + "#";
                    strSQL += "," + listChatCount[i].numChatCount + "";
                    strSQL += ");";
                    gfMDBTableUpdateOrInsert(strDBSource, strSQL, ref iReturn, ref bErrFlg);
                }
                while (false);
            }
        }
        private DateTime gfGetPreviousAnalysis(string strDBSource, string strSourceFileName)
        {
            var dtmLastLogDate = DateTime.Parse("2015/01/01");
            //Logger logger = Logger.GetInstance();
            //logger.print("gfViewMember 表示");
            do
            {
                OleDbConnection conn = new OleDbConnection();
                OleDbCommand comm1 = new OleDbCommand();

                conn.ConnectionString = strDBSource; // MDB名など

                // 接続します。
                conn.Open();

                // SELECT文を設定します。
                comm1.CommandText = "";
                comm1.CommandText += " SELECT tblChatCount.LogDate";
                comm1.CommandText += " FROM tblChatCount";
                comm1.CommandText += " ORDER BY tblChatCount.LogDate DESC";
                comm1.CommandText += ";";

                comm1.Connection = conn;
                OleDbDataReader reader1 = comm1.ExecuteReader();

                try
                {
                    reader1.Read();
                    dtmLastLogDate = DateTime.Parse(reader1.GetValue(0).ToString());
                }
                catch
                {
                }
                // 接続を解除します。
                conn.Close();
            }
            while (false);
            return dtmLastLogDate;
        }
        private string ExtractTargetTranslationNormal(string strLastLine, ref string strDateTime, ref string strMemberID, ref string strMember, ref bool bBreakFlg)
        {
            var myCheck = false;
            var strTmp = "";
            var strTmp1 = "";
            var strTmp2 = "";
            var strTmp3 = "";
            var numFlg = 0;
            var strDataOriginal = "";
            bBreakFlg = false;

            do
            {
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //取得した最後の行がチャットログであることを確認し、会話の部分を抜き出すまた、ダブルクォートは排除する
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                //myCheck = Regex.IsMatch(lastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t*\\t*", RegexOptions.Singleline);
                myCheck = Regex.IsMatch(strLastLine, "\\d\\d:\\d\\d:\\d\\d\\t\\d{8}\\t.*\\t.*", RegexOptions.Singleline);
                if (myCheck == false)
                {
                    //チャットログではないので終了
                    bBreakFlg = true;
                    break;
                }
                //Console.WriteLine(lastLine.Length - 1);
                for (int i = strLastLine.Length - 1; 0 <= i; i--)
                {
                    strTmp = strLastLine.Substring(i, 1);
                    if (Regex.IsMatch(strTmp, "\\t", RegexOptions.Singleline))
                    {
                        numFlg++;
                        //if (numFlg == 2)
                        //{
                        //    //タブが後ろから数えて２つ目
                        //    break;
                        //}
                    }
                    if (Regex.IsMatch(strTmp, "\\x22", RegexOptions.Singleline) == false)
                    {
                        if (numFlg == 0)
                        {
                            strDataOriginal = strTmp + strDataOriginal;
                        }
                        if (numFlg == 1)
                        {
                            strTmp1 = strTmp + strTmp1;
                        }
                        if (numFlg == 2)
                        {
                            strTmp2 = strTmp + strTmp2;
                        }
                        if (numFlg == 3)
                        {
                            strTmp3 = strTmp + strTmp3;
                        }
                    }
                }
                //Console.WriteLine("strData = " + strData);
                //Console.WriteLine("strMember = " + strMember);
                //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
            while (false);
            strMember = strTmp1.Trim();
            strMemberID = strTmp2.Trim();
            strDateTime = strTmp3.Trim();
            return strDataOriginal;
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
        public void FileReadChatLogToVariable(string strFullPath, string strLogType, ref string[] strReadBuf)
        {
            var i = 0;
            var strYYYY = "";
            var strMM = "";
            var strDD = "";
            //
            GetFileNameFileNameToDateTime(strFullPath, ref strYYYY, ref strMM, ref strDD);
            DateTime dtmExt = new DateTime(Int32.Parse(strYYYY), Int32.Parse(strMM), Int32.Parse(strDD));
            DateTime dtmNow = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
            //当日のファイルは排他しない。
            if (DateTime.Compare(dtmNow, dtmExt) != 0)
            {
                //当日以外のファイルは、読み込み排他する。
                ClsChatDetailReadFile.strFullPathNormal = strFullPath;
            }

            do//読込チャレンジループ
            {
                try
                {
                    //読込チャレンジしてみる
                    //strReadBuf = File.ReadAllLines(strFullPath);
                    //strTextALL = File.ReadAllText(strFullPath, Encoding.GetEncoding("shift-jis"));
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
                    strReadBuf = Regex.Split(strBuf, "\r\n");

                }
                //失敗判定
                catch (System.IO.IOException)
                {
                    //失敗していたらスルーして再読込
                }
                if (0 < strReadBuf.Length)
                {
                    //読み込まれていたらチャレンジ終了
                    break;
                }
                i++;
                Thread.Sleep(10);
            }
            while (i <= 10);

            ClsChatDetailReadFile.strFullPathNormal = "";
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
        private string gfGetFileName(string strFullPath)
        {
            var strExecPath = "";
            var strTmp = "";

            for (int i = strFullPath.Length - 1; 0 <= i; i--)
            {
                strTmp = strFullPath.Substring(i, 1);
                //最後の\までを抜き出す
                if (Regex.IsMatch(strTmp, "\\\\", RegexOptions.Singleline))
                {
                    break;
                }
                strExecPath = strTmp + strExecPath;
            }
            return strExecPath;
        }
        private string gfGetFileStrDate(string strFullPath)
        {
            var strExecPath = "";
            var strTmp = "";

            for (int i = 0; i < strFullPath.Length - 1; i++)
            {
                strTmp = strFullPath.Substring(i, 1);
                if (Regex.IsMatch(strTmp, "[0-9]", RegexOptions.Singleline))
                {
                    strExecPath = strExecPath + strTmp;
                }
            }
            return strExecPath;
        }
        private DateTime gfGetDtmFromYYYYMMDD(string strYYYYMMDD)
        {
            var strYYYY = strYYYYMMDD.Substring(0, 4);
            var strMM = strYYYYMMDD.Substring(4, 2);
            var strDD = strYYYYMMDD.Substring(6, 2);
            var strTmp = strYYYY + "/" + strMM + "/" + strDD;
            DateTime dtmExt = new DateTime(Int32.Parse(strYYYY), Int32.Parse(strMM), Int32.Parse(strDD));
            return dtmExt;
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
    }
}
