using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
namespace eStore.Presentation.eStoreControls
{
    public class Menu: System.Web.UI.WebControls.Menu
    {

        protected override void OnPreRender(EventArgs e)
        {
            if (!HttpContext.Current.User.IsInRole("Customer"))
            {
                this.FindItem("Cart").Enabled = false;
            }

            base.OnPreRender(e);
        }
    }
}
