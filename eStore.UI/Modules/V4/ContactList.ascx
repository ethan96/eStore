<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactList.ascx.cs"
     Inherits="eStore.UI.Modules.V4.ContactList" %>
<asp:DropDownList ID="ddl_CompanyName" runat="server" CssClass="styled"></asp:DropDownList>
<eStore:Repeater ID="rpContact" runat="server"  OnItemDataBound="rpContact_ItemDataBound" OnItemCommand="rpContact_OnItemCommand">
    <HeaderTemplate>
        <ol id="AddressSelect">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="show" name='<%#Eval("AttCompanyName") %>'>
            <div class="input"><input type="radio" name="AddressBook" value='<%#Eval("AddressID") %>' /></div>
            <div class="list">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td><b><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name) %>：</b><%#Eval("FirstName").ToString()%></td>
                        <td><b><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name) %>：</b><%#Eval("LastName").ToString()%></td>
                    </tr>
                    <tr>
                        <td colspan="2"><b><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address) %>：</b><%#Eval("Address1").ToString()%></td> 
                    </tr>
                </table>
            </div>
            <div class="action">
                <asp:LinkButton ID="lb_Delete" runat="server" CommandName="Delete" CommandArgument='<%#Eval("AddressID") %>' OnClientClick="return confirm('Are you sure?')"></asp:LinkButton>
                <asp:Literal runat="server" ID="lt_Default" Visible="false"></asp:Literal>
            </div>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ol>
    </FooterTemplate>
</eStore:Repeater>
<script type="text/javascript" language="javascript">
    $(function () {
        $("select[id*='ddl_CompanyName']").bind("change", function () {
            var val = $("select[id*='ddl_CompanyName']").val();
            $("#AddressSelect li").each(function () {
                if (val == "All") {
                    $(this).show();
                }
                else if ($(this).attr("name") == val) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            });
        });
    });
</script>