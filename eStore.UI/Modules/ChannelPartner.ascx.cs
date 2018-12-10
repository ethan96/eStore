using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class ChannelPartner : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindScript("url", "jquery1-4", "jquery-1.4.1.min.js");
                if (Presentation.eStoreContext.Current.Order != null ||
                    (Request.QueryString["isQuoteToOrder"] != null && Request.QueryString["isQuoteToOrder"] == "true"))
                {
                    if (Request.QueryString["countryName"] != null)
                        BindCeSelector(Request.QueryString["countryName"]);
                    //bindFonts();
                }
            }            
        }
        //Bing ceSelector
        private void BindCeSelector(string countryName)
        {
            StringBuilder content = new StringBuilder();
            List<POCOS.PocoX.ChannelPartner> channelPartners = Presentation.eStoreContext.Current.Store.getAvailableChannelPartners(countryName, Presentation.eStoreContext.Current.UserShoppingCart);
            foreach (POCOS.PocoX.ChannelPartner channel in channelPartners)
            {
                //channel.Company + ";" + channel.City + ";" + channel.Country
                content.AppendFormat("<input type='radio' name='cpSelector' value=\"{0}\" title=\"{1}\">{1} <br> ", channel.Channelid, channel.Company);
            }
            ltRadio.Text = content.ToString();
        }

        //protected void bindFonts()
        //{
        //    btnSelectCp.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select);
        //}
    }
}