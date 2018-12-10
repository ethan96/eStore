using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using eStore.BusinessModules.TaxService;
using eStore.POCOS;
using eStore.Presentation;

namespace eStore.UI.Cart
{
    public partial class Contact : eStoreBaseOrderPage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected override eStoreBaseOrderPage.PageStep minStepNo
        {
            get
            {
                return PageStep.SelectContacts;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ContactSelector1.cart = Presentation.eStoreContext.Current.Order.cartX;
            if (Presentation.eStoreContext.Current.Store.offerShippingService||Presentation.eStoreContext.Current.Store.offerDropShipmentService)
            {
                this.ShippingCalculator1.cart = Presentation.eStoreContext.Current.Order.cartX;
                this.ShippingCalculator1.shippingMethod = Presentation.eStoreContext.Current.Order.ShippingMethod;
                this.ShippingCalculator1.CourierAccount = Presentation.eStoreContext.Current.Order.CourierAccount;
                this.ShippingCalculator1.Visible = true;
                if (!IsPostBack)
                {
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.CourierAccount))
                    {
                        this.ShippingCalculator1.isCustomerCourier = true;
                    }
                    this.ShippingCalculator1.CalculateShippingRate();

                    //Check ship to address is correct
                    if (eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true && eStoreContext.Current.Order.cartX.ShipToContact != null)
                    {
                        string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
                        if (!string.IsNullOrEmpty(ShippingCalculator1.shippingMethod))
                            shippingmethods = ShippingCalculator1.shippingMethod.ToUpper();

                        var result = eStoreContext.Current.Store.isValidateUSAShiptoAddress(eStoreContext.Current.Order.cartX.ShipToContact, shippingmethods, eStoreContext.Current.User);
                        if (result.isValid == false)
                        {
                            if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                            {
                                eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.POBOX.ToString();
                                eStoreContext.Current.AddStoreErrorCode(eStoreLocalization.Tanslation(result.TranslationKey));
                            }
                            else
                            {
                                eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.Unknown.ToString();
                                eStoreContext.Current.AddStoreErrorCode(eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Unknown));
                            }
                        }
                        else
                            eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.Valid.ToString();
                    }

