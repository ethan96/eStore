<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Veritas.ascx.cs" Inherits="eStore.UI.Modules.Payment.Veritas" %>
<div class="paymentcontent">
    <div class="paymentheader">
        クレジットカード
    </div>
    <div class="paymentbody editorpanel">
        <p>
            <label>
                Cardholder&#39;s Name: &nbsp;</label>
            <eStore:TextBox ID="cardholder" runat="server"></eStore:TextBox>
        </p>
        <p>
            <label>
                カード番号: &nbsp;</label>
            <eStore:TextBox ID="cardnumber" runat="server"></eStore:TextBox></p>
        <p>
            <label>
                ご利用可能なカード:&nbsp;</label>
            <img src="/images/creditcard_cards.gif" alt="CreditCard" /></p>
        <p>
            <label>
                有効期限: &nbsp;</label>
            <asp:DropDownList ID="creditCardExpireMonth" runat="server">
            </asp:DropDownList>
            -
            <asp:DropDownList ID="creditCardExpireYear" runat="server">
            </asp:DropDownList>
        </p>
        <p>
            お客様の情報はSSLにより暗号化されて送信されます。 オンラインで照会致しますので、確定ボタンを押した後、少々お待ち下さい。 （10秒から30秒）
        </p>
        <p>
            <label>
                &nbsp;</label>
            <a href="http://payment.veritrans.co.jp/" target="_blank">
                <img src="/images/PayMent_CreditCard_AJP_Supportby.gif" alt="CreditCard Support by"
                    style="border: 0" /></a>
        </p>
    </div>
</div>
