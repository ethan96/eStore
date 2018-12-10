using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;
using System.IO;
using System.Text.RegularExpressions;
using eStore.POCOS;

namespace eStore.Presentation.Widget
{
    public class WidgetHandlerV2 : BusinessModules.Widget.HtmlNodeManager
    {
        /// This method is a static method, it takes a full path file name. The input file has to be HTML file.
        /// In return, it returns HTMLPage instance. 
        /// It will try to identify widget enable regions and replace widget value holders with real values.
        public static HTMLPage processHTMLFile(String fileName, BusinessModules.Store store, String leadingURL, string OriginalURL = "")
        {
            HTMLPage htmlPage = new HTMLPage();
            try
            {
                //生成一个resolvedfile html
                string resolvedfilepath = fileName.Insert(fileName.LastIndexOf('\\'), "\\resolvedfile");
                String htmlContent = string.Empty;

                if (File.Exists(resolvedfilepath) && (new FileInfo(resolvedfilepath)).LastWriteTimeUtc > (new FileInfo(fileName)).LastWriteTimeUtc)
                {
                    //直接 加载resolvedfile
                    htmlContent = loadFile(resolvedfilepath);
                }
                else
                {
                    string tempContent = loadFile(fileName);
                    retrieveHTMLBody(tempContent, htmlPage);//body

                    alternateLinkInHTMLBody(htmlPage, leadingURL, OriginalURL);//替换a/img标签 路径

                    try
                    {
                        htmlContent = updateHTMLDocument(tempContent, htmlPage.body.value, @"//*/body", resolvedfilepath);
                        if (string.IsNullOrEmpty(htmlContent))
                            htmlContent = tempContent;
                    }
                    catch (ArgumentException ex)
                    {
                        throw ex;
                    }
                }

                retrieveHTMLHeader(htmlContent, htmlPage, leadingURL);
                retrieveHTMLBody(htmlContent, htmlPage);//body
                htmlPage.ServerControl = processWidgets(htmlPage, store);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at parseing HTML file", "", "", "", ex);
            }

            return htmlPage;
        }

        //加载body和body内容
        private static void retrieveHTMLBody(String htmlContent, HTMLPage page)
        {
            page.body.orig = retrieveHTMLSegment(htmlContent, @"//*/body", true);
            page.body.value = retrieveHTMLSegment(htmlContent, @"//*/body");
        }

        //把img background a标签相对路径 改为绝对的。
        private static void alternateLinkInHTMLBody(HTMLPage page, String leadingURL, String OriginalURL = "")
        {
            //获取所有 img标签
            List<string> imageTags = retrieveHTMLSegments(page.body.value, @"//*/img", true);
            foreach (string imgTag in imageTags)
            {
                String newImgTag = alternateRelativeLink(imgTag, "src", leadingURL);
                page.body.value = page.body.value.Replace(imgTag, newImgTag);
            }

            //样式 暂时不需要
            //String backgroundURLPattern = @"<.+?background:url\((?<url>.*?)\).*?>";
            //List<String> backgroundTags = regexHTMLSegments(page.body.value, @"<.+?background:url\((.+?)\).*?>");
            //foreach (String backgroundTag in backgroundTags)
            //{
            //    String newbackgroundTag = alternateRelativeLink(backgroundTag, backgroundURLPattern, leadingURL, "", true);
            //    page.body.value = page.body.value.Replace(backgroundTag, newbackgroundTag);
            //}

            //identify background attributes
            String backgroundURLPattern2 = @"<.+?background=""(?<url>.*?)"".*?>";
            List<String> backgroundTags2 = regexHTMLSegments(page.body.value, "<.+?background=\"(.+?)\".*?>");
            foreach (String backgroundTag in backgroundTags2)
            {
                String newbackgroundTag = alternateRelativeLink(backgroundTag, backgroundURLPattern2, leadingURL, "", true);
                page.body.value = page.body.value.Replace(backgroundTag, newbackgroundTag);
            }

            //获取所有 A标签
            List<string> anchorTags = retrieveHTMLSegments(page.body.value, @"//*/a", true);
            foreach (string anchorTag in anchorTags)
            {
                String newAnchoragTag = alternateRelativeLink(anchorTag, "href", leadingURL, OriginalURL);
                page.body.value = page.body.value.Replace(anchorTag, newAnchoragTag);
            }
        }

