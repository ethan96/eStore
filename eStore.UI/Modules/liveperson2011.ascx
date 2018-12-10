<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="liveperson2011.ascx.cs"
    ViewStateMode="Disabled" Inherits="eStore.UI.Modules.liveperson2011" %>
<%@ Register Src="CallMeNow.ascx" TagName="CallMeNow" TagPrefix="eStore" %>
<asp:Panel ID="pLiveperson" runat="server">
    <div class="<%=livepersonStyle %>">
        <p>
            <asp:Literal ID="laskanexpert" runat="server" Text=""></asp:Literal></p>
        <a id="_lpChatBtn" runat="server" clientidmode="Static">
            <img id="livechat" runat="server" name='hcIcon' border="0" /></a><br />
        <a id="_lpLiveCallBtn" runat="server" clientidmode="Static">
            <img id="liveCall" runat="server" alt='' border="0" /></a>
    </div>
    <eStore:CallMeNow ID="CallMeNow1" runat="server" />
    <script language='javascript' type="text/javascript">
		//<![CDATA[
        var lpMTagConfig = { 'lpServer': "server.iad.liveperson.net", 'lpNumber': "68676965", 'lpProtocol': "https" };
        function lpAddMonitorTag(src) { if (typeof (src) == 'undefined' || typeof (src) == 'object') { src = lpMTagConfig.lpMTagSrc ? lpMTagConfig.lpMTagSrc : '/hcp/html/mTag.js'; } if (src.indexOf('http') != 0) { src = lpMTagConfig.lpProtocol + "://" + lpMTagConfig.lpServer + src + '?site=' + lpMTagConfig.lpNumber; } else { if (src.indexOf('site=') < 0) { if (src.indexOf('?') < 0) src = src + '?'; else src = src + '&'; src = src + 'site=' + lpMTagConfig.lpNumber; } }; var s = document.createElement('script'); s.setAttribute('type', 'text/javascript'); s.setAttribute('charset', 'iso-8859-1'); s.setAttribute('src', src); document.getElementsByTagName('head').item(0).appendChild(s); } if (window.attachEvent) window.attachEvent('onload', lpAddMonitorTag); else window.addEventListener("load", lpAddMonitorTag, false);

        if (typeof (lpMTagConfig.sessionVar) == "undefined") { lpMTagConfig.sessionVar = new Array(); }
        lpMTagConfig.sessionVar[lpMTagConfig.sessionVar.length] = 'skill=';

        $("#_lpChatBtn").click(function () {
            var wmx_model = document.getElementsByName("wmx_model");
            if (wmx_model != undefined) {
                BtnTrack(wmx_model, "4", "A");
            }
            
        });
        $("#_lpLiveCallBtn").click(function () {
            var wmx_model = document.getElementsByName("wmx_model");
            if (wmx_model != undefined) {
                BtnTrack(wmx_model, "4", "B");
            }
           
        });
        //]]> 
    </script>
</asp:Panel>
<asp:HyperLink ID="hlContactUS" runat="server" Visible="false" />