using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.Utilities;
using esUtilities;

namespace eStore.UI.Modules
{
    public partial class ContactUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void CreateContactInformation()
        {
            Widget ContactInformationWidget = LoadControl("~/Modules/Widget.ascx") as Widget;
            string ContactInformation = Presentation.eStoreContext.Current.getStringSetting("ContactInformation");
            if (!string.IsNullOrEmpty(ContactInformation))
                ContactInformationWidget.WidgetName = ContactInformation;
            else
                ContactInformationWidget.WidgetName = "ContactInformation";
            phContactInformationWidget.Controls.Add(ContactInformationWidget);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateContactInformation();
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (UVerification1.VerificationUser())
            {
                if (Request.Form.Get("inquiryType") == "generalInquiries")
                {
                    POCOS.UserRequest contactus = T_CustomerProfile.getUserRequest(eStore.POCOS.UserRequest.ReqType.GeneralInquiries);
                    contactus.Comment = inComment.Text.Trim();
                    contactus.save();

                    eStore.BusinessModules.EMailDiscountRequest request = new BusinessModules.EMailDiscountRequest();
                    EMailReponse response = request.getDiscountRequestEmailTemplate(Presentation.eStoreContext.Current.Store, contactus
                        , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_General_Inquiries_MailObj)
                        , Presentation.eStoreContext.Current.User, eStoreContext.Current.CurrentLanguage
                        , Presentation.eStoreContext.Current.MiniSite);
                    if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Failed_at_sending_confirmation_message) + response.ErrCode.ToString());
                    else
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                        {
                            Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), contactus);
                        }

                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Your_request_is_sent));
                    }
                }

                else if (btn_Submit.Attributes["select"] == "technicalSupport")
                {
                    POCOS.UserRequest contactus = T_CustomerProfile.getUserRequest(eStore.POCOS.UserRequest.ReqType.TechnicalSupport);
                    contactus.Comment = inComment.Text.Trim();
                    contactus.SoftwareVersion = string.IsNullOrEmpty(Request["inSoftwareVersion"]) ? "" : Request["inSoftwareVersion"].Trim();
                    contactus.ProductCategory = string.IsNullOrEmpty(Request["inProductCategory"]) ? "" : Request["inProductCategory"].Trim();
                    contactus.ProductModelNO = string.IsNullOrEmpty(Request["inModelNo"]) ? "" : Request["inModelNo"].Trim();
                    contactus.PurchaseDate = string.IsNullOrEmpty(Request["inPurchaseDate"]) ? "" : Request["inPurchaseDate"].Trim();
                    contactus.save();

                    eStore.BusinessModules.EMailDiscountRequest request = new BusinessModules.EMailDiscountRequest();
                    EMailReponse response = request.getDiscountRequestEmailTemplate(Presentation.eStoreContext.Current.Store, contactus,
                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Technical_Support_MailObj), Presentation.eStoreContext.Current.User, eStoreContext.Current.CurrentLanguage, eStoreContext.Current.MiniSite);
                    if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Failed_at_sending_confirmation_message) + response.ErrCode.ToString());
                    else
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                        {
                            Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), contactus);
                        }

                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Your_request_is_sent));
                    }
                }

                else if (btn_Submit.Attributes["select"] == "technicalSupport")
                {
                    POCOS.UserRequest contactus = T_CustomerProfile.getUserRequest(eStore.POCOS.UserRequest.ReqType.Sales);
                    contactus.Comment = inComment.Text.Trim();
                    contactus.ProductCategory = string.IsNullOrEmpty(Request["inProductInterest"]) ? "" : Request["inProductInterest"].Trim();
                    contactus.MostAppropriate = string.IsNullOrEmpty(Request["inPick"]) ? "" : Request["inPick"].Trim();
                    contactus.save();

                    eStore.BusinessModules.EMailDiscountRequest request = new BusinessModules.EMailDiscountRequest();
                    EMailReponse response = request.getDiscountRequestEmailTemplate(Presentation.eStoreContext.Current.Store, contactus,
                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Sales_MailObj), Presentation.eStoreContext.Current.User, eStoreContext.Current.CurrentLanguage, eStoreContext.Current.MiniSite);
                    if (response != null && response.ErrCode != EMailReponse.ErrorCode.NoError)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Failed_at_sending_confirmation_message) + response.ErrCode.ToString());
                    }
                    else
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                        {
                            Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), contactus);
                        }

                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Your_request_is_sent));
                    }
                }
            }
        }
    }
}