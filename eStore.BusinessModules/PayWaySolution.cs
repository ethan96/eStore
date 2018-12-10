using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using eStore.POCOS;
using eStore.Utilities;
using Qvalent.PayWay;

namespace eStore.BusinessModules
{
    class PayWaySolution : PaymentSolution
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
            try
            {
                PayWayAPI paymentGateway = getPaymentGateway();
                order.Surcharge = 0m;
                if (paymentInfo.CardType == "American Express")
                {
                    order.Surcharge = Converter.CartPriceRound( order.totalAmountX * 0.03m,order.StoreID);
                    paymentInfo.Amount = order.totalAmountX;
                }

                Dictionary<String, String> paramList = new Dictionary<string, string>();
                addAuthInfo(paramList, simulation);
                addOrderInfo(paramList, paymentInfo, order);
                addCardInfo(paramList, paymentInfo, simulation);
                String request = paymentGateway.FormatRequestParameters(paramList);

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                //send transaction request
                String response = paymentGateway.ProcessCreditCard(request);

                //receive and process response
                Dictionary<String, String> resultSet = new Dictionary<string, string>();
                resultSet.Add("Order Amount", order.totalAmountX.ToString());
                String name = paymentGateway.ParseResponseParameters(response).GetType().FullName;
                Hashtable responseSet = (Hashtable)paymentGateway.ParseResponseParameters(response);
                foreach (String key in responseSet.Keys)
                    resultSet.Add(key, (String)responseSet[key]);

                //Fill up payment information
                fillupPayment(resultSet, paymentInfo);
            }
            catch (Exception ex)
            {
                paymentInfo.statusX = Payment.PaymentStatus.Declined;
                paymentInfo.TransactionDesc = "Doesn't have sufficient information";

                eStoreLoger.Fatal("Exception at VeriTransPayment solution", "", "", "", ex);
            }

            return paymentInfo;
        }


        /// <summary>
        /// This method will initial PaymentGateway connection and return the initialized connection back
        /// </summary>
        /// <returns></returns>
        private PayWayAPI getPaymentGateway()
        {
            PayWayAPI paymentGateway = new PayWayAPI();

            Dictionary<String, String> initParams = new Dictionary<string,string>();
            initParams.Add("certificateFile", certificateFile);
            initParams.Add("logDirectory", logFile);

            //these two are not needed in production environment....need to remove these two setting to AppSetting later on
            //initParams.Add("proxyHost", "172.21.1.1");
            //initParams.Add("proxyPort", "8080");

            String initString = paymentGateway.FormatRequestParameters(initParams);

            paymentGateway.Initialise(initString);

            return paymentGateway;
        }

        /// <summary>
        /// This method adds complete transaction setting and informaiton including transaction request type to the input param
        /// </summary>
        /// <param name="payway"></param>
        /// <returns></returns>
        private void addOrderInfo(Dictionary<String, String> paramList, Payment payment, Order order)
        {
            paramList.Add("order.type", "capture");
            paramList.Add("order.ECI", "SSL");
            String orderAmount = Math.Ceiling(payment.Amount.GetValueOrDefault() * 100).ToString();
            paramList.Add("customer.orderNumber", payment.PaymentID);
            paramList.Add("order.amount", orderAmount);
        }

        private void addAuthInfo(Dictionary<String,String> paramList, Boolean simulation)
        {
            paramList.Add("customer.username", "Q10709");
            paramList.Add("customer.password", "A8dkefgg5");
            if (simulation)
                paramList.Add("customer.merchant", "TEST");
            else
                paramList.Add("customer.merchant", "23425697");
        }

        private void addCardInfo(Dictionary<String, String> paramList, Payment payment, Boolean simulation)
        {
            if (simulation)
            {   //testing account
                paramList.Add("card.PAN", "4564710000000004");
                paramList.Add("card.CVN", "847");
                paramList.Add("card.expiryYear", "19");
                paramList.Add("card.expiryMonth", "02");
            }
            else
            {
                if (!String.IsNullOrEmpty(payment.CardHolderName))
                    paramList.Add("card.cardHolderName", payment.CardHolderName);
                paramList.Add("card.PAN", payment.cardNo);
                paramList.Add("card.CVN", payment.SecurityCode);
                paramList.Add("card.expiryYear", payment.cardExpiredYear);
                paramList.Add("card.expiryMonth", payment.cardExpiredMonth);
            }
            paramList.Add("card.currency", "AUD");
        }


        private void fillupPayment(IDictionary<String, String> responseSet, Payment paymentInfo)
        {
            String resultCode = null;

            paymentInfo.responseValues = responseSet;

            //check transaction status
            resultCode = (responseSet.ContainsKey("response.summaryCode")) ? responseSet["response.summaryCode"] : "Unknown status";
            switch (resultCode)
            {
                case "0":
                    paymentInfo.statusX = Payment.PaymentStatus.Approved;
                    paymentInfo.errorCode = "PaymentStatus." + paymentInfo.statusX;
                    break;
                case "1":
                    paymentInfo.statusX = Payment.PaymentStatus.Declined;
                    paymentInfo.errorCode = "PaymentStatus." + paymentInfo.statusX;
                    break;
                case "2":
                    paymentInfo.statusX = Payment.PaymentStatus.GeneralError;
                    paymentInfo.errorCode = "PaymentStatus." + Payment.PaymentStatus.Declined;
                    break;
                case "3":
                    paymentInfo.statusX = Payment.PaymentStatus.NotSupported;
                    paymentInfo.errorCode = "PaymentStatus." + Payment.PaymentStatus.Declined;
                    break;
                default:
                    paymentInfo.statusX = Payment.PaymentStatus.NotSupported;
                    paymentInfo.errorCode = "PaymentStatus." + Payment.PaymentStatus.Declined;
                    break;
            }

            paymentInfo.TransactionDesc = (responseSet.ContainsKey("response.text")) ? responseSet["response.text"] : "";
            paymentInfo.StatusCode = (responseSet.ContainsKey("response.responseCode")) ? responseSet["response.responseCode"] : "";
            paymentInfo.CCPNREF = (responseSet.ContainsKey("response.receiptNo")) ? responseSet["response.receiptNo"] : "";
            paymentInfo.CCAuthCode = (responseSet.ContainsKey("response.authId")) ? responseSet["response.authId"] : "";
            paymentInfo.CCAVSAddr = "";
            paymentInfo.CCAVSZIP = "";
            paymentInfo.CCIAVS = "";
            paymentInfo.CCPOSTFPSMSG = (responseSet.ContainsKey("response.carditGroup")) ? responseSet["response.carditGroup"] : "";
            paymentInfo.CCPREFPSMSG = (responseSet.ContainsKey("response.cardSchemeName")) ? responseSet["response.cardSchemeName"] : "";
            paymentInfo.CCRESPMSG = paymentInfo.TransactionDesc;
            paymentInfo.responseValues = responseSet;

            //record last 4 digits of credit card regardless
            String cardNo = paymentInfo.cardNo;
            try
            {
                paymentInfo.LastFourDigit = Convert.ToInt16(cardNo.Substring(cardNo.Length - 4));
            }
            catch (Exception)
            {
                ;   //this shall never arrive
            }
        }

        protected override int timeout
        {
            get { return 30; }
        }

        private String certificateFile
        {
            get
            {
                //the following file path is only for temporary purpose.  It requires further refinement
                String fullPath = Path.GetFullPath(configPath + "/AAU/ccapi.q0");

                return fullPath;
            }
        }

        private String logFile
        {
            get { return Path.GetFullPath(logPath + "/AAU/"); }
        }

    }
}
