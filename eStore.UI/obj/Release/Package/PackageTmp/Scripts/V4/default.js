// JavaScript Document
function DetectOutDateIE() {
    if (navigator.appVersion.indexOf("MSIE 6.") != -1 || navigator.appVersion.indexOf("MSIE 7.") != -1 || navigator.appVersion.indexOf("MSIE 8.") != -1) {

        $(".ctrlGDPR").prepend($("#eStore_analyzingBrowser").show());
        return true;
    }
    else {
        document.getElementById("eStore_analyzingBrowser").style.display = "none";
        return false;
    }
}
function CloseAnalyzingBrowser() {
    document.getElementById("eStore_analyzingBrowser").style.display = "none";
}

var linkStatus=0,linkStatusBG,MyAccountEnter,windowW,menuW,mobileShow;

function clearFloat(){
	
    $(".eStore_block980,.eStore_menuLink,.eStore_footerAreaTop,.eStore_chatBox,.eStore_chatBox_txt,.eStore_ShoppingBox li ,.orderlistTable,.eStore_compareProduct_list li,.eStore_index_Highlight,.eStore_category_banner_promo_listBottom,.eStore_specfilter li,.eStore_category_content_listBlock,.eStore_category_content_btnBlock,.eStore_productBlock,.eStore_other_BGBlock,.eStore_product_product,.eStore_index_Highlight_contentBlock,.eStore_index_Solution,.eStore_index_Solution_linkBlock,.eStore_compare_contentTop,.eStore_compare_content h2,.eStore_txtPage_content,.eStore_4Step_row li,.eStore_system_action .price,.eStore_customersBought .txt .priceOrange,.eStore_order_radioList,.eStore_orderStep2 td,.eStore_ShippingPersonal_msg,.eStore_order_payWay .eStore_order_radioList_content,.eStore_account_msgBlock,.eStore_account_msgBlock_content.AddressBookList li,.eStore_account_content .eStore_order_orderDetails,.eStore_account_content h4,.eStore_system_listFloat .eStore_openBox_select,.eStore_LogIn,.eStore_footerLinks").append( "<div class='clearfix'></div>" );
	
}//清除float

function footerPosition(){
	
//    var footerH = $(".eStore_footer").outerHeight( true );
//    var heightH = footerH + 40;//數字部分可以依照內容想與下面的footer距離做調整
//    $(".eStore_wrapper").css("margin-bottom",-footerH);
//    $(".eStore_wrapper .eStore_wrapper-push").remove();//resize時 需重新計算
//    $(".eStore_wrapper").append("<div class='eStore_wrapper-push'></div>");
//    $(".eStore_wrapper-push").css("height",heightH);
	
}//footer置底 自動抓取footer高度 (一定要放在最下面,需等到浮動都清除後，高度才會正確)

function txtRightH(){
	
    var txtRight = $(".eStore_rightBlock").outerHeight( true );
    var txtLeft =$(".eStore_leftBlock").outerHeight( true );
    var minH=(txtRight>txtLeft)?txtRight:txtLeft;
    $(".eStore_txtPage_content .eStore_leftBlock").css({
        "min-height":minH
    });

}//純文字頁面選單間格線計算

function footerAreaClick(){
	
    var footerArea = $(".eStore_footerAreaBottom a.on").text();
    $(".eStore_footerAreaNow").html(":  "+footerArea);
	
}//footer area計算

function hideMenuH(){

    $(".eStore_headerBottom,.eStore_wrapper").css({
        "min-height":"0"
    });
    var menuH = $(".eStore_headerBottom").outerHeight( true );
    var contentH =$(".eStore_wrapper").height();
    var minH=(menuH>contentH)?menuH:contentH;
    $(".eStore_headerBottom,.eStore_wrapper").css({
        "min-height":minH
    });


}//隱藏的menu高度

function meunLinkStyle(){
	
    return mobileShow;

}//meunLinkStyle

function shoppingCart_title() {
    if ($(".eStore_ShoppingBox").data("loaded") == undefined) {
        var xhr;

        if (xhr && xhr.readystate != 4) {
            xhr.abort();
        }
        xhr = $.getJSON(GetStoreLocation() + 'api/Account/getShoppingCartItems',
        function (data) {
            $(".eStore_ShoppingBox").html(data).data("loaded", true);
            initmobileCart();
        });
    }
    else {
        initmobileCart();
    }
}

