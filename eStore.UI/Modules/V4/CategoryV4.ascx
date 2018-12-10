<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryV4.ascx.cs" Inherits="eStore.UI.Modules.V4.CategoryV4" %>

<%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/category")%>
<%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/categoryV4S")%>

<div class="eStore_category_link">
    <div class="eStore_category_moblieLink">
        <span data-class=".eStore_category_link_linkBlock">category</span>
    </div>
    <div id="cate-sub">
        <asp:Literal ID="lSubCategories" runat="server" EnableViewState="false"></asp:Literal>
    </div>    
    <div id="cateAttr-filter"></div>
    <div id="storeSideAds"></div> 
</div>

<asp:Panel ID="pMatrix" runat="server">    
<div id="category-matrix" class="hiddenitem">
<div class="eStore_category_content eStore_categoryFull">
    <div id="matrix-block"></div>
        <div class="eStore_category_content_btnBlock row10">
            <a class="eStore_btn deepBlue acompare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: Comparison().length" class="scompare">0</span>)</a>
        </div>
        <div id="matrix-template" data-bind="template: { name: matrixTemplate, data: $data}"></div>
        <!--productBlock-->
        <div class="eStore_category_content_btnBlock row10">
            <a class="eStore_btn deepBlue acompare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: Comparison().length" class="scompare">0</span>)</a></div>
    </div>
</div>
</asp:Panel>

