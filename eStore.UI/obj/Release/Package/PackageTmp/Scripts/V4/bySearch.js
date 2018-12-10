// JavaScript Document
var SearchModel = function () {
    var self = this;
    var xhr, xhrp;

    self.count = ko.observable(0);
    self.allcountNumber = ko.observable(1);
    self.selectedSource = ko.observable("");
    self.isProcessing = ko.observable(true);
    self.hasMatchedItems = ko.observable(true);
    self.keywords = ko.observable("");
    self.groups = ko.observableArray([]);
    self.items = ko.observableArray([]);
    self.categories = ko.observableArray([]);
    self.prevSize = ko.observable(1);
    self.nextSize = ko.observable(1);

    self.search = function (keywords) {
        if (xhr && xhr.readystate != 4) {
            xhr.abort();
        }
        self.isProcessing(true);
        xhr = $.getJSON(GetStoreLocation() + 'api/Search/Search/', { keywords: keywords },
            function (data) {

                if (data.exactItem != null && data.exactItem != null) {
                    window.document.location = data.exactItem
                }
                else {
                    self.isProcessing(false);
                    self.hasMatchedItems(data.Count > 0);
                    self.count(data.Count);
                    self.allcountNumber(data.Count);
                    self.keywords(data.Keywords);
                    self.items(data.items);
                    self.categories(data.categories);
                    self.groups(data.groups);
                    //$(".eStore_search_content_categoriesBlock,.eStore_search_content_productBlock").mark(data.SearchTerms);
                    LazyLoadImg();
                }
            }
        );
    }

    self.loadproducts = function (keywords, page, pagesize) {
        $(".eStore_search_content").mask("");
        if (xhrp && xhrp.readystate != 4) {
            xhrp.abort();
        }
        var sg = "";
        if (self.selectedGroup() != undefined && self.selectedGroup().length > 0)
            sg = self.selectedGroup().join(",");
        xhrp = $.getJSON(GetStoreLocation() + 'api/Search/PagedItems/', { keywords: keywords, page: page, pagesize: pagesize, selectedGroup: sg, selectedSource: self.selectedSource(), SortAsc: self.SortAsc() },
            function (data) {
                self.items(data);
                LazyLoadImg();
                $(".eStore_search_content").unmask();
            }
        );
    }

    self.changinggroup = function (item) {
        self.pageNumber(1);
        var temp = new Array(1);
        temp[0] = item.Id;
        self.selectedGroup(temp);
        self.selectedSource(item.Source);
        self.count(item.Count);
        self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
    };


    self.selectedGroup = ko.observableArray([]);
    self.selectedGroup.subscribe(function (item) {
        self.pageNumber(1);
        var count = 0;
        $.each(self.groups(), function (i, n) {
            if ($.inArray(n.Id, item) >= 0) {
                count += n.Count;
            }
        })
        if (count == 0) {
            self.count(self.allcountNumber());
        } else {
            self.count(count);
        }
        self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
    })
    
    self.SortAsc = ko.observable("Depth");
    self.sort = function (sortStr) {
        if (self.SortAsc() == sortStr)
            return false;
        self.SortAsc(sortStr);
        self.pageNumber(1);
        self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
    };

    self.comparison = ko.observableArray([]);

    self.compare = function () {
        if (self.comparison().length > 0) {

            var comparisonitems = [];
            ko.utils.arrayForEach(self.comparison(), function (item) {
                comparisonitems.push(item.Id);
            });
            window.document.location = GetStoreLocation() + "Compare.aspx?parts=" + comparisonitems.join(",");
        }
        return false;
    }


    //pagingation
    self.pageNumber = ko.observable(1);

    self.nbPerPage = ko.observable(9);
    self.totalPages = ko.computed(function () {
        var div = Math.floor(self.count() / self.nbPerPage());
        div += self.count() % self.nbPerPage() > 0 ? 1 : 0;
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
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }

    self.previous = function () {
        if (self.pageNumber() != 1) {
            self.pageNumber(self.pageNumber() - 1);
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }
    self.changenbPerPage = function (number) {
        if (self.nbPerPage() != number) {
            self.nbPerPage(number);
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        if (self.pageNumber() > self.totalPages()) {
            self.pageNumber(1);
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }

    self.changePage = function (number) {
        if (number == "...") {
            self.pageNumber(self.prevSize());
        }
        else if (number == "....") {
            self.pageNumber(self.nextSize());
        }
        else {
            self.pageNumber(number);
        }
        self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        return false;
    }

    // product pages.
    self.Pages = ko.computed(function () {
        var _pages = ko.observableArray([]);
        var _size = 5;
        var _d = Math.floor(self.pageNumber() / _size);
        if (self.pageNumber() == _size * _d) {
            _d = _d - 1;
        }

        if (self.pageNumber() > 5) {
            _pages.push("...");
            self.prevSize(_d * _size);
        }

        var _md = ((_d + 1) * _size) > self.totalPages() ? self.totalPages() : (_d + 1) * _size;

        for (var i = _d * _size + 1; i <= _md; i++) {
            _pages.push(i);
        }

        if (_md < self.totalPages()) {
            _pages.push("....");
            self.nextSize(_md + 1);
        }

        return _pages;
    })

    self.firstPage = function () {
        if (self.pageNumber() != 1) {
            self.pageNumber(1);
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }
    self.lastPage = function () {
        if (self.pageNumber() < self.totalPages()) {
            self.pageNumber(self.totalPages());
            self.loadproducts(self.keywords(), self.pageNumber(), self.nbPerPage());
        }
        return false;
    }

    self.remmarktings = ko.observableArray([]);
    self.SeachMarketing = function (keywords) {
        $(".marketing_resources").mask("");
        var xhrp = $.getJSON(GetStoreLocation() + 'api/Search/SearchMarketings/', { keywords: keywords },
            function (data) {
                self.marketcount(data.Count);
                self.marketGroups(data.Groups);
                $(".marketing_resources").unmask();
                self.remmarktings(data.MarketingResources);
            }
        );
    }

    // remarketing
    self.pageMarketNumber = ko.observable(1);
    self.nbMarketPerPage = ko.observable(3);
    self.marketcount = ko.observable(20);
    self.prevMarkeSize = ko.observable(1);
    self.nextMarkeSize = ko.observable(1);
    self.marketGroups = ko.observableArray([]);
    self.marketCategory = ko.observable("");

    self.totalMarkePages = ko.computed(function () {
        var div = Math.floor(self.marketcount() / self.nbMarketPerPage());
        div += self.marketcount() % self.nbMarketPerPage() > 0 ? 1 : 0;
        return div;
    });
    self.hasMarketPrevious = ko.computed(function () {
        return self.pageMarketNumber() !== 1;
    });
    self.hasMarketNext = ko.computed(function () {
        return self.pageMarketNumber() !== self.totalMarkePages();
    });

    self.previousMarket = function () {
        if (self.pageMarketNumber() != 1) {
            self.pageMarketNumber(self.pageMarketNumber() - 1);
            self.loadMarkets(self.keywords(), self.pageMarketNumber());
        }
        return false;
    }
    self.nextMarket = function () {
        if (self.pageMarketNumber() < self.totalMarkePages()) {
            self.pageMarketNumber(self.pageMarketNumber() + 1);
            self.loadMarkets(self.keywords(), self.pageMarketNumber());
        }
        return false;
    }

    self.firstMarketPage = function () {
        if (self.pageMarketNumber() != 1) {
            self.pageMarketNumber(1);
            self.loadMarkets(self.keywords(), self.pageMarketNumber());
        }
        return false;
    }
    self.lastMarketPage = function () {
        if (self.pageMarketNumber() < self.totalMarkePages()) {
            self.pageMarketNumber(self.totalMarkePages());
            self.loadMarkets(self.keywords(), self.pageMarketNumber());
        }
        return false;
    }
    self.changeMarketPage = function (number) {
        if (number == "...") {
            self.pageMarketNumber(self.prevMarkeSize());
        }
        else if (number == "....") {
            self.pageMarketNumber(self.nextMarkeSize());
        }
        else {
            self.pageMarketNumber(number);
        }
        self.loadMarkets(self.keywords(), self.pageMarketNumber());
        return false;
    }

    self.MarketPages = ko.computed(function () {
        var _pages = ko.observableArray([]);
        var _size = 5;
        var _d = Math.floor(self.pageMarketNumber() / _size);
        if (self.pageMarketNumber() == _size * _d) {
            _d = _d - 1;
        }

        if (self.pageMarketNumber() > 5) {
            _pages.push("...");
            self.prevMarkeSize(_d * _size);
        }

        var _md = ((_d + 1) * _size) > self.totalMarkePages() ? self.totalMarkePages() : (_d + 1) * _size;

        for (var i = _d * _size + 1; i <= _md; i++) {
            _pages.push(i);
        }

        if (_md < self.totalMarkePages()) {
            _pages.push("....");
            self.nextMarkeSize(_md + 1);
        }

        return _pages;
    })
    self.changeMarket = function (item) {
        self.marketCategory(item.Key);
        self.marketcount(item.Count);
        self.loadMarkets(self.keywords(), self.pageMarketNumber());
    }
    self.loadMarkets = function (keywords, page) {
        $(".marketing_resources").mask("");
        var xhrp = $.getJSON(GetStoreLocation() + 'api/Search/PagedMarketItems/', { keywords: keywords, page: page, selectedGroup: self.marketCategory() },
            function (data) {
                $(".marketing_resources").unmask();
                self.remmarktings(data);
            }
        );
    }

}  

var groupsRenderedHandler = function (elements) {
    $(elements).find("a.search_linkBlock").click(function () {
        var hasClass = $(this).hasClass("on");
        if (hasClass) {
            $("a.search_linkBlock").removeClass("on");

        }
        else {
            $("a.search_linkBlock").removeClass("on");
            $(this).addClass("on");
        }
        return false;
    });
}
$(function () {
    SearchModel = new SearchModel();
    ko.applyBindings(SearchModel);
    SearchModel.search($(".storekeyworddispay:eq(1)").val());
    SearchModel.SeachMarketing($(".storekeyworddispay:eq(1)").val());
    $(".eStore_search_content_btnBlock,.eStore_search_content_listBlock,.eStore_search_moblieLink").append("<div class='clearfix'></div>");

    $(".eStore_search_moblieLink").click(function () {
        $(this).toggleClass("on");
        $(".eStore_search_link_linkBlock").slideToggle();
    });

    $(window).resize(function () {
        $(".eStore_search_moblieBlock,.eStore_search_link_linkListBlock").css({
            display: ""
        });
        $(".eStore_search_moblieLink span,.search_linkBlock").removeClass("on");
        $(".eStore_selectTitle").removeClass("openBox");
    });

    $(".product_sort").hover(function () {
        $(this).addClass("hover");
        $(".product_sort_list").show();
    }, function () {
        $(this).removeClass("hover");
        $(".product_sort_list").hide();
    })

    var sort_data;
    $(".product_sort_list li a").click(function () {
        sort_data = $(this).attr("data-text");
        // alert(sort_data);
        $(".product_sort_more_text").html(sort_data);
        $(".product_sort").removeClass("hover");
        $(".product_sort_list").hide();
        return false;
    })

    $(".eStore_search_refine .refine_section").each(function () {
        var $this = $(this),
            $this_title = $this.find(".refine_section_title .fa"),
            $this_title_link = $this.find(".refine_section_title a"),
            $this_content = $this.find(".refine_section_list"),
            $this_toggle = $this.find(".refine_toggle");

        $this_title.click(function () {
            $(this).parent().toggleClass("active");
            $this_content.slideToggle();
            return false;

        })
        $this_title_link.click(function () {
            if ($(window).width() <= 768) {
                $(this).parent().toggleClass("active");
                $this_content.slideToggle();
                return false;
            }
        })

    })

    $("#refine_title_1 .refine_section_title a").click(function () {
        if ($(window).width() > 768) {
            $('html,body').animate({ scrollTop: $(".eStore_search_content").offset().top }, 800, "easeInOutExpo");
            return false;
        }
    })
    $("#refine_title_2 .refine_section_title a").click(function () {
        if ($(window).width() > 768) {
            $('html,body').animate({ scrollTop: $("#marketing_resources").offset().top }, 800, "easeInOutExpo");
            return false;
        }
    })

    $(window).bind('scroll', scroll_top).scroll();
});

function scroll_top() {
    var $this = $(this),
        TT = $this.scrollTop(),
        $refine = $(".eStore_search_refine"),
        $refine_offset = $(".eStore_search_content").offset().top;

    if ($(window).width() > 768) {
        if (TT < $refine_offset) {
            $refine.removeClass("fixed");
        } else if (TT > $refine_offset) {
            $refine.addClass("fixed");
        }
    } else {
        $refine.removeClass("fixed");
    }
}