using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using eStore.Utilities;

namespace eStore.UI.Modules
{
    public partial class OrderInvoiceDetail : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Order order { get; set; }
        public Boolean showATP { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            OrderContentPreview1.order = order ?? Presentation.eStoreContext.Current.Order;
            if (!IsPostBack)
            {

                


                //this.lOrderNumber.Text = order.OrderNo;
                //this.lOrderStatus.Text = order.OrderStatus;
                //this.lOrderDate.Text = Presentation.eStoreLocalization.DateTime((DateTime)order.OrderDate);


                //this.CartContentPreview1.cart = order.cartX;
                //this.ShipToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Ship_to);
                //this.ShipToContact.cartContact = order.cartX.ShipToContact;
                //this.BillToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Bill_to);
                //this.BillToContact.cartContact = order.cartX.BillToContact;
                //this.SoldToContact.HeaderText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Sold_to);

                //this.SoldToContact.cartContact = order.cartX.SoldToContact;
                //switch (order.paymentTypeX)
                //{
                //    case POCOS.Payment.Payment_Type.PayByCreditCard:
                //        POCOS.Payment payment = order.Payments.OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                //        if (payment != null)
                //        {

                //            lpaymentInfo.Text = string.Format(" <p class=\"fontBold\">{3}:</p><p>Last 4 digits:{0}</p><br /><p class=\"fontBold\">{4}:</p><p>{1}</p><p>{2}</p>"
                //                                  , payment.LastFourDigit == null ? string.Empty : payment.LastFourDigit.ToString()
                //                                  , Presentation.eStoreContext.Current.Store.getCultureFullName(this.order.cartX.BillToContact)
                //                                  , string.Format("{0}<br />{1}, {2} {3}, {4}",
                //                             this.order.cartX.BillToContact.Address1,
                //                             this.order.cartX.BillToContact.City,
                //                             this.order.cartX.BillToContact.State,
                //                             this.order.cartX.BillToContact.ZipCode,
                //                             this.order.cartX.BillToContact.Country)
                //                                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Information)
                //                                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Billing_Address));

                //        }
                //        else
                //        {
                //            lpaymentInfo.Text = string.Empty;
                //        }

                //        break;
                //    case POCOS.Payment.Payment_Type.PayByNetTerm:
                //        lpaymentInfo.Text = string.Format(" <p class=\"fontBold\">{1}:</p><p>PurchaseNO: {0}</p>"
                //            , order.PurchaseNO
                //            , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Information));
                //        break;
                //    case POCOS.Payment.Payment_Type.PayByWireTransfer:
                //    case POCOS.Payment.Payment_Type.NotClassified:
                //        break;
                //}
                //if (!string.IsNullOrEmpty(order.ShippingMethod))
                //{
                //    this.lShippingInfo.Text = string.Format("<p class=\"fontBlue\">{1}: {0}</p>", order.ShippingMethod, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Ship_via));
                //}
                //else
                //{ this.lShippingInfo.Text = string.Empty; }



                //this.lFreight.Text = Presentation.Product.ProductPrice.FormartFreight(order.Freight, order.currencySign);
                //this.lTax.Text = Presentation.Product.ProductPrice.FormartTax(order.Tax, order.currencySign);
                //this.lSubTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.cartX.TotalAmount, order.cartX.currencySign);
                //this.lTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.totalAmountX, order.currencySign);
                //if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
                //{
                //    if (eStore.Presentation.eStoreContext.Current.CurrentCurrency != null && eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != order.cartX.Currency)
                //        lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(order.totalAmountX, order.cartX.currencySign));
                //}
            }
        }
    }
}