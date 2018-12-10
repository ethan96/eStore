<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderDetail.ascx.cs" 
    Inherits="eStore.UI.Modules.V4.OrderDetail" %>
<%@ Register Src="~/Modules/V4/CartContactTemplate.ascx" TagName="CartContactTemplate" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/CartContentPreview.ascx" TagName="CartContentPreview" TagPrefix="eStore" %>
<h4><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_number)%>：<asp:Literal ID="lOrderNumber" runat="server"></asp:Literal></h4>
<div class="eStore_order_orderDetails row20">
    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="eStore_table_orderTO">
        <tr>
            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Bill_to)%></th>
            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Sold_to)%></th>
            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Ship_to)%></th>
        </tr>
        <tr>
            <eStore:CartContactTemplate ID="BillToContact" runat="server" />
            <eStore:CartContactTemplate ID="SoldToContact" runat="server" />
            <eStore:CartContactTemplate ID="ShipToContact" runat="server" />
        </tr>
    </table>
    <eStore:CartContentPreview ID="CartContentPreview1" runat="server" />
    <div class="eStore_orderStep_subTotal">
        <div>
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Item_Subtotal)%>：</span>
            <span><asp:Literal ID="lSubTotal" runat="server"></asp:Literal></span>
        </div>
        <div>
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Ship_via)%>：</span>
            <span><asp:Literal ID="lShippingInfo" runat="server"></asp:Literal></span>
        </div>
        <asp:Panel ID="pShipHandling" runat="server" Visible="false">
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Shipping_Handling)%>：</span>
            <span><asp:Literal ID="lFreight" runat="server"></asp:Literal></span>
        </asp:Panel>
        <asp:Panel ID="pTaxEstimated" runat="server" Visible="false">
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Estimated_Tax)%>：</span>
            <span><asp:Literal ID="lTax" runat="server"></asp:Literal></span>
        </asp:Panel>
        <asp:Panel ID="pDutyTax" runat="server" Visible="false">
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_DutyAndTax)%>：</span>
            <span><asp:Literal ID="lDutyAndTax" runat="server"></asp:Literal></span>
        </asp:Panel>
        <asp:Panel ID="pSurcharge" runat="server" Visible="false">
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Surcharge)%>：</span>
            <span><asp:Literal ID="lSurcharge" runat="server"></asp:Literal></span>
        </asp:Panel>
        <asp:Panel ID="pTotalDiscount" runat="server" Visible="false">
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Discount)%>：</span>
            <span><asp:Literal ID="lTotalDiscount" runat="server"></asp:Literal></span>
        </asp:Panel>
        <div>
            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Grand_Total)%>：</span>
            <span><asp:Literal ID="lTotal" runat="server"></asp:Literal><asp:Literal ID="lSubStorePrice" runat="server"></asp:Literal></span>
        </div>
    </div>
</div>
<div class="eStore_order_btnBlock minWidth row20">
    <a href="/Account/MyAccount.aspx" class="eStore_btn borderBlue" onclick="javascript:window.history.go(-1);"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Back)%></a>
    <asp:LinkButton ID="lb_Reorder" runat="server" CssClass="eStore_btn borderBlue fancybox" Text="Re Order" OnClick="lb_Reorder_Click"></asp:LinkButton>
    <a href="<%=PrintUrl %>" class="eStore_btn"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_printorder)%></a>
</div>