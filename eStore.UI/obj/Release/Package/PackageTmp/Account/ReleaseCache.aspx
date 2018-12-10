<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ReleaseCache.aspx.cs" Inherits="eStore.UI.Account.ReleaseCache" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <asp:Button ID="btnReleaseCache" runat="server" Text="Release Cache" OnClick="btnReleaseCache_Click" />
    <asp:Button ID="btnRemoveSelectedCache" runat="server" OnClick="btnRemoveSelectedCache_Click"
        ClientIDMode="Static" Text="Remove Selected Items" />
    <br />
    <asp:GridView ID="gvReleaseCache" runat="server" AutoGenerateColumns="false" CssClass="estoretable">
        <Columns>
            <asp:TemplateField HeaderText="Select">
                <ItemTemplate>
                    <input type="checkbox" name="selectedCacheItem" value=" <%# Container.DataItem %>" />
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select)%>
                </HeaderTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Items" ItemStyle-CssClass="left">
                <ItemTemplate>
                    <%# Container.DataItem %>
                </ItemTemplate>
                <HeaderTemplate>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Items)%>
                </HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <script type="text/javascript" language="javascript">
        $("#btnRemoveSelectedCache").click(function () {
            if ($("input[name='selectedCacheItem']:checked").length == 0)
            { alert("Please select items!"); return false; }
        });
    </script>
</asp:Content>
