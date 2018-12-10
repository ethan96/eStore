
$(document).ready(function () {
    lazyLoadImg();
    //product add to cart
    $(".canAddToCart").bind("click", function () {
        $(this).attr({ "disabled": "disabled", "style": "background-image: url(" + GetStoreLocation() + "images/Loading.gif); background-repeat: no-repeat;" });
        add2cart($(this).attr("sproductid"), 1);
    });

    $(".btnSearch").click(function () {
        var value = $.trim($("#storekeyworddispay").val());
        if (value == $("#storekeyworddispay").attr("title") || value.length < 3) {
            alert($.eStoreLocalizaion("Iot_plese_input_some_keywords"));
            return false;
        }
    });
});

// 780 banner advs
jQuery.fn.IotModelAdBanners = function (data, count) {
    var iAds = 0;
    var obj = $(this);
    obj.html("");
    if (data != null) {
        $(data).each(function (i, n) {
            if (n.type == "StoreAds" && n.HtmlContent == "") {
                if (iAds > count - 1) return;
                else {
                    obj.append(n.HtmlContent || "<div class=\"iot-block margin-TopBottom15\"><a href=\"" + n.Hyperlink + "\"><img src=\"" + n.image + "\" width=\"780\" /></a></div>");
                }
                iAds++;
            }
        });
    }
}

// add side banner
jQuery.fn.IotModelSideBanners = function (data, count) {
    var iAds = 0;
    var obj = $(this);
    obj.html("");
    if (data != null) {
        $(data).each(function (i, n) {
            if (n.type == "SliderBanner") {
                if (iAds > count - 1) return;
                else {
                    obj.append(n.HtmlContent || "<div class=\"iot-navBlock\"><a href=\"" + n.Hyperlink + "\"><img src=\"" + n.image + "\" /></a></div>");
                }
                iAds++;
            }
        });
    }
}


// home adv banners
jQuery.addIotAdBanners = function (data, count) {
    var topTwoAdList = new Array();
    var fourAdList = new Array();
    var iHight = 0;
    if (data != null) {
        $(data).each(function (i, n) {
            if (n.type == "TodayHighLight") {
                if (iHight > count - 1) return;
                if (iHight < 2) topTwoAdList.push(n);
                else fourAdList.push(n);
                iHight++;
            }
        });
        addIotAdBanner($(".IotAds[adtype='twoDailAd']"), topTwoAdList);
        addIotAdBanner($(".IotAds[adtype='fourDailAd']"), fourAdList);
    }
}


function addIotAdBanner(divobj, data) {
    var obj = $(divobj);
    obj.html("");
    if (data.length > 0) {
        $(data).each(function (i, n) {
            obj.append(n.HtmlContent || "<div class=\"iot-bannerSIndex\"><a href=\"" + n.Hyperlink + "\"  target=\"" + n.Target + "\"><img src=\"" + n.image + "\" height=\"120\" /></a></div>");
        });
    }
}

jQuery.fn.eStoreMostBuyPro = function (count) {
    var obj = $(this).html("");
    var temp = "<div class='iot-eStoreblock'><div class='iot-eStoreblock-pic'><a href='{PUrl}' target='_blank'><img src='{PImage}' height='100' /></a></div><div class='iot-eStoreblock-txt'><div class='iot-eStoreblock-title'><a href='{PUrl}' target='_blank'>{PProId}</a></div><div class='iot-eStoreblock-content'><a href='{PUrl}' target='_blank'>{PDesc}</a></div><div class='iot-iconLinkBlock'><a href='{PUrl}' target='_blank'>{BuyNow}</a></div></div></div>";
    eStore.UI.eStoreScripts.getMostBuyPro(count, "",true, function (res) {
        $(res).each(function (i, n) {
            var mostbuy = temp;
            mostbuy = mostbuy.replace(/[{]PUrl[}]/g, "" + n.Link + "");
            mostbuy = mostbuy.replace(/[{]PImage[}]/g, "" + n.Image + "");
            mostbuy = mostbuy.replace(/[{]PProId[}]/g, "" + n.SProductID + "");
            mostbuy = mostbuy.replace(/[{]PDesc[}]/g, "" + n.Desc + "");
            mostbuy = mostbuy.replace(/[{]BuyNow[}]/g, "" + $.eStoreLocalizaion("Iot_Buy_Now") + "");
            obj.append(mostbuy);
        });
        obj.append("<div class='clearfix'></div>");
    });
};

