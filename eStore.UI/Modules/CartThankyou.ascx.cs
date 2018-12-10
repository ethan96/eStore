using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CartThankyou : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lAmount.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(Presentation.eStoreContext.Current.Order.totalAmountX);
            lOrderNo.Text = Presentation.eStoreContext.Current.Order.OrderNo;
            ltOrderDate.Text = Presentation.eStoreContext.Current.Order.OrderDate.ToString();
            lStatus.Text = Presentation.eStoreContext.Current.Order.OrderStatus;
            eStore.POCOS.Store.TranslationKey nPayPey;
            string paytypeKey = Presentation.eStoreContext.Current.Order.PaymentType;
            if (!string.IsNullOrWhiteSpace(Presentation.eStoreContext.Current.Order.PaymentType)
                &&
                Enum.TryParse(string.Format("Cart_{0}", Presentation.eStoreContext.Current.Order.PaymentType.Replace(" ", "_")), out nPayPey))
                paytypeKey = eStore.Presentation.eStoreLocalization.Tanslation(nPayPey);
            ltPayMentType.Text = paytypeKey;
            ltReseller.Text = Presentation.eStoreContext.Current.Order.ResellerID;

            if (ltReseller.Text == string.Empty)
                li_Reseller.Visible = false;

            string payKey = Presentation.eStoreContext.Current.Order.statusX.ToString();
            try
            {
                eStore.POCOS.Store.TranslationKey nPayKey;
                if (Enum.TryParse(string.Format("Cart_{0}", Presentation.eStoreContext.Current.Order.statusX.ToString()), out nPayKey))
                    payKey = eStore.Presentation.eStoreLocalization.Tanslation(nPayKey);
            }
            catch (Exception)
            {
                Utilities.eStoreLoger.Error("Payment Key Error", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
            }
            lStatus.Text = payKey;
            
            
        }
        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            string _localUrl = esUtilities.CommonHelper.GetStoreLocation();
            Response.Redirect(_localUrl.EndsWith("/") ? _localUrl.Substring(0, _localUrl.Length - 1) : _localUrl);
        }
    }
}