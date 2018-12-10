<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartContactTemplate.ascx.cs"
    Inherits="eStore.UI.Modules.CartContactTemplate" %>
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <th>
            <asp:Literal ID="ltHeaderText" runat="server"></asp:Literal>
        </th>
    </tr>
    <tr>
        <td>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Name)%>
                    : </span>
                <asp:Literal ID="lAttention" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Company_Name)%>
                    : </span>
                <asp:Literal ID="ltCompany" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address)%>:
                </span>
                <asp:Literal ID="lAddress1" runat="server"></asp:Literal></p>
            <asp:Panel ID="pLegalForm" runat="server" Visible="false">
                <p>
                    <span class="title">Legal Form : </span>
                    <asp:Literal ID="ltLegalForm" runat="server"></asp:Literal></p>
            </asp:Panel>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_City)%>
                    : </span>
                <asp:Literal ID="lCity" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_State)%>
                    : </span>
                <asp:Literal ID="lState" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_ZIP)%>
                    : </span>
                <asp:Literal ID="lZipCode" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country)%>
                    : </span>
                <asp:Literal ID="lCountry" runat="server"></asp:Literal></p>
            <p>
                <span class="title">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Phone)%>
                    : </span>
                <asp:Literal ID="lTelNo" runat="server"></asp:Literal></p>
            <p id="pvat" runat="server" visible="false">
                <span class="title">VAT Number : </span>
                <asp:Literal ID="lvat" runat="server"></asp:Literal></p>
            <p class="float-right">
            <span class="title"></span>
                <asp:Button ID="btEditContact" runat="server" Text="Edit" CssClass="eStore_btn borderBlue" /></p>
        </td>
    </tr>
</table>

