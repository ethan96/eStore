using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eStore.UI.Cart
{
    public partial class Cart : eStoreBaseOrderPage
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
                return PageStep.ShppingCart;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current.Order != null && Presentation.eStoreContext.Current.Order.statusX == POCOS.Order.OStatus.Open)
                {
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.CustomerComment))
                        this.txtComment.Text = Presentation.eStoreContext.Current.Order.CustomerComment;
                }

                if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
                {
                    if (Presentation.eStoreContext.Current.User != null)
                    {
                        this.VATSetting1.VATValidStatus = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATValidStatus;

                        if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe))
                            this.VATSetting1.VATNumber = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe;

                        this.VATSetting1.RegistrationNumber = Presentation.eStoreContext.Current.User.actingUser.mainContact.RegistrationNumber;
                    }
                    this.VATSetting1.Visible = true;

                    this.btnSetContact.Attributes.Add("onclick", "if(checkSubQTYInfo()){return vatvalidation();}else{return false;};");
                }
                else
                {
                    this.btnSetContact.Attributes.Add("onclick", "return checkSubQTYInfo();");
                    this.VATSetting1.Visible = false;
                }
                BindTranslationFonts();
            }



            this.CartContent1.CartDataSource = Presentation.eStoreContext.Current.UserShoppingCart;
            eStore.UI.Modules.SuggestingProductsAds suggestPro = this.LoadControl("~/Modules/SuggestingProductsAds.ascx") as eStore.UI.Modules.SuggestingProductsAds;
            //if (eStore.Presentation.eStoreContext.Current.MiniSite != null &&
            //    (eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.IotMart || eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop))
            phSuggestProsCenter.Controls.Add(suggestPro);
            //else
            //{
            //    //phSuggestProsRight.Controls.Add(suggestPro);
            //}
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            decimal discountAmount = Presentation.eStoreContext.Current.UserShoppingCart.CartItems.Sum(x => x.DiscountAmount).Value;
            if (discountAmount > 0)
            {
                this.pDiscount.Visible = true;
                this.lTotalDiscount.Text = "-" + Presentation.Product.ProductPrice.FormartPriceWithDecimal(discountAmount);
            }
            else
            {
                this.pDiscount.Visible = false;
            }
            lSubPrice.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
            {
                if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != Presentation.eStoreContext.Current.UserShoppingCart.Currency)
                    lSubStorePrice.Text = string.Format("<span><br />({0})</span>", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount, Presentation.eStoreContext.Current.UserShoppingCart.currencySign));
            }
            ChangeCurrency1.defaultListPrice = Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount;
        }
        private void BindTranslationFonts()
        {
            btnSetContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Checkout);
            btnAdd2Quote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Add_to_Quote);
            btnContinueShopping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Button_Continue_Shopping);
            //btnUpdateAll.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Update_Cart);
            lb_Comment.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Comment);
            txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
            txtComment.Attributes.Add("placeholder", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special));
        }

        protected void btnSetContact_Click(object sender, EventArgs e)
        {
            this.CartContent1.UpdateCartContent();

            POCOS.Cart cart = eStoreContext.Current.UserShoppingCart;
            if (cart.isBelowCost())
            {
                //show error message
                eStoreContext.Current.AddStoreErrorCode("Pricing Error");
            }
            else
            {
                POCOS.Order order = cart.checkOut(cart.userX, eStore.Presentation.eStoreContext.Current.MiniSite == null ? 0 : eStore.Presentation.eStoreContext.Current.MiniSite.ID);
                if (order != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
                    order.CustomerComment = this.txtComment.Text;

                if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting")
                    && !string.IsNullOrEmpty(this.VATSetting1.VATNumber))
                {
                    order.VATNumbe = this.VATSetting1.VATNumber;
                    order.RegistrationNumber = this.VATSetting1.RegistrationNumber;
                    if (this.VATSetting1.VATValidStatus != POCOS.Contact.VATValidResult.VALID)
                    {
                        order.statusX = POCOS.Order.OStatus.NeedTaxIDReview;
                        order.setAlert(POCOS.Order.OAlert.NeedTaxIDReview);
                    }
                    //Presentation.eStoreContext.Current.User.FederalID = this.VATSetting1.VATNumber;
                    //Presentation.eStoreContext.Current.User.save();
                    if (Presentation.eStoreContext.Current.User.actingUser != null && Presentation.eStoreContext.Current.User.actingUser.mainContact != null)
                    {
                        Presentation.eStoreContext.Current.User.mainContact.VATValidStatus = this.VATSetting1.VATValidStatus;

                        Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe = this.VATSetting1.VATNumber;
                        Presentation.eStoreContext.Current.User.actingUser.mainContact.RegistrationNumber = this.VATSetting1.RegistrationNumber;
                        Presentation.eStoreContext.Current.User.actingUser.mainContact.save();
                    }
                }

                if (Presentation.eStoreContext.Current.getBooleanSetting("BBFlag", false) == true)
                    eStoreContext.Current.Store.AdjustOrderAndCartTime(order);

                if (order.cartX.ShipToContact == null && eStoreContext.Current.Store.isValidatedShiptoAddress(cart.userX.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                    order.cartX.setShipTo(cart.userX.mainContact);
                if (order.cartX.SoldToContact == null)
                    order.cartX.setSoldTo(cart.userX.mainContact);
                if (order.cartX.BillToContact == null && eStoreContext.Current.Store.isValidatedBilltoAddress(cart.userX.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                    order.cartX.setBillTo(cart.userX.mainContact);

                if (order.save() == 0)
                {
                    eStoreContext.Current.Order = order;
                    if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
                        Presentation.eStoreContext.Current.Store.savePopularModelLogByOrder(Presentation.eStoreContext.Current.Order);
                    Response.Redirect("~/Cart/Contact.aspx");
                }
                else
                {
                    eStoreContext.Current.AddStoreErrorCode("Order Save Error",null,true);
                }
                    
                
            }
        }

        protected void btnAdd2Quote_Click(object sender, EventArgs e)
        {
            if (!Presentation.eStoreContext.Current.Quotation.isModifiable())//get new quote
                Presentation.eStoreContext.Current.Quotation = null;
            Presentation.eStoreContext.Current.Quotation.mergeCartItems(Presentation.eStoreContext.Current.UserShoppingCart);
            Presentation.eStoreContext.Current.UserShoppingCart.CartItems.Clear();
             Presentation.eStoreContext.Current.UserShoppingCart.save();
             Presentation.eStoreContext.Current.Quotation.save();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void btnUpdateAll_Click(object sender, EventArgs e)
        {
            this.CartContent1.UpdateCartContent();
            lSubPrice.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
            {
                if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != Presentation.eStoreContext.Current.UserShoppingCart.Currency)
                    lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount, Presentation.eStoreContext.Current.UserShoppingCart.currencySign));
            }
            ChangeCurrency1.defaultListPrice = Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount;
            ChangeCurrency1.BindData();
        }

        protected void btnContinueShopping_Click(object sender, EventArgs e)
        {
            string _localUrl = esUtilities.CommonHelper.GetStoreLocation();
            Response.Redirect(_localUrl.EndsWith("/") ? _localUrl.Substring(0, _localUrl.Length - 1) : _localUrl);
        }
    }
}