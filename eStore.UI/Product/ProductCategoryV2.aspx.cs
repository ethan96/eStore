using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class ProductCategoryV2 : eStore.Presentation.eStoreBaseControls.eStoreBasePage 
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            AddStyleSheet(ResolveUrl("~/Styles/MedicalDefault.css"));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product category is not available", null, true);
            }
            else
            {
                string _category = Request["category"].ToString();
                var cate = eStore.Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                CategoryRepeater1.DataSource = cate.childCategoriesX.ToList();
                CategoryRepeater1.DataBind();

                ltCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryDescription(cate,eStore.Presentation.eStoreContext.Current.CurrentLanguage);
                ltCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(cate, eStore.Presentation.eStoreContext.Current.CurrentLanguage);

                Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);
                Presentation.eStoreContext.Current.BusinessGroup = cate.businessGroup;
                this.isExistsPageMeta = setPageMeta(cate.pageTitle, cate.metaData, cate.keywords);
                eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(cate);
                this.ProductCategory1.productCategory = cate;
               
            }

            
        }
    }
}