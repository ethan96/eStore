using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;

namespace eStore.Presentation.eStoreControls
{
    public abstract  class eStoreBaseControl  :  System.Web.UI.WebControls.WebControl 
    {
      
        protected override void OnPreRender(EventArgs e)
        {
            string id = this.ID;
            string pagename = this.Page.GetType().Name;

            this.Enabled  = HttpContext.Current.User.IsInRole("Customer");
            base.OnPreRender(e);
        }
    }
}
