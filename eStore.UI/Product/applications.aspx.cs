using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class applications : Presentation.eStoreBaseControls.eStoreBasePage
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
            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            }
            else
            {
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(Request["category"]);
                if (productCategory != null)
                {
                    this.YouAreHere1.productCategory = productCategory;
                    this.YouAreHere1.ProductName = productCategory.LocalCategoryName;
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", productCategory.CategoryPath);
                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);
                    AddStyleSheet(ResolveUrl("~/Styles/uStoreApplication.css"));
                    BindScript("url", "jquery.sBubble", "jquery.sBubble-0.1.1.js");
                    BindScript("url", "uStoreApplication", "uStoreApplication.js");

                }
            }

        }
    }
}