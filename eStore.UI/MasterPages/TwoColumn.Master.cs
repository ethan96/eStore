using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;

namespace eStore.UI.MasterPages
{
    public partial class TwoColumn : eStoreBaseMasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            ServiceReference Service = eStoreScriptManager.Services.FirstOrDefault();
            if (Service != null)
                Service.Path = ResolveUrl("~/eStoreScripts.asmx");
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}