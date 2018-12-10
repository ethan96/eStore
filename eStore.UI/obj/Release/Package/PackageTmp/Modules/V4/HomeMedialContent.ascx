<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeMedialContent.ascx.cs" EnableViewState ="false"
    Inherits="eStore.UI.Modules.V4.HomeMedialContent" %>
 
<script>
    $(document).ready(function () {
        var url_string = document.location.href;
        url_string = url_string.replace("Default.aspx#", "Default.aspx?");
        var url = new URL(url_string);
        var Categoryid = "a#" + url.searchParams.get("Categoryid");
        var ctrlclass = ".ctrl" + url.searchParams.get("Categoryid");
        var TabName = url.searchParams.get("Tab");
        
        if ($(Categoryid).length > 0)
        {
            if(TabName == "PRODUCTS")
            {
                $(".eStore_index_Highlight_tabPRODUCTS").get(0).click();
            }
            else
            {
                $(".eStore_index_Highlight_tabSYSTEMS").get(0).click();
            }

            $(Categoryid).get(0).click();
            $("#Highlight_Summery").css("display", "none");
            $(ctrlclass).css("display", "block");
        }
    });

    function setCategoryLog(CategoryID, TabName)
    {
        $("#CategoryID_text").val(CategoryID);
        $("#TabName_text").val(TabName);
    }
</script>

<input type="hidden" id="CategoryID_text" value="">
<input type="hidden" id="TabName_text" value="">

<asp:Repeater ID="rpThemeBanners" runat="server" Visible="false">
    <HeaderTemplate>
        <div id="themeBanner">
            <div class="linkbox-wrapper">
                <h2>KNOWLEDGE CENTER</h2>
                <ul class="linkbox clearfix">
    </HeaderTemplate>
    <ItemTemplate>
        <li><a href="<%# (Eval("HyperLink") != null && !string.IsNullOrEmpty(Eval("HyperLink").ToString())) ? Eval("HyperLink") : ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl((Container.DataItem as eStore.POCOS.KitTheme).KitThemeTypes.OrderBy(c=>c.Seq).FirstOrDefault())) %>" target="<%#Eval("Target") %>">
            <span class="icon"><img alt="<%#Eval("AlterText") %>" src="<%#Eval("ImageFileX") %>" border="0" /></span> 
            <span class="link-text <%# Eval("Title").ToString().Length > 22 ? "no-top-margin" : "" %>"><%#Eval("Title") %></span></a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
        </div>
        </div>
    </FooterTemplate>
</asp:Repeater>
<div class="eStore_container eStore_block980">
    <div class="eStore_mobileTopBlock">
        <h5>
            TODAY’S HIGHLIGHT</h5>
        <div class="carouselBannerSingle" id="eStore_mobileTop">
            <ul>
            </ul>
            <div class="clearfix">
            </div>
            <div class="carousel-control">
                <a id="prev" class="prev" href="#"></a><a id="next" class="next" href="#"></a>
            </div>
        </div>
    </div>
    <!--eStore_mobileTopBlock-->
    <div class="eStore_index_Highlight row20">
        <div class="eStore_index_Highlight_contentBlock eStore_index_proContent ">
            <h5>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_TODAY_HIGHLIGHT)%></h5>
            <div class="eStore_index_HighlightContainer">
            </div>
        </div>
        <!--eStore_index_Highlight_contentBlock-->
        <div class="eStore_index_Highlight_tabBlock">
            <ul>
                <li class="eStore_index_Highlight_tabPRODUCTS on" dataname="#tab-PRODUCTS">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_PRODUCTS)%></li>
                <li class="eStore_index_Highlight_tabSYSTEMS" dataname="#tab-SYSTEMS">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_SYSTEMS)%></li>
                <div class="clearfix">
                </div>
            </ul>
            <div class="eStore_index_Highlight_tabList">
                <asp:Literal ID="lCategories" runat="server" ViewStateMode="Disabled"></asp:Literal>
            </div>
        </div>
        <!--eStore_index_Highlight_tabBlock-->
    </div>
    <!--eStore_index_Highlight-->
    <div class="eStore_index_Solution row20" data-bind="visible:hassolutions()">
        <div class="eStore_index_Solution_linkBlock">
            <div class="eStore_index_solutionTitle">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_Solution)%></div>
            <div class="eStore_index_solutionMobil">
                <asp:Repeater ID="rpSolutionsMobil" runat="server">
                    <ItemTemplate>
                        <a href="<%# Eval("Link") %>" ref="<%# Eval("Id") %>"><%# Eval("Name") %></a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="eStore_index_solutionnormal">
                <asp:Repeater ID="rpSolutions" runat="server" OnItemDataBound="rpSolutions_ItemDataBound">
                    <ItemTemplate>
                        <a  href="<%# Eval("Link") %>"  ref="<%# Eval("Id") %>"><%# Eval("Name") %></a>
                    </ItemTemplate>
                    <FooterTemplate>
                             <a href='<%# ResolveUrl("~/Product/AllSolution.aspx") %>' class="more">more...</a>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

        </div>
        <div class="eStore_index_Solution_contentBlock">
        <a  class="eStore_index_Solution_Link"></a>
            <div class="carouselBannerSingle" id="SolutionItmes">
                <ul>
                </ul>
                <div class="clearfix">
                </div>
                <div class="carousel-control" id="SolutionItmescarouselcontrol">
                    <a id="prev" class="prev" href="#"></a><a id="next" class="next" href="#"></a>
                </div>
            </div>
        </div>
    </div>
    <!--eStore_index_Solution-->
