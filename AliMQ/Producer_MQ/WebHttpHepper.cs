using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;

namespace Producer_MQ
{
    public class HttpWebResponseEx
    {
        public HttpStatusCode StatusCode { get; set; }
        public string RetureValue { get; set; }
    }

    public class WebHttpHepper
    {
        //回调验证证书问题
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {   // 总是接受    
            return true;
        }

        /// <summary>
        /// 传入URL返回网页的html代码带有证书的方法
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static HttpWebResponseEx SendRequest(string url, string method, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null, string postData = null)
        {
            StringBuilder urlBuilder = new StringBuilder(url);
            urlBuilder.Append(ConcatQueryString(parameters));
            url = urlBuilder.ToString();

            HttpWebRequest request = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            // 与指定URL创建HTTP请求
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
            request.Method = method;
            request.Accept = "*/*";

            //headers
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            if (method.ToUpper() == "POST")
            {
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "text/plain;charset=UTF-8";
                request.ContentLength = data.Length;

                using (Stream steam = request.GetRequestStream())
                {
                    steam.Write(data, 0, data.Length);
                }
            }

            // 获取对应HTTP请求的响应
            HttpWebResponseEx res = new HttpWebResponseEx();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            res.StatusCode = response.StatusCode;

            // 对接响应流(以"UTF8"字符集)
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                res.RetureValue = reader.ReadToEnd();
            }
            return res;
        }

        private static string ConcatQueryString(Dictionary<String, String> parameters)
        {
            if (null == parameters)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder("?");

            foreach (var entry in parameters)
            {
                String key = entry.Key;
                String val = entry.Value;

                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }
                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);

            return sb.ToString();
        }
    }
}
