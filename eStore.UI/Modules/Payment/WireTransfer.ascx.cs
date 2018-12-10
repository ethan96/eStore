using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class WireTransfer : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        string checkPaymentInfo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
        //payment method will be changed befor render
        protected override void OnPreRender(EventArgs e)
        {
            lbankInfo.Text = eStore.Presentation.eStoreContext.Current.Store.getBankInformation(eStore.Presentation.eStoreContext.Current.Order);
            if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PurchaseNO))
                PONo1.PONoText = Presentation.eStoreContext.Current.Order.PurchaseNO;
            base.OnPreRender(e);
        }

        public POCOS.Payment GetPaymentInfo()
        {

            POCOS.Payment paymentInfo = new POCOS.Payment();
            paymentInfo.FederalID = this.txtFederalID.Text;
            paymentInfo.PurchaseNO = PONo1.PONoText;
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }
        public string ClientValidataionFunction()
        {
            BindCompleteInfor();
            return checkPaymentInfo;
        }
        public bool PreLoad()
        {
            return true;
        }

        protected void BindCompleteInfor()
        {
            string completeoffer = eStore.Presentation.eStoreContext.Current.getStringSetting("eStore_Complete_offer");
            if (!string.IsNullOrEmpty(completeoffer))
            {
                var pc = this.Page.LoadControl(completeoffer) as files.BaseComplete;
                this.phPaymentMessage.Controls.Add(pc);
                checkPaymentInfo = pc.ClientValidataionFunction();
            }
        }
    }
}