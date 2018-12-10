<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorizeNet.ascx.cs" Inherits="eStore.UI.Modules.Payment.AuthorizeNet" %>
<%@ Register Src="PONo.ascx" TagName="PONo" TagPrefix="uc1" %>
<div id="paymentMessage" class="hiddenitem">
    <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CreditCard_Show_Term"))
       {%>
    <asp:PlaceHolder ID="phPaymentMessage" runat="server"></asp:PlaceHolder>
    <!--ordering Rule-->
    <div class="eStore_order_iaAgree">
        <asp:CheckBox ID="checkboxAgreeTerms" ClientIDMode="Static" runat="server" />
        <span class="eStore_redStar colorRed">*</span>I Agree</div>
    <%} %>
</div>
<div class="eStore_order_radioList_content content1">
    <div class="eStore_order_byCreditCard_left">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td width="125">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Type)%>
                </td>
                <td colspan="2" class="CreditCardType">
                    <asp:DropDownList ID="ddlcardtype" runat="server" CssClass="styled">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Number)%>
                </td>
                <td colspan="2">
                    <eStore:TextBox ID="cardnumber" runat="server" MaxLength="16" onkeyup="lockNum(this)"></eStore:TextBox>
                    &nbsp;(1234567890123456)
                </td>
            </tr>
            <tr>
                <td>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Holder_Name)%>
                </td>
                <td colspan="2">
                    <eStore:TextBox ID="cardholder" runat="server"></eStore:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Expiration_Date)%>
                </td>
                <td class="small">
                    <asp:DropDownList ID="creditCardExpireMonth" runat="server" CssClass="styled">
                    </asp:DropDownList>
                </td>
                <td class="small">
                    <asp:DropDownList ID="creditCardExpireYear" runat="server" CssClass="styled">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Credit_Card_Security_Code)%><br />
                    <span class="eStore_redStar"><a href="javascript:;" id="CardVerificationCodeCVV">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_what_is_this)%></a></span>
                </td>
                <td class="small">
                    <eStore:TextBox ID="CVV2" runat="server" MaxLength="4"></eStore:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <!--eStore_order_byCreditCard_left-->
    <div class="eStore_order_byCreditCard_right">
        <table border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td width="45%">
                    <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CreditCard_Show_Federal"))
                       {%>
                    <span class="blue">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_FederalID)%></span>
                    <eStore:TextBox ID="txtFederalID" runat="server"></eStore:TextBox>
                    <%} %>
                </td>
                <td>
                    <uc1:PONo ID="PONo1" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Credit_Card_LongNote)%>
                    <br />
                    <br />
                    <a target="_blank" href="/Privacy-Policy/7.article.htm">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Commitment_Shopping)%>
                    </a>&nbsp;|&nbsp; 
                    <%=eStore.Presentation.eStoreLocalization.Tanslation("Cart_Card_Policy_withlink")%>
                </td>
                <td  valign="bottom" style="padding-left:10px">
                    <div class="McAfee">
                        <script type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=1258746696012f9ee50127116f364086917fd90299165042291691120"></script>
                    </div>
                </td>
            </tr>
        </table>

    </div>
    <!--eStore_order_byCreditCard_right-->
</div>
<div class="CardVerificationCodeCVV hiddenitem">
    <h3>
        Card Verification Value (ccv/cvc/ccv2/cid) Help</h3>
    <h4 class="blueLink">
        What is a Card Verification Value (ccv/cvc/ccv2/cid)?</h4>
    <p>
        The Card Verification Value (ccv/cvc/ccv2/cid) is a 3-digit number found on the
        signature panel on the back of your credit card. It is an additional safeguard that
        helps us validate your purchase and protect against fraud. It is not contained in
        the magnetic stripe information and is therefore not printed on sales receipts.
    </p>
    <h4 class="blueLink">
        Where is the ccv/cvc/ccv2/cid located?
    </h4>
    <p>
        <strong>VISA, MASTERCARD, DISCOVERY &amp; most other cards</strong>
    </p>
    <p>
        You can find your card verification code on the reverse side of your credit card,
        printed into the signature field. It is a 3-digit group for MasterCard, Visa, Discovery,
        and most other cards.
    </p>
    <p>
        <img title=" CVV Help " height="139" alt="CVV Help" src="/images/cvm_help1.jpg" width="200"
            border="0" />
    </p>
    <p>
        <strong>MERICAN EXPRESS</strong>
    </p>
    <p>
        he American Express Card verification code is a 4-digit number printed on the front
        of your card. It appears after and to the right (above) of your card number.
    </p>
    <p>
        <img title=" CVV Help " height="139" alt="CVV Help" src="/images/cvm_help2.jpg" width="200"
            border="0" />
    </p>
</div>

<script type="text/javascript" language="javascript">
    $('#CardVerificationCodeCVV').click(function () {
        popupDialog(".CardVerificationCodeCVV");
    });
    function lockNum(Obj) {
        var theValue = Obj.value;
        if (theValue.match(/[^0-9]/g)) {
            var arr = theValue.match(/[0-9]/g);
            Obj.value = arr != null ? arr.join('') : '0';
        }
    }
    function checkPaymentInfo() {
        <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CreditCard_Show_Term"))
            {%>
        if (!$("#checkboxAgreeTerms").prop("checked")) {
            alert($.eStoreLocalizaion("You_have_to_accept_the_terms_and_conditions_before_continue"));
             return false;
        }
        <%} %>
        if ($('#<%=cardholder.ClientID %>').val() == '') {
           alert($.eStoreLocalizaion("Please_enter_the_Cardholders_Name"));
            $('#<%=cardholder.ClientID %>').focus(); return false;
        }
        if ($('#<%=cardnumber.ClientID %>').val() == '') {
            alert($.eStoreLocalizaion("Please_enter_a_vaild_Card_Number"));
            $('#<%=cardnumber.ClientID %>').focus(); return false;
        }
        if ($('#<%=CVV2.ClientID %>').val() == '') {
            alert($.eStoreLocalizaion("Please_choose_the_3_or_4_digit_number_at_the_back_of_the_card"));
            $('#<%=CVV2.ClientID %>').focus(); return false;
        }
        else {
            return true;
        }
    }

</script>
