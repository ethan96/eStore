using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class liveperson2013 : livepersonBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            laskanexpert.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Ask_an_Expert);
            _lpChatBtn.Title = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
            _lpLiveCallBtn.Title = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
            _lpChatBtn.InnerText  = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
            _lpLiveCallBtn.InnerText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
            if (Presentation.eStoreContext.Current.Store.storeID == "ABR")
            {
                _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                _lpChatBtn.Attributes.Add("class", "blackA");
            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "AUS")
            {
                _lpChatBtn.HRef = HttpUtility.HtmlEncode(string.Format("http://c.velaro.com/visitor/requestchat.aspx?siteid=10770&showwhen=inqueue"));
                _lpChatBtn.Attributes.Add("onclick", string.Format("javascript:this.newWindow = window.open('http://c.velaro.com/visitor/requestchat.aspx?siteid=10770&showwhen=inqueue', 'LiveChatSoftware', 'toolbar=no,location=no,directories=no,menubar=no,status=no,scrollbars=no,resizable=yes,replace=no,width=400,height=455');this.newWindow.focus();this.newWindow.opener=window;return false;"));

                _lpLiveCallBtn.HRef = HttpUtility.HtmlEncode(string.Format("http://c.velaro.com/visitor/requestchat.aspx?siteid=10770&showwhen=inqueue&ctc=yes"));
                _lpLiveCallBtn.Attributes.Add("onclick", string.Format("javascript:this.newWindow = window.open('http://c.velaro.com/visitor/requestchat.aspx?siteid=10770&showwhen=inqueue&ctc=yes', 'ClickToCall', 'toolbar=no,location=no,directories=no,menubar=no,status=no,scrollbars=no,resizable=yes,replace=no,width=400,height=455');this.newWindow.focus();this.newWindow.opener=window;return false;"));
                
                this.CallMeNow1.Visible = false;
            }
            else
            {
                _lpChatBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&byhref=1&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/' target='chat68676965' ", Presentation.eStoreContext.Current.Store.storeID));
                _lpChatBtn.Attributes.Add("onclick", string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID));


                if (Presentation.eStoreContext.Current.Store.storeID == "ASC" ||
                Presentation.eStoreContext.Current.Store.storeID == "ANC")
                {
                    string skill = DateTime.Now.Hour >= 5 && DateTime.Now.Hour <= 8 ? "Voice PBX AAC" : "Voice PBX";
                    _lpLiveCallBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1&ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&byhref=1&imageUrl=https://buy.advantech.com/images/{0}/livetalk/' target='call68676965'", Presentation.eStoreContext.Current.Store.storeID, skill));
                    _lpLiveCallBtn.Attributes.Add("onclick", string.Format("javascript:window.open('https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1%26ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&imageUrl=https://buy.advantech.com/images/{0}/livetalk/&referrer='+escape(document.location),'call68676965','width=475,height=420');return false;", Presentation.eStoreContext.Current.Store.storeID, skill));
                    this.CallMeNow1.Visible = false;
                }
                else if (Presentation.eStoreContext.Current.Store.storeID == "ATW"
                    || Presentation.eStoreContext.Current.Store.storeID == "SAP"
                    || Presentation.eStoreContext.Current.Store.storeID == "AEU"
                    || Presentation.eStoreContext.Current.Store.storeID == "ACN"
                    || Presentation.eStoreContext.Current.Store.storeID == "AKR")
                {
                    _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                    this.CallMeNow1.Visible = false;
                    if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
                    {
                        ltCNQQ.Text = "<!-- WPA Button Begin --><script type=\"text/javascript\" src=\"http://static.b.qq.com/account/bizqq/js/wpa.js?wty=1&type=4&kfuin=8008100345&ws=http%3A%2F%2Fbuy.advantech.com.cn%2F&btn1=QQ%E5%9C%A8%E7%BA%BF%E5%92%A8%E8%AF%A2&aty=0&a=&key=%0Ek%0D%3CTa%00%3D%041%0EhWgP%3AW3Q%60%04%3DP%3E%074%00nR4P6%5C3%02%3B%072%01%3D\"></script><!-- WPA Button END -->";
                    }
                }
                else
                {
                    _lpLiveCallBtn.Attributes.Add("class", "popcallmenow");
                    _lpLiveCallBtn.HRef = Request.RawUrl + (Request.RawUrl.EndsWith("#") ? string.Empty : "#");
                    this.CallMeNow1.Visible = true;
                }
            }
            
        }
    }

}