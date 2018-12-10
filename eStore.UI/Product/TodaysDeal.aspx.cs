using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Product
{
    public partial class TodaysDeal : eStore.Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<POCOS.Product> lsPromotionProducts = new List<POCOS.Product>();
            lsPromotionProducts = eStore.Presentation.eStoreContext.Current.Store.getPromotionProducts(eStore.Presentation.eStoreContext.Current.MiniSite,12);
            rpPromotionProduct.DataSource = lsPromotionProducts;
            rpPromotionProduct.DataBind();

            List<POCOS.Product> lsClearanceProducts = eStore.Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.CLEARANCE, eStore.Presentation.eStoreContext.Current.MiniSite,12).ToList();
            rpClearanceProduct.DataSource = lsClearanceProducts;
            rpClearanceProduct.DataBind();

            List<Campaign> lsCampaigns = eStore.Presentation.eStoreContext.Current.Store.getTodayPromotionCampaigns();
            rpCampaigns.DataSource = lsCampaigns;
            rpCampaigns.DataBind();

            Dictionary<string, string> keywrods = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            keywrods.Add("HomePage", "true");
            IList<POCOS.Advertisement> adsList = eStore.Presentation.eStoreContext.Current.Store.getAdBanner(eStore.Presentation.eStoreContext.Current.MiniSite, keywrods);

            dlHomeBanner.DataSource = adsList;
            dlHomeBanner.DataBind();
        }

        protected void rpCampaigns_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.Campaign c = e.Item.DataItem as POCOS.Campaign;
                Literal ltStartDate = e.Item.FindControl("ltStartDate") as Literal;
                Literal ltEndDate = e.Item.FindControl("ltEndDate") as Literal;
                ltStartDate.Text = string.Format("Start: {0}", c.EffectiveDate.ToShortDateString());
                ltEndDate.Text = string.Format("End: {0}", c.ExpiredDate.ToShortDateString());
                if (!string.IsNullOrEmpty(c.PromotionCode))
                {
                    Literal ltPromotionCode = e.Item.FindControl("ltPromotionCode") as Literal;
                    ltPromotionCode.Text = string.Format("Code: <span class='textCenter colorRed'>{0}</span>", c.PromotionCode);
                }
            }
        }

        protected void dlHomeBanner_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.Advertisement advItem = e.Item.DataItem as POCOS.Advertisement;
                HyperLink hlHomeBanner = e.Item.FindControl("hlHomeBanner") as HyperLink;
                Image imgHomeBanner = e.Item.FindControl("imgHomeBanner") as Image;

                hlHomeBanner.NavigateUrl = esUtilities.CommonHelper.ConvertToAppVirtualPath(advItem.Hyperlink);
                hlHomeBanner.Target = string.IsNullOrEmpty(advItem.Target) ? "_self" : advItem.Target;

                imgHomeBanner.ImageUrl = advItem.Imagefile.StartsWith("http", true, null) ? advItem.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), advItem.Imagefile);
            }
        }
    }
}