using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class Category : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
                BindData();
        }
        void BindData()
        {
            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            }
            else
            {
                string _category = Request["category"].ToString();
                //List<POCOS.Product> products = new List<POCOS.Product>();
                //POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProducts(_category, out products, false);
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                if (productCategory != null)
                {
                    //set user activit log
                    this.UserActivitLog.CategoryID = productCategory.CategoryID.ToString();
                    this.UserActivitLog.CategoryName = productCategory.CategoryName;
                    this.UserActivitLog.CategoryType = productCategory.CategoryType;

                    this.ProductCategoryList1.productCategory = productCategory;
                    this.ProductCategory1.productCategory = productCategory;                
                    Presentation.eStoreContext.Current.BusinessGroup = productCategory.businessGroup;
                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);
                    if (productCategory.CategoryID == 355 && productCategory.Storeid == "AUS")
                    {
                        this.phWidget.Visible = true;
                        Modules.Widget widget = new Modules.Widget();
                        widget.WidgetName = "appliedembeddedcomputing";
                        phWidget.Controls.Clear();
                        phWidget.Controls.Add(widget);
                    }
                }
                else
                {
                    this.UserActivitLog.CategoryName = _category;
                    this.UserActivitLog.CategoryType = "ErrorCategory";
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
                }
                 Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);
            }
        }

    }
}