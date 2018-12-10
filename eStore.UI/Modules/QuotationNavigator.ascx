<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuotationNavigator.ascx.cs"
    Inherits="eStore.UI.Modules.QuotationNavigator" %>
<div class="eStore_breadcrumb eStore_block980">
    <a href="/"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
    <asp:Literal ID="ltnavigator" runat="server"></asp:Literal>
</div>
<!--breadcrumb-->
<div class="eStore_container eStore_block980">
    <div class="eStore_order_content">
        <div class="eStore_order_steps">
            <h2>
                <asp:Literal ID="ltNavigatorTitle" runat="server"></asp:Literal></h2>
            <ol class="eStore_order_stepsBlock">
                <asp:Literal ID="ltQuotation" runat="server"></asp:Literal>
                <asp:Literal ID="ltAddress" runat="server"></asp:Literal>
                <asp:Literal ID="ltConfirm" runat="server"></asp:Literal>
            </ol>
        </div>
    </div>
</div>
