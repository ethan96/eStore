<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeCurrency.ascx.cs"
    Inherits="eStore.UI.Modules.ChangeCurrency" %>
<div id="MutliCurrencies">
    <asp:RadioButtonList ID="rblCurrencies" runat="server" RepeatDirection="Horizontal">
    </asp:RadioButtonList>
   
    <span class="exchangedprice"> <asp:Literal ID="preExchange" runat="server"></asp:Literal></span>
    <asp:HiddenField ID="hlistprice" ClientIDMode="Static" runat="server" />
</div>
<script type="text/javascript" language="javascript">
    $(function () {
        var rblCurrencies = $("input[name='<%=rblCurrencies.UniqueID %>']");
        if (rblCurrencies.length > 1) {
            changeCurrency();
        }
    });
    $("#MutliCurrencies :radio").change(function () {
        changeCurrency();
    });
    function changeCurrency() {
        $(".exchangedprice").html($("<img>").attr("src", GetStoreLocation() + "images/priceprocessing.gif"));
        vlistprice = $("#hlistprice").val();
        $.getJSON(GetStoreLocation()+"proc/do.aspx",
        { func: "<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.CurrencyExchange %>"
        , listprice: vlistprice
        , currency: $("input[name='<%=rblCurrencies.UniqueID %>']:checked").val()//(this).val()
        , time: new Date().getTime()
        },
        function (data) {
            if (data.exchangedprice && data.exchangedprice != "0")
                $(".exchangedprice").html(data.exchangedprice);
            else
                $(".exchangedprice").html("");
        });
    }
</script>
