<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="CheckOut.aspx.cs" Inherits="eStore.UI.Cart.CheckOut" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
     <link href="../App_Themes/V4/bytxtPage.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <eStore:CartNavigator ID="CartNavigator1" runat="server" OrderProgressStep="Payment" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <div class="paymentcontent" runat="server" id="paymentmethodpanel">
                    <div class="eStore_order_step4">
                        <div class="bgGray">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type)%></div>
                    </div>
                </div>
                <!--eStore_order_step4-->
            </div>
            <!--eStore_order_steps-->

            <div id="paymentMsg"></div>

            <div class="eStore_order_payWay row20">
                <div class="eStore_order_radioList" id="dPaymentMethod">
                    <div for="PaymentMethodBefore"></div>
                    <div class="eStore_order_radioList">
                    <asp:PlaceHolder ID="phPaymentInfo" runat="server"></asp:PlaceHolder></div>
                    <div for="PaymentMethodAfter"></div>
                </div>
            </div>
            <!---->
            <asp:CheckBox ID="cbsimulation" runat="server" Checked="false" Text="Simulation?"
                ClientIDMode="Static" OnCheckedChanged="cbsimulation_CheckedChanged" />
            <asp:Literal ID="ltBottomMarked" runat="server"></asp:Literal>
            <div class="eStore_order_btnBlock row20">
                <asp:HyperLink ID="hBacktoComfirm" NavigateUrl="~/Cart/confirm.aspx" runat="server"
                    Text="Back" CssClass="eStore_btn borderBlue"></asp:HyperLink>
                <asp:Button ID="btnCheckout" runat="server" Text="Checkout" OnClick="btnCheckout_Click"
                    CssClass="eStore_btn" />
            </div>
        </div>
        <!--eStore_order_content-->
    </div>
    <div class="hiddenitem">
        <asp:RadioButtonList ID="rblPaymentMethod" runat="server" Width="100%" RepeatDirection="Vertical"
                        AutoPostBack="true" OnSelectedIndexChanged="rblPaymentMethod_SelectedIndexChanged">
                    </asp:RadioButtonList>
    </div>
    <script language="javascript">
        $(document).ready(function () {
            $("form:first")[0].onsubmit = formSubmit;
            $("#paymentMessage").show().appendTo($("#paymentMsg"));
        });

        function formSubmit() {
            //这里用来阻止表单重复提交
            $("form:first")[0].onsubmit = function () { return false; }
            return true;
        }

        $(document).ready(function () {
            var items = $("#<%=rblPaymentMethod.ClientID %> input:radio");
            var findcheck = false;
            items.each(function (i, item) {
                var lable = $(item).next("label");
                if (findcheck) {
                    $("#dPaymentMethod div[for='PaymentMethodAfter']").append($("<div />").addClass("eStore_order_radioList show").append(item).append(lable));
                }
                else {
                    $("#dPaymentMethod div[for='PaymentMethodBefore']").append($("<div />").addClass("eStore_order_radioList show").append(item).append(lable));
                }
                if ($(item).attr("checked") == "checked")
                    findcheck = true;
            });
        });
    </script>
</asp:Content>
