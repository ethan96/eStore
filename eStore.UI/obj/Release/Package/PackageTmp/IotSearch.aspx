<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master" AutoEventWireup="true" CodeBehind="IotSearch.aspx.cs" Inherits="eStore.UI.IotSearch" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    
    <h2 class="CategoryTitle">
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_IoTSearchResult)%></span></h2>
    <eStore:ProductList ID="ProductList1" runat="server" ResultsLocationType="Bottom" isShowSelect="false" />
    <div class="clear"></div>
    <asp:Panel ID="pnCustom" runat="server">
    <br /><br />
    <h2 class="CategoryTitle">
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_eStoreSearchResult)%></span>
    </h2>
    <asp:Literal ID="lNoMatchedMessage" Text="No matched product found" runat="server"
        Visible="false"></asp:Literal>
    <eStore:ProductList ID="ProductList2" runat="server" ResultsLocationType="Bottom" target="_bank" isToeStore="true" isShowSelect="false" />
    </asp:Panel>
</asp:Content>
