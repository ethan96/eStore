<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCategoryAllList.ascx.cs" EnableViewState="false"
    Inherits="eStore.UI.Modules.ProductCategoryAllList" %>

<asp:Repeater ID="rp_AllCategoryList" runat="server" ClientIDMode="Static" onitemdatabound="rp_AllCategoryList_ItemDataBound">
    <ItemTemplate>
        <ol>
            <asp:Literal ID="lt_title" runat="server"></asp:Literal>
            <asp:Repeater ID="repert_subCategory" runat="server">
                <ItemTemplate>
                    <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("localCategoryNameX")%></a></li>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Literal ID="lt_more" runat="server"></asp:Literal>
        </ol>
    </ItemTemplate>
</asp:Repeater>