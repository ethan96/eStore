using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INIpayNet;

namespace eStore.UI.Modules.Payment
{
    public partial class INIpay : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 
        }
        protected override void OnPreRender(EventArgs e)
        {
            ltBankInfo.Text = eStore.Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Bank_Information_AKR);
            base.OnPreRender(e);
        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment payment = new POCOS.Payment();
            payment.CardType = "Card";
            return payment;
        }

        public bool ValidateForm()
        {
            return true;
        }
        public string ClientValidataionFunction()
        {
            if (Request.Browser.Browser == "IE" || Request.Browser.Browser == "InternetExplorer")
            {
                if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("AKR_Show_Info"))
                    return "return checkAKRcheckbox();";
                return string.Empty;
            }
            else
                return "return inicisforieonly();";
        }

        public bool PreLoad()
        {

            return true;
        }
    }
}