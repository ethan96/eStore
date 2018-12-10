<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="T_CustomerProfile.ascx.cs"
    Inherits="eStore.UI.Modules.T_CustomerProfile" %>
<%@ Register Src="~/Modules/CountrySelector.ascx" TagName="CountrySelector" TagPrefix="eStore" %>
<%@ Register Src="CultureFullName.ascx" TagName="CultureFullName" TagPrefix="eStore" %>
<div class="eStore_contactUs_input">
<eStore:CultureFullName ID="CultureFullName1" runat="server" />
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_Email" runat="server">Email </asp:Label>
        <span class="eStore_redStar">*</span>:
    </label>
    <eStore:TextBox ID="txt_email" runat="server" Columns="20" ValidationGroup="CustomerProfile" />
    <asp:RegularExpressionValidator ID="RequiredEmailREV" runat="server" ControlToValidate="txt_email"
        ErrorMessage="Your e-Mail is required." SetFocusOnError="True" Display="None"
        ValidationGroup="CustomerProfile" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Email address is required."
        ValidationGroup="CustomerProfile" Display="None" ControlToValidate="txt_email"></asp:RequiredFieldValidator>
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_Phone" runat="server" >Phone</asp:Label>
        <span class="eStore_redStar">*</span>:
    </label>
    <eStore:TextBox ID="txt_Phone1" runat="server" Columns="4" CssClass = "shorttext"/>-
    <eStore:TextBox ID="txt_Phone2" runat="server" Columns="10" ValidationGroup="CustomerProfile" CssClass = "shorttext"/>
    <asp:Label ID="lbl_Phone_Ext" runat="server">Ext.</asp:Label>
    <eStore:TextBox ID="txt_Phone_Ext" runat="server" Columns="4" CssClass = "shorttext"/>
    <asp:RequiredFieldValidator ID="RequiredPhone" runat="server" ControlToValidate="txt_Phone1"
        ErrorMessage="Please enter correct phone number." Display="None" SetFocusOnError="True"
        ValidationGroup="CustomerProfile"></asp:RequiredFieldValidator>
    <asp:RequiredFieldValidator ID="RequiredPhone2" runat="server" ControlToValidate="txt_Phone2"
        SetFocusOnError="True" ErrorMessage="Please enter correct phone number." ValidationGroup="CustomerProfile"
        Display="None"></asp:RequiredFieldValidator>
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_Company" runat="server">Company</asp:Label>
        <span class="eStore_redStar">*</span>:
    </label>
    <eStore:TextBox ID="txt_company" runat="server" Columns="20" ValidationGroup="CustomerProfile" />
    <asp:RequiredFieldValidator ID="RequiredCompany" runat="server" ControlToValidate="txt_company"
        ErrorMessage="Company name is required." Display="None" SetFocusOnError="True"
        ValidationGroup="CustomerProfile"></asp:RequiredFieldValidator>
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_Department" runat="server">Department</asp:Label>：
    </label>
    <eStore:TextBox ID="txt_department" runat="server" Columns="20" />
</div>
<div class="eStore_contactUs_input">
<eStore:CountrySelector ID="CountrySelector1" runat="server" ValidationGroup="CustomerProfile" />
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_City" runat="server">City</asp:Label>:
        &nbsp;
    </label>
    <eStore:TextBox ID="txt_city" runat="server" Columns="20" ValidationGroup="CustomerProfile" />
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_ZipCode" runat="server">Zip Code(USA - 5 Digit)</asp:Label>:
        &nbsp;
    </label>
    <eStore:TextBox ID="txt_zip" runat="server" Columns="20" ValidationGroup="CustomerProfile" />
</div>
<div class="eStore_contactUs_input">
    <label class="title">
        <asp:Label ID="lbl_Address" runat="server">Address</asp:Label>
        <span class="eStore_redStar">*</span>:
    </label>
    <eStore:TextBox ID="txt_address" runat="server" Columns="20" ValidationGroup="CustomerProfile" CssClass = "longtext"/>
    <asp:RequiredFieldValidator ID="RequiredAddress" runat="server" ControlToValidate="txt_address"
        ErrorMessage="Address is required." Display="None" SetFocusOnError="True" ValidationGroup="CustomerProfile"></asp:RequiredFieldValidator>
</div>
<asp:ValidationSummary ID="CustomerProfileValidateSum" runat="server" DisplayMode="list"
    ShowSummary="false" ShowMessageBox="true" ValidationGroup="CustomerProfile" />
