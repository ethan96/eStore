<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IotProductList.ascx.cs" Inherits="eStore.UI.Modules.IoTMart.IotProductList" %>
<%@ Register src="PaginationProductList.ascx" tagname="PaginationProductList" tagprefix="uc1" %>


<div class="iot-block">
        <div class="iot-block iot-switchingBlock">
            <div class="iot-proStyleBlock">
                <span class="iot-proColumnBtn iot-show"></span><span class="iot-proListBtn"></span>
            </div>
            <div class="iot-proBtn">
                <asp:Button ID="btCompare" CssClass="btnStyle btnCompare" runat="server" Text="Compare"
                    OnClick="btCompare_Click" /></div>
        </div>
        <asp:Repeater ID="rpCategories" runat="server" OnItemDataBound="rpCategories_ItemDataBound">
            <ItemTemplate>
                <div class="iot-block">
                    <a id='<%#Eval("CategoryPath")%>'></a>
                    <div class="iot-titleBG">
                        <%#Eval("localCategoryNameX")%></div>
                    <div class="iot-proColumnBlock iot-show">
                        <ul class="iot_proList">
                            <asp:Repeater runat="server" ID="rpProducts">
                                <ItemTemplate>
                                    <li class="iot-proBlock">
                                        <div class="iot-proBlock-check">
                                            <%# getCompareStr(Container.DataItem)%>
                                        </div>
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
                                                <div class="iot-proBtn">
                                                    <input type="button" class="btnStyle needlogin canAddToCart" sproductid="<%# Eval("SProductID")%>"
                                                        value="<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_AddToCart)%>"></div>
                                            </div>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                    <div class="iot-proListBlock">

                        <uc1:PaginationProductList ID="ppList" runat="server" />


                        <div class="iot-block">
                            <div class="iot-proBtn textRight">
                                <eStore:Button ID="bt_AddtoCart" runat="server" CssClass="needlogin needCheckSelect btnStyle"
                                    Text="Add to Cart" OnClick="bt_AddtoCart_Click" EnableViewState="false" />
                                <eStore:Button ID="bt_AddtoQuote" runat="server" CssClass="needlogin needCheckSelect btnStyle"
                                    Text="Add to Quote" OnClick="bt_AddtoQuote_Click" EnableViewState="false" /></div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $(".needCheckSelect:input").click(function () {
            return checkSelect();
        });

        $(".btnCompare").click(function () {
            if ($("input:[name='compare']:checked").length == 0) {
                alert($.eStoreLocalizaion("ScriptMessage_Iot_select_product_first"));
                return false;
            }
        });
    });

    function checkSelect() {
        //alert($("[name='cbproduct'][checked='true']").length);
        //alert($("[name='cbproduct']").length);
        var cbproduct = $("[name='cbproduct']");
        var checkCount = false;
        if (cbproduct.length > 0) {
            cbproduct.each(function (i) {
                if (this.checked) checkCount = true;
            });
        } else {
            alert($.eStoreLocalizaion("ScriptMessage_Can_not_find_the_product"));
            return false;
        }
        if (checkCount) return true;
        alert($.eStoreLocalizaion("ScriptMessage_Can_not_find_the_product"));
        return false;
    }
</script>