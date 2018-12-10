using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Quote
    {
        public Quote()
        {
        
        }

        public Quote(POCOS.Quotation quote)
        {
            this.QuoteNo = quote.QuotationNumber;
            this.QuoteNoUrl = string.Format("/Account/QuoteDetail.aspx?ReviewQuotation={0}", quote.QuotationNumber);
            this.QuoteReviseUrl = string.Format("/Account/QuoteDetail.aspx?QuotationRevise={0}", quote.QuotationNumber);
            this.QuoteID = string.Empty;
            this.SubTotal = Presentation.Product.ProductPrice.FormartPriceWithDecimal(quote.totalAmountX, quote.currencySign);
            this.TotalAmount = quote.totalAmountX;
            this.CurrencySign = quote.currencySign;
            this.ShipTo = quote.cartX != null && quote.cartX.ShipToContact != null ? Presentation.eStoreContext.Current.Store.getCultureFullName(quote.cartX.ShipToContact) : string.Empty;
            this.Status = quote.statusX.ToString();
            this.QuoteDate = Presentation.eStoreLocalization.Date(quote.QuoteDate);
            this.QuoteExpiredDate = Presentation.eStoreLocalization.Date(quote.QuoteExpiredDate);
            this.QuoteAction = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise);
        }

        public string QuoteID { get; set; }
        public string QuoteNo { get; set; }
        public string QuoteNoUrl { get; set; }
        public string QuoteReviseUrl { get; set; }
        public string SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencySign { get; set; }
        public string ShipTo { get; set; }
        public string Status { get; set; }
        public string QuoteDate { get; set; }
        public string QuoteExpiredDate { get; set; }
        public string QuoteAction { get; set; }
        public List<Models.Order> Orders
        {
            get
            {
                List<Models.Order> orders = new List<Order>();
                if (!string.IsNullOrEmpty(this.QuoteNo) && Presentation.eStoreContext.Current.User != null)
                {
                    orders = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                            where o.Source == this.QuoteNo
                            select new Models.Order(o)).ToList();
                }
                return orders;
            }
        }

    }
}