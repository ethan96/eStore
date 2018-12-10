using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class Footer : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<POCOS.Menu> ls = Presentation.eStoreContext.Current.Store.getFooterLinks(Presentation.eStoreContext.Current.MiniSite)
                .Take(eStore.Presentation.eStoreContext.Current.Store.storeID == "ABB" ? 4 : 3).ToList();
            System.Text.StringBuilder sbfooter = new System.Text.StringBuilder();
            int index = 0;
            sbfooter.Append("<div class=\"eStore_footerLinks\">");
          
            foreach (POCOS.Menu menu in ls)
            {
             
                if (index % 2 == 0)
                {
                    sbfooter.Append("<div class=\"eStore_col_S\">");
                }
                else
                {
                    sbfooter.Append("<div class=\"eStore_col_B\">");
                }
                index++;
                sbfooter.AppendFormat("<h3>{0}</h3>", menu.MenuName);
                sbfooter.Append("<ol>");
                foreach (POCOS.Menu sm in menu.subMenusX.OrderBy(x=>x.Sequence))
                {
                    sbfooter.AppendFormat("<li><a href=\"{0}\">{1}</a></li>",
                ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(sm)),
                    sm.MenuName
                        );
                }
                sbfooter.Append("</ol>");
                sbfooter.Append("</div>");
            }

          
            sbfooter.Append("</div>");

            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //{
            //    sbfooter.Append("<div class=\"eStore_footerLinks\">Advantech Europe B.V. | Science Park Eindhoven 5708 | 5692 ER Son en Breugel | The Netherlands | <br> Toll Free: 00800-2426-8081 | Email us: customercare@advantech.eu</div>");
            //}


            lFooter.Text = sbfooter.ToString();
            
                
            if (Presentation.eStoreContext.Current.getBooleanSetting("OpenMarket", true))
            {
                rpStores.DataSource = BusinessModules.StoreSolution.getInstance().stores.OrderBy(x=>x.RegionName);
                rpStores.DataBind();
                rpStores.Visible = true;
            }
            else
            {
                rpStores.Visible = false;
            }
            if (Presentation.eStoreContext.Current.getBooleanSetting("ShowBottomPolicy", false))
            {
                lPolicyInformation.Text = eStore.Presentation.eStoreLocalization.Tanslation("eStoreBottomPolicyInformation");
            }

        }
        protected void rpStores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChangeStore")
            {
                BusinessModules.Store storex = BusinessModules.StoreSolution.getInstance().getStore(e.CommandArgument.ToString());
                Presentation.eStoreContext.Current.CurrentCountry = storex.profile.Countries.FirstOrDefault(x => x.CountryName == storex.profile.DefaultCountry);
            }
        }

        protected string TypeFormID()
        {
            string typeFormID = Presentation.eStoreContext.Current.getStringSetting("TypeForm");
            if (!string.IsNullOrEmpty(typeFormID))
            {
                return typeFormID;
                //StringBuilder sbTypeFormLink = new StringBuilder();
                
                // Add GTM
                //sbTypeFormLink.AppendFormat("<a class=\"typeform - share link\" href=\"https://estore.typeform.com/to/{0}\"",typeFormID);
                //sbTypeFormLink.Append("data-mode=\"2\" target=\"_blank\">Gives Feedback</a>");
                //this.questionnaire.Text = sbTypeFormLink.ToString();

                // register JS
                //String csname = "PoppupScript";
                //Type cstype = this.GetType();

                //ClientScriptManager cs = Page.ClientScript;

                //if (!cs.IsStartupScriptRegistered(cstype, csname))
                //{
                //    StringBuilder sbTypeFormScript = new StringBuilder();


                //    sbTypeFormScript.Append("<script>(function(){ var qs, js, q, s, d = document, gi = d.getElementById, ce = d.createElement, gt = d.getElementsByTagName, id = 'typef_orm',");
                //    sbTypeFormScript.Append("b = 'https://s3-eu-west-1.amazonaws.com/share.typeform.com/';");
                //    sbTypeFormScript.Append(" if (!gi.call(d, id)) { js = ce.call(d, 'script'); js.id = id; js.src = b + 'share.js'; q = gt.call(d, 'script')[0]; q.parentNode.insertBefore(js, q)} })()</script>");

                //    cs.RegisterStartupScript(cstype, csname, sbTypeFormScript.ToString());
                //}
                   

                //Generate JS Code in HTML
                //HtmlGenericControl con = new HtmlGenericControl();
                //con.TagName = "script";
                //con.Attributes.Add("type", "text/javascript");
                //con.InnerHtml = sbTypeForm.ToString();
                //Page.Header.Controls.Add(con);
                
            }
            return null;

        }

        protected void rpStores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Image logoImage = (Image)e.Item.FindControl("logoImage");
                logoImage.ImageUrl = Presentation.eStoreContext.Current.getStringSetting("StoreLogoSUrl", "/images/eStore_logoS.png");
            }
            
        }
    }
}