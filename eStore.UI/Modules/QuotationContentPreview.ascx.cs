using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.BusinessModules;
using eStore.BusinessModules.TaxService;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    public partial class QuotationContentPreview : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Quotation quotation { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            if (quotation != null)
            {

                this.CartContentPreview1.cart = quotation.cartX;
                this.ShipToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Ship_to);
                this.ShipToContact.cartContact = quotation.cartX.ShipToContact;
                this.BillToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Bill_to);
                this.BillToContact.cartContact = quotation.cartX.BillToContact;
                this.SoldToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Sold_to);
                this.SoldToContact.cartContact = quotation.cartX.SoldToContact;


                this.lShippingMethod.Text = quotation.ShippingMethod;
                this.lFreight.Text = Presentation.Product.ProductPrice.FormartFreight(quotation.Freight, quotation.currencySign);
                this.lTax.Text = Presentation.Product.ProductPrice.FormartTax(quotation.Tax, quotation.currencySign);
                string taxMessage = Presentation.eStoreContext.Current.Store.specialTaxMessage(quotation.cartX.ShipToContact);
                ltOrderMessage.Text = string.IsNullOrEmpty(taxMessage) ? "" : string.Format("<br /><span class=\"colorRed\">{0}</span>", taxMessage);
                this.lSubTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(quotation.cartX.TotalAmount, quotation.currencySign);
                this.lDiscount.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(quotation.TotalDiscount.GetValueOrDefault() * (-1), quotation.currencySign);
                this.lDutyAndTax.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(quotation.DutyAndTax.GetValueOrDefault(), quotation.currencySign);
                this.lTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(quotation.totalAmountX, quotation.currencySign);
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
                {
                    if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != quotation.cartX.Currency)
                        lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(quotation.totalAmountX, quotation.cartX.currencySign));
                }
                if (!string.IsNullOrEmpty(quotation.Comments))
                {
                    this.pComment.Visible = true;
                    this.lcomment.Text = quotation.Comments;
                }
                else
                { this.pComment.Visible = false; }

                if (quotation.statusX == eStore.POCOS.Quotation.QStatus.Confirmed)
                {
                    BillToContact.ShowEdit = false;
                    SoldToContact.ShowEdit = false;
                    ShipToContact.ShowEdit = false;
                }
            }
            base.OnPreRender(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ShipToContact.eEditContact += EditContact;
            ShipToContact.EditContactCommName = "Ship";

            BillToContact.eEditContact += EditContact;
            BillToContact.EditContactCommName = "Bill";

            SoldToContact.eEditContact += EditContact;
            SoldToContact.EditContactCommName = "Sold";
        }

        public void EditContact(object sender, EventArgs e)
        {
            var bt = sender as Button;
            if (bt != null)
            {
                eStoreContact esc = new eStoreContact();
                switch (bt.CommandName)
                {
                    case "Ship":
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact);
                        break;
                    case "Bill":
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Quotation.cartX.BillToContact);
                        break;
                    case "Sold":
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Quotation.cartX.SoldToContact);
                        break;
                }
                ContactDetailsBill.setContact(esc, true);
                btSaveBillto.CommandArgument = esc.AddressID;
                BindScript("script", "showAddress", "$(function() { showAddress('" + bt.CommandName + "'); });");

            }
        }

        protected void btnSaveUserContact_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string addressid = button.CommandArgument.ToString();
            Presentation.eStoreContact contact = ContactDetailsBill.Contact;
            var contactType = this.hdContactType.Value;
            if (!string.IsNullOrEmpty(contactType))
            {
                switch (contactType)
                {
                    case "Bill":
                        contact.isBillTo = true;
                        break;
                    case "Ship":
                        contact.isShipTo = true;
                        break;
                    case "Sold":
                        contact.isSoldTo = true;
                        break;
                }
            }

            if (contact.contactType == ContactType.Contact || contact.contactType == ContactType.CartContact)
            {
                POCOS.Contact cNew = Presentation.eStoreContext.Current.User.actingUser.updateContact(contact.AddressID, contact.AttCompanyName, contact.FirstName, contact.LastName, contact.Country, contact.State, contact.City, contact.ZipCode, contact.Address1, contact.TelNo, contact.Address2, contact.TelExt);
                cNew.FaxNo = contact.FaxNo;
                cNew.Mobile = contact.Mobile;
                cNew.Address2 = contact.Address2;

                cNew.VATNumbe = contact.VATNumbe;
                cNew.LegalForm = contact.LegalForm;
                cNew.VATValidStatus = contact.VATValidStatus;

                Presentation.eStoreContext.Current.User.save();
                if (contact.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID)
                    Presentation.eStoreContext.Current.Store.syncStoreUserToSSOUserProfile(Presentation.eStoreContext.Current.User.actingUser, BusinessModules.Store.SSO_Update_Type.Contact);
            }
            //addContactAndSetDefault(contact);
            SetCartShipToContact(contact);

            if (contact.isShipTo)
            {

                if (Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact.isVerifyState())
                {
                    BindScript("script", "showContactError",
                        "$(function() { alert('" +
                        eStore.Presentation.eStoreLocalization.Tanslation(
                            eStore.POCOS.Store.TranslationKey.ScriptMessage_Please_Select_State_Province) + "'); });");
                    return;
                }

                if (Presentation.eStoreContext.Current.Store.offerShippingService)
                {
                    var cart = Presentation.eStoreContext.Current.Quotation.cartX;
                    var currentQuotation = Presentation.eStoreContext.Current.Quotation;
                    ShippingMethod sm;

                    switch (currentQuotation.CustomerShippingMethodTemp)
                    {
                        case "Dropoff":
                        case "Recommend":
                            List<ShippingMethod> sms;
                            try
                            {
                                sms = eStoreContext.Current.Store.getAvailableShippingMethods(cart,
                                    currentQuotation.CustomerShippingMethodTemp == "Dropoff");
                            }
                            catch (Exception ex)
                            {
                                sms = null;
                                Utilities.eStoreLoger.Error("Can not get shipping",
                                    Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(),
                                    Presentation.eStoreContext.Current.Store.storeID, ex);
                                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                            }
                            if (sms == null || sms.Count == 0)
                            {
                                Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                                // show error message don't get shipping method list
                                Presentation.eStoreContext.Current.AddStoreErrorCode("Can't get shipping method list");
                                return;
                            }
                            else
                            {
                                sm = sms.FirstOrDefault(t => t.ShippingMethodDescription == currentQuotation.ShippingMethod);
                                if (sm == null)
                                {
                                    Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                                    // show error message new address don't get current shipping method
                                    Presentation.eStoreContext.Current.AddStoreErrorCode("Can't get current shipping method");
                                    return;
                                }
                                else
                                {
                                    var oldFreight =
                                        new
                                        {
                                            Freight = Presentation.eStoreContext.Current.Quotation.Freight,
                                            FreightDiscount = Presentation.eStoreContext.Current.Quotation.FreightDiscount
                                        };
                                    Presentation.eStoreContext.Current.Quotation.Freight =
                                        eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(
                                            (decimal)sm.ShippingCostWithPublishedRate,
                                            eStore.Presentation.eStoreContext.Current.CurrentCurrency).value;
                                    Presentation.eStoreContext.Current.Quotation.FreightDiscount =
                                        eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(
                                            (decimal)sm.Discount, eStore.Presentation.eStoreContext.Current.CurrentCurrency)
                                            .value;
                                    TaxCalculator taxProvider =
                                        Presentation.eStoreContext.Current.Store.calculateTax(
                                            Presentation.eStoreContext.Current.Quotation);
                                    if (taxProvider.Status == false)
                                    {
                                        Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                                        return;
                                    }
                                    Presentation.eStoreContext.Current.Quotation.updateTotalAmount();
                                    Presentation.eStoreContext.Current.Quotation.save();
                                    Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
                                    if (Presentation.eStoreContext.Current.Quotation.Freight != oldFreight.Freight)
                                        Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format("Freight changed from {0} to {1}",
                                            oldFreight.Freight, Presentation.eStoreContext.Current.Quotation.Freight));
                                    quotation = Presentation.eStoreContext.Current.Quotation;
                                }
                            }
                            break;
                    }
                }
            }
        }

        protected void SetCartShipToContact(eStoreContact newContact)
        {
            //unselect current ShipTo
            if (newContact != null && eStoreContext.Current.Store.isValidatedShiptoAddress(newContact.CountryCode, Presentation.eStoreContext.Current.User))
            {
                var contactls = Presentation.UserContactManager.GeteStoreContacts(Presentation.eStoreContext.Current.Quotation.cartX);
                eStoreContact currentContact;
                if (newContact.isBillTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isBillTo);
                    if (currentContact != null)
                        currentContact.isBillTo = false;
                    Presentation.eStoreContext.Current.Quotation.cartX.setBillTo(newContact.toCartContact());
                }
                else if (newContact.isShipTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isShipTo);
                    if (currentContact != null)
                        currentContact.isShipTo = false;
                    Presentation.eStoreContext.Current.Quotation.cartX.setShipTo(newContact.toCartContact());
                }
                else if (newContact.isSoldTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isSoldTo);
                    if (currentContact != null)
                        currentContact.isSoldTo = false;
                    Presentation.eStoreContext.Current.Quotation.cartX.setSoldTo(newContact.toCartContact());
                }
                this.ShipToContact.cartContact = Presentation.eStoreContext.Current.Quotation.cartX.ShipToContact;
                this.BillToContact.cartContact = Presentation.eStoreContext.Current.Quotation.cartX.BillToContact;
                this.SoldToContact.cartContact = Presentation.eStoreContext.Current.Quotation.cartX.SoldToContact;
            }
        }
    }
}