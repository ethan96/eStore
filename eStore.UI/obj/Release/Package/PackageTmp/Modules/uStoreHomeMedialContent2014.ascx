<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uStoreHomeMedialContent2014.ascx.cs"
    Inherits="eStore.UI.Modules.uStoreHomeMedialContent2014" %>
<asp:Repeater ID="rpBanners" runat="server">
    <ItemTemplate>
        <header id="HeaderBanner" style="background-image: url(<%#ResolveUrl("~/resource"+ Eval("Imagefile")) %>);"
            class="<%=sitename %>">
	<div class="container">
    	<div class="intro-text">
            <h1>
          <%# Eval("Title") %>
            </h1>
             <h2>
               <%# Eval("AlternateText") %>
             </h2>
         </div>
     </div>
</header>
    </ItemTemplate>
</asp:Repeater>
<eStore:Repeater ID="rpApplications" runat="server">
    <HeaderTemplate>
        <h1 class="homepagetitle">
            Application</h1>
        <ul class="appcontainer">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="app">
            <div class="image">
                <a href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>'
                    title='<%# Eval("LocalCategoryName")%>'>
                    <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
?string.Empty
                    : string.Format("<img src=\"{0}\"  alt=\"{1}\"  />"
                    ,  eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL")
                    , Eval("LocalCategoryName"))%>
                    <h3 class="application"><%# Eval("LocalCategoryName")%>
                       </h3>
                    <h6 class="view">
                        View All >></h6>
                </a>
            </div>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>  <div class="clearfix"></div>
            <button class="eStore_btn hiddenitem" id="seeallapps">Hide Full Selection</button>
             <div class="clearfix"></div>
         <script type="text/javascript" language="javascript">
             $(document).ready(function () {
                 if ($(".appcontainer li").length > 4) {
                     $("#seeallapps").show().data("showall", true); 
                     $("#seeallapps").click(function () {
                         if ($(this).data("showall") == true) {
                             $(".appcontainer li:gt(3)").slideUp("slow");
                             $(this).data("showall", false);
                             $(this).text("See Full Selection");
                         }
                         else {
                             $(".appcontainer li:gt(3)").slideDown("slow");
                             $(this).data("showall", true);
                             $(this).text("Hide Full Selection");
                         }
                         return false;
                     });
                    // $(".appcontainer li:gt(3)").hide();
                 }

             });

        </script>
    </FooterTemplate>
</eStore:Repeater>
<eStore:Repeater ID="rpCategories" runat="server">
    <HeaderTemplate>
         <h1 class="homepagetitle">
            Products</h1>
        <ol class="medical-home-content">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <h3 class="double-line">
                <%# Eval("LocalCategoryName")%></h3>
            <div class="text">
                <div class="pic">
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
?string.Empty
                                : string.Format("<img src=\"{1}\"  alt=\"{2}\"  /> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
, ResolveUrl("~"+ eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL"))
, Eval("LocalCategoryName") )%></a>
                </div>
                <p class="pad2">
                    <%# Eval("descriptionX")%>
                </p>
                <div class="priceOrange">
                    From <span class="price">
                        <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithoutDecimal((decimal)((eStore.POCOS.ProductCategory)Container.DataItem).getLowestPrice())%>
                </div>
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                    class="eStore_btn">See Full Selection </a>
            </div>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ol></FooterTemplate>
</eStore:Repeater>
<eStore:Repeater ID="rpEcoPartnership" runat="server">
    <HeaderTemplate>
       <h1 class="homepagetitle">
            Eco Partnership</h1>
        <ol class="medical-home-EcoPartnership">
    </HeaderTemplate>
    <ItemTemplate>
        <li> <a href='<%#Eval("WebSiteUrl") %>' target="_blank">
            <img src='<%#  string.Format("{0}resource/{1}", esUtilities.CommonHelper.GetStoreLocation(), Eval("ImageUrl"))%>'  alt='<%#Eval("CompanyName")%>' /></a>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ol>
        <div class="clearfix"></div>
        <div class="medical-home-EcoPartnership-action">
            <a class='eStore_btn' href="ECO/BecomePartner.aspx">Become an EcoPartner </a><a class='eStore_btn borderBlue'
                href="ECO/FindPartner.aspx">Find an EcoPartner</a>
        </div>
    </FooterTemplate>
</eStore:Repeater>
