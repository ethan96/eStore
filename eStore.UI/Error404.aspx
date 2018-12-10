<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="eStore.UI.Error404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
<div class="body404">
    <div class="blue">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Error_404)%>
    </div>
</div> 
</asp:Content>