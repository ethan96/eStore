﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="system_fms.aspx.cs" Inherits="eStore.UI.LiveDemo.system_fms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script language="JavaScript" type="text/JavaScript">
<!--
    function MM_swapImgRestore() { //v3.0
        var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
    }

    function MM_preloadImages() { //v3.0
        var d = document; if (d.images) {
            if (!d.MM_p) d.MM_p = new Array();
            var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
                if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; } 
        }
    }

    function MM_findObj(n, d) { //v4.01
        var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
            d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
        }
        if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
        for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
        if (!x && d.getElementById) x = d.getElementById(n); return x;
    }

    function MM_swapImage() { //v3.0
        var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
            if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
    }
//-->
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
<table width="100%" height="777" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td height="58" colspan="2" valign="top">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td rowspan="3">
                            <img src="/images/LiveDome/livedemologo.gif" width="117" height="58"></td>
                        <td rowspan="3">
                            <img src="/images/LiveDome/menu01.gif" width="4" height="30"></td>
                        <td valign="bottom">
                            <a href="Idfa.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image18','','/images/LiveDome/menu_fa02.gif',1)">
                                <img src="/images/LiveDome/menu_fa01.gif" name="Image18" width="205" height="13"
                                    border="0"></a></td>
                        <td rowspan="3">
                            <img src="/images/LiveDome/menu01.gif" width="4" height="30"></td>
                        <td valign="bottom">
                            <a href="Idfms.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image20','','/images/LiveDome/menu_fms02.gif',1)">
                                <img src="/images/LiveDome/menu_fms03.gif" name="Image20" width="205" height="13"
                                    border="0"></a></td>
                        <td rowspan="3">
                            <img src="/images/LiveDome/menu01.gif" width="4" height="30"></td>
                    </tr>
                    <tr>
                        <td>
                            <img src="/images/LiveDome/menu02.gif" width="205" height="4"></td>
                        <td>
                            <img src="/images/LiveDome/menu02.gif" width="205" height="4"></td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <a href="Idems.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image22','','/images/LiveDome/menu_ems02.gif',1)">
                                <img src="/images/LiveDome/menu_ems01.gif" name="Image22" width="205" height="13"
                                    border="0"></a></td>
                        <td valign="top">
                            <a href="Idma.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image21','','/images/LiveDome/menu_ma02.gif',1)">
                                <img src="/images/LiveDome/menu_ma01.gif" name="Image21" width="205" height="13"
                                    border="0"></a></td>
                    </tr>
                </table>
            </td>
            <td valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td height="1" colspan="3" valign="top" bgcolor="#CCCCCC">
                <img src="/images/LiveDome/clear.gif" height="1" width="1" border="0"></td>
        </tr>
        <tr>
            <td height="97" colspan="2" valign="top">
                <img src="/images/LiveDome/title_subfms.gif" width="615" height="97"></td>
            <td valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td height="10" colspan="2" valign="top">
            </td>
            <td valign="top" class="style" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td height="7" colspan="2" valign="top">
                <img src="/images/LiveDome/lineb.gif" height="7"></td>
            <td valign="top" class="style" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td width="491" height="15" valign="top">
                &nbsp;</td>
            <td width="491" align="right" valign="top">
                &nbsp;</td>
            <td valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td height="440" colspan="2" align="center" valign="top">
                <img src="/images/LiveDome/System_fms.jpg" border="0" usemap="#Map">
                <map name="Map">
                    <area shape="poly" coords="547,332,547,381,547,392,546,420,737,421,737,388,605,389,605,322,545,322"
                        href="../products/model.aspx?id=MstrCATE_EAPRO_ADAM4015T" target="_blank">
                    <area shape="poly" coords="373,337,373,380,373,391,372,419,529,419,530,386,431,388,431,323,371,324"
                        href="../products/model.aspx?id=MstrCATE_EAPRO_ADAM4055" target="_blank">
                    <area shape="poly" coords="593,210,594,259,594,270,593,298,801,299,801,267,648,267,647,200,592,199"
                        href="../products/model.aspx?id=MstrCATE_EAPRO_ADAM6017" target="_blank">
                    <area shape="poly" coords="301,207,301,261,336,261,336,297,417,296,415,264,347,265,347,198,301,198"
                        href="../products/model.aspx?id=MstrCATE_EAPRO_ESADAM6520" target="_blank">
                    <area shape="rect" coords="40,180,181,269" href="../product/model.aspx?ID=MstrCATE_EAPRO_ASTUDIO60"
                        target="_blank">
                    <area shape="rect" coords="36,305,203,410" href="../product/model.aspx?id=MstrCATE_EAPRO_UNO2160"
                        target="_blank">
                    <area shape="poly" coords="424,205,424,261,424,275,425,299,583,296,582,265,470,265,471,196,424,196"
                        href="../products/model.aspx?id=MstrCATE_EAPRO_ADAM6051" target="_blank">
                </map>
            </td>
            <td valign="top" class="body">
            </td>
        </tr>
        <tr>
            <td height="15" valign="top">
                &nbsp;</td>
            <td align="right" valign="top">
                &nbsp;</td>
            <td valign="top">
            </td>
        </tr>
        <tr>
            <td height="0" colspan="2" valign="top">
            </td>
            <td valign="top">
            </td>
        </tr>
        <tr>
            <td height="7" colspan="2" valign="top" style="background-repeat: repeat-x; background-position: top;">
                <img src="/images/LiveDome/lineb2.gif" height="7"></td>
            <td valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
        <tr>
            <td height="25" colspan="2" valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
            <td valign="top" style="background-repeat: repeat-x; background-position: top;">
            </td>
        </tr>
    </table>
</asp:Content>
