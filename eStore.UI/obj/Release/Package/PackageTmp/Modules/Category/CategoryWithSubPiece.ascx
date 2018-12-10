<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithSubPiece.ascx.cs" Inherits="eStore.UI.Modules.Category.CategoryWithSubPiece" %>
<div class="medical-ProCol">
    <div class="medical-ProCol-left">
    <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <img src="<%=String.IsNullOrEmpty(productCategory.ImageURL)?string.Empty:(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ productCategory.ImageURL)%>"
            alt="<%=productCategory.LocalCategoryName%>" class="CateImg" /></a></div>
    <div class="medical-ProCol-right">
    <div class="medical-ProCol-right-content">
        <h2><%=eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory)%></h2>
            <% = getDescription() %>
            <asp:Repeater ID="rpCateList" runat="server">
                    <ItemTemplate>
                    <div class="medical-ProCol-right-Size">
                    <%#eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%>
                        <asp:Repeater ID="rpSubCateList" runat="server" DataSource='<%#  Eval("childCategoriesX") %>'>
                            <ItemTemplate>
                                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                    <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a>
                            </ItemTemplate>
                        </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
        
    </div>
        <div class="medical-ProCol-right-msg text-right"><%=getFormatedAJAXMinPrice()%>
            <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>" class="btn">See Full Selection</a></div>
    </div>
</div>