<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLoginUshop.ascx.cs"
    Inherits="eStore.UI.Modules.IoTMart.UserLoginUshop" %>
<div class="floatLeft">
    <div id="iot_uShop_login" class="iot_uShopPOP uShopPOP_account" style="display: none;">
        <a href="#iot_uShop_login" id="ushoplogin" class="fancybox hiddenitem">Login</a>
        <h1>
            会员登录</h1>
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RememberMeSet="true"
        RenderOuterTable="false" OnAuthenticate="LoginUser_Authenticate">
        <LayoutTemplate>
        <table border="0" cellspacing="1" cellpadding="0" class="formBlcok">
            <tr>
                <th>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                会员帐号</asp:Label>
                </th>
                <td>
                    <eStore:TextBox ID="UserName" Width="180" runat="server" CssClass="textEntry InputValidation" tooltip="请输入您的邮箱"></eStore:TextBox>
                    <asp:RegularExpressionValidator ID="RegularUserNameEmail" runat="server" ControlToValidate="UserName"
                        CssClass="failureNotification red" ErrorMessage="User Email is required." 
                        ToolTip="User Email is required." ValidationGroup="LoginUserValidationGroup" 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <th>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                密码</asp:Label>
                </th>
                <td>
                    <eStore:TextBox ID="Password" Width="180" runat="server" CssClass="textEntry" TextMode="Password"></eStore:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                        CssClass="failureNotification red" ErrorMessage="Password is required." ToolTip="Password is required."
                        ValidationGroup="LoginUserValidationGroup">请输入密码</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <th>
                </th>
                <td class="att">
                    <asp:CheckBox ID="RememberMe" runat="server" />
                    <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Keep_me_logged_in)%>
                    </asp:Label>
                    <a href="#iot_uShop_forgetPSW" id="spforgetpassword" class="fancybox" style="float: right;">
                        忘记密码</a>
                </td>
            </tr>
            <tr>
                <th>
                </th>
                <td>
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" ClientIDMode="Static"
                        CssClass="btnStyle" Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                    <div class="divFeature hiddenitem">
                        <asp:Label ID="lbShow" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
                </LayoutTemplate>
    </asp:Login>
        <div class="loginBottom">
            没有帐号?<a href="<%=esUtilities.CommonHelper.GetStoreLocation() %>RegisterForm.aspx"
                class="btnStyle">立即注册</a>
        </div>


    </div>
    <div id="iot_uShop_forgetPSW" class="iot_uShopPOP uShopPOP_account" style="display: none;">
        <h1>
            忘记密码</h1>
        <p class="bigTxt">
            忘记密码？没问题，请输入您的邮箱地址，<br />
            系统会将更新密码的网址寄送给您。</p>
        <table border="0" cellspacing="1" cellpadding="0" class="formBlcok">
            <tr>
                <th style="font-size:15px;">
                    您的邮箱地址
                </th>
                <td>
                    <eStore:TextBox ID="txsendforgetemail" ClientIDMode="Static" runat="server"></eStore:TextBox><span
                        id="spcheckemailAddress" class="hiddenitem red">不是正确的邮箱</span>
                    <asp:Button ID="btsendforgetemail" ClientIDMode="Static" runat="server" CssClass="btnStyle"
                        Text="确认" OnClick="btsendforgetemail_Click" />
                </td>
            </tr>
        </table>
    </div>

     <div id="ito_ushop_sendpswSuccess" href="#ito_ushop_sendpswSuccess" class="iot_uShopPOP uShopPOP_account" style="display: none;">
        <h1>
            忘记密码</h1>
        <p class="bigTxt">
            重设密码的链结已寄到您的邮箱，请点击电邮中的链结后在线上变更您的密码。</p>
        <table border="0" cellspacing="1" cellpadding="0" class="formBlcok">
            <tr>
                <th style="font-size:15px;">
                    
                </th>
                <td>
                <a class="btnStyle" href="/" onclick="gotohomepage()">返回首页</a>
                <p>&nbsp;</p>
                <p class="att">3秒后将自动跳至商城首页</p>
                </td>
            </tr>
        </table>
    </div>

</div>

<input type="hidden" name="timezoneOffset" id="timezoneOffset" />
<input type="hidden" name ="loginTrigger" id="loginTrigger" />

<script type="text/javascript" language="javascript">

    $(function () {
        $("#ushoplogin").fancybox({
            padding: 0,
            margin: 0,
            onStart: function () { $("#iot_uShop_login").show() },
            onClosed: function () { $("#iot_uShop_login").hide() }
        });
        $("#spforgetpassword").fancybox({
            padding: 0,
            margin: 0,
            onStart: function () { $("#iot_uShop_forgetPSW").show(); $("#iot_uShop_login").hide() },
            onClosed: function () { $("#iot_uShop_forgetPSW").hide() }
        });
        $("#btsendforgetemail").click(function () {
            if (!checkEmailFormat($("#txsendforgetemail").val())) {
                $("#spcheckemailAddress").show();
                return false;
            }
            else {
                $("#spcheckemailAddress").hide();
                return true;
            }
        }).blur(function () {
            if (!checkEmailFormat($("#txsendforgetemail").val())) {
                $("#spcheckemailAddress").show();
                return false;
            }
            else {
                $("#spcheckemailAddress").hide();
                return true;
            }
        });

        $(".needlogin").click(function () {
            $("#ushoplogin").click();
            return false;
        });

        $(".formBlcok :text").PreFillToolTips();

        $("#ito_ushop_sendpswSuccess").fancybox({
            padding: 0,
            margin: 0,
            onStart: function () { $("#ito_ushop_sendpswSuccess").show(), window.setTimeout(gotohomepage, 3000); },
            onClosed: function () { $("#ito_ushop_sendpswSuccess").hide() }
        });
    });

    function gotohomepage() {
        window.location.href = "<%=esUtilities.CommonHelper.GetStoreLocation() %>";
    }
    

</script>
