<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryList.ascx.cs"  EnableViewState="false"
    Inherits="eStore.UI.Modules.CategoryList" %>
<eStore:Repeater ID="rpCategoryList" runat="server">
    <ItemTemplate>
        <div class="categorylist">
            <div class="categoryimag">
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <img src="<%# eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ Eval("ImageURL")%>"
                        alt="<%#Eval("LocalCategoryName")%>" class="CategoryImg" /></a>
            </div>
            <div class="categorydetail">
                <h3>
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%#Eval("LocalCategoryName")%></a></h3>
                <p>
                    <%#Eval("Description")%></p>
            </div>
        </div>
    </ItemTemplate>
    <SeparatorTemplate>
        <div class="clear">
        </div>
    </SeparatorTemplate>
</eStore:Repeater>
