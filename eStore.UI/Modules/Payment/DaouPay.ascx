<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DaouPay.ascx.cs" Inherits="eStore.UI.Modules.Payment.DaouPay" %>

 <div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Terms_and_Conditions)%>
    </div>
    <div class="paymentbody editorpanel">
       <asp:Literal ID="ltBankInfo" runat="server"></asp:Literal>
       <p align="right">
            <span style="color:Red">*</span>상기 조건에 동의하시면 확인 버튼을 누르고 결제를 진행하십시오.
        </p>
    </div>
</div>