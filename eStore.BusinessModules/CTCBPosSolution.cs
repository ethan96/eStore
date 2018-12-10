using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using esUtilities;
using CTCB.POS;

using CTCB.PayPortal;
using System.Configuration;

namespace eStore.BusinessModules
{
    class CTCBPosSolution:PaymentSolution, IMailer
    {
        private string _testing = ConfigurationManager.AppSettings.Get("TestingMode");
        
        /******************** Trial Credit Cart ********************/
        private static string _CTCBTestingServerName = "testepos.chinatrust.com.tw";
        private static int _CTCBTrialMerID = 342;
        private static string _trialPAN = "5432112345123456"; // Testing card number
        private static string _trialCVV2 = "123"; 
        private static string _trialExpiryDate = "201212";      //yyyyMM
        /*********************************************************/

        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        public string TestingOrderDeptEmail
        {
            get { return testingOrderDeptEmail; }
            set { testingOrderDeptEmail = value; }
        }
        
        private static string _CTCBProductionServerName = "epos.chinatrust.com.tw";
        private static int _CTCBProductionMerID = 372;
        private static string _currencyCode = "901";    //NTD

        //private Store _store = null;

        public override Boolean supportDirectAccess()
        { return true; }

        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
             
            try
            {
                if (order == null)
                    throw new Exception("Order is null.");

                if (paymentInfo == null)
                    throw new Exception("PaymentInfo is null.");

                //If it's a simulation, then it will initiate.
                //Simulate card No. is no longer valid, and CTCB didn't have Sandbox environmnet.
                //if (simulation)                 
                //    initialPaymentForSimulation(order, paymentInfo, _trialExpiryDate.Substring(2, 2), _trialExpiryDate.Substring(4, 2), _trialPAN, _trialCVV2);

                    Auth auth = new Auth();
                     
                    Auth.ServerName = (simulation) ? _CTCBTestingServerName : _CTCBProductionServerName;
                    auth.MerID = (simulation) ? _CTCBTrialMerID : _CTCBProductionMerID;  //300 is for production purpose, 342 is for testing purpose

                    auth.OrderNo = (simulation) ? "TEST_" + order.OrderNo : order.OrderNo;
                    auth.PAN = (simulation) ? _trialPAN : paymentInfo.cardNo;
                    auth.CVV2 = (simulation) ? _trialCVV2 : paymentInfo.SecurityCode;
                    auth.ExpiryDate = (simulation) ? _trialExpiryDate : "20"+paymentInfo.cardExpiredYear + paymentInfo.cardExpiredMonth;   //yyyyMM
                    auth.PurchAmt = System.Convert.ToInt32(order.totalAmountX);

                    // following are optional fields，not necessary 
                    auth.Currency = _currencyCode; // transaction currency
                    auth.Exponent = 0; // decimal
                    Auth.TimeOut = 3000; // Remote host response time
                    auth.OrderDesc = order.CustomerComment;

                    // 以下區塊3D 特店為必填, 非3D 特店則不必填


                    //auth.ECI = 5; // ECI 和CAVV 請填入MPI 的回傳值
                    //auth.CAVV = "12ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                    //CTCB didn't have SandBox account, so just use users card to purchase 1 TWD.
                    if (simulation) auth.PurchAmt = 1;
                    
                    //Start to transaction
                     int ret = auth.Action();    //ret = 0, it means transaction is success
                      //Process response and fill up payment status
                      processDirectPaymentResponse(ret, auth, paymentInfo);

                      //Clear auth data
                      auth.ClearData();
               



               
            }
            catch (Exception ex)
            {
                paymentInfo.errorCode = "CTCB_PAYMENT_13_X_X";
                eStoreLoger.Fatal("Exception at CTCB payment. OrderNO is " + order.OrderNo, "", "", order.StoreID, ex);
            }

            return paymentInfo;
        }

