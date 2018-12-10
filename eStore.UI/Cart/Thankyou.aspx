<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="Thankyou.aspx.cs" Inherits="eStore.UI.Cart.Thankyou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:CartNavigator ID="CartNavigator1" runat="server" OrderProgressStep="Complete" />
                <eStore:CartThankyou ID="CartThankyou1" runat="server" />
                <eStore:OrderInvoiceDetail ID="OrderInvoiceDetail1" runat="server" />
            </div>
            <!--eStore_order_steps-->
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" CssClass="eStore_btn borderBlue"
                    OnClick="btnContinueShopping_Click" />
                <asp:Button ID="btnPirntOrder" runat="server" Text="Print this Page" CssClass="eStore_btn" />
            </div>
        </div>
        <!--eStore_order_content-->
    </div>
    <div class="hiddenitem">
        <asp:Image ID="imgSiteId" runat="server" /></div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("div[labfor='cartTotal']").show();
        });
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
 <eStore:Advertisement ID="Advertisement1" runat="server"  />
</asp:Content>