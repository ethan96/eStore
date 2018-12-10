using eStore.POCOS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.BusinessModules
{
    class DaouPaySolution : PaymentSolution
    {
        private bool _testing = ConfigurationManager.AppSettings.Get("TestingMode").ToLower() == "true";
        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");

        /// <summary>
        /// test data and url
        /// </summary>
        /// <returns></returns>
        //private string CPID = "CTS14846";
        //private Dictionary<string, string> serverLinks = new Dictionary<string, string>()
        //{
        //    { "DaoupayCard", "https://ssltest.kiwoompay.co.kr/card/DaouCardMng.jsp" },
        //    { "DaoupayBank", "https://ssltest.kiwoompay.co.kr/bank/DaouBankMng.jsp" },
        //    { "DaoupayVirtual", "http://ssltest.kiwoompay.co.kr/vaccount/DaouVaccountMng.jsp" }
        //};

        private const string CPID = "CAD21276";
        private readonly Dictionary<string, string> serverLinks = new Dictionary<string, string>()
        {
            { "DaoupayCard", "https://ssl.kiwoompay.co.kr/card/DaouCardMng.jsp" },
            { "DaoupayBank", "https://ssl.kiwoompay.co.kr/bank/DaouBankMng.jsp" },
            { "DaoupayVirtual", "http://ssl.kiwoompay.co.kr/vaccount/DaouVaccountMng.jsp" }
        };



        public override Boolean supportDirectAccess()
        { return false; }

        public override IDictionary<String, String> getIndirectPaymentRequestForm(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            Dictionary<String, String> formItems = new Dictionary<string, string>();
            formItems.Add("actionURL", serverLinks[order.PaymentType]);

            // 必填
            formItems.Add("CPID", CPID); // store id in daoupay
            formItems.Add("ORDERNO", order.OrderNo); // order number
            formItems.Add("PRODUCTTYPE", "2"); // Product type (1: digital, 2: physical)
            formItems.Add("BILLTYPE", "1"); // Billing type (1: General)
            formItems.Add("TAXFREECD", "00"); //Taxable or tax exempt (00: taxable, 01: tax exempt, 02: combined taxation)
            formItems.Add("AMOUNT", ((int)paymentInfo.Amount).ToString()); // Payment amount
            formItems.Add("PRODUCTNAME", "eStore Order - " + order.OrderNo); // Product name

            //选填
            formItems.Add("CPQUOTA", "0:3"); // Number of months for instalment payments delimiter “ : “
            formItems.Add("EMAIL", order.UserID); // Customer ID
            formItems.Add("USERID", order.UserID); // Name of customer
            //formItems.Add("PRODUCTCODE", order.UserID);// Product code
            //formItems.Add("TELNO2", order.UserID); // Tax exempt amount If TAXFREECD 02
            //formItems.Add("RESERVEDINDEX1", order.UserID); // Reserved item 1 (managed internally by INDEX)
            //formItems.Add("RESERVEDINDEX2", order.UserID); // Reserved item 2 (managed internally by INDEX)
            //formItems.Add("RESERVEDSTRING", order.UserID); // Reserved item 3

            //formItems.Add("CARDLIST", order.UserID); Payment page visible to card issuer delimiter “ : “
            //formItems.Add("HIDECARDLIST", order.UserID); // Payment page visible limited to card issuer delimiter “ : “

            formItems.Add("USERNAME", "");
            formItems.Add("RETURNURL", string.Format("https://{0}/completeDaouPayment.aspx", order.storeX.StoreURL)); // Unlimited   URL to redirect to after successful payment (new window)
            formItems.Add("HOMEURL", string.Format("https://{0}/completeDaouPayment.aspx", order.storeX.StoreURL)); // Unlimited  URL to redirect to after successful payment (redirect from payment page)
            formItems.Add("DIRECTRESULTFLAG", "Y"); //Send the DIRECTRESULTFLAG value to Y, and it will redirect to HOMEURL without displaying the DAOU PAY payment complete window.
            formItems.Add("RESERVEDINDEX1", paymentInfo.PaymentID); // payment id
            formItems.Add("RESERVEDSTRING", order.UserID); // Unlimited Directly redirect to HOMEURL without DAOU PAY payment complete window （Y / N）
            //formItems.Add("POPUPTYPE", ""); 1  Card issuer payment limits pop-up appears for cash businesses or game companies   A: Limits pop-up for game companies B: Limits pop-up for cash businesses
            formItems.Add("CASHRECEIPTFLAG", "0"); // Issue or not issue cash receipt (1: issue, 0: not issue)

            return formItems;
        }

        public override string getIndirectPaymentOrderResponseNO(System.Collections.Specialized.NameValueCollection response)
        {
            if (response != null && response["ORDERNO"] != null)
                return (string)response["ORDERNO"];
            else
                return string.Empty;
        }

        public override Payment processIndirectPaymentResponse(System.Collections.Specialized.NameValueCollection response, Order order, Boolean simulation = false)
        {
            Payment rlt = order.getLastOpenPayment();
            if (rlt == null)
            {
                rlt = new Payment();
                rlt.Amount = order.totalAmountX;
            }
            
            // if get daoupay server get request as success payment
            rlt.CCResultCode = "00"; //00 is ok
            rlt.CCPREFPSMSG = "SUCCESS"; // success
            rlt.CCPNREF = response["ACCOUNTNO"]; // account no
            rlt.CCRESPMSG = response["PAYMETHOD"]; // payment type
            rlt.Comment1 = response["DEPOSITENDDATE"];		// 암호화 결과값

            rlt.responseValues = convertToDictionary(response);
            
            if (rlt.CCResultCode.Equals("00"))
            {
                rlt.statusX = Payment.PaymentStatus.Approved;
            }
            else
                rlt.statusX = Payment.PaymentStatus.Declined;
            
            return rlt;
        }

        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            try
            {
                if (order == null)
                    throw new Exception("Order is null.");

                if (paymentInfo == null)
                    throw new Exception("PaymentInfo is null.");


            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Fatal("Exception at INIPay payment. OrderNO is " + order.OrderNo, "", "", order.StoreID, ex);
            }

            return paymentInfo;
        }

    }
}
