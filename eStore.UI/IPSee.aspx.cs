using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class IPSee : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ip = Presentation.eStoreContext.Current.getUserIP();
            ltIp.Text = ip;

            var isLocalIp = esUtilities.IPUtility.IpIsWithinBoliviaRange(ip,
                            eStore.Presentation.eStoreContext.Current.Store.profile.LocationIps.Select(c => c.IPAtrrs).ToList());
            ltArea.Text = isLocalIp ? "Is Location ip" : "Is not Location ip";
        }
    }
}