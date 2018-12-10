<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KuaiQian.ascx.cs" Inherits="eStore.UI.Modules.Payment.KuaiQian" %>
<%@ Register Src="../OrderContentPreview.ascx" TagName="OrderContentPreview" TagPrefix="eStore" %>
<div class="paymentcontent">
    <div class="paymentheader">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_KuaiQian_Title)%>
    </div>
    <div class="paymentbody editorpanel">
        <div id="bankList">
            <ul>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_abc" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_icbc" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_ccb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_post" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_boc" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_cmb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_bcom" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_spdb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_ceb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_citic" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_pab" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_cmbc" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_sdb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_hxb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_gzcb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_cib" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_shb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_nbcb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_hzb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_bob" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_bea" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_bjrcb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_cbhb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_czb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_gdb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_hsb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_njcb" runat="server" /></li>
                <li>
                    <div class="bankRBlis"><input name="rbKuaiQianButton" type="radio" /></div><asp:Image ID="bank_shrcc" runat="server" /></li>
            </ul>
        </div>
        <div class="clear">
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        getBankInfor($(".bankselectImg"));
        $("#bankList li img").click(function () {
            getBankInfor($(this));
        });
        $("#bankList li input[name='rbKuaiQianButton']").click(function () {
            $(this).parent().next("img").click();
        });
    });
    function getBankInfor(selectobj) {
        $("input:radio[name='rbKuaiQianButton']").prop("checked", false);
        selectobj.prev().children("input[name='rbKuaiQianButton']").prop("checked", 'checked');
        $("#bankList li img").removeClass("bankselectImg");
        selectobj.addClass("bankselectImg");
        $("#eStoreMainContent_btnCheckout").removeClass("storeBlueButton").addClass("storeGrayButton").prop("disabled", "disabled");
        var bankid = selectobj.attr("bankid");
        var simulation = $("#cbsimulation").prop("checked");
        simulation = simulation == null ? false : simulation;
        eStore.UI.eStoreScripts.getsafeBankStr(bankid, simulation, function (res) {
            $("#bankId").val(bankid);
            $("#signMsg").val(res.signMsg);
            $("#orderTime").val(res.orderTime);
            $("#eStoreMainContent_btnCheckout").removeClass("storeGrayButton").addClass("storeBlueButton").removeAttr("disabled");
        });
    }
</script>
