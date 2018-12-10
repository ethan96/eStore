<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="eStore.UI.ErrorPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="graybordercontent editorpanel">
        <h1>
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Information)%>
        </h1>
        <div class="grayborderbody">
            <h2>
                <asp:Literal ID="lmessage" Text="Please be patient - This Page is Under Construction!"
                    runat="server"></asp:Literal></h2>
            <p>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_click_here)%></p>
        </div>
        <div class="grayborderbottom">
            <div class="graybordercorner">
            </div>
        </div>
    </div>
</asp:Content>
