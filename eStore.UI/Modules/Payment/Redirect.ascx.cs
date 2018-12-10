using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class Redirect : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RedirectPaymentDesc.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_here_is_store_desc_for_redirect_payment);
            }
        }

        public POCOS.Payment GetPaymentInfo()
        {

            return new POCOS.Payment();
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