using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Configuration;
using System.IO;
using System.Configuration;
using eStore.Utilities;

namespace esUtilities
{
    /// <summary>
    /// This class composes by all mail elements.
    /// </summary>
    public class EMail
    {
        #region properties
        /// <summary>
        /// SMTP setting is in web.config
        /// SMTP Server     172.20.1.62  (EMKT)
        /// SMTP Server     172.21.1.31
        /// SMTP Server     170.21.0.12
        /// </summary>
        private string _smtpMasterHost = ConfigurationManager.AppSettings.Get("MasterSMTP");
        private string _smtpSlaveHost = ConfigurationManager.AppSettings.Get("SlaveSMTP");
        private const int _retryToSendMailPeriod = 100000;  //ms 
        
        private List<string> _mailToAddress;
        public List<string> MailToAddress
        {
            get { return _mailToAddress; }
            set { _mailToAddress = value; }
        }

        private List<string> _mailCCAddress;
        public List<string> MailCCAddress
        {
            get { return _mailCCAddress; }
            set { _mailCCAddress = value; }
        }

        private List<string> _mailBCCAddress;
        public List<string> MailBCCAddress
        {
            get { return _mailBCCAddress; }
            set { _mailBCCAddress = value;}
        }

        private string _mailFrom;
        public string MailFrom
        {
            get { return _mailFrom; }
            set { _mailFrom = value; }
        }

        private string _mailFromName;
        public string MailFromName
        {
            get { return _mailFromName; }
            set { _mailFromName = value; }
        }

        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string _mailBody;
        public string MailBody
        {
            get { return _mailBody; }
            set { _mailBody = value; }
        }

        private List<string> _attached;
        public List<string> Attached
        {
            get { return _attached; }
            set { _attached = value; }
        }

        private List<string> _embeddedImage;
        public List<string> EmbeddedImage
        {
            get { return _embeddedImage; }
            set { _embeddedImage = value; }
        }

        private List<LinkedResource> _customizedImage;
        public List<LinkedResource> CustomizedImage
        {
            get { return _customizedImage; }
            set { _customizedImage = value; }
        }

        //default using UTF-8 encoding
        private bool _utf8Encoding = true;
        public bool UTF8Encoding
        {
            get { return _utf8Encoding; }
            set { _utf8Encoding = value; }
        }

        //Default email using HTML type
        public enum Encoding { TEXT, HTML };
        private Encoding _type = Encoding.HTML;
        public Encoding Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private MailPriority _priority = MailPriority.Normal;
        public MailPriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
        }

        // Send mail at reserved date/time
        private DateTime _reservedSendDateTime;
        public DateTime ReservedSendDateTime
        {
            get { return _reservedSendDateTime; }
            set { _reservedSendDateTime = value; }
        }

        public enum MailAttrType { Quotation, 
                                                               TransferredQuotation,
                                                               Order, 
                                                               QuantityDiscount, 
                                                               PaymentResult,
                                                               System, 
                                                               Others };
        public MailAttrType MailType
        {
            get;
            set;
        }

        // Save email in mail repository, default is no save.
        private bool _saveEmail;
        public bool SaveEmail
        {
            get { return _saveEmail; }
            set { _saveEmail = value; }
        }

        private string _saveFolderName;
        public string SaveFolderName
        {
            get { return _saveFolderName; }
            set { _saveFolderName = value; }
        }

        private string _storeId;
        public string StoreID
        {
            get { return _storeId; }
        }
        #endregion

        #region methods
        //Constructor
        public EMail()
        { }

        public EMail(List<string> mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC="", String mailBcc ="")
        {
            Init(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC, mailBcc);
        }

        public EMail(string mailTo, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "")
        {
            List<string> mailToAddress = new List<string>();
            mailToAddress = stringConvertToListString(mailTo);

            Init(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC, mailBcc);          
        }

