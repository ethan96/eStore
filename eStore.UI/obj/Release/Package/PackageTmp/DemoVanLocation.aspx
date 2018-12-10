<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="DemoVanLocation.aspx.cs" Inherits="eStore.UI.DemoVanLocation" %>
<%@ Register assembly="Artem.Google" namespace="Artem.Google.UI" tagprefix="Google" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <Google:GoogleMap ID="GoogleMap1" runat="server" MapType="Roadmap" Zoom="8"
        Width="770" DefaultAddress="台湾台北市">
    </Google:GoogleMap>
</asp:Content>
