<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" 
CodeBehind="equotation.aspx.cs" Inherits="eStore.UI.equotation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_My_Quote)%>
    </h1>
    <asp:GridView ID="gvmyquotation" runat="server" CssClass="estoretable" AutoGenerateColumns="False"  GridLines="None"
        OnRowCommand="gvmyquotation_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="Quotation Number">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnreviewQuotation" Text='<%# Eval("CustomID") %>' CommandArgument='<%# Eval("QuotationID") %>'
                        CommandName="FromeQuotation" runat="server"></asp:LinkButton>
                    <eStore:Repeater ID="rpRelatedOrders" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li><a href="<%=esUtilities.CommonHelper.GetStoreLocation() %>Account/orderdetail.aspx?orderid=<%# Eval("OrderNo") %>">
                                <%# Eval("OrderNo") %></a></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul></FooterTemplate>
                    </eStore:Repeater>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount">
                <ItemTemplate>
                    <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("totalAmountX"),Eval("currencySign").ToString())%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Amount)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lbStatus" runat="server" Text='<%# Bind("statusX") %>'></asp:Label>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Status)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Created Date">
                <ItemTemplate>
                    <%#eStore.Presentation.eStoreLocalization.Date(Eval("QuoteDate"))%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Created_Date)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Expires on">
                <ItemTemplate>
                    <%#eStore.Presentation.eStoreLocalization.Date(Eval("QuoteExpiredDate"))%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expires_on)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Revise ">
                <ItemTemplate>
                    <asp:Button ID="bt_QuoteRevise" runat="server" Text="Revise" CommandArgument='<%# Eval("QuotationNumber") %>' CssClass="btnStyle"
                        CommandName="QuotationRevise" Visible="false" />
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>

