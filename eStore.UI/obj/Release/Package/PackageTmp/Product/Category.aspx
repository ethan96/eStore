<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="Category.aspx.cs" Inherits="eStore.UI.Product.Category" %>

<asp:Content ID="Content1" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:ProductCategory ID="ProductCategory1" runat="server" />
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:ProductCategoryList ID="ProductCategoryList1" runat="server" />
    <asp:PlaceHolder ID="phWidget" runat="server" Visible="false"></asp:PlaceHolder>
</asp:Content>
