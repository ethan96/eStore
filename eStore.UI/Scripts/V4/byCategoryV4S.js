jQuery.fn.ShowMoreCategory = function (id) {
    var objtemp = $(this);
    $.getJSON(GetStoreLocation() + 'api/category/ProductCategories/' + id,
    function (data) {
        if (data != null) {
            var temp = $("<div>");
            // apply "template" binding to div with specified data
            var name = "_tmpcategories";

            ko.applyBindingsToNode(temp[0], { template: { name: name, data: data } });
            // save inner html of temporary div
            var html = temp.html();
            objtemp.html(html);
        }
        else
            objtemp.html("");
    });
}

$(function () {
    viewmodel = new CategoryModel();
    viewmodel.Id($("#hdCategoryid").val());
    viewmodel.nbPerPage($(".eStore_category_content_listBlock_results a.on").text()); // set page size on firs bind
    viewmodel.ProductCount(Number($("#totalPagesNumber").attr("data-procount")));
    //switch display type
    $(".eStore_category_content_listBlock_style a").bind("click", function () { changblock($(this)); });
    if ($.cookie('eStore_category_displayModel') == "byPhoto") {
        $(".eStore_category_content_listBlock_style a.eStore_style_byPhoto").click();
    };

    BindCompare();

    // category click
    ChangeCategoryLayt("#cate-sub ");


    // select category
    $(".eStore_category_link a[ref]").on("click", function (e)
    {
        var $item = $(this);
        if ($item.attr("skipjs") != undefined)
        {
            document.location = $item.attr("href");
            return;
        }

        $(".eStore_category_link a[ref]").removeClass("on");
        if ($item.parent().parent().is("ol")) {
            $(".eStore_category_link .dcate").removeClass("on");
            $item.addClass("on");
        }
        else if ($item.parent().hasClass("dcate")) {
            $(".eStore_category_link .dcate").removeClass("on");
            $item.parent().click();
        }
        else
            $item.addClass("on");
        viewmodel.pageNumber(1);
        var cateid = $item.attr("ref");
        viewmodel.filterType($item.attr("parentpath") || "");
        viewmodel.MatrixPage($item.attr("istab") == "true");
        if (viewmodel.MatrixPage()) {
            ShowMatrixTab();
        }

       
        var id = $(this).attr("ref");
        viewmodel.PageState().URL = $(this).attr("href");
        viewmodel.PageState().PageUpdated = true;
        viewmodel.PageState().State = {Title:"", filterType: viewmodel.filterType(), MatrixPage: viewmodel.MatrixPage(), id: id };

        viewmodel.loadcategory(cateid);
        e.stopPropagation();//阻止向上冒泡
        if ($(".eStore_mobile").is(':visible')) //mobile
        {
            if ($.cookie('eStore_category_displayModel') == "byPhoto")
                $(".eStore_category_content_listBlock_style .eStore_style_byPhoto").click();
            else
                $(".eStore_category_content_listBlock_style .eStore_style_byList").click();
        }
        return false;
    });

    window.onpopstate = function (event) {
        var currentState = history.state;
        viewmodel.filterType(currentState.filterType);
        viewmodel.MatrixPage(currentState.MatrixPage);
        viewmodel.loadcategory(currentState.id);
        document.title = currentState.Title;
        var subcategory = $(".eStore_category_link_linkBlock a[ref='" + currentState.id + "']");
        if (subcategory.length > 0) {
            var hasClass = $(subcategory).hasClass("on");
            if (hasClass) {
                return false;
            };
                
            $("a.category_linkBlock").removeClass("on");
            $(subcategory).addClass("on");
            if ($(subcategory).hasClass("haveList")) {
                $(subcategory).parent().parent().find(".eStore_category_link_linkListBlock").slideUp(0);
                //$(".eStore_category_link_linkListBlock").slideUp();
                $(subcategory).siblings('ul').slideDown();
            }
            return false;
        }
    };

    // show more category on buttom
    $("#moreCategoryInfor").ShowMoreCategory($("#hdCategoryid").val());

    //change page size
    $(".eStore_category_content_listBlock_results a").on("click", function () {
        viewmodel.nbPerPage($(this).text());
        viewmodel.pageNumber(1);
        ReLoadProducts();
        $(".eStore_category_content_listBlock_results a").removeClass("on");
        $(this).addClass("on");
        return false;
    });

    // sort products
    $(".eStore_category_content_listBlock_price a").on("click", function () {
        var text = $(this).attr("data-Sort");
        viewmodel.SortType(text);
        viewmodel.pageNumber(1);
        ReLoadProducts();
        $(".eStore_category_content_listBlock_price a").removeClass("on");
        $(this).addClass("on");
        return false;
    });

    // next or prev
    $(".eStore_category_content_listBlock_page a").on("click", function () {
        var isprev = $(this).hasClass("prev");
        if (isprev) {
            if (viewmodel.pageNumber() == 1)
                return false;
            viewmodel.previous();
        }
        else {
            if (!_hasPostBack)
            {
                if ($(this).attr("href") == undefined)
                    return false;
            }
            viewmodel.next();
        }
        return false;
    });
    // moblie category click
    $(".eStore_category_moblieLink span").click(function () {
        $(this).toggleClass("on").siblings("span").removeClass("on");
        if ($(this).hasClass("on")) {
            var thisClass = $(this).attr("data-class");
            $(thisClass).slideDown().siblings(".eStore_category_moblieBlock").slideUp();
        } else {
            $(".eStore_category_moblieBlock").slideUp();
        }
    });

    // click compare button
    $(".acompare").on("click", function () {
        if (viewmodel.Comparison().length > 0) {
            window.document.location = GetStoreLocation() + "Compare.aspx?parts=" + viewmodel.Comparison().join(",");
        }
        return false;
    });

    //page ready , if matrix page will show tab page
    if ($("#hdCategoryDisType").val().toLowerCase() == "mtrix") {
        $(".eStore_style_byTable").click();
    }
    else if ($("#hdCategoryDisType").val().toLowerCase() == "selectbyspec") {
        loadCategoryAttr($("#hdCategoryid").val());
    }
    fixTableLayout("#category-list  .eStore_category_content_productBlock.byPhoto #dfirst", ".eStore_productBlock");

    var hashId = window.location.hash;
    if (hashId != "") {
        hashId = hashId.substring(1);
    }
    if (hashId != "") {
        var subcategory = $(".eStore_category_link_linkBlock a[ref='" + hashId + "']");
        if (subcategory.length > 0) {
            subcategory.click();
        }
    }

    LazyLoadImg();
});

