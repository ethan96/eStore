<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/OneColumn.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="eStore.UI.eautomation.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <style type="text/css">
        .OPN_td
        {
            height: 191px;
            width: 388px;
            margin: 0px;
            padding: 0px;
            background-repeat: no-repeat;
        }
        .OPN_td_Div
        {
            float: right;
            height: 140px;
            width: 160px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
            overflow: hidden;
        }
        .OPN_td_bg1
        {
            background-image: url(/images/eautomation/DistIO_CatImage2.jpg);
        }
        .OPN_td_bg2
        {
            background-image: url(/images/eautomation/PlugIn_CatImage.jpg);
        }
        .OPN_td_bg3
        {
            background-image: url(/images/eautomation/AutoCont_CatImage.jpg);
        }
        .OPN_td_bg4
        {
            background-image: url(/images/eautomation/ICOM_CatImage.jpg);
        }
        .OPN_td_bg5
        {
            background-image: url(/images/eautomation/Human_Machine_Interface.jpg);
        }
        .OPN_td_bg6
        {
            background-image: url(/images/eautomation/Embedded_Automation_Compute.jpg);
        }
    </style>
    <eStore:AdRotator ID="AdRotator1" runat="server" />
    <div class="master-wrapper-center">
        <div class="master-wrapper-cph">
            <table width="778" border="0" cellspacing="1" cellpadding="0">
                <tr>
                    <td class="OPN_td OPN_td_bg1">
                        <div class="OPN_td_Div" id="OPN_td_Div1">
                            <asp:HyperLink ID="HyperLink1" runat="server" CssClass="text_med" NavigateUrl="~/seo/data-acquisition-module.htm"
                                Text="Data Acquisition &amp; I/O" Font-Bold="True" meta:resourcekey="hl_5Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/PCI-bus-Cards/DAQ_PCI_Series.ps.htm"
                                    Text="PCI-bus Cards" meta:resourcekey="hl_5_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/PCI-104-and-PC-104-Modules/DAQ_PCI-PC104.mx.htm"
                                    Text="PC/104 & PCI-104" meta:resourcekey="hl_5_8Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/ISA-bus-Cards/DAQ_ISA-bus_Cards.ps.htm"
                                    Text="ISA-bus Cards" meta:resourcekey="hl_5_2Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/USB-IO-Modules/DAQ_USB_IO_Modules.ps.htm"
                                    Text="USB IO Modules" meta:resourcekey="hl_5_3Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/Ethernet-IO-Modules/DAQ_Ethernet_IO_Modules.ps.htm"
                                    meta:resourcekey="hl_5_4Resource1">Ethernet IO Modules</asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/RS-485-IO-Modules/DAQ_RS-485_IO_Modules.ps.htm"
                                    Text="RS-485 IO Modules" meta:resourcekey="hl_5_5Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink8" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/Wireless-IO/DAQ_Wireless_IO_Modules.ps.htm"
                                    Text="Wireless IO" meta:resourcekey="hl_5_6Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink9" runat="server" NavigateUrl="~/Data-Acquisition-and-I-O/Signal-Conditioning/Signal_Conditioning.ps.htm"
                                    Text="Signal Conditioning" meta:resourcekey="hl_5_7Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                    <td class="OPN_td OPN_td_bg2">
                        <div class="OPN_td_Div" id="OPN_td_Div2">
                            <asp:HyperLink ID="HyperLink10" runat="server" CssClass="text_med" NavigateUrl="~/9/Motion-Control/MstrCATE_EAPRO_AUTOSOFT.pc.htm"
                                Text="Motion Control" Font-Bold="True" meta:resourcekey="hl_6Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink11" runat="server" NavigateUrl="~/Motion-Control/Decentralized-Motion-Control/Decentralized_Motion_Control.ps.htm"
                                    Text="Decentralized Motion Control" meta:resourcekey="hl_6_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink12" runat="server" NavigateUrl="~/Motion-Control/PCI-bus-Cards/MC_PCI.ps.htm"
                                    Text="PCI-bus Cards" meta:resourcekey="hl_6_2Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink13" runat="server" NavigateUrl="~/Motion-Control/ISA-bus-Cards/MC_ISA.ps.htm"
                                    Text="ISA-bus Cards" meta:resourcekey="hl_6_3Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="OPN_td OPN_td_bg3">
                        <div class="OPN_td_Div" id="OPN_td_Div3">
                            <asp:HyperLink ID="HyperLink14" runat="server" CssClass="text_med" NavigateUrl="~/1/Programmable-Automation-Controllers-PAC-/PAC.pc.htm"
                                Text="Programmable<br />Automation Controllers (PAC)" Font-Bold="True" meta:resourcekey="hl_4Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink15" runat="server" NavigateUrl="~/Programmable-Automation-Controllers-PAC-/Controller/PAC_Controller.ps.htm"
                                    Text="Controller" meta:resourcekey="hl_4_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink16" runat="server" NavigateUrl="~/Programmable-Automation-Controllers-PAC-/IO-Modules/PAC_IO_Modules.ps.htm"
                                    Text="IO Modules" meta:resourcekey="hl_4_2Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                    <td class="OPN_td OPN_td_bg4">
                        <div class="OPN_td_Div" id="OPN_td_Div4">
                            <asp:HyperLink ID="HyperLink17" runat="server" CssClass="text_med" NavigateUrl="~/seo/MstrCATE_EAPRO_RSCOMM.htm"
                                Text="Industrial Ethernet Networking" Font-Bold="True" meta:resourcekey="hl_2Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink18" runat="server" NavigateUrl="~/Industrial-Ethernet-Networking/Media-Converters/IEN_Media_Converters.ps.htm"
                                    Text="Media Converters" meta:resourcekey="hl_2_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink19" runat="server" NavigateUrl="~/Industrial-Ethernet-Networking/Industrial-Ethernet-Switches/IEN_Ethernet_Switches.ps.htm"
                                    Text="Industrial Ethernet Switches" meta:resourcekey="hl_2_2Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink20" runat="server" NavigateUrl="~/Industrial-Ethernet-Networking/Multiport-Serial-Cards/IEN_Multiport_Serial_Cards.pc.htm"
                                    Text="Multiport Serial Cards" meta:resourcekey="hl_2_3Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink21" runat="server" NavigateUrl="~/Industrial-Ethernet-Networking/Device-Servers/IEN_Device_Servers.ps.htm"
                                    Text="Device Servers" meta:resourcekey="hl_2_4Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="OPN_td OPN_td_bg5">
                        <div class="OPN_td_Div" id="OPN_td_Div5">
                            <asp:HyperLink ID="HyperLink22" runat="server" CssClass="text_med" NavigateUrl="~/13/Human-Machine-Interface/EPPEZEC_HMI.cc.htm"
                                Text="Human Machine<br />Interface" Font-Bold="True" meta:resourcekey="hl_1Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink23" runat="server" NavigateUrl="~/Industrial-Accessories/Flat-Panel-Monitor/HMI_FPM.ps.htm"
                                    Text="Flat Panel Monitors" meta:resourcekey="hl_1_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink24" runat="server" NavigateUrl="~/Industrial-Panel-PC/Industrial-Panel-PC/configure-EPPEZEC_HMI_IPPC.htm"
                                    Text="Industrial Panel PC" meta:resourcekey="hl_1_2Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink25" runat="server" NavigateUrl="~/Industrial-Panel-PC/Industrial-Panel-PC/configure-EPPEZEC_HMI_IPPC.htm"
                                    Text="Industrial Workstation" meta:resourcekey="hl_1_3Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink26" runat="server" NavigateUrl="~/Touch-Panel-Computers/Touch-Panel-Computers/EPPEZEA_HMI_TPC.cs.htm?catalogid=113"
                                    Text="Touch Panel Computers" meta:resourcekey="hl_1_4Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                    <td class="OPN_td OPN_td_bg6">
                        <div class="OPN_td_Div" id="OPN_td_Div6">
                            <asp:HyperLink ID="HyperLink27" runat="server" CssClass="text_med" NavigateUrl="~/Automation-Series-Embedded-Computers/Automation-Series-Embedded-Computers/UNO_PLUS.cs.htm"
                                Text="Embedded Automation Computers (UNO)" Font-Bold="True" meta:resourcekey="hl_3Resource1"></asp:HyperLink>
                            <br />
                            <span class="text_mini">
                                <asp:HyperLink ID="HyperLink28" runat="server" NavigateUrl="~/UNO-1000/UNO-1000/configure-UNO-1000.htm"
                                    Text="DIN-rail UNO" meta:resourcekey="hl_3_1Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink29" runat="server" NavigateUrl="~/UNO-2000/UNO-2000/configure-UNO_CP.htm"
                                    Text="Compact Fanless UNO" meta:resourcekey="hl_3_2Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink30" runat="server" NavigateUrl="~/UNO-2100/UNO-2100/configure-UNO_2100.htm"
                                    Text="Fanless UNO with PC/104<br />Expansion" meta:resourcekey="hl_3_3Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink31" runat="server" NavigateUrl="~/UNO-3000/UNO-3000/configure-UNO_FA.htm"
                                    Text="Fanless UNO with PCI Expansion" meta:resourcekey="hl_3_4Resource1"></asp:HyperLink>
                                <br />
                                <asp:HyperLink ID="HyperLink32" runat="server" NavigateUrl="~/UNO-4000/UNO-4000/configure-UNO-4000.htm"
                                    Text="Rackmount Fanless UNO" meta:resourcekey="hl_3_5Resource1"></asp:HyperLink>
                            </span>
                        </div>
                    </td>
                </tr>
            </table>
            
        </div>
    </div>
    <div class="master-wrapper-side">
        <eStore:liveperson ID="liveperson1" runat="server" />
        <div id="storeSideAds">
        </div>
    </div>
</asp:Content>