        private Payment processDirectPaymentResponse(int ret, Auth auth, Payment payment)
        {
            //tokenize response and buildup a Dictionary for query
            
            Dictionary<String, String> resultItems = new Dictionary<string, string>();
            resultItems.Add("APIReturnedValue", ret.ToString());
            resultItems.Add("authStatus", auth.Status.ToString());
            resultItems.Add("authErrorCode", auth.ErrCode);
            resultItems.Add("authErrorDesc", auth.ErrorDesc);
            resultItems.Add("backendTransactionID", auth.XID);
            resultItems.Add("authTransactionID", auth.AuthRRPID);
            resultItems.Add("transactionCurrency", auth.Currency);
            resultItems.Add("currencyExponent", auth.Exponent.ToString());
            resultItems.Add("authAmount", auth.AuthAmt.ToString());
            resultItems.Add("authCode", auth.AuthCode);
            resultItems.Add("RetrRef", auth.RetrRef);
            resultItems.Add("termSeq", auth.TermSeq.ToString());
            resultItems.Add("authVersion", auth.Version);
            resultItems.Add("authRevision", auth.Revision);
            if (ret == 0)
                resultItems.Add("authSuccess", "True");
            else
            {
                if(auth.Status != 0 || !auth.ErrCode.StartsWith("00"))
                resultItems.Add("authSuccess", "False");
            }

            //process payment status
            setPaymentStatus(resultItems, payment);

            //Process the rest of payment transaction result
            //payment.CCAuthCode = resultItems.ContainsKey("authCode") ? resultItems["authCode"] : "";
            payment.CCResultCode = resultItems.ContainsKey("authSuccess") ? resultItems["authSuccess"] : "";

            payment.responseValues = resultItems;
            if (payment.statusX == Payment.PaymentStatus.Approved && resultItems["authSuccess"] == "True")
            {
                string cardNo = payment.cardNo;
                try
                {
                    payment.LastFourDigit = Convert.ToInt16(cardNo.Substring(cardNo.Length - 4));
                }
                catch (Exception)
                {
                    ;   //this shall never arrive
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
            Int16 ierrorCode = -1000;
            string apiReturnedCode = null;
            string statusCode = null;
            string errorCode = null;
            string eStoreErrorCodePrefix = "CTCB_PAYMENT_";
            results.TryGetValue("APIReturnedValue", out apiReturnedCode);
            results.TryGetValue("authStatus", out statusCode);
            results.TryGetValue("authErrorCode", out errorCode);
            int apiReturnedValue = System.Convert.ToInt32(apiReturnedCode);
            int status = System.Convert.ToInt32(statusCode);
            bool isErrorCodeConvert = Int16.TryParse(errorCode, out ierrorCode);

            //eStoreErrorCode will store in database, the format is CTCB_PAYMENT_APIReturnedValue_AuthStatus_AuthErrorCode
            // Ex: CTCB_PAYMENT_0_4_8
            string eStoreErrorCode = "";
            //errorCode = "43";
            //status = 8;
            if (apiReturnedValue == 0 && statusCode == "0" && errorCode.StartsWith("00"))
            {
                //Connection, Authentication, Transaction are all success, it's a legal transaction.
                payment.statusX = Payment.PaymentStatus.Approved;
            }
            else if (apiReturnedValue >= 11 && apiReturnedValue <= 14)
            {
                //Socket connection error
                payment.statusX = Payment.PaymentStatus.CommunicationError;
                eStoreErrorCode = apiReturnedValue.ToString() + "_X_X";
            }
            else if (apiReturnedValue == 15)
            {
                //Socket receive timeout
                payment.statusX = Payment.PaymentStatus.Timeout;
                eStoreErrorCode = apiReturnedValue.ToString() + "_X_X";
            }
            else if (apiReturnedValue == 268435471)
            {
                //Exceed per transaction limit
                payment.statusX = Payment.PaymentStatus.ExceedTransactionLimit;
                eStoreErrorCode = apiReturnedValue.ToString() + "_X_X";
            }
            else if (apiReturnedValue >= 268435457 && apiReturnedValue <= 268435479)
            {
                //It occurs usually due to wrong field format 
                payment.statusX = Payment.PaymentStatus.Declined;
                eStoreErrorCode = apiReturnedValue.ToString() + "_X_X";
            }
            else if ((apiReturnedValue == 0 && status == 9 
                && (errorCode == "nz" || errorCode == "ns")) 
                || (apiReturnedValue == 0 && status == 8 && errorCode == "03"))
            {
                //nz: Connection from unauthenticated client
                //ns: CTCB can't find the merchant information in their server
                //03: Merchant ID is unauthenticated.
                payment.statusX = Payment.PaymentStatus.AuthenticationFailed;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 4 
                && isErrorCodeConvert
                && ierrorCode == 7)
            {
                //Wrong type of card number(only 16 digits is valid), card is expired.
                payment.statusX = Payment.PaymentStatus.Declined;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 12
                && isErrorCodeConvert
                && ierrorCode == 25)
            {
                //CTCB don't accept the kind of credit card.
                payment.statusX = Payment.PaymentStatus.Declined;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 2)
            {
                //CTCB'a payment gateway wrror, CTCB has system error.
                payment.statusX = Payment.PaymentStatus.RemoteServerError;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 8 
                && isErrorCodeConvert
                && (ierrorCode == 1 ||
                        ierrorCode == 02 ||
                        ierrorCode == 05 ||
                        ierrorCode == 06 ||
                        ierrorCode == 12))
            {
                //In this case, please contact with the bank who issued the card about the  reason why authenticated failed.
                payment.statusX = Payment.PaymentStatus.Declined;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 8
                && isErrorCodeConvert
                && (ierrorCode >= 13 && ierrorCode <= 33) ||
                        (ierrorCode >= 44 && ierrorCode <= 58) ||
                        (ierrorCode >= 60))
            {
                payment.statusX = Payment.PaymentStatus.Declined;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }
            else if (apiReturnedValue == 0 && status == 8
                && isErrorCodeConvert
                && (ierrorCode == 7 || ierrorCode == 34 || ierrorCode == 39 || ierrorCode == 41 || ierrorCode == 43 || ierrorCode == 59))
            {
                //7: Capture card
                //34: Reserved, suspected fraud
                //39: System can't get the credit information from card holder
                //41: Lost card
                //43: Stolen card
                //59: Syspected card
                payment.statusX = Payment.PaymentStatus.FraudAlert;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
                string subject = "System Warning!!! Triggled Chinatrust payment fraud alert";
                string emailMsg = "Order NO: "+ payment.OrderNo;
                emailMsg += "<br />Customer name: " + payment.Order.cartX.userX.FirstName.ToUpper() + " " + payment.Order.cartX.userX.LastName.ToUpper();
                emailMsg += "<br />Customer company: " + payment.Order.cartX.userX.CompanyName;
                emailMsg += "<br />Customer contact tel: " + payment.Order.cartX.userX.mainContact.TelNo + ", Ext: " + payment.Order.cartX.userX.mainContact.TelExt;
                emailMsg += "<br />Customer mobile phone: " + payment.Order.cartX.userX.mainContact.Mobile;
                emailMsg += "<br />Card holder name: "+ payment.CardHolderName;
                emailMsg += "<br />Card no: " + payment.cardNo.Substring(0,8)+"****"+payment.cardNo.Substring(12,4);
                emailMsg += "<br /> Card type: " + payment.CardType;
                emailMsg += "<br />Payment ID: " + payment.PaymentID;
                emailMsg += "<br />Error message: " + eStoreErrorCodePrefix + eStoreErrorCode + "<br />(Description: APIErrorCode_TransactionStatusCode_ErrorCode, please refer to Chinatrust bank POS.Net API system manual.)";
                sendWarningEMail(subject, emailMsg);
            }
            else
            {
                payment.statusX = Payment.PaymentStatus.GeneralError;
                eStoreErrorCode = apiReturnedValue + "_" + status.ToString() + "_" + errorCode;
            }

            if (payment.statusX != Payment.PaymentStatus.Approved)
            {
                payment.errorCode = eStoreErrorCodePrefix + eStoreErrorCode;
                string msg = payment.errorCode + "[OrderNO: " +payment.OrderNo + "]";
                eStoreLoger.Info(msg, payment.Order.UserID, "", "ATW");
            }
       
            results.Add("eStoreErrorCode", eStoreErrorCodePrefix+ eStoreErrorCode);
        }

        /// <summary>
        /// When user has a serious problem in transaction process, system will send a notice to order department mail.
        /// </summary>
        /// <param name="warningMsg"></param>
        public void sendWarningEMail(string  subject, string warningMsg)
        {
            string mailToaddress = "";
            string subjectPrefix = "";
            string mailSubject = subject+"  in "+"ATW" + "store.";
            string storeITmailGroup = "";
            storeITmailGroup = testingOrderDeptEmail;
            if (_testing == "true")
            {
                subjectPrefix = "[eStore 3.0 TESTING] ";
                mailToaddress = testingOrderDeptEmail;
            }
            else
                mailToaddress = "buy@advantech.com.tw";

            sendMail(mailToaddress, "buy@advantech.com", "eStore System", subjectPrefix + mailSubject, warningMsg, "ATW",storeITmailGroup);
        }

        public EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "")
        {
            EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC);
            mail.Priority = System.Net.Mail.MailPriority.High;
            EMailReponse response = mail.sendMailNow();
            return response;
        }

