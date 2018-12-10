using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Principal;

namespace eStore.UI.Account
{
    public partial class Login : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                Control header = this.Master.FindControl("eStoreHeader");
                if (header != null)
                {
                    Control HeadLoginView =  header.FindControl("HeadLoginView");

                    if (HeadLoginView != null)
                    {
                        Control popLogin =  header.FindControl("UserLogin1");
                        if (popLogin != null)

                            popLogin.Visible = false;
                    }
                }
            }
            catch { }
        }
    
    }
}
