using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Quotation
{
    public class eStoreBaseQuotationPage : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public enum PageStep
        {
            EmptyCart = 0,
            ShppingCart = 10,
            SelectContacts = 20,
            Confirm = 30,
            Finish = 40
        }

        protected virtual PageStep minStepNo
        {
            get { return PageStep.EmptyCart; }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
            {
                var cart = Presentation.eStoreContext.Current.UserShoppingCart;
                var quotation = Presentation.eStoreContext.Current.Quotation;
                var step = verifyStep(quotation, cart);
                relocatePage(step, quotation);
            }
            base.OnInit(e);
        }

        void relocatePage(PageStep orderStep, POCOS.Quotation quotation)
        {
            if (orderStep == PageStep.Finish)
            {
                if (this.Page is confirm)
                    return;
                Presentation.eStoreContext.Current.AddStoreErrorCode("Quotation is confirmed");
                Response.Redirect("~/Quotation/confirm.aspx?quotation=" + quotation.QuotationNumber);
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
                        Response.Redirect("~/Quotation/Quote.aspx");
                        break;
                    case PageStep.SelectContacts:
                        Response.Redirect("~/Quotation/Contact.aspx");
                        break;
                    case PageStep.Confirm:
                        Response.Redirect("~/Quotation/confirm.aspx");
                        break;
                    default:
                        break;
                }
            }

        }


        private PageStep verifyStep(POCOS.Quotation quotation, POCOS.Cart cart)
        {
            if (quotation == null && (cart == null || cart.CartItems.Count == 0))
                return PageStep.EmptyCart;

            if (quotation == null || quotation.Cart == null || quotation.cartX.CartItems.Count == 0)
                return PageStep.EmptyCart;

            if (quotation == null && cart != null)
            {
                return PageStep.ShppingCart;
            }
            switch (quotation.statusX)
            {
                case POCOS.Quotation.QStatus.Confirmed:
                case POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview:
                case POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview:
                case POCOS.Quotation.QStatus.Expired:
                    return PageStep.Finish;
                case POCOS.Quotation.QStatus.Unfinished:
                    return PageStep.ShppingCart;
                case POCOS.Quotation.QStatus.NeedFreightReview:
                case POCOS.Quotation.QStatus.NeedTaxAndFreightReview:
                    if(Presentation.eStoreContext.Current.Store.offerNoShippingMothed)
                        return PageStep.Confirm;
                    else
                        return PageStep.SelectContacts;
                case POCOS.Quotation.QStatus.NeedTaxIDReview:
                case POCOS.Quotation.QStatus.NeedGeneralReview:
                case POCOS.Quotation.QStatus.Open:
                    if (Presentation.eStoreContext.Current.Store.offerShippingService)
                    {
                        if (string.IsNullOrEmpty(quotation.ShippingMethod))
                            return PageStep.SelectContacts;
                        else
                            return PageStep.Confirm;
                    }
                    else if (quotation.ShippingMethod == null)
                        return PageStep.SelectContacts;
                    else
                        return PageStep.Confirm;
                default:
                    return PageStep.ShppingCart;
            }
        }
    }
}