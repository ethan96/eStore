using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    public interface IPaymentSolutionProvider
    {
        /// <summary>
        /// The return value of this method indicates whether this payment solution provider support direct API call.
        /// </summary>
        /// <returns></returns>
        Boolean supportDirectAccess();

        /// <summary>
        /// This method is for transaction made through direct API call
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo">This parameter needs to contain CreditCard informatin, plus charge amount</param>
        /// <param name="simulation">This Boolean value is an flag specifying coming payment transaction is for real or for simulation purpose</param>
        /// <returns></returns>
        Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false);

        /// <summary>
        /// This method is an extended method of another makePayment method.  It provides more transaction type choices. 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="transactionType"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        Payment makePayment(Order order, Payment paymentInfo, Payment.TransactionType transactionType, Boolean simulation = false);

        /// <summary>
        /// getIndirectPaymentRequestForm shall be invoked in paired with processIndirectPaymentResponse.
        /// Application call getIndirectPaymentRequestForm to embedded it in returning HTML page for client
        /// to submit transaction request to Payment solution provider.  Once payment solution provider complete
        /// payment transaction, it will redirect user back to eStore with transaction status. processIndirectPaymentResponse
        /// will take care of the status validation and create a payment result if it's a successful payment transaction
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        IDictionary<String, String> getIndirectPaymentRequestForm(Order order, Payment paymentInfo, Boolean simulation = false);

        string getIndirectPaymentOrderResponseNO(NameValueCollection response);

        Payment processIndirectPaymentResponse(NameValueCollection response, Order order, Boolean simulation = false);
    }
}
