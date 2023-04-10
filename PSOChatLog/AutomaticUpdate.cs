using ADODB;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PSOChatLog
{
    internal class AutomaticUpdate
    {
        static readonly HttpClient httpClient = new HttpClient();
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
        public async Task DoAutomaticUpdateAsync(CancellationToken cancelToken)
        {
            this.AwaitCancelToken = cancelToken;
            await Task.Run(() => {
                ProcTask();
            });
        }
        public async void ProcTask()
        {
            if (this.AwaitProcType.Equals("CheckVer"))
            {
                await doCheckVer();
            }
            if (this.AwaitProcType.Equals("Download"))
            {
                await doDownload();
            }
        }
        public void SetAwaitParameter(string strAwaitProcType, IntPtr hWindowHandleOwnerWindow, IntPtr hWindowHandleCallWindow)
        {
            this.AwaitProcType = strAwaitProcType;
            this.AwaitWindowHandleOwnerWindow = hWindowHandleOwnerWindow;
            this.AwaitWindowHandleCallWindow = hWindowHandleCallWindow;
        }
        public string AwaitProcType { get; set; }
        public string AwaitCheckVerResult { get; set; }
        public string AwaitDownloadResult { get; set; }
        public string AwaitFileNameDownload { get; set; }
        public bool AwaitUpDateBTEnable { get; set; }
        public string AwaitUpDateBTText { get; set; }
        public IntPtr AwaitWindowHandleOwnerWindow { get; set; }
        public IntPtr AwaitWindowHandleCallWindow { get; set; }

        public int AwaitProcessId { get; set; }
        public CancellationToken AwaitCancelToken { get; set; }

        public async Task doCheckVer()
        {
            var strVersion = "";
            var strFileName = "";
            var strResult = "";
            do
            {
                var URLCheckVer = "https://www.emilia-pso.com/PSOChatLog/Archive/PSOChatLog_VerCheck.txt";
                strResult = await httpClient.GetStringAsync(URLCheckVer);

                //"{\r\n\t\"Version\": \"0.61\",\r\n\t\"FileName\": \"PSOChatLog_ver0.61.zip\"\r\n}"
                if (ConvertJsonToStringRegexGeneralPurpose(strResult, ref strVersion, "Version\": \"", "\",\r\n\t\"") == false)
                {
                    break;
                }
                if (strVersion == null)
                {
                    break;
                }
                if (strVersion.Equals(""))
                {
                    break;
                }
                //"{\r\n\t\"Version\": \"0.61\",\r\n\t\"FileName\": \"PSOChatLog_ver0.61.zip\"\r\n}"
                if (ConvertJsonToStringRegexGeneralPurpose(strResult, ref strFileName, "\"FileName\\\": \\\"", "\"\r\n}") == false)
                {
                    break;
                }
                if (strFileName == null)
                {
                    break;
                }
                if (strFileName.Equals(""))
                {
                    break;
                }
                this.AwaitCheckVerResult = strVersion;
                this.AwaitFileNameDownload = strFileName;
                this.AwaitUpDateBTEnable = true;
                this.AwaitUpDateBTText = "UpDate";
            } while (false) ;
        }
        public async Task doDownload()
        {
            do
            {
                var PathDownload = System.Environment.CurrentDirectory + "\\Archive\\".Trim();

                Directory.CreateDirectory(PathDownload);
                //"C:\\Users\\admin\\source\\repos\\PSOChatLog\\PSOChatLog\\bin\\Debug\\Archive\\PSOChatLog_ver0.61.zip"
                PathDownload = PathDownload + this.AwaitFileNameDownload.Trim();

                var URLDownloadBase = "https://www.emilia-pso.com/PSOChatLog/Archive/";
                var URLDownload = URLDownloadBase + this.AwaitFileNameDownload;

                using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(URLDownload)))
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var content = response.Content)
                        using (var stream = await content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(PathDownload, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            stream.CopyTo(fileStream);
                            this.AwaitUpDateBTEnable = false;
                            this.AwaitUpDateBTText = "Success";
                        }
                    }
                }
            } while (false);
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
    }
}
