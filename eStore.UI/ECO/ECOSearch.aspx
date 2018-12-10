<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="ECOSearch.aspx.cs" Inherits="eStore.UI.ECOSearch" %>

<%@ Register Src="../Modules/ECO/ECOPartnerSearch.ascx" TagName="ECOPartnerSearch"
    TagPrefix="uc1" %>
<%@ Register Src="../Modules/ECO/ECOPartnerList.ascx" TagName="ECOPartnerList" TagPrefix="uc2" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div id="ecoParterList-title">
        <asp:Image ID='imgEcosystem' runat="server" ImageUrl="~/Images/Ecosystem_Partnership_Banner1.jpg"
            ToolTip="Your Ecosystem Partners" />
    </div>
    <div class="master-wrapper-center">
        <uc2:ECOPartnerList ID="ECOPartnerList1" runat="server" />
    </div>
    <div class="master-wrapper-side">
        <uc1:ECOPartnerSearch ID="ECOPartnerSearch1" runat="server" />
    </div>
</asp:Content>
