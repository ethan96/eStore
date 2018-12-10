using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using eStore.POCOS;
using esUtilities;

namespace eStore.UI
{
    public partial class completeIndirectPayment : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            NameValueCollection responseValues = new NameValueCollection();
            responseValues = Request.Params;

            string orderno = Presentation.eStoreContext.Current.Store.getIndirectPaymentOrderResponseNO(responseValues);

            POCOS.Order order = Presentation.eStoreContext.Current.Store.getOrder(orderno);
            POCOS.StorePayment payment = Presentation.eStoreContext.Current.Store.getStorePayment(order.PaymentType);
            try
            {
                Payment paymentRlt = Presentation.eStoreContext.Current.Store.completeIndirectPayment(order,payment, responseValues);

                switch (paymentRlt.statusX)
                {
                    case POCOS.Payment.PaymentStatus.Approved:
                    case POCOS.Payment.PaymentStatus.NeedAttention:
                    case Payment.PaymentStatus.FraudAlert://for simulation
                        {
                            Presentation.eStoreContext.Current.Order = order;
                            if (Presentation.eStoreContext.Current.User == null || Presentation.eStoreContext.Current.User.UserID != order.UserID)
                            {
                                Presentation.eStoreContext.Current.User = order.userX;
                                Presentation.eStoreContext.Current.UserShoppingCart = null;
                                Presentation.eStoreContext.Current.Quotation = null;
                            }

                            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
                            {
                                Presentation.eStoreContext.Current.UserShoppingCart.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                                //Presentation.eStoreContext.Current.UserShoppingCart.save();
                            }
                            else
                            {
                                Presentation.eStoreContext.Current.Quotation.releaseToOrder(Presentation.eStoreContext.Current.User.actingUser, Presentation.eStoreContext.Current.Order);
                            }

                            if (Presentation.eStoreContext.Current.Order.save() != 0)
                            {
                                Presentation.eStoreContext.Current.Order = null;

                                Utilities.eStoreLoger.Error("SaveOrderFailed", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                                Presentation.eStoreContext.Current.AddStoreErrorCode("Save Order Failed", null, true);
                                return;
                            }

                            eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                            mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                            EMailReponse response = mailTemplate.getOrderMailContent(Presentation.eStoreContext.Current.Order, eStore.Presentation.eStoreContext.Current.CurrentLanguage
                                , eStore.Presentation.eStoreContext.Current.MiniSite);
                            if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                            {
                                Utilities.eStoreLoger.Error(string.Format("{0} sent mail failed. {1}", Presentation.eStoreContext.Current.Order.OrderNo, response.ErrCode.ToString())
                                    , Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);

                            }

                            //生成一个待 审核的log
                            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
                                Presentation.eStoreContext.Current.Store.calculateOrderReward(Presentation.eStoreContext.Current.Order, Presentation.eStoreContext.Current.MiniSite);

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
            catch (Exception)
            {

                Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed", null, true);
                return;
            }
        }
    }
}