//product model
var Product = function (options) {
    options = options || {};
    this.Id = options.Id || "";
    this.Name = options.Name || "";
    this.Description = options.Description || "";
    this.Fetures = options.Fetures || "";
    this.Image = options.Image || "";
    this.Status = options.Status || "";
    this.Price = options.Price || "";
    this.SalePrice = options.SalePrice || "";
    this.CurrencySign = options.CurrencySign || "";
    this.Url = options.Url || "";
    this.Sequence = options.Sequence || "";
    this.Matrix = options.Matrix || [];
};
var PageState = function (options) {
    options = options || {};
    this.URL = options.URL || location.href;
    this.Title = options.Title || document.title;
    this.State = options.State || {};
    this.PageUpdated = options.PageUpdated || false;
    this.pushState = function () {
        try {
            if (this.PageUpdated) {
                this.State.Title = this.Title;
                history.pushState(this.State, this.Title, this.URL);
                document.title = this.Title;
                this.PageUpdated = false;
            }

        }
        catch (ex)
        { };
    };
}
var CategoryModel = function () {
    var self = this;
    self.Id = ko.observable(0);
    self.Name = ko.observable("");
    self.Description = ko.observable("");
    self.HtmlContext = ko.observable("");
    self.ShowReadMore = ko.observable(false);
    self.Image = ko.observable("");
    self.BannerImage = ko.observable("");
    self.Fetures = ko.observable("");
    self.Breadcrumbs = ko.observable("");
    self.ProductCount = ko.observable(0);
    self.pageNumber = ko.observable(1);
    self.nbPerPage = ko.observable(9);
    self.SortType = ko.observable("");
    self.Products = ko.observableArray([]);
    self.Comparison = ko.observableArray([]);
    self.MatrixPage = ko.observable(false);
    self.filterType = ko.observable("");
    self.PageState = ko.observable(new PageState(undefined));
    self.loadcategory = function (cateid) {
        if (!$(".eStore_category_content").isMasked()) {
            $(".eStore_category_content").mask("");
        }
        var xhr = $.getJSON(GetStoreLocation() + 'api/Category/Get/' + cateid, { withMatrx: self.MatrixPage(), filterType: self.filterType() },
            function (options) {
                self.Id(options.Id || "");
                self.Name(options.Name || "");
                self.Description(options.Description || "");
                self.HtmlContext(options.HtmlContext || "");
                self.ShowReadMore(options.ShowReadMore || false);
                self.Image(options.Image || "");
                self.BannerImage(options.BannerImage || "");
                self.Fetures(options.Fetures || "");
                self.ProductCount(options.ProductCount || 0);
                self.isTabCategory(options.isTabCategory || false);
                self.DisplayType(options.DisplayType);
                if (options.SliderBanners != null && options.SliderBanners.length > 0) {
                    $("#HeaderBanner .cycle-slideshow").find(".cycle-slideitem").remove();
                    $.each(options.SliderBanners, function (i, n) {
                        $("#HeaderBanner .cycle-slideshow").append($(n));
                    });
                    $(".cycle-slideshow").cycle('reinit');
                    $("#HeaderBanner").show();
                    $(".eStore_category_banner").hide();

                }
                else {
                    $("#HeaderBanner").hide();
                    $(".eStore_category_banner").show();
                }
                if (options.DisplayType == "SelectBySpec") {
                    if (cateid != $("#hdCategoryid").val()) {
                        self.CateAttrFilter([]);
                        $("#cateAttr-filter").html("");
                        loadCategoryAttr(cateid);
                    }                    
                }
                else {
                    self.CateAttrFilter(null);
                    $("#cateAttr-filter").html("");
                    $(".application-tag").html("");
                }
                self.BUPhoneNumber(options.BUPhoneNumber || "");
                if (self.MatrixPage()) {
                    self.Parent(options.Parent || []);
                    self.Specs(options.Specs || []);
                    self.selectItemFilterArr(options.Specs || []);
                }
                else {
                    self.Parent(function () { Children: [] });
                }
                $("#totalPagesNumber").attr("data-procount", options.ProductCount || 0);
                self.Breadcrumbs(options.Breadcrumbs || "");
                if (self.Breadcrumbs() != "") {
                    $(".eStore_breadcrumb").html(self.Breadcrumbs());
                }
                if (self.BUPhoneNumber() != "") {
                    $("#top_phonenumber").html(self.BUPhoneNumber());
                    $(".eStore_chatBox_txt .float-left h3").html(self.BUPhoneNumber());
                }
                self.PageState().Title = self.Name();
                self.PageState().pushState();
                self.loadproducts();
                //KoBind();
                $("#hdCategoryid").val(cateid);
                loadAdvertisement(cateid);
                $("#moreCategoryInfor").ShowMoreCategory($("#hdCategoryid").val());
            });
    };
    self.loadproducts = function () {
        if (!$(".eStore_category_content").isMasked()) {
            $(".eStore_category_content").mask("");
        }

        if (self.CateAttrFilter() != null) {
            var redata = { Id: self.Id(), Page: self.pageNumber(), Pagesize: self.nbPerPage(), SortType: self.SortType(), FilterType: self.filterType(), MatrixPage: true };
            redata.CategoryAttrs = [];
            $.each(self.CateAttrFilter(), function (i, n) {
                redata.CategoryAttrs.push(n.Value);
            });
            $.post(GetStoreLocation() + 'api/Category/ProductsPost', redata).success(function (data) {
                self.setProducts(data.Products);
                self.ProductCount(data.Count);
                if (self.totalPages() < self.pageNumber())
                    self.pageNumber(1);
                if (data.ProductAttrs == null) {
                    $("#cateAttr-filter li[data-ref]").show();
                }
                else {
                    $.each(data.ProductAttrs, function (i, n) {
                        $("li[data-ref='" + n.Id + "']").addClass("willShow");
                    });
                    $.each(self.CateAttrFilter(), function (i, n){
                        $("li[data-ref='" + n.Id + "']").addClass("willShow");
                    });
                    $("li[data-ref^='" + _selectClickAttr + "']").addClass("willShow");
                    $("#cateAttr-filter li.willShow").show();
                    $("#cateAttr-filter li[data-ref]").not(".willShow").hide();
                    $("#cateAttr-filter li[data-ref]").removeClass("willShow");
                }
                self.SortType(data.SortType);
                if (data.SortType != null && data.SortType != "") {
                    $("a[data-sort]").removeClass("on");
                    $("a[data-sort='" + data.SortType + "']").addClass("on");
                }
                LazyLoadImg();
            });
        }
        else {
            var xhrp = $.getJSON(GetStoreLocation() + 'api/Category/Products/' + self.Id(), { page: self.pageNumber(), pagesize: self.nbPerPage(), SortType: self.SortType(), filterType: self.filterType(), MatrixPage: self.MatrixPage() },
                function (data) {
                    self.setProducts(data);
                    LazyLoadImg();
                });
        }        
    };
    self.setProducts = function (data) {
        var prosucts = [];
        if (data != undefined && data != null) {
            ko.utils.arrayForEach(data, function (item) {
                prosucts.push(new Product(item));
            });
        };
        self.Products(prosucts);
        KoBind();
        //BindCompare();
        if (self.MatrixPage()) {
            self.mappingProducts(prosucts);
            loadTabInfor();
        }
        else {
            fixTableLayout("#category-list  .eStore_category_content_productBlock.byPhoto #dpost", ".eStore_productBlock");
        }
        if (self.ShowReadMore()) {
            addReadMore();
        }
        $(".eStore_category_content").unmask();
        $("#dfirst").remove();
    }
    self.totalPages = ko.computed(function () {
        var div = Math.floor(self.ProductCount() / self.nbPerPage());
        div += self.ProductCount() % self.nbPerPage() > 0 ? 1 : 0;
        return div;
    });
    self.changenbPerPage = function (number) {
        if (self.nbPerPage() != number) {
            self.nbPerPage(number);
            self.pageNumber(1);
            self.loadproducts();
        }
        return false;
    };
    self.previous = function () {
        if (self.pageNumber() != 1) {
            self.pageNumber(self.pageNumber() - 1);
            self.loadproducts();
        }
        return false;
    };
    self.next = function () {
        if (self.pageNumber() < self.totalPages()) {
            self.pageNumber(self.pageNumber() + 1);
            ReLoadProducts();
        }
        return false;
    };


    self.Parent = ko.observable(null);
    self.isTabCategory = ko.observable(false);
    self.DisplayType = ko.observable("");
    self.BUPhoneNumber = ko.observable("");
    self.Specs = ko.observableArray([]);
    self.matrixTemplate = ko.observable("pcMatrix-template");
    self.mappingProducts = ko.observableArray([]);
    self.selectAttrs = ko.observableArray([]); // for matrix
    self.selectItemFilterArr = ko.observableArray([]); // for matrix
    self.CateAttrFilter = ko.observableArray([]); // for list with filter type
    //show product spec
    self.getmargProAttr = function (item, pro) {
        var str = "-";
        $.each(pro.Matrix, function (i, n) {
            if (n.AttrId == item.Id && n.CatID == item.ParentId) {
                str = n.AttrValueName;
                return;
            }
        });
        return str;
    };
    self.mergerProductByAttr = function (attr) {
        var attrPros = [];
        attrPros = $.map(self.mappingProducts(), function (n) {
            var marg = false;
            var attrvalue = "";
            $.each(n.Matrix, function (t, s) {
                if (s.AttrId == attr.Id) { marg = true; attrvalue = s.AttrValueName; return; }
            });
            if (marg)
                return { ProductName: n.Name, AttrValue: attrvalue, Product: n };
        });
        return attrPros;
    }
    // change matrix type
    self.changeAttr = function (obj, event) {
        var valueId = $(event.target).val();
        self.selectAttrs.remove(function (item) { return item.AttrId == obj.Id && item.CatId == obj.ParentId });
        if (valueId != -1) {
            self.selectAttrs.push({ CatId: obj.ParentId, AttrId: obj.Id, ValueId: valueId }); //set select attrs
        }
        var tempAtt = [];
        $.extend(true, tempAtt, self.Specs());
        if (self.selectAttrs().length > 0) {
            var tempPros = [];
            var maxLs = [];
            $.each(self.Products(), function (i, n) { // check product have attr or not
                var allMarg = true;
                $.each(self.selectAttrs(), function (m, v) {
                    var marg = false;
                    $.each(n.Matrix, function (t, s) {
                        if (s.AttrId == v.AttrId && s.AttrValueId == v.ValueId && s.CatID == v.CatId) { marg = true; return; }
                    });
                    if (!marg) { allMarg = false; return; }
                });
                if (allMarg) {
                    tempPros.push(n);
                    $.merge(maxLs, n.Matrix);
                    $.unique(maxLs);
                }
            });
            self.mappingProducts(tempPros);
            //reset attr category dropdown list
            if (obj.Id != -1) {
                $.each(tempAtt, function (i, n) {
                    $.each(n.Values, function (si, sn) {
                        if (sn.Id == obj.Id && obj.ParentId == n.Id) {
                            sn.SelectId = ko.observable([parseInt(valueId)]);
                            //return;
                        }
                        sn.Values = $.map(sn.Values, function (value) {
                            if (value.Id == -1) { return value; }
                            $.each(self.selectAttrs(), function (sai, sa) {
                                if (sa.AttrId == sn.Id && sa.ValueId == value.Id && sa.CatId == n.Id) { sn.SelectId = ko.observable([parseInt(sa.ValueId)]); return false; }
                            });
                            if (self.selectAttrs().length == 1 && self.selectAttrs()[0].AttrId == sn.Id && self.selectAttrs()[0].CatId == n.Id) { return value; }
                            var pms = $.map(maxLs, function (pm) {
                                if (pm.AttrId == sn.Id && pm.AttrValueId == value.Id && pm.CatID == n.Id) { return pm; }
                            });
                            if (pms.length > 0) { return value; }
                        });
                    });
                });
            }
        }
        else
            self.mappingProducts(self.Products());
        self.selectItemFilterArr(tempAtt);
        loadTabInfor();
        fixMatrixNumber();
    };
    self.clearMatrixFilter = function () {
        self.selectItemFilterArr([]);
        self.selectAttrs([]);
        self.mappingProducts([]);
    };
    //change attr tab item
    self.changeItem = function (item) {
        self.MatrixPage(item.isTabCategory);
        if (item.isTabCategory) {
            viewmodel.loadcategory(item.Id);
            var id = item.Id;
            viewmodel.PageState().URL = (item.Url == null ? null : item.Url.replace("~", ""));
            viewmodel.PageState().PageUpdated = true;
            viewmodel.PageState().State = { Title: "", filterType: viewmodel.filterType(), MatrixPage: viewmodel.MatrixPage(), id: id };
        }
        else {
            $(".eStore_style_byList").click();
            $(".eStore_category_link a[ref='" + item.Id + "']").click();
            
        }
    };
    //moble style
    self.styleModel = ko.pureComputed({
        read: function () { },
        write: function (value) {
            if (value == "MOBILE") {
                self.matrixTemplate("mobileMatrix-template");
                $(".eStore_slideToggle").unbind("click").click(function () {
                    $(this).toggleClass("on").siblings().slideToggle("fast");
                });
                $(".eStore_category_link").show();
            }
            else {
                self.matrixTemplate("pcMatrix-template");
                if ($("#category-list").is(':visible'))
                    $(".eStore_category_link").show();
                else
                    $(".eStore_category_link").hide();
            }
        },
        owner: this
    });
    var sortstr = GetQueryString("sort") || GetQueryString("Sort");
    if (sortstr != null) {
        self.SortType(sortstr);
    }
    // show attr filter
    self.showAttrSelectTop = function () {
        var $attrTag = $(".application-tag");
        $attrTag.html("");
        $.each(self.CateAttrFilter(), function (i, an) {
            var $attrSpan = $("<span />");
            $attrSpan.on("click", function () {
                $("input[data-ref='" + an.Id + "']").click()
            });
            $attrTag.append($attrSpan.attr({ "data-id": an.Id }).html(an.Name));            
        });
        $attrTag.append($("<div />").addClass("clearfix"));
    };
};
var viewmodel = null;
var _hasPostBack = false;
var _selectClickAttr = "";
// bind ko model
function KoBind() {
    if (!_hasPostBack) {
        ko.applyBindings(viewmodel);
    }
    _hasPostBack = true;
};
//load product , if ko has bind model will load products
//else load category
function ReLoadProducts() {
    if (!_hasPostBack)
        viewmodel.loadcategory($("#hdCategoryid").val());
    else
        viewmodel.loadproducts();
};

