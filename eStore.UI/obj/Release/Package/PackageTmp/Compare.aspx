<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Compare.aspx.cs" Inherits="eStore.UI.Compare" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/compare")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/compare")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="eStore_container eStore_block980">
    <eStore.V4:ProductsComparison id="ProductsComparison1" runat="server" Title="Comparison Results"></eStore.V4:ProductsComparison>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server"></asp:Content>