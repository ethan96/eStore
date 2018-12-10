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
    public partial class OrderContentPreview : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Order order { get; set; }
        protected override void OnPreRender(EventArgs e)
        {
            if (order != null)
            {
                //this.lOrderDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)order.OrderDate);


                this.CartContentPreview1.cart = order.cartX;
                this.ShipToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Ship_to);
                this.ShipToContact.cartContact = order.cartX.ShipToContact;
                this.BillToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Bill_to);
                this.BillToContact.cartContact = order.cartX.BillToContact;
                this.SoldToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Sold_to);
                this.SoldToContact.cartContact = order.cartX.SoldToContact;

                this.lShippingMethod.Text = order.ShippingMethod;
                this.lFreight.Text = Presentation.Product.ProductPrice.FormartFreight(order.Freight);
                this.lTax.Text = Presentation.Product.ProductPrice.FormartTax(order.Tax);
                string taxMessage = Presentation.eStoreContext.Current.Store.specialTaxMessage(order.cartX.ShipToContact);
                ltOrderMessage.Text = string.IsNullOrEmpty(taxMessage) ? "" : string.Format("<br /><span class=\"colorRed\">{0}</span>", taxMessage);
                this.lSubTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.cartX.TotalAmount);
                this.lDiscount.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.TotalDiscount.GetValueOrDefault() * (-1));
                this.lDutyAndTax.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.DutyAndTax.GetValueOrDefault());
                this.lSurcharge.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.Surcharge.GetValueOrDefault());
                this.lTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.totalAmountX);
                this.ldiscountType.Text = "(" + order.discountType + ")";
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
                {
                    if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != order.cartX.Currency)
                        lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(order.totalAmountX, order.cartX.currencySign));
                }
                if (!string.IsNullOrEmpty(order.CustomerComment))
                {
                    this.pComment.Visible = true;
                    this.lcomment.Text = order.CustomerComment;
                }
                else
                { this.pComment.Visible = false; }

                if (order.OrderStatus == "Confirmed")
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
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Order.cartX.ShipToContact);
                        break;
                    case "Bill":
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Order.cartX.BillToContact);
                        break;
                    case "Sold":
                        esc = new eStoreContact(Presentation.eStoreContext.Current.Order.cartX.SoldToContact);
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
                
                if (Presentation.eStoreContext.Current.Order.cartX.ShipToContact.isVerifyState())
                {
                    BindScript("script", "showContactError",
                        "$(function() { alert('" +
                        eStore.Presentation.eStoreLocalization.Tanslation(
                            eStore.POCOS.Store.TranslationKey.ScriptMessage_Please_Select_State_Province) + "'); });");
                    return;
                }

                if (Presentation.eStoreContext.Current.Store.offerShippingService &&
                    !Presentation.eStoreContext.Current.Order.isReferredToChannelPartner())
                {
                    var cart = Presentation.eStoreContext.Current.Order.cartX;
                    var currentOrder = Presentation.eStoreContext.Current.Order;
                    ShippingMethod sm;

                    switch (currentOrder.CustomerShippingMethodTemp)
                    {
                        case "Dropoff":
                        case "Recommend":
                            List<ShippingMethod> sms;
                            try
                            {
                                sms = eStoreContext.Current.Store.getAvailableShippingMethods(cart,
                                    currentOrder.CustomerShippingMethodTemp == "Dropoff");
                            }
                            catch (Exception ex)
                            {
                                sms = null;
                                Utilities.eStoreLoger.Error("Can not get shipping",
                                    Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(),
                                    Presentation.eStoreContext.Current.Store.storeID, ex);
                                Presentation.eStoreContext.Current.Order.setAlert(Order.OAlert.NeedFreightReview);
                            }
                            if (sms == null || sms.Count == 0)
                            {
                                Presentation.eStoreContext.Current.Order.setAlert(Order.OAlert.NeedFreightReview);
                                // show error message don't get shipping method list
                                Presentation.eStoreContext.Current.AddStoreErrorCode("Can't get shipping method list");
                                return;
                            }
                            else
                            {
                                sm = sms.FirstOrDefault(t => t.ShippingMethodDescription == currentOrder.ShippingMethod);
                                if (sm == null)
                                {
                                    Presentation.eStoreContext.Current.Order.setAlert(Order.OAlert.NeedFreightReview);
                                    // show error message new address don't get current shipping method
                                    Presentation.eStoreContext.Current.AddStoreErrorCode("Can't get current shipping method");
                                    return;
                                }
                                else
                                {
                                    var oldFreight =
                                        new
                                        {
                                            Freight = Presentation.eStoreContext.Current.Order.Freight,
                                            FreightDiscount = Presentation.eStoreContext.Current.Order.FreightDiscount
                                        };
                                    Presentation.eStoreContext.Current.Order.Freight =
                                        eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(
                                            (decimal)sm.ShippingCostWithPublishedRate,
                                            eStore.Presentation.eStoreContext.Current.CurrentCurrency).value;
                                    Presentation.eStoreContext.Current.Order.FreightDiscount =
                                        eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(
                                            (decimal)sm.Discount, eStore.Presentation.eStoreContext.Current.CurrentCurrency)
                                            .value;
                                    TaxCalculator taxProvider =
                                        Presentation.eStoreContext.Current.Store.calculateTax(
                                            Presentation.eStoreContext.Current.Order);
                                    if (taxProvider.Status == false)
                                    {
                                        Presentation.eStoreContext.Current.AddStoreErrorCode(taxProvider.ErrorCode);
                                        return;
                                    }
                                    Presentation.eStoreContext.Current.Order.updateTotalAmount();
                                    Presentation.eStoreContext.Current.Order.save();
                                    Presentation.eStoreContext.Current.Order.setAlert(Order.OAlert.None);
                                    if (Presentation.eStoreContext.Current.Order.Freight != oldFreight.Freight)
                                        Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format("Freight changed from {0} to {1}",
                                            oldFreight.Freight, Presentation.eStoreContext.Current.Order.Freight));
                                    order = Presentation.eStoreContext.Current.Order;
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
                var contactls = Presentation.UserContactManager.GeteStoreContacts(Presentation.eStoreContext.Current.Order.cartX);
                eStoreContact currentContact;
                if (newContact.isBillTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isBillTo);
                    if (currentContact != null)
                        currentContact.isBillTo = false;
                    Presentation.eStoreContext.Current.Order.cartX.setBillTo(newContact.toCartContact());
                }
                else if (newContact.isShipTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isShipTo);
                    if (currentContact != null)
                        currentContact.isShipTo = false;
                    Presentation.eStoreContext.Current.Order.cartX.setShipTo(newContact.toCartContact());
                }
                else if (newContact.isSoldTo)
                {
                    currentContact = contactls.FirstOrDefault(ec => ec.isSoldTo);
                    if (currentContact != null)
                        currentContact.isSoldTo = false;
                    Presentation.eStoreContext.Current.Order.cartX.setSoldTo(newContact.toCartContact());
                }
                this.ShipToContact.cartContact = Presentation.eStoreContext.Current.Order.cartX.ShipToContact;
                this.BillToContact.cartContact = Presentation.eStoreContext.Current.Order.cartX.BillToContact;
                this.SoldToContact.cartContact = Presentation.eStoreContext.Current.Order.cartX.SoldToContact;
            }
        }
    }
}