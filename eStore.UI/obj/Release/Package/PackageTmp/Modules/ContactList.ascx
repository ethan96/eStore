<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactList.ascx.cs"
    Inherits="eStore.UI.Modules.ContactList" %>
<%@ Register Src="ContactDetails.ascx" TagName="ContactDetails" TagPrefix="eStore" %>
<div class="eStore_information">
                    <div class="eStore_ShippingCompany">
                        <select class="styled" name="ShipCompany">
                        </select>
                    </div>
                    <div class="eStore_ShippingPersonal">
                        <asp:Repeater ID="rpContactInfor" runat="server" OnItemDataBound="rpContactInfor_ItemDataBound">
                        <ItemTemplate>
                            <div class="eStore_ShippingPersonal_msg show">
                                <div class="radio">
                                    <input type="radio" name="Ship" checked="checked" /></div>
                                <div class="info">
                                    <div class="name">
                                        <%#Eval("Attention").ToString()%></div>
                                    <p>
                                        <asp:Literal ID="ltAddressInfor" runat="server"></asp:Literal></p>
                                    <p>
                                        <asp:Literal ID="ltPhoneInfor" runat="server"></asp:Literal></p>
                                </div>
                                <div class="action">
                                    <a href="#"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Edit)%></a>
                                    <a href="#"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Delete)%></a></div>
                            </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="eStore_addReceiver">
                            <span class="btn"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Add_a_receiver)%></span>
                        </div>
                        <!--Add a receiver-->
                    </div>
                    <!--eStore_ShippingPersonal-->
                </div>
    <!--eStore_information-->
    <div class="eStore_addReceiverBlock">
        <eStore:ContactDetails ID="ContactDetails1" runat="server" ValidationGroup="ContactDetails" />
    </div>
<!--eStore_addReceiverBlock-->
<script type="text/javascript" language="javascript">
    $(".invalidshippingaddress").change(function () {

        if ($(this).attr("checked")) {
            $(this).removeAttr("checked");
            alert($.eStoreLocalizaion("You_have_entered_a_Shipping_Address_that_this_region_s_eStore_does_not_support"));
            return false;
        }
    });
    $(".invalidbillingaddress").change(function () {

        if ($(this).attr("checked")) {
            $(this).removeAttr("checked");
            alert($.eStoreLocalizaion("You_have_entered_a_Billing_Address_that_this_region_s_eStore_does_not_support"));
            return false;
        }
    });
</script>
