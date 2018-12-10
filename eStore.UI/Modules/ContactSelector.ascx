<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactSelector.ascx.cs"
    Inherits="eStore.UI.Modules.ContactSelector" %>
<%@ Register Src="SAPContatSelector.ascx" TagName="SAPContatSelector" TagPrefix="eStore" %>
<%@ Register Src="ContactDetails.ascx" TagName="ContactDetails" TagPrefix="eStore" %>
<div id="contact-context" class="eStore_orderStep2">
<table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_order eStore_orderStep2">
    <tr>
        <th width="30">
            <asp:Literal ID="ltAddress" runat="server"></asp:Literal>
        </th>
    </tr>
    <tr>
        <td>
            <div class="eStore_ShippingStyle">
                <asp:Literal ID="ltBillTo" runat="server"></asp:Literal></div>
            <div class="eStore_information">
                <div class="eStore_ShippingCompany">
                    <asp:DropDownList ID="ddlBillCompany" runat="server" ClientIDMode="Static" CssClass="styled">
                    </asp:DropDownList>
                </div>
                <div class="eStore_ShippingPersonal" lb="hIsBillto">
                    <asp:Repeater ID="rpContactInforBill" runat="server" OnItemDataBound="rpContactInfor_ItemDataBound"
                        OnItemCommand="rpContactInfor_ItemComm">
                        <ItemTemplate>
                            <div companyfor='<%# Eval("AttCompanyName") %>' class="eStore_ShippingPersonal_msg">
                                <div class="radio">
                                    <input type="radio" name="BillTo" <%#(bool)Eval("isBillTo")?"checked=\"checked\"":"" %>
                                        <%#eStore.Presentation.eStoreContext.Current.Store.isValidatedBilltoAddress(Eval("CountryCode").ToString(), eStore.Presentation.eStoreContext.Current.User)?String.Empty:"class=\"invalidbillingaddress\"" %>
                                        value='<%#Eval("AddressID") %>' id='<%#"BillTo" + Eval("AddressID") %>' title='<%#Eval("Country") %>'
                                        <%# eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableVATSetting")? string.Format(" vatNumber='{0}' vatvalidstatus='{1}' ",Eval("VATNumbe"),(int)Eval("VATValidStatus")):string.Empty %> /></div>
                                <div class="info">
                                    <div class="name">
                                        <%#Eval("Attention").ToString()%></div>
                                    <p>
                                        <asp:Literal ID="ltAddressInfor" runat="server"></asp:Literal></p>
                                    <p>
                                        <asp:Literal ID="ltPhoneInfor" runat="server"></asp:Literal></p>
                                </div>
                                <div class="action">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Edit" CommandName="editContact" CssClass="deleteButton padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Edit)%>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDel" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Delete" CommandName="deleteContact" CssClass="deleteButton padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Delete)%>
                                    </asp:LinkButton></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="eStore_addReceiver">
                        <span contactType="Bill" class="btn showAddress fancybox"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Add_a_new_billing_address)%></span>
                    </div>
                    <!--Add a receiver-->
                </div>
                <!--eStore_ShippingPersonal-->
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div class="eStore_ShippingStyle">
                <asp:Literal ID="ltShipTo" runat="server"></asp:Literal></div>
            <div class="eStore_information">
                <div class="eStore_sameAddress">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Same_as_Bill_to_Address)%>
                    <input type="radio" name="sameAddress" class="sameAddressYes" checked="checked" value="true" hflb="hdShipTo" />Yes
                    <input type="radio" name="sameAddress" value="false" hflb="hdShipTo" />No
                    <asp:HiddenField ID="hdShipTo" ClientIDMode="Static" runat="server" />
                </div>
                <div class="eStore_ShippingCompany">
                    <asp:DropDownList ID="ddlShipCompany" runat="server" ClientIDMode="Static" CssClass="styled">
                    </asp:DropDownList>
                </div>
                <div class="eStore_ShippingPersonal ContactSelector" lb="hIsShippto">
                    <asp:Repeater ID="rpContactInforShip" runat="server" OnItemDataBound="rpContactInfor_ItemDataBound"
                        OnItemCommand="rpContactInfor_ItemComm">
                        <ItemTemplate>
                            <div companyfor='<%# Eval("AttCompanyName") %>' class="eStore_ShippingPersonal_msg">
                                <div class="radio">
                                    <input type="radio" name="ShipTo" <%#(bool)Eval("isShipTo")?"checked=\"checked\"":"" %>
                                        <%#eStore.Presentation.eStoreContext.Current.Store.isValidatedShiptoAddress(Eval("CountryCode").ToString(), eStore.Presentation.eStoreContext.Current.User)?String.Empty:"class=\"invalidshippingaddress\"" %>
                                        value='<%#Eval("AddressID") %>' id='<%#"ShipTo" + Eval("AddressID") %>' /></div>
                                <div class="info bg">
                                    <div class="name">
                                        <%#Eval("Attention").ToString()%></div>
                                    <p>
                                        <asp:Literal ID="ltAddressInfor" runat="server"></asp:Literal></p>
                                    <p>
                                        <asp:Literal ID="ltPhoneInfor" runat="server"></asp:Literal></p>
                                </div>
                                <div class="action">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Edit" CommandName="editContact" CssClass="deleteButtonGrey padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Edit)%>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDel" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Delete" CommandName="deleteContact" CssClass="deleteButtonGrey padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Delete)%>
                                    </asp:LinkButton></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="eStore_addReceiver">
                        <span contactType="Ship" class="btn showAddress fancybox"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Add_a_new_shipping_address)%></span>
                    </div>
                    <!--Add a receiver-->
                </div>
                <!--eStore_ShippingPersonal-->
            </div>
        </td>
    </tr>
    <asp:Panel ID="psoldto" runat="server">
        <tr>
        <td>
            <div class="eStore_ShippingStyle">
                <asp:Literal ID="ltSoldTo" runat="server"></asp:Literal></div>
            <div class="eStore_information">
                <div class="eStore_sameAddress">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Same_as_Bill_to_Address)%>
                    <input type="radio" name="soldSameAddress" class="sameAddressYes" checked="checked" value="true" hflb="hfSoldTo" />Yes
                    <input type="radio" name="soldSameAddress" value="false" hflb="hfSoldTo" />No
                    <asp:HiddenField ID="hfSoldTo" ClientIDMode="Static" runat="server" />
                </div>
                <div class="eStore_ShippingCompany">
                    <asp:DropDownList ID="ddlSoldCompany" ClientIDMode="Static" runat="server" CssClass="styled">
                    </asp:DropDownList>
                </div>
                <div class="eStore_ShippingPersonal" lb="hIsSoldto">
                    <asp:Repeater ID="rpContactInforSold" runat="server" OnItemDataBound="rpContactInfor_ItemDataBound"
                        OnItemCommand="rpContactInfor_ItemComm">
                        <ItemTemplate>
                            <div companyfor='<%# Eval("AttCompanyName") %>' class="eStore_ShippingPersonal_msg">
                                <div class="radio">
                                    <input type="radio" name="SoldTo" <%#(bool)Eval("isSoldTo")?"checked=\"checked\"":"" %>
                                        value='<%#Eval("AddressID") %>' id='<%#"SoldTo" + Eval("AddressID") %>' />
                                </div>
                                <div class="info">
                                    <div class="name">
                                        <%#Eval("Attention").ToString()%></div>
                                    <p>
                                        <asp:Literal ID="ltAddressInfor" runat="server"></asp:Literal></p>
                                    <p>
                                        <asp:Literal ID="ltPhoneInfor" runat="server"></asp:Literal></p>
                                </div>
                                <div class="action">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Edit" CommandName="editContact" CssClass="deleteButton padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Edit)%>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDel" runat="server" CommandArgument='<%# Bind("AddressID") %>'
                                        Text="Delete" CommandName="deleteContact" CssClass="deleteButton padding3">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Delete)%>
                                    </asp:LinkButton></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="eStore_addReceiver">
                        <span contactType="Sold" class="btn showAddress fancybox"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Add_a_new_sold_to_address)%></span>
                    </div>
                    <!--Add a receiver-->
                </div>
                <!--eStore_ShippingPersonal-->
            </div>
        </td>
    </tr>
    </asp:Panel>