<div id="category-list">
    <div class="eStore_category_content" data-bind="visible: (Products().length > 0)">
        <div id="list-block">
            <div id="pro-block">
             <div class="eStore_category_content_listBlock row10">
            <div class="float-left">
                <span class="eStore_category_content_listBlock_results">Results per page 
                    <asp:PlaceHolder ID="phSize" runat="server"></asp:PlaceHolder>
                    |</span>
                <span class="eStore_category_content_listBlock_price">
                    Sort by Price 
                    <asp:HyperLink ID="hlHighest" runat="server">Highest</asp:HyperLink> / 
                    <asp:HyperLink ID="hlLowest" runat="server">Lowest</asp:HyperLink>
                    <%if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("SortByLatestOnCategory")) {%>
                         / <asp:HyperLink ID="ltLaest" runat="server">Latest</asp:HyperLink>
                    <% } %>
                </span>
            </div>
            <div class="float-right">
                <span class="eStore_category_content_listBlock_style">
                <a class="eStore_style_byTable" style="display: none;" data-bind="visible: isTabCategory()"></a>
                <a class="eStore_style_byList on"></a>
                <a class="eStore_style_byPhoto"></a></span><span class="eStore_category_content_listBlock_page">
                    Page <span data-bind="text: pageNumber()">
                        <asp:Literal ID="ltPage" runat="server"></asp:Literal></span> of <span id="totalPagesNumber" data-procount="<%= productCount %>" data-bind="text: totalPages()">
                            <asp:Literal ID="ltPageCount" runat="server"></asp:Literal></span>
                    <asp:HyperLink ID="hprev" runat="server" CssClass="prev"></asp:HyperLink>
                    <asp:HyperLink ID="hnext" runat="server" CssClass="next"></asp:HyperLink></span>
                </div>
                </div>
                <div class='clearfix'></div>
            </div>            
        </div>
        <div class="eStore_category_content_btnBlock row10">
            <div class="application-tag"></div>
            <a class="eStore_btn deepBlue acompare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: Comparison().length" class="scompare">0</span>)</a></div>
        <div class='clearfix'>
        </div>

        <div class="eStore_category_content_productBlock row20 byList">
            <div id="dpost" style="display: none;" data-bind="visible: (Products().length > 0)">
                <!-- ko foreach: Products() --> 
                <div class="eStore_productBlock">
                    <div class="eStore_productBlock_pic row10">
                        <a data-bind="attr: { href: Url}">
                            <img class="lazy" src="/images/igback.png" data-bind="attr: { 'data-src': Image, alt: Description}" alt="product image"/></a></div>
                    <div class="eStore_productBlock_txt row10">
                        <a class="eStore_productBlock_name" data-bind="text: Name,attr: { href: Url}"></a>
                        <span class="icon">
                            <!-- ko foreach: Status --> 
                            <img data-bind="attr: { src: '/images/'+$data+'.gif',alt:$data}" alt="product status" />
                            <!-- /ko --></span>
                        <div class="eStore_productBlock_att" data-bind="html: Description">
                        </div>
                        <ol class="eStore_listPoint" data-bind="html: Fetures">
                        </ol>
                    </div>
                    <div class="eStore_productBlock_action">
                        <div class="eStore_productBlock_price row10" data-bind="html:Price">
                        </div>
                        <div class="eStore_productBlock_btn">
                            <span class="compareBox">
                                <input type="checkbox" data-bind="checkedValue: Id, checked: $root.Comparison" />
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%></span><a data-bind="attr: { href: Url}" class="eStore_btn">More</a></div>
                    </div>
                </div>
                <!-- /ko -->
            </div>
            <div id="dfirst">
            <asp:Repeater ID="rpProducts" runat="server" EnableViewState ="false">
                <ItemTemplate>
                    <div class="eStore_productBlock">
                        <div class="eStore_productBlock_pic row10">
                            <a href='<%# Eval("Url") %>'>
                                <img class="lazy" src="/images/igback.png" data-src='<%# Eval("Image") %>' alt='<%# Eval("Description") %>' /></a></div>
                        <div class="eStore_productBlock_txt row10">
                            <a class="eStore_productBlock_name" href='<%# Eval("Url") %>'><%# Eval("Name") %></a>
                            <span class="icon">
                                <%# BindImageIco(Eval("Status")) %></span>
                            <div class="eStore_productBlock_att">
                                <%# Eval("Description") %>
                            </div>
                            <ol class="eStore_listPoint">
                                <%# Eval("Fetures") %>
                            </ol>
                        </div>
                        <div class="eStore_productBlock_action">
                            <div class="eStore_productBlock_price row10">
                                <%# Eval("Price") %>
                            </div>
                            <div class="eStore_productBlock_btn">
                                <span class="compareBox">
                                    <input type="checkbox" value='<%# Eval("Id") %>' />
                                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%></span>
                                <a href='<%# Eval("Url") %>' class="eStore_btn">More</a></div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            </div>
            <div class="clearfix">
        </div>
        </div>
        <!--productBlock-->
        <div class='clearfix'>
        </div>
        <div class="eStore_category_content_btnBlock row10">
            <a class="eStore_btn deepBlue acompare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: Comparison().length" class="scompare">0</span>)</a></div>
        <div class="eStore_category_content_listBlock row20">
            <div class="float-right">
                <span class="eStore_category_content_listBlock_page">
                    Page <span data-bind="text: pageNumber()">
                        <asp:Literal ID="ltBpage" runat="server"></asp:Literal></span> of <span data-bind="text: totalPages()">
                            <asp:Literal ID="ltBpageCount" runat="server"></asp:Literal></span>
                    <asp:HyperLink ID="hBprev" runat="server" CssClass="prev"></asp:HyperLink>
                    <asp:HyperLink ID="hBnext" runat="server" CssClass="next"></asp:HyperLink></span>
            </div>
        </div>
        <!--listBlock-->
    </div>
    <div class="clearfix">
    </div>
    <asp:HiddenField ID="hdCompares" runat="server" EnableViewState="true" ClientIDMode="Static" />
    <asp:HiddenField ID="hdCategoryid" runat="server" EnableViewState="true" ClientIDMode="Static" />
    <asp:HiddenField ID="hdCategoryDisType" runat="server" EnableViewState="true" ClientIDMode="Static" />
</div>
<div id="moreCategoryInfor">
</div>
<eStore:Advertisement ID="Advertisement1" runat="server" />
<script id="_tmpcategories" type="text/x-jquery-tmpl">
<div class="eStore_product_moreInfo eStore_other_BGBlock row20" data-bind="visible: $data.length  > 0">
<h4><%=eStore.Presentation.eStoreLocalization.Tanslation("ScriptMessage_Check_More_Information_for_Your_Project")%></h4>
<!-- ko foreach:  $data -->
    <div class="eStore_productBlock">
        <div class="eStore_productBlock_pic row10"><img data-bind="attr: { src: Image, title: Name,alt:Description }" /></div>
        <div class="eStore_productBlock_txt row10" data-bind="text: Name"></div>
        <div class="eStore_productBlock_price row10">
            <div class="priceOrange" data-bind="html: Price"></div>
        </div>
        <a data-bind="attr: { href: Url }" class="eStore_btn">More</a>
        <div class="clearfix"></div>
    </div><!--product end-->
