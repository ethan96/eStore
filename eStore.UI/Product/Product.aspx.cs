using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;

namespace eStore.UI.Product
{
    public partial class Product : BaseProduct
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
        protected void Page_Load(object sender, EventArgs e)
        {
            POCOS.Product product = getProduct();
            if (product!=null)
            {
                base.CurrProduct = product;
                this.ProductContent.CurrentProduct = product;
                if (!IsPostBack)
                {
                    setPopularProduct();

                    //Add AJP APERZA tracking data 
                    if (!string.IsNullOrEmpty(Request["SiteID"]) && eStoreContext.Current.getBooleanSetting("Aperza", false) == true && !string.IsNullOrEmpty(eStoreContext.Current.SessionID))
                        eStoreContext.Current.Store.addAperza("Aperza", product.SProductID, eStoreContext.Current.SessionID, Request.Url.AbsoluteUri);

                    //2017/02/23 test add to cart for ehance Ecommerce
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureProductDetail", GoogleGAHelper.MeasureProductDetail(product).ToString(), true);
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureAddToCart", GoogleGAHelper.MeasureAddToCart(product).ToString(), true);
                }
                setCanonicalPage(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
                this.BlockSearchIndexing = product.notAvailable;
                if (Presentation.eStoreContext.Current.getBooleanSetting("TutorialEnabled"))
                {
                    this.BindScript("url", "eStoreTutorial.js", "v4/eStoreTutorial.js");
                    this.AddStyleSheet(ResolveUrl("~/Styles/eStoreTutorial.css"));
                    string eStoreTutorial = "$(function () { InitTutorial(\"Product\", \"" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID + "\", \"product-tutorial.txt\", \".eStore_product_productPic\", \"\", \"\"); });";
                    this.BindScript("Script", "eStoreTutorial_Home", eStoreTutorial, true);
                }
            }
        }

        private POCOS.Product getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            base.ProductNo = ProductId;
            //设置Popular Product id
            //if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
            //    AdPopularProduct1.SproductId = ProductId;

            POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(ProductId, true);
            if (product != null && !IsPostBack)
            {
                this.UserActivitLog.ProductID = product.SProductID;
                this.UserActivitLog.CategoryType = product.productType.ToString();
                this.UserActivitLog.CategoryName = product.pageTitle;
            }
            if(product == null)
            {
                this.UserActivitLog.ProductID = ProductId;
                this.UserActivitLog.CategoryType = "ErrorPart";
                //this.UserActivitLog.CategoryName = product.pageTitle;
                base.ToSearch();
            }
            return product;
        }

        //设置Popular log为点击的
        private void setPopularProduct()
        {
            if (Request.QueryString["SourceLog"] != null && eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
            {

                string sourceProduct = Request.QueryString["SourceLog"];
                string popularPorduct = this.ProductContent.CurrentProduct.SProductID;

                string userId = eStore.Presentation.eStoreContext.Current.User != null ? eStore.Presentation.eStoreContext.Current.User.UserID : "";

                //点击次数+1
                Presentation.eStoreContext.Current.Store.savePopularProductByClicks(Session.SessionID, userId, sourceProduct, popularPorduct);
                //暂时取消了 奇偶数显示的功能
                //设置点击 cookie
                //if (this.ProductContent.CurrentProduct != null)
                //    AdPopularProduct1.setPopularCookie(this.ProductContent.CurrentProduct.SProductID);
            }
        }
    }
}