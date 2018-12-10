using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.Product;

namespace eStore.UI.Product
{
    public partial class Config : Presentation.eStoreBaseControls.eStoreBasePage
    {
        IList<POCOS.Product> compareProducts { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            }
            else
            {
                string _category = Request["category"].ToString();
                //List<POCOS.Product> products = new List<POCOS.Product>();
                //POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProducts(_category, out products);
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                if (productCategory != null)
                {
                    //set user activit log
                    this.UserActivitLog.CategoryID = productCategory.CategoryID.ToString();
                    this.UserActivitLog.CategoryName = productCategory.CategoryName;
                    this.UserActivitLog.CategoryType = productCategory.CategoryType;

                    this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                    this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
                    this.YouAreHere1.productCategory = productCategory;
                    Presentation.eStoreContext.Current.BusinessGroup = productCategory.businessGroup;
                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);
                    this.hPrint.NavigateUrl = "/Product/PrintComparisonTable.aspx?category=" + Request["category"].ToString();
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);

                    eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(productCategory);
                    //if root category and have slider banners, then hide you are here
                    if (productCategory.parentCategoryX == null)
                        YouAreHere1.Visible = eStoreLiquidSlider1.Advertisements.Any() == false;
                }
                else
                {
                    this.UserActivitLog.CategoryName = _category;
                    this.UserActivitLog.CategoryType = "ErrorCategory";
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
                }
             
            }
        }

      
    }
}