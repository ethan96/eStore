<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCompare.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.ProductCompare" %>
<asp:Literal ID="toppager" runat="server"></asp:Literal>
<table id="tblCompareProducts" runat="server" class="estoretable padding0 clear">
</table>
<div class="rightside">
    <eStore:CollectionPager ID="CollectionPager1" runat="server" PageSize="4" PagingMode="QueryString">
    </eStore:CollectionPager>
</div>