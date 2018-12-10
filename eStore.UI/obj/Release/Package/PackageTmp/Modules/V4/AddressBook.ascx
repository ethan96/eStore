<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressBook.ascx.cs" 
    Inherits="eStore.UI.Modules.V4.AddressBook" %>
    <%@ Register Src="~/Modules/V4/ContactList.ascx" TagName="ContactList" TagPrefix="eStore" %>
    <%@ Register Src="~/Modules/ContactDetails.ascx" TagName="ContactDetails" TagPrefix="eStore"  %>
    <div class="eStore_account_att"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_The_fields_indicated_with)%></div>
    <div class="eStore_account_msg">
        <div class="eStore_account_msgBlock">
            <div class="eStore_account_msgLeft"><h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Account_Settings)%></h2></div>
            <div class="eStore_account_msgRight">
                <div class="eStore_account_msgBlock_content AccountSettings">
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eMail) %>：</th>
                            <td colspan="3"><%=CurrentUser.UserID %></td>
                        </tr>
                        <tr>
                            <th width="120"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name) %>：</th>
                            <td width="200"><%=CurrentUser.mainContact.FirstName%></td>
                            <th width="80"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name) %>：</th>
                            <td><%=CurrentUser.mainContact.LastName %></td>
                        </tr>
                        <tr>
                            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Company_Name) %>：</th>
                            <td><%=CurrentUser.mainContact.AttCompanyName%></td>
                            <th><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country) %>：</th>
                            <td><%=CurrentUser.mainContact.Country%></td>
                        </tr>
                    </table>
                    <div class="eStore_order_btnBlock">
                        <asp:LinkButton ID="lb_EditProfile" runat="server" Text="Edit" CssClass="eStore_btn"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
        <div class="eStore_account_msgBlock">
            <div class="eStore_account_msgLeft"><h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Address_Book)%></h2></div>
            <div class="eStore_account_msgRight">
                <div class="eStore_account_msgBlock_content AddressBookList">
                    <eStore:ContactList ID="ContactList1" runat="server" />
                    <eStore:ContactDetails ID="ContatcDetails1" runat="server" />
                    <div class="eStore_order_btnBlock">
                        <asp:LinkButton ID="lb_EditAddressBook" runat="server" Text="Edit" CssClass="eStore_btn" OnClick="lb_EditAddressBook_Click" OnClientClick="return CheckRadio()"></asp:LinkButton>
                        <asp:LinkButton ID="lb_AddAddressBook" runat="server" Text="Add" CssClass="eStore_btn" OnClick="lb_AddAddressBook_Click"></asp:LinkButton>
                        <asp:LinkButton ID="lb_AddorUpdate" runat="server" Text="AddorUpdate" CssClass="eStore_btn" OnClick="lb_AddorUpdate_Click" OnClientClick="return CheckValidate()"></asp:LinkButton>
                        <asp:LinkButton ID="lb_Cancel" runat="server" Text="Cancel" CssClass="eStore_btn" OnClick="lb_Cancel_Click"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript" language="javascript">
        function CheckRadio() {
            if ($("input[name=AddressBook]:checked").val() == null) {
                alert($.eStoreLocalizaion("Please_select_item"));
                return false;
            }
        }
    </script>