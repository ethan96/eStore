
var _isreadyload = true;
var Category = function (options) {
    options = options || {};

    this.Id = options.Id || "";
    this.Name = options.Name || "";
    this.Description = options.Description || "";
    this.Image = options.Image || "";
    this.BannerImage = options.BannerImage || "";
    this.Fetures = options.Fetures || "";
    this.ProductCount = options.ProductCount || 0;
    this.Breadcrumbs = options.Breadcrumbs || "";
    var factors = [];

    if (options.Factors != undefined && options.Factors != null) {
        ko.utils.arrayForEach(options.Factors, function (item) {
            factors.push(new Factor(item));
        });
    };
    this.Factors = ko.observableArray(factors);

    //add for matrix
    var children = [];
    this.Children = ko.observableArray(children);

    if (options.Children != undefined && options.Children != null) {
        ko.utils.arrayForEach(options.Children, function (item) {
            children.push(new Category(item));
        });
    };
    this.isTabCategory = options.isTabCategory || false;
    this.Products = options.Products || {};
    this.Specs = options.Specs || {};
    if (this.Specs != undefined && this.Specs != null) {
        ko.utils.arrayForEach(this.Specs, function (spec) {
            if (spec.Values != undefined && spec.Values != null) {
                ko.utils.arrayForEach(spec.Values, function (attr) {
                    attr.defaultOption = [];
                    attr.ParentId = spec.Id;
                });
            };
        });
    };
    this.isSelect = ko.observable(false);
    this.hasChild = ko.computed(function () {
        return children.length > 0;
    });
}
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
    this.FactorIds = ko.observableArray(options.FactorIds || []);
    this.Matrix = options.Matrix || [];
};


var Factor = function (options) {
    options = options || {};
    this.Id = options.Id || "";
    this.Name = options.Name || "";
    this.ParentId = options.ParentId || "";
    this.TemplateName = options.TemplateName || "";
    this.LazyloadChildren = options.LazyloadChildren || "";
    this.Sequence = options.Sequence || "";
    this.IsUsed = ko.observable(true);
    this.SubFactors = ko.observableArray([]);
    this.IsEndNode = ko.dependentObservable(function () {
        return (this.SubFactors == undefined
        || this.SubFactors == null
        || this.SubFactors.length == 0);
    }, this);

    this.UpdateIsUsed = function () {
        if (this.SubFactors().length > 0) {
            this.IsUsed(ko.utils.arrayFilter(this.SubFactors(), function (item) {
                item.UpdateIsUsed();
                return item.IsUsed;
            }).length > 0);
        }
    }
};


