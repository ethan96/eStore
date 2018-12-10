using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using eStore.POCOS;
using esUtilities;
using eStore.BusinessModules;
namespace eStore.UI
{
    public partial class completeKuaiQianPayment : Page
    {

        private bool _UseSSL = false;
        public virtual bool UseSSL
        {
            get { return _UseSSL; }
            set { _UseSSL = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection responseValues = new NameValueCollection();
            responseValues = Request.Params;
            string orderno = Presentation.eStoreContext.Current.Store.getIndirectPaymentOrderResponseNO(responseValues);
            POCOS.Order order = Presentation.eStoreContext.Current.Store.getOrder(orderno);
            if (order == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Can not find Order");
                return;
            }
            string msg = responseValues["msg"];
            if (!string.IsNullOrEmpty(msg))
            {
                passPayment(msg, order);
            }
            else
            {
                noPayment(responseValues, order);
            }
            
        }



        protected void passPayment(string msg, POCOS.Order order)
        {
            if (msg.ToLower() == "success")
            {
                if (order.statusX == Order.OStatus.Confirmed && order.getLastPayment().statusX == Payment.PaymentStatus.Approved)
                {
                    Presentation.eStoreContext.Current.Order = order;
                    Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/thankyou.aspx");
                }
                else
                    Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/CheckOut.aspx");
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed");
                return;
            }
        }



        protected void noPayment(NameValueCollection responseValues, POCOS.Order order)
        {
            if (order.statusX == Order.OStatus.Confirmed && order.getLastPayment().statusX == Payment.PaymentStatus.Approved)
                Response.Redirect(esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "Cart/thankyou.aspx");

            try
            {

                POCOS.StorePayment payment = Presentation.eStoreContext.Current.Store.getStorePayment(order.PaymentType);
                Payment paymentRlt = Presentation.eStoreContext.Current.Store.completeIndirectPayment(order, payment, responseValues);

                switch (paymentRlt.statusX)
                {
                    case POCOS.Payment.PaymentStatus.Approved:
                        {
                            Presentation.eStoreContext.Current.Order = order;

                            string userid = responseValues["ext1"];
                            POCOS.User user = Presentation.eStoreContext.Current.User;
                            if (user == null && !string.IsNullOrEmpty(userid) && order.UserID == userid)
                                Presentation.eStoreContext.Current.User = user = Presentation.eStoreContext.Current.Store.getUser(userid);
                            if (user == null || user.UserID != order.UserID)
                            {
                                Presentation.eStoreContext.Current.User = order.userX;
                                Presentation.eStoreContext.Current.UserShoppingCart = null;
                                Presentation.eStoreContext.Current.Quotation = null;
                            }

                            if (string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.Source))
                            {
                                if (user.store == null)
                                    user.store = Presentation.eStoreContext.Current.Store.profile;
                                user.shoppingCart.releaseToOrder(user.actingUser, Presentation.eStoreContext.Current.Order);
                                //Presentation.eStoreContext.Current.UserShoppingCart.save();
                            }
                            else
                            {
                                Presentation.eStoreContext.Current.Quotation.releaseToOrder(user.actingUser, Presentation.eStoreContext.Current.Order);
                            }
                            if (Presentation.eStoreContext.Current.Order.save() != 0)
                            {
                                Presentation.eStoreContext.Current.Order = null;
                                Utilities.eStoreLoger.Error("SaveOrderFailed", user.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
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
                                    , user.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);
                            }

                            //生成一个待 审核的log
                            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
                                Presentation.eStoreContext.Current.Store.calculateOrderReward(Presentation.eStoreContext.Current.Order, Presentation.eStoreContext.Current.MiniSite);

                            eStore.Utilities.eStoreLoger.Error("X. msg:success");
                            Response.Write("<result>1</result>" + "<redirecturl>" + esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "completeKuaiQianPayment.aspx?msg=success</redirecturl>");
                            break;
                        }
                    // case POCOS.Payment.PaymentStatus.Referral:
                    default:
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode("PaymentStatus." + paymentRlt.statusX.ToString());
                            Presentation.eStoreContext.Current.Order.save();
                            Response.Write("<result>1</result>" + "<redirecturl>" + esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "completeKuaiQianPayment.aspx?msg=error</redirecturl>");
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error("completeKuaiQianPayment.load Error", "", "", "", ex);
                Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed", null, true);
                Response.Write("<result>1</result>" + "<redirecturl>" + esUtilities.CommonHelper.GetStoreLocation(UseSSL) + "completeKuaiQianPayment.aspx?msg=error</redirecturl>");
                return;
            }
        }


        
    }
}