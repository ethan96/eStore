<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCategory.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.ProductCategory" %>
<asp:TreeView ID="tvProductCategories" runat="server" NodeWrap="True" ImageSet="Arrows"
    CssClass="eStoreTreeview">
    <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
    <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
        VerticalPadding="0px" />
    <LevelStyles>
        <asp:TreeNodeStyle CssClass="categotitle" VerticalPadding="3px" />
        <asp:TreeNodeStyle CssClass="categotitle01" VerticalPadding="3px" />
        <asp:TreeNodeStyle CssClass="categotitle02" VerticalPadding="3px" />
    </LevelStyles>
    <ParentNodeStyle CssClass="categotitle02" VerticalPadding="3px" />
    <LeafNodeStyle CssClass="categotitle02" VerticalPadding="3px" />
</asp:TreeView>
<script language="javascript" type="text/javascript">
    $(".eStoreTreeview>table").addClass("RootNodeStyle");
    $(".eStoreTreeview>div").addClass("LeafNodeStyle");
</script>
