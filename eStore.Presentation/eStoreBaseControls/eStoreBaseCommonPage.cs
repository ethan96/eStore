using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using esUtilities;
using System.IO;
using System.Web.UI;
using System.Collections.Specialized;

namespace eStore.Presentation.eStoreBaseControls
{
    public class eStoreBaseCommonPage : System.Web.UI.Page
    {
        public System.Collections.Specialized.NameValueCollection keywords { get; set; }

        public Boolean isExistsPageMeta { get; set; }
        private bool? _UseSSL;
       
        public virtual bool UseSSL
        {
            get
            {
                if (_UseSSL.HasValue == false)
                {

                    _UseSSL = Presentation.eStoreContext.Current.getBooleanSetting("IsSecureEntireSite", false);

                }
                return _UseSSL.Value;

            }
            set { _UseSSL = value; }
        }


        private bool _isMobileFriendly = false;
        public virtual bool isMobileFriendly
        {
            get { return _isMobileFriendly; }
            set { _isMobileFriendly = value; }
        }
        public bool isMobile
        {
            get
            {
                string HfMobile = Request.Form["HfMobile"];
                bool _isMobile = false;
                bool.TryParse(HfMobile, out _isMobile);
                return _isMobile;
            }
        }
        private bool _BlockSearchIndexing = false;
        public bool BlockSearchIndexing
        {
            get
            {
                return _BlockSearchIndexing;
            }
            set
            {
                _BlockSearchIndexing = value;
            }
        }
        private POCOS.UserActivityLog _userActivitlog;
        public virtual POCOS.UserActivityLog UserActivitLog
        {
            get
            {
                if (_userActivitlog == null)
                {
                    _userActivitlog = new POCOS.UserActivityLog();
                    try
                    {
                        _userActivitlog.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                        _userActivitlog.SessionID = Session.SessionID;
                        _userActivitlog.RemoteAddr = eStore.Presentation.eStoreContext.Current.getUserIP();
                        if (Presentation.eStoreContext.Current.User != null)
                            _userActivitlog.UserId = Presentation.eStoreContext.Current.User.UserID;
                        _userActivitlog.CreatedDate = DateTime.Now;
                        _userActivitlog.URL = Request.Url.ToString();
                        _userActivitlog.ReferredURL = Request.ServerVariables["HTTP_REFERER"] ?? "";
                        _userActivitlog.BrowserVersion = Request.Browser.Version;
                        _userActivitlog.BrowserType = Request.Browser.Browser;
                    }
                    catch (Exception)
                    {
                    }
                }
                return _userActivitlog;
            }
        }
        protected override void OnPreInit(EventArgs e)
        {
            if (UseSSL != Request.IsSecureConnection)
            {
                try
                {
                    if (!Presentation.eStoreContext.Current.isTestMode())
                        esUtilities.CommonHelper.ReloadCurrentPage(UseSSL);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "init request failed.")
                    {
                        string _storeid = Request.ServerVariables["SERVER_NAME"];
                        System.Net.IPAddress serverip = new System.Net.IPAddress(0);
                        //estore doesn't support IP access
                        if (System.Net.IPAddress.TryParse(_storeid, out serverip))
                        {
                            var ipstore = BusinessModules.StoreSolution.getInstance().locateStore(null, _storeid);
                            if (ipstore != null)
                            {
                                Response.Redirect($"http://{ipstore.profile.StoreURL}");
                            }
                            else
                            {
                                Utilities.eStoreLoger.Info(string.Format("fake request for {0}", _storeid));
                                Response.Redirect("/alive.html");
                            }

                        }
                        else
                            throw ex;
                    }
                }
            }
            base.OnPreInit(e);
        }

        public virtual Boolean setPageMeta(string title, string Description, string keyword)
        {
            if (string.IsNullOrEmpty(title))
            {
                if (Presentation.eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.MiniSite.SiteTitle))
                {
                    this.Page.Title = Presentation.eStoreContext.Current.MiniSite.SiteTitle;
                }
                else
                    this.Page.Title = Presentation.eStoreContext.Current.Store.profile.Title;
            }
            else
                this.Page.Title = title;
            HtmlMeta KeyWordsMeta = new HtmlMeta();
            KeyWordsMeta.Name = "KeyWords";
            HtmlMeta DescriptionsMeta = new HtmlMeta();
            DescriptionsMeta.Name = "Description";

