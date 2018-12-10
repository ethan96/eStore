<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StoreSearch.ascx.cs"
    Inherits="eStore.UI.Modules.StoreSearch" %>
<script type="text/javascript">
    $.widget("custom.catcomplete", $.ui.autocomplete, {

        _create: function () {
            this._super();
            // this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
        },
        _renderMenu: function (ul, items) {

            var that = this,
                currentCategory = "";
            $(".eStore_menuLink_searchBlock .close").show();
            $.each(items, function (index, item) {
                var li;
                if (item.GroupId != currentCategory) {
                    ul.append("<div class='searchBlock_result_type " + item.GroupId + "' aria-label='" + item.GroupName+"'>" + item.GroupName + "</div>");
                    currentCategory = item.GroupId;
                }

                li = that._renderItemData(ul, item);

                if (item.GroupId) {
                    li.attr("aria-label", item.GroupId + " : " + item.Id);
                }
            });
            ul.addClass("eStore_menuLink_searchBlock_result");
        },

        _renderItem: function (ul, item) {
            var _li = $("<li>")
                .addClass(item.GroupId)
                .attr("id", "ui-menu-item-" + item.Id)
                .attr("data-value", item.Id);
            var _a = $("<a>").addClass("searchResult_product_sub").html(item.Name).attr("href", item.Url);
            if (item.GroupId == "Category" && item.ProCount != null) {
                _a.append("<i> (" + item.ProCount +")</i>");
            }
            _li.append(_a);
            _li.appendTo(ul);
            return _li;
        }
    });
    $(function () {
        $(".storekeyworddispay").catcomplete({
            position: { my: "right top", at: "right bottom" },
            source: function (request, response) {
                $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=<%=(int)eStore.Presentation.eStoreContext.Current.SearchConfiguration.HeaderAJAXFunctionType %>",
                 {
                     maxRows: 12,
                     keyword: request.term
                 }, response);
            },
            select: function (event, ui) {
                window.document.location = ui.item.Url;
            },
            focus: function (event, ui) {
                $('#JT').remove();
                if (ui.item != null && ui.item.GroupId == "Product") {
                    $("input.storekeyworddispay:last").data("autocompletefocus", true);
                    var url = "<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/html.aspx?type=ProductDetailTip&ProductID=" + ui.item.Id;
                    JT_showPro(url, "ui-menu-item-" + ui.item.Id, ui.item.Name, $("#ui-menu-item-" + ui.item.Id));
                }
            },
            close: function (event, ui) { $('#JT').remove(); }
        });
        $(".eStore_menuLink_searchBlock .close").click(function () {
            $(".eStore_menuLink_searchBlock .storekeyworddispay").val("").focus();
            $(this).hide();
        })
    });
</script>
<input type="text" title="Search key" name="keysearch" class="InputValidation storekeyworddispay" value='<%=string.IsNullOrEmpty( Request["skey"])?Request["storekeyworddispay"]: Request["skey"] %>' placeholder="<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_search_box)%>" />
 