function initmobileCart()
{
    $(".eStore_ShoppingBox .orderlistTable .top,.eStore_ShoppingBox .img").css({
        "height": ""
    });
    if ($(".eStore_shoppingCart").hasClass("show")) {
        $(".eStore_ShoppingBox .orderlistTable .top").each(function () {
            var contentH = $(".eStore_ShoppingBox .orderlistTable .top").height();
            var contentH = (contentH > contentH) ? contentH : contentH;
            $(".eStore_ShoppingBox .orderlistTable .top").css({
                "height": contentH
            });
        });

        var imgBlock = $(".eStore_ShoppingBox .img").width();
        $(".eStore_ShoppingBox .img").css({
            "height": imgBlock
        });

        var imgW = $(".eStore_ShoppingBox .img img").width();
        var imgH = $(".eStore_ShoppingBox .img img").height();
        if (imgH > imgW) {
            $(".eStore_ShoppingBox .img img").css({
                height: 100 + "%",
                width: "auto"
            });
        } else {
            $(".eStore_ShoppingBox .img img").css({
                height: "auto",
                width: 100 + "%"
            });
        }
    }
}

function search(key) {
    //window.document.location = GetStoreLocation() + "Search.aspx?skey=" + key;
    eStore.UI.eStoreScripts.getSearchURL(key,function (res) {
        if (res) {
            window.document.location = res;
        }
        else {
            window.document.location = GetStoreLocation() + "Search.aspx?skey=" + key;
        }
    });  
}
var xhrsearch;
function searchproduct(id) {
    if (xhrsearch && xhrsearch.readystate != 4) {
        xhrsearch.abort();
    }
    xhrsearch = $.getJSON(GetStoreLocation() + 'api/Search/getproducturl', { productid: id },

        function (url) {
            if (url != "") {
                window.document.location = url; 
            }
            else
            { search(ui.item.label); }
        });
    }

var StepTop,StepBottom,StepBottom2,scrollH,StepH,listH,startH,timeOut,timeOut2,listFloatTopH,i=0,scrollLeft;

