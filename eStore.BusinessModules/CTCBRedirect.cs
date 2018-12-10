using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Collections.Specialized;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class CTCBRedirect : PaymentSolution
    {
       
        /// <summary>
        /// The return value of this method indicates whether this payment solution provider support direct API call.
        /// </summary>
        /// <returns></returns>
        public override Boolean supportDirectAccess() { return false; }

        /// <summary>
        /// getIndirectPaymentRequestForm shall be invoked in paired with processIndirectPaymentResponse.
        /// Application call getIndirectPaymentRequestForm to embedded it in returning HTML page for client
        /// to submit transaction request to Payment solution provider.  Once payment solution provider complete
        /// payment transaction, it will redirect user back to eStore with transaction status. processIndirectPaymentResponse
        /// will take care of the status validation and create a payment result if it's a successful payment transaction
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IDictionary<String, String> getIndirectPaymentRequestForm(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            Dictionary<String, String> formItems = new Dictionary<string, string>();


            formItems.Add("actionURL", "https://epos.chinatrust.com.tw/auth/SSLAuthUI.jsp");
            String paymentAmount = "";
            if (simulation)
                paymentAmount = "1";
            else
                paymentAmount = order.totalAmountX.ToString("F0");


            formItems.Add("purchAmt", paymentAmount);
            formItems.Add("MerchantID", "8220131600060");
            formItems.Add("TerminalID", "99810510");
            formItems.Add("MerchantName", "Advantech");
            formItems.Add("customize", "1");
            formItems.Add("lidm", order.OrderNo);
            formItems.Add("merID", "372");
            formItems.Add("AutoCap", "0");
            formItems.Add("txType", "0");
            formItems.Add("AuthResURL", esUtilities.CommonHelper.GetStoreLocation() + "completeIndirectPayment.aspx");
            return formItems;
        }
        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            if (response != null && response["lidm"] != null)
                return (string)response["lidm"];
            else
                return string.Empty;
        }
        public override Payment processIndirectPaymentResponse(NameValueCollection responseValues, Order order, Boolean simulation = false)
        {
            //retrieve pending payment from order
            Payment payment = order.getLastOpenPayment();

            try
            {
                Dictionary<String, String> response = convertToDictionary(responseValues);

                //fill up payment values
                String strStatus = getValue(response, "status");
                String strErrCode = getValue(response, "errcode");
                String strAuthCode = getValue(response, "authCode");
                String strAuthAmount = getValue(response, "authAmt");
                String strFailDesc = getValue(response, "errDesc");
                String strTransNo = getValue(response, "xid");
                String strOrderNO = getValue(response, "lidm");


                if (payment == null)
                {
                    payment = new Payment();

                    Decimal paymentAmount = 0m;
                    try { paymentAmount = Convert.ToDecimal(strAuthAmount);
                    payment.Amount = order.totalAmountX; ;
                    payment.OrderNo = order.OrderNo;
                    }
                    catch (Exception) { 
                    
                    }
                    //payment = new Payment(strOrderNO, getValue(response, "CN"), cardType, getValue(response, "ED"), "", paymentAmount);
                }

                if (strStatus.Equals("0"))
                    if (payment.Amount.Value.ToString("F0") == strAuthAmount && payment.OrderNo == strOrderNO)
                        setPaymentStatus(payment, strStatus, "OK", strAuthCode);
                else
                    payment.statusX = Payment.PaymentStatus.Declined;

                response.Add("Payment Status", payment.statusX.ToString());
                response.Add("Payment Amount", Convert.ToString(payment.Amount.GetValueOrDefault()));
                payment.responseValues = response;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at CTCBRedirect payment", "", "", "", ex);
                payment.statusX = Payment.PaymentStatus.Declined;
            }

            return payment;
        }


        private String getValue(Dictionary<String, String> dictionary, String key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return "";
        }

        private Dictionary<String, String> convertToDictionary(NameValueCollection items)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();

            try
            {
                foreach (String key in items.AllKeys)
                {
                    if (!String.IsNullOrEmpty(key))
                        dictionary.Add(key, items[key]);
                }
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        /// <summary>
        /// This disctionary provides interface to get human readable description of specifying status code
        /// </summary>
        private static Dictionary<String, String> _statusTable = new Dictionary<string, string>()
        {
            {"0", "Authorized"} 
        };


        /// <summary>
        /// This method is to interpret payment return status and convert it to eStore standandard payment status
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="status"></param>
        protected void setPaymentStatus(Payment payment, String statusCode, String cvcCode, String acceptanceCode)
        {
            if (cvcCode != null && cvcCode.Equals("OK"))
            {
                if (_statusTable.ContainsKey(statusCode))
                {
                    payment.TransactionDesc = _statusTable[statusCode];
                    if (!String.IsNullOrEmpty(acceptanceCode) &&
                        (statusCode.Equals("0")))
                    {
                        payment.statusX = Payment.PaymentStatus.Approved;
                    }
                    else
                        payment.statusX = Payment.PaymentStatus.Declined;
                }
                else
                {
                    payment.TransactionDesc = "Rejected";
                    payment.statusX = Payment.PaymentStatus.Declined;
                }
            }
            else
            {
                payment.TransactionDesc = "CVC Failed";
                payment.statusX = Payment.PaymentStatus.Declined;
            }
        }

        private void setPaymentInfo(Payment payment, Order order, Dictionary<String, String> response)
        {
            try
            {
                payment.CCResultCode = getValue(response, "STATUS");
                payment.CCPNREF = getValue(response, "PNREF");
                payment.CCAuthCode = getValue(response, "PAYID");
                payment.CCRESPMSG = getValue(response, "NCERROR");
                payment.CCAVSAddr = "";
                payment.CCAVSZIP = "";
                payment.CCIAVS = getValue(response, "BRAND");
                payment.CCPREFPSMSG = "";
                payment.CCPOSTFPSMSG = getValue(response, "ACCEPTANCE");
                payment.CardType = getValue(response, "PM");
                payment.cardNo = getValue(response, "CARDNO");
                payment.CardHolderName = getValue(response, "CN");
                payment.CardExpiredDate = getValue(response, "ED");
                payment.CCUser1 = payment.CardHolderName;

                //convert namevaluecollection to dictionary
                payment.responseValues = response;

                payment.LastFourDigit = Convert.ToInt16(payment.cardNo.Substring(payment.cardNo.Length - 4));
            }
            catch (Exception)
            {
            }
        }
    }
}