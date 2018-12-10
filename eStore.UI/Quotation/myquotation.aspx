<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="myquotation.aspx.cs" Inherits="eStore.UI.Quotation.myquotation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_My_Quote)%>
    </h1>
    <div>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%>
        :<eStore:TextBox ID="tb_searchQuoteNo" runat="server" Width="180px" CssClass="textEntry InputValidation"
            ValidationGroup="vgQuotationNumber"></eStore:TextBox>
        &nbsp;<eStore:Button ID="bt_searchQuote" runat="server" Text="Search" OnClick="bt_searchQuote_Click" CssClass="btnStyle"
            ValidationGroup="vgQuotationNumber" />
        <asp:RegularExpressionValidator ID="revQuotation" runat="server" ControlToValidate="tb_searchQuoteNo"
            ValidationExpression="[a-zA-Z0-9]+" ErrorMessage="Invalid Quotation Number" ValidationGroup="vgQuotationNumber"></asp:RegularExpressionValidator>
    </div>
    <asp:GridView ID="gvmyquotation" runat="server" CssClass="estoretable" AutoGenerateColumns="False"  GridLines="None"
        OnRowCommand="gvmyquotation_RowCommand" OnRowDataBound="gvmyquotation_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Quotation Number">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnreviewQuotation" Text='<%# Eval("QuotationNumber") %>' CommandArgument='<%# Eval("QuotationNumber") %>'
                        CommandName="reviewQuotation" runat="server"></asp:LinkButton>
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
                        CommandName="QuotationRevise" />
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <h1 id="mytransferedHeader" runat="server">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_My_Transfered_Quotation)%>
    </h1>
    <asp:GridView ID="gvTransfered" runat="server" CssClass="estoretable" AutoGenerateColumns="False"  GridLines="None"
        OnRowDataBound="gvmyquotation_RowDataBound" OnRowCommand="gvmyquotation_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="QuotationNumber">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnreviewQuotation" Text='<%# Eval("QuotationNumber") %>' CommandArgument='<%# Eval("QuotationNumber") %>'
                        CommandName="reviewQuotation" runat="server"></asp:LinkButton>
                    <eStore:Repeater ID="rpRelatedOrders" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                           <li><a href="/Account/orderdetail.aspx?orderid=<%# Eval("OrderNo") %>">
                                <%# Eval("OrderNo") %></a></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul></FooterTemplate>
                    </eStore:Repeater>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="totalAmount">
                <ItemTemplate>
                    <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("totalAmountX"))%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Amount)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Label ID="lbStatus" runat="server" Text='<%# Bind("statusX") %>'></asp:Label>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Status)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="QuoteDate">
                <ItemTemplate>
                    <%#eStore.Presentation.eStoreLocalization.Date(Eval("QuoteDate"))%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Created_Date)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ExpiredDate">
                <ItemTemplate>
                    <%#eStore.Presentation.eStoreLocalization.Date(Eval("QuoteExpiredDate"))%>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="UserID">
                <ItemTemplate>
                    <asp:Label ID="lbUserID" runat="server" Text='<%# Bind("UserID") %>'></asp:Label>
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Revise">
                <ItemTemplate>
                    <asp:Button ID="bt_TQuotate" runat="server" Text="Revise" CommandArgument='<%# Eval("QuotationNumber") %>' CssClass="btnStyle"
                        CommandName="QuotationRevise" />
                </ItemTemplate>
                <HeaderTemplate><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
