using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class WireTransferAKR : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            lbankInfo.Text = eStore.Presentation.eStoreContext.Current.Store.getBankInformation(eStore.Presentation.eStoreContext.Current.Order);
            //if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PurchaseNO))
            //    PONo1.PONoText = Presentation.eStoreContext.Current.Order.PurchaseNO;
            ltBankInfo.Text = eStore.Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Bank_Information_AKR);
            base.OnPreRender(e);
        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment paymentInfo = new POCOS.Payment();
            //paymentInfo.PurchaseNO = PONo1.PONoText;
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }
        public string ClientValidataionFunction()
        {
            return "";
        }
        public bool PreLoad()
        {
            return true;
        }
    }
}