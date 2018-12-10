using eStore.POCOS;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    /// <summary>
    /// db processing page 
    /// get daoupay server request 
    /// will update user order and send success tag.
    /// </summary>
    public partial class completeDaouPayment : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            POCOS.Order order = Presentation.eStoreContext.Current.Order;
            if (order == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Get Order Error", null, true);
                Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/cart.aspx");
            }

            //StringBuilder sb = new StringBuilder();
            //sb.Append("IP: " + Page.Request.UserHostAddress + "\r\n");

            /// wait for daoupay server confirm.
            POCOS.Order orderTemp = null;
            for (int willcont = 0; willcont <= 15; willcont++)
            {
                orderTemp = Presentation.eStoreContext.Current.Store.getOrder(order.OrderNo);
                if (!orderTemp.isConfirmdOrder)
                {
                    //sb.Append("willcont " + willcont.ToString() + " time: " + DateTime.Now.ToString() + "\r\n");
                    System.Threading.Thread.Sleep(1000);
                }
                else
                    break;
            }
            
            //sb.Append("back time: " + DateTime.Now.ToString() + "\r\n");
            //sb.AppendFormat("SessionOrder: {0} \r\n", order.OrderStatus);
            //sb.AppendFormat("DBOrderOrder: {0} \r\n", orderTemp.OrderStatus);
            //System.IO.File.WriteAllText("e:\\log\\asp_RE_" + (new Random()).Next(9999) + ".txt", sb.ToString());

            try
            {
                Payment paymentRlt = orderTemp.getLastPayment();

                switch (paymentRlt.statusX)
                {
                    case POCOS.Payment.PaymentStatus.Approved:
                    case POCOS.Payment.PaymentStatus.NeedAttention:
                    case Payment.PaymentStatus.FraudAlert://for simulation
                        {
                            //临时解决方案
                            //此逻辑理应放在order confirm部分.
                            if (!string.IsNullOrEmpty(order.PromoteCode))
                            {
                                if (orderTemp.promotionAppliedLogs.FirstOrDefault(c => c.PromotionCode == order.PromoteCode) == null)
                                    order.savePromotionAppliedLogs(order.PromoteCode);
                            }

                            Presentation.eStoreContext.Current.Order = orderTemp;
                            if (Presentation.eStoreContext.Current.User == null || Presentation.eStoreContext.Current.User.UserID != orderTemp.UserID)
                            {
                                Presentation.eStoreContext.Current.User = orderTemp.userX;
                                Presentation.eStoreContext.Current.UserShoppingCart = null;
                                Presentation.eStoreContext.Current.Quotation = null;
                            }

                            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
                            {
                                Presentation.eStoreContext.Current.UserShoppingCart.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                                Presentation.eStoreContext.Current.UserShoppingCart.save();
                            }
                            else
                            {
                                Presentation.eStoreContext.Current.Quotation.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                            }
                            
                            Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/thankyou.aspx");
                            break;
                        }
                    // case POCOS.Payment.PaymentStatus.Referral:
                    default:
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode("PaymentStatus." + paymentRlt.statusX.ToString());
                            Presentation.eStoreContext.Current.Order.save();
                            Server.Transfer(esUtilities.CommonHelper.GetStoreLocation(Presentation.eStoreContext.Current.getBooleanSetting("IsSecureCheckout", true)) + "Cart/CheckOut.aspx");

                            break;

                        }
                }

            }
            catch (Exception ex)
            {

                Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed", null, true);
                return;
            }
        }
    }
}