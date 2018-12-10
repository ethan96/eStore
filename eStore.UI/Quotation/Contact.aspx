<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs" Inherits="eStore.UI.Quotation.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/scojs")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <eStore:QuotationNavigator ID="QuotationNavigator1" runat="server" QuotationProgressStep="Address" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:ContactSelector ID="ContactSelector1" runat="server" />
            </div>
            <eStore:ShippingCalculator ID="ShippingCalculator1" runat="server" />
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" PostBackUrl="~/"
                    CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Quotation/quote.aspx"
                    CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" ClientIDMode="Static"
                    CssClass="eStore_btn" OnClientClick="return validUSAaddress();" />
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

        <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableVATSetting"))
        {%>
 
         $(".ContactSelector :radio").click(function () {

             //varify vat
             if(!validselectedvat(this)) {
                alert("The VAT number you entered is not recognized.");
             }
         });

         $("#<%=btnNext.ClientID %>").click(function () {
             var vatObject="";
             $(".ContactSelector :checked").each(function (i, n) {
                 if(!validselectedvat(n)) {
                    vatObject+=$(n).attr("name")+",";
                 }
             });
             if(vatObject!="") {//如果为空,则说明vat正确
                vatObject=vatObject.substr(0,vatObject.length-1);//去掉最后的逗号
                alert("The VAT number you entered of "+vatObject+" is not recognized.");
             }
         });

         function validselectedvat(selecteditem) {
             var billtoradio = $(selecteditem).closest("tr").find(":radio:first");
             var vatNumber = $(billtoradio).attr("vatNumber");
             var vatvalidstatus = $(billtoradio).attr("vatvalidstatus");
             if (vatvalidstatus == "0") {
                 if (!checkVATNumber(vatNumber)) {
                     //alert("The VAT number you entered is not recognized.");

                     eval($(selecteditem).closest("tr").find("td:last a:last").attr("href"));
                     return false;
                 }
                 else {
                    return true;
                 }
             }
             else {
                return true;
             }

         }
         <%} %>
        var needCheck = true;
        $("#<%=btnNext.ClientID %>").click(function () {
            //return SelectedAllContact();
            var selectItem = $("#pShippingMethodOptions span[data-trigger='tooltip'] input[type='radio']:checked");
            if (selectItem.length > 0 && needCheck) {
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
                data: { shippingmethod: shippingservice, type: "quotation" },
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
