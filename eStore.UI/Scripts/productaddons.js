(function ($) {
    var productaddonsdata = null;
    var idkTemp = "<tr id='mainSTR{Id}' class='mainSTR eStore_openBoxTable'>" +
                    "<td id='STR{i}' style='text-align: left;'><span id='{Name}' name='{Name}' class='jTipProductDetail'>{Name}</span></td>" +
                    "<td style='text-align: left;'>{Desc}</td>" +
                    "<td class='remind'>{CurrencySign}{Price}</td>" +
                    "<td><input name='inputAddonQTY_{Id}' type='text' id='inputAddonQTY_{Id}' class='qtytextbox' warrantyprice='{Warrantyprice}'></td>" +
                    "</tr>";
    var idsTemp = "<tr id='mainSTR{Id}' class='mainSTR eStore_openBoxTable'>" +
                    "<td id='STR{i}' style='text-align: left;'><span id='{Name}' name='{Name}' class='jTipProductDetail'>{Name}</span></td>" +
                    "<td style='text-align: left;'>{Desc}</td>" +
                    "<td>{CurrencySign}{Price}</td>" +
                    "<td><input name='inputAddonQTY_{Id}' type='text' id='inputAddonQTY_{Id}' class='qtytextbox' warrantyprice='{Warrantyprice}'></td>" +
                    "</tr>";
    var idkDetialTemp = "<tr class='STR{i} subTR hiddenitem'>" +
                    "<td class='' style='padding-left:25px;'>{DName}</td>" +
                    "<td style='padding-left:25px;'>{DDesc}</td>" +
                    "<td class=' colorRed right'></td>" +
                    "<td class=''></td>" +
                    "</tr>";
    var idkNewDetailTemp = "<div><span class='title'>{DName}</span><span>{DDesc}</span></div>";


    var idkMobileTemp = "<div class='content mainSTR' id='mainSTR{Id}'>" +
                         "<div class='title'>" +
                         "<span>{Name}</span>" +
                         "<input type='text' class='inputQty'>" +
                         "</div>" +
                         "<div class='description'>{Desc}" +
                        "<ol class='eStore_listPoint moreContent'>" +
                        "{mobileDetail}" +
                        "</ol></div>" +
                        "<div class='unitPrice remind'><span>Unit Price: </span>{CurrencySign}{Price}</div>" +
                        "</div>";

    var idkMobileDetailTemp = "<li><span class='title'>{DName}</span><span>{DDesc}</span></li>"

    $.fn.productaddons = function (productid, _currencySign) {

        var container = $(this);
        eStore.UI.eStoreScripts.getIDKAddons(productid, function (response) {
            if (response != null) {
                $(container).each(function () {
                    rendercontent(this, response, _currencySign);
                });
                compareHeight();
                fixSpanHeight();
            }
        });
    }
    $.fn.IDKCompatibilityEmbeddedBoard = function (productid, _currencySign) {
        var container = $(this);
        eStore.UI.eStoreScripts.getIDKCompatibilityEmbeddedBoard(productid, function (response) {
            if (response != null) {
                $(container).each(function () {
                    rendercontent(this, response, _currencySign);
                });
                compareHeight();
                fixSpanHeight();
            }
        });

    }

    function rendercontent(container, response, _currencySign) {
        if (response != null && response.Addons.length > 0) {
            productaddonsdata = response;
            idkTemp = idkTemp.replace(/[{]CurrencySign[}]/g, _currencySign);
            idsTemp = idsTemp.replace(/[{]CurrencySign[}]/g, _currencySign);

            idkMobileTemp = idkMobileTemp.replace(/[{]CurrencySign[}]/g, _currencySign);

            var ids = new Array();
            $.each(response.Addons, function (i, addon) {
                ids[i] = addon.Id;
            });
            $(container).append("<input type=\"hidden\" name=\"hdProductAddonQtyList\" id=\"hdProductAddonQtyList\" value=\"" + ids.join('|') + "\" />");
            createFilter(container, response.Attibutes, null);
            $(container).find(".pDDLcontent select").on("change", function () { filterAddonsByAttr(container, this); });
            //dispayAddons(response.Addons);
            $(container).find(".pDDLcontent select:first :last-child").prop("selected", "selected");
            var selectValue = $(container).find(".pDDLcontent select:first").attr("selectValue");
            if (selectValue != "" && selectValue != undefined) {
                if ($(container).find(".pDDLcontent select:first option").length > parseInt(selectValue)) {
                    $(container).find(".pDDLcontent select:first option:eq(" + parseInt(selectValue) + ")").prop("selected", "selected");
                }
            }
            $(container).find(".pDDLcontent select:first").trigger("change");
            $(container).show(0);
            checkEstoreInputQTY();
        }
    }

    function dispayAddons(container, addons) {
        $.each(addons, function (i, addon) {
            var temp;
            var tempMobile;
            if (addon.Detail == null) {
                temp = idsTemp;
                tempMobile = idkMobileTemp;
            }
            else {
                temp = idkTemp;
                tempMobile = idkMobileTemp;
            }
            temp = temp.replace(/[{]Id[}]/g, "" + addon.Id + "");
            temp = temp.replace(/[{]Name[}]/g, "" + addon.Name + "");
            temp = temp.replace(/[{]Desc[}]/g, "" + addon.Desc + "");
            temp = temp.replace(/[{]Price[}]/g, "" + formatdecimal(addon.Price.value) + "");
            temp = temp.replace(/[{]Warrantyprice[}]/g, "" + addon.Warrantyprice + "");
            temp = temp.replace(/[{]i[}]/g, "" + i + "");

            tempMobile = tempMobile.replace(/[{]Id[}]/g, "" + addon.Id + "");
            tempMobile = tempMobile.replace(/[{]Name[}]/g, "" + addon.Name + "");
            tempMobile = tempMobile.replace(/[{]Desc[}]/g, "" + addon.Desc + "");
            tempMobile = tempMobile.replace(/[{]Price[}]/g, "" + formatdecimal(addon.Price.value) + "");
            tempMobile = tempMobile.replace(/[{]Warrantyprice[}]/g, "" + addon.Warrantyprice + "");
            tempMobile = tempMobile.replace(/[{]i[}]/g, "" + i + "");

            var tempDetailpro = "";
            if (addon.Detail != null) {
                var temppro = "";
                $.each(addon.Detail, function (t, detail) {
                    var dtemp = idkNewDetailTemp;
                    var dtempMobile = idkMobileDetailTemp;

                    dtemp = dtemp.replace(/[{]DName[}]/g, "" + detail.Name + "");
                    dtemp = dtemp.replace(/[{]DDesc[}]/g, "" + detail.Desc + "");
                    dtemp = dtemp.replace(/[{]i[}]/g, "" + i + "");

                    dtempMobile = dtempMobile.replace(/[{]DName[}]/g, "" + detail.Name + "");
                    dtempMobile = dtempMobile.replace(/[{]DDesc[}]/g, "" + detail.Desc + "");
                    dtempMobile = dtempMobile.replace(/[{]i[}]/g, "" + i + "");

                    temppro = temppro + dtemp;
                    tempDetailpro = tempDetailpro + dtempMobile;
                });
                temp = temp + "<tr class='eStore_openBox_select odd'><td colspan='8' style='text-align: left;'>" + temppro + "</td></tr>";
            }
            tempMobile = tempMobile.replace(/[{]mobileDetail[}]/g, "" + tempDetailpro + "");

            $(container).find("table tbody").append(temp);
            $(container).find("#mobile_idk").append(tempMobile);

        });
        JT_init();
        showProductIDK();
        if ($(container).attr("id") == "divreversionaddons") {
            $(container).find("table tbody").find("td.coloptionimg").wrapInner($("<a />").bind("click", function () {
                var productid = $(this).find("span").attr("id");
                eStore.UI.eStoreScripts.getProductPageLink(productid, function (url) {
                    if (url != undefined && url != "") {
                        document.location.href = url;
                    }
                });
            }));
        }
    }


    function filterAddonsByAttr(container, selectObj) {
        if (productaddonsdata != null) {
            var changeditems = $(container).find(".pDDLcontent :selected[value!='none']");
            var selecteditemcount = $(changeditems).length;

            var matchedAddons = $.map(productaddonsdata.Addons, function (n) {
                var valid = true;
                $(container).find(".pDDLcontent :selected[value!='none']").each(function (i, selector) {
                    selectitemname = $(selector).parent().attr("attr");
                    if ($.map(n.attribute, function (a) {
                        if (a.AttributeName == selectitemname && a.AttributeValue == $(selector).val()) {
                            return true;
                        }
                    }).length == 0) {
                        valid = false;
                    }
                });
                if (valid)
                    return n;
            });
            $(container).find("table tbody").empty();
            $(container).find("#mobile_idk").empty(); 
            dispayAddons(container, matchedAddons);
            if (selecteditemcount == 0) {
                createFilter(container, productaddonsdata.Attibutes, null);
            }
            else if (selecteditemcount == 1) {
                var selectitemname = $(changeditems).attr("attr");
                removeUnuserdattr(container, matchedAddons, selectitemname, selectObj);
            }
            else {
                removeUnuserdattr(container, matchedAddons, "", selectObj);
            }
            checkEstoreInputQTY();
        }
    }
    function removeUnuserdattr(container, mathecdaddons, keepthisallitem, selectObj) {
        if (mathecdaddons == null || productaddonsdata == null)
            return;
        var filtedAttrs = $.map(productaddonsdata.Attibutes, function (attr) {
            var tmp = clone(attr);
            if (keepthisallitem != tmp.Key) {
                tmp.Value = $.map(tmp.Value, function (v) {
                    var matched = false;
                    $.each(mathecdaddons, function (k, addon) {
                        $.each(addon.attribute, function (j, a) {
                            if (a.AttributeName == tmp.Key && a.AttributeValue == v) {
                                matched = true;
                            }
                        });
                    });
                    if (matched)
                        return v;
                });
            }
            return tmp;
        });
        createFilter(container, filtedAttrs, selectObj);
    }
    function createFilter(container, attrs, selectObj) {
        var selectAttr = selectObj == null ? "" : $(selectObj).attr("attr");
        $.each(attrs, function (i, attr) {

            var ddl = $(container).find(".pDDLcontent select[attr='" + attr.Key + "']");
            if (attr.Key != selectAttr) {
                var selectedvalue = $(ddl).val();
                $(ddl).find("option[value!='none']").remove();
                $.each(attr.Value, function (i, attrvalue) {
                    $("<option/>")
                    .prop("value", attrvalue)
                    .prop("selected", selectedvalue == attrvalue)
                    .append(attrvalue.replace("INCH", "''"))
                    .appendTo(ddl);
                });
            }
        });
    }
    function clone(o) {
        if (!o) {
            return o;
        } else {
            var c;
            if (Object.prototype.toString.apply(o) === '[object Array]') {
                c = [];
                for (var i = 0; i < o.length; i++) {
                    c.push(clone(o[i]));
                }
            } else if (Object.prototype.toString.call(o) === '[object Object]') {
                c = {};
                for (var p in o) {
                    c[p] = clone(o[p]);
                }
            } else {
                return o;
            }
            return c;
        }
    }

    function compareHeight() {
        StepH = $(".eStore_product_leftTable").outerHeight();
        listH = $(".eStore_system_listFloat").outerHeight(true);
        startH = (StepH < listH) ? listH : StepH;
        $(".positionFixed").animate({
            "height": startH
        }, 0);
        clearTimeout(timeOut, timeOut2);
    }
})(jQuery);
 