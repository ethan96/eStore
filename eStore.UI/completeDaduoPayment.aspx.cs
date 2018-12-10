using eStore.POCOS;
using eStore.Utilities;
using esUtilities;
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
    public partial class completeDaduoPayment : System.Web.UI.Page
    {
        /// <summary>
        /// for daoupay server 
        /// daoup serve will send message to this page 
        /// test server ip is: 123.140.121.205
        /// get daoupay server request 
        /// will update user order and send success tag.
        /// </summary>

        private List<string> ips = new List<string>()
        {
            "123.140.121.205", // kaoupay test server
            "::1;127.0.0.1",
            "27.102.213.200-27.102.213.209" // kaoupay production server
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            NameValueCollection responseValues = new NameValueCollection();
            responseValues = Request.Params;


            //StringBuilder sb = new StringBuilder();
            //sb.Append("IP: " + responseValues["HTTP_X_FORWARDED_FOR"] + "\r\n");
            //sb.Append("time: " + DateTime.Now.ToString() + "\r\n");
            //foreach (var item in responseValues.AllKeys)
            //{
            //    sb.AppendFormat("{0}: {1} \r\n", item, responseValues[item]);
            //}

            //System.IO.File.WriteAllText("c:\\log\\asp_EX_" + (new Random()).Next(9999) + ".txt", sb.ToString());

            if (!checkServerIp(responseValues)) //ip 地址写在 httpheader中，泪奔。
            {
                ltResult.Text = "Fail-b";
                return;
            }
            string orderno = responseValues["ORDERNO"];
            string userId = responseValues["RESERVEDSTRING"]; 
            string payMethod = responseValues["PAYMETHOD"];
            //string accountNo = responseValues["ACCOUNTNO"]; 
            string paymentId = responseValues["RESERVEDINDEX1"];
            //string amount = responseValues["AMOUNT"];

            if (!checkBaseInfo(orderno, userId, payMethod, paymentId))
            {
                ltResult.Text = "Fail-infor";
                return;
            }

            POCOS.Order order = Presentation.eStoreContext.Current.Store.getOrder(orderno);

            if (order == null || order.isConfirmdOrder)
            {
                ltResult.Text = "Fail-confirm";
                return;
            }

            if (!userId.Equals(order.UserID, StringComparison.OrdinalIgnoreCase))
            {
                ltResult.Text = "Fail-user";
                return;
            }

            try
            {
                POCOS.StorePayment payment = Presentation.eStoreContext.Current.Store.getStorePayment(order.PaymentType);
                Payment paymentRlt = Presentation.eStoreContext.Current.Store.completeIndirectPayment(order, payment, responseValues);
                //sb.AppendFormat("{0}: {1} \r\n", "paymentRlt.statusX", paymentRlt.statusX.ToString());
                switch (paymentRlt.statusX)
                {
                    case POCOS.Payment.PaymentStatus.Approved:
                    case POCOS.Payment.PaymentStatus.NeedAttention:
                    case Payment.PaymentStatus.FraudAlert://for simulation
                        {
                            if (order.save() != 0)
                            {
                                Utilities.eStoreLoger.Error("SaveOrderFailed - " + orderno, userId, Page.Request.UserHostAddress, null, null);
                                ltResult.Text = "Fail-save";
                                return;
                            }

                            eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                            mailTemplate.isShowStorePrice = eStore.Presentation.eStoreContext.Current.getBooleanSetting("willShowStoreCurrencyTotalAmount");
                            EMailReponse response = mailTemplate.getOrderMailContent(order, null ,null);
                            if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                            {
                                Utilities.eStoreLoger.Error(string.Format("{0} sent mail failed. {1}", order.OrderNo, response.ErrCode.ToString())
                                    , "Daoupay server", Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, null);

                            }

                            //生成一个待 审核的log
                            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
                                Presentation.eStoreContext.Current.Store.calculateOrderReward(order, null);

                            ltResult.Text = "<RESULT>SUCCESS</RESULT>";
                            break;
                        }
                    // case POCOS.Payment.PaymentStatus.Referral:
                    default:
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode("PaymentStatus." + paymentRlt.statusX.ToString());
                            //Presentation.eStoreContext.Current.Order.save();
                            ltResult.Text = "Fail-status";
                            eStoreLoger.Error("payment error paymentid: {" + paymentRlt.PaymentID + "} payment status: {" + paymentRlt.statusX.ToString() + "}", "", "", "");
                            break;
                        }
                }
                
            }
            catch (Exception ex)
            {
                ltResult.Text = "Fail-ex";
                eStoreLoger.Error("payment error orderNo: {" + orderno + "}", "", "", "", ex);
                return;
            }
            //finally
            //{
            //    System.IO.File.WriteAllText("c:\\log\\asp_EX-2_" + (new Random()).Next(9999) + ".txt", sb.ToString());
            //}
        }

        /// <summary>
        /// check request is from daoupay server or not.
        /// </summary>
        /// <returns></returns>
        private bool checkServerIp(NameValueCollection responseValues)
        {
            if (responseValues["HTTP_X_FORWARDED_FOR"] == null)
                return false;
            return esUtilities.IPUtility.IpIsWithinBoliviaRange(Page.Request.UserHostAddress, ips)
                    || esUtilities.IPUtility.IpIsWithinBoliviaRange(responseValues["HTTP_X_FORWARDED_FOR"], ips);
        }

        /// <summary>
        /// check base return information
        /// </summary>
        /// <param name="orderno"></param>
        /// <param name="userId"></param>
        /// <param name="payMethod"></param>
        /// <param name="accountNo"></param>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        private bool checkBaseInfo(string orderno, string userId, string payMethod, string paymentId)
        {
            return !string.IsNullOrEmpty(orderno) &&
                    !string.IsNullOrEmpty(userId) &&
                    !string.IsNullOrEmpty(payMethod) &&
                    !string.IsNullOrEmpty(paymentId);
        }
    }
}