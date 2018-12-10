using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using esUtilities;

namespace eStore.Presentation.eStoreBaseControls
{
    public class eStoreBasePage : eStoreBaseCommonPage
    {
        public delegate bool OnLoggedin(string Sender);
        public event OnLoggedin OnLoggedinHandler;

        public bool Loggedin(string Sender)
        {
            if (OnLoggedinHandler != null)
                return OnLoggedinHandler(Sender);
            else
                return false;
        }

        private bool _OverwriteMasterPageFile = true;
        public virtual bool OverwriteMasterPageFile
        {
            get { return _OverwriteMasterPageFile; }
            set { _OverwriteMasterPageFile = value; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.MasterPageFile) && OverwriteMasterPageFile)
            {
                if (Presentation.eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.MiniSite.MasterPage))
                {
                    this.MasterPageFile = Presentation.eStoreContext.Current.MiniSite.MasterPage;
                }
                else
                {
                    this.MasterPageFile = "~/MasterPages/MPV4.Master";
                }
            }
            if (this.MasterPageFile!=null && !this.MasterPageFile.Equals(ResolveUrl( "~/MasterPages/MPV4.Master"), StringComparison.OrdinalIgnoreCase))
            {
                this.Theme = "Default";
                this.BindScript("url", "jquery-1.4.1.min", "jquery-1.4.1.min.js");
                this.BindScript("url", "jquery-ui-1.8.2.custom.min", "jquery-ui-1.8.2.custom.min.js");
                this.BindScript("url", "jquery.jtip", "jquery.jtip.js");
                this.BindScript("url", "setting", "estoresetting.axd?s=" + this.UseSSL.ToString());
                this.BindScript("url", "storeutilities", "storeutilities.js");
            }
            else
            {
                this.BindScript("url", "setting", "estoresetting.axd?s=" + this.UseSSL.ToString());
            
            }
         

            //register js base root;
            base.OnPreInit(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (string.IsNullOrWhiteSpace(Request["CouponCode"]) == false)
            {
                this.Session["autoAppliedCouponCode"] = Request["CouponCode"];
            }
            this.AddStyleSheet(ResolveUrl(string.Format("~/App_Themes/{0}/store.css", Presentation.eStoreContext.Current.Store.storeID)));
            if (!string.IsNullOrEmpty(eStoreContext.Current.Store.profile.ThemeManager))
            {
                AddStyleSheet(ResolveUrl(string.Format("~/Styles/{0}.css", eStoreContext.Current.Store.profile.ThemeManager)));
            }
            if (Presentation.eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.MiniSite.ResourceName))
            {
                AddStyleSheet(ResolveUrl(string.Format("~/Styles/{0}.css", Presentation.eStoreContext.Current.MiniSite.ResourceName)));
            }


            //log usr activity only if the request is not from robot search agents
            if (eStoreContext.Current.isRequestFromSearchEngine() == false)
            {
                if (!IsPostBack || this.GetType().Name.ToUpper() == "SEARCH_ASPX") // if page is search will write log all ways
                    this.UserActivitLog.save();
            }            
            if (!isExistsPageMeta)
                this.setPageMeta("", "", "");
            string clientIP = eStoreContext.Current.getUserIP();// string.IsNullOrEmpty(Request.UserHostAddress) ? "" : Request.UserHostAddress;

            var isLocalIp = esUtilities.IPUtility.IpIsWithinBoliviaRange(clientIP,
                            eStore.Presentation.eStoreContext.Current.Store.profile.LocationIps.Select(c => c.IPAtrrs).ToList());

            if (!isLocalIp)
            {
                this.pushGlobalDatalayer();
                this.setGTM();
                //AddFacebookPixelTracking();  //move to GTM
                //only add to acn
                if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
                {
                    setBaiduRemarketing();
                    setBaiduTongJi();
                }
                if (Presentation.eStoreContext.Current.Store.storeID == "AKR")
                {
                    AddNaverTrafficTracking();
                    addDDNRetargeting();
                    AddAceCounterLogGathering();
                }
                if (!UseSSL)
                {
                    //this.AddMigoTrackingCode();
                    AddHubSpotTracking();
                    AddHubSpotTrackingV2();
                }
                //Google Adwords Convension Tracking Code
                //if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("Google_Adwords_Convension") == true)
                //{
                //    this.GoogleAdwordsConvensionTrackingCode();
                //}

            }
         
