<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Payway.ascx.cs" Inherits="eStore.UI.Modules.Payment.Payway" %>
<div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Credit_Card_Information)%>
        <div class="paymentnote">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Credit_Card_Note)%>
        </div>
    </div>
    <div class="paymentbody editorpanel">
        <p>
            <label>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cardholder_s_Name)%></label>
            <eStore:TextBox ID="cardholder" runat="server"></eStore:TextBox>
        </p>
        <p>
            <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Type)%></label>
            <asp:DropDownList ID="ddlcardtype" runat="server">
                <asp:ListItem Text="MasterCard" Value="MasterCard" />
                <asp:ListItem Text="VISA" Value="VISA" />
                <asp:ListItem Text="MasterCard" Value="MasterCard" />
                <asp:ListItem Text="American Express" Value="American Express" />
                <asp:ListItem Text="Discover" Value="Discover" />
            </asp:DropDownList>
        </p>
        <p>
            <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Number)%></label>
            <eStore:TextBox ID="cardnumber" runat="server" MaxLength="16" onkeyup="lockNum(this)"></eStore:TextBox>
            &nbsp;(1234567890123456)
        </p>
        <p>
            <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Expiration_Date)%></label>
            <asp:DropDownList ID="creditCardExpireMonth" runat="server">
            </asp:DropDownList>
            -
            <asp:DropDownList ID="creditCardExpireYear" runat="server">
            </asp:DropDownList>
            <span>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Credit_Card_Security_Code)%>:</span>
            <eStore:TextBox ID="CVV2" runat="server" MaxLength="4"></eStore:TextBox>
        </p>
        <p>
            <a class="blueLink" target="_blank" href="/information.aspx?type=PrivacyPolicy">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Commitment_Shopping)%></a> &nbsp;&nbsp;&nbsp;&nbsp; <a class="blueLink" target="_blank"
                    href="/information.aspx?type=ReturnPolicy"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_Policy)%></a>
        </p>
    </div>
</div>
<script type="text/javascript" language="javascript">

    function lockNum(Obj) {
        var theValue = Obj.value;
        if (theValue.match(/[^0-9]/g)) {
            var arr = theValue.match(/[0-9]/g);
            Obj.value = arr != null ? arr.join('') : '0';
        }
    }
    function checkPaymentInfo() {

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
