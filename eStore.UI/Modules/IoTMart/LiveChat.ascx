<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LiveChat.ascx.cs" Inherits="eStore.UI.Modules.IoTMart.LiveChat" %>
<div class="slide-out-div" style="display: none" haderimg="<%=esUtilities.CommonHelper.GetStoreLocation(false) %>App_Themes/<%=eStore.Presentation.eStoreContext.Current.Store.storeID %>/<%=eStore.Presentation.eStoreContext.Current.MiniSite.SiteName %>/contact_tab.gif">
    <a class="handle" href="#">
        <asp:Literal ID="ltLiveChatCUS" runat="server"></asp:Literal></a>
    <div class="panel-left">
        <asp:Literal ID="loSiteTimeAndPhone" runat="server"></asp:Literal>
        <ul>
            <li class="callback" id="callback" runat="server" visible="false">
                <asp:HyperLink ID="hyCallBack" runat="server">Request call back</asp:HyperLink></li>
            <li class="livechat">
                <asp:HyperLink ID="hyLiveChart" runat="server">Live chat</asp:HyperLink></li>
            <li class="emailus">
                <asp:HyperLink ID="hyEmailUs" runat="server">Email us</asp:HyperLink><a href="#"></a></li>
        </ul>
    </div>
    <div class="panel-right">
        <img src="<%=esUtilities.CommonHelper.GetStoreLocation(false) %>App_Themes/<%=eStore.Presentation.eStoreContext.Current.Store.storeID %>/contact_photo.jpg"
            alt="Contact Advantech" />
    </div>
    <br class="clear" />
</div>
