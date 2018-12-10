<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryWithSubCategory.ascx.cs" EnableViewState="false"
    Inherits="eStore.UI.Modules.CategoryWithSubCategory" %>
<h5>
    <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
        <%=productCategory.LocalCategoryName%></a></h5>
<table>
    <tr>
        <td valign="top">
            <a href="<%=ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory))%>">
                <img src="<%=String.IsNullOrEmpty(productCategory.ImageURL)?string.Empty:(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ productCategory.ImageURL)%>" alt="<%=productCategory.LocalCategoryName%>"
                    width="60px" /></a>
        </td>
        <td valign="top">
            <asp:DataList ID="subCategory" runat="server" RepeatColumns="2">
                <ItemTemplate>
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <img src="<%#(Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
                                    ?string.Empty
                                    :eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString()+ Eval("ImageURL") %>" alt="<%# Eval("LocalCategoryName") %>"
                            width="80px" /></a>
                    <br />
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("LocalCategoryName")%></a>
                    <div class="CategoryMinPrice">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)%> <span id="<%# Eval("CategoryPath")%>">
                        <img src ="<%# esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif" />
                        </span>
                    </div>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" />
            </asp:DataList>
        </td>
    </tr>
</table>
