using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Text.RegularExpressions;
using eStore.Utilities;
using System.IO;

namespace eStore.Presentation.Widget
{
    public class WidgetHandler
    {
        /// <summary>
        /// This method is a static method, it takes a full path file name. The input file has to be HTML file.
        /// In return, it returns HTMLPage instance. 
        /// It will try to identify widget enable regions and replace widget value holders with real values.
        /// <eStoreWidget name="CategoryWidget" value "xCategoryId">
        ///     $Widget.MinPrice
        ///     $Widget.MaxPrice
        ///     $Widget.Name
        ///     $Widget.Description
        ///     $Widget.ImageURL
        ///     $Widget.URL
        /// </eStoreWidget>
        /// 
        /// <eStoreWidget name="ProductWidget" value "xProductId">
        ///     $Widget.Price
        ///     $Widget.Name
        ///     $Widget.Description
        ///     $Widget.ImageURL
        ///     $Widget.Features
        ///     $Widget.URL
        /// </eStoreWidget>
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static HTMLPage processHTMLFile(String filename, BusinessModules.Store store, String leadingURL, string OriginalURL="")
        {
            HTMLPage page = new HTMLPage();
            try
            {
                string resolvedfilepath = filename.Insert(filename.LastIndexOf('\\'), "\\resolvedfile");
                String htmlContent = string.Empty;
                if (File.Exists(resolvedfilepath) && (new FileInfo(resolvedfilepath)).LastWriteTimeUtc>(new FileInfo(filename)).LastWriteTimeUtc)
                {
                    htmlContent = loadFile(resolvedfilepath);
                }
                else
                {
                    string tempcontent = loadFile(filename);
                    retrieveHTMLBody(tempcontent, page);
                    alternateLinkInHTMLBody(page, leadingURL, OriginalURL);
                   
                    try
                    {
                        Regex regexObj = new Regex(@"<\s*body[^>]*>([\w\W]*?)<\s*/\s*body>");
                        String replacement = string.Format(@"<body>{0}</body>", page.body.value );
                        MatchCollection matches = regexObj.Matches(tempcontent);
                        foreach (Match match in matches)
                            tempcontent = tempcontent.Replace(match.Value, replacement);

                        //htmlContent = regexObj.Replace(tempcontent, replacement, 1);
                        htmlContent = tempcontent;
                        saveResolvedFile(resolvedfilepath, htmlContent);
                    }
                    catch (ArgumentException ex)
                    {
                        throw ex;
                    }

                }
                retrieveHTMLHeader(htmlContent, page, leadingURL);
                retrieveHTMLBody(htmlContent, page);
               page.ServerControl= processWidgets(page, store);
               
                return page;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at parseing HTML file", "", "", "", ex);
            }

            return page;
        }

