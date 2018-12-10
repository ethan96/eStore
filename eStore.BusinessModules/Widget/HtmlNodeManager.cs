using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.IO;

namespace eStore.BusinessModules.Widget
{
    public class HtmlNodeManager
    {
        // 节点规则必须小写,LoadHtml后会把节点变成小写。 规则:/body
        // 运算规则符区分大小写。   <div id="Test">测试</div>   /div[@id="Test"]
        // "//*/body" 
        // "//*[@id=\"test\"]/span[1]"
        // //*/html/head/meta[@name=\"keywords\"]
        // DocumentNode.Descendants("meta")

        #region 暂时不要
        public static String retrieveHTMLSegmentX(String htmlContent, String regEx)
        {
            try
            {
                Regex reg = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match match = reg.Match(htmlContent);
                if (match.Success)
                    return match.Value;
                else
                    return "";
            }
            catch (ArgumentException)
            {
            }

            return "";
        }

        //返回 匹配的正则内容
        public static List<String> retrieveHTMLSegmentsX(String htmlContent, String regEx)
        {
            List<String> segments = new List<String>();
            try
            {
                Regex reg = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection matches = reg.Matches(htmlContent);
                foreach (Match match in matches)
                    segments.Add(match.Value);
            }
            catch (ArgumentException)
            {
            }

            return segments;
        }

        //返回 匹配的正则内容
        public static String retrieveHTMLSegmentInnerX(String htmlContent, String regEx)
        {
            try
            {
                Regex reg = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match match = reg.Match(htmlContent);
                if (match.Success && match.Groups.Count > 1)
                    return match.Groups[1].Value;
                else
                    return "";
            }
            catch (ArgumentException)
            {
            }

            return "";
        }
        #endregion
        
        //返回一个匹配的字符串. 
        //isSelf 是否包含节点自己
        public static String retrieveHTMLSegment(String htmlContent, String regEx, bool isSelf = false)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNode tableNode = doc.DocumentNode.SelectSingleNode(regEx);
                if (tableNode != null)
                {
                    if (isSelf)
                        return tableNode.OuterHtml;
                    else
                        return tableNode.InnerHtml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return "";
        }

