<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeMedialContent2018V2.ascx.cs" Inherits="eStore.UI.Modules.V4.HomeMedialContent2018V2" %>
<%@ Register Src="../HomeEnhancedService.ascx" TagName="HomeEnhancedService" TagPrefix="eStore" %>

<eStore:Repeater ID="rpBanners" runat="server">
    <headertemplate>
        <div class="eStore_container eStore_block980">
            <div id="HeaderBanner" style="margin-bottom: 10px;">
                <div class="cycle-slideshow" data-cycle-fx="scrollHorz" data-cycle-timeout="5000" data-cycle-random="true"
                    data-cycle-pause-on-hover="true" data-cycle-overlay-template="<h1><a   href='{{href}}' >{{title}}</a></h1><h4>{{desc}}</h4><br><a   href='{{href}}' class='eStore_btn {{btnstyle}}'>{{btntext}}</a>">
                    <div class="cycle-overlay">
                    </div>
                    <div class="cycle-pager">
                    </div>
    </headertemplate>
    <itemtemplate>
        <img src="<%#ResolveUrl("~/resource"+ Eval("Imagefile")) %>" data-cycle-href="<%# Eval("Hyperlink") %>" alt="<%# Eval("Title") %>" <%# string.IsNullOrEmpty((string) Eval("HtmlContent"))?"data-cycle-btnstyle='blue'  data-cycle-btntext='More'":Eval("HtmlContent")%> 
            data-cycle-title="<%# Eval("Title") %>" data-cycle-desc="<%# Eval("AlternateText") %>" />
    </itemtemplate>
    <footertemplate>
        </div> </div> </div></footertemplate>
</eStore:Repeater>


<eStore:Repeater ID="rpRootCategories" runat="server">
    <headertemplate>
                <div class="eStore_index_Product row20">
    <div class="eStore_index_Highlight">
        <ul>
            </headertemplate>
    <itemtemplate>
                           <li>
                            <div class="eStore_productBlock">
                                   <div class="eStore_productBlock_txt row5">
                                <a href="<%# Eval("Url") %>"  >  <%# Eval("Name")%></a>
                                       <div class="clearfix"></div>
                                </div>
                                <div class="eStore_productBlock_pic row5">
                                    <a href="<%# Eval("Url") %>">
                                        <img src="<%# Eval("Image") %>" title="<%# Eval("Name") %>" alt="<%#  System.Web.HttpUtility.HtmlEncode( Eval("Description")) %>" /></a>
                                </div>
                        
                                <div class="clearfix"></div>
                            </div>
                        </li>
            </itemtemplate>
    <footertemplate>
        </ul>
        <a href='<%# ResolveUrl("~/Product/AllProduct.aspx?TYPE=all1") %>' class="more">more...</a>
            
        </div>
</div></footertemplate>
</eStore:Repeater>

<eStore:Repeater ID="rpSolutions" runat="server" OnItemDataBound="rpSolutions_ItemDataBound">
    <headertemplate>
                <div class="eStore_index_Solution row20">
    <div class="eStore_index_Highlight">
        <h2 class="eStore_index_solutionTitle">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_Solution)%>
        </h2>
            </headertemplate>
    <itemtemplate>
                <a href="<%# Eval("Link") %>" ref="<%# Eval("Id") %>" datatitle="<%# string.IsNullOrEmpty((string)Eval("Title"))?(string)Eval("Name"):(string)Eval("Title")%>"
                    dataimage="<%# Eval("Image") %>">
                    <%# Eval("Name") %></a>
            </itemtemplate>
    <footertemplate>
                <a href='<%# ResolveUrl("~/Product/AllSolution.aspx") %>' class="more">more...</a>
            </footertemplate>
    <footertemplate> </div>
</div></footertemplate>
</eStore:Repeater>

