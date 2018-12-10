<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderMenu.ascx.cs"
    Inherits="eStore.UI.Modules.V4.HeaderMenu" %>
<%--<%@ OutputCache Duration="120" VaryByParam="None" %>--%>
<%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/byMenuV1") %>
<%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/eStoreMenuV1")%>
<asp:Literal ID="productCategoryMenu" runat="server" EnableViewState="false">
</asp:Literal>
