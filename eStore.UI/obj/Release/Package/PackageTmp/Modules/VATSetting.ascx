<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VATSetting.ascx.cs"
    Inherits="eStore.UI.Modules.VATSetting" %>
<div id="VATSetting" class="eStore_order_VATNumber row20">
    <h4><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_VAT_Number)%></h4>
    <div class="eStore_order_radioList">
        <div class="VATNumber_content">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_National_Prefix)%>
                        &nbsp;<eStore:TextBox ID="txtCode" runat="server" Width="30" MaxLength="2" ToolTip="National Prefix"
                            ClientIDMode="Static"></eStore:TextBox>&#12288;&#12288;VAT&nbsp;
                <eStore:TextBox ID="txtVATNumber" runat="server" ToolTip="VAT Number" CssClass="VATNumber"
                    Columns="15" ClientIDMode="Static">
                </eStore:TextBox>
        </div>
        <div class="Registration_number">Company Registration Number &nbsp;
            <eStore:TextBox ID="tbRegistNumber" runat="server" Length="9" ToolTip="Company Registration Number" ClientIDMode="Static"></eStore:TextBox>
        </div>
        <div class="VATNumber_att">
            <span></span><b><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Valid_VAT_number_is_required)%></b><br />
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_You_will_have_the_option_to_pay_either_by_credit_card)%>
        </div>
    </div>

</div>

<input type="hidden" value="" />
<asp:HiddenField ID="hVATValidStatus" runat="server" ClientIDMode="Static" Value="0" />
<script type="text/javascript" language="javascript">
    var cnt = 0;
    $(function () {
        $("#VATSetting :text").PreFillToolTips();
    });
    $("#txtCode,#txtVATNumber").keyup(function () {
        $("#hVATValidStatus").val("0");
    });
    function vatvalidation() {
        if (!$("#VATSetting :text").validateTextBoxWithToolTip())
            return false;
        if ($("#hVATValidStatus").val() == 0) {
            var myVATNumber = $("#txtCode").val() + $("#txtVATNumber").val();
            if (!checkVATNumber(myVATNumber)) {
                if (cnt == 0) {
                    cnt++;
                    alert("The VAT number you entered is not recognized.");
                    $("#txtVATNumber").focus();
                    return false;
                }
                else {
                    if (confirm("Our system does not recognize the VAT number. Please continue the checkout process, our customer service will contact you shortly to complete the order.")) {
                        $("#hVATValidStatus").val("-1");
                        return true;
                    }
                    else {
                        $("#txtVATNumber").focus();
                        return false;
                    }
                }
            }
            else {
                $("#hVATValidStatus").val("1");
            }
        }
    }
</script>
