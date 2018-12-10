<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Print.Master" AutoEventWireup="true" CodeBehind="printorder.aspx.cs" Inherits="eStore.UI.Cart.printorder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>  
    <eStore:OrderInvoiceDetail ID="OrderInvoiceDetail1" runat="server" />

</asp:Content>
