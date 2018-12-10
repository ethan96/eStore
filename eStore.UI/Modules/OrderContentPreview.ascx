<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderContentPreview.ascx.cs"
    Inherits="eStore.UI.Modules.OrderContentPreview" %>
<%@ Register Src="CartContactTemplate.ascx" TagName="CartContactTemplate" TagPrefix="eStore" %>
<%@ Register Src="CartContentPreview.ascx" TagName="CartContentPreview" TagPrefix="eStore" %>
<%@ Register TagPrefix="eStore" TagName="ContactDetails" Src="~/Modules/ContactDetails.ascx" %>
<table width="100%" border="0" cellspacing="0" cellpadding="0" class="eStore_table_orderTO eStore_orderStep3">
    <tr>
        <td>
            <eStore:CartContactTemplate ID="BillToContact" runat="server" />
        </td>
        <td>
            <eStore:CartContactTemplate ID="SoldToContact" runat="server" />
        </td>
        <td>
            <eStore:CartContactTemplate ID="ShipToContact" runat="server" />
        </td>
    </tr>
</table>
<eStore:CartContentPreview ID="CartContentPreview1" runat="server" />
<asp:Panel ID="pComment" runat="server">
    <div class="DarkBlueHeader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Comment)%>
    </div>
    <p>
        <asp:Literal ID="lcomment" runat="server"></asp:Literal>
    </p>
</asp:Panel>
<div class="hiddenitem eStore_orderStep_subTotal" labfor="cartTotal">
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Sub_Total)%></span>
        <span>
            <asp:Literal ID="lSubTotal" runat="server"></asp:Literal></span>
    </div>
    <%if (eStore.Presentation.eStoreContext.Current.Store.offerShippingService)
      {%>
    <%if (!string.IsNullOrWhiteSpace(order.ShippingMethod))
      {%>
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Shipping_Method)%></span>
        <span>
            <asp:Literal ID="lShippingMethod" runat="server"></asp:Literal></span></div>
    <%} %>
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Freight)%>
        </span><span>
            <asp:Literal ID="lFreight" runat="server"></asp:Literal></span></div>
    <%} %>
    <%if (eStore.Presentation.eStoreContext.Current.Store.hasTaxCalculator)
      {%>
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Tax)%>
        </span><span>
            <asp:Literal ID="lTax" runat="server"></asp:Literal></span></div>
    <%} %>
    <%if (order.DutyAndTax.GetValueOrDefault() > 0)
      {%>
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_DutyAndTax)%></span>
        <span>
            <asp:Literal ID="lDutyAndTax" runat="server"></asp:Literal></span></div>
    <%} %>
    <%if (order.Surcharge.GetValueOrDefault() > 0)
      {%>
    <div>
        <span class="colorRed">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Surcharge)%></span>
        <span>
            <asp:Literal ID="lSurcharge" runat="server"></asp:Literal></span></div>
    <%} %>
    <%if (order.TotalDiscount.GetValueOrDefault() > 0)
      {%>
    <div class="colorRed">
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Discount)%><asp:Literal ID="ldiscountType" runat="server"></asp:Literal></span>
        <span>
            <asp:Literal ID="lDiscount" runat="server"></asp:Literal></span></div>
    <%} %>
    <div>
        <span>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Total)%>
        </span><span>
            <asp:Literal ID="lTotal" runat="server"></asp:Literal>
            <asp:Literal ID="lSubStorePrice" runat="server"></asp:Literal>    <asp:Literal ID="ltOrderMessage" runat="server"></asp:Literal></span></div>
</div>
<div class="eStore_orderStep2" id="cartContactContext"></div>
<div class="eStore_addReceiverBlock" id="dContactDetailsBill">
    <eStore:ContactDetails ID="ContactDetailsBill" runat="server" ValidationGroup="ContactDetails" />
    <div class="eStore_order_btnBlock">
        <asp:Button ID="btSaveBillto" runat="server" OnClientClick="return CheckValidate('#dContactDetailsBill')" Text="Save" CssClass="eStore_btn" OnClick="btnSaveUserContact_Click" />
        <a href="#" class="eStore_btn borderBlue btnCancel"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Cancel)%></a> 
        <a class="eStore_btn borderBlue mousehand">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Clear)%></a>
        <asp:HiddenField ID="hdContactType" runat="server" ClientIDMode="Static" />
    </div>
</div>
<script type="text/javascript">
    function clearAddressPopInfor() {
        var addTable = $(".AddressBook_personal");
        addTable.find("input[type='text']").val("");
    }

    function showAddress(lable) {
        $("#hdContactType").val(lable || "");
        $.fancybox.open("#dContactDetailsBill", {
            modal: true,
            parent: "#cartContactContext",
            afterClose: function () { clearAddressPopInfor(); }
        });
    }
    $(".eStore_addReceiverBlock .btnCancel").click(function () {
        $.fancybox.close();
        return false;
    });
    $(function() {
        $(".mousehand").click(function (i, n) {
            clearAddressPopInfor();
        });
    });
</script>