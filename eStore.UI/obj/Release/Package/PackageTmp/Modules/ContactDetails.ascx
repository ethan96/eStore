<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactDetails.ascx.cs"
    Inherits="eStore.UI.Modules.ContactDetails" %>
<%@ Register Src="~/Modules/CountrySelector.ascx" TagName="CountrySelector" TagPrefix="eStore" %>
<%@ Register Src="CultureFullName.ascx" TagName="CultureFullName" TagPrefix="eStore" %>
<div class="title">
    <asp:Literal ID="ltTitle" runat="server"></asp:Literal></div>
<table border="0" cellspacing="0" cellpadding="0" class="AddressBook_personal">
    <tr>
        <eStore:CultureFullName ID="CultureFullName1" currentCss="" runat="server" />
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lAttCompanyName" runat="server">Company Name</asp:Literal><span
                class="eStore_redStar">*</span>：
        </td>
        <td>
            <eStore:TextBox ID="txtAttCompanyName" runat="server" CssClass="require"></eStore:TextBox>
        </td>
        <td>
            <span id="spanLegalForm" runat="server" visible="false">
                <asp:Literal ID="ltlegalForm" runat="server">LegalFrom</asp:Literal></span>
        </td>
        <td>
            <eStore:TextBox ID="txtlegalForm" runat="server" Visible="false"></eStore:TextBox><asp:LinkButton
                ID="lb_uploadLegalForm" runat="server" Text="Upload" Visible="false"></asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lAddress1" runat="server">Address 1</asp:Literal><span class="eStore_redStar">*</span>：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtAddress1" runat="server" ToolTip="Address 1" CssClass="require"></eStore:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lCity" runat="server">City</asp:Literal><span class="eStore_redStar">*</span>：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtCity" runat="server" ToolTip="City" CssClass="require"></eStore:TextBox>
        </td>
    </tr>
    <eStore:CountrySelector ID="CountrySelector1" IsForShipping="true" CountryTitleCss="" CountryCss="selectcss require" StatesCss="selectcss require" runat="server" />
    <tr id="pvat" runat="server" visible="false">
        <td>
            <label>
                <asp:Literal ID="ltVAT" runat="server">VAT </asp:Literal>
            </label>
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtVAT" runat="server" ClientIDMode="Static"></eStore:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lZipCode" runat="server">ZipCode</asp:Literal><span class="eStore_redStar">*</span>：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtZipCode" runat="server" ToolTip="ZipCode" CssClass="require"></eStore:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lTelNo" runat="server">TelNo</asp:Literal><span class="eStore_redStar">*</span>：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtTelNo" runat="server" ToolTip="TelNo" CssClass="require"></eStore:TextBox>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Ext)%>：
            <eStore:TextBox ID="txtTelExt" runat="server" CssClass="small"></eStore:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lFaxNo" runat="server">FaxNo</asp:Literal>
            ：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtFaxNo" runat="server"></eStore:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Literal ID="lMobile" runat="server">Mobile</asp:Literal>
            ：
        </td>
        <td colspan="3">
            <eStore:TextBox ID="txtMobile" runat="server"></eStore:TextBox>
        </td>
    </tr>
</table>
<div class="bottomHide">
    <asp:HiddenField ID="hcontactType" runat="server" EnableViewState="false" ClientIDMode="Static" />
    <asp:HiddenField ID="hAddressID" runat="server" EnableViewState="false" ClientIDMode="Static" />
</div>
<% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableVATSetting"))
   { %>
<script type="text/javascript">
    $("#txtVAT").keyup(function () {

        $("#hDetailVATValidStatus").val("0");
    });
    var detailVatValidcnt = 0;
    function checkvatvalidation() {
        if ($("#hDetailVATValidStatus").val() == "0") {
            var myVATNumber = $("#txtVAT").val();
            if ($.trim(myVATNumber) == "" || $.trim(myVATNumber) == "VAT")
                return true; //如果为空AJAX控件将会检查
            if (!checkVATNumber(myVATNumber)) {
                if (detailVatValidcnt == 0) {
                    detailVatValidcnt++;
                    alert("The VAT number you entered is not recognized.");
                    $("#txtVAT").focus();
                    return false;
                }
                else {
                    if (confirm("The VAT number you entered is not recognized,do you want to continue?")) {
                        $("#hDetailVATValidStatus").val("-1");
                        return true;
                    }
                    else {
                        $("#txtVAT").focus();
                        return false;
                    }
                }
            }
            else {
                $("#hDetailVATValidStatus").val("1");
                return true;
            }
        }
        return true;
    }
</script>
<% } %>