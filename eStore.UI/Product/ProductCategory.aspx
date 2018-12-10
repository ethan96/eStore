<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ProductCategory.aspx.cs" Inherits="eStore.UI.Product.ProductCategory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:ProductSpec ID="ProductSpec1" Visible="false" runat="server" />
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:ProductCategoryList ID="ProductCategoryList1" runat="server" />
    <div class="clear">
    </div>
    <eStore:ProductList ID="ProductList1" runat="server" />
</asp:Content>
