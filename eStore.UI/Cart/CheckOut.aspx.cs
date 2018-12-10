using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.Payment;
using eStore.Presentation;
using eStore.POCOS;
using esUtilities;

namespace eStore.UI.Cart
{
    public partial class CheckOut : eStoreBaseOrderPage
    {
        protected bool paymentControlLoaded = false;

        protected override eStoreBaseOrderPage.PageStep minStepNo
        {
            get
            {
                return PageStep.ToPayment;
            }
        }

        public override bool UseSSL
        {
            get
            {
                return !Presentation.eStoreContext.Current.isTestMode() && Presentation.eStoreContext.Current.getBooleanSetting("IsSecureCheckout",true);
            }
            set
            {
                base.UseSSL = value;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //check order
            if (eStoreContext.Current.Order != null)
            {
                var _dborder = Presentation.eStoreContext.Current.Store.getOrder(eStoreContext.Current.Order.OrderNo);
                if (_dborder == null)
                {
                    eStoreContext.Current.Order = null;
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Save Order Failed", null, true);
                }
            }
            

            //ICollection<StorePayment> storePayments = Presentation.eStoreContext.Current.Store.profile.StorePayments;
            IEnumerable<StorePayment> storePayments;
            if (Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedTaxIDReview
                || Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedTaxAndFreightReview
                || Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedGeneralReview
                || Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedFreightReview)
                storePayments = Presentation.eStoreContext.Current.Store.getStorePayments(null, true);
            else
            {
                eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                POCOS.Country billto = eSolution.countries.FirstOrDefault(c => c.Shorts == Presentation.eStoreContext.Current.Order.cartX.BillToContact.countryCodeX);
                storePayments = Presentation.eStoreContext.Current.Store.getStorePayments(billto, false);
            }
            LoadPaymentType(storePayments);

            if (Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.SimulateTransaction))

                this.cbsimulation.Visible = true;
            else
            {
                this.cbsimulation.Visible = false;
                this.cbsimulation.Checked = false;
            }


            LoadPaymentMethod(storePayments);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (rblPaymentMethod.Items.Count > 0 && rblPaymentMethod.SelectedIndex == -1)
            {
                rblPaymentMethod.SelectedValue = Presentation.eStoreContext.Current.Order.PaymentType;
            }
            if (!IsPostBack)
            {
                //2017/02/23 test ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureCheckout", GoogleGAHelper.MeasureCheckout(Presentation.eStoreContext.Current.Order, 3).ToString(), true);

                cbsimulation.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Simulation);
                btnCheckout.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_SubmitPayment);
                string checkoutBottomMarked = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Checkout_Bottom_Marked);
                if (!string.IsNullOrEmpty(checkoutBottomMarked))
                    ltBottomMarked.Text = "<span class='colorRed'>&nbsp;*&nbsp;</span>" + checkoutBottomMarked;
            }
        }

        protected void rblPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var ctrl in this.phPaymentInfo.Controls)
            {
                if (ctrl is IPaymentMethodModule)
                {
                    UserControl storePayment = (UserControl)ctrl;

                    if (storePayment.ID == rblPaymentMethod.SelectedValue)
                    {

                        if (((IPaymentMethodModule)ctrl).PreLoad())
                        {
                            storePayment.Visible = true;
                            Presentation.eStoreContext.Current.Order.PaymentType = rblPaymentMethod.SelectedValue;
                            string ClientValidataionFunction = string.Empty;
                            ClientValidataionFunction = ((IPaymentMethodModule)ctrl).ClientValidataionFunction();
                            prepareIndirectPayment(ClientValidataionFunction);
                        }
                        else
                        {
                            storePayment.Visible = false;
                        }
                    }
                    else
                    {
                        storePayment.Visible = false;

                    }
                }

            }
        }
        void prepareIndirectPayment(string ClientValidataionFunction)
        {
            POCOS.StorePayment storePayment = Presentation.eStoreContext.Current.Store.getStorePayment(Presentation.eStoreContext.Current.Order.PaymentType);
            POCOS.Payment paymentInfo = new POCOS.Payment();
            paymentInfo.Amount = Presentation.eStoreContext.Current.Order.totalAmountX;

            if (storePayment.PaymentType == "Redirect")
            {
                this.cbsimulation.AutoPostBack = true;  
                if (this.cbsimulation.Checked)
                    paymentInfo.Comment2 = "Simulation";
                else
                    paymentInfo.Comment2 = string.Empty;

                IDictionary<String, String> paras = Presentation.eStoreContext.Current.Store.prepareIndirectPayment(Presentation.eStoreContext.Current.Order, storePayment, paymentInfo, this.cbsimulation.Checked, storePayment.PaymentMethod.StartsWith("INIpay")==false);
                ClientScriptManager csm = Page.ClientScript;
               
                if (paras.ContainsKey("actionURL") && !string.IsNullOrEmpty(ClientValidataionFunction))
                {
                    btnCheckout.Attributes.Add("onclick", HttpUtility.HtmlDecode("$(\"form:first\").attr(\"action\", \"" + paras["actionURL"] + "\").submit( function(){" + ClientValidataionFunction + "}); "));
                    paras.Remove("actionURL");
                }
                else if (!string.IsNullOrEmpty(ClientValidataionFunction))
                {
                    btnCheckout.Attributes.Add("onclick", ClientValidataionFunction);
                }
                else if (paras.ContainsKey("actionURL"))
                {
                    btnCheckout.Attributes.Add("onclick", HttpUtility.HtmlDecode("$(\"form:first\").attr(\"action\", \"" + paras["actionURL"] + "\").submit(); "));
                    paras.Remove("actionURL");
                }
                else
                {
                    btnCheckout.Attributes.Remove("onclick");
                }
                if (paras.ContainsKey("ServerReference"))
                {
                    paymentInfo.CCPNREF = paras["ServerReference"];
                    paras.Remove("ServerReference");
                }
                foreach (var p in paras)
                {
                    csm.RegisterHiddenField(p.Key, p.Value);
                }
                if (Presentation.eStoreContext.Current.Order.save() != 0)
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Save Order Failed", null, true);
                    return;
                }
            }
            else
            {
                this.cbsimulation.AutoPostBack = false;
                if (!string.IsNullOrEmpty(ClientValidataionFunction))
                {
                    btnCheckout.Attributes.Add("onclick", ClientValidataionFunction);
                }
                else
                {
                    btnCheckout.Attributes.Remove("onclick");
                }
            }
        }

        protected void cbsimulation_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var ctrl in this.phPaymentInfo.Controls)
            {
                if (ctrl is IPaymentMethodModule)
                {
                    UserControl storePayment = (UserControl)ctrl;

                    if (storePayment.ID == rblPaymentMethod.SelectedValue)
                    {

                        if (((IPaymentMethodModule)ctrl).PreLoad())
                        {
                            storePayment.Visible = true;
                            Presentation.eStoreContext.Current.Order.PaymentType = rblPaymentMethod.SelectedValue;
                            string ClientValidataionFunction = string.Empty;
                            ClientValidataionFunction = ((IPaymentMethodModule)ctrl).ClientValidataionFunction();
                            prepareIndirectPayment(ClientValidataionFunction);
                        }
                        else
                        {
                            storePayment.Visible = false;
                        }
                    }
                    else
                    {
                        storePayment.Visible = false;

                    }
                }

            }
        }
        protected void btnCheckout_Click(object sender, EventArgs e)
        {

            POCOS.Payment payment = GetPaymentInfo(Presentation.eStoreContext.Current.Order.PaymentType);
            if (payment == null)
            {
                Utilities.eStoreLoger.Error("Can not get PaymentInfo", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                //Presentation.eStoreContext.Current.AddStoreErrorCode("Can't get Payment Method", null, true);
                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_The_selected_payment_module), null, true);
                return;
            }

            payment.Amount = Presentation.eStoreContext.Current.Order.totalAmountX;

            Boolean simulation = this.cbsimulation.Checked;
            Presentation.eStoreContext.Current.Order.PaymentType = this.rblPaymentMethod.SelectedValue;
            //save order before make payment
            if (Presentation.eStoreContext.Current.Order.save() != 0)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Save_Order_Failed), null, true);
                return;
            }
            POCOS.StorePayment storePayment = Presentation.eStoreContext.Current.Store.getStorePayment(Presentation.eStoreContext.Current.Order.PaymentType);
            POCOS.Payment paymentRlt = Presentation.eStoreContext.Current.Store.makePayment(Presentation.eStoreContext.Current.Order, storePayment, payment, simulation);

            if (paymentRlt.isValidPaymentAccepted())
            {
                if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
                {
                    Presentation.eStoreContext.Current.UserShoppingCart.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                    Presentation.eStoreContext.Current.UserShoppingCart.save();
                }
                else
                {
                    Presentation.eStoreContext.Current.Quotation.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                }
                if (Presentation.eStoreContext.Current.Order.save() != 0)
                {
                    Presentation.eStoreContext.Current.Order = null;

                    Utilities.eStoreLoger.Error("SaveOrderFailed", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Save_Order_Failed), null, true);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(paymentRlt.FederalID))
                {
                    Presentation.eStoreContext.Current.User.FederalID = paymentRlt.FederalID;
                    //Presentation.eStoreContext.Current.User.CompanyID = paymentRlt.FederalID;
                    Presentation.eStoreContext.Current.User.save();
                    Presentation.eStoreContext.Current.Order.userX.FederalID = paymentRlt.FederalID; //暂时没找到Current.User 和 Current.Order.userX 不同步到原因。
                }


                switch (storePayment.PaymentType)
                {

                    case "Redirect":
                        {

                            Response.Redirect("~/completeIndirectPayment.aspx");
                            break;
                        }
                    case "API":
                    case "NetTerm":
                    case "WireTransfer":
                    case "PayDirectly":
                        {

                            eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                            mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                            EMailReponse response = mailTemplate.getOrderMailContent(Presentation.eStoreContext.Current.Order, eStoreContext.Current.CurrentLanguage
                                , eStore.Presentation.eStoreContext.Current.MiniSite);
                            if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                            {
                                Utilities.eStoreLoger.Error(string.Format("{0} sent mail failed. {1}", Presentation.eStoreContext.Current.Order.OrderNo, response.ErrCode.ToString())
                                    , Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);

                            }
                            if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                            {
                                Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), Presentation.eStoreContext.Current.Order);
                            }
                            //生成一个待 审核的log
                            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
                                Presentation.eStoreContext.Current.Store.calculateOrderReward(Presentation.eStoreContext.Current.Order, Presentation.eStoreContext.Current.MiniSite);

                            //ATW iAble
                            if (Presentation.eStoreContext.Current.getBooleanSetting("iAblePointClub", false) == true)
                                Presentation.eStoreContext.Current.Store.CalculateiAblePointAfterOrder(Presentation.eStoreContext.Current.Order, Presentation.eStoreContext.Current.MiniSite, simulation);

                            //B+B sync order
                            if (Presentation.eStoreContext.Current.getBooleanSetting("BBFlag", false) == true && eStoreContext.Current.isTestMode() == false)
                                Presentation.eStoreContext.Current.Store.syncBBorderToSAP(Presentation.eStoreContext.Current.Order);

                            Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/thankyou.aspx");
                            break;
                        }
                    default:
                        {
                            Utilities.eStoreLoger.Error("Not defined payment type", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);

                            throw new Exception("Not defined payment type");

                        }
                }
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(paymentRlt.errorCode);
                Presentation.eStoreContext.Current.Order.save();
            }

        }

        void LoadPaymentType(IEnumerable<StorePayment> storePayments)
        {

            if (storePayments.Count() > 0)
            {
                this.paymentmethodpanel.Visible = true;
                //this.rblPaymentMethod.DataSource = storePayments;
                //this.rblPaymentMethod.DataTextField = "Description";
                //this.rblPaymentMethod.DataValueField = "PaymentMethod";
                //this.rblPaymentMethod.DataBind();

                foreach (StorePayment sp in storePayments)
                {
                    ListItem li = new ListItem();
                    string payKey = sp.Description;
                    try
                    {
                        eStore.POCOS.Store.TranslationKey nPayPey;
                        if(Enum.TryParse(string.Format("Cart_{0}", sp.PaymentMethod.Replace(" ", "_")), out nPayPey))
                            payKey = eStore.Presentation.eStoreLocalization.Tanslation(nPayPey);
                        if(eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount") && payKey.IndexOf("{0}")>0)
                            payKey = string.Format(payKey, Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(Presentation.eStoreContext.Current.Order.totalAmountX, Presentation.eStoreContext.Current.UserShoppingCart.currencySign));
                    }
                    catch (Exception)
                    {
                        Utilities.eStoreLoger.Error("Payment Key Error", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                    }
                    li.Text = payKey;
                    li.Value = sp.PaymentMethod;
                    rblPaymentMethod.Items.Add(li);
                }

                if (storePayments.Count() == 1)
                {

                    this.paymentmethodpanel.Visible = false;
                }
            }

            else
            {
                Utilities.eStoreLoger.Error("Not set payment method for this store", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);

                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_The_selected_payment_module), null, true);
                return;
            }
            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PaymentType))
            {
                Presentation.eStoreContext.Current.Order.PaymentType = storePayments.First().PaymentMethod;
                if (rblPaymentMethod.Items.Count > 0)
                { rblPaymentMethod.SelectedValue = Presentation.eStoreContext.Current.Order.PaymentType; }
            }
        }
        void LoadPaymentMethod(IEnumerable<StorePayment> storePayments)
        {
            if (paymentControlLoaded)
            {
                this.phPaymentInfo.Controls.Clear();
            }
            foreach (POCOS.StorePayment storePayment in storePayments)
            {
                try
                {
                    Control child = null;
                    child = base.LoadControl(storePayment.Account);
                    child.ID = storePayment.PaymentMethod;
                    if (Presentation.eStoreContext.Current.Order.PaymentType == storePayment.PaymentMethod)
                    {
                        if (!IsPostBack && !((IPaymentMethodModule)child).PreLoad())
                        {
                            child.Visible = false;
                        }
                        else
                        {
                            child.Visible = true;
                            string ClientValidataionFunction = string.Empty;
                            if (child is IPaymentMethodModule)
                                ClientValidataionFunction = ((IPaymentMethodModule)child).ClientValidataionFunction();
                            if (!IsPostBack)
                                prepareIndirectPayment(ClientValidataionFunction);
                        }
                    }
                    else
                    { child.Visible = false; }

                    this.phPaymentInfo.Controls.Add(child);
                    paymentControlLoaded = true;
                }
                catch (Exception ex)
                {
                    Utilities.eStoreLoger.Error("LoadPaymentMethod Error", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, ex);
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cant_Load_Payment_Method), null, true);
                    return;
                }

            }
        }


        public POCOS.Payment GetPaymentInfo(string PaymentType)
        {
            POCOS.Payment paymentInfo = null;
            var ctrl = GetPaymentModule(PaymentType);
            if (ctrl != null)
            {
                paymentInfo = ctrl.GetPaymentInfo();

            }
            return paymentInfo;
        }

        protected IPaymentMethodModule GetPaymentModule(string PaymentType)
        {
            foreach (var ctrl in this.phPaymentInfo.Controls)
            {
                if (ctrl is IPaymentMethodModule)
                {
                    UserControl storePayment = (UserControl)ctrl;

                    if (storePayment.ID == PaymentType)
                    {

                        return (IPaymentMethodModule)ctrl;
                    }

                }

            }
            return null;
        }

    }
}