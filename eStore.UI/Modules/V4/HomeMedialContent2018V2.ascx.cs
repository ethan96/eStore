using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class HomeMedialContent2018V2 : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
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



            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLevelStandardProductCategories(eStoreContext.Current.MiniSite);

            rpProducts.DataSource = spc.SelectMany(x => x.childCategoriesX);
            rpProducts.DataBind();
            rpProductscarousel.DataSource = spc.SelectMany(x => x.childCategoriesX).Select(x => new Models.TodaysHighlight(x));
            rpProductscarousel.DataBind();

 
            List<POCOS.ProductCategory> cc = eStoreContext.Current.Store.getTopLevelCTOSProductCategories(eStoreContext.Current.MiniSite);

            rpSystems.DataSource = cc.SelectMany(x=>x.childCategoriesX);
            rpSystems.DataBind();
            rpSystemscarousel.DataSource = cc.SelectMany(x => x.childCategoriesX).Select(x => new Models.TodaysHighlight(x));
            rpSystemscarousel.DataBind();
 

            IList<POCOS.Solution> sulotions = eStoreContext.Current.Store.getAllSolution().Where(s => s.PublishStatus == true).ToList();
            showMoreLink = sulotions.Count() > 12;
            rpSolutions.DataSource = sulotions.Take(12);
            rpSolutions.DataBind();
            if (!showMoreLink)
                rpSolutions.FooterTemplate = null;

            rpRootCategories.DataSource = spc.Union(cc).Take(10).Select(x => new Models.TodaysHighlight(x)).OrderBy(x => x.Name);
            rpRootCategories.DataBind();
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