<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uStoreHomeMedialContent.ascx.cs"
    Inherits="eStore.UI.Modules.uStoreHomeMedialContent" %>
<eStore:Repeater ID="rpCategories" runat="server">
    <ItemTemplate>
        <div class="pd_repeat">
            <div class="pd_groupname">
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <%# Eval("LocalCategoryName")%></a></div>
            <div class="pd_description">
                <%# string.IsNullOrEmpty( Eval("descriptionX").ToString())?""
                                                            : (Eval("descriptionX").ToString().Length > 120
                                                                             ? (Eval("descriptionX").ToString().Substring(0, 120) + "...")
                                        : Eval("descriptionX"))%></div>
            <div class="pd_pic">
                <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
?string.Empty
                                : string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"163px\" height=\"97px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
, eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL")
, Eval("LocalCategoryName") )%></div>
            <div class="pd_price">
                <%# string.Format(string.IsNullOrEmpty(eStore.Presentation.eStoreContext.Current.Store.profile.getStringSetting("MinPriceFormat"))?"{0} {1}"
            :eStore.Presentation.eStoreContext.Current.Store.profile.getStringSetting("MinPriceFormat")
            ,
                "<span class=\"txt_brown\">" +eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)+"</span>"
                 ,eStore.Presentation.Product.ProductPrice.FormartPriceWithoutDecimal((decimal)((eStore.POCOS.ProductCategory)Container.DataItem).getLowestPrice()))%></div>
        </div>
    </ItemTemplate>
</eStore:Repeater>

  <script type="text/javascript" language="javascript">
      $(document).ready(function () {
          $(".pd_repeat").equalHeight();
      });
 
            </script>
