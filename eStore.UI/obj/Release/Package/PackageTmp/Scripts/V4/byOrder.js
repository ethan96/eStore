// JavaScript Document
function sameAddress(obj) {
    var billValue = $(".eStore_ShippingPersonal[lb='hIsBillto'] input:checked").val();
    var objCon = obj.parents(".eStore_sameAddress");
    var checked = obj.prop("value");
    if (checked == "true") {
        objCon.addClass("small").siblings("div").hide();
        var btinput = objCon.parent(".eStore_information").find("div[lb]").find("input[value='" + billValue + "']");
        //objCon.parent(".eStore_information").find(".eStore_ShippingCompany select").find("option[value='" + btinput.parents("[companyfor]").attr("companyfor") + "']").click();
        var select = objCon.parent(".eStore_information").find(".eStore_ShippingCompany select");
        select.find("option").removeAttr("selected");
        select.find("option[value='" + btinput.parents("[companyfor]").attr("companyfor") + "']").prop("selected", true);
        select.trigger("change");
        objCon.parent(".eStore_information").find("div[companyfor]").removeClass("blueBorder");
        btinput.click().parents(".eStore_ShippingPersonal_msg").addClass("blueBorder");
    } else {
        objCon.removeClass("small").siblings("div").show();
        fixSpanHeight();
    }
}

$(function () {

    $(".eStore_sameAddress .sameAddressYes").each(function (i, n) {
        sameAddress($(n));
    });
    $(".eStore_order_stepsBlock li").append("<span></span>");
    $(".eStore_orderStep_subTotal").after("<div class='clearfix'></div>");

    /**/
    var StepTdN = $(".eStore_table_order.eStore_orderItem tr:first-child th").length;
    //$(".eStore_orderSystemList").after("<tr class='eStore_orderSystemList_borderTop'><td colspan='" + StepTdN + "'></td></tr>");
    //$(".eStore_orderSystemList td").attr("colspan", StepTdN);
    //$(".eStore_table_order .eStore_orderSystemList li b").append(":");
    $(".eStore_table_order tr:nth-child(odd)").addClass("odd");

    //orderStep1 SubTotal
    var w1 = $(".eStore_orderStep1 tr th:last-child").outerWidth(true);
    var w2 = $(".eStore_orderStep1 tr th:nth-last-child(2)").outerWidth(true);
    $(".eStore_orderStep_subTotal.SubTotal_step1 div span:nth-child(2)").css({

        "min-width": w2,
        "padding-right": w1 + 8
    });

    //orderStep1 VAT Number
    var w3 = $(".VATNumber_content").outerWidth();
    //console.log(w3);
    $(".VATNumber_att").css("width", 980 - w3 - 30);

    $(".VATNumber").focus(function () {
        $(".VATNumber_att").fadeIn();
    }).focusout(function () {
        $(".VATNumber_att").fadeOut();
    });

    //orderStep2 radioList----------------------------------------
    $(".eStore_addReceiverBlock").before("<div class='clearfix'></div>");

    $(".eStore_ShippingPersonal_msg .radio").children("input").click(function () {
        $(this).parents(".eStore_ShippingPersonal").children(".eStore_ShippingPersonal_msg").removeClass("show blueBorder");
        $(this).parents(".eStore_ShippingPersonal_msg").addClass("show blueBorder");
    });

    $(".eStore_sameAddress input").click(function () {
        $(this).siblings("input[type='radio']").attr('checked', false);
        $(this).attr('checked', true);
        sameAddress($(this));
        var hfitem = $(this).attr("hflb");
        $("#" + hfitem).val($(this).attr("value"));
    });

    $(".eStore_information [companyfor] .info").on("click", function () {
        $(this).prev(".radio").find("input:radio").click();
    });

    //step 4 select type card
    $(".eStore_order_byCreditCard_left .CreditCardType").prepend("<span class='cardBG'></span>");

    //add Receiver
    $(".eStore_orderStep2 .eStore_addReceiverBlock table tr td:nth-child(4),.eStore_orderStep2 .eStore_addReceiverBlock table tr td:nth-child(2)").css("padding-right", 30);
    $(".eStore_addReceiver span.btn").click(function () {

        $(this).parents(".eStore_information").siblings(".eStore_addReceiverBlock").show();

    });
    

    //inline
    $("#inline1 table.content tr:nth-child(odd)").addClass("odd");

});