                    //2017/02/23 test ehance Ecommerce
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureCheckout", GoogleGAHelper.MeasureCheckout(Presentation.eStoreContext.Current.Order, 1).ToString(), true);
                }
            }
            else
            {
                this.ShippingCalculator1.Visible = false;
            }
            if (!IsPostBack)
            {
                bindFonts();
            }
            this.BindScript("url", "jsvat", "jsvat.js");
        }

        private string orderChannelID;
        public string OrderChannelID
        {
            get 
            {
                if (orderChannelID == null)
                {
                    if (Presentation.eStoreContext.Current.Order.ChannelID.HasValue)
                        orderChannelID = Presentation.eStoreContext.Current.Order.ChannelID.Value.ToString();
                    else
                        orderChannelID = "";
                }
                return orderChannelID; 
            }
        }

        private string orderChannelName;
        public string OrderChannelName
        {
            get
            {
                if (orderChannelName == null)
                {
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.ChannelName))
                        orderChannelName = Presentation.eStoreContext.Current.Order.ChannelName;
                    else
                        orderChannelName = "N/A";
                }
                return orderChannelName;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            //set channelPartner
            string channelId = Request["hChannelID"];
            string channelName = Request["hChannelName"];
            if (!string.IsNullOrEmpty(channelId) && !string.IsNullOrEmpty(channelName))
            {
                Presentation.eStoreContext.Current.Order.ChannelID = int.Parse(channelId);
                Presentation.eStoreContext.Current.Order.ChannelName = channelName;
                Presentation.eStoreContext.Current.Order.Channel = eStore.Presentation.eStoreContext.Current.Store.getChannelPartner(int.Parse(channelId));
            }

            if (!ContactSelector1.setCartContact())
                return;
            if (Presentation.eStoreContext.Current.Order.cartX.ShipToContact.isVerifyState())
            {
                eStore.Presentation.eStoreContact cc = new Presentation.eStoreContact(Presentation.eStoreContext.Current.Order.cartX.ShipToContact);
                ContactSelector1.editContact(cc, "ship");
                BindScript("script", "showContactError", "$(function() { alert('" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ScriptMessage_Please_Select_State_Province) + "'); });");
                return;
            }
            //设置order状态重新验证vat和Freight
            Presentation.eStoreContext.Current.Order.statusX = POCOS.Order.OStatus.Open;
            Presentation.eStoreContext.Current.Order.setAlert(POCOS.Order.OAlert.None);

            if (Presentation.eStoreContext.Current.Store.offerShippingService && !Presentation.eStoreContext.Current.Order.isReferredToChannelPartner())
            {
                ShippingMethod sm = ShippingCalculator1.CalculateShippingRate(true);

                if (this.ShippingCalculator1.isAvailable == false)
                {
                    if (Presentation.eStoreContext.Current.Store.offerNoShippingMothed) //没有配送方式，
                    {
                        Presentation.eStoreContext.Current.Order.ShipmentTerm = string.Empty;
                        Presentation.eStoreContext.Current.Order.ShippingMethod = string.Empty;
                        Presentation.eStoreContext.Current.Order.Freight = 0;
                        Presentation.eStoreContext.Current.Order.FreightDiscount = 0;
                        //Presentation.eStoreContext.Current.Order.TotalDiscount = 0;
                        Presentation.eStoreContext.Current.Order.statusX = Order.OStatus.NeedFreightReview;
                        Presentation.eStoreContext.Current.Order.setAlert(Order.OAlert.NeedFreightReview);
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("No shipping method is selected");
                        return;
                    }
                }
                else
                {
                    if (this.ShippingCalculator1.isCustomerCourier)
                        Presentation.eStoreContext.Current.Order.CourierAccount = sm.ServiceCode;

                    Presentation.eStoreContext.Current.Order.Courier = sm.ShippingCarrier;
                    Presentation.eStoreContext.Current.Order.ShippingMethod = sm.ShippingMethodDescription;
                    Presentation.eStoreContext.Current.Order.Freight = eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice((decimal)sm.ShippingCostWithPublishedRate,eStore.Presentation.eStoreContext.Current.CurrentCurrency).value;
                    Presentation.eStoreContext.Current.Order.FreightDiscount = eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice((decimal)sm.Discount, eStore.Presentation.eStoreContext.Current.CurrentCurrency).value;
                    //Presentation.eStoreContext.Current.Order.TotalDiscount = (decimal)sm.Discount;
                    Presentation.eStoreContext.Current.Order.CustomerShippingMethodTemp = ShippingCalculator1.ShippingCarrierValue;
                }
                if (Presentation.eStoreContext.Current.Order.FreightDiscount == 0)
                {
                    Presentation.eStoreContext.Current.Order.cartX.clearFreeGroupShipmentPromotionStrategy();
                }
                else
                {
                    Presentation.eStoreContext.Current.Order.cartX.setFreeGroupShipPromotionStrategy();
                }
            }
            else
            {
                Presentation.eStoreContext.Current.Order.ShipmentTerm = string.Empty;
                Presentation.eStoreContext.Current.Order.ShippingMethod = string.Empty;
                Presentation.eStoreContext.Current.Order.Freight = 0;
                Presentation.eStoreContext.Current.Order.FreightDiscount = 0;
                //Presentation.eStoreContext.Current.Order.TotalDiscount = 0;
            }

            //Check ship to address is correct
            if (eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true)
            {
                string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
                if (!string.IsNullOrEmpty(ShippingCalculator1.shippingMethod))
                    shippingmethods = ShippingCalculator1.shippingMethod;
                var result = eStoreContext.Current.Store.isValidateUSAShiptoAddress(eStoreContext.Current.Order.cartX.ShipToContact, shippingmethods, eStoreContext.Current.User);
                if (result.isValid == false)
                {
                    if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                    {
                        eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.POBOX.ToString();
                        eStoreContext.Current.AddStoreErrorCode(eStoreLocalization.Tanslation(result.TranslationKey));
                        return; //If address is PO box, then stop placing order.
                    }
                    else
                        eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.CustomerConfirmed.ToString(); //Continue shopping
                }
                else
                    eStoreContext.Current.Order.cartX.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.Valid.ToString();
            }

            TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Order);
            if (taxProvider.Status == false)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                return;
            }

            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
            {
                try
                {
                    if (Presentation.eStoreContext.Current.Order.cartX.BillToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID
                        || Presentation.eStoreContext.Current.Order.cartX.ShipToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID
                        || Presentation.eStoreContext.Current.Order.cartX.SoldToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID)
                    {
                        if (Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedFreightReview)
                        {
                            Presentation.eStoreContext.Current.Order.statusX = POCOS.Order.OStatus.NeedTaxAndFreightReview;
                            Presentation.eStoreContext.Current.Order.setAlert(POCOS.Order.OAlert.NeedTaxAndFreightReview);
                        }
                        else
                        {
                            Presentation.eStoreContext.Current.Order.statusX = POCOS.Order.OStatus.NeedTaxIDReview;
                            Presentation.eStoreContext.Current.Order.setAlert(POCOS.Order.OAlert.NeedTaxIDReview);
                        }
                    }
                }
                catch (Exception)
                {
                    if (Presentation.eStoreContext.Current.Order.statusX == Order.OStatus.NeedFreightReview)
                    {
                        Presentation.eStoreContext.Current.Order.statusX = POCOS.Order.OStatus.NeedTaxAndFreightReview;
                        Presentation.eStoreContext.Current.Order.setAlert(POCOS.Order.OAlert.NeedTaxAndFreightReview);
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.Order.statusX = POCOS.Order.OStatus.NeedTaxIDReview;
                        Presentation.eStoreContext.Current.Order.setAlert(POCOS.Order.OAlert.NeedTaxIDReview);
                    }
                }
            }
            
            Presentation.eStoreContext.Current.Store.getLandedCost(Presentation.eStoreContext.Current.Order);

            try
            {
                string promotioncode = string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PromoteCode)
                    ? (Session["autoAppliedCouponCode"] == null ? "" : Session["autoAppliedCouponCode"].ToString())
                    : Presentation.eStoreContext.Current.Order.PromoteCode;
                if (!string.IsNullOrEmpty(promotioncode) || (Presentation.eStoreContext.Current.Order.FreightDiscount == 0))//if free shipping is not avaiable, check other auto applied promotions.
                {
                    POCOS.CampaignManager.PromotionCodeStatus promotionCodeStatus = Presentation.eStoreContext.Current.Order.applyPromotionCode(Presentation.eStoreContext.Current.User, promotioncode);
                    if (promotionCodeStatus == CampaignManager.PromotionCodeStatus.Valid)
                        Presentation.eStoreContext.Current.Order.save();
                }
            }
            catch (Exception)
            {
            }

            Presentation.eStoreContext.Current.Order.updateTotalAmount();
            Presentation.eStoreContext.Current.Order.save();
            Response.Redirect("~/Cart/confirm.aspx");

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
            { Response.Redirect("~/Cart/Cart.aspx"); }
            else
            { Response.Redirect("~/Quotation/confirm.aspx"); }
        }

        protected void bindFonts()
        {
            btnNext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Next);
            btnback.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Back);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Continue_Shopping);
        }
    }
}