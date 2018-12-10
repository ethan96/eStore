<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="QuantityDiscountRequest.aspx.cs" Inherits="eStore.UI.Product.QuantityDiscountRequest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server" />
</asp:Content>
