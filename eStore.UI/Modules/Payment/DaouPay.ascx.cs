using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class DaouPay : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected override void OnPreRender(EventArgs e)
        {
            ltBankInfo.Text = eStore.Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Bank_Information_AKR);
            base.OnPreRender(e);
        }

        public string ClientValidataionFunction()
        {
            return "";
        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment payment = new POCOS.Payment();
            payment.CardType = "Card";
            return payment;
        }

        public bool PreLoad()
        {
            return true;
        }

        public bool ValidateForm()
        {
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}