function BindCompare() {
    $(".eStore_category_content_productBlock .eStore_productBlock input[type='checkbox']")
        .on("click", function () {
            var selectitem = $(this).val();
            var arrayObj = viewmodel.Comparison();
            var i = $.inArray(selectitem, arrayObj);
            if ($(this).is(':checked') && i == -1) {
                arrayObj.push(selectitem);
            }
            else if (!$(this).is(':checked') && i >= 0) {
                arrayObj.splice(i, 1);
            }
            viewmodel.Comparison(arrayObj);
            $(".scompare").html(arrayObj.length);
    });
};

//change product show type
function changblock(item) {
    if (item.hasClass("on"))
        return false;
    item.addClass("on");
    item.siblings().removeClass("on");
    if (item.hasClass("eStore_style_byTable")) {
        ShowMatrixTab();
        viewmodel.MatrixPage(true);
        viewmodel.loadcategory(viewmodel.Id()); // reload category with attr and spec
    }
    else {
        $("#category-matrix").hide();
        $("#pro-block").appendTo($("#list-block"));
        $("#category-list, span.eStore_category_content_listBlock_page, .eStore_category_content_listBlock_results, .eStore_category_link").show();
        if (viewmodel.MatrixPage())
        {
            viewmodel.MatrixPage(false);
            //$(".eStore_category_link ol.eStore_category_link_linkBlock a[ref='" + viewmodel.Id() + "']").click();
            viewmodel.loadcategory(viewmodel.Id());
            var selectitem = $(".eStore_category_link a[ref='" + viewmodel.Id() + "']");
            selectitem.addClass("on");
            selectitem.parent("li").siblings().find("a").removeClass("on");
        }
    }
    if (item.hasClass("eStore_style_byList") || item.hasClass("eStore_style_byTable")) {
        $(".eStore_category_content_productBlock").removeClass("byPhoto").addClass("byList");
        $.cookie('eStore_category_displayModel', "byList", { expires: 365, path: '/' });
    }
    else {
        $(".eStore_category_content_productBlock").removeClass("byList").addClass("byPhoto");
        $.cookie('eStore_category_displayModel', "byPhoto", { expires: 365, path: '/' });
    }
    return false;
};

