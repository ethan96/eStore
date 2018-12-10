using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.IoTMart
{
    public partial class LiveChat : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loSiteTimeAndPhone.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_ContactUsPhoneAndTime);
            ltLiveChatCUS.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ContactAdvantech);
            
            if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
            {
                hyCallBack.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_RequestCallBack);
                hyCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyEmailUs.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Emailus);
                hyEmailUs.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyLiveChart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Livechat);
                hyLiveChart.NavigateUrl = "javascript:showQQAPI();";
            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "ATW")
            {
                hyCallBack.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_RequestCallBack);
                hyCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyEmailUs.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Emailus);
                hyEmailUs.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyLiveChart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Livechat);
                hyLiveChart.NavigateUrl = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&byhref=1&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/' target='chat68676965' ", Presentation.eStoreContext.Current.Store.storeID));
                hyLiveChart.Attributes.Add("onclick", string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID));

            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "AKR")
            {
                hyCallBack.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_RequestCallBack);
                hyCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyEmailUs.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Emailus);
                hyEmailUs.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                hyLiveChart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Livechat);
                hyLiveChart.NavigateUrl = HttpUtility.HtmlEncode("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.co.kr/images/AKR/livechat/&referrer=http%3A//buy.advantech.co.kr/");
                hyLiveChart.Attributes.Add("onclick", string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.co.kr/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID));

            }
        }
    }
}