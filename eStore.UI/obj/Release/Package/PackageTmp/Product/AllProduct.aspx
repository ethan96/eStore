<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="AllProduct.aspx.cs" Inherits="eStore.UI.Product.AllProduct" %>

<%@ Register Src="../Modules/ProductCategoryAllList.ascx" TagName="ProductCategoryAllList"
    TagPrefix="uc1" %>
<%@ Register Src="../Modules/AdRotatorSelect.ascx" TagName="AdRotatorSelect" TagPrefix="uc2" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/allProductcss")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <uc2:AdRotatorSelect ID="AdRotatorSelect1" runat="server" />
    <div class="eStore_breadcrumb eStore_block980">
    	<a href="/"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
        <%=this.ProductTitle%>
    </div>
    <div class="eStore_container eStore_block980">
        <div id="div_AllCategoryListMain" class="eStore_allList_content">
            <h2><%=this.ProductTitle %></h2>
            <uc1:ProductCategoryAllList ID="ProductCategoryAllList1" runat="server" />
            <asp:HyperLink ID="hlViewAll" NavigateUrl="~/Product/AllProduct.aspx?type=all1" runat="server" EnableViewState ="false" Visible="false" CssClass="viewalllistlink" Text="> See All Products"></asp:HyperLink>
        </div>
    </div>
        <script>
            $(function () {
                fixTableLayout(".eStore_allList_content", "ol");
                $(window).resize(function () {
                    fixTableLayout(".eStore_allList_content", "ol");
                });
            });
    </script>
</asp:Content>
