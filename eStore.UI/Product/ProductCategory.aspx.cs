using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class ProductCategory : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            BindData();
        }
        void BindData()
        {
            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {

                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true);

            }
            else
            {
                string _category = Request["category"].ToString();
                string keyword = this.ProductSpec1.keyword;
         
                POCOS.ProductCategory _productcategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);

                if (_productcategory != null)
                {
                    //set user activit log
                    this.UserActivitLog.CategoryID = _productcategory.CategoryID.ToString();
                    this.UserActivitLog.CategoryName = _productcategory.CategoryName;
                    this.UserActivitLog.CategoryType = _productcategory.CategoryType;

                    this.ProductCategoryList1.productCategory = _productcategory;
                    Presentation.eStoreContext.Current.BusinessGroup = _productcategory.businessGroup;

                    if (_productcategory.productList != null)
                    {
                        this.ProductList1.productList = _productcategory.productList;
                        if (_productcategory.childCategoriesX.Count == 0)
                            ProductList1.KeepOriginalSequence = true;
                        else
                            ProductList1.KeepOriginalSequence = false;
                    }
                    this.isExistsPageMeta = setPageMeta(_productcategory.pageTitle, _productcategory.metaData, _productcategory.keywords);

                }
                else
                {
                    this.UserActivitLog.CategoryName = _category;
                    this.UserActivitLog.CategoryType = "ErrorCategory";
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true);
                    return;
                }
                Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);
            }
        }
    }
}