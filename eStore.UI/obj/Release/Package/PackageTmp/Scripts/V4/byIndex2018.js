$(function () {
    if ($(window).width() > 767) {
        $(".eStore_menuLink_linkList_block").each(function () {
            var $this = $(this),
                $thisLink = $this.find("a"),
                $thisList = $this.find("ol");
            if ($thisList.length > 0) {
                $thisLink.find("i").show();
            }
        })
        
        $(".eStore_menuLink_link").mouseenter(function () {
            $(this).find(".eStore_menuLink_linkList").show();
            $(this).addClass("onFocus");
        });
        $(".eStore_menuLink_link").mouseleave(function () {
            $(this).find(".eStore_menuLink_linkList").hide();
            $(this).removeClass("onFocus");
        });
        $(".eStore_menuLink_linkList_block").mouseenter(function () {
            $(this).find("ol").show();
            $(this).addClass("onFocus");
        })
        $(".eStore_menuLink_linkList_block").mouseleave(function () {
            $(this).find("ol").hide();
            $(this).removeClass("onFocus");
        })
    } else if ($(window).width() <= 767) {
        $(".eStore_menuLink_link").click(function () {
            $(this).find(".eStore_menuLink_linkList").toggle();
            $(this).toggleClass("show");
        })
    }

    var navOpen = false;
    $(".eStore_mobile .eStore_seeMore").click(function () {
        if (navOpen == false) {
            $(".eStore_wrapper,.eStore_footer").stop().animate({ left: -251 }, 500);
            $(".eStore_wrapper").css("overflow", "visible");
            hideMenuH();
            navOpen = true;
        } else {
            $(".eStore_wrapper,.eStore_footer").stop().animate({ left: 0 }, 500, function () {
                $(".eStore_wrapper").css("overflow", "hidden");
            });
            hideMenuH();
            navOpen = false;
        }
        $(".eStore_mobileBox .eStore_menuLink_searchBlock_result").removeClass("block");
        $(".eStore_mobile .eStore_search").removeClass("show");
        $(".eStore_searchBox").slideUp();
    })

    function hideMenuH() {
        $(".eStore_headerBottom,.eStore_wrapper").css({
            "min-height": "0"
        });
        var n = $(".eStore_headerBottom").outerHeight(!0),
            t = $(".eStore_wrapper").height(),
            i = n > t ? n : t;
        $(".eStore_headerBottom,.eStore_wrapper").css({
            "min-height": i
        })
    }

    /* search  result */
    var searchResult = false;

    $(".eStore_mobile .eStore_search").click(function () {
        var $this = $(this);
        $(".eStore_wrapper,.eStore_footer").stop().animate({ left: 0 }, 500, function () {
            $(".eStore_wrapper").css("overflow", "hidden");
        });
        hideMenuH();
        navOpen = false;
        if (searchResult == false) {
            $this.addClass("show");
            $(".eStore_searchBox").slideDown();
            searchResult = true;
        } else {
            $this.removeClass("show");
            $(".eStore_searchBox").slideUp();
            searchResult = false;
            $(".eStore_mobileBox .eStore_menuLink_searchBlock_result").hide();
        }
        $(".eStore_menuLink_searchBlock .eStore_menuLink_searchBlock_result").hide();
    })

    $(".eStore_menuLink_searchBlock .storekeyworddispay").keyup(function () {
        $(".eStore_menuLink_searchBlock .eStore_menuLink_searchBlock_result").show();
        $(".eStore_menuLink_searchBlock .close").show();
    })
    $(".eStore_searchBox .storekeyworddispay").keyup(function () {
        $(".eStore_mobileBox .eStore_menuLink_searchBlock_result").show();
        $(".eStore_mobileBox .close").show();
    })
    $(".searchBlock_result_product ul li").each(function () {
        var $this = $(this),
            $hover_link = $this.find(".searchResult_product_sub"),
            $show_products = $this.find(".searchResult_product_content");
        $this.mouseenter(function () {
            $hover_link.addClass("hover");
            $show_products.addClass("show");
        })
        $this.mouseleave(function () {
            $hover_link.removeClass("hover");
            $show_products.removeClass("show");
        })

    })

    $(".storekeyworddispay").focusout(function () {
        $(".eStore_menuLink_searchBlock_result").fadeOut(500);
    })

    /*  result close & reset */
    $(".eStore_menuLink_searchBlock .close").click(function () {
        $(".eStore_menuLink_searchBlock .eStore_menuLink_searchBlock_result").hide();
        $(".eStore_menuLink_searchBlock .storekeyworddispay").val("");
        $(this).hide();
    })
    $(".eStore_mobileBox .close").click(function () {
        $(".eStore_mobileBox .eStore_menuLink_searchBlock_result").hide();
        $(".eStore_mobileBox .storekeyworddispay").val("");
        $(this).hide();
    })
})

