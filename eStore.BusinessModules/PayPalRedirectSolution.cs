using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Collections.Specialized;
using eStore.Utilities;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Collections;
using System.Net;
using System.Web;
using System.IO;
using System.Configuration;

namespace eStore.BusinessModules
{
    /// <summary>
    /// Prepare Certificate 
    ///http://kb.siteground.com/article/How_to_generate_the_necessary_certificates_for_PayPal_IPN_.html
    ///openssl genrsa -out my-prvkey.pem 1024
    /// openssl req -new -key my-prvkey.pem -x509 -days 365 -out my-pubcert.pem
    /// openssl pkcs12 -export -in my-pubcert.pem -inkey my-prvkey.pem -out paypal_cert.p12 
    /// 
    /// 1. Upload my-pubcert.pem cert in Encrypted Payment Settings and change CERT_ID
    /// 2. download PayPal Public Certificate
    /// 3. change config setting
    /// 3. Activating PDT
    ///To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal.Follow these steps to configure your account for PDT:
    ///Log in to your PayPal account.
    ///Click the Profile subtab.
    ///Click Website Payment Preferences in the Seller Preferences column.
    ///Under Auto Return for Website Payments, click the On radio button.
    ///For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment.
    ///Under Payment Data Transfer, click the On radio button.
    ///Click Save.
    ///Click Website Payment Preferences in the Seller Preferences column.
    ///Scroll down to the Payment Data Transfer section of the page to view your PDT identity token.
    /// </summary>
    class PayPalRedirectSolution : PaymentSolution
    {
        private string StoreID;
        private PPConfiguration _config;

        private Encoding _encoding = Encoding.Default;

        private X509Certificate2 _signerCert;

        private X509Certificate2 _recipientCert;
        protected bool _testing = false;
        protected virtual PPConfiguration config
        {
            get
            {
                if (_config == null)
                {
                    _config = new PPConfiguration();
                    if (StoreID == "AEU")
                    {
                        if (_testing)
                        {
                            _config.STANDARD_IDENTITY_TOKEN = "K4K3QLHTlVwoDr8hd-IhEbM4bxkHB9gZd_RL4E1Jr4g0nWVO39bSD50WLCa";
                            _config.STANDARD_EMAIL_ADDRESS = "mike.l_1331673611_biz@advantech.com.cn";
                            _config.PAYPAL_WEBSCR_URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                            _config.CERT_ID = "AGYDW6XP7EBZG";
                            _config.signerPfxPath = configPath + @"\AEU\AEUcert\mycert.p12";
                            _config.signerPfxPassword = "88888888";
                            _config.paypalCertPath = configPath + @"\AEU\AEUcert\paypal_sandbox_cert_pem.txt";
                        }
                        else
                        {
                            _config.STANDARD_IDENTITY_TOKEN = "3RnGKEn57NDNbvbi2uozR7hCd2QqLYBk1rWMENjUQML2J5vZI54-9K1Snvi";
                            _config.STANDARD_EMAIL_ADDRESS = "mike.liu@advantech.com";
                            _config.PAYPAL_WEBSCR_URL = "https://www.paypal.com/cgi-bin/webscr";
                            _config.CERT_ID = "WCHM6VPBWMHV2";
                            _config.signerPfxPath = configPath + @"\AEU\AEUcert\paypal_cert.p12";
                            _config.signerPfxPassword = "88888888";
                            _config.paypalCertPath = configPath + @"\AEU\AEUcert\paypal_cert_pem.txt";
                        }
                    
                    }
                    else if (StoreID == "SAP" || StoreID == "ATH")
                    {
                        if (_testing)
                        {
                            _config.STANDARD_IDENTITY_TOKEN = "nGd65cSC_GsqwV7Ed8zplDxDLBlLlovdTjKYZ7deTT98sXDM4H_8iIwuq2C";
                            _config.STANDARD_EMAIL_ADDRESS = "mike.l_1243905565_biz@advantech.com.cn";
                            _config.PAYPAL_WEBSCR_URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                            _config.CERT_ID = "R4VLU9Y4WBNVS";
                            _config.signerPfxPath = configPath + @"\SAP\SAPcert\mycert.p12";
                            _config.signerPfxPassword = "88888888";
                            _config.paypalCertPath = configPath + @"\SAP\SAPcert\paypal_sandbox_cert_pem.txt";

                        }
                        else
                        {
                            _config.STANDARD_IDENTITY_TOKEN = "J-4l9luCu73cePXOHd8A1hfMzDc8o0HwZur5T3v7d_aU9NDqv0JT7vCZRMq";
                            _config.STANDARD_EMAIL_ADDRESS = "irene.foo@advantech.com";
                            _config.PAYPAL_WEBSCR_URL = "https://www.paypal.com/cgi-bin/webscr";
                            _config.CERT_ID = "3Y8ZVSNTYT5QU";
                            _config.signerPfxPath = configPath + @"\SAP\SAPcert20170417\mycert.p12";
                            _config.signerPfxPassword = "88888888";
                            _config.paypalCertPath = configPath + @"\SAP\SAPcert20170417\paypal_cert_pem.txt";
                        }
                    }
                }
                return _config;

            }
        }

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
            StoreID = order.StoreID;
            Dictionary<String, String> formItems = new Dictionary<string, string>();