function compareHeight(){

	StepH = $(".eStore_4Steps").outerHeight( true );
	listH = $(".eStore_system_listFloat").outerHeight( true );
	startH = (StepH < listH)?listH:StepH;
	$(".positionFixed").animate({
		"height":startH
	},0);
	clearTimeout(timeOut,timeOut2);	
}
function scrollingHeight(){

	StepBottom = StepTop + StepH -($(".eStore_4Step_title").outerHeight(true));
	//閸掋倖鏌楀?scoll left
	if(StepTop < scrollH){
		$(".positionFixed").addClass("fixedLeft");
		if(scrollLeft!==0){
			$(".eStore_4Steps .eStore_4Step_title").css("left",-scrollLeft);
		}else{
			$(".eStore_4Steps .eStore_4Step_title").css("left","")
		}
		
		if( StepBottom < scrollH){
			$(".eStore_4Steps .eStore_4Step_title").css("top",-(scrollH-StepBottom));
		}else{
			$(".eStore_4Steps .eStore_4Step_title").css("top",0);
		}
	}else{
		$(".positionFixed").removeClass("fixedLeft");
	}
	
	//閸掋倖鏌楀?scoll right
	if( StepTop-10 < scrollH){
		$(".eStore_system_listFloat .eStore_system_listFloatPrice").fadeIn(1000,compareHeight());
		StepBottom2 = StepTop + (startH-listH);		
		if( StepBottom2 > scrollH ){
			$(".eStore_system_listFloat").css("top",(scrollH - StepTop)+10);
		}else{
			$(".eStore_system_listFloat").css("top",((StepTop+(startH-listH))-scrollH)+((scrollH - StepTop)+10));
		}
	} else {
		$(".eStore_system_listFloat").css("top",0);
		$(".eStore_system_listFloat .eStore_system_listFloatPrice").hide("fast",compareHeight());
	}
	
}
function showCmsResourceAdv() {
    if ($(".eStore_4Steps").length > 0) {
        try {
            if (typeof (eval("compareHeight")) == "function") {
                compareHeight();
            }
        }
        catch (e)
        { }
        $(".eStore_openBox").unbind("click").click(function () {
            $(this).toggleClass("openBox").siblings().toggle("fast");
            timeOut = setTimeout(function () {
                compareHeight();
            }, 300);
            timeOut2 = setTimeout(function () {
                scrollingHeight();
            }, 400);

        });
    }
    else {
        $(".eStore_openBox").unbind("click").click(function () {
            $(this).toggleClass("openBox").siblings().toggle("fast");
        });
    }
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
function fixTableLayout(container, children) {
    var containerwidth = 0; childrenwidth = 0;
    containerwidth = $(container).width();
   
    if ($(container).find(children).length>0)
        childrenwidth = $(container).find(children).first().width();
    if (childrenwidth > 0) {
        var columns = Math.floor(containerwidth / childrenwidth);
        $(container).find(children).css("clear", "");
        $(container).find(children).each(function (index, item) {
            if (index >= columns && index % columns == 0) {
                $(item).css("clear", "left");
            }
        });
     
    }
}
function popupDialogDelay(selector, delay) {
    setTimeout(function () {
        popupDialog(selector);
    }, delay);
}
function popupMessage(message) {
    popupDialog($("<div/>").html(message));
}
function popupMessagewithTitle(title, message) {
    popupDialog($("<div/>")
        .css("width", "320px")
        .append($("<h1/>").addClass("headertitle row20").html(title))
        .append($("<p/>").html(message)
        ));
}
function popupDialog(selector) {
    $.fancybox($(selector), {
        parent: "form:first",
        'autoDimensions': true,
        arrows:false,
        'width': 'auto',
        'height': 'auto',
        'transitionIn': 'none',
        'transitionOut': 'none'
    })
    ;
}
$.fn.imagesLoaded = function () {

    // Edit: in strict mode, the var keyword is needed
    var $imgs = this.find('img[src!=""]');
    // if there's no images, just return an already resolved promise
    if (!$imgs.length) { return $.Deferred().resolve().promise(); }

    // for each image, add a deferred object to the array which resolves when the image is loaded (or if loading fails)
    var dfds = [];
    $imgs.each(function () {

        var dfd = $.Deferred();
        dfds.push(dfd);
        var img = new Image();
        img.onload = function () { dfd.resolve(); }
        img.onerror = function () { dfd.resolve(); }
        img.src = this.src;

    });

    // return a master promise object which will resolve when all the deferred objects have resolved
    // IE - when all the images are loaded
    return $.when.apply($, dfds);

}

function createUnicaActivity(activity, productid, url) {
    $.ajax({
        url: GetStoreLocation() + 'proc/do.aspx?func=19&activitytype=' + activity + '&productID=' + productid + '&url=' + url,
        type: "POST",
        success: function (retData) {
        },
        error: function () {
        }
    });
    return true;
}

function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else {
        begin += 2;
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
            end = dc.length;
        }
    }
    return decodeURI(dc.substring(begin + prefix.length, end));
}

function checkGDPR(ie) {
    if ($("#eStore_gdpr").length > 0) {
        var gdpr = getCookie("GDPR");
        if (gdpr == null || gdpr == "false") {
            if (ie == true)
                $("#eStore_gdpr").addClass("eStore_GDPR_Seperate");
            else
                $("#eStore_gdpr").removeClass("eStore_GDPR_Seperate");
            $(".ctrlGDPR").prepend($("#eStore_gdpr").show());
        }
        else
            document.getElementById("eStore_gdpr").style.display = "none";
    }
}

function closeGDPR() {
    if ($("#eStore_gdpr").length > 0 && $("#cb_gdpr").length > 0) {
        $.cookie("GDPR", $("#cb_gdpr").prop("checked"), { expires: 365, path: '/' });
        $("#eStore_gdpr").hide();
    }
}

