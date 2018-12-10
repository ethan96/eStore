<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResaleSetting.ascx.cs"
    Inherits="eStore.UI.Modules.ResaleSetting" %>
<div class="eStore_order_radioList">
    <asp:CheckBox ID="chkResale" ClientIDMode="Static" runat="server" Text="Resale?" />
    <div class="eStore_order_radioList_content">
    <div class="hiddenitem" id="resellerCert" runat="server"
        clientidmode="Static">
        <p>
            <%=eStore.Presentation.eStoreLocalization.Tanslation("eStore_Resale_Message")%></p>
        <p>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Reseller_ID)%>
            :
            <eStore:TextBox ID="resellerid" runat="server" ToolTip="Reseller ID" MaxLength="24"
                Width="90px"></eStore:TextBox><asp:Button ID="btnRecalculateTax" runat="server" Text="Recalculate Tax" CssClass="eStore_btn borderBlue" /></p>
        <p>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Certificate)%>
            :
            <asp:FileUpload ID="filecertificate" runat="server" /></p>
        <p id="pdownloadreseller" runat="server" visible="false">
            click
            <asp:HyperLink ID="ldownload" runat="server" CssClass="colorBlue">here</asp:HyperLink>
            &nbsp;to review your reseller certificate application form.
        </p>
    </div></div>
</div>
<script type="text/javascript" language="javascript">
    $("#chkResale").click(function () {
        if (this.checked) {
            $("#resellerCert :text").PreFillToolTips();
            $("#resellerCert").show();

            $("#btnNext").bind("click", resalevalidation);
        }
        else {
            $("#<%=btnRecalculateTax.ClientID%>").click();
        }

    });

    if ($("#chkResale").prop("checked")) {
        $("#resellerCert").removeClass("hiddenitem");
    }

    function resalevalidation() {
        if (!$("#resellerCert :text").validateTextBoxWithToolTip())
            return false;
    }
 
</script>
