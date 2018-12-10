<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" 
    CodeBehind="MyQuote.aspx.cs" Inherits="eStore.UI.Account.MyQuote" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/account")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/account")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="eStore_breadcrumb eStore_block980">
    	<a href='<%= ResolveUrl("~/") %>'><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home) %></a>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account) %>
    </div>
    <div class="eStore_account_content">
        <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account) %></h2>
        <div class="eStore_account_link">
            <div class="eStore_account_linkName"><span data-bind="text: account().WelcomeName"></span></div>
            <a href="account_setting.html">Settings</a>
            <a href="MyOrder.aspx"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a>
            <a class="on"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a>
        </div>
        <div class="eStore_account_att">View &amp; Modify Recent Quote</div>
        <div class="eStore_account_msg">
            <div class="eStore_account_msgBlock">
                <div class="eStore_account_msgLeft searchNoBlock">
                    <p>Search by quote no.</p>
                    <input id="searchNo" type="text" placeholder="" />
                    <p>Search by period</p>
                    <select class="styled" name="period">
                          <option value="0" selected="selected"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select)%></option>
                          <option value="1">1-3 month</option>
                          <option value="2">4-6 month</option>
                    </select>
                    <div class="eStore_order_btnBlock">
                        <a href="#" class="eStore_btn" data-bind="click: searchquote"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Search)%></a>
                    </div>
                </div>
                <div id="MyQuote" class="eStore_account_msgRight">
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
                        <tbody data-bind="foreach: account().Quotes()">
                            <tr>
                                <td><a data-bind="attr: { href: QuoteNoUrl, title: QuoteNo}"><span data-bind="text: QuoteNo"></span></a></td>
                                <td><span data-bind="text: QuoteDate"></span></td>
                                <td><span data-bind="text: QuoteExpiredDate"></span></td>
                                <td><span data-bind="text: SubTotal"></span></td>
                                <td><span data-bind="text: ShipTo"></span></td>
                                <td><span data-bind="text: Status"></span>
                                       <ul data-bind="foreach: Orders">
                                            <li><a data-bind="attr: { href: OrderNoUrl, title: OrderNo}"><span data-bind="text: OrderNo"></span></a></li>
                                       </ul>
                                </td>
                                <td><a data-bind="attr: { href: QuoteReviseUrl, title: QuoteAction}"><span data-bind="text: QuoteAction"></span></a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div id="MyTquote" class="eStore_account_msgRight">
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
                        <tbody data-bind="foreach: account().tQuotes()">
                            <tr>
                                <td><a data-bind="attr: { href: QuoteNoUrl, title: QuoteNo}"><span data-bind="text: QuoteNo"></span></a></td>
                                <td><span data-bind="text: QuoteDate"></span></td>
                                <td><span data-bind="text: QuoteExpiredDate"></span></td>
                                <td><span data-bind="text: SubTotal"></span></td>
                                <td><span data-bind="text: ShipTo"></span></td>
                                <td><span data-bind="text: Status"></span>
                                    <ul data-bind="foreach: Orders">
                                        <li><a data-bind="attr: { href: OrderNoUrl, title: OrderNo}"><span data-bind="text: OrderNo"></span></a></li>
                                    </ul>
                                </td>
                                <td><a data-bind="attr: { href: QuoteReviseUrl, title: QuoteAction}"><span data-bind="text: QuoteAction"></span></a></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>

