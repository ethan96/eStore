using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class SpecialProducts : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                BindData();
            }

        }

        private void BindData()
        {
            List<POCOS.Product.PRODUCTMARKETINGSTATUS> ls = new List<POCOS.Product.PRODUCTMARKETINGSTATUS>()
            {
                POCOS.Product.PRODUCTMARKETINGSTATUS.NEW,
                POCOS.Product.PRODUCTMARKETINGSTATUS.HOT,
                POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION,
                POCOS.Product.PRODUCTMARKETINGSTATUS.CLEARANCE
            };
            IEnumerable<POCOS.Product> products = Presentation.eStoreContext.Current.Store.getProducts(ls, eStore.Presentation.eStoreContext.Current.MiniSite);


            var tabs = from tab in products
                       group tab by
                        new { tab.status }
                           into Tabs
                           select new
                           {
                               TabName = Tabs.Key.status.ToString(),
                               TabProducts = Tabs
                           };

            this.rpSpecialProductsTab.DataSource = tabs;
            this.rpSpecialProductsTab.DataBind();
            this.rpSpecialProductsItems.DataSource = tabs;
            this.rpSpecialProductsItems.DataBind();
        }
    }
}