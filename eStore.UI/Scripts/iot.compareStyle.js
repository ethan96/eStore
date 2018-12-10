$(function () {
    // ** compare 變色 **
    $("li.iot-proBlock :input[type='checkbox']").bind('click', function () {
        bindBoxBlock($(this));
    });
    $("li.iot-proBlock :input[type='checkbox']").each(function () {
        bindBoxBlock($(this));
    });
});

function bindBoxBlock(obj) {
    var checked = $(obj).attr("checked");
    if (checked) {
        $(obj).closest('li.iot-proBlock').addClass("checkbox-checked");
        checkCompareCount($(obj), 4);
    } else {
        $(obj).closest('li.iot-proBlock').removeClass("checkbox-checked");
    }
}

function checkCompareCount(obj,count) {
    var length = $("li.iot-proBlock :input[type='checkbox'][checked]").length;
    if (length > count) {
        alert("max select!");
        obj.removeAttr("checked").closest('li.iot-proBlock').removeClass("checkbox-checked");
        return false;
    }
}