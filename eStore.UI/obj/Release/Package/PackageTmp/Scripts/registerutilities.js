

function doubleCheckPSD() {
    var result = false;
    if ($.trim($("#tbUserPassWord").val()).length < 1) {
        $("#splengthpsd").show();
        return result;
    }
    if ($.trim($("#tbUserPassWord").val()) !== $.trim($("#tbUserPassWordDou").val())) {
        $("#spanconfpassword").show();
    }
    else {
        $("#spanconfpassword").hide();
        result = true;
    }
    return result;
}

function checkEmail() {
    var re = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/
    var checkresult = false;
    if (re.test($.trim($("#tbUserEmail").val()))) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            url: "eStoreScripts.asmx/checkUser",
            data: { useremail: $.trim($("#tbUserEmail").val()) },
            dataType: "json",
            async: false,
            success: function (result) {
                if (!result.d) {
                    $("#spanemailstr").html("").append($("<img />").attr("src", GetStoreLocation() + "images/ok-icon.png"));
                    checkresult = true;
                }
                else {
                    $("#spanemailstr").html("对不起，这个帐号（电子邮件）不可用。").show();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    } else {
        $("#spanemailstr").html("邮箱格式不正确");
    }
    return checkresult;
}


function bindStates(country) {
    if ($.trim(country) == "")
        return false;
    $("#ddlStates").empty().append("<option value=''>-- 请选择 --</option>");
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "eStoreScripts.asmx/getAllStateByCountry",
        data: { countryname: country },
        dataType: "json",
        success: function (result) {
            if (result != null || result != undefined) {
                var states = eval('(' + result.d + ')');
                var ddlStates = $("#ddlStates");
                $.each(states, function (i, item) {
                    ddlStates.append("<option value='" + $.trim(item.abbr) + "'>" + item.state + "</option>");
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function checkMustInputInfor(stepid) {
    var result = true;
    $(".cannotnull" + stepid).each(function (i, item) {
        if ($.trim($(item).val()) == "") {
            if (!$(item).next("span").is(".red"))
                $(item).after($("<span />").addClass("red").html("必填"))
            $(item).attr("style", "border: 1px solid #F00;").click(function () {
                $(this).removeAttr("style");
                if ($(this).next("span").is(".red"))
                    $(this).next("span").remove();
            }).blur(function(){
                $(this).removeAttr("style");
                if ($(this).next("span").is(".red"))
                    $(this).next("span").remove();
            }).focus();
            result = false;
        }
    });

    $(".cannotnullbox" + stepid).each(function (i, item) {
        var hasselect = false;
        if ($(item).find("input[type='checkbox']:checked").length <= 0) {
            if (!$(item).next("span").is(".red"))
                $(item).after($("<span />").addClass("red").html("必填"))
            $(item).attr("style", "border: 1px solid #F00;").click(function () {
                $(this).removeAttr("style");
                if ($(this).next("span").is(".red"))
                    $(this).next("span").remove();
            }).blur(function () {
                $(this).removeAttr("style");
                if ($(this).next("span").is(".red"))
                    $(this).next("span").remove();
            }).focus();
            result = false;
        }
    });
    return result;
}

// ----------------------------------------------







