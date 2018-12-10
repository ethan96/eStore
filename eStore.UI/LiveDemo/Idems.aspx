<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Idems.aspx.cs" Inherits="eStore.UI.LiveDemo.Idems" %>
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
										<a href="Idfms.aspx" onmouseout="MM_swapImgRestore()" onmouseover="MM_swapImage('Image20','','/images/LiveDome/menu_fms02.gif',1)">
											<img src="/images/LiveDome/menu_fms01.gif" name="Image20" width="205" height="13" border="0" alt="" /></a>
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
										<img src="/images/LiveDome/menu_ems03.gif" width="205" height="13" border="0" alt="" />
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
											<img src="/images/LiveDome/title_ems.gif" width="483" height="75" title="<%=liveDemoSubTitle%>" alt="" />
										</td>
									</tr>
									<tr valign="top">
										<td height="115" align="left" valign="top">
											<div align="left">
												<p class="body">
													Waste Water Treatment is one of typical Environmental Monitoring applications. The main processes includes Screening, Aerating, Sludge Removing, and Bacteria Killing. In screening stage, ADAM-4017+ Analog Input module takes response to monitor the floating rate in case of the pipeline entry been plugged. Once the floating rate drop down, the ADAM-4055 Digital I/O module will issue alarms to operators to clean the big size garbage from pipeline entry. In Aerating stage, the ADAM-4055 will control the miller and Oxygen supply. In Sludge Removing stage, the ADAM-4017+ will work as a gate keeper to monitor the turbidity of waste water before outlet to next process. In Bacteria Killing stage, ADAM-5510KW can control the Chlorine and PH Chemical Material supply through ADAM-5055 DI/O module. Then the ADAM-5017 continuous watch the PH value in the latest stage.
												</p>
												<p class="body">
													The whole waste water treatment system is control by ADAM-5510KW basing on RS-485 networking. It collect the local data from ADAM-5000 I/O modules and remote data from ADAM-4000 I/O modules, then feedback to the supervisory center for operator to manage the whole process.
												</p>
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
					<param name="movie" value="/images/LiveDome/demo_ems.swf" />
					<param name="quality" value="high" />
					<%--<embed src="/images/LiveDome/demo_ems.swf" quality="high" pluginspage="http://www.macromedia.com/go/getflashplayer" type="application/x-shockwave-flash" width="770" height="400"></embed>--%>
				</object>
			</td>
		</tr>
	</table>
</asp:Content>
