using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.aboutus
{
    public partial class _default : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool isMobileFriendly
        {
            get
            {
                return this.aboutusWidget.isMobileFriendly;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}