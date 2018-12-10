using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eStore.Presentation.eStoreControls
{
    public class GridView :  System.Web.UI.WebControls.GridView 
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (!HttpContext.Current.User.IsInRole("Customer"))
            { 
                this.Columns[0].Visible = false;
            }
         
            base.OnPreRender(e);
        }
    }
}
