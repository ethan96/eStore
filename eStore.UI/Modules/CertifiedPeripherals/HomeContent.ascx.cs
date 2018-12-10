using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
 
using eStore.POCOS;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class HomeContent :  Presentation.eStoreBaseControls.eStoreBaseUserControl 

    {
        protected void Page_Load(object sender, EventArgs e)
        {

            eStore.BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);

            List<eStore.POCOS.StoreDeal.PromotionType> types = new List<eStore.POCOS.StoreDeal.PromotionType>();
            types.Add(eStore.POCOS.StoreDeal.PromotionType.BestSeller);
            types.Add(eStore.POCOS.StoreDeal.PromotionType.HottestDeals);
            types.Add(POCOS.StoreDeal.PromotionType.NewArrive);

            Dictionary<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>> promotionproducts = pstore.getPromotionPStoreProducts(types);

            this.rpPromotionPStoreProducts.DataSource = promotionproducts.OrderBy(x => x.Key);
            this.rpPromotionPStoreProducts.DataBind();
        }

        protected void rpPromotionPStoreProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ProductList pl = (ProductList)e.Item.FindControl("ProductList1");
                pl.Products = ((System.Collections.Generic.KeyValuePair<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>>)e.Item.DataItem).Value;
            }
        }
    }
}