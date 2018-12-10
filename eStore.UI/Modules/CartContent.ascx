<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartContent.ascx.cs"
    Inherits="eStore.UI.Modules.CartContent" %>
<asp:Label ID="lblCartItemMessage" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
<eStore:Repeater ID="rpCartContent" runat="server" OnItemDataBound="rpCartContent_ItemDataBound"
    OnItemCommand="rpCartContent_ItemCommand">
    <HeaderTemplate>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_order eStore_orderItem eStore_orderStep1">
            <thead>
                <tr class="carthead <%if (showATP){%> normalTh<%}%>">
                    <th width="30">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_ItemIndex)%>
                    </th>
                    <th width="120">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                    </th>
                    <th class="tbldesc">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                    </th>
                    <%if (showATP)
                      { %>
                    <th class="adminonly" width="30">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>
                    </th>
                    <th class="adminonly" width="60">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                    </th>
                    <th class="adminonly" width="30">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                    </th>
                    <%} %>
                    <th width="60">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                    </th>
                    <th width="30">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                    </th>
                    <th width="60">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_SubTotal)%>
                    </th>
                    <th width="55">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Remove)%>
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <span class="fontBold"><%# Container.ItemIndex+1 %></span>
            </td>
            <td class="left">
                <%# Eval("productNameX")%>
                <%# (eStore.POCOS.Product.PRODUCTTYPE)Eval("type") == eStore.POCOS.Product.PRODUCTTYPE.CTOS 
                    && ((eStore.POCOS.BTOSystem)Eval("BTOSystem")).BTOSConfigs.FirstOrDefault(bc => bc.isNoneCTOS() == true && bc.CategoryComponentID==bc.OptionComponentID) == null
                                                                                                    ? string.Format(@"<br /><a class='ReconfigSystem' href='{0}?ItemNO={1}&source={2}'>{3}</a>",
                                                                            ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Eval("partX"))),
                                                                            Eval("ItemNo"), 
                                                                            CartSourceType,
                                                                                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Reconfig)) : ""%>
                <%# showATP 
                    && (eStore.POCOS.Product.PRODUCTTYPE)Eval("type") == eStore.POCOS.Product.PRODUCTTYPE.CTOS
                                                                            ? string.Format(@" | <a href='/cart/BTOSystemEditor.aspx?ItemNO={0}&source={1}' class='adminonly'>{2}</a>",
                                                                            Eval("ItemNo"), 
                                                                            CartSourceType,
                                                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Modify)) : ""%>
                <asp:HyperLink ID="hlAddWarranty" CssClass="addwarrantylink mousehand" runat="server">Add Warranty</asp:HyperLink>
            </td>
            <td class="left">
                <%# Eval("Description") %><br />
                <span class="colorRed">
                    <%# Eval("CustomerMessage")%>
                    <%#Eval("PromotionMessage")==null|| string.IsNullOrEmpty(Eval("PromotionMessage").ToString()) 
                         ? string.Empty
                         : (Eval("CustomerMessage") == null || string.IsNullOrEmpty(Eval("CustomerMessage").ToString())? string.Empty:"<br />")
                         + Eval("PromotionMessage").ToString()%>
                </span>
            </td>
            <%if (showATP)
              { %>
            <td class="adminonly forAuthor">
                <%#Eval("ABCInd")%>
            </td>
            <td class="adminonly forAuthor">
                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
            </td>
            <td class="adminonly forAuthor">
                <%#Eval("atp.availableQty")%>
            </td>
            <td class="left">
                <eStore:TextBox ID="txtUnitPrice" Text='' runat="server" CssClass="pricetextbox">
                </eStore:TextBox>
            </td>
            <%}
              else
              {%>
            <td class="right">
                <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("UnitPrice"))%>
            </td>
            <%} %>
            <td>
                <eStore:TextBox ID="txtQty" Text='<%# Eval("Qty") %>' runat="server" CssClass="qtyboxOnlyNO">
                </eStore:TextBox><%# Eval("partX.MininumnOrderQty") == null ? "" : string.Format("<input type='hidden' class='moqValue isCheckMOQ' value='{0}' sproductid='{1}' />", Eval("partX.MininumnOrderQty"), Eval("productNameX"))%>
            </td>
            <td class="right">
                <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("AdjustedPrice"))%>
                <%#Eval("DiscountAmount")==null?string.Empty:string.Format("<br/><span class=\"colorRed\">-{0}</span>",eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("DiscountAmount")))%>
            </td>
            <td>
                <asp:Button ID="btDelete" runat="server" Text="Delete" CommandName="DeleteItem" CommandArgument='<%# Eval("ItemNo") %>'
                    CssClass="deleteButton" />
            </td>
        </tr>
        <tr id="warrrantyItem" runat="server">
            <td class="center">
                <span class="paddingleft15">1</span>
            </td>
            <td class="left">
                <asp:Literal ID="lproductNameX" runat="server"></asp:Literal>
            </td>
            <td class="left">
                <asp:Literal ID="lDescription" runat="server"></asp:Literal>
            </td>
            <td class="adminonly forAuthor">
                &nbsp;
            </td>
            <td class="adminonly forAuthor">
                <asp:Literal ID="lavailableDate" runat="server"></asp:Literal>
            </td>
            <td class="adminonly forAuthor">
                <asp:Literal ID="lavailableQty" runat="server"></asp:Literal>
            </td>
            <td class="right">
                <asp:Literal ID="lUnitPrice" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:Literal ID="lQty" runat="server"></asp:Literal>
            </td>
            <td class="right">
                <asp:Literal ID="lAdjustedPrice" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:Button ID="btDeleteWarranty" runat="server" Text="Delete" CommandName="DeleteItemWarranty"
                    CommandArgument='<%# Eval("ItemNo") %>' CssClass="deleteButton" />
            </td>
        </tr>
        <eStore:Repeater ID="rpBTOSConfig" runat="server">
            <ItemTemplate>
                <tr class="eStore_orderSystemList">
                    <%if (showATP)
                      { %>
                    <td>
                        &nbsp;
                    </td>
                    <td class="left">
                        <span class="fontBold"><%# Eval("CategoryComponentDesc")%></span>
                        <%#((bool)Eval("isBuildin")) ? "<br /><span class=\"colorBlue\" >(built-in)</span>" : string.Empty%>
                        <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("BTOSConfigDetails") %>'>
                            <ItemTemplate>
                                <br />
                                <%# Eval("SProductID")%></li>
                            </ItemTemplate>
                        </eStore:Repeater>
                    </td>
                    <td class="left">
                        <%# Eval("OptionComponentDesc")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : Eval("ABCInd")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : Eval("atp.availableQty")%>
                    </td>
                    <td class="adminonly right">
                        <%#Eval("AdjustedPrice")%>
                    </td>
                    <td class="adminonly">
                        <%# Eval("qty")%>
                    </td>
                    <td class="adminonly right">
                        <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("subtotal"))%>
                    </td>
                    <td>
                    </td>
                    <%}
                      else
                      { %>
                    <td colspan="10" class="left">
                        <ol>
                            <li><b>
                                <%# Eval("CategoryComponentDesc")%>
                                <%#((bool)Eval("isBuildin"))? "built-in":string.Empty %>
                            </b>
                                <%# Eval("OptionComponentDesc")%>
                            </li>
                        </ol>
                    </td>
                    <%}%>
                </tr>
            </ItemTemplate>
        </eStore:Repeater>
        <eStore:Repeater ID="rpBundleItem" runat="server" OnItemDataBound="rpBundleItem_ItemDataBound"
            OnItemCommand="rpBundleItem_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td class="center">
                        <span class="paddingleft15"><%# Container.ItemIndex+1 %></span>
                    </td>
                    <td class="left">
                        <%# Eval("part.name")%>
                    </td>
                    <%if (showATP)
                      { %>
                    <td class="left">
                        <%# Eval("part.productDescX")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#Eval("part.ABCInd")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#eStore.Presentation.eStoreLocalization.Date(Eval("part.atp.availableDate"))%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#Eval("part.atp.availableQty")%>
                    </td>
                    <td class="right adminonly">
                        <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("AdjustedPrice"))%>
                    </td>
                    <td class="center adminonly">
                        <%#((eStore.POCOS.CartItem)((System.Web.UI.WebControls.RepeaterItem)Container.Parent.Parent).DataItem).Qty * (int)Eval("Qty")%>
                    </td>
                    <td class="right adminonly">
                        <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("AdjustedPrice") * ((eStore.POCOS.CartItem)((System.Web.UI.WebControls.RepeaterItem)Container.Parent.Parent).DataItem).Qty * (int)Eval("Qty"))%>
                    </td>
                    <%}
                      else
                      {%>
                    <td colspan="4" class="left">
                        <%# Eval("part.productDescX")%>
                    </td>
                    <%}%>
                    <td class="center">
                        <asp:Button ID="btDeleteBundleItem" runat="server" Visible="false" Text="Delete"
                            CommandName="DeleteBundleItem" CommandArgument='<%# Eval("BundleItemID") %>'
                            CssClass="deleteButton" />
                    </td>
                </tr>
                <eStore:Repeater ID="rpBTOSConfig" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <%if (showATP)
                              { %>
                            <td class="left">
                                <%# Eval("CategoryComponentDesc")%>
                                <%#((bool)Eval("isBuildin")) ? "<br /><span class=\"colorBlue\" >(built-in)</span>" : string.Empty%>
                                <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("BTOSConfigDetails") %>'>
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <%# Eval("SProductID")%></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul></FooterTemplate>
                                </eStore:Repeater>
                            </td>
                            <td class="left">
                                <%# Eval("OptionComponentDesc")%>
                            </td>
                            <td class="adminonly center">
                                <%#(bool)Eval("isBuildin") ? string.Empty : Eval("ABCInd")%>
                            </td>
                            <td class="adminonly">
                                <%#(bool)Eval("isBuildin") ? string.Empty : eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
                            </td>
                            <td class="adminonly center">
                                <%#(bool)Eval("isBuildin") ? string.Empty : Eval("atp.availableQty")%>
                            </td>
                            <td class="adminonly right">
                                <%#Eval("AdjustedPrice")%>
                            </td>
                            <td class="adminonly center">
                                <%# Eval("qty")%>
                            </td>
                            <td class="adminonly right">
                                <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("subtotal"))%>
                            </td>
                            <%}
                              else
                              { %>
                            <td class="left">
                                <%# Eval("CategoryComponentDesc")%>
                                <%#((bool)Eval("isBuildin"))? "Buildin":string.Empty %>
                            </td>
                            <td colspan="4" class="left">
                                <%# Eval("OptionComponentDesc")%>
                            </td>
                            <%}%>
                        </tr>
                    </ItemTemplate>
                </eStore:Repeater>
            </ItemTemplate>
        </eStore:Repeater>
    </ItemTemplate>
    <FooterTemplate>
        </tbody> </table>
    </FooterTemplate>
