using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;
using eStore.BusinessModules;
using eStore.BusinessModules.TaxService;

namespace eStore.UI.Quotation
{
    public partial class confirm : eStoreBaseQuotationPage
    {
        public string billtoCountry
        {
            get
            {
                if (Presentation.eStoreContext.Current.Quotation != null && Presentation.eStoreContext.Current.Quotation.cartX != null)
                    return Presentation.eStoreContext.Current.Quotation.cartX.BillToContact.Country;
                else
                    return string.Empty;
            }
        }

        protected override eStoreBaseQuotationPage.PageStep minStepNo
        {
            get
            {
                return PageStep.Confirm;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ResetResale();
            this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["quotation"]))
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == Request["quotation"]
                                                     select Quotation).FirstOrDefault();
                if (currentdQuotation != null)
                { Presentation.eStoreContext.Current.Quotation = currentdQuotation; }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cant_find_the_quotation), null, true);
                    return; ;
                }
            }
            if (Presentation.eStoreContext.Current.Quotation == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Timeout), null, true);
                return;
            }
            if (Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP))
            {
                this.txtTransferUser.Visible = true;
                this.btnTransfer.Visible = true;
                this.fTransferUser.Visible = true;
            }
            else
            {
                this.txtTransferUser.Visible = false;
                this.btnTransfer.Visible = false;
                this.fTransferUser.Visible = false;
            }

            showOrHideButton();

            //re-calculate freight and tax
            if (Presentation.eStoreContext.Current.Store.offerShippingService && Presentation.eStoreContext.Current.Quotation.isModifiable())
            {
                if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.ShippingMethod) && !Presentation.eStoreContext.Current.Store.offerNoShippingMothed)
                    Response.Redirect("~/Quotation/Contact.aspx");
            }
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.PromoteCode))
                    this.txtPromoteCode.Text = Presentation.eStoreContext.Current.Quotation.PromoteCode;
              
                this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
                this.lQuotationNumber.Text = Presentation.eStoreContext.Current.Quotation.QuotationNumber;
                this.lQuoteDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)Presentation.eStoreContext.Current.Quotation.QuoteDate);
                this.lQuoteExpiredDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)Presentation.eStoreContext.Current.Quotation.QuoteExpiredDate);
                bindFonts();

            }
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSetting"))
            {
                this.ResaleSetting1.Visible = true;
                this.ResaleSetting1.btnRecalculateTax_Click = btnRecalculateTax_Click;
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.ResellerID))
                {
                    if (!IsPostBack)
                    {
                        this.ResaleSetting1.ResellerID = Presentation.eStoreContext.Current.Quotation.ResellerID;

                        this.ResaleSetting1.EnableReseller = true;
                        if (Presentation.eStoreContext.Current.Quotation.isModifiable())
                        {
                            this.ResaleSetting1.DisplayAndEnableModify();
                        }
                        else
                        {
                            this.ResaleSetting1.DisplayOnly();
                        }
                    }
                }
                else
                {
                    if (!Presentation.eStoreContext.Current.Quotation.isModifiable())
                    {
                        this.ResaleSetting1.Visible = false;//
                    }
                }

            }
            else
            {
                this.ResaleSetting1.Visible = false;
            }
            //CNPJ Visible
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSettingCNPJ"))
            {
                this.ResaleSetting_CNPJ1.Visible = true;
                btnNext.OnClientClick = "return validateCNPJ()";
                btnTransfer.OnClientClick = "return validateCNPJ()";
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.ResellerID) && !IsPostBack)
                    this.ResaleSetting_CNPJ1.ResellerID = Presentation.eStoreContext.Current.Quotation.ResellerID;
            }
            else
                this.ResaleSetting_CNPJ1.Visible = false;
        }

        protected void btnRecalculateTax_Click(object sender, EventArgs e)
        {
            ResetResale();
        }

        protected void ResetResale()
        {
            if (this.ResaleSetting1.EnableReseller)
            {
                if (this.ResaleSetting1.setResellerCertificate())
                {
                    Presentation.eStoreContext.Current.Quotation.ResellerID = this.ResaleSetting1.ResellerID;
                    Presentation.eStoreContext.Current.Quotation.ResellerCertificate = Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate; //附件跟随ResellerID
                    TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
                    //Presentation.eStoreContext.Current.Quotation.save();
                    this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
                    this.ResaleSetting1.DisplayAndEnableModify();
                    this.ResaleSetting1.EnableReseller = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Presentation.eStoreContext.Current.Quotation.ResellerID = string.Empty;
                Presentation.eStoreContext.Current.Quotation.ResellerCertificate = string.Empty;
                TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
                if (taxProvider.Status == false)
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                    return;
                }
                //Presentation.eStoreContext.Current.Quotation.save();
            }
            this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
        }

        protected void btnconfirm_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.NeedFreightReview)
            {
                QuotationContentPreview1.EditContact(new Button { CommandName = "Ship" }, null);
                return;
            }

            //validating price
            if (Presentation.eStoreContext.Current.Quotation.isBelowCost())
            {
                //show error message
                Presentation.eStoreContext.Current.AddStoreErrorCode("Price Error");
                return;
            }
            
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSetting") && this.ResaleSetting1.EnableReseller)
            {
                if (this.ResaleSetting1.setResellerCertificate())
                {
                    Presentation.eStoreContext.Current.Quotation.ResellerID = this.ResaleSetting1.ResellerID;
                    Presentation.eStoreContext.Current.Quotation.ResellerCertificate = Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate;
                    TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
                    this.ResaleSetting1.DisplayOnly();
                    this.ResaleSetting1.EnableReseller = true;
                    this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.ResaleSetting1.Visible = false;
            }
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSettingCNPJ") && this.ResaleSetting_CNPJ1.Visible)
            {
                if (!string.IsNullOrEmpty(this.ResaleSetting_CNPJ1.ResellerID))
                    Presentation.eStoreContext.Current.Quotation.ResellerID = this.ResaleSetting_CNPJ1.ResellerID;
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Please Enter CNPJ/CPF!");
                    return;
                }
            }
            if (Presentation.eStoreContext.Current.Quotation.confirm(Presentation.eStoreContext.Current.User.actingUser) == 0)
            {

                if (Presentation.eStoreContext.Current.Quotation.save() == 0)
                {
                    String GoogleQuoteConversion = Presentation.eStoreContext.Current.getStringSetting("GoogleQuoteConversion");
                    if (!String.IsNullOrWhiteSpace(GoogleQuoteConversion))    //has Google conversion tracking setting
                    {
                        string[] trackingCodes = GoogleQuoteConversion.Split(';');
                        if (trackingCodes.Length > 1)   //Tracking code shall come in pair and separated by ";"
                            this.setAdWordsConversionTracking(trackingCodes[0], trackingCodes[1], Presentation.eStoreContext.Current.Quotation.totalAmountX);
                    }
                    showOrHideButton();

                    if (Presentation.eStoreContext.Current.User.actingUser.actingUser.hasRight(POCOS.User.ACToken.ATP))
                    {
                        this.txtTransferUser.Visible = true;
                        this.btnTransfer.Visible = true;
                        this.fTransferUser.Visible = true;
                        this.btnTransfer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Transfer);
                    }
                    else
                    {
                        this.txtTransferUser.Visible = false;
                        this.btnTransfer.Visible = false;
                        this.fTransferUser.Visible = false;
                    }

                    eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                    mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                    EMailReponse response = mailTemplate.getQuoteMailContent(Presentation.eStoreContext.Current.Quotation, null, null, eStore.Presentation.eStoreContext.Current.CurrentLanguage
                        , eStore.Presentation.eStoreContext.Current.MiniSite);
                    if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Failed at sending confirmation message.  Eror code is " + response.ErrCode.ToString());
                    }
                    if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                    {
                        Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), Presentation.eStoreContext.Current.Quotation);
                    }
                    Presentation.eStoreContext.Current.AffiliateTracking(Presentation.eStoreContext.Current.Quotation);

                    //If save quote success, show banner
                    if (!Presentation.eStoreContext.Current.keywords.ContainsKey("CompleteQuote")
                        && (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.Confirmed
                        || Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview
                        || Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview))
                        Presentation.eStoreContext.Current.keywords.Add("CompleteQuote", "true");
                }
                else
                {
                    if (Presentation.eStoreContext.Current.Quotation.needVATReview())
                    {
                        if(Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview)
                            Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                    }
                    else if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                    {
                        if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview)
                            Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                    }
                    else
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Save quotation failed");
                }
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Quotation confirmation failed");

            }
        }


        protected void btnReleasetoOrder_Click(object sender, EventArgs e)
        {
            //eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
            // EMailReponse response = mailTemplate.getQuoteMailContent(Presentation.eStoreContext.Current.Quotation);

            Presentation.eStoreContext.Current.Order = Presentation.eStoreContext.Current.Quotation.checkOut(Presentation.eStoreContext.Current.Store.profile, Presentation.eStoreContext.Current.User.actingUser);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Support_Channel_Partner"))
            {
                string channelId = Request["hChannelID"];
                string channelName = Request["hChannelName"];
                Presentation.eStoreContext.Current.Order.ChannelID = int.Parse(channelId);
                Presentation.eStoreContext.Current.Order.ChannelName = channelName;
                Presentation.eStoreContext.Current.Order.Channel = eStore.Presentation.eStoreContext.Current.Store.getChannelPartner(int.Parse(channelId));
            }

            if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
            {
                //Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Presentation.eStoreContext.Current.Quotation);
            }

            // Save eQuotation quote here
            if (Presentation.eStoreContext.Current.Quotation.Source == POCOS.Quotation.QuoteSource.eQuotation)
                Presentation.eStoreContext.Current.Quotation.save();

            //以前Quotation转order的时候,没有save. 直到最后checkout才有
            Presentation.eStoreContext.Current.Order.save();
            if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
                Presentation.eStoreContext.Current.Store.savePopularModelLogByOrder(Presentation.eStoreContext.Current.Order);

            Response.Redirect("~/Cart/confirm.aspx");
        }
        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            POCOS.User transferUser = Presentation.eStoreContext.Current.Store.getUser(this.txtTransferUser.Text);


            if (!transferUser.newUser)
            {
                if (Presentation.eStoreContext.Current.Quotation.isModifiable())
                {
                    if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSetting") && this.ResaleSetting1.EnableReseller)
                    {
                        if (this.ResaleSetting1.setResellerCertificate())
                        {
                            Presentation.eStoreContext.Current.Quotation.ResellerID = this.ResaleSetting1.ResellerID;
                            Presentation.eStoreContext.Current.Quotation.ResellerCertificate = Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate;
                            TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
                            this.ResaleSetting1.DisplayOnly();
                            this.ResaleSetting1.EnableReseller = true;
                            this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        this.ResaleSetting1.Visible = false;
                    }
                    if (Presentation.eStoreContext.Current.getBooleanSetting("EnableResaleSettingCNPJ") && this.ResaleSetting_CNPJ1.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.ResaleSetting_CNPJ1.ResellerID))
                            Presentation.eStoreContext.Current.Quotation.ResellerID = this.ResaleSetting_CNPJ1.ResellerID;
                        else
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode("Please Enter CNPJ/CPF!");
                            return;
                        }
                    }
                    if (Presentation.eStoreContext.Current.Quotation.confirm(Presentation.eStoreContext.Current.User.actingUser) == 0)
                    {

                        if (Presentation.eStoreContext.Current.Quotation.save() == 0)
                        {
                            showOrHideButton();
                        }
                        else
                        {
                            if (Presentation.eStoreContext.Current.Quotation.needVATReview())
                            {
                                if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview)
                                    Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                            }
                            else if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                            else
                                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
                            Presentation.eStoreContext.Current.AddStoreErrorCode("Save quotation failed");
                            return;
                        }
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Quotation confirmation failed");
                        return;
                    }
                }

                Presentation.eStoreContext.Current.Quotation.transfer(transferUser);
                eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                EMailReponse response = mailTemplate.getTransferredQuoteMailContent(Presentation.eStoreContext.Current.Quotation, transferUser, eStore.Presentation.eStoreContext.Current.CurrentLanguage, eStore.Presentation.eStoreContext.Current.MiniSite);

                object[] args;

                if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                {
                    args = new object[3];
                    args[0] = Presentation.eStoreContext.Current.Quotation.QuotationNumber;
                    args[1] = transferUser.UserID;
                    args[2] = response.ErrCode.ToString();
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Failed to transfer quotation {0} to {1}.\\r\\nErrorMessage:{2}", args);

                }
                else
                {
                    args = new object[2];
                    args[0] = Presentation.eStoreContext.Current.Quotation.QuotationNumber;
                    args[1] = transferUser.UserID;
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Successfully_transfer_quotation), args);
                }
            }
            else
            {
                object[] args = new object[1];
                args[0] = this.txtTransferUser.Text;
                Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Invalid_user) + " {0}", args);
            }
        }

        protected void btnApplyPromoteCode_Click(object sender, EventArgs e)
        {
            POCOS.CampaignManager.PromotionCodeStatus promotionCodeStatus = Presentation.eStoreContext.Current.Quotation.applyPromotionCode(Presentation.eStoreContext.Current.User, this.txtPromoteCode.Text);
            switch (promotionCodeStatus)
            {
                case POCOS.CampaignManager.PromotionCodeStatus.Valid:
                    {
                        TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
                        if (taxProvider.Status == false)
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                            return;
                        }
                        Presentation.eStoreContext.Current.Quotation.save();
                        //eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                        //EMailReponse response = mailTemplate.getQuoteMailContent(Presentation.eStoreContext.Current.Quotation);
                        
                        break;
                    }
                case POCOS.CampaignManager.PromotionCodeStatus.ExceedCampaignLimit:
                case POCOS.CampaignManager.PromotionCodeStatus.ExceedUserLimit:
                case POCOS.CampaignManager.PromotionCodeStatus.Expired:
                case POCOS.CampaignManager.PromotionCodeStatus.Invalid:
                case POCOS.CampaignManager.PromotionCodeStatus.LessThanMinimumRequirement:
                case POCOS.CampaignManager.PromotionCodeStatus.NotApplicable:
                case POCOS.CampaignManager.PromotionCodeStatus.IsBelowCost:
                default:
                    this.txtPromoteCode.Text = string.Empty;
                    Presentation.eStoreContext.Current.AddStoreErrorCode("PromotionCode" + promotionCodeStatus.ToString());
                    break;
            }

            this.QuotationContentPreview1.quotation = Presentation.eStoreContext.Current.Quotation;

        }

        protected void btnRevise_Click(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.Quotation = Presentation.eStoreContext.Current.Quotation.revise();
            if (Presentation.eStoreContext.Current.Quotation.needVATReview())
                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
            else if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
            else
                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
            Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void lbtnemailquote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.Confirmed
                    || Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview
                    || Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview)
            {
                eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                EMailReponse response = mailTemplate.getQuoteMailContent(Presentation.eStoreContext.Current.Quotation, null, null, eStore.Presentation.eStoreContext.Current.CurrentLanguage);
                if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Failed at sending confirmation message.  Eror code is " + response.ErrCode.ToString());
                }
                else
                {

                    Presentation.eStoreContext.Current.AddStoreErrorCode("Quote Mail is sent to your mailbox successful.");

                }
            }
        }

        protected void bindFonts()
        {
            hmyquotation.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_My_Quote);
            lbtnemailquote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Email_Quote);
            hprintorder.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Print_Quote);
            btnApplyPromoteCode.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Apply);
            btnRevise.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise);
            btnReleasetoOrder.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Checkout);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Continue_Shopping);
            btnNext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Save_Quote);
            btnTransfer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Transfer);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void showOrHideButton()
        {
            switch (Presentation.eStoreContext.Current.Quotation.statusX)
            {
                case POCOS.Quotation.QStatus.Open:
                case POCOS.Quotation.QStatus.NeedTaxIDReview:
                case POCOS.Quotation.QStatus.NeedFreightReview:
                case POCOS.Quotation.QStatus.Unfinished:
                    btnRevise.Visible = false;
                    btnNext.Visible = true;
                    btnReleasetoOrder.Visible = false;
                    btnContinueShopping.Visible = false;
                    this.btnTransfer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Save_Quote_and_Transfer);
                    //need check wheather store enable using promote
                    txtPromoteCode.Enabled = true;
                    btnApplyPromoteCode.Visible = true;
                    this.pWaitingConfirmInfo.Visible = true;
                    this.pThankyouInfo.Visible = false;
                    this.pConfirmedInfo.Visible = false;
                    break;
                case POCOS.Quotation.QStatus.NeedTaxAndFreightReview:
                case POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview:
                case POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview:
                case POCOS.Quotation.QStatus.NeedGeneralReview:
                case POCOS.Quotation.QStatus.Confirmed:
                    btnRevise.Visible = true;
                    btnNext.Visible = false;
                    if (Presentation.eStoreContext.Current.getBooleanSetting("QuoteOnly", false))
                    
                        btnReleasetoOrder.Visible = true;
                    btnContinueShopping.Visible = true;
                    txtPromoteCode.Enabled = false;
                    btnApplyPromoteCode.Visible = false;
                    this.hprintorder.Visible = true;
                    this.hmyquotation.Visible = true;
                    this.lbtnemailquote.Visible = true;
                    this.lseparator.Visible = true;
                    this.lseparator2.Visible = true;
                    this.pWaitingConfirmInfo.Visible = false;
                    this.pThankyouInfo.Visible = true;
                    this.pConfirmedInfo.Visible = true;

                    this.btnTransfer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Transfer);
    
                    break;
                case POCOS.Quotation.QStatus.Expired:
                    btnRevise.Visible = true;
                    btnNext.Visible = false;
                    btnReleasetoOrder.Visible = false;
                    btnContinueShopping.Visible = true;
                    this.txtTransferUser.Visible = false;
                    this.btnTransfer.Visible = false;
                    this.fTransferUser.Visible = false;
                    break;
                default:
                    btnRevise.Visible = false;
                    btnNext.Visible = false;
                    btnContinueShopping.Visible = true;
                    btnReleasetoOrder.Visible = false;
                    this.txtTransferUser.Visible = false;
                    this.btnTransfer.Visible = false;
                    this.fTransferUser.Visible = false;
                    break;

            }
            //eQuotation
            switch (Presentation.eStoreContext.Current.Quotation.Source)
            {
                case POCOS.Quotation.QuoteSource.eQuotation:
                    this.pThankyouInfo.Visible = false;
                    btnRevise.Visible = false;
                    btnNext.Visible = false;
                    btnContinueShopping.Visible = false;
                    this.fTransferUser.Visible = false;
                    break;
                case POCOS.Quotation.QuoteSource.eStore:
                default:
                    break;
            }
        }
    }
}