// JavaScript Document
var mobile;
$(function () {
    mobile = $(".eStore_mobile").is(':visible');
    if (mobile)
    { downloadmobilecontent(); }
    else
    { downloadcontent(); }
});
$(window).resize(function () {
    mobile = $(".eStore_mobile").is(':visible');
    if (mobile)
    { downloadmobilecontent(); }
    else
    { downloadcontent(); }
});
var xhr;
function downloadcontent() {
    $(".eStore_compare_content").show();
    $(".eStore_compare_content_Mobile").hide();
    if ($(".eStore_compare_content .productlist").data("loaded") == "False") {
        jQuery.ajaxSettings.traditional = true;
        xhr = $.get(GetStoreLocation() + 'api/Comparision/getComparisionContent', { pns: comparisionproducts },
            function (data) {
                $(".eStore_compare_content .productlist").html(data).data("loaded", "True");
                init();
            }
        );
    }
    else if ($(".eStore_compare_content .productlist").data("init") == "False")
    {
        init();

    }

}
function downloadmobilecontent() {
    $(".eStore_compare_content").hide();
    $(".eStore_compare_content_Mobile").show();
    if ($(".eStore_compare_content_Mobile .productlist").data("loaded") == "False") {
        jQuery.ajaxSettings.traditional = true;
        xhr = $.get(GetStoreLocation() + 'api/Comparision/getMobileComparisionContent', { pns: comparisionproducts },
            function (data) {
                $(".eStore_compare_content_Mobile .productlist").html(data).data("loaded", "True");
                initmobile();

            }
        );
    }
    else if ($(".eStore_compare_content_Mobile .productlist").data("init") == "False") {
        initmobile();
    }

}
function fixtablewidth() {
    var itemscount = $("#comparingProducts ul li").length;
    if (itemscount < 4) {
        $(".eStore_compare_contentBottom").width(180 + 200 * itemscount);
        $(".eStore_compare_right").width(200 * itemscount);
        $(".caroufredsel_wrapper").width(200 * itemscount);
    }
}

