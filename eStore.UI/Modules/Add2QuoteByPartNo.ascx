<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Add2QuoteByPartNo.ascx.cs" Inherits="eStore.UI.Modules.Add2QuoteByPartNo" %>
<div style="padding: 3px; background-color: #CCCCCC">
    <asp:Panel ID="pnAddQuotation" runat="server" DefaultButton="bt_ByPartNoAddtoQuote">
    <eStore:TextBox ID="tb_ByPartNoAddtoQuote" runat="server" Width="170px"></eStore:TextBox>
&nbsp;<asp:Button ID="bt_ByPartNoAddtoQuote" runat="server" Text="Add to Quote" CssClass="adminbutton eStore_btn borderOrange" 
    onclick="bt_ByPartNoAddtoQuote_Click" /></asp:Panel>
</div>

<script type="text/javascript" language="javascript">
    $(function () {
        var tbValue = $('#<%=tb_ByPartNoAddtoQuote.ClientID %>');
            tbValue.autocomplete({
            source: function (request, response) {
                $.getJSON("/proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getOderByPNSearchKey %>",
                 {
                     maxRows: 12,
                     keyword: request.term
                 }, response);
            },
            select: function (event, ui) {
                $('#JT').remove();
                tbValue.val(ui.item.value);
                return false;
            },
            focus: function (event, ui) {
                tbValue.val(ui.item.value);
                var url = "/proc/html.aspx?type=ProductDetailTip&ProductID=" + ui.item.value;
                $('#JT').remove();
                JT_show(url, $(this).attr("id"), ui.item.label);
            },
            close: function (event, ui) { $('#JT').remove(); }
        });
    });
</script>