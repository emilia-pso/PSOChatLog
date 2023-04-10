using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HttpConnection;

namespace PSOChatLog
{
    internal class HttpClientOAuth20Async
    {
        public void GetAwaitParameter(ref string content)
        {
            content = AwaitContent;
        }
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
            // continuation();
        }
        public void SetAwaitParameter(string method, Uri requestUri, Dictionary<string, string> param, ref string content, Dictionary<string, string> headerInfo)
        {
            this.AwaitMethod = method;
            this.AwaitRequestUri = requestUri;
            this.AwaitParam = param;
            this.AwaitContent = content;
            this.AwaitHeaderInfo = headerInfo;
        }
        public HttpStatusCode GetResult()
        {
            HttpStatusCode ret;
            var strJSON = "";
            HttpWebRequest webReq = CreateRequest(this.AwaitMethod, this.AwaitRequestUri, this.AwaitParam, false);
            // OAuth認証ヘッダを付加
            if (string.IsNullOrEmpty(token))
                AppendOAuthInfo(webReq, this.AwaitParam, token, tokenSecret);

            if (AwaitContent == null)
            {
                ret = GetResponse(webReq, this.AwaitHeaderInfo, false);
            }
            else
            {
                ret = GetResponse(webReq, ref strJSON, this.AwaitHeaderInfo, false);
            }
            this.AwaitContent = strJSON;
            return ret;
        }
        private string AwaitMethod { get; set; }
        private Uri AwaitRequestUri { get; set; }
        private Dictionary<string, string> AwaitParam { get; set; }
        private string AwaitContent { get; set; }
        private Dictionary<string, string> AwaitHeaderInfo { get; set; }



        protected HttpWebRequest CreateRequest(string method, Uri requestUri, Dictionary<string, string> param, bool withCookie)
        {
            if (!isInitialize)
                throw new Exception("Sequence error.(not initialized)");

            // GETメソッドの場合はクエリとurlを結合
            UriBuilder ub = new UriBuilder(requestUri.AbsoluteUri);
            if (method == "GET" || method == "DELETE" || method == "HEAD")
                ub.Query = CreateQueryString(param);

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(ub.Uri);

            // Proxy設定
            if (proxyKind != ProxyType.IE)
                webReq.Proxy = proxy;

            webReq.Method = method;
            if (method == "POST" || method == "PUT")
            {
                webReq.ContentType = "application/x-www-form-urlencoded";
                // POST/PUTメソッドの場合は、ボディデータとしてクエリ構成して書き込み
                using (StreamWriter writer = new StreamWriter(webReq.GetRequestStream()))
                {
                    writer.Write(CreateQueryString(param));
                }
            }
            // cookie設定
            if (withCookie)
                webReq.CookieContainer = cookieContainer;
            // タイムアウト設定
            webReq.Timeout = DefaultTimeout;

            return webReq;
        }

        /// <summary>
        ///     '''HTTPの応答を処理し、応答ボディデータをテキストとして返却する
        ///     '''</summary>
        ///     '''<remarks>
        ///     '''リダイレクト応答の場合（AllowAutoRedirect=Falseの場合のみ）は、headerInfoインスタンスがあればLocationを追加してリダイレクト先を返却
        ///     '''WebExceptionはハンドルしていないので、呼び出し元でキャッチすること
        ///     '''テキストの文字コードはUTF-8を前提として、エンコードはしていません
        ///     '''</remarks>
        ///     '''<param name="webRequest">HTTP通信リクエストオブジェクト</param>
        ///     '''<param name="contentText">[OUT]HTTP応答のボディデータ</param>
        ///     '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。ヘッダ名をキーにして空データのコレクションを渡すことで、該当ヘッダの値をデータに設定して戻す</param>
        ///     '''<param name="withCookie">通信にcookieを使用する</param>
        ///     '''<returns>HTTP応答のステータスコード</returns>
        private void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query, string token, string tokenSecret)
        {
            // OAuth共通情報取得
            Dictionary<string, string> parameter = GetOAuthParameter(token);
            // OAuth共通情報にquery情報を追加
            if (query != null)
            {
                foreach (KeyValuePair<string, string> item in query)
                    parameter.Add(item.Key, item.Value);
            }
            // 署名の作成・追加
            parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter));
            // HTTPリクエストのヘッダに追加
            StringBuilder sb = new StringBuilder("OAuth ");
            foreach (KeyValuePair<string, string> item in parameter)
            {
                // 各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
                if (item.Key.StartsWith("oauth_"))
                    sb.AppendFormat("{0}=\"{1}\",", item.Key, UrlEncode(item.Value));
            }
            webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString());
        }

        /// <summary>
        ///     '''OAuthで使用する共通情報を取得する
        ///     '''</summary>
        ///     '''<param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
        ///     '''<returns>OAuth情報のディクショナリ</returns>
    }
}
