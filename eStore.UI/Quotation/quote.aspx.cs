using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Quotation
{
    public partial class quote : eStoreBaseQuotationPage
    {
        protected override eStoreBaseQuotationPage.PageStep minStepNo
        {
            get
            {
                return PageStep.ShppingCart;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.Quotation != null)
            {
                if (!Presentation.eStoreContext.Current.Quotation.isModifiable())
                    Response.Redirect("~/Quotation/confirm.aspx");
            }
            else
                Response.Redirect("~/Default.aspx?needlogin=true");            

            this.CartContent1.CartDataSource = Presentation.eStoreContext.Current.Quotation;
            lSubPrice.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
            {
                if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != Presentation.eStoreContext.Current.Quotation.cartX.Currency)
                    lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount, Presentation.eStoreContext.Current.Quotation.cartX.currencySign));
            }
            ChangeCurrency1.defaultListPrice = Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount;

            if (!IsPostBack)
            {
                if(Presentation.eStoreContext.Current.Quotation != null 
                    && Presentation.eStoreContext.Current.Quotation.isModifiable())
                        //&& (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.Open || Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.NeedTaxIDReview))
                {
                    if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
                    {
                        if (Presentation.eStoreContext.Current.User != null)
                        {
                            this.VATSetting1.VATValidStatus = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATValidStatus;

                            if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe))
                                this.VATSetting1.VATNumber = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe;

                            this.VATSetting1.RegistrationNumber = Presentation.eStoreContext.Current.User.actingUser.mainContact.RegistrationNumber;
                        }
                    }
                } 



                btnSetContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Next); //Bind Translation
                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Quotation.Comments) && txtComment.Text.Trim() != this.txtComment.ToolTip)
                {
                    this.txtComment.Text = Presentation.eStoreContext.Current.Quotation.Comments;
                }
                if (Presentation.eStoreContext.Current.User.actingUser.role == POCOS.User.Role.Employee)
                { pAddAddtionParts.Visible = true; }
                else
                    pAddAddtionParts.Visible = false;

                if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
                {
                    this.VATSetting1.Visible = true;
                    this.btnSetContact.Attributes.Add("onclick", "if(checkSubQTYInfo()){return vatvalidation();}else{return false;};");
                }
                else
                {
                    this.btnSetContact.Attributes.Add("onclick", "return checkSubQTYInfo();");
                    this.VATSetting1.Visible = false;
                }

                bindFonts();

                eStore.UI.Modules.SuggestingProductsAds suggestPro = this.LoadControl("~/Modules/SuggestingProductsAds.ascx") as eStore.UI.Modules.SuggestingProductsAds;
                if (eStore.Presentation.eStoreContext.Current.MiniSite != null && 
                        (eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.IotMart || eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop))
                    phSuggestProsCenter.Controls.Add(suggestPro);
                else
                    phSuggestProsRight.Controls.Add(suggestPro);
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            decimal discountAmount = Presentation.eStoreContext.Current.Quotation.cartX.CartItems.Sum(x => x.DiscountAmount).Value;
            if (discountAmount > 0)
            {
                this.pDiscount.Visible = true;
                this.lTotalDiscount.Text = "-" + Presentation.Product.ProductPrice.FormartPriceWithDecimal(discountAmount);
            }
            else
            {
                this.pDiscount.Visible = false;
            }
         
        }
        protected void btnSetContact_Click(object sender, EventArgs e)
        {
            if (eStore.Presentation.eStoreContext.Current.MiniSite != null)
                Presentation.eStoreContext.Current.Quotation.cartX.MiniSiteID = eStore.Presentation.eStoreContext.Current.MiniSite.ID;

            this.CartContent1.UpdateCartContent();

            if (Presentation.eStoreContext.Current.Quotation.isBelowCost())
            {
                //show error message
                eStoreContext.Current.AddStoreErrorCode("Price Error");
            }
            else
            {
                Presentation.eStoreContext.Current.Quotation.ShipmentTerm = string.Empty;
                Presentation.eStoreContext.Current.Quotation.ShippingMethod = string.Empty;
                Presentation.eStoreContext.Current.Quotation.Freight = 0;

                if (Presentation.eStoreContext.Current.User.actingUser.mainContact != null)
                {
                    if (Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact == null)
                        if (Presentation.eStoreContext.Current.User.actingUser.mainContact != null
                            && eStoreContext.Current.Store.isValidatedShiptoAddress(Presentation.eStoreContext.Current.User.actingUser.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                            Presentation.eStoreContext.Current.Quotation.cartX.setShipTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                    if (Presentation.eStoreContext.Current.Quotation.cartX.SoldToContact == null)
                        Presentation.eStoreContext.Current.Quotation.cartX.setSoldTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                    if (Presentation.eStoreContext.Current.Quotation.cartX.BillToContact == null
                        && eStoreContext.Current.Store.isValidatedBilltoAddress(Presentation.eStoreContext.Current.User.actingUser.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                        Presentation.eStoreContext.Current.Quotation.cartX.setBillTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                }
                if (!string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
                {
                    Presentation.eStoreContext.Current.Quotation.Comments = this.txtComment.Text;
                }

                if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting")
                    && !string.IsNullOrEmpty(this.VATSetting1.VATNumber))
                {
                    Presentation.eStoreContext.Current.Quotation.VATNumber = this.VATSetting1.VATNumber;
                    Presentation.eStoreContext.Current.Quotation.RegistrationNumber = this.VATSetting1.RegistrationNumber;
                    if (this.VATSetting1.VATValidStatus != POCOS.Contact.VATValidResult.VALID)
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;

                    if (Presentation.eStoreContext.Current.User.actingUser != null && Presentation.eStoreContext.Current.User.actingUser.mainContact != null)
                    {
                        Presentation.eStoreContext.Current.User.mainContact.VATValidStatus = this.VATSetting1.VATValidStatus;

                        Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe = this.VATSetting1.VATNumber;
                        Presentation.eStoreContext.Current.User.actingUser.mainContact.RegistrationNumber = this.VATSetting1.RegistrationNumber;
                        Presentation.eStoreContext.Current.User.actingUser.mainContact.save();
                    }
                }

                if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
                    Presentation.eStoreContext.Current.Store.savePopularModelLogByQuotation(Presentation.eStoreContext.Current.Quotation);

                Response.Redirect("~/Quotation/Contact.aspx");
            }
        }
        protected void btnUpdateAll_Click(object sender, EventArgs e)
        {
            this.CartContent1.UpdateCartContent();

            Presentation.eStoreContext.Current.Quotation.ShipmentTerm = string.Empty;
            Presentation.eStoreContext.Current.Quotation.ShippingMethod = string.Empty;
            Presentation.eStoreContext.Current.Quotation.Freight = 0;

            lSubPrice.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
            {
                if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != Presentation.eStoreContext.Current.Quotation.cartX.Currency)
                    lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount, Presentation.eStoreContext.Current.Quotation.cartX.currencySign));
            }
            ChangeCurrency1.defaultListPrice = Presentation.eStoreContext.Current.Quotation.cartX.TotalAmount;
        }

        protected void bindFonts()
        {
            btnUpdateAll.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Update_Quote);
            btnSetContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Next);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Continue_Shopping);
            txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
            QuotationNavigator1.NavigatorTitle = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Items);
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            string _localUrl = esUtilities.CommonHelper.GetStoreLocation();
            Response.Redirect(_localUrl.EndsWith("/") ? _localUrl.Substring(0, _localUrl.Length - 1) : _localUrl);
        }
    }
}