using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Sai.Crawler
{
    public class HTMLHelper
    {
        private static String ParsToString(Dictionary<string, object> pars)
        {
            if (pars == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (string k in pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                //string ss = HttpUtility.UrlEncode(Pars[k].ToString());
                string valueStr = "";//"\"\"";
                if (pars[k] != null)
                {
                    valueStr = HttpUtility.UrlEncode(pars[k].ToString());
                }
                else
                {
                    valueStr = HttpUtility.UrlEncode(valueStr);
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + valueStr);
            }
            return sb.ToString();
        }





        /// <summary>
        /// 获取html ok
        /// </summary>
        /// <param name="getUrl">请求url地址</param>
        /// <param name="header">请求header</param>
        /// <returns>html内容</returns>
        public static string GetContentByUrl(string getUrl, HttpHeader header)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);

                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;

                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 30000;
                httpWebRequest.ReadWriteTimeout = 30000;

                httpWebRequest.KeepAlive = false;


                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
            }
        }



        public static CookieContainer GetLoginCooKie(string loginUrl, string postdata, CookieContainer cookieContainer, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                CookieContainer cc = new CookieContainer();
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.method;
                request.CookieContainer = cookieContainer;
                request.ContentType = header.contentType;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabyte.Length;
                request.AllowAutoRedirect = false;
                request.CookieContainer = cc;
                request.KeepAlive = true;

                //提交请求
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();

                //return html;

                CookieCollection cook = response.Cookies;
                //Cookie字符串格式
                string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

                return cc;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 获取CooKie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postdata"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCooKie(string loginUrl, string postdata, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                CookieContainer cc = new CookieContainer();
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.method;
                request.ContentType = header.contentType;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabyte.Length;
                request.AllowAutoRedirect = false;
                request.CookieContainer = cc;
                request.KeepAlive = true;

                //提交请求
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                CookieCollection cook = response.Cookies;
                //Cookie字符串格式
                string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

                return cc;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string GetHtml(string getUrl, HttpHeader header, WebProxy proxy)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);

                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;

                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Proxy = proxy;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 12000;
                httpWebRequest.ReadWriteTimeout = 12000;

                httpWebRequest.KeepAlive = false;
               

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
            }
        }

        public static string GetHtml(string getUrl, HttpHeader header,Encoding code)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);

                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;

                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebRequest.KeepAlive = false;


                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, code);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
            }
        }



        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetHtml(string getUrl, CookieContainer cookieContainer,string referUrl, HttpHeader header, out CookieContainer cookie)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                if (cookieContainer != null)
                {
                    httpWebRequest.CookieContainer = cookieContainer;
                }
                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = referUrl;// "sbcx.saic.gov.cn:9080";
                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 5*1000;
                httpWebRequest.ReadWriteTimeout = 5*1000;

                httpWebRequest.KeepAlive = false;


                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //httpWebResponse.Cookies = httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri);
                CookieCollection cook = httpWebResponse.Cookies;
                cookie = new CookieContainer();
                foreach (Cookie cc in cook)
                {
                    cookie.Add(cc);
                }
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
                cookie = null;
                return string.Empty;
            }
        }






        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetHtml(string getUrl, CookieContainer cookieContainer, HttpHeader header, out CookieContainer cookie,out string responseUri)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";

                httpWebRequest.Timeout = 5000;
                httpWebRequest.ReadWriteTimeout = 5000;

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                responseUri = httpWebResponse.ResponseUri.ToString();
                httpWebResponse.Cookies = httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri);
                CookieCollection cook = httpWebResponse.Cookies;
                cookie = new CookieContainer();
                foreach (Cookie cc in cook)
                {
                    cookie.Add(cc);
                }
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
                cookie = null;
                responseUri = "";
                return string.Empty;
            }
        }

        public static string GetResoposeUrl(string getUrl, CookieContainer cookieContainer, HttpHeader header,out string responseUri)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                responseUri = httpWebResponse.ResponseUri.ToString();

                if (!getUrl.Equals(responseUri))
                {
                    httpWebRequest.Abort();
                    httpWebResponse.Close();
                    return responseUri;
                }

                httpWebResponse.Cookies = httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri);
               


                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebRequest.Abort();
                httpWebResponse.Close();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                responseUri = "";
                return string.Empty;
            }
        }


        /// <summary>
        /// 获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetHtml(string getUrl, out CookieContainer cookie, HttpHeader header,out string cookieStr)
        {
            //Thread.Sleep(1000);

            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                //httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = header.contentType;
                httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpWebRequest.Referer = getUrl;
                httpWebRequest.Accept = header.accept;
                httpWebRequest.UserAgent = header.userAgent;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 60000;
                httpWebRequest.ReadWriteTimeout = 60000;

                httpWebRequest.KeepAlive = false;

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                cookieStr = httpWebRequest.CookieContainer.GetCookieHeader(new Uri(getUrl));

                CookieCollection cook = httpWebResponse.Cookies;
                cookie = new CookieContainer();
                foreach (Cookie cc in cook)
                {
                    cookie.Add(cc);
                }

                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close(); 
                httpWebResponse.Close();
                httpWebRequest.Abort();
                return html;
            }
            catch (Exception e)
            {
                if (httpWebRequest != null) httpWebRequest.Abort();
                if (httpWebResponse != null) httpWebResponse.Close();
                throw e;
                return string.Empty;
            }
        }
    }

    public class HttpHeader
    {
        public string contentType { get; set; }

        public string accept { get; set; }

        public string userAgent { get; set; }

        public string method { get; set; }

        public int maxTry { get; set; }
    }
}