function lazyLoadImg() {
    $(".lazyImg").each(function (i, n) {
        $(this).attr("src", $(this).attr("lazysrc")).removeAttr("lazysrc");
    });
}

jQuery.fn.showLoadImg = function () {
    $(this).prepend($("<img />").attr({ "class": "linLoadImg", "src": GetStoreLocation() + "images/Loading.gif" }));
}

jQuery.fn.clearLoadImg = function () {
    $(this).find(".linLoadImg").remove();
}



function add2cart(productid, qty) {
    eStore.UI.eStoreScripts.addProducttoCart(productid, qty
    , function (result) {
        if (result) { location = GetStoreLocation() + "Cart/Cart.aspx"; }
    });
}
function add2quote(productid, qty) {
    eStore.UI.eStoreScripts.addProducttoQuotation(productid, qty
        , function (result) {
            if (result) { location = GetStoreLocation() + "Quotation/quote.aspx"; }
        });
}


jQuery.fn.bindPagination = function (itemsseletor, pagesize) {
    var itemsOnPage = pagesize;
    $(this).pagination({
        items: $(itemsseletor).length,
        itemsOnPage: itemsOnPage,
        cssStyle: 'dark-theme',
        onPageClick: function (pageNumber, event) { return showpaginationproducts(itemsseletor, pageNumber, pagesize); },
        onInit: function () { return showpaginationproducts(itemsseletor, 1, pagesize); }
    });
}

// seach function


// ** pagination **


function showpaginationproducts(itemsseletor, page, pagesize) {
    var items = $(itemsseletor);
    var from = (page - 1) * pagesize;
    var to = page * pagesize - 1;
    $(items).hide();
    for (var i = from; i <= to; i++)
        items.filter(":eq(" + i + ")").show();
    return false;
}


// JavaScript Document

$(function () {
    //** top ******************************************************************************************************************************************
    $('input[placeholder]').each(function () {
        var input = $(this);
        $(input).val(input.attr('placeholder'));

        $(input).focus(function () {
            if (input.val() == input.attr('placeholder')) {
                input.val('');
            }
        });

        $(input).blur(function () {
            if (input.val() == '' || input.val() == input.attr('placeholder')) {
                input.val(input.attr('placeholder'));
            }
        });
    });

    //** nav ******************************************************************************************************************************************

    //spec filter
    $(".iot-navBlock-link")
	.mouseenter(function () {
	    $(this).find(".iot_iconList").addClass("showList");
	    $(this).find("ul").css("display", "block");
	})
	.mouseleave(function () {
	    $(this).find(".iot_iconList").removeClass("showList");
	    $(this).find("ul").css("display", "none");
	});
    //移除List a最後一個下面的灰線
    $(".iot-Wrapper .iot-navBlock-link ul li:last-child").css("border-bottom", "none");


    //** right ******************************************************************************************************************************************

    //index 清除bannerS最後一個的margin
    $(".iot-block .iot-bannerSIndex:last-child").css("margin-bottm", "0");


    // 產品顯示為欄還是列
    $(".iot-proStyleBlock span").click(function () {
        var i = $(this).hasClass("iot-show");
        //alert(i);
        if (i == false) {
            $(".iot-proStyleBlock span").removeClass("iot-show");
            $(this).addClass("iot-show");
            $(".iot-proColumnBlock,.iot-proListBlock").toggleClass("iot-show");
        } else if (i == true) {
            return false;
        }
    });



    // ** 清除float *******************************************************************************************************************************
    $(".iot-header,.iot-topMsg,.iot-storiesTxt-Top,.iot-container,.iot-carouselBlock,.iot-storiesSelectBlock,.iot-block,.iot-highlightBlock,.iot-footer,.iot_proList,.iot-switchingBlock,.iot-proStyleBlock").append("<div class='clearfix'></div>")



});


function showQQAPI()
{ window.open('http://b.qq.com/webc.htm?new=0&sid=8008100345&eid=218808P8z8p8Q8K8z8p80&o=http://buy.advantech.com.cn/&q=7&ref=' + document.location, '_blank', 'height=544, width=644,toolbar=no,scrollbars=no,menubar=no,status=no'); }


function checkEmailFormat(emailAddress) {
    var re = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/;
    if (re.test($.trim(emailAddress)))
        return true;
    else
        return false;
}