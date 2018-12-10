using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class HomeMedialContent2016 : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (eStoreContext.Current.Store.storeID == "ABB")
            {
                HomeEnhancedService.Visible = true;
                system_block.Visible = false;
            }

            rpBanners.DataSource = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite, false);
            rpBanners.DataBind();
          
            System.Text.StringBuilder sbCategories = new System.Text.StringBuilder();


            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);

            rpProducts.DataSource = spc;
            rpProducts.DataBind();
            rpProductscarousel.DataSource = spc.Select(x => new Models.TodaysHighlight(x));
            rpProductscarousel.DataBind();

            sbCategories.Append("<div id=\"tab-PRODUCTS\" class=\"eStore_index_Highlight_tablink\"  style=\"display: block;\">");
            foreach (POCOS.ProductCategory sp in spc)
            {
                sbCategories.AppendFormat("<a href=\"{0}\" ref=\"{1}\">{2}</a><div class=\"eStore_index_proContent\"></div>"
                   , "#"//ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(sp))
                   , sp.CategoryPath
                   , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sp)
                    );
            }
            sbCategories.Append("</div>");
            List<POCOS.ProductCategory> cc = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);              

            rpSystems.DataSource = cc;
            rpSystems.DataBind();
            rpSystemscarousel.DataSource = cc.Select(x => new Models.TodaysHighlight(x));
            rpSystemscarousel.DataBind();

            sbCategories.Append("<div id=\"tab-SYSTEMS\" class=\"eStore_index_Highlight_tablink\" >");
            foreach (POCOS.ProductCategory sp in cc)
            {
                sbCategories.AppendFormat("<a href=\"{0}\" ref=\"{1}\">{2}</a><div class=\"eStore_index_proContent\"></div>"
                    , "#"//ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(sp))
                    , sp.CategoryPath
                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sp)
                    );
            }
            sbCategories.Append("</div>");
            

            IList<POCOS.Solution> sulotions = eStoreContext.Current.Store.getAllSolution().Where(s => s.PublishStatus == true).ToList();
            showMoreLink = sulotions.Count() > 12;
            rpSolutions.DataSource = sulotions.Take(12);
            rpSolutions.DataBind();
            if (!showMoreLink)
                rpSolutions.FooterTemplate = null;

        }
        private bool showMoreLink = false;
        protected void rpSolutions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Footer)
            {
                if (showMoreLink == false)
                {
                    //e.Item.Controls.Clear();
                }

            }
        }
    }
}