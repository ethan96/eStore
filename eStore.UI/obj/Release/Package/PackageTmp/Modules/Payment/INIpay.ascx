<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="INIpay.ascx.cs" Inherits="eStore.UI.Modules.Payment.INIpay" %>

 <div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Terms_and_Conditions)%>
    </div>
    <div class="paymentbody editorpanel">
       <asp:Literal ID="ltBankInfo" runat="server"></asp:Literal>
       <p align="right">
            <input type="checkbox" id="akrCheckBox" />
            <span style="color:Red">*</span>동의
        </p>
    </div>
</div>
<div style="display: none;">
    <div id="ofirst">
        <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" valign="top">
                    <br>
                    <table width="563" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img2.gif" width="563" height="43">
                            </td>
                        </tr>
                        <tr>
                            <td align="center" bgcolor="6398BF">
                                <table width="551" border="0" cellspacing="0" cellpadding="0">
                                    <tr align="center" bgcolor="#FFFFFF">
                                        <td height="15" colspan="3">
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td width="190" align="center">
                                            <table width="125" height="116" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="center" valign="bottom" background="/images/INIpayNet/time_02.gif">
                                                    </td>
                                                </tr>
                                            </table>
                                            <img src="/images/INIpayNet/logo.gif" width="87" height="25" hspace="5" vspace="3">
                                        </td>
                                        <td width="1" background="/images/INIpayNet/check_img10.gif">
                                            <img src="/images/INIpayNet/check_img6.gif" width="1" height="1">
                                        </td>
                                        <td valign="top" style="padding: 0 0 0 19">
                                            <table width="324" border="0" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td align="left">
                                                        <font color="#3366FF"><b>Your safety Gyeoljeyong encryption program for payment of Installation
                                                            requires.<br>
                                                            Your computer environment is Windows XP (SP2) is
                                                            <br>
                                                            다Please follow the following steps.</b></font>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5" class="pl_01">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="16" bgcolor="#145181" class="pl_01">
                                                        <b><font color="#FFFF00">1. Click the mouse on the yellow Information Bar .<br>
                                                            2.<font color="#FFFFFF">&quot; ActivX control installation &quot;</font> to select</font></b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                        <img src="/images/INIpayNet/img_01.gif" width="324" height="108"><br>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="16" bgcolor="145181" class="pl_01">
                                                        <b><font color="#FFFF00">3. Security Warning window appears, " <font color="#FFFFFF">
                                                            &quot;설치&quot;</font> to Press to proceed. " </font></b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                        <img src="/images/INIpayNet/img_02.gif" width="324" height="115">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <b>Encryption does not install the program automatically </b>
                                                        <br>
                                                        1. <a href="http://plugin.inicis.com/repair/INIpayWizard.exe"><b><font color="red">Here
                                                        </font></b></a>Click to download INIpayWizard program.<br>
                                                        2. Run the downloaded program.<br>
                                                        3. Please try your payment again.
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td height="5" colspan="3">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img7.gif" width="563" height="11">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </center>
    </div>
    <div id="osecond">
        <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" valign="top">
                    <br>
                    <table width="563" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img2.gif" width="563" height="43">
                            </td>
                        </tr>
                        <tr>
                            <td align="center" bgcolor="6398BF">
                                <table width="551" border="0" cellspacing="0" cellpadding="0">
                                    <tr align="center" bgcolor="#FFFFFF">
                                        <td height="15" colspan="3">
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td width="190" align="center">
                                            <table width="125" height="116" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="center" valign="bottom" background="/images/INIpayNet/time_02.gif" width="124">
                                                    </td>
                                                </tr>
                                            </table>
                                            <img src="/images/INIpayNet/logo.gif" width="87" height="28" hspace="19" vspace="3">
                                        </td>
                                        <td width="1" background="/images/INIpayNet/check_img10.gif">
                                            <img src="/images/INIpayNet/check_img6.gif" width="1" height="1">
                                        </td>
                                        <td valign="top" style="padding: 0 0 0 19">
                                            <table width="324" border="0" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td align="left">
                                                        <font color="#3366FF"><b>Your secure payment Encryption is required to install the program.
                                                            <br>
                                                            For this task, depending on your communications environment may take about 5 seconds
                                                            to complete the installation of water until the moment Please wait. </b></font>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5" class="pl_01">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="16" bgcolor="#145181" class="pl_01">
                                                        <b><font color="#FFFF00">Security Alert When prompted <font color="#FFFFFF">&quot;예&quot;</font>To
                                                            Press to proceed.</font></b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                        <img src="/images/INIpayNet/check_img.gif" width="321" height="185">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <b>Encryption does not install the program automatically</b><br>
                                                        1. <a href="http://plugin.inicis.com/repair/INIpayWizard.exe"><b><font color="red">Here
                                                        </font></b></a>Click to download the program INIpayWizard.<br>
                                                        2. Run the downloaded program.
                                                        <br>
                                                        3. Payment will try again.
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td height="5" colspan="3">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img7.gif" width="563" height="11">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </center>
    </div>
    <div id="othird">
        <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center" valign="top">
                    <br>
                    <table width="563" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img2.gif" width="563" height="43">
                            </td>
                        </tr>
                        <tr>
                            <td align="center" bgcolor="6398BF">
                                <table width="551" border="0" cellspacing="0" cellpadding="0">
                                    <tr align="center" bgcolor="#FFFFFF">
                                        <td height="15" colspan="3">
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td width="190" align="center">
                                            <table width="125" height="116" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="center" valign="bottom" background="/images/INIpayNet/time_02.gif">
                                                    </td>
                                                </tr>
                                            </table>
                                            <img src="/images/INIpayNet/logo.gif" width="87" height="25" hspace="5" vspace="3">
                                        </td>
                                        <td width="1" background="/images/INIpayNet/check_img10.gif">
                                            <img src="/images/INIpayNet/check_img6.gif" width="1" height="1">
                                        </td>
                                        <td valign="top" style="padding: 0 0 0 19">
                                            <table width="324" border="0" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td align="left">
                                                        <font color="#3366FF"><b>Your safety Gyeoljeyong encryption program for payment of Installation
                                                            is required.
                                                            <br>
                                                            Your computer is a Windows Vista environment
                                                            <br>
                                                            Please follow the following steps.</b></font>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5" class="pl_01">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="16" bgcolor="#145181" class="pl_01">
                                                        <b><font color="#FFFF00">1. Click the mouse on the yellow Information Bar.
                                                            <br>
                                                            2. <font color="#FFFFFF">"ActivX control installation" </font>to Select. </font>
                                                        </b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                        <img src="/images/INIpayNet/img_vista2.gif" width="324" height="108"><br>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="16" bgcolor="145181" class="pl_01">
                                                        <b><font color="#FFFF00">3. Security Warning window appears <font color="#FFFFFF">"install"
                                                        </font>to Press to proceed.</font></b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                        <img src="/images/INIpayNet/img_vista1.gif" width="324" height="115">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="12">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <b>Encryption does not install automatically when the program </b>
                                                        <br>
                                                        1. <a href="http://plugin.inicis.com/repair/INIpayWizard.exe"><b><font color="red">here
                                                        </font></b></a>Click to download INIpayWizard program.
                                                        <br>
                                                        2. Run the downloaded program.
                                                        <br>
                                                        3. Please try your payment again.
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#FFFFFF">
                                        <td height="5" colspan="3">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="/images/INIpayNet/check_img7.gif" width="563" height="11">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </center>
    </div>
