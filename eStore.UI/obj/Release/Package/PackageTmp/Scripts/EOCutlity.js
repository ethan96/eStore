$(document).ready(function () {
    $(".ecoInterested").click(function () {
        window.open("http://member.advantech.com/yourcontactinformation.aspx?formid=841a3255-ee3b-4de5-b0c6-3a2835e5dfc5&urlid=577c2433-e471-437c-a7a9-807ca7ca556d&utm_campaign=Report&utm_medium=Banner-on-eStore&utm_source=Banner-on-eStore");
        ddlInECOIndustryClick();
    });

    $(".sendECOFrindEmail").click(function () {
        var dataObj = eval("(" + $(this).attr("objFr") + ")");
        $("#txtFr_Company").val(dataObj.Company);
        $("#hfFr_PartnerId").val(dataObj.PartnerId);
        popupDialog("#ecoEmailFrinds");
    });

    $(".sendRequestECOEmail").click(function () {
        var dataObj = eval("(" + $(this).attr("objFr") + ")");
        $("#hfFr_PartnerId").val(dataObj.PartnerId);
        popupDialog("#ecoRequestAssistance");
    });

    $(".ddlInECOIndustry").click(function () {
        ddlInECOIndustryClick();
    });

    function ddlInECOIndustryClick() {
        var txt = $(".ddlInECOIndustry").find("option:selected").text();
        if (txt == "Other") {
            $("#OtherIndustrySpan").show();
        }
        else {
            $("#OtherIndustrySpan").hide();
        }
    }

});

function changeContactDate() {
    var startDate = document.getElementById("ContactHourStart").value;
    var endDate = document.getElementById("ContactHourEnd").value;
    if (startDate >= endDate) {
        alert($.eStoreLocalizaion("Best_contact_start_date_must_earlier_end_date"));
    }
}

function checkECOInput(group) {
    var isOk = true;
    $("." + group).each(function () {
        var val = $.trim($(this).val());
        if (val == "") {
            $(this).focus();
            isOk = false;
            return false;
        }
        else {
            if ($(this).hasClass("mustEmail")) {
                var patten = new RegExp(/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]+$/);
                if (!patten.test(val)) {
                    $(this).focus();
                    isOk = false;
                    return false;
                }
            }
        }
    });
    if (!isOk) {
        if (event) 
            event.returnValue = false;
    }
    return isOk;
}