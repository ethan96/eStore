using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class Shortcuts : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string name = Request["name"];
            if (string.IsNullOrEmpty(name))
            {
                Response.Redirect("~/");
            }
            else
            {
                POCOS.Shortcut shortcut = Presentation.eStoreContext.Current.Store.profile.shortcuts.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (shortcut == null || string.IsNullOrEmpty(shortcut.Link))
                {
                    Response.Redirect("~/");
                }
                else
                {
                    Response.Redirect(shortcut.Link);
                }
            }
        }
    }
}