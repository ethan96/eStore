using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class _Default : Presentation.eStoreBaseControls.eStoreBasePage 
    {
        private Models.HomePageStyle _homePageStyle;
        public Models.HomePageStyle HomePageStyle
        {
            get
            {
                if (_homePageStyle == null)
                {
                    string homepage = Presentation.eStoreContext.Current.getStringSetting("eStore_HomePage").Replace("\\","");
                    _homePageStyle = esUtilities.JsonHelper.JsonDeserialize<Models.HomePageStyle>(homepage);
                }
                return _homePageStyle;
            }
        }
        private bool? _isNewLayout;
        public bool isNewLayout
        {
            get
            {
                if (!_isNewLayout.HasValue)
                {
                    _isNewLayout = (Presentation.eStoreContext.Current.Store.storeID == "AUS"
                        || Presentation.eStoreContext.Current.Store.storeID == "ALA"
                        || Presentation.eStoreContext.Current.Store.storeID == "ASC"
                        || Presentation.eStoreContext.Current.getBooleanSetting("isNewHomepageLayout", false)
                        );
                }
                return _isNewLayout.GetValueOrDefault(false);
            }
        }


        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            //Keep original URL path when doing store redirect.  
            if (!string.IsNullOrEmpty(Request["transferto"]))
            {
                try
                {
                    string relativeUri = ResolveUrl(Request["transferto"]);
                    //do not redirect if home page
                    Uri transferto = new Uri(Request.Url, relativeUri);
                    if (transferto.AbsolutePath.Equals(Request.Url.AbsolutePath,StringComparison.OrdinalIgnoreCase)==false)
                        Response.Redirect(transferto.ToString());
                }
                catch (Exception ex)
                {

                }
            }

            if (!string.IsNullOrEmpty(Request["EDMid"]))
                Presentation.eStoreContext.Current.Store.SaveEDMParameter(Convert.ToString(Request["EDMid"]));

            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ContentPlaceHolder eStoreContent = (ContentPlaceHolder)Master.FindControl("eStoreMainContent");
            eStoreContent.Controls.Clear();
            if (isNewLayout)
            {
                ContentPlaceHolder banner = (ContentPlaceHolder)Master.FindControl("eStoreHeaderFullSizeContent");
                banner.Visible = false;
            }
            else
            {
                var banners = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite, true);
                if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("hasRegionBanner") == true)
                    banners = banners.Where(x => x.IsGEOMatched(Presentation.eStoreContext.Current.CurrentCountry.Shorts) != null).ToList();
                eStoreCycle2Slider1.Advertisements = banners;
            }

            eStoreContent.Controls.Add(LoadControl(HomePageStyle.HomeMedialContent));

            Presentation.eStoreContext.Current.keywords.Add("HomePage", "true");
            if (Presentation.eStoreContext.Current.getBooleanSetting("TutorialEnabled"))
            {
                this.BindScript("url", "eStoreTutorial.js", "v4/eStoreTutorial.js");
                this.AddStyleSheet(ResolveUrl("~/Styles/eStoreTutorial.css"));
                string eStoreTutorial = "$(function () {InitTutorial(\"HomePage\", \"" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID + "\", \"myaccount.txt\", \".eStore_MyAccount\", \"tabs.txt\", \".eStore_index_Highlight_tabBlock\"); });";
                this.BindScript("Script", "eStoreTutorial_Home", eStoreTutorial, true);
            }
            setCanonicalPage(esUtilities.CommonHelper.GetStoreLocation());
            eStore.Presentation.OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new eStore.Presentation.OpenGraphProtocolAdapter("HomePage");
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);

            

            //20160809 Alex:add  homepage structured date 
            eStore.Presentation.StructuredDataMarkup structuredDataMarkup = new eStore.Presentation.StructuredDataMarkup();
            structuredDataMarkup.GenerateHomePageStruturedData("HomePage", this.Page);
        }


    }
}
