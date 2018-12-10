<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="default.aspx.cs" Inherits="eStore.UI.aboutus._default" %>

<%@ Register Src="~/Modules/Widget.ascx" TagName="Widget" TagPrefix="eStore" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:Widget ID="aboutusWidget" runat="server" WidgetName="aboutus" />
</asp:Content>
