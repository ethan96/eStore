using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using esUtilities;
using System.Configuration;


namespace eStore.BusinessModules.Templates
{
    public class EmailBasicTemplate
    {
        /// <summary>
        /// If test mode is enable, will append tag [TESTING] to subject, and send mail to eStore IT.
        /// </summary>
        private bool _stagingMode = true;
        private string _internalEmail;

        /// <summary>
        /// Default constructor -- all inheriting subclass shall invoke this method
        /// </summary>
        public EmailBasicTemplate()
        {
            String stagingMode = ConfigurationManager.AppSettings.Get("TestingMode");
            if (stagingMode.ToLower().Equals("true"))
                _stagingMode = true;
            else
                _stagingMode = false;

            _internalEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        }

        #region Methods
        /// <summary>
        /// This function will compose email content with basic Email template and send it out.  It return email sending status back
        /// to the caller.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="requestInfo"></param>
        /// <returns>Email content in string</returns>
        public virtual EMailReponse sendEmailInBasicTemplate(Store store, Object requestInfo, Boolean sendInternalCopy = false, Store.TEMPLATE_TYPE template = Store.TEMPLATE_TYPE.BasicEmailTemplate)
        {
            try
            {
                String templateContent = store.getTemplate(template);
                return sendEmailWithCustomContent(store, requestInfo, templateContent, sendInternalCopy);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at composing basic email template", "", "", "", ex);
            }

            return null;
        }

        public virtual EMailReponse sendEmailInBasicTemplate(Store store, Object requestInfo, Boolean sendInternalCopy = false, String templateContent = "")
        {
            return sendEmailWithCustomContent(store, requestInfo, templateContent, sendInternalCopy);
        }

        /// <summary>
        /// This function will compose email content with preloaded template and send it out.  It return email sending status back
        /// The following are required keys for using this method
        ///     1. EmailSubject
        ///     3. EmailTo          optional -- if this is not provided, the email will be sent to emailInternalGroup or eStore IT only
        ///     7. EmailInternalGroup optional  -- if this is not provided, the email will be sent to eStore IT only
        ///     2. EmailSenderName  optional -- if not provided, it uses sendTo email as sender name
        ///     4. EmailFrom        optional  -- if not provided, it uses store.defaultContactEmail
        ///     5. EmailCC          optional 
        ///     6. EmailBcc         optional
        /// </summary>
        /// <param name="store"></param>
        /// <param name="requestInfo"></param>
        /// <param name="templateContent"></param>
        /// <returns></returns>
        private EMailReponse sendEmailWithCustomContent(Store store, Object requestInfo, String templateContent, Boolean sendInternalCopy)
        {
            //this basic template will only hanlde Dictionary type as requestInfo
            if (requestInfo != null && requestInfo is Dictionary<String, String>)
            {
                String basicTemplate = templateContent;
                String emailSubject = "";
                String emailSenderName = "";
                String emailTo = "";
                String emailFrom = "";
                String emailCC = "";
                String emailBcc = "";
                String emailDictionaryContent = "";
                String emailContent = "";
                String emailInternalGroup = "";

                Dictionary<String, String> requests = (Dictionary<String, String>)requestInfo;
                try
                {
                    emailSubject = extractValue("EmailSubject", requests);
                    emailTo = extractValue("EmailTo", requests);
                    emailFrom = extractValue("EmailFrom", requests);
                    emailCC = extractValue("EmailCC", requests);
                    emailBcc = extractValue("EmailBcc", requests);
                    emailSenderName = extractValue("EmailSenderName", requests);
                    emailInternalGroup = extractValue("EmailInternalGroup", requests);

                    //validating required information
                    if (!String.IsNullOrEmpty(emailInternalGroup))
                        emailBcc = emailBcc + ";" + emailInternalGroup;
                    if (String.IsNullOrEmpty(emailSubject))
                        return null;                            //invalid request
                    if (String.IsNullOrEmpty(emailTo) && String.IsNullOrEmpty(emailInternalGroup))  //send only to internal IT group
                    {
                        emailTo = _internalEmail;           //set internal email group as default if emailTo is not specified
                        //如果mail to的地址和抄送地址是一样的. 清除抄送地址.
                        //ABR 没有PM维护,  所以没有mail to
                        if (emailTo == emailCC)
                            emailCC = "";
                    }
                    if (String.IsNullOrEmpty(emailFrom))
                        emailFrom = store.profile.OrderDeptEmail;   //set store order department email as default if emailFrom is not specified
                    if (String.IsNullOrEmpty(emailSenderName))
                        emailSenderName = store.profile.StoreName;  //set store name as default sender if sendername is not specified

                    //compose dictionary list content
                    StringBuilder buffer = new StringBuilder();
                    foreach (String dataKey in requests.Keys)
                        buffer.Append(dataKey).Append("  :  ").AppendLine(requests[dataKey]).Append("<br> ");
                    emailDictionaryContent = buffer.ToString();

                    //check basicTemplate
                    if (String.IsNullOrWhiteSpace(basicTemplate))
                        emailContent = emailDictionaryContent;
                    else
                    {
                        emailContent = basicTemplate;
                        String replacingTag = String.Empty;
                        foreach (String dataKey in requests.Keys)
                        {
                            replacingTag = "[/" + dataKey + "]";
                            emailContent = emailContent.Replace(replacingTag, requests[dataKey]);
                        }
                        emailContent = emailContent.Replace("[/EmailDictionaryList]", emailDictionaryContent);
                    }

                    if (String.IsNullOrEmpty(emailContent))
                        emailContent = "Message from " + emailSenderName; //set default content
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Exception at composing basic email template", "", "", "", ex);
                }

                //adding eStore 3.0 testing for internal testing message
                emailSubject = (_stagingMode == true) ? ("[eStore 3.0 TESTING] " + emailSubject) : emailSubject;

                EMailReponse response = null;

                //send message to emailTo account
                response = sendMail(emailTo, emailFrom, emailSenderName, emailSubject, emailContent, store.storeID, emailCC, emailBcc);
                //send internal copy
                if (sendInternalCopy && !String.IsNullOrEmpty(emailInternalGroup)) //send only to sendTo
                {
                    //send to internal group for double valdiation purpose
                    emailSubject = "(internal) " + emailSubject;
                    response = sendMail(emailInternalGroup, emailFrom, emailSenderName, emailSubject, emailContent, store.storeID);
                }

                return response;
            }

            return null;
        }

        /// <summary>
        /// This method is used to send Email and it return mail response. If programmer needs to save the Email, mail.SaveEmail should be set as true.
        /// </summary>
        /// <param name="mailToAddress"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailFromName"></param>
        /// <param name="subject"></param>
        /// <param name="mailBody"></param>
        /// <returns></returns>
        public EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String emailCC = "", String emailBcc = "")
        {
            EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, emailCC, emailBcc);
            EMailReponse response = mail.sendMailNow();
            return response;
        }

        /// <summary>
        /// This method will extract value of matched key in input requests and remove this key/value entry from requests
        /// </summary>
        /// <param name="key"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        private String extractValue(String key, Dictionary<String, String> requests)
        {
            String value = null;
            if (requests.ContainsKey(key))
            {
                value = requests[key];
                requests.Remove(key);
            }

            return value;
        }
        #endregion
    }
}