        private static String alternateRelativeLink(String source, String regExpress, String leadingURL, String OriginalURL = "", bool isStyel = false)
        {
            //only perform URL replacement when leadingURL is not null or empty
            if (!String.IsNullOrEmpty(leadingURL))
            {
                String url = string.Empty;
                //获取 对应的属性. 
                if (isStyel)
                {
                    //如果是样式里面的属性.  就用regex
                    Regex regexObj = new Regex(regExpress, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match matchResults = regexObj.Match(source);
                    if (matchResults.Success)
                        url = matchResults.Groups["url"].Value;
                }
                else
                    url = retrieveHTMLAttribute(source, regExpress);
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
                        if (url.Trim().StartsWith("~/"))
                            source = source.Replace(url, url.Remove(0, 1));
                        else if (url.Trim().StartsWith("$"))
                        {
                            //do nothing for widget target
                        }
                        else if (url.Trim().StartsWith("#") && !url.Trim().StartsWith("#tab"))
                            source = source.Replace(url, String.Format("{0}{1}", OriginalURL, url));
                        else if (uri.IsAbsoluteUri == false && url.Trim().StartsWith("/") == false)  //relative path
                            source = source.Replace(url, String.Format("{0}/{1}", leadingURL, url));
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return source;
        }

        private static void retrieveHTMLHeader(String htmlContent, HTMLPage page, String leadingURL)
        {
            page.header.content = retrieveHTMLSegment(htmlContent, @"//*/head", true);
            page.header.title = retrieveHTMLSegment(page.header.content, @"//head/title");

            retrieveHTMLHeaderMeta(page.header.content, page);
            retrieveHTMLHeaderLinks(page.header.content, page, leadingURL);
            retrieveHTMLHeaderScripts(page.header.content, page, leadingURL);
        }

        public static List<POCOS.Product> WigetCrossSellProduct = new List<POCOS.Product>();
        //生成 head meta
        private static void retrieveHTMLHeaderMeta(String htmlHeader, HTMLPage page)
        {
            List<string> metaXpathList = retrieveHTMLSegmentXpath(htmlHeader, @"//head/meta");
            foreach (string metaXpath in metaXpathList)
            {
                String metaName = "", metaValue = "";
                Dictionary<string, string> metaAttribute = retrieveHTMLAttributes(htmlHeader, metaXpath);
                foreach (String metaKey in metaAttribute.Keys)
                {
                    if ("http-equiv" == metaKey.ToLower() || "name" == metaKey.ToLower())
                        metaName = metaAttribute[metaKey];

                    if ("content" == metaKey.ToLower())
                        metaValue = metaAttribute[metaKey];
                }

                if (!String.IsNullOrWhiteSpace(metaName))
                {
                    if ((metaName.ToUpper() == "KEYWORDS" || metaName.ToUpper() == "DESCRIPTION") && page.header.meta.ContainsKey(metaName.ToUpper()))
                    {
                        //keyword  DESCRIPTION加一次
                    }
                    else
                        page.header.meta.Add(metaName.ToUpper(), metaValue);
                }
                    
                if (metaName == "AnchorProducts")
                {
                    string[] meta = metaValue.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string productid in meta)
                    {
                        POCOS.Product product = eStoreContext.Current.Store.getProduct(productid);
                        WigetCrossSellProduct.Add(product);
                    }
                }
            }
        }

        //生成 head link
        private static void retrieveHTMLHeaderLinks(String htmlHeader, HTMLPage page, String leadingURL)
        {
            StringBuilder buffer = new StringBuilder();
            List<string> linkTags = retrieveHTMLSegments(htmlHeader, @"//head/link", true);
            foreach (string linkTag in linkTags)
            {
                String newLinkTag = alternateRelativeLink(linkTag, "href", leadingURL);
                buffer.Append(newLinkTag).Append("\r\n");
            }
            page.header.links = buffer.ToString();
        }

        //生成 head script
        private static void retrieveHTMLHeaderScripts(String htmlHeader, HTMLPage page, String leadingURL)
        {
            StringBuilder buffer = new StringBuilder();
            List<string> scriptTags = retrieveHTMLSegments(htmlHeader, @"//head/script", true);
            foreach (string scriptTag in scriptTags)
            {
                String newScriptTag = alternateRelativeLink(scriptTag, "src", leadingURL);
                buffer.Append(newScriptTag).Append("\r\n");
            }
            page.header.scripts = buffer.ToString();
        }

        //estorewidget
        private static List<ServerControl> processWidgets(HTMLPage page, BusinessModules.Store store)
        {
            List<ServerControl> serverControls = new List<ServerControl>();
            bool hasCategoryWidget = false;
            List<string> widgetXpathList = retrieveHTMLSegmentXpath(page.body.value, @"//*/estorewidget");

            foreach (string itemXpath in widgetXpathList)
            {
                string widgetContent = retrieveHTMLSegment(page.body.value, itemXpath, true);
                String widgetType = "", widgetId = "";
                Dictionary<string, string> widgetsAttribute = retrieveHTMLAttributes(page.body.value, itemXpath);
                //type="ProductWidget" value="2943"
                foreach (String widgetKey in widgetsAttribute.Keys)
                {
                    if ("type" == widgetKey.ToLower())
                        widgetType = widgetsAttribute[widgetKey];
                    if ("value" == widgetKey.ToLower())
                        widgetId = widgetsAttribute[widgetKey];
                }

                if (!String.IsNullOrWhiteSpace(widgetType) && !String.IsNullOrWhiteSpace(widgetId))
                {
                    if (Enum.GetNames(typeof(ServerControlType)).Contains(widgetType))
                    {
                        ServerControl con = new ServerControl();
                        Guid id = new Guid();
                        id = Guid.NewGuid();
                        con.ID = id;
                        con.ServerControlType = (ServerControlType)Enum.Parse(typeof(ServerControlType), widgetType);
                        con.para = widgetId;
                        serverControls.Add(con);
                        page.body.value = page.body.value.Replace(widgetContent, ServerControlContent(id));
                    }
                    else
                    {
                        String alterWidget = substitudeWidget(widgetContent, widgetType, widgetId, store);
                        page.body.value = page.body.value.Replace(widgetContent, alterWidget);
                        if (widgetType == "CategoryWidget")
                            hasCategoryWidget = true;
                    }
                }
            }

            if (hasCategoryWidget)
                page.body.value += getCategoryMinPriceAJAXScripts();

            return serverControls;
        }

        //根据随机数 生成div
        private static String ServerControlContent(Guid ID)
        {
            return string.Format("<div id=\"{0}\"></div>", ID.ToString());
        }

        //ProductWidget和CategoryWidget 生成对应的资料
        private static String substitudeWidget(String widgetContent, String widgetType, String widgetId, BusinessModules.Store store)
        {
            String resultValue = "";

            //create real instance according to Widget setup
            switch (widgetType)
            {
                case "CategoryWidget":
                    resultValue = substitudeCategoryWidget(widgetContent, widgetId, store);
                    break;
                case "ProductWidget":
                    resultValue = substitudeProductWidget(widgetContent, widgetId, store);
                    break;
                default:
                    resultValue = widgetContent;
                    break;  //not support
            }

            return resultValue;
        }

        //生成 category资料
        private static String substitudeCategoryWidget(String widgetContent, String widgetId, BusinessModules.Store store)
        {
            String resultValue = widgetContent;
            ProductCategory category = store.getProductCategory(widgetId);
            if (category != null)
            {
                Currency currency = store.profile.defaultCurrency;
                if (resultValue.Contains("$Widget.MinPrice"))
                    resultValue = resultValue.Replace("$Widget.MinPrice", String.Format("<span id=\"{0}\" class=\"widgetminprice\"><img alt=\"loading..\" src=\"{1}images/priceprocessing.gif\" /></span>", category.CategoryPath, esUtilities.CommonHelper.GetStoreLocation()));
                if (resultValue.Contains("$Widget.MaxPrice"))
                    resultValue = resultValue.Replace("$Widget.MaxPrice", String.Format("{0}{1}", currency.CurrencySign, string.Format("{0:n0}", category.getHighestPrice())));
                if (resultValue.Contains("$Widget.Name"))
                    resultValue = resultValue.Replace("$Widget.Name", String.Format("{0}", category.LocalCategoryName));
                if (resultValue.Contains("$Widget.Description"))
                    resultValue = resultValue.Replace("$Widget.Description", String.Format("{0}", category.Description));
                if (resultValue.Contains("$Widget.ImageURL"))
                    resultValue = resultValue.Replace("$Widget.ImageURL", String.Format("/resource{0}", category.ImageURL));
                if (resultValue.Contains("$Widget.URL"))
                    resultValue = resultValue.Replace("$Widget.URL", String.Format("{0}", ResolveUrl(UrlRewriting.MappingUrl.getMappingUrl(category))));
            }
            else
            {
                resultValue = resultValue.Replace("$Widget.MinPrice", "Call for price");
                resultValue = resultValue.Replace("$Widget.MaxPrice", "Call for price");
                resultValue = resultValue.Replace("$Widget.Name", "");
                resultValue = resultValue.Replace("$Widget.Description", "");
                resultValue = resultValue.Replace("$Widget.ImageURL", "");
                resultValue = resultValue.Replace("$Widget.URL", "");
            }

            return resultValue;
        }

        //生成 product资料
        private static String substitudeProductWidget(String widgetContent, String widgetId, BusinessModules.Store store)
        {
            String resultValue = widgetContent;
            POCOS.Product product = store.getProduct(widgetId);
            if (product != null)
            {
                decimal price = product.getListingPrice().value;
                Currency currency = store.profile.defaultCurrency;
                if (resultValue.Contains("$Widget.Price"))
                    resultValue = resultValue.Replace("$Widget.Price",
                        price == 0 ? eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price) :
                        Presentation.Product.ProductPrice.FormartPriceWithDecimal(price));
                if (resultValue.Contains("$Widget.Name"))
                    resultValue = resultValue.Replace("$Widget.Name", String.Format("{0}", product.name));
                if (resultValue.Contains("$Widget.Features"))
                    resultValue = resultValue.Replace("$Widget.Features", String.Format("{0}", product.productFeatures));
                if (resultValue.Contains("$Widget.Description"))
                    resultValue = resultValue.Replace("$Widget.Description", String.Format("{0}", product.productDescX));
                if (resultValue.Contains("$Widget.ImageURL"))
                    resultValue = resultValue.Replace("$Widget.ImageURL", String.Format("{0}", ResolveUrl(product.thumbnailImageX)));
                if (resultValue.Contains("$Widget.URL"))
                    resultValue = resultValue.Replace("$Widget.URL", String.Format("{0}", ResolveUrl(UrlRewriting.MappingUrl.getMappingUrl(product))));
                if (resultValue.Contains("$Widget.StatusIcon"))
                    resultValue = resultValue.Replace("$Widget.StatusIcon", string.Format("<img src='/images/{0}.gif' />", product.status.ToString()));
                if (resultValue.Contains("$Widget.Status"))
                    resultValue = resultValue.Replace("$Widget.Status", product.status.ToString());
            }
            else
            {
                resultValue = resultValue.Replace("$Widget.Price", "Call for price");
                resultValue = resultValue.Replace("$Widget.Features", "No feature available");
                resultValue = resultValue.Replace("$Widget.Name", "");
                resultValue = resultValue.Replace("$Widget.Description", "No description available");
                resultValue = resultValue.Replace("$Widget.ImageURL", "");
                resultValue = resultValue.Replace("$Widget.URL", "");
                resultValue = resultValue.Replace("$Widget.Status", "");
                resultValue = resultValue.Replace("$Widget.StatusIcon", "");
            }

            return resultValue;
        }

