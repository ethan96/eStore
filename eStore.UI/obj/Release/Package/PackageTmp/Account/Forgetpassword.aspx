<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="Forgetpassword.aspx.cs" Inherits="eStore.UI.Account.Forgetpassword" %>
<%@ Register src="~/Modules/UVerification.ascx" tagname="UVerification" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="/App_Themes/V4/byregister.css" rel="stylesheet" />
    <script src="/Scripts/jquery.validate.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="register-container">
        <div class="container">
            <h4><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Forgot_Your_Password")%></h4>
            <p class="ma-signinTitle"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Please_enter_the_email_address")%></p>
            <div class="ma-emailcontext">
                <ul>
                    <li>
                        <label><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Email_address")%><span class="eStore_redStar">*</span>:</label>
                        <asp:TextBox ID="tbEmail" ClientIDMode="Static" runat="server"></asp:TextBox>
                    </li>
                    <li>
                        <uc1:UVerification ID="UVerification1" lableCss="editorpanelplabel" runat="server" />
                    </li>
                    <li>
                        <label></label>
                        <asp:Button ID="btSendMail" CssClass="eStore_btn noneborder widthauto" ClientIDMode="Static" runat="server" OnClick="btSendMail_Click" />
                        <input name="cancel" type="button" class="eStore_btn borderBlue noneborder widthauto" value='<%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Cancel")%>' />
                    </li>
                </ul>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            $("#eStoreMainForm").validate({
                rules: {
                    ctl00$eStoreMainContent$tbEmail:
                        {
                            required: true,
                            email: true
                        },
                    ctl00$eStoreMainContent$UVerification1$txtVerificationInput: "required"
                }
            });
            $('input[name="cancel"]').click(function () {
                $('.container input[type="text"]').val("");
            });
        });
    </script>
</asp:Content>

