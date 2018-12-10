// JavaScript Document

function adjustContentHeight() {
    timeOut = setTimeout(function () {
        compareHeight();
    }, 300);
    timeOut2 = setTimeout(function () {
        scrollingHeight();
    }, 400);
}

function addanchorforspecialcategories() {
    if (typeof (specialcategories) != "undefined") {
        var strHref = location.href;
        var intPos = strHref.indexOf("#");
        if (intPos > 0) {
            strHref = strHref.substring(0, intPos);
        }
        for (i = 0; i < specialcategories.length; i++) {
            if ($("#module-" + specialcategories[i]).length > 0) {
                $("#anchorforspecialcategories").append(
                            $("<a>")
                            .attr("href", strHref + "#module-" + specialcategories[i])
                            .html($("#module-" + specialcategories[i] + " .moduleheader .ctosCategory").html())).append($("<br />"));
            }
        }
    }
}

function setMaxfloatbtospanelHeight() {
    $("#floatbtospanel,.master-wrapper-side,.master-wrapper-center").css("height", "auto")
    $("#floatbtospanel").height($("#floatbtos").innerHeight());
    $(".master-wrapper-center").height($(".master-wrapper-cph").innerHeight());
    $("#floatbtospanel,.master-wrapper-side,.master-wrapper-center").equalHeight();
    //$('#floatbtos').scrollFollow("relocation");
}
function expandParentCategory(category) {
    if (category.length > 0) {
        $(category).show();
        $(category).children(".moduleheader").removeClass("coloptionimg");
        $(category).children(".moduleheader").addClass("expoptionimg");
        expandParentCategory($(category).parent(".extendedmodule"));
    }
}

function addanchorforspecialcategories() {
    if (typeof (specialcategories) != "undefined") {
        var strHref = location.href;
        var intPos = strHref.indexOf("#");
        if (intPos > 0) {
            strHref = strHref.substring(0, intPos);
        }
        for (i = 0; i < specialcategories.length; i++) {
            if ($("#module-" + specialcategories[i]).length > 0) {
                $("#anchorforspecialcategories").append(
                            $("<a>")
                            .attr("href", strHref + "#module-" + specialcategories[i])
                            .html($("#module-" + specialcategories[i] + " .moduleheader .ctosCategory").html())).append($("<br />"));
            }
        }
    }
}

function swithPanel(event) {
    
//    if (checkOSHardDisk() == false)
//        return false;
    if (preventswithToNextPanel)
        return false;

    if (event.data.id == null)
        return false;
    var $navigator = $("#ConfigureSystemNavigator");
    var $activeTab = $navigator.children("li[ref='" + event.data.id + "']");
    if ($activeTab.length > 0) {

        $navigator.data("activepanel", event.data.id);
        $navigator.data("action", event.data.action);

        $("#configurationsystem>div").hide();
        $("#" + event.data.id).show();


        if ($activeTab.prevAll("li[ref]").length == 0) {
            //first
            $(".productliteratures").show();
            $("a[actiongroup]").show();
            $("a.swithpanelback").hide();
            $("a[actiongroup]").each(function (i, n) {
                $(n).find("span").html($(n).attr("firststep"));
                $(n).bind("click", { id: $activeTab.next().attr("ref"), action: $(n).attr("actiongroup") }, swithPanel);
            });
            //$navigator.data("action", event.data.action);

        }
        else if ($activeTab.nextAll("li[ref]").length == 0) {
            //last
            $(".productliteratures").hide();
            $("a[actiongroup='" + event.data.action + "']").siblings("a[actiongroup]").hide();
            $("a.swithpanelback").show();
            $("a[actiongroup]").each(function (i, n) {
                $(n).find("span").html($(n).attr("laststep"));
                $(n).unbind("click", swithPanel);
            });
        }
        else {
            // middle
            $(".productliteratures").hide();
            $("a[actiongroup='" + event.data.action + "']").siblings("a[actiongroup]").hide();
            $("a.swithpanelback").show();
            $("a[actiongroup]").each(function (i, n) {
                $(n).find("span").html($(n).attr("middlestep"));
                $(n).bind("click", { id: $activeTab.next().attr("ref"), action: $(n).attr("actiongroup") }, swithPanel);
            });

            //$navigator.data("action", event.data.action);
        }
        if ($navigator.data("action") != null) {
            $navigator.find("li[ref] ").removeClass("disable");
            //$navigator.find("li[ref] span").css("cursor", "pointer");
            $activeTab.siblings().removeClass("active");
            $activeTab.addClass("active");
        }

        adjustContentHeight()
    }
    return false;
}

