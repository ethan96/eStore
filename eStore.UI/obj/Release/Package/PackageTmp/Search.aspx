<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="Search.aspx.cs" Inherits="eStore.UI.Search" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/search") %>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/search")%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="eStore_breadcrumb eStore_block980">
        <a href="/">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search)%>
    </div>
    <!--breadcrumb-->
    <asp:Literal ID="lNoMatchedMessage" Text="No matched product found" runat="server"
        Visible="false"></asp:Literal>

    <div class="eStore_container_top">
        <div class="eStore_search_categories" data-bind="visible: categories().length > 0">
            <div class="eStore_search_subject"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Category")%></div>
            <div data-bind="foreach: categories()">
                <div class="categories_section">
                     <a class="categories_title_link" data-bind="attr: { href: Url }">
                         <h4 class="categories_section_title" data-bind="html: Name, attr: { href: Url }"></h4></a>
                  <p class="categories_section_text" data-bind="html: Description"></p>
                </div>
            </div>
        <div class="clearfix"></div>    
      </div>
    </div>


    <div class="eStore_container eStore_block980" data-bind="visible:!isProcessing()&&hasMatchedItems()">

      <div class="eStore_search_refine">
          <div class="eStore_search_subject"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Refine")%></div>
      <div class="refine_block">
        <div class="refine_section" id="refine_title_1">
          <div class="refine_section_title">
              <a href="#eStore_search_products">Products </a>
              <i class="fa fa-angle-down"></i>
                <i class="fa fa-angle-up"></i> 
          </div>
          <ul class="refine_section_list" data-bind="foreach: { data: groups(), afterRender: groupsRenderedHandler }, visible: groups().length > 0">
            <li>
              <div class="table">
                <div class="row">
                  <div class="td"><input type="checkbox" data-bind="checkedValue: Id, checked: $parent.selectedGroup" /></div>
                  <div class="td mousehand" data-bind="click: $parent.changinggroup"><label data-bind="html: Name"></label> 
                      (<span class="blue" data-bind="text: Count">0</span>)</div>
                </div>
              </div>
            </li>
          </ul>
        </div>

        <div class="refine_section" id="refine_title_2" data-bind="visible: remmarktings().length > 0">
          <div class="refine_section_title"><a href="#marketing_resources"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Marketing_Resources")%> </a>
              <i class="fa fa-angle-down"></i>
                <i class="fa fa-angle-up"></i>
          </div>
          <ul class="refine_section_list" data-bind="foreach: { data: marketGroups(), as: 'g'  }">
            <li>
              <div class="table">
                <div class="row">
                  <div class="td mousehand" data-bind="click: $parent.changeMarket, css: { hov: $root.marketCategory() == g.Key }">
                      <label data-bind="text: g.Key"></label>  (<span data-bind="text: g.Count"></span>)</div>
                </div>
              </div>
            </li>
          </ul>
        </div>
        
      </div>
    
  </div>

        <div class="eStore_search_content">
            <div>
                <div class="eStore_search_subject"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Products")%>
                    <span class="eStore_search_title">
                        <span data-bind="text: count()"></span> <%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Search_Results_for")%> <span class="eStore_search_title_count"
                        data-bind="text: keywords()"></span>
                </span>
                </div>
                
                <div class="fixed"></div>
            </div>
            
            <div class="eStore_search_content_listBlock row10">
                <div class="float-left search-page">
                    Page <span data-bind="text: pageNumber "></span> of <span data-bind="text: totalPages"></span>
                </div>
                <div class="float-right">
                    <span class="eStore_search_content_listBlock_page">
                        <a class="prev" data-bind="click: previous, enabled: hasPrevious"><i class="fa fa-chevron-left"></i></a>
                        <a class="next" data-bind="click: next, enabled: hasNext"><i class="fa fa-chevron-right"></i></a>
                    </span>
                </div>
                <div class="product_sort_wrap float-right">
                <div class="product_sort">
                  <span class="arrow">
                    <i class="fa fa-chevron-down"></i>
                    <i class="fa fa-chevron-up"></i>
                  </span>
                  <div class="product_sort_more_text">Relevance</div>
                  <ul class="product_sort_list" style="display: none;">
                    <li><a href="#" data-bind="click: sort.bind($data, 'Depth')" data-text="Relevance">Relevance</a></li>
                    <li><a href="#" data-bind="click: sort.bind($data, 'OnSale')" data-text="On Sale">On Sale</a></li>
                    <li><a href="#" data-bind="click: sort.bind($data, 'High')" data-text="Price High to Low">Price High to Low</a></li>
                    <li><a href="#" data-bind="click: sort.bind($data, 'Low')" data-text="Price Low to High">Price Low to High</a></li>
                    <li><a href="#" data-bind="click: sort.bind($data, 'New')" data-text="New">New</a></li>
                  </ul>            
                </div>
              </div>
            </div>
            <!--listBlock-->
            <div class='clearfix'>
            </div>
            <div class="eStore_search_content_btnBlock row10">
                <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">Compare(<span
                    data-bind="text: comparison().length"></span>)</a></div>
            <div class="eStore_search_content_productBlock row20" data-bind="foreach: items()">
                <div class="eStore_productBlock">
                    <div class="eStore_productBlock_pic row10">
                        <a data-bind="attr: { href: Url}">
                            <img class="lazy" src="/images/igback.png" data-bind="attr: { 'data-src': Image, 'alt': Id}" /></a></div>
                    <div class="eStore_productBlock_txt row10">
                        <a class="eStore_productBlock_name" data-bind="html: Name,attr: { href: Url}"></a>
                        <span class="icon" data-bind="foreach: Status"><img data-bind="attr: { src: '/images/' + $data + '.gif', alt: $data }" /></span>
                        <div class="eStore_productBlock_att" data-bind="html: Description">
                        </div>
                    </div>
                    <div class="eStore_productBlock_action">
                        <div class="eStore_productBlock_price row10" data-bind="html:Price">
                        </div>
                        <div class="eStore_productBlock_btn">
                            <span class="compareBox">
                                <input type="checkbox" data-bind="checkedValue: $data, checked: $root.comparison" />Compare</span><a
                                    data-bind="attr: { href: Url}" class="eStore_btn">More</a></div>
                    </div>
                </div>
            </div>
            <!--productBlock-->
            <div class='clearfix'>
            </div>
            <div class="eStore_search_content_btnBlock row10">
                <a href="#" class="eStore_btn deepBlue" data-bind="click: compare">Compare(<span
                    data-bind="text: comparison().length"></span>)</a></div>
            <!--listBlock-->

            <div class="pages_control row10">
              <ul>
                  <li><a href="#" data-bind="click: firstPage, enabled: hasPrevious">First</a></li>
                  <li><a href="#" data-bind="click: previous, enabled: hasPrevious">Prev</a></li>
                  <!-- ko foreach: {data: Pages(), as: 'p'} -->
                    <li><a href="#" data-bind="click: $root.changePage.bind($data, p), text: p, css: { active: p == $root.pageNumber()}"></a></li>
                  <!-- /ko -->
                  <li><a href="#" data-bind="click: next, enabled: hasNext">Next</a></li>
                  <li><a href="#" data-bind="click: lastPage, enabled: hasNext">Last</a></li>
              </ul>
            </div>

        </div>
        <!--eStore_content_search-->

        <div id="refine_title_2" class="eStore_marketing_resource" data-bind="visible: remmarktings().length > 0 && !isProcessing()">
            <div class="marketing_resources" id="marketing_resources">
              <div class="eStore_search_subject"><%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Marketing_Resources")%></div>
                    <!-- ko foreach: {data: remmarktings(), as: 'r'} -->
              <div class="marketing_resources_section">
                <h4 class="marketing_resource_title" data-bind="html: r.Title"></h4>
                <a data-bind="text: r.Url, attr: { href: r.Url }"></a>
                <p data-bind="html: r.ShortDesc">
                </p>
              </div>
                    <!-- /ko -->
            </div><!-- marketing_resources : end -->
            <div class="pages_control row10">
              <ul>
                  <li><a href="#" data-bind="click: firstMarketPage, enabled: hasMarketPrevious">First</a></li>
                  <li><a href="#" data-bind="click: previousMarket, enabled: hasMarketPrevious">Prev</a></li>
                  <!-- ko foreach: {data: MarketPages(), as: 'p'} -->
                    <li><a href="#" data-bind="click: $root.changeMarketPage.bind($data, p), text: p, css: { active: p == $root.pageMarketNumber() }"></a></li>
                  <!-- /ko -->
                  <li><a href="#" data-bind="click: nextMarket, enabled: hasMarketNext">Next</a></li>
                  <li><a href="#" data-bind="click: lastMarketPage, enabled: hasMarketNext">Last</a></li>
              </ul>
            </div>
        </div>
    </div>

    <div class="eStore_container eStore_block980" data-bind="visible:!hasMatchedItems()&&!isProcessing()">
        <h2> No products were found. </h2><div class="clear">
        </div>
        <h4>
            Some Search Tips:</h4>
        <div class="clear">
        </div>
        <ul style="list-style-type: disc; font-size: 15px; line-height: 22px;">
            <li>Make sure all words are spelled correctly.</li>
            <li>Try different keywords.</li>
            <li>The product you'd like to search might be phased out and please contact our sales
                for support.</li>
        </ul>
    </div>
    <div class="eStore_container eStore_block980" data-bind="visible:isProcessing()">
        <img src="/images/ajax-loader.gif" />
    </div>
    <asp:HiddenField ID="hkeywords" runat="server" ClientIDMode="Static" Value='<%=skey %>' />
</asp:Content>
