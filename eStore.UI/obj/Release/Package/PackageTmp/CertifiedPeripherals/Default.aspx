<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.Default" %>

<%@ Register Src="~/Modules/CertifiedPeripherals/LeftSideMenus.ascx" TagName="LeftSideMenus"
    TagPrefix="uc1" %>
<%@ Register Src="~/Modules/CertifiedPeripherals/HomeContent.ascx" TagName="HomeContent"
    TagPrefix="uc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <div class="eStore_container eStore_block980">
        <div id="HeaderBanner" style="margin-bottom: 10px;">
            <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <div class="master-wrapper-side">
        <uc1:LeftSideMenus ID="LeftSideMenus1" runat="server" />
        <div id="storeSideAds">
        </div>
         <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="master-wrapper-center">
        <div class="master-wrapper-cph">
            <uc2:HomeContent ID="HomeContent1" runat="server" />
        </div>
    </div>
</asp:Content>
