using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using esUtilities;
using System.Configuration;
using System.IO;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class EmailECOPartner
    {
        public enum MailType { Partner, RequestAssistance, Recommend2Friend, None }

        private bool _stagingMode = true;
        private string _internalEmail;
        private Store _store;
        public Store store
        {
            get { return _store; }
        }

        private ECOPartner _partner;

        public ECOPartner partner
        {
            get { return _partner; }
            set { _partner = value; }
        }


        public EmailECOPartner(Store store,ECOPartner partner)
        {
            String stagingMode = ConfigurationManager.AppSettings.Get("TestingMode");
            if (stagingMode.ToLower().Equals("true"))
                _stagingMode = true;
            else
                _stagingMode = false;

            _internalEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
            this._store = store;
            this._partner = partner;
        }

        public EMailReponse sendECOPartner(User user)
        {
            ECOEmailTemp emailTemp = new ECOEmailTemp(MailType.Partner, _store);
            if (!string.IsNullOrEmpty(emailTemp.ldrTempStr))
            {
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/UserName]", _store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/STOREURL]", _store.profile.StoreURL);
            }
            if (!string.IsNullOrEmpty(emailTemp.customerTempStr))
            {
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/UserName]", _store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/STOREURL]", _store.profile.StoreURL);
            }

            return setMail(emailTemp, user);
        }

        public EMailReponse sendECORecommend2Friend(User user,string emailTo)
        {
            ECOEmailTemp emailTemp = new ECOEmailTemp(MailType.Recommend2Friend, _store);
            if (!string.IsNullOrEmpty(emailTemp.customerTempStr))
            {
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/Customer]", _store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/STOREURL]", _store.profile.StoreURL);
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/Copany]", _partner.CompanyName);
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/CopanyWebUrl]", _partner.WebSiteUrl);
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/UserId]", user.UserID);
            }
            return setMail(emailTemp, user, emailTo);
        }

        public EMailReponse sendRequestAssistance(User user, UserRequest request)
        {
            ECOEmailTemp emailTemp = new ECOEmailTemp(MailType.RequestAssistance, _store);
            if (!string.IsNullOrEmpty(emailTemp.ldrTempStr))
            {
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/Customer]",_store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/COMPANYNAME]", _partner.CompanyName);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/ProjectName]",request.ProductDesc);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/ProductInterest]",request.ProductName);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/Location]",request.Address);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/ContactInfo]",request.Telephone);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/BesttimeofContact]",request.ContactType);
                emailTemp.ldrTempStr = emailTemp.ldrTempStr.Replace("[/AdditionalInformation]",request.Comment);
            }
            if (!string.IsNullOrEmpty(emailTemp.venderTempStr))
            {
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/Customer]", _store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/COMPANYNAME]", _partner.CompanyName);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/ProjectName]", request.ProductDesc);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/ProductInterest]", request.ProductName);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/Location]", request.Address);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/ContactInfo]", request.Telephone);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/BesttimeofContact]", request.ContactType);
                emailTemp.venderTempStr = emailTemp.venderTempStr.Replace("[/AdditionalInformation]", request.Comment);
            }
            if (!string.IsNullOrEmpty(emailTemp.customerTempStr))
            {
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/UserName]", _store.getCultureGreetingName(user.FirstName, user.LastName));
                emailTemp.customerTempStr = emailTemp.customerTempStr.Replace("[/STOREURL]", _store.profile.StoreURL);
            }

            return setMail(emailTemp,user);
        }

        private EMailReponse setMail(ECOEmailTemp temp, User user, string emailTo = "")
        {
            List<EMailReponse> results = new List<EMailReponse>();
            string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
            string contactMail = _store.profile.OrderDeptEmail;
            string emailGroup = contactMail;

            if(!string.IsNullOrEmpty(temp.ldrTempStr)) //内部邮件
            {
                //POCOS.Address _storeAddress = _store.getAddressByCountry(_store.profile.Country, POCOS.Store.BusinessGroup.eP); //需要获取user当前选中的country
                //if (_storeAddress != null)
                //{
                //    if (string.IsNullOrEmpty(_storeAddress.EmailGroup))
                //        emailGroup = contactMail;
                //    else if (!contactMail.Split(';').Contains(_storeAddress.EmailGroup))
                //    {
                //        if (_store.profile.getBooleanSetting("ccStoreOrderGroup"))
                //            emailGroup = contactMail + ";" + _storeAddress.EmailGroup;
                //        else
                //            emailGroup = _storeAddress.EmailGroup;
                //    }
                //}
                if (_stagingMode)
                {
                    string mailFromName = temp.ldrTempTitle + string.Format(" {0} ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester));
                    string mailSubject = string.Format("[{0}] ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING)) + temp.ldrTempTitle;
                    results.Add(sendMail(testingOrderDeptEmail, user.UserID, mailFromName, mailSubject, temp.ldrTempStr, _store.storeID.ToUpper()));
                    
                    results.Add(sendMail(user.UserID, emailGroup.Split(';')[0], mailFromName, mailSubject, temp.ldrTempStr, _store.storeID.ToUpper()));
                }
                else
                {
                    results.Add(sendMail(emailGroup, user.UserID, temp.ldrTempTitle, "(" + _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal) + ")" + temp.ldrTempTitle, temp.ldrTempStr, _store.storeID.ToUpper(), "", testingOrderDeptEmail, true));
                    results.Add(sendMail(user.UserID, emailGroup.Split(';')[0], temp.ldrTempTitle, temp.ldrTempTitle, temp.ldrTempStr, _store.storeID.ToUpper()));
                }
            }
            if (!string.IsNullOrEmpty(temp.venderTempStr))
            {
                if (_stagingMode)
                {
                    string mailFromName = temp.venderTempTitle + string.Format(" {0} ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester));
                    string mailSubject = string.Format("[{0}] ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING)) + temp.venderTempTitle;
                    results.Add(sendMail(testingOrderDeptEmail, user.UserID, mailFromName, mailSubject, temp.venderTempStr, _store.storeID.ToUpper()));
                }
                else
                {
                    results.Add(sendMail(partner.Email, emailGroup.Split(';')[0], temp.ldrTempTitle, temp.venderTempTitle, temp.venderTempStr, _store.storeID.ToUpper()));
                }
            }
            if (!string.IsNullOrEmpty(temp.customerTempStr))
            {
                if (string.IsNullOrEmpty(emailTo))
                    emailTo = user.UserID;
                if (_stagingMode)
                {
                    string mailFromName = temp.customerTempTitle + string.Format(" {0} ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester));
                    string mailSubject = string.Format("[{0}] ", _store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING)) + temp.customerTempTitle;
                    results.Add(sendMail(emailTo, user.UserID, mailFromName, mailSubject, temp.customerTempStr, _store.storeID.ToUpper(), testingOrderDeptEmail));
                }
                else
                {
                    results.Add(sendMail(emailTo, emailGroup.Split(';')[0], temp.customerTempTitle, temp.customerTempTitle, temp.customerTempStr, _store.storeID.ToUpper()));
                }
            }

            results.RemoveAll(c => c.ErrCode == EMailReponse.ErrorCode.NoError);
            if (results.Any())
                return results.FirstOrDefault();
            else
                return new EMailReponse() { ErrCode = EMailReponse.ErrorCode.NoError };
        }

        private EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "", bool IsneedBcc = false)
        {
            try
            {
                if (!_stagingMode)
                {
                    if (IsneedBcc)
                    {
                        string eStoreAdminGroup = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
                        if (string.IsNullOrEmpty(mailBcc))
                            mailBcc = eStoreAdminGroup + ";" + _store.profile.OrderDeptEmail;
                        else
                            mailBcc = eStoreAdminGroup + ";" + mailBcc;
                    }
                    else
                        mailBcc = "";
                }
                EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC, mailBcc);
                mail.SaveEmail = true;
                EMailReponse response = mail.sendMailNow();
                return response;
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("ECO Mail send Error", mailFrom + "[function EmailECOPartner.sendMail]", "", "", ex);
                throw ex;
            }
        }
    }

    internal class ECOEmailTemp
    {
        private string _ldrTempTitle;

        public string ldrTempTitle
        {
            get { return _ldrTempTitle; }
            set { _ldrTempTitle = value; }
        }

        private string _venderTempTitle;

        public string venderTempTitle
        {
            get { return _venderTempTitle; }
            set { _venderTempTitle = value; }
        }

        private string _customerTempTitle;

        public string customerTempTitle
        {
            get { return _customerTempTitle; }
            set { _customerTempTitle = value; }
        }


        private string _ldrTempStr;

        public string ldrTempStr
        {
            get { return _ldrTempStr; }
            set { _ldrTempStr = value; }
        }

        private string _venderTempStr;

        public string venderTempStr
        {
            get { return _venderTempStr; }
            set { _venderTempStr = value; }
        }

        private string _customerTempStr;

        public string customerTempStr
        {
            get { return _customerTempStr; }
            set { _customerTempStr = value; }
        }



        private EmailECOPartner.MailType _type = EmailECOPartner.MailType.None;
        private Store _store;

        public ECOEmailTemp(EmailECOPartner.MailType type, Store store)
        {
            this._type = type;
            this._store = store;
            getMailTemple();
        }


        private void getMailTemple()
        {

            string filePath = string.Format("{0}\\{1}\\", ConfigurationManager.AppSettings.Get("Template_Path"), _store.storeID);

            string LDR = filePath + "{0}_LDR_"+ _store.storeID +".htm";
            string Vendor = filePath + "{0}_Vendor_" + _store.storeID + ".htm";
            string Customer = filePath + "{0}_Customer_" + _store.storeID + ".htm";

            switch (_type)
            {
                case EmailECOPartner.MailType.Partner:
                    LDR = string.Format(LDR, "ECOPartner");
                    Vendor = string.Format(Vendor, "ECOPartner");
                    Customer = string.Format(Customer, "ECOPartner");
                    _ldrTempTitle = _venderTempTitle = _customerTempTitle = "Advantech Ecosystem Network Request Confirmation";
                    break;
                case EmailECOPartner.MailType.Recommend2Friend:
                    LDR = string.Format(LDR, "Recommend2Friend");
                    Vendor = string.Format(Vendor, "Recommend2Friend");
                    Customer = string.Format(Customer, "Recommend2Friend");
                    _ldrTempTitle = _venderTempTitle = _customerTempTitle = "Software Integrator Recommendation for You";
                    break;
                case EmailECOPartner.MailType.RequestAssistance:
                    LDR = string.Format(LDR, "RequestAssistance");
                    Vendor = string.Format(Vendor, "RequestAssistance");
                    Customer = string.Format(Customer, "RequestAssistance");
                    _ldrTempTitle = _venderTempTitle = _customerTempTitle = "Advantech Ecosystem Network Request";
                    break;
                default:
                    break;
            }
            this._ldrTempStr = getTempleStr(LDR);
            this._venderTempStr = getTempleStr(Vendor);
            this._customerTempStr = getTempleStr(Customer);
        }

        private string getTempleStr(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
                return "";
            try
            {
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("File could not be read", fullPath, "", "", ex);
                return "";
            }
        }

    }

    
}
