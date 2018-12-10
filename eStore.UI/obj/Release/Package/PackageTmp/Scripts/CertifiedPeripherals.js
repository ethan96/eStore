// JavaScript Document
var myInterval, myInterval2, countN = 0, animateSpotlightsN, pointN, pointOn;

function preClick() {
    moveBanner(-1);
}

function nextClick() {
    moveBanner(1);
}

function pointClick($this) {

    var onPointIndex = $(".epaps-onPoint").index();
    var clickPointIndex = $("div.epaps-point").index($this);
    var comparePoint = clickPointIndex - onPointIndex;

    var id = $this.attr('id');
    var position = $(".epaps-indexBannerBlock[data-ID='#" + id + "']").index();
    if (comparePoint > 0) {
        position = position;
    } else {
        var indexBannerNumber = $(".epaps-indexBannerBlock").length;
        position = -(indexBannerNumber - position);
    }
    moveBanner(position);
}

/**
 * 移動banner到指定位置 @param integer $position 移動左右方向第幾張 
 **/
function moveBanner(position) {

    if ($(".epaps-indexBannerRow").is(':animated')) {
        return false;
    }

    clearInterval(myInterval);
    if (position > 0) { //nextClick
        $(".epaps-indexBannerRow").animate({
            "margin-left": (-980 * position)
        }, {
            duration: 500,
            complete: function () {

                var abs = Math.abs(position);
                for (i = 0; i < abs; i++) {
                    $(".epaps-indexBannerRow").append($(".epaps-indexBannerBlock:first-child"));
                }

                $(".epaps-indexBannerRow").css("margin-left", "0");

                $(".epaps-point").removeClass("epaps-onPoint");
                var dataID = $(".epaps-indexBannerBlock:first-child").attr("data-ID");
                $(dataID).addClass("epaps-onPoint");

                autoNextClick();
            }
        });
    } else { //preClick
        var abs = Math.abs(position);
        for (i = 0 ; i < abs ; i++) {
            $(".epaps-indexBannerRow").prepend($(".epaps-indexBannerBlock:last"));
        }

        $(".epaps-indexBannerRow").css("margin-left", abs * (-980)).animate({
            "margin-left": 0
        }, {
            duration: 500,
            complete: function () {
                $(".epaps-point").removeClass("epaps-onPoint");
                var dataID = $(".epaps-indexBannerBlock:first-child").attr("data-ID");
                $(dataID).addClass("epaps-onPoint");

                autoNextClick();
            }
        });
    }
}

function autoNextClick() {
    myInterval = setInterval("nextClick()", 5000);
}

//3.0**************************************
function spotlightsPreClick() {
    spotlightsMoveBanner(-animateSpotlightsN);
}

function spotlightsNextClick() {
    spotlightsMoveBanner(animateSpotlightsN);
}

