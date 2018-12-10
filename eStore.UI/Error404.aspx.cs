using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class Error404 : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Status = "404 Not Found";
        }
    }
}