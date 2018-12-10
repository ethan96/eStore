using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class IotModel : eStore.Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected string pageUrl = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "jquery.simplePagination", "jquery.simplePagination.js");
            AddStyleSheet(esUtilities.CommonHelper.GetStoreLocation() + "Styles/simplePagination.css");

            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");
            AddStyleSheet(esUtilities.CommonHelper.GetStoreLocation() + "Styles/IotMart/colorbox.css");

            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            else
            {
                string _category = Request["category"].ToString();
                var cate = eStore.Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                if (cate == null)
                    Presentation.eStoreContext.Current.AddStoreErrorCode("IoT Product category is not available", null, true);
                HotDeals1.Category = cate;
                HotDeals1.eHaveInfor += new EventHandler(hideHotDeals);
                IotProductList1.category = cate;

                pageUrl = ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(cate, eStore.Presentation.eStoreContext.Current.MiniSite));

                this.YouAreHere1.productCategory = cate;
                ltCategoryName.Text = cate.localCategoryNameX;
                ltCateDescription.Text = cate.descriptionX;
                if (cate.childCategoriesX.Any())
                {
                    rpSubCateogries.DataSource = cate.childCategoriesX;
                    rpSubCateogries.DataBind();
                }

                Presentation.eStoreContext.Current.keywords.Add("CategoryID", cate.CategoryPath);
                setPageMeta(cate.pageTitle, cate.PageDescription, cate.keywords);
                var sideadv = Presentation.eStoreContext.Current.Store.sliderBanner(cate).FirstOrDefault();
                if (sideadv != null)
                {
                    ltImageSideAdv.Text = string.Format("<a href=\"{1}\"{2}><img src=\"{0}\"></a>"
                        , esUtilities.CommonHelper.GetStoreLocation() + "resource" +sideadv.Imagefile, sideadv.Hyperlink,string.IsNullOrEmpty(sideadv.Target)?"":" target=\""+ sideadv.Target +"\"");
                }
            }
            if (!IsPostBack)
            {
                bindWords();
                if (eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop)
                    this.pnMostBuy.Visible = false;
            }

            
        }

        void bindWords()
        {
            hyVisiteNow.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_VisitNow);
            hyVisiteNow.NavigateUrl = "http://" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreURL;
        }

        public void hideHotDeals(Object sender, EventArgs e)
        {
            pnHotDeals.Visible = false;
        }
    }
}