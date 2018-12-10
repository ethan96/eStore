using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Account
{
    public partial class OrderDetail : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }

        }

        private string welcomename = string.Empty;
        public string WelcomeName
        {
            get
            {
                if (string.IsNullOrEmpty(this.welcomename))
                {
                    if (eStore.Presentation.eStoreContext.Current.User != null)
                        this.welcomename = string.Format("Hi! {0}", eStore.Presentation.eStoreContext.Current.User.FirstName);
                }
                return this.welcomename;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["orderid"]))
                    Response.Redirect("~/Account/MyOrder.aspx");
                else
                {
                    string orderNo = Request["orderid"].Trim().ToUpper();
                    POCOS.Order currentdorder = (from order in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                                 where order.OrderNo == orderNo
                                                 select order).FirstOrDefault();
                    if (currentdorder == null)
                    {
                        try
                        {
                            POCOS.Order saporder = new POCOS.Order();
                            saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                            saporder.OrderNo = orderNo;
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
                        catch (Exception ex)
                        {

                        }
                    }

                    if (currentdorder == null)
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
                    this.OrderDetail1.order = currentdorder;
                    //this.OrderInvoiceDetail1.showATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                    //this.hprintorder.NavigateUrl = "~/Cart/printorder.aspx?orderid=" + Request["orderid"];
                }
            }
        }
    }
}