<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn2013.Master"
    AutoEventWireup="true" CodeBehind="iautomation.aspx.cs" Inherits="eStore.UI.iautomation" %>

<%@ Register Src="Modules/AutomationHomeContent.ascx" TagName="AutomationHomeContent"
    TagPrefix="eStore" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
<link href="Styles/goldeneggs.css" rel="stylesheet" type="text/css" />
<script src="Scripts/GoldEggsUtil.js" type="text/javascript"></script>
    <eStore:Repeater ID="rpPromotionProduct" runat="server" OnItemDataBound="rpPromotionProduct_ItemDataBound">
        <HeaderTemplate>
            <div id="SaleArea">
                <div id="SaleHeader">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.iautomation_store_TodayDetails)%></div>
                <div id="SaleProduct">
                    <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <span class="title">
                    <asp:HyperLink ID="hlcategory" runat="server"></asp:HyperLink>
                </span>
                <div class="photo">
                    <%# string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"88px\" height=\"88px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                              , Eval("thumbnailImageX")
, Eval("name") )%></div>
                <div class="info">
                    <div class="sno">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("name")%></a></div>
                    <div class="desc">
                        <%# Eval("productDescX")%></div>
                  
                        <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productprice)%>
                    <div class="bushop">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            SHOP NOW</a></div>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul> </div> </div></FooterTemplate>
    </eStore:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:AutomationHomeContent ID="AutomationHomeContent1" runat="server" />
</asp:Content>