function spotlightsMoveBanner(position) {

    if ($(".epaps-landingCol-Spotlights .epaps-productRow ul").is(':animated')) {
        return false;
    }
    var liheight = $(".epaps-landingCol-Spotlights ul li").innerHeight();

    var pointSelect = position / animateSpotlightsN;
    countN = countN + pointSelect;

    $(".epaps-landingCol-Spotlights .carousel-control .pager span").removeClass("selected");

    clearInterval(myInterval2);
    if (position > 0) { //nextClick

        if (countN < pointN) {
            $(".epaps-landingCol-Spotlights .carousel-control .pager span").eq(countN).addClass("selected");
        } else {
            $(".epaps-landingCol-Spotlights .carousel-control .pager span").eq(0).addClass("selected");
            countN = 0;
        }
        var abs = Math.abs(position);
        var n = 0;
        for (i = 0; i < abs; i++) {
            $(".epaps-landingCol-Spotlights ul li:eq(" + n + ")").clone().appendTo($(".epaps-landingCol-Spotlights ul"));
            n = n + 1;
        }

        $(".epaps-landingCol-Spotlights ul").animate({
            "margin-top": (-liheight) * (position)
        }, {
            duration: 400,
            complete: function () {
                for (j = 0; j < abs; j++) {
                    $(".epaps-landingCol-Spotlights ul li:first-child").remove();
                }
                $(".epaps-landingCol-Spotlights ul").css("margin-top", "0");

                //spotlightsAutoNextClick();
            }
        });
    } else { //preClick
        //var abs = Math.abs(countN);
        if (countN > -pointN) {
            $(".epaps-landingCol-Spotlights .carousel-control .pager span").eq(countN).addClass("selected");
        } else {
            $(".epaps-landingCol-Spotlights .carousel-control .pager span").eq(0).addClass("selected");
            countN = 0;
        }
        var abs = Math.abs(position);
        for (i = 0 ; i < abs ; i++) {
            $(".epaps-landingCol-Spotlights ul").prepend($(".epaps-landingCol-Spotlights ul li:last"));
        }

        $(".epaps-landingCol-Spotlights ul").css("margin-top", (abs * (-liheight))).animate({
            "margin-top": 0
        }, {
            duration: 400,
            complete: function () {

                //spotlightsAutoNextClick();
            }
        });
    }
}

/*function spotlightsAutoNextClick() {
	myInterval2 = setInterval("spotlightsNextClick()", 4000);
}*/

function spotlightsPager() {
    var spotlightsLiNumber;
    spotlightsLiNumber = $(".epaps-landingCol-Spotlights li").length;
    if ($(".epaps-landingCol-Spotlights").hasClass("lessCol")) {
        animateSpotlightsN = 6;
        pointN = Math.ceil(spotlightsLiNumber / 6);
    } else {
        animateSpotlightsN = 4;
        pointN = Math.ceil(spotlightsLiNumber / 4);
    }
    for (i = 0 ; i < pointN ; i++) {
        $(".epaps-landingCol-Spotlights .carousel-control .pager").prepend("<span></span>");
    }
    $(".epaps-landingCol-Spotlights .carousel-control .pager span").eq(0).addClass("selected");

}
// ** pagination **
var pagesize = 8;
function paginationproducts(panelseletor, itemsseletor, isreset) {
    var totalitems = $(itemsseletor).length;

    if (totalitems > pagesize) {
        $(panelseletor).find(".simple-pagination").pagination({
            items: totalitems,
            itemsOnPage: pagesize,
            cssStyle: 'epaps-page'
            , onPageClick: function (pageNumber, event) { return showpaginationproducts(itemsseletor, pageNumber); }
            , onInit: function () { return showpaginationproducts(itemsseletor, 1); }

        });
        $(panelseletor).find(".epaps-page").show();
    }
    else {
        $(panelseletor).find(".epaps-page").hide();
    }
    if (isreset) {
        $(itemsseletor).siblings().hide();
        //$(panelseletor).find(".simple-pagination").pagination('destroy');
        if ($(panelseletor).find(".simple-pagination").data("pagination") != undefined) {
            $(panelseletor).find(".simple-pagination").pagination('redraw');
        }

        showpaginationproducts(itemsseletor, 1)
    }
}

