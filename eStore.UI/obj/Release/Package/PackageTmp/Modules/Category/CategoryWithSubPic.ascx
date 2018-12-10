<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithSubPic.ascx.cs" Inherits="eStore.UI.Modules.Category.CategoryWithSubPic" %>
<div class="medical-ProCol">
<div class="medical-ProCol-left">
<a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <img src="<%=String.IsNullOrEmpty(productCategory.ImageURL)?string.Empty:(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ productCategory.ImageURL)%>"
            alt="<%=eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory)%>" class="CateImg" /></a></div>
<div class="medical-ProCol-right">
    <div class="medical-ProCol-right-contentT">
        <h2><%=eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory)%></h2>
        <% = getDescription() %>
    </div>
    <asp:Repeater ID="rpCategoryList" runat="server">
        <ItemTemplate>
                <div class="medical-ProCol-right-contentB">
                    <div class="medical-ProCol-left"><img src='<%# getImageUrl(Eval("ImageURL"))%>'
            alt='<%#eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%>' width="90" height="70" /></div>
                    <div class="medical-ProCol-right"><h4><%#eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></h4>
                    <asp:Repeater ID="rpSubCateList" runat="server" DataSource='<%#  Eval("childCategoriesX") %>'>
                            <ItemTemplate>
                                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                    <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="clear"></div>
                </div>
        </ItemTemplate>
    </asp:Repeater>
    <div class="medical-ProCol-right-msg text-right"><%=getFormatedAJAXMinPrice()%>
        <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>" class="btn">See Full Selection</a></div>
</div>
</div>