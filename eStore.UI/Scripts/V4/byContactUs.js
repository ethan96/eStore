$(function () {
    $(".inquiryType select").change(function () {
        var txt2 = $(".inquiryType select option:first-child").val();
        var txt1 = $(".inquiryType select option:selected").val();
        //console.log(txt2, txt1);
        if (txt1 == 'technicalSupport') {
            $(".eStore_btn_Submit").attr("select", "technicalSupport");
            $(".eStore_inputBlock").removeClass("hide");
            Reset();
            clear();
            $(".softwareVersion").removeClass("hide");
            $(".productCategory").removeClass("hide");
            $(".productModelNO").removeClass("hide");
            $(".purchaseDate").removeClass("hide");
        } else if (txt1 == 'sales') {
            $(".eStore_btn_Submit").attr("select", "sales");
            $(".eStore_inputBlock").removeClass("hide");
            Reset();
            clear();
            $(".productInterest").removeClass("hide");
            $(".pick").removeClass("hide");
        } else if (txt1 == 'generalInquiries') {
            $(".eStore_btn_Submit").attr("select", "generalInquiries");
            $(".eStore_inputBlock").removeClass("hide");
            Reset();
            clear();
        } else {
            Reset();
            $(".eStore_inputBlock").addClass("hide");
        }
    });

    $(".eStore_btn_Reset").click(function () {
        Reset();
    });

    function clear() {
        $(".softwareVersion").addClass("hide");
        $(".productCategory").addClass("hide");
        $(".productModelNO").addClass("hide");
        $(".purchaseDate").addClass("hide");
        $(".productInterest").addClass("hide");
        $(".pick").addClass("hide");
    }

    function Reset() {
        $('.eStore_inputBlock').find('.eStore_contactUs_input').find('input , textarea').each(function () {
            if ($(this).attr('type') == null || $(this).attr('type').toUpperCase() == 'TEXT')
                $(this).val('');
        });
        $('.eStore_inputBlock').find('.eStore_contactUs_input').find('select').each(function () {
            if ($(this).attr('class') == 'dependent') {
                $(this).empty();
                $(this).append('<option>None</option>');
                $(this).attr('disabled', 'true');
            }
            else
                $(this).find('option:first').attr('selected', 'true');
        });
        $('.eStore_imgVerificationtxtResult').addClass('hiddenitem');
    };
});