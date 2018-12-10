using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Web.Security;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using eStore.Utilities;
using System.Collections;
using System.Net;

namespace eStore.BusinessModules
{
     class OgoneDirectLink : ABN_AMROSolution
    {
         private static string _userid = "estoreAdvantech";
         private static string _pswd = "AESC07ccp";
        private static string _commerceUId = "Advantech1";
        private static string _commercepassCode = "!qaz3edc5tgb7ujm";
        //production url: https://secure.ogone.com/ncol/prod/orderdirect.asp
        private static string _url = "https://secure.ogone.com/ncol/prod/orderdirect_utf8.asp";
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

          

            webposter.Add("AMOUNT", paymentAmount);  // amount   
            //webposter.Add("BRAND ", paymentInfo.CardType);  // CardType   
            webposter.Add("CARDNO", paymentInfo.cardNo);  // card no
            webposter.Add("Currency", _currency);  // Currency
            webposter.Add("CVC", paymentInfo.SecurityCode);  //  security code. 
            webposter.Add("ED", paymentInfo.CardExpiredDate);  // expired date
            webposter.Add("operation", operation);  // 
            webposter.Add("orderID", order.OrderNo);  // orderID   
            webposter.Add("PSPID", _commerceUId);  // Affiliation name 
            webposter.Add("PSWD", _pswd);  // password   
            webposter.Add("PM", "CreditCard");  // payment method   
            webposter.Add("USERID", _userid);  // USERID

            CartContact orderBillTo = order.cartX.BillToContact;    //estore
            webposter.Add("OWNERADDRESS", orderBillTo.Address1);
            webposter.Add("OWNERZIP", orderBillTo.ZipCode);
            webposter.Add("OWNERTOWN", orderBillTo.City);
            webposter.Add("OWNERCTY", orderBillTo.countryCodeX );
            webposter.Add("OWNERTELNO", orderBillTo.TelNo);  

            string beforeSHA = string.Join("", webposter.getQueryData().ToArray().OrderBy(x => x).Select(x => string.Format("{0}={1}{2}", x.ToString().Split('=')[0].ToUpper(), x.ToString().Split('=')[1], _commercepassCode)));
            string SHAOut = FormsAuthentication.HashPasswordForStoringInConfigFile(System.Web.HttpUtility.UrlDecode(beforeSHA,Encoding.UTF8), "SHA1");

            webposter.Add("SHASign", SHAOut);  //  security code.                     
            string expected = string.Empty; // TODO: Initialize to an appropriate value

            string responsexml;

            //post the payment to the bank, get response right away

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                responsexml = webposter.GetResponse();
            }
            catch (Exception e)
            {
                responsexml = composeExceptionResponse(order, e);
            }

            ncresponse res = parsingResponse(responsexml);
            Payment payment = processDirectPaymentResponse(res, order, paymentInfo, SHAOut, simulation);
            payment.responseValues.Add("Raw Data", responsexml);

            return payment;
        }

        private String composeExceptionResponse(Order order, Exception ex)
        {
            return String.Format("<?xml version=\"1.0\"?><ncresponse orderID=\"{0}\" PAYID=\"Exception\" NCSTATUS=\"{1}\" NCERROR=\"{2}\" />", order.OrderNo, "Exception", ex.StackTrace.Length > 600 ? ex.StackTrace.Substring(0, 600) : ex.StackTrace);
        }

        /// <summary>
        /// Deserialize XML returned from the bank to class
        /// </summary>
        /// <param name="responsexml"></param>
        /// <returns></returns>
        private ncresponse parsingResponse(string responsexml)
        {

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
                String orderID = response.orderID;
                responseItems.Add("Order No", orderID);
                String amount = response.amount;
                responseItems.Add("Payment Amount", amount);
                String currency = response.currency;
                String acceptance = response.ACCEPTANCE;
                String statusCode = response.STATUS;
                responseItems.Add("Status Code", statusCode);
                String paymentMethod = response.PM;
                String cardNo = payment.cardNo;
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
                    payment = new Payment(cardNo, payment.CardHolderName, cardType, payment.CardExpiredDate, "", paymentAmount);
                }
                payment.responseValues = responseItems;

                //signature validation
                if (simulation)
                {
                    _paymentAmount = "1";
                }
                else
                    _paymentAmount = String.Format("{0:F0}", payment.Amount * 100);

                Dictionary<string, string> requestdata = new Dictionary<string, string>();
                requestdata.Add("AMOUNT", _paymentAmount);  // amount   
                //requestdata.Add("BRAND ", payment.CardType);  // CardType   
                requestdata.Add("CARDNO", payment.cardNo);  // card no
                requestdata.Add("CURRENCY", _currency);  // Currency
                requestdata.Add("CVC", payment.SecurityCode);  //  security code. 
                requestdata.Add("ED", payment.CardExpiredDate);  // expired date
                requestdata.Add("OPERATION", "RES");  // 
                requestdata.Add("ORDERID", order.OrderNo);  // orderID   
                requestdata.Add("PSPID", _commerceUId);  // Affiliation name 
                requestdata.Add("PSWD", _pswd);  // password   
                requestdata.Add("PM", "CreditCard");  // payment method   
                requestdata.Add("USERID", _userid);  // USERID

                CartContact orderBillTo = order.cartX.BillToContact;    //estore
                requestdata.Add("OWNERADDRESS", orderBillTo.Address1);
                requestdata.Add("OWNERZIP", orderBillTo.ZipCode);
                requestdata.Add("OWNERTOWN", orderBillTo.City);
                requestdata.Add("OWNERCTY", orderBillTo.countryCodeX);
                requestdata.Add("OWNERTELNO", orderBillTo.TelNo);  

                string privateKey = string.Join("", requestdata.OrderBy(x => x.Key).Select(x => string.Format("{0}={1}{2}", x.Key, x.Value, _commercepassCode)));
                String SHASIGN = FormsAuthentication.HashPasswordForStoringInConfigFile(privateKey, "SHA1");

                if (String.IsNullOrEmpty(publicKey))
                {
                    payment.statusX = Payment.PaymentStatus.Declined;
                    payment.errorCode = "PaymentStatus." + payment.statusX;
                }
                else
                {
                    if (SHASIGN.Equals(publicKey))  //signature match
                    {
                        setPaymentStatus(payment, statusCode, cvcCode, acceptance, errorCode);
                    }
                    else
                    {
                        payment.statusX = Payment.PaymentStatus.Declined;
                        payment.errorCode = "PaymentStatus." + payment.statusX;
                        payment.TransactionDesc = "SHASIGN mismatch";
                    }


                    payment.CCPREFPSMSG = response.NCERROR + ":" + response.NCERRORPLUS;
                    //payment.errorCode = response.NCERROR;
                    payment.StatusCode = response.STATUS;
                    payment.CCPNREF = response.PAYID;
                    payment.TransactionDesc = "Authorization";
                    payment.CreatedDate = DateTime.Now;
                    payment.CardType = response.BRAND;
                    payment.CCIAVS = response.AAVCheck;

                    try
                    {
                        decimal _amount = 0;
                        if (decimal.TryParse(response.amount, out _amount))
                            payment.Amount = _amount;
                        payment.LastFourDigit = Convert.ToInt16(payment.cardNo.Substring(cardNo.Length - 4));
                    }
                    catch (Exception)
                    {

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
