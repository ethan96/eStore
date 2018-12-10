// JavaScript Document
var sizeL = 1560, sizeM = 980, sizeS = 768, sizeXS = 480; //RWD size
var windowW, windowH, headerH, footerH, scrollH; //variables
var footerPush = 30;

var fixedTopDist, fixedTopWidth, fixedBlockHeight = 0, fixedOnLastHeight; //fix to top
var windowWBefore, windowWAfter;

function clearbothFloat() {

    $('.clearboth-t').append('<div class="clear-float"></div>');

}/** 清除float ***/

function windowWidth() {

    $('#site-AEU').removeClass();

    //設定累加，css可以重複使用
    if (windowW > sizeL) {
        $('#site-AEU').addClass('screenL');
    }
    if (windowW < sizeM) {
        $('#site-AEU').addClass('screenM');
    }
    if (windowW <= sizeS) {
        $('#site-AEU').addClass('screenS');
    }
    if (windowW <= sizeXS) {
        $('#site-AEU').addClass('screenXS');
    }

}/** 計算螢幕寬度 **/

function footerPosition() {

    windowH = $(window).outerHeight(true);
    headerH = $('.site-header').outerHeight(true);
    footerH = $('.site-footer').outerHeight(true);
    $('.site-container').css({
        'margin-bottom': -footerH,
        'min-height': windowH - headerH
    });
    $('.site-container .site-container-push').remove(); //resize時 避免重複新增
    $('.site-container').append('<div class="site-container-push"></div>');
    var heightH = footerH + footerPush; //數字可調整
    $('.site-container-push').css('height', heightH);

}//** footer置底 自動抓取footer高度

var timer =  0;
function equalHeightBlock() {

    $(".col-height-equal .equal-block").css({
        'height': ''
    });

    windowWBefore = $(window).width();
    equalHeightCol();

    if (windowWBefore !== windowWAfter) {
        equalHeightCol();
    }
    if (timer < 3) {
        window.setTimeout(equalHeightBlock, 1000);
        timer = timer + 1;
    }
}// ** 同區域等高計算

function equalHeightCol() {

    //當欄寬不一樣
    $(".col-height-equal").each(function () {
        $(this).find(".buttom").removeAttr("style");
        //each 都要歸0
        var equalHeightBoxAmt, equalHeightPartentBoxW, box1W, box2W, boxTotalW = 0, boxHeight, BoxFinalH = 0, boxBF = 0, i = 0, j = 0, paddingIE7 = 0;

        equalHeightBoxAmt = $(this).children(".equal-block:visible").length; //計算同區塊內有幾欄
        equalHeightPartentBoxW = $(this).outerWidth();

        for (r = 0; r < equalHeightBoxAmt; r++) {
            box1W = parseInt(($(this).children(".equal-block:visible").eq(r).outerWidth(true)) - 1);// 由於JQ width(); 會自動四捨五入顧改成下面寫法
            //console.log(equalHeightBoxAmt,r,box1W,equalHeightPartentBoxW);	
            if ((r + 1) == equalHeightBoxAmt) {
                boxTotal = equalHeightPartentBoxW + 1;
            } else {
                var n = parseInt(r + 1);
                box2W = parseInt(($(this).children(".equal-block:visible").eq(n).outerWidth(true)) - 1);
                boxTotal = box1W + box2W + (boxBF);
            }
            //console.log('計算到第'+r+'個欄位'+ ' 第'+r+'個寬'+box1W,' 第'+(r+1)+'個寬'+box2W,' 加總='+ boxTotal,' 母總寬'+equalHeightPartentBoxW ,' boxBF='+ boxBF ,' 最高高度='+ height);
            if (boxTotal > equalHeightPartentBoxW) {
                for (i; i <= r; i++) {
                    var paddingTop = 0, paddingBottom = 0;
                    if ($('body').hasClass('ie7')) {
                        paddingTop = parseInt($(this).children(".equal-block:visible").eq(i).css('padding-top'));
                        paddingBottom = parseInt($(this).children(".equal-block:visible").eq(i).css('padding-bottom'));
                        paddingIE7 = paddingTop + paddingBottom;
                    }
                    boxHeight = ($(this).children(".equal-block:visible").eq(i).outerHeight()) - paddingIE7;
                    BoxFinalH = BoxFinalH > boxHeight ? BoxFinalH : boxHeight;
                }
                for (j; j <= r; j++) {
                    $(this).children(".equal-block:visible").eq(j).css('height', BoxFinalH);
                }
                boxBF = 0;
                BoxFinalH = 0;
            } else {
                boxBF = box1W + boxBF;

            }
            //console.log('計算到第'+r+'個欄位'+ ' 第'+r+'個寬'+box1W,' 第'+(r+1)+'個寬'+box2W,' 加總='+ boxTotal,' 母總寬'+equalHeightPartentBoxW ,' boxBF='+ boxBF );
        }//end for迴圈
        $(this).find(".buttom").css({ "position": "absolute","bottom":0});
    });

    windowWAfter = $(window).width();

}

