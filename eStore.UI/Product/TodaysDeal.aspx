<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TodaysDeal.aspx.cs" Inherits="eStore.UI.Product.TodaysDeal" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="DarkBlueHeader">
        Promotion Products
    </div>
    <asp:DataList ID="rpPromotionProduct" runat="server" RepeatColumns="4" ItemStyle-VerticalAlign="Top"
            ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <h5>
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("name")%></a></h5>
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <asp:Image ID="imgProduct" runat="server" ImageUrl='<%#  Eval("thumbnailImageX")  %>' AlternateText='<%# Eval("name") %>' Width="80" />
                </a>
                <p>
                    <%# Eval("productDescX")%>
                </p>
                <div class="CategoryMinPrice">
                    <%# eStore.Presentation.Product.ProductPrice.getAJAXProductPrice((eStore.POCOS.Product)Container.DataItem, eStore.Presentation.Product.PriceStyle.productprice)%>
                </div>
            </ItemTemplate>
            <ItemStyle Width="180" />
            <AlternatingItemStyle Width="180" />
        </asp:DataList>
        
    <div class="DarkBlueHeader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Today_Highlights)%>
    </div> 
    <asp:DataList ID="dlHomeBanner" runat="server" RepeatColumns="4" ItemStyle-VerticalAlign="Top" Width="100%"
            ItemStyle-HorizontalAlign="Center" onitemdatabound="dlHomeBanner_ItemDataBound">
        <ItemTemplate>
            <asp:HyperLink ID="hlHomeBanner" runat="server">
                <asp:Image ID="imgHomeBanner" runat="server" AlternateText='<%# Eval("AlternateText") %>' Width="188" Height="110" />
            </asp:HyperLink>
        </ItemTemplate>
        <ItemStyle Width="180" />
        <AlternatingItemStyle Width="180" />
    </asp:DataList>

    <div class="DarkBlueHeader">
        Current Promotion Campaigns
    </div> 
    <asp:DataList ID="rpCampaigns" runat="server" CssClass="CampaignsCss" RepeatColumns="4"  CellPadding="3" ItemStyle-VerticalAlign="Top"
        onitemdatabound="rpCampaigns_ItemDataBound">
            <ItemTemplate> 
                <p class="greenBold">
                    <%# Eval("Description")%>
                </p>
                <p class="textCenter">
                    <asp:Literal id="ltStartDate" runat="server"></asp:Literal> <br />
                    <asp:Literal id="ltEndDate" runat="server"></asp:Literal>
                </p>
                <p class="textCenter"><asp:Literal id="ltPromotionCode" runat="server"></asp:Literal></p>
            </ItemTemplate>
            <ItemStyle Width="180" />
            <AlternatingItemStyle Width="180" />
    </asp:DataList>

    <div class="DarkBlueHeader">
        Clearance
    </div>
    <asp:DataList ID="rpClearanceProduct" runat="server" RepeatColumns="4" ItemStyle-VerticalAlign="Top"
            ItemStyle-HorizontalAlign="Center">
        <ItemTemplate>
            <h5>
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <%# Eval("name")%></a></h5>
            <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                <asp:Image ID="imgProduct" runat="server" ImageUrl='<%#  Eval("thumbnailImageX")  %>' AlternateText='<%# Eval("name") %>' Width="80" />
            </a>
            <p>
                <%# Eval("productDescX")%>
            </p>
            <div class="CategoryMinPrice">
                <%# eStore.Presentation.Product.ProductPrice.getAJAXProductPrice((eStore.POCOS.Product)Container.DataItem, eStore.Presentation.Product.PriceStyle.productprice)%>
            </div>
        </ItemTemplate>
        <ItemStyle Width="180" />
        <AlternatingItemStyle Width="180" />
    </asp:DataList>

</asp:Content>
