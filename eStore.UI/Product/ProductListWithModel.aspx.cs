using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class ProductListWithModel : Presentation.eStoreBaseControls.eStoreBasePage
    {
       
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
                    lTitle.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                    rpCategories.DataSource = productCategory.childCategoriesX;
                    rpCategories.DataBind();

                    eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(productCategory);
                    this.YouAreHere1.productCategory = productCategory;
                    this.YouAreHere1.ProductName = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", productCategory.CategoryPath);
                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);

                    AddStyleSheet(ResolveUrl("~/Styles/ustore.css"));
                    AddStyleSheet(ResolveUrl("~/Styles/uStoreApplication.css"));
                    BindScript("url", "jquery.sBubble", "jquery.sBubble-0.1.1.js");
                    BindScript("url", "uStoreApplication", "uStoreApplication.js");

                }
            }

        }

        protected void rpCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rpModels = e.Item.FindControl("rpModels") as Repeater;

                POCOS.ProductCategory child = e.Item.DataItem as POCOS.ProductCategory;
                var datasource = (from prod in child.productList
                                  group prod by
                                 string.IsNullOrEmpty(prod.ModelNo) ? prod.name : prod.ModelNo into g
                                  select new {
                                      ModelNO = g.Key,
                                      URL = (g.Count() == 1 ? ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(g.First())) : "~/Compare.aspx?parts=" + string.Join(",", g.Select(x => x.SProductID).ToArray()))
                                 
                                  }).ToList();

                rpModels.DataSource = datasource;
                rpModels.DataBind();

            }
        }
    }
}