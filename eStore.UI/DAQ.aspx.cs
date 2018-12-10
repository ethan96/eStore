using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class DAQ : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "jquery.iframe-auto-height.plugin.1.9.3.min.js", "jquery.iframe-auto-height.plugin.1.9.3.min.js");
        }
    }
}