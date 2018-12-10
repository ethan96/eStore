<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs" Inherits="eStore.UI.Cart.Cart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">


    <eStore:CartNavigator ID="CartNavigator1" runat="server" OrderProgressStep="Cart" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:CartContent ID="CartContent1" runat="server" ShowCartMessage="true" />
            </div>
            <!--eStore_order_steps-->
            <div class="eStore_orderStep_subTotal SubTotal_step1 row20">
                <div>
                    <span>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_SubTotal)%></span>
                    <span>
                        <asp:Literal ID="lSubPrice" runat="server"></asp:Literal></span> 
                            <asp:Literal ID="lSubStorePrice" runat="server"></asp:Literal>
                    <asp:Panel ID="pDiscount" runat="server" Visible="false">
                        <label>
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Discount)%></label>:
                        <asp:Label ID="lTotalDiscount" runat="server" CssClass="colorRed"></asp:Label>
                    </asp:Panel>
                            <br />
                            <asp:Button ID="btnUpdateAll" runat="server" Text="Update Cart" CssClass="needCheckMOQ eStore_btn borderBlue" OnClick="btnUpdateAll_Click"
            OnClientClick="return checkSubQTYInfo()" />
                    <eStore:ChangeCurrency ID="ChangeCurrency1" runat="server" />
                </div>
            </div>
            <eStore:VATSetting ID="VATSetting1" runat="server" />
            <div class=" row20">
                <h4>
                    <asp:Label ID="lb_Comment" runat="server" Text="Comment"></asp:Label></h4>
                <eStore:TextBox TextMode="MultiLine" runat="server" ID="txtComment"
                    ToolTip="Write your special instruction here"></eStore:TextBox>
            </div>
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnAdd2Quote" runat="server" Text="Add to Quote" CssClass="needCheckMOQ eStore_btn borderBlue"
                    OnClick="btnAdd2Quote_Click" />
                <asp:Button ID="btnContinueShopping" CssClass="eStore_btn borderBlue" runat="server"
                    Text="Continue Shopping" OnClick="btnContinueShopping_Click" />
                <asp:Button ID="btnSetContact" runat="server" Text="Checkout" OnClick="btnSetContact_Click"
                    ClientIDMode="Static" CssClass="eStore_btn needCheckMOQ" />
            </div>


            <asp:PlaceHolder ID="phSuggestProsCenter" runat="server"></asp:PlaceHolder>

            
            <!--eStore_order_moreInfo-->
        </div>
        <!--eStore_order_content-->
    </div>
    <script type="text/javascript">
        $(function () {

            $(".fancybox").fancybox();
        });
    </script>
</asp:Content>
