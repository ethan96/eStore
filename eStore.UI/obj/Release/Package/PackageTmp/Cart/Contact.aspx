<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs" Inherits="eStore.UI.Cart.Contact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/scojs")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <eStore:CartNavigator ID="CartNavigator1" runat="server" OrderProgressStep="Address" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:ContactSelector ID="ContactSelector1" runat="server" />
            </div>
            <!--eStore_order_steps-->
            <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("Support_Channel_Partner"))
               {%>
            <div class="eStoreOrderServingParty">
                <input type="hidden" id="hChannelID" name="hChannelID" />
                <input type="hidden" id="hChannelName" name="hChannelName" />
                Serving Party:&nbsp;&nbsp;<span id="spanChannelName"><%=OrderChannelName%></span>&nbsp;&nbsp;
                <input type="button" id="btnChangeChannel" value="Change Serving Party" class="eStore_btn borderBlue" onclick="showChannelPartnerDialog()" />
            </div>
            <%} %>
            <eStore:ShippingCalculator ID="ShippingCalculator1" runat="server" />
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" PostBackUrl="~/"
                    CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnback" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="eStore_btn" ClientIDMode="Static"
                    OnClick="btnNext_Click" OnClientClick="return validUSAaddress();" />
            </div>
        </div>
        <!--eStore_order_content-->
    </div>
    <script type="text/javascript" language="javascript">
   if($("#btnCalculateShipping").length>0)
   {
        var defaultShipto;
        $(function () {
            if ($(".ContactSelector :radio[name='ShipTo']:checked").length == 0)
            { defaultShipto = ""; }
            else
            { defaultShipto = $(".ContactSelector :radio[name='ShipTo']:checked").val(); }
            $(".ContactSelector :radio[name='ShipTo']").bind("change",changeShipTo);
            $(".eStore_ShippingPersonal[lb='hIsBillto']").bind("change",changeBillto);
        });

        function changeShipTo(){
            if (defaultShipto != "" && defaultShipto == $(".ContactSelector :radio[name='ShipTo']:checked").val()) {
                    $("#<%=btnNext.ClientID %>").unbind("click",  AlertReCalculateMsg).removeClass("borderBlue");
                $(".selectMethod :radio").removeAttr("disabled");
                $("#btnCalculateShipping").addClass("borderBlue");
            }
            else {
                $("#<%=btnNext.ClientID %>").bind("click",  AlertReCalculateMsg);
                $(".selectMethod :radio").attr("disabled", "disabled");
                $("#btnCalculateShipping").removeClass("borderBlue");
                $("#<%=btnNext.ClientID %>").addClass("borderBlue");
            }
        }

        function changeBillto(){
            $(".eStore_sameAddress .sameAddressYes[checked]").each(function (i, n) {
                sameAddress($(n));
            })
        }

        function AlertReCalculateMsg()
        {
            alert($.eStoreLocalizaion("Please_click_calculate_shipping_button_and_select_new_shipping_method"));
            return false;
        }        
      }

        <% if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Support_Channel_Partner"))
           {%>
            var currentBillToCuntry;
            var channelID ="";
            var isVatStatus = true;//Vat是否OK

            $(function () {                
                if ($(".eStore_ShippingPersonal[lb='hIsBillto'] input[name='BillTo']:checked").length == 0)
                    currentBillToCuntry="";
                else
                    currentBillToCuntry = $(".eStore_ShippingPersonal[lb='hIsBillto'] input[name='BillTo']:checked").attr("title");
                
                channelID = "<%=OrderChannelID %>";
                if (channelID != "" && currentBillToCuntry != "") {
                    $("#<%=btnNext.ClientID %>").unbind("click",  showChannelPartnerDialog);
                }else{
                    $("#<%=btnNext.ClientID %>").bind("click",  showChannelPartnerDialog);
                    }
            });
            


         $(".ContactSelector :radio").click(function () {
             if ($(this).attr("name") == "BillTo") {
                 if (currentBillToCuntry != $(this).attr("title")) {
                     currentBillToCuntry = $(this).attr("title");
                     showChannelPartnerDialog();
                     //return;
                 }
             }
             //varify vat
             if(!validselectedvat(this,false)) {
                alert("The VAT number you entered is not recognized.");
             }
         });

        var dicVat = new Object();
        dicVat.Keys = new Array();      //键数组
        dicVat.Values = new Array();   //值数组

        function AddVatItem(key, value) {
            var keyCount = dicVat.Keys.length;
            if (keyCount > 0) {
                var vatIndex = -1;
                for (var i = 0; i < keyCount; i++) {
                    if (dicVat.Keys[i] == key) {
                        vatIndex = i;
                        break; //如果存在则不添加
                    }
                }
                if (vatIndex > -1)//修改 追加type
                    dicVat.Values[vatIndex] = dicVat.Values[vatIndex]+"|"+value;
                else{
                    dicVat.Keys.push(key)
                    dicVat.Values.push(value);
                }
            }
            else {
                dicVat.Keys.push(key)
                dicVat.Values.push(value);
            }
        }

         $("#<%=btnNext.ClientID %>").click(function () {
//            $(".ContactSelector :checked").each(function (i, n) {
//                if(!validselectedvat(n,true)) {
//                    var firstVat = $(n).closest("tr").find(":radio:first").attr("vatNumber");
//                    if(firstVat == "" || firstVat == undefined)
//                        firstVat = "null";
//                    AddVatItem(firstVat,$(n).attr("name"));
//                }
//            });
            
            if (dicVat.Keys.length > 0) {//如果为空,则说明vat正确
                var vatMessage = vatTypeMessage = "";
                for (var i = 0; i < dicVat.Keys.length; i++) {
                    var vatTypeArray = dicVat.Values[i].split("|");
                    if (vatTypeArray.length == 3)
                        vatTypeMessage = vatTypeArray[0] + ", " + vatTypeArray[1] + " and " + vatTypeArray[2];
                    else if(vatTypeArray.length == 2)
                        vatTypeMessage = vatTypeArray[0] + " and " + vatTypeArray[1];
                    else
                        vatTypeMessage = vatTypeArray[0];

                    if(i > 0)
                        vatMessage +="\n";
                    vatMessage += "The VAT number, "+ dicVat.Keys[i] +", of "+ vatTypeMessage +" is incorrect.";
                }
                alert(vatMessage);
                dicVat.Keys = new Array();      //重置键数组
                dicVat.Values = new Array();   //重置值数组
            }
         });

         function validselectedvat(selecteditem,isNext) {
             var billtoradio = $(selecteditem).closest("tr").find(":radio:first");
             var vatNumber = $(billtoradio).attr("vatNumber");
             var vatvalidstatus = $(billtoradio).attr("vatvalidstatus");
             if ((vatvalidstatus == "0") || (isNext && vatvalidstatus == "-1")) {
                //Next时,状态为-1时,只跳出提示信息
                if(isNext && vatvalidstatus == "-1")
                    return false;
                //Next时,状态为0并且vat不等于空，只跳出提示信息
                if(isNext && vatvalidstatus == "0" && vatNumber != "")
                    return false;

                if (!checkVATNumber(vatNumber)) {
                    //alert("The VAT number you entered is not recognized.");
                    isVatStatus = false;//VAT statu no ok
                    eval($(selecteditem).closest("tr").find("td:last a:last").attr("href"));
                    return false;
                }
                else{                    
                    return true;
                }
             }
             else
                return true;

         }

            function showChannelPartnerDialog() {
                //Next时,如果VAT需要修改,页面会回发.  IE中ChannelPartner如果返回true. 会跳过这一步
                if(!isVatStatus)
                    return false;
                if (currentBillToCuntry == "") {
                    alert("Please select billto!");
                    return false;
                }
                var channelResult = window.showModalDialog(GetStoreLocation() +"Popup.aspx?popupType=ChannelPartnerType&countryName=" + currentBillToCuntry, "Select ChannelPartner", "scrollbars=yes;resizable=no;help=no;status=no;dialogHeight=380px;dialogwidth=500px;");
                var channelName;
                if (channelResult != "" && channelResult != undefined) {
                    channelID = channelResult.split("||")[0];
                    channelName = channelResult.split("||")[1];
                }else
                {
                    channelID = "";
                    $("#spanChannelName").html("N/A");
                    //$("#<%=btnNext.ClientID %>").bind("click",  showChannelPartnerDialog);
                    return false;
                }
                if (channelID != "" && channelID != undefined && channelName != undefined) {
                    $("#hChannelID").val(channelID);
                    $("#hChannelName").val(channelName);
                    $("#spanChannelName").html(channelName);
                    $("#<%=btnNext.ClientID %>").unbind("click",  showChannelPartnerDialog);
                    return true;
                }
                else                
                    return false;
            }
        <%} %>
        var needCheck = true;
        $("#<%=btnNext.ClientID %>").click(function () {
            //return SelectedAllContact();
            var selectItem = $("#pShippingMethodOptions span[data-trigger='tooltip'] input[type='radio']:checked");
            if (selectItem.length > 0 && needCheck)
            {
                popupMessagewithTitle("warning", selectItem.parent("span").attr("data-content"));
                needCheck = false;
                return false;
            }
        });
        function showContactDialog(){
            $(".showShip").click();
        }
        function validUSAaddress() {
            <%if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true)
        {%>
            var flag = 2;
            var msg = "";
            var shippingservice = $(".selectMethod #pShippingMethodOptions :radio:checked").val();
            if (shippingservice == null || shippingservice == "" || shippingservice.length == 0)
                shippingservice == '<%=eStore.BusinessModules.AddressValidator.ValidatationProvider.UPS.ToString()%>';
            $.ajax({
                url: "<%=esUtilities.CommonHelper.GetStoreLocation() %>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.ValidateUSAShiptoAddress %>",
                data: { shippingmethod: shippingservice },
                type: "GET",
                async: false,
                success: function (data) {
                    if (!!data) {
                        flag = data.result;
                        msg = data.message;
                    }
                },
                error: function (data) {
                }
            });

            if (flag == 0) {
                alert(msg);
                return false;
            }
            else if (flag == 1) {
                return confirm(msg);
            }
            else
                return true;
            <% }
        else
        {%>
            return true;
            <%}%>
        }
    </script>
</asp:Content>
