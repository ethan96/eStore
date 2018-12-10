<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="PeculiarCategory.aspx.cs" Inherits="eStore.UI.Product.PeculiarCategory" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/peculiarCategoryCss")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/peculiarCategoryJs")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="icgContext <%=categoryName.ToLower() %>_theme">
        <div id="ictTreeContext">
            <div id="icgTree">
                <asp:Repeater ID="rpCategories" runat="server" OnItemDataBound="rpCategories_ItemDataBound"  EnableViewState="false">
                    <HeaderTemplate>
                        <table id="wizard">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="category">
                                <%#Eval("Translation_Name")%>
                            </td>
                        </tr>
                        <td>
                            <div class="icg-addSubCatelay">
                                <div dataid="<%#Eval("CATEGORY_ID") %>" class="<%=RBUStyles %>">
                                    <asp:Literal ID="ltCategoryTree" runat="server"></asp:Literal>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                        </td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div class="clear">
                </div>
                <div class="rightside">
                    <a id="Reset" onclick="Reset()">
                        <input type="button" class="button" value="Reset" /></a>
                </div>
            </div>
        </div>
        <div id="productContext">
            <div id="products">
            </div>
            <div id="icgProListBottom" style="display: none;">
                <div id="productsTemp">
                </div>
                <div id="icgCompareButton">
                    <asp:Button ID="btConpare" runat="server" Text="" CssClass="button" OnClick="btConpare_Click" Visible="false"
                        OnClientClick="return CompareCheck();" />
                </div>
                <div class="clear">
                </div>
            </div>
        </div>
        <div>
            <a id="btnCompare" href="#" target="_blank" class="button" style="position: absolute;"  onclick="return CompareCheck();"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_ProductCompare) %></a>
        </div>
    </div>
    <div class="float-right ">
        <div id="storeSideAds">
        </div>
    </div>
    <div id="appendixDiv1" style="text-align: center; display: none;" class="appendix_table">
    </div>
    <div id="appendixDiv2" style="text-align: center; display: none;" class="appendix_table">
    </div>
    <div id="appendixDiv3" style="text-align: center; display: none;" class="appendix_table">
    </div>
    <script type="text/javascript">
        var sp = [];
        $(document).ready(function () {
            
            $.fn.hideOption = function() {
                this.each(function() {

                    $(this).wrap('<span>').hide()
                });

            }
            $.fn.showOption = function() {
                this.each(function() {
                    var opt = $(this).find('option').show();
                    $(this).replaceWith(opt)
        
                });
     
            }
            getProducts(<%=category.CATEGORY_ID %>,true, 1, <%=this.appendixID%>);
            $("select.willgetSelect").on("change", getSelectPro);
            $(":checkbox.willgetSelect").on("click", getSelectPro);

            if('<%=categoryName %>' == "ECG") {

                $.each($(".icg-addSubCate-ECG div"),function(){
                    if($(this).width() < 120) {
                        $(this).addClass("ecgLeftline");
                    }
                    else {
                        $(this).addClass("ecg-addSubCatelayFull");
                    }
                });
                $.each($(".icg-addSubCate-ECG div select"),function(){
                    if($(this).width() < 120) {
                        $(this).addClass("ecg-addSubCatelayHalf");
                    }
                    else {
                        $(this).addClass("ecg-addSubCatelayFull");
                    }
                });

                $.each($(".icg-addSubCate-ECG div"),function(){
                    var p = $(this).position();
                    if(p.left > 240){
                        $(this).removeClass("ecgLeftline");
                        $(this).addClass("ecg-addSubCatelayHalf");
                    }
                });

                $.each($(".ecg-addSubCatelayHalf select.willgetSelect .willgetSelect"), function(){
                    //$(this).attr("disabled", "disabled").hide();
                    $(this).hideOption();
                });

                $(".doubleListSelect").change(function(){
                    var pid = $(this).attr("name");
                    var pc = $(this).children(":selected").val();

                    $.each($("#" + pid).children("option"), function(i, n){
                        if($(n).val() != "0"){
                            $(n).hideOption();
                        }
                    });

                    if(pc == "0"){
                        $("#" + pid).val("0");
                        getSelectPro();
                    }
                    else{
                        $.each($("#" + pid + " span"), function(i, n){
                            if(pc == $(n).children("option").attr("name")){
                                $(n).showOption();
                            }
                        });
                        $("#" + pid).val("0");
                        getSelectPro();
                    }
                });
            }

            $('#btnCompare').attr("href", GetStoreLocation() + "Compare.aspx");
            var $bc = $('#btnCompare'), _top = $bc.position().top;
            var $win = $(window).scroll(function(){
                var winTop = $win.scrollTop();
                if(_top -  winTop < _top){
                    if($bc.css('position') != 'fixed'){
                        $bc.css({
                            position: 'fixed',
                            top: winTop + _top
                        });
                    }
                }else{
                    $bc.css({
                        position: 'absolute',
                        top : _top
                    });
                }
            });
        });

        function getProducts(ids, isroot, p, appendixID) {
            $("#products").html($("<img />").attr({ "class": "linLoadImg", "src": GetStoreLocation() + "images/Loading.gif" }));
            $("#icgProListBottom").hide();
            var myPage = 1;
            var cb = "";
            $.getJSON(GetStoreLocation() + "eStoreScripts.asmx/GetProduct",{ data: ids, isRoot: isroot, page: p, appendix: appendixID}, function (res) {
                var s = "";
                if(res.length > 0){
                    myPage = res[0].Count;
                }
                $.each(res, function (i, n) {
                    s += "<div class='results'>";
                    s += "<table border='0' width='520' class='resultsTable' categoryids='"+n.Categoryids+"'><tbody><tr>";
                    s += "<td valign='top'><a target=_blank href='" + n.URL + "'><img src='" + n.TumbnailimageID + "' width='160' height='160'></a><h5>" + n.SProductID + "</h5></td>";
                    s += "<td><div>";
                    s += "<h3 class='model'><span>" + n.Desc + "</span></h3>";
                    s += "<ul class='description'>" + n.VendorFeatures + "</ul></div>";
                    s += "<table class='compareBoxTable'><tbody><tr class='buttons'>";
                    s += "<td width='5'></td>";
                    if (sp.length > 0 && $.inArray(n.SProductID, sp) > -1)
                        cb = "checked='checked'";
                    else cb = "";
                    s += "<td class='compare'> <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare)%> <input type='checkbox' name='temperature' value='" + n.SProductID + "' " + cb + " onclick='CalculateCP(this);'></td>";
                    s += "<td width='5'></td>";
                    s += "<td class='datasheet'><a target=_blank href='" + n.DataSheet + "'>Datasheet &nbsp;<img src='"+GetStoreLocation()+"images/link_arrow.png' alt='Datasheet' width='15' height='15' valign='top'></a></td>";
                    s += "<td width='5'></td>";
                    if (appendixID > 0) {
                        if(!!n.Appendix && n.Appendix.length > 0)
                            s += "<td class='more'><a href='javascript: void(0)' onclick='ShowAppendix(" + JSON.stringify(n.Appendix) + ");'>iDoor<img src='"+GetStoreLocation()+"images/link_arrow.png' alt='iDoor' width='15' height='15' valign='top' /></a></td>";
                        s += "<td width='5'></td>";
                        s += "<td class='sproduct'><a target=_blank href='" + n.URL + "'><span id=\"" + n.SProductID + "\" class=\"ajaxproductprice displayblock\"><img alt=\"loading..\" src=\""+GetStoreLocation()+"images/priceprocessing.gif\" /></span></a></td>";
                        s += "</tr></tbody></table>";
                        s += "</td></tr></tbody></table></div>";
                    }
                    else {
                        s += "<td class='more'><a target=_blank href='" + n.URL + "'><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%>&nbsp;>></a></td>";
                        s += "<td width='5'></td>";
                        s += "<td class='sproduct'><span id=\"" + n.SProductID + "\" class=\"ajaxproductprice displayblock\"><img alt=\"loading..\" src=\""+GetStoreLocation()+"images/priceprocessing.gif\" /></span></td>";
                        s += "</tr></tbody></table>";
                        s += "</td></tr></tbody></table></div>";
                    }
                });
            
                if(s == "") {
                    s += "<div class='emptymessage'>";
                    s+="<p>You current selections have no exact product match.";
                    s+="<br />Please modify your selections to see the alternative products.</p>";
                    s+=" <a id='Reset2' onclick='Reset()'><input type='button' class='button' value='Reset' /></a>";
                    s += "</div>";
                    $("#products").html(s + "<div class='clear'></div>");
                    $("#icgProListBottom").hide();
                }
                else {
                    if(isroot){ 
                        s = "<h3 class='buttomLine'>Featured Products</h3>"+s;
                    }   
                    $("#products").html(s + "<div class='clear'></div>");
                    $("#icgProListBottom").show();
                }
                if(p == 1){
                    $("#productsTemp").bindPagination(myPage,10);
                }

            //$.each($(".category-count"),function(i,cate){
                //$(cate).html($(cate).attr("oldText"));
            //});
            //var objs = {};
            //$.each($("#products .resultsTable"),function(index,obj){
                //$.each($(obj).attr("categoryids").split(','),function(i,cid){
                    //if(objs[cid] == null)
                       //objs[cid] = 1;
                    //else
                      //objs[cid] = objs[cid] + 1;   
                //});
            //});

            //$.each(objs,function(i,obj){
                //$("#c"+i).html($("#c"+i).attr("oldText") + " ["+obj+"]");
                //$("#c"+i).html($("#c"+i).attr("oldText"));
            //});

            //var catetree = {};
            //$.each($(".willgetSelect:input[type='checkbox'],.willgetSelect option"),function(i,n){
                //if($(n).val() != "0")
                //{
                    //if(objs[$(n).val()] != null)
                        //catetree[$(n).val()] = 1;
                    //else 
                        //catetree[$(n).val()] = 0;
                //}
            //});

            var rbu = '<%=RBUStyles %>';
            //$.each(catetree,function(i,n){
                //if(n == 0)
                //{
                    //if($("#inp"+i).attr("type") == "checkbox")
                        //$("#c"+i).parents(".ckbItemLine").addClass("willHide");
                    //else
                        //$("#c"+i).addClass("willHide");
                //}
                //else
                //{
                   //if($("#inp"+i).attr("type") == "checkbox"){
                        //$("#c"+i).parents("." + rbu).find("div.ckbItemLine").show();
                    //}
                    //else
                        //$("#c"+i).show();

                //}
            //});

            $(".willgetSelect:input[type='checkbox']:checked").each(function(i,n){
                $(n).parents("." + rbu).find(".ckbItemLine").removeClass("willHide");
            });
            $(".icgPower").each(function(){
                $(this).show();
             });
            $(".icgPower").children().children(".willgetSelect:input[type='checkbox']:checked").each(function(i,n){
                var id = $(n).attr("name");
		        $(".icgPower").each(function(){
                    var divid = $(this).attr("id");
		            //if(id != divid)
                    //    $(this).hide();
                    //else
		            //    $(this).show();
                    $(this).show();
                });
            });
            $(".willgetSelect option:selected").each(function (i, n) {
                if($(n).val() != 0)
                    $(n).parents("select").find("option").removeClass("willHide");
            });

            $(".willHide").hide()

            showProductAjaxPric();
        });
        $(".willHide").removeClass("willHide");
    }

    jQuery.fn.bindPagination = function (itemsseletor, pagesize) {
        var itemsOnPage = pagesize;
        $(this).pagination({
            items: itemsseletor,
            itemsOnPage: itemsOnPage,
            onPageClick: function (pageNumber, event) { return showpaginationproducts(pageNumber, pagesize); }
        });
    }

    function showpaginationproducts(page, pagesize) {
          $("html, body").scrollTop(0);
          getSelectProByPage(page);
          return false;
    }

    function getSelectPro() {
        var list = FilterList();
        if(list.length == 0)
            getProducts(<%=category.CATEGORY_ID %>,true, 1, <%=this.appendixID%>);
        else
            getProducts(list.toString(), false, 1, <%=this.appendixID%>);
    }

    function getSelectProByPage(page) {
        var list = FilterList();
        if(list.length == 0)
            getProducts(<%=category.CATEGORY_ID %>,true, page, <%=this.appendixID%>);
        else
            getProducts(list.toString(), false, page, <%=this.appendixID%>);
    }

    function FilterList(){
        var list = new Array();
        $(".willgetSelect:input[type='checkbox']:checked").each(function(i,n){
            list.push($(n).val());
        });
        $(".willgetSelect option:selected").each(function (i, n) {
            if ($(n).val() != "0") {
                list.push($(n).val());
            }
        });
        if('<%=categoryName %>' == "ECG"){
            $.each($(".doubleListSelect"), function(){
                var pid = $(this).attr("name");
                var pc = $(this).children(":selected").val();
                if(pc != "0"){
                    var cv = $("#" + pid).children(":selected").val();
                    if(cv == "0"){
                        $.each($("#" + pid + " option"), function(i,n){
                            if(pc == $(n).attr("name")){
                                list.push($(n).val());
                            }
                        });
                    }
                }
            });
        }
        return list;
    }

    function Reset() {
        $(".willgetSelect:input[type='checkbox']:checked").each(function(i,n){
            $(n).attr("checked", false);
        });
        $(".willgetSelect option").each(function (i, n) {
            if($(n).val() == 0) {
                $(n).attr("selected", true);
            }
            else {
                $(n).attr("selected", false);
            }
        });
        $(".doubleListSelect option").each(function (i, n) {
            if($(n).val() == 0) {
                $(n).attr("selected", true);
            }
            else {
                $(n).attr("selected", false);
            }
        });
        $.each($(".ecg-addSubCatelayHalf select.willgetSelect .willgetSelect"), function(){
            $(this).hide();
        });
        $("html, body").scrollTop(0);
        getSelectPro();
        CalculateCP(null);
    };

    function CompareCheck(){
        var result = false;
        if(sp.length == 0)
            alert('<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_select_product_first)%>');
        else {
            $.ajax({
                url: GetStoreLocation() + "eStoreScripts.asmx/SetCompare",
                type: "GET",
                data: { data: sp.toString() },
                async: false,
                success: function (retData) {
                    result = true;
                }
            });
        }
        return result;
    }

        function ShowAppendix(appendix){

            var n1 = [];
            var n2 = [];
            var n3 = [];
            var n4 = [];
            for(var i = 0; i < appendix.length; i++){
                switch(appendix[i].Category.split(",")[0]){
                    case "1":
                        n1.push(appendix[i]);
                        break;
                    case "2":
                        n2.push(appendix[i]);
                        break;
                    case "3":
                        n3.push(appendix[i]);
                        break;
                    case "4":
                        n4.push(appendix[i]);
                        break;
                    default:
                        break;
                }
            }

            var count = 0;
            if(n1.length > count)
                count = n1.length;
            if(n2.length > count)
                count = n2.length;
            if(n3.length > count)
                count = n3.length;
            if(n4.length > count)
                count = n4.length;

            if(count > 10)
                count = 3;
            else if(count > 5)
                count = 2;
            else
                count = 1;

            var div1 = "";
            var div2 = "";
            var div3 = "";

            for(var i = 0; i < count; i++){
                switch(i){
                    case 0:
                        div1 += GetAppendixContent(1, 0, 4, n1);
                        div1 += GetAppendixContent(2, 0, 4, n2);
                        div1 += GetAppendixContent(3, 0, 4, n3);
                        div1 += GetAppendixContent(4, 0, 4, n4);
                        break;
                    case 1:
                        div2 += GetAppendixContent(1, 5, 9, n1);
                        div2 += GetAppendixContent(2, 5, 9, n2);
                        div2 += GetAppendixContent(3, 5, 9, n3);
                        div2 += GetAppendixContent(4, 5, 9, n4);
                        break;
                    case 2:
                        div3 += GetAppendixContent(1, 10, 14, n1);
                        div3 += GetAppendixContent(2, 10, 14, n2);
                        div3 += GetAppendixContent(3, 10, 14, n3);
                        div3 += GetAppendixContent(4, 10, 14, n4);
                        break;
                    default:
                        break;
                }
            }

            var gallery = [];
            if (div1 != "") {
                $("#appendixDiv1").html(div1);
                gallery.push({ href: "#appendixDiv1"});
            }
            if (div2 != "") {
                $("#appendixDiv2").html(div2);
                gallery.push({ href: "#appendixDiv2"});
            }
            if (div3 != "") {
                $("#appendixDiv3").html(div3);
                gallery.push({ href: "#appendixDiv3"});
            }
            $.fancybox(gallery, {
                'autoSize': false,
                'width': 800,
                'height': 400,
                'autoCenter': true,
            });
        }

        function GetAppendixContent(appendixID, startIndex, endIndex, array) {
            var iDoorTitle = { 1 : "Smart I/O & Comm.", 2 : "Multiple I/O & Peripheral", 3 : "Communication", 4 : "Industrial Fieldbus" };
            //var iDoorBgColor = { 1: "#1AAA9F", 2: "#169E53", 3: "#0D61A7", 4: "#FC5255"};
            var html = "";
            var count = endIndex;
            if (array.length < endIndex)
                count = array.length;

            html += "<div class='appendix_tr'><div class='appendix_td' style='background-color: #f3f3f3'><div class='appendix_title'>" + iDoorTitle[appendixID] + "</div></div>";
            for(var i = startIndex; i < count; i ++) 
                html += ("<div class='appendix_td'><a target='_blank' href='" + array[i].ItemURL + "'><img src='" + array[i].ImageURL + "' height='60' width='60' / ></a><br /><span class='unoFont'>" + array[i].Category.split(",")[1] + "</span></div>");
            html += "</div>";
            return html;
        }

        function CalculateCP (node){
            var text = '<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_ProductCompare) %>';

            if (node == null)
                sp = [];
            else {
                if($(node).prop("checked") == true) {
                    var product = $(node).val();
                    if ($.inArray(product, sp) === -1)
                        sp.push(product);
                }
                else {
                    var product = $(node).val();
                    if ($.inArray(product, sp) !== -1)
                        sp.pop(product);
                }
            }
            if (sp.length > 0)
                $('#btnCompare').text(text + " (" + sp.length.toString() + ")");
            else
                $('#btnCompare').text(text);
        }
    </script>
</asp:Content>
