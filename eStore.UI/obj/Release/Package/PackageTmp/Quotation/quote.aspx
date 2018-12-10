<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="quote.aspx.cs" Inherits="eStore.UI.Quotation.quote" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">

    <eStore:QuotationNavigator ID="QuotationNavigator1" runat="server" QuotationProgressStep="Quotation" />
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:CartContent ID="CartContent1" runat="server" ShowCartMessage="true" />
            </div>
            <!--eStore_order_steps-->
            <div class="eStore_orderStep_AddParts row20">
                <asp:Panel ID="pAddAddtionParts" runat="server" CssClass="adminArea modules">
                    <input type="checkbox" id="ckShowaddtionalparts" />
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Add_addtional_part)%>
                    <fieldset id="fieldsetaddtionalparts" style="display: none;">
                        <eStore:Add2QuoteByPartNo ID="Add2QuoteByPartNo1" runat="server" />
                    </fieldset>
                    <script type="text/javascript" language="javascript">
                        $("#ckShowaddtionalparts").click(function () {
                            if (this.checked == true)
                                $("#fieldsetaddtionalparts").show();
                            else
                                $("#fieldsetaddtionalparts").hide();
                        });
                    </script>
                </asp:Panel>
            </div>
            <div class="eStore_orderStep_subTotal SubTotal_step1 row20">
                <div>
                    <span>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_SubTotal)%></span>
                    <span>
                        <asp:Literal ID="lSubPrice" runat="server"></asp:Literal></span> <span>
                            <asp:Literal ID="lSubStorePrice" runat="server"></asp:Literal></span>
                    <br />
                    <asp:Button ID="btnUpdateAll" runat="server" Text="Update Quote" OnClick="btnUpdateAll_Click"
                        CssClass="eStore_btn borderBlue" OnClientClick="return checkSubQTYInfo()" />
                    <asp:Panel ID="pDiscount" runat="server" Visible="false">
                        <label>
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Discount)%></label>:
                        <asp:Label ID="lTotalDiscount" runat="server" CssClass="colorRed"></asp:Label>
                    </asp:Panel>
                    <eStore:ChangeCurrency ID="ChangeCurrency1" runat="server" />
                </div>
            </div>
            <eStore:VATSetting ID="VATSetting1" runat="server" />
            <div class=" row20">
                <h4>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Comment)%></h4>
                <eStore:TextBox TextMode="MultiLine" runat="server" ID="txtComment" ToolTip="Write your special instruction here"></eStore:TextBox>
            </div>
            <div class="eStore_order_btnBlock row20">
                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" CssClass="eStore_btn borderBlue"
                    OnClick="btnContinueShopping_Click" />
                <asp:Button ID="btnSetContact" runat="server" Text="Next" OnClick="btnSetContact_Click"
                    CssClass="eStore_btn" />
            </div>
            <!--eStore_order_moreInfo-->
        </div>
        <!--eStore_order_content-->
    </div>
    <asp:PlaceHolder ID="phSuggestProsCenter" runat="server"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="eStoreRightSide" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <asp:PlaceHolder ID="phSuggestProsRight" runat="server"></asp:PlaceHolder>
</asp:Content>