// show matrix tab
function ShowMatrixTab() {
    $(".eStore_category_content_productBlock").removeClass("byPhoto").addClass("byList");
    $("#category-matrix").show();
    $("#category-list, span.eStore_category_content_listBlock_page, .eStore_category_content_listBlock_results, .eStore_category_link").hide();
    $("#pro-block").appendTo($("#matrix-block"));
    var item = $(".eStore_style_byTable");
    item.addClass("on");
    item.siblings().removeClass("on");
};

var leftNumber = 0;
// --- tab category
function loadTabInfor() {
    //-- matrix滚动 ---//
    function noneClickClass() {
        var fixedW = ($('.eStore_productContent .selectBlock').outerWidth()) + ($('.eStore_productContent .numberBlock').outerWidth()) + ($('.eStore_productContent .priceBlock').outerWidth());
        var typeBlockW = $('.eStore_productContent').width() - fixedW;
        var leftPosition = fullBlockW - typeBlockW - 1;
        //var leftNumber = parseInt($('.fullBlock').css('left'));

        if (leftPosition < 0 || leftPosition == NaN) {
            $('.byTable .eStore_productContent .scrollBG.scrollRight').addClass('noneClick');
            if (leftNumber == 0)
                $('.byTable .eStore_productContent .scrollBG.scrollLeft').addClass('noneClick');
        }
        else if (leftNumber == 0) {
            $('.byTable .eStore_productContent .scrollBG').removeClass('noneClick');
            $('.byTable .eStore_productContent .scrollBG.scrollLeft').addClass('noneClick');
        } else if ((-leftNumber) > leftPosition) {
            $('.byTable .eStore_productContent .scrollBG').removeClass('noneClick');
            $('.byTable .eStore_productContent .scrollBG.scrollRight').addClass('noneClick');
        } else {
            $('.byTable .eStore_productContent .scrollBG').removeClass('noneClick');
        }
    }
    function scrollFunction(clickType) {

        if (clickType) {
            $('.byTable .eStore_productContent .fullBlock').animate({
                left: "+=160"
            }, 300, function () {
                leftNumber = parseInt($('.fullBlock').css('left'));
                noneClickClass();
            });
        } else {
            $('.byTable .eStore_productContent .fullBlock').animate({
                left: "-=160"
            }, 300, function () {
                leftNumber = parseInt($('.fullBlock').css('left'));
                noneClickClass();
            });
        }

    }
    var groupBlockNumber = $('.byTable .eStore_productContent .title .groupBlock').length;
    fullBlockW = groupBlockNumber * 160;
    $('.byTable .eStore_productContent .fullBlock').css({ 'width': fullBlockW, "left": 0 });

    $('.scrollBG').unbind("click").click(function () {

        if ($(this).hasClass('noneClick')) {
            return false;
        }
        $('.byTable .eStore_productContent .scrollBG').addClass('noneClick');
        var clickType = $(this).hasClass('scrollLeft');
        scrollFunction(clickType);
    });
    //-- end matrix滚动 -- //


    //table-------------------
    $(window).resize(function () {
        if (_hasPostBack) {
            checkBrowser();
        }
        windowsResize();
    }).resize();

    function windowsResize() {
        //計算輪播剩下的寬度
        var fixedW = ($('.eStore_productContent .selectBlock').outerWidth()) + ($('.eStore_productContent .numberBlock').outerWidth()) + ($('.eStore_productContent .priceBlock').outerWidth());
        var typeBlockW = $('.eStore_productContent').width() - fixedW;
        $('.typeBlock').css('width', typeBlockW);

        //統一title裡面的高度
        $('.byTable .eStore_productContent li.title').find('.typeBlock').css('height', '');

        var thisH = $('.byTable .eStore_productContent li.title').height();
        $('.byTable .eStore_productContent li.title').find('.selectBlock,.numberBlock,.priceBlock').css({
            'padding-top': (thisH - 16) / 2
        });
        $('.byTable .eStore_productContent li.title').find('.typeBlock').css('height', thisH);
        var contentH = $('.eStore_productContent').outerHeight() - 1;
        $('.byTable .eStore_productContent .scrollBG').css('height', contentH);

        //判斷輪播內的數量
        var liNumber = $('.byTable .eStore_productContent .title .fullBlock>li').length;
        if (liNumber == 1) {
            $('.byTable .eStore_productContent .fullBlock h6').css('width', typeBlockW);

            var groupNumber = $('.byTable .eStore_productContent .fullBlock .groupBlock').length;
            if (groupNumber <= 4) {
                $('.byTable .eStore_productContent .fullBlock .groupBlock').css('width', (typeBlockW / groupNumber) - 1);
            }
        }
        if ($(".typeBlock ul.fullBlock div.groupBlock select").length <= 4) {
            $('.byTable .eStore_productContent .scrollBG').css({ width: 0 }).html("");
        }
        else {
            $('.byTable .eStore_productContent .scrollBG').css({ width: 16 }).html("<span></span>");
        }
    }

    //windowsResize();

    $(".byTable .eStore_productContent .selectBlock input[type=checkbox]").each(function (i, n) {
        var checkboxType = $(this).prop("checked");
        //console.log(checkboxType);
        if (checkboxType) {
            $(this).parents('li').addClass('checked');
        } else {
            $(this).parents('li').removeClass('checked');
        }
    });


    //判斷checkbox
    var countChecked = function () {
        var checkboxType = $(this).prop("checked");
        //console.log(checkboxType);
        if (checkboxType) {
            $(this).parents('li').addClass('checked');
        } else {
            $(this).parents('li').removeClass('checked');
        }
    };
    $('.byTable .eStore_productContent .selectBlock input[type=checkbox]').on("click", countChecked);

    var checkboxChecked = function () {
        var checkboxType = $(this).prop("checked");
        //console.log(checkboxType);
        if (checkboxType) {
            $(this).parents('tr').addClass('checked');
        } else {
            $(this).parents('tr').removeClass('checked');
        }
    };
    $('.byTable_mobile .checkboxBlock input[type=checkbox]').on("click", checkboxChecked);


    $(".eStore_slideToggle").unbind("click").click(function () {
        $(this).toggleClass("on").siblings().slideToggle("fast");
    });
    noneClickClass();
}

