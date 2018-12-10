using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    class WireTransferSolution : PaymentSolution
    {
        /// <summary>
        /// The return value of this method indicates whether this payment solution provider support direct API call.
        /// </summary>
        /// <returns></returns>
        public override Boolean supportDirectAccess() { return true; }

        /// <summary>
        /// This method is for transaction made through direct API call
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo">This parameter needs to contain CreditCard informatin, plus charge amount</param>
        /// <param name="simulation">This Boolean value is an flag specifying coming payment transaction is for real or for simulation purpose</param>
        /// <returns></returns>
        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            paymentInfo.statusX = Payment.PaymentStatus.NeedAttention;

            Dictionary<String, String> resultItems = new Dictionary<string, string>();
            resultItems.Add("Payment Method", "Wire Transfer");
            resultItems.Add("Order amount", Convert.ToString(order.totalAmountX));
            paymentInfo.responseValues = resultItems;
            order.PurchaseNO = paymentInfo.PurchaseNO;

            return paymentInfo;
        }
    }
}
