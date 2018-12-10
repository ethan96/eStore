<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductMatrix.ascx.cs"  EnableViewState="false"
    Inherits="eStore.UI.Modules.ProductMatrix" %>
<div class="clear">
</div>
<table id="tblMatrixProducts" runat="server" style="margin:0px" class="estoretable productMatrixTable clear"
    enableviewstate="false">
</table>
<div class="rightside">
    <eStore:Button ID="bt_compare" runat="server" Text="Compare" CssClass="needCheckSelect" OnClick="bt_compare_Click" />
    <eStore:Button ID="bt_AddToQuote" runat="server" CssClass="needCheckMOQ" Text="Add to Quote" OnClick="bt_AddToQuote_Click" />
    <eStore:Button ID="bt_AddToCart" runat="server" CssClass="needCheckMOQ" Text="Add to Cart" OnClick="bt_AddToCart_Click" /></div>
<script language="javascript" type="text/javascript">

            function checkSelect() {
                var cbproduct = $("[name='cbproduct']");
                var checkCount = false;
                if (cbproduct.length > 0) {
                    cbproduct.each(function (i) {
                        if (this.checked) checkCount = true;
                    });
                } else {
                    alert($.eStoreLocalizaion("Can_not_find_the_product"));
                    return false;
                }
                if (checkCount) return true;
                alert($.eStoreLocalizaion("Can_not_find_the_product"));
                return false;
            }

            $(document).ready(function () {
                $(".rightside input.needCheckSelect").click(function () {
                    return checkSelect();
                });

                $(".needCheckMOQ").click(function () {
                    if (checkSelect())
                        return checkMOQ();
                    else 
                        return false;
                });
            });

            function checkMOQ() {
                var cc = true;
                var productids = "";
                var i = 0;
                $(".estoretable input:checkbox[MOQ][value!=''][checked]").each(function () {
                    if (i == 0) {
                        productids = $(this).attr("value") + "[" + $(this).attr("MOQ") + "]";
                        $(this).focus();
                    }
                    else
                        productids = productids + "," + $(this).attr("value") + "[" + $(this).attr("MOQ") + "]";
                    cc = false;
                    i++;
                });
                if (!cc) {
                    if (confirm(productids))
                        return true;
                    else 
                        return false;
                }
                return cc;
            }
</script>

<script type="text/javascript">

    var mouseSelect;
    
    $(document).ready(function () {
        $(".estoredropdownlist li.selectedItem").mouseover(function () {
            $(this).parent().find("ul").show();
            var maxwidth = 0;
            $(this).find("ul li").each(function (i) {
                $(this).width() > maxwidth ? maxwidth = $(this).width() : "";
            });
            $(this).find("ul li").width(maxwidth);
        });

        $(".cssDivDropDownList").each(function () {
            $(this).find("li.selectedItem").each(function () {
                var selectvalue = $(this).find("span").text();
                $.each($(this).parent().find("ul li"), function () {
                    if ($(this).text() == selectvalue)
                    { $(this).addClass("selected"); }
                    else {
                        $(this).removeClass("selected");
                    }
                });
            });
        });

        $(".estoredropdownlist li.selectedItem").mouseout(function () {
            $(this).parent().find("ul").hide();
            if (mouseSelect != null)
                mouseSelect.addClass("selected");
        });

        $(".selectedItem .option li").mouseover(function () {
            mouseSelect = $(this).parent("ul").find("li.selected");
            $(this).parent("ul").find("li").each(function () {
                $(this).removeClass("selected");
            });
        });

        $(".estoredropdownlist li ul li").click(function () {
            $(this).parent().parent().find("span").text($(this).text()).attr("value", $(this).attr("value"));
            $(this).parent().hide();

            $(".productMatrixTable tr:gt(1)").show();

            $.each($(".estoredropdownlist li span"), function () {
                var cid = $(this).attr("value");
                if (cid != "0") {
                    var colIndex = $(this).closest("td")[0].cellIndex + 3;
                    $(".productMatrixTable tr:gt(1) td:nth-child(" + colIndex + ")[name!='" + cid + "']").parent().hide();
                }
            });
            mouseSelect = null;
            $(this).addClass("selected");
        });
        $.each($(".estoredropdownlist li.selectedItem"), function (i, n) {
            if ($(this).width() + 8 > $(this).find("ul.option").width()) {
                $(this).find("ul.option").width($(this).width() + 8);
            }
            $(this).find("span").text($(this).find("ul.option li:first").text());
        });
    });
</script>

<!--[if IE 6]>
    <script type="text/javascript">
    $(document).ready(function () {
        $(".productMatrixTable .attributevalue")
		   .hover(function () {
		       $(this).css("background-color","#FFFFDB");
		   }, function () {
		       $(this).css("background-color","#FFFFff");
		   });
        $(".selectedItem .option li").hover(function () {
                $(".selectedItem .option li").css("background-color","FFFFff");
                $(this).css("background-color","#39F");
            },function(){
                $(".selectedItem .option li").css("background-color","FFFFff");
            }
        );

        $(".cssDivDropDownList").mouseover(function(){
            $(this).find("ul li.selected").css("background-color","#39F");
        });
    });
    </script>
<![endif]-->