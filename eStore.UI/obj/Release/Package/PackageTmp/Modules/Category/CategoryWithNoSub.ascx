<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithNoSub.ascx.cs"
    Inherits="eStore.UI.Modules.Category.CategoryWithNoSub" %>
<div class="medical-ProCol">
    <div class="medical-ProCol-left">
        <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <img src="<%=String.IsNullOrEmpty(productCategory.ImageURL)?string.Empty:(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ productCategory.ImageURL)%>"
            alt="<%=eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory)%>" class="CateImg" /></a></div>
    <div class="medical-ProCol-right">
        <div class="medical-ProCol-right-content">
            <h2>
                <%=eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory)%></h2>
            <% = getDescription() %>
        </div>
        <div class="medical-ProCol-right-msg text-right">
            <%=getFormatedAJAXMinPrice()%>
            <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>" class="btn">See Full Selection</a></div>
    </div>
</div>
