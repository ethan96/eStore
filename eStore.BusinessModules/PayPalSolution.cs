using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;
using System.Net;

namespace eStore.BusinessModules
{
    class PayPalSolution : PaymentSolution
    {
        private static UserInfo _merchantInfo = null;
        private static PayflowConnectionData _productionConnectionData = null;
        private static PayflowConnectionData _trialConnectionData = null;
        private static CreditCard _trialCreditCard = new CreditCard("4111111111111111", "0823");

        public override Boolean supportDirectAccess()
        {
            return true;
        }

        /// <summary>
        /// This method is a typical method for performing pre-auth transaction.  Delay-Capture transaction will be performed
        /// to realize this pre-auth transaction and complete the payment transaction.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            try
            {
                order.VATNumbe = paymentInfo.FederalID;
                order.PurchaseNO = paymentInfo.PurchaseNO;
                PayflowConnectionData connection = getConnectionData(simulation);

                //preparing transaction data
                //Populate billing inf
                BillTo billTo = new BillTo();   //paypal 
                CartContact orderBillTo = order.cartX.BillToContact;    //estore

                String[] nameFields = paymentInfo.CardHolderName.Split(' ');
                billTo.FirstName = (nameFields.Count() > 0) ? nameFields[0] : "";
                billTo.LastName = (nameFields.Count() > 1) ? nameFields[1] : "";

                if (string.IsNullOrEmpty(orderBillTo.Address2))
                    billTo.Street = orderBillTo.Address1;
                else
                    billTo.Street = string.Format("{0} {1}", orderBillTo.Address1, orderBillTo.Address2);
                billTo.City = orderBillTo.City;
                billTo.Zip = orderBillTo.ZipCode;
                billTo.State = orderBillTo.State;

                // Populate invoice info
                Invoice invoice = new Invoice();
                invoice.BillTo = billTo;
                // transaction amount will be divided by 10 for simulation purpose
                if (simulation)
                    invoice.Amt = new PayPal.Payments.DataObjects.Currency(order.totalAmountX / 10);
                else
                    invoice.Amt = new PayPal.Payments.DataObjects.Currency(order.totalAmountX);
                invoice.PoNum = order.OrderNo;
                invoice.InvoiceDate = DateTime.Now.ToString();  //this format needs to be verified *****************
                invoice.InvNum = "Advantech Corp. " + order.OrderNo;

                //populating credit card details
                CreditCard cc = null;
                if (simulation)
                    cc = _trialCreditCard;
                else
                    cc = new CreditCard(paymentInfo.cardNo, paymentInfo.CardExpiredDate);   //MMYY

                // Populate card tender
                CardTender cardTender = new CardTender(cc);

                // Prepare payment request
                AuthorizationTransaction transaction = new AuthorizationTransaction(merchantInfo, connection, invoice, cardTender, paymentInfo.PaymentID);

                //submit transaction
                if (System.Configuration.ConfigurationManager.AppSettings.Get("enableAllSecurityProtocols") == "true")
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Ssl3;
                }
                transaction.SubmitTransaction();
                String response = transaction.Response.ResponseString;

                //process response and fill up payment status
                processDirectPaymentResponse(response, paymentInfo);

                //reset transaction context, this part is possibly not thread-safe call.  Need more research ************
                PayflowConnectionData.ResetContext();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal("Exception at PayPal payment", order.OrderNo, "", "", ex);
            }

