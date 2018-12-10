using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules.V4
{
    public partial class HomeMedialContent : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected string sort = eStore.Presentation.eStoreContext.Current.getBooleanSetting("isSortTodayHighLight", false) ? "?sort=true" : ""; // today hight light sort
        protected void Page_Load(object sender, EventArgs e)
        {

            ///show theme banner
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EableThemeBanner"))
            {
                var themebanners = eStore.Presentation.eStoreContext.Current.Store.GetKitThemeLs();
                rpThemeBanners.DataSource = themebanners;
                rpThemeBanners.DataBind();
                rpThemeBanners.Visible = themebanners.Any();
            }

            System.Text.StringBuilder sbCategories = new System.Text.StringBuilder();
            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);
            sbCategories.Append("<div id=\"tab-PRODUCTS\" class=\"eStore_index_Highlight_tablink\"  style=\"display: block;\">");
            foreach (POCOS.ProductCategory sp in spc)
            {
                sbCategories.AppendFormat("<a id=\"" + sp.CategoryID + "\" data-cid=\"" + sp.CategoryID + "\"  data-tabname=\"PRODUCTS\"    href=\"{0}\" ref=\"{1}\">{2}</a><div class=\"eStore_index_proContent ctrl" + sp.CategoryID + "\" ></div>"
                   , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(sp))
                   , sp.CategoryPath
                   , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sp)
                    );
            }
            sbCategories.Append("</div>");
            List<POCOS.ProductCategory> cc = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);
            sbCategories.Append("<div id=\"tab-SYSTEMS\" class=\"eStore_index_Highlight_tablink\" >");
            foreach (POCOS.ProductCategory sp in cc)
            {
                sbCategories.AppendFormat("<a id=\"" + sp.CategoryID + "\"data-cid=\"" + sp.CategoryID + "\"  data-tabname=\"SYSTEMS\"   href=\"{0}\" ref=\"{1}\">{2}</a><div class=\"eStore_index_proContent ctrl" + sp.CategoryID + " \"></div>"
                   , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(sp))
                   , sp.CategoryPath
                   , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sp)
                    );
            }
            sbCategories.Append("</div>");
            lCategories.Text = sbCategories.ToString();
            IList<POCOS.Solution> sulotions = eStoreContext.Current.Store.getAllSolution().Where(s => s.PublishStatus == true).ToList();
            showMoreLink = sulotions.Count() > 12;
            rpSolutions.DataSource = sulotions.Take(12);
            rpSolutions.DataBind();

            rpSolutionsMobil.DataSource = sulotions;
            rpSolutionsMobil.DataBind();
        }
        private bool showMoreLink = false;
        protected void rpSolutions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (showMoreLink == false)
                {
                    e.Item.Controls.Clear();
                }

            }
        }
    }
}