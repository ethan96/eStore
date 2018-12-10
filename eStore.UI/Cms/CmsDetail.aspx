<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="CmsDetail.aspx.cs" Inherits="eStore.UI.CmsDetail" %>

<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <script type="text/javascript" src="../Scripts/pic.js"></script>
    <link href="../Styles/CMS.css" rel="stylesheet" type="text/css" />
    <div id="cmsMainDiv">
        <asp:Literal ID="ltAppCate" runat="server"  EnableViewState="false"></asp:Literal>
        <div id="cmsPic">
            <asp:Image ID="Image1" Visible="false" runat="server"  EnableViewState="false"/></div>
        <div class="clear">
        </div>
        <h1>
            <asp:Literal ID="ltTitle" runat="server"  EnableViewState="false"></asp:Literal>
        </h1>
        <p id="cmsDate">Date:
            <asp:Literal ID="ltDate" runat="server"  EnableViewState="false"></asp:Literal></p>
        <div class="clear">
        </div>
        <p class="comContent">
            <asp:Literal ID="ltContext" runat="server" EnableViewState="false"></asp:Literal></p>
        <div class="clear">
        </div>
        <div class="cmsView">
            <asp:HyperLink ID="hlDetails" runat="server" CssClass="eStore_btn" Text="View"  EnableViewState="false"/>
        </div>
        <div class="clear">
        </div>
    </div>
</asp:Content>
