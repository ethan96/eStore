using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class YouAreHereMutli : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<POCOS.ProductCategory> productCategories { get; set; }
        public string ProductName { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategories != null)
            {
                //this.rpMutliYouarehere.DataSource = productCategories;
                this.rpMutliYouarehere.DataSource =  productCategories.Take(Presentation.eStoreContext.Current.Store.maxYouAreHereLinks);
                this.rpMutliYouarehere.DataBind();
            }
            pnoneLink.Visible = (productCategories == null || productCategories.Count <= 0);
        }

        protected void rpMutliYouarehere_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                YouAreHere youarehere = (YouAreHere)e.Item.FindControl("YouAreHere1");
                youarehere.ProductName = this.ProductName;
                youarehere.productCategory =(POCOS.ProductCategory) e.Item.DataItem;
            }
        }
    }
}