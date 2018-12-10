<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="ProductCategoryV4.aspx.cs" Inherits="eStore.UI.Product.ProductCategoryV4" EnableViewState="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/category")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/category")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <div class="eStore_category_banner row20">
        <div class="eStore_block980" data-bind="style:{ backgroundImage: category().BannerImage }">
            <div class="eStore_category_banner_txt float-left">
                <h1 data-bind="html: category().Name">
                    <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal></h1>
                <span data-bind="html: category().Description">
                    <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
                </span>
            </div>
        </div>
    </div>
    <!--banner-->
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
<div class="eStore_category_link hiddenitem">
    <div class="eStore_category_moblieLink">
        <span data-class=".eStore_category_link_linkBlock">category</span> <span data-class=".eStore_filter"
            data-bind="visible:factors().length>0">filter</span>
    </div>
    <asp:Literal ID="lSubCategories" runat="server" EnableViewState="false"></asp:Literal>
    <div class="eStore_filter eStore_category_moblieBlock" data-bind="foreach:{data:factors(),afterRender:factorsRenderedHandler},visible:factors().length>0">
        <!-- ko if: IsUsed -->
        <div>
            <div class="eStore_selectTitle eStore_openBox openBox" data-bind="text: Name">
            </div>
            <ul data-bind="foreach: SubFactors" style="display: none">
                <!-- ko if: IsUsed -->
                <li>
                    <div class="eStore_selectTitle eStore_openBox openBox" data-bind="text: Name">
                    </div>
                    <ul data-bind="foreach: SubFactors" style="display: none">
                        <!-- ko if: IsUsed -->
                        <li><span class="checkboxBlock">
                            <input type="checkbox" />
                            <label data-bind="html: Name">
                            </label>
                        </span><span>(7)</span></li>
                        <!-- /ko -->
                    </ul>
                </li>
                <!-- /ko -->
            </ul>
        </div>
        <!-- /ko -->
    </div>
    <div id="storeSideAds">
    </div>
</div>
<asp:Panel ID="pMatrix" runat="server">    
<div id="category-matrix" class="hiddenitem">
<div class="eStore_category_content eStore_categoryFull">
        <div id="matrix-block"></div>
            <div class="eStore_category_content_btnBlock row10">
                <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: comparison().length">0</span>)</a>
            </div>
            <div id="matrix-template" data-bind="template: { name: matrixTemplate, data: $data}"></div>
            <!--productBlock-->
            <div class="eStore_category_content_btnBlock row10">
                <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: comparison().length">0</span>)</a></div>
        </div>
</div>
</asp:Panel>

