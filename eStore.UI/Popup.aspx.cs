using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class Popup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["popupType"] != null)
                {
                    string popupType = Request.QueryString["popupType"];
                    switch (popupType)
                    {
                        case "ChannelPartnerType":
                            Modules.ChannelPartner channelPartner = (Modules.ChannelPartner)LoadControl("~/Modules/ChannelPartner.ascx");
                            phPopup.Controls.Add(channelPartner);
                            break;
                        default:
                            break;
                    }                    
                }               
            }
        }
    }
}