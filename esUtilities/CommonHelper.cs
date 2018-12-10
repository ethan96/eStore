
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Numerics;

namespace esUtilities
{
    public class CommonHelper
    {
        #region Methods
        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            bool result = false;
            if (String.IsNullOrEmpty(email))
                return result;
            email = email.Trim();
            result = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return result;
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static string QueryString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.QueryString[name] != null)
                result = HttpContext.Current.Request.QueryString[name].ToString();
            return result;
        }

        /// <summary>
        /// Gets boolean value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static bool QueryStringBool(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            return (resultStr == "YES" || resultStr == "TRUE" || resultStr == "1");
        }

        /// <summary>
        /// Gets integer value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static int QueryStringInt(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            int result;
            Int32.TryParse(resultStr, out result);
            return result;
        }

        /// <summary>
        /// Gets integer value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Query string value</returns>
        public static int QueryStringInt(string name, int defaultValue)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            if (resultStr.Length > 0)
            {
                return Int32.Parse(resultStr);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets GUID value from query string 
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Query string value</returns>
        public static Guid? QueryStringGuid(string name)
        {
            string resultStr = QueryString(name).ToUpperInvariant();
            Guid? result = null;
            try
            {
                result = new Guid(resultStr);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Gets Form String
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Result</returns>
        public static string GetFormString(string name)
        {
            string result = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request[name] != null)
                result = HttpContext.Current.Request[name].ToString();
            return result;
        }

        /// <summary>
        /// Set meta http equiv (eg. redirects)
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="httpEquiv">Http Equiv</param>
        /// <param name="content">Content</param>
        public static void SetMetaHttpEquiv(Page page, string httpEquiv, string content)
        {
            if (page.Header == null)
                return;

            HtmlMeta meta = new HtmlMeta();
            if (page.Header.FindControl("meta" + httpEquiv) != null)
            {
                meta = (HtmlMeta)page.Header.FindControl("meta" + httpEquiv);
                meta.Content = content;
            }
            else
            {
                meta.ID = "meta" + httpEquiv;
                meta.HttpEquiv = httpEquiv;
                meta.Content = content;
                page.Header.Controls.Add(meta);
            }
        }

        /// <summary>
        /// Selects item
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="value">Value to select</param>
        public static void SelectListItem(DropDownList list, object value)
        {
            if (list.Items.Count != 0)
            {
                var selectedItem = list.SelectedItem;
                if (selectedItem != null)
                    selectedItem.Selected = false;
                if (value != null)
                {
                    selectedItem = list.Items.FindByValue(value.ToString());
                    if (selectedItem != null)
                        selectedItem.Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public static string ServerVariables(string name)
        {
            string tmpS = string.Empty;
            try
            {
                if (HttpContext.Current.Request.ServerVariables[name] != null)
                {
                    tmpS = HttpContext.Current.Request.ServerVariables[name].ToString();
                }
            }
            catch
            {
                tmpS = string.Empty;
            }
            return tmpS;
        }

        /// <summary>
        /// Gets a value indicating whether requested page is an admin page
        /// </summary>
        /// <returns>A value indicating whether requested page is an admin page</returns>
        public static bool IsAdmin()
        {
            string thisPageUrl = GetThisPageUrl(false);
            if (string.IsNullOrEmpty(thisPageUrl))
                return false;

            string adminUrl1 = GetStoreLocation(false) + "administration";
            string adminUrl2 = GetStoreLocation(true) + "administration";
            bool flag1 = thisPageUrl.ToLowerInvariant().StartsWith(adminUrl1.ToLower());
            bool flag2 = thisPageUrl.ToLowerInvariant().StartsWith(adminUrl2.ToLower());
            bool isAdmin = flag1 || flag2;
            return isAdmin;
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public static bool IsCurrentConnectionSecured()
        {
            bool useSSL = false;
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                useSSL = HttpContext.Current.Request.IsSecureConnection;
                //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true, use the statement below
                //just uncomment it
                //useSSL = HttpContext.Current.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }

            return useSSL;
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <returns></returns>
        public static string GetThisPageUrl(bool includeQueryString)
        {
            string URL = string.Empty;
            if (HttpContext.Current == null)
                return URL;

            if (includeQueryString)
            {
                bool useSSL = IsCurrentConnectionSecured();
                string storeHost = GetStoreHost(useSSL);
                if (storeHost.EndsWith("/"))
                    storeHost = storeHost.Substring(0, storeHost.Length - 1);
                URL = storeHost + HttpContext.Current.Request.RawUrl;
            }
            else
            {
                URL = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            }
            URL = URL.ToLowerInvariant();
            return URL;
        }

        public static string SubString(string source, int length, bool withellipsis = true)
        {
            if (string.IsNullOrEmpty(source) || length == 0)
                return string.Empty;
            else
            {
                if (source.Length <= length)
                    return source;
                else
                    if (withellipsis)
                        return source.Substring(0, length) + "...";
                    else
                        return source.Substring(0, length);
            }
        }

        //根据字节，取出字符串
        public static string SubStringByBytes(string source, int ByteNum, bool HaveDot = true)
        {
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(source);
            byte[] newbyte = new byte[ByteNum];
            string returnstr = string.Empty;
            int len = 0;
            if (mybyte.Length < ByteNum)
                returnstr = source;
            else
            {
                if (HaveDot)
                    ByteNum = ByteNum - 4;
                byte[] s = new ASCIIEncoding().GetBytes(source);
                for (int i = 0; i < s.Length; i++)
                {
                    if ((int)s[i] == 63)
                    {
                        if (len + 2 > ByteNum)
                            break;
                        len += 2;
                    }
                    else
                    {
                        if (len + 1 > ByteNum)
                            break;
                        len += 1;
                    }
                }

                for (int i = 0; i < len; i++)
                {
                    newbyte[i] = mybyte[i];
                }

                returnstr = System.Text.Encoding.Default.GetString(newbyte);
                if (HaveDot)
                    returnstr += "...";
            }
            return returnstr;
        }

        public static string RemoveHtmlTags(string source, bool ReturnSourceOnError = false)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            else
            {

                try
                {
                    return Regex.Replace(source, "</?[a-zA-Z][a-zA-Z0-9]*[^<>]*>", "");
                }
                catch (ArgumentException ex)
                {
                    eStore.Utilities.eStoreLoger.Warn("RemoveHtmlTags Error:" + HttpUtility.HtmlEncode(source), "", "", "", ex);
                    if (ReturnSourceOnError)
                        return source;
                    else
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <returns>Store location</returns>
        public static string GetStoreLocation()
        {
            bool useSSL = IsCurrentConnectionSecured();
            return GetStoreLocation(useSSL);
        }

        /// <summary>
        /// get ftp server file path
        /// </summary>
        /// <returns></returns>
        public static string GetStoreFtpServerPath(string filePath)
        {
            if (filePath.ToLower().Contains("resource"))
                return "http://advantech.vo.llnwd.net/o35/eStore" + filePath.Substring(filePath.IndexOf("resource"));
            filePath = filePath.Replace("../", "/").Replace("~/", "/");
            if (filePath.StartsWith("/"))
                return "http://advantech.vo.llnwd.net/o35/eStore" + filePath;
            else
                return "http://advantech.vo.llnwd.net/o35/eStore/" + filePath;
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store location</returns>
        public static string GetStoreLocation(bool useSsl)
        {
            string result = GetStoreHost(useSsl);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            result = result + HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }
        public static string GetStoreRelativeLocation()
        {
            string result = HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }
        /// <summary>
        /// Gets store admin location
        /// </summary>
        /// <returns>Store admin location</returns>
        public static string GetStoreAdminLocation()
        {
            bool useSSL = IsCurrentConnectionSecured();
            return GetStoreAdminLocation(useSSL);
        }

        /// <summary>
        /// Gets store admin location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store admin location</returns>
        public static string GetStoreAdminLocation(bool useSsl)
        {
            string result = GetStoreLocation(useSsl);
            result += "Administration/";

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location</returns>
        public static string GetStoreHost(bool useSsl)
        {
            string result = "http://" + ServerVariables("HTTP_HOST");
            if (!result.EndsWith("/"))
                result += "/";

            if (useSsl)
            {
                result = result.Replace("http:/", "https:/");
            }



            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Reloads current page
        /// </summary>
        public static void ReloadCurrentPage()
        {
            bool useSSL = IsCurrentConnectionSecured();
            ReloadCurrentPage(useSSL);
        }

        /// <summary>
        /// Reloads current page
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        public static void ReloadCurrentPage(bool useSsl)
        {
            string storeHost = GetStoreHost(useSsl);
            if (storeHost.EndsWith("/"))
                storeHost = storeHost.Substring(0, storeHost.Length - 1);
            string url = storeHost + HttpContext.Current.Request.RawUrl;
            url = url.ToLowerInvariant();
            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="targetLocationModification">Target location modification</param>
        /// <returns>New url</returns>
        public static string ModifyQueryString(string url, string queryStringModification, string targetLocationModification)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryStringModification == null)
                queryStringModification = string.Empty;
            queryStringModification = queryStringModification.ToLowerInvariant();

            if (targetLocationModification == null)
                targetLocationModification = string.Empty;
            targetLocationModification = targetLocationModification.ToLowerInvariant();


            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(targetLocationModification))
            {
                str2 = targetLocationModification;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        public static string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();


            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        public static string ResolveUrl(string relativeUrl)
        {
            if (relativeUrl == null)
                return "";
            if (relativeUrl.Length == 0 || relativeUrl[0] == '/' ||
                relativeUrl[0] == '\\') return relativeUrl;
            int idxOfScheme =
              relativeUrl.IndexOf(@"://", StringComparison.Ordinal);
            if (idxOfScheme != -1)
            {
                int idxOfQM = relativeUrl.IndexOf('?');
                if (idxOfQM == -1 || idxOfQM > idxOfScheme) return relativeUrl;
            }
            StringBuilder sbUrl = new StringBuilder();
            sbUrl.Append(HttpRuntime.AppDomainAppVirtualPath);
            if (sbUrl.Length == 0 || sbUrl[sbUrl.Length - 1] != '/') sbUrl.Append('/');
            //　found　question　mark　already?　query　string,　do　not　touch!
            bool foundQM = false;
            bool foundSlash;　//　the　latest　char　was　a　slash?
            if (relativeUrl.Length > 1
                && relativeUrl[0] == '~'
                && (relativeUrl[1] == '/' || relativeUrl[1] == '\\'))
            {
                relativeUrl = relativeUrl.Substring(2);
                foundSlash = true;
            }
            else foundSlash = false;
            foreach (char c in relativeUrl)
            {
                if (!foundQM)
                {
                    if (c == '?') foundQM = true;
                    else
                    {
                        if (c == '/' || c == '\\')
                        {
                            if (foundSlash) continue;
                            else
                            {
                                sbUrl.Append('/');
                                foundSlash = true;
                                continue;
                            }
                        }
                        else if (foundSlash) foundSlash = false;
                    }
                }
                sbUrl.Append(c);
            }
            return sbUrl.ToString();
        }

        /// <summary>
        /// will change / to /VirtualPath/
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string ConvertToAppVirtualPath(string relativeUrl)
        {
            if (relativeUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                return relativeUrl;

            if (relativeUrl.Length > 0 && relativeUrl[0] == '/')
            {
                relativeUrl=relativeUrl.Insert(0, "~");
            }
            return ResolveUrl(relativeUrl);
        }
        /// <summary>
        /// Ensures that requested page is secured (https://)
        /// </summary>
        public static void EnsureSsl()
        {
            if (!IsCurrentConnectionSecured())
            {
                //bool useSSL = false;
                //if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSSL"]))
                //    useSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSL"]);
                //if (useSSL)
                //{
                //    //if (!HttpContext.Current.Request.Url.IsLoopback)
                //    //{
                //    ReloadCurrentPage(true);
                //    //}
                //}
            }
        }

        /// <summary>
        /// Ensures that requested page is not secured (http://)
        /// </summary>
        public static void EnsureNonSsl()
        {
            if (IsCurrentConnectionSecured())
            {
                ReloadCurrentPage(false);
            }
        }

        /// <summary>
        /// Sets cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="cookieValue">Cookie value</param>
        /// <param name="ts">Timespan</param>
        public static void SetCookie(string cookieName, string cookieValue, TimeSpan ts)
        {
            try
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Value = HttpContext.Current.Server.UrlEncode(cookieValue);
                DateTime dt = DateTime.Now;
                cookie.Expires = dt.Add(ts);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        /// <summary>
        /// Gets cookie string
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="decode">Decode cookie</param>
        /// <returns>Cookie string</returns>
        public static string GetCookieString(string cookieName, bool decode)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] == null)
            {
                return string.Empty;
            }
            try
            {
                string tmp = HttpContext.Current.Request.Cookies[cookieName].Value.ToString();
                if (decode)
                    tmp = HttpContext.Current.Server.UrlDecode(tmp);
                return tmp;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets boolean value from cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Result</returns>
        public static bool GetCookieBool(string cookieName)
        {
            string str1 = GetCookieString(cookieName, true).ToUpperInvariant();
            return (str1 == "TRUE" || str1 == "YES" || str1 == "1");
        }

        /// <summary>
        /// Gets integer value from cookie
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <returns>Result</returns>
        public static int GetCookieInt(string cookieName)
        {
            string str1 = GetCookieString(cookieName, true);
            if (!String.IsNullOrEmpty(str1))
                return Convert.ToInt32(str1);
            else
                return 0;
        }


        /// <summary>
        /// Write XML to response
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="fileName">Filename</param>
        public static void WriteResponseXml(string xml, string fileName)
        {
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                XmlDeclaration decl = document.FirstChild as XmlDeclaration;
                if (decl != null)
                {
                    decl.Encoding = "utf-8";
                }
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/xml";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
                response.BinaryWrite(Encoding.UTF8.GetBytes(document.InnerXml));
                response.End();
            }
        }

        /// <summary>
        /// Write text to response
        /// </summary>
        /// <param name="txt">text</param>
        /// <param name="fileName">Filename</param>
        public static void WriteResponseTxt(string txt, string fileName)
        {
            if (!String.IsNullOrEmpty(txt))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/plain";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
                response.BinaryWrite(Encoding.UTF8.GetBytes(txt));
                response.End();
            }
        }

        /// <summary>
        /// Write XLS file to response
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="targetFileName">Target file name</param>
        public static void WriteResponseXls(string filePath, string targetFileName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/xls";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", targetFileName));
                response.BinaryWrite(File.ReadAllBytes(filePath));
                response.End();
            }
        }

        /// <summary>
        /// Write PDF file to response
        /// </summary>
        /// <param name="filePath">File napathme</param>
        /// <param name="targetFileName">Target file name</param>
        /// <remarks>For BeatyStore project</remarks>
        public static void WriteResponsePdf(string filePath, string targetFileName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Charset = "utf-8";
                response.ContentType = "text/pdf";
                response.AddHeader("content-disposition", string.Format("attachment; filename={0}", targetFileName));
                response.BinaryWrite(File.ReadAllBytes(filePath));
                response.End();
            }
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Covnerted string</returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        /// <summary>
        /// Fills drop down list with values of enumaration
        /// </summary>
        /// <param name="list">Dropdownlist</param>
        /// <param name="enumType">Enumeration</param>
        public static void FillDropDownWithEnum(DropDownList list, Type enumType)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType must be enum type");
            }

            list.Items.Clear();
            string[] strArray = Enum.GetNames(enumType);
            foreach (string str2 in strArray)
            {
                int enumValue = (int)Enum.Parse(enumType, str2, true);
                ListItem ddlItem = new ListItem(CommonHelper.ConvertEnum(str2), enumValue.ToString());
                list.Items.Add(ddlItem);
            }
        }

        /// <summary>
        /// Set response NoCache
        /// </summary>
        /// <param name="response">Response</param>
        public static void SetResponseNoCache(HttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            //response.Cache.SetCacheability(HttpCacheability.NoCache) 

            response.CacheControl = "private";
            response.Expires = 0;
            response.AddHeader("pragma", "no-cache");
        }

        private static int switchtoLocal = 0;
        public static String IPtoNation(String sourceIP)
        {
            String nation = "";
            if (switchtoLocal == 0)
            {
                nation = IPtoNationGeoIPCDatabase(sourceIP);
                if (nation == "ERROR")
                {
                    switchtoLocal = 50;
                    nation =  IPtoNationwipmania(sourceIP);
                }
            }
            else
            {
                nation = IPtoNationwipmania(sourceIP);
                switchtoLocal--;
            }
            return nation;
        }

        private static String IPtoNationwipmania(String sourceIP)
        {
            String url = "http://api.wipmania.com/" + sourceIP.Trim() + "?k=NoA-k2UCnQvC0tSmTMIR7Ji23A1";
            String nation = "";

            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 2000;
                WebResponse webResponse = webRequest.GetResponse();

                StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding(1252));

                nation = responseStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Warn("IPToNation failed", sourceIP, "", "", ex);
                nation = "ERROR";
            }

            return nation;
        }

        private static String IPtoNationwipmaniaFromLoacl(String sourceIP)
        {
            String url = "http://172.21.128.41:7080/default.aspx?ip=" + sourceIP.Trim();
            String nation = "";

            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Timeout = 2000;
                WebResponse webResponse = webRequest.GetResponse();

                StreamReader responseStream = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding(1252));

                nation = responseStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Warn("IPToNation failed", sourceIP, "", "", ex);
                nation = "ERROR";
            }

            return nation;
        }


        public static String IPtoNationGeoIPCDatabase(String sourceIP)
        {
            Int64 ip = convertIPtoInteger(sourceIP);

            if (ip == 0)
            {
                BigInteger iPV6 = convertIPtoBigInt(sourceIP);
                if (iPV6 == 0)
                    return "XX";
                else
                    return getCountryShortFromGeoIPV6CDatabase(iPV6.ToString());
            }
            else
            {
                return getCountryshortfromGeoIPCDatabase(ip);

            }
        }
        private static string getCountryshortfromGeoIPCDatabase(Int64 ip)
        {
            //string CommText = string.Format("select shorts from GeoLoc where   {0} between ipfromN and   iptoN ", ip);
            //Change to new IP tables.
            string table = DateTime.Now.AddDays(-3).Month % 2 > 0 ? "Ip2Location_Ip2Country_A" : "Ip2Location_Ip2Country_B";
            string CommText = string.Format("select shorts from  {0}  where  {1}  between  IpFrom  and  IpTo ", table, ip);

            string connString = ConfigurationManager.ConnectionStrings["GeoIP"].ToString();
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();

                    if (con.State == System.Data.ConnectionState.Closed)
                        con.Open();

                    SqlCommand cmd = new SqlCommand(CommText, con);
                    cmd.CommandType = CommandType.Text;
                    return cmd.ExecuteScalar() != null ? (string)cmd.ExecuteScalar() : "XX";
                }
            }
            catch (Exception)
            {
                return "ERROR";
            }
            
        }

        private static string getCountryShortFromGeoIPV6CDatabase(string iPV6)
        {
            string table = DateTime.Now.AddDays(-3).Month % 2 > 0 ? "Ip2Location_Ip2Country_IPV6_A" : "Ip2Location_Ip2Country_IPV6_B";
            string CommText = string.Format("select top 1 shorts from  {0} where '{1}' <= IpTo ", table, iPV6);

            string connString = ConfigurationManager.ConnectionStrings["GeoIP"].ToString();
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();

                    if (con.State == System.Data.ConnectionState.Closed)
                        con.Open();

                    SqlCommand cmd = new SqlCommand(CommText, con);
                    cmd.CommandType = CommandType.Text;
                    return cmd.ExecuteScalar() != null ? (string)cmd.ExecuteScalar() : "XX";
                }
            }
            catch (Exception)
            {
                return "ERROR";
            }

        }

        private static Int64 convertIPtoInteger(string ip)
        {
            Int64 rlt = 0;
            string[] ips = ip.Split('.');
            Int64 a, b, c, d;
            if (ips.Length == 4)
            {
                if (Int64.TryParse(ips[0], out a) &&
                    Int64.TryParse(ips[1], out b) &&
                    Int64.TryParse(ips[2], out c) &&
                    Int64.TryParse(ips[3], out d))
                {
                    rlt = 16777216 * a + 65536 * b + 256 * c + d;
                }
            }
            return rlt;
        }

        private static BigInteger convertIPtoBigInt(string ipv6)
        {
            System.Net.IPAddress address;
            BigInteger ipnum = 0; //BigInteger 可用來處理超大整數
            
            if (System.Net.IPAddress.TryParse(ipv6, out address))
            {
                byte[] addrBytes = address.GetAddressBytes();

                if (System.BitConverter.IsLittleEndian)
                {
                    System.Collections.Generic.List<byte> byteList = new System.Collections.Generic.List<byte>(addrBytes);
                    byteList.Reverse();
                    addrBytes = byteList.ToArray();
                }

                if (addrBytes.Length > 8)
                {
                    //IPv6
                    ipnum = System.BitConverter.ToUInt64(addrBytes, 8);
                    ipnum <<= 64;
                    ipnum += System.BitConverter.ToUInt64(addrBytes, 0);
                }
                else
                {
                    //IPv4
                    ipnum = System.BitConverter.ToUInt32(addrBytes, 0);
                }
                return ipnum;
            }
            return ipnum;
        }

        /// <summary>
        /// 修改上标的显示
        /// </summary>
        /// <param name="oldStr"></param>
        /// <returns></returns>
        public static string replaceSpecialSymbols(string oldStr)
        {
            if (!string.IsNullOrEmpty(oldStr))
            {
                string[] specialSymbols = new string[] { "®", "&#174;" };
                foreach (var c in specialSymbols)
                {
                    oldStr = oldStr.Replace("<sup>" + c + "</sup>", c); //保证只有一对sup
                    oldStr = oldStr.Replace(c, "<sup>" + c + "</sup>");
                }
                return oldStr;
            }
            return "";
        }

        #endregion

        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="list">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> entitys)
        {
            if (entitys == null || entitys.Count < 1)
                throw new Exception("entitys is null!");

            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            DataTable dt = new DataTable("tb01");
            for (int i = 0; i < entityProperties.Length; i++)
                dt.Columns.Add(entityProperties[i].Name);


            foreach (object entity in entitys)
            {
                if (entity.GetType() != entityType)
                    throw new Exception("not same type!");
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                    entityValues[i] = entityProperties[i].GetValue(entity, null);

                dt.Rows.Add(entityValues);
            }
            return dt;
        }

        /// <summary>
        /// fix local image url path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string fixLocalImgPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "/images/photounavailable.gif";
            string _path = path.ToLower();
            if (_path.StartsWith("http"))
                return path;
            else if (_path.StartsWith("/resource"))
                return path;
            else if (_path.StartsWith("resource"))
                return path;
            else if (_path.StartsWith("/"))
                return "/resource" + path;
            else if (_path.StartsWith("~/resource"))
                return path.Replace("~/", "/");
            else
                return "resource/" + path;
        }
    }
}