<div id="category-list" class="hiddenitem">
    <div class="eStore_category_content" data-bind="visible:(category().ProductCount>0)">
        <div id="list-block">
            <div id="pro-block">
             <div class="eStore_category_content_listBlock row10">
            <div class="float-left">
                <span class="eStore_category_content_listBlock_results">Results per page <a data-bind="click: changenbPerPage.bind($data,9),css:{on: nbPerPage()==9}">
                    9</a> <a data-bind="click: changenbPerPage.bind($data,18),css:{on: nbPerPage()==18}">
                        18</a> |</span><span class="eStore_category_content_listBlock_price">Sort by Price <a
                            data-bind="click: sort.bind($data,false),css:{on: isSortAsc()==false}">Highest</a>
                            / <a data-bind="click: sort.bind($data,true),css:{on: isSortAsc()==true}">Lowest</a></span>
            </div>
            <div class="float-right">
                <span class="eStore_category_content_listBlock_style">
                <a class="eStore_style_byTable" data-bind="visible: category().isTabCategory, click: selectMatrixTab"></a>
                <a class="eStore_style_byList on"></a>
                <a class="eStore_style_byPhoto"></a></span><span class="eStore_category_content_listBlock_page">
                    Page <span data-bind="text:pageNumber "></span> of <span data-bind="text: totalPages">
                    </span><a class="prev" data-bind="click: previous, enabled: hasPrevious"></a><a class="next"
                        data-bind="click: next, enabled: hasNext"></a></span>
                </div>
                </div>
                <div class='clearfix'></div>
            </div>            
        </div>
        <div class="eStore_category_content_btnBlock row10">
            <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: comparison().length"></span>)</a></div>
        <div class='clearfix'>
        </div>
        <div class="eStore_category_content_productBlock row20 byList" data-bind="foreach: Products">
            <div class="eStore_productBlock">
                <div class="eStore_productBlock_pic row10">
                    <a data-bind="attr: { href: Url}">
                        <img data-bind="attr: { src: Image, alt: Description}" /></a></div>
                <div class="eStore_productBlock_txt row10">
                    <a class="eStore_productBlock_name" data-bind="text: Name,attr: { href: Url}"></a>
                    <span class="icon">
                        <!-- ko foreach: Status --> 
                        <img data-bind="attr: { src: '/images/'+$data+'.gif',alt:$data}" />
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
                            <input type="checkbox" data-bind="checkedValue: Id, checked: $root.comparison" />
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%></span><a data-bind="attr: { href: Url}" class="eStore_btn">More</a></div>
                </div>
            </div>
        </div>
        <!--productBlock-->
        <div class='clearfix'>
        </div>
        <div class="eStore_category_content_btnBlock row10">
            <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%>(<span data-bind="text: comparison().length"></span>)</a></div>
        <div class="eStore_category_content_listBlock row20">
            <div class="float-right">
                <span class="eStore_category_content_listBlock_page">Page <span data-bind="text:pageNumber ">
                </span> of <span data-bind="text: totalPages"></span><a class="prev" data-bind="click: previous, enabled: hasPrevious">
                </a><a class="next" data-bind="click: next, enabled: hasNext"></a></span>
            </div>
        </div>
        <!--listBlock-->
    </div>
    <div class="clearfix">
    </div>
</div>
<div id="moreCategoryInfor">
</div>
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
                            <td class="checkboxBlock"><input type="checkbox" data-bind="checkedValue: Product.Id, checked: $root.comparison" /></td>
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
<div class="eStore_productLink" data-bind="foreach: rootCategory().Children, visible: rootCategory().hasChild">
    <a href="#" data-bind="text: Name, attr: { class: isSelect() ? 'on' : '', ref: Id }, visible: isTabCategory, click: $parent.changeItem"></a>
</div>
<div class="eStore_productContent eStore_productBlock" style="padding-top:0px; padding-bottom: 0px" data-bind="visible: selectItem().Name != undefined">
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
                            <select data-bind="foreach: Values, event: { change: $root.changeAttr }, selectedOptions: (defaultOption || [])">
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
            <div class="selectBlock"><div class="input"><input type="checkbox" data-bind="checkedValue: Id, checked: $root.comparison" /></div></div>
            <div class="numberBlock">
                <a data-bind="text: Name, attr: {href: Url}"></a>
                <span class="icon" data-bind="foreach: Status"><img data-bind="attr: { src: '/images/' + $data + '.gif' }" /></span>
            </div>
            <div class="typeBlock">
                <ul class="fullBlock">
                <!-- ko foreach: {data: $root.selectItem().Specs, as: 'spec'} -->
                    <li>
                    <!-- ko foreach: {data: Values, as: 'item'} -->
                        <div class="groupBlock" data-bind="html: $root.getmargProAttr($data, product)"></div>
                    <!-- /ko -->
                    </li>
                <!-- /ko -->
                </ul>
                <ul data-bind="html: Description, visible: ($root.selectItem().Specs == undefined || $root.selectItem().Specs.length == 0)" style="padding: 7px;"></ul>
            </div>
            <div class="priceBlock" data-bind="html: Price"></div>
            <div class="clearfix"></div>
        </li>
        <!-- /ko -->
    </ul>
</div>
</div>
</script>
    <!--recommended-->
    <!--eStore_content_category-->
    <asp:HiddenField ID="hcategory" runat="server" ClientIDMode="Static" />
      <asp:HiddenField ID="hcselectedategory" runat="server" ClientIDMode="Static" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
 
</asp:Content>
