<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ProductList.aspx.cs" Inherits="eStore.UI.Product.ProductList" %>
    <%@ Register Src="~/Modules/eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
    <%@ Register Src="~/Modules/AdPopularProduct.ascx" TagName="AdPopularProduct" TagPrefix="eStore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:ProductSpec ID="ProductSpec1" runat="server" />
    <asp:PlaceHolder ID="phRightSide" runat="server"></asp:PlaceHolder>
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <asp:Panel ID="pCategoryHeader" runat="server">
        <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
        dynamicArrows="false" dynamicTabs="true" showDescription="false" MinHeight="120" />
        <h1>
            <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal>
        </h1>
        <div class="CategoryDescription">
            <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
        </div>
    </asp:Panel>
    <div class="clear">
    </div>
    <asp:PlaceHolder ID="phLeftSide" runat="server"></asp:PlaceHolder>
    <eStore:AdPopularProduct ID="AdPopularProduct1" runat="server" PopularType="Category" />
</asp:Content>
