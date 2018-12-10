<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductSubCategory.ascx.cs" Inherits="eStore.UI.Modules.ProductSubCategory" %>
<h2 class="CategoryTitle">
    <span>
        <asp:Literal ID="lCategoryName" runat="server"></asp:Literal></span>
</h2>
<div class="CategoryDescription">
    <asp:Literal ID="lCategoryDescription" runat="server"></asp:Literal>
</div>
<div class="clear">
</div>
<asp:DataList ID="dlSubCategory" runat="server" RepeatColumns="3" ItemStyle-VerticalAlign="Top"
    EnableViewState="false">
    <ItemTemplate>
        <h5>
            <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                <%# Eval("LocalCategoryName")%></a></h5>
        <p>
            <%# Eval("Description")%></p>
        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
            <img src="<%# Eval("ImageURL","http://buy.advantech.com/Resources/{0}") %>" alt="<%# Eval("LocalCategoryName") %>"
                width="180px" /></a>
        <br />
        <div class="CategoryMinPrice">
            From <span>$<%# ((eStore.POCOS.ProductCategory)Container.DataItem).getLowestPrice()%></span>
        </div>
        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
            class="viewConfigbutton"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_View_Complete_Selection_Now)%></a>
    </ItemTemplate>
    <ItemStyle CssClass="SubCategory" />
    <SeparatorTemplate>
    </SeparatorTemplate>
    <SeparatorStyle CssClass="dotlinemidial" />
</asp:DataList>