</table>
<!--eStore_information-->
<div class="eStore_addReceiverBlock" id="dContactDetailsBill">
    <eStore:ContactDetails ID="ContactDetailsBill" runat="server" ValidationGroup="ContactDetails" />
    <div class="eStore_order_btnBlock">
        <asp:Button ID="btSaveBillto" runat="server" OnClientClick="return CheckValidate('#dContactDetailsBill')" Text="Save" CssClass="eStore_btn" OnClick="btnSaveUserContact_Click" />
        <a href="#" class="eStore_btn borderBlue btnCancel"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Cancel)%></a> 
        <a class="eStore_btn borderBlue mousehand">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Clear)%></a>
        <asp:HiddenField ID="hdContactType" runat="server" ClientIDMode="Static" />
    </div>
</div>
<!--eStore_addReceiverBlock--></div>
<div class="bottomHide">
    <asp:HiddenField ID="hcontactType" runat="server" EnableViewState="false" ClientIDMode="Static" />
    <asp:HiddenField ID="hAddressID" runat="server" EnableViewState="false" ClientIDMode="Static" />
    <asp:HiddenField ID="hDetailVATValidStatus" runat="server" EnableViewState="false"
        ClientIDMode="Static" />
    <asp:HiddenField ID="hIsShippto" runat="server" EnableViewState="false" ClientIDMode="Static" />
    <asp:HiddenField ID="hIsBillto" runat="server" EnableViewState="false" ClientIDMode="Static" />
    <asp:HiddenField ID="hIsSoldto" runat="server" EnableViewState="false" ClientIDMode="Static" />
