<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Redirect.ascx.cs" Inherits="eStore.UI.Modules.Payment.Redirect" %>
<div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Wire_Transfer)%>
    </div>
    <div class="paymentbody">
        <asp:Literal ID="RedirectPaymentDesc" runat="server"></asp:Literal>
    </div>
</div>
