<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithSubCategoryAndProducts.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.CategoryWithSubCategoryAndProducts" %>
<div class="RootCategory">
    <h3>
        <asp:HyperLink ID="hCategory" runat="server"></asp:HyperLink></h3>
</div>
<estore:repeater id="rpsubCategoryList" runat="server" onitemdatabound="rpsubCategoryList_ItemDataBound">
    <ItemTemplate>
        <div class="blankspace">
        </div>
        <div class="headercorner">
            <span></span>
        </div>
        <h4>
            <table>
                <tr>
                    <td align="left">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("LocalCategoryName")%></a>
                    </td>
                    <td align="right" valign="top">
                        <asp:Panel ID="pSeefullselection" runat="server" CssClass="btn_view_container">
                            <div class="btn_selection">
                                <div class="btn_view_left">
                                </div>
                                <div class="btn_view_bg">
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_See_full_selection)%>
                                    </a>
                                </div>
                                <div class="btn_view_right">
                                </div>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </h4>
        <asp:DataList ID="dlSubCategoryProducts" runat="server" RepeatColumns="2" ItemStyle-VerticalAlign="Top"
            ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <h5>
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("name")%></a></h5>
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <img src="<%#  Eval("thumbnailImageX")  %>" alt="<%# Eval("name") %>" width="80px" /></a>
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
    </ItemTemplate>
</estore:repeater>
