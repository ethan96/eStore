<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="myorders.aspx.cs" Inherits="eStore.UI.Cart.myorders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_My_Orders)%> </h1>
    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_OrderNo)%>:
    <eStore:TextBox ID="tb_orderNo" runat="server" Width="168px" ValidationGroup="vgOrderNumber" CssClass="textEntry InputValidation"></eStore:TextBox>
    &nbsp;<asp:Button ID="bt_searchOrder" runat="server" Text="Search" OnClick="bt_searchOrder_Click" CssClass="btnStyle"
        ValidationGroup="vgOrderNumber" />
    <asp:RegularExpressionValidator ID="revOrder" runat="server" ControlToValidate="tb_orderNo" ValidationExpression="[a-zA-Z0-9]+"
        ErrorMessage="Invalid Order Number" ValidationGroup="vgOrderNumber"></asp:RegularExpressionValidator>
    <asp:GridView ID="gvmyorders" runat="server" CssClass="estoretable" 
        AutoGenerateColumns="False" GridLines="None">
        <Columns>
            <asp:TemplateField HeaderText=" Order No">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" 
                        NavigateUrl='<%# Eval("OrderNo", "~/Account/orderdetail.aspx?orderid={0}") %>' 
                        Text='<%# Eval("OrderNo") %>'></asp:HyperLink>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_No)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount">
                <ItemTemplate>
                    <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("totalAmountX"), Eval("currencySign").ToString())%>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Amount)%>
                </HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Order date">
                <ItemTemplate>
                    <%#eStore.Presentation.eStoreLocalization.Date(Eval("OrderDate"))%>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%>
                </HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
