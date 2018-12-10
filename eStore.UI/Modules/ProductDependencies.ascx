<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductDependencies.ascx.cs" Inherits="eStore.UI.Modules.ProductDependencies" %>

<eStore:Repeater ID="rpDependencyProducts" runat="server" 
    onitemdatabound="rpDependencyProducts_ItemDataBound">
    <HeaderTemplate>
        <table class="estoretable fontbold" width="100%">
            <thead>
                <tr>
                    <th class="tablecolwidth145">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber)%>
                    </th>
                    <th>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                    </th>
                    <%if (ShowATP)
                      { %>
                    <th class="adminonly tablecolwidth90">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                    </th>
                    <th class="adminonly tablecolwidth45">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableQty)%>
                    </th>
                    <%} %>
                    <th class="tablecolwidth75">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                    </th>
                    <th class="tablecolwidth45">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="tablecolwidth145">
                <span id='<%# Eval("SProductID") %>' name='<%# Eval("SProductID") %>' class="jTipProductDetailWithoutImage">
                    <%# Eval("SProductID")%></span>
                <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("SProductID") %> ' />
            </td>
            <td class="left">
                <%# Eval("productDescX")%>
            </td>
            <%if (ShowATP)
              { %>
            <td class="adminonly tablecolwidth90">
                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atpX.availableDate"))%>
            </td>
            <td class="adminonly tablecolwidth45">
                <%#Eval("atpX.availableQty")%>
            </td>
            <%}%>
            <td class="tablecolwidth75 colorRed right">
                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.Part)Container.DataItem).getListingPrice().value)%>
            </td>
            <td class="tablecolwidth45 text-center">
                <eStore:TextBox ID="txtQty" runat="server" CssClass="qtytextbox maxWith50"></eStore:TextBox>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</eStore:Repeater>
<table width="100%">
    <tr>
        <td colspan="5" style="text-align:left">
            <asp:Label ID="lbDependencyMessage" runat="server" CssClass="lbDependencyMessage" ForeColor="Red"></asp:Label>
        </td>
        <td colspan="1" style="text-align:right">
            <eStore:Button ID="btnAddDependencyToCart" runat="server" Text="Add" CssClass="AddDpToCart" style="display:none" OnClientClick="return checkAllDependency();" OnClick="btnAddDependencyToCart_Click" />
            <eStore:Button ID="btnAddDependencyToQuote" runat="server" Text="Add" CssClass="AddDpToQuote" style="display:none" OnClientClick="return checkAllDependency();" OnClick="btnAddDependencyToQuote_Click" />
        </td>
    </tr>
</table>