<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderbyPartNO.ascx.cs"
    Inherits="eStore.UI.Modules.OrderbyPartNO" %>
    <div class="eStore_breadcrumb eStore_block980">
    	<a href="/"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a>
    </div>
    <div class="eStore_container eStore_block980">
    	<div class="eStore_order_content">
        	<div class="eStore_orderByNo row20">
            	<h2><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_By_Multiple_Part_Number)%></h2>
            	<asp:Repeater ID="rp_table_list" runat="server">
                    <HeaderTemplate>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_order eStore_orderItem">
                            <tr>
                                <th width="30">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_No)%>
                                </th>
                                <th width="140">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_PartNO)%>
                                </th>
                                <th>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Description)%>
                                </th>
                                <th width="80">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Price)%>
                                </th>
                                <th width="40">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_QTY)%>
                                </th>
                                <th width="100">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Subtotal)%>
                                </th>
                                <th width="55">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Delete)%>
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                            <tr id="tr_<%# Container.ItemIndex + 1 %>">
                                <td>
                                    <%# Container.ItemIndex + 1 %>
                                </td>
                                <td>
                                    <eStore:TextBox ID="tbpartno" runat="server" placeholder="Please enter part No." CssClass="orderbypartnotextbox"></eStore:TextBox>
                                </td>
                                <td class="eStoreV4" style="text-align:left">
                                    &nbsp;
                                </td>
                                <td id="price_<%# Container.ItemIndex %>" class="eStoreV4">
                                    &nbsp;
                                </td>
                                <td>
                                    <eStore:TextBox ID="tbqty" runat="server" Width="30px" CssClass="qtytextbox"></eStore:TextBox>
                                </td>
                                <td class="eStoreV4 totalPrice">
                                    &nbsp;
                                </td>
                                <td>
                                    <a id="delete_<%# Container.ItemIndex + 1 %>" href="#"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Delete)%></a>
                                </td>
                            </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            <tr>
                  	            <td colspan="7" class="addMore"><a id="addrow" href="">Add more Part No.</a></td>
                            </tr>   
                        </table>
                    </FooterTemplate>
                </asp:Repeater>   
			</div>
            <div class="eStore_orderStep_subTotal row20">
            	<div>
                    <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Subtotal)%></span>
                    <span id="total"></span>
                </div>
            </div>
            <div class="clearfix"></div>
            <div class="eStore_order_btnBlock row20">
                <eStore:Button ID="bt_mAddtoQuote" runat="server" Text="Add to Quote" CssClass="eStore_btn borderBlue AddProduct" OnClick="bt_mAddtoQuote_Click" />&nbsp;
                <eStore:Button ID="bt_mAddtoCart" runat="server" CssClass="eStore_btn AddProduct" Text="Add to Cart" OnClick="bt_mAddtoCart_Click" />
            </div>
        </div>
	</div>

<div id="ProductDependency" style="display: none;">
    
