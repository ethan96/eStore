using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Cart
{
    public class eStoreBaseOrderPage : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public enum PageStep 
        { 
            EmptyCart = 0, 
            ShppingCart = 10,
            SelectContacts = 20,
            Confirm = 30,
            ToPayment = 40,
            Finish = 50 
        }

        protected virtual PageStep minStepNo
        {
            get { return PageStep.EmptyCart; }
        }

        protected override void OnInit(EventArgs e)
        {
            var cart = Presentation.eStoreContext.Current.UserShoppingCart;
            var order = Presentation.eStoreContext.Current.Order;
            var step = verifyStep(order, cart);
            switch (minStepNo)
            { 
                case PageStep.Finish:
                case PageStep.ToPayment:
                case PageStep.Confirm:
                case PageStep.SelectContacts:
                    if (order == null)
                    {
                        step = PageStep.ShppingCart;
                        relocatePage(step, order, cart);
                    }
                    if (order.isConfirmdOrder)
                        relocatePage(PageStep.Finish, order, cart);

                    break;
            }
            if (!IsPostBack)
                relocatePage(step, order, cart);
            base.OnInit(e);
        }

        void relocatePage(PageStep orderStep, POCOS.Order order, POCOS.Cart cart)
        {
            if (orderStep == PageStep.Finish)
            {
                if (this.Page is Thankyou)
                    return;
                else if (this.Page is Cart)
                {
                    if (cart == null || !cart.cartItemsX.Any())
                        orderStep = PageStep.EmptyCart;
                    else
                        orderStep = PageStep.ShppingCart;
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Order is confirmed");
                    Response.Redirect("~/Account/orderdetail.aspx?orderid=" + order.OrderNo);
                }
            }
            int orderWeight = (int)orderStep;
            int pageWeight = (int)minStepNo;
            if (orderWeight < pageWeight)
            {
                switch (orderStep)
                {
                    case PageStep.EmptyCart:
                        Response.Redirect("~/Cart/emptycart.aspx");
                        break;
                    case PageStep.ShppingCart:
                            Response.Redirect("~/Cart/Cart.aspx");
                        break;
                    case PageStep.SelectContacts:
                            Response.Redirect("~/Cart/Contact.aspx");
                        break;
                    case PageStep.Confirm:
                            Response.Redirect("~/Cart/confirm.aspx");
                        break;
                    case PageStep.ToPayment:
                            Response.Redirect("~/Cart/CheckOut.aspx");
                        break;
                    default:
                        break;
                }
            }

        }


        private PageStep verifyStep(POCOS.Order order, POCOS.Cart cart)
        {
            if (order == null && (cart == null || cart.CartItems.Count == 0))
                return PageStep.EmptyCart;

            if (order == null && cart != null)
            {
                //Presentation.eStoreContext.Current.AddStoreErrorCode("Connection Session Time Out");
                return PageStep.ShppingCart;
            }
            switch (order.statusX)
            {
                case POCOS.Order.OStatus.Closed:
                case POCOS.Order.OStatus.Closed_Complete:
                case POCOS.Order.OStatus.Closed_Converted:
                case POCOS.Order.OStatus.ReviewRejected:
                case POCOS.Order.OStatus.ReviewedApproved:
                case POCOS.Order.OStatus.Approved:
                case POCOS.Order.OStatus.Cancelled:
                case POCOS.Order.OStatus.Declined:
                case POCOS.Order.OStatus.Confirmed:
                case POCOS.Order.OStatus.ConfirmdButNeedFreightReview:
                case POCOS.Order.OStatus.ConfirmdButNeedTaxIDReview:                
                    return PageStep.Finish;
                case POCOS.Order.OStatus.WaitForPaymentResponse:
                    if (order.cartX.isModified())
                        return PageStep.SelectContacts;
                    return PageStep.ToPayment;
                case POCOS.Order.OStatus.NotSpecified:
                    return PageStep.ShppingCart;
                case POCOS.Order.OStatus.NeedTaxAndFreightReview:
                case POCOS.Order.OStatus.NeedFreightReview:
                    if(Presentation.eStoreContext.Current.Store.offerNoShippingMothed)
                        return PageStep.ToPayment;
                    else
                        return PageStep.SelectContacts;
                case POCOS.Order.OStatus.NeedTaxIDReview:
                case POCOS.Order.OStatus.NeedGeneralReview:
                case POCOS.Order.OStatus.Open:
                    if (Presentation.eStoreContext.Current.Store.offerShippingService && !order.isReferredToChannelPartner())
                    {
                        if (string.IsNullOrEmpty(order.ShippingMethod))
                            return PageStep.SelectContacts;
                        else
                            return PageStep.ToPayment;
                    }
                    else if (order.ShippingMethod == null)
                        return PageStep.SelectContacts;
                    else
                        return PageStep.ToPayment;
                default:
                    return PageStep.ShppingCart;
            }
        }

    }
}