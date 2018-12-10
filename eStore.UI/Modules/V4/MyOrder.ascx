<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyOrder.ascx.cs" 
    Inherits="eStore.UI.Modules.V4.MyOrder" %>
    <div class="eStore_account_att"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Modify_Recent_Order)%></div>
    <div class="eStore_account_msg">
        <div class="eStore_account_msgBlock">
            <div class="eStore_account_msgLeft searchNoBlock">
                <p><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Search_by_order_no)%></p>
                <eStore:TextBox ID="tb_orderNo" runat="server" ValidationGroup="vgOrderNumber" placeholder=""></eStore:TextBox>
                <p><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Search_by_period)%></p>
                <asp:DropDownList ID="ddl_period" runat="server" CssClass="styled" name="period"></asp:DropDownList>
                <div class="eStore_order_btnBlock">
                    <asp:LinkButton ID="lb_searchOrder" runat="server" CssClass="eStore_btn" ValidationGroup="vgOrderNumber" OnClick="lb_searchOrder_Click"></asp:LinkButton><br />
                    <asp:RegularExpressionValidator ID="revOrder" runat="server" ControlToValidate="tb_orderNo" ValidationExpression="[a-zA-Z0-9]+"
                        ErrorMessage="Invalid Order Number" ValidationGroup="vgOrderNumber" BackColor="Red"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="eStore_account_msgRight">
                <eStore:Repeater ID="rpMyOrder" runat="server" OnItemCommand="rpMyOrder_OnItemCommand">
                    <HeaderTemplate>
                        <div class="searchTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></div>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                            <thead>
                                <tr>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_No)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Amount)%></th>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></th>
                                    <% if (this.StatusVisible)
                                       { %>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Status)%></th>
                                    <% } %>
                                    <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_ReOrder)%></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                                <tr>
                                    <td><a href='<%#Eval("OrderNoUrl") %>' title='<%#Eval("OrderNo") %>' ><%#Eval("OrderNo") %></a></td>
                                    <td><%#Eval("OrderDate")%></td>
                                    <td><%#Eval("SubTotal")%></td>
                                    <td><%#Eval("ShipTo")%></td>
                                    <% if (this.StatusVisible)
                                       { %>
                                    <td><%#Eval("Status")%></td>
                                    <% } %>
                                    <td><asp:LinkButton ID="lb_ReOrder" runat="server" CommandName="ReOrder" CommandArgument='<%#Eval("OrderNo") %>' Text='<%#this.ReOrder %>'></asp:LinkButton></td>
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
                    