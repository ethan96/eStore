<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="livepersonAJP.ascx.cs"
    Inherits="eStore.UI.Modules.livepersonAJP" %>
<div id="ajplivechat">
    <asp:ImageMap ID="imgBigPic" runat="server" ClientIDMode="Static" ImageUrl="~/images/AJP/livechat/ajpBigPic.gif">
        <asp:RectangleHotSpot Left="144" Top="44" Right="185" Bottom="61" NavigateUrl="../ContactUS.aspx?tabs=general-inquiries"
            HotSpotMode="Navigate" />
        <asp:RectangleHotSpot Left="71" Top="68" Right="111" Bottom="82"/>
        <asp:RectangleHotSpot Left="52" Top="87" Right="139" Bottom="111" NavigateUrl="http://www.advantech.co.jp/news/mail/120201contact/index.htm#livechat"
            HotSpotMode="Navigate" Target="_blank" />
    </asp:ImageMap>
    <asp:ImageMap ID="imgSmallPic" runat="server" ClientIDMode="Static" ImageUrl="~/images/AJP/livechat/ajpSmallPic.gif">
        <asp:RectangleHotSpot Bottom="55" Left="125" Right="165" Top="43" NavigateUrl="../ContactUS.aspx?tabs=general-inquiries"
            HotSpotMode="Navigate" />
        <asp:RectangleHotSpot Bottom="73" Left="65" Right="110" Top="60" />
        <asp:RectangleHotSpot Bottom="100" Left="50" Right="145" Top="80" NavigateUrl="http://www.advantech.co.jp/news/mail/120201contact/index.htm#livechat"
            HotSpotMode="Navigate" Target="_blank" />
    </asp:ImageMap>
</div>
<script type="text/javascript" language="javascript">
$("#ajplivechat map area:eq(1)").click(function()  {
        if (typeof (_wmx) != "undefined") {
            var wmx_model = GetMetaContent("wmx_model");
            BtnTrack(wmx_model, "4", "A");
        }
        <%=string.Format("lpButtonCTTUrl = 'https://server.iad.liveperson.net/hc/68676965/?cmd=file&file=visitorWantsToChat&site=68676965&SESSIONVAR!skill=&imageUrl=https://buy.advantech.com/images/{0}/livechat/&referrer='+escape(document.location); lpButtonCTTUrl = (typeof(lpAppendVisitorCookies) != 'undefined' ? lpAppendVisitorCookies(lpButtonCTTUrl) : lpButtonCTTUrl); window.open(lpButtonCTTUrl,'chat68676965','width=475,height=400,resizable=yes');return false;", eStore.Presentation.eStoreContext.Current.Store.storeID) %>
    });
    $("#ajplivechat map area:eq(0)").click(function()  {
        if (typeof (_wmx) != "undefined") {
            var wmx_model = GetMetaContent("wmx_model");
            BtnTrack(wmx_model, "4", "B");
        }
     });
</script>
