<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Compare.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.Compare" %>

<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="epaps-row980">
        <div class="epaps-title-bgGray">
            <h1>
                Product Comparison</h1>
            <%--<a href="#" class="epaps-emailIcon">Email</a>--%><a href="#" onclick="window.print();return false"
                class="epaps-printIcon">Print</a></div>
        <table id="tblCompareProducts" runat="server" width="100%" border="0" cellspacing="0"
            cellpadding="0" class="epaps-table epaps-ProductComparison">
        </table>
    </div>
</asp:Content>
