using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class liveperson2012 : livepersonBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            laskanexpert.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Ask_an_Expert);
            _lpChatBtn.Title = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
            _lpLiveCallBtn.Title = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
            _lpChatBtn.InnerText  = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
            _lpLiveCallBtn.InnerText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
            System.Text.StringBuilder sbLPScripts = new System.Text.StringBuilder();

            sbLPScripts.AppendLine("if(typeof window.lpTag==='undefined'){window.lpTag={site:'68676965',_v:'1.3',protocol:location.protocol,events:{bind:function(app,ev,fn){lpTag.defer(function(){lpTag.events.bind(app,ev,fn)},0)},trigger:function(app,ev,json){lpTag.defer(function(){lpTag.events.trigger(app,ev,json)},1)}},defer:function(fn,fnType){if(fnType==0){this._defB=this._defB||[];this._defB.push(fn)}else if(fnType==1){this._defT=this._defT||[];this._defT.push(fn)}else{this._defL=this._defL||[];this._defL.push(fn)}},load:function(src,chr,id){var t=this;setTimeout(function(){t._load(src,chr,id)},0)},_load:function(src,chr,id){var url=src;if(!src){url=this.protocol+'//'+((this.ovr&&this.ovr.domain)?this.ovr.domain:'lptag.liveperson.net')+'/tag/tag.js?site='+this.site}var s=document.createElement('script');s.setAttribute('charset',chr?chr:'UTF-8');if(id){s.setAttribute('id',id)}s.setAttribute('src',url);document.getElementsByTagName('head').item(0).appendChild(s)},init:function(){this._timing=this._timing||{};this._timing.start=(new Date()).getTime();var that=this;if(window.attachEvent){window.attachEvent('onload',function(){that._domReady('domReady')})}else{window.addEventListener('DOMContentLoaded',function(){that._domReady('contReady')},false);window.addEventListener('load',function(){that._domReady('domReady')},false)}if(typeof(window._lptStop)=='undefined'){this.load()}},_domReady:function(n){if(!this.isDom){this.isDom=true;this.events.trigger('LPT','DOM_READY',{t:n})}this._timing[n]=(new Date()).getTime()}};lpTag.init()}");

            BindScript("scripts", "LPScripts", sbLPScripts.ToString());
            if (Presentation.eStoreContext.Current.Store.storeID == "ABR")
            {
                _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
            }
            _lpChatBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&byhref=1&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/' target='chat68676965' ", Presentation.eStoreContext.Current.Store.storeID));
            _lpChatBtn.Attributes.Add("onclick", string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID));


            if (Presentation.eStoreContext.Current.Store.storeID == "AUS" ||
            Presentation.eStoreContext.Current.Store.storeID == "ASC" ||
            Presentation.eStoreContext.Current.Store.storeID == "ANC" ||
            Presentation.eStoreContext.Current.Store.storeID == "AEU")
            {
                this.CallMeNow1.Visible = false;
                if (Presentation.eStoreContext.Current.Store.storeID == "AEU" &&(
                    Presentation.eStoreContext.Current.BusinessGroup != POCOS.Store.BusinessGroup.ECG
                    ||Presentation.eStoreContext.Current.BusinessGroup != POCOS.Store.BusinessGroup.ACG )
                  )
                    _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                else
                {
                    string skill = DateTime.Now.Hour < 5 ? "Voice PBX AEU" : DateTime.Now.Hour <= 8 ? "Voice PBX AAC" : "Voice PBX";// DateTime.Now.Hour >= 5 && DateTime.Now.Hour <= 8 ? "Voice PBX AAC" : "Voice PBX";
                    _lpLiveCallBtn.HRef = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1&ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&byhref=1&imageUrl=https://buy.advantech.com/images/{0}/livetalk/' target='call68676965'", Presentation.eStoreContext.Current.Store.storeID, skill));
                    _lpLiveCallBtn.Attributes.Add("onclick", string.Format("javascript:window.open('https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1%26ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&imageUrl=https://buy.advantech.com/images/{0}/livetalk/&referrer='+escape(document.location),'call68676965','width=475,height=420');return false;", Presentation.eStoreContext.Current.Store.storeID, skill));

                }
            }
            else if (Presentation.eStoreContext.Current.Store.storeID == "ATW"
                || Presentation.eStoreContext.Current.Store.storeID == "SAP"
                || Presentation.eStoreContext.Current.Store.storeID == "ACN"
                || Presentation.eStoreContext.Current.Store.storeID == "AKR"
                || Presentation.eStoreContext.Current.Store.storeID == "AJP")
            {
                _lpLiveCallBtn.HRef = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                this.CallMeNow1.Visible = false;
                if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
                {
                    ltCNQQ.Text = "<!-- WPA Button Begin --><script type=\"text/javascript\" src=\"http://static.b.qq.com/account/bizqq/js/wpa.js?wty=1&type=4&kfuin=8008100345&ws=http%3A%2F%2Fbuy.advantech.com.cn%2F&btn1=QQ%E5%9C%A8%E7%BA%BF%E5%92%A8%E8%AF%A2&aty=0&a=&key=%0Ek%0D%3CTa%00%3D%041%0EhWgP%3AW3Q%60%04%3DP%3E%074%00nR4P6%5C3%02%3B%072%01%3D\"></script><!-- WPA Button END -->";

                    PNAskanExpert.Visible = false;
                }
            }
            else
            {
                _lpLiveCallBtn.Attributes.Add("class", "popcallmenow");
                _lpLiveCallBtn.HRef = Request.RawUrl + (Request.RawUrl.EndsWith("#") ? string.Empty : "#");
                this.CallMeNow1.Visible = true;
                this.CallMeNow1.EnableState = true;
            }            
        }
    }

}