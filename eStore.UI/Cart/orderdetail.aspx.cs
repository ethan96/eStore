using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Cart
{
    public partial class orderdetail:  Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Redirect("~/Account/orderdetail.aspx?orderid=" + Request["orderid"]);
                if (string.IsNullOrEmpty(Request["orderid"]))
                { }
                else
                {
                    POCOS.Order currentdorder = (from order in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                                where order.OrderNo == Request["orderid"]
                                                select order).FirstOrDefault();
                    if (currentdorder == null)
                    {
                        try
                        {
                            POCOS.Order saporder = new POCOS.Order();
                            saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                            saporder.OrderNo = Request["orderid"].Trim();
                            saporder.User = Presentation.eStoreContext.Current.User.actingUser;

                            SAPOrderTracking SAPOrderTracking = new SAPOrderTracking(saporder);
                            saporder = SAPOrderTracking.getStoreOrder();
                            if (saporder.UpdateBySAP)
                            {
                                currentdorder = saporder;
                                currentdorder.OrderStatus = POCOS.Order.OStatus.Confirmed.ToString();
                            }
                            else

                            {
                                currentdorder = null;
                            }


                        }
                        catch (Exception)
                        {
                            currentdorder = null;
                        }

                    }
                    else
                    {
                        try
                        {

                            SAPOrderTracking SAPOrderTracking = new SAPOrderTracking(currentdorder);
                            currentdorder = SAPOrderTracking.getStoreOrder();
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (currentdorder== null)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Not_Found), null, true);
                     
                        return;
                    }
                    switch (currentdorder.statusX)
                    {
                        case POCOS.Order.OStatus.Open:
                            Presentation.eStoreContext.Current.UserShoppingCart = currentdorder.cartX;
                            Response.Redirect("~/Cart/Cart.aspx");
                            break;
                        case POCOS.Order.OStatus.WaitForPaymentResponse:
                            Presentation.eStoreContext.Current.Order = currentdorder;
                            Response.Redirect("~/Cart/CheckOut.aspx");
                            break;
                        default:
                      
                           
                           //Response.Redirect("~/Cart/orderdetail.aspx?orderid=" + Presentation.eStoreContext.Current.Order.OrderNo);
                            break;
                    }
                    if (currentdorder == null)
                    {
                        return;
                    }
                    this.OrderInvoiceDetail1.order = currentdorder;
                    this.OrderInvoiceDetail1.showATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                    this.hprintorder.NavigateUrl = "~/Cart/printorder.aspx?orderid=" + Request["orderid"];
                }
                bindFonts();
            }
        }

        protected void bindFonts()
        {
            lMyorders.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_My_Orders);
            hprintorder.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_printorder);
        }
    }
}