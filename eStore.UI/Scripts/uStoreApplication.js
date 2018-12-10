(function ($) {
    $.renderApplication = function (container, path) {
        var container = $(container);
        var theme;
        switch (path.toLowerCase()) {
            case "hotel":
                theme = "orange";
                break;
            case "restaurant":
                theme = "green";
                break;
            case "retail":
                theme = "yellow";
                break;
            default:
                theme = "orange";
                break;
        }
        container.addClass(theme);

        eStore.UI.eStoreScripts.getApplication(path, function (app) {
            if (app != null) {
                CreateDemoArea(app);
                CreateTabs(app.Scenarios);
                $("#categoriestabs").append($("<div id=\"blueLine\"></div>"));
                var categoryid;
                var scenarioid;
                var scenario;
                if (window.location.hash) {
                    scenarioid = window.location.hash.replace("#", "");
                    scenario = $.grep(app.Scenarios, function (n, i) {
                        return n.ID == scenarioid;
                    });
                }
                if (scenario != null && scenario.length > 0) {
                    var activespots = getScenarioActiveSpots(scenario[0]);
                    showTip(activespots);
                    if (scenario[0].Categories.length > 0) {
                        categoryid = scenario[0].Categories[0].Path;
                        scenarioid = scenario[0].Path;
                        activeCategory(scenarioid, categoryid, false);
                    }
                } else if (app.Scenarios != undefined && app.Scenarios[0].Categories != undefined) {
                    categoryid = app.Scenarios[0].Categories[0].Path;
                    scenarioid = app.Scenarios[0].Path;
                    activeCategory(scenarioid, categoryid, false);
                }
            }
        });

        function CreateDemoArea(app) {
            var demoarea = $("#demoarea");
            $(demoarea).css("width", app.BackgroundImageWidth + "px")
            .css("height", app.BackgroundImageHeight + "px")
            .css("background-image", "url(" + app.BackgroundImage + ")")
            .append($("<div id='prodTitle' />").html(app.Name));

            CreateSpots(demoarea, app);
            CreateScenarios(demoarea, app);
        }

        function getRelatedscenarios(Scenarios, spotID) {
            var relatedscenarios = new Array();
            $.each(Scenarios, function (i, s) {
                if (jQuery.inArray(spotID, s.Spots) >= 0) {
                    relatedscenarios.push(s);
                }
                var subScenarios = getRelatedscenarios(s.SubScenarios, spotID);
                $.each(subScenarios, function (j, ss) {
                    relatedscenarios.push(ss);
                });
            });
            return relatedscenarios;
        }

        function CreateSpots(demoarea, app) {

            $.each(app.Spots, function (i, spot) {
                var relatedscenarios = getRelatedscenarios(app.Scenarios, spot.ID);
                if (relatedscenarios != null && relatedscenarios.length > 0) {
                    var newspot = $('<div id="d' + spot.ID + '" class="dot" tag="' + spot.ID + '"></div>').css("top", spot.Top + "px").css("left", spot.Left + "px");
                    $(demoarea).append(newspot);
                    animateDot(newspot, 1);
                    var spotcontent = $("<div/>");
                    var maxTextWidth = 0;
                    $.each(relatedscenarios, function (rsi, rs) {
                        var textwidth = rs.Name.width(); //getStringWidth();
                        if (textwidth > maxTextWidth)
                            maxTextWidth = textwidth;
                        if (rsi > 0) {
                            $(spotcontent).append($("<br />"));
                        }

                        spotcontent.append($("<a/>").attr("href", "#").html(rs.Name).click(function () {
                            activeCategory(rs.Path, null, true);
                            return false;
                        }));
                    });

                    $(newspot).sBubble({
                        id: spot.ID,
                        position: spot.MessagePosition,
                        content: spotcontent,
                        height: 12 * relatedscenarios.length + 3,
                        width: maxTextWidth + 10,
                        topOffset: spot.MessagePosition == "top" ? 0 : -25,
                        leftOffset: spot.MessagePosition == "top" ? 0 : 5
                    });

                    //每一個單獨的點
                    $(newspot).hover(function () {
                        id = $(this).attr('tag');
                        hideAllTip(id);
                        $('#b' + id).addClass('visible');
                        $('#b' + id).css('display', 'block');
                        $('#ba' + id).addClass('visible');
                        $('#ba' + id).css('display', 'block');
                    });

                }
            });
        }
        function CreateScenarios(demoarea, app) {
            var scenairos = $("<ul/>").attr("id", "scenairoslist");
            $.each(app.Scenarios, function (i, scenario) {
                var activespots = getScenarioActiveSpots(scenario);
                $(scenairos).append($("<li/>").html(scenario.Name).hover(function () {
                    $(this).addClass("hover");
                    showTip(activespots);
                }, function () {
                    $(this).removeClass("hover");
                })
                .click(function () {
                    activeCategory(scenario.Path, null, true);
                    return false;
                })
                );
            });
            ($("<div id='appScenarios' />")
            .append($("<div id='apptitle' />").html(app.Name)).append($(scenairos).append($("<div id='bom' />")))).appendTo($(demoarea));

        }

        function getScenarioActiveSpots(scenario) {
            var activespots = new Array();
            $.each(scenario.Spots, function (i, spot) {
                activespots.push(spot);
            });
            $.each(scenario.SubScenarios, function (i, ss) {
                var subScenarioSpots = getScenarioActiveSpots(ss);
                $.each(subScenarioSpots, function (j, ssspot) {
                    activespots.push(ssspot);
                });
            });
            return activespots;
        }

        function CreateTabs(Scenarios) {
            var categoriestabs = $("#categoriestabs");
            $.each(Scenarios, function (i, scenario) {
                var tab = $("<ul/>").attr("id", scenario.Path).hide();
                $.each(scenario.Categories, function (j, category) {
                    $(tab).append($("<li/>")
                    .attr("id", category.Path)
                    .html(category.Name)
                    .click(function () {
                        activeCategory(scenario.Path, category.Path, true);
                        return false;
                    }));
                });
                if (scenario.Categories.length > 2) {
                    $(tab).find("li")
                    .css("font-size", "12px")
                    .css("font-weight", "bold");
                }
                $(categoriestabs).append(tab);
                CreateTabs(scenario.SubScenarios);
            });
        }

        function activeCategory(scenarioid, categoryid, scrollToProduct) {
            $("#categoriestabs ul").each(function (i, c) {
                if ($(c).attr("id") == scenarioid) {
                    $(c).show();
                    if ($(c).data("init") == undefined) {
                        var tabstotalwidth = 0;
                        var padding = 0;
                        var tabscount = 0;
                        $(c).find("li").each(function (j, n) {
                            tabscount++;
                            tabstotalwidth += $(n).outerWidth();
                        });
                        padding = (780 - tabstotalwidth - 4 * tabscount) / tabscount;
                        $(c).find("li").each(function (j, n) {
                            $(n).css("width", ($(n).outerWidth() + padding) + "px");
                        });
                        $(c).data("init", true);
                    }
                    if (categoryid == null && $("#categoriestabs").data("PreviousCategory") != undefined) {
                        categoryid = $("#categoriestabs").data("PreviousCategory");
                    }
                    if (categoryid == null || $(c).find("li[id='" + categoryid + "']").length == 0) {
                        categoryid = $(c).find("li:first").attr("id");
                    }

                    $(c).find("li").each(function (j, s) {
                        if ($(s).attr("id") == categoryid) {
                            $(s).addClass("select");
                            $("#categoriestabs").data("PreviousCategory", categoryid)
                            loadproducts(scenarioid, categoryid, scrollToProduct, 1);

                        }
                        else {
                            $(s).removeClass("select");
                        }
                    });
                }
                else {
                    $(c).hide();
                }
            });

        }
        function loadproducts(scenarioid, categoryid, scrollToProduct, page) {
            var query = $.param({ type: "ScenarioProducts", ScenarioID: scenarioid, CategoryID: categoryid, Page: page });
            $.ajax({
                type: "GET",
                url: GetStoreLocation() + "proc/html.aspx",
                dataType: "html",
                data: query,
                success: function (data, textStatus) {
                    $("#products").html(data);
                    if (typeof (popuploginnormal) != "undefined") {
                        $("#products").find(".needlogin").click(function () {
                            $("#loginTrigger").val($(this).attr("id"));
                            return popuploginnormal($(this).text());
                        });
                    }
                    if (scrollToProduct)
                        scrollToElement("#categoriestabs");
                    //page
                    var ipage = 1;
                    $("#products div.rightside a").click(function () {
                        try {
                            var eqindex = $(this).attr("href").lastIndexOf("=");

                            ipage = parseInt($(this).attr("href").substring(eqindex + 1, $(this).attr("href").length));

                        }
                        catch (err) {
                            ipage = 1;
                        }
                        if (!isNaN(ipage)) {
                            loadproducts(scenarioid, categoryid, scrollToProduct, ipage);
                            return false;
                        }
                    });
                }
            });
        }


        function showSingleTip(id) {
            $('#b' + id).addClass('visible');
            $('#b' + id).css('display', 'block');
            $('#ba' + id).addClass('visible');
            $('#ba' + id).css('display', 'block');
        }

        function hideSingleTip(id) {
            $('#b' + id).removeClass('visible');
            $('#b' + id).css('display', 'none');
            $('#ba' + id).removeClass('visible');
            $('#ba' + id).css('display', 'none');
        }

        function hideAllTip(id) {
            $(container).find(".dot").each(function (i, dot) {
                var dotid = $(dot).attr("tag");
                if (id != dotid) {
                    hideSingleTip(dotid);
                }
            });
        }

        function showTip(tmpArr) {
            tmpArr.sort();
            //console.log(tmpArr);
            j = 0;
            $(container).find(".dot").each(function (i, dot) {
                var dotid = parseInt($(dot).attr("tag"));
                if (jQuery.inArray(dotid, tmpArr) == -1)
                { hideSingleTip(dotid); }
                else
                { showSingleTip(dotid); }

            });
        }

        function scrollToElement(selector, time, verticalOffset) {
            time = typeof (time) != 'undefined' ? time : 300;
            verticalOffset = typeof (verticalOffset) != 'undefined' ? verticalOffset : 0;
            element = $(selector);
            offset = element.offset();
            offsetTop = offset.top + verticalOffset;
            $('html, body').stop().animate({
                scrollTop: offsetTop
            }, time);
        }


        function animateDot(spot, func) {
            if (func == 1) {
                $(spot).animate({ top: '+=3' }, 500, function () {
                    //$(spot).css('background-image', 'url(/Images/dot_1.png)');
                    animateDot(spot, 0);
                });
            }
            if (func == 0) {
                $(spot).animate({ top: '-=3' }, 500, function () {
                    //$(spot).css('background-image', 'url(/Images/dot_2.png)');
                    animateDot(spot, 1);
                });
            }
        }
    }
})(jQuery);


function getStringWidth(text) {
    var sensor = $('<span style="display: inline;"/>').css({ margin: 0, padding: 0 }).text($.trim(text));
    $("body").append(sensor);
    var width = sensor.width();
    sensor.remove();
    return width;
}
String.prototype.width = function (font) {
    var f = font || '12px arial',
      o = $('<div>' + this + '</div>')
            .css({ 'position': 'absolute', 'float': 'left', 'white-space': 'nowrap', 'visibility': 'hidden', 'font': f })
            .appendTo($('body')),
      w = o.width();

    o.remove();

    return w;
}