</div>
<script language="javascript" type="text/javascript" src="http://plugin.inicis.com/pay40_check.js"></script>
<script language="javascript"  type="text/javascript">
    function inicisforieonly() {
        alert("인터넷 익스플로어에서만 신용 카드 결제 사용이 가능합니다.");
        return false;
    }
    if (navigator.userAgent.indexOf("Windows NT 6.0") != -1) {
        $(".koreaInipaynet").html(othird.innerHTML);

    }
    else {
        var g_fIsSP1 = false;
        g_fIsSP1 = (window.navigator.appMinorVersion.indexOf("SP2") != -1);
        if (g_fIsSP1) {
            $(".koreaInipaynet").html(ofirst.innerHTML);
        }
        else {
            $(".koreaInipaynet").html(osecond.innerHTML);
        }
    }
    StartSmartUpdate();

    function waitingpluginchecking() {
        alert("Plug-in program is installing, please wait");
        return false;
    }
    function checkPlugin() {
        if (navigator.appName == 'Netscape') {
            if (document.INIpay == null) {

                alert(" Plug-in program is not installed automatically. \n \n Please install according to the remedial actions on the page.");
            }
            else {
                //When actually applied to the next page to POST data in the form of the following lines, please apply.
                //document.way_INI.submit(); 

                //window.location = "/INIsecureStart.aspx";

            }
        }
        else {
            if (document.INIpay.object == null) {

                alert(" Plug-in program is not installed automatically. \n \n Please install according to the remedial actions on the page.");
            }
            else {
                //When actually applied to the next page to POST data in the form of the following lines, please apply.
                //document.way_INI.submit(); 

                //window.location = "/INIsecureStart.aspx";
            }
        }
    }
    checkPlugin();

    function checkAKRcheckbox() {
        if (!$("#akrCheckBox").prop("checked")) {
            alert($.eStoreLocalizaion("You_have_to_accept_the_terms_and_conditions_before_continue"));
            return false;
        }
        return true;
    }

    $(function () {
        if ($("#eStoreMainContent_btnCheckout").attr("onclick").indexOf("checkAKRcheckbox") != -1) {
            $("#eStoreMainContent_btnCheckout").removeAttr("onclick").click(function () { return checkAKRcheckbox2(); });
        }
    });

    function checkAKRcheckbox2() {
        if (!checkAKRcheckbox()) {
            return false;
        }
        else {
            $("form:first").attr("action", "/INIsecureStart.aspx").submit(); 
        }
    }
</script>