            if (Presentation.eStoreContext.Current.StoreErrorCode.Count > 0)
            {
                StringBuilder pageErrors = new StringBuilder();
                foreach (POCOS.StoreErrorCode error in Presentation.eStoreContext.Current.StoreErrorCode)
                {
                    pageErrors.AppendFormat("{0}\\r\\n",string.IsNullOrEmpty(error.UserActionMessage)?error.DefaultMessage:error.UserActionMessage);
                }
                BindScript("Script", "eStoreMessage", "$(document).ready(function () { alert(\"" + pageErrors.ToString() + "\"); });");
            }
            // <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
            if (isMobileFriendly)
            {
                HtmlMeta viewportmeta = new HtmlMeta();
                viewportmeta.Name = "viewport";
                viewportmeta.Content = "width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0";
                this.Header.Controls.Add(viewportmeta);
                AddStyleSheet(ResolveUrl("~/App_Themes/V4/responsive.css"));
            }
            if (BlockSearchIndexing)
            {
                HtmlMeta noindexmeta = new HtmlMeta();
                noindexmeta.Name = "robots";
                noindexmeta.Content = "noindex";
                this.Header.Controls.Add(noindexmeta);
            }
            /// google abtest 
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EableAbTest"))
            {
                HttpCookie abtest = HttpContext.Current.Request.Cookies["AbGroup"]; //  用户第一次进来后将abgroup保存到 cookie中
                if (abtest == null)
                {
                    BindGoogleAbTest();
                }
            }
        }

        public void alertMessage(string message)
        { 
            if(!string.IsNullOrEmpty(message))
                BindScript("Script", "eStoreAlertMessage", "$(document).ready(function () { alert(\"" + message + "\"); });");
        }

        //move viewstate to bottom
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(stringWriter);
            if (isUseAdvTrack && !UseSSL)
            {
                System.Web.HttpCookie cookieUser = new System.Web.HttpCookie("eStore_Adv_Webtracking");
                string username = "eStoreGuester";
                if (Presentation.eStoreContext.Current.User != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.UserID))
                    username = Presentation.eStoreContext.Current.User.UserID;
                cookieUser.Value = username;
                cookieUser.Expires = DateTime.Now.AddDays(365);
                Response.AppendCookie(cookieUser);

                if (!UseSSL && Presentation.eStoreContext.Current.getBooleanSetting("EnableAdvTrack", false))
                {
                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("AdvTrack"))
                    {
                        string IncludeScript = @"https://advwebtracking.advantech.com/Track/AdvTrack.js";
                        this.Page.ClientScript.RegisterClientScriptInclude("AdvTrack", IncludeScript);
                    }
                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("jquerycookies"))
                    {
                        string IncludeScript2 = @"https://advwebtracking.advantech.com/js/jquery.cookies.2.2.0.js";
                        this.Page.ClientScript.RegisterClientScriptInclude("jquerycookies", IncludeScript2);
                    }

                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("AdvTrackParameters"))
                    {
                        string AdvTrackParameters = "var args = new Object(); args = GetUrlParms(location.search.substring(1));  var _advWebTrackingPortal = location.host; var _advWebTrackingEngagementLevel = \"\"; var _advWebTrackingPageType = \"\"; var _advWebTrackingContentID = \"\"; var _Email; var Message = $.cookies.get('Adv_Webtracking');  if (Message != null) { _Email = Message; } else { _Email = \"\"; }var _UID = args['uid']; var _CampId = args['campid'];";
                        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AdvTrackParameters", AdvTrackParameters,true);
                    }

                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("AdvTrackGoogleAdWordConvert"))
                    {
                        string IncludeScript = @"https://advwebtracking.advantech.com/GoogleAdwords/ad.js";
                        this.Page.ClientScript.RegisterClientScriptInclude("AdvTrackGoogleAdWordConvert", IncludeScript);
                    }
                }

            }
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            int StartPoint = html.IndexOf("<input type=\"hidden\" name=\"__VIEWSTATE\"");
            if (StartPoint >= 0)
            {
                int EndPoint = html.IndexOf("/>", StartPoint) + 2;
                string viewstateInput = html.Substring(StartPoint, EndPoint - StartPoint);
                html = html.Remove(StartPoint, EndPoint - StartPoint);
                int FormEndStart = html.IndexOf("</form>") ;
                if (FormEndStart >= 0)
                {
                    html = html.Insert(FormEndStart, viewstateInput);
                }
            }
            writer.Write(html);
        }

        public bool IsUseAdvTrack
        {
            get { return isUseAdvTrack; }
            set { isUseAdvTrack = value; }
        }
        private bool isUseAdvTrack = true;

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            string SiteID = Request["Site"];
            string TrackingID = Request["ReferralId"];
            if (!string.IsNullOrEmpty(SiteID) && !string.IsNullOrEmpty(TrackingID))
            {
                eStore.POCOS.Affiliate aff = new POCOS.Affiliate() { SiteID = SiteID };
                if (aff.isExistInStore())
                {
                    System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
                    col.Add("SiteID", SiteID);
                    col.Add("TrackingID", TrackingID);
                    Presentation.eStoreContext.Current.SiteID = col;
                }
            }
        }

        protected void popLoginDialog(object sender)
        {
            if (string.IsNullOrEmpty(Request["needlogin"]))
            {
                string purpose = "";
                if (sender is System.Web.UI.WebControls.Button)
                { purpose = (sender as System.Web.UI.WebControls.Button).Text; }
                else if (sender is System.Web.UI.WebControls.LinkButton)
                { purpose = (sender as System.Web.UI.WebControls.LinkButton).Text; }
                else if (sender is System.Web.UI.WebControls.HyperLink)
                { purpose = (sender as System.Web.UI.WebControls.HyperLink).Text; }

                if (Request.RawUrl.IndexOf("?") > 0)
                    Response.Redirect(Request.RawUrl + "&needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
                else
                    Response.Redirect(Request.RawUrl + "?needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
            }
        }
    }

    public class eStoreBasePagePrint : eStoreBaseCommonPage
    {
        protected override void OnPreInit(EventArgs e)
        {

            this.MasterPageFile = "~/MasterPages/Print.Master";
            this.Theme = "Print";
            base.OnPreInit(e);
        }

    }


}
