<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Product.aspx.cs" Inherits="eStore.UI.Product.Product" EnableEventValidation="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/product")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/product")%>
</asp:Content>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
    <eStore:Product ID="ProductContent" runat="server" EnableViewState ="false" />
    <eStore:Advertisement ID="Advertisement1" runat="server"  OnHtmlLoaded="fixAdsHeight();"  />
</asp:Content>
