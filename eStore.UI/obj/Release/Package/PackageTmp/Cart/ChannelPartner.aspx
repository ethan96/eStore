<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="ChannelPartner.aspx.cs" Inherits="eStore.UI.Cart.ChannelPartner" %>
<%@ Register Src="~/Modules/ChannelPartner.ascx" TagName="T_ChannelPartner" TagPrefix="eStore" %>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:T_ChannelPartner ID="T_ChannelPartner1" runat="server" />
</asp:Content>
