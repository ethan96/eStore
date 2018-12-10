// JavaScript Document
var StepTop, StepBottom, StepBottom2, scrollH, StepH, startH, timeOut, listH, timeOut2, listFloatTopH;

function compareHeight() {
    StepH = $(".eStore_product_leftTable").outerHeight();
    listH = $(".eStore_system_listFloat").outerHeight(true);
    startH = (StepH < listH) ? listH : StepH;
    $(".positionFixed").animate({
        "height": startH
    }, 0);
    clearTimeout(timeOut, timeOut2);
}
function scrollingHeight() {

    if (StepTop - 10 < scrollH) {

        StepBottom2 = StepTop + (startH - listH);
        if (StepBottom2 > scrollH) {
            $(".eStore_system_listFloat").css("top", (scrollH - StepTop) + 10);
        } else {
            $(".eStore_system_listFloat").css("top", ((StepTop + (startH - listH)) - scrollH) + ((scrollH - StepTop) + 10));
        }
    } else {
        $(".eStore_system_listFloat").css("top", 0);
    }

}
function fixAdsHeight() {
    $("#storeSideAds img").one('load', function () {
        compareHeight();
    });
}

$(function () {

    $(".eStore_system_mobile .content .title").append("<div class='clearfix'></div>");
    $(".eStore_breadcrumb a:last").css("background-image", "none");
    //    //resourcesBlock
    //    $(".resourcesBlock span").append("|");
    //    $(".resourcesBlock span a").css("margin-right", 8);
    $(".eStore_chatBox").data("invitemeagain", true);
    var id = $(".eStore_product_productID input").val();
    var catContainer = $("#mostcategory");
    $.getJSON(GetStoreLocation() + 'api/category/AssociatedProductCategories/' + id,
    function (data) {
        if (data != null) {
            var temp = $("<div>");
            // apply "template" binding to div with specified data
            var name = "_tmpcategories";

            ko.applyBindingsToNode(temp[0], { template: { name: name, data: data} });
            // save inner html of temporary div
            $(temp).find("h4").text($.eStoreLocalizaion("Check_More_Information_for_Your_Project"));
            var html = temp.html();
            $(catContainer).html(html);
        }
        else
            $(catContainer).html("");
    });

    $(window).resize(function () {

        //resourcesBlock
        $(".eStore_resourcesList span .txt").remove();
        if ($(".eStore_resourcesTop").is(':hidden')) {
            $(".eStore_resourcesList span").append("<span class='txt'>|</span>");
        }

        var content = $(".eStore_container").outerWidth();
        var contentW = content - 226;
        if (content < 750 & content > 463) {
            $(".eStore_product_leftTable").css({
                "width": "100%"
            });
            $(".eStore_product_productDetail").css({
                "width": contentW * 0.90,
                "margin": "0 0 0 3%"
            });
            $(".eStore_product_productAction").css({
                "width": content * 0.98
            });
        } else if (content < 980 & content > 749) {
            $(".eStore_product_productDetail").css({
                "width": contentW * 0.5,
                "margin": "0 5% 0 3.5%"
            });
            $(".eStore_product_productAction").css({
                "width": contentW * 0.35
            });
            $(".eStore_product_leftTable").css({
                "width": contentW * 0.535 + 226,
                "margin": "0 5% 0 0"
            });
            $(".eStore_system_listFloat").css({
                "width": contentW * 0.35
            });
        } else {
            $(".eStore_product_productDetail,.eStore_product_productAction,.eStore_product_leftTable,.eStore_system_listFloat").css({
                "width": "",
                "margin": ""
            });
        }

        compareHeight();

    }).resize();

    compareHeight();

    StepTop = $(".eStore_product_leftTable").offset().top;
    StepTop = StepTop - 84;
    //***scrolling

    $(window).bind('scroll load', function () {

        scrollH = $(this).scrollTop();
        scrollingHeight();

    });

    $(".eStore_openBox").click(function () {
        timeOut = setTimeout(function () {
            compareHeight();
        }, 300);
        timeOut2 = setTimeout(function () {
            scrollingHeight();
        }, 400);

    });

    $(".eStore_resourcesTop").click(function () {
        $(this).toggleClass("on");
        if ($(".eStore_resourcesTop").hasClass("on")) {
            $(".eStore_resourcesList").show();
        } else {
            $(".eStore_resourcesList").hide();
        }
    });

    $("#eStore_ResourcesTab").after("<div class='clearfix'></div>");
    $("#eStore_ResourcesTab li").click(function () {
        $(this).addClass("on").siblings("li").removeClass("on");
        showResource();
    });
    showResource();
    function showResource() {
        $(".eStore_titleTab li[labfor]").each(function (i, n) {
            if ($(n).hasClass("on")) {
                $("#RTab-" + $(n).attr("labfor")).show();
            }
            else {
                $("#RTab-" + $(n).attr("labfor")).hide();
            }
        });
    }
    $(".carouselBannerSingle").each(function () {
        var id = $(this).attr('id');
        id = "#" + id;
        //console.log(id);
        $(id).find("ul").carouFredSel({
            auto: false,
            scroll: 1,
            prev: id + ' #prev',
            next: id + ' #next',
            pagination: id + ' #pager'
        });
    });
});

function showProductIDK() {
    $(".eStore_openBoxTable").click(function () {
        $(this).toggleClass("openBox").next(".eStore_openBox_select").toggle("fast");
        $(this).next(".eStore_openBox_select").next(".eStore_orderSystemList_borderTop").toggle("fast");
        timeOut = setTimeout(function () {
            compareHeight();
        }, 300);
        timeOut2 = setTimeout(function () {
            scrollingHeight();
            
        }, 400);
    });
}

$(function() {
    $(".eStore_product_leftTable").NoEnterSubmit();
});