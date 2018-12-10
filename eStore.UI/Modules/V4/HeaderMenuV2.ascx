<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderMenuV2.ascx.cs" Inherits="eStore.UI.Modules.V4.HeaderMenuV2" %>

<%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/byMenuV2") %>
<%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/eStoreMenuV2")%>

<asp:Literal ID="productCategoryMenu" runat="server" EnableViewState="false">
</asp:Literal>