function menuPositionShow() {

    if ($("body").hasClass("noRWD") & windowW < 980) {
        return;
    }

    menuW = $(".eStore_menuLink").outerWidth(true);
    var menuPosition = (windowW - menuW) / 2;
    $(".eStore_menuLink_linkList").css({
        "left":-menuPosition,
        "padding-left":menuPosition,
        "padding-right":menuPosition,
        "display":" "
    });

}//menuBG計算


$(function () {
    var menuLeave, menuEnter;
    $(".eStore_menuLink_link").bind('mouseenter mouseleave click', function (event) {

        if (notMatch(event)) {
            return;
        }
        if (mouseStyle == 'PC-mouseenter') {
            $(".eStore_menuLink_link").removeClass("on");
            $(this).addClass("on");
            var haveList = $(".eStore_menuLink_link.on").find("div").hasClass("eStore_menuLink_linkList");

            if (haveList) {
                clearTimeout(menuLeave);
                menuEnter = setTimeout(function () {
                    $(".eStore_menuLink_linkList").css({
                        "display": ""
                    });
                    $(".eStore_menuLink_link").removeClass("onMenuLink");
                    $(".eStore_menuLink_linkListBG").remove();
                    $(".eStore_menuLink_link.on").addClass("onMenuLink");
                    $(".eStore_menuLink_linkBlock").append("<div class='eStore_menuLink_linkListBG'></div>");
                }, 500);
            }
        } else if (mouseStyle == 'PC-mouseleave') {
            clearTimeout(menuEnter);
            menuLeave = setTimeout(function () {
                $(".eStore_menuLink_link").removeClass("onMenuLink on");
                $(".eStore_menuLink_linkListBG").remove();
            }, 300);
        } else if (mouseStyle == 'PC-click' || mouseStyle == 'mobile-click-left') {
            $(this).toggleClass("show");

            $(this).find(".eStore_menuLink_linkList").slideToggle("400", function () {
                hideMenuH();
            });

        } else {
            var haveList = $(this).find("div").hasClass("eStore_menuLink_linkList");
            if (haveList) {
                var on = $(this).hasClass("onMenuLink");
                //alert(on);
                if (on) {
                    $(".eStore_menuLink_link").removeClass("onMenuLink");
                    $(".eStore_menuLink_linkListBG").remove();
                } else {
                    $(this).addClass("onMenuLink").siblings().removeClass("onMenuLink");
                    $(".eStore_menuLink_linkBlock").append("<div class='eStore_menuLink_linkListBG'></div>");
                }
            } else {
                $(".eStore_menuLink_link").removeClass("onMenuLink");
                $(".eStore_menuLink_linkListBG").remove();
            }
        }
    });

    var notMatch = function (event) {

        var width = getWidth();
        if (device.mobile() || device.tablet()) {
            if (event.type == 'click') {
                if (width) {
                    mouseStyle = 'mobile-click-left';
                    return false;
                } else {
                    mouseStyle = 'mobile-click';
                    return false;
                }
            }
            return true;
        } else {
            if (width) {
                if (event.type == 'click') {
                    mouseStyle = 'PC-click';
                    return false;
                }
            } else {
                if (event.type == 'mouseenter') {
                    mouseStyle = 'PC-mouseenter';
                } else if (event.type == 'mouseleave') {
                    mouseStyle = 'PC-mouseleave';
                }
                return false;
            }
        }
        return true;
    };

    var getWidth = function () {
        return $(".eStore_mobile .eStore_seeMore").is(':visible');
    };


    $(window).resize(function () {
        //判斷link是否需延長
        mobileShow = $(".eStore_mobile").is(':visible');
        $("#HfMobile").val(mobileShow);
        if (mobileShow) {
            $(".eStore_menuLink_linkList").css({
                "left": 0,
                "padding-left": 0,
                "padding-right": 0
            });
            //$(".mobileNotShow").hide();
            $("a.mobileHref").each(function () {
                $(this).attr("link", $(this).attr("href")).removeAttr("href");
            });
        } else {
            $(".eStore_headerBottom,.eStore_wrapper").css({
                "min-height": 0
            });
            //menuBG計算
            menuPositionShow();
            //$(".mobileNotShow").show();
            $("a.mobileHref").each(function () {
                $(this).attr("href", $(this).attr("link")).removeAttr("link");
            });
        }
    }).resize();
})
