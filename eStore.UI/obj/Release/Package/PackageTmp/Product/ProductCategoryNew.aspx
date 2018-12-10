<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="ProductCategoryNew.aspx.cs" Inherits="eStore.UI.Product.ProductCategoryNew" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/category")%>
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/categorycssPAPS")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/categoryjsPAPS")%>
</asp:Content>
<asp:Content ID="banner" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
   <eStore.V4:eStoreCycle2Slider ID="eStoreCycle2Slider1" runat="server"  />
 
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="eStore_category_link">
        <div class="eStore_category_moblieLink">
            <span data-class=".eStore_category_link_linkBlock">category</span>
        </div>
         <asp:Label ID="lSubCategories" runat="server" EnableViewState ="false"></asp:Label>
        <div id="storeSideAds">
        </div>
          <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
    <div id="category-list">
        <div class="eStore_category_content product_new">
            <h2 class="epaps_blockTitle">
                NEW ARRIVAL</h2>
            <div id="product_new" class="eStore_category_content_productBlock row20 byPhoto">
            </div>
        </div>
        <div class="eStore_category_content product_hot">
            <h2 class="epaps_blockTitle">
                HOT PRODUCTS</h2>
            <div id="product_hot" class="eStore_category_content_productBlock row20 byPhoto">
            </div>
        </div>
        <div class="eStore_category_content product_feature">
            <h2 class="epaps_blockTitle">
                FEATURE PRODUCTS</h2>
            <div id="product_feature" class="eStore_category_content_productBlock row20 byPhoto">
            </div>
        </div>
        <asp:HiddenField ID="hcategory" runat="server" ClientIDMode="Static" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
