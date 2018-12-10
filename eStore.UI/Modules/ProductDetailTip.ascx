<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDetailTip.ascx.cs"
    Inherits="eStore.UI.Modules.ProductDetailTip" %>
<h4>
    <asp:Literal ID="lproductnmae" runat="server"></asp:Literal>
        <asp:Image CssClass="imgNoneCSS" ID="productstatus" runat="server" />
    </h4>
<p>
    <asp:Image ID="imgProductimg" runat="server" />
    <asp:Literal ID="lproductdesc" runat="server"></asp:Literal>
</p>
<ul>
    <asp:Literal ID="lproductfeature" runat="server"></asp:Literal>
</ul>
