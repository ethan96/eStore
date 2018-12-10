using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;
using eStore.BusinessModules;

namespace eStore.UI.Cart
{
    public partial class printorder : eStoreBasePagePrint
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["orderid"]))
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Not_Found), null, true);
                    return;
                }
                else
                {
                    POCOS.Order currentdorder = (from order in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                                 where order.OrderNo == Request["orderid"]
                                                 select order).FirstOrDefault();

                    if (currentdorder == null)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Not_Found), null, true);
                         
                        return;
                    }
                    switch (currentdorder.statusX)
                    {
                        case POCOS.Order.OStatus.Open:
                        case POCOS.Order.OStatus.NotSpecified:
                            Presentation.eStoreContext.Current.UserShoppingCart = currentdorder.cartX;
                            Response.Redirect("~/Cart/Cart.aspx");
                            break;
                        case POCOS.Order.OStatus.NeedTaxAndFreightReview:
                        case POCOS.Order.OStatus.NeedTaxIDReview:
                        case POCOS.Order.OStatus.NeedGeneralReview:
                            Response.Redirect("~/Cart/confirm.aspx");
                            break;
                        case POCOS.Order.OStatus.WaitForPaymentResponse:
                            Response.Redirect("~/Cart/CheckOut.aspx");
                            break;
                        case POCOS.Order.OStatus.Confirmed:
                          
                            //Response.Redirect("~/Cart/orderdetail.aspx?orderid=" + Presentation.eStoreContext.Current.Order.OrderNo);
                            break;
                        default:
                            break;
                    }
                    if (currentdorder == null)
                    {
                        return;
                    }
                    this.OrderInvoiceDetail1.showATP = false;
                    this.OrderInvoiceDetail1.order = currentdorder;
                   
                }
            }
        }
    }
}