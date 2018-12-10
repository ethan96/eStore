<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AEU_CompleteOffer.ascx.cs" Inherits="eStore.UI.Modules.Payment.files.AEU_CompleteOffer" %>
<%@ Register src="AEU_CompleteOfferWithoutAgreeBt.ascx" tagname="AEU_CompleteOfferWithoutAgreeBt" tagprefix="uc1" %>


<uc1:AEU_CompleteOfferWithoutAgreeBt ID="AEU_CompleteOfferWithoutAgreeBt1" runat="server" />
<div class="eStore_order_iaAgree">
    <asp:CheckBox ID="checkboxAgreeTerms" ClientIDMode="Static" runat="server" />
    <span class="eStore_redStar colorRed">*</span>I Agree
    <div class="clearfix"></div>
</div>

<script type="text/javascript">
    function checkPaymentInfo() {
        if (!$("#checkboxAgreeTerms").prop("checked")) {
            alert($.eStoreLocalizaion("You_have_to_accept_the_terms_and_conditions_before_continue"));
            return false;
        }
    }
</script>