</div>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        //show address
        $(".fancybox").click(function () {
            $("#hAddressID").val("");
            clearAddressPopInfor();
            showAddress($(this).attr("contactType"));
        });
        //select default address
        $(".eStore_table_order input[type=radio]:checked").parents(".eStore_ShippingPersonal_msg").addClass("show");

        //change company
        $(".eStore_ShippingCompany select").change(function () {
            var selectValue = $(this).val();
            var crls = $(this).parents(".eStore_information").find(".eStore_ShippingPersonal_msg").addClass("hiddenitem").filter("[companyfor='" + selectValue + "']").removeClass("hiddenitem");
            var currentitem = crls.find("input[type='radio']:checked");
            if (currentitem.length == 0)
                currentitem = crls.find("input[type='radio']").first();
            if (currentitem.length != 0)
                currentitem.click();
        }).change();

        //reset contact with hiden textbox value
        $(".eStore_ShippingPersonal[lb]").each(function () {
            var lb = $(this).attr("lb");
            var item = $("#" + lb).val();
            if (item != "") {
                $(this).find("input[type=radio][value='" + item + "']").click();
            }

            $("#" + lb).val($(this).find("input[type=radio]:checked").val());
            $(this).find("input[type=radio]").bind("change", function () {
                $("#" + lb).val($(this).val());
            });
        });

        $(".eStore_addReceiverBlock .colspan2 .eStore_contactUs_input").after($("<div />").addClass("clearfix"));

        if ($("#hdShipTo").val() != "")
            $("input:radio[hflb='hdShipTo'][value='" + $("#hdShipTo").val() + "']").click();
        if ($("#hfSoldTo").val() != "")
            $("input:radio[hflb='hfSoldTo'][value='" + $("#hfSoldTo").val() + "']").click();

        //clear adress detail text
        $(".mousehand").click(function (i, n) {
            clearAddressPopInfor();
        });

        //reset company by current contact
        $(".eStore_ShippingCompany").each(function (i, n) {
            var companyName = $(n).next("div[lb]").find("input[type=radio]:checked").parents("div[companyfor]").attr("companyfor");
            var select = $(n).find("select.styled");
            if (companyName != select.val()) {
                select.find("option").removeAttr("selected");
                select.val(companyName);
                //select.find("option[value='" + companyName + "']").attr("selected", true);
                select.trigger("change"); //
            }
        });
    });

    function clearAddressPopInfor() {
        var addTable = $(".AddressBook_personal");
        addTable.find("input[type='text']").val("");
    }

    //show contact address detail table
    function showAddress(lable) {
        $("#hdContactType").val(lable || "");
        $.fancybox.open("#dContactDetailsBill", {
            modal: true,
            parent: "#contact-context",
            afterClose: function () { clearAddressPopInfor(); }
        });
    }
    $(".eStore_addReceiverBlock .btnCancel").click(function () {
        $.fancybox.close();
        return false;
    });
</script>