        private static string loadFile(string path)
        {
            String fileContent = "";
            try
            {
                StreamReader fileReader = new StreamReader(path, System.Text.Encoding.UTF8);
                fileContent = fileReader.ReadToEnd();
                fileReader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fileContent;
        }

        private static List<string> resolveJsonToList(string json)
        {
            List<string> list = new List<string>();
            try
            {
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at resolving exclude Json file", "", "", "", ex);
            }
            return list;
        }
        private static void saveResolvedFile(string filePath, string filecontent)
        {

            StreamWriter fileWriter = null;
            string dirpath=filePath.Substring(0,filePath.LastIndexOf('\\'));
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
        public static HTMLPage retrieveServerControlContent(Guid ID, string content, HTMLPage page)
        {
            page.body.value = page.body.value.Replace(ServerControlContent(ID), content);
            return page;
        }
        private static void retrieveHTMLHeader(String htmlContent, HTMLPage page, String leadingURL)
        {
            page.header.content = retrieveHTMLSegment(htmlContent, @"<\s*head[^>]*>([\w\W]*?)<\s*/\s*head>");
            page.header.title = retrieveHTMLSegmentInner(page.header.content, @"<\s*title[^>]*>([\w\W]*?)<\s*/\s*title>");
            retrieveHTMLHeaderMeta(page.header.content, page);
            retrieveHTMLHeaderLinks(page.header.content, page, leadingURL);
            retrieveHTMLHeaderScripts(page.header.content, page, leadingURL);
        }

        private static void retrieveHTMLBody(String htmlContent, HTMLPage page)
        {
            page.body.orig = retrieveHTMLSegment(htmlContent, @"<\s*body[^>]*>([\w\W]*?)<\s*/\s*body>");
            page.body.value = retrieveHTMLSegmentInner(htmlContent, @"<\s*body[^>]*>([\w\W]*?)<\s*/\s*body>");
        }

        private static void retrieveHTMLHeaderMeta(String htmlHeader, HTMLPage page)
        {
            Regex metaregex =
                new Regex(@"<meta\s*(?:(?:\b(\w|-)+\b\s*(?:=\s*(?:""[^""]*""|'" +
                          @"[^']*'|[^""'<> ]+)\s*)?)*)/?\s*>",
                          RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

            foreach (Match metamatch in metaregex.Matches(htmlHeader))
            {
                String metaName = "";
                String metaValue = "";
                Regex submetaregex =
                    new Regex(@"(?<name>\b(\w|-)+\b)\s*=\s*(""(?<value>" +
                              @"[^""]*)""|'(?<value>[^']*)'|(?<value>[^""'<> ]+)\s*)+",
                              RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

                foreach (Match submetamatch in submetaregex.Matches(metamatch.Value.ToString()))
                {
                    if ("http-equiv" == submetamatch.Groups["name"].ToString().ToLower())
                        metaName = submetamatch.Groups["value"].ToString();

                    if ("name" == submetamatch.Groups["name"].ToString().ToLower())
                    {
                        // if already set, HTTP-EQUIV overrides NAME
                        metaName = submetamatch.Groups["value"].ToString();
                    }
                    if ("content" == submetamatch.Groups["name"].ToString().ToLower())
                    {
                        metaValue = submetamatch.Groups["value"].ToString();
                    }
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

        public static List<POCOS.Product> WigetCrossSellProduct = new List<POCOS.Product>();

        private static void retrieveHTMLHeaderLinks(String htmlHeader, HTMLPage page, String leadingURL)
        {
            StringBuilder buffer = new StringBuilder();
            List<String> links = retrieveHTMLSegments(htmlHeader, @"<link\s*(?:(?:\b(\w|-)+\b\s*(?:=\s*(?:""[^""]*""|'" +
                          @"[^']*'|[^""'<> ]+)\s*)?)*)/?\s*>");

            string filepath = string.Format("{0}\\style.json", System.Configuration.ConfigurationManager.AppSettings.Get("Widget_Path"));
            List<string> exclude = new List<string>();
            if (File.Exists(filepath))
            {
                string styles = loadFile(filepath);
                if (!string.IsNullOrEmpty(styles))
                    exclude = resolveJsonToList(styles);
            }
            //alternate package relative link to eStore relative link
            //String linkPattern = @"<link.+?href=""(?<url>.*?)"".*?/>";
            String linkPattern = @"<link.+?href=""(?<url>.*?)"".*?>";
            foreach (String link in links)
            {
                if (exclude.Contains(link))
                    continue;
                buffer.Append( alternateRelativeLink(link, linkPattern, leadingURL)).Append("\r\n");
            }

            page.header.links = buffer.ToString();
        }

        /// <summary>
        /// This method will try to alternate relative pat to eStore Widget compatible path.  If the path is a absolute path, no alternation
        /// will happen.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="regExpress"></param>
        /// <returns></returns>
        private static String alternateRelativeLink(String source, String regExpress, String leadingURL, String OriginalURL = "")
        {
            //only perform URL replacement when leadingURL is not null or empty
            if (! String.IsNullOrEmpty(leadingURL))
            {
                Regex regexObj = new Regex(regExpress, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match matchResults = regexObj.Match(source);
                while (matchResults.Success)
                {
                    String url = matchResults.Groups["url"].Value;
                    if (!String.IsNullOrEmpty(url))
                    {
                        try
                        {
                            Uri uri = new Uri(matchResults.Groups["url"].Value, UriKind.RelativeOrAbsolute);
                            if (url.Trim().StartsWith("~/"))
                            { source = source.Replace(url,  url.Remove(0,1)); }
                            else if (url.Trim().StartsWith("$"))
                            { 
                            //do nothing for widget target
                            }
                            else if (url.Trim().StartsWith("#") && !url.Trim().StartsWith("#tab"))
                            {
                                source = source.Replace(url, String.Format("{0}{1}", OriginalURL, url));
                            }
                            else if (uri.IsAbsoluteUri == false && url.Trim().StartsWith("/") == false)  //relative path
                                source = source.Replace(url, String.Format("{0}/{1}", leadingURL, url));
                          
                        }
                        catch (Exception)
                        {
                        }
                    }

                    matchResults = matchResults.NextMatch();
                }
            }

            return source;
        }

        private static void retrieveHTMLHeaderScripts(String htmlHeader, HTMLPage page, String leadingURL)
        {
            StringBuilder buffer = new StringBuilder();
            List<String> scripts = retrieveHTMLSegments(htmlHeader, @"<\s*script[^>]*>([\w\W]*?)<\s*/\s*script>");

            List<string> exclude = new List<string>();
            string filepath = string.Format("{0}\\script.json", System.Configuration.ConfigurationManager.AppSettings.Get("Widget_Path"));
            if (File.Exists(filepath))
            {
                string script = loadFile(filepath);
                if (!string.IsNullOrEmpty(script))
                    exclude = resolveJsonToList(script);
            }
            //alternate package relative link to eStore relative link
            //String scriptPattern = @"<script.+?src=""(?<url>.*?)"".*?/>";
            String scriptPattern = @"<script.+?src=""(?<url>.*?)"".*?>";
            foreach (String script in scripts)
            {
                if (exclude.Contains(script))
                    continue;
                buffer.Append(alternateRelativeLink(script, scriptPattern, leadingURL)).Append("\r\n");
            }

            page.header.scripts = buffer.ToString();
        }

        private static String retrieveHTMLSegment(String htmlContent, String regEx)
        {
            try
            {
                Regex exp = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match match = exp.Match(htmlContent);
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

        private static List<String> retrieveHTMLSegments(String htmlContent, String regEx)
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

        private static String retrieveHTMLSegmentInner(String htmlContent, String regEx)
        {
            try
            {
                Regex exp = new Regex(regEx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Match match = exp.Match(htmlContent);
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

        /// <summary>
        /// In the method, it will try to identify widget tag and replace them with real value
        /// <eStoreWidget type="CategoryWidget" value="xCategoryId">
        ///     $Widget.MinPrice
        ///     $Widget.MaxPrice
        ///     $Widget.Description
        ///     $Widget.ImageURL
        ///     $Widget.URL
        /// </eStoreWidget>
        /// 
        /// <eStoreWidget type="ProductWidget" value="xProductId">
        ///     $Widget.Price
        ///     $Widget.Description
        ///     $Widget.ImageURL
        ///     $Widget.Features
        ///     $Widget.URL
        /// </eStoreWidget>
        /// </summary>
        /// <param name="page"></param>
        private static List<ServerControl> processWidgets(HTMLPage page, BusinessModules.Store store)
        {
            //identify widget segments
            List<String> widgets = retrieveHTMLSegments(page.body.value, @"<\s*eStoreWidget[^>]*>([\w\W]*?)<\s*/\s*eStoreWidget>");
            List<ServerControl> serverControls = new List<ServerControl>();
            bool hascategorywidget = false;
            foreach (String widget in widgets)
            {
                String widgetType = "";
                String widgetId = "";

                //find widget Identity
                Regex widgetTypeExp =
                    new Regex(@"(?<name>\b(\w|-)+\b)\s*=\s*(""(?<value>" +
                                @"[^""]*)""|'(?<value>[^']*)'|(?<value>[^""'<> ]+)\s*)+",
                                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);

                foreach (Match attribute in widgetTypeExp.Matches(widget))
                {
                    if ("type" == attribute.Groups["name"].ToString().ToLower())
                        widgetType = attribute.Groups["value"].ToString();
                    if ("value" == attribute.Groups["name"].ToString().ToLower())
                        widgetId = attribute.Groups["value"].ToString();
                }

                if (!String.IsNullOrWhiteSpace(widgetType) && !String.IsNullOrWhiteSpace(widgetId))
                {
                    if (Enum.GetNames(typeof(ServerControlType)).Contains(widgetType))
                    {
                        ServerControl con = new ServerControl();
                        Guid id = new Guid();
                        id = Guid.NewGuid();
                        con.ID =id;
                        con.ServerControlType = (ServerControlType)Enum.Parse(typeof(ServerControlType), widgetType);
                        con.para = widgetId;
                        serverControls.Add(con);
                        page.body.value = page.body.value.Replace(widget, ServerControlContent(id));
                    }
                    else
                    {
                        String alterWidget = substitudeWidget(widget, widgetType, widgetId, store);
                        page.body.value = page.body.value.Replace(widget, alterWidget);
                        if (widgetType == "CategoryWidget")
                        {
                            hascategorywidget = true;
                        
                        }
                    }
                }
            }
            if (hascategorywidget)
            {
                page.body.value += getCategoryMinPriceAJAXScripts();
            }
            return serverControls;
        }

        private static string getCategoryMinPriceAJAXScripts()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\" language=\"javascript\">");
            sb.AppendLine("$(document).ready(function () {");
            sb.AppendLine("$.each($(\".widgetminprice\"), function () {");
            sb.AppendLine("var pricepanel = $(this);");
            sb.AppendFormat("$.getJSON(\"{0}proc/do.aspx\",",esUtilities.CommonHelper.GetStoreLocation());
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
        private static String ServerControlContent(Guid ID)
        {
            return string.Format("<div id=\"{0}\"></div>", ID.ToString());
        }
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

        private static String substitudeProductWidget(String widgetContent, String widgetId, BusinessModules.Store store)
        {
            String resultValue = widgetContent;
            POCOS.Product product = store.getProduct(widgetId);
            if (product != null)
            {
                decimal price=product.getListingPrice().value;
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

        private static void alternateLinkInHTMLBody(HTMLPage page, String leadingURL, String OriginalURL="")
        {
            //identify image segments
            //String imageURLPattern = @"<img.+?src=""(?<url>.*?)"".*?/>";
            String imageURLPattern = @"<img.+?src=""(?<url>.*?)"".*?>";
            //List<String> imageTags = retrieveHTMLSegments(page.body.value, "<img.+?src=\"(.+?)\".*?/>");
            List<String> imageTags = retrieveHTMLSegments(page.body.value, "<img.+?src=\"(.+?)\".*?>");

            foreach (String imageTag in imageTags)
            {
                String newImageTag = alternateRelativeLink(imageTag, imageURLPattern, leadingURL);
                page.body.value = page.body.value.Replace(imageTag, newImageTag);
            }

            //identify background attributes
            String backgroundURLPattern = @"<.+?background=""(?<url>.*?)"".*?>";
            List<String> backgroundTags = retrieveHTMLSegments(page.body.value, "<.+?background=\"(.+?)\".*?>");

            foreach (String backgroundTag in backgroundTags)
            {
                String newbackgroundTag = alternateRelativeLink(backgroundTag, backgroundURLPattern, leadingURL);
                page.body.value = page.body.value.Replace(backgroundTag, newbackgroundTag);
            }

            //identify anchor href attributes
            String anchorURLPattern = @"<a.+?href=""(?<url>.*?)"".*?/>";
            List<String> anchorTags = retrieveHTMLSegments(page.body.value, "<a.+?href=\"(.+?)\".*?/>");

            foreach (String anchorTag in anchorTags)
            {
                String newAnchorTag = alternateRelativeLink(anchorTag, anchorURLPattern, leadingURL, OriginalURL);
                page.body.value = page.body.value.Replace(anchorTag, newAnchorTag);
            }

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
