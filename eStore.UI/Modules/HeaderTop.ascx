<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderTop.ascx.cs" Inherits="eStore.UI.Modules.HeaderTop" %>
<%@ Register Src="~/Modules/ChangeRegion.ascx" TagName="ChangeRegion" TagPrefix="uc1" %>
<div class="header" runat="server" id="headerdivision">
    <div>
        <a class="eStoreLogo" href="<%= ResolveUrl("~/") %>"></a>
    </div>
    <ul class="HeaderList">
        <li><a href="<%= ResolveUrl("~/Compare.aspx") %>">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Compare_List)%>
        </a></li>
        <li><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Cart)%></a><span>|</span></li>
        <li><a href="<%= ResolveUrl("~/Quotation/myquotation.aspx") %>" class="needlogin">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a><span>|</span></li>
        <li><a href="<%= ResolveUrl("~/Cart/myorders.aspx") %>" class="needlogin">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a><span>|</span></li>
        <li>
            <uc1:ChangeRegion ID="ChangeRegion1" runat="server" />
        </li>
    </ul>
</div>