</div>
<!--container-->
<script id="_tmpmobilehighlight" type="text/x-jquery-tmpl">
  <!-- ko foreach: $data -->
        <li>
                        <a data-bind="attr: { href: Url }" class="eStore_productBlock">
                            <div class="eStore_productBlock_pic row10"><a  data-bind="attr: { href: Url }"><img data-bind="attr: { src: Image, title: Name, alt: Name }" /></a></div>
                            <div class="eStore_productBlock_txt row10" data-bind="html: Name"></div>
                            <div class="eStore_productBlock_price row10">
                                <div class="priceOrange" data-bind="html: Price"></div>
                            </div>
                              <div class="clearfix" />
                        </a>
                    </li>
    <!-- /ko -->
  <div class="clearfix">
                </div>
</script>
<script id="_tmphighlight" type="text/x-jquery-tmpl">
      <!-- ko foreach: $data -->
        <div class="eStore_productBlock">
            <div class="eStore_productBlock_pic row10">
                <a  data-bind="attr: { href: Url }"><img data-bind="attr: { src: Image, title: Name, alt: Name }" /></a>
            </div>
            <div class="eStore_productBlock_txt row10" data-bind="text: Name">
                <span class="icon">
                    <img data-bind="attr: { src: Icon }" /></span>
            </div>
            <div class="eStore_productBlock_price row10">
                <div class="priceOrange"  data-bind="html: Price">
                    </div>
            </div>
            <a data-bind="attr: { href: Url + '<%= sort %>' }" class="eStore_btn"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%></a>
              <div class="clearfix" />
        </div>
        <!-- /ko -->
          <div class="clearfix">
                </div>
</script>
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
                <div class="priceOrange"  data-bind="html: Price">
                    </div>
            </div>
            <a data-bind="attr: { href: Url }" class="eStore_btn"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%></a>
              <div class="clearfix" />
        </div>
        <!-- /ko -->
          <div class="clearfix">
                </div>
</script>
<script id="_tmpproductswithdesc" type="text/x-jquery-tmpl">
  <!-- ko foreach:  $data -->
<li>
<div class="eStore_productBlock">

<div class="eStore_productBlock_pic row5"><a  data-bind="attr: { href: Url }"><img data-bind="attr: { src: Image, title: Name, alt: Name }" /></a></div>
<div class="eStore_productBlock_txt row5"><a  data-bind="html: Name,attr: { href: Url }"></a></div>
<div class="eStore_productBlock_att row5" data-bind="html: Description"></div>
<div class="eStore_productBlock_price row5">
<div class="priceOrange" data-bind="html: Price"></div>
</div>
<div class="clearfix" />
</div>
</li>
        <!-- /ko -->
</script>
