<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="MyOrder.aspx.cs" Inherits="eStore.UI.Account.MyOrder"%>
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
            <a class="on"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a>
            <a href="MyQuote.aspx"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a>
        </div>
        <div class="eStore_account_att">View &amp; Modify Recent Order</div>
        <div class="eStore_account_msg">
            <div class="eStore_account_msgBlock">
                <div class="eStore_account_msgLeft searchNoBlock">
                    <p>Search by order no.</p>
                    <input id="searchNo" type="text" placeholder="" />
                    <p>Search by period</p>
                    <select class="styled" name="period">
                          <option value="0" selected="selected"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select)%></option>
                          <option value="1">1-3 month</option>
                          <option value="2">4-6 month</option>
                    </select>
                    <div class="eStore_order_btnBlock">
                        <a href="#" class="eStore_btn" data-bind="click: searchorder"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search)%></a>
                    </div>
                </div>
                <div id="MyOrder" class="eStore_account_msgRight">
                    <div class="searchTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></div>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_thHight">
                        <thead>
                            <tr>
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_No)%></th>
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%></th>
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Amount)%></th>
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_UserID)%></th><!--TODO-->
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Status)%></th>
                                <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></th><!--TODO-->
                            </tr>
                        </thead>
                        <tbody data-bind="foreach: account().Orders()">
                            <tr>
                                <td><a target="_blank" data-bind="attr: { href: OrderNoUrl, title: OrderNo}"><span data-bind="text: OrderNo"></span></a></td>
                                <td><span data-bind="text: OrderDate"></span></td>
                                <td><span data-bind="text: SubTotal"></span></td>
                                <td><span data-bind="text: ShipTo"></span></td>
                                <td><span data-bind="text: Status"></span></td>
                                <td><a href="../Cart/Cart.aspx"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)%></a></td><!--TODO-->
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
