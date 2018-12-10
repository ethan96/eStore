<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductList.ascx.cs" Inherits="eStore.UI.Modules.ProductList" %>
<%@ Register Src="QuantityDiscountRequest.ascx" TagName="QuantityDiscountRequest" TagPrefix="eStore" %>
<%@ Register Src="GVProductList.ascx" TagName="GVProductList" TagPrefix="uc1" %>
<div id="ProductList">
    <uc1:GVProductList ID="GVProductList1" runat="server" />
</div>
<div class="rightside">
    <asp:Button ID="bt_Compare" runat="server" Text="Compare" CssClass="eStore_btn borderBlue"
        OnClick="bt_Compare_Click" EnableViewState="false" />
    &nbsp;<eStore:Button ID="bt_AddtoQuote" runat="server" CssClass="needCheckMOQ eStore_btn borderBlue" Text="Add to Quote"
        OnClick="bt_AddtoQuote_Click" EnableViewState="false" />
    &nbsp;<eStore:Button ID="bt_AddtoCart" runat="server" CssClass="needCheckMOQ eStore_btn" Text="Add to Cart"
        OnClick="bt_AddtoCart_Click" EnableViewState="false" />
</div>
<%  if (!string.IsNullOrEmpty(eStore.Presentation.eStoreContext.Current.getStringSetting("SOMSalesPhoneNumber")))
    {%>
<div id="QuantityDiscountRequestDialog" style="display: none;">
    <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server" NeedLogin="false" />
</div>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {

        var $SOMHelper = $("<ul />").append(
            $("<li />").css("height", "28px").css("list-style", "none").css("line-height", "28px").text('<%=eStore.Presentation.eStoreContext.Current.getStringSetting("SOMSalesPhoneNumber") %>').prepend($("<img />").attr("src", GetStoreLocation() + "images/iconphone.jpg").css("width", "26px")))
            .append(
            $("<li />").attr("id", "contactforprice").css("cursor", "pointer").css("height", "28px").css("list-style", "none").css("line-height", "28px").text(" E-mail Us").prepend($("<img />").attr("src", GetStoreLocation() + "images/iconemail.jpg").css("width", "26px").css("clear", "left"))
             );
        var params = new Object();
        params["width"] = "160";
        $(".ajaxproductprice[id^='SOM-']").each(function (i, n) {
            $(n).parent().attr("id", "SOM-CELL-" + this.id);
        });
        $("td[id^='SOM-CELL-']")
		   .hover(function () {

		       $("#QuantityDiscountRequestDialog table tr:eq(1) td:eq(1)").text($(this).parent().find("td:eq(1)").text());
		       $("#eStoreMainContent_ctl00_QuantityDiscount_hSProductID").val($.trim($(this).parent().find("td:eq(1)").text()));
		       $("#QuantityDiscountRequestDialog table tr:eq(2) td:eq(1)").text($(this).parent().find("td:eq(2)").text());
		       $('#JT').remove();
		       JT_showContent($SOMHelper.html(), this.id, "Contact for Price:", params);
		       $("#JT #contactforprice").live("click", function () {
		           $('#JT').remove();

		           return showQuantityDiscountRequestDialog();
		       })
		   }, function () { });
    });

	function showQuantityDiscountRequestDialog() {
	    popupDialog("#QuantityDiscountRequestDialog");
        return false;
    }
</script>
<%
    } %>
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $(".rightside input").click(function () {
            if (checkSelect())
                if ($(this).hasClass("needCheckMOQ"))
                    return checkMOQ();
                else
                    return true;
            else
                return false;
        });
    });
</script>