<!--eStore_index_Solution-->
<div class="eStore_container eStore_block980 row20">
    <div class="eStore_index_Highlight">
        <h2 class="eStore_index_solutionTitle">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_PRODUCTS)%>
        </h2>
        <asp:Repeater ID="rpProducts" runat="server">
            <ItemTemplate>
                <a href="<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                    ref="<%# Eval("CategoryPath") %>">
                    <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a>
                <div class="eStore_index_proContent">
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clearfix">
        </div>
        <div class="eStore_index_proContentBlock carouselBannerSingle" id="eStore_product_carousel">
            <ul>
                <asp:Repeater ID="rpProductscarousel" runat="server">
                    <ItemTemplate>
                        <li>
                            <div class="eStore_productBlock">
                                <div class="eStore_productBlock_pic row5">
                                    <a href="<%# Eval("Url") %>">
                                        <img src="<%# Eval("Image") %>" title="<%# Eval("Name") %>" alt="<%#  System.Web.HttpUtility.HtmlEncode( Eval("Description")) %>" /></a>
                                </div>
                                <div class="eStore_productBlock_txt row5">
                                    <%# Eval("Name")%>
                                </div>
                                <div class="eStore_productBlock_price row5">
                                    <div class="priceOrange">
                                        <%# Eval("Price")%>
                                    </div>
                                </div>
                                <a href="<%# Eval("Url") %>" class="eStore_btn">More</a>
                                <div class="clearfix" />
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <div class="clearfix">
            </div>
            <div class="carousel-control">
                <a class="prev" href="#"></a><a class="next" href="#"></a>
            </div>
        </div>
    </div>
</div>
<div class="clearfix">
</div>

<eStore:HomeEnhancedService ID="HomeEnhancedService" runat="server" visible="false" />
<div class="clearfix">
</div>

<div id="system_block" class="eStore_container eStore_block980 row20" runat="server">
    <div class="eStore_index_Highlight">
        <h2 class="eStore_index_solutionTitle">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_SHOP_BY_SYSTEMS)%></li>
        </h2>
        <asp:Repeater ID="rpSystems" runat="server">
            <ItemTemplate>
                <a href="<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                    ref="<%# Eval("CategoryPath") %>">
                    <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory) Container.DataItem)%></a>
                <div class="eStore_index_proContent">
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clearfix">
        </div>
        <div class="eStore_index_proContentBlock carouselBannerSingle" id="eStore_System_carousel">
            <ul>
                <asp:Repeater ID="rpSystemscarousel" runat="server">
                    <ItemTemplate>
                        <li>
                            <div class="eStore_productBlock">
                                <div class="eStore_productBlock_pic row5">
                                    <a href="<%# Eval("Url") %>">
                                        <img src="<%# Eval("Image") %>" title="<%# Eval("Name") %>" alt="<%#System.Web.HttpUtility.HtmlEncode( Eval("Description")) %>" /></a>
                                </div>
                                <div class="eStore_productBlock_txt row5">
                                    <%# Eval("Name")%>
                                </div>
                                <div class="eStore_productBlock_price row5">
                                    <div class="priceOrange">
                                        <%# Eval("Price")%>
                                    </div>
                                </div>
                                <a href="<%# Eval("Url") %>" class="eStore_btn">More</a>
                                <div class="clearfix" />
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <div class="clearfix">
            </div>
            <div class="carousel-control">
                <a class="prev" href="#"></a><a class="next" href="#"></a>
            </div>
        </div>
    </div>
</div>
<!--container-->
<script>
    $(function () {
        $(".eStore_index_proContent").append("<div class='clearfix'></div>");

        $(".carouselBannerSingle").each(function () {
            var id = $(this).attr('id');
            id = "#" + id;
            //console.log(id);
            $(id).find("ul").carouFredSel({
                auto: false,
                scroll: 1,
                prev: id + ' .prev',
                next: id + ' .next',
                pagination: id + ' .pager',
                items: {
                    start: "random"
                }
            });
        });
    });
</script>