<!-- /ko -->
<div class="clearfix"></div>
</div>
</script>

<script type="text/html" id="mobileMatrix-template">
<div class="eStore_category_content_productBlock row20 byTable_mobile">
    <!-- ko foreach: {data: selectItemFilterArr(), as: 'spec'} -->
    <div class="eStore_tableContent">
        <h3 data-bind="text: Name"></h3>
        <ul>
            <!-- ko foreach: {data: Values, as: 'item'} -->
            <li>
                <h4 class="eStore_slideToggle" data-bind="text: item.Name"></h4>
                <div class="eStore_category_tableList">
                    <table cellpadding="0" cellspacing="0" width="100%" data-bind="foreach: {data: $root.mergerProductByAttr($data), as: 'pro'}">
                        <tr>
                            <td class="checkboxBlock"><input type="checkbox" data-bind="checkedValue: Product.Id, checked: $root.Comparison" /></td>
                            <td class="numberBlock"><a href="#" data-bind="text: ProductName, attr: {href: Product.Url}"></a>
                                <span class="icon" data-bind="foreach: Product.Status"><img data-bind="attr: { src: '/images/' + $data + '.gif',alt:$data }" /></span></td>
                            <td class="txtBlock" data-bind="text: AttrValue"></td>
                        </tr>
                    </table>
                </div>
            </li>
            <!-- /ko -->
        </ul>
    </div>
    <!-- /ko -->
</div>
</script>

<script type="text/html" id="pcMatrix-template">
<div class="eStore_category_content_productBlock row20 byTable">
<div class="eStore_productLink" data-bind="foreach: { data: Parent().Children, as: 'sub'}, visible: Parent().Children != undefined">
    <a data-bind="text: Name, attr: { class: (sub.Id == $root.Id() ? 'on' : '')}, click: $parent.changeItem"></a>
</div>
<div class="eStore_productContent eStore_productBlock" style="padding-top:0px; padding-bottom: 0px">
    <div class="scrollBG scrollLeft noneClick"><span></span></div>
    <div class="scrollBG scrollRight noneClick"><span></span></div>
    <ul>
        <li class="title">
            <div class="selectBlock"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select)%></div>
            <div class="numberBlock"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PartNumber)%></div>
            <div class="typeBlock">
                <ul class="fullBlock">
                <!-- ko foreach: {data: selectItemFilterArr(), as: 'spec'} -->
                    <li>
                        <h6 data-bind="text: Name"></h6>
                        <!-- ko foreach: {data: Values, as: 'item'} -->
                        <div class="groupBlock">
                            <span data-bind="text: item.Name"></span>
                            <select data-bind="foreach: Values, event: { change: $root.changeAttr }, selectedOptions: (SelectId || [])">
                                <option data-bind="text: Name, value: Id"></option>
                            </select>
                        </div>
                        <!-- /ko -->
                    </li>
                <!-- /ko -->
                    <div class="clearfix"></div>
                </ul>
            </div>
            <div class="priceBlock"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price)%></div>
            <div class="clearfix"></div>
        </li>
        <!-- ko foreach: {data: mappingProducts, as: 'product'} -->
        <li>
            <div class="selectBlock"><div class="input"><input type="checkbox" data-bind="checkedValue: Id, checked: $root.Comparison" /></div></div>
            <div class="numberBlock">
                <a data-bind="text: Name, attr: {href: Url}"></a>
                <span class="icon" data-bind="foreach: Status"><img data-bind="attr: { src: '/images/' + $data + '.gif' }" /></span>
            </div>
            <div class="typeBlock">
                <ul class="fullBlock">
                <!-- ko foreach: {data: $root.Specs, as: 'spec'} -->
                    <li>
                    <!-- ko foreach: {data: Values, as: 'item'} -->
                        <div class="groupBlock" data-bind="html: $root.getmargProAttr($data, product)"></div>
                    <!-- /ko -->
                    </li>
                <!-- /ko -->
                </ul>
                <ul data-bind="html: Description, visible: ($root.Specs() == undefined || $root.Specs().length == 0)" style="padding: 7px;"></ul>
            </div>
            <div class="priceBlock" data-bind="html: Price"></div>
            <div class="clearfix"></div>
        </li>
        <!-- /ko -->
    </ul>
</div>
</div>
</script>