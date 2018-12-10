<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="confirm.aspx.cs" Inherits="eStore.UI.Quotation.confirm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/order")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/order")%>
</asp:Content>
<asp:Content ID="eStoreContent" ContentPlaceHolderID="eStoreMainContent" runat="server">


    <eStore:QuotationNavigator ID="QuotationNavigator1" runat="server" QuotationProgressStep="Confirm" />
    <asp:Panel ID="pWaitingConfirmInfo" runat="server">
        <h1>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Confirm_Quotation)%>
        </h1>
    </asp:Panel>
    <asp:Panel ID="pThankyouInfo" runat="server" CssClass="quotethankyouinfo" Visible="false">
        <h2>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Thank_You)%>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Your_Quotation_has_been_saved)%>
        </h2>
        <p>
            - <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_An_email_has_been_sent_to_your_address)%>
        </p>
        <p>
            - <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_You_may_access_your_Quotation)%>
        </p>
    </asp:Panel>
    <asp:Panel ID="pConfirmedInfo" runat="server" Visible="false">
        <h1 class="pagetitle">
            <label>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%>
                :</label>
            <span style="font-weight:bold"><asp:Literal ID="lQuotationNumber" runat="server"></asp:Literal></span>
        </h1>
        <div class="printandemail">
   
            <asp:HyperLink NavigateUrl="~/Quotation/myquotation.aspx" runat="server" CssClass="productcompare"
                Visible="false" ID="hmyquotation" Text="My Quote"></asp:HyperLink><asp:Literal
                    ID="lseparator" runat="server" Visible="false">|</asp:Literal>
                         <asp:LinkButton ID="lbtnemailquote" runat="server" CssClass="productemail"
                Visible="false"  Text="Email Quote" onclick="lbtnemailquote_Click"></asp:LinkButton>
                <asp:Literal
                    ID="lseparator2" runat="server" Visible="false">|</asp:Literal>
            <asp:HyperLink NavigateUrl="~/Quotation/printquotation.aspx" runat="server" ID="hprintorder"
                Visible="false" Target="_blank" Text="Print Quote" CssClass="productprint"></asp:HyperLink>
        </div>
        <div class="clear">
        </div>
        <p>
            <label>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Date)%>:
            </label>
            <asp:Literal ID="lQuoteDate" runat="server"></asp:Literal>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <label>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%>:</label>
            <asp:Literal ID="lQuoteExpiredDate" runat="server"></asp:Literal></p>
    </asp:Panel>
    <div class="eStore_container eStore_block980">
        <div class="eStore_order_content">
            <div class="eStore_order_steps row20">
                <eStore:QuotationContentPreview ID="QuotationContentPreview1" runat="server" />
            </div>
            <!--eStore_order_steps-->
            <div class="eStore_orderStep3_moreInfo row20">
                <div id="DivPromoteCode" runat="server">
                    <eStore:ResaleSetting ID="ResaleSetting1" runat="server" ResellerType="Quotation" />
                    <eStore:ResaleSetting_CNPJ ID="ResaleSetting_CNPJ1" runat="server" />
                    <div class="eStore_order_radioList">
                        <input type="radio" name="1" /><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Promotion_Code)%>
                        <eStore:TextBox ID="txtPromoteCode" runat="server"></eStore:TextBox>
                <asp:Button ID="btnApplyPromoteCode" runat="server" OnClick="btnApplyPromoteCode_Click" CssClass="eStore_btn borderBlue" Text="Apply" />
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
                <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" PostBackUrl="~/" CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnRevise" runat="server" Text="Revise" OnClick="btnRevise_Click" CssClass="eStore_btn borderBlue" />
                <asp:Button ID="btnReleasetoOrder" runat="server" Text="Checkout" OnClick="btnReleasetoOrder_Click" CssClass="eStore_btn" />
                <asp:Button ID="btnNext" ClientIDMode="Static" runat="server" Text="Save Quote" OnClick="btnconfirm_Click" CssClass="eStore_btn" />
            </div>
        </div>
        <!--eStore_order_content-->
    </div>
        <fieldset id="fTransferUser" runat="server" class="adminArea CartActionArea">
        <eStore:TextBox ID="txtTransferUser" runat="server" CssClass="textEntry InputValidation eStoreText"></eStore:TextBox>
        <asp:Button ID="btnTransfer" runat="server" Text="Transfer" CssClass="adminbutton eStore_btn borderOrange" OnClick="btnTransfer_Click" />
    </fieldset>
    <input type="hidden" id="hChannelID" name="hChannelID" />
    <input type="hidden" id="hChannelName" name="hChannelName" />
    <script type="text/javascript">
        <% if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("Support_Channel_Partner")) {%>
            var currentBillToCuntry = "<%= billtoCountry %>";
            var channelID ="";            

            $(function () {                
                $("#<%=btnReleasetoOrder.ClientID %>").bind("click", showChannelPartnerDialog);

                //Delete center popup setting in cookie to prevent banner cannot show
                $.each(document.cookie.split(/; */), function (index, cookieString) {
                    var splitCookie = cookieString.split('=');
                    if (splitCookie[0].indexOf("centerpopup") > -1) {
                        $.removeCookie(splitCookie[0], { path: '/' });
                    }
                });
            });
            
            function showChannelPartnerDialog() {
                if (currentBillToCuntry == "") {
                    alert("Please select billto!");
                    return false;
                }
                var channelResult = window.showModalDialog(GetStoreLocation() +"Popup.aspx?popupType=ChannelPartnerType&countryName=" + currentBillToCuntry + "&isQuoteToOrder=true", "Select ChannelPartner", "scrollbars=yes;resizable=no;help=no;status=no;dialogHeight=380px;dialogwidth=500px;");
                var channelName;
                if (channelResult != "" && channelResult != undefined) {
                    channelID = channelResult.split("||")[0];
                    channelName = channelResult.split("||")[1];
                }else
                {
                    channelID = "";
                    $("#<%=btnReleasetoOrder.ClientID %>").bind("click",  showChannelPartnerDialog);
                    return false;
                }
                if (channelID != "" && channelID != undefined && channelName != undefined) {
                    $("#hChannelID").val(channelID);
                    $("#hChannelName").val(channelName);
                    $("#<%=btnReleasetoOrder.ClientID %>").unbind("click",  showChannelPartnerDialog);
                    return true;
                }
                else                
                    return false;
            }
        <%} %>
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
 <eStore:Advertisement ID="Advertisement1" runat="server"  />
</asp:Content>