var CategoryModel = function () {
    var self = this;
    self.category = ko.observable(new Category(undefined));
    var xhr, xhrp;
    self.loadcategory = function (cate) {
        var id = (typeof cate == "object") ? cate.Id : cate;

        if (xhr && xhr.readystate != 4) {
            xhr.abort();
        }
        var cateid = self.parentPath() == "" ? id : self.parentPath();
        xhr = $.getJSON(GetStoreLocation() + 'api/Category/Get/' + cateid, { parentPath: self.parentPath() },
            function (data) {
                self.clearMatrixFilter();
                self.category(new Category(data)); // promotion and delivery category will not change
                if (self.category().Breadcrumbs != "") {
                    $(".eStore_breadcrumb").html(self.category().Breadcrumbs);
                    $(".eStore_breadcrumb a:last").css("background-image", "none");
                }
                if (_isreadyload) {
                    _isreadyload = false;
                    if (self.category().isTabCategory) {
                        self.detailUseMatrix(false);
                        $(".eStore_style_byTable").click();
                        return;
                    }
                    else
                        $("#category-list").show();
                    checkBrowser();
                }
                if (self.category().isTabCategory && !self.category().hasChild() && self.detailUseMatrix()) {
                    self.detailUseMatrix(false);
                    if ($(".eStore_category_content").isMasked()) {
                        $(".eStore_category_content").unmask("");
                    }
                    $(".eStore_style_byTable").click();
                    return;
                }
                self.detailUseMatrix(true);
                self.productCategoryId(id);
                if (!(self.nbPerPage() == 9 || self.nbPerPage() == 18))
                    self.changenbPerPage(9);
                self.pageNumber(1);
                if (self.parentPath() != "") {
                    $.getJSON(GetStoreLocation() + 'api/Category/Get/' + id, { parentPath: self.parentPath() },
                        function (dt) {
                            self.productCount(dt.ProductCount || 0);
                        });
                }
                else
                    self.productCount(data.ProductCount || 0);
                if (typeof cate == "object") {
                    self.selectItem(self.category());
                    self.selectItemFilterArr(self.category().Specs);
                }
                self.loadproducts(cate, self.pageNumber(), self.nbPerPage());
                loadAdvertisement(cateid);
            });
        $("#moreCategoryInfor").ShowMoreCategory(id);
    }
    self.Products = ko.observableArray([]);
    self.loadproducts = function (cate, page, pagesize) {
        var id = (typeof cate == "object") ? cate.Id : cate;
        if (!$(".eStore_category_content").isMasked()) {
            $(".eStore_category_content").mask("");
        }
        if (xhrp && xhrp.readystate != 4) {
            xhrp.abort();
        }
        var sort = self.isSortAsc();
        if (self.isSortAsc() == undefined)
            sort = null;
        xhrp = $.getJSON(GetStoreLocation() + 'api/Category/Products/' + id, { page: page, pagesize: pagesize, isSortAsc: sort, parentPath: self.parentPath(), _type: self._type() },
            function (data) {
                var products = [];
                if (data != undefined && data != null) {
                    ko.utils.arrayForEach(data, function (item) {
                        products.push(new Product(item));
                    });
                }
                self.Products(products);
                self.mappingProducts(products);

                fixTableLayout("#category-list .eStore_category_content_productBlock.byPhoto", ".eStore_productBlock");
                if (typeof cate == "object" || self._type) {
                    loadTabInfor();
                }
                $(".eStore_category_content").unmask();
            });
    }
    self.isSortAsc = ko.observable(undefined);
    var sortstr = GetQueryString("sort") || GetQueryString("Sort");
    if (sortstr != null) {
        self.isSortAsc(sortstr === "true");
    }
    self.sort = function (isSortAsc) {
        if (self.isSortAsc() == isSortAsc)
            return false;
        self.isSortAsc(isSortAsc);
        self.pageNumber(1);
        self.loadproducts(self.productCategoryId(), self.pageNumber(), self.nbPerPage());
    };
    self.factors = ko.computed(function () {
        var factorstree = ko.utils.arrayFilter(self.category().Factors(), function (item) {
            return (item.ParentId == null || item.ParentId == "");
        });


        ko.utils.arrayForEach(factorstree, function (ft) {
            var factorAttributes = ko.utils.arrayFilter(self.category().Factors(), function (fti) {
                var ftid = ft.Id;
                return fti.ParentId == ftid;
            });
            ko.utils.arrayForEach(factorAttributes, function (fa) {
                var factorAttributeValues = ko.utils.arrayFilter(self.category().Factors(), function (fai) {
                    var faid = fa.Id;
                    return fai.ParentId == faid;
                });
                fa.SubFactors(factorAttributeValues);

            });
            ft.SubFactors(factorAttributes);
        });
        var unmapped = ko.mapping.toJS(factorstree);


        return factorstree;
    });

    self.comparison = ko.observableArray([]);
    self.addComparison = ko.pureComputed({
        read: function () {
            return new Product();
        },
        write: function (value) {
            cc = value;
        },
        owner: this
    });

    self.compare = function () {
        if (self.comparison().length > 0) {
            window.document.location = GetStoreLocation() + "Compare.aspx?parts=" + self.comparison().join(",");
        }
        return false;
    }


    //pagingation
    self.parentPath = ko.observable("");
    self.pageNumber = ko.observable(1);
    self.nbPerPage = ko.observable(9);
    self.productCount = ko.observable(0);
    self.productCategoryId = ko.observable("");
    self.detailUseMatrix = ko.observable(true);
    self.totalPages = ko.computed(function () {
        var div = Math.floor(self.productCount() / self.nbPerPage());
        div += self.productCount() % self.nbPerPage() > 0 ? 1 : 0;
        return div;
    });

    self.hasPrevious = ko.computed(function () {
        return self.pageNumber() !== 1;
    });
    self.hasNext = ko.computed(function () {
        return self.pageNumber() !== self.totalPages();
    });
    self.next = function () {
        if (self.pageNumber() < self.totalPages()) {
            self.pageNumber(self.pageNumber() + 1);
            self.loadproducts(self.productCategoryId(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }

    self.previous = function () {
        if (self.pageNumber() != 1) {
            self.pageNumber(self.pageNumber() - 1);
            self.loadproducts(self.productCategoryId(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }
    self.changenbPerPage = function (number) {
        if (self.nbPerPage() != number) {
            self.nbPerPage(number);
            self.loadproducts(self.productCategoryId(), self.pageNumber(), self.nbPerPage());
        }
        if (self.pageNumber() > self.totalPages()) {
            self.pageNumber(1);
            self.loadproducts(self.productCategoryId(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }
    self.showall = function () {
        self.changenbPerPage(self.productCount());
        return false;
    }

    self.loadPerCategory = function (rootId, caid) {
        $.getJSON(GetStoreLocation() + 'api/Category/Get/' + rootId, { parentPath: self.parentPath() },
            function (data) {
                self.rootCategory(new Category(data));
                self.loadcategory(caid);
            });
    }

    self.rootCategoryId = ko.pureComputed({
        read: function () { },
        write: function (value) {
            $.getJSON(GetStoreLocation() + 'api/Category/Get/' + value, { parentPath: self.parentPath() },
            function (data) {
                self.rootCategory(new Category(data));
            });
        },
        owner: this
    });
    self.rootCategory = ko.observable([]);
    self.selectItem = ko.observable([]);
    self.selectItemFilterArr = ko.observableArray([]);
    self.selectAttrs = ko.observableArray();
    self.mappingProducts = ko.observableArray([]);
    self.changeItem = function (item) {
        $.each(self.rootCategory().Children(), function (i, n) {
            n.isSelect(false)
        });
        item.isSelect(true);
        self.loadcategory(item);
    }

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
    self.changeAttr = function (obj, event) {
        var valueId = $(event.target).val();
        self.selectAttrs.remove(function (item) { return item.AttrId == obj.Id && item.CatId == obj.ParentId });
        if (valueId != -1) {
            self.selectAttrs.push({ CatId: obj.ParentId, AttrId: obj.Id, ValueId: valueId });
        }
        var tempAtt = [];
        $.extend(true, tempAtt, self.selectItem().Specs);
        if (self.selectAttrs().length > 0) {
            var tempPros = [];
            var maxLs = [];
            $.each(self.Products(), function (i, n) {
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

            if (obj.Id != -1) {
                $.each(tempAtt, function (i, n) {
                    $.each(n.Values, function (si, sn) {
                        if (sn.Id == obj.Id && obj.ParentId == n.Id) {
                            sn.defaultOption = ko.observable([parseInt(valueId)]);
                            //return;
                        }
                        sn.Values = $.map(sn.Values, function (value) {
                            if (value.Id == -1) { return value; }
                            $.each(self.selectAttrs(), function (sai, sa) {
                                if (sa.AttrId == sn.Id && sa.ValueId == value.Id && sa.CatId == n.Id) { sn.defaultOption = ko.observable([parseInt(sa.ValueId)]); return false; }
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
        self.selectItem([]);
        self.selectItemFilterArr([]);
        self.selectAttrs([]);
        self.mappingProducts([]);
    }
    self._type = ko.observable(false);
    self.selectMatrixTab = function () {
        self._type(true);
        if ($(".eStore_productLink a").length > 0) {
            if (self.category().Id == self.rootCategory().Id)
                $(".eStore_productLink a:first").click();
            else
                $(".eStore_productLink a[ref='" + self.category().Id + "']").click();
        }
        else
            self.loadcategory(self.category());
    }

    self.matrixTemplate = ko.observable("pcMatrix-template");
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
}
var factorsRenderedHandler = function (elements) {
    //** category spec filter **
    $(elements).find(".eStore_openBox").click(function () {
        $(this).toggleClass("openBox").siblings().slideToggle("fast");
    });
}
function showParentPanel(panel) {
    var parentpanel = $(panel).parent().parent();
    if ($(parentpanel).is("ul") && $(parentpanel).hasClass("eStore_category_link_linkListBlock")) {
        $(parentpanel).show();
        showParentPanel(parentpanel);
    }
}
function loadAdvertisement(id) {
    $.getJSON(GetStoreLocation() + "proc/do.aspx?func=7&CategoryID=" + id,
       function (data) {
           $.eStoreAD(data);
       });
}
// JavaScript Document
$(function () {
    CategoryModel = new CategoryModel();
    ko.applyBindings(CategoryModel);
    var categoryid = $("#hcategory").val();
    var hashId = window.location.hash;
    if (hashId == "")
        hashId = $("#hcselectedategory").val();
    else {
        hashId = hashId.substring(1);
    }
    if (hashId != "") {
        var subcategory = $(".eStore_category_link_linkBlock a[ref='" + hashId + "']");
        if (subcategory.length > 0) {
            categoryid = hashId;
            CategoryModel.parentPath($(subcategory).attr("parentPath") || "");
            $(subcategory).addClass("on");
            $(subcategory).siblings("ul.eStore_category_link_linkListBlock").show();
            showParentPanel(subcategory);
        }
    }
    $(".eStore_category_link_linkBlock > li >a.haveList").siblings("ul.eStore_category_link_linkListBlock").show();

    var pid = $("#category-list ol.eStore_category_link_linkBlock a[ref='" + categoryid + "']").attr("ppath");
    CategoryModel.loadPerCategory(pid || categoryid, categoryid);

    var menuLeave, menuLeave2, menuEnter, mouseStyle;
    $(".eStore_breadcrumb a:last").css("background-image", "none");
    //** category nav link **
    $("a.category_linkBlock").click(function () {
        var hasClass = $(this).hasClass("on");
        if (hasClass) {
            return false;
        };

        $("a.category_linkBlock").removeClass("on");
        $(this).addClass("on");
        if ($(this).hasClass("haveList")) {
            $(this).parent().parent().find(".eStore_category_link_linkListBlock").slideUp(0);
            //$(".eStore_category_link_linkListBlock").slideUp();
            $(this).siblings('ul').slideDown();
        }
        return false;
    });
    $(".eStore_category_link_linkBlock a[ref]").click(function () {
        if (!$(".eStore_category_content").isMasked()) {
            $(".eStore_category_content").mask("");
        }
        CategoryModel.parentPath($(this).attr("parentPath") || "");
        CategoryModel.loadPerCategory($(this).attr("ppath") || $(this).attr("ref"), $(this).attr("ref"));
        if ($.cookie('eStore_category_displayModel') == "byPhoto")
            $(".eStore_category_content_listBlock_style .eStore_style_byPhoto").click();
        else
            $(".eStore_category_content_listBlock_style .eStore_style_byList").click();
    });


    //switch display type
    $(".eStore_category_content_listBlock_style a").bind("click", function () { changblock($(this)); });

    function changblock(item) {
        if (item.hasClass("on"))
            return false;
        if (item.hasClass("eStore_style_byList") || item.hasClass("eStore_style_byTable")) {
            $(".eStore_category_content_productBlock").removeClass("byPhoto").addClass("byList");
            $.cookie('eStore_category_displayModel', "byList", { expires: 365, path: '/' });
        }
        else {
            $(".eStore_category_content_productBlock").removeClass("byList").addClass("byPhoto");
            $.cookie('eStore_category_displayModel', "byPhoto", { expires: 365, path: '/' });
        }
        item.addClass("on");
        item.siblings().removeClass("on");
        if (item.hasClass("eStore_style_byTable")) {
            $("#category-matrix").show();
            $("#category-list, span.eStore_category_content_listBlock_page, .eStore_category_content_listBlock_results, .eStore_category_link").hide();
            $("#pro-block").appendTo($("#matrix-block"));
        }
        else {
            $("#category-matrix").hide();
            $("#pro-block").appendTo($("#list-block"));
            $("#category-list, span.eStore_category_content_listBlock_page, .eStore_category_content_listBlock_results, .eStore_category_link").show();
            if (CategoryModel._type()) {
                CategoryModel._type(false);
                CategoryModel.detailUseMatrix(false);
                $(".eStore_category_link ol.eStore_category_link_linkBlock a[ref='" + CategoryModel.category().Id + "']").click();
            }
        }
        fixTableLayout("#category-list  .eStore_category_content_productBlock.byPhoto", ".eStore_productBlock");
        //$(".eStore_category_content_listBlock_style a").unbind("click").bind("click", function () { changblock($(this)); });
        return false;
    }

    if ($.cookie('eStore_category_displayModel') == "byPhoto") {
        $(".eStore_category_content_listBlock_style a.eStore_style_byPhoto").click();
    }
    $(".eStore_category_moblieLink span").click(function () {
        $(this).toggleClass("on").siblings("span").removeClass("on");

        if ($(this).hasClass("on")) {
            var thisClass = $(this).attr("data-class");
            $(thisClass).slideDown().siblings(".eStore_category_moblieBlock").slideUp();
        } else {
            $(".eStore_category_moblieBlock").slideUp();
        }
    });

    $(window).resize(function () {
        //$(".eStore_category_moblieBlock,.eStore_category_link_linkListBlock,.eStore_specfilter ul").css({
        //display: ""
        //});
        //$(".eStore_category_moblieLink span,.category_linkBlock").removeClass("on");
        $(".eStore_selectTitle").removeClass("openBox");
        fixTableLayout("#category-list  .eStore_category_content_productBlock.byPhoto", ".eStore_productBlock");

    });


    $(".eStore_system_mobile .content .title").append("<div class='clearfix'></div>");
    //    //resourcesBlock
    //    $(".resourcesBlock span").append("|");
    //    $(".resourcesBlock span a").css("margin-right", 8);


});

jQuery.fn.ShowMoreCategory = function (id) {
    var objtemp = $(this);
    $.getJSON(GetStoreLocation() + 'api/category/ProductCategories/' + id,
    function (data) {
        if (data != null) {
            var temp = $("<div>");
            // apply "template" binding to div with specified data
            var name = "_tmpcategories";

            ko.applyBindingsToNode(temp[0], { template: { name: name, data: data} });
            // save inner html of temporary div
            var html = temp.html();
            objtemp.html(html);
        }
        else
            objtemp.html("");
    });
}


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
        if (!_isreadyload) {
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
        left: leftNumber}, 0, function () {  });
}
function checkBrowser() {
    var mobileShow = $(".eStore_mobile").is(':visible');
    if (mobileShow)
        CategoryModel.styleModel("MOBILE");
    else
        CategoryModel.styleModel("PC");
}

// --- end tab category