            return paymentInfo;
        }

        /// <summary>
        /// This method supports more transaction types including Authorization transaction, Address validation, void transaction and
        /// changing payment amount transaction.   OM will be the main application using this method.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="transactionType"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public override Payment makePayment(Order order, Payment paymentInfo, Payment.TransactionType transactionType, Boolean simulation = false)
        {
            Payment newPayment = null;
            try
            {
                switch (transactionType)
                {
                    case Payment.TransactionType.Authorization:
                        BillTo billTo = prepareBillToInfo(order);
                        paymentInfo.OrderNo = order.OrderNo;
                        paymentInfo.Amount = order.totalAmountX;
                        paymentInfo.StoreID = order.StoreID;
                        paymentInfo.Order = order;
                        newPayment = authorizePaymentAmount(paymentInfo, billTo, simulation);
                        break;
                    case Payment.TransactionType.Reauthorization:
                        newPayment = reauthorizePayment(paymentInfo, simulation);
                        break;
                    case Payment.TransactionType.AddresValidation:
                        newPayment = validateAddress(order, paymentInfo, simulation);
                        break;
                    case Payment.TransactionType.Void:
                        newPayment = voidPayment(paymentInfo, simulation);
                        break;
                    default:
                        newPayment = new Payment();
                        newPayment.statusX = Payment.PaymentStatus.NotSupported;
                        break;
                }

                newPayment.TranxType = transactionType.ToString();
            }
            catch (Exception)
            {
            }

            return newPayment;
        }

        private Payment authorizePaymentAmount(Payment paymentInfo, BillTo billTo, Boolean simulation)
        {
            try
            {
                PayflowConnectionData connection = getConnectionData(simulation);

                //preparing transaction data
                // Populate invoice info
                Invoice invoice = new Invoice();
                invoice.BillTo = billTo;
                // transaction amount will be divided by 10 for simulation purpose
                if (simulation)
                    invoice.Amt = new PayPal.Payments.DataObjects.Currency(paymentInfo.Amount.GetValueOrDefault() / 10);
                else
                    invoice.Amt = new PayPal.Payments.DataObjects.Currency(paymentInfo.Amount.GetValueOrDefault());
                invoice.PoNum = paymentInfo.OrderNo;
                invoice.InvoiceDate = DateTime.Now.ToString();  
                invoice.InvNum = "Advantech Corp. " + paymentInfo.OrderNo;

                //populating credit card details
                CreditCard cc = null;
                if (simulation)
                    cc = _trialCreditCard;
                else
                    cc = new CreditCard(paymentInfo.cardNo, paymentInfo.CardExpiredDate);   //MMYY

                // Populate card tender
                CardTender cardTender = new CardTender(cc);

                // Prepare payment request
                AuthorizationTransaction transaction = new AuthorizationTransaction(merchantInfo, connection, invoice, cardTender, paymentInfo.PaymentID);

                //submit transaction
                transaction.SubmitTransaction();
                String response = transaction.Response.ResponseString;

                //process response and fill up payment status
                processDirectPaymentResponse(response, paymentInfo);

                //reset transaction context, this part is possibly not thread-safe call.  Need more research ************
                PayflowConnectionData.ResetContext();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal("Exception at PayPal payment", paymentInfo.OrderNo, "", "", ex);
            }

            return paymentInfo;
        }

        private BillTo prepareBillToInfo(Order order)
        {
            //Populate billing info
            BillTo billTo = new BillTo();   //paypal 
            CartContact orderBillTo = order.cartX.BillToContact;    //estore

            billTo.FirstName = orderBillTo.FirstName;
            billTo.LastName = orderBillTo.LastName;

            if (string.IsNullOrEmpty(orderBillTo.Address2))
                billTo.Street = orderBillTo.Address1;
            else
                billTo.Street = string.Format("{0} {1}", orderBillTo.Address1, orderBillTo.Address2);
            billTo.City = orderBillTo.City;
            billTo.Zip = orderBillTo.ZipCode;
            billTo.State = orderBillTo.State;

            return billTo;
        }

        /// <summary>
        /// In this method, we use one-dollar authorization for address validation.  System will automatically void this transaction
        /// right after the address validation.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        private Payment validateAddress(Order order, Payment paymentInfo, Boolean simulation)
        {
            BillTo billTo = prepareBillToInfo(order);
            paymentInfo.Amount = 10;
            paymentInfo.OrderNo = order.OrderNo;

            return authorizePaymentAmount(paymentInfo, billTo, simulation);
        }

        private Payment voidPayment(Payment origPaymentInfo, Boolean simulation)
        {
            Payment newPaymentInfo = new Payment();
            newPaymentInfo.copyAccountInfo(origPaymentInfo);
            newPaymentInfo.Amount = origPaymentInfo.Amount;
            newPaymentInfo.CardType = "Voided Payment";
            newPaymentInfo.Comment1 = "Voided Payment";
            try
            {
                if (!String.IsNullOrEmpty(origPaymentInfo.CCPNREF))
                {
                    PayflowConnectionData connection = getConnectionData(simulation);
                    VoidTransaction transaction = new VoidTransaction(origPaymentInfo.CCPNREF, merchantInfo, connection, newPaymentInfo.PaymentID);
                    transaction.SubmitTransaction();
                    String response = transaction.Response.ResponseString;
                    //process response and fill up payment status
                    processDirectPaymentResponse(response, newPaymentInfo);
                    if (newPaymentInfo.statusX == Payment.PaymentStatus.Approved)
                        origPaymentInfo.statusX = Payment.PaymentStatus.Voided;

                    //reset transaction context, this part is possibly not thread-safe call.  Need more research ************
                    PayflowConnectionData.ResetContext();
                }
                else
                    newPaymentInfo.statusX = Payment.PaymentStatus.Declined;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal("Exception at PayPal payment", newPaymentInfo.OrderNo, "", "", ex);
            }

            return newPaymentInfo;
        }

        /// <summary>
        /// This method can possibly return multiple payment records depending on the payment situation.  If the entire new amount can
        /// be reauthorized, then there will be two payment records returned.  The first one will be the authorization payment and the second one
        /// will be voided payment record.   Otherwise, there will be only one record to alpha 
        /// </summary>
        /// <param name="origPaymentInfo"></param>
        /// <param name="newAmount"></param>
        /// <param name="simulation"></param>
        /// <returns></returns>
        private Payment reauthorizePayment(Payment newPaymentInfo, Boolean simulation)
        {
            try
            {
                if (!String.IsNullOrEmpty(newPaymentInfo.CCPNREF))
                {
                    PayflowConnectionData connection = getConnectionData(simulation);
                    // Populate invoice info
                    Invoice invoice = new Invoice();
                    if (simulation)  // transaction amount will be divided by 10 for simulation purpose
                        invoice.Amt = new PayPal.Payments.DataObjects.Currency(newPaymentInfo.Amount.GetValueOrDefault() / 10);
                    else
                        invoice.Amt = new PayPal.Payments.DataObjects.Currency(newPaymentInfo.Amount.GetValueOrDefault());
                    CreditCard cc = new CreditCard("", "");
                    CardTender cardTender = new CardTender(cc);
                    ReferenceTransaction transaction = new ReferenceTransaction("A", newPaymentInfo.CCPNREF, merchantInfo, connection, invoice, cardTender, newPaymentInfo.PaymentID);
                    transaction.SubmitTransaction();
                    String response = transaction.Response.ResponseString;
                    //process response and fill up payment status
                    processDirectPaymentResponse(response, newPaymentInfo);

                    //reset transaction context, this part is possibly not thread-safe call.  Need more research ************
                    PayflowConnectionData.ResetContext();
                }
                else
                {
                    newPaymentInfo.statusX = Payment.PaymentStatus.Declined;
                }
            }
            catch (Exception ex)
            {
                newPaymentInfo.statusX = Payment.PaymentStatus.Declined;
                eStoreLoger.Fatal("Exception at PayPal payment", newPaymentInfo.OrderNo, "", "", ex);
            }

            return newPaymentInfo;
        }

        private Payment processDirectPaymentResponse(String response, Payment payment)
        {
            //tokenize response and buildup a Dictionary for query
            Dictionary<String, String> resultItems = new Dictionary<string, string>();
            String[] paymentResults = response.Split('&');
            foreach (String item in paymentResults)
            {
                if (! String.IsNullOrEmpty(item))
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
                
            results.TryGetValue("RESULT", out resultCode);
            results.TryGetValue("RESPMSG", out transactionDesc);

            if (String.IsNullOrEmpty(resultCode))
                resultCode = "-200";
            if (String.IsNullOrEmpty(transactionDesc))
                transactionDesc = "Unknow Communication Error";

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

            if (resultValue == 0)
                payment.statusX = Payment.PaymentStatus.Approved;
            //IC 2014/07/08 Add PaymentStatus 23/24/51
            else if (resultValue == 23)
                payment.statusX = Payment.PaymentStatus.InvalidAccountNumber;
            else if (resultValue == 24)
                payment.statusX = Payment.PaymentStatus.InvalidExpirationDate;
            else if (resultValue == 51)
                payment.statusX = Payment.PaymentStatus.ExceedsPerTransactionLimit;
            else if ((resultValue >= 1 && resultValue <= 5) || 
                     (resultValue >= 25 && resultValue <= 29))
                payment.statusX = Payment.PaymentStatus.AuthenticationFailed;
            else if ((resultValue >= 6 && resultValue <= 10) || 
                     (resultValue >= 31 && resultValue <= 37) ||
                     (resultValue >= 100 && resultValue <= 102))
                payment.statusX = Payment.PaymentStatus.NotSupported;
            else if (resultValue == 11)
                payment.statusX = Payment.PaymentStatus.Timeout;
            else if (resultValue == 12 || resultValue == 23 || resultValue == 34)
                payment.statusX = Payment.PaymentStatus.Declined;
            else if (resultValue == 13)
                payment.statusX = Payment.PaymentStatus.Referral;
            else if (resultValue == 30)
                payment.statusX = Payment.PaymentStatus.DuplicateTransaction;
            else if (resultValue == 50)
                payment.statusX = Payment.PaymentStatus.InsufficientFund;
            else if (resultValue == 51)
                payment.statusX = Payment.PaymentStatus.ExceedTransactionLimit;
            else if (resultValue == 103 || resultValue < 0 )
                payment.statusX = Payment.PaymentStatus.CommunicationError;
            else if (resultValue >= 125 && resultValue <= 128)
                payment.statusX = Payment.PaymentStatus.FraudAlert;
            else
                payment.statusX = Payment.PaymentStatus.GeneralError;

            results.Add("Result code", resultCode);
            results.Add("PaymentStatus", payment.statusX.ToString());

            //Add transaction failed information to payment.
            if(resultValue != 0)
                setPaymentErrorCode(results, payment); 
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

            results.TryGetValue("RESULT", out resultCode);
            results.TryGetValue("RESPMSG", out transactionDesc);
            
            //For eStore error code purpose
            String eStoreErrorCodePrefix = "PayPal_PAYMENT_";
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
            
            if (resultValue != 0)
            {
                eStoreErrorCode = eStoreErrorCodePrefix + resultValue;
                payment.errorCode = eStoreErrorCode;
                eStoreLoger.Info(eStoreErrorCode + ", " + transactionDesc + "[OrderNO: " + payment.OrderNo + "]", payment.Order.UserID, "", payment.StoreID);
                results.Add("eStoreErrorCode", eStoreErrorCode);
            }
        }

        /// <summary>
        /// This method will return PayPal connection data
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        private PayflowConnectionData getConnectionData(Boolean simulation)
        {
            //connection settings
            String _hostName = "payflowpro.paypal.com";
            String _hostNameSimulation = "pilot-payflowpro.paypal.com";
            int _hostPort = 443;
            int _timeOut = 30;  //30 seconds

            if (simulation)
            {
                if (_trialConnectionData == null)
                    _trialConnectionData = new PayflowConnectionData(_hostNameSimulation, _hostPort, _timeOut);

                return _trialConnectionData;
            }
            else
            {
                if (_productionConnectionData == null)
                    _productionConnectionData = new PayflowConnectionData(_hostName, _hostPort, _timeOut);

                return _productionConnectionData;
            }
        }

        private UserInfo merchantInfo
        {
            get
            {
                if (_merchantInfo == null)
                {
                    //the following constants are Advantech PayPay user info
                    String _userName = "Advantech";
                    String _vendorName = "Advantech";
                    String _partnerName = "verisign";
                    String _credential = "2ws3ed4rf";

                    _merchantInfo = new UserInfo(_userName, _vendorName, _partnerName, _credential);
                }

                return _merchantInfo;
            }
        }
    }
}