</eStore:Repeater>
<div id="selectWarrantyItem" style="display: none">
    <asp:RadioButtonList ID="rblWarranty" runat="server" CssClass="fontbold" ClientIDMode="Static">
    </asp:RadioButtonList>
</div>
<asp:HiddenField runat="server" ID="hfWarranyCartItem" ClientIDMode="Static"></asp:HiddenField>
<script type="text/javascript">

    function showSelectWarrantyItemDialog(objPartNo, total, qty) {
        $("#hfWarranyCartItem").val(objPartNo);
        $.each($("#rblWarranty span[rate!='0']").has(".addtionprice"), function (i, n) {

            var warrantyitemprice = parseFloat(total) * parseFloat($(this).attr("rate") / 100) * parseInt(qty);

            var sumSign = "";
            if (warrantyitemprice >= 0) {
                sumSign = "+";
            }
            else if (warrantyitemprice < 0) {
                sumSign = "-";
            }
            $(this).find(".priceSing").html(sumSign);
            $(this).find(".addtionprice").html(formatdecimal(Math.abs(warrantyitemprice)));
        });
        popupDialog("#selectWarrantyItem");
    }

    $(document).ready(function () {
        $(".needCheckMOQ").click(function () {
            return checkMOQ();
        });
    });

    function checkMOQ() {
        var cc = true;
        var productids = "";
        var i = 0;
        var errorMessage = "<%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Minimum_Quantity_Error)%>";
        $(":hidden.isCheckMOQ").each(function () {
            var tb = $(this).prev("input");
            if (parseInt($(this).val()) > parseInt($(tb).val())) {
                if (i == 0) {
                    productids = $(this).attr("sproductid") + "[" + $(this).val() + "]";
                    $(tb).focus();
                }
                else
                    productids = productids + "," + $(this).attr("sproductid") + "[" + $(this).val() + "]";
                cc = false;
                i++;
            }
        });
        if (!cc)
            alert(errorMessage.replace("{0}", productids));
        return cc;
    }

    $(document).ready(function () {
        $(":checkbox").click(function () {
            var trItem = $(this).closest("tr");
            doSome(trItem, $(this));
            if (trItem.hasClass("cartitem")) {
                nextItem(trItem.next("tr"), $(this));
            }
        });
    });

    function doSome(item, sender) {
        var itcc = item.find("input:hidden.moqValue");
        if (sender.attr("checked") == true)
            itcc.removeClass("isCheckMOQ");
        else
            itcc.addClass("isCheckMOQ");
        //alert(itcc.attr("class"));
        //alert(item.find("input:hidden.moqValue").val());
    }

    function nextItem(item, sender) {
        if (item.length == 0 || item.hasClass("cartitem"))
            return;
        else {
            doSome(item, sender);
            nextItem(item.next("tr"), sender);
        }
    }

</script>
