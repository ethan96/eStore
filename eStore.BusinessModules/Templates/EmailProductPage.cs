using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using esUtilities;

namespace eStore.BusinessModules
{
    public class EmailProductPage:IMailer
    {
        public EMailReponse getEmailProductPage(Store store,Product product, string firstname, string lastname,string comments, string emailTo, string emailFrom, MiniSite minisite = null)
        {
            if (store == null)
                throw new Exception("StoreIsNull");
            if (product == null)
                throw new Exception("ProductIsNull");
            if (emailTo == null)
                throw new Exception("EMailToIsNull");

            if (string.IsNullOrEmpty(emailFrom))
                emailFrom = store.profile.OrderDeptEmail;
            
            PartHelper helper = new PartHelper();
            Part part = helper.getPart(product.SProductID, product.storeX);
            string productReferralTemplate = null;
            string modelName = part.ModelNo;
            string modelDesc = part.productDescX;
            string modelFeature = part.productFeatures;
            string modelPicture = part.thumbnailImageX;
            Price modelListingPrice = part.getListingPrice();
            //string currency = (modelListingPrice.currency == null) ? "" : modelListingPrice.currency.ToString();
            string currency = (part.Currency == null) ? "" : part.Currency;
            string modelPrice = "Price: " + currency + " " + part.getListingPrice().value;
            string fromName = firstname.ToUpper() + " " + lastname.ToUpper();
            string storeUrl = store.getCurrStoreUrl(store.profile, minisite);
            string contactPhone = null;
            string sendDate = DateTime.Now.ToString("MM/dd/yyyy");
            foreach (StoreAddress addr in store.profile.StoreAddresses)
            {
                if (addr.Division == "EP")
                    contactPhone = addr.Address.Tel;
            }
            string _contactMail = store.profile.OrderDeptEmail;
            string result = null;

            try
            {
                productReferralTemplate = store.getTemplate(Store.TEMPLATE_TYPE.ProductReferral);
                result = productReferralTemplate.Replace("[/FROMNAME]", fromName);
                result = result.Replace("[/COMMENTS]", comments);
                result = result.Replace("[/MODELNAME]", modelName);
                result = result.Replace("[/MODELDESC]", modelDesc);
                result = result.Replace("[/MODELPICTURE]", modelPicture);
                result = result.Replace("[/MODELFEATURES]", modelFeature);
                result = result.Replace("[/MODELPRICE]", modelPrice);
                result = result.Replace("[/CONTACTPHONE]", contactPhone);
                result = result.Replace("[/SENDDATE]", sendDate);
                result = result.Replace("[/CONTACTEMAIL]", _contactMail);
                result = result.Replace("[/STOREURL]", storeUrl);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Can not get email template of product referral", "", "", "", ex);
                throw ex;
            }

            // Prepare to send mail
            EMailReponse response = sendMail(emailTo,emailFrom , "Advantech eStore", fromName + " would like to share a product with you from buy.advantech.com", result, store.storeID);

            return response;
        }

        public EMailReponse getEmailProductPage(Store store, Product_Ctos product, string firstname, string lastname, string comments, string emailTo, string emailFrom, MiniSite minisite = null)
        {

            Part part = product.parthelper.getPart(product.SProductID, product.storeX);
            string productReferralTemplate = null;
            string modelName = product.name;
            string modelDesc = product.ProductDesc;
            string modelFeature = product.productFeatures;
            string modelPicture = part.thumbnailImageX;
            Price modelListingPrice = product.getListingPrice();
            string currency = (modelListingPrice.currency == null) ? "" : modelListingPrice.currency.ToString();
            string modelPrice = "Price: " + currency + " " + modelListingPrice.value.ToString();
            string fromName = firstname.ToUpper() + " " + lastname.ToUpper();
            string storeUrl = store.getCurrStoreUrl(store.profile,minisite);
            string contactPhone = null;
            foreach (StoreAddress addr in store.profile.StoreAddresses)
            {
                if (addr.Division == "EP")
                    contactPhone = addr.Address.Tel;
            }
            string _contactMail = store.profile.OrderDeptEmail;
            string result = null;

            try
            {
                productReferralTemplate = store.getTemplate(Store.TEMPLATE_TYPE.ProductReferral);
                result = productReferralTemplate.Replace("[/FROMNAME]", fromName);
                result = result.Replace("[/COMMENTS]", comments);
                result = result.Replace("[/MODELNAME]", modelName);
                result = result.Replace("[/MODELDESC]", modelDesc);
                result = result.Replace("[/MODELURL]", modelPicture);
                result = result.Replace("[/MODELFEATURES]", modelFeature);
                result = result.Replace("[/MODELPRICE]", modelPrice);
                result = result.Replace("[/STOREURL]", storeUrl);
                result = result.Replace("[/CONTACTPHONE]", contactPhone);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Can not get email template of product referral", "", "", "", ex);
            }

            // Prepare to send mail
            EMailReponse response = sendMail(emailTo, emailFrom, "Advantech eStore", fromName + " would like to share a product with you from buy.advantech.com", result, store.storeID);

            return response;
        }

        public EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "")
        {
            EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId);
            EMailReponse response = mail.sendMailNow();
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="minisite"></param>
        /// <returns></returns>
        protected string getCurrStoreUrl(POCOS.Store store, POCOS.MiniSite minisite = null)
        {
            if (minisite != null && !string.IsNullOrEmpty(minisite.StoreURL))
                return minisite.StoreURL;

            return (string.IsNullOrEmpty(store.StoreURL)) ? "#" : store.StoreURL;
        }
    }
}