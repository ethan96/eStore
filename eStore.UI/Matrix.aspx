<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="Matrix.aspx.cs" Inherits="eStore.UI.Matrix" %>
<%@ Register src="Modules/ProductMatrix.ascx" tagname="ProductMatrix" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <uc1:ProductMatrix ID="ProductMatrix1" runat="server" />
</asp:Content>
