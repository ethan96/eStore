using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using eStore.BusinessModules.TaxService;
using esUtilities;

namespace eStore.UI.Quotation
{
    public partial class Contact : eStoreBaseQuotationPage
    {

        protected override eStoreBaseQuotationPage.PageStep minStepNo
        {
            get
            {
                return PageStep.SelectContacts;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Quotation == null)
            { Response.Redirect("~/"); }
 
            this.ContactSelector1.cart = Presentation.eStoreContext.Current.Quotation.cartX;
            this.ShippingCalculator1.cart = Presentation.eStoreContext.Current.Quotation.cartX;
            if (Presentation.eStoreContext.Current.Store.offerShippingService)
            {
                this.ShippingCalculator1.cart = Presentation.eStoreContext.Current.Quotation.cartX;
                this.ShippingCalculator1.shippingMethod = Presentation.eStoreContext.Current.Quotation.ShippingMethod;
                this.ShippingCalculator1.CourierAccount = Presentation.eStoreContext.Current.Quotation.ShipmentTerm;
                this.ShippingCalculator1.Visible = true;
                if (!IsPostBack)
                {
                    if(!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.ShipmentTerm))
                        this.ShippingCalculator1.isCustomerCourier = true;
                    this.ShippingCalculator1.CalculateShippingRate();

                    //Check ship to address is correct
                    if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true && Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact != null)
                    {
                        string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
                        if (!string.IsNullOrEmpty(ShippingCalculator1.shippingMethod))
                            shippingmethods = ShippingCalculator1.shippingMethod.ToUpper();

                        var result = Presentation.eStoreContext.Current.Store.isValidateUSAShiptoAddress(Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact, shippingmethods, Presentation.eStoreContext.Current.User);
                        if (result.isValid == false)
                        {
                            if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                            {
                                Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.POBOX.ToString();
                                Presentation.eStoreContext.Current.AddStoreErrorCode(Presentation.eStoreLocalization.Tanslation(result.TranslationKey));
                            }
                            else
                            {
                                Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.Unknown.ToString();
                                Presentation.eStoreContext.Current.AddStoreErrorCode(Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_Unknown));
                            }
                        }
                        else
                            Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.Valid.ToString();
                    }
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

