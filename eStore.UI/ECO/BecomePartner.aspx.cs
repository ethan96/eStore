using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.ECO
{
    public partial class BecomePartner : Presentation.eStoreBaseControls.eStoreBasePage
    {

        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AddStyleSheet(ResolveUrl("~/Styles/iServicesHomepage.css"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}