<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SuggestingProducts.ascx.cs" Inherits="eStore.UI.Modules.SuggestingProducts" %>
<eStore:Repeater ID="rpSuggestingProducts" runat="server">
    <HeaderTemplate>
        <div class="DarkBlueHeader">
          Customers bought this product also bought
        </div>
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
                <span id='<%# Eval("VProductID") %>' name='<%# Eval("VProductID") %>' class="jTipProductDetailWithoutImage">
                    <%# Eval("SProductID")%></span>
            </td>
            <td class="left">
                <%# Eval("productDescX")%>
            </td>
            <%if (ShowATP)
              { %>
            <td class="adminonly tablecolwidth90">
                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
            </td>
            <td class="adminonly tablecolwidth45">
                <%#Eval("atp.availableQty")%>
            </td>
            <%}%>
            <td class="tablecolwidth75 colorRed right">
                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.Part)Container.DataItem).getListingPrice().value)%>
            </td>
            <td class="tablecolwidth45">
                <input type="text" name="suggestingproductqty" id="suggestingproduct_<%# Eval("SProductID")%>"
                    class="qtytextbox" />
                <input type="hidden" name="suggestingproductSProductID"  value="<%# Eval("SProductID")%>"
                    class="qtytextbox" />
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody> </table>
    </FooterTemplate>
</eStore:Repeater>