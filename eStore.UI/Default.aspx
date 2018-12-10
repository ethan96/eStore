<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eStore.UI._Default" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render(HomePageStyle.PageTheme)%>
    <%= System.Web.Optimization.Scripts.Render(HomePageStyle.Scripts)%>
</asp:Content>
<asp:Content ID="banner" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <eStore.V4:eStoreCycle2Slider ID="eStoreCycle2Slider1" runat="server"  />
</asp:Content>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
    <eStore.V4:HomeMedialContent ID="HomeMedialContent1" runat="server" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
 <eStore:Advertisement ID="Advertisement1" runat="server"  />
</asp:Content>
