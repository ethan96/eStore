<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SAPContatSelector.ascx.cs"
    Inherits="eStore.UI.Modules.SAPContatSelector" %>
<script type="text/javascript" language="javascript">
    $(function () {
        $("#storeSAPCompaniesDisplay").autocomplete({
            source: function (request, response) {
                $.getJSON("/proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getSAPCompany %>",
                 {
                     maxRows: 12,
                     keyword: request.term
                 }, response);
            },
             select: function (event, ui) {
                 $("#storeSAPCompaniesDisplay").val(ui.item.label);
                 $("#storeSAPCompanies").val(ui.item.value);
                return false;
            },
            focus: function (event, ui) {
                $("#storeSAPCompaniesDisplay").val(ui.item.label);
                $("#storeSAPCompanies").val(ui.item.value);
                var url = "/proc/html.aspx?type=SAPContatDetailTip&companyid=" + ui.item.value;
                $('#JT').remove();
                JT_show(url, $(this).attr("id"), ui.item.label);
            },
            close: function (event, ui) { $('#JT').remove(); }
        });

    });
</script>
<input id="storeSAPCompaniesDisplay" name="storeSAPCompaniesDisplay" class="textEntry InputValidation" />
<input id="storeSAPCompanies" name="storeSAPCompanies"  type="hidden"/>
