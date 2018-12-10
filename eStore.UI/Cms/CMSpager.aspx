<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true" CodeBehind="CMSpager.aspx.cs" Inherits="eStore.UI.CMSpager" %>
<%@ Register src="~/Modules/CMSTab.ascx" tagname="CMSTab" tagprefix="uc1" %>

<%@ Register src="../Modules/AdRotatorSelect.ascx" tagname="AdRotatorSelect" tagprefix="uc2" %>

<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">
<div class="row20"></div>
    <uc1:CMSTab ID="CMSTab1" runat="server"  EnableViewState ="false"/>
</asp:Content>
