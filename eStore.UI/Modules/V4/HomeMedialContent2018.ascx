<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeMedialContent2018.ascx.cs" Inherits="eStore.UI.Modules.V4.HomeMedialContent2018" %>


<asp:Repeater ID="rpThemeBanners" runat="server" Visible="false">
    <HeaderTemplate>
        <div id="themeBanner">
            <div class="linkbox-wrapper">
                <h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_KNOWLEDGE_CENTER)%></h2>
                <ul class="linkbox clearfix">
    </HeaderTemplate>
    <ItemTemplate>
        <li><a href="<%# (Eval("HyperLink") != null && !string.IsNullOrEmpty(Eval("HyperLink").ToString())) ? Eval("HyperLink") : ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl((Container.DataItem as eStore.POCOS.KitTheme).KitThemeTypes.OrderBy(c=>c.Seq).FirstOrDefault())) %>" target="<%#Eval("Target") %>">
            <span class="icon">
                <img alt="<%#Eval("AlterText") %>" src="<%#Eval("ImageFileX") %>" border="0" /></span>
            <span class="link-text <%# Eval("Title").ToString().Length > 22 ? "no-top-margin" : "" %>"><%#Eval("Title") %></span></a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
        </div>
        </div>
    </FooterTemplate>
</asp:Repeater>

<div class="eStore_container eStore_block980">
    <div class="eStore_mobileTopBlock" style="">
        <h5><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_TODAY_HIGHLIGHT)%></h5>
        <div class="carouselBannerSingle" id="eStore_mobileTop">
            <ul id="highlineMobile">
                </ul>
            <div class="clearfix">
            </div>
            <div class="carousel-control">
                <a id="prev" class="prev" href="#" style="display: block;"></a><a id="next" class="next" href="#" style="display: block;"></a>
            </div>
        </div>
    </div>
    <!--eStore_mobileTopBlock-->
    <div class="eStore_index_Highlight row20">
        <div id="Highlight_Summery" class="eStore_index_Highlight_contentBlock eStore_index_proContent">
            <h5><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_TODAY_HIGHLIGHT)%></h5>
            <div class="eStore_index_HighlightContainer">
            </div>
            <div class="clearfix"></div>
        </div>

        <!--eStore_index_Highlight_contentBlock-->

        <div class="eStore_index_Highlight_Block">
            <div class="eStore_index_Highlight_section" id="shop_by_products">
                <div class="eStore_index_Highlight_sub">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_PRODUCTS)%>
                </div>

                <div class="eStore_index_Highlight_List ">
                    <dl class="eStore_accordion">
                        <asp:Repeater ID="rpCategories" runat="server">
                            <ItemTemplate>
                                <dt id='<%# Eval("CategoryID")%>' ><a href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem)) %>'><%# Eval("CategoryName") %> 
                                    <i class="fa fa-angle-down"></i><i class="fa fa-angle-up"></i></a></dt>
                        <dd style="display: none;">
                            <ul>
                                <asp:Repeater ID="rpSubCategory" runat="server" DataSource='<%#  Eval("childCategoriesX") %>'>
                                    <ItemTemplate>
                                        <li>
                                    <a id='<%# Eval("CategoryID")%>' ref='<%# Eval("CategoryPath") %>' data-cid='<%# Eval("CategoryID")%>'  data-pid='<%# Eval("ParentCategoryID")%>'  href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem)) %>'><span class="L1-icon">-</span> <span class="L1-text"><%# Eval("CategoryName") %> </span> 
                                        <i class="fa fa-angle-down"></i><i class="fa fa-angle-up"></i></a>
                                    <div class="eStore_index_proContent ctrl<%# Eval("CategoryID")%>" style="display: none;"></div>
                                </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </dd>
                            </ItemTemplate>
                        </asp:Repeater>
                    </dl>

                </div>
            </div>

            <div class="eStore_index_Highlight_section" id="shop_by_solutions">
                <div class="eStore_index_Highlight_sub">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_SHOP_BY_Application")%>
                </div>

                <div class="eStore_index_Highlight_List">
                    <dl class="eStore_accordion">
                        <asp:Repeater ID="rpApps" runat="server">
                            <ItemTemplate>
                                <dt id='<%# Eval("CategoryID")%>' class=""><a href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem)) %>'><%# Eval("CategoryName") %> <i class="fa fa-angle-down"></i><i class="fa fa-angle-up"></i></a></dt>
                        <dd style="display: none;">
                            <ul>
                                <asp:Repeater ID="rpSubCategory" runat="server" DataSource='<%#  Eval("childCategoriesX") %>'>
                                    <ItemTemplate>
                                        <li>
                                    <a id='<%# Eval("CategoryID")%>' ref='<%# Eval("CategoryPath") %>' data-cid='<%# Eval("CategoryID")%>'  data-pid='<%# Eval("ParentCategoryID")%>' href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem)) %>'><span class="L1-icon">-</span> <span class="L1-text"><%# Eval("CategoryName") %> </span> <i class="fa fa-angle-down"></i><i class="fa fa-angle-up"></i></a>
                                            <div class="eStore_index_proContent ctrl<%# Eval("CategoryID")%>" style="display: none;"></div>
                                </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </dd>
                            </ItemTemplate>
                        </asp:Repeater>
                    </dl>

                </div>
            </div>


        </div>

        <!--eStore_index_Highlight_tabBlock-->

        <div class="clearfix"></div>
    </div>

    <div class="clearfix"></div>
</div>

<script id="_tmpproducts" type="text/x-jquery-tmpl">
  <!-- ko foreach:  $data -->
        <div class="eStore_productBlock">
            <div class="eStore_productBlock_pic row10">
                <a  data-bind="attr: { href: Url }"><img data-bind="attr: { src: Image, title: Name, alt: Name }" /></a>
            </div>
            <div class="eStore_productBlock_txt row10" data-bind="text: Name">
                <span class="icon">
                    <img data-bind="attr: { src: Icon }" /></span>
            </div>
            <div class="eStore_productBlock_price row10">
                <div class="priceOrange priceOrange_fix"  data-bind="html: Price">
                    </div>
            </div>
            <a data-bind="attr: { href: Url }, html: ReadText" class="eStore_btn"></a>
              <div class="clearfix" />
        </div>
        <!-- /ko -->
          <div class="clearfix"></div>
    
</script>

<script id="_tmpMobileproducts" type="text/x-jquery-tmpl">
    <!-- ko foreach:  $data -->
    <li style="width: 317px;">
        <a data-bind="attr: { href: Url }" class="eStore_productBlock"></a>
        <div class="eStore_productBlock_pic row10"><a data-bind="attr: { href: Url }" class="eStore_productBlock"></a><a data-bind="attr: { href: Url }">
            <img data-bind="attr: { src: Image, title: Name, alt: Name }" ></a></div>
        <div class="eStore_productBlock_txt row10" data-bind="html: Name"></div>
        <div class="eStore_productBlock_price row10">
            <div class="priceOrange" data-bind="html: Price"></div>
        </div>
        <div class="clearfix"></div>
    </li>
    <!-- /ko -->
    <div class="clearfix">
                    </div>
</script>