var currRowIndex = 10;
function CheckReadMore() {
    var filter = ".readmore li.col:lt(" + currRowIndex + ")";
    $(filter).show();
    //console.log(currRowIndex);
    fixFullImg();
    if ($(".readmore li.col:hidden").length <= 0) {
        $(".readymore-btn").hide();
    }
}

var currTableIndex = 16;
function CheckReadMoreTable(isfilter) {
    $(".readymoreTable-btn").show();
    var filter;
    if (isfilter) {
        filter = ".readmoreTable li.col:visible:gt(" + (currTableIndex - 1) + ")";
        if ($(filter).length <= 0) {
            $(".readymoreTable-btn").hide();
        }
        $(filter).hide();
    }
    else {
        filter = ".readmoreTable li.col:lt(" + currTableIndex + ")";
        $(filter).show();
        if ($(".readmoreTable li.col:hidden").length <= 0) {
            $(".readymoreTable-btn").hide();
        }
    }
        
    //console.log(currRowIndex);
    equalHeightBlock();    
}

function fixFullImg() {
    $('.leftImg .fullimg').each(function () {
        var thisW = $(this).width();
        //console.log(thisW);
        $(this).parents('.leftImg').css('height', thisW * 0.614)
    });
}

$(function () {

    // 滿高
    //$(".col-height-equal .equal-block").wrapInner( "<div class='col-full-height'></div>" );

    //listIcon-number
    $('.list-number li').prepend("<span class='number'></span>");
    $('.list-number').each(function () {
        var listAmt = $(this).children('li').length;
        var numberSpace = 5;
        for (i = 0; i < listAmt; i++) {
            var number = i + 1;
            $(this).find('.number').eq(i).text(number + '.');
            var thisW = $(this).find('.number').eq(i).outerWidth(true);
            thisW = thisW + numberSpace;
            $(this).find('li').eq(i).css('margin-left', thisW);
            $(this).find('.number').eq(i).css({
                'margin-left': -thisW,
                'padding-right': numberSpace
            });
        }
    });

    //** inline-block元素中的4px空白 **
    $('.clear-inlinespace').contents().filter(function () {
        return this.nodeType === 3;
    }).remove();

    //** 清除float **/
    clearbothFloat();

    $('.listStyle-block .col').each(function () {
        var styleType = $(this).children('div').hasClass('leftImg');
        //console.log(styleType);
        if (!styleType) {
            $(this).addClass('onlyTxt');
        }
    });

    $('.OnlineResources-OnlineCatalogs .moreBlock .moreBtn').click(function () {
        var scrollTo = $(this).parents('.col').offset().top;
        $("html, body").animate({
            scrollTop: scrollTo
        }, 500);
        var thisH = $(this).parents('.rightContent').find('.txt').height();
        $(this).parents('.moreBlock').hide();
        $(this).parents('.rightContent').find('.content').animate({
            height: thisH
        }, 500);
        return false;
    });

    $('.OnlineResources-OnlineCatalogs .closeBtn').click(function () {
        var scrollTo = $(this).parents('.col').offset().top;
        $("html, body").animate({
            scrollTop: scrollTo
        }, 500);
        $(this).parents('.moreBlock').hide();
        $(this).parents('.rightContent').find('.content').animate({
            height: 78
        }, 500, function () {
            $(this).parents('.rightContent').find('.moreBlock').show();
        });
        return false;
    });

    $('.OnlineResources-OnlineCatalogs .col:nth-child(2n)').after('<div class="clear-float"></div>');
    

    $(window).resize(function () {

        //** 計算螢幕寬度 **
        windowW = $(window).width();

        //** 指定螢幕所屬類型 **
        windowWidth(windowW);

        if ($('body').hasClass('ie7')) {

            $('body .col').each(function () {
                $(this).css('width', '');
                var thisW = $(this).width();
                var paddingL = parseInt($(this).css('padding-left'));
                var paddingR = parseInt($(this).css('padding-right'));
                $(this).css({
                    width: thisW - paddingL - paddingR - 1
                });
            });
        }
        $('.OnlineResources-WhitePapers .col').each(function () {
            var thisW = $(this).width();
            var contentW = thisW - 100 - 1;
            $(this).children('.titleLink').css('width', contentW);
        });

        // ** 同區域等高計算 **
        equalHeightBlock();
        fixFullImg();
        
        
        //** footer置底(要計算container最低高度 放在最後) **
        //footerPosition();

    }).resize();

    
    //read more
    CheckReadMore();
    $(".readymore-btn").click(function () {
        currRowIndex = currRowIndex + 10;
        CheckReadMore(); 
    });

    CheckReadMoreTable();
    $(".readymoreTable-btn").click(function () {
        currTableIndex = currTableIndex + 16;
        filterApplist();
    });

    var $buttonAppClose = $('#site-AEU .filter-close,#site-AEU .cs-application'),
        $menuApp = $('#site-AEU .application-menu'),
        $AppFilter = [],
        visibleApp = true;
    $buttonAppClose.click(function (event) {
        event.stopPropagation();
        if (visibleApp) {
            $menuApp.removeClass('hide').addClass('show');
            visibleApp = !visibleApp;

        } else {
            $menuApp.addClass('hide').removeClass('show');
            visibleApp = !visibleApp;
        }
    });
    $('ul.menu-item a[data-type="app"]').on("click", function () {
        
        if ($.inArray($(this).attr("data-id"), $AppFilter) == -1)
        {
            $AppFilter.push($(this).attr("data-id"));
            $("#site-AEU .application-tag").append($("<span />").text($(this).text()).attr("data-id", $(this).attr("data-id")));
            $(this).addClass("selected");
            bindBaaFilterList();
            currTableIndex = 16;
            filterApplist();
        }
    });

    function bindBaaFilterList()
    {
        $("#site-AEU .application-tag span").on("click", function () {
            $(".application-menu .menu-item a[data-id='" + $(this).attr("data-id") + "']").removeClass("selected");
            $(this).remove();
            var itemno = $.inArray($(this).attr("data-id"), $AppFilter);
            if (itemno != -1) {
                $AppFilter.splice(itemno, 1);
            }
            currTableIndex = 16;
            filterApplist();
        });
    }

    $(".selectall").click(function () {
        $AppFilter = [];
        $(".application-menu .menu-item a").removeClass("selected");
        $("#site-AEU .application-tag span").remove();
        currTableIndex = 16;
        filterApplist();
    });

    function filterApplist()
    {
        if ($AppFilter.length == 0) {
            $(".AEU-container li[data-gp]").show();
        }
        else {
            $(".AEU-container li[data-gp]").hide();
            $.each($AppFilter, function (i, v) {
                $.each($(".AEU-container li[data-gp]"), function (bi, bn) {
                    $.each($(bn).attr("data-gp").split(","), function (ci, cv) {
                        if (v == cv)
                            $(bn).show();
                    });
                });
            });
        }
        CheckReadMoreTable(true);
        equalHeightBlock();
    }
});

