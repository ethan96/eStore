<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.ascx.cs" Inherits="eStore.UI.Modules.UserLogin" %>
<div id="eStore_LogIn_input" style="display: none;">
    <div class="eStore_LogIn">
        <div class="eStore_normalLogin">
            <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RememberMeSet="true"
                RenderOuterTable="false" OnAuthenticate="LoginUser_Authenticate">
                <LayoutTemplate>
                    <div class="headertitle">
                        <asp:Literal ID="llogin" runat="server"></asp:Literal></div>
                    <div class="content">
                        <p class="inputline">
                            <eStore:TextBox ID="UserName" runat="server" CssClass="InputValidation"></eStore:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required."
                                ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        </p>
                        <p class="inputline">
                            <eStore:TextBox ID="Password" runat="server" TextMode="Password"></eStore:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                                ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        </p>
                        <p>
                            <span class="float-left">
                                <asp:CheckBox ID="RememberMe" runat="server" />
                                <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">
                         
                                </asp:Label></span> <span class="float-right">
                                    <asp:HyperLink ID="hlforgotpassword" runat="server" CssClass="eStore_forgot"></asp:HyperLink>
                                </span>
                            <div class="clearfix">
                            </div>
                        </p>
                        <p class="divFeature hiddenitem">
                            <asp:Literal ID="lbShow" runat="server"></asp:Literal></p>
                    </div>
                    <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                        ValidationGroup="LoginUserValidationGroup" />
                    <asp:LinkButton ID="LoginButton" runat="server" CommandName="Login" ClientIDMode="Static"
                        CssClass="eStore_btn submit" Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                    <asp:HyperLink ID="hlSignup" runat="server" CssClass="eStore_btn deepBlue" Text="Sign up now" />
                    <span class="failureNotification">
                        <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                    </span>
                </LayoutTemplate>
            </asp:Login>
        </div>
    </div>
</div>
<input type="hidden" name="timezoneOffset" id="timezoneOffset" />
<input type="hidden" name="loginTrigger" id="loginTrigger" />
<input type="hidden" name="hRegPurpose" id="hRegPurpose" />
<script type="text/javascript" language="javascript">
    $(function () {

        var d = new Date();
        var timezone = -d.getTimezoneOffset();
        $("#timezoneOffset").val(timezone);
        $(".eStore_MyAccount").fancybox({
            parent: "form:first" // jQuery selector 
            , autoHeight: true
            , autoWidth: true
        });
        $(".needlogin").click(function () {
            $("#loginTrigger").val($(this).attr("id"));
            $("#eStore_LogIn_input .divFeature").hide();
            $("#hRegPurpose").val($(this).text());
            $(".eStore_MyAccount").click(); return false;
        });
        $(".ctosneedlogin").click(function () {
            $("#eStore_LogIn_input .divFeature").show();
            $("#loginTrigger").val($(this).attr("id"));
            $("#hRegPurpose").val($(this).text());
            $(".eStore_MyAccount").click(); return false;
        }
        );
        $("#eStore_LogIn_input .divFeature").find(".close").click(function () {
            $.fancybox.close();
            return false;
        });
        $("#Header1_HeadLoginView_UserLogin1_LoginUser_hlSignup").click(function () {
            document.location = getRegisterUrl();
            return false;
        });
        $("#eStore_LogIn_input .divFeature").find(".Register").click(function () {
            document.location = getRegisterUrl();
            return false;
        });

        if ($('a[href="#eStore_LogIn_input"]').length == 1) {
            var userName = GetQueryString("e");
            if (userName != null) {
                $("#Header1_HeadLoginView_UserLogin1_LoginUser_UserName").val(userName);
                popuploginnormal();
            }
        }
    });

    function getRegisterUrl() {
        var tracking = '<%= HttpUtility.UrlEncode(string.Format("http://buy.advantech.com/admin/SessionHistory.aspx?sessionID={0}",eStore.Presentation.eStoreContext.Current.SessionID)) %>';
        var loginPurpose;
        if ($("#hRegPurpose").val() == "") {
            loginPurpose = "Tracking: " + tracking;
        }
        else {
            loginPurpose = $.trim($("#hRegPurpose").val()) + ". Tracking: " + tracking;
        }
        var link = $("#Header1_HeadLoginView_UserLogin1_LoginUser_hlSignup").attr("href");
        var con = "&";
        if (link.indexOf("?") < 0)
            con = "?";
        return link + con + "RegPurpose=" + loginPurpose;
    }
    function popuploginnormal(loginPurpose) {
        $("#eStore_LogIn_input .divFeature").hide();
        $(".eStore_MyAccount").click();
        if (loginPurpose != null && loginPurpose != "")
            $("#hRegPurpose").val(loginPurpose);
        return false;
    }
    function popLoginDialog(dheight, dwidth, loginPurpose) {

        $(".eStore_MyAccount").click();
        $("#loginTrigger").val($(this).attr("id"));
        if (loginPurpose != null && loginPurpose != "")
            $("#hRegPurpose").val(loginPurpose);
        return false;
    }
</script>
