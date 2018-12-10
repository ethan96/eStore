$(function () {
    $('.eStore_policy_tabList a').click(function () {
        $(this).toggleClass("on").siblings(".eStore_policy_tabList a").removeClass("on");
        var id = $(this).attr('pid');
        $.getJSON(GetStoreLocation() + 'api/category/PolicyCategory/' + id,
            function (data) {
                $('.eStore_policy_html').html(data.Html);
            });
    });
});
