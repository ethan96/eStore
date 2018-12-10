<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotDeals.ascx.cs" Inherits="eStore.UI.Modules.IoTMart.HotDeals" %>
<div class="<%=videoCss %>">
    <asp:Repeater ID="rpHotDeals" runat="server" OnItemDataBound="rpHotDeals_ItemDataBound">
        <ItemTemplate>
            <div class="iot-proBlock">
                <asp:Literal ID="ltProStatus" runat="server"></asp:Literal>
                <div class="iot-proBlock-pic">
                    <%# string.Format("<a href=\"{0}\"><img src=\"{3}\"  alt=\"{2}\" lazysrc=\"{1}\" class=\"lazyImg\" /></a> "
                                , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                , Eval("thumbnailImageX")
                                , Eval("name")
                                , ResolveUrl("~/images/Loading.gif") )%>
                    <asp:Literal ID="ltProPromotion" runat="server"></asp:Literal>
                </div>
                <div class="iot-proBlock-msg">
                    <div class="iot-proDescription">
                        <div class="iot-proPartNumber">
                            <%# Eval("name")%></div>
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("productDescX")%></a>
                    </div>
                    <div class="iot-proAction">
                        <asp:Literal ID="ltViedo" runat="server"></asp:Literal>
                        <div class="iot-proPrice">
                            <div class="iot-priceOrange">
                                <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productprice)%></div>
                        </div>
                        <div class="iot-proBtn">
                            <asp:Panel ID="porderEable" runat="server">
                                <input type="button" class="btnStyle needlogin canAddToCart" sproductid="<%# Eval("SProductID")%>"
                                value="<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_AddToCart)%>" />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
