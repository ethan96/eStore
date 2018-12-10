using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class iautomation : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rpPromotionProduct.DataSource = Presentation.eStoreContext.Current.Store.getPromotionProducts(Presentation.eStoreContext.Current.MiniSite, 4);
            rpPromotionProduct.DataBind();
            Presentation.eStoreContext.Current.keywords.Add("GoldEnggs", "true");

            eStore.Presentation.OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new eStore.Presentation.OpenGraphProtocolAdapter("HomePage");
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);
        }

        protected void rpPromotionProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hlcategory = e.Item.FindControl("hlcategory") as HyperLink ;
                POCOS.Product prod = e.Item.DataItem as POCOS.Product;
                if (prod != null && prod.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).Any())
                {
                    hlcategory.Text = prod.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).First().LocalCategoryName;
                    hlcategory.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod.productCategories.First());
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