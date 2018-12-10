<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyQuote.ascx.cs" 
    Inherits="eStore.UI.Modules.V4.MyQuote" %>
    <div class="eStore_account_att"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Modify_Recent_Quote)%></div>
    <div class="eStore_account_msg">
        <div class="eStore_account_msgBlock">
            <div class="eStore_account_msgLeft searchNoBlock">
               <p><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Search_by_quote_no)%></p>
                <eStore:TextBox ID="tb_searchQuoteNo" runat="server" ValidationGroup="vgQuotationNumber" placeholder=""></eStore:TextBox>
                <p><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Search_by_period)%></p>
                <asp:DropDownList ID="ddl_period" runat="server" CssClass="styled" name="period"></asp:DropDownList>
                <div class="eStore_order_btnBlock">
                    <asp:LinkButton ID="lb_searchQuote" runat="server" CssClass="eStore_btn" OnClick="lb_searchQuote_Click" ValidationGroup="vgQuotationNumber"></asp:LinkButton><br />
                    <asp:RegularExpressionValidator ID="revQuotation" runat="server" ControlToValidate="tb_searchQuoteNo"
                        ValidationExpression="[a-zA-Z0-9]+" ErrorMessage="Invalid Quotation Number" ValidationGroup="vgQuotationNumber"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="eStore_account_msgRight">
                <eStore:Repeater ID="rpMyQuote" runat="server" OnItemDataBound="rpMyQuote_ItemDataBound" OnItemCommand="rpMyQuote_OnItemCommand">
                    <HeaderTemplate>
                        <div class="searchTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></div>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                            <thead>
                                <tr>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Created_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Amount) %></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Status)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                                <tr>
                                    <td><asp:LinkButton ID="lb_Review" runat="server" CommandName="Review" CommandArgument='<%#Eval("QuoteNo") %>' Text='<%#Eval("QuoteNo")%>'></asp:LinkButton></td>
                                    <td><%#Eval("QuoteDate")%></td>
                                    <td><%#Eval("QuoteExpiredDate")%></td>
                                    <td><%#Eval("SubTotal")%></td>
                                    <td><%#Eval("ShipTo")%></td>
                                    <td><%#Eval("Status")%>
                                        <eStore:Repeater ID="rpRelatedOrders" runat="server">
                                            <HeaderTemplate>
                                                <ul>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li><a href="<%=esUtilities.CommonHelper.GetStoreLocation() %>Account/orderdetail.aspx?orderid=<%# Eval("OrderNo") %>">
                                                    <%# Eval("OrderNo") %></a></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </eStore:Repeater>
                                    </td>
                                    <td><asp:LinkButton ID="lb_Revise" runat="server" CommandName="Revise" CommandArgument='<%#Eval("QuoteNo") %>' Text='<%#Eval("QuoteAction")%>'></asp:LinkButton></td>
                                </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </eStore:Repeater>
                <eStore:Repeater ID="rpMyTrQuote" runat="server" OnItemDataBound="rpMyQuote_ItemDataBound" OnItemCommand="rpMyQuote_OnItemCommand">
                    <HeaderTemplate>
                        <div class="searchTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_My_Transfered_Quotation)%></div>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                            <thead>
                                <tr>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Created_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Amount) %></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Status)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                                <tr>
                                    <td><asp:LinkButton ID="Review" runat="server" CommandName="Review" CommandArgument='<%#Eval("QuoteNo") %>' Text='<%#Eval("QuoteNo")%>'></asp:LinkButton></td>
                                    <td><%#Eval("QuoteDate")%></td>
                                    <td><%#Eval("QuoteExpiredDate")%></td>
                                    <td><%#Eval("SubTotal")%></td>
                                    <td><%#Eval("ShipTo")%></td>
                                    <td><%#Eval("Status")%>
                                        <eStore:Repeater ID="rpRelatedOrders" runat="server">
                                            <HeaderTemplate>
                                                <ul>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li><a href="/Account/orderdetail.aspx?orderid=<%# Eval("OrderNo") %>">
                                                    <%# Eval("OrderNo") %></a></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </eStore:Repeater>
                                    </td>
                                    <td><asp:LinkButton ID="Revise" runat="server" CommandName="Revise" CommandArgument='<%#Eval("QuoteNo") %>' Text='<%#Eval("QuoteAction")%>'></asp:LinkButton></td>
                                </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </eStore:Repeater>
                <eStore:Repeater ID="rpEqQuote" runat="server" OnItemCommand="rpMyQuote_OnItemCommand">
                    <HeaderTemplate>
                        <div class="searchTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Equotation_Quote)%></div>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                            <thead>
                                <tr>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Created_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Amount) %></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Status)%></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                                <tr>
                                    <td><asp:LinkButton ID="Review" runat="server" CommandName="Equotation" CommandArgument='<%#Eval("QuotationID") %>' Text='<%#Eval("QuoteNo")%>'></asp:LinkButton></td>
                                    <td><%#Eval("QuoteDate")%></td>
                                    <td><%#Eval("QuoteExpiredDate")%></td>
                                    <td><%#Eval("SubTotal")%></td>
                                    <td><%#Eval("ShipTo")%></td>
                                    <td><%#Eval("Status")%></td>
                                </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </eStore:Repeater>
            </div>
        </div>
    </div>