$(function () {

    //清除float
    clearFloat();

    //Check IE 8 and lower
    var ie = DetectOutDateIE();

    checkGDPR(ie);

    //login
    $(".eStore_topMsg li .eStore_MyAccount,.eStore_topMsg li .eStore_login").mouseenter(function () {
        clearTimeout(MyAccountEnter);
        $(".eStore_login").addClass("show");
    });
    $(".eStore_topMsg li .eStore_MyAccount,.eStore_topMsg li .eStore_login").mouseleave(function () {
        MyAccountEnter = setTimeout(function () {
            $(".eStore_login").removeClass("show");
        }, 500);
    });
    $(".eStore_menuLink_searchBlock input.storekeyworddispay,.eStore_searchBox input.storekeyworddispay").keypress(function (event) {

        if (event.keyCode == 13) {
            if ($("input.storekeyworddispay:last").data("autocompletefocus")) {
                searchproduct($(this).val());
            }
            else {
                search($(this).val());
            }
            return false;
        }
        else {
            $("input.storekeyworddispay:last").data("autocompletefocus", true);
        }
    });
    $(".eStore_menuLink_searchBlock a,.eStore_searchBox a").click(function () {
        search($(this).prev("input.storekeyworddispay").val());
        return false;
    });

    if ($(".hl_LiveChat").attr("onclick") != undefined && $(".hl_LiveChat").attr("onclick") != "")
        $("#tophl_LiveChat").attr("onclick", $(".hl_LiveChat").attr("onclick"));

    //footer area計算/
    footerAreaClick();
    //footer area計算顯示文字
    $(".eStore_footerAreaBtn").click(function () {
        $(".eStore_footerArea").toggleClass("show");
    });
    $(".eStore_footerAreaBottom a").click(function () {
        $(this).addClass("on").siblings().removeClass("on");
        footerAreaClick();
    });

    //openBox
    $(".eStore_openBox").click(function () {
        $(this).toggleClass("openBox").siblings().toggle("fast");
    });

    //table thHight
    $(".eStore_table_thHight tr td:nth-child(1),.eStore_table_thHight tr td:nth-child(2)").css("text-align", "left");
    $(".eStore_table_thHight tr:nth-child(odd)").addClass("odd");


    //第二層的li margin-top -8
    $(".eStore_listNumber li p").siblings("ol").css("margin-top", -5);
    $(".eStore_listNumber.fontBold").children("li").addClass("big");


    //純文字頁面links
    var linkTxt = $(".eStore_leftBlock_link a.on").text();
    $(".eStore_leftBlock_link span").html(linkTxt);
    $(".eStore_leftBlock_link").click(function () {
        $(this).toggleClass("highlight");
    });

    $(".eStore_index_banner .carouselBanner ul li").css("width", windowW);

    $(".eStore_index_banner .carouselBanner").each(function () {
        var id = $(this).attr('id');
        id = "#" + id;
        $(id).find("ul").carouFredSel({
            auto: true,
            prev: id + ' .prev',
            next: id + ' .next',
            pagination: id + ' .pager',
            scroll: 1000
        });
    });
    var pointW = $(".eStore_index_banner .carousel-controlCenter").width();
    $(".eStore_index_banner .carousel-controlCenter").css({
        "margin-left": -(pointW / 2)
    });

    $(window).resize(function () {

        windowW = $(window).outerWidth(true);

        //文字頁面的高度計算
        $(".eStore_txtPage_content .eStore_leftBlock").css({
            "min-height": 0
        });
        if (windowW > 767) { //767是RWD內設定的大小
            txtRightH();
        }

        //判斷logo顯示長短
        var logoPath = $(".eStore_logo a img").attr('data-logoPath');
        var slogoPath = $(".eStore_logo a img").attr('data-slogoPath');
        if (751 < windowW && windowW < 800 || windowW < 480) {
            $(".eStore_logo a img").attr("src", slogoPath);
        } else {
            $(".eStore_logo a img").attr("src", logoPath);
        }
        
        
        var pointW = $(".eStore_index_banner .carousel-controlCenter").width();
        $(".eStore_index_banner .carousel-controlCenter").css({
            "margin-left": -(pointW / 2)
        });

        $(".eStore_index_banner .carouselBanner ul li").css("width", windowW);
        $(".eStore_index_banner .carouselBanner").each(function () {
            var id = $(this).attr('id');
            id = "#" + id;
            $(id).find("ul").carouFredSel({
                auto: true,
                prev: id + ' .prev',
                next: id + ' .next',
                pagination: id + ' .pager',
                scroll: 1000
            });
        });

        //shoppingCart_title();

        var inputParentsW = $(".eStore_contactUs_input").outerWidth(true);
        var inputTitleW = $(".eStore_contactUs_input .title").outerWidth(true);
        if (windowW < 480) {
            $(".eStore_contactUs_input .bigSize").css({
                width: ""
            });
            return;
        } else {
            $(".eStore_contactUs_input .bigSize").css({
                width: inputParentsW - inputTitleW - 20
            });
        }

        //footer置底
        footerPosition();

    }).resize();

    var mouseStyle, i = 1;
    
    

    //手機平板menu animate
    $(".eStore_mobile img").click(function () {
        $(this).toggleClass("show").siblings().removeClass("show");
        var seeMore = $(".eStore_seeMore").hasClass("show");
        var chat = $(".eStore_mobile .eStore_chat").hasClass("show");
        var searchcss = $(".eStore_mobile .eStore_search").hasClass("show");
        var shoppingCart = $(".eStore_mobile .eStore_shoppingCart").hasClass("show");
        if (seeMore) {
            $(".eStore_wrapper,.eStore_footer").stop().animate({
                left: -251
            }, 500);
            $(".eStore_wrapper").css("overflow", "visible");
            hideMenuH();
        } else {
            $(".eStore_wrapper").css("overflow", "hidden");
            $(".eStore_wrapper,.eStore_footer").stop().animate({
                left: 0
            }, 500);
        }
        if (chat || searchcss || shoppingCart) {
            $(".eStore_mobileBox").css({
                "margin-bottom": 20,
                "border-bottom": "1px solid #ccc"
            });
        } else {
            $(".eStore_mobileBox").css({
                "margin-bottom": 0,
                "border-bottom": "none"
            });
        }
        if (chat) {
            $(".eStore_chatBox").slideDown();
        } else {
            $(".eStore_chatBox").slideUp();
        }
        if (searchcss) {
            $(".eStore_searchBox").slideDown();
        } else {
            $(".eStore_searchBox").slideUp();
        }
        if (shoppingCart) {
            $(".eStore_ShoppingBox").slideDown();
        } else {
            $(".eStore_ShoppingBox").slideUp();
        }


    });

    $(".eStore_chatBox").click(function () {
        var show = $(".eStore_chatBox").hasClass("show");
        if (!show) {
            $(".eStore_chatBox").stop().animate({
                right: 0
            });
            $(this).addClass("show");
        } else {
            $(".eStore_chatBox.show").stop().animate({
                right: -280
            });
            $(this).removeClass("show");
        }

    });

    //隱藏版的購物車內容 title高度比較
    var mobileShow2 = $(".eStore_mobile").is(':visible');
    if (!mobileShow2) {
        if (!$.cookie('isReturnCustomer') == true) {

            $(".eStore_chatBox").delay(10000).queue(function () {
                $.cookie('isReturnCustomer', true, { path: '/' });
                $(this).trigger("click").delay(5000).queue(function () {
                    $(this).trigger("click");
                    $(this).dequeue();
                }); ;
            });
        }
        else {
            setTimeout(function () {
                if ($(".eStore_chatBox").data("invitemeagain") == true) {
                    $(".eStore_chatBox").queue(function () {
                        $(this).trigger("click").delay(10000).queue(function () {
                            $(this).trigger("click");
                            $(this).dequeue();
                        });
                    });
                }
            }, 60000);
        }
    }
    $(".eStore_shoppingCart").click(function () {

        //shoppingCart_title();

    });

    $(".inquiryType select").change(function () {
        var txt2 = $(".inquiryType select option:first-child").text();
        var txt1 = $(".inquiryType select option:selected").text();
        console.log(txt2, txt1);
        if (txt1 != txt2) {
            $(".eStore_inputBlock").addClass("show");
        } else {
            $(".eStore_inputBlock").removeClass("show");
        }
    });
});

