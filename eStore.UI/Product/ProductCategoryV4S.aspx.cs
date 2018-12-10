using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class ProductCategoryV4S : Presentation.eStoreBaseControls.eStoreBasePage
    {
        

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
        protected string BannerImage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true, 404);
            }
            else
            {
                string _category = Request["category"].ToString();
                
                POCOS.ProductCategory _currentProductcategory = Presentation.eStoreContext.Current.Store.getProductCategoryForCategoryPathSEO(_category);

                if (_currentProductcategory == null)
                {
                    _currentProductcategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                }

                // if store support show delete category -- default true, will add to storeparam table.
                // for seo, 
                if (true && _currentProductcategory == null)
                {
                    _currentProductcategory = eStore.Presentation.eStoreContext.Current.Store.getProductCateogryWithNonFilterForCategoryPathSEO(_category, true);
                }

                if (true && _currentProductcategory == null)
                {
                    _currentProductcategory = eStore.Presentation.eStoreContext.Current.Store.getProductCateogryWithNonFilter(_category, true);
                }

                if (_currentProductcategory != null)
                {
                    int abgoroup = 0;
                    HttpCookie abtest = HttpContext.Current.Request.Cookies["AbGroup"];
                    if (abtest != null)
                        int.TryParse(abtest.Value, out abgoroup);
                    // ab test for cateogry
                    if (_currentProductcategory.DisplayTypeX == POCOS.ProductCategory.RenderStyle.CategoryList && _currentProductcategory.childCategoriesX != null
                        && _currentProductcategory.childCategoriesX.Any() && eStore.Presentation.eStoreContext.Current.getBooleanSetting("EableAbTest") && abgoroup == 1)
                    {
                        Modules.V4.CategoryV2 v2 = this.LoadControl("~/Modules/V4/CategoryV2.ascx") as Modules.V4.CategoryV2;
                        v2.CurrentCategory = _currentProductcategory;
                        phContext.Controls.Add(v2);
                    }
                    else
                    {
                        Modules.V4.CategoryV4 v4 = this.LoadControl("~/Modules/V4/CategoryV4.ascx") as Modules.V4.CategoryV4;
                        v4.CurrentCategory = _currentProductcategory;
                        phContext.Controls.Add(v4);
                    }

                    if (Presentation.eStoreContext.Current.getBooleanSetting("TutorialEnabled"))
                    {
                        this.BindScript("url", "eStoreTutorial.js", "v4/eStoreTutorial.js");
                        this.AddStyleSheet(ResolveUrl("~/Styles/eStoreTutorial.css"));
                        string eStoreTutorial = "$(function () {InitTutorial(\"Category\", \"" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID + "\", \"category-tutorial.txt\", \".eStore_category_link\", \"\", \"\"); });";
                        this.BindScript("Script", "eStoreTutorial_Home", eStoreTutorial, true);
                    }
                    BindCategoryBaseInfor(_currentProductcategory);
                    Presentation.eStoreContext.Current.BusinessGroup = _currentProductcategory.businessGroup;

                    this.YouAreHere1.productCategory = _currentProductcategory;
                    this.isExistsPageMeta = setPageMeta(_currentProductcategory.pageTitle, _currentProductcategory.metaData, _currentProductcategory.keywords);
                    setCanonicalPage(Presentation.UrlRewriting.MappingUrl.getMappingUrl(_currentProductcategory));
                    //20160809 Alex:add ProductCategory page structured date 
                    eStore.Presentation.StructuredDataMarkup structuredDataMarkup = new eStore.Presentation.StructuredDataMarkup();
                    structuredDataMarkup.GenerateProudctCategoryStruturedData(_currentProductcategory, this.Page);
                    structuredDataMarkup.GenerateBreadcrumbStruturedData(_currentProductcategory, this.Page);
                    structuredDataMarkup.GenerateLPSections(_currentProductcategory, this.Page);
                    ltHtml.Text = _currentProductcategory.HtmlContent;
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", _currentProductcategory.CategoryPath);

                }
                else
                {
                    if (Session["fromStore"] != null)
                    {
                        string formStoreId = Session["fromStore"].ToString();
                        var protemp = Presentation.eStoreContext.Current.Store.GetSearchDiffSiteCategory(formStoreId, eStore.Presentation.eStoreContext.Current.Store.storeID, _category);
                        if (protemp.Any())
                        {
                            _currentProductcategory = new POCOS.ProductCategory { CategoryName = eStore.Presentation.eStoreLocalization.Tanslation("Product_Category_Search_result") };
                            foreach (var item in protemp)
                                _currentProductcategory.AddCategoryX(item);
                            Modules.V4.CategoryV2 v2 = this.LoadControl("~/Modules/V4/CategoryV2.ascx") as Modules.V4.CategoryV2;
                            v2.CurrentCategory = _currentProductcategory;
                            phContext.Controls.Add(v2);
                            BindCategoryBaseInfor(_currentProductcategory);
                        }
                        else
                            Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true, 404);
                    }
                    else
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true, 404);
                }
            }
        }

        /// <summary>
        /// bind category base information
        /// </summary>
        /// <param name="_currentProductcategory"></param>
        protected void BindCategoryBaseInfor(POCOS.ProductCategory _currentProductcategory)
        {
            this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(_currentProductcategory);
            if ("delete".Equals(_currentProductcategory.CategoryStatus, StringComparison.OrdinalIgnoreCase))
            {
                this.lCategoryName.Text += $"<i class='delete'>Has Deleted</i>";
            }
            this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(_currentProductcategory);

            List<eStore.POCOS.Advertisement> ads = eStore.Presentation.eStoreContext.Current.Store.sliderBanner(_currentProductcategory, false, true);
            if (ads != null && ads.Any(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite))
            {
                eStore.POCOS.Advertisement ad = ads.First(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite);
                if (!string.IsNullOrEmpty(ad.Imagefile))
                {
                    BannerImage = string.Format("url({0})", esUtilities.CommonHelper.ResolveUrl("~/resource" + ad.Imagefile));
                }
                else
                { BannerImage = string.Format("url({0})", esUtilities.CommonHelper.ResolveUrl("~/App_Themes/V4/images/eStore_BG_category_banner.jpg")); }
            }
            else
            {
                BannerImage = string.Format("url({0})", esUtilities.CommonHelper.ResolveUrl("~/App_Themes/V4/images/eStore_BG_category_banner.jpg"));
            }

            List<POCOS.Advertisement> adsf = Presentation.eStoreContext.Current.Store.sliderBanner(_currentProductcategory, true);
            if (adsf != null && adsf.Any(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite))
            {
                rpBanners.DataSource = adsf.Where(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite).ToList();
                rpBanners.DataBind();
                hasfullsizebanner = true;
            }

        }

        protected bool hasfullsizebanner = false;


    }
}