<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShippingCalculator.ascx.cs"
    Inherits="eStore.UI.Modules.ShippingCalculator" %>
<%@ Register Src="PackageDetails.ascx" TagName="PackageDetails" TagPrefix="eStore" %>

<asp:Button ID="btnCalculateShipping" runat="server" Text="Calculate Shipping" ClientIDMode="Static"
    OnClick="btnCalculateShipping_Click" CssClass="eStore_btn borderBlue" />
&nbsp;&nbsp;&nbsp;
<asp:Button ID="btnShowPackingDetails" runat="server" Visible="false" Text="Packing Details"
    ClientIDMode="Static" CssClass="eStore_btn borderBlue" OnClick="btnShowPackingDetails_Click" /><eStore:PackageDetails
        ID="PackageDetails1" runat="server" Visible="false" />
<asp:RadioButtonList ID="rblShippingCarrier" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
    CssClass="ShippingCarrier" RepeatLayout="Flow" OnSelectedIndexChanged="rblShippingCarrier_SelectedIndexChanged">
    <asp:ListItem Value="Recommend" Selected="True">Recommended
        Shipping Service</asp:ListItem>
    <asp:ListItem Value="Customer">My own
        carrier</asp:ListItem>
    <asp:ListItem Value="Dropoff">Direct drop shipment from TW</asp:ListItem>
</asp:RadioButtonList>
<div id="dvShippingMethod" class="eStore_order_shippingService row20">
    <asp:Panel ID="pRecommendedCarrier" runat="server" CssClass="selectMethod">
        <h4>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Select_Shipping_Method)%></h4>
        <p>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Under_normal_circumstances)%></p>
        <div class="eStore_order_radioList show">
            <span forsm="Recommend"></span>
            <asp:Panel ID="pShippingMethodOptions" runat="server" ClientIDMode="Static">
                <div class="eStore_order_radioList_content content1">
                    <asp:RadioButtonList ID="rblShippingRate" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                        CssClass="eStoreShippingTable">
                    </asp:RadioButtonList>
                </div>
                <asp:Literal ID="lShippingRate" runat="server">contact Advantech for shipment arrangement</asp:Literal>
                <script language="javascript" type="text/javascript">
        $("#btnShowPackingDetails").click(function(){});
        $(function () {
            if ($(".selectMethod :radio").length < 5)
            { $(".selectMethod .selectMethodScrollable").removeClass("selectMethodScrollable"); }
        });
        <%if(cart!=null && cart.ShipToContact!=null && cart.ShipToContact.countryCodeX=="CA"){ %>
        $("#<%= pRecommendedCarrier.ClientID %> :radio").change(function(){
            if($(this).val()=="UPS Ground"||$(this).val()=="FedEx International Ground")
            {
            $("#eStoreMainContent_ShippingCalculator1_rblShippingCarrier_1").attr("checked",true);
            var para=$(this).val();
           __doPostBack('ctl00$eStoreMainContent$ShippingCalculator1$rblShippingCarrier$1',para);
            }
         });
        <%} %>
                </script>
            </asp:Panel>
        </div>
    </asp:Panel>
    <div class="eStore_order_radioList">
        <span forsm="Customer"></span>
        <asp:Panel ID="pCustomerCarrier" runat="server" CssClass="selectMethod">
            <div class="eStore_order_radioList_content content2">
                <span>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Account)%>
                    &nbsp;<eStore:TextBox ID="txtCourierAccount" runat="server" Width="100"></eStore:TextBox>
                    &#12288;&#12288;<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Ship_via)%>&nbsp;
                    <asp:DropDownList ID="ddlShipping" runat="server" CssClass="styled">
                    </asp:DropDownList>
                </span>
            </div>
        </asp:Panel>
    </div>
    <div class="eStore_order_radioList special">
        <span forsm="Dropoff"></span>
    </div>

    <div>
        <asp:Literal ID="ltDeliveryMessage" runat="server"></asp:Literal>
    </div>


    <script type="text/javascript">
        $(document).ready(function () {
            $("#dvShippingMethod span[forsm]").each(function (i, n) {
                var item = $("#<%=rblShippingCarrier.ClientID %> input:radio[value='" + $(n).attr("forsm") + "']");
                var lable = item.next("label");
                $(n).append(item).append(lable);
                if (item.val() == "Dropoff" && item.is(":checked")) {
                    $(n).append($("#pShippingMethodOptions"));
                }
            });
        });
    </script>
</div>
