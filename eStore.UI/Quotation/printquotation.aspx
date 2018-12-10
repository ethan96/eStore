<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Print.Master" AutoEventWireup="true"
    CodeBehind="printquotation.aspx.cs" Inherits="eStore.UI.Quotation.printquotation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <h1>
        <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Quotation_Number)%>
        :</label>
        <asp:Literal ID="lQuotationNumber" runat="server"></asp:Literal>
    </h1>
    <p>
        <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Date)%>
            :
        </label>
        <asp:Literal ID="lQuoteDate" runat="server"></asp:Literal>
        <label>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Expired_Date)%>
            :</label><asp:Literal ID="lQuoteExpiredDate" runat="server"></asp:Literal></p>
    <eStore:QuotationContentPreview ID="QuotationContentPreview1" runat="server" />
    <div class="clear"></div>
</asp:Content>
