<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartContactTemplate.ascx.cs" 
    Inherits="eStore.UI.Modules.V4.CartContactTemplate" %>
<td>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Name)%> : </span>
        <asp:Literal ID="lAttention" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Company_Name)%> : </span>
        <asp:Literal ID="ltCompany" runat="server"></asp:Literal></p>
    <asp:Panel ID="pLegalForm" runat="server" Visible="false">
    <p>
        <span>Legal Form : </span>
        <asp:Literal ID="ltLegalForm" runat="server"></asp:Literal></p>
    </asp:Panel>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address)%>: </span>
        <asp:Literal ID="lAddress1" runat="server"></asp:Literal>
        <asp:Literal ID="lAddress2" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_City)%> : </span>
        <asp:Literal ID="lCity" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_State)%> : </span>
        <asp:Literal ID="lState" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_ZIP)%> : </span>
        <asp:Literal ID="lZipCode" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country)%> : </span>
        <asp:Literal ID="lCountry" runat="server"></asp:Literal></p>
    <p>
        <span><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Phone)%> : </span>
        <asp:Literal ID="lTelNo" runat="server"></asp:Literal></p>
    <p id="pvat" runat="server" visible="false">
        <span>VAT Number : </span>
        <asp:Literal ID="lvat" runat="server"></asp:Literal></p>
</td>    
