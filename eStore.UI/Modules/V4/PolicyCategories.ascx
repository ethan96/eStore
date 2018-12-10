<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PolicyCategories.ascx.cs"
    Inherits="eStore.UI.Modules.V4.PolicyCategories" %>

<div class="eStore_leftBlock eStore_leftBlock_fix float-left">
    <h1>
        <asp:Literal ID="lGroupName" runat="server"></asp:Literal></h1>
    <div class="eStore_leftBlock_link eStore_policy_tabList">
        <span></span>
        <asp:Repeater ID="rpPolicyCagtegories" runat="server" OnItemDataBound="rpPolicyCagtegories_ItemDataBound">
            <ItemTemplate>
                <asp:HyperLink ID="hlPurl" runat="server" Style="cursor: pointer"></asp:HyperLink>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

