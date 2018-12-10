<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountrySelector.ascx.cs"
    Inherits="eStore.UI.Modules.CountrySelector" %>
<script type="text/javascript">
    function changeCustomerCountry(country, sate, hfState) {
        if (country != "") {
            var selectBlow = "[<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Select_Below) %>]";
            $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation() %>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getStoreCountries %>", { id: country },
            function (data) {
                var ddlState = document.getElementById(sate);
                ddlState.options.length = 0;
                if (data == null || data == "") {
                    ddlState.options.add(new Option("None", "None"));
                    ddlState.disabled = "disabled";
                    return;
                }
                else if (data.length > 0) {
                    ddlState.options.add(new Option(selectBlow, ""));
                    ddlState.disabled = false;
                    for (var i = 0; i < data.length; i++) {
                        ddlState.options.add(new Option(data[i].state, data[i].short));
                    }
                    var selectedstate = $("#" + hfState).val();
                    if (selectedstate) {
                        for (i = 0; i < ddlState.options.length; i++) {
                            if (ddlState.options[i].value == selectedstate) {
                                ddlState.options[i].selected = true;
                            }
                        }
                    }

                }
                else {
                    ddlState.options.add(new Option("None", "None"));
                    ddlState.disabled = "disabled";
                }
            }
            );
        }
        else {
            var ddlState = document.getElementById(sate);
            ddlState.options.length = 0;
            ddlState.options.add(new Option("None", "None"));
            ddlState.disabled = "disabled";
        }
    }
    $(function () {
        changeCustomerCountry($("#<%=ddl_Country.ClientID %>").val(), '<%=ddl_State.ClientID %>', '<%=hf_State.ClientID %>');
        $("#<%=ddl_Country.ClientID %>").change(function () {
            changeCustomerCountry($("#<%=ddl_Country.ClientID %>").val(), '<%=ddl_State.ClientID %>', '<%=hf_State.ClientID %>');
        });
    });

</script>
<tr class="store_country">
    <td>
        <label class="<%=CountryTitleCss %>">
            <asp:Label ID="lbl_Country" runat="server">Country </asp:Label>
            <span class="eStore_redStar">*</span>:
        </label>
    </td>
    <td>
        <asp:DropDownList ID="ddl_Country" runat="server" CssClass="selectcss">
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="RequiredCountry" runat="server" ControlToValidate="ddl_Country"
            ErrorMessage="Your country is required." Display="None" SetFocusOnError="True"></asp:RequiredFieldValidator>
    </td>
</tr>
<tr>
    <td>
        <label class="<%=CountryTitleCss %>">
            <asp:Label ID="lbl_State" runat="server" Text="Label">State/Province </asp:Label>
            <span class="eStore_redStar">*</span>:
        </label>
    </td>
    <td>
        <asp:DropDownList ID="ddl_State" runat="server" CssClass="selectcss">
        </asp:DropDownList>
        <asp:HiddenField ID="hf_State" runat="server" />
    </td>
</tr>