using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CartNivagator : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private readonly string inactivelink = "<li>{0}</li>";
        private readonly string activelink = "<li class='on'>{0}<span></span></li>";

        public string OrderProgressStep
        {
            get
            {
                object obj2 = this.ViewState["OrderProgressStep"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["OrderProgressStep"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                setSatus();
            }
            if (eStore.Presentation.eStoreContext.Current.Store.storeID == "ABR" || Presentation.eStoreContext.Current.getBooleanSetting("SkipPayment", false))
            {
                hlPayment.Visible = false;
            }
        }

        protected void setSatus()
        {
            hlCart.Text = string.Format(inactivelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
            hlAddress.Text = string.Format(inactivelink,eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts));
            hlConfirm.Text = string.Format(inactivelink,eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm));
            hlPayment.Text = string.Format(inactivelink,eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Payment));
            hlThankyou.Text = string.Format(inactivelink,eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Complete));
            switch (this.OrderProgressStep.ToLowerInvariant())
            {
                case "cart":
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
                    ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart);
                    break;
                case "address":
                    hlAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts));
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
                    ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts);
                    break;
                case "confirm":
                    hlConfirm.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm));
                    hlAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts));
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
                    ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm);
                    break;
                case "payment":
                    hlPayment.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Payment));
                    hlAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts));
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
                    hlConfirm.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm));
                    ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Payment);
                    break;
                case "complete":
                    hlThankyou.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Complete));
                    hlConfirm.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm));
                    hlPayment.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Payment));
                    hlAddress.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts));
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Complete));
                    break;
                default:
                    hlCart.Text = string.Format(activelink, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart));
                    ltnavigator.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Shopping_Cart);
                    break;
            }
        }
        
    }
}