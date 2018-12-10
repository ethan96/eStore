using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class ChinaServicePartner : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liveperson1.LivePersonType = "livepersonACN";
            liveperson1.UserLargerImage = true;

            this.BindScript("url", "swfobject", "swfobject.js");

            rpPromotionProduct.DataSource = Presentation.eStoreContext.Current.Store.getPromotionProducts(Presentation.eStoreContext.Current.MiniSite, 4);
            rpPromotionProduct.DataBind();
        }

        protected void rpPromotionProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hlcategory = e.Item.FindControl("hlcategory") as HyperLink;
                POCOS.Product prod = e.Item.DataItem as POCOS.Product;
                if (prod != null && prod.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).Any())
                {
                    hlcategory.Text = prod.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).First().LocalCategoryName;
                    hlcategory.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).First());
                }
                else
                {
                    hlcategory.Text = prod.name;
                    hlcategory.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod);
                }

            }
        }
    }
}