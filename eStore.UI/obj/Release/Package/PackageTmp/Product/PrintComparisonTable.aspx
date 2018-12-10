<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Print.Master" AutoEventWireup="true" CodeBehind="PrintComparisonTable.aspx.cs" Inherits="eStore.UI.Product.PrintComparisonTable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <eStore:ProductCompare ID="ProductCompare1" runat="server" showRemoveButton="false" showPrintButton="false" showHeaderButtons="false" />

</asp:Content>
