<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartThankyou.ascx.cs"
    Inherits="eStore.UI.Modules.CartThankyou" %>
<div class="eStore_order_thankU">
    <div class="top">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Thank_you_for_your_order)%></div>
    <p>
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_This_order_information_has_been_emailed)%>
        <%=eStore.Presentation.eStoreContext.Current.User.actingUser.UserID%>
        ,<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_including_a_link_to_view_your_order)%></p>
    <ol>
        <li><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_No)%>:</b><asp:Literal
                ID="lOrderNo" runat="server"></asp:Literal></li>
        <li><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_date)%>:</b><asp:Literal
                ID="ltOrderDate" runat="server"></asp:Literal></li>
        <li><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Status)%>:</b><asp:Literal
                ID="lStatus" runat="server"></asp:Literal></li>
        <li><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_Amount)%>:</b><asp:Literal
                ID="lAmount" runat="server"></asp:Literal></li>
        <li><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type)%>:</b><asp:Literal
                ID="ltPayMentType" runat="server"></asp:Literal></li>
        <li runat="server" id="li_Reseller"><b>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Reseller_ID)%>:</b><asp:Literal
                ID="ltReseller" runat="server"></asp:Literal></li>
    </ol>
</div>
