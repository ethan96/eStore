using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace esUtilities
{
    public class StringUtility
    {
        //返回字符串字节长度
        public static int GetStringEncodedLength(string content)
        {
            content = content.Replace("®", "*").Replace("\r\n", " ");
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(content);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StringEncry(string str,string key = "")
        {
            var md5Hasher = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            var data = md5Hasher.ComputeHash(Encoding.Unicode.GetBytes(key + str));
            var sBuilder = new StringBuilder();
            for (var c = 0; c < data.Length - 1; c++)
            {
                sBuilder.Append(data[c].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static bool checkTextLength(Int32 mixLengthX, Int32 maxLengthX, string content,string toolTip, ref string errorMessage)
        {
            if (mixLengthX == 0 && maxLengthX == 0)
                return true;

            int len = GetStringEncodedLength(content);
            if (len < mixLengthX || len > maxLengthX)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = string.Format("{0}'s length must between {1} and {2}.", toolTip, mixLengthX, maxLengthX);
                return false;
            }
            else
                return true;
        }

        public static bool checkBulletPoints(Int32 maxBulletPoints, Int32 mixLengthX, string content, string toolTip, ref string errorMessage)
        {
            int count = 0;
            if (maxBulletPoints == 0)
                return true;

            if (string.IsNullOrEmpty(content) && mixLengthX != 0)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = string.Format("{0} can not be empty.",toolTip);
                return false;
            }

            try
            {
                count = Regex.Matches(content, @"<li>.*?", RegexOptions.IgnoreCase).Count;
            }
            catch (ArgumentException)
            {
                count = 0;
            }

            if (count == 0)
                return true;
            else if (count > maxBulletPoints)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = string.Format("{0}'s Bullet Points over {1}.", toolTip, maxBulletPoints);
                return false;
            }
            else
                return true;
        }

        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// 去除特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="exeptat">是否排除@</param>
        /// <returns></returns>
        public static string replaceSpecialString(string str, bool exeptat = false)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            str = Regex.Replace(str, @"<[^>]*?>", "");
            var spstr = @"[~!@#$%\^*()+=|{}':;'\\]";
            if (exeptat)
                spstr = @"[~!#$%\^*()+=|{}':;'\\]";
            return System.Web.HttpUtility.HtmlDecode(Regex.Replace(str, spstr, ""));
        }


        static Random r = new Random();
        /// <summary>
        /// 根据权重抽奖
        /// </summary>
        /// <param name="ls"></param>
        /// <returns></returns>
        public static int lottery(Dictionary<int, decimal> ls)
        {
            int id = 0;
            Dictionary<int, decimal> newls = new Dictionary<int, decimal>();
            decimal ct = 0;
            foreach (var c in ls)
            {
                ct += c.Value;
                newls.Add(c.Key, ct);
            }
            var key = (r).Next((int)Math.Round(ct, 0));
            foreach (var v in newls)
            {
                if (key <= v.Value)
                {
                    id = v.Key;
                    break;
                }
            }
            return id;
        }

        /// <summary>
        /// 去除html标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(string html, int length = 0, string end = "")
        {
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length) + end;

            return strText;
        }

        /// <summary>
        /// 按字节长度截取字符串(支持截取带HTML代码样式的字符串)
        /// </summary>
        /// <param name="param">将要截取的字符串参数</param>
        /// <param name="length">截取的字节长度</param>
        /// <param name="end">字符串末尾补上的字符串</param>
        /// <returns>返回截取后的字符串</returns>
        public static string substringToHTML(string param, int length, string end)
        {
            if (string.IsNullOrEmpty(param))
                return "";
            if (param.Length <= length)
                end = "";
            string Pattern = null;
            MatchCollection m = null;
            StringBuilder result = new StringBuilder();
            int n = 0;
            char temp;
            bool isCode = false; //是不是HTML代码
            bool isHTML = false; //是不是HTML特殊字符,如&nbsp;
            char[] pchar = param.ToCharArray();
            for (int i = 0; i < pchar.Length; i++)
            {
                temp = pchar[i];
                if (temp == '<')
                {
                    isCode = true;
                }
                else if (temp == '&')
                {
                    isHTML = true;
                }
                else if (temp == '>' && isCode)
                {
                    n = n - 1;
                    isCode = false;
                }
                else if (temp == ';' && isHTML)
                {
                    isHTML = false;
                }

                if (!isCode && !isHTML)
                {
                    n = n + 1;
                    //UNICODE码字符占两个字节
                    if (System.Text.Encoding.Default.GetBytes(temp + "").Length > 1)
                    {
                        n = n + 1;
                    }
                }

                result.Append(temp);
                if (n >= length)
                {
                    break;
                }
            }
            result.Append(end);
            //取出截取字符串中的HTML标记
            string temp_result = result.ToString().Replace("(>)[^<>]*(<?)", "$1$2");
            //去掉不需要结素标记的HTML标记
            temp_result = Regex.Replace(temp_result, @"<(area|base|basefont|body|br|col|colgroup|dd|dt|frame|head|hr|html|img|input|isindex|li|link|meta|option|p|param|tbody|td|tfoot|th|thead|tr)[ ]?/?>"
        , "", RegexOptions.IgnoreCase);
            //去掉成对的HTML标记
            temp_result = Regex.Replace(temp_result, @"<([a-zA-Z]+)[^<>]*>(.*?)</\1>", "$2");
            //用正则表达式取出标记
            Pattern = ("<([a-zA-Z]+)[^<>]*>");
            m = Regex.Matches(temp_result, Pattern);

            ArrayList endHTML = new ArrayList();

            foreach (Match mt in m)
            {
                endHTML.Add(mt.Result("$1"));
            }
            //补全不成对的HTML标记
            for (int i = endHTML.Count - 1; i >= 0; i--)
            {
                result.Append("</");
                result.Append(endHTML[i]);
                result.Append(">");
            }

            return result.ToString();
        }

        /// <summary>
        /// 格式化时间 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string subDate(object obj)
        {
            if (obj == null)
                return DateTime.Now.ToString("MM/dd/yyyy");
            DateTime date;
            DateTime.TryParse(obj.ToString(), out date);
            return date.ToString("MM/dd/yyyy");
        }

        // 除去所有在html元素中标记
        public static string striphtml(string strhtml)
        {
            string stroutput = strhtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            stroutput = regex.Replace(stroutput, "");
            return stroutput;
        }

        public static string subString(string str, int length)
        {
            if(string.IsNullOrEmpty(str))
                return "";
            if (str.Length > length)
                return str.Substring(0, length) + "...";
            else
                return str;
        }

        public static bool CheckEmail(string email)
        {
            string emailStr = @"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";
            //邮箱正则表达式对象
            Regex emailReg = new Regex(emailStr);
            return emailReg.IsMatch(email.Trim());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curUrl"></param>
        /// <param name="rep"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string SubUrl(string curUrl, Dictionary<string, object> rep, string exp = "")
        {
            if (string.IsNullOrEmpty(curUrl))
                return "";
            var temp = curUrl.Substring(1, curUrl.Length - 1);
            foreach (var item in temp.Split('&'))
            {
                if (item.IndexOf("=") > 0)
                {
                    var kv = item.Split('=');
                    if (!rep.Keys.Contains(kv[0]))
                        rep.Add(kv[0], kv[1]);
                }
            }
            var repTemp = rep.Where(c => c.Value != null && (string.IsNullOrEmpty(exp) ? true : (c.Key != exp))).ToArray();
            if (repTemp == null || !repTemp.Any())
                return "";
            else
            {
                var u = "?";
                foreach (var item in repTemp)
                {
                    if (u != "?")
                        u += "&";
                    u += $"{item.Key}={item.Value.ToString()}";
                }
                return u; 
            }
        }

        /// <summary>
        /// get safe time
        /// </summary>
        /// <param name="timestr"></param>
        /// <returns></returns>
        public static DateTime GetSafeTime(string timestr)
        {
            if (string.IsNullOrEmpty(timestr))
                return DateTime.Now;
            DateTime dt = DateTime.Now;
            DateTime.TryParse(timestr, out dt);
            return dt;
        }
        public static string GetSafeSQLString(string keyworks)
        {
            string[] strs = new string[] { "   ", "  ", "-","&"," ", ",","'" };
            foreach (var item in strs)
                keyworks = keyworks.Replace(item, ";");
            return keyworks;
        }

        /// <summary>
        /// get safe int
        /// </summary>
        /// <param name="intstr"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static int GetSafeInt(string intstr, int def = 0)
        {
            if (string.IsNullOrEmpty(intstr))
                return def;
            int i = 0;
            if (int.TryParse(intstr, out i))
                return i;
            else
                return def;
        }

        public static bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;
            return !(url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"));
        }

        public static string ReplaceString(string input, string output, string context)
        {
            if (string.IsNullOrEmpty(context))
                return "";
            var str = context;
            Regex regex = new Regex(input, RegexOptions.IgnoreCase);
            MatchCollection ls = regex.Matches(context);
            List<string> items = new List<string>();
            if (ls.Count > 0)
            {
                foreach (Match item in ls)
                {
                    if (item.Success && !items.Contains(item.Value))
                        items.Add(item.Value);
                }
            }
            foreach (var item in items)
                str = str.Replace(item, string.Format(output, item));
            return str;
        }

        public static void ReplaceString(string input, string output, ref string context)
        {
            Regex regex = new Regex(input, RegexOptions.IgnoreCase);
            MatchCollection ls = regex.Matches(context);
            List<string> items = new List<string>();
            if (ls.Count > 0)
            {
                foreach (Match item in ls)
                {
                    if (item.Success && !items.Contains(item.Value))
                        items.Add(item.Value);
                }
            }
            foreach (var item in items)
                context = context.Replace(item, string.Format(output, item));
        }

    }
}
