function eableGoldEggsAdv(ad) {
    eableGoldEggsEvent(ad);
    showGoldEggsDialog();
    checkUserId();
    $.preloadImages("images/gift-1.png", "images/gift-2.png", "images/gift-3.png", "images/gift-4.png", "images/gift-5.png", "images/goldeggs-bk.png", "images/hammer.ico", "images/hammer-down.ico");
};

function isInellipse(egg, e) {
	var Xc = $(egg).offset().left +135;//+ $(egg).width() / 2;
	var Yc = $(egg).offset().top + $(egg).height() / 2-24;
	var a = 62;//$(egg).width() / 2;
	var b = 80;//$(egg).height() / 2;
	return containsXY(e.clientX, e.clientY, Xc, Yc,a, b);
}
function containsXY(x, y, Xc, Yc, a, b) {
	return Math.pow((y - Yc), 2) / Math.pow(b, 2) + Math.pow((x - Xc), 2) / Math.pow(a, 2) <= 1;
}

//function reEableGoldEggsAdv() {
//    $(".eggareabk").removeClass("eggareabk").addClass("eggarea");
//    $(".eggarea").html("");
//    eableGoldEggsEvent();
//}
function eableGoldEggsEvent(ad) {
    $(".eggarea").click(function (e) {
        if (isInellipse(this, e)) {
            $(".eggarea").unbind("mousemove").unbind("click").css('cursor', 'default');
            var obj = $(this).attr("id");
            $("#" + obj).css('cursor', 'url(images/hammer-down.ico),auto');
            eStore.UI.eStoreScripts.smashGoldEggs(ad.id, function (res) {
                $("#" + obj).removeClass("eggarea").addClass("eggareabk").append($("<img />").attr("src", res.ImageUrl).attr("title", res.Desc));
                if (res.smashed) {
                    $("#" + obj).append($("<p>").addClass("getHome").append($("<a>").addClass("takeithome").attr("href", "#").html("您已中奖")));
                }
                else {
                    $("#" + obj).append($("<p>").addClass("getHome").append($("<a>").addClass("takeithome").attr("href", "#").html("领回家").bind("click", function () { showUserInfor() })));
                }
                $("#" + obj).css('cursor', "default");
            });
        }
    });
    $(".eggarea").mousemove(function (e) {
        if (isInellipse(this, e)) {
            $(this).css('cursor', 'url(images/hammer.ico),auto');
        }
        else {
            $(this).css('cursor', "default");
        }
    });
}
function showGoldEggsDialog() {
    $("#eggsContext").dialog({
        resizable: false,
        height: 632,
        width: 960,
        zIndex: 210,
        modal: true
    }).dialog('widget').find('.ui-widget-header').hide();
    $("#close").click(function () {
        $("#eggsContext").dialog("close");
    });
}
function clikdRegisterHere() {
    if ($("#RegisterHereLink").length > 0)
        window.location.href = $("#RegisterHereLink").attr("href");
    else
        alert("您已登录！");
}
function checkUserId() {
    var temid = $.cookie("LogTemId");
    if (temid != undefined && temid != null && temid != "") {
        eStore.UI.eStoreScripts.modifyGoldEggsGiftUser(temid, function (res) { });
    }
}
function showUserInfor() {
    eStore.UI.eStoreScripts.submintUserInfor("", "",false, function (res) {
        if (res) {
            $("#divUserInfor").show().dialog({
                title: "信息确认",
                zIndex: 230,
                width: 345,
                modal: true
            });
        }
        else {
            return popuploginnormal(null);
        }
    });    
}
function submintUserInfor() {
    var _userAddrss = $.trim($("#txGEAddrss").val());
    var _userTel = $.trim($("#txGETel").val());
    if (_userAddrss == "" || _userTel == "") {
        alert("地址和电话不能为空！");
        return false;
    }
    $("#btSubUserInfor").after("<img id='imloadingPress' src='images/Loading.gif' />").hide();
    eStore.UI.eStoreScripts.submintUserInfor(_userAddrss, _userTel, true, function (res) {
        if (res) {
            alert("修改成功！");
            $("#divUserInfor").dialog("close");
            $(".takeithome").unbind("click");
        }
        else {
            alert("通讯失败！");
        }
    });
}
jQuery.preloadImages = function () {
    for (var i = 0; i < arguments.length; i++) {
        $("<img>").attr("src", arguments[i]);
    }
}
