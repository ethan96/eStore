using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class liveperson2011 : livepersonBase
    {
        public override string livepersonStyle
        {
            get
            {
                if (UserLargerImage)
                    return "liveperson";
                else
                    return "livepersonSmall";
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            pLiveperson.Visible = true;
            hlContactUS.Visible = false;
            _lpChatBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&byhref=1&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/' target='chat68676965' ", Presentation.eStoreContext.Current.Store.storeID));
            _lpChatBtn.Attributes.Add("onclick", string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID));
            livechat.Src = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=repstate&site=68676965&channel=web&&ver=1&imageUrl=https://buy.advantech.com/images/{0}/livechat/&skill=", Presentation.eStoreContext.Current.Store.storeID));
            laskanexpert.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Ask_an_Expert);
            livechat.Alt = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
            liveCall.Alt = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
            if (Presentation.eStoreContext.Current.Store.storeID == "AUS" ||
                Presentation.eStoreContext.Current.Store.storeID == "ASC" ||
                Presentation.eStoreContext.Current.Store.storeID == "ANC")
            {
                _lpLiveCallBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill=Voice PBX&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1&ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&byhref=1&imageUrl=https://buy.advantech.com/images/{0}/livetalk/' target='call68676965'", Presentation.eStoreContext.Current.Store.storeID));
                _lpLiveCallBtn.Attributes.Add("onclick", string.Format("javascript:window.open('https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill=Voice PBX&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1%26ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&imageUrl=https://buy.advantech.com/images/{0}/livetalk/&referrer='+escape(document.location),'call68676965','width=475,height=420');return false;", Presentation.eStoreContext.Current.Store.storeID));
                liveCall.Src = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=repstate&site=68676965&skill=Voice PBX&channel=voice&&ver=1&imageUrl=https://buy.advantech.com/images/{0}/livetalk/", Presentation.eStoreContext.Current.Store.storeID));
                liveCall.Attributes.Add("name", "hcIcon");
                this.CallMeNow1.Visible = false;
            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "ATW"
                || Presentation.eStoreContext.Current.Store.storeID == "SAP"
                 || Presentation.eStoreContext.Current.Store.storeID == "AEU")
            {
                _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                liveCall.Src = string.Format("/images/{0}/livetalk/voice_reponline.gif", Presentation.eStoreContext.Current.Store.storeID);
                this.CallMeNow1.Visible = false;
            }
            else
            {
                _lpLiveCallBtn.Attributes.Add("class", "popcallmenow");
                _lpLiveCallBtn.HRef = Request.RawUrl + (Request.RawUrl.EndsWith("#") ? string.Empty : "#");
                liveCall.Src = string.Format("/images/{0}/livetalk/voice_reponline.gif", Presentation.eStoreContext.Current.Store.storeID);
                this.CallMeNow1.Visible = true;
            }

        }
    }
}