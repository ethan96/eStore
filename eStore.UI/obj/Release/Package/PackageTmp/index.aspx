<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master"
    AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="eStore.UI.index" %>

<%@ Register Src="~/Modules/uStoreHomeMedialContent.ascx" TagName="uStoreHomeMedialContent"
    TagPrefix="eStore" %>

<%@ Register Src="~/Modules/uStoreHomeMedialContent2014.ascx" TagName="uStoreHomeMedialContent2014"
    TagPrefix="eStore" %>
<%@ Register Src="~/Modules/AdRotatorSelect.ascx" TagName="AdRotatorSelect" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/iServiceHomeLiquidSlider.ascx" TagName="iServiceHomeLiquidSlider"
    TagPrefix="eStore" %>

<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server" Visible="false">
 
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <link href='https://fonts.googleapis.com/css?family=Lato:300' rel='stylesheet' type='text/css' />

    <eStore:uStoreHomeMedialContent2014 ID="uStoreHomeMedialContent1" runat="server" />
</asp:Content>
