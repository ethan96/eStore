using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class ABN_AMROSolution : PaymentSolution
    {
        protected static String _passCode = "1qa2ws3ed";
        protected static String _commerceId = "Advantech";
        protected  static String _currency = "EUR";

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

            formItems.Add("OrderNo", order.OrderNo);
            formItems.Add("orderID", order.OrderNo);
            String paymentAmount = "";
            if (simulation)
                paymentAmount = "1";
            else
                paymentAmount = String.Format("{0:F0}", paymentInfo.Amount * 100);

            formItems.Add("amount", paymentAmount);
            formItems.Add("CN", "");

            //prepare SHASign hash code
            String normalString = order.OrderNo + paymentAmount + _currency + _commerceId + _passCode;
            String SHASign = FormsAuthentication.HashPasswordForStoringInConfigFile(normalString, "SHA1");

            formItems.Add("actionURL", "https://internetkassa.abnamro.nl/ncol/prod/orderstandard.asp");
            //fill up payment response handling methods
            formItems.Add("TP", "");
            formItems.Add("accepturl", esUtilities.CommonHelper.GetStoreLocation(false) + "completeIndirectPayment.aspx");
            formItems.Add("declineurl", esUtilities.CommonHelper.GetStoreLocation(false) + "completeIndirectPayment.aspx");
            formItems.Add("exceptionurl", esUtilities.CommonHelper.GetStoreLocation(false) + "completeIndirectPayment.aspx");
            formItems.Add("cancelurl", esUtilities.CommonHelper.GetStoreLocation(false) + "completeIndirectPayment.aspx");
            formItems.Add("SHASign", SHASign);

            //fill up billing info
            CartContact billTo = order.cartX.BillToContact;
            formItems.Add("ownerZIP", billTo.ZipCode);
            formItems.Add("owneraddress", billTo.Address1);
            //fillup advantech commercial info
            formItems.Add("PSPID", _commerceId);
            formItems.Add("currency", _currency);
            formItems.Add("language", "en_US");
            formItems.Add("TITLE", "Advantech eStore");
            formItems.Add("BGCOLOR", "#FFFFFF");
            formItems.Add("TXTCOLOR", "#0F0F78");
            formItems.Add("TBLBGCOLOR", "#FFFFFF");
            formItems.Add("TBLTXTCOLOR", "#0F0F78");
            formItems.Add("BUTTONBGCOLOR", "#4B78B4");
            formItems.Add("BUTTONTXTCOLOR", "#FFFFFF");
            formItems.Add("FONTTYPE", "FONTTYPE");
            formItems.Add("LOGO", "https://buy.advantech.com/app_themes/aeu/logo.gif");
            //miscellanous settings
            formItems.Add("homeurl", "http://buy.advantech.eu");
            formItems.Add("catalogurl", "http://buy.advantech.eu");
            formItems.Add("PM", "");
            formItems.Add("BRAND", "");
            formItems.Add("Alias", "");
            formItems.Add("AliasUsage", "");
            formItems.Add("AliasOperation", "");

            formItems.Add("COM", "");
            formItems.Add("COMPLUS", "");
            formItems.Add("PARAMPLUS", "");
            formItems.Add("USERID", "");
            formItems.Add("CreditCode", "");

            return formItems;
        }
        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)

        {
            if (response != null && response["orderId"] != null)
                return (string)response["orderId"];
            else
                return string.Empty;
        }
        public override Payment processIndirectPaymentResponse(NameValueCollection responseValues, Order order, Boolean simulation = false)
        {
            //retrieve pending payment from order
            Payment payment = order.getLastOpenPayment();

            try
            {
                Dictionary<String,String> response = convertToDictionary(responseValues);

                //fill up payment values
                String orderID = getValue(response, "orderId");
                String amount = getValue(response, "amount");
                String currency = getValue(response, "currency");
                String acceptance = getValue(response, "ACCEPTANCE");
                String statusCode = getValue(response, "STATUS");
                String paymentMethod = getValue(response, "PM");
                String cardNo = getValue(response, "CARDNO");
                String confirmationNo = getValue(response, "PAYID");
                String errorCode = getValue(response, "NCERROR");
                String cardType = getValue(response, "BRAND");
                String publicKey = getValue(response, "SHASIGN");
                String cvcCode = getValue(response, "CVCCheck");

                if (payment == null)
                {
                    Decimal paymentAmount = 0m;
                    try  {  paymentAmount = Convert.ToDecimal(amount);       }
                    catch (Exception)   {  payment.Amount = order.totalAmountX;       }

                    //create a new payment since the previous one is not available.
                    //This shall not happen in normal situation
                    payment = new Payment(cardNo, getValue(response, "CN"), cardType, getValue(response, "ED"), "", paymentAmount);
                }

                //signature validation
                String privateKey = orderID + currency + amount + paymentMethod + acceptance + statusCode + cardNo + confirmationNo +
                                    errorCode + cardType + _passCode;
                String SHASIGN = FormsAuthentication.HashPasswordForStoringInConfigFile(privateKey, "SHA1");

                if (String.IsNullOrEmpty(publicKey))
                    payment.statusX = Payment.PaymentStatus.Declined;
                else
                {
                    if (SHASIGN.Equals(publicKey))  //signature match
                    {
                        setPaymentStatus(payment, statusCode, cvcCode, acceptance);

                    }
                    else
                    {
                        payment.statusX = Payment.PaymentStatus.Declined;
                        payment.TransactionDesc = "SHASIGN mismatch";
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at ABN payment", "", "", "", ex);
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
        private static Dictionary<String, String> _statusTable = new Dictionary<string,string>()
        {
            {"0", "Incomplete or invalid"},
            {"1", "Cancelled by client"},
            {"2", "Authorization refused"},
            {"4", "Order stored"},
            {"41", "Waiting client payment"},
            {"5", "Authorized"},
            {"51", "Authorization waiting"},
            {"52", "Authorization not known"},
            {"55", "Stand-bo"},
            {"59", "Authoriz. to get manually"},
            {"6", "Authorized and cancelle"},
            {"61", "Author. deletion waitin"},
            {"62", "Author. deletion uncertain"},
            {"63", "Author. deletion refusee"},
            {"64", "Authorized and cancelle"},
            {"7", "Payment deletev"},
            {"71", "Payment deletion pending"},
            {"72", "Payment deletion uncertain"},
            {"73", "Payment deletion refuse"},
            {"74", "Payment delete"},
            {"75", "Deletion processed by merchan"},
            {"8", "Refuno"},
            {"81", "Refund pendin"},
            {"82", "Refund uncertaic"},
            {"83", "Refund refuse"},
            {"84", "Payment declined by the acquired"},
            {"85", "Refund processed by merchan"},
            {"9", "Payment requesteu"},
            {"91", "Payment processin"},
            {"92", "Payment uncertaic"},
            {"93", "Payment refuse"},
            {"94", "Refund declined by the acquirer"},
            {"95", "Payment processed by merchanb"},
            {"99", "Being processe"}
        };

        
        /// <summary>
        /// This method is to interpret payment return status and convert it to eStore standandard payment status
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="status"></param>
        protected void setPaymentStatus(Payment payment, String statusCode, String cvcCode, String acceptanceCode, string errorCode="")
        {
            if (cvcCode!=null && cvcCode.Equals("OK"))
            {
                if (_statusTable.ContainsKey(statusCode))
                {
                    payment.TransactionDesc = _statusTable[statusCode];
                    if (!String.IsNullOrEmpty(acceptanceCode) &&
                        (statusCode.Equals("5") || statusCode.Equals("51") ||
                         statusCode.Equals("9") || statusCode.Equals("91")))
                    {
                        payment.statusX = Payment.PaymentStatus.Approved;
                        payment.errorCode = "PaymentStatus." + payment.statusX;
                    }
                    else if (statusCode.Equals("0") && errorCode.Equals("50001113") && !String.IsNullOrEmpty(acceptanceCode))//Duplicate request
                    {
                        payment.statusX = Payment.PaymentStatus.Approved;
                        payment.errorCode = "PaymentStatus." + payment.statusX;
                    }
                    else
                    {
                        payment.statusX = Payment.PaymentStatus.Declined;
                        payment.errorCode = "PaymentStatus." + payment.statusX;
                    }
                      

                }
                else
                {
                    payment.TransactionDesc = "Rejected";
                    payment.statusX = Payment.PaymentStatus.Declined;
                    payment.errorCode = "PaymentStatus." + payment.statusX;
                }
            }
            else if (statusCode.Equals("0") && errorCode.Equals("50001113") && !String.IsNullOrEmpty(acceptanceCode))//Duplicate request
            {
                payment.statusX = Payment.PaymentStatus.Approved;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }

            else
            {
                payment.TransactionDesc = "CVC Failed";
                payment.statusX = Payment.PaymentStatus.Declined;
                payment.errorCode = "PaymentStatus." + payment.statusX;
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
