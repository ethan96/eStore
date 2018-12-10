using eStore.POCOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using eStore.Utilities;
using System.Collections.Specialized;

namespace eStore.BusinessModules
{
    class AuthorizeNetSolution :PaymentSolution
    {

        public override bool supportDirectAccess()
        {
            return true;
        }

        private string _ApiLoginID = "";
        private string _ApiTransactionKey = "";

        public override Payment makePayment(Order order, Payment paymentInfo, bool simulation = false)
        {
            try
            {
                if (simulation)
                {
                    ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
                    _ApiLoginID = "2mph3GZ9y";
                    _ApiTransactionKey = "9cqEa4N2z84ZH6M7";
                }
                else
                {
                    ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
                    _ApiLoginID = "8Z5XRg8nB";
                    _ApiTransactionKey = "3DPQc4953he62w7n";
                }

                // define the merchant information (authentication / transaction id)
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
                {
                    name = _ApiLoginID,
                    ItemElementName = ItemChoiceType.transactionKey,
                    Item = _ApiTransactionKey,
                };

                var creditCard = new creditCardType();
                if (simulation)
                {
                    creditCard = new creditCardType
                    {
                        cardNumber = "4111111111111111",
                        expirationDate = "2027-08",
                        cardCode = "123"
                    };
                }
                else
                {
                    creditCard = new creditCardType
                    {
                        cardNumber = paymentInfo.cardNo,
                        expirationDate = paymentInfo.CardExpiredDate,
                        cardCode = paymentInfo.SecurityCode
                    };
                }


                //preparing transaction data
                //Populate billing inf
                CartContact orderBillTo = order.cartX.BillToContact;    //estore
                String[] nameFields = paymentInfo.CardHolderName.Split(' ');

                string billToStreet = "";
                if (string.IsNullOrEmpty(orderBillTo.Address2))
                    billToStreet = orderBillTo.Address1;
                else
                    billToStreet = string.Format("{0} {1}", orderBillTo.Address1, orderBillTo.Address2);

                var billingAddress = new customerAddressType
                {
                    firstName = (nameFields.Count() > 0) ? nameFields[0] : "",
                    lastName = (nameFields.Count() > 1) ? nameFields[1] : "",
                    address = billToStreet,
                    city = orderBillTo.City,
                    zip = orderBillTo.ZipCode
                };

                //standard api call to retrieve response
                var paymentType = new paymentType { Item = creditCard };

                // Add line Items
                //var lineItems = new lineItemType[2];
                //lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
                //lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };

                var requestOrder = new orderType { invoiceNumber = order.OrderNo};

                var transactionRequest = new transactionRequestType
                {
                    transactionType = transactionTypeEnum.authOnlyTransaction.ToString(),    // charge the card
                    amount = order.totalAmountX,
                    payment = paymentType,
                    billTo = billingAddress,
                    order = requestOrder

                };

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var request = new createTransactionRequest { transactionRequest = transactionRequest };

                // instantiate the contoller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();

                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();


                if (response != null)
                {
                    processDirectPaymentResponse(response, paymentInfo);
                }
                else
                {
                    paymentInfo.errorCode =  "PaymentStatus." + "Declined";
                    eStoreLoger.Info("Null Response at Authorize.Net payment" + "[OrderNO: " + order.OrderNo + "]", order.UserID, "", order.StoreID);
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal("Exception at Authorize.Net payment", order.OrderNo, "", "", ex);
            }
            return paymentInfo;
        }

        private Payment processDirectPaymentResponse(createTransactionResponse response, Payment payment)
        {
            //tokenize response and buildup a Dictionary for query
            Dictionary<String, String> resultItems = new Dictionary<string, string>();

            //validate
            if (response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse.messages != null)
                {
                    //Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                    //Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                    //Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                    //Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                    //Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);

                    //resultItems.Add("TransID", response.transactionResponse.transId);
                    resultItems.Add("ResultCode", response.transactionResponse.responseCode);
                    //resultItems.Add("MessageCode", response.transactionResponse.messages[0].code);
                    resultItems.Add("ResultMessage", response.transactionResponse.messages[0].description);
                    resultItems.Add("AuthCode", response.transactionResponse.authCode);


                }
                else
                {
                    //Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse.errors != null)
                    {
                        //Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        //Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        resultItems.Add("ResultCode", response.transactionResponse.errors[0].errorCode);
                        resultItems.Add("ResultMessage", response.transactionResponse.errors[0].errorText);
                    }
                }
            }
            else
            {
                //Console.WriteLine("Failed Transaction.");
                if (response.transactionResponse != null && response.transactionResponse.errors != null)
                {
                    //Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                    //Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    resultItems.Add("ResultCode", response.transactionResponse.errors[0].errorCode);
                    resultItems.Add("ResultMessage", response.transactionResponse.errors[0].errorText);
                }
                else
                {
                    //Console.WriteLine("Error Code: " + response.messages.message[0].code);
                    //Console.WriteLine("Error message: " + response.messages.message[0].text);
                    resultItems.Add("ResultCode", response.messages.message[0].code);
                    resultItems.Add("ResultMessage", response.messages.message[0].text);
                }
            }


            //if (response.messages != null)
            //{
            //    resultItems.Add("ReponseResult", response.messages.resultCode.ToString());
            //    resultItems.Add("ReponseMessageCode", response.messages.message[0].code);
            //    resultItems.Add("ReponseMessage", response.messages.message[0].text);
            //}
            //if (response.transactionResponse.authCode != null)
            //    resultItems.Add("AuthCode", response.transactionResponse.authCode);
            //if (response.transactionResponse.responseCode != null)
            //    resultItems.Add("ResultCode", response.transactionResponse.responseCode);
            //if (response.transactionResponse.messages != null)
            //{
            //    resultItems.Add("MessageCode", response.transactionResponse.messages[0].code);
            //    resultItems.Add("Description", response.transactionResponse.messages[0].description);
            //}
            if (response.transactionResponse.accountNumber != null)
                resultItems.Add("AccountNumber", response.transactionResponse.accountNumber);
            if (response.transactionResponse.accountType != null)
                resultItems.Add("AccountType", response.transactionResponse.accountType);
            if (response.transactionResponse.avsResultCode != null)
                resultItems.Add("AVSResultCode", response.transactionResponse.avsResultCode);
            if (response.transactionResponse.cavvResultCode != null)
                resultItems.Add("CAVVResultCode", response.transactionResponse.cavvResultCode);
            if (response.transactionResponse.cvvResultCode != null)
                resultItems.Add("CVVResultCode", response.transactionResponse.cvvResultCode);
            if (response.transactionResponse.transId != null)
                resultItems.Add("TransID", response.transactionResponse.transId);
            if (response.transactionResponse.transHash != null)
                resultItems.Add("TransHash", response.transactionResponse.transHash);
            if (response.transactionResponse.transHashSha2 != null)
                resultItems.Add("TransHashSha2", response.transactionResponse.transHashSha2);
            if (response.transactionResponse.testRequest != null)
                resultItems.Add("TestRequest", response.transactionResponse.testRequest);
            if (response.transactionResponse.entryMode!=null)
                resultItems.Add("EntryMode", response.transactionResponse.entryMode);
            if (response.transactionResponse.rawResponseCode != null)
                resultItems.Add("RawResponseCode", response.transactionResponse.rawResponseCode);
            if (response.transactionResponse.refTransID!=null)
                resultItems.Add("RefTransID", response.transactionResponse.refTransID);
            if (response.transactionResponse.splitTenderId != null)
                resultItems.Add("SplitTenderId", response.transactionResponse.splitTenderId);
            //if (response.transactionResponse.errors != null)
            //    resultItems.Add("ResponseErrorCode", response.transactionResponse.errors[0].errorCode);
            //if (response.transactionResponse.errors != null)
            //    resultItems.Add("ResponseErrorMessage", response.transactionResponse.errors[0].errorText);
            if (response.transactionResponse.prePaidCard != null)
            {
                resultItems.Add("refTransID", response.transactionResponse.prePaidCard.approvedAmount);
                resultItems.Add("refTransID", response.transactionResponse.prePaidCard.balanceOnCard);
                resultItems.Add("refTransID", response.transactionResponse.prePaidCard.requestedAmount);
            }


            //process payment status
            setPaymentStatus(resultItems, payment);

            //Add transaction failed information to payment.
            //if (response.transactionResponse.responseCode != Convert.ToString(1))
            //    setPaymentErrorCode(resultItems, payment);

            resultItems.Add("Payment amount", Convert.ToString(payment.Amount.GetValueOrDefault()));

            //Process the rest of payment transaction result
            payment.CCAuthCode = resultItems.ContainsKey("AuthCode") ? resultItems["AuthCode"] : "";
            payment.CCAVSAddr =  "";
            payment.CCAVSZIP =  "";
            payment.CCIAVS = "";
            payment.CCPOSTFPSMSG =  "";
            payment.CCPREFPSMSG = "";
            payment.CCPNREF = resultItems.ContainsKey("TransID") ? resultItems["TransID"] : "";
            payment.CCRESPMSG = resultItems.ContainsKey("ResultMessage") ? resultItems["ResultMessage"] : "";
            payment.CCResultCode = resultItems.ContainsKey("ResultCode") ? resultItems["ResultCode"] : "";
            payment.responseValues = resultItems;

            //record last 4 digits of credit card regardless
            String cardNo = payment.cardNo;
            try
            {
                payment.LastFourDigit = Convert.ToInt16(cardNo.Substring(cardNo.Length - 4));
            }
            catch (Exception)
            {
                //this shall never arrive
            }

            return payment;
        }