</div>
<script type="text/javascript" language="javascript">

    var dependencisPart = new Array(); //是否含有Dependency products

    $(document).keypress(function(e){
        if(e.which == 13)
            return false;
    });

    $(document).ready(function () {
        var count = 1;
        $("tr[id^='tr_']").each(function(){
            if(count % 2 == 0){
                $(this).addClass("odd");
            }
            if(count > 3){
                $(this).hide();
            }
            count++;
        });

        $("#addrow").click(function(){
            for(var i = 4; i <= 15; i++){
                if($("#tr_" + i).is(":visible") == false){
                    $("#tr_" + i).show();
                    return false;
                }
            }
            //TO DO: 
            alert("Out of maximun number");
            return false;
        });

        $("a[id^='delete_']").each(function(){
            $(this).click(function(){
                var id = $(this).attr("id").replace("delete_", "");
                $("#tr_" + id).find("input").each(function(){
                    $(this).val("");
                });
                $("#tr_" + id + " > td.eStoreV4").each(function(){
                    $(this).html("");
                });

                UpdateTotalPrice(); //ICC
                return false;
            });
        });

        $(".orderbypartnotextbox").autocomplete({
            source: function (request, response) {
                $.getJSON(GetStoreLocation()+"proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getOderByPNSearchKey %>",
                     {
                         maxRows: 12,
                         keyword: request.term
                     },
                     response);
            },
            minLength: 3,
            select: function (event, ui) {
                $('#JT').remove();
                var proNo = ui.item.label;
                var _parent = $(this).parent();
                return setProductInfo(this, _parent, ui.item.label, ui.item.value);
            },
            focus: function (event, ui) {
                if (ui.item.value.match(/\bSOM.*/) || ui.item.value.match(/.*-[xX]{2}\b/))
                { }
                else {
                    var url = GetStoreLocation()+"proc/html.aspx?type=ProductDetailTip&ProductID=" + ui.item.value;
                    $(this).attr("id", ui.item.value);
                    $('#JT').remove();
                    JT_show(url, ui.item.value, ui.item.label);
                }
            },
            close: function (event, ui) { $('#JT').remove(); }
        });
        $(".orderbypartnotextbox").autocomplete({
            selectFirst: true
        });

        $(".qtytextbox").focus(function () {
            this.select();
        });

        $(".orderbypartnotextbox").keydown(function (event) {

            var key = event.keyCode ? event.keyCode : event.which;
            if (key == '13') {
                // enter
                if ($(this).val() == "") return false;

                setProductInfo(this, $(this).parent(), $(this).val(), $(this).val());
                var nextorderbypartnotextbox = $(this).parent().parent().next("tr").find(".orderbypartnotextbox");
                if (nextorderbypartnotextbox.length > 0)
                    nextorderbypartnotextbox.focus();
            }
            else if (key == '9') {
                //tab
                if ($(this).val() == "") return false;
                setProductInfo(this, $(this).parent(), $(this).val(), $(this).val());
            }

        });


        $(".AddProduct").click(function(){
            return checkDependency($(this));
        });

        

        function setProductInfo(sender, container, productname, productid) {

            if (productname.match(/\bSOM.*/)) {
                $(sender).val("");
                container.siblings().eq(1).html("");
                container.parent().find("input:last").val("");
                container.siblings().eq(2).html("");
                container.siblings().eq(4).html("");
                alert($.eStoreLocalizaion("This_product_requires_custom_effort"));
                return false;
            }

           
            if (productname.match(/.*-[xX]{2}\b/)) {
                $(sender).val("");
                container.siblings().eq(1).html("");
                container.parent().find("input:last").val("");
                container.siblings().eq(2).html("");
                container.siblings().eq(4).html("");
                alert($.eStoreLocalizaion("This_product_is_only_good_for_demo"));
                return false;
            }
            $(sender).val(productname);

            $.ajax({
                url: GetStoreLocation()+"proc/do.aspx",
                type: "GET",
                data: { func: <%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getProductPrice %>, id: productid ,isCheckOS:true},
                dataType: "json",
                timeout: 20000,
                error: function () {
                    alert($.eStoreLocalizaion("Can_not_find_the_product"));
                    container.siblings().eq(1).html("");
                    $(sender).val("");
                },
                beforeSend: function () {
                    container.siblings().eq(1).html("<img src='"+GetStoreLocation()+"images/Loading.gif'>");
                },
                success: function (xml) {
                    if (xml == null) {
                        alert($.eStoreLocalizaion("Can_not_find_the_product"));
                        $(sender).val("");
                        container.siblings().eq(1).html("");
                        container.parent().find("input:last").val("");
                        container.siblings().eq(2).html("");
                        container.siblings().eq(4).html("");
                    }
                    else {
                        container.siblings().eq(2).html(xml.price);
                        container.siblings().eq(4).html(xml.price);
                        if (xml.discription == "") {
                            container.siblings().eq(1).html(proNo);
                        }
                        else {
                            if (xml.phasedOut == true) {
                                container.siblings().eq(1).html(xml.discription + "<p class='regularprice'><span>[Phased Out]</span></p>");
                            }
                            else {
                                container.siblings().eq(1).html(xml.discription);
                            }
                        }
                        //container.siblings().eq(1).html(xml.discription);
                        var lastInput = container.parent().find("input:last");
                        if (xml.phasedOut == true) {
                            lastInput.val(0);
                            lastInput.attr("disabled", "disabled");
                        }
                        else {
                            lastInput.val(1);
                            lastInput.removeAttr("disabled", "disabled");
                        }
                    }
                    UpdateTotalPrice(); //ICC
                }
            });
            $('#JT').remove();
            $(".orderbypartnotextbox").autocomplete("close");

            var sproductids = "";
            $(".orderbypartnotextbox").each(function(i){
                if($(this).val() != ""){
                    if(sproductids == "") {
                        sproductids = $(this).val();
                    }
                    else {
                        sproductids += "," + $(this).val();
                    }
                }
            });
            $("#ProductDependency")
            .load(GetStoreLocation()+"proc/do.aspx?func=13&partNos="+sproductids);

            //TO DO: 
            var subtotal = 0;
            $(".regularprice > span").each(function(){
//                if(!isNaN($(this).text().replace("NT$", ""))){
//                    subtotal += $(this).text();
//                }
            });
            $("#total").text(subtotal);
//            var sproductids = "";
//            $(".orderbypartnotextbox").each(function(i){
//                if($(this).val() != ""){
//                    if(sproductids == "") {
//                        sproductids = $(this).val();
//                    }
//                    else {
//                        sproductids += "," + $(this).val();
//                    }
//                }
//            });
//            if(sproductids != "")
//            {
//                $.ajax({
//                    url: "/proc/do.aspx",
//                    type: "GET",
//                    data: { func: 13, partNos: sproductids },
//                    dataType: "json",
//                    timeout: 20000,
//                    error: function () {
//                        alert($.eStoreLocalizaion("Error"));
//                    },
//                    success: function (xml) {
//                        if (xml == null) {
//                            alert($.eStoreLocalizaion("Null"));
//                        }
//                        else {
//                            dependencisPart.length = 0;
//                            $.each(xml,function(i,citem){
//                                dependencisPart.push(citem.productid);
//                            });
//                        }
//                    }
//                });
//            }            
            return false;
        }

        $(".qtytextbox").keyup(function () {

            var _parent = $(this).parent();
            var priceStr = _parent.siblings().eq(3).text();
            var qty = $(this).val();
            var reg = /\d{1,20}(,\d{3})*(\.\d+)?/;
            //全局匹配 是否满足这种9.876,54
            var regAllPrice = /((\d{1,3}\.)+)?(\d{1,3}),\d{2}$/;
            var priceFH;var priceInt;var priceTotal;
            if (regAllPrice.test(priceStr)) {
                reg = /\d{1,20}(\.\d{3})*(\,\d{1,2})?/;
                priceFH = priceStr.replace(reg, "");
                priceInt = priceStr.replace(priceFH, "").replace(/\./g, "").replace(",", ".");
                //如果是5.55 变成5,55
                priceTotal = (priceInt * 100 * qty / 100).toString().replace(".",",");
            }
            else{
                priceFH = priceStr.replace(reg, "");
                priceInt = priceStr.replace(priceFH, "").replace(/\,/g, "");
                priceTotal = priceInt * 100 * qty / 100;
            }
            
            _parent.siblings().eq(4).html(priceFH + priceTotal);

            UpdateTotalPrice(); //ICC 
        });

        
    });
    function closeAutocomplete() {
        $(".orderbypartnotextbox").autocomplete("close");
    }

    function checkDependency(obj)
    {
        if($("#ProductDependency").find("table").html() != null){
            if(isCheckDependency == true){
                showDependencyDiv(obj);
                return false;// test is true
            }
        }
        return true;
    }
    var isCheckDependency = true;
    function showDependencyDiv(obj)
    {
        $("#ProductDependency .btnProductDependency").remove();
        popupDialog($("#ProductDependency").append($("<input />")
                                          .prop({ type: "button", value: "<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ScriptMessage_OK) %>" , class : "btnProductDependency"})
                                          .bind("click",function () {
                                             isCheckDependency = false;
                                             __doPostBack($(obj).attr("name"),'');               
                                           }))
                                           .append($("<input />")
                                          .prop({ type: "button", value: "<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ScriptMessage_Cancel) %>" ,  class : "btnProductDependency"})
                                          .bind("click",function () {
                                             isCheckDependency = true;
                                             parent.$.fancybox.close();               
                                           }))
                    );
        return false;
    }
    $(".AddProduct").click(function(){
      if (typeof (_wmx) != "undefined") {
            var wmx_model = GetMetaContent("wmx_model");
            BtnTrack(wmx_model, "5", "");
        }
    });
    function UpdateTotalPrice(){
        var sign = "<%=eStore.Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign %>";
        var subtotal = 0;
        $(".totalPrice").each(function(){
            var price = $.trim($(this).html()).replace("&nbsp;", "");
            if(price != "") {
                var result = parseInt(price.replace(/[^0-9.]/g,""), 10) || 0;
                subtotal += result;
            }
        });
        var result = sign + FormatNumber(subtotal).toString();
        $("#total").text(result);
    };
    function FormatNumber(n) { 
	    n += ""; 
	    var arr = n.split("."); 
	    var re = /(\d{1,3})(?=(\d{3})+$)/g; 
	    return arr[0].replace(re,"$1,") + (arr.length == 2 ? "."+arr[1] : ""); 
	} 
</script>
