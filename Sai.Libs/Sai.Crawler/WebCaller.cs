using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Sai.Crawler
{
    /// <summary>
    /// https 请求者
    /// </summary>
    public class HttpsCaller
    {
        private HttpsCaller()
        { }

        private static HttpsCaller m_Instance;

        public static HttpsCaller Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new HttpsCaller();
                return m_Instance;
            }
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开
            return true;
        }

        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Pars"></param>
        /// <returns></returns>
        public string GetUrl(string url, Dictionary<string, object> pars)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //https://api.paperpass.com/user/info
                string par = ParsToString(pars);
                if (!string.IsNullOrEmpty(par))
                {
                    par = "?" + par;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url + par));

                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Pars"></param>
        /// <returns></returns>
        public string GetUrl(string url)
        {
            try
            {
                //ServicePointManager.ServerCertificateValidationCallback = new
                    //System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //https://api.paperpass.com/user/info
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Pars">参数</param>
        /// <returns></returns>
        public string PostUrl(string url, Dictionary<string, object> Pars)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //https://api.paperpass.com/user/info
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                byte[] datas = EncodePars(Pars);

                request.Method = "POST";
                request.ContentLength = datas.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(datas, 0, datas.Length);
                    writer.Close();
                }

                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PostUrl(string url)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //https://api.paperpass.com/user/info
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                request.Method = "POST";

                request.ContentType = "application/x-www-form-urlencoded";

                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// api接口get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="getPars">get参数</param>
        /// <returns></returns>
        public string GetApi(string url, Dictionary<string, object> getPars)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                string par = ParsToString(getPars);
                if (!string.IsNullOrEmpty(par))
                {
                    par = "?" + par;
                }
                url += par;//添加get参数

                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";


                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 请求api地址Post调用
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headerPars">header参数</param>
        /// <param name="postPars">post参数</param>
        /// <returns>结果返回json</returns>
        public string PostApi(string url, Dictionary<string, object> headerPars, Dictionary<string, object> postPars)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                string par = ParsToString(headerPars);
                if (!string.IsNullOrEmpty(par))
                {
                    par = "?" + par;
                }
                url += par;//添加get参数

                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

                byte[] datas = EncodePars(postPars);

                request.Method = "POST";
                request.ContentLength = datas.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(datas, 0, datas.Length);
                    writer.Close();
                }

                //读取返回消息
                string res = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    res = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private byte[] EncodePars(Dictionary<string, object> Pars)
        {
            string parStr = ParsToString(Pars);
            return Encoding.UTF8.GetBytes(parStr);
        }

        private void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(data, 0, data.Length);
                writer.Close();
            }
        }

        private String ParsToString(Dictionary<string, object> pars)
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
                    valueStr= HttpUtility.UrlEncode(valueStr);
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + valueStr);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 支持http请求Post
    /// </summary>
    public class HttpCaller
    {
        private HttpCaller()
        { }

        private static HttpCaller m_Instance;

        public static HttpCaller Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new HttpCaller();
                return m_Instance;
            }
        }

        public static string QueryPostUrl(String URL, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);//+ "/" + MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            byte[] data = EncodePars(Pars);
            WriteRequestData(request, data);
            //读取返回消息
            string res = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                return null;//连接服务器失败
            }

            return res;
        }


        private static Hashtable _xmlNamespaces = new Hashtable();//缓存xmlNamespace，避免重复调用GetNamespace
        /// <summary>
        /// 需要WebService支持Post调用
        /// </summary>
        public static XmlDocument QueryPostWebService(String URL, String MethodName, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);//+ "/" + MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            byte[] data = EncodePars(Pars);
            WriteRequestData(request, data);
            return ReadXmlResponse(request.GetResponse());
        }


        /// <summary>
        /// 需要WebService支持Get调用
        /// </summary>
        public static XmlDocument QueryGetWebService(String URL, String MethodName, Hashtable Pars)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL + "/" + MethodName + "?" + ParsToString(Pars));
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            return ReadXmlResponse(request.GetResponse());
        }


        /// <summary>
        /// 通用WebService调用(Soap),参数Pars为String类型的参数名、参数值
        /// </summary>
        public static XmlDocument QuerySoapWebService(String URL, String MethodName, Hashtable Pars)
        {
            if (_xmlNamespaces.ContainsKey(URL))
            {
                return QuerySoapWebService(URL, MethodName, Pars, _xmlNamespaces[URL].ToString());
            }
            else
            {
                return QuerySoapWebService(URL, MethodName, Pars, GetNamespace(URL));
            }
        }

        private static XmlDocument QuerySoapWebService(String URL, String MethodName, Hashtable Pars, string XmlNs)
        {
            _xmlNamespaces[URL] = XmlNs;//加入缓存，提高效率
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "\"" + XmlNs + (XmlNs.EndsWith("/") ? "" : "/") + MethodName + "\"");
            SetWebRequest(request);
            byte[] data = EncodeParsToSoap(Pars, XmlNs, MethodName);
            WriteRequestData(request, data);
            XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
            doc = ReadXmlResponse(request.GetResponse());
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            String RetXml = doc.SelectSingleNode("//soap:Body/*/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + RetXml + "</root>");
            AddDelaration(doc2);
            return doc2;
        }

        private static string GetNamespace(String URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + "?WSDL");
            SetWebRequest(request);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            sr.Close();
            return doc.SelectSingleNode("//@targetNamespace").Value;
        }

        private static byte[] EncodeParsToSoap(Hashtable Pars, String XmlNs, String MethodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            AddDelaration(doc);
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapMethod = doc.CreateElement(MethodName);
            soapMethod.SetAttribute("xmlns", XmlNs);
            foreach (string k in Pars.Keys)
            {
                XmlElement soapPar = doc.CreateElement(k);
                soapPar.InnerXml = ObjectToSoapXml(Pars[k]);
                soapMethod.AppendChild(soapPar);
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }

        private static string ObjectToSoapXml(object o)
        {
            XmlSerializer mySerializer = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            mySerializer.Serialize(ms, o);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(ms.ToArray()));
            if (doc.DocumentElement != null)
            {
                return doc.DocumentElement.InnerXml;
            }
            else
            {
                return o.ToString();
            }
        }

        private static void SetWebRequest(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 10000;
        }

        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(data, 0, data.Length);
                writer.Close();
            }
        }

        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }

        private static String ParsToString(Hashtable Pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in Pars.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
                //sb.Append(HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(Pars[k].ToString()));
            }
            return sb.ToString();
        }

        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }
        private static void AddDelaration(XmlDocument doc)
        {
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
        }
    }
}
