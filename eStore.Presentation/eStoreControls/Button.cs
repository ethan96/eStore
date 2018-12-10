using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace eStore.Presentation.eStoreControls
{
    public class Button : System.Web.UI.WebControls.Button
    {
        protected override void OnPreRender(EventArgs e)
        {
            string id = this.ID;
            string pagename = this.Page.GetType().Name;
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {// this.Enabled = false ;
                this.Attributes.Add("class", "needlogin");
            }

            base.OnPreRender(e);
        }

    }
}
