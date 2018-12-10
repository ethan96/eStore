using eStore.Presentation;
using eStore.Presentation.eStoreBaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class Bundle : BaseProduct
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            this.BundleProductContent.CurrentBundleProduct = getProduct();

            if (!IsPostBack)
            {
                //2017/02/23 test  for ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureProductDetail", GoogleGAHelper.MeasureProductDetail(getProduct()).ToString(), true);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureAddToCart", GoogleGAHelper.MeasureAddToCart(getProduct()).ToString(), true);

            }

            //20160809 Alex:add bundle page structured date 
            //eStore.Presentation.StructuredDataMarkup structuredDataMarkup = new eStore.Presentation.StructuredDataMarkup();
            //structuredDataMarkup.GenerateProductStruturedData(this.BundleProductContent.CurrentBundleProduct, this.Page);
            //structuredDataMarkup.GenerateBreadcrumbStruturedData(this.BundleProductContent.CurrentBundleProduct, this.Page);
        }

        private POCOS.Product_Bundle getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(ProductId, true);
            base.CurrProduct = product;
            base.ProductNo = ProductId;
            if (product == null)
            {
                this.UserActivitLog.ProductID = ProductId;
                this.UserActivitLog.CategoryType = "ErrorBundle";
                base.ToSearch();
            }
            if (product != null && !IsPostBack)
            {
                this.UserActivitLog.ProductID = product.SProductID;
                this.UserActivitLog.CategoryType = product.productType.ToString();
                setCanonicalPage(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product));
            }
            if (product is POCOS.Product_Bundle)
                return (POCOS.Product_Bundle)product;
            else
                return null;
        }
    }
}