        private Payment processDirectPaymentResponse(String response, Payment payment)
        {
            //tokenize response and buildup a Dictionary for query
            Dictionary<String, String> resultItems = new Dictionary<string, string>();
            String[] paymentResults = response.Split('&');
            foreach (String item in paymentResults)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    String[] tokens = item.Split('=');
                    if (tokens.Count() > 1) //it's only meaningful when both key and value are available.
                    {
                        if (!resultItems.ContainsKey(tokens[0]))
                            resultItems.Add(tokens[0], tokens[1]);
                        else
                        {
                            //new key
                            String newKey = tokens[0] + ".1";
                            if (!resultItems.ContainsKey(newKey))
                                resultItems.Add(newKey, tokens[1]);
                            else
                                resultItems[newKey] = tokens[1];
                        }
                    }
                    else
                    {
                        if (!resultItems.ContainsKey(tokens[0]))
                            resultItems.Add(tokens[0], "");
                    }
                }
            }

            //process payment status
            setPaymentStatus(resultItems, payment);
            resultItems.Add("Payment amount", Convert.ToString(payment.Amount.GetValueOrDefault()));

            //Process the rest of payment transaction result
            payment.CCAuthCode = resultItems.ContainsKey("AUTHCODE") ? resultItems["AUTHCODE"] : "";
            payment.CCAVSAddr = resultItems.ContainsKey("AVSADDR") ? resultItems["AVSADDR"] : "";
            payment.CCAVSZIP = resultItems.ContainsKey("AVSZIP") ? resultItems["AVSZIP"] : "";
            payment.CCIAVS = resultItems.ContainsKey("IAVS") ? resultItems["IAVS"] : "";
            payment.CCPNREF = resultItems.ContainsKey("PNREF") ? resultItems["PNREF"] : "";
            payment.CCPOSTFPSMSG = resultItems.ContainsKey("POSTFPSMSG") ? resultItems["POSTFPSMSG"] : "";
            payment.CCPREFPSMSG = resultItems.ContainsKey("PREFPSMSG") ? resultItems["PREFPSMSG"] : "";
            payment.CCRESPMSG = resultItems.ContainsKey("RESPMSG") ? resultItems["RESPMSG"] : "";
            payment.CCResultCode = resultItems.ContainsKey("RESULT") ? resultItems["RESULT"] : "";
            payment.responseValues = resultItems;