        //返回所有匹配的字符串
        public static List<string> retrieveHTMLSegments(String htmlContent, String regEx, bool isSelf = false)
        {
            List<string> segments = new List<string>();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection tableNodeList = doc.DocumentNode.SelectNodes(regEx);
                if (tableNodeList != null && tableNodeList.Count > 0)
                {
                    foreach (HtmlNode nodeItem in tableNodeList)
                    {
                        if (isSelf)
                            segments.Add(nodeItem.OuterHtml);
                        else
                            segments.Add(nodeItem.InnerHtml);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return segments;
        }

        //获取所有匹配的 xpath
        public static List<string> retrieveHTMLSegmentXpath(String htmlContent, String regEx)
        {
            List<string> segments = new List<string>();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection tableNodeList = doc.DocumentNode.SelectNodes(regEx);
                if (tableNodeList != null && tableNodeList.Count > 0)
                {
                    foreach (HtmlNode nodeItem in tableNodeList)
                    {
                        segments.Add(nodeItem.XPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return segments;
        }

        //获取节点中的attribute value
        //<img src="test.jpg" />   attrKey=src得到test.jpg
        public static String retrieveHTMLAttribute(String htmlContent, string attrKey)
        {
            string attrValue = string.Empty;
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                if (doc.DocumentNode.FirstChild != null)
                {
                    HtmlNode nodeItem = doc.DocumentNode.FirstChild;
                    attrValue = nodeItem.GetAttributeValue(attrKey, "");
                    if (string.IsNullOrEmpty(attrValue))
                    {
                        HtmlAttribute attrItem = nodeItem.Attributes[attrKey];
                        //HtmlAttribute attrItem = nodeItem.Attributes.FirstOrDefault(p => p.Name.ToLower() == attrKey.ToLower());//Name.ToLower()
                        if (attrItem != null)
                            attrValue = attrItem.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return attrValue;
        }

        //返回 匹配对象的 所有Attribute
        public static Dictionary<string, string> retrieveHTMLAttributes(String htmlContent, String regEx)
        {
            Dictionary<string, string> widgetDic = new Dictionary<string, string>();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNode tableNode = doc.DocumentNode.SelectSingleNode(regEx);
                if (tableNode != null)
                {
                    foreach (HtmlAttribute attrItem in tableNode.Attributes)
                    {
                        if (!widgetDic.ContainsKey(attrItem.Name))
                            widgetDic.Add(attrItem.Name, attrItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return widgetDic;
        }

        #region 暂时没使用,load html 会把node弄乱
        //检查是否是唯一节点,如果有多个节点,删除它们  如:keywords description
        public static string checkUniqueNode(String htmlContent, String regEx, String fileName)
        {
            try
            {
                //运算符区分大小写
                List<string> xPathList = retrieveHTMLSegmentXpath(htmlContent.ToLower(), regEx);
                if (xPathList != null && xPathList.Count > 1)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);
                    for (int i = 1; i < xPathList.Count; i++)
                    {
                        HtmlNode removeNode = doc.DocumentNode.SelectSingleNode(xPathList[i]);
                        if (removeNode != null)
                            removeNode.Remove();
                    }
                    doc.Save(fileName, Encoding.UTF8);
                    return doc.DocumentNode.InnerHtml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlContent;
        }

        //暂时没用
        //检查html 是否有节点错误, 有就保存document加载完的。
        public static string checkHTMLDocument(String htmlContent, String fileName)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                if (doc.DocumentNode.OwnerDocument.ParseErrors != null && doc.DocumentNode.OwnerDocument.ParseErrors.Count() > 0)
                {
                    doc.Save(fileName, Encoding.UTF8);//记得编码
                    htmlContent = doc.DocumentNode.InnerHtml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlContent;
        }

        //修改html 节点
        public static string updateHTMLDocument(String htmlContent, String replaceContent, String regEx, String fileName)
        {
            try
            {
                string dirpath = fileName.Substring(0, fileName.LastIndexOf('\\'));
                if (Directory.Exists(dirpath) == false)
                    Directory.CreateDirectory(dirpath);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection tableNodeList = doc.DocumentNode.SelectNodes(regEx);
                if (tableNodeList != null && tableNodeList.Count > 0)
                {
                    foreach (HtmlNode nodeItem in tableNodeList)
                    {
                        nodeItem.InnerHtml = replaceContent;
                    }
                    doc.Save(fileName, Encoding.UTF8);
                    htmlContent = doc.DocumentNode.InnerHtml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlContent;
        }
        #endregion
        
        //检查html中node的错误信息
        public static string checkHTMLNode(String htmlContent)
        {
            StringBuilder errorHtml = new StringBuilder();
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                if (doc.DocumentNode.OwnerDocument.ParseErrors != null && doc.DocumentNode.OwnerDocument.ParseErrors.Count() > 0)
                {
                    foreach (HtmlParseError item in doc.DocumentNode.OwnerDocument.ParseErrors)
                    {
                        errorHtml.Append("[===]Line: " + item.Line + "  Line Position: " + item.LinePosition + " Error: " + item.Reason);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return errorHtml.ToString();
        }

        //根据文件地址 获取内容
        public static string loadFile(string filePath)
        {
            String fileContent = "";
            try
            {
                //记得编码
                StreamReader fileReader = new StreamReader(filePath, System.Text.Encoding.UTF8);
                fileContent = fileReader.ReadToEnd();
                fileReader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fileContent;

            #region 备注
            //UI以前用的buffer
            //StreamReader fileReader = new StreamReader(filePath);
            //StringBuilder buffer = new StringBuilder();
            //String line = "";

            //while ((line = fileReader.ReadLine()) != null)
            //    buffer.Append(line);

            //fileReader.Close();

            //String htmlContent = buffer.ToString();
            #endregion
        }

        //根据文件地址 添加修改内容
        public static void saveFile(string filePath, string filecontent)
        {
            StreamWriter fileWriter = null;
            string dirpath = filePath.Substring(0, filePath.LastIndexOf('\\'));
            if (Directory.Exists(dirpath) == false)
                Directory.CreateDirectory(dirpath);

            if (File.Exists(filePath))
                fileWriter = File.AppendText(filePath);
            else
                fileWriter = File.CreateText(filePath);
            try
            {
                fileWriter.Write(filecontent);
            }
            finally
            {
                fileWriter.Close();
            }
        }
    }
}