function showpaginationproducts(itemsseletor, page) {
    var items = $(itemsseletor);
    var from = (page - 1) * pagesize;
    var to = page * pagesize - 1;
    $(itemsseletor).show();
    $(itemsseletor + ":lt(" + from + ")").hide();
    $(itemsseletor + ":gt(" + to + ")").hide();
    return false;
}
$(function () {

    // ** epaps-indexBanner 輪播 **
    var indexBannerNumber = ($(".epaps-indexBannerBlock").length) * 980;
    $(".epaps-indexBannerRow").css("width", indexBannerNumber);

    //往後
    $(".epaps-indexBanner-Next").click(function () {
        nextClick();
    });

    //往前
    $(".epaps-indexBanner-Pre").click(function () {
        preClick();
    });

    //點選point
    $(".epaps-point").click(function () {
        if ($(this).hasClass('epaps-onPoint')) {
            return false;
        }
        pointClick($(this));
    });

    //自動輪播
    autoNextClick();

    //滑入時停止
    $(".epaps-indexBannerBlock")
	.mouseenter(function () {
	    clearInterval(myInterval);
	})
	.mouseleave(function () {
	    autoNextClick();
	});



    // ** epaps-landingCol 計算有幾個 兩欄時拉寬 **
    var landingCol = $(".epaps-landingCol").length;
    if (landingCol == 2) {
        $(".epaps-landingCol-Spotlights").addClass("lessCol");
    }
    $(".epaps-landingCol-Video li a").prepend("<span></span>");

    // 計算幾個point
    spotlightsPager();

    //往後
    $(".epaps-spotlights-Next").click(function () {
        spotlightsNextClick();
    });

    //往前
    $(".epaps-spotlights-Pre").click(function () {
        spotlightsPreClick();
    });

    //點選point
    $(".epaps-landingCol-Spotlights .pager span").click(function () {
        if ($(this).hasClass('selected')) {
            return false;
        }
        var now = $(".epaps-landingCol-Spotlights .pager span.selected").index();
        var clickOn = $(this).index();
        pointOn = (clickOn - now) * animateSpotlightsN;

        spotlightsMoveBanner(pointOn);

    });

    //自動輪播
    //spotlightsAutoNextClick();

    //滑入時停止
    /*$(".epaps-landingCol-Spotlights .epaps-productRow")
    .mouseenter(function() {
    clearInterval(myInterval2);
    })
    .mouseleave(function() {
    spotlightsAutoNextClick();
    });*/


    // ** 產品輪播 **
    $(".epaps-carousel").each(function () {

        var id = $(this).attr('id');

        $(this).find("ul.productlist").carouFredSel({
            auto: false,
            prev: '#' + id + ' .epaps-arrow1',
            next: '#' + id + ' .epaps-arrow2',
            pagination: '#' + id + ' .pager',
            scroll: {
                duration: 500,
                timeoutDuration: 5000,
                easing: 'easeOutSine',
                pauseOnHover: 'immediate'
            }
        });
    });


    $(".epaps-productRow2").each(function () {
        var id = $(this).attr('id');
        var itemsseletor = '#' + id + ' ul.productlist>li';
        paginationproducts('#' + id, itemsseletor, false);
    });
    String.prototype.endsWith = function (suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };
    // ** video 上面加上play按鈕
    $(".epaps-productCol-video li a").each(function (i, n) {
        var href = $(n).attr("href");
        if (href != null && href.toLowerCase().endsWith(".pdf")) {
        }
        else {
            $(n).prepend("<span></span>");
        }
    });

    $(".epaps-productCol-video li").hover(
	function () {
	    $(this).siblings("li").animate({
	        "opacity": 0.4
	    }, 200);
	}, function () {
	    $(this).siblings("li").animate({
	        "opacity": 1
	    }, 200);
	});



    // ** 3.3 3.4 
    //產品頁 上面小圖點擊
    $(".epaps-listImg img").mousemove(function () {
        var src = $(this).attr("src");
        $(".epaps-mainImg img").attr('src', src);
        $(".epaps-listImg img").removeClass("onShowe");
        $(this).addClass("onShowe");
    });
    //viewAll
    if ($(".epaps-ProductSpec tr").length > 12) {
        $(".epaps-ProductSpec tr:nth-child(n+12)").addClass("epaps-hiddenBlock");
        $(".epaps-ViewAll").click(function () {
            $(this).siblings(".epaps-ProductSpec").find(".epaps-hiddenBlock").slideToggle("fast");
            $(this).toggleClass("lessNow");
            $(".epaps-ViewAll").text("View Complete Specification");
            $(".epaps-ViewAll.lessNow").text("View Less");
        });
    }
    else {
        $(".epaps-ViewAll").hide();
    }
    //選擇產品後
    $(".epaps-productRow-bgBlue .epaps-productCol").click(function () {
        $(this).toggleClass("SelectThis");
    });

    // ** 3.3 epaps-productRow-bgBlue 最後一個增加距離 **
    $(".epaps-productRow-bgBlue:last-child").css("margin-bottom", "35px");
    //    //針對價錢跟數量增加文字
    //    $(".epaps-productCol-right .epaps-productprice").append("<span>Price</span>");
    //    $(".epaps-productQty").append("<span>Qty</span>");


    // ** 3.4 mouseEnter show **
    $(".epaps-popproductdetails").mouseenter(function () {
        $(this).bind("mousemove", function (event) {
            $(".epaps-productCol-hidden").empty();
            $(".epaps-productCol-hidden").html(
          ' <div class="epaps-productCol-hiddenTop"><div class="epaps-productCol-hiddenImg"><img src="' + $(this).find("img").attr("src")
          + '" width="166" height="166" /></div><div class="epaps-productCol-hiddenMsg"><span>' + $(this).next(".epaps-productContent").find("a").text()
          + '</span>' + $(this).next(".epaps-productContent").find(".epaps-productTxt").html()
          + '</div></div>' + $(this).next(".epaps-productContent").find(".epaps-feature").html()
            );
            $(".epaps-productCol-hidden").css({
                display: "block",
                left: event.pageX + 5,
                top: event.pageY + 5
            });
        });
    })
	  .mouseleave(function () {
	      $(".epaps-productCol-hidden").css({
	          display: "none"
	      });
	  });


    // ** 5.0 search 頁面 +號計算 **
    $(".epaps-SolutionCol-itemProduct:nth-child(4n+1)").css({
        "padding-left": 0,
        "background": "none"
    })

    $(".epaps-SolutionCol-block").each(function (index, block) {
        if ($(block).find(".epaps-SolutionCol-item").length < 4) {
            $(block).siblings(".epaps-ViewMore").hide();
        }
    });
    //點擊後動作
    $(".epaps-SolutionCol-item:nth-child(n+4)").addClass("epaps-hiddenBlock");
    $(".epaps-ViewMore").click(function () {
        $(this).siblings(".epaps-SolutionCol-block").find(".epaps-hiddenBlock").slideToggle("fast");
        $(this).toggleClass("lessNow");
        $(".epaps-ViewMore").text("View More");
        $(".epaps-ViewMore.lessNow").text("View Less");
    });


    // ** 6.0 Add one item click
    $(".epaps-AddItem").click(function () {
        $(".epaps-msgProduct:last").after("<table width='100%' border='0' cellspacing='0' cellpadding='0' class='epaps-table epaps-msgProduct'>" +
		  "<tr>" +
			"<th width='120'>Product Description*</th>" +
			"<th width='110'>Manufacturer</th>" +
			"<th width='180'>Manufacturer Part Number</th>" +
			"<th>Spec Sheet</th>" +
			"<th width='50'>Qty</th>" +
		  "</tr>" +
		  "<tr>" +
			"<td><input type='text'></td>" +
			"<td><input type='text'></td>" +
			"<td><input type='text'></td>" +
			"<td><a href=''>Choose</a>XXXXXX-XXXXXXXXX</td>" +
			"<td><select>" +
			"<option value='1'>1</option>" +
			"<option value='2'>2</option>" +
			"<option value='3'>3</option>" +
			"</select></td>" +
		 "</tr>" +
		 "<tr>" +
			 "<th colspan='5'>Remark</th>" +
		   "</tr>" +
		   "<tr>" +
			 "<td colspan='5'><input type='text' ></td>" +
		   "</tr>" +
		 "</table>"
		);
    });


    //    // ** tab **
    //    $(".epaps-tabs li").click(function () {
    //        var id = $(this).attr('data-ID');
    //        if ($(id).css("display") == 'none') {
    //            $(".epaps-tabs li").removeClass("epaps-tabOn");
    //            $(this).addClass("epaps-tabOn");
    //            $(".epaps-tabTxt").hide();
    //            $(id).fadeIn();
    //        }
    //    });


    // ** 針對輪播有底色 a增加分隔線 **
    //    $(".epaps-productRow-bgBlue .epaps-title_line a").after("<span>|</span>");


    // ** 增加三種緞帶 **
    //    $(".epaps-productHot .epaps-productImg,.epaps-productBest .epaps-productImg,.epaps-productNew .epaps-productImg,.epaps-productBestDeal .epaps-productImg").prepend("<span></span>");


    // ** 內頁banner文字置中 **
    var bannerTxtHeight = $(".epaps-bannerBg-txt").outerHeight(true);
    bannerTxtHeight = -(bannerTxtHeight / 2);
    $(".epaps-bannerBg-txt").css("margin-top", bannerTxtHeight);


    // ** compare 變色 **
    //	$("li").bind('click', 'input', function(){
    //		var checked = $(this).prop("checked");
    //		if(checked){
    //			$(this).closest('li').addClass("checkbox-checked");
    //		} else {
    //			$(this).closest('li').removeClass("checkbox-checked");
    //		}
    //	});


    /**
    * Compare
    **/


    // ** 清除float **
    $("ul.epaps-listChild,.epaps-title-bgGray-borderLeft,.epaps-title_bgGray,.epaps-productRow,.epaps-productRow2 ul,.epaps-productprice,.epaps-bannerTop,.epaps-bannerTopRight,.epaps-rowBottom,.epaps-logoRow,.epaps-row780,.epaps-title_line,.epaps-tabs ul,.epaps-searchRow,.epaps-SolutionCol-itemTop,.epaps-SolutionCol-itemBottom,.epaps-ServiceSteps,.epaps-comfirmedCheckbox,.epaps-productCol-msg,.epaps-productCol-video,.epaps-productQty,.epaps-productInfo,.epaps-productCol-hiddenTop").append("<div class='clearfix'></div>")


    /***************************************** 
    lightBox start 
    *****************************************/
    $(".epaps-tableSelect-bottm td").eq(1).css("width", "188PX");
    // ** compare 變色 **
    $("table.epaps-tableSelect-bottm").bind('click', 'input', function () {
        var checked = $(this).prop("checked");
        if (checked) {
            $(this).closest('tr').addClass("checkbox-checked");
        } else {
            $(this).closest('tr').removeClass("checkbox-checked");
        }
    });

    /***************************************** 
    左邊連結判斷語法 start 
    *****************************************/

    // ** 針對第二層之後的 links ul 計算寬度 **
    $(".epaps-morelink").each(function () {
        //debugger;
        var navlistN = $(this).next("ul").find("li").length;
        if (navlistN < 3) {
            var navlistW = navlistN * 181;
            $(this).next("ul").css("width", navlistW + "px");
        } else {
            $(this).next("ul").css("width", "543px");
        }
    });
    // 針對第二層之後的 links ul 計算寬度 且如果還有第三層 寬度絕對為一欄且border-right移除
    $(".epaps-listChild").parents(".epaps-listChild").css("width", "180px");
    $(".epaps-listChild").parents(".epaps-listChild").find("li").css("border-right", "");

    // 滑入後顯示子層	
    $(".epaps-listMenu li").hover(function () {
        $(this).children(".epaps-listChild").show();
        $(this).parent(".epaps-listChild").siblings(".epaps-morelink").addClass("epaps-onSelect");
    }, function () {
        $(this).children(".epaps-listChild").hide();
        $(this).parent(".epaps-listChild").siblings(".epaps-morelink").removeClass("epaps-onSelect");
    });
    //** spec filter **
    $(".epaps-openBox").click(function () {
        $(this).toggleClass("openBox").siblings().toggle();
    });

    $(".epaps-specfilter input").click(function () {
        specfilter(this);
    });
});

