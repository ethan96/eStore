<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="BTOSystemEditor.aspx.cs" Inherits="eStore.UI.Cart.BTOSystemEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
<div class="eStore_order_content">
   <eStore:BTOSystemDetails ID="BTOSystemDetails1" runat="server" />
   </div>
</asp:Content>
