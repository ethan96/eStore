<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PONo.ascx.cs" Inherits="eStore.UI.Modules.Payment.PONo" %>
<label class="blue">Purchase Order number</label><eStore:TextBox ID="txtPoNo" runat="server"
    ToolTip="Purchase Order number"></eStore:TextBox>
<asp:Literal ID="ltMustInput" runat="server"></asp:Literal>