function loadPageVar(sVar) {
    try {
        return decodeURI(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURI(sVar).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
    }
    catch (error) {
        return "";
    }
}

function checkResoruce() {
    var resourceSettings = new Array();
    var overcapacity = new Array();
    $("#configurationsystem").find(":checked[resource],:selected[resource]").each(function (i) {
        $(eval($(this).attr("resource"))).each(function (newindex, newitem) {
            var exists = false;
            $(resourceSettings).each(function (index, item) {
                if (newitem.ResourceName == item.ResourceName) {
                    item.AvaiableQty += newitem.AvaiableQty;
                    item.ConsumingQty += newitem.ConsumingQty;
                    exists = true;
                    return;
                }
            });
            if (!exists) {
                resourceSettings.push(newitem);
            }
        });
    });

    $("#configurationsystem").find(":text[resource][value!='']").each(function (i) {
        if (parseInt($(this).val()) != NaN && parseInt($(this).val()) > 0) {
            var qty = parseInt($(this).val());
            $(eval($(this).attr("resource"))).each(function (newindex, newitem) {
                var exists = false;
                $(resourceSettings).each(function (index, item) {
                    if (newitem.ResourceName == item.ResourceName) {
                        item.AvaiableQty += newitem.AvaiableQty * qty;
                        item.ConsumingQty += newitem.ConsumingQty * qty;
                        exists = true;
                        return;
                    }
                });
                if (!exists) {
                    newitem.AvaiableQty = newitem.AvaiableQty * qty;
                    newitem.ConsumingQty = newitem.ConsumingQty * qty;
                    resourceSettings.push(newitem);
                }
            });
        }
    });

    resourceSettings = $.map(resourceSettings, function (n) {
        if (n.AvaiableQty >= n.ConsumingQty) {
            n.AvaiableQty = n.AvaiableQty - n.ConsumingQty;
            n.ConsumingQty = 0;
        }
        else {
            n.AvaiableQty = 0;
            n.ConsumingQty = n.ConsumingQty - n.AvaiableQty;
        }
        return n;
    });

    var MultiConsumingResource = $.grep(resourceSettings, function (n, i) {
        return n.ConsumingQty > 0;
    });
    $(MultiConsumingResource).each(function (ci, cn) {
        var result = false;
        $(resourceSettings).each(function (ai, an) {
            $(cn.ResourceName.split(",")).each(function (i, r) {
                if (jQuery.inArray(r, an.ResourceName.split(",")) >= 0 && an.AvaiableQty >= cn.ConsumingQty) {
                    an.AvaiableQty -= cn.ConsumingQty;
                    cn.ConsumingQty = 0;
                    return;
                }
                else if (jQuery.inArray(r, an.ResourceName.split(",")) >= 0 && an.AvaiableQty > 0 && an.AvaiableQty < cn.ConsumingQty) {

                    cn.ConsumingQty -= an.AvaiableQty;
                    an.AvaiableQty = 0;
                }
            });
        });
        if (cn.ConsumingQty > 0) {
            $.merge(overcapacity, cn.ResourceName.split(","));
        }
    });

    $($.grep(resourceSettings, function (n, i) {
        return n.AvaiableQty < 0 && n.ResourceName.indexOf(",") < 0
    })).each(function (i, n) {
        overcapacity.push(n.ResourceName);
    });

    if (overcapacity.length > 0) {
        alert("The selection you made exceeds system's [" + $.unique(overcapacity).join(",") + "] maximum capacity.");
        return false;
    }
    else
        return true;

}

function checkQuoteOnly()
{
    if ($("#configurationsystem input:checked[data-quoteonly='true']").length > 0) {
        $("#eStoreMainContent_btnAdd2CartFloat,#eStoreMainContent_btnAdd2CartTop").hide();
        $("#eStoreMainContent_btnAdd2QuoteFloat,#eStoreMainContent_btnAdd2QuoteTop").removeClass("borderBlue");

    }
    else {
        $("#eStoreMainContent_btnAdd2CartFloat,#eStoreMainContent_btnAdd2CartTop").show();
        $("#eStoreMainContent_btnAdd2QuoteFloat,#eStoreMainContent_btnAdd2QuoteTop").addClass("borderBlue");
    }
}
var confirmednoOS = false;
var confirmedinstallationinstruction = false;
function SystemIntegrityCheck() {
    preventswithToNextPanel = false;
    var noneedOS = true;
    $(".module[integritychecktype='OS']").each(function (i, os) {
        if (getOrderedQty($(os)) > 0)
            noneedOS = false;
    });
    var StorageCnt = 0;
    $(".module[integritychecktype='Storage']").each(function (i, storage) {
        StorageCnt += getOrderedQty($(storage));
    });

    if (!noneedOS && StorageCnt > 1) {
        if ($("#txtComment").val() == "" || $("#txtComment").val() == $("#txtComment").attr("title")) {
            alert($.eStoreLocalizaion("please_indicate_which_device_you_would_like_the_OS_installed"));
            $("#txtComment").val($.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive")).css("color", "#000");
            $("#txtComment").focus();
            preventswithToNextPanel = true;
            return false;
        }
        else {
            if (confirmedinstallationinstruction) {
                return true;
            }
            else {
                if (confirm($.eStoreLocalizaion("Did_you_tell_us_where_to_install_the_OS_in_the_installation_instruction"))) {
                    confirmedinstallationinstruction = true;
                    return true;
                }
                else {

                    $("#txtComment").focus();
                    preventswithToNextPanel = true;
                    return false;
                }
            }
        }
    }
    else if (noneedOS && StorageCnt > 1) {
        if (!confirmednoOS) {
            if (confirm($.eStoreLocalizaion("Are_you_sure_you_dont_want_to_select_any_OS"))) {
                confirmednoOS = true;
                return true;
            }
            else {
                if ($("div[integritychecktype='OS']").length > 0) {
                    window.location.hash = $("div[integritychecktype='OS']")[0].id;
                    preventswithToNextPanel = true;
                }
                return false;
            }

        }
        else {
            return true;
        }
    }
    else if (!noneedOS && StorageCnt == 0) {
        alert($.eStoreLocalizaion("Are_you_sure_you_dont_want_any_storage_device_with_your_OS"));
        if ($("div[integritychecktype='Storage']").length > 0) {
            window.location.hash = $("div[integritychecktype='Storage']")[0].id;
            preventswithToNextPanel = true;
        }
        return false;
    }
    else if (!noneedOS && StorageCnt == 1) {
        if ($("#txtComment").val() == $.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive")) {
            $("#txtComment").val($("#txtComment").attr("title"));
        }
    }
}

function getOrderedQty(module) {
    var selectedcount = 0;
    if (module != null && module.length > 0)
        $(module).find("input:checked").each(function (i, option) {
            if ($(option).prop("type") == "checkbox") {
                if (!isNaN(parseInt($(option).siblings("input[type='text']").val()))) {
                    selectedcount += parseInt($(option).siblings("input[type='text']").val());
                }
            }
            else {
                if ($(option).attr("nonoption") == "true") {

                }
                else {
                    selectedcount++;
                }
            }
        });
    $(module).find("input:selected").each(function (i, option) {

    });
    return selectedcount;

}

function insertCheckBoxBtosInfor(obj, ctoModuleID, isOnlyOne) {

    var objModel = obj.parent().parent().parent();
    var selectitemprice = 0;

    if (isOnlyOne) {
        if (obj.attr("type") == "select-one")
            selectitemprice = parseFloat(obj.find("option:selected").attr("addtion"));
        else
            selectitemprice = parseFloat(obj.attr("addtion"));
        var optiondesc = obj.siblings(".optiondesc").html();
        //set header desc
        objModel.find(".moduleheader .ctosSelectItem").html(optiondesc);
    }
    else { // get all select bots then show them
        var inContextStr = "";
        var inContext = new Array();
        objModel.find(".options").each(function () {
            var ct = $(this).find("input:checkbox");
            if (ct.prop('checked') == true) {
                inContext.push($(this).find(".optiondesc").html());
                var cp = $(this).find("input:checkbox");
                var cpint = parseFloat(cp.attr("addtion"));
                selectitemprice = selectitemprice + cpint;
            }
        });
        if (inContext.length > 0) {
            inContextStr = inContext.join(" | ");
        }
        if (inContextStr != "") {

            //set header desc
            objModel.find(".moduleheader .ctosSelectItem").html(inContextStr);
        }
        else { // select None

            objModel.find(".moduleheader .ctosSelectItem").html("None");
        }
    }
    if (selectitemprice == 0) {
        $("#floatbtos #" + ctoModuleID + " .btosSelectItem").removeClass("btosSelectItemChanged");
    }
    else {
        $("#floatbtos #" + ctoModuleID + " .btosSelectItem").addClass("btosSelectItemChanged");
    }

    if (ctoModuleID != "btos-2139" && ctoModuleID != "btos-21") {
        if (obj.attr("type") == "select-one") // input type is select
        {
            obj.find("option").each(function () {
                var sum = (parseFloat($(this).attr("addtion")) - selectitemprice);
                var sumSign = "";
                if (sum >= 0) {
                    sumSign = "+";
                }
                else if (sum < 0) {
                    sumSign = "-";
                }
                $(this).text($(this).attr("optiondesc").replace("$.'", "\"") + " [" + sumSign + $(this).attr("currency") + formatRound(Math.abs(sum), _unit) + "]");
            });
        }
        else if (obj.attr("type") != "checkbox" && obj.attr("type") != "text") // type is radio
        {
            objModel.find(".options").each(function () {
                var sum = (parseFloat($(this).find("input:radio").attr("addtion")) - selectitemprice);
                var sumSign = "";
                if (sum >= 0) {
                    sumSign = "+";
                }
                else if (sum < 0) {
                    sumSign = "-";
                }
                $(this).find(".priceSing").html(sumSign);
                $(this).find(".addtionprice").html(formatRound(Math.abs(sum), _unit));
            });
        }
    }
}

function getBTOSId() {
    selectoption = new Array();
    $("#configurationsystem input:checked").each(function () {
        selectoption.push(this.name + ":" + this.value);
    });
    $("#configurationsystem input:text").each(function () {
        if (this.value != "" && this.value != "0")
            selectoption.push(this.name + ":" + this.value);
    });
    $("#configurationsystem select").each(function () {
        var pc = $(this).find("option:selected");
        if (pc.val() != "" && pc.val() != "0")
            selectoption.push(pc.parent().attr("name") + ":" + pc.val());
    });
    return selectoption.join(";");
}
function getAddons() {
    selectoption = new Array();
    $("#configurationsystem .AddonCategory input:text").each(function () {
        if (this.value != "" && this.value != "0")
            selectoption.push(this.name + ":" + this.value);
    });
    return selectoption.join(";");
}
function getBTOSId2() {
    var rlt = new Array();
    var selectmodule;
    var selectoption;
    $("#configurationsystem .module").each(function () {
        selectmodule = new Array();
        selectoption = new Array();
        selectQTY = new Array();
        $(this).find("input:checked").each(function () {
            selectoption.push(this.value);
            if ($(this).attr("type") != "checkbox") {
                selectQTY.push(1);
            }
            else {
                selectQTY.push($(this).parent().find("input:text").val());
            }
        });
        var selectoptionStr = "";
        for (var c = 0; c < selectoption.length; c++) {
            if (c == 0) {
                selectoptionStr += selectoption[c] + "|" + selectQTY[c];
            }
            else {
                selectoptionStr += "," + selectoption[c] + "|" + selectQTY[c];
            }
        }
        selectmodule.push(this.id.toString().replace("module-", "") + ":" + selectoptionStr);
        rlt.push(selectmodule.toString());
    });
    return rlt.join(";");
}
function setNoneItemStatusForMulti(component) {
    if ($(component).closest(".options").find(".optiondesc").text() == "None") {
        if ($(component).closest(".options").find(":checked").length > 0) {
            $(component).closest(".options").siblings().each(function () {
                $(this).find(":checkbox").removeAttr("checked");
                $(this).find(":text").val("0");
            });
        }
        return;
    }
    var noneItem = null;
    var hasoptions = false;
    $(component).closest(".module").find(".options").each(function () {
        if ($(this).find(".optiondesc").text() == "None") {
            noneItem = this;
        }
        else {
            if ($(this).find(":checked").length > 0) {
                hasoptions = true;
            }
        }
    });
    if (noneItem != null) {
        if (hasoptions) {
            $(noneItem).find(":checkbox").removeAttr("checked");
            $(noneItem).find(":text").val("0");
        }
        else {
            $(noneItem).find(":checkbox").attr("checked", "checked");
            $(noneItem).find(":text").val("1");
        }
    }
}

function Preview() {
    $("#inline1").empty();
    //最外层
    $.each($("#ConfigureSystemNavigator li"), function () {
        var content = "";
        var id = "#" + $(this).attr("ref");
        //第二层
        $.each($(this).children(".moduleheader").children("span").eq(1) != null && $("#configurationsystem").children(id).children(".module"), function () {
            //这段if逻辑解释：如果moduleheader下的选项为None或则moduleheader下没勾选任何数据，则不显示
            if (($(this).children(".moduleheader").children("span").eq(1).length > 0 && $(this).children(".moduleheader").children("span").eq(1).text().toUpperCase() != "NONE") ||
                ($(this).children(".moduleheader").children("span").eq(1).length == 0 && $(this).hasClass("hasValue"))) {
                content += "<div class='eStore_openBox_type'>" + $(this).children(".moduleheader").children("span").eq(0).text() + "</div><ul>";
                if ($(this).children(".moduleheader").children("span").eq(1).text() != "")
                    content += "<li>" + $(this).children(".moduleheader").children("span").eq(1).text() + "</li></ul>";
            }
            //第三层
            $.each($(this).children(".extendedmodule"), function () {
                var count = 0;
                //判断title下是否有数据，如果没有，则不显示title（title下为checkbox的情况）
                if ($(this).children(".extendedmodule").length > 0) {
                    $.each($(this).children(".extendedmodule").children(".options").children(".optionstext"), function () {
                        if ($(this).children(":checkbox").prop("checked")) {
                            count += 1;
                        }
                    });
                    if (count > 0)
                        content += "<li class = 'title'>" + $(this).children(".moduleheader").children("span").text() + "</li>";
                }
                else {
                    count = 0;
                    $.each($(this).children(".options").children(".optionstext"), function () {
                        if ($(this).children(":checkbox").prop("checked")) {
                            count += 1;
                        }
                    });
                    if (count > 0)
                        content += "<li class = 'title'>" + $(this).children(".moduleheader").children("span").text() + "</li>";
                }

                $.each($(this).children(".options").children(".optionstext"), function () {
                    if ($(this).children(":checkbox").prop("checked")) {
                        content += "<li>" + $(this).children(".optiondesc").text() + "</li>";
                    }
                });

                $.each($(this).children(".extendedmodule"), function () {
                    var count = 0;
                    var self = $(this);
                    $.each($(this).children(".options").children(".optionstext"), function () {
                        if ($(this).children(":checkbox").prop("checked")) {
                            count += 1;
                            if (count == 1)
                                content += "<ul><li class = 'title'>" + self.children(".moduleheader").children("span").text() + "</li>";
                            content += "<li>" + $(this).children(".optiondesc").text() + "</li>";
                        }
                    });
                    if (count > 0)
                        content += "</ul>";
                });
            });
             
        });
        if (content.length > 0) {
            content = "<div class='eStore_block'><div class='eStore_openBox_title eStore_openBox'>" + $(this).children("span").text() + "</div>"
            + " <div class='eStore_openBox_previw'>" 
            + content;
            content += "</div></div>";

        $("#inline1").append(content);
        }
});
$("#inline1").find(".eStore_openBox").click(function () {
    $(this).toggleClass("openBox").siblings().toggle("fast");
    timeOut = setTimeout(function () {
        compareHeight();
    }, 300);
    timeOut2 = setTimeout(function () {
        scrollingHeight();
    }, 400);
});
}

$(function () {

    addanchorforspecialcategories();
    $(".eStore_breadcrumb a:last").css("background-image", "none");
    $.each($("#module-extended_header :checked"), function (i, checkeditem) {
        $(checkeditem).parent().parent().find(".options").show();
        $(checkeditem).parent().parent().parent().find(".extendedmodule").show();
        expandParentCategory($(checkeditem).closest(".extendedmodule"));
    });
    $(".eStore_chatBox").data("invitemeagain", true);
    $(".SystemIntegrityCheck").bind("click", checkOSHardDisk)

    if ($("#ConfigureSystemNavigator li").length > 1) {
        var source = loadPageVar("source");
        var action = null;
        if (source == "Quotation") {
            action = "AddtoQuotation";
        }
        else if (source == "Cart") {
            action = "AddtoCart";
        }

        swithPanel({ data: { id: $("#ConfigureSystemNavigator li[ref]:eq(0)").attr("ref"), action: action} });
    }
    else
        adjustContentHeight()

    //resourcesBlock
    $(".resourcesBlock span").append("|");
    $(".resourcesBlock span a").css("margin-right", 8);

    var id = $(".eStore_product_productID input").val();
    var catContainer = $("#mostcategory");
    $.getJSON(GetStoreLocation() + 'api/category/AssociatedProductCategories/' + id,
                function (data) {
                    if (data != null) {
                        var temp = $("<div>");
                        // apply "template" binding to div with specified data
                        var name = "_tmpcategories";

                        ko.applyBindingsToNode(temp[0], { template: { name: name, data: data} });
                        // save inner html of temporary div
                        $(temp).find("h4").text($.eStoreLocalizaion("Check_More_Information_for_Your_Project"));
                        var html = temp.html();
                        $(catContainer).html(html);
                    }
                    else
                        $(catContainer).html("");
                });

    $(".eStore_4Step_title a.disable,.eStore_4Step_title a.on").click(function () {
        return false;
    });
    $(".eStore_4Step_title a:first-child").css({
        "margin-left": 0,
        "padding-left": 10,
        "background": "none"
    });



    //***scrolling

    $(window).bind('scroll load', function () {

        scrollH = $(this).scrollTop();
        scrollLeft = $(this).scrollLeft();
        scrollingHeight();

    });



    $(".moduleheader").click(function () {

        //  debugger;
        if ($(this).attr("class").match("expoptionimg") == null) {
            $(this).removeClass("coloptionimg");
            $(this).addClass("expoptionimg");
            if ($(this).hasClass("extendedmoduleheader")) {
                $(this).parent().children(".options").show();
                $(this).parent().children(".extendedmodule").show();
            }
            else {
                $(this).parent().find(".options").show();
                $(this).parent().find(".moduleheader").removeClass("coloptionimg");
                $(this).parent().find(".moduleheader").addClass("expoptionimg");
            }

        }
        else {
            $(this).addClass("coloptionimg");
            $(this).removeClass("expoptionimg");

            if ($(this).hasClass("extendedmoduleheader")) {
                $(this).parent().children(".options").hide();
                $(this).parent().children(".extendedmodule").hide();
            }
            else {
                $(this).parent().find(".options").hide();
                $(this).parent().find(".moduleheader").addClass("coloptionimg");
                $(this).parent().find(".moduleheader").removeClass("expoptionimg");
            }
        }
        adjustContentHeight()
    });

    $("#configurationsystem .options input:radio").click(function () {
        selectItem($(this), true);
        checkResoruce();
        checkQuoteOnly();

    });
    $("#configurationsystem .options input:checkbox").click(function () {
        if ($(this).prop('checked') == false) {
            $(this).parent().find("input:text").val(0);
        }
        else {
            $(this).parent().parent().parent().parent().addClass("hasValue");
            var cc = $(this).parent().find("input:text").val();
            if (cc <= 0)
                $(this).parent().find("input:text").val(1);
        }
        setNoneItemStatusForMulti(this);
        selectItem($(this), false);
        checkResoruce();
        checkQuoteOnly();

    });
    $("#configurationsystem .options select").change(function () {
        setNoneItemStatusForMulti(this);
        selectItem($(this), true);
        checkResoruce();
        checkQuoteOnly();
    });


    $("#configurationsystem .options input:text").keyup(function () {
        var inputQTY = $(this).val();
        if ($.trim(inputQTY) == "") {
            $(this).parent().find("input:checkbox").prop("checked", false);
            $(this).val(0);
        }
        else if (inputQTY > 0) {
            $(this).parent().find("input:checkbox").prop("checked", true);
        }
        else {
            $(this).parent().find("input:checkbox").prop("checked", false);
        }
        setNoneItemStatusForMulti(this);
        selectItem($(this), false);
        checkResoruce();
    });
  
    $("#ConfigureSystemNavigator li[ref]").click(function () {
        var $navigator = $("#ConfigureSystemNavigator");
        if ($navigator.data("action") != null) {

            swithPanel({ data: { id: $(this).attr("ref"), action: $navigator.data("action")} });
        }
    }
        );

    $("a.swithpanelback").click(function () {
        var $navigator = $("#ConfigureSystemNavigator");
        var $activeTab = $navigator.children("li[ref='" + $navigator.data("activepanel") + "']");
        swithPanel({ data: { id: $activeTab.prev().prev().attr("ref"), action: $navigator.data("action")} });
        return false;
    });
    $(".systemBomPreview").click(function () {
        Preview();
        popupDialog("#inline1");
        return false;
    });


    $("#eStore_ResourcesTab").after("<div class='clearfix'></div>");
    $("#eStore_ResourcesTab li").click(function () {
        $(this).addClass("on").siblings("li").removeClass("on");
        showResource();
    });

    $(".carouselBannerSingle").each(function () {
        var id = $(this).attr('id');
        id = "#" + id;
        //console.log(id);
        $(id).find("ul").carouFredSel({
            auto: false,
            scroll: 1,
            prev: id + ' #prev',
            next: id + ' #next',
            pagination: id + ' #pager'
        });
    });

    //**4 step	
    //compareHeight();

    StepTop = $(".positionFixed").offset().top;

    showResource();
    function showResource() {
        $(".eStore_titleTab li[labfor]").each(function (i, n) {
            if ($(n).hasClass("on")) {
                $("#RTab-" + $(n).attr("labfor")).show();
            }
            else {
                $("#RTab-" + $(n).attr("labfor")).hide();
            }
        });
    }

    $(".eStore_openBox").click(function () {
        $(this).toggleClass("openBox").siblings().toggle("fast");
        timeOut = setTimeout(function () {
            compareHeight();
        }, 300);
        timeOut2 = setTimeout(function () {
            scrollingHeight();
        }, 400);
    });

    if (typeof defaultOptions != 'undefined' && defaultOptions != null) {
        if (defaultOptions.length > 0)
        {
            var tmpOption = $("#configurationsystem .options  input:radio[data-component='" + defaultOptions[0] + "']");
            if (tmpOption != null && tmpOption.length > 0)
                tmpOption.click();
        }
        checkQuoteOnly();
    }
});