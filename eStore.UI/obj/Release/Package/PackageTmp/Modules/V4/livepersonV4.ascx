<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="livepersonV4.ascx.cs"
    Inherits="eStore.UI.Modules.V4.livepersonV4" %>
<%@ Register Src="../CallMeNow.ascx" TagName="CallMeNow" TagPrefix="uc1" %>
<div class="eStore_chatBox">
    <div class="chatboxController">
        x</div>
    <asp:Image ID="Image1" runat="server" CssClass="link" />
    <div class="eStore_chatBox_txt">
        <div class="float-left">
            <asp:Literal ID="lContactMobile" runat="server" EnableViewState="false"></asp:Literal>
            <ul>
                <% if (this.ShowLivechat == true)
                   { %>
                <li class="livechat">
                    <asp:HyperLink ID="hl_LiveChat" CssClass="hl_LiveChat" runat="server"></asp:HyperLink></li>
                <% }%>
                <% if (this.ShowCallBack == true)
                   { %>
                <li class="callback">
                    <asp:HyperLink ID="hl_RequestCallBack" runat="server"></asp:HyperLink></li>
                <% }%>
                <% if (this.ShowEmailUs == true)
                   { %>
                <li class="emailus">
                    <asp:HyperLink ID="hl_EmailUs" runat="server"></asp:HyperLink></li>
                <%} %>
                <asp:Literal ID="lt_EmailUsAddition" runat="server" EnableViewState="false"></asp:Literal>
            </ul>
        </div>
        <div class="float-right">
            <img src="<%=esUtilities.CommonHelper.GetStoreLocation() %>App_Themes/<%=eStore.Presentation.eStoreContext.Current.Store.storeID %>/chat_bg_big.jpg"
                alt="Contact Advantech" />
        </div>
        <div style="text-align: left; font-size: 10px; padding-top: 110px;">
            <asp:Literal ID="lPrivacyPolicy" runat="server" EnableViewState="false"></asp:Literal>
        </div>
    </div>
</div>
<asp:PlaceHolder ID="phCallMeNow" runat="server"></asp:PlaceHolder>
