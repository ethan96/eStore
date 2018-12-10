<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderMenu.ascx.cs"
    Inherits="eStore.UI.Modules.HeaderMenu" %>
<%@ Register Src="StoreSearch.ascx" TagName="StoreSearch" TagPrefix="eStore" %>
<ul id="nav" class="dropdown dropdown-linear dropdown-columnar">
    <asp:Literal ID="productCategoryMenu" runat="server" EnableViewState="false">
    </asp:Literal>
    <li class="end"></li>
    <li class="item">
    <div id="headersearch">
        <eStore:StoreSearch ID="StoreSearch1" runat="server" />
        <asp:Button ID="btnSearch" CssClass="storeBlueButton" runat="server"  EnableViewState="false" 
            Text="Search" ClientIDMode="Static" onclick="btnSearch_Click" /></div>
    </li>
</ul>
<script type="text/javascript" language="javascript">
    $("ul.dropdown .dir").hover(function () {
        var menu = $(this);
        clearTimeout(menu.data("timer"));
        menu.data("timer", setTimeout(function () {
            $(menu).addClass("hover");
            var child = $(menu).find("ul:first");
            if ($(menu).position().left + $(child).width() < 980) {
                $(child).css({ left: $(menu).position().left + "px" });
            }
            $(menu).find("ul:first").show(200);
        }, 100));
    }, function () {
        var menu = $(this);
        clearTimeout(menu.data("timer"));
          menu.data("timer", setTimeout(function () {
            $(menu).removeClass("hover");
            $(menu).find("ul:first").hide(100);
        }, 200));
    });

    $("ul.dropdown li").click(function () {

        var menu = $(this).closest(".dir");
        clearTimeout(menu.data("timer"));
        $(menu).removeClass("hover");
        $(menu).find("ul:first").hide(200);
    });
    $("#<%=btnSearch.ClientID %>").click(function () {
        if ($("#storekeyworddispay").validateTextBoxWithToolTip()) {
            window.location =   '<%=ResolveUrl( eStore.Presentation.eStoreContext.Current.SearchConfiguration.ResultPageUrl)%>' + "?skey=" + encodeURIComponent($("#storekeyworddispay").val());
        }
        return false;
    });
</script>
