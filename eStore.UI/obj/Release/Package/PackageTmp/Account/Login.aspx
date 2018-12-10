<%@ Page Title="Log In" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eStore.UI.Account.Login" %>

<%@ Register Src="../Modules/UserLogin.ascx" TagName="UserLogin" TagPrefix="uc2" %>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
    <uc2:UserLogin ID="UserLogin1" runat="server" />
</asp:Content>
