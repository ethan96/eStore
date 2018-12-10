<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="Config.aspx.cs" Inherits="eStore.UI.Product.Config" %>

<%@ Register Src="~/Modules/eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider"
    TagPrefix="eStore" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
        dynamicArrows="false" dynamicTabs="true" showDescription="false" MinHeight="120" />
    <h1>
        <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal>
    </h1>
    <div class="CategoryDescription">
        <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
    </div>
    <asp:HyperLink ID="hPrint" runat="server" Target="_blank" CssClass="productprint left"
        EnableViewState="false">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Print)%>
    </asp:HyperLink>
    <eStore:ProductCompare ID="ProductCompare1" runat="server" showPrintButton="true"
        showRemoveButton="false" showHeaderButtons="true" />
</asp:Content>
