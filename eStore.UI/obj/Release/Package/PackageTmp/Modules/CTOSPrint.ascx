<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CTOSPrint.ascx.cs" Inherits="eStore.UI.Modules.CTOSPrint" %>
<table>
    <tr>
        <td colspan="2">
            <h3 class="productname">
                <asp:Literal ID="lProductName" runat="server" />
            </h3>
            <h4>
                <asp:Literal ID="lShortDescription" runat="server" />
            </h4>
        </td>
    </tr>
    <tr width="280px">
        <td class="ProductPicture">
        <asp:Image ID="imgProduct" runat="server" style="max-width:275px;"/>
        </td>
        <td valign="top">
            <ul class="ProductFeature">
                <asp:Literal ID="lProductFeature" runat="server"></asp:Literal></ul>
            <div class="clear" />
            <asp:Literal ID="lProductprice" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
<div class="clear">
</div>
<div id="configurationsystem">
    <div id="configItemsTitle">
        <asp:Label ID="lbl_configItemsTitle" Text="Build Your System" runat="server"></asp:Label>
 
    </div>
    <asp:Literal ID="CTOSModules" runat="server" />
</div>