function showQQAPI()
{ window.open('http://b.qq.com/webc.htm?new=0&sid=8008100345&eid=218808P8z8p8Q8K8z8p80&o=http://buy.advantech.com.cn/&q=7&ref=' + document.location, '_blank', 'height=544, width=644,toolbar=no,scrollbars=no,menubar=no,status=no'); }

function showVideo() {
 
    $(".youtube").click(function () {
        $.fancybox({
            'padding': 0,
            'autoScale': false,
            'transitionIn': 'none',
            'transitionOut': 'none',
            'title': this.title,
            'width': 640,
            'height': 385,
            'href': this.href.replace(new RegExp("watch\\?v=", "i"), 'v/'),
            'type': 'swf',
            'swf': {
                'wmode': 'transparent',
                'allowfullscreen': 'true'
            }
        });

        return false;
    });
}

function CheckValidate(contextid) {
    var revaiObj = (contextid == null || contextid == undefined) ? $(".require") : $(contextid + " .require");
    var isValid = true;
    revaiObj.each(function () {
        if ($.trim($(this).val()) == "") {
            isValid = false;
            $(this).css({ "background": "#FFCECE" });
        }
        else $(this).css({ "background": "" });
    });

    if (isValid == false) {
        alert($.eStoreLocalizaion("can_not_be_empty"));
        return false;
    }
}
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

$(function () {
    if (typeof (chosenVariation) != "undefined") {
        if (chosenVariation != null && chosenVariation != undefined) {
            $.cookie('AbGroup', chosenVariation, { expires: 365, path: '/' });
        }
    }
    
})