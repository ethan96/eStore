<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CTOS.ascx.cs" Inherits="eStore.UI.Modules.CTOS" EnableViewState="false" %>
<%@ Register Src="ProductLiterature.ascx" TagName="ProductLiterature" TagPrefix="eStore" %>
<%@ Register Src="ProductSharetoFriends.ascx" TagName="ProductSharetoFriends" TagPrefix="eStore" %>
<%@ Register Src="QuantityDiscountRequest.ascx" TagName="QuantityDiscountRequest"
    TagPrefix="eStore" %>
<%@ Register Src="YouAreHereMutli.ascx" TagName="YouAreHereMutli" TagPrefix="eStore" %>
<%@ Register Src="ChangeCurrency.ascx" TagName="ChangeCurrency" TagPrefix="eStore" %>
<%@ Register Src="liveperson.ascx" TagName="liveperson" TagPrefix="eStore" %>
<%@ Register Src="SocialNetworkContent.ascx" TagName="SocialNetworkContent" TagPrefix="eStore" %>
<%@ Register src="CMSByKeyWords.ascx" tagname="CMSByKeyWords" tagprefix="eStore" %>
<div id="mainconfigpanel">
    <eStore:YouAreHereMutli ID="YouAreHereMutli1" runat="server" />
    <h1 class="pagetitle">
        <asp:Literal ID="lProductName" runat="server" />
        <asp:Image ID="imgproductStatus" runat="server" /> <span class="productPhaseOut">
        <asp:Literal ID="ltPhaseOut" runat="server" Visible="false"></asp:Literal></span>
    </h1>
    <div class="printandemail">
        <estore:SocialNetworkContent id="SocialNetworkContent1" runat="server"></estore:SocialNetworkContent>
        <asp:ImageButton ID="lAddtoCompareList" runat="server" ImageUrl="~/images/Compare_Icon.jpg"
            OnClick="lAddtoCompareList_Click1"></asp:ImageButton>
        <asp:ImageButton ID="hEmail" runat="server" CssClass="needlogin" ImageUrl="~/images/Email_Icon.jpg">
        </asp:ImageButton>
        <asp:ImageButton ID="hPrint" runat="server" Target="_blank" ImageUrl="~/images/Printpg_Icon.jpg">
        </asp:ImageButton>
    </div>
    <div class="clear">
    </div>
    <h2>
        <asp:Literal ID="lShortDescription" runat="server" />
    </h2>
    <table class="productliteratures">
        <tr>
            <td id="productimages" runat="server" clientidmode="Static" align="center" valign="top">
            </td>
            <td rowspan="2" valign="top">
                <ul class="ProductFeature">
                    <asp:Literal ID="lProductFeature" runat="server"></asp:Literal></ul>
                    <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" Visible="false">
                        <HeaderTemplate>
                            <div class="replaceProduct">
            	                <ul>
                	                <p><%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommending_replacement)%>:</p>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li> 
                                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>" id='<%# Eval("VProductID") %>' name='<%# Eval("name")  %> ' class="jTipProductDetail" >
                                <%# Eval("name")%></a>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </div></FooterTemplate>
                    </eStore:Repeater>
            </td>
        </tr>
        <tr>
            <td id="productframes" runat="server" clientidmode="Static" valign="bottom">
            </td>
        </tr>
        <tr>
            <td valign="top" id="productresources" runat="server" clientidmode="Static">
            </td>
            <td valign="top">
                <fieldset class="productactions">
                    <div class="pricepanel">
                        <asp:Literal ID="lProductprice" runat="server"></asp:Literal>
                        <asp:Label ID="lPriceExtendedDescripton" runat="server"  CssClass="colorRed"></asp:Label>
                        <eStore:ChangeCurrency ID="ChangeCurrency1" runat="server" />
                         <asp:Label ID="LtPromotionMessage" Visible="false" runat="server" CssClass="colorBlue"></asp:Label>
                    </div>
                    <div ID="divFastDelivery" runat="server" Visible="false" class="FastDelivery"></div>
                    <asp:HyperLink ImageUrl="/images/2years.jpg" CssClass="productwarranty" ToolTip="2 years extended warranty" ID="imgproductwarranty" runat="server"></asp:HyperLink>
                    <div class="clear">
                    </div>
                    <div class="rightside">
                        <asp:HyperLink ID="hRequestQuantityDiscountTop" runat="server" CssClass="ctosneedlogin StoreGreyButton StoreButton" Target="_blank">
                            <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount)%></span>
                        </asp:HyperLink>
                        <asp:LinkButton ID="btnUpdateTop" runat="server" CssClass="ctosneedlogin StoreGreyButton StoreButton"
                            OnClick="btnUpdate_Click" Text="Update" />
                        <asp:LinkButton ID="btnAdd2QuoteTop" runat="server" CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck"
                            OnClick="btnAdd2Quote_Click" Text="Add to Quote" />
                        <asp:LinkButton ID="btnAdd2CartTop" runat="server" CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck"
                            OnClick="btnAdd2Cart_Click" Text="Add to Cart" /></div>
                </fieldset>
            </td>
        </tr>
    </table>
    <div class="clear">
    </div>
    <div id="configurationsystem">
        <div id="configItemsTitle">
            <asp:Label ID="lbl_configItemsTitle" Text="Build Your System" runat="server"></asp:Label>
            <asp:Label ID="lbl_configItemsmessage" Text="(Login first to build your system)"
                CssClass="colorRed" Visible="false" runat="server"></asp:Label>
            <div class="expoptionimg expAll">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Hide_all_options)%></div>
        </div>
        <asp:Literal ID="CTOSModules" runat="server" />
        <div class="tableTitle">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Comment)%>
        </div>
        <eStore:TextBox TextMode="MultiLine" ClientIDMode="Static" Width="775px" runat="server"
            ToolTip="Write your special instruction here" ID="txtComment"></eStore:TextBox>
    </div>
    <div class="rightside">
        <asp:HyperLink ID="hRequestQuantityDiscount" runat="server" CssClass="ctosneedlogin StoreGreyButton StoreButton"
            NavigateUrl="#" Target="_blank"><span>Request Quantity Discount</span></asp:HyperLink>
        <asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton" ID="btnUpdate"
            runat="server" Text="Update" OnClick="btnUpdate_Click" />
        &nbsp;<asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck" ID="btnAdd2Quote"
            runat="server" Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
        &nbsp;<asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck" ID="btnAdd2Cart"
            runat="server" Text="Add to Cart" OnClick="btnAdd2Cart_Click" />
    </div><div class="clear">
    </div>
