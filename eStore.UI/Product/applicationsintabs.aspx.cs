using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Product
{
    public partial class applicationsintabs : Presentation.eStoreBaseControls.eStoreBasePage
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
                if (productCategory != null && productCategory is POCOS.Application && productCategory.parentCategoryX == null)
                {
                    bindData(productCategory);
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", productCategory.CategoryPath);
                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);

                    AddStyleSheet(ResolveUrl("~/Styles/ustore.css"));
                    AddStyleSheet(ResolveUrl("~/Styles/uStoreApplication.css"));
                }
            }
        }
        protected StringBuilder sbTabs = new StringBuilder();
        private void bindData(POCOS.ProductCategory productCategory)
        {
            List<POCOS.Application> allApps = Presentation.eStoreContext.Current.Store.getTopLevelApplications(Presentation.eStoreContext.Current.MiniSite);
            //create tabs
        

            sbTabs.AppendFormat("<div id=\"categoriestabs\">");
            sbTabs.AppendFormat("<ul>");
            string selected = "";
            foreach (POCOS.Application app in allApps)
            {
                //active tab
                if (app.CategoryID == productCategory.CategoryID)
                {
                    //bind products
                    lTitle.Text = app.LocalCategoryName;
                    imgApp.ImageUrl = string.Format("{0}/Application/{1}", Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString(), app.BackgroundImage);
                    rpCategories.DataSource = app.scenarioCategoriesX;
                    rpCategories.DataBind();
                    selected = " class=\"select\"";
                }
                else
                {
                    selected = "";
                }
                sbTabs.AppendFormat("<li{1}><a href=\"{0}\" target=\"_self\">{2}</a></li>"
                    ,ResolveUrl( Presentation.UrlRewriting.MappingUrl.getMappingUrl(app))
                    , selected
                    , app.LocalCategoryName
                    );
            }

            sbTabs.AppendFormat("</ul>");
            sbTabs.AppendFormat("</div>");

            
        }

        protected void rpCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rpModels = e.Item.FindControl("rpModels") as Repeater;

                POCOS.ScenarioCategory child = e.Item.DataItem as POCOS.ScenarioCategory;
                var datasource = (from prod in child.productList
                                  group prod by
                                 string.IsNullOrEmpty(prod.ModelNo) ? prod.name : prod.ModelNo into g
                                  select new
                                  {
                                      ModelNO = g.Key,
                                      URL = (g.Count() == 1 ? ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(g.First())) : "~/Compare.aspx?parts=" + string.Join(",", g.Select(x => x.SProductID).ToArray()))
                                      
                                  }).ToList();

                rpModels.DataSource = datasource;
                rpModels.DataBind();

            }
        }
    }
}