        private void Init(List<string> mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC, String mailBcc)
        {
            _mailToAddress = mailToAddress;
            //only one emailFrom email address is allowed
            if (mailFrom.Contains(';'))
                _mailFrom = stringConvertToListString(mailFrom)[0];
            else
                _mailFrom = mailFrom;
            _mailFromName = mailFromName;
            _createDateTime = DateTime.Now;
            _subject = subject;
            setEmailType();
            _mailBody = mailBody;
            _storeId = storeId;
            _mailCCAddress = (String.IsNullOrEmpty(mailCC)) ? null : stringConvertToListString(mailCC);
            _mailBCCAddress = (String.IsNullOrEmpty(mailBcc)) ? null : stringConvertToListString(mailBcc);
            String saveEmailType = System.Configuration.ConfigurationManager.AppSettings.Get("SaveMailType");
            if (!String.IsNullOrEmpty(saveEmailType) && saveEmailType.Contains(MailType.ToString()))
                _saveEmail = true;
        }

        public void setEmailType()
        {
            if (_subject.Contains("Quotation Information"))
                MailType = MailAttrType.Quotation;
            else if (_subject.Contains("created a Quote for you"))
                MailType = MailAttrType.TransferredQuotation;
            else if (_subject.Contains("Order"))
                MailType = MailAttrType.Order;
            else if (_subject.Contains("Quantity Discount"))
                MailType = MailAttrType.QuantityDiscount;
            else if (_subject.Contains("System"))
                MailType = MailAttrType.System;
            else if (_subject.Contains("Payment Result"))
                MailType = MailAttrType.PaymentResult;
            else
                MailType = MailAttrType.Others;

            //Set save folder name
            _saveFolderName = MailType.ToString();
            setSaveEmail(true);
        }

        public void setSaveEmail(bool setAsSave)
        {
            _saveEmail = setAsSave;
        }

