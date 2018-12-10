using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using esUtilities;
using System.Configuration;

namespace eStore.BusinessModules
{
    public class EMailDiscountRequest:IMailer
    {
        /// <summary>
        /// If test mode is enable, will append tag [TESTING] to subject, and send mail to eStore IT.
        /// </summary>
        private string testing = ConfigurationManager.AppSettings.Get("TestingMode");
        private bool testMode()
        {
            if (testing == "true")
                return true;
            else
                return false;
        }

        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        public string TestingOrderDeptEmail
        {
            get { return testingOrderDeptEmail; }
            set { testingOrderDeptEmail = value; }
        }

        public EMailDiscountRequest()
        { }

        #region Methods
        /// <summary>
        /// This function will generate a finished email content of volumn discount request.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="request"></param>
        /// <returns>Email content in string</returns>
        public EMailReponse getDiscountRequestEmailTemplate(Store store, UserRequest request, string emailTitle = "", POCOS.User user = null, Language language = null, MiniSite minisite = null)
        {
            string _emailResult = null;     //Finished EMail content
            string _productDisplayName = string.Empty;
            string _productDescription =string.Empty;
            POCOS.Store.BusinessGroup group = POCOS.Store.BusinessGroup.eP;
            if (request.ProductX != null)
            {
                  _productDisplayName = request.ProductX.DisplayPartno;
                  _productDescription = request.ProductX.productDescX;
                  group = request.ProductX.businessGroup;
            }
          
            string _qty = request.Quantity.ToString();
            string _budget = string.IsNullOrEmpty(request.Budget) ? string.Empty : request.Budget;
            string _customerName = store.getCultureGreetingName(request.FirstName, request.LastName);
            string _customerFullName = store.getCultureFullName(request.FirstName, request.LastName);
            string _company = string.IsNullOrEmpty(request.Company) ? string.Empty : request.Company;
            string _email = string.IsNullOrEmpty(request.Email) ? string.Empty : request.Email;
            string _country = string.IsNullOrEmpty(request.Country) ? string.Empty : request.Country;
            string _address = string.IsNullOrEmpty(request.Address) ? string.Empty : request.Address;
            string _phone = string.IsNullOrEmpty(request.Telephone) ? string.Empty : request.Telephone;

            string _contactType = string.IsNullOrEmpty(request.ContactType) ? string.Empty : request.ContactType;

            string _comments = string.IsNullOrEmpty(request.Comment) ? string.Empty : request.Comment;
            string _tel = null;
            foreach (StoreAddress addr in store.profile.StoreAddresses)
            {
                if (addr.Division == "EP")
                    _tel = addr.Address.Tel;
            }
            string _storeurl = store.getCurrStoreUrl(store.profile, minisite);
            string _contactMail = store.profile.OrderDeptEmail;
            string emailGroup = _contactMail;

            if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup")) // now only ushop minisite use site Parameter email group
                emailGroup = minisite.Settings["EmailGroup"];
            else
            {
                POCOS.Address _storeAddress = store.getAddressByCountry(request.Country, group);
                if (_storeAddress != null)
                {
                    if (string.IsNullOrEmpty(_storeAddress.EmailGroup))
                        emailGroup = _contactMail;
                    else if (!_contactMail.Split(';').Contains(_storeAddress.EmailGroup))
                    {
                        if (!(_storeAddress.Exclusive.HasValue && _storeAddress.Exclusive.Value))
                            emailGroup = _contactMail + ";" + _storeAddress.EmailGroup;
                        else
                            emailGroup = _storeAddress.EmailGroup;
                    }
                    if (!string.IsNullOrEmpty(_storeAddress.Tel))
                        _tel = _storeAddress.Tel;
                }
            }
            string _emptyTemplate = null;
            string resultInternalDisplay = string.Empty;
            string resultCustomerDisplay = string.Empty;

            try
            {
                _emptyTemplate = store.getTemplate(Store.TEMPLATE_TYPE.VolumnDiscountRequest, language,minisite);
                _emailResult = _emptyTemplate.Replace("[/CustomerName]", _customerName);
                _emailResult = _emailResult.Replace("[/CustomerFullName]", _customerFullName);
                _emailResult = _emailResult.Replace("[/ProductDisplayName]", _productDisplayName);
                _emailResult = _emailResult.Replace("[/ProductDescription]", _productDescription);
                _emailResult = _emailResult.Replace("[/QTY]", _qty);
                _emailResult = _emailResult.Replace("[/Budget]", _budget);
                _emailResult = _emailResult.Replace("[/Country]", _country);
                _emailResult = _emailResult.Replace("[/CompanyName]", _company);
                _emailResult = _emailResult.Replace("[/Address]", _address);
                _emailResult = _emailResult.Replace("[/Email]", _email);
                _emailResult = _emailResult.Replace("[/Phone]", _phone);
                _emailResult = _emailResult.Replace("[/ContactType]", _contactType);
                _emailResult = _emailResult.Replace("[/Comments]", _comments);
                _emailResult = _emailResult.Replace("[/TEL]", _tel);
                _emailResult = _emailResult.Replace("[/CONTACTMAIL]", emailGroup);
                _emailResult = _emailResult.Replace("[/STOREURL]", _storeurl);
                _emailResult = _emailResult.Replace("[/CONTACTUSTYPE]", emailTitle == "" ? "quantity discount" : emailTitle.ToLower());

                if (minisite != null && (minisite.MiniSiteType == MiniSite.SiteType.IotMart || minisite.MiniSiteType == MiniSite.SiteType.UShop))
                    _emailResult = _emailResult.Replace("/AUS/logo.gif", string.Format("/{0}/{1}/logo.gif", minisite.StoreID, minisite.SiteName));

                if (emailTitle.Equals(store.Tanslation("eStore_Advantech_eStore_Contact_for_Price"), StringComparison.OrdinalIgnoreCase))
                {
                    _emailResult = System.Text.RegularExpressions.Regex.Replace(_emailResult, @"<tr id=""rQTY[\w\W]*?</tr>", "");
                    _emailResult = System.Text.RegularExpressions.Regex.Replace(_emailResult, @"<tr id=""rRequestDate[\w\W]*?</tr>", "");
                    _emailResult = System.Text.RegularExpressions.Regex.Replace(_emailResult, @"<tr id=""rLeadTime[\w\W]*?</tr>", "");
                    _emailResult = System.Text.RegularExpressions.Regex.Replace(_emailResult, @"<tr id=""rBudget[\w\W]*?</tr>", "");
                }
                resultInternalDisplay = _emailResult;
                string _requestInternalDate = store.getLocalTime(request.CreatedDate.HasValue ? request.CreatedDate.Value : DateTime.Now, user, true);
                string _LeadInternalTime = store.getLocalTime(request.ExpectedDate.HasValue ? request.ExpectedDate.Value : DateTime.Now, user, true);
                resultInternalDisplay = resultInternalDisplay.Replace("[/RequestDate]", _requestInternalDate);
                resultInternalDisplay = resultInternalDisplay.Replace("[/LeadTime]", _LeadInternalTime);

                resultCustomerDisplay = _emailResult;
                string _requestCustomerDate = store.getLocalTime(request.CreatedDate.HasValue ? request.CreatedDate.Value : DateTime.Now, user, true);
                string _LeadCustomerTime = store.getLocalTime(request.ExpectedDate.HasValue ? request.ExpectedDate.Value : DateTime.Now, user, true);
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/RequestDate]", _requestCustomerDate);
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/LeadTime]", _LeadCustomerTime);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Can not get email template of discount request", "", "", "", ex);
            }

            string mailFromName = "";
            string mailSubject = "";
            mailFromName = store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore, true, language,minisite);
            mailSubject = string.IsNullOrEmpty(emailTitle) == true ? store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Request_Quantity_Discount, true, language,minisite) : emailTitle;

            if (testMode() == true)
            {
                mailFromName += string.Format(" {0} ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language,minisite));
                mailSubject = string.Format("[{0}] ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language,minisite)) + mailSubject;

                EMailReponse orderDeptEmailResponse = sendMail(testingOrderDeptEmail, _email, mailFromName, mailSubject, resultInternalDisplay, store.storeID.ToUpper());
                EMailReponse customerEmailResponse = sendMail(_email, _contactMail, emailGroup.Split(';')[0], mailSubject, resultCustomerDisplay, store.storeID.ToUpper());
                return customerEmailResponse;
            }
            else
            {
                // Prepare to send mail
                if (request.RequestType.ToLower().Equals("technicalsupport"))
                    emailGroup += ";" + store.profile.getStringSetting("TechnicalSupportMail");
                EMailReponse orderDeptEmailResponse = sendMail(emailGroup, _email, mailFromName, mailSubject, resultInternalDisplay, store.storeID.ToUpper());
                EMailReponse customerEmailResponse = sendMail(_email, emailGroup.Split(';')[0], mailFromName, mailSubject, resultCustomerDisplay, store.storeID.ToUpper());
                return customerEmailResponse;
            }
            
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
        public EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "")
        {
            EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC, mailBcc);
            EMailReponse response = mail.sendMailNow();
            return response;
        }
        #endregion
    }
}
