using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class Payway : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }
        private void BindData()
        {
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                creditCardExpireYear.Items.Add(new ListItem(year, year));
            }

            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i.ToString() : i.ToString();
                creditCardExpireMonth.Items.Add(new ListItem(text, text));
            }
        }

        public POCOS.Payment GetPaymentInfo()
        {

            POCOS.Payment paymentInfo = new POCOS.Payment();
            paymentInfo.cardNo = this.cardnumber.Text;
            paymentInfo.CardHolderName = this.cardholder.Text;
            //Expiration format is MMDD
            paymentInfo.CardExpiredDate = this.creditCardExpireMonth.Text + this.creditCardExpireYear.Text.Substring(2);
            paymentInfo.CardType = this.ddlcardtype.SelectedValue;
            paymentInfo.SecurityCode = this.CVV2.Text;
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }

        public string ClientValidataionFunction()
        {
            return  string.Empty;
        }
        public bool PreLoad()
        {
            return true;
        }
    }
}