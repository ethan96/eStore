<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master"
    AutoEventWireup="true" CodeBehind="IoTMart.aspx.cs" Inherits="eStore.UI.IoTMart" %>

<%@ Register Src="Modules/AdRotatorSelect.ascx" TagName="AdRotatorSelect" TagPrefix="uc1" %>
<%@ Register src="Modules/IoTMart/HotDeals.ascx" tagname="HotDeals" tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="eStoreLeftContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="iot-block iot-topBannerIndex">
        <uc1:AdRotatorSelect ID="AdRotatorSelect1" BannerWidth="470" runat="server" />

        <div id="Iot-TwoDailAdBanner" class="IotAds iot-bannerSBlockIndex" adtype="twoDailAd">
            <img src="images/Loading.gif" alt="loading banner" />
        </div>
        <!--iot-bannerSBlockIndex-->
    </div>
    <div class="iot-block">
        <div class="iot-titleBG">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_HotDeals)%></div>

   <uc2:HotDeals ID="HotDeals1" runat="server" />
        <!--iot-highlightBlock-->
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <div class="iot-carouselBlockIndex">
        <div class="iot-block iot-carouselBlock">
            <div class="iot-titleBG">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_BestSellers)%></div>
            <div id="iot-carouselBest" class="iot-carouselBlock-content">
                <ul>
                    <asp:Repeater ID="rpBestSeller" runat="server">
                        <ItemTemplate>
                            <li class="iot-proBlock">
                                <div class="iot-proBlock-pic">
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                        <%# string.Format("<a href=\"{0}\"><img src=\"{3}\"  alt=\"{2}\" lazysrc=\"{1}\" class=\"lazyImg\" /></a> "
                    , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                    , Eval("thumbnailImageX")
                    , Eval("name")
                    , ResolveUrl("~/images/Loading.gif") )%></a>
                                </div>
                                <div class="iot-proBlock-msg">
                                    <div class="iot-proDescription">
                                        <div class="iot-proPartNumber">
                                            <%# Eval("name")%></div>
                                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                            <%# Eval("productDescX")%></a>
                                    </div>
                                    <div class="iot-proAction">
                                        <div class="iot-proPrice">
                                            <div class="iot-priceOrange">
                                                <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productprice)%></div>
                                        </div>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <div class="iot-carouselControl">
                    <span id="Span1" class="iot-pager"></span><a id="iot-prev" class="iot-prev" href="#">
                    </a><a id="iot-next" class="iot-next" href="#"></a>
                </div>
            </div>
        </div>
        <!--iot-carouselBlock-->
        <div class="iot-block iot-carouselBlock">
            <div class="iot-titleBG">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_NewArrivals)%></div>
            <div id="iot-carouselNew" class="iot-carouselBlock-content">
                <ul>
                    <asp:Repeater ID="rpNewArrivals" runat="server">
                        <ItemTemplate>
                            <li class="iot-proBlock">
                                <div class="iot-proBlock-pic">
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                        <%# string.Format("<a href=\"{0}\"><img src=\"{3}\"  alt=\"{2}\" lazysrc=\"{1}\" class=\"lazyImg\" /></a> "
                    , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                    , Eval("thumbnailImageX")
                    , Eval("name")
                    , ResolveUrl("~/images/Loading.gif") )%></a>
                                </div>
                                <div class="iot-proBlock-msg">
                                    <div class="iot-proDescription">
                                        <div class="iot-proPartNumber">
                                            <%# Eval("name")%></div>
                                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                            <%# Eval("productDescX")%></a>
                                    </div>
                                    <div class="iot-proAction">
                                        <div class="iot-proPrice">
                                            <div class="iot-priceOrange">
                                                <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productprice)%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <div class="iot-carouselControl">
                    <span id="Span2" class="iot-pager"></span><a id="iot-prev" class="iot-prev" href="#">
                    </a><a id="iot-next" class="iot-next" href="#"></a>
                </div>
            </div>
        </div>
        <!--iot-carouselBlock-->
    </div>
    <!--iot-carouselBlockIndex-->
    <div class="IotAds iot-bannerSBlockIndex" adtype="fourDailAd">
            <img src="<%=esUtilities.CommonHelper.GetStoreLocation()%>images/Loading.gif" alt="loading banner" />
    </div>
    <!--iot-bannerSBlockIndex-->
    <script type="text/javascript">
        $(document).ready(function () {
            $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=7&HomePage=true",
               function (data) {
                   $.addIotAdBanners(data, 6);
               });
        });
    </script>
</asp:Content>
