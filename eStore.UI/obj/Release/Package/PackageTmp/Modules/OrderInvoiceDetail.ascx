<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderInvoiceDetail.ascx.cs"
    Inherits="eStore.UI.Modules.OrderInvoiceDetail" %>
<%@ Register Src="OrderContentPreview.ascx" TagName="OrderContentPreview" TagPrefix="ucc" %>
<h4>
    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Detail)%> (Order Number: <%= order.OrderNo %>)</h4>
<div class="eStore_order_orderDetails row20">
    <ucc:OrderContentPreview ID="OrderContentPreview1" runat="server" />
</div>