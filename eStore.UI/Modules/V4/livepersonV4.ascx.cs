using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class livepersonV4 :Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = Presentation.eStoreContext.Current.getBooleanSetting("EnableLiveperson", true);
            if (Presentation.eStoreContext.Current.Store.storeID == "ALA")
            {
                var callmenow = Page.LoadControl("~/Modules/CallMeNow.ascx") as CallMeNow;
                callmenow.EnableState = true;
                phCallMeNow.Controls.Add(callmenow);
            }
        }

        private bool showcallback = true;
        public bool ShowCallBack
        {
            set
            {
                this.showcallback = value;
            }
            get
            {
                return this.showcallback;
            }
        }
        private bool showlivechat = true;
        public bool ShowLivechat
        {
            set
            {
                this.showlivechat = value;
            }
            get
            {
                return this.showlivechat;
            }
        }
        private bool showemailus = true;
        public bool ShowEmailUs
        {
            set
            {
                this.showemailus = value;
            }
            get
            {
                return this.showemailus;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {

                POCOS.Address storeAddress = Presentation.eStoreContext.Current.CurrentAddress;
                Image1.ImageUrl = string.Format("~/App_Themes/{0}/contact_tab.gif", eStore.Presentation.eStoreContext.Current.Store.storeID);
                Image1.AlternateText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Contact_Us);
                if (storeAddress != null)
                {
                    string contactsl = eStore.Presentation.eStoreContext.Current.getBooleanSetting("ShowTel", true)
                        ? "<h3>{0}</h3><h4>{1}</h4>" : "<h4></h4>";
                    lContactMobile.Text = string.Format(contactsl
                                     , string.IsNullOrEmpty(storeAddress.Tel) ? string.Empty : storeAddress.Tel
                                     , string.IsNullOrEmpty(storeAddress.ServiceTime) ? string.Empty : storeAddress.ServiceTime);
                    lContactMobile.Visible = true;

                    hl_RequestCallBack.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Call_Me_Now);
                    hl_LiveChat.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Live_Chat);
                    hl_EmailUs.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Email_Advantech_eStore);

                    if (Presentation.eStoreContext.Current.Store.storeID == "ABR")
                    {
                        hl_RequestCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                    }

                    if (Presentation.eStoreContext.Current.Store.storeID == "ACN")
                    {
                        hl_LiveChat.NavigateUrl = "javascript:showQQAPI();";
                    }

                    if (Presentation.eStoreContext.Current.getBooleanSetting("EnableLiveEngage", false))
                    {
                        System.Text.StringBuilder sbLPScripts = new System.Text.StringBuilder();

                        sbLPScripts.AppendLine(@"window.lpTag=window.lpTag||{},""undefined""==typeof window.lpTag._tagCount?(window.lpTag={site:'30800048'||"""",section:lpTag.section||"""",tagletSection:lpTag.tagletSection||null,autoStart:lpTag.autoStart!==!1,ovr:lpTag.ovr||{},_v:""1.8.0"",_tagCount:1,protocol:""https:"",events:{bind:function(t,e,i){lpTag.defer(function(){lpTag.events.bind(t,e,i)},0)},trigger:function(t,e,i){lpTag.defer(function(){lpTag.events.trigger(t,e,i)},1)}},defer:function(t,e){0==e?(this._defB=this._defB||[],this._defB.push(t)):1==e?(this._defT=this._defT||[],this._defT.push(t)):(this._defL=this._defL||[],this._defL.push(t))},load:function(t,e,i){var n=this;setTimeout(function(){n._load(t,e,i)},0)},_load:function(t,e,i){var n=t;t||(n=this.protocol+""//""+(this.ovr&&this.ovr.domain?this.ovr.domain:""lptag.liveperson.net"")+""/tag/tag.js?site=""+this.site);var a=document.createElement(""script"");a.setAttribute(""charset"",e?e:""UTF-8""),i&&a.setAttribute(""id"",i),a.setAttribute(""src"",n),document.getElementsByTagName(""head"").item(0).appendChild(a)},init:function(){this._timing=this._timing||{},this._timing.start=(new Date).getTime();var t=this;window.attachEvent?window.attachEvent(""onload"",function(){t._domReady(""domReady"")}):(window.addEventListener(""DOMContentLoaded"",function(){t._domReady(""contReady"")},!1),window.addEventListener(""load"",function(){t._domReady(""domReady"")},!1)),""undefined""==typeof window._lptStop&&this.load()},start:function(){this.autoStart=!0},_domReady:function(t){this.isDom||(this.isDom=!0,this.events.trigger(""LPT"",""DOM_READY"",{t:t})),this._timing[t]=(new Date).getTime()},vars:lpTag.vars||[],dbs:lpTag.dbs||[],ctn:lpTag.ctn||[],sdes:lpTag.sdes||[],hooks:lpTag.hooks||[],ev:lpTag.ev||[]},lpTag.init()):window.lpTag._tagCount+=1;");

                        //add mrktInfo
                        sbLPScripts.AppendLine(@"lpTag.sdes = lpTag.sdes || [];
lpTag.sdes.push({
""type"": ""mrktInfo"",
""info"": {
""channel"": ""0"",");
                        sbLPScripts.AppendLine($@"""campaignId"": ""{(eStore.Presentation.eStoreContext.Current.getStringSetting("LiveEngageLOB", "Advantech"))}""");

sbLPScripts.AppendLine(@"}
});");
                        //add sections
                        sbLPScripts.AppendLine($@"lpTag.section = [""eStore"", ""{eStore.Presentation.eStoreContext.Current.Store.storeID}""]; ");

                        System.Web.UI.HtmlControls.HtmlGenericControl con = new System.Web.UI.HtmlControls.HtmlGenericControl();
                        con.TagName = "script";
                        con.Attributes.Add("type", "text/javascript");
                        con.InnerHtml = sbLPScripts.ToString();
                        Page.Header.Controls.AddAt(0, con);
                        //BindScript("scripts", "LPScripts", sbLPScripts.ToString());

                        this.Visible = false;
                        return;
                    }
                    else if (Presentation.eStoreContext.Current.Store.storeID == "AKR")
                    {
                        hl_LiveChat.Attributes.Add("onclick", string.Format("createUnicaActivity('livechat', '', '{0}');lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SV!skill=Asia%20-%20Korea&LEAppKey=f907f2d9acd64b7f8c00b83bed3c2822&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", hl_LiveChat.NavigateUrl));

                        System.Text.StringBuilder sbLPScripts = new System.Text.StringBuilder();

                        sbLPScripts.AppendLine(@"var lpMTagConfig = { 'lpServer': ""server.iad.liveperson.net"", 'lpNumber': ""68676965"", 'lpProtocol': (document.location.toString().indexOf('https:') == 0) ? 'https' : 'http' }; function lpAddMonitorTag(src) { if (typeof (src) == 'undefined' || typeof (src) == 'object') { src = lpMTagConfig.lpMTagSrc ? lpMTagConfig.lpMTagSrc : '/hcp/html/mTag.js'; } if (src.indexOf('http') != 0) { src = lpMTagConfig.lpProtocol + ""://"" + lpMTagConfig.lpServer + src + '?site=' + lpMTagConfig.lpNumber; } else { if (src.indexOf('site=') < 0) { if (src.indexOf('?') < 0) src = src + '?'; else src = src + '&'; src = src + 'site=' + lpMTagConfig.lpNumber; } }; var s = document.createElement('script'); s.setAttribute('type', 'text/javascript'); s.setAttribute('charset', 'iso-8859-1'); s.setAttribute('src', src); document.getElementsByTagName('head').item(0).appendChild(s); } if (window.attachEvent) window.attachEvent('onload', lpAddMonitorTag); else window.addEventListener(""load"", lpAddMonitorTag, false);");

                        BindScript("scripts", "LPScripts", sbLPScripts.ToString());
                    }
                    else
                    {
                        hl_LiveChat.NavigateUrl = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&byhref=1&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/' target='chat68676965' ", Presentation.eStoreContext.Current.Store.storeID));
                        hl_LiveChat.Attributes.Add("onclick", string.Format("createUnicaActivity('livechat', '', '{1}');lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", Presentation.eStoreContext.Current.Store.storeID, hl_LiveChat.NavigateUrl));
                        System.Text.StringBuilder sbLPScripts = new System.Text.StringBuilder();

                        sbLPScripts.AppendLine(@"var lpMTagConfig = { 'lpServer': ""server.iad.liveperson.net"", 'lpNumber': ""68676965"", 'lpProtocol': (document.location.toString().indexOf('https:') == 0) ? 'https' : 'http' }; function lpAddMonitorTag(src) { if (typeof (src) == 'undefined' || typeof (src) == 'object') { src = lpMTagConfig.lpMTagSrc ? lpMTagConfig.lpMTagSrc : '/hcp/html/mTag.js'; } if (src.indexOf('http') != 0) { src = lpMTagConfig.lpProtocol + ""://"" + lpMTagConfig.lpServer + src + '?site=' + lpMTagConfig.lpNumber; } else { if (src.indexOf('site=') < 0) { if (src.indexOf('?') < 0) src = src + '?'; else src = src + '&'; src = src + 'site=' + lpMTagConfig.lpNumber; } }; var s = document.createElement('script'); s.setAttribute('type', 'text/javascript'); s.setAttribute('charset', 'iso-8859-1'); s.setAttribute('src', src); document.getElementsByTagName('head').item(0).appendChild(s); } if (window.attachEvent) window.attachEvent('onload', lpAddMonitorTag); else window.addEventListener(""load"", lpAddMonitorTag, false);");

                        BindScript("scripts", "LPScripts", sbLPScripts.ToString());

                    }
                    
                    if (Presentation.eStoreContext.Current.Store.storeID == "AUS" ||
                        Presentation.eStoreContext.Current.Store.storeID == "ASC" ||
                        Presentation.eStoreContext.Current.Store.storeID == "ANC" ||
                        Presentation.eStoreContext.Current.Store.storeID == "AEU")
                    {
                        if (Presentation.eStoreContext.Current.Store.storeID == "AEU" && (
                            Presentation.eStoreContext.Current.BusinessGroup != POCOS.Store.BusinessGroup.ECG ||
                            Presentation.eStoreContext.Current.BusinessGroup != POCOS.Store.BusinessGroup.ACG))
                            hl_RequestCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                        else
                        {
                            string skill = DateTime.Now.Hour < 5 ? "Voice PBX AEU" : DateTime.Now.Hour <= 8 ? "Voice PBX AAC" : "Voice PBX";// DateTime.Now.Hour >= 5 && DateTime.Now.Hour <= 8 ? "Voice PBX AAC" : "Voice PBX";
                            hl_RequestCallBack.NavigateUrl = HttpUtility.HtmlEncode(string.Format("https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1&ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&byhref=1&imageUrl=https://buy.advantech.com/images/{0}/livetalk/' target='call68676965'", Presentation.eStoreContext.Current.Store.storeID, skill));
                            hl_RequestCallBack.Attributes.Add("onclick", string.Format("createUnicaActivity('callmenow', '', '{2}');javascript:window.open('https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToTalk&site=68676965&SESSIONVAR!skill={1}&onlineURL=https://server.iad.liveperson.net/hcp/voice/forms/precall.asp?site=68676965%26identifier=1%26ResponseURL=https://server.iad.liveperson.net/hcp/voice/forms/callStatus.asp&imageUrl=https://buy.advantech.com/images/{0}/livetalk/&referrer='+escape(document.location),'call68676965','width=475,height=420');return false;", Presentation.eStoreContext.Current.Store.storeID, skill, hl_RequestCallBack.NavigateUrl));
                            hl_RequestCallBack.Attributes.Add("target", "call68676965");
                        }
                        this.ShowCallBack = true;
                        this.ShowEmailUs = false;
                    }
                    else if (Presentation.eStoreContext.Current.Store.storeID == "ATW" ||
                               Presentation.eStoreContext.Current.Store.storeID == "SAP" ||
                               Presentation.eStoreContext.Current.Store.storeID == "ACN" ||
                               Presentation.eStoreContext.Current.Store.storeID == "AKR" ||
                               Presentation.eStoreContext.Current.Store.storeID == "AJP" ||
                               Presentation.eStoreContext.Current.Store.storeID == "ABB")
                    {
                        hl_RequestCallBack.NavigateUrl = ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries");
                        this.ShowEmailUs = false;

                        string policy = Presentation.eStoreContext.Current.getStringSetting("Private_Policy");
                        if (!string.IsNullOrEmpty(policy))
                            lPrivacyPolicy.Text = policy;

                        if (Presentation.eStoreContext.Current.Store.storeID == "SAP")
                        {
                            Dictionary<string, string> dicCountryparameter = Presentation.eStoreContext.Current.CurrentCountry.getSettings(Presentation.eStoreContext.Current.Store.profile);
                            if (dicCountryparameter != null && dicCountryparameter.ContainsKey("Call_Me_Now"))
                            {
                                hl_RequestCallBack.Width = Unit.Pixel(150);
                                hl_RequestCallBack.Text = dicCountryparameter["Call_Me_Now"];
                            }

                            if (dicCountryparameter != null && dicCountryparameter.ContainsKey("Email_Us"))
                                lt_EmailUsAddition.Text = dicCountryparameter["Email_Us"];
                        }
                    }
                    else if (Presentation.eStoreContext.Current.Store.storeID == "ALA")
                    {
                        hl_RequestCallBack.Attributes.Add("class", "popcallmenow");
                    }
                    else
                    {
                        hl_RequestCallBack.NavigateUrl = Request.RawUrl + (Request.RawUrl.EndsWith("#") ? string.Empty : "#");
                    }

                    if (Presentation.eStoreContext.Current.Store.storeID == "AIN")
                    {
                        this.ShowCallBack = false;
                    }

                    var cmn = hl_RequestCallBack.Attributes["onclick"];
                    if (cmn == null && !string.IsNullOrEmpty(hl_RequestCallBack.NavigateUrl))
                        hl_RequestCallBack.Attributes.Add("onclick", string.Format("createUnicaActivity('callmenow', '', '{0}')", hl_RequestCallBack.NavigateUrl));

                    var lc = hl_LiveChat.Attributes["onclick"];
                    if (lc == null && !string.IsNullOrEmpty(hl_LiveChat.NavigateUrl))
                        hl_LiveChat.Attributes.Add("onclick", string.Format("createUnicaActivity('livechat', '', '{0}')", hl_LiveChat.NavigateUrl));
                }
                else
                {
                    lContactMobile.Text = string.Empty;
                    lContactMobile.Visible = false;
                }
                hl_EmailUs.NavigateUrl = ResolveUrl("~/ContactUS.aspx");
            }
            catch (Exception)
            {

            }
            base.OnPreRender(e);
        }
    }
}