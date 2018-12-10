<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UVerification.ascx.cs"
    Inherits="eStore.UI.Modules.UVerification" %>
<asp:Panel ID="plVerfication" runat="server">
    <div class="eStore_contactUs_input">
        <div>
            <label class="<%=lableCss %>">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Verification_Code)%>
                <span class="eStore_redStar">*</span>:
            </label>
            <asp:TextBox ID="txtVerificationInput" runat="server" Width="60px" Style="height: 19px;"></asp:TextBox>
            <asp:Image ID="imgVerificationtxtResult" runat="server" CssClass="hiddenitem eStore_imgVerificationtxtResult" ImageUrl="~/App_Themes/Default/Wrong.jpg" AlternateText="Verificationtxt Result" />&nbsp;
            <asp:Image ID="imgVerificationCaptcha" runat="server" CssClass="hiddenitem" ToolTip="Refresh" AlternateText ="Captcha"
                ImageUrl="~/images/p.gif" Style="cursor: pointer" />
            <span id="spanRefreshVerification" runat="server" Style="cursor: pointer">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Change_Picture)%>
            </span>
            <div style="clear: both">
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            getImage();
            $("#<%=txtVerificationInput.ClientID %>").click(function () {
                if ($("#<%=imgVerificationCaptcha.ClientID %>").hasClass("hiddenitem"))
                    getImage();
            }).keyup(function () {
                if ($.trim($(this).val()).length > 4) {
                    eStore.UI.eStoreScripts.checkVerification($(this).val(), function (res) {
                        if (res == "false")
                            $("#<%=imgVerificationtxtResult.ClientID %>").attr("src", "<%=esUtilities.CommonHelper.GetStoreLocation()%>App_Themes/Default/Wrong.jpg").removeClass("hiddenitem");
                        else
                            $("#<%=imgVerificationtxtResult.ClientID %>").attr("src", "<%=esUtilities.CommonHelper.GetStoreLocation()%>App_Themes/Default/ok.jpg").removeClass("hiddenitem");
                    });
                }
                else
                    $("#<%=imgVerificationtxtResult.ClientID %>").attr("src", "").addClass("hiddenitem");
            });
            $("#<%=spanRefreshVerification.ClientID %>").click(function () {
                getImage();
            });
            $("#<%=imgVerificationCaptcha.ClientID %>").click(function () {
                getImage();
            });
        });
        function getImage() {
            var uid = (new Date()).getSeconds().toString() + Math.round(Math.random() * 10000);
            $("#<%=imgVerificationCaptcha.ClientID %>").attr("src", "<%=esUtilities.CommonHelper.GetStoreLocation()%>Verfication/captcha.ashx?uid=" + uid).removeClass("hiddenitem");
            $("#<%=spanRefreshVerification.ClientID %>").removeClass("hiddenitem");
            $("#<%=txtVerificationInput.ClientID %>").val("");
            $("#<%=imgVerificationtxtResult.ClientID %>").addClass("hiddenitem");
        }
    </script>
</asp:Panel>
