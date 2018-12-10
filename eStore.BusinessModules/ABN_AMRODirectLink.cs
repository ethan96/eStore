using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using eStore.POCOS;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class ABN_AMRODirectLink : ABN_AMROSolution
    {
        private static string _userid="estoreAdvantech";
        private static string _pswd = "YQHTHA20";
        private static string _url = "https://internetkassa.abnamro.nl/ncol/prod/orderdirect.asp";
        private static int _timeout = 10000;

        public Payment sendDirectLinkPayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {

            WebPostRequest webposter = new WebPostRequest(_url);
            webposter.setTimeout(_timeout);

            string operation = "RES"; //Authorization  

            string paymentAmount;

            if (simulation)
            {
                paymentAmount = "1";
                paymentInfo.Amount = 1;
            }
            else
                paymentAmount = String.Format("{0:F0}", paymentInfo.Amount * 100);  

            string beforeSHA = order.OrderNo + paymentAmount + _currency +  paymentInfo.cardNo + _commerceId + operation + _passCode;
            string SHAOut = FormsAuthentication.HashPasswordForStoringInConfigFile(beforeSHA, "SHA1");      

            webposter.Add("AMOUNT", paymentAmount );  // amount   
            webposter.Add("CARDNO", paymentInfo.cardNo );  // card no
            webposter.Add("Currency ", _currency);  // Currency
            webposter.Add("CVC", paymentInfo.SecurityCode);  //  security code. 
            webposter.Add("ED", paymentInfo.CardExpiredDate);  // expired date
            webposter.Add("operation", operation);  // 
            webposter.Add("orderID", order.OrderNo);  // orderID   
            webposter.Add("PSPID", _commerceId);  // Affiliation name 
            webposter.Add("PSWD", _pswd);  // password   
            webposter.Add("PM", "CreditCard");  // payment method   
            webposter.Add("USERID", _userid);  // USERID

            webposter.Add("SHASign", SHAOut);  //  security code.                     
            string expected = string.Empty; // TODO: Initialize to an appropriate value

            string responsexml;

            //post the payment to the bank, get response right away

            try
            {
                responsexml = webposter.GetResponse();
            }
            catch (Exception e)
            {
                responsexml = composeExceptionResponse(order, e);
            }

            ncresponse res = parsingResponse(responsexml);
            Payment payment =  processDirectPaymentResponse(res, order, paymentInfo, SHAOut, simulation);
            payment.responseValues.Add("Raw Data", responsexml);

            return payment;
        }

        private String composeExceptionResponse(Order order, Exception ex)
        {
            return String.Format("<?xml version=\"1.0\"?><ncresponse orderID=\"{0}\" PAYID=\"Exception\" NCSTATUS=\"{1}\" NCERROR=\"{2}\" />", order.OrderNo, "Exception",ex.StackTrace.Length > 600 ? ex.StackTrace.Substring(0, 600) : ex.StackTrace);
        }

        /// <summary>
        /// Deserialize XML returned from the bank to class
        /// </summary>
        /// <param name="responsexml"></param>
        /// <returns></returns>
        private ncresponse parsingResponse(string responsexml){
            
            XmlSerializer xmls = new XmlSerializer(typeof(ncresponse));
            StringReader sr = new StringReader(responsexml);
            XmlTextReader xr = new XmlTextReader(sr);
            ncresponse response = (ncresponse)xmls.Deserialize(xr);
            return response;

        }

        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
           {
               paymentInfo.OrderNo = order.OrderNo;
               paymentInfo.Amount = order.totalAmountX;
               paymentInfo.StoreID = order.StoreID;
               paymentInfo.Order = order;
               Payment payment = sendDirectLinkPayment(order, paymentInfo, simulation);

               return payment;

           }

        private Payment processDirectPaymentResponse(ncresponse response, Order order, Payment payment, string publicKey, Boolean simulation)
        {          
            try
            {
                Dictionary<String, String> responseItems = new Dictionary<string, string>();

                //fill up payment values
                String orderID = response.orderID ;
                responseItems.Add("Order No", orderID);
                String amount = response.amount;
                responseItems.Add("Payment Amount", amount);
                String currency = response.currency;
                String acceptance = response.ACCEPTANCE;
                String statusCode = response.STATUS;
                responseItems.Add("Status Code", statusCode);
                String paymentMethod = response.PM;
                String cardNo = payment.cardNo ;
                String confirmationNo = response.PAYID;
                String errorCode = response.NCERROR;
                responseItems.Add("Error Code", errorCode);
                String cardType = payment.CardType;

                String cvcCode = response.CVCCheck;
                string _paymentAmount = "";

                if (payment == null)
                {
                    Decimal paymentAmount = 0m;
                    try { paymentAmount = Convert.ToDecimal(amount); }
                    catch (Exception) { payment.Amount = order.totalAmountX; }

                    //create a new payment since the previous one is not available.
                    //This shall not happen in normal situation
                    payment = new Payment(cardNo, payment.CardHolderName, cardType, payment.CardExpiredDate , "", paymentAmount);
                }
                payment.responseValues = responseItems;

                //signature validation
                if (simulation)
                {
                    _paymentAmount = "1";                    
                }
                else
                    _paymentAmount = String.Format("{0:F0}", payment.Amount * 100);

                String privateKey = order.OrderNo + _paymentAmount + _currency + payment.cardNo + _commerceId + "RES" + _passCode;
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


                    payment.CCPREFPSMSG = response.NCERROR + ":" + response.NCERRORPLUS;
                    payment.errorCode = response.NCERROR;
                    payment.StatusCode = response.STATUS;
                    payment.CCPNREF = response.PAYID;
                    payment.TransactionDesc = "Authorization";
                    payment.CreatedDate = DateTime.Now;
                    payment.CardType = response.BRAND;
                    payment.CCIAVS  = response.AAVCheck;

                    try
                    {
                        decimal _amount = 0;
                        if (decimal.TryParse(response.amount, out _amount))
                            payment.Amount = _amount;
                        payment.LastFourDigit = Convert.ToInt16(payment.cardNo.Substring(cardNo.Length - 4));
                    }
                    catch (Exception) { 
                        
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at ABN Direct payment", "", "", "", ex);
                payment.statusX = Payment.PaymentStatus.Declined;
            }

            return payment;
        }

    }

}