            _testing = simulation;
            formItems.Add("actionURL", config.PAYPAL_WEBSCR_URL);
            string EncryptedOder = this.SignAndEncrypt(order);
            formItems.Add("cmd", "_s-xclick");
            formItems.Add("encrypted", EncryptedOder);
            return formItems;
        }
        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            if (response != null && response["item_number"] != null)
                return (string)response["item_number"];
            else
                return string.Empty;
        }
        public override Payment processIndirectPaymentResponse(NameValueCollection responseValues, Order order, Boolean simulation = false)
        {
            StoreID = order.StoreID;
            //retrieve pending payment from order
            Payment payment = order.getLastOpenPayment();
            if (payment == null)
            {
                payment = new Payment();

                try
                {
                    payment.Amount = order.totalAmountX; ;
                    payment.OrderNo = order.OrderNo;
                }
                catch (Exception)
                {

                }
                //payment = new Payment(strOrderNO, getValue(response, "CN"), cardType, getValue(response, "ED"), "", paymentAmount);
            }
            try
            {
                _testing = payment.isSimulation;
                Dictionary<String, String> response = convertToDictionary(responseValues);

                //fill up payment values
                String strtx = getValue(response, "tx");

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.config.PAYPAL_WEBSCR_URL);

                //Set values for the request back
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                string strRequest = null;
                strRequest = "cmd=_notify-synch&tx=" + strtx + "&at=" + this.config.STANDARD_IDENTITY_TOKEN;
                req.ContentLength = strRequest.Length;
                //Send the request to PayPal and get the response
                StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();
                StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                string sQuerystring = streamIn.ReadToEnd();
                streamIn.Close();


                if (sQuerystring.Length < 9)
                    payment.statusX = Payment.PaymentStatus.Declined;
                else if (sQuerystring.Substring(0, 7).Equals("SUCCESS"))
                {
                    string[] @params = HttpUtility.UrlDecode(sQuerystring.Remove(0, 8)).Split('\n');
                    string pName = null;
                    string pValue = null;
                    string[] pArray = null;
                    Dictionary<string, string> htParams = new Dictionary<string, string>();
                    foreach (string p in @params)
                    {
                        pName = "";
                        pValue = "";
                        pArray = p.Split('=');

                        if (pArray.Length == 2)
                        {
                            pName = pArray[0];
                            pValue = pArray[1];
                            htParams.Add(pName, pValue);
                        }
                    }
                    if (htParams["receiver_email"] != this.config.STANDARD_EMAIL_ADDRESS)
                        payment.statusX = Payment.PaymentStatus.FraudAlert;
                    else if(htParams["payment_status"] != "Completed" && htParams["payment_status"] != "Pending")
                        payment.statusX = Payment.PaymentStatus.AuthenticationFailed;
                    else
                        payment.statusX = Payment.PaymentStatus.Approved;

                    payment.CCPNREF = htParams["txn_id"];
                    payment.CCUser1 = htParams["payer_email"];
                    payment.CCResultCode = htParams["payment_status"];
                    payment.CardType = htParams["payment_type"];

                    payment.responseValues = htParams;

                }
                else
                {
                    payment.statusX = Payment.PaymentStatus.Declined;
                }




            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at CTCBRedirect payment", "", "", "", ex);
                payment.statusX = Payment.PaymentStatus.Declined;
            }

            return payment;
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


        private string SignAndEncrypt(Order order)
        {

            _signerCert = new X509Certificate2(this.config.signerPfxPath, this.config.signerPfxPassword);

            string result = string.Empty;

            byte[] messageBytes = this._encoding.GetBytes(getClearText(order));

            byte[] signedBytes = sign(messageBytes);

            byte[] encryptedBytes = Envelope(signedBytes);

            result = Base64Encode(encryptedBytes);

            return result;

        }

        private byte[] sign(byte[] messageBytes)
        {

            ContentInfo content = new ContentInfo(messageBytes);

            SignedCms signed = new SignedCms(content);

            CmsSigner signer = new CmsSigner(_signerCert);

            signed.ComputeSignature(signer);

            byte[] signedBytes = signed.Encode();

            return signedBytes;

        }

        private byte[] Envelope(byte[] contentBytes)
        {

            ContentInfo content = new ContentInfo(contentBytes);

            EnvelopedCms envMsg = new EnvelopedCms(content);

            _recipientCert = new X509Certificate2(this.config.paypalCertPath);

            CmsRecipient recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, this._recipientCert);

            envMsg.Encrypt(recipient);

            byte[] encryptedbytes = envMsg.Encode();

            return encryptedbytes;

        }

        private string Base64Encode(byte[] encoded)
        {

            const string PKCS7_HEADER = "-----BEGIN PKCS7-----";

            const string PKCS7_FOOTER = "-----END PKCS7-----";

            string BASE64 = Convert.ToBase64String(encoded);

            StringBuilder Formatted = new StringBuilder();

            Formatted.Append(PKCS7_HEADER);

            Formatted.Append(BASE64);

            Formatted.Append(PKCS7_FOOTER);

            return Formatted.ToString();

        }



        private string getClearText(Order order)
        {
            StringBuilder sbCleartext = new StringBuilder();
            if (order == null)
            {
                throw new Exception("please set order information");
            }
            else
            {
                sbCleartext.Append(string.Format("{0}={1}\n", "cmd", "_xclick"));
                sbCleartext.Append(string.Format("{0}={1}\n", "business", config.STANDARD_EMAIL_ADDRESS));

                sbCleartext.Append(string.Format("{0}={1}\n", "item_name", "eStore Order"));
                sbCleartext.Append(string.Format("{0}={1}\n", "item_number", order.OrderNo));
                sbCleartext.Append(string.Format("{0}={1}\n", "custom", order.OrderNo));
                sbCleartext.Append(string.Format("{0}={1}\n", "amount", (order.totalAmountX - order.Freight - order.Tax).Value.ToString("f2")));
                sbCleartext.Append(string.Format("{0}={1}\n", "shipping", order.Freight.Value.ToString("f2")));
                sbCleartext.Append(string.Format("{0}={1}\n", "tax", order.Tax.Value.ToString("f2")));
                sbCleartext.Append(string.Format("{0}={1}\n", "quantity", 1));
                sbCleartext.Append(string.Format("{0}={1}\n", "no_note", 1));
                sbCleartext.Append(string.Format("{0}={1}\n", "currency_code", string.IsNullOrEmpty(order.cartX.Currency)?order.storeX.defaultCurrency.CurrencyID:order.cartX.Currency));
                sbCleartext.Append(string.Format("{0}={1}\n", "address_override", 0));
                sbCleartext.Append(string.Format("{0}={1}\n", "first_name", order.userX.FirstName));
                sbCleartext.Append(string.Format("{0}={1}\n", "last_name", order.userX.LastName));
                sbCleartext.Append(string.Format("{0}={1}\n", "address1", order.cartX.BillToContact.Address1));
                sbCleartext.Append(string.Format("{0}={1}\n", "city", order.cartX.BillToContact.City));
                sbCleartext.Append(string.Format("{0}={1}\n", "state", order.cartX.BillToContact.State));
                sbCleartext.Append(string.Format("{0}={1}\n", "zip", order.cartX.BillToContact.ZipCode));

                sbCleartext.Append(string.Format("{0}={1}\n", "return", esUtilities.CommonHelper.GetStoreLocation() + "completeIndirectPayment.aspx"));
                sbCleartext.Append(string.Format("{0}={1}\n", "cancel_return", esUtilities.CommonHelper.GetStoreLocation() + "Cart/checkout.aspx"));

                sbCleartext.Append(string.Format("{0}={1}", "cert_id", this.config.CERT_ID));
                return sbCleartext.ToString();
            }
        }

        private bool verifyingIPNRusult()
        {
            Hashtable htParams = new Hashtable();

            //Post back to either sandbox or live
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.config.PAYPAL_WEBSCR_URL);
            string strRequest = null;
            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] Param = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            strRequest = Encoding.ASCII.GetString(Param);


            string[] @params = HttpUtility.UrlDecode(strRequest).Split('&');
            string pName = null;
            string pValue = null;
            string[] pArray = null;

            foreach (string p in @params)
            {
                pName = "";
                pValue = "";
                pArray = p.Split('=');

                if (pArray.Length == 2)
                {
                    pName = pArray[0];
                    pValue = pArray[1];
                    htParams.Add(pName, pValue);
                }
            }

            strRequest = strRequest + "&cmd=_notify-validate";

            req.ContentLength = strRequest.Length;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();
            if (strResponse == "VERIFIED")
            {
                //check the payment_status is Completed
                //check that txn_id has not been previously processed
                //check that receiver_email is your Primary PayPal email
                //check that payment_amount/payment_currency are correct
                //process payment
                if (!(htParams["receiver_email"] != null && htParams["receiver_email"].ToString() == this.config.STANDARD_EMAIL_ADDRESS))
                {
                    htParams["Error"] += "<li>receiver_email is not match!</li>";
                }

                if (htParams["custom"] == null || string.IsNullOrEmpty(htParams["custom"].ToString()))
                {
                    htParams["Error"] += "<li>CartID is Null!</li>";
                }

                if (htParams["payment_status"] != null && htParams["payment_status"].ToString() != "Completed" & htParams["payment_status"].ToString() != "Pending")
                {
                    htParams["Error"] += "<li>payment_status is not correct</li>";
                }
            }
            else if (strResponse == "INVALID")
            {
                htParams["Error"] += "<li>INVALID</li>";
            }
            else
            {
                htParams["Error"] += "<li>Unknow response</li>";
            }
            if (string.IsNullOrEmpty(htParams["Error"].ToString()))
            {
                //PaypalResult = htParams;
                return true;
            }
            else
            {
                //this.CustomMessage = "<hr>" + htParams["Error"];
                return false;
            }
        }

        private bool verifyingPTDRusult()
        {

            Hashtable htParams = new Hashtable();
            htParams.Add("Error", "");
            string txToken = null;

            txToken = HttpContext.Current.Request.QueryString["tx"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.config.PAYPAL_WEBSCR_URL);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            string strRequest = null;
            strRequest = "cmd=_notify-synch&tx=" + txToken + "&at=" + this.config.STANDARD_IDENTITY_TOKEN;
            req.ContentLength = strRequest.Length;
            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string sQuerystring = streamIn.ReadToEnd();
            streamIn.Close();


            if (sQuerystring.Length < 9)
                return false;
            if (sQuerystring.Substring(1, 7).Equals("SUCCESS"))
            {
                string[] @params = HttpUtility.UrlDecode(sQuerystring.Remove(0, 9)).Split('&');
                string pName = null;
                string pValue = null;
                string[] pArray = null;

                foreach (string p in @params)
                {
                    pName = "";
                    pValue = "";
                    pArray = p.Split('=');

                    if (pArray.Length == 2)
                    {
                        pName = pArray[0];
                        pValue = pArray[1];
                        htParams.Add(pName, pValue);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class PPConfiguration
    {
        private string _STANDARD_IDENTITY_TOKEN;
        public string STANDARD_IDENTITY_TOKEN
        {
            get { return _STANDARD_IDENTITY_TOKEN; }
            set { _STANDARD_IDENTITY_TOKEN = value; }
        }

        private string _STANDARD_EMAIL_ADDRESS;
        public string STANDARD_EMAIL_ADDRESS
        {
            get { return _STANDARD_EMAIL_ADDRESS; }
            set { _STANDARD_EMAIL_ADDRESS = value; }
        }

        private string _PAYPAL_WEBSCR_URL;
        public string PAYPAL_WEBSCR_URL
        {
            get { return _PAYPAL_WEBSCR_URL; }
            set { _PAYPAL_WEBSCR_URL = value; }
        }

        private string _CERT_ID;
        public string CERT_ID
        {
            get { return _CERT_ID; }
            set { _CERT_ID = value; }
        }

        private string _signerPfxPath;
        public string signerPfxPath
        {
            get { return _signerPfxPath; }
            set { _signerPfxPath = value; }
        }

        private string _signerPfxPassword;
        public string signerPfxPassword
        {
            get { return _signerPfxPassword; }
            set { _signerPfxPassword = value; }
        }

        private string _paypalCertPath;
        public string paypalCertPath
        {
            get { return _paypalCertPath; }
            set { _paypalCertPath = value; }
        }

    }

}