function menuPositionShow() {

    if ($("body").hasClass("noRWD") & windowW < 980) {
        return;
    }

    menuW = $(".eStore_menuLink").outerWidth(true);
    var menuPosition = (windowW - menuW) / 2;
    $(".eStore_menuLink_linkList").css({
        "left": -menuPosition,
        "padding-left": menuPosition,
        "padding-right": menuPosition,
        "display": " "
    });
}//menuBG計算

/*left accordion*/
$(function () {

    $(".eStore_accordion").find("dt").click(function () {
        var $this = $(this);
        $(".eStore_index_Highlight").css("height", "auto");
        $(".eStore_accordion dt").removeClass("open");
        $(".eStore_accordion li .eStore_index_proContent").hide()
        if ($(window).width() > 768) {
            if ($("#Highlight_Summery").is(":hidden")) {
                $("#Highlight_Summery").show();
            }
        }

        $(".eStore_accordion").find("dd").slideUp(200);
        if ($this.next().is(":hidden")) {
            $this.addClass("open");
            $this.next().slideDown(200);
        }
        /*else if ($this.next().is(":visible")){
          //如果內容展開則什麼都不做
        }*/

        return false;
    })

    $(".eStore_accordion").find("dt").hover(function () {
        $(this).addClass("hover");
    }, function () {
        $(this).removeClass("hover");
    })

    $(".eStore_accordion li > a[ref]").click(function () {
        var $this = $(this);
        $("#Highlight_Summery").hide();
        if ($(window).width() > 768) {
            $(".eStore_accordion li > a").removeClass("open");
            $(".eStore_accordion li").find(".eStore_index_proContent").hide().removeClass("showContent");
            $this.addClass("open");
            $this.next().show().addClass("showContent");
        } else { // for mobile
            $this.parent().siblings().find("a").removeClass("open");
            $this.parent().siblings().find(".eStore_index_proContent").hide().removeClass("showContent");
            if ($this.next().is(":hidden")) {
                $this.addClass("open");
                $this.next().show().addClass("showContent");
            } else if ($this.next().is(":visible")) {
                $this.removeClass("open");
                $this.next().hide().removeClass("showContent");
            }
        }
        mobile = $(".eStore_mobileTopBlock").is(':visible');
        if ($this.has("on"))//load products here
        {
            var $proContainer = $this.next("div.eStore_index_proContent");
            if ($.trim($proContainer.html()) != "") {
                tabHeight();
            } else {
                $(".eStore_index_Highlight_List div.eStore_index_proContent").hide();
                $proContainer.show();
                var id = $this.attr("ref");
                if ($proContainer.prop("loaded") == undefined) {
                    var categoryname = $this.find("span.L1-text").html();
                    $proContainer.html($("<img />").prop("src", "/images/ajax-loader.gif"));
                    $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/' + id,
                        function (data) {
                            var temp = $("<div>");
                            // apply "template" binding to div with specified data
                            var name = "_tmpproducts";

                            ko.applyBindingsToNode(temp[0], { template: { name: name, data: data.TodaysHighlights } });
                            // save inner html of temporary div
                            var html = "<h5>" + categoryname + "</h5>" + temp.html();

                            if (data.BtnMore !== null) {
                                html += '<div class="eStore_more_products"><a href="' + data.Url + '" class="eStore_btn_gray">' + data.BtnMore + '</a></div>';
                            }

                            $proContainer.html(html);
                            //fixTableLayout(".eStore_index_proContent:visible", ".eStore_productBlock");
                            tabHeight();
                            bindProductReadMore();
                        });
                }
            }
            var url = document.location.host;

            var state = ({ url: document.location.href });

            window.history.pushState(state, "CategoryPath", "Default.aspx#Categoryid=" + $this.attr("data-cid") + "&ParentCategoryid=" + $this.attr("data-pid") + "");
        }
        return false;
    })
    // fix
    // $(".eStore_accordion li > a").hover(function(){
    //   $(this).addClass("hover");
    // },function(){
    //   $(this).removeClass("hover");
    // })

    HomeModel = new HomeModel();
    ko.applyBindings(HomeModel);
    $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/',
        function (data) {
            var temp = $("<div>");
            // apply "template" binding to div with specified data
            ko.applyBindingsToNode(temp[0], { template: { name: "_tmpproducts", data: data.TodaysHighlights } });
            // save inner html of temporary div
            var html = temp.html();
            $("#Highlight_Summery .eStore_index_HighlightContainer").html(html);

            var tempmodel = $("<div>");
            ko.applyBindingsToNode(tempmodel[0], { template: { name: "_tmpMobileproducts", data: data.TodaysHighlights } });
            // save inner html of temporary div
            var htmlmobile = tempmodel.html();
            $("#highlineMobile").html(htmlmobile);
            $("#eStore_mobileTop").each(function () {
                var id = $(this).attr('id');
                id = "#" + id;
                $(id).find("ul").carouFredSel({
                    auto: false,
                    prev: id + ' .prev',
                    next: id + ' .next',
                    pagination: id + ' .pager',
                    scroll: 1000
                });
            });
        });

    function tabHeight() {
        var indexContentH = $(".eStore_index_proContent.showContent").outerHeight();
        var categoryH = $(".eStore_index_Highlight_Block").outerHeight();
        var maxH = (indexContentH > categoryH) ? indexContentH : categoryH;
        $(".eStore_index_Highlight").css("height", maxH);
    };

    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var s;
    (s = ua.match(/edge\/([\d.]+)/)) ? Sys.edge = s[1] :
    (s = ua.match(/rv:([\d.]+)\) like gecko/)) ? Sys.ie = s[1] :
    (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
    (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
    (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
    (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
    (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;

    if ((Sys.safari) || (Sys.opera) || (Sys.firefox) || (navigator.userAgent.toLowerCase().indexOf('chrome') > -1)) {
        var url_string = document.location.href;
        url_string = url_string.replace("Default.aspx#", "Default.aspx?");
        var url = new URL(url_string);
        var Categoryid = "a#" + url.searchParams.get("Categoryid");
        var ParentCategoryid = "#" + url.searchParams.get("ParentCategoryid");
        var ctrlclass = ".ctrl" + url.searchParams.get("Categoryid");

        if ($(Categoryid).length > 0 && $(ParentCategoryid).length > 0) {
            $(Categoryid).get(0).click();
            $(ParentCategoryid).get(0).click();
            $("#Highlight_Summery").css("display", "none");
            $(ctrlclass).css("display", "block");
        }
    }
    else {
        Categoryid = getParameterByName("Categoryid");
        ParentCategoryid = getParameterByName("ParentCategoryid");
    }
});



var HomeModel = function () {
    var self = this;

    //self.languages = ko.observableArray([]);
    //self.stores = ko.observableArray([]);
    self.banners = ko.observableArray([]);
    self.highlights = ko.observableArray([]);

    var xhr;
    self.loadhighlights = function (id) {
        if (xhr && xhr.readystate != 4) {
            xhr.abort();
        }
        xhr = $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/' + id,
            function (data) {
                self.highlights(data.TodaysHighlights);
            }
        );
    }
};


function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    url = url.replace("Default.aspx#", "Default.aspx?");
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
};

// if product more then 9, will show read more button.
function bindProductReadMore() {
    var $more_btn = $(".eStore_btn_gray");
    if ($more_btn.length > 0) {
        $(window).bind('scroll', scroll_top).scroll();
        function scroll_top() {
            var $this = $(this),
                TT = $this.scrollTop();
            $more_btn_top = $more_btn.offset().top;
            $more_btn_show = $more_btn_top - $(window).height() + ($('.eStore_more_products').height() * 2);

            if (TT > $more_btn_show) {
                $more_btn.delay(500).animate({
                    opacity: 1
                }, 1000);

                $(window).unbind('scroll', scroll_top);

            }
        }
    }
}