        /// <summary>
        /// This function is used for testing purpose to initiate payment obj.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo"></param>
        /// <param name="expiredYear"></param>
        /// <param name="expiredMonth"></param>
        /// <param name="cardNo"></param>
        /// <param name="cvv2"></param>
        /// <param name="cardHolder"></param>
        /// <param name="cardType"></param>
        /// <returns></returns>
        private Payment initialPaymentForSimulation(Order order, Payment paymentInfo, string expiredYear, string expiredMonth, string cardNo, string cvv2, string cardHolder = "Tester", string cardType="MasterCard")
        {
            paymentInfo.Order = order;
            paymentInfo.Amount = order.totalAmountX;
            paymentInfo.CardExpiredDate = expiredMonth + expiredYear;
            paymentInfo.cardNo = cardNo;
            paymentInfo.CCAuthCode = cvv2;
            paymentInfo.CardHolderName = cardHolder;
            paymentInfo.CardType = cardType;

            return paymentInfo;
        }

        #region Unit test
        //Unit test
        public static int authTest(string[] agrs)
        {
            //Testing information
            int _merId = 342;
            string _merchantId = "8220276805217";
            string _terminalId = "90000170";
            string _orderNo = "OTW_TEST";
            string _pan = "5433111111111111";
            string _cvv2 = "123";
            string _expiryDate = "2012";
            int _purchAmt =1;
            string _txType = "9";
            string _option = "";
            string _key = "RfVrHiQv";

            Auth auth = new Auth();
            Auth.ServerName = "testepos.chinatrust.com.tw";
           
            auth.MerID = _merId; // 依照中信發出的特店啟用通知email 填入
            auth.OrderNo = _orderNo; // 訂單編號
            auth.PAN = _pan; // 卡號
            auth.CVV2 = _cvv2; // 卡片背面末三碼
            auth.ExpiryDate = _expiryDate; // 有效年月
            auth.PurchAmt = _purchAmt; // 銷貨金額
            //*********** 以下區塊為選填欄位，可不填 *********************
            auth.Currency = "901"; // 交易幣別
            auth.Exponent = 0; // 小數位數
            //Auth.TimeOut = 0; // 等待中信主機回覆的時間
            auth.OrderDesc = "訂單描述test";

            //*************************************************************
            //*********** 以下區塊3D 特店為必填, 非3D 特店則不必填 *********
            //auth.ECI = 5; // ECI 和CAVV 請填入MPI 的回傳值
            //auth.CAVV = "12ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //*************************************************************
            int ret = auth.Action();
            Console.WriteLine("AuthActionRet: " + ret);
            Console.WriteLine("----------- 一般特店授權交易 -----------");
            if (ret == 0)
            {
                Console.WriteLine("Status=" + auth.Status);
                Console.WriteLine("ErrCode=" + auth.ErrCode);
                if (auth.Status == 0 && auth.ErrCode.StartsWith("00"))
                    Console.WriteLine("\t\t---- 授權成功 ----");
                else
                    Console.WriteLine("\t\t---- 授權失敗 ----");
                Console.WriteLine("中信後台的交易識別碼=" + auth.XID);
                Console.WriteLine("授權交易之代碼=" + auth.AuthRRPID);
                Console.WriteLine("交易幣別代碼=" + auth.Currency);
                Console.WriteLine("幣值指數=" + auth.Exponent);
                Console.WriteLine("授權金額=" + auth.AuthAmt);
                Console.WriteLine("交易授權碼=" + auth.AuthCode);
                Console.WriteLine("調閱編號=" + auth.RetrRef);
                Console.WriteLine("調閱序號=" + auth.TermSeq);

                //Console.WriteLine("BatchClose="+ inst.batchclose);
                Console.WriteLine("訊息規格版本=" + auth.Version);
                Console.WriteLine("版本修訂日期=" + auth.Revision);
                Console.WriteLine("錯誤訊息=" + auth.ErrorDesc);
            }
            else
            {
                Console.WriteLine("Auth ret=" + ret);
            }
            Console.WriteLine("----------- 交易結束 -------------------");
            if (ret != 0 || auth.Status != 0 || !auth.ErrCode.StartsWith("00"))
            { // 交易失敗
                Console.Write("Transaction failed!");
                //Console.ReadKey();
                return ret;
            }

            Console.WriteLine();
            //Console.WriteLine("--------Start to validate the returned auth ----------");
            //AuthOutMac validate = new AuthOutMac();
            //validate.Status = auth.Status.ToString();
            //validate.ErrCode = auth.ErrCode;
            //validate.AuthCode = auth.AuthCode;
            //validate.AuthAmt = auth.AuthAmt.ToString();//auth.AuthAmt.ToString();
            //validate.OrderNo = auth.OrderNo;
            //validate.OffsetAmt = "";        //Discount acoumt
            //validate.OriginalAmt = "";
            //validate.UtilizedPoint = "";    //Bonus discount
            //validate.Option = "";
            //validate.Last4digitPAN = _pan.Substring(_pan.Length - 5, 4);       //Last 4 digits of credit card
            //validate.Key = _key;
            

            //if (validate.LastError == 0)
            //    Console.WriteLine("Mac is : {0}", validate.Mac);
            //else
            //    Console.WriteLine("Invalidate parameter. Error code: {0}", validate.LastError);
            //Console.WriteLine();


            Reversal authrev = new Reversal();
            authrev.MerID = auth.MerID;
            authrev.AuthCode = auth.AuthCode;
            authrev.XID = auth.XID;
            authrev.AuthRRPID = auth.AuthRRPID;
            authrev.TermSeq = auth.TermSeq;
            authrev.RetrRef = auth.RetrRef;
            authrev.AuthNewAmt = 0; // 更正的授權金額
            authrev.PurchAmt = auth.PurchAmt; // 原交易銷貨金額
            ret = authrev.Action();
            Console.WriteLine("----------- 一般特店取消交易 -----------");
            if (ret == 0)
            {
                Console.WriteLine("Status=" + authrev.Status);
                Console.WriteLine("ErrCode=" + authrev.ErrCode);
                if (auth.Status == 0 && auth.ErrCode.StartsWith("00"))
                    Console.WriteLine("\t\t---- 取消授權成功 ----");
                else
                    Console.WriteLine("\t\t---- 取消授權失敗 ----");
                Console.WriteLine("交易幣別代碼=" + authrev.Currency);
                Console.WriteLine("幣值指數=" + authrev.Exponent);
                Console.WriteLine("授權取消的金額=" + authrev.AuthNewAmt);
                Console.WriteLine("終端機序號=" + authrev.TermSeq);
                Console.WriteLine("調閱編號=" + authrev.RetrRef);
                Console.WriteLine("訊息規格版本=" + authrev.Version);
                Console.WriteLine("版本修訂日期=" + authrev.Revision);
                Console.WriteLine("錯誤訊息=" + authrev.ErrorDesc);
            }
            else
            {
                Console.WriteLine("Reverse ret=" + ret);
                Console.Write("按任一鍵即可結束!");
                //Console.ReadKey();
                return ret;
            }
            Console.WriteLine("----------- 交易結束 -------------------");
            Console.Write("按任一鍵即可結束!");
            //Console.ReadKey();


            // Encryption test
            Console.WriteLine("\n\n########### AuthInMac test ############");
            //AuthInMac authInMac = new AuthInMac();
            //authInMac.MerchantID = _merchantId;
            //authInMac.TerminalID = _terminalId;
            //authInMac.OrderNo = _orderNo;
            //authInMac.AuthAmt = _purchAmt.ToString();
            //authInMac.TxType = _txType;
            //authInMac.Option = _option;
            //authInMac.Key = _key;

            //if (authInMac.LastError == 0)
            //    Console.WriteLine("Mac is {0}", authInMac.Mac);
            //else
            //    Console.WriteLine("Invalid parameter. Error code: {0}", authInMac.LastError);
            return 0;
        }
        #endregion

    }
}
