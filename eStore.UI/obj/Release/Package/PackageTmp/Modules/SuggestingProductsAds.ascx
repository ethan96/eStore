<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SuggestingProductsAds.ascx.cs"
    Inherits="eStore.UI.Modules.SuggestingProductsAds" %>
<div id="SuggestingProductsAds" class="eStore_order_moreInfo row20">
</div>

<script id="_tmpalsobuy" type="text/x-jquery-tmpl">
<h2>Customers Also Bought</h2>
<div class="eStore_customersBought carouselBannerSingle" id="customersBought">
<ul>
    <!-- ko foreach:  $data -->
    <li>
        <div class="pic"><img data-bind="attr: { src: image, title: ProductID }" /></div>
        <div class="txt">
            <div class="title" data-bind="text: ProductID"></div>
            <div class="content" data-bind="text: Desc"></div>
            <div class="priceOrange"><a href="#">Datasheet</a><span data-bind="html: price"></span></div>
            <div class="action"><a data-bind="attr: { href: Hyperlink }" class="eStore_btn">Add to Cart</a></div>
        </div>
    </li>
    <!-- /ko -->
</ul>
<div class="clearfix"></div>
<div class="carousel-control">
    <a id="prev" class="prev" href="#"></a>
    <a id="next" class="next" href="#"></a>
</div>
</script>


<script type="text/javascript" language="javascript">

    $(document).ready(function () {

        eStore.UI.eStoreScripts.getSuggestingProducts('<%=type %>'
        , function (data) {
            if (data && data.length != 0) {
                var temp = $("#SuggestingProductsAds");
                // apply "template" binding to div with specified data
                var name = "_tmpalsobuy";
                $.each(data, function (j, group) {
                    ko.applyBindingsToNode(temp[0], { template: { name: name, data: group.Products} });
                    showAlsoBuy();
                });

            }
        });
    });
    function showAlsoBuy() {
        $(".carouselBannerSingle").each(function () {
            var id = $(this).attr('id');
            id = "#" + id;
            $(id).find("ul").carouFredSel({
                auto: false,
                scroll: 1,
                prev: '#prev',
                next: '#next',
                pagination: '#pager'
            });
        });
    }
</script>
