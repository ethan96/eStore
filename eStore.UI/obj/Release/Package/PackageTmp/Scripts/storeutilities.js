$(document).ready(function () {
    //$(":text,:password").keypress(function (event) {
    $(".textEntry:text").keypress(function (event) {
        var key = event.keyCode ? event.keyCode : event.which;
        if (key == '13') {
            if ($(this).hasClass("textEntry")) {
                var closestButton = $(this).closest(":has(:submit)").find(":submit:first");
                if (closestButton) {
                    $(closestButton).trigger("click");
                    return false;
                }
            }
            else {
                return false;
            }
        }
        else if ($(this).hasClass("InputValidation")) {
            if (event.which > 29 && !String.fromCharCode(event.which).match(/[a-zA-Z0-9\-\\\/+\s.@;#_\(\)]+/)) {
                alert($.eStoreLocalizaion("not_a_valid_char"));
                return false;

            }
        }
    });

    $("#eStore_LogIn_input :password,#eStore_LogIn_input :text").keypress(function (event) {
        if (event.keyCode == '13') {
            eval($("#LoginButton").attr('href'));
            return false;

        }
    });

    checkEstoreInputQTY();

    $(".qtyboxOnlyNO").keyup(function (event) {
        if (!checkQTYInfo($(this))) {
            $(this).val(1);
        }
        else {
            if ($(this).val().match(/\d{5,}/)) {
                $(this).val($(this).val().substr(0, 5));
                alert($.eStoreLocalizaion("too_large_number"));
                return false;
            }
        }
    });

    $("img[src='']").attr("src", GetStoreLocation() + "images/photounavailable.gif");

    $(".editorpanel :text,textarea").PreFillToolTips();

    showProductAjaxPric();
});

function showProductAjaxPric() {
    $.each($(".ajaxproductprice"), function () {
        var pricepanel = $(this);
        var _isminprice = pricepanel.hasClass("miniprice");
        var pricestyle = pricepanel.hasClass("miniprice") ? "MinPrice" : (pricepanel.hasClass("productpriceLarge") ? "productpriceLarge" : "productprice");
        $.ajaxSetup({ cache: false });
        $.getJSON(GetStoreLocation() + "proc/do.aspx",
        {
            func: "12"
        , id: $(pricepanel).attr("id")
        , isminprice: _isminprice
        , pricestyle: pricestyle
        },
        function (data) {
            $(pricepanel).html(data.price);
        });
    });
}


$.fn.equalHeight = function () {
    var tallest = 0;
    this.each(function () {
        thisHeight = $(this).height();
        if (thisHeight > tallest) {
            tallest = thisHeight;
        }
    });
    return $(this).height(tallest);
};
jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", ($(window).height() - this.height()) / 2 + $(window).scrollTop() + "px");
    this.css("left", ($(window).width() - this.width()) / 2 + $(window).scrollLeft() + "px");
    return this;
};

jQuery.fn.PreFillToolTips = function () {
    return this.each(function () {
        if ((this.type === "text" || this.type === "password" || this.type === "textarea") && this.title) {
            if ($.trim($(this).val()) == "" || $.trim($(this).val()) == this.title) {
                $(this).val(this.title).css("color", "#CCC");
            }
            $(this).bind("focus", function () {
                if ($.trim($(this).val()) == this.title) {
                    $(this).val('').css("color", "#000");
                }
            });
            $(this).bind("blur", function () {
                if ($.trim($(this).val()) == '') {
                    $(this).val(this.title).css("color", "#CCC");
                }
            });
        }

    })
};

jQuery.fn.validateTextBoxWithToolTip = function () {
    var message = "";
    var isValidated = true;
    var i = 0;
    var errorObj = null;
    this.each(function () {
        if ((this.type === "text" || this.type === "password") && this.title) {
            if ($.trim($(this).val()) == "" || $.trim($(this).val()) == this.title) {
                message += this.title + $.eStoreLocalizaion("can_not_be_empty") + "\r";
                isValidated = false;
                if (i == 0) {
                    i++;
                    errorObj = $(this);
                }
            }
        }
    })
    if (!isValidated) {
        alert(message);
        if (errorObj != null) {
            errorObj.focus();
        }
        return false;
    }
    else
        return true;
};

jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = 1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        // CAUTION: Needed to parenthesize options.path and options.domain
        // in the following expressions, otherwise they evaluate to undefined
        // in the packed version for some reason...
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

//store ads
var centerpopuptimeout;
function centerpopup(ad) {
    if ($(".centerpopupadvertisement").length == 0) {
        if (ad.HtmlContent != "") {
            $("<div/>").attr("class", "centerpopupadvertisement").html(ad.HtmlContent)
        .appendTo("body");
        }
        else {
            $("<div/>").attr("class", "centerpopupadvertisement").html(
            $("<a/>")
            .attr("href", ad.Hyperlink)
            .attr("target", ad.Target)
            .html($("<img/>").attr("src", ad.image).attr("alt", ad.Title)))
            .append($("<div/>").attr("class", "adclose").html(" &nbsp;&nbsp;&nbsp &nbsp;&nbsp;&nbsp"))
        .appendTo("body");
        }
        if ($(".centerpopupadvertisement").css("position") == "")
            $(".centerpopupadvertisement").center();
        else {
            $(".centerpopupadvertisement").css("top", $(".eStore_container").offset().top + $(".centerpopupadvertisement").offset().top + "px");
            $(".centerpopupadvertisement").css("left", $(".eStore_container").offset().left + $(".centerpopupadvertisement").offset().left + "px");

        }
        //        $(".centerpopupadvertisement").css("position", "absolute");
        //        $(".centerpopupadvertisement").css("top", $(".master-wrapper-page").offset().top + 325 + "px");
        //        $(".centerpopupadvertisement").css("left", $(".master-wrapper-page").offset().left + 312 + "px");

        $("#centerpopupreopen").html(ad.Title).bind("click", function () {
            $.cookie("centerpopup" + ad.id, false, { expires: -1 });
            $(".centerpopupadvertisement").show();
        });

        $(".centerpopupadvertisement .adclose").bind("click", function () {
            if ($(".centerpopupadvertisement .adcookiecheckbox").length > 0) {
                if ($(".centerpopupadvertisement .adcookiecheckbox input").attr("checked")) {
                    $.cookie("centerpopup" + ad.id, true);
                }
            }
            else
                $.cookie("centerpopup" + ad.id, true);
            $(".centerpopupadvertisement").hide();
            clearTimeout(centerpopuptimeout);
        });
    }

    if (!$.cookie("centerpopup" + ad.id)) {
        $(".centerpopupadvertisement").show();
        centerpopuptimeout = setTimeout(function () {
            $(".centerpopupadvertisement").hide();
        }, 180000);
    }
    else {
        $(".centerpopupadvertisement").hide();
    }
};

function topsliderdown(ad) {
    $(".master-wrapper-content").before($("<div/>").attr("class", "advertisementtop").addClass("hide").html(
                        $("<a/>")
            .attr("href", ad.Hyperlink)
            .attr("target", ad.Target)
            .html($("<img/>").attr("src", ad.image).attr("alt", ad.Title)))
            .append($("<div/>").attr("class", "adclose").html(" &nbsp;&nbsp;&nbsp &nbsp;&nbsp;&nbsp")));
    setTimeout(function () {
        $(".advertisementtop").slideDown(2000, function () {
            $(this).removeClass("hide");
            setTimeout(function () {
                $(".advertisementtop").slideUp(2000)
            }, 10000)
        });
    }, 3000);
    $(".advertisementtop .adclose").bind("click", function () {
        $.cookie("topsliderdown" + ad.id, true);
        $(this).parent().remove();
    });
};

function GoldenEggsAd(ad) {
    $(ad.HtmlContent).appendTo("body");
    eableGoldEggsAdv(ad);
};

//start expanding adv
function expanding(ad) {
    $("#expandingAdvertisement").append(ad.HtmlContent);
    var cc = $(ad.HtmlContent).find("img");
    var swidth = $(cc[1]).attr("dwidth"); //第二个图片宽度
    var twidth = $(cc[2]).attr("dwidth"); //第三个图片宽度
    $("#expandingAdvertisement").floatdiv({ left: "5px", bottom: "10px" });
    setTimeout(function () { tanimate(swidth, twidth); }, 2000);
    setTimeout(function () { closeAds(swidth, twidth); }, 7000);
    $("#expandingAdvertisement #sideadsstagingclose").click(function () {
        closeAds(swidth, twidth);
    });
}

function loadImage(imgs) {
    imgs.each(function (i, img) {
        var imgshow = new Image();
        imgshow.src = img.src;
        imgshow.onload = function () { };
    });
}

function tanimate(swidth, twidth) {
    $("#expandingAdvertisement .sideads .sideadstitle").unbind("click");
    $("#expandingAdvertisement .sideads .sideadsstaging1").animate({ "width": "+=" + swidth + "px" }, swidth * 3.5, function () {
        setTimeout(function () {
            $("#expandingAdvertisement .sideads .sideadsstaging2").animate({ "width": "+=" + twidth + "" }, twidth * 3.5, function () {
                $("#expandingAdvertisement #sideadsstagingclose").show();
            });
        }, 500);
    });
}
function closeAds(swidth, twidth) {
    $("#expandingAdvertisement #sideadsstagingclose").hide();
    $("#expandingAdvertisement .sideads .sideadsstaging2").animate({ "width": "-=" + twidth + "px" }, twidth * 3.5, function () {
        setTimeout(function () {
            $("#expandingAdvertisement .sideads .sideadsstaging1").animate({ "width": "-=" + swidth + "px" }, twidth * 3.5, function () {
                $(".sideads .sideadstitle").bind("click", function () { tanimate(swidth, twidth) });
            });
        }, 500);
    });
}
//end expanding adv


var leftInt = 0;
var rightInt = 0;
var leftPicTimer;
var rightPicTimer;
var timeSpan = 6000;
function changeLeftDuiLianImage() {
    if (leftInt >= leftDuilianData.length)
        leftInt = 0;
    var context = "<div class=\"duilian_con\"><a target='" + leftDuilianData[leftInt].Target + "' href='" + leftDuilianData[leftInt].Hyperlink + "'><img src='" + leftDuilianData[leftInt].image + "' alt='" + leftDuilianData[leftInt].Title + "' /></a></div>";
    if (leftDuilianData.length == 1)
        $("#leftDuiLianImage").html(context);
    else {
        $("#leftDuiLianImage").fadeTo("slow", 0.01, function () {
            $(this).html(context);
            $(this).fadeTo("slow", 1, function () {
                leftInt++;
            });
        });
    }
}
function changeRightDuiLianImage() {
    if (rightInt >= rightDuilianData.length)
        rightInt = 0;
    var context = "<div class=\"duilian_con\"><a target='" + rightDuilianData[rightInt].Target + "' href='" + rightDuilianData[rightInt].Hyperlink + "'><img src='" + rightDuilianData[rightInt].image + "' alt='" + rightDuilianData[rightInt].Title + "' /></a></div>"
    if (rightDuilianData.length == 1)
        $("#rightDuiLianImage").html(context);
    else {
        $("#rightDuiLianImage").fadeTo("slow", 0.01, function () {
            $(this).html(context);
            $(this).fadeTo("slow", 1, function () {
                rightInt++;
            });
        });
    }
}

var leftDuilianData = new Array();;
var rightDuilianData = new Array();;
function showDuilian() {
    if (leftDuilianData != undefined && leftDuilianData.length > 0) {
        $("form").append("<div class=\"duilian duilian_left\"><div id='leftDuiLianImage'></div><a class=\"duilian_close\">X close</a></div>");
        changeLeftDuiLianImage();
        if (leftDuilianData.length > 1) {
            leftPicTimer = setInterval(changeLeftDuiLianImage, timeSpan);
            $("#leftDuiLianImage").hover(function () {
                clearInterval(leftPicTimer);
            },
                function () {
                    leftPicTimer = setInterval(changeLeftDuiLianImage, timeSpan);
                }
            );
        }
    }
    if (rightDuilianData != undefined && rightDuilianData.length > 0) {
        $("form").append("<div class=\"duilian duilian_right\"><div id='rightDuiLianImage'></div><a class=\"duilian_close\">X close</a></div>");
        changeRightDuiLianImage();
        if (rightDuilianData.length > 1) {
            rightPicTimer = setInterval(rightDuilianData, timeSpan);
            $("#leftDuiLianImage").hover(function () {
                clearInterval(rightPicTimer);
            },
                function () {
                    rightPicTimer = setInterval(rightDuilianData, timeSpan);
                }
            );
        }
    }
}
jQuery.eStoreDuiLianAD = function (data) {
    $(document).ready(function () {
        if (data.length > 0) {
            $.each(data, function (i, n) {
                if (n.LocationPath == "Left") {
                    leftDuilianData.push(n);
                }
                else if (n.LocationPath == "Right") {
                    rightDuilianData.push(n);
                }
            });

            showDuilian();

            var duilian = $("div.duilian");
            var duilian_close = $("a.duilian_close");

            var window_w = $(window).width();
            if (window_w > 1000) { duilian.show(); }
            $(window).scroll(function () {
                var scrollTop = $(window).scrollTop();
                duilian.stop().animate({ top: scrollTop + 150 });
            });
            duilian_close.click(function () {
                $(this).parent().hide();
                return false;
            });
        }
    });
}

jQuery.eStoreAD = function (data) {

    if (data && data.length != 0) {
        var floatList = new Array();
        var adList = new Array();
        $.each(data, function (i, n) {
            if (n.type == "Floating") {
                floatList.push(n);
            }
            else {
                adList.push(n);
            }
        });
        if (floatList.length > 0) {
            $.eStoreDuiLianAD(floatList);
        }
        $.each(adList, function (i, item) {
            switch (adList[i].type) {
                case "TopSliderDown":
                    if (!$.cookie("topsliderdown" + adList[i].id)) {
                        topsliderdown(adList[i]);
                    }
                    delete adList[i];
                    break;
                case "CenterPopup":
                    centerpopup(adList[i]);
                    delete adList[i];
                    break;
                case "Expanding":
                    var adObj = adList[i];
                    setTimeout(function () {
                        var cc = $(adObj.HtmlContent).find("img");
                        loadImage(cc);
                        expanding(adObj);
                    }, 5000);
                    delete adList[i];
                    break;
                case "GoldenEggs":
                    GoldenEggsAd(adList[i]);
                    delete adList[i];
                    break;
                default:
                    break;
            }
        });
        var imgitems = 0;

        if ($("#storeSideAds").length != 0) {
            $("#storeSideAds").empty();
            if ($("#storeSideAds").closest(".eStore_container").length != 0) {
                var deltaLength = $("#storeSideAds").closest(".eStore_container").height() - $("#storeSideAds").parent().height();
                var imgheight = 110;
                imgitems = parseInt(deltaLength / imgheight);
            }
            else
                imgitems = 2;
            imgitems = imgitems < 2 ? 2 : imgitems;
            var i = 0;
            for (; i < imgitems && i < adList.length; i++) {
                if (adList[i] && adList[i].image != null)
                    generateEstoreAds(adList[i], "#storeSideAds", adList[i].HtmlContent);
            }
            for (; i < adList.length; i++) {
                if (adList[i] && adList[i].image != null)
                    generateEstoreAds(adList[i], "#storeBottomAds", adList[i].HtmlContent);
                if (i - imgitems == 4) return false;
            }
            $("#storeSideAds").find(".needlogin").bind("click", function () {
                if (typeof (popuploginnormal) != "undefined") {
                    return popuploginnormal($(this).text());
                }
            });
        }
        else if ($("#storeBottomAds").length != 0) {
            var cnt = 0;
            for (var i = 0; i < adList.length; i++) {
                if (adList[i]) {
                    generateEstoreAds(adList[i], "#storeBottomAds", adList[i].HtmlContent);
                    cnt++;
                }
            }
            if (cnt > 0) {
                if (cnt > 5 * HomeBannerLineCap && HomeBannerLineCap == 1) {
                    carouFredSelFun();
                }
                $(".storeBottomAdsHeader").show(0);
                $("#storeBottomAds").find(".needlogin").bind("click", function () {
                    if (typeof (popuploginnormal) != "undefined") {
                        return popuploginnormal($(this).text());
                    }
                });
            }
        }

    }
};
function carouFredSelFun() {
    $('#storeBottomAds').carouFredSel({ scroll: 1 });
}
jQuery.eStoreLocalizaion = function (key) {
    var reslut;
    $.each(eStoreTranslation, function (i, n) {
        if (n.key == key)
            reslut = n.value;
    });
    if (reslut == undefined || reslut == null)
        reslut = key.replace(/_/g, " ");
    return reslut;

};

function generateEstoreAds(data, container, content) {
    if (content == "") {
        var node = $("<a/>")
            .attr("href", data.Hyperlink)
            .attr("target", data.Target)
            .html($("<img/>").attr("src", data.image).attr("alt", data.Title));
        if (container == "#storeBottomAds") {
            $("<li/>").html(node).appendTo(container);
        }
        else
            node.appendTo(container);
    }
    else {
        $(container).append(data.HtmlContent)
    }
}

function checkSubQTYInfo() {
    var isResult = true;
    var isReturnEach = false;
    $(".qtyboxOnlyNO").each(function (i) {
        if (!checkQTYInfo($(this))) {
            if (confirm("QTY must input NO. Don't notice it?")) {
                isResult = true;
                isReturnEach = true;
            }
            else {
                $(this).focus();
                isResult = false;
                isReturnEach = true;
            }
        };
        if (isReturnEach) {
            return false;
        };
    });
    return isResult;
}
function checkQTYInfo(obj) {
    var str = obj.val();
    if (str.match(/^0/g) || str.match(/[^0-9]/g)) {
        return false;
    }
    else {
        return true;
    }
}

function checkEstoreInputQTY() {
    $(".qtytextbox").keyup(function (event) {
        if ($(this).val().match(/[^0-9]/g)) {
            $(this).val(1);
        }
        else {
            if ($(this).val().match(/\d{5,}/)) {
                $(this).val($(this).val().substr(0, 5));
                alert($.eStoreLocalizaion("too_large_number"));
                return false;
            }
        }
    });
}

function ValidateRequestSpecial(th) {
    if (th.value != "") {
        th.value = th.value.replace(/</g, "").replace(/>/g, "");
    }
}
function formatNum(num) {
    var cc = num;
    cc = String(cc.toFixed(0));
    var re = /(-?\d+)(\d{3})/;
    while (re.test(cc)) cc = cc.replace(re, "$1,$2")
    return cc;
}

function formatdecimal(num) {
    var cc = num;
    cc = String(cc.toFixed(2));
    var re = /(-?\d+)(\d{3})/;
    while (re.test(cc)) cc = cc.replace(re, "$1,$2")
    return cc;
}

function formatRound(num, unit) {
    if (unit == null || unit == "" || unit == undefined)
        unit = 1;
    num = num.toString().replace(/\$|\,/g, '');
    var cc = num;
    cc = Math.round(num / unit) * (unit * 10000) / 10000;
    var ccNun = cc.toString();
    var re = /(-?\d+)(\d{3})/;
    while (re.test(ccNun))
        ccNun = ccNun.replace(re, "$1,$2")
    return ccNun;
}

jQuery.fn.floatdiv = function (location) {
    //ie6要隐藏纵向滚动条
    var isIE6 = false;
    if ($.browser.msie && $.browser.version == "6.0") {
        $("html").css("overflow-x", "auto").css("overflow-y", "hidden");
        isIE6 = true;
    };
    return this.each(function () {
        var loc; //层的绝对定位位置
        if (location == undefined || location.constructor == String) {
            switch (location) {
                case ("rightbottom"): //右下角
                    loc = { right: "0px", bottom: "0px" };
                    break;
                case ("leftbottom"): //左下角
                    loc = { left: "0px", bottom: "0px" };
                    break;
                case ("lefttop"): //左上角
                    loc = { left: "0px", top: "0px" };
                    break;
                case ("righttop"): //右上角
                    loc = { right: "0px", top: "0px" };
                    break;
                case ("middle"): //居中
                    var l = 0; //居左
                    var t = 0; //居上
                    var windowWidth, windowHeight; //窗口的高和宽
                    //取得窗口的高和宽
                    if (self.innerHeight) {
                        windowWidth = self.innerWidth;
                        windowHeight = self.innerHeight;
                    } else if (document.documentElement && document.documentElement.clientHeight) {
                        windowWidth = document.documentElement.clientWidth;
                        windowHeight = document.documentElement.clientHeight;
                    } else if (document.body) {
                        windowWidth = document.body.clientWidth;
                        windowHeight = document.body.clientHeight;
                    }
                    l = windowWidth / 2 - $(this).width() / 2;
                    t = windowHeight / 2 - $(this).height() / 2;
                    loc = { left: l + "px", top: t + "px" };
                    break;
                default: //默认为右下角
                    loc = { right: "0px", bottom: "0px" };
                    break;
            }
        } else {
            loc = location;
        }
        $(this).css("z-index", "9999").css(loc).css("position", "fixed");
        if (isIE6) {
            if (loc.right != undefined) {
                //2008-4-1修改：当自定义right位置时无效，这里加上一个判断
                //有值时就不设置，无值时要加18px已修正层位置
                if ($(this).css("right") == null || $(this).css("right") == "") {
                    $(this).css("right", "18px");
                }
            }
            $(this).css("position", "absolute");
        }
    });
};

function showAlertMassage(message) {
    $("<div id='DivshowAlertMassage'><p>" + message + "</p></div>")
        .dialog({
            height: 140,
            zIndex: 3899, title: 'Message',
            modal: true
        });
}

function showConfirmMessage(message, trueFun, FalseFun) {

    popupDialog($("<div>").html(message).append($("<br />")).append($("<input />").prop({ type: "button", value: "No" }).bind("click",FalseFun))
                                        .append($("<input />").prop({ type: "button", value: "Yes" }).bind("click", trueFun)));
}


function checkSelect() {
    //alert($("[name='cbproduct'][checked='true']").length);
    //alert($("[name='cbproduct']").length);
    var cbproduct = $("[name='cbproduct']");
    var checkCount = false;
    if (cbproduct.length > 0) {
        cbproduct.each(function (i) {
            if (this.checked) checkCount = true;
        });
    } else {
        alert($.eStoreLocalizaion("Can_not_find_the_product"));
        return false;
    }
    if (checkCount) return true;
    alert($.eStoreLocalizaion("Can_not_find_the_product"));
    return false;
}


function checkMOQ() {
    var cc = true;
    var productids = "";
    var i = 0;
    $(".estoretable input:checkbox[MOQ][value!=''][checked]").each(function () {
        if (i == 0) {
            productids = $(this).attr("value") + "[" + $(this).attr("MOQ") + "]";
            $(this).focus();
        }
        else
            productids = productids + "," + $(this).attr("value") + "[" + $(this).attr("MOQ") + "]";
        cc = false;
        i++;
    });
    if (!cc) {
        if (confirm(productids))
            return true;
        else
            return false;
    }
    return cc;
}

//是否存在指定函数 
function isExitsFunction(funcName) {
    try {
        if (typeof (eval(funcName)) == "function") {
            return true;
        }
    } catch (e) { }
    return false;
}

//获取翻译文件
function lazyLoadingLoaclTranslation(txtfile,language) {
    $.get("/tutorial/" + language + "/" + txtfile, function (data) {
        popupMessage(data);
    });
}

(function($) {
    $.fn.NoEnterSubmit = function() {
        $(this).find("input:text").keypress(function (e) {
            if (e.which === 13) { // 判断所按是否回车键  
                return false;
            }
        });
    };
})(jQuery);

// lazy load image
function LazyLoadImg() {
    $(".lazy").Lazy({
        afterLoad: function (element) {
            $(element).css({ "background-image": "none" })
        }
    });
}