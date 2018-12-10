<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    EnableViewState="true" CodeBehind="confirm.aspx.cs" Inherits="eStore.UI.Cart.confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">


    <eStore:CartNavigator ID="CartNavigator1" runat="server" OrderProgressStep="confirm" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:OrderContentPreview ID="OrderContentPreview1" runat="server" />
            </div>
            <!--eStore_order_steps-->
            <div class="eStore_orderStep3_moreInfo row20">
                <div id="DivPromoteCode" runat="server">
                    <eStore:ResaleSetting ID="ResaleSetting1" ResellerType="Order" runat="server" />
                    <eStore:ResaleSetting_CNPJ ID="ResaleSetting_CNPJ1" runat="server" />
                    <div class="eStore_order_radioList" id="PromotionPanel" runat="server">
                        <input type="radio" name="1" /><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Promote_code)%>
                        <eStore:TextBox ID="txtPromoteCode" runat="server"></eStore:TextBox>
                        <asp:Button ID="btnApplyPromoteCode" runat="server" OnClick="btnApplyPromoteCode_Click"
                            CssClass="eStore_btn borderBlue" Text="Apply" />
                    </div>
                </div>
            </div>
            <div id="cartTotal" class="eStore_orderStep_subTotal row20">
                <script type="text/javascript">
                    $(document).ready(function () {
                        $("div[labfor='cartTotal']").show().appendTo("#cartTotal");
                    });
                </script>
            </div>
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnBack2Contact" runat="server" CssClass="eStore_btn borderBlue"
                    Text="Back" OnClick="btnBack2Contact_Click" />
                <asp:Button ID="btnNext" ClientIDMode="Static" runat="server" Text="Payment" OnClick="btnNext_Click"
                    CssClass="eStore_btn" />
            </div>
        </div>
        <!--eStore_order_content-->
    </div>
</asp:Content>
