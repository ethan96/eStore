var HomeModel = function () {
    var self = this;

    //self.languages = ko.observableArray([]);
    //self.stores = ko.observableArray([]);
    self.banners = ko.observableArray([]);
    self.highlights = ko.observableArray([]);
    self.solutions = [];

    var xhr;
    self.loadhighlights = function (id) {
        if (xhr && xhr.readystate != 4) {
            xhr.abort();
        }
        xhr = $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/' + id,
            function (data) {
                self.highlights(data.TodaysHighlights);
            }
        );
    }
    self.hassolutions = ko.observable(false);
    self.loadsolutions = function () {
        $.getJSON(GetStoreLocation() + 'api/home/Solutions',
                function (data) {
                    self.solutions = data;
                    if (data.length > 0) {
                        self.hassolutions(true);
                        self.bindSolution(data[0].Id);
                    }
                }
            );
    }

    self.bindSolution = function (id) {
        if (HomeModel.solutions != undefined) {

            var data = $.map(self.solutions, function (n) {
                if (n.Id == id)
                    return n;

            });
            if (data != undefined && data.length > 0) {
                $(".eStore_index_solutionnormal a[ref='" + id + "']").addClass("on");
                $(".eStore_index_solutionnormal a[ref!='" + id + "']").removeClass("on");
                $(".eStore_index_Solution_contentBlock").css({
                    "background-image": "url(/resource" + data[0].Img + ")"
                });
                $(".eStore_index_Solution_Link").attr("href", data[0].Link);
                var temp = $("<div>");
                // apply "template" binding to div with specified data
                var name = "_tmpproductswithdesc";

                ko.applyBindingsToNode(temp[0], { template: { name: name, data: data[0].AssosociateItems} });
                // save inner html of temporary div
                var html = temp.html();
                $("#SolutionItmes ul").html(html).carouFredSel({
                    auto: false,
                    prev: '#SolutionItmes  .prev',
                    next: '#SolutionItmes .next',
                    pagination: '#SolutionItmes .pager',
                    scroll: 1000
                });
            }

        }
    }


};
// JavaScript Document
function tabHeight() {
    var indexContentH = $(".eStore_index_proContent.showContent").outerHeight();
    var tabH = ($(".eStore_index_Highlight_tabList").outerHeight()) + ($(".eStore_index_Highlight_tabBlock ul").outerHeight());
    var maxH = (indexContentH > tabH) ? indexContentH : tabH;
    $(".eStore_index_Highlight_tabBlock").css("height", maxH);
    $(".eStore_index_Highlight").css("height", maxH);

};
// JavaScript Document
$(function () {
    HomeModel = new HomeModel();
    ko.applyBindings(HomeModel);
    // Load initial data via Ajax
    //HomeModel.loadbanners();
    HomeModel.loadsolutions();

    //    $(".eStore_index_Highlight_tabList div a").click(function () {
    //        var id = $(this).attr("ref");
    //        HomeModel.loadhighlights(id);
    //        return false;
    //    });
    var eStore_index_Highlight = $(".eStore_index_HighlightContainer");
    var eStore_mobileTop = $("#eStore_mobileTop ul");
    $(eStore_index_Highlight).append($("<img />").prop("src", "/images/ajax-loader.gif"));
    $(eStore_mobileTop).append($("<img />").prop("src", "/images/ajax-loader.gif"));
    $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/',
            function (data) {
                var temp = $("<div>");

                var name = "_tmphighlight";

                ko.applyBindingsToNode(temp[0], { template: { name: name, data: data.TodaysHighlights} });
                // save inner html of temporary div
                var html = temp.html();
                $(eStore_index_Highlight).html(html);

                name = "_tmpmobilehighlight";

                ko.applyBindingsToNode($(eStore_mobileTop)[0], { template: { name: name, data: data} });
                // save inner html of temporary div
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
                $("#eStore_mobileTop").each(function () {
                    var id = $(this).attr('id');
                    id = "#" + id;
                    $(id).find("ul").carouFredSel({
                        auto: false,
                        prev: id + ' .prev',
                        next: id + ' .next',
                        pagination: id + ' .pager',
                        scroll: 1000
                    });
                });
                fixTableLayout(".eStore_index_HighlightContainer", ".eStore_productBlock");
                tabHeight();
            });

    var menuLeave, menuLeave2, menuEnter, mouseStyle, windowW, mobile, indexHighlight;

    $(".eStore_index_proContent").append("<div class='clearfix'></div>");


    $(".eStore_index_Solution_linkBlock a").click(function () {
        $(this).addClass("on").siblings().removeClass("on");
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

        var indexSolutionW = Math.floor($(".eStore_index_Solution_linkBlock").outerWidth());
        if (indexSolutionW > 606) {
            $(".eStore_index_Solution_contentBlock").css({
                "width": indexSolutionW - 220
            });
            var w = Math.floor(indexSolutionW - 220) - 40;
            var n = Math.floor(w / 173);
            if (n == 2) {
                $(".eStore_index_Solution_contentBlock li").css({
                    "width": (w / 2) - 30,
                    "padding": "0 15px"
                });
            } else if (n == 3) {
                $(".eStore_index_Solution_contentBlock li").css({
                    "width": (w / 3) - 30,
                    "padding": "0 15px"
                });
            }
        } else {

            $(".eStore_index_Solution_contentBlock").css({
                "width": "",
                "padding-left": ""
            });
            $(".eStore_index_Solution_contentBlock li").css({
                "width": "",
                "padding": ""
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
                pagination: id + ' .pager'
            });
        });


        mobile = $(".eStore_mobileTopBlock").is(':visible');
        indexHighlight = $(".eStore_index_Highlight_tabList a").hasClass("on");
        if (!mobile) {
            if (indexHighlight) {
                $(".eStore_index_Highlight_contentBlock.eStore_index_proContent").removeClass("showContent").hide();
            } else {
                $(".eStore_index_Highlight_contentBlock.eStore_index_proContent").addClass("showContent").show();
            }
        } else {
            $(".eStore_index_Highlight_contentBlock.eStore_index_proContent").removeClass("showContent").hide();
        }

        fixTableLayout(".eStore_index_Highlight_tabBlock .eStore_index_proContent:visible", ".eStore_productBlock");
        if (mobile) {
            $(".eStore_index_Highlight_tabBlock").css("height", "");
        } else {

            tabHeight();
        }

    }).resize();


    //** index_Highlight **
    $(".eStore_index_Highlight_tabBlock li").click(function () {
        var id = $(this).attr('dataName');
        $(id).show().siblings('div').hide();
        $(this).addClass("on").siblings("li").removeClass("on");
        $(".eStore_index_Highlight_tabList a").removeClass("on");
        if (mobile) {
            $(".eStore_index_proContent").removeClass("showContent").hide();
        } else {
            $(".eStore_index_proContent").removeClass("showContent").hide();
            $(".eStore_index_Highlight_contentBlock").addClass("showContent").show();

            //var url = document.location.host;
            //url = "http://" + url + "/Default.aspx#";
            //document.location.href = url;

            var url = document.location.host;

            var state = ({ url: document.location.href });
            window.history.pushState(state, "CategoryPath", "Default.aspx#");

            tabHeight();
        }

    });
    $(".eStore_index_Highlight_tabList a").click(function () {
        mobile = $(".eStore_mobileTopBlock").is(':visible');
        $(this).toggleClass("on").siblings(".eStore_index_Highlight_tabList a").removeClass("on");
        // $(".eStore_index_proContent").hide();
        if ($(this).has("on"))//load products here
        {
            var proContainer = $(".eStore_index_Highlight_tabList a.on").next(".eStore_index_proContent");
            var id = $(this).attr("ref");
            if ($(proContainer).prop("loaded") == undefined) {
                var categoryname = $(this).html();
                $(proContainer).append($("<img />").prop("src", "/images/ajax-loader.gif"));
                $.getJSON(GetStoreLocation() + 'api/home/TodaysHighlights/' + id,
                function (data) {
                    var temp = $("<div>");
                    // apply "template" binding to div with specified data
                    var name = "_tmpproducts";

                    ko.applyBindingsToNode(temp[0], { template: { name: name, data: data.TodaysHighlights} });
                    // save inner html of temporary div
                    var html = "<h5>" + categoryname + "</h5>" + temp.html();
                    $(proContainer).html(html);
                    fixTableLayout(".eStore_index_proContent:visible", ".eStore_productBlock");
                    tabHeight();
                });
            }

        }

        if (mobile) {
            $(".eStore_index_Highlight_tabList a").next(".eStore_index_proContent").removeClass("showContent").hide()
            $(".eStore_index_Highlight_tabList a.on").next(".eStore_index_proContent").show().addClass("showContent");
        } else
        {
            $(".eStore_index_Highlight_tabList a").next(".eStore_index_proContent").removeClass("showContent").hide()
            $(".eStore_index_Highlight_tabList a.on").next(".eStore_index_proContent").show().addClass("showContent");
            indexHighlight = $(".eStore_index_Highlight_tabList a").hasClass("on");
            if (!mobile) {
                if (indexHighlight) {
                    $(".eStore_index_Highlight_contentBlock.eStore_index_proContent").removeClass("showContent").hide();

                } else {
                    $(".eStore_index_Highlight_contentBlock.eStore_index_proContent").addClass("showContent").show();
                }
            }

            var url = document.location.host;
            var indexHighlight;
            var CategoryID = $("#CategoryID_text").val();
            var TabName = $("#TabName_text").val();

            indexHighlight = $("div.eStore_index_Highlight_contentBlock:first").hasClass("showContent");


            var url = document.location.host;

            var state = ({ url: document.location.href });
            

            if(indexHighlight)
            {
                window.history.pushState(state, "CategoryPath", "Default.aspx#");
            }
            else
            {
                window.history.pushState(state, "CategoryPath", "Default.aspx#Categoryid=" + $(this).attr("data-cid") + "&Tab=" + $(this).attr("data-TabName") + "");
            }

            //document.location.href = url;

            tabHeight();
        }
        return false;
    });

    $(".eStore_index_solutionnormal a").click(function () {
        if (!$(this).hasClass("more")) {
            HomeModel.bindSolution($(this).attr("ref"));
            return false;
        }
    });
});