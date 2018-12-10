using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.BusinessModules;
using eStore.BusinessModules.TaxService;
using eStore.Presentation;

namespace eStore.UI.Cart
{
    public partial class confirm : eStoreBaseOrderPage
    {
        protected override eStoreBaseOrderPage.PageStep minStepNo
        {
            get
            {
                return PageStep.Confirm;
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            this.OrderContentPreview1.order = Presentation.eStoreContext.Current.Order;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PromoteCode))
                {
                    this.txtPromoteCode.Text = Presentation.eStoreContext.Current.Order.PromoteCode;
                }

                //2017/02/23 test ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureCheckout", GoogleGAHelper.MeasureCheckout(Presentation.eStoreContext.Current.Order, 2).ToString(), true);

                //2017/12/01 BB want to hide promotion code
                if (Presentation.eStoreContext.Current.getBooleanSetting("BBPromotion", false) == true)
                    PromotionPanel.Visible = false;

                this.OrderContentPreview1.order = Presentation.eStoreContext.Current.Order;
                bindFonts();
            }
            SetResaleInfor();
            //CNPJ Visible
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSettingCNPJ"))
            {
                this.ResaleSetting_CNPJ1.Visible = true;
                btnNext.OnClientClick = "return validateCNPJ()";
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.ResellerID) && !IsPostBack)
                    this.ResaleSetting_CNPJ1.ResellerID = Presentation.eStoreContext.Current.Order.ResellerID;
            }
            else
                this.ResaleSetting_CNPJ1.Visible = false;
            // if store support channelPartener and channelpartener is not advantech will don't show promote code
            if (Presentation.eStoreContext.Current.Order.storeX.channelPartnerReferralEnabled)
            {
                if (Presentation.eStoreContext.Current.Order.isReferredToChannelPartner())
                    DivPromoteCode.Visible = false;
                else
                    DivPromoteCode.Visible = true;
            }

            // ICC eQuotation's order should not see promotion code
            if (Presentation.eStoreContext.Current.Quotation != null && Presentation.eStoreContext.Current.Quotation.Source == POCOS.Quotation.QuoteSource.eQuotation)
                DivPromoteCode.Visible = false;
        }

        private void SetResaleInfor()
        {
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSetting"))
            {
                this.ResaleSetting1.Visible = true;
                this.ResaleSetting1.btnRecalculateTax_Click = btnRecalculateTax_Click;
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.ResellerID) && !IsPostBack)
                {
                    this.ResaleSetting1.EnableReseller = true;
                    this.ResaleSetting1.DisplayAndEnableModify();
                    this.ResaleSetting1.ResellerID = Presentation.eStoreContext.Current.Order.ResellerID;

                }
            }
            else
            { this.ResaleSetting1.Visible = false; }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Order.GetAlert() == Order.OAlert.NeedFreightReview)
            {
                OrderContentPreview1.EditContact(new Button {CommandName = "Ship"}, null);
                return;
            }

            if (Presentation.eStoreContext.Current.Order.isBelowCost())
            {
                //show error message
                Presentation.eStoreContext.Current.AddStoreErrorCode("Price Error");
                return;
            }

            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSetting") && this.ResaleSetting1.EnableReseller)
            {
                if (this.ResaleSetting1.setResellerCertificate())
                {
                    Presentation.eStoreContext.Current.Order.ResellerID = this.ResaleSetting1.ResellerID;
                    Presentation.eStoreContext.Current.Order.ResellerCertificate = Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate;
                    TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Order);
                    Presentation.eStoreContext.Current.Order.save();
                }
                else
                    return;
            }
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSettingCNPJ") && this.ResaleSetting_CNPJ1.Visible)
            {
                if (!string.IsNullOrEmpty(this.ResaleSetting_CNPJ1.ResellerID))
                    Presentation.eStoreContext.Current.Order.ResellerID = this.ResaleSetting_CNPJ1.ResellerID;
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Please Enter CNPJ/CPF!");
                    return;
                }
            }
            /*
            bool UseSSL = !Presentation.eStoreContext.Current.isTestMode()
                && Presentation.eStoreContext.Current.Store.profile.boolSetting("IsSecureCheckout");
            Response.Redirect(esUtilities.CommonHelper.GetStoreHost(UseSSL) + "Cart/checkout.aspx");
             * */

            if (Presentation.eStoreContext.Current.Order.isReferredToChannelPartner() && Presentation.eStoreContext.Current.Store.storeID == "AEU")
            {
                if (Presentation.eStoreContext.Current.Store.ServiceByChannelPartner(Presentation.eStoreContext.Current.Order)
                    && Presentation.eStoreContext.Current.Order.save() == 0
                    )
                {
                    Presentation.eStoreContext.Current.UserShoppingCart.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                    Presentation.eStoreContext.Current.UserShoppingCart.save(); // when order confirm will clear user cart

                    eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                    mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                    esUtilities.EMailReponse response = mailTemplate.getOrderMailContent(Presentation.eStoreContext.Current.Order, eStore.Presentation.eStoreContext.Current.CurrentLanguage
                                , eStore.Presentation.eStoreContext.Current.MiniSite);
                    if (!Presentation.eStoreContext.Current.isTestMode())
                        Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(true) + "Cart/thankyou.aspx");
                    else
                        Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(false) + "Cart/thankyou.aspx");
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Save_Order_Failed), null, true);
                    return;
                }
            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "ABR"|| Presentation.eStoreContext.Current.getBooleanSetting("SkipPayment", false))
            {
                if (Presentation.eStoreContext.Current.Store.ServiceByChannelPartner(Presentation.eStoreContext.Current.Order)
          && Presentation.eStoreContext.Current.Order.save() == 0
          )
                {
                    Presentation.eStoreContext.Current.UserShoppingCart.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                    Presentation.eStoreContext.Current.UserShoppingCart.save(); // when order confirm will clear user cart

                    eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                    mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                    esUtilities.EMailReponse response = mailTemplate.getOrderMailContent(Presentation.eStoreContext.Current.Order, eStore.Presentation.eStoreContext.Current.CurrentLanguage
                                , eStore.Presentation.eStoreContext.Current.MiniSite);
                    if (!Presentation.eStoreContext.Current.isTestMode() && Presentation.eStoreContext.Current.getBooleanSetting("IsSecureCheckout"))
                        Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(true) + "Cart/thankyou.aspx");
                    else
                        Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(false) + "Cart/thankyou.aspx");
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Save_Order_Failed), null, true);
                    return;
                }
            }
            else
            {
                if (!Presentation.eStoreContext.Current.isTestMode() && Presentation.eStoreContext.Current.getBooleanSetting("IsSecureCheckout"))
                    Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(true) + "Cart/checkout.aspx");
                else
                    Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(false) + "Cart/checkout.aspx");
            }
        }

        protected void btnRecalculateTax_Click(object sender, EventArgs e)
        {
            if (this.ResaleSetting1.EnableReseller)
            {
                if (this.ResaleSetting1.setResellerCertificate())
                {
                    Presentation.eStoreContext.Current.Order.ResellerID = this.ResaleSetting1.ResellerID;
                    Presentation.eStoreContext.Current.Order.ResellerCertificate = Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate;
                    //this.OrderContentPreview1.order = Presentation.eStoreContext.Current.Order;
                    this.ResaleSetting1.DisplayAndEnableModify();
                    this.ResaleSetting1.EnableReseller = true;
                }
                else
                { return; }

            }
            else
            {
                Presentation.eStoreContext.Current.Order.ResellerID = string.Empty;
                Presentation.eStoreContext.Current.Order.ResellerCertificate = string.Empty;
                //重新绑定UserResaleID
                if(Presentation.eStoreContext.Current.User != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.ResellerID))
                    this.ResaleSetting1.ResellerID = Presentation.eStoreContext.Current.User.actingUser.ResellerID;
            }
            TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Order);
            if (taxProvider.Status == false)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                return;
            }
            Presentation.eStoreContext.Current.Order.save();

        }
        protected void btnApplyPromoteCode_Click(object sender, EventArgs e)
        {
            POCOS.CampaignManager.PromotionCodeStatus promotionCodeStatus = Presentation.eStoreContext.Current.Order.applyPromotionCode(Presentation.eStoreContext.Current.User, this.txtPromoteCode.Text);
            switch (promotionCodeStatus)
            {
                case POCOS.CampaignManager.PromotionCodeStatus.Valid:
                    {
                        TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Order);
                        if (taxProvider.Status == false)
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                            return;
                        }
                        this.OrderContentPreview1.order= Presentation.eStoreContext.Current.Order;
                        break;
                    }
                case POCOS.CampaignManager.PromotionCodeStatus.ExceedCampaignLimit:
                case POCOS.CampaignManager.PromotionCodeStatus.ExceedUserLimit:
                case POCOS.CampaignManager.PromotionCodeStatus.Expired:
                case POCOS.CampaignManager.PromotionCodeStatus.Invalid:
                case POCOS.CampaignManager.PromotionCodeStatus.LessThanMinimumRequirement:
                case POCOS.CampaignManager.PromotionCodeStatus.NotApplicable:
                default:
                    Presentation.eStoreContext.Current.AddStoreErrorCode("PromotionCode" + promotionCodeStatus.ToString());
                    this.txtPromoteCode.Text = string.Empty;
                    break;
            }

        }

        protected void btnBack2Contact_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
            { Response.Redirect("~/Cart/Contact.aspx"); }
            else
            { Response.Redirect("~/Quotation/confirm.aspx"); }
        }

        protected void bindFonts()
        {
            btnApplyPromoteCode.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Apply);
            if (Presentation.eStoreContext.Current.getBooleanSetting("SkipPayment", false))
            {
                btnNext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Confirm);
            }
            else if (!Presentation.eStoreContext.Current.Order.isReferredToChannelPartner())
                btnNext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Payment);

            else
                btnNext.Text = "Submit ChannelPartner";
            btnBack2Contact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Back);
        }

    }
}