var HomeModel = function () {
    var self = this;
    self.banners = ko.observableArray([]);
    self.highlights = ko.observableArray([]);
 


};
 
// JavaScript Document
$(function () {
    HomeModel = new HomeModel();
    ko.applyBindings(HomeModel);
    // Load initial data via Ajax
    //HomeModel.loadbanners();
 

    var eStore_index_Highlight = $(".eStore_index_HighlightContainer");
    var eStore_mobileTop = $("#eStore_mobileTop ul");
    $(eStore_index_Highlight).append($("<img />").prop("src", "/images/ajax-loader.gif"));
    $(eStore_mobileTop).append($("<img />").prop("src", "/images/ajax-loader.gif"));

    var menuLeave, menuLeave2, menuEnter, mouseStyle, windowW, mobile, indexHighlight;

    $(".eStore_index_proContent").append("<div class='clearfix'></div>");

    $(".carouselBannerSingle").each(function () {
        var id = $(this).attr('id');
        id = "#" + id;
        //console.log(id);
        $(id).find("ul").carouFredSel({
            auto: false,
            scroll: 1,
            prev: id + ' .prev',
            next: id + ' .next',
            pagination: id + ' .pager',
            items: {
                start: "random"
            }
        });
    });
    $(window).resize(function () {
        windowW = $(window).outerWidth(true);

        var mobileTopBlockW = $(".eStore_mobileTopBlock").width() - 40;
        if (mobileTopBlockW < 380) {
            $(".eStore_mobileTopBlock li").css({
                width: Math.floor(mobileTopBlockW)
            });
        } else {
            $(".eStore_mobileTopBlock li").css({
                width: Math.floor(mobileTopBlockW / 2)
            });
        }


        $(".carouselBannerSingle").each(function () {
            var id = $(this).attr('id');
            id = "#" + id;
            //console.log(id);
            $(id).find("ul").carouFredSel({
                auto: false,
                scroll: 1,
                prev: id + ' .prev',
                next: id + ' .next',
                pagination: id + ' .pager',
                items: {
                    start: "random"
                }
            });
        });
    }).resize();
 
});