        protected void bindFonts()
        {
            btnNext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Next);
            btnBack.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Back);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Continue_Shopping);
            //btnContinueShopping.PostBackUrl = esUtilities.CommonHelper.GetStoreLocation();
            QuotationNavigator1.NavigatorTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Navigation_Contacts);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!ContactSelector1.setCartContact())
                return;
            if (Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.isVerifyState())
            {
                eStore.Presentation.eStoreContact cc = new Presentation.eStoreContact(Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact);
                ContactSelector1.editContact(cc, "ship");
                BindScript("script", "showContactError", "$(function() { alert('" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ScriptMessage_Please_Select_State_Province) + "'); });");
                return;
            }
            if (Presentation.eStoreContext.Current.Quotation.isModifiable())
                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;

            if (Presentation.eStoreContext.Current.Store.offerShippingService)
            {
                ShippingMethod sm = ShippingCalculator1.CalculateShippingRate(true);
                if (this.ShippingCalculator1.isAvailable == false)
                {
                    if (Presentation.eStoreContext.Current.Store.offerNoShippingMothed) //没有配送方式，
                    {
                        Presentation.eStoreContext.Current.Quotation.ShipmentTerm = string.Empty;
                        Presentation.eStoreContext.Current.Quotation.ShippingMethod = string.Empty;
                        Presentation.eStoreContext.Current.Quotation.Freight = 0;
                        Presentation.eStoreContext.Current.Quotation.FreightDiscount = 0;
                        Presentation.eStoreContext.Current.Quotation.TotalDiscount = 0;
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("No shipping method is selected");
                        return;
                    }
                }
                else if (sm.Error != null)
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(sm.Error.Code.ToString());
                    return;
                }
                else
                {
                    if (this.ShippingCalculator1.isCustomerCourier)
                        Presentation.eStoreContext.Current.Quotation.ShipmentTerm = sm.ServiceCode;

                    //Presentation.eStoreContext.Current.Quotation.ShipmentTerm = sm.ShippingCarrier;
                    Presentation.eStoreContext.Current.Quotation.ShippingMethod = sm.ShippingMethodDescription;
                    Presentation.eStoreContext.Current.Quotation.Freight = (decimal)sm.ShippingCostWithPublishedRate;
                    Presentation.eStoreContext.Current.Quotation.FreightDiscount = (decimal)sm.Discount;
                    //Presentation.eStoreContext.Current.Quotation.TotalDiscount = (decimal)sm.Discount;
                    Presentation.eStoreContext.Current.Quotation.CustomerShippingMethodTemp = ShippingCalculator1.ShippingCarrierValue;
                }

                if (Presentation.eStoreContext.Current.Quotation.FreightDiscount == 0)
                {
                    Presentation.eStoreContext.Current.Quotation.cartX.clearFreeGroupShipmentPromotionStrategy();
                }
                else
                {
                    Presentation.eStoreContext.Current.Quotation.cartX.setFreeGroupShipPromotionStrategy();
                }
           
            }
            else
            {
                Presentation.eStoreContext.Current.Quotation.ShipmentTerm = string.Empty;
                Presentation.eStoreContext.Current.Quotation.ShippingMethod = string.Empty;
                Presentation.eStoreContext.Current.Quotation.Freight = 0;
                Presentation.eStoreContext.Current.Quotation.FreightDiscount = 0;
                //Presentation.eStoreContext.Current.Quotation.TotalDiscount = 0;
            }

            if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true)
            {
                string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
                if (!string.IsNullOrEmpty(ShippingCalculator1.shippingMethod))
                    shippingmethods = ShippingCalculator1.shippingMethod;
                var result = Presentation.eStoreContext.Current.Store.isValidateUSAShiptoAddress(Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact, shippingmethods, Presentation.eStoreContext.Current.User);
                if (result.isValid == false)
                {
                    if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                    {
                        Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.POBOX.ToString();
                        Presentation.eStoreContext.Current.AddStoreErrorCode(Presentation.eStoreLocalization.Tanslation(result.TranslationKey));
                        return; //If address is PO box, then stop placing quotation.
                    }
                    else
                        Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.CustomerConfirmed.ToString(); //Continue shopping
                }
                else
                    Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.ValidationStatus = POCOS.CartContact.AddressValidationStatus.Valid.ToString();
            }

            TaxCalculator taxProvider = Presentation.eStoreContext.Current.Store.calculateTax(Presentation.eStoreContext.Current.Quotation);
            if (taxProvider.Status == false)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                return;
            }
            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
            {
                try
                {
                    if (Presentation.eStoreContext.Current.Quotation.cartX.BillToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID
                        || Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID
                        || Presentation.eStoreContext.Current.Quotation.cartX.SoldToContact.VATValidStatus == POCOS.Contact.VATValidResult.INVALID)
                    {
                        if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                        {
                            Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxAndFreightReview;
                        }
                        else
                            Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                    }
                   
                }
                catch (Exception)
                {
                    if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                    {
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxAndFreightReview;
                    }
                    else
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                }
            }
            Presentation.eStoreContext.Current.Store.getLandedCost(Presentation.eStoreContext.Current.Quotation);

            try
            {
                string promotioncode = string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.PromoteCode)
                    ? (Session["autoAppliedCouponCode"] == null ? "" : Session["autoAppliedCouponCode"].ToString())
                    : Presentation.eStoreContext.Current.Quotation.PromoteCode;
                POCOS.CampaignManager.PromotionCodeStatus promotionCodeStatus = Presentation.eStoreContext.Current.Quotation.applyPromotionCode(Presentation.eStoreContext.Current.User, promotioncode);
            }
            catch (Exception)
            {
            }

            Presentation.eStoreContext.Current.Quotation.updateTotalAmount();
            if (Presentation.eStoreContext.Current.Quotation.isModifiable())
                Presentation.eStoreContext.Current.Quotation.save();
            else
            {
                if (Presentation.eStoreContext.Current.Quotation.needVATReview())
                    Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                else if (Presentation.eStoreContext.Current.Quotation.needFreightReview())
                    Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
            }
            Response.Redirect("~/Quotation/confirm.aspx");


        }
    }
}