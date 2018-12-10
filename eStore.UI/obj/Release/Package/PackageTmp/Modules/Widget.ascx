<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Widget.ascx.cs" Inherits="eStore.UI.Modules.Widget" %>
<asp:PlaceHolder runat="server" ID="phServerControls"></asp:PlaceHolder>
<asp:Panel ID="pActions" runat="server" CssClass="rightside" Visible="false">
    <eStore:Button ID="bt_compare" runat="server" Text="Compare" OnClick="bt_compare_Click" />
    <eStore:Button ID="bt_AddToQuote" runat="server" Text="Add to Quote" OnClick="bt_AddToQuote_Click" />
    <eStore:Button ID="bt_AddToCart" runat="server" Text="Add to Cart" OnClick="bt_AddToCart_Click" />
  
</asp:Panel>
