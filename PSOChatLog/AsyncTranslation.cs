using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PSOChatLog
{
    internal class AsyncTranslation
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
        public async Task DoASyncTranslation(CancellationToken cancelToken)
        {
            this.AwaitCancelToken = cancelToken;
            await Task.Run(() => {
                ProcTask();
            });
        }
        public void ProcTask()
        {
            ASyncTranslation();
        }
        public DateTime dtmAddLogDate { get; set; }
        public string strSource { get; set; }
        public string strLangTarget { get; set; }
        public string strLangSource { get; set; }
        public string strLogType { get; set; }
        public string strIndexNo { get; set; }
        public string strDateTime { get; set; }
        public string strMemberID { get; set; }
        public string strMember { get; set; }
        public string strAfterTranslation { get; set; }
        public int AwaitProcessId { get; set; }
        //public string AwaitJSONSend { get; set; }
        public HttpContent AwaitContentSend { get; set; }
        public string AwaitJSONRecv { get; set; }
        public string AwaitJSONMessage { get; set; }
        public string AwaitResultCode { get; set; }
        public string AwaitURL { get; set; }
        public string AwaitTypeHost { get; set; }
        public string AwaitAPIKey { get; set; }
        public string AwaitInstallPath { get; set; }
        //public HttpClient httpClient { get; set; }
        static readonly HttpClient httpClient = new HttpClient();

        public CancellationToken AwaitCancelToken { get; set; }
        private async void ASyncTranslation()
        {
            //stop();

            var strTranslator = AwaitTypeHost;
            var strJSON = "";
            var strMessageTarget = "";
            var strMessageError = "";
            var strCode = "";

            HttpResponseMessage res1;
            do
            {
                if (this.AwaitTypeHost == null)
                {
                    break;
                }
                if (this.AwaitTypeHost == "DeepL")
                {
                    do//空ループ
                    {
                        try
                        {
                            //WEB APIを呼ぶ
                            res1 = await httpClient.PostAsync(AwaitURL, AwaitContentSend);
                            this.AwaitJSONMessage = res1.ToString();

                            //リターンコードを取得する。
                            //"StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:\r\n{\r\n  access-control-allow-origin: *\r\n  strict-transport-security: max-age=63072000; includeSubDomains; preload\r\n  server-timing: l7_lb_tls;dur=556, l7_lb_idle;dur=0, l7_lb_receive;dur=5, l7_lb_total;dur=601\r\n  access-control-expose-headers: Server-Timing\r\n  Date: Sun, 26 Mar 2023 02:36:28 GMT\r\n  Server: nginx\r\n  Content-Length: 104\r\n  Content-Type: application/json\r\n}"
                            if (ConvertJsonToStringRegexGeneralPurpose(this.AwaitJSONMessage, ref strCode, "StatusCode..", ",") == false)
                            {
                                break;
                            }
                            if (strCode.Equals("200") == false)
                            {
                                //200以外ならエラーメッセージを取得する。
                                if (ConvertJsonToStringRegexGeneralPurpose(this.AwaitJSONMessage, ref strMessageError, ".*text\x22:\x22", "\\x22") == false)
                                {
                                    //ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, "", DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " API call error " + strCode + " = " + strMessageError);
                                    break;
                                }
                                break;
                            }
                            //リターンコードが200なら正常終了
                            //"result":{"text":"\u30c6\u30b9\u30c8"
                            //翻訳文字列を取得する。

                            var res2 = "";
                            res2 = await res1.Content.ReadAsStringAsync();
                            this.AwaitJSONRecv = res2.ToString();
                        }
                        catch (ArgumentNullException e)
                        {
                        }
                        if (ConvertJsonToStringRegexGeneralPurpose(this.AwaitJSONRecv, ref strMessageTarget, ".*text\x22:\x22", "\\x22") == false)
                        {
//#if DEBUG
//                            Assembly myAssembly = Assembly.GetEntryAssembly();
//                            string strSystemPath = myAssembly.Location;
//                            var strExecPath = GetFileNameFullPathToPathName(strSystemPath);
//                            //Console.WriteLine("path = " + strExecPath);
//                            File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".html", strJSON);
//                            //Console.WriteLine("path = " + strExecPath);
//                            //File.WriteAllText(strExecPath + "PSOChatLog_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".json.txt", strSendJson);
//                            ArryClsListBoxPreWriteBufferAddNewLog(dtmAddLogDate, "", "ErrMsg", false, strLogType, DateTime.Now.ToString("HH:mm:ss"), "--------", "--------", strTranslator + " server error\t" + "\t" + "\"" + strSource + "\"");
//#endif
                        }
                    } while (false);
                    if (this.AwaitTypeHost == "Google")
                    {
                        break;
                    }
                    if (this.AwaitTypeHost == "Baidu")
                    {
                        break;
                    }
                    if (this.AwaitTypeHost == "TexTra")
                    {
                        break;
                    }
                } while (false);
                strTranslator = "Trans " + strTranslator;
                strTranslator += " U";
                //ListboxDisplayUpdateTranceAsync(strIndexNo, strTranslator, strLogType, strDateTime, strMemberID, strMember, strMessageTarget);
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
    }
}
