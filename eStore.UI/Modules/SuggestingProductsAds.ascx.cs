using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class SuggestingProductsAds : System.Web.UI.UserControl
    {
        public string type {

            get {
                return this.Page.GetType().Name;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Suggesting_Products")||
                eStore.Presentation.eStoreContext.Current.getBooleanSetting("Cart_Cross_Sell_Products"))
                this.Visible=true;
            else
                this.Visible=false;

        }
    }
}