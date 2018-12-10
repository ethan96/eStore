<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartNavigator.ascx.cs"
    Inherits="eStore.UI.Modules.CartNivagator" %>

<div class="eStore_breadcrumb eStore_block980">
    <a href="/"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
    <asp:Literal ID="ltnavigator" runat="server"></asp:Literal>
</div><!--breadcrumb-->
<div class="eStore_container eStore_block980">
    <div class="eStore_order_content">
        <div class="eStore_order_steps">
            <h2>
                <%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Complete_the_order")%></h2>
            <ol class="eStore_order_stepsBlock">
                <asp:Literal ID="hlCart" runat="server"></asp:Literal>
                <asp:Literal ID="hlAddress" runat="server"></asp:Literal>
                <asp:Literal ID="hlConfirm" runat="server"></asp:Literal>
                <asp:Literal ID="hlPayment" runat="server"></asp:Literal>
                <asp:Literal ID="hlThankyou" runat="server"></asp:Literal>
            </ol>
        </div>
    </div>
</div>
