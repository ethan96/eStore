using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class OrderDetail : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Order order { get; set; }

        private bool _IsChangeSessionOrder = true;

        public bool IsChangeSessionOrder
        {
            get { return _IsChangeSessionOrder; }
            set { _IsChangeSessionOrder = value; }
        }


        public string PrintUrl 
        {
            get
            {
                return string.Format("/Cart/printorder.aspx?orderid={0}", Request["orderid"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.order == null)
                return;

            if (!IsPostBack)
            {
                if(IsChangeSessionOrder)
                    Presentation.eStoreContext.Current.Order = this.order;
                this.lOrderNumber.Text = order.OrderNo;
                this.BillToContact.cartContact = order.cartX.BillToContact;
                this.SoldToContact.cartContact = order.cartX.SoldToContact;
                this.ShipToContact.cartContact = order.cartX.ShipToContact;

                this.CartContentPreview1.cart = order.cartX;
                this.lSubTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.cartX.TotalAmount, order.cartX.currencySign);
                this.lTotal.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.totalAmountX, order.currencySign);

                if (!string.IsNullOrEmpty(order.ShippingMethod))
                    this.lShippingInfo.Text = order.ShippingMethod;

                if (Presentation.eStoreContext.Current.Store.offerShippingService)
                {
                    this.pShipHandling.Visible = true;
                    this.lFreight.Text = Presentation.Product.ProductPrice.FormartFreight(order.Freight, order.currencySign);
                }

                if (Presentation.eStoreContext.Current.Store.hasTaxCalculator)
                {
                    this.pTaxEstimated.Visible = true;
                    this.lTax.Text = Presentation.Product.ProductPrice.FormartTax(order.Tax, order.currencySign);
                }

                if (order.DutyAndTax > 0)
                {
                    this.pDutyTax.Visible = true;
                    this.lDutyAndTax.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.DutyAndTax);
                }

                if (order.Surcharge > 0)
                {
                    this.pSurcharge.Visible = true;
                    this.lSurcharge.Text = Presentation.Product.ProductPrice.FormartPriceWithDecimal(order.Surcharge);
                }

                if (order.TotalDiscount > 0)
                {
                    this.pTotalDiscount.Visible = true;
                    this.lTotalDiscount.Text = string.Format("-{0}", order.TotalDiscount);
                }

                if (Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount"))
                {
                    if (Presentation.eStoreContext.Current.CurrentCurrency != null && Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID != order.cartX.Currency)
                        this.lSubStorePrice.Text = string.Format("<br />({0})", Presentation.Product.ProductPrice.FormartPriceWithParameterCurrency(order.totalAmountX, order.cartX.currencySign));
                }

                this.lb_Reorder.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_ReOrder);
            }

        }

        protected void lb_Reorder_Click(object sender, EventArgs e)
        {
            POCOS.Order currentdorder = (from order in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                         where order.OrderNo == Request["orderid"].ToString()
                                         select order).FirstOrDefault();

            //Go to SAP to get order
            //MyOrder.ascx.cs has same code
            if (currentdorder == null)
            {
                POCOS.Order saporder = new POCOS.Order();
                saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                saporder.OrderNo = Request["orderid"].ToString();
                saporder.User = Presentation.eStoreContext.Current.User.actingUser;
                BusinessModules.SAPOrderTracking SAPOrderTracking = new BusinessModules.SAPOrderTracking(saporder, -360);
                saporder = SAPOrderTracking.getStoreOrder();
                if (saporder.UpdateBySAP)
                    currentdorder = saporder;
            }

            if (currentdorder != null)
            {
                Presentation.eStoreContext.Current.User.actingUser.reOrder(currentdorder);
                Presentation.eStoreContext.Current.UserShoppingCart = Presentation.eStoreContext.Current.User.actingUser.shoppingCart;
                Response.Redirect("~/Cart/Cart.aspx");
            }
        }
    }
}