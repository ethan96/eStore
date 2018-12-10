using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Cart
{
    public partial class Thankyou : eStoreBaseOrderPage
    {
        protected override eStoreBaseOrderPage.PageStep minStepNo
        {
            get
            {
                return PageStep.Finish;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            imgSiteId.ImageUrl = ResolveUrl("~/App_Themes/Default/arrow01.gif");
            this.OrderInvoiceDetail1.order = Presentation.eStoreContext.Current.Order;
            this.OrderInvoiceDetail1.showATP = false;
            if (!IsPostBack
                && !Presentation.eStoreContext.Current.isTestMode()
                && Presentation.eStoreContext.Current.User.actingUser.role == POCOS.User.Role.Customer)
            {
                this.AddGoogleECommerceTracking(Presentation.eStoreContext.Current.Order);

                //2017/02/23 test ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureCheckout", GoogleGAHelper.MeasureCheckout(Presentation.eStoreContext.Current.Order,4).ToString(), true);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasurePurchases", GoogleGAHelper.MeasurePurchases(Presentation.eStoreContext.Current.Order).ToString(), true);
               
                String GoogleOrderTracking = Presentation.eStoreContext.Current.getStringSetting("GoogleOrderConversion");
                if (!String.IsNullOrWhiteSpace(GoogleOrderTracking))    //has Google conversion tracking setting
                {
                    string[] trackingCodes = GoogleOrderTracking.Split(';');
                    if (trackingCodes.Length > 1)   //Tracking code shall come in pair and separated by ";"
                        this.setAdWordsConversionTracking(trackingCodes[0], trackingCodes[1], Presentation.eStoreContext.Current.Order.totalAmountX);
                }

                //Add AJP APERZA tracking data
                string siteID = string.Empty;
                if (eStoreContext.Current.getBooleanSetting("Aperza", false) == true)
                    siteID = eStoreContext.Current.SessionID;
                Presentation.eStoreContext.Current.AffiliateTracking(Presentation.eStoreContext.Current.Order, siteID);
            }
            btnPirntOrder.PostBackUrl = "~/Cart/printorder.aspx?orderid=" + eStore.Presentation.eStoreContext.Current.Order.OrderNo;
            if (!IsPostBack)
            {
                bindFonts();
                if (!Presentation.eStoreContext.Current.keywords.ContainsKey("CompleteOrder"))
                    Presentation.eStoreContext.Current.keywords.Add("CompleteOrder", "true");
            }
        }
        protected void bindFonts()
        {
            btnPirntOrder.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Print_this_page);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Continue_Shopping);
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            string _localUrl = esUtilities.CommonHelper.GetStoreLocation();
            Response.Redirect(_localUrl.EndsWith("/") ? _localUrl.Substring(0, _localUrl.Length - 1) : _localUrl);
        }
    }
}