            //record last 4 digits of credit card regardless
            //if (payment.statusX == Payment.PaymentStatus.Approved)
            {
                String cardNo = payment.cardNo;
                try
                {
                    payment.LastFourDigit = Convert.ToInt16(cardNo.Substring(cardNo.Length - 4));
                }
                catch (Exception)
                {
                    //this shall never arrive
                }
            }

            return payment;
        }

        /// <summary>
        /// This method will retrive transaction status and message from payment response
        /// </summary>
        /// <param name="results"></param>
        /// <param name="payment"></param>
        private void setPaymentStatus(IDictionary<String, String> results, Payment payment)
        {
            String resultCode = null;
            String transactionDesc = null;

            results.TryGetValue("ResultCode", out resultCode);
            results.TryGetValue("ResultMessage", out transactionDesc);

            if (String.IsNullOrEmpty(resultCode))
                resultCode = "-200";
            if (String.IsNullOrEmpty(transactionDesc))
            {
                transactionDesc = "Unknow Communication Error";
            }
            payment.TransactionDesc = transactionDesc;

            int resultValue = -200;
            if (String.IsNullOrEmpty(resultCode))
                resultValue = -200;
            else
            {
                try
                {
                    resultValue = Convert.ToInt16(resultCode);
                }
                catch (Exception)
                {
                    //ignore and do nothing
                    resultValue = -200;
                }
            }

            //if (resultValue == 1)
            //    payment.statusX = Payment.PaymentStatus.Approved;
            //else if (resultValue == 2)
            //    payment.statusX = Payment.PaymentStatus.Declined;
            //else if (resultValue == 3)
            //    payment.statusX = Payment.PaymentStatus.GeneralError;
            //else if (resultValue == 4)
            //    payment.statusX = Payment.PaymentStatus.HeldForReview;
            //else
            //    payment.statusX = Payment.PaymentStatus.GeneralError;



            if (resultValue == 1)
            {
                payment.statusX = Payment.PaymentStatus.Approved;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }
            else if (resultValue == 5 || resultValue == 10)
            {
                payment.statusX = Payment.PaymentStatus.InvalidAmount;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }
            else if (resultValue == 6)
            {
                payment.statusX = Payment.PaymentStatus.InvalidAccountNumber;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }
            else if (resultValue == 7 || resultValue == 8)
            {
                payment.statusX = Payment.PaymentStatus.InvalidExpirationDate;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }
            else if (resultValue == 11)
            {
                payment.statusX = Payment.PaymentStatus.DuplicateTransaction;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }
            else
            {
                payment.statusX = Payment.PaymentStatus.Declined;
                payment.errorCode = "PaymentStatus." + payment.statusX;
            }

            results.Add("PaymentStatus", payment.statusX.ToString());
            if (resultValue != 1)
            {
                payment.statusX = Payment.PaymentStatus.Declined;
                payment.errorCode = "PaymentStatus." + payment.statusX;
                //setPaymentErrorCode(results, payment); 詳細Error Code Msg未來擴充
            }
        }

        /// <summary>
        /// This method is used to add eStoreErrorCode to Payment
        /// </summary>
        /// <param name="results"/></param>
        /// <param name="payment"></param>
        private void setPaymentErrorCode(IDictionary<String, String> results, Payment payment)
        {
            String resultCode = null;
            String transactionDesc = null;

            results.TryGetValue("ResultCode", out resultCode);
            results.TryGetValue("ResultMessage", out transactionDesc);

            //For eStore error code purpose
            String eStoreErrorCodePrefix = "AuthorizeNet_PAYMENT_";
            String eStoreErrorCode = "";

            int resultValue = -200;
            if (String.IsNullOrEmpty(resultCode))
                resultValue = -200;
            else
            {
                try
                {
                    resultValue = Convert.ToInt16(resultCode);
                }
                catch (Exception)
                {
                    //ignore and do nothing
                    resultValue = -200;
                }
            }

            if (resultValue != 1)
            {
                eStoreErrorCode = eStoreErrorCodePrefix + resultValue;
                payment.errorCode = eStoreErrorCode;
                eStoreLoger.Info(eStoreErrorCode + ", " + transactionDesc + "[OrderNO: " + payment.OrderNo + "]", payment.Order.UserID, "", payment.StoreID);
                results.Add("eStoreErrorCode", eStoreErrorCode);
            }
        }
    }
}
