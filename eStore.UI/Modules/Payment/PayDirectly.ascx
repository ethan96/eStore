<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PayDirectly.ascx.cs" Inherits="eStore.UI.Modules.Payment.PayDirectly" %>
<%@ Register src="PONo.ascx" tagname="PONo" tagprefix="uc1" %>
<div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Pay_Directly)%>
    </div>
    <div class="paymentbody editorpanel">
       <asp:Literal ID="lbankInfo" runat="server"></asp:Literal>
    </div>
</div>