            if (string.IsNullOrEmpty(Description))
                if (Presentation.eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.MiniSite.SiteMetaDesc))
                {
                    DescriptionsMeta.Content = Presentation.eStoreContext.Current.MiniSite.SiteMetaDesc;
                }
                else
                    DescriptionsMeta.Content = Presentation.eStoreContext.Current.Store.profile.MetaDesc;
            else
                DescriptionsMeta.Content = Description;

            if (string.IsNullOrEmpty(keyword))
                if (Presentation.eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.MiniSite.SiteMetaKeywords))
                {
                    KeyWordsMeta.Content = Presentation.eStoreContext.Current.MiniSite.SiteMetaKeywords;
                }
                else
                    KeyWordsMeta.Content = Presentation.eStoreContext.Current.Store.profile.MetaKeywords;
            else
                KeyWordsMeta.Content = keyword;

            Page.Header.Controls.Add(KeyWordsMeta);
            Page.Header.Controls.Add(DescriptionsMeta);

            return true;
        }
        /// <summary>
        /// https://support.google.com/webmasters/answer/139066?visit_id=1-636096444752470649-3391615490&hl=en&rd=1
        /// <link rel="canonical" href="https://blog.example.com/dresses/green-dresses-are-awesome" />
        /// </summary>
        /// <param name="url"></param>
        protected void setCanonicalPage(string url)
        {
            HtmlLink canonicalpageLink = new HtmlLink();
            canonicalpageLink.Attributes.Add("rel", "canonical");
            if (url.StartsWith("~/"))
            {
                url = esUtilities.CommonHelper.GetStoreLocation() + url.Substring(2);
            }
            canonicalpageLink.Href = esUtilities.CommonHelper.ResolveUrl(url);
            Page.Header.Controls.Add(canonicalpageLink);
        }
        protected void setGTM()
        {
            StringBuilder sbGoogleAnalytics = new StringBuilder();

            // Add GTM
            sbGoogleAnalytics.Append("(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':");
            sbGoogleAnalytics.Append("new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],");
            sbGoogleAnalytics.Append("j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=");
            sbGoogleAnalytics.Append("'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);");
            sbGoogleAnalytics.Append("})(window,document,'script','dataLayer',");
            sbGoogleAnalytics.AppendFormat("'{0}'", eStoreContext.Current.getStringSetting("GTMId", "GTM-N4LS26"));
            sbGoogleAnalytics.Append(");");

            //Generate JS Code in HTML
            HtmlGenericControl con = new HtmlGenericControl();
            con.TagName = "script";
            con.Attributes.Add("type", "text/javascript");
            con.InnerHtml = sbGoogleAnalytics.ToString();
            Page.Header.Controls.Add(con);
        }

        protected void pushGlobalDatalayer()
        {
            // 將storeid&countryname 裝入datalayer 傳至GTM
            StringBuilder globalDatalayer = new StringBuilder();
            globalDatalayer.AppendLine("window.dataLayer = window.dataLayer || []");
            globalDatalayer.AppendLine("dataLayer.push ({");
            globalDatalayer.AppendFormat("'storeid': '{0}',", Presentation.eStoreContext.Current.Store.storeID);
            globalDatalayer.AppendFormat("'countryName': '{0}',", Presentation.eStoreContext.Current.CurrentCountry.CountryName);

            if (eStoreContext.Current.User != null)
            {
                globalDatalayer.AppendLine("'userProfile': {");
                globalDatalayer.AppendFormat("'userId': '{0}',", Presentation.eStoreContext.Current.User.UserID);
                globalDatalayer.AppendFormat("'userName': '{0}',", Presentation.eStoreContext.Current.User.FirstName + " " + Presentation.eStoreContext.Current.User.LastName);
                globalDatalayer.AppendFormat("'userCompany': '{0}',", Presentation.eStoreContext.Current.User.CompanyName);
                globalDatalayer.AppendFormat("'sessionId': '{0}'", Session.SessionID);
                globalDatalayer.AppendLine("}");
            }
            globalDatalayer.Append("});");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GlobalDatalayer", globalDatalayer.ToString(), true);
        }



        //百度 Remarket
        protected void setBaiduRemarketing()
        {
            //add baidu remarketing code only to ACN store
            if (Presentation.eStoreContext.Current.Store.storeID.Equals("ACN", StringComparison.OrdinalIgnoreCase))
            {
                HtmlGenericControl baiduRemarketingCtrl = new HtmlGenericControl("script");
                baiduRemarketingCtrl.Attributes.Add("type", "text/javascript");

                string baiduScript = "<!-- ";
                baiduScript += "(function (d) {";
                baiduScript += "window.bd_cpro_rtid=\"rjc4PjT\";";
                baiduScript += "var s = d.createElement(\"script\");s.type = \"text/javascript\";s.async = true;s.src = location.protocol + \"//cpro.baidu.com/cpro/ui/rt.js\";";
                baiduScript += "var s0 = d.getElementsByTagName(\"script\")[0];s0.parentNode.insertBefore(s, s0);";
                baiduScript += "})(document);";
                baiduScript += "//-->";

                baiduRemarketingCtrl.InnerHtml = baiduScript;
                this.Form.Controls.Add(baiduRemarketingCtrl);
            }
        }
        //百度统计
        protected void setBaiduTongJi()
        {
            //add baidu remarketing code only to ACN store
            if (Presentation.eStoreContext.Current.Store.storeID.Equals("ACN", StringComparison.OrdinalIgnoreCase))
            {
                var baiduCode = Presentation.eStoreContext.Current.getStringSetting("BaiduTongJi");
                if(!string.IsNullOrEmpty("baiduCode"))
                {
                    HtmlGenericControl BaiduTongJiCtrl = new HtmlGenericControl("script");
                    BaiduTongJiCtrl.Attributes.Add("type", "text/javascript");
                    string baiduScript = "var _hmt = _hmt || [];(function() {  var hm = document.createElement(\"script\");  hm.src = \"//hm.baidu.com/hm.js?" + baiduCode + "\";  var s = document.getElementsByTagName(\"script\")[0];   s.parentNode.insertBefore(hm, s);})();";
                    BaiduTongJiCtrl.InnerHtml = baiduScript;
                    this.Form.Controls.Add(BaiduTongJiCtrl);
                }
            }
        }
        /// <summary>
        /// This method is to add Google AdWord conversion tracking report. 
        /// Current tracking AdWord conversions are order conversion and quote conversion
        /// </summary>
        /// <param name="totalamount"></param>
        protected void setAdWordsConversionTracking(String trackingID, String trackingLabel, decimal totalamount)
        {
            HtmlGenericControl AdWordsConversionTrackingCtrl = new HtmlGenericControl("div");
            AdWordsConversionTrackingCtrl.Style.Add("display", "inline");
            AdWordsConversionTrackingCtrl.InnerHtml =
                string.Format("<img height=\"1\" width=\"1\" style=\"border-style:none;\" alt=\"\" src=\"//www.googleadservices.com/pagead/conversion/{0}/?value={2}&label={1}&guid=ON&script=0\"/>", trackingID, trackingLabel, totalamount);
            this.Form.Controls.Add(AdWordsConversionTrackingCtrl);
        }

        protected void AddGoogleECommerceTracking(POCOS.Order order)
        {
            StringBuilder sbECommerceTracking = new StringBuilder();
            sbECommerceTracking.AppendLine("window.dataLayer = window.dataLayer || []");

            ////2016/11/08 Alex  add universal GA EC code in GTM
            sbECommerceTracking.AppendLine("dataLayer.push ({");
            sbECommerceTracking.AppendFormat("'transactionId': '{0}',", order.OrderNo);
            sbECommerceTracking.AppendFormat("'transactionAffiliation': '{0}',", Presentation.eStoreContext.Current.Store.profile.StoreName);
            sbECommerceTracking.AppendFormat("'transactionTotal': {0},", order.totalAmountX);
            sbECommerceTracking.AppendFormat("'transactionShipping': {0},", order.Freight);
            sbECommerceTracking.AppendFormat("'transactionTax': {0},", order.Tax);
            sbECommerceTracking.AppendLine("'transactionProducts': [");

            foreach (POCOS.CartItem ci in order.cartX.cartItemsX)
            {
                if (ci.BTOSystem != null)
                {
                    string productname = ci.ProductName;
                    if (ci.BTOSystem.isSBCBTOS())
                    {
                        productname = ci.ProductName + (from bc in ci.BTOSystem.BTOSConfigs
                                                        from bd in bc.BTOSConfigDetails
                                                        select bd.SProductID
                                                            ).FirstOrDefault();
                    }

                    //2016/11/08 Alex  add universal GA EC code in GTM - Adding Items
                    sbECommerceTracking.AppendLine("{");
                    sbECommerceTracking.AppendFormat("'name': '{0}',", ci.ProductName);
                    sbECommerceTracking.AppendFormat("'sku': '{1}({0})',", ci.SProductID, productname);
                    sbECommerceTracking.AppendFormat("'category': '{0}',", ci.type.ToString());
                    sbECommerceTracking.AppendFormat("'price': {0},", ci.UnitPrice.ToString());
                    sbECommerceTracking.AppendFormat("'quantity': {0}", ci.Qty);
                    sbECommerceTracking.Append("},");
                }
                else if (ci.bundleX != null)
                {
                    foreach (POCOS.BundleItem bi in ci.bundleX.BundleItems)
                    {
                        sbECommerceTracking.AppendLine("{");
                        sbECommerceTracking.AppendFormat("'name': '{0}',", bi.ItemSProductID);
                        sbECommerceTracking.AppendFormat("'sku': '{0}',", bi.ItemSProductID);
                        sbECommerceTracking.AppendFormat("'category': '{0}',", ci.type.ToString());
                        sbECommerceTracking.AppendFormat("'price': {0},",  bi.adjustedPrice.ToString());
                        sbECommerceTracking.AppendFormat("'quantity': {0}", ci.Qty * bi.Qty);
                        sbECommerceTracking.Append("},");
                    }
                }
                else
                {
                    sbECommerceTracking.AppendLine("{");
                    sbECommerceTracking.AppendFormat("'name': '{0}',", ci.ProductName);
                    sbECommerceTracking.AppendFormat("'sku': '{0}',", ci.SProductID);
                    sbECommerceTracking.AppendFormat("'category': '{0}',", ci.type.ToString());
                    sbECommerceTracking.AppendFormat("'price': {0},",  ci.UnitPrice.ToString());
                    sbECommerceTracking.AppendFormat("'quantity': {0}", ci.Qty);
                    sbECommerceTracking.Append("},");
                }
            }
            sbECommerceTracking.Length--;
            sbECommerceTracking.Append("]});");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GoogleECommerceTracking", sbECommerceTracking.ToString(), true);

            /*  This log is for debiugging purpose. It's to find out why Orders reported in Google analytics doesn't match to number of eStore orders.  */
            //Add ECommerceTracking to Log
            Utilities.eStoreLoger.Info(string.Format("ECommerceTracking Log - OrderNO:{0} - SupportJavaScript:{1} - JScriptVersion:{2} - UserAgent:{3} - GoogleECommerceTracking:{4}"
                , (String.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.OrderNo)) ? "" : Presentation.eStoreContext.Current.Order.OrderNo
                , (Request.Browser.JavaScript == null) ? "" : Request.Browser.JavaScript.ToString()
                , (Request.Browser.JScriptVersion.Major == null) ? "" : Request.Browser.JScriptVersion.Major.ToString()
                , (String.IsNullOrEmpty(Request.UserAgent)) ? "" : Request.UserAgent
                , (sbECommerceTracking == null) ? "" : sbECommerceTracking.ToString()));

        }


        bool? _isTransferedFromGoogleMerchant;
        /// <summary>
        /// http://www.google.com/aclk?sa=l&ai=C8k04cF4QVN6CI6HOsQegl4HoCcXRxYoFxZL-xcEBrYim37gCCAQQASgFUMq3hPD5_____wFgyf68h-CjtBCgAefKv_8DyAEHqgQmT9CAgv0JSMMzAUN4CANp0gFh4rewtPl6WHDeYdD5iWNEHH7qmkrABQWgBiaAB4G1QJAHA6gHpr4b4BKCv42T2qi3rM4B&sig=AOD64_2-THR2uDhSCvHp_oBbrPxclMYR5Q&ctype=5&rct=j&q=&ved=0CCEQww8&adurl=http://buy.advantech.com/Intel%2BAtom%2BN455%2BFanless%2BUltra%2BCo/ARK-1120-3S50/system-21333.htm
        /// if IsTransferedFromGoogleMerchant, HTTP_REFERER should include para ai and adurl
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsTransferedFromGoogleMerchant()
        {
            if (!_isTransferedFromGoogleMerchant.HasValue)
         
            {
                if (Presentation.eStoreContext.Current.Store.storeID != "AUS")
                {
                    _isTransferedFromGoogleMerchant = false;
                    return false;
                }

                try
                {
                    string referer = Request.ServerVariables["HTTP_REFERER"] ?? "";
                    //string referer = @"http://www.google.com/aclk?sa=l&ai=C8k04cF4QVN6CI6HOsQegl4HoCcXRxYoFxZL-xcEBrYim37gCCAQQASgFUMq3hPD5_____wFgyf68h-CjtBCgAefKv_8DyAEHqgQmT9CAgv0JSMMzAUN4CANp0gFh4rewtPl6WHDeYdD5iWNEHH7qmkrABQWgBiaAB4G1QJAHA6gHpr4b4BKCv42T2qi3rM4B&sig=AOD64_2-THR2uDhSCvHp_oBbrPxclMYR5Q&ctype=5&rct=j&q=&ved=0CCEQww8&adurl=http://buy.advantech.com/Intel%2BAtom%2BN455%2BFanless%2BUltra%2BCo/ARK-1120-3S50/system-21333.htm";

                    if (!string.IsNullOrEmpty(referer))
                    {
                        try
                        {
                            Uri uri = new Uri(referer);
                            if (uri.Host.ToLower().Contains("google"))
                            {
                                NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                                if (query.AllKeys.Contains("ai") && query.AllKeys.Contains("adurl"))
                                    _isTransferedFromGoogleMerchant = true;
                                else

                                    _isTransferedFromGoogleMerchant = false;
                            }
                            else
                            {
                                _isTransferedFromGoogleMerchant = false;
                            }
                        }
                        catch (Exception)
                        {

                            _isTransferedFromGoogleMerchant = false;
                        }
                    }
                    else
                        _isTransferedFromGoogleMerchant = false;
                }
                catch (Exception)
                {

                    _isTransferedFromGoogleMerchant = false;
                }
                
            }
            return _isTransferedFromGoogleMerchant.GetValueOrDefault(false);
        }

        protected virtual void BindScript(string ScriptsType, string ScriptsName, string Script, Boolean addScriptTags = true)
        {
            if (ScriptsType.ToLower() == "url")
            {
                string jquery = "";
                if (!Script.ToLower().Contains("http"))
                    jquery = CommonHelper.GetStoreLocation() + "Scripts/" + Script;
                else
                    jquery = Script;
                Page.ClientScript.RegisterClientScriptInclude(ScriptsName, jquery);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), ScriptsName, Script, addScriptTags);
            }
        }

        protected virtual void AddStyleSheet(string link, string media = null)
        {
            HtmlLink csslink = new HtmlLink();
            csslink.Href = link;
            csslink.Attributes.Add("rel", "stylesheet");
            csslink.Attributes.Add("type", "text/css");
            if (!string.IsNullOrEmpty(media))
                csslink.Attributes.Add("media", media);
            try
            {
                if (this.Page.Header != null)
                    this.Page.Header.Controls.Add(csslink);
            }
            catch (Exception)
            {

            }

        }

        protected void AddMigoTrackingCode()
        {
            HtmlMeta wmx_model = new HtmlMeta();
            wmx_model.Name = "wmx_model";
            HtmlMeta wmx_engagement = new HtmlMeta();
            wmx_engagement.Name = "wmx_engagement";
            if (Presentation.eStoreContext.Current.keywords.ContainsKey("ProductID"))
            {
                wmx_model.Content = Presentation.eStoreContext.Current.keywords["ProductID"];
                wmx_engagement.Content = "2";
            }
            else if (Presentation.eStoreContext.Current.keywords.ContainsKey("CategoryID"))
            {
                wmx_model.Content = Presentation.eStoreContext.Current.keywords["CategoryID"];
                wmx_engagement.Content = "1";
            }
            else
            {
                wmx_model.Content = "";
                wmx_engagement.Content = "1";
            }
            Page.Header.Controls.Add(wmx_model);
            Page.Header.Controls.Add(wmx_engagement);
            StringBuilder sbMigoTrackingCode = new StringBuilder();
            //sbMigoTrackingCode.AppendLine("<script  language=\"javascript\">");
            sbMigoTrackingCode.AppendLine("document.write(\"<scrip\"+ \"t src=\\\"\" + (document.location.protocol==\"https:\"?\"https:\":\"http:\") + \"//migotracking.advantech.com.tw/web_service/TrackingService/wmx_e15d24edeb.js\\\"></\" + \"script>\");");
            //sbMigoTrackingCode.AppendLine("</script>");
            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "script";
            c.Attributes.Add("type", "text/javascript");
            c.InnerHtml = sbMigoTrackingCode.ToString();
            Page.Header.Controls.Add(c);


        }


        protected void AddNaverTrafficTracking()
        {
            StringBuilder sbTrackingCode = new StringBuilder();
            sbTrackingCode.AppendLine("//<![CDATA[");
            sbTrackingCode.AppendLine("function logCorpAScript_full() {");
            sbTrackingCode.AppendLine("HTTP_MSN_MEMBER_NAME = \"\"; /*member name*/");
            sbTrackingCode.AppendLine("var prtc = (document.location.protocol == \"https:\") ? \"https://\" : \"http://\";");
            sbTrackingCode.AppendLine("var hst = prtc + \"asp28.http.or.kr\";");
            sbTrackingCode.AppendLine("var rnd = \"r\" + (new Date().getTime() * Math.random() * 9);");
            sbTrackingCode.AppendLine("this.ch = function () {");
            sbTrackingCode.AppendLine("if (document.getElementsByTagName(\"head\")[0]) { logCorpAnalysis_full.dls(); } else { window.setTimeout(logCorpAnalysis_full.ch, 30) }");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("this.dls = function () {");
            sbTrackingCode.AppendLine("var h = document.getElementsByTagName(\"head\")[0];");
            sbTrackingCode.AppendLine("var s = document.createElement(\"script\"); s.type = \"text/jav\" + \"ascript\"; try { s.defer = true; } catch (e) { }; try { s.async = true; } catch (e) { };");
            sbTrackingCode.AppendLine("if (h) { s.src = hst + \"/HTTP_MSN/UsrConfig/advantechkr/js/ASP_Conf.js?s=\" + rnd; h.appendChild(s); }");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("this.init = function () {");
            sbTrackingCode.AppendLine("document.write('<img src=\"' + hst + '/sr.gif?d=' + rnd + '\"  style=\"width:1px;height:1px;position:absolute;\" alt=\"\" onload=\"logCorpAnalysis_full.ch()\" />');");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("if (typeof logCorpAnalysis_full == \"undefined\") { var logCorpAnalysis_full = new logCorpAScript_full(); logCorpAnalysis_full.init(); }");
            sbTrackingCode.AppendLine("//]]>");



            sbTrackingCode.AppendLine("if( typeof HL_GUL == 'undefined' ){");

            sbTrackingCode.AppendLine("var HL_GUL = 'ngc13.nsm-corp.com';var HL_GPT='80'; var _AIMG = new Image(); var _bn=navigator.appName; var _PR = location.protocol==\"https:\"?\"https://\"+HL_GUL:\"http://\"+HL_GUL+\":\"+HL_GPT;if( _bn.indexOf(\"Netscape\") > -1 || _bn==\"Mozilla\"){ setTimeout(\"_AIMG.src = _PR+'/?cookie';\",1); } else{ _AIMG.src = _PR+'/?cookie'; };");
            sbTrackingCode.AppendLine("var _JV=\"AMZ2014031401\";//script Version");
            sbTrackingCode.AppendLine("var HL_GCD = 'CS4B39394315752'; // gcode");
            sbTrackingCode.AppendLine("var _UD='undefined';var _UN='unknown';");
            sbTrackingCode.AppendLine("function _IX(s,t){return s.indexOf(t)}");
            sbTrackingCode.AppendLine("function _GV(b,a,c,d){ var f = b.split(c);for(var i=0;i<f.length; i++){ if( _IX(f[i],(a+d))==0) return f[i].substring(_IX(f[i],(a+d))+(a.length+d.length),f[i].length); }	return ''; }");
            sbTrackingCode.AppendLine("function _XV(b,a,c,d,e){ var f = b.split(c);var g='';for(var i=0;i<f.length; i++){ if( _IX(f[i],(a+d))==0){ try{eval(e+\"=f[i].substring(_IX(f[i],(a+d))+(a.length+d.length),f[i].length);\");}catch(_e){}; continue;}else{ if(g) g+= '&'; g+= f[i];}; } return g;};");
            sbTrackingCode.AppendLine("function _NOB(a){return (a!=_UD&&a>0)?new Object(a):new Object()}");
            sbTrackingCode.AppendLine("function _NIM(){return new Image()}");
            sbTrackingCode.AppendLine("function _IL(a){return a!=_UD?a.length:0}");
            sbTrackingCode.AppendLine("function _ILF(a){ var b = 0; try{eval(\"b = a.length\");}catch(_e){b=0;}; return b; }");
            sbTrackingCode.AppendLine("function _VF(a,b){return a!=_UD&&(typeof a==b)?1:0}");
            sbTrackingCode.AppendLine("function _LST(a,b){if(_IX(a,b)>0){ a=a.substring(0,_IX(a,b));}; return a;}");
            sbTrackingCode.AppendLine("function _CST(a,b){if(_IX(a,b)>0) a=a.substring(_IX(a,b)+_IL(b),_IL(a));return a}");
            sbTrackingCode.AppendLine("function _UL(a){a=_LST(a,'#');a=_CST(a,'://');return a}");
            sbTrackingCode.AppendLine("function _AA(a){return new Array(a?a:0)}");
            sbTrackingCode.AppendLine("function _IDV(a){return (typeof a!=_UD)?1:0}");
            sbTrackingCode.AppendLine("if(!_IDV(HL_GUL)) var HL_GUL ='ngc13.nsm-corp.com'; ");
            sbTrackingCode.AppendLine("if(!_IDV(HL_GPT)) var HL_GPT ='80';");
            sbTrackingCode.AppendLine("_DC = document.cookie ;");
            sbTrackingCode.AppendLine("function _AGC(nm) { var cn = nm + \"=\"; var nl = cn.length; var cl = _DC.length; var i = 0; while ( i < cl ) { var j = i + nl; if ( _DC.substring( i, j ) == cn ){ var val = _DC.indexOf(\";\", j ); if ( val == -1 ) val = _DC.length; return unescape(_DC.substring(j, val)); }; i = _DC.indexOf(\" \", i ) + 1; if ( i == 0 ) break; } return ''; }");
            sbTrackingCode.AppendLine("function _ASC( nm, val, exp ){var expd = new Date(); if ( exp ){ expd.setTime( expd.getTime() + ( exp * 1000 )); document.cookie = nm+\"=\"+ escape(val) + \"; expires=\"+ expd.toGMTString() +\"; path=\"; }else{ document.cookie = nm + \"=\" + escape(val);};}");
            sbTrackingCode.AppendLine("function SetUID() {     var newid = ''; var d = new Date(); var t = Math.floor(d.getTime()/1000); newid = 'UID-' + t.toString(16).toUpperCase(); for ( var i = 0; i < 16; i++ ){ var n = Math.floor(Math.random() * 16).toString(16).toUpperCase(); newid += n; }       return newid; }");
            sbTrackingCode.AppendLine("var _FCV = _AGC(\"ACEFCID\"); if ( !_FCV ) { _FCV = SetUID(); _ASC( \"ACEFCID\", _FCV , 86400 * 30 * 12 ); _FCV=_AGC(\"ACEFCID\");}");
            sbTrackingCode.AppendLine("var _AIO = _NIM(); var _AIU = _NIM();  var _AIW = _NIM();  var _AIX = _NIM();  var _AIB = _NIM();  var __hdki_xit = _NIM();");
            sbTrackingCode.AppendLine("var _gX='/?xuid='+HL_GCD+'&sv='+_JV,_gF='/?fuid='+HL_GCD+'&sv='+_JV,_gU='/?uid='+HL_GCD+'&sv='+_JV+\"&FCV=\"+_FCV,_gE='/?euid='+HL_GCD+'&sv='+_JV,_gW='/?wuid='+HL_GCD+'&sv='+_JV,_gO='/?ouid='+HL_GCD+'&sv='+_JV,_gB='/?buid='+HL_GCD+'&sv='+_JV;");

            sbTrackingCode.AppendLine("var _d=_rf=_end=_fwd=_arg=_xrg=_av=_bv=_rl=_ak=_xrl=_cd=_cu=_bz='',_sv=11,_tz=20,_ja=_sc=_ul=_ua=_UA=_os=_vs=_UN,_je='n',_bR='blockedReferrer';");
            sbTrackingCode.AppendLine("if(!_IDV(_CODE)) var _CODE = '' ;");
            sbTrackingCode.AppendLine("_tz = Math.floor((new Date()).getTimezoneOffset()/60) + 29 ;if( _tz > 24 ) _tz = _tz - 24 ;");
            sbTrackingCode.AppendLine("// Javascript Variables");
            sbTrackingCode.AppendLine("if(!_IDV(_amt)) var _amt=0 ;if(!_IDV(_pk)) var _pk='' ;if(!_IDV(_pd)) var _pd='';if(!_IDV(_ct)) var _ct='';");
            sbTrackingCode.AppendLine("if(!_IDV(_ll)) var _ll='';if(!_IDV(_ag)) var _ag=0;	if(!_IDV(_id)) var _id='' ;if(!_IDV(_mr)) var _mr = _UN;");
            sbTrackingCode.AppendLine("if(!_IDV(_gd)) var _gd=_UN;if(!_IDV(_jn)) var _jn='';if(!_IDV(_jid)) var _jid='';if(!_IDV(_skey)) var _skey='';");
            sbTrackingCode.AppendLine("if(!_IDV(_ud1)) var _ud1='';if(!_IDV(_ud2)) var _ud2='';if(!_IDV(_ud3)) var _ud3='';");
            sbTrackingCode.AppendLine("if( !_ag ){ _ag = 0 ; }else{ _ag = parseInt(_ag); }");
            sbTrackingCode.AppendLine("if( _ag < 0 || _ag > 150 ){ _ag = 0; }");
            sbTrackingCode.AppendLine("if( _gd != 'man' && _gd != 'woman' ){ _gd =_UN;};if( _mr != 'married' && _mr != 'single' ){ _mr =_UN;};if( _jn != 'join' && _jn != 'withdraw' ){ _jn ='';};");
            sbTrackingCode.AppendLine("if( _ag > 0 || _gd == 'man' || _gd == 'woman'){ _id = 'undefined_member';}");
            sbTrackingCode.AppendLine("if( _jid != '' ){ _jid = 'undefined_member'; }");
            sbTrackingCode.AppendLine("_je = (navigator.javaEnabled()==true)?'1':'0';_bn=navigator.appName;");
            sbTrackingCode.AppendLine("if(_bn.substring(0,9)==\"Microsoft\") _bn=\"MSIE\";");
            sbTrackingCode.AppendLine("_bN=(_bn==\"Netscape\"),_bI=(_bn==\"MSIE\"),_bO=(_IX(navigator.userAgent,\"Opera\")>-1);if(_bO)_bI='';");
            sbTrackingCode.AppendLine("_bz=navigator.appName; _pf=navigator.platform; _av=navigator.appVersion; _bv=parseFloat(_av) ;");
            sbTrackingCode.AppendLine("if(_bI){_cu=navigator.cpuClass;}else{_cu=navigator.oscpu;};");
            sbTrackingCode.AppendLine("if((_bn==\"MSIE\")&&(parseInt(_bv)==2)) _bv=3.01;_rf=document.referrer;var _prl='';var _frm=false;");
            sbTrackingCode.AppendLine("function _WO(a,b,c){window.open(a,b,c)}");
            sbTrackingCode.AppendLine("function ACEF_Tracking(a,b,c,d,e,f){ if(!_IDV(b)){var b = 'FLASH';}; if(!_IDV(e)){ var e = '0';};if(!_IDV(c)){ var c = '';};if(!_IDV(d)){ var d = '';}; var a_org=a; b = b.toUpperCase(); var b_org=b;	if(b_org=='FLASH_S'){ b='FLASH'; }; if( typeof CU_rl == 'undefined' ) var CU_rl = _PT(); if(_IDV(HL_GCD)){ var _AF_rl = document.URL; if(a.indexOf('://') < 0  && b_org != 'FLASH_S' ){ var _AT_rl  = ''; if( _AF_rl.indexOf('?') > 0 ){ _AF_rl = _AF_rl.substring(0,_AF_rl.indexOf('?'));}; var spurl = _AF_rl.split('/') ;	for(var ti=0;ti < spurl.length ; ti ++ ){ if( ti == spurl.length-1 ){ break ;}; if( _AT_rl  == '' ){ _AT_rl  = spurl[ti]; }else{ _AT_rl  += '/'+spurl[ti];}; }; var _AU_arg = ''; if( a.indexOf('?') > 0 ){ _AU_arg = a.substring(a.indexOf('?'),a.length); a = a.substring(0,a.indexOf('?')); }; var spurlt = a.split('/') ; if( spurlt.length > 0 ){ a = spurlt[spurlt.length-1];}; a = _AT_rl +'/'+a+_AU_arg;	_AF_rl=document.URL;}; _AF_rl = _AF_rl.substring(_AF_rl.indexOf('//')+2,_AF_rl.length); if( typeof f == 'undefined' ){ var f = a }else{f='http://'+_AF_rl.substring(0,_AF_rl.indexOf('/')+1)+f}; var _AS_rl = CU_rl+'/?xuid='+HL_GCD+'&url='+escape(_AF_rl)+'&xlnk='+escape(f)+'&fdv='+b+'&idx='+e+'&'; var _AF_img = new Image(); _AF_img.src = _AS_rl; if( b_org == 'FLASH' && a_org != '' ){ if(c==''){ window.location.href = a_org; }else{ if(d==''){ window.open(a_org,c);}else{ window.open(a_org,c,d); };};	};} ; }");
            sbTrackingCode.AppendLine("function _PT(){return location.protocol==\"https:\"?\"https://\"+HL_GUL:\"http://\"+HL_GUL+\":\"+HL_GPT}");
            sbTrackingCode.AppendLine("function _EL(a,b,c){if(a.addEventListener){a.addEventListener(b,c,false)}else if(a.attachEvent){a.attachEvent(\"on\"+b,c)} }");
            sbTrackingCode.AppendLine("function _NA(a){return new Array(a?a:0)}");
            sbTrackingCode.AppendLine("function HL_ER(a,b,c,d){_xrg=_PT()+_gW+\"&url=\"+escape(_UL(document.URL))+\"&err=\"+((typeof a==\"string\")?a:\"Unknown\")+\"&ern=\"+c+\"&bz=\"+_bz+\"&bv=\"+_vs+\"&RID=\"+Math.random()+\"&\";");
            sbTrackingCode.AppendLine("if(_IX(_bn,\"Netscape\") > -1 || _bn == \"Mozilla\"){ setTimeout(\"_AIW.src=_xrg;\",1); } else{ _AIW.src=_xrg; } }");
            sbTrackingCode.AppendLine("function HL_PL(a){if(!_IL(a))a=_UL(document.URL);");
            sbTrackingCode.AppendLine("_arg = _PT()+_gU;");
            sbTrackingCode.AppendLine("if( typeof HL_ERR !=_UD && HL_ERR == 'err'){ _arg = _PT()+_gE;};");
            sbTrackingCode.AppendLine("if( _ll.length > 0 ) _arg += \"&md=b\";");
            sbTrackingCode.AppendLine("_AIU.src = _arg+\"&url=\"+escape(a)+\"&ref=\"+escape(_rf)+\"&cpu=\"+_cu+\"&bz=\"+_bz+\"&bv=\"+_vs+\"&os=\"+_os+\"&dim=\"+_d+\"&cd=\"+_cd+\"&je=\"+_je+\"&jv=\"+_sv+\"&tz=\"+_tz+\"&ul=\"+_ul+\"&ad_key=\"+escape(_ak)+\"&skey=\"+escape(_skey)+\"&age=\"+_ag+\"&gender=\"+_gd+\"&marry=\"+_mr+\"&join=\"+_jn+\"&member_key=\"+_id+\"&jid=\"+_jid+\"&udf1=\"+_ud1+\"&udf2=\"+_ud2+\"&udf3=\"+_ud3+\"&amt=\"+_amt+\"&frwd=\"+_fwd+\"&pd=\"+escape(_pd)+\"&ct=\"+escape(_ct)+\"&ll=\"+escape(_ll)+\"&RID=\"+Math.random()+\"&\";");
            sbTrackingCode.AppendLine("setTimeout(\"\",300);");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("_EL(window,\"error\",HL_ER); //window Error");
            sbTrackingCode.AppendLine("if( typeof window.screen == 'object'){_sv=12;_d=screen.width+'*'+screen.height;_sc=_bI?screen.colorDepth:screen.pixelDepth;if(_sc==_UD)_sc=_UN;}");
            sbTrackingCode.AppendLine("_ro=_NA();if(_ro.toSource||(_bI&&_ro.shift))_sv=13;");
            sbTrackingCode.AppendLine("if( top && typeof top == 'object' &&_ILF(top.frames)){eval(\"try{_rl=top.document.URL;}catch(_e){_rl='';};\"); if( _rl != document.URL ) _frm = true;};");
            sbTrackingCode.AppendLine("if(_frm){ eval(\"try{_prl = top.document.URL;}catch(_e){_prl=_bR;};\"); if(_prl == '') eval(\"try{_prl=parent.document.URL;}catch(_e){_prl='';};\"); ");
            sbTrackingCode.AppendLine("if( _IX(_prl,'#') > 0 ) _prl=_prl.substring(0,_IX(_prl,'#')); ");
            sbTrackingCode.AppendLine("_prl=_LST(_prl,'#');");
            sbTrackingCode.AppendLine("if( _IX(_rf,'#') > 0 ) _rf=_rf.substring(0,_IX(_rf,'#')); ");
            sbTrackingCode.AppendLine("if( _IX(_prl,'/') > 0 && _prl.substring(_prl.length-1,1) == '/' ) _prl =_prl.substring(0,_prl.length-1);");
            sbTrackingCode.AppendLine("if( _IX(_rf,'/') > 0 && _rf.substring(_rf.length-1,1) == '/' ) _rf =_rf.substring(0,_rf.length-1);");
            sbTrackingCode.AppendLine("if( _rf == '' ) eval(\"try{_rf=parent.document.URL;}catch(_e){_rf=_bR;}\"); ");
            sbTrackingCode.AppendLine("if(_rf==_bR||_prl==_bR){ _rf='',_prl='';}; if( _rf == _prl ){ eval(\"try{_rf=top.document.referrer;}catch(_e){_rf='';}\"); ");
            sbTrackingCode.AppendLine("if( _rf == ''){ _rf = 'bookmark';};if( _IX(document.cookie,'ACEN_CK='+escape(_rf)) > -1 ){ _rf = _prl;} ");
            sbTrackingCode.AppendLine("else{ ");
            sbTrackingCode.AppendLine("if(_IX(_prl,'?') > 0){ _ak = _prl.substring(_IX(_prl,'?')+1,_prl.length); _prl = _ak; }");
            sbTrackingCode.AppendLine("if( _IX(_prl.toUpperCase(),'OVRAW=') >= 0 ){ _ak = 'src=overture&kw='+_GV(_prl.toUpperCase(),'OVRAW','&','=')+'&OVRAW='+_GV(_prl.toUpperCase(),'OVRAW','&','=')+'&OVKEY='+_GV(_prl.toUpperCase(),'OVKEY','&','=')+'&OVMTC='+_GV(_prl.toUpperCase(),'OVMTC','&','=').toLowerCase() }; ");
            sbTrackingCode.AppendLine("if(_IX(_prl,'gclid=') >= 0 ){ _ak='src=adwords'; }; if(_IX(_prl,'DWIT=') >= 0 ){_ak='src=dnet_cb';}; ");
            sbTrackingCode.AppendLine("if( _IX(_prl,\"rcsite=\") >= 0 &&  _IX(_prl,\"rctype=\") >= 0){ _prl += '&'; _ak = _prl.substring(_IX(_prl,'rcsite='),_prl.indexOf('&',_IX(_prl,'rcsite=')+7))+'-'+_prl.substring(_IX(_prl,'rctype=')+7,_prl.indexOf('&',_IX(_prl,'rctype=')+7))+'&'; };");
            sbTrackingCode.AppendLine("if( _GV(_prl,'src','&','=') ) _ak += '&src='+_GV(_prl,'src','&','='); if( _GV(_prl,'kw','&','=') ) _ak += '&kw='+_GV(_prl,'kw','&','='); ");
            sbTrackingCode.AppendLine("if( _IX(_prl, 'FWDRL')> 0 ){ _prl = _XV(_prl,'FWDRL','&','=','_rf'); _rf = unescape(_rf); }; if( _IX(_ak,'FWDRL') > 0 ){_ak = _XV(_ak,'FWDRL','&','=','_prl');}; if( typeof FD_ref=='string' && FD_ref != '' ) _rf = FD_ref; _fwd = _GV(_prl,'FWDIDX','&','=');");
            sbTrackingCode.AppendLine("document.cookie='ACEN_CK='+escape(_rf)+';path=/;'; ");
            sbTrackingCode.AppendLine("}; ");
            sbTrackingCode.AppendLine("if(document.URL.indexOf('?')>0 && ( _IX(_ak,'rcsite=') < 0 && _IX(_ak,'NVAR=') < 0 && _IX(_ak,'src=') < 0 && _IX(_ak,'source=') < 0 && _IX(_ak,'DMCOL=') < 0 ) ) _ak =document.URL.substring(document.URL.indexOf('?')+1,document.URL.length); }; ");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("else{ ");
            sbTrackingCode.AppendLine("_rf=_LST(_rf,'#');_ak=_CST(document.URL,'?');");
            sbTrackingCode.AppendLine("if( _IX(_ak,\"rcsite=\") > 0 &&  _IX(_ak,\"rctype=\") > 0){");
            sbTrackingCode.AppendLine("    _ak += '&';");
            sbTrackingCode.AppendLine("    _ak = _ak.substring(_IX(_ak,'rcsite='),_ak.indexOf('&',_IX(_ak,'rcsite=')+7))+'-'+_ak.substring(_IX(_ak,'rctype=')+7,_ak.indexOf('&',_IX(_ak,'rctype=')+7))+'&';");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("_rl=document.URL;");
            sbTrackingCode.AppendLine("var _trl = _rl.split('?'); if(_trl.length>1){ if( _IX(_trl[1],'FWDRL') > 0 ){ _trl[1] = _XV(_trl[1],'FWDRL','&','=','_rf'); _rf = unescape(_rf); _fwd = _GV(_trl[1],'FWDIDX','&','='); _rl=_trl.join('?'); };  if( _IX(_ak,'FWDRL') > 0 ){ _ak = _XV(_ak,'FWDRL','&','=','_prl');}; }; if( typeof FD_ref=='string' && FD_ref != '' ) _rf = FD_ref;");
            sbTrackingCode.AppendLine("if( _rf.indexOf('googlesyndication.com') > 0 ){ ");
            sbTrackingCode.AppendLine("var _rf_idx = _rf.indexOf('&url=');  if( _rf_idx > 0 ){ var _rf_t = unescape(_rf.substring(_rf_idx+5,_rf.indexOf('&',_rf_idx+5)));  if( _rf_t.length > 0 ){ _rf = _rf_t ;};  };  };");
            sbTrackingCode.AppendLine("_rl = _UL(_rl); _rf = _UL(_rf);");

            sbTrackingCode.AppendLine("if( typeof _rf_t != 'undefined' && _rf_t != '' ) _rf = _rf_t ;");
            sbTrackingCode.AppendLine("if( typeof _ak_t != 'undefined' && _ak_t != '' ) _ak = _ak_t ;");

            sbTrackingCode.AppendLine("if( typeof _rf==_UD||( _rf == '' )) _rf = 'bookmark' ;_cd=(_bI)?screen.colorDepth:screen.pixelDepth;");
            sbTrackingCode.AppendLine("_UA = navigator.userAgent;_ua = navigator.userAgent.toLowerCase();");
            sbTrackingCode.AppendLine("if (navigator.language){  _ul = navigator.language.toLowerCase();}else if(navigator.userLanguage){  _ul = navigator.userLanguage.toLowerCase();};");

            sbTrackingCode.AppendLine("_st = _IX(_UA,'(') + 1;_end = _IX(_UA,')');_str = _UA.substring(_st, _end);_if = _str.split('; ');_cmp = _UN ;");

            sbTrackingCode.AppendLine("if(_bI){ _cmp = navigator.appName; _str = _if[1].substring(5, _if[1].length); _vs = (parseFloat(_str)).toString();} ");
            sbTrackingCode.AppendLine("else if ( (_st = _IX(_ua,\"opera\")) > 0){ _cmp = \"Opera\" ;_vs = _ua.substring(_st+6, _ua.indexOf('.',_st+6)); } ");
            sbTrackingCode.AppendLine("else if ((_st = _IX(_ua,\"firefox\")) > 0){_cmp = \"Firefox\"; _vs = _ua.substring(_st+8, _ua.indexOf('.',_st+8)); } ");
            sbTrackingCode.AppendLine("else if ((_st = _IX(_ua,\"netscape6\")) > 0){ _cmp = \"Netscape\"; _vs = _ua.substring(_st+10, _ua.length);  ");
            sbTrackingCode.AppendLine("if ((_st = _IX(_vs,\"b\")) > 0 ) { _str = _vs.substring(0,_IX(_vs,\"b\")); _vs = _str ;  };}");
            sbTrackingCode.AppendLine("else if ((_st = _IX(_ua,\"netscape/7\")) > 0){  _cmp = \"Netscape\";  _vs = _ua.substring(_st+9, _ua.length);  if ((_st = _IX(_vs,\"b\")) > 0 ){ _str = _vs.substring(0,_IX(_vs,\"b\")); _vs = _str;};");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("else{");
            sbTrackingCode.AppendLine("if (_IX(_ua,\"gecko\") > 0){ if(_IX(_ua,\"safari\")>0){ _cmp = \"Safari\";_ut = _ua.split('/');for( var ii=0;ii<_ut.length;ii++) if(_IX(_ut[ii],'safari')>0){ _vst = _ut[ii].split(' '); _vs = _vst[0];} }else{ _cmp = navigator.vendor;  } } else if (_IX(_ua,\"nav\") > 0){ _cmp = \"Netscape Navigator\";}else{ _cmp = navigator.appName;}; _av = _UA ; ");
            sbTrackingCode.AppendLine("}");
            sbTrackingCode.AppendLine("if (_IX(_vs,'.')<0){  _vs = _vs + '.0'}");
            sbTrackingCode.AppendLine("_bz = _cmp; ");


            sbTrackingCode.AppendLine("var nhn_ssn={uid:HL_GCD,g:HL_GUL,p:HL_GPT,s:_JV,rl:_rl,m:[],run:nhn_ssn?nhn_ssn.uid:this.uid};");
            sbTrackingCode.AppendLine("function CS4B39394315752(){var f={e:function(s,t){return s.indexOf(t);},d:function(s){var r=String(s); return r.toUpperCase();},f:function(o){var a;a=o;if(f.d(a.tagName)=='A' || f.d(a.tagName)=='AREA'){return a;}else if(f.d(a.tagName)=='BODY'){return 0;}else{return f.f(a.parentNode);} },n:function(s){var str=s+\"\";var ret=\"\";for(i = 0; i < str.length; i++){	var at = str.charCodeAt(i);var ch=String. fromCharCode(at);	if(at==10 || at==32){ret+=''+ch.replace(ch,'');}else if (at==34||at==39|at==35){ret+=''+ch.replace(ch,' ');	}else{ret+=''+ch;}  } return ret;},ea:function(c,f){var wd;if(c=='click'){wd=window.document;}else{wd=window;}if(wd.addEventListener){wd.addEventListener(c,f,false)}else if(wd.attachEvent){wd.attachEvent(\"on\"+c,f)} } };");
            sbTrackingCode.AppendLine("var p={h:location.host,p:(location.protocol=='https:')?\"https://\"+nhn_ssn.g:\"http://\"+nhn_ssn.g+\":\"+nhn_ssn.p,s:'/?xuid='+nhn_ssn.uid+'&sv='+nhn_ssn.s,u:function(){var r='';r=String(nhn_ssn.rl);var sh=r.indexOf('#'); if(sh!=-1){r=r.substring(0,sh);}return r+'';},ol:new Image(0,0),xL:function(x){if(typeof(Amz_T_e)==s.u){ p.ol.src=p.p+p.s+'&url='+escape(p.u())+'&xlnk='+escape(x)+'&xidx=0'+'&crn='+Math.random()+'&';nhn_ssn.m.push(p.ol);} } };");
            sbTrackingCode.AppendLine("var s={Lp:'a.tagName==\"B\" || a.tagName==\"I\" || a.tagName== \"U\" || a.tagName== \"FONT\" || a.tagName==\"I\" || a.tagName==\"STRONG\"'  ,f:'function',	j:'javascript:',u:'undefined'};var c={Run:function(){f.ea('click',this.ec);},ec:function(e){var ok='';var m = document.all ? event.srcElement : e.target;var a=m;var o=m.tagName;if(o==\"A\" || o==\"AREA\" || o==\"IMG\" || eval(s.Lp)){ok=c.lc(m);if(ok.length != 0){p.xL(unescape(ok));};	};},lc:function(o){ try{var ar='';var obj;obj=f.f(o);if(typeof obj==s.u){return '';};if(typeof(obj.href)==s.u){return '';};ar = String(obj.href);if(ar.length == 0){return '';};ar=f.n(ar);if(f.e(ar,'void(') == -1 && f.e(ar,'void0') == -1){	return ar;}else{return s.j + 'void(0)';};return '';}catch(er){return '';} } };");
            sbTrackingCode.AppendLine("if(p.u().charAt(1) != ':'){if(nhn_ssn.uid!=nhn_ssn.run){c.Run(); } };");
            sbTrackingCode.AppendLine("};eval(nhn_ssn.uid + '();');");


            sbTrackingCode.AppendLine("if( _IX(_pf,_UD) >= 0 || _pf ==  '' ){ _os = _UN ;}else{ _os = _pf ; };");
            sbTrackingCode.AppendLine("if( _IX(_os,'Win32') >= 0 ){if( _IX(_av,'98')>=0){ _os = 'Windows 98';}else if( _IX(_av,'95')>=0 ){ _os = 'Windows 95';}else if( _IX(_av,'Me')>=0 ){ _os = 'Windows Me';}else if( _IX(_av,'NT')>=0 ){ _os = 'Windows NT';}else{ _os = 'Windows';};if( _IX(_ua,'nt 5.0')>=0){ _os = 'Windows 2000';};if( _IX(_ua,'nt 5.1')>=0){_os = 'Windows XP';if( _IX(_ua,'sv1') > 0 ){_os = 'Windows XP SP2';};};if( _IX(_ua,'nt 5.2')>=0){_os ='Windows Server 2003';};if( _IX(_ua,'nt 6.0')>=0){_os ='Windows Vista';};if( _IX(_ua,'nt 6.1')>=0){_os ='Windows 7';};};");
            sbTrackingCode.AppendLine("_pf_s = _pf.substring(0,4);if( _pf_s == 'Wind'){if( _pf_s == 'Win1'){_os = 'Windows 3.1';}else if( _pf_s == 'Mac6' ){ _os = 'Mac';}else if( _pf_s == 'MacO' ){ _os ='Mac';}else if( _pf_s == 'MacP' ){_os='Mac';}else if(_pf_s == 'Linu'){_os='Linux';}else if( _pf_s == 'WebT' ){ _os='WebTV';}else if(  _pf_s =='OSF1' ){ _os ='Compaq Open VMS';}else if(_pf_s == 'HP-U' ){ _os='HP Unix';}else if(  _pf_s == 'OS/2' ){ _os = 'OS/2' ;}else if( _pf_s == 'AIX4' ){ _os = 'AIX';}else if( _pf_s == 'Free' ){ _os = 'FreeBSD';}else if( _pf_s == 'SunO' ){ _os = 'SunO';}else if( _pf_s == 'Drea' ){ _os = 'Drea'; }else if( _pf_s == 'Plan' ){ _os = 'Plan'; }else{ _os = _UN; };};");
            sbTrackingCode.AppendLine("if( _cu == 'x86' ){ _cu = 'Intel x86';}else if( _cu == 'PPC' ){ _cu = 'Power PC';}else if( _cu == '68k' ){ _cu = 'Motorola 680x';}else if( _cu == 'Alpha' ){ _cu = 'Compaq Alpa';}else if( _cu == 'Arm' ){ _cu = 'ARM';}else{ _cu = _UN;};if( _d == '' || typeof _d==_UD ){ _d = '0*0';}");

            sbTrackingCode.AppendLine("HL_PL(_rl); // Site Logging");
            sbTrackingCode.AppendLine("}");

            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "script";
            c.Attributes.Add("type", "text/javascript");
            c.InnerHtml = sbTrackingCode.ToString();
            Page.Header.Controls.Add(c);

            HtmlGenericControl noscript = new HtmlGenericControl();
            noscript.TagName = "noscript";
            noscript.InnerHtml = "<img src=\"http://asp28.http.or.kr/HTTP_MSN/Messenger/Noscript.php?key=advantechkr\" border=\"0\" style=\"display:none;width:0;height:0;\" />";
            Page.Header.Controls.Add(noscript);

        }

        protected void addDDNRetargeting()
        {
            StringBuilder sbTrackingCode = new StringBuilder();
            sbTrackingCode.AppendLine("var roosevelt_params = {");
            sbTrackingCode.AppendLine("retargeting_id:'TFk9u1BmBQB8evqezUbnoQ00',");
            sbTrackingCode.AppendLine("tag_label:'TaY4_rBsQcq0J2Pq8Xqfaw'};");

            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "script";
            c.Attributes.Add("type", "text/javascript");
            c.InnerHtml = sbTrackingCode.ToString();
            Page.Header.Controls.Add(c);

            HtmlGenericControl link = new HtmlGenericControl();
            link.TagName = "script";
            link.Attributes.Add("type", "text/javascript");
            link.Attributes.Add("src", "//adimg.daumcdn.net/rt/roosevelt.js");
            Page.Header.Controls.Add(link);
        }

        protected void AddAceCounterLogGathering()
        {
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("AceCounterLogGathering"))
            {
                StringBuilder AceCounterLogGathering = new StringBuilder();
                AceCounterLogGathering.AppendLine("var _AceGID=(function(){var Inf=['dgc14.acecounter.com','8080','BR1N40514915213','CW','0','NaPm,Ncisy','ALL','0'];");
                AceCounterLogGathering.AppendLine("var _CI=(!_AceGID)?[]:_AceGID.val;var _N=0;var _T=new Image(0,0);");
                AceCounterLogGathering.AppendLine(@"if(_CI.join('.').indexOf(Inf[3])<0){ _T.src =( location.protocol==""https:""?""https://""+Inf[0]:""http://""+Inf[0]+"":""+Inf[1]) +'/?cookie'; _CI.push(Inf);");
                AceCounterLogGathering.AppendLine("_N=_CI.length; } return {o: _N,val:_CI}; })();");
                AceCounterLogGathering.AppendLine("var _AceCounter=(function(){var G=_AceGID;if(G.o!=0){var _A=G.val[G.o-1];");
                AceCounterLogGathering.AppendLine("var _G=( _A[0]).substr(0,_A[0].indexOf('.'));");
                AceCounterLogGathering.AppendLine(@"var _C=(_A[7]!='0')?(_A[2]):_A[3];var _U=( _A[5]).replace(/\,/g,'_');");
                AceCounterLogGathering.AppendLine(@"var _S=((['<scr','ipt','type=""text/javascr','ipt""></scr','ipt>']).join('')).replace('tt','t src=""'+location.protocol+ '//cr.acecounter.com/Web/AceCounter_'+_C+'.js?gc='+_A[2]+'&py='+_A[4]+'&gd='+_G+'&gp='+_A[1]+'&up='+_U+'&rd='+(new Date().getTime())+'"" t');");
                AceCounterLogGathering.AppendLine("document.writeln(_S); return _S;} })();");
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AceCounterLogGathering", AceCounterLogGathering.ToString(), true);
            }
            
            HtmlGenericControl ns = new HtmlGenericControl("noscript");
            LiteralControl lc = new LiteralControl("<img src='http://dgc14.acecounter.com:8080/?uid=BR1N40514915213&je=n&' border='0' width='0' height='0' style='display:none' alt='' />");
            ns.Controls.Add(lc);
            this.Page.Form.Controls.Add(ns);
        }

        protected void AddHubSpotTracking()
        {
            StringBuilder sbTrackingCode = new StringBuilder();

            sbTrackingCode.AppendLine("(function(d,s,i,r) {");
            sbTrackingCode.AppendLine("if (d.getElementById(i)){return;}");
            sbTrackingCode.AppendLine("var n=d.createElement(s),e=d.getElementsByTagName(s)[0];");
            sbTrackingCode.AppendLine("n.id=i;n.src='//js.hs-analytics.net/analytics/'+(Math.ceil(new Date()/r)*r)+'/398452.js';");
            sbTrackingCode.AppendLine("e.parentNode.insertBefore(n, e);");
            sbTrackingCode.AppendLine("})(document,\"script\",\"hs-analytics\",300000);");
 
 
            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "script";
            c.Attributes.Add("type", "text/javascript");
            c.InnerHtml = sbTrackingCode.ToString();
            Page.Header.Controls.Add(c);
        }

        //New version of HubSpot tracking code
        protected void AddHubSpotTrackingV2()
        {
            StringBuilder sbTrackingCode = new StringBuilder();

            sbTrackingCode.AppendLine("(function(d,s,i,r) {");
            sbTrackingCode.AppendLine("if (d.getElementById(i)){return;}");
            sbTrackingCode.AppendLine("var n=d.createElement(s),e=d.getElementsByTagName(s)[0];");
            sbTrackingCode.AppendLine("n.id=i;n.src='//js.hs-analytics.net/analytics/'+(Math.ceil(new Date()/r)*r)+'/1964232.js';");
            sbTrackingCode.AppendLine("e.parentNode.insertBefore(n, e);");
            sbTrackingCode.AppendLine("})(document,\"script\",\"hs-analytics\",300000);");


            HtmlGenericControl c = new HtmlGenericControl();
            c.TagName = "script";
            c.Attributes.Add("type", "text/javascript");
            c.InnerHtml = sbTrackingCode.ToString();
            Page.Header.Controls.Add(c);
        }

        //Google Adwords Convension Tracking Code
        protected void GoogleAdwordsConvensionTrackingCode()
        {
            HtmlGenericControl jq = new HtmlGenericControl("script");
            jq.Attributes.Add("type", "text/javascript");
            jq.Attributes.Add("src", "http://advwebtracking-cloud.advantech.com/GoogleAdwords/ad.js");
            Page.Header.Controls.Add(jq);
        }

        //FaceBook Pixel tracking code
        protected void AddFacebookPixelTracking()
        {
            string ID = Presentation.eStoreContext.Current.getStringSetting("FacebookPixel");
            if (!string.IsNullOrEmpty(ID))
            {
                StringBuilder sbTrackingCode = new StringBuilder();

                sbTrackingCode.AppendLine("!function(f,b,e,v,n,t,s){if(f.fbq)return;n=f.fbq=function(){n.callMethod?");
                sbTrackingCode.AppendLine("n.callMethod.apply(n,arguments):n.queue.push(arguments)};if(!f._fbq)f._fbq=n;");
                sbTrackingCode.AppendLine("n.push=n;n.loaded=!0;n.version='2.0';n.queue=[];t=b.createElement(e);t.async=!0;");
                sbTrackingCode.AppendLine("t.src=v;s=b.getElementsByTagName(e)[0];s.parentNode.insertBefore(t,s)}");
                sbTrackingCode.AppendLine("(window, document,'script','//connect.facebook.net/en_US/fbevents.js');");
                sbTrackingCode.AppendLine(string.Format("fbq('init', '{0}');", ID));
                sbTrackingCode.AppendLine("fbq('track', \"PageView\");");

                HtmlGenericControl c = new HtmlGenericControl();
                c.TagName = "script";
                c.Attributes.Add("type", "text/javascript");
                c.InnerHtml = sbTrackingCode.ToString();
                Page.Header.Controls.Add(c);

                HtmlGenericControl ns = new HtmlGenericControl("noscript");
                LiteralControl lc = new LiteralControl(string.Format("<img height='1' width='1' style='display:none' src='https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1'/>", ID));
                ns.Controls.Add(lc);
                Page.Form.Controls.Add(ns);
            }
        }

        protected void BindGoogleAbTest()
        {
            if (eStore.Presentation.eStoreContext.Current.getStringSetting("GAExperimentID")!= string.Empty)
            {
                //Generate AB Test JS Code in HTML
                HtmlGenericControl GnerateABTestApi = new HtmlGenericControl();
                GnerateABTestApi.TagName = "script";

                string apiUrl = String.Format("//www.google-analytics.com/cx/api.js?experiment={0}", eStore.Presentation.eStoreContext.Current.getStringSetting("GAExperimentID"));

                GnerateABTestApi.Attributes.Add("src", apiUrl);
                Page.Header.Controls.Add(GnerateABTestApi);

                StringBuilder ChooseVariationFunc = new StringBuilder();
                ChooseVariationFunc.AppendLine("var chosenVariation = cxApi.chooseVariation();");
                HtmlGenericControl ABTestChooseVariation = new HtmlGenericControl();
                ABTestChooseVariation.TagName = "script";
                ABTestChooseVariation.InnerHtml = ChooseVariationFunc.ToString();
                Page.Header.Controls.Add(ABTestChooseVariation);
            }
        }
    }

}
