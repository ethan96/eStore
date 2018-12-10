<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="ContactUS.aspx.cs" Inherits="eStore.UI.ContactUS1" EnableEventValidation="false" %>

<%@ Register Src="~/Modules/ContactUS.ascx" TagName="T_ContactUS" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/V4/PolicyCategories.ascx" TagName="PolicyCategories"
    TagPrefix="eStore" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/ContactUSCSS")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/ContactUSJS")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="eStore_txtPage_content">
        <!--left-->
 
            <eStore:PolicyCategories ID="PolicyCategories1" runat="server" />
     
        <!--left-->
   
            <eStore:T_ContactUS ID="ContactUS" runat="server" />
 
    </div>
</asp:Content>
