<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Print.Master" AutoEventWireup="true" CodeBehind="print.aspx.cs" Inherits="eStore.UI.Product.print" %>
<%@ Register src="../Modules/CTOSPrint.ascx" tagname="CTOSPrint" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <asp:PlaceHolder ID="phPrinter" runat="server"></asp:PlaceHolder>
</asp:Content>