        //生成获取 价钱的js
        private static string getCategoryMinPriceAJAXScripts()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\" language=\"javascript\">");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("$.each($(\".widgetminprice\"), function () {");
            sb.AppendLine("var pricepanel = $(this);");
            sb.AppendFormat("$.getJSON(\"{0}proc/do.aspx\",", esUtilities.CommonHelper.GetStoreLocation());
            sb.AppendLine("{ func: \"9\"");
            sb.AppendLine(", id: $(pricepanel).attr(\"id\")");
            sb.AppendLine("},");
            sb.AppendLine("function (data) {");
            sb.AppendLine("$(pricepanel).html(data.LowestPrice);");
            sb.AppendLine("});");
            sb.AppendLine("});");
            sb.AppendLine("});");

            sb.AppendLine("</script>");
            return sb.ToString();
        }

        public static HTMLPage retrieveServerControlContent(Guid ID, string content, HTMLPage page)
        {
            page.body.value = page.body.value.Replace(ServerControlContent(ID), content);
            return page;
        }

        //使用正则 获取内容
        private static List<String> regexHTMLSegments(String htmlContent, String regEx)
        {
            List<String> segments = new List<String>();
            try
            {
                Regex exp = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection matches = exp.Matches(htmlContent);
                foreach (Match match in matches)
                    segments.Add(match.Value);
            }
            catch (ArgumentException)
            {
            }

            return segments;
        }

        public static string ResolveUrl(string relativeUrl)
        {
            if (relativeUrl == null)
                return "/images/photounavailable.gif";
            //throw new ArgumentNullException("relativeUrl");

            if (relativeUrl.Length == 0 || relativeUrl[0] == '/' || relativeUrl[0] == '\\')
                return relativeUrl;

            int idxOfScheme = relativeUrl.IndexOf(@"://", StringComparison.Ordinal);
            if (idxOfScheme != -1)
            {
                int idxOfQM = relativeUrl.IndexOf('?');
                if (idxOfQM == -1 || idxOfQM > idxOfScheme) return relativeUrl;
            }

            StringBuilder sbUrl = new StringBuilder();
            sbUrl.Append(System.Web.HttpRuntime.AppDomainAppVirtualPath);
            if (sbUrl.Length == 0 || sbUrl[sbUrl.Length - 1] != '/') sbUrl.Append('/');

            // found question mark already? query string, do not touch!
            bool foundQM = false;
            bool foundSlash; // the latest char was a slash?
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
    }
}
