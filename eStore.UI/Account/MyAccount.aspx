<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" 
    CodeBehind="MyAccount.aspx.cs" Inherits="eStore.UI.Account.MyAccount" %>
    <%@ Register Src="~/Modules/V4/AddressBook.ascx" TagName="AddressBook" TagPrefix="eStore" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/account")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="eStore_breadcrumb eStore_block980">
    	<a href="<%= ResolveUrl("~/") %>"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home) %></a>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account) %>
    </div>
    <div class="eStore_account_content">
        <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account) %></h2>
        <div class="eStore_account_link">
            <div class="eStore_account_linkName"><%=WelcomeName %></div>
            <asp:LinkButton ID="lb_MySetting" runat="server" OnClick="lb_MySetting_Click"></asp:LinkButton>
            <asp:LinkButton ID="lb_MyOrder" runat="server" OnClick="lb_MyOrder_Click"></asp:LinkButton>
            <asp:LinkButton ID="lb_MyQuote" runat="server" OnClick="lb_MyQuote_Click"></asp:LinkButton>
            <asp:LinkButton ID="lb_MyReward" runat="server" OnClick="lb_MyReward_Click"></asp:LinkButton>
            <asp:Literal ID="ltiAblePoint" runat="server"></asp:Literal>
        </div>
        <eStore.V4:MyOrder ID="MyOrder1" runat="server" />
        <eStore.V4:MyQuote ID="MyQuote1" runat="server" />
        <eStore:AddressBook ID="AddressBook1" runat="server" />
        <eStore.V4:MyReward ID="MyReward1" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
