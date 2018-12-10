<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimulateFreightCartList.ascx.cs" Inherits="eStore.UI.Modules.V4.SimulateFreightCartList" %>
<asp:GridView ID="gvCartContent" runat="server" AutoGenerateColumns="false" 
    OnRowDataBound="gvCartContent_RowDataBound" CssClass="eStore_table_order">
    <Columns>
        <asp:TemplateField HeaderText="Line No" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="ltLineNo" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Part No" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="ltSProductID" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display part No" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="ltDisplayPartNo" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Product name" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="ltProductName" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Stock status" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Literal ID="ltStockStatus" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qty">
            <ItemTemplate>
                <asp:Literal ID="ltQty" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Unit Price">
            <ItemTemplate>
                <asp:Literal ID="ltUnitPrice" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:Literal ID="ltSubTotal" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:Literal ID="ltDeleteBtn" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>