<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YouAreHereMutli.ascx.cs"  EnableViewState="false"
    Inherits="eStore.UI.Modules.YouAreHereMutli" %>
<%@ Register Src="YouAreHere.ascx" TagName="YouAreHere" TagPrefix="eStore" %>
<eStore:Repeater ID="rpMutliYouarehere" runat="server" OnItemDataBound="rpMutliYouarehere_ItemDataBound">
    <ItemTemplate>
        <eStore:YouAreHere ID="YouAreHere1" ShowSelfLink="true" runat="server" />
    </ItemTemplate>
</eStore:Repeater>
<asp:Panel ID="pnoneLink" runat="server" Visible="false">
    <div class="eStore_breadcrumb eStore_block980">        </div>
</asp:Panel>
