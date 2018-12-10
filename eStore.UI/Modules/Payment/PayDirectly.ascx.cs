using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class PayDirectly : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //payment method will be changed befor render
        protected override void OnPreRender(EventArgs e)
        {
            lbankInfo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Pay_Directly_Information);
            base.OnPreRender(e);
        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment paymentInfo = new POCOS.Payment();
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }

        public string ClientValidataionFunction()
        {
            return string.Empty;
        }

        public bool PreLoad()
        {
            return true;
        }
    }
}