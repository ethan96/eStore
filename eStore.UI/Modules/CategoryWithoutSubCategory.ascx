<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithoutSubCategory.ascx.cs"  EnableViewState="false"
    Inherits="eStore.UI.Modules.CategoryWithoutSubCategory" %>
<h5>
    <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <%=productCategory.LocalCategoryName%></a></h5>
<p>
    <%=productCategory.descriptionX%>
</p>
<div class="CategoryImg">
    <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <img src="<%=String.IsNullOrEmpty(productCategory.ImageURL)?string.Empty:(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ productCategory.ImageURL)%>"
            alt="<%=productCategory.LocalCategoryName%>" class="CategoryImg" /></a>
</div>
<div class="CategoryMinPrice">
   <%=getFormatedAJAXMinPrice()%></div>
<a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>"
    class="viewConfigbutton"><span>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_View_Complete_Selection_Now)%></span></a>