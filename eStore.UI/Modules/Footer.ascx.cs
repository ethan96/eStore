using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class Footer : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindTranslationFonts();
            bindImage();
            this.rpLanding.DataSource = Presentation.eStoreContext.Current.Store.getLandingPages(Presentation.eStoreContext.Current.MiniSite);
            this.rpLanding.DataBind();

            List<POCOS.Menu> ls = Presentation.eStoreContext.Current.Store.getFooterLinks(Presentation.eStoreContext.Current.MiniSite).Take(3).ToList();
            if (ls != null)
            {
                if (ls.Count < 3)
                    for (int i = 3 - ls.Count; i > 0; i--)
                        ls.Add(null);
                FooterContent3.FoodMemu = ls[2];
                FooterContent2.FoodMemu = ls[1];
                FooterContent1.FoodMemu = ls[0];
            }
            if (Presentation.eStoreContext.Current.Store.storeID == "AJP")
            {
                this.footerchat.Visible = false;
                this.hlJPContact.Visible = true;
                this.hlJPContact.ImageUrl = "/images/ajp/Freecall.jpg";
                this.hlJPContact.NavigateUrl = "~/ContactUS.aspx?tabs=general-inquiries";
            }
            else
            {
                this.footerchat.Visible = true;
                this.hlJPContact.Visible = false;
            } 

        }

        /// <summary>
        /// Bind Translation Fonts
        /// </summary>
        private void BindTranslationFonts()
        {
            Literal_Feedback.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Website_User_Feedback);
            Literal_Copyright.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Website_Copyright);
            ltLiveHelpTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Footer_LiveHelpTitle);
            ltFooterCallme.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Footer_footerCallme);
        }

        void bindImage()
        {
            imgLiveHelp.ImageUrl = string.Format("/images/{0}/img01.gif", Presentation.eStoreContext.Current.Store.storeID);
        }
    }
}