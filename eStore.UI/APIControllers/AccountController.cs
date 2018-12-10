using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eStore.UI.APIControllers
{
    public class AccountController : ApiController
    {
        [HttpGet]
        public Models.Account GetAccountOrder(DateTime? startdate = null, DateTime? enddate = null, bool isConfirm = false)
        {
            List<POCOS.Order> orders = new List<POCOS.Order>();
            if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.ordersX != null)
            {
                if (startdate != null && enddate != null)
                    orders = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                              where o.OrderDate >= startdate.Value && o.OrderDate <= enddate.Value
                              orderby o.statusX, o.ConfirmedDate descending
                              select o).ToList();
                else
                    orders = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                orderby o.statusX, o.ConfirmedDate descending
                                select o).ToList();
            }
            if (isConfirm)
                orders = orders.Where(c => c.isConfirmdOrder).ToList();
            return new Models.Account(orders);
        }

        [HttpGet]
        public Models.Account GetAccountOrderBySearch(string orderNo = "", string range = "0")
        {
            List<POCOS.Order> match = new List<POCOS.Order>();
            if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.ordersX != null)
            {
                match = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                           orderby o.statusX, o.ConfirmedDate descending
                                           select o).ToList();

                if (!string.IsNullOrEmpty(orderNo))
                    match = match.Where(o => o.OrderNo.Contains(orderNo)).ToList();

                DateTime month3 = DateTime.Now.AddMonths(-3);
                DateTime month6 = DateTime.Now.AddMonths(-6);
                DateTime month12 = DateTime.Now.AddMonths(-12);
                DateTime month24 = DateTime.Now.AddMonths(-24);
                switch (range)
                {
                    case "1":
                        match = match.Where(o => o.OrderDate.Value >= month3).ToList();
                        break;
                    case "2":
                        match = match.Where(o => o.OrderDate.Value <= month3 && o.OrderDate.Value >= month6).ToList();
                        break;
                    case "3":
                        match = match.Where(o => o.OrderDate.Value <= month6 && o.OrderDate.Value >= month12).ToList();
                        break;
                    case "4":
                        match = match.Where(o => o.OrderDate.Value <= month12 && o.OrderDate.Value >= month24).ToList();
                        break;
                    case "0":
                    default:
                        break;
                }

                if (match == null || match.Count == 0)
                {
                    try
                    {
                        POCOS.Order saporder = new POCOS.Order();
                        saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                        saporder.OrderNo = orderNo;
                        saporder.User = Presentation.eStoreContext.Current.User.actingUser;

                        BusinessModules.SAPOrderTracking SAPOrderTracking = new BusinessModules.SAPOrderTracking(saporder, -180, true);//Change default range from 1 year to 6 month 
                        var orderlist = SAPOrderTracking.getSAPOrders();
                        if (orderlist != null)
                            match = orderlist;
                        //saporder = SAPOrderTracking.getStoreOrder();
                        //if (saporder.UpdateBySAP)
                        //{
                        //    match = new List<POCOS.Order>();
                        //    match.Add(saporder);
                        //}

                    }
                    catch
                    {

                    }
                }
            }
            return new Models.Account(match);
        }

        [HttpGet]
        public Models.Account GetAccountQuote()
        {
            List<POCOS.Quotation> quotes = new List<POCOS.Quotation>();
            List<POCOS.Quotation> trQuotes = new List<POCOS.Quotation>();
            if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.quotations != null)
            {
                quotes = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                where q.cartX.UserID == q.UserID
                                                orderby q.statusX, q.ConfirmedDate descending
                                                select q).ToList();

                List<POCOS.Quotation> openQuotes = quotes.Where(q => q.statusX == POCOS.Quotation.QStatus.Open).ToList();
                foreach (var q in openQuotes)
                {
                    quotes.Remove(q);
                    quotes.Insert(0, q);
                }

                trQuotes = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                 where q.cartX.UserID != q.UserID
                                                 orderby q.statusX, q.ConfirmedDate descending
                                                 select q).ToList();
            }
            return new Models.Account(quotes, trQuotes);
        }

        [HttpGet]
        public Models.Account GetAccountQuoteBySearch(string quoteNo = "", string range = "0")
        {
            if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.quotations != null)
            {
                List<POCOS.Quotation> quotes = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                where q.cartX.UserID == q.UserID
                                orderby q.statusX, q.ConfirmedDate descending
                                                    select q).ToList();

                List<POCOS.Quotation> tQuotes = (from q in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                 where q.cartX.UserID != q.UserID
                                                 orderby q.statusX, q.ConfirmedDate descending
                                                 select q).ToList();

                if (!string.IsNullOrEmpty(quoteNo))
                {
                    quotes = quotes.Where(q => q.QuotationNumber == quoteNo.Trim()).ToList();
                    tQuotes = tQuotes.Where(q => q.QuotationNumber == quoteNo.Trim()).ToList();
                }

                DateTime month3 = DateTime.Now.AddMonths(-3);
                DateTime month6 = DateTime.Now.AddMonths(-6);
                DateTime month12 = DateTime.Now.AddMonths(-12);
                DateTime month24 = DateTime.Now.AddMonths(-24);
                switch (range)
                {
                    case "1":
                        quotes = quotes.Where(q => q.QuoteDate.Value >= month3).ToList();
                        tQuotes = tQuotes.Where(q => q.QuoteDate.Value >= month3).ToList();
                        break;
                    case "2":
                        quotes = quotes.Where(q => q.QuoteDate.Value <= month3 && q.QuoteDate.Value >= month6).ToList();
                        tQuotes = tQuotes.Where(q => q.QuoteDate.Value <= month3 && q.QuoteDate.Value >= month6).ToList();
                        break;
                    case "3":
                        quotes = quotes.Where(q => q.QuoteDate.Value <= month6 && q.QuoteDate.Value >= month12).ToList();
                        tQuotes = tQuotes.Where(q => q.QuoteDate.Value <= month6 && q.QuoteDate.Value >= month12).ToList();
                        break;
                    case "4":
                        quotes = quotes.Where(q => q.QuoteDate.Value <= month12 && q.QuoteDate.Value >= month24).ToList();
                        tQuotes = tQuotes.Where(q => q.QuoteDate.Value <= month12 && q.QuoteDate.Value >= month24).ToList();
                        break;
                    case "0":
                    default:
                        break;
                }

                List<POCOS.Quotation> openQuotes = quotes.Where(q => q.statusX == POCOS.Quotation.QStatus.Open).ToList();
                foreach (var q in openQuotes)
                {
                    quotes.Remove(q);
                    quotes.Insert(0, q);
                }

                return new Models.Account(quotes, tQuotes);
            }
            return null;
        }
        [HttpGet]
        public string getShoppingCartItems()
        {
            if (Presentation.eStoreContext.Current.User == null ||
                Presentation.eStoreContext.Current.UserShoppingCart == null ||
                 !Presentation.eStoreContext.Current.UserShoppingCart.cartItemsX.Any())
                return string.Empty;
            else
            {
                System.Text.StringBuilder sbCart = new System.Text.StringBuilder();
                sbCart.Append("<ol>");
                foreach (var item in Presentation.eStoreContext.Current.UserShoppingCart.cartItemsX)
                {
                    sbCart.Append("<li>");
                    sbCart.Append("<div class=\"img\">");
                    if (!string.IsNullOrEmpty(item.partX.thumbnailImageX))
                        sbCart.AppendFormat("<img src=\"{0}\">", item.partX.thumbnailImageX);
                    else
                        sbCart.Append("<img src=\"https://buy.advantech.com/images/photounavailable.gif\">");
                    sbCart.Append("</div>");
                    sbCart.Append("<div class=\"content\">");
                    sbCart.AppendFormat("<h6 class=\"productTitle\">{0}</h6>",item.productNameX);
                    sbCart.AppendFormat("<p>{0}</p>",item.Description);
                    sbCart.Append("<div class=\"orderlistTable\">");
                    sbCart.Append("<div class=\"unitPrice\">");
                    sbCart.Append("<div class=\"top\">Unit Price</div>");
                    sbCart.AppendFormat("<div class=\"bottom\">{0}</div>",Presentation.Product.ProductPrice.FormartPriceWithCurrency(item.UnitPrice));
                    sbCart.Append("</div>");
                    sbCart.Append("<div class=\"qty\">");
                    sbCart.Append("<div class=\"top\">Qty</div>");
                    sbCart.AppendFormat("<div class=\"bottom\">{0}</div>",item.Qty);
                    sbCart.Append("</div>");
                    sbCart.Append("<div class=\"subTotal\">");
                    sbCart.Append("<div class=\"top\">Sub Total</div>");
                    sbCart.AppendFormat("<div class=\"bottom\">{0}</div>",Presentation.Product.ProductPrice.FormartPriceWithCurrency(item.AdjustedPrice));
                    sbCart.Append("</div>");
                    sbCart.Append("</div>");
                    sbCart.Append("</div>");
                    sbCart.AppendFormat("<a href=\"#\" class=\"close\"><img src=\"{0}\" /></a>", esUtilities.CommonHelper.ResolveUrl("~/images/orderlistTable_close.png"));
                    sbCart.Append("<div class=\"clearfix\"></div>");
                    sbCart.Append("</li>");
                }
                sbCart.Append("</ol>");
                sbCart.AppendFormat("<div class=\"orderTotla\"><span>Total</span>{0}</div>",Presentation.Product.ProductPrice.FormartPriceWithCurrency(Presentation.eStoreContext.Current.UserShoppingCart.TotalAmount));
                sbCart.AppendFormat("<a href=\"{0}\" class=\"eStore_btn\">order</a>", esUtilities.CommonHelper.ResolveUrl("~/Cart/Cart.aspx"));
                return sbCart.ToString();
            }
        }
    }
}