function initmobile() {
    $(".eStore_slideToggle").click(function () {
        $(this).toggleClass("on").siblings().slideToggle("fast");
    });
    $(".eStore_compare_content_Mobile .close").click(function () {
        var deleteditmeIndex = $(this).closest("li").index();
        $(this).closest("li").remove();
      
        $(".eStore_compare_content_Mobile .eStore_compareProduct_tableList table").each(function (i, n) {
            $(this).find("tr:eq("+deleteditmeIndex+")").remove();
        });
        return false;
    });
    if ($("#eStore_LogIn_input").length > 0) {
        $(".needlogin").click(function () {

            $(".eStore_MyAccount").click(); return false;
        });
    }
    $(".eStore_compare_content_Mobile .productlist").data("init","True");
}
function init() {
    $(".eStore_compare_contentCategory").append("<div class='clearfix'></div>");

    //** compare height**
    var nameLeft = $(".eStore_compare_contentTop .eStore_compare_left .eStore_productBlock_name").height();
    $(".eStore_compare_contentTop .eStore_compare_right .eStore_productBlock_name").each(function () {
        var nameRight = $(this).height();
        nameLeft = (nameLeft > nameRight) ? nameLeft : nameRight;
    });
    $(".eStore_compare_contentTop .eStore_productBlock_name").css("height", nameLeft);

    var actionLeft = $(".eStore_compare_contentTop .eStore_compare_left .eStore_productBlock_action").height();
    $(".eStore_compare_contentTop .eStore_compare_right .eStore_productBlock_action").each(function () {
        var actionRight = $(this).height();
        actionLeft = (actionLeft > actionRight) ? actionLeft : actionRight;
    });
    $(".eStore_compare_contentTop .eStore_productBlock_action").css("height", actionLeft);

    var linkLeft = $(".eStore_compare_contentTop .eStore_compare_left .eStore_productBlock_link").height();
    $(".eStore_compare_contentTop .eStore_compare_right .eStore_productBlock_link").each(function () {
        var linkRight = $(this).height();
        linkLeft = (linkLeft > linkRight) ? linkLeft : linkRight;
    });
    $(".eStore_compare_contentTop .eStore_productBlock_name").css("height", linkLeft);

    //下面變動表格的計算
    var leftNumber, liNumber, leftH = 0, rightH = 0, finalH = 0;
    leftNumber = $(".eStore_compare_contentCategory .eStore_compare_left").length;
    for (n = 0 ; n < leftNumber; n++) {
        $(".eStore_compare_contentCategory .eStore_compare_left").eq(n).each(function () {
            liNumber = $(this).find("li").length;

            for (m = 0 ; m < liNumber ; m++) {
                leftH = 0; rightH = 0; finalH = 0;
                leftH = $(this).find("li").eq(m).height();
                //console.log(n,m,leftH);
                $(".eStore_compare_contentCategory").eq(n).find(".eStore_compare_right ul").eq(m).find("li").each(function () {
                    rightH = $(this).height();
                    finalH = (finalH > rightH) ? finalH : rightH;
                    //console.log(n,m,leftH,rightH,finalH);
                }); //每一列計算
                $(".eStore_compare_contentCategory").eq(n).find(".eStore_compare_right ul").eq(m).find("li").each(function () {
                    $(this).css("height", finalH);
                });
                $(this).find("li").eq(m).css("height", finalH);
            }
        });	//eStore_compare_left

    }

    //-----------------------------------------------------------
  
    $(window).scroll(function () {
        var comparetabletop = ($(".eStore_compare_content .productlist").offset().top);
        //** compare scroll **
        var blockTop = comparetabletop + 170;//169是要隱藏的欄位高
        var scrollTop = $(this).scrollTop();
        var scrollLeft = $(this).scrollLeft();

        $(".eStore_compare_left").css("left", scrollLeft);

        if (scrollTop >= blockTop) {//-5是為了留白
            $(".eStore_compare_contentTop").addClass("positionFixed");
            $(".positionFixed.eStore_compare_contentTop").css({
                "top": (scrollTop - 170 - comparetabletop),
                //"margin-top":-169

            });//180=固定欄位的高度+padding5
            $(".eStore_compare_contentBottom").css({
                //"padding-top":169
            });
        } else {
            $(".eStore_compare_contentTop").removeClass("positionFixed");
            $(".eStore_compare_contentTop").css({
                "top": "",
                "margin-top": ""
            });
            $(".eStore_compare_contentBottom").css({
                "padding-top": ""
            });
        }
    });
    $("#comparingProducts").find("ul").carouFredSel({
        synchronise: ['.eStore_compare_contentBottom .eStore_compare_right > ul', true, true, 0],
        circular: true,
        infinite: false,

        direction: "left",
        auto: false,
        scroll: 1,
        prev: '#comparingProducts  #prev',
        next: '#comparingProducts  #next',
        pagination: '#pager'
    });
    $('.eStore_compare_contentBottom .eStore_compare_right  > ul').carouFredSel({
        circular: true,
        infinite: false,

        direction: "left",
        auto: false,
        scroll: 1
    });

    $(".eStore_compare_contentTop .close").click(function () {
        var deleteditmeIndex = $(this).closest("li").index();
        //$(this).closest("li").remove();
        $("#comparingProducts").find("ul").trigger("removeItem", deleteditmeIndex);
        $(".eStore_compare_contentCategory .eStore_compare_right ul").each(function (i, n) {
            $(this).trigger("removeItem", deleteditmeIndex);
        });
        //$("#pager a:last").remove();
        fixtablewidth();
        return false;
    });

    

    fixtablewidth();
    $('a.not-active').click(function () { return false; });
    //openBox
  
    if ($("#eStore_LogIn_input").length > 0) {
        $(".needlogin").click(function () {

            $(".eStore_MyAccount").click(); return false;
        });
    }

    $(".eStore_compare_content .productlist").data("init","True");
}