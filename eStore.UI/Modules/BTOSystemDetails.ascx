<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BTOSystemDetails.ascx.cs"
    Inherits="eStore.UI.Modules.BTOSystemDetails" %>
<eStore:Repeater ID="rpBTOSConfig" runat="server">
    <HeaderTemplate>
        <table class="eStore_table_order eStore_orderItem" width="100%">
            <thead>
                <tr class="carthead">
                    <th width="140">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                    </th>
                    <th>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                    </th>
                    <th width="80">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                    </th>
                    <th width="40">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                    </th>
                    <th width="100">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_SubTotal)%>
                    </th>
                    <th width="60">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Remove)%>
                    </th>
                </tr>
            </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="cartitem">
            <td style="text-align:left;">
                <%# Eval("CategoryComponentDesc")%>
            </td>
            <td colspan="5" style="text-align:left;">
                <%# Eval("OptionComponentDesc")%>
                <asp:LinkButton ID="btnRemoveComponent" runat="server" CommandName="remove" CommandArgument='<%# Eval("ConfigID")%>'
                    Visible="false">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Remove)%>    
                </asp:LinkButton>
            </td>
        </tr>
        <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("BTOSConfigDetails") %>'
            OnItemCommand="rpBTOSConfigDetails_ItemCommand" OnItemDataBound="rpBTOSConfigDetails_ItemDataBound">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="text-align:left;">
                        <%# Eval("SProductID")%>
                    </td>
                    <td style="text-align:left;">
                        <%# Eval("descriptionX")%>
                    </td>
                    <td class="right">
                        <eStore:TextBox ID="txtUnitPrice" Text='<%# eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(((decimal)Eval("AdjustedPrice"))).value.ToString("f2")%>'
                            runat="server" CssClass="pricetextbox"></eStore:TextBox>
                    </td>
                    <td class="center">
                        <eStore:TextBox ID="txtQty" Text='<%# (int)Eval("Qty")%>' runat="server" CssClass="qtyboxOnlyNO"></eStore:TextBox>
                    </td>
                    <td class="right">
                        <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal(((int)Eval("Qty"))  * ((decimal)Eval("AdjustedPrice")))%>
                    </td>
                    <td class="center">
                        <asp:LinkButton ID="btnBTOSConfigDetail" runat="server" CommandName="remove" CommandArgument='<%# Eval("ID")%>'>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_remvoe)%>    
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </eStore:Repeater>
    </ItemTemplate>
    <FooterTemplate>
        </table></FooterTemplate>
</eStore:Repeater>
<fieldset class="editorpanel addtionalproducts">
    <div class="rightside row20">
        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" CssClass="eStore_btn" OnClientClick="return checkSubQTYInfo()" />
        <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" CssClass="eStore_btn borderBlue" />
    </div>
    <div class="clear">
    </div>
    <p>
        <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%></label>
        <eStore:TextBox ID="txtAddtionalProduct" ToolTip="Part No." ClientIDMode="Static"
            runat="server" Width="170px"></eStore:TextBox>
           
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>:
        <eStore:TextBox ID="txtAddtionalProductQty" ClientIDMode="Static" ToolTip="Qty" runat="server"
            Width="170px"></eStore:TextBox><asp:Button ID="btnAddAddtionalProduct" runat="server" CssClass="eStore_btn borderBlue"
                Text="Add Component" ClientIDMode="Static" OnClick="btnAddAddtionalProduct_Click" /></p>
</fieldset>
<script type="text/javascript" language="javascript">
    $(function () {
        var tbValue = $('#txtAddtionalProduct');
        tbValue.autocomplete({
            source: function (request, response) {
                $.getJSON("/proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getOderByPNSearchKey %>",
                 {
                     maxRows: 12,
                     keyword: request.term
                 }, response);
            },
            select: function (event, ui) {
                $('#JT').remove();
                tbValue.val(ui.item.value);
                return false;
            },
            focus: function (event, ui) {
                tbValue.val(ui.item.value);
                var url = "/proc/html.aspx?type=ProductDetailTip&ProductID=" + ui.item.value;
                $('#JT').remove();
                JT_show(url, $(this).attr("id"), ui.item.label);
            },
            close: function (event, ui) { $('#JT').remove(); }
        });
    });
    $("#btnAddAddtionalProduct").click(function () {
        return $(".addtionalproducts :text").validateTextBoxWithToolTip();
    });
</script>
