using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using eStore.POCOS;
using eStore.Utilities;
using SBIVeriTrans;

namespace eStore.BusinessModules
{
    class VeriTransSolution : PaymentSolution
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
                VTPayment paymentGateway = getPaymentGateway();

                //prepare transaction request
                paymentGateway.SetCommand("authonly");
                paymentGateway.SetMsg("order-id", order.OrderNo);
                if (simulation)
                    paymentGateway.SetMsg("amount", "1");
                else
                    paymentGateway.SetMsg("amount", Math.Ceiling(paymentInfo.Amount.GetValueOrDefault()).ToString());
                paymentGateway.SetMsg("card-number", paymentInfo.cardNo);
                paymentGateway.SetMsg("card-exp", paymentInfo.cardExpiredMonth + "/" + paymentInfo.cardExpiredYear);

                //send transaction request
                paymentGateway.DoTransaction();

                //receive and process response
                Dictionary<String, String> resultSet = new Dictionary<string,string>();
                if (paymentGateway.GetKeyCount() > 0)   //receive response
                {
                    paymentGateway.GetKeyStartPos();
                    String key = null;
                    Boolean toContinue = true;
                    do 
                    {
                        key = paymentGateway.GetNextKey();
                        if (string.IsNullOrEmpty(key))  //no more key available
                            toContinue = false;
                        else
                            resultSet.Add(key, paymentGateway.GetValueByKey(key));
                    } while ( toContinue );

                }
                else   //probably time out or other issue
                {
                    paymentInfo.statusX = Payment.PaymentStatus.Timeout;
                    paymentInfo.errorCode = "PaymentStatus." + paymentInfo.statusX;
                    paymentInfo.TransactionDesc = "Transaction time out...please resubmit";
                }

                //Fill up payment information
                fillupPayment(resultSet, paymentInfo);

                paymentGateway.Dispose();
                paymentGateway = null;
            }
            catch (Exception ex)
            {
                paymentInfo.statusX = Payment.PaymentStatus.Declined;
                paymentInfo.errorCode = "PaymentStatus." + paymentInfo.statusX;
                paymentInfo.TransactionDesc = "Doesn't have sufficient information";

                eStoreLoger.Fatal("Exception at VeriTransPayment solution", "", "", "", ex);
            }

            return paymentInfo;
        }


        /// <summary>
        /// This method will initial PaymentGateway connection and return the initialized connection back
        /// </summary>
        /// <returns></returns>
        private VTPayment getPaymentGateway()
        {
            VTPayment paymentGateway = new VTPayment();
            paymentGateway.SetServerFromConf(this.configFile);

            //overwrite setting
            paymentGateway.SetAcTable(actionCodeTable);
            paymentGateway.SetLogFile(logFile);
            paymentGateway.SetMsgTable(msgTable);
            paymentGateway.SetPendingDir(pendingLogPath);

            paymentGateway.SetTimeOut(timeout);

            return paymentGateway;
        }

        private void fillupPayment(Dictionary<String, String> responseSet, Payment paymentInfo)
        {
            String resultCode = null;

            paymentInfo.responseValues = responseSet;

            //check transaction status
            resultCode = (responseSet.ContainsKey("MStatus")) ? responseSet["MStatus"] : "Unknown status";
            if (resultCode.Equals("success"))   //good transaction
            {
                paymentInfo.statusX = Payment.PaymentStatus.Approved;
                paymentInfo.TransactionDesc = (responseSet.ContainsKey("aux-msg")) ? responseSet["aux-msg"] : "";
            }
            else //failure or not determinable status
            {
                paymentInfo.statusX = Payment.PaymentStatus.Declined;
                paymentInfo.errorCode = "PaymentStatus." + paymentInfo.statusX;

                StringBuilder desc = new StringBuilder();
                if (responseSet.ContainsKey("MErrLoc"))
                    desc.Append(responseSet["MErrLoc"]);
                if (responseSet.ContainsKey("MErrMsg"))
                    desc.Append(" - ").Append(responseSet["MErrMsg"]);
                paymentInfo.TransactionDesc = desc.ToString();
            }

            paymentInfo.CCAuthCode = (responseSet.ContainsKey("auth-code")) ? responseSet["auth-code"] : "";
            paymentInfo.CCAVSAddr = (responseSet.ContainsKey("avs-code")) ? responseSet["avs-code"] : "";
            paymentInfo.CCAVSZIP = paymentInfo.CCAVSAddr;
            paymentInfo.CCIAVS = paymentInfo.CCAVSAddr;
            paymentInfo.CCPNREF = (responseSet.ContainsKey("ref-code")) ? responseSet["ref-code"] : "";
            paymentInfo.CCPOSTFPSMSG = (responseSet.ContainsKey("merch-txn")) ? responseSet["merch-txn"] : "";
            paymentInfo.CCPREFPSMSG = (responseSet.ContainsKey("cust-txn")) ? responseSet["cust-txn"] : "";
            paymentInfo.CCRESPMSG = paymentInfo.TransactionDesc;
            paymentInfo.responseValues = responseSet;

            //record last 4 digits of credit card regardless
            //if (payment.statusX == Payment.PaymentStatus.Approved)
            {
                String cardNo = paymentInfo.cardNo;
                try
                {
                    paymentInfo.LastFourDigit = Convert.ToInt16(cardNo.Substring(cardNo.Length - 4));
                }
                catch (Exception)
                {
                    //this shall never arrive
                }
            }
        }

        protected override int timeout
        {
            get
            {
                return 60;
            }
        }

        private String configFile
        {
            get
            {
                //the following file path is only for temporary purpose.  It requires further refinement
                String fullPath = Path.GetFullPath(configPath + "/AJP/flexlib.conf");

                return fullPath;
            }
        }

        private String msgTable
        {
            get { return Path.GetFullPath(configPath + "/AJP/msg_table"); }
        }

        private String actionCodeTable
        {
            get { return Path.GetFullPath(configPath + "/AJP/actioncode.def"); }
        }

        private String logFile
        {
            get { return Path.GetFullPath(logPath + "/AJP/CCTransaction.log"); }
        }

        private String pendingLogPath
        {
            get { return Path.GetFullPath(logPath + "/AJP/pending"); }
        }
    }
}
