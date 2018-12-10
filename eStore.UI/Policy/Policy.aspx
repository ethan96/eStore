<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Policy.aspx.cs" Inherits="eStore.UI.Policy.Policy" %>

<%@ Register Src="~/Modules/V4/PolicyCategories.ascx" TagName="PolicyCategories"
    TagPrefix="eStore" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/article")%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
<%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/policyJs")%>
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="eStore_txtPage_content">
        <!--left-->
        <eStore:PolicyCategories ID="PolicyCategories1" runat="server" />
        <!--left-->
        <!--right-->
        <div class="eStore_rightBlock float-left eStore_policy_html">
            <%= Html %>
        </div>
        <!--right-->
    </div>
</asp:Content>
