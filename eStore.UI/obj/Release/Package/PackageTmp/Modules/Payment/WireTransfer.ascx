<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WireTransfer.ascx.cs"
    Inherits="eStore.UI.Modules.Payment.WireTransfer" %>
<%@ Register src="PONo.ascx" tagname="PONo" tagprefix="uc1" %>

<div id="paymentMessage" class="hiddenitem">
    <asp:PlaceHolder ID="phPaymentMessage" runat="server"></asp:PlaceHolder>
</div>
<div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Bank_Information)%>
    </div>
    <div class="paymentbody editorpanel">
       <asp:Literal ID="lbankInfo" runat="server"></asp:Literal>
        <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CreditCard_Show_Federal"))
           {%>
        <p>
            <label>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Card_FederalID)%>
            </label>
            <eStore:TextBox id="txtFederalID" runat="server">
            </eStore:TextBox>
        </p>
        <%} %>
        <p>
            <uc1:PONo ID="PONo1" runat="server" />
            </p>
    </div>
</div>
