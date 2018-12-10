<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Idfms.aspx.cs" Inherits="eStore.UI.LiveDemo.Idfms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script language="JavaScript" type="text/JavaScript">
		//<![CDATA[
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
		//]]>
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
<table width="770" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td valign="top">
				<table width="770" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td>
							<table border="0" cellpadding="0" cellspacing="0">
								<tr>
									<td rowspan="3">
										<img src="/images/LiveDome/livedemologo.gif" width="117" height="58" alt="" />
									</td>
									<td rowspan="3">
										<img src="/images/LiveDome/menu01.gif" width="4" height="30" alt="" />
									</td>
									<td valign="bottom">
										<a href="Idfa.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image18','','/images/LiveDome/menu_fa02.gif',1)">
											<img src="/images/LiveDome/menu_fa01.gif" name="Image18" width="205" height="13" border="0" alt="" /></a>
									</td>
									<td rowspan="3">
										<img src="/images/LiveDome/menu01.gif" width="4" height="30" alt="" />
									</td>
									<td valign="bottom">
										<img src="/images/LiveDome/menu_fms03.gif" name="Image20" width="205" height="13" border="0" alt="" />
									</td>
									<td rowspan="3">
										<img src="/images/LiveDome/menu01.gif" width="4" height="30" alt="" />
									</td>
								</tr>
								<tr>
									<td>
										<img src="/images/LiveDome/menu02.gif" width="205" height="4" alt="" />
									</td>
									<td>
										<img src="/images/LiveDome/menu02.gif" width="205" height="4" alt="" />
									</td>
								</tr>
								<tr>
									<td valign="top">
										<a href="Idems.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image22','','/images/LiveDome/menu_ems02.gif',1)">
											<img src="/images/LiveDome/menu_ems01.gif" name="Image22" width="205" height="13" border="0" alt="" /></a>
									</td>
									<td valign="top">
										<a href="Idma.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image23','','/images/LiveDome/menu_ma02.gif',1)">
											<img src="/images/LiveDome/menu_ma01.gif" name="Image23" width="205" height="13" border="0" alt="" /></a>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td width="770">
							<table class="bodytext" cellspacing="5" cellpadding="10" width="100%" border="0">
								<tbody>
									<tr valign="bottom">
										<td height="75" valign="top">
											<img src="/images/LiveDome/title_fms.gif" width="483" height="75" title="<%=liveDemoSubTitle%>" alt="" />
										</td>
									</tr>
									<tr valign="top">
										<td height="115" align="left" valign="top">
											<div align="left">
												<p class="body">
													The airport grounding system is one of typical Facility Management applications. Generally, it will involve the monitoring for lighting, wind speed, temperature, security, and so on.</p>
												<p class="body">
													In this application, UNO embedded fan-less controller is been installed in the information center as head of airport grounding system. The ADAM-4000 collect the in-door data for UNO, includes of security, fire, and temperature. The ADAM-6000 also gather the out-door data for UNO, includes of temperature, wind speed, and lighting system. This remote integrated solution greatly provide an efficiency solution for airport facility management.</p>
											</div>
											<br />
										</td>
									</tr>
								</tbody>
							</table>
						</td>
					</tr>
				</table>
				<br />
				<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0" width="770" height="400">
					<param name="movie" value="/images/LiveDome/demo_fms.swf" />
					<param name="quality" value="high" />
					<%--<embed src="/images/LiveDome/demo_fms.swf" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" width="770" height="400"></embed>--%>
				</object>
			</td>
		</tr>
	</table>
</asp:Content>
