function GetProductCategory(id, ppath) {
    xhrp = $.getJSON(GetStoreLocation() + 'api/Category/Products/' + id, { page: 1, pagesize: 1, SortType: "PriceLowest", filterType: ppath, MatrixPage: false, paps: true },
            function (data) {
                var newHtml = "";
                var newCount = 0;
                var hotHtml = "";
                var hotCount = 0;
                var feaHtml = "";
                var feaCount = 0;
                if (data != undefined && data != null) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].MarketingStatus != undefined && data[i].MarketingStatus != null) {
                            for (var j = 0; j < data[i].MarketingStatus.length; j++) {
                                switch (data[i].MarketingStatus[j]) {
                                    case (1):
                                        newCount++;
                                        newHtml += GetHtml(data[i], newCount, " newHide");
                                        break;
                                    case (2):
                                        hotCount++;
                                        hotHtml += GetHtml(data[i], hotCount, " htoHide");
                                        break;
                                    case (6):
                                        feaCount++;
                                        feaHtml += GetHtml(data[i], feaCount, " feaHide");
                                        break;
                                }
                            }
                        }
                    }
                    if (newCount > 6)
                        newHtml += "<div class=\"clearfix\"></div><a href=\"javascript:void(0)\" class=\"epaps_btn epaps_viewAll\" data-hide=\"newHide\" onclick=\"ShowAll(this);\">View All</a>";
                    if (hotCount > 6)
                        hotHtml += "<div class=\"clearfix\"></div><a href=\"javascript:void(0)\" class=\"epaps_btn epaps_viewAll\" data-hide=\"htoHide\" onclick=\"ShowAll(this);\">View All</a>";
                    if (feaCount > 6)
                        feaHtml += "<div class=\"clearfix\"></div><a href=\"javascript:void(0)\" class=\"epaps_btn epaps_viewAll\" data-hide=\"feaHide\" onclick=\"ShowAll(this);\">View All</a>";
                }
                $(".product_new").hide();
                $(".product_hot").hide();
                $(".product_feature").hide();

                if (newHtml != "") {
                    $("#product_new").html(newHtml);

                    $(".product_new").show();
                    fixTableLayout("#product_new", ".eStore_productBlock");
                }
                if (hotHtml != "") {
                    $("#product_hot").html(hotHtml);

                    $(".product_hot").show();
                    fixTableLayout("#product_hot", ".eStore_productBlock");
                }
                if (feaHtml != "") {
                    
                    $("#product_feature").html(feaHtml);

                    $(".product_feature").show();
                    fixTableLayout("#product_feature", ".eStore_productBlock");
                }
                $(".newHide").slideToggle();
                $(".htoHide").slideToggle();
                $(".feaHide").slideToggle();
            });
}

function GetHtml(data, i, css) {
    var html = "";
    var rangeclass = "";
    if (i > 6)
        rangeclass = css;
    html += "<div class=\"eStore_productBlock" + rangeclass + "\"><div class=\"eStore_productBlock_pic row10\">";
    html += "<a href=" + data.Url + "><img src=" + data.Image + " /></a></div>";
    html += "<div class=\"eStore_productBlock_txt row10\">";
    html += "<a class=\"eStore_productBlock_name\" href=" + data.Url + ">" + data.Name + "</a>";
    if (data.MarketingStatusIcons.length > 0) {
        html += "<span class=\"icon\">"
        for (var j = 0; j < data.MarketingStatusIcons.length; j++)
            html += data.MarketingStatusIcons[j];

        html += "</span>";
    }
 
    html += "<div class=\"eStore_productBlock_att\">" + data.Description + "</div>";
    html += "<ol class=\"eStore_listPoint\">" + data.Fetures + "</ol></div>";
    html += "<div class=\"eStore_productBlock_action\"><div class=\"eStore_productBlock_price row10\">" + data.Price + "</div></div></div>";
    return html;
}

function ShowAll(t) {
    var txt = $(t).text();
    var css = $(t).attr("data-hide");
    var val = "View All";
    if (txt == val) val = "Close";

    $(t).text(val);
    $("." + css).slideToggle();
    return false;
}

$(function () {
    var categoryid = $("#hcategory").val();
    var hash = window.location.hash;
    if (hash != "") {
        var hashId = hash.substring(1);
        if (hashId.toUpperCase() == "HOME") {
            $("a.category_linkBlock").each(function (i, n) {
                if ($(n).hasClass("haveList")) {
                    $(n).siblings('ul').slideDown();
                }
            });
            $(".product_new").hide();
            $(".product_hot").hide();
            $(".product_feature").hide();
        }
        else if (hashId.toUpperCase() == "PROMOTION") {

        }
        else
            GetProductCategory(categoryid, "");
    }
    else
        GetProductCategory(categoryid, "");

    $(".eStore_category_moblieLink span").click(function () {
        $(this).toggleClass("on").siblings("span").removeClass("on");

        if ($(this).hasClass("on")) {
            var thisClass = $(this).attr("data-class");
            $(thisClass).slideDown().siblings(".eStore_category_moblieBlock").slideUp();
        } else {
            $(".eStore_category_moblieBlock").slideUp();
        }
    });

    $(window).resize(function () {
        fixTableLayout("#product_new", ".eStore_productBlock");
        fixTableLayout("#product_hot", ".eStore_productBlock");
        fixTableLayout("#product_feature", ".eStore_productBlock");
    });
 
});