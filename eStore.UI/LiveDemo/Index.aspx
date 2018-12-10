<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="eStore.UI.LiveDemo.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
<table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:ImageMap ID="ImageMap1" runat="server" ImageUrl="/images/LiveDome/livedemo_index.gif" GenerateEmptyAlternateText="True">
                    <asp:RectangleHotSpot AlternateText="Environmental" Bottom="125" Left="391" Right="459" Top="5" NavigateUrl="~/LiveDemo/Idems.aspx" />
                    <asp:RectangleHotSpot AlternateText="Facility" Bottom="334" Left="164" Right="235" Top="203" NavigateUrl="~/LiveDemo/Idfms.aspx" />
                    <asp:RectangleHotSpot AlternateText="Factory" Bottom="391" Left="83" Right="152" Top="267" NavigateUrl="~/LiveDemo/Idfa.aspx" />
                    <asp:RectangleHotSpot AlternateText="Machine" Bottom="292" Left="11" Right="79" Top="167" NavigateUrl="~/LiveDemo/Idma.aspx" />
                </asp:ImageMap>
            </td>
            <td>
                <img src="/images/LiveDome/livedemo_index1.jpg" style="float: left;" alt="Map" />
            </td>
        </tr>
    </table>
</asp:Content>
