<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="orderdetail.aspx.cs" Inherits="eStore.UI.Cart.orderdetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1 class="pagetitle">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Detail)%>
    </h1>
    <div class="printandemail">
        <asp:HyperLink NavigateUrl="~/Cart/myorders.aspx" runat="server" CssClass="productcompare" ID="lMyorders" Text="My Orders"></asp:HyperLink>|
        <asp:HyperLink NavigateUrl="~/Cart/printorder.aspx" runat="server" ID="hprintorder" Target="_blank"
            Text="printorder" CssClass="productprint"></asp:HyperLink>
    </div>
    <div class="clear"></div>
    <eStore:OrderInvoiceDetail ID="OrderInvoiceDetail1" runat="server" />
</asp:Content>
