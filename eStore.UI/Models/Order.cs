using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Order
    {
        public Order()
        {
        
        }

        public Order(POCOS.Order order)
        {
            this.OrderNo = order.OrderNo;
            this.OrderNoUrl = string.Format("/Account/OrderDetail.aspx?orderid={0}", order.OrderNo);
            this.OrderDate = Presentation.eStoreLocalization.Date(order.OrderDate);
            this.SubTotal = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.totalAmountX, order.currencySign);
            this.ShipTo = Presentation.eStoreContext.Current.Store.getCultureFullName(order.cartX.ShipToContact);
            this.Status = order.statusX.ToString();
            this.subTotalX = order.totalAmountX;
            this.UserId = order.UserID;
        }

        public string OrderNo { get; set; }

        public string OrderNoUrl { get; set; }

        public string OrderDate { get; set; }

        public string SubTotal { get; set; }

        public string ShipTo { get; set; }

        public string Status { get; set; }

        public decimal subTotalX { get; set; }

        public string UserId { get; set; }
    }
}