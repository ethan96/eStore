using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class Paypal : Presentation.eStoreBaseControls.eStoreBaseUserControl,Presentation.Payment.IPaymentMethodModule
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            BindCartType();
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
            int currentMonth = DateTime.Now.AddMonths(1).Month;
            string expirationMonth = (currentMonth < 10) ? "0" + currentMonth : currentMonth.ToString();
            if (DateTime.Now.Month == 12)
            {
                ListItem expirationYear = creditCardExpireYear.Items.FindByText((DateTime.Now.Year+1).ToString());
                if (expirationYear != null)
                {
                    creditCardExpireYear.ClearSelection();
                    expirationYear.Selected = true;
                }
            }
            ListItem expirationItem = creditCardExpireMonth.Items.FindByText(expirationMonth);
            if (expirationItem != null)
            {
                creditCardExpireMonth.ClearSelection();
                expirationItem.Selected = true;
            }

            if (Presentation.eStoreContext.Current.Order != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PurchaseNO))
                PONo1.PONoText = Presentation.eStoreContext.Current.Order.PurchaseNO;
        }
        
        private void BindCartType()
        {
            if (ddlcardtype.Items.Count == 0)
            {
                string cartTypeStr = eStore.Presentation.eStoreContext.Current.getStringSetting("CreditCard_Show_Type");
                if (!string.IsNullOrEmpty(cartTypeStr))
                {
                    string[] cartTypeList = cartTypeStr.Split('|');
                    foreach (string item in cartTypeList)
                    {
                        string[] valueList = item.Split('=');
                        if (valueList.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(valueList[1]))
                                ddlcardtype.Items.Add(new ListItem(valueList[1], valueList[0]));
                            else
                                ddlcardtype.Items.Add(new ListItem(valueList[0], valueList[0]));
                        }                            
                        else
                            ddlcardtype.Items.Add(new ListItem(valueList[0], valueList[0]));
                    }
                }
                else
                    ddlcardtype.Items.Add(new ListItem("None", "None"));   
            }            
        }

        public POCOS.Payment GetPaymentInfo()
        {
         
            POCOS.Payment paymentInfo= new POCOS.Payment();
            paymentInfo.cardNo   = this.cardnumber.Text ;
            paymentInfo.CardHolderName = this.cardholder.Text;
            //Expiration format is MMDD
            paymentInfo.CardExpiredDate = this.creditCardExpireMonth.Text + this.creditCardExpireYear.Text.Substring(2);
            paymentInfo.CardType = this.ddlcardtype.SelectedValue;
            paymentInfo.SecurityCode = this.CVV2.Text;
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
            return "return checkPaymentInfo();";
        }


        public bool PreLoad()
        {
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CreditCard_Show_Term"))
            {
                System.Web.UI.UserControl pc;
                if (eStore.Presentation.eStoreContext.Current.Store.storeID == "ABB")
                    pc = this.Page.LoadControl("~/Modules/Payment/files/BB_PaymentMessage.ascx") as files.BB_PaymentMessage;
                else if(eStore.Presentation.eStoreContext.Current.Store.storeID == "AEU")
                    pc = this.Page.LoadControl("~/Modules/Payment/files/AEU_CompleteOfferWithoutAgreeBt.ascx") as files.BaseComplete;
                else
                    pc = this.Page.LoadControl("~/Modules/Payment/files/AUS_PaymentMessage.ascx") as files.AUS_PaymentMessage;
                this.phPaymentMessage.Controls.Add(pc);
            }

        }
    }
}