function fixMatrixNumber() {
    $('.byTable .eStore_productContent .fullBlock').animate({
        left: leftNumber
    }, 0, function () { });
}
function checkBrowser() {
    var mobileShow = $(".eStore_mobile").is(':visible');
    if (mobileShow)
        viewmodel.styleModel("MOBILE");
    else
        viewmodel.styleModel("PC");
}
function loadAdvertisement(id) {
    $.getJSON(GetStoreLocation() + "proc/do.aspx?func=7&CategoryID=" + id,
       function (data) {
           $.eStoreAD(data);
       });
}
function loadCategoryAttr(cateid) {
    var xhr = $.getJSON(GetStoreLocation() + 'api/Category/Get/' + cateid, { withMatrx: true, filterType: "" },
        function (options) {
            if (options.Specs != null) {
                var $ul = $("<ol />").addClass("eStore_category_link_linkBlock row20 eStore_category_moblieBlock");
                $.each(options.Specs, function (ci, cn) {
                    var categoryName = cn.Name.replace(" ", "");
                    var $cateli = $("<li />");
                    if (categoryName == "") {
                        $cateli.addClass("simpSpec");
                    }
                    var $cate = (categoryName == "" ? "" : $("<div />").addClass("dcate").append($("<a />").addClass("category_linkBlock haveList").append(cn.Name)));
                    var $cateul = $("<ul style='display: block;' />").addClass("eStore_category_link_linkListBlock");
                    $.each(cn.Values, function (ai, an) {
                        var $attrli = $("<li />");
                        var $attr = $("<div />").addClass("dcate").append($("<a />").addClass("category_linkBlock haveList").append(an.Name));
                        var $attrul = $("<ul style='display: block;' />").addClass("eStore_category_link_linkListBlock");
                        $.each(an.Values, function (vi, vn) {
                            if (vn.Name === "-")
                                return;
                            var $valli = $("<li />").attr({ "data-ref": cn.Id + "-" + an.Id + "-" + vn.Id });
                            var $checkbox = $("<input />").attr({ type: 'checkbox', value: vn.Id, "data-ref": cn.Id + "-" + an.Id + "-" + vn.Id });
                            $checkbox.on("change", function () {
                                _selectClickAttr = cn.Id + "-" + an.Id;
                                if ($(this).prop('checked')) {
                                    viewmodel.CateAttrFilter.push({ Id: _selectClickAttr + "-" + vn.Id, Value: { CateId: cn.Id, AttrId: an.Id, ValueId: vn.Id }, Name:vn.Name });
                                }
                                else {
                                    viewmodel.CateAttrFilter.remove(function (item) { return item.Id == (_selectClickAttr + "-" + vn.Id) });
                                }
                                ReLoadProducts();
                                viewmodel.showAttrSelectTop();
                            });
                            var $lable = $("<a />").addClass("category_linkBlock").append($("<label />").append($checkbox).append(vn.Name));
                            $valli.append($lable);
                            $attrul.append($valli);
                        });
                        $attrli.append($attr).append($attrul);
                        $cateul.append($attrli);
                    });
                    $cateli.append($cate).append($cateul);
                    $ul.append($cateli);
                });
                $("#cateAttr-filter").append($ul);
                ChangeCategoryLayt("#cateAttr-filter ");
            }
        });
}

// category click
function ChangeCategoryLayt(context){
    $("" + context +".eStore_category_link_linkBlock  .dcate").on("click", function () {
        var $item = $(this);
        var $parent = $(this).parents(".dcate");
        var hason = $item.hasClass("on");
        $("" + context +".eStore_category_link_linkBlock  .dcate").removeClass("on");
        if (!hason)
            $item.toggleClass("on");
        if ($item.next("ul") != null) {
            $item.next("ul").slideToggle("fast");;
        }
    });
}
// current aeu only
function addReadMore() {
    $(".eStore_listPoint").each(function (i, listPoint) {
        var $listPoint = $(listPoint);
        var $lis = $listPoint.find("li");
        if ($lis.length > 3) {
            $lis.slice(3, $lis.length).hide();
            var $moreDt = $("<dt />").addClass("read_more").html("Read More");
            $moreDt.click(function () {
                if ($lis.last().is(":hidden")) {
                    $lis.show();
                    $moreDt.html("Back");
                } else {
                    $lis.slice(3, $lis.length).hide();
                    $moreDt.html("Read More");
                }
            });
            $listPoint.append($moreDt);
        }
    })
}