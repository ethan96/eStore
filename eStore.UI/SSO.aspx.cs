using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class SSO : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string callbackurl = "~/";
            if (!string.IsNullOrEmpty(Request["tempid"]) && !string.IsNullOrEmpty(Request["id"]))
            {
                Presentation.eStoreUserAccount.TrySSOSignIn(Presentation.eStoreContext.Current.getUserIP(), Request["tempid"], Request["id"]);
            }

            if (Presentation.eStoreContext.Current.User != null)
            {
                if (!string.IsNullOrEmpty(Request["callbackurl"]))
                {
                    callbackurl = Request["callbackurl"];
                    if (callbackurl.StartsWith("_autoOrder"))
                        callbackurl = "~/Account/orderdetail.aspx?orderid=" + callbackurl.Replace("_autoOrder", "");
                }
                else
                { callbackurl = "~/"; }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request["callbackurl"]))
                    callbackurl = Request["callbackurl"];
                else
                { callbackurl = "~/default.aspx"; }

                if (callbackurl.IndexOf("?") > 0)
                    callbackurl = callbackurl + "&needlogin=true";
                else
                    callbackurl = callbackurl + "?needlogin=true";
            }
            Response.Redirect(callbackurl);

        }
    }
}