</div>
<asp:PlaceHolder ID="phMenu2013" runat="server"></asp:PlaceHolder>
<div id="floatbtospanel">
    <div id="floatbtos">
        <div class="DarkBlueHeader">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Your_System)%>
        </div>
        <asp:Literal ID="lFloatBTOS" runat="server" />
        <asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton" ID="btnUpdateFloat"
            runat="server" Text="Update" OnClick="btnUpdate_Click" />
        <asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck" ID="btnAdd2QuoteFloat"
            runat="server" Text="Add to Quote" OnClick="btnAdd2Quote_Click" />
        <asp:LinkButton CssClass="ctosneedlogin StoreGreyButton StoreButton SystemIntegrityCheck" ID="btnAdd2CartFloat"
            runat="server" Text="Add to Cart" OnClick="btnAdd2Cart_Click" /></div>
</div>
<div class="clear"></div>
<div id="ProductSharetoFriendsDialog" style="display: none;">
    <eStore:ProductSharetoFriends ID="ProductSharetoFriends1" runat="server" />
</div>
<div id="QuantityDiscountRequestDialog" style="display: none;">
    <eStore:QuantityDiscountRequest ID="QuantityDiscount" runat="server" />
</div>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {

        $(function () {
            $('a.fancybox').fancybox();
        });

        if($("#productframes .frameUl").length>0)
        {
          $("#productframes").tabs({
                    select: function (event, ui) {
                        $("#productimages img").addClass("ui-tabs-hide");
                        $("#productimages img:eq(" + ui.index + ")").removeClass("ui-tabs-hide");
                        $("#productresources div").addClass("ui-tabs-hide");
                        $("#productresources div:eq(" + ui.index + ")").removeClass("ui-tabs-hide");
                 
                    }
                });
        }
        $('#floatbtos').scrollFollow(
			{
			    container: 'floatbtospanel'
			});
        $("#floatbtospanel").height($("#mainconfigpanel").height());
       
        addanchorforspecialcategories();

        <%if( eStore.Presentation.eStoreContext.Current.User==null && eStore.Presentation.eStoreContext.Current.Store.storeID != "AKR"){ %>
         $("#loginDialog .divFeature").removeClass("hiddenitem");
         popLoginDialog(360,480,"View System Page");
         $("#configurationsystem input").attr("disabled","disabled");
         $("#configurationsystem *").click(function () {
            $("#loginDialog .divFeature").removeClass("hiddenitem");
            return popLoginDialog(360,480,"Configuration System");
        }
        );
        <%}%>


        $(".SystemIntegrityCheck").click(function(){
			if(checkResoruce() == false)
                return false;
            <%if(eStore.Presentation.eStoreContext.Current.User!= null && EnableSystemIntegrityCheck) {%>
                    var rlt = SystemIntegrityCheck();
                    return rlt;
            <%} %>
	    });

        $.each($("#module-extended_header :checked"),function(i,checkeditem){
             $(checkeditem).parent().parent().find(".options").show();
              $(checkeditem).parent().parent().parent().find(".extendedmodule").show();
             expandParentCategory($(checkeditem).closest(".extendedmodule"));
        });
    });

    function expandParentCategory(category)
    {
        if(category.length>0){
            $(category).show();
            $(category).children(".moduleheader").removeClass("coloptionimg");
            $(category).children(".moduleheader").addClass("expoptionimg");
            expandParentCategory($(category).parent(".extendedmodule"));
        }
    }

    function addanchorforspecialcategories()
    {
        if (typeof (specialcategories) != "undefined") {
            var strHref=location.href;
            var intPos = strHref.indexOf("#");
            if(intPos>0)
            {
                strHref=strHref.substring(0,intPos);
            }
            for(i=0;i<specialcategories.length;i++)
            {
                if($("#module-"+specialcategories[i]).length>0)
                    {
                    $("#anchorforspecialcategories").append(
                        $("<a>")
                        .attr("href",strHref+"#module-"+specialcategories[i])
                        .html($("#module-"+specialcategories[i] +" .moduleheader .ctosCategory").html())).append($("<br />"));
                    }
            }
        }
    }
    $(".moduleheader").click(function () {

        //  debugger;
        if ($(this).attr("class").match("expoptionimg") == null) {
            $(this).removeClass("coloptionimg");
            $(this).addClass("expoptionimg");
            if($(this).hasClass("extendedmoduleheader"))
            {
                $(this).parent().children(".options").show();
                $(this).parent().children(".extendedmodule").show();
            }
            else
            {
                $(this).parent().find(".options").show();
                $(this).parent().find(".moduleheader").removeClass("coloptionimg");
                $(this).parent().find(".moduleheader").addClass("expoptionimg");
            }
         
        }
        else {
            $(this).addClass("coloptionimg");
            $(this).removeClass("expoptionimg");

            if($(this).hasClass("extendedmoduleheader"))
            {
                $(this).parent().children(".options").hide();
                $(this).parent().children(".extendedmodule").hide();
             }
            else
            {
                $(this).parent().find(".options").hide();
                $(this).parent().find(".moduleheader").addClass("coloptionimg");
                $(this).parent().find(".moduleheader").removeClass("expoptionimg");
            }
        }
        $("#floatbtospanel").height($("#mainconfigpanel").height());
    });
    var showmsg = "Show all options";
    var hiddenmsg = "Hide all options";
    $(".expAll").click(function () {
        if ($(this).attr("class").match("expoptionimg") == null) {
            $(this).removeClass("coloptionimg");
            $(this).addClass("expoptionimg");
            $(this).parent().parent().find(".moduleheader").removeClass("coloptionimg");
            $(this).parent().parent().find(".moduleheader").addClass("expoptionimg");
            $(this).parent().parent().find(".options").show();
            $(this).parent().parent().find(".extendedmodule").show();
            $(this).html(hiddenmsg);
        }
        else {
            $("#configItemsTitle .expAll").addClass("coloptionimg");
            $("#configItemsTitle .expAll").removeClass("expoptionimg");
            $(this).parent().parent().find(".moduleheader").addClass("coloptionimg");
            $(this).parent().parent().find(".moduleheader").removeClass("expoptionimg");
            $(this).parent().parent().find(".options").hide();
            $(this).parent().parent().find(".extendedmodule").hide();
            $(this).html(showmsg);
        }
        $("#floatbtospanel").height($("#mainconfigpanel").height());
    });

    $(".showfloatsystemdetail").click(function () {
        if ($(this).attr("class").match("expoptionimg") == null) {
            $(this).removeClass("coloptionimg");
            $(this).addClass("expoptionimg");
             $("#floatsystemdetail").show("fast");
            $(this).html("Hide Details");
        }
        else {
             $(this).addClass("coloptionimg");
             $(this).removeClass("expoptionimg");
            $("#floatsystemdetail").hide("fast");
            $(this).html("Show Details");
        }
        
    });
    $("#configurationsystem .options input:radio").click(function() {
		selectItem($(this),true);
        checkResoruce();
	});
	$("#configurationsystem .options input:checkbox").click(function() {
        if($(this).attr('checked') == false) {
			$(this).parent().find("input:text").val(0);
		}
        else {
			var cc = $(this).parent().find("input:text").val();
			if(cc <= 0)
				$(this).parent().find("input:text").val(1);
		}
        setNoneItemStatusForMulti(this);
		selectItem($(this),false);
        checkResoruce();
	});
	$("#configurationsystem .options select").click(function() {
        setNoneItemStatusForMulti(this);
		selectItem($(this),true);
        checkResoruce();
	});


    $("#configurationsystem .options input:text").keyup(function() {
		var inputQTY = $(this).val();
        if($.trim(inputQTY) == "") {
            $(this).parent().find("input:checkbox").attr("checked","");
            $(this).val(0);
        }
		else if(inputQTY > 0) {
			$(this).parent().find("input:checkbox").attr("checked",true);
		}
		else {
			$(this).parent().find("input:checkbox").attr("checked","");
		}
        setNoneItemStatusForMulti(this);
        selectItem($(this),false);
        checkResoruce();
	});
    function setNoneItemStatusForMulti(component) {
        if ($(component).closest(".options").find(".optiondesc").text() == "None") {
            if ($(component).closest(".options").find(":checked").length > 0) {
                $(component).closest(".options").siblings().each(function () { 
                    $(this).find(":checkbox").removeAttr("checked");
                    $(this).find(":text").val("0");
                });
            }
            return;
        }
        var noneItem = null;
        var hasoptions = false;
        $(component).closest(".module").find(".options").each(function () {
            if ($(this).find(".optiondesc").text() == "None") {
                noneItem = this;
            }
            else {
                if ($(this).find(":checked").length > 0) {
                    hasoptions = true;
                }
            }
        });
        if (noneItem != null) {
            if (hasoptions) {
                $(noneItem).find(":checkbox").removeAttr("checked");
                $(noneItem).find(":text").val("0");
            }
            else {
                $(noneItem).find(":checkbox").attr("checked", "checked");
                $(noneItem).find(":text").val("1");
            }
        }
    }
    function selectItem(obj,isOnlyOne) {
        //set btos changing
        var currentprice = $("#hdefaultprice").val();
        var ctoModuleID = obj.parent().parent().attr("id").replace("module", "btos");
        insertCheckBoxBtosInfor(obj,ctoModuleID,isOnlyOne);
        
        //set price         
        $(".specialprice span").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
        $(".regularprice span").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
        $(".exchangedprice").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));
        $("#module-2139 .addtionprice").html($("<img>").attr("src","<%=esUtilities.CommonHelper.GetStoreLocation()%>images/priceprocessing.gif"));


        $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx",
            { func:"<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getCBOMPrice %>"
             ,packageid: "<%=esUtilities.CommonHelper.QueryString("packageid") %>"
             ,productID: "<%=esUtilities.CommonHelper.QueryString("ProductID") %>"
             ,para: getBTOSId()
             ,time: new Date().getTime()
             ,addons: getAddons() },
            function (data) {
                $(".specialprice span").html(data.markupprice);
                $(".regularprice span").html(data.listprice);
                $("#hlistprice").val(data.hlistprice);
                if(data.exchangedprice&&data.exchangedprice!="0")
                    $(".exchangedprice").html(data.exchangedprice);
                    else
                        $(".exchangedprice").html("");
               
                if(data.warrantyprice)
                 {
                     $.each(data.warrantyprice,function(i,n){
                        var sum = n.price;
                        var sumSign = "";
                        if(sum >= 0){
                            sumSign = "+";
                        }
                        else if(sum < 0){
                            sumSign = "-";
                        }
                        $("#module-2139 .options").has(":radio[value='"+ n.id+ "']").find(".priceSing").html(sumSign);
                        $("#module-2139 .options").has(":radio[value='"+ n.id+ "']").find(".addtionprice").html(formatNum(Math.abs(sum)));
                     });
                  
                 }
            });
    }

    function insertCheckBoxBtosInfor(obj,ctoModuleID,isOnlyOne){
        var objModel = obj.parent().parent();
        var selectitemprice = 0;
        var floatItem=$("#floatsystemdetail #" + ctoModuleID);
        if(floatItem.length==0)
        {
            floatItem=$("<li></li>").attr("id",ctoModuleID)
            .append($("<span></span>").addClass("btosCategory").html(objModel.find(".moduleheader .ctosCategory").html()))
            .append(" : ")
            .append($("<span></span>").addClass("btosSelectItem"));
            $("#floatsystemdetail ul:last").append(floatItem);
        }
        if(isOnlyOne) {
            selectitemprice = parseFloat(obj.attr("addtion"));
            var optiondesc=obj.siblings(".optiondesc").html();
            if(optiondesc=="" || optiondesc=="None"){
                $(floatItem).replaceWith("");
            }
            else
            {
                $(floatItem).find(".btosSelectItem").html(optiondesc);
            }
            //set header desc
            objModel.find(".moduleheader .ctosSelectItem").html(optiondesc);
        }
        else { // get all select bots then show them
            var inContextStr = "";
		    var inContext = new Array(); 
            objModel.find(".options").each(function() {
			    var ct = $(this).find("input:checkbox");
			    if(ct.attr('checked')==true) {
				    inContext.push($(this).find(".optiondesc").html());
                    var cp = $(this).find("input:checkbox");
			        var cpint = parseFloat(cp.attr("addtion"));
			        selectitemprice = selectitemprice + cpint;
			    }
		    });
		    if(inContext.length > 0) {
			    inContextStr = inContext.join(" | ");
		    }
            if(inContextStr != "") {
                $(floatItem).find(".btosSelectItem").html(inContextStr);
                //set header desc
                objModel.find(".moduleheader .ctosSelectItem").html(inContextStr);
            }
            else { // select None
                $(floatItem).replaceWith("");
                objModel.find(".moduleheader .ctosSelectItem").html("None");
            }
        }
        if (selectitemprice == 0) {
            $("#floatbtos #" + ctoModuleID + " .btosSelectItem").removeClass("btosSelectItemChanged");
        }
        else {
            $("#floatbtos #" + ctoModuleID + " .btosSelectItem").addClass("btosSelectItemChanged");
        }
        if(ctoModuleID!="btos-2139" && ctoModuleID!="btos-21" && obj.attr("type")!="checkbox" && obj.attr("type")!="text")
        {
            objModel.find(".options").each(function () {
                var sum = (parseFloat($(this).find("input:radio").attr("addtion")) - selectitemprice).toFixed(0);
                var sumSign = "";
                if(sum >= 0){
                    sumSign = "+";
                }
                else if(sum < 0){
                    sumSign = "-";
                }
                $(this).find(".priceSing").html(sumSign);
                $(this).find(".addtionprice").html(formatNum(Math.abs(sum)));
            });

        }
	}
    
    function getBTOSId()
    { 
        selectoption=new Array(); 
        $("#configurationsystem input:checked").each(function () {
          selectoption.push(this.name + ":"+this.value);
        });
        $("#configurationsystem input:text").each(function () {
        if(this.value!=""&&this.value!="0")
          selectoption.push(this.name + ":"+this.value);
        });
        return selectoption.join(";");
    }
    function getAddons()
    {
        selectoption=new Array();
        $("#configurationsystem .AddonCategory input:text").each(function () {
        if(this.value!=""&&this.value!="0")
          selectoption.push(this.name + ":"+this.value);
        });
        return selectoption.join(";");
    }
    function getBTOSId2(){
        var rlt=new Array();
        var selectmodule;
        var selectoption;
        $("#configurationsystem .module").each(function () {
            selectmodule=new Array();  
            selectoption=new Array();  
            selectQTY = new Array();
            $(this).find("input:checked").each(function () {
                    selectoption.push(this.value);
                    if($(this).attr("type")!="checkbox") {
                        selectQTY.push(1);
                    }
                    else {
                        selectQTY.push($(this).parent().find("input:text").val());
                    }
                }); 
            var selectoptionStr = "";
            for(var c = 0; c < selectoption.length; c++){
                if(c == 0){
                    selectoptionStr += selectoption[c] + "|" + selectQTY[c];
                }
                else {
                    selectoptionStr += "," + selectoption[c] + "|" + selectQTY[c];
                }
            }
            selectmodule.push(this.id.toString().replace("module-","") + ":" + selectoptionStr);
            rlt.push(selectmodule.toString());
            });
        return rlt.join(";");
    }
    function showProductSharetoFriendsDialog() {
       popupDialog("#ProductSharetoFriendsDialog");
        return false;
    }
    function showQuantityDiscountRequestDialog() {
        popupDialog("#QuantityDiscountRequestDialog");
        return false;
    }
    function showProductSpecsDialog(productid) {
        popupDialogDelay(
        $("<div></div>")
        .load("/proc/html.aspx", { type: "ProductSpecList", ProductID: productid }) , 300);
        return false;
    }
    function showProduct3DModelDialog(productid) {
        popupDialogDelay(
        $("<div></div>")
        .load("/proc/html.aspx", { type: "Product3DModel", ProductID: productid }),300);
        return false;
    }
   

    function SystemIntegrityCheck(){

    var noneedOS=isNoNeed($("#module-<%= System.Configuration.ConfigurationManager.AppSettings["OSID"]%>"));
    var StorageCnt=0;
    var Storages="<%= System.Configuration.ConfigurationManager.AppSettings["StorageID"]%>";
    var StorageIDs=Storages.split(";");
    for(i=0;i<StorageIDs.length;i++)
    {
        if(!isNoNeed($("#module-"+StorageIDs[i])))
        {
            StorageCnt++;
        }
    }

    if(!noneedOS && StorageCnt>1)
    {
        if($("#txtComment").val()==""||$("#txtComment").val()==$("#txtComment").attr("title"))
        {
            alert($.eStoreLocalizaion("please_indicate_which_device_you_would_like_the_OS_installed"));
            $("#txtComment").val($.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive")).css("color", "#000");
            $("#txtComment").focus();
            return false;
        }
        else
        {
            if (confirm($.eStoreLocalizaion("Did_you_tell_us_where_to_install_the_OS_in_the_installation_instruction"))) {
                return true;
            }
            else {

                $("#txtComment").focus();
                return false;
            }

        }
    } 
    else if(noneedOS && StorageCnt>1)
    {
     return confirm($.eStoreLocalizaion("Are_you_sure_you_dont_want_to_select_any_OS"));
    }
    else if(!noneedOS && StorageCnt==0)
    {
        alert($.eStoreLocalizaion("Are_you_sure_you_dont_want_any_storage_device_with_your_OS"));
           return false;
    }
      else if(!noneedOS && StorageCnt==1)
    {
      if($("#txtComment").val()==$.eStoreLocalizaion("Please_install_the_OS_in_Hard_Drive"))
      {
      $("#txtComment").val($("#txtComment").attr("title"));
      }
    }
    }

    function isNoNeed(module){
    if(module!=null&&module.length>0)
        return  $(module).find("input:checked").siblings(".optiondesc").html()=="None";
    else
        return  true;
    }

    
    function checkResoruce(){
        //debugger;
        var resourceSettings=new Array();　
        var overcapacity=new Array();
        $("#configurationsystem").find(":checked[resource],:selected[resource]").each(function(i){
            $(eval($(this).attr("resource"))).each(function(newindex,newitem){
                var exists=false;
                $(resourceSettings).each(function(index,item){
                    if(newitem.ResourceName==item.ResourceName)
                    {
                        item.AvaiableQty+=newitem.AvaiableQty;
                        item.ConsumingQty+=newitem.ConsumingQty;
                        exists=true;
                        return;
                    }
                });
                if(!exists)
                {
                    resourceSettings.push(newitem);
                }
            });
        });

        $("#configurationsystem").find(":text[resource][value!='']").each(function(i){
            if(parseInt($(this).val())!=NaN && parseInt($(this).val())>0)
            {
                var qty=parseInt($(this).val());
                $(eval($(this).attr("resource"))).each(function(newindex,newitem){
                    var exists=false;
                    $(resourceSettings).each(function(index,item){
                        if(newitem.ResourceName==item.ResourceName)
                        {
                            item.AvaiableQty+=newitem.AvaiableQty*qty;
                            item.ConsumingQty+=newitem.ConsumingQty*qty;
                            exists=true;
                            return;
                        }
                    });
                    if(!exists)
                    {
                        newitem.AvaiableQty=newitem.AvaiableQty*qty;
                        newitem.ConsumingQty=newitem.ConsumingQty*qty;
                        resourceSettings.push(newitem);
                    }
                });
             }
        });

        resourceSettings=$.map(resourceSettings, function(n){
            if(n.AvaiableQty>=n.ConsumingQty)
            {
                n.AvaiableQty=n.AvaiableQty-n.ConsumingQty ;
                n.ConsumingQty=0;
            }
            else
            {
                n.AvaiableQty=0;
                n.ConsumingQty=n.ConsumingQty-n.AvaiableQty;
            }
            return n;
        });

        var MultiConsumingResource=$.grep(resourceSettings, function(n,i){
          return n.ConsumingQty>0;
        });
        $(MultiConsumingResource).each(function(ci,cn){
            var result=false;
            $(resourceSettings).each(function(ai,an){
                $(cn.ResourceName.split(",")).each(function(i,r)
                {
                    if(jQuery.inArray(r,an.ResourceName.split(","))>=0 && an.AvaiableQty>=cn.ConsumingQty)
                    {
                        an.AvaiableQty-=cn.ConsumingQty;
                        cn.ConsumingQty=0;
                        return;
                    }
                    else if(jQuery.inArray(r,an.ResourceName.split(","))>=0 && an.AvaiableQty>0 && an.AvaiableQty<cn.ConsumingQty)
                    {
                       
                        cn.ConsumingQty-=an.AvaiableQty;
                        an.AvaiableQty=0;
                    }
                });
            });
            if( cn.ConsumingQty>0)
            {
                $.merge(overcapacity ,cn.ResourceName.split(","));
            }
        });

       $($.grep(resourceSettings, function(n,i){
          return n.AvaiableQty <0 && n.ResourceName.indexOf(",")<0
        })).each(function(i,n){
           overcapacity.push(n.ResourceName);
        });

       if(overcapacity.length>0)
       {
            alert("The selection you made exceeds system's ["+ $.unique(overcapacity).join(",") +"] maximum capacity.");
            return false;
       }
       else
            return true;
        
    } 

    $(".StoreGreyButton").click(function(){
        if (typeof (_wmx) != "undefined") {
            var wmx_model = GetMetaContent("wmx_model");
            BtnTrack(wmx_model, "5", "");
        }
    });
    $(".resourcelist li a").click(function(){
        if (typeof (_wmx) != "undefined") {
            var wmx_model = GetMetaContent("wmx_model");
            BtnTrack(wmx_model, "3", "");
        }
    });

</script>
