<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="Bundle.aspx.cs" Inherits="eStore.UI.Product.Bundle" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/product")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/product")%>
  <eStore:Bundle ID="BundleProductContent" runat="server"  EnableViewState ="false"/>
</asp:Content>