        /// <summary>
        /// Send email real time
        /// </summary>
        /// <returns>boolean</returns>
        /// <remarks>If returns false and </remarks>
        public EMailReponse sendMailNow()
        {
            EMailReponse _response = null;
            _response = validateEmailProperties(); 
            //Validate Email all properties
            
            if (_response.ValidateEmail != false)
            {
                MailMessage mail = new MailMessage();

                //Set mail text encoding
                if (_utf8Encoding == true)
                {
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                }

                //Compose mail
                mail.Priority = _priority;
                mail.Subject = _subject;
                mail.Body = _mailBody;

                //Mail from
                MailAddress from = new MailAddress(_mailFrom, _mailFromName);
                mail.From = from;

                //Mail To
                if (_mailToAddress != null)
                {
                    foreach (string m in _mailToAddress)
                    {
                        MailAddress addr = new MailAddress(m);
                        mail.To.Add(addr);
                    }
                }

                //Mail CC
                if (_mailCCAddress != null)
                {
                    foreach (string m in _mailCCAddress)
                    {
                        MailAddress addr = new MailAddress(m);
                        mail.CC.Add(addr);
                    }
                }

                //Mail BCC
                if (_mailBCCAddress != null)
                {
                    foreach (string m in _mailBCCAddress)
                    {
                        MailAddress addr = new MailAddress(m);
                        mail.Bcc.Add(addr);
                    }
                }

                // Create plain view
                AlternateView plainView = null;
                if (_type == Encoding.TEXT)
                {
                    plainView = AlternateView.CreateAlternateViewFromString(_mailBody, null, "text/plain");
                    mail.AlternateViews.Add(plainView);
                }

                // Create HTML view and embedded image
                AlternateView htmlView = null;
                if (_type == Encoding.HTML)
                {
                    string bodyAppend = null;
                    if (_embeddedImage != null)
                    {
                        //Create the LinkedResource (embedded image)
                        for (int i = 1; i <= _embeddedImage.Count; i++)
                        {
                            bodyAppend += "<br /><img src=cid:embed" + i + ">";
                        }
                        htmlView = AlternateView.CreateAlternateViewFromString(_mailBody + bodyAppend, null, "text/html");

                        for (int i = 0; i <= _embeddedImage.Count - 1; i++)
                        {
                            LinkedResource logo = new LinkedResource(_embeddedImage[i].ToString());
                            logo.ContentId = "embed" + (i + 1).ToString();
                            htmlView.LinkedResources.Add(logo);
                            i += 1;
                        }
                    }
                    else
                    {
                        if (_customizedImage != null)
                        {
                            htmlView = AlternateView.CreateAlternateViewFromString(_mailBody, null, System.Net.Mime.MediaTypeNames.Text.Html);
                            foreach (var image in _customizedImage)
                                htmlView.LinkedResources.Add(image);
                        }
                        else
                            htmlView = AlternateView.CreateAlternateViewFromString(_mailBody, null, "text/html");
                    }
                    mail.AlternateViews.Add(htmlView);

                    // Add attachments
                    if (_attached != null)
                    {
                        foreach (string filePath in _attached)
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                mail.Attachments.Add(new Attachment(filePath));
                            }
                        }
                    }

                    //Save the EMail to repository
                    if (this.SaveEmail == true)
                        saveEmailAsFile();

                    //Prepare SMTP setting, and then send the message
                    if (useMasterSMTP(mail))
                    {
                        _response.SendResult = true;
                        _response.SendDateTime = DateTime.Now;
                        return _response;
                    }
                    else
                    {
                        _response.SendResult = false;
                        return _response;
                    }                    
                }
                return _response;
            }
            else
                return _response;
        }

        /// <summary>
        /// Use master SMTP server as first choice, if failed then call slave SMTP server.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        private bool useMasterSMTP(MailMessage mail)
        {
            if (!string.IsNullOrEmpty(_smtpMasterHost))
            {
                SmtpClient masterSMTP;
                masterSMTP = new SmtpClient(_smtpMasterHost);
                try 
                {
                    masterSMTP.Send(mail);
                    return true;
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        // If mail server is busy or server is unavailable, mailer will resend mail in 5 minutes.
                        if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                        {
                            System.Threading.Thread.Sleep(_retryToSendMailPeriod);
                            masterSMTP.Send(mail);
                        }
                        else
                        {
                            eStoreLoger.Warn("Failed to deliver message to " + ex.FailedRecipient[i], "", "", "", ex);
                        }
                    }
                    return false;
                }

                catch (SmtpException ex)
                {
                    eStoreLoger.Warn(ex.Message, "", "", "", ex);
                    //Active slave smtp to send mail
                    if (useSlaveSMTP(mail))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                if (useSlaveSMTP(mail))
                    return true;
                else
                    return false;
            }
        }

        private bool useSlaveSMTP(MailMessage mail)
        {
            try
            {
                if (!string.IsNullOrEmpty(_smtpSlaveHost))
                {
                    SmtpClient slaveSMTP;
                    slaveSMTP = new SmtpClient(_smtpSlaveHost);
                    try 
                    {
                        slaveSMTP.Send(mail);
                        return true;
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            // If mail server is busy or server is unavailable, mailer will resend mail in 5 minutes.
                            if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                            {
                                System.Threading.Thread.Sleep(_retryToSendMailPeriod);
                                slaveSMTP.Send(mail);
                            }
                            else
                            {
                                eStoreLoger.Warn("Failed to deliver message to " + ex.FailedRecipient[i], "", "", "", ex);
                            }
                        }
                        return false;
                    }

                    catch (SmtpException ex)
                    {
                        eStoreLoger.Warn(ex.Message, "", "", "", ex);
                        return false;
                    }
                }
                else
                {
                    Exception ex = new Exception("No available SMTP servers");
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("No available SMTP servsers", "", "", "", ex);
                return false;
            }
        }

        /// <summary>
        /// This method is used to validate all email properties. 
        ///  If occurs error, it will set send result as false and active error code.
        ///  Validate following properties - 
        ///  1. mail from address
        ///  2. mail to address. If validator find a invalid emailaddress, it will be removed.
        ///  3. CC address.  If validator find a invalid emailaddress, it will be removed.
        ///  4. BCC address. If validator find a invalid emailaddress, it will be removed.
        ///  5. Subject.
        ///  6. Mail body.
        /// </summary>
        public EMailReponse validateEmailProperties()
        {
            EMailReponse _response = new EMailReponse();

            // Validate mail from address.
            if (Validator.isValidEmail(_mailFrom) == false)
            {
                _response.ValidateEmail = false;
                _response.InvalidMailFromAddress = _mailFrom;
                _response.ErrCode = EMailReponse.ErrorCode.MissingOrInvalidMailFromAddress;
            }

            // Validate all mail to address, and remove invalid mail address
            if (Validator.areValidEmail(_mailToAddress) == false)
            {
                List<string> _invalidMailToAddr = new List<string>();
                foreach (string m in Validator.getInvalidEmails(_mailToAddress))
                {
                    _invalidMailToAddr.Add(m);
                }
                _response.InvalidMailToAddress = _invalidMailToAddr;

                if (_mailToAddress.Count == _response.InvalidMailToAddress.Count)
                {
                    _response.ErrCode = EMailReponse.ErrorCode.NoneValidMailToAddress;
                    _response.ValidateEmail = false;
                }
                else
                {
                    MailAddressCollection validMailtoAddress = Validator.getValidEmails(_mailToAddress);
                    _mailToAddress = null;
                    _mailToAddress = new List<string>();
                    foreach (MailAddress m in validMailtoAddress)
                    {
                        string addr = m.Address;
                        _mailToAddress.Add(addr);
                    }
                    _response.ErrCode = EMailReponse.ErrorCode.SomeInvalidMailToAddress;
                }
            }

            // Validate all mail CC address, and remove invalid mail address
            if (_mailCCAddress != null)
            {
                if (Validator.areValidEmail(_mailCCAddress) == false)
                {
                    List<string> _invalidMailCCAddr = new List<string>();
                    foreach (string m in Validator.getInvalidEmails(_mailCCAddress))
                    {
                        _invalidMailCCAddr.Add(m);
                    }
                    _response.InvalidMailCCAddress = _invalidMailCCAddr;

                    if (_response.InvalidMailCCAddress != null)
                    {
                        if (_mailCCAddress.Count == _response.InvalidMailCCAddress.Count)
                        {
                            _response.ErrCode = EMailReponse.ErrorCode.NoneValidMailCCAddress;
                            _response.ValidateEmail = false;
                        }
                        else
                        {
                            MailAddressCollection validMailCCAddress = Validator.getValidEmails(_mailCCAddress);
                            _mailCCAddress = null;
                            _mailCCAddress = new List<string>();
                            foreach (MailAddress m in validMailCCAddress)
                            {
                                string addr = m.Address;
                                _mailCCAddress.Add(addr);
                            }
                            _response.ErrCode = EMailReponse.ErrorCode.SomeInvalidMailCCAddress;
                        }
                    }
                }
            }

            // Validate all mail BCC address, and remove invalid mail address
            if (_mailBCCAddress != null)
            {
                if (Validator.areValidEmail(_mailBCCAddress) == false)
                {
                    List<string> _invalidMailBCCAddr = new List<string>();
                    foreach (string m in Validator.getInvalidEmails(_mailBCCAddress))
                    {
                        _invalidMailBCCAddr.Add(m);
                    }
                    _response.InvalidMailCCAddress = _invalidMailBCCAddr;

                    if (_response.InvalidMailBCCAddress != null)
                    {
                        if (_mailCCAddress.Count == _response.InvalidMailBCCAddress.Count)
                        {
                            _response.ErrCode = EMailReponse.ErrorCode.NoneValidMailBCCAddress;
                            _response.ValidateEmail = false;
                        }
                        else
                        {
                            MailAddressCollection validMailBCCAddress = Validator.getValidEmails(_mailBCCAddress);
                            _mailBCCAddress = null;
                            _mailBCCAddress = new List<string>();
                            foreach (MailAddress m in validMailBCCAddress)
                            {
                                string addr = m.Address;
                                _mailBCCAddress.Add(addr);
                            }
                            _response.ErrCode = EMailReponse.ErrorCode.SomeInvalidMailBCCAddress;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(_subject))
            {
                _response.ValidateEmail = false;
                _response.ErrCode = EMailReponse.ErrorCode.MissingSubject;
            }

            if (string.IsNullOrEmpty(_mailBody))
            {
                _response.ValidateEmail = false;
                _response.ErrCode = EMailReponse.ErrorCode.EmptyMailContent;
            }

            return _response;
        }

        /// <summary>
        /// This method is used to convert string to List<string>
        /// It can convert a couple email address that seperate by ';' to list of string.
        /// </summary>
        /// <param name="mails"></param>
        /// <returns></returns>
        public List<string> stringConvertToListString(string mails)
        {
            List<string> mailList = new List<string>();

            if (string.IsNullOrEmpty(mails))
            {
                mailList = null;
            }
            else
            {
                if (mails.Contains(';'))
                {
                    string[] mailListSplit = mails.Split(';');
                    foreach (string s in mailListSplit)
                    {
                        if (!String.IsNullOrWhiteSpace(s))
                            mailList.Add(s.Trim());
                    }
                }
                else
                    mailList.Add(mails.Trim());
            }

            return mailList;
        }

        /// <summary>
        /// This method is used to convert List<string> to MailAddressCollection
        /// </summary>
        /// <param name="mailList"></param>
        /// <returns></returns>
        public MailAddressCollection listStringConvertToMailAddressCollection(List<string> mailList)
        {
            MailAddressCollection mailAddresssCollection = new MailAddressCollection();
            foreach (string addr in mailList)
            {
                MailAddress mail = new MailAddress(addr);
                mailAddresssCollection.Add(mail);
            }

            return mailAddresssCollection;
        }

        /// <summary>
        /// This function can save email as file. According to mail from address then save different folder.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public void saveEmailAsFile()
        {
            string mailFrom = this._mailFrom;
            StringBuilder filePath = new StringBuilder();
            //string storePath = (string.IsNullOrEmpty(_storeId)) ? "Others" : _storeId;
            //storePath = (_subject.Contains("System")) ? "System" : storePath;  
            
            if(_saveFolderName != "" && _saveFolderName != "System")
                filePath.Append(ConfigurationManager.AppSettings.Get("Mail_Path")).Append("/").Append(_storeId).Append("/").Append(_saveFolderName).Append("/");
            else if(_saveFolderName != "" && _saveFolderName == "System")
                filePath.Append(ConfigurationManager.AppSettings.Get("Mail_Path")).Append("/").Append(_storeId).Append("/").Append(_saveFolderName).Append("/");
            else
                filePath.Append(ConfigurationManager.AppSettings.Get("Mail_Path")).Append("/").Append(_storeId).Append("/").Append("Others").Append("/");

            if (Directory.Exists(filePath.ToString()) == false)//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(filePath.ToString());
            }

            //Compose as HTML file
            string mailHeader = "";
            string mailHtml = "";
            string fileName = "";
            string sender = "<b>Sender:</b> " + this._mailFrom+"<br />";
            string sendDate = "<b>Send Date:</b> " + this._createDateTime.ToString("yyyy/MM/dd  HH:mm:ss") + "<br />";
            string receiverMail = "";
            int i = 0;
            string filenameAppend = "";
            foreach (string r in this._mailToAddress)
            {
                if (i == 0)
                    filenameAppend = r; i = 1;
                receiverMail += r + "; ";
            }
            string recevier = "<b>Receiver:</b> " + receiverMail+"<br />";
            string ccMails = "";
            string cc = "";
            if (_mailCCAddress != null)
            {
                if (_mailCCAddress.Count > 0)
                {
                    foreach (string r in this._mailCCAddress)
                    { ccMails += r + "; "; }
                    cc = "<b>CC:</b> " + ccMails + "<br />";
                }
            }
            string bccMails = "";
            string bcc = "";
            if (_mailBCCAddress != null)
            {
                if (_mailBCCAddress.Count > 0)
                {
                    foreach (string r in this._mailBCCAddress)
                    { bccMails += r + "; "; }
                    bcc = "<b>BCC:</b> " + bccMails + "<br />";
                }
            }
            string subject = "<b>Subject:</b> "+this._subject+"<br />";
            mailHeader = sender + sendDate + recevier + cc + bcc + subject;
            mailHtml = mailHeader + this._mailBody;
            fileName = this._createDateTime.ToString("yyyyMMdd_HHmmss_fff") + "__" + filenameAppend + ".html";


            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            {
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }

            using (StreamWriter sw = new StreamWriter(filePath+fileName))
            {
                sw.Write(mailHtml);
                sw.Close();
            }
        }
        #endregion

        #region Send Mail Unit Test
        /// <summary>
        /// Unit test
        /// To send a test email.
        /// </summary>
        public void sendTestMail()
        {
            // Mail From Address
            string mailFromAddr = "buy@advantech.com";
            string mailFromName = "Jimmy.Xiao";

            // Mail To Address
            List<string> mailTo = new List<string>();
            string mailTo1 = "jimmy@yesjimmy.com";
            string mailTo2 = "think.jimmy@gmail.com";
            string mailTo3 = "admin@yesjimmy.com";
            mailTo.Add(mailTo1);
            mailTo.Add(mailTo2);
            mailTo.Add(mailTo3);

            // Mail CC Address
            List<string> mailCCs = new List<string>();
            string mailCC1 = "jimmy.xiao@advantech.com.tw";
            string mailCC2 = "xiao_jimmy@yahoo.com";
            string mailCC3 = "jimmy.xiao@advantech.com";
            mailCCs.Add(mailCC1);
            mailCCs.Add(mailCC2);
            mailCCs.Add(mailCC3);

            // Mail BCC Address
            List<string> mailBCCs = new List<string>();
            string mailBCC1 = "jimmy.xiao@advantech.com";
            mailBCCs.Add(mailBCC1);

            string subject = "[Test Email] eStore 3.0     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string body = "<h1><font color=red>This is a test mail.</font></h1>";

            EMail mail = new EMail(mailTo, mailFromAddr, mailFromName, subject, body,"");

            mail._mailCCAddress = mailCCs;
            mail._mailBCCAddress = mailBCCs;
            mail.SaveEmail = true;
            EMailReponse _response = mail.sendMailNow();
        }
        #endregion

    }


    /// <summary>
    /// This class is used to describe the return status of email validation or send out.
    /// </summary>
    public class EMailReponse
    {
        #region properties
        private DateTime _sendDateTime;
        public DateTime SendDateTime
        {
            get { return _sendDateTime; }
            set { _sendDateTime = value; }
        }

        private bool _sendResult;
        public bool SendResult
        {
            get { return _sendResult; }
            set { _sendResult = value; }
        }

        private string _invalidMailFromAddress;
        public string InvalidMailFromAddress
        {
            get { return _invalidMailFromAddress; }
            set { _invalidMailFromAddress = value; }
        }

        private List<string> _invalidMailToAddress;
        public List<string> InvalidMailToAddress
        {
            get{ return _invalidMailToAddress;}
            set { _invalidMailToAddress = value; }
        }

        private List<string> _invalidMailCCAddress;
        public List<string> InvalidMailCCAddress
        {
            get { return _invalidMailCCAddress; }
            set { _invalidMailCCAddress = value; }
        }

        private List<string> _invalidMailBCCAddress;
        public List<string> InvalidMailBCCAddress
        {
            get { return _invalidMailBCCAddress; }
            set { _invalidMailBCCAddress = value; }
        }

        //All mail will be saved as physical files.
        private string _mailFilename;
        public string MailFilename
        {
            get { return _mailFilename; }
            set { _mailFilename = value; }
        }

        private bool _validateEMail = true;
        public bool ValidateEmail
        {
            get { return _validateEMail; }
            set { _validateEMail = value; }
        }

        public enum ErrorCode
        {
            NoError,
            MasterSMTPconnectingFailed,
            AllSMTPconnectingFailed,
            NoneValidMailToAddress,
            SomeInvalidMailToAddress,
            MissingOrInvalidMailFromAddress,
            NoneValidMailCCAddress,
            SomeInvalidMailCCAddress,
            NoneValidMailBCCAddress,
            SomeInvalidMailBCCAddress,
            MissingSubject,
            EmptyMailContent,
            FailToGetEMailTemplate,
            FailToSaveEmail,
            UnknowError,
            CalledFunctionException
        };

        private ErrorCode _errCode;
        public ErrorCode ErrCode
        {
            get { return _errCode; }
            set { _errCode = value; }
        }
        #endregion

        #region methods
        public EMailReponse()
        { }
        #endregion
    }


    public class EmailHelper
    {
        public EmailHelper()
        { }
        ~EmailHelper()
        { }

        #region


        public static EMailReponse send2eStoreItGroup(string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "")
        {
            string _eStoreItEmailGroup = System.Configuration.ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
            EMail mail = new EMail(_eStoreItEmailGroup,
                mailFrom,
                mailFromName,
                subject,
                mailBody,
                storeId);
            return mail.sendMailNow();
        }

        public static EMailReponse sendEdiOrderError2eStoreItGroup(string subject, string mailBody, Exception ex = null)
        {
            if (ex != null)
                mailBody = string.Format("{0}<br />Exception Message:{1}<br />Exception Inner Message{2}<br />", mailBody, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);
            return send2eStoreItGroup("eStoreEDI@advantech.com",
                "eStoreEDI",
                subject,
                mailBody,
                "AUS");
        }


        #endregion
    }
}
