<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResaleSetting_CNPJ.ascx.cs" Inherits="eStore.UI.Modules.ResaleSetting_CNPJ" %>
<div class="modules">
    CNPJ/CPF: <span class="colorRed">&nbsp;&nbsp;*&nbsp;&nbsp;&nbsp;</span>
    <asp:TextBox ID="txtResaleSettingCNPJ" runat="server" CssClass="resaleSettingCNPJ"></asp:TextBox>
</div>
    <script type="text/javascript" language="javascript">
        function validateCNPJ() {
            var CNPJValue = $(".resaleSettingCNPJ").val();
            if (CNPJValue == undefined || CNPJValue == null) {
                return true;
            }
            if (CNPJValue.length > 0) {
                var regOne = /^\d{3}\.\d{3}\.\d{3}\-\d{2}$/;
                var regTwo = /^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$/;
                if (!(regOne.test(CNPJValue))) {
                    if (!(regTwo.test(CNPJValue))) {
                        alert("Incorrect CNPJ / CPF format.");
                        return false;
                    }
                }
            }
            else {
                alert("Please Enter CNPJ/CPF!");
                return false;
            }
            return true;
        }
    </script>