function specfilter(sender) {
    //[2] 將各大分類下每個被勾選的小分類的[產品代碼]集行聯集
    var $area = $('.epaps-specfilter');

    //[1] Checkbox 
    //1.1確認現在被點擊的checkbox
    $('.this-be-click').removeClass('this-be-click');
    //1.1 若傳入是null, 表可能剛做完reset所以沒有checkbox被點擊
    if ($(sender) != null) {
        $(sender).addClass('this-be-click');
    }
    var filterObject = {};                                       //紀錄所有要顯示的[大分類名稱]和[產品代碼]

    $.each($area.children('.epaps-selectRow'), function (index, object) {
        var $title = $(object).find('.epaps-selectTitle').text(), //[大分類名稱]
                unionArray = [];                                     //存放聯集後的[產品代碼]

        var InputCheckeds = $(object).find('input:checked');     //取得這個大分類下有被勾選到的checkbox

        if (InputCheckeds.length > 0) {                          //如果這個大分類有至少一個checkbox被勾選

            InputCheckeds.each(function () {

                var newArray = $(this).attr('products').split(','); //取得被勾選的checkbox(小分類)下的[產品代碼]
                unionArray = _.union(newArray, unionArray);      //將這個區塊被勾選的所有產品代碼進行聯集

            });
        }
        filterObject[$title] = unionArray;                       //儲存這個大分類下所有聯集後的[產品代碼]
    });

    //[3] 將各大分類聯集後的[產品代碼]，進行大分類之間的交集，最後產物為Filter後的[產品代碼]

    var _result = $.map($(".productlist li[productid]"), function (n) {
        return $(n).attr("productid");
    });             //取得所有的[產品代碼]

    $.each(filterObject, function (index, thisArray) {
        if (thisArray.length > 0) {                              //如果這個大分類下有至少一個[產品代碼]
            _result = _.intersection(thisArray, _result);        //將每個大分類的[產品代碼]進行交集
        }
    });

    //[4] 重新整理要顯示的checkbox(小分類)名稱，並更新括號中的[產品代碼]數量

    $.each($area.children('.epaps-selectRow'), function (index, object) {
        var inputCheckeds = $(object).find('input:checked');

        //當 (1) 這個大分類沒有任何checkbox(小分類)被勾選時
        //或 (2) 這個大分類並非正在點擊區塊
        if (inputCheckeds.length == 0 || !$(object).find('input').hasClass('this-be-click')) {

            $(object).find('input').each(function () {
                var newArray = $(this).attr('products').split(',');

                var myCount = _.intersection(_result, newArray).length;

                $(this).siblings(".matchedcount").text(myCount);


                if (myCount) {                                     //如果這個小分類目前[產品代碼] 數量"不為 0"
                    $(this).attr('disabled', false);
                    $(this).parent().show();

                } else {                                            //如果這個小分類目前[產品代碼] 數量"為 0"
                    if ($(this).attr('checked')) {
                        $(this).attr('disabled', true);                 // 仍被勾選的checkbox,顯示為 disabled
                    } else {
                        $(this).parent().hide();                                 // 沒被勾選的checkbox 直接hide
                    }

                }
            });
        }
    }); 
    $(".productlist li[productid]").each(function (index, item) {
        if (_.contains(_result, $(item).attr("productid"))) {
            $(item).addClass("matcheditem");
        }
        else {
            $(item).removeClass("matcheditem");
        }
    });
    paginationproducts(".epaps-row780", ".productlist li.matcheditem", true);

    //[6] 重新整理 要顯示的 Filter Tags (小分類) (SelectedBox)

    var checkedList = {};

    //[6.1]先將要顯示的小分類名稱找出來，並存入checkedList。
    $.each($area.children(".epaps-selectRow"), function (index, object) {

        var $title = $(object).find('.epaps-selectTitle').text();
        var selectedItemArray = [];
        $(object).find('input:checked').each(function () {
            var myText = $(this).siblings(".AttributeValueName").text();
            selectedItemArray.push({ Name: myText,Checkbox:this });
        });

        checkedList[$title] = selectedItemArray; //以[大分類名稱]為key，存放[小分類名稱]Array
    });
    //[6.2] 製作 html Tag
    //結構為:
    //   <大分類span class="caps-selectboxTxt" > 
    //      <小分類span> 小分類 1 <img [x] click/> </小分類span>
    //      <小分類span> 小分類 2 <img [x] click/> </小分類span>
    //   </大分類span> 
    //   <span> Reset </span>
    var $box = $('.epaps-SelectedBox');
    $box.empty();

    $.each(checkedList, function (key, object) {

        if (object.length > 0) {

            var $outerSpan = $('<span class="epaps-selectboxTxt"></span>');

            $.each(object, function (i, value) {
                var $img = $('<img src="/images/epaps-selectbox-X.png" width="10" height="10"/>');

                $img.click(function () {
                    $(value.Checkbox).attr("checked", false);
                    specfilter(value.Checkbox);
                });
                var $innerSpan= $('<span></span>').html(value.Name).append($img);
                $outerSpan.append($innerSpan);
            });

            $box.append($outerSpan);
        }
    });

    if ($box.children().length != 0) {  //有 Filter Tag 的時候才加 Reset Tag

        var $resetSpan = $('<span></span>').addClass('epaps-selectboxTxt reset').html('<a>Reset</a>');

        $resetSpan.click(function () {
          $area.find('input').attr('checked', false);
          specfilter(null);
        });

        $box.append($resetSpan);
    }
 $box.append($("<div class='clearfix'></div>"));
 
}
function checkcomparisionitems(checkbox) {
    $checkeditems = $(checkbox).closest(".epaps-comparecontainer").find(":checked");
    if ($checkeditems.length > 4) {
        alert("You can only compare products upto 4 at once.");
        $(checkbox).attr("checked", "");
    }
    else if ($checkeditems.length > 0) {
        $(".epaps-comparedisabled").removeClass("disabled");
        updateCurrentSelections($checkeditems);
    }
    else {
        $(".epaps-comparedisabled").addClass("disabled");
        updateCurrentSelections($checkeditems);
    }

}
function updateCurrentSelections($checkeditems) {

    if ($(".caps-CurrentSelections").length > 0) {
        
        $(".caps-CurrentSelections").find(".CurrentSelectionsCount").text($checkeditems.length);
        $(".caps-CurrentSelections").find(".caps-content_add").empty();
        $.each($checkeditems, function (index, item) {
            $(".caps-CurrentSelections").find(".caps-content_add").append(
            ($("<li/>").text($(item).val())).append($("<img src='/images/title_delete.jpg'/>").bind("click", function () {
                $checkeditem = $(".epaps-comparecontainer").find(":checkbox[value='" + $(item).val() + "']");
                $checkeditem.prop("checked", "");
                checkcomparisionitems($checkeditem);
            }))
            );
        });
    }
}
$(document).ready(function () {

    $(".epaps-comparecontainer :checkbox").click(function () {
        checkcomparisionitems(this);
    });
    $(".epaps-comparecontainer .epaps-comparelink").click(function () {
        $checkeditems = $(this).closest(".epaps-comparecontainer").find(":checked");
        if ($checkeditems.length == 0) {
            alert("Please select products to compare.");
        }
        else {
            window.open('/CertifiedPeripherals/Compare.aspx?' + $.param($checkeditems));

        }
        return false;
    });
    $(".epaps-comparedisabled").click(function () {
        $checkeditems = $(this).closest(".epaps-comparecontainer").find(":checked");
        $.each($checkeditems, function (i, n) {
            $(n).attr("checked", "");
        });
        $(".epaps-comparedisabled").addClass("disabled");
        updateCurrentSelections([]);
        return false;
    });
});