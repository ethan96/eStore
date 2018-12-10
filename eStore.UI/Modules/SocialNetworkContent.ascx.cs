using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class SocialNetworkContent : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected override void OnPreRender(EventArgs e)
        {
            string supportedSocialNetwork = eStore.Presentation.eStoreContext.Current.getStringSetting("SocialNetwork");
            if (string.IsNullOrEmpty(supportedSocialNetwork) == false)
            {
                StringBuilder sbSocialNetworks = new StringBuilder();
                string[] socialNetworks = supportedSocialNetwork.Split(',');

                foreach (string sn in socialNetworks)
                {
                    switch (sn.ToLower())
                    {
                        case "facebook":
                            sbSocialNetworks.Append(" <fb:like send=\"false\" layout=\"button_count\" width=\"90\" show_faces=\"false\" font=\"arial\"></fb:like> ");
                            break;
                        case "google":
                            sbSocialNetworks.Append(" <g:plusone size=\"medium\" count=\"false\"></g:plusone> ");
                            break;
                        case "twitter":
                            sbSocialNetworks.Append(" <a href=\"http://twitter.com/share\" class=\"twitter-share-button\" data-count=\"none\" data-via=\"advantechestore\"></a> ");
                            break;
                        default:
                            break;
                    }

                }
                if (sbSocialNetworks.Length > 0)
                {
                    lContent.Text = sbSocialNetworks.ToString();
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }
            base.OnPreRender(e);
        }
    }
}