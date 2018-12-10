<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductPrint.ascx.cs"
    Inherits="eStore.UI.Modules.ProductPrint" %>
<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2">
            <h3 class="productname">
                <asp:literal id="lProductName" runat="server" />
            </h3>
            <h4>
                <asp:literal id="lShortDescription" runat="server" />
            </h4>
        </td>
    </tr>
    <tr>
        <td width="280px">
            <div class="ProductPicture">
                <%--<eStore:ProductLiterature ID="ProductLiterature1" runat="server" />--%>
                <asp:image id="imgProduct" runat="server" style="max-width:275px;"/>
            </div>
            <div class="clear">
            </div>
        </td>
        <td valign="top">
            <ul class="ProductFeature">
                <asp:literal id="lProductFeature" runat="server"></asp:literal></ul>
            <div class="clear">
            </div>
            <asp:literal id="lProductprice" runat="server"></asp:literal>
        </td>
    </tr>
</table>
<br />
<div class="clear">
</div>
<eStore:Repeater ID="rpBundletems" runat="server">
    <HeaderTemplate>
        <div class="DarkBlueHeader">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Bundle_items_detail)%>
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
                    
                    <th class="tablecolwidth75">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_UnitPrice)%>
                    </th>
                    <%} %>
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
                <span id='<%# Eval("ItemSProductID") %>' name='<%# Eval("ItemSProductID") %>' class="jTipProductDetailWithoutImage">
                    <%# Eval("ItemSProductID")%></span>
                <asp:HiddenField ID="hSProductID" runat="server" Value='<%# Eval("ItemSProductID") %> ' />
            </td>
            <td class="left">
                <%# Eval("part.productDescX")%>
            </td>
            <%if (ShowATP)
              { %>
            <td class="adminonly tablecolwidth90">
                <%#eStore.Presentation.eStoreLocalization.Date(Eval("part.atpX.availableDate"))%>
            </td>
            <td class="adminonly tablecolwidth45">
                <%#Eval("part.atpX.availableQty")%>
            </td>
           
            <td class="tablecolwidth75 colorRed right">
                <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal) Eval("adjustedPrice"))%>
            </td>
             <%}%>
            <td class="tablecolwidth45 center">
                <span><%# Eval("Qty")%></span>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody> </table>
    </FooterTemplate>
</eStore:Repeater>
<div class="clear">
</div>
<eStore:Repeater ID="rpPeripheralCompatibles" runat="server" OnItemDataBound="rpPeripheralCompatibles_ItemDataBound">
    <HeaderTemplate>
        <div class="DarkBlueHeader">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Recommended_Industrial_Grade_Peripherals)%>
            (<span class="colorRed"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Optional_Not_Required)%></span>)
        </div>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="LightBlueHeader" id="Peripheral-<%# Eval("ID")%>">
            <%# Eval("CategoryName")%>
        </div>
        <eStore:Repeater ID="rpPeripheralProducts" runat="server">
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
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="tablecolwidth145">
                        <span id='<%# Eval("SProductID") %>' name='<%# Eval("SProductID") %>' class="jTipProductDetail">
                            <%# Eval("SProductID")%></span>
                        <asp:hiddenfield id="hSProductID" runat="server" value='<%# Eval("SProductID") %> ' />
                    </td>
                    <td>
                        <%# Eval("Description")%>
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
                        <%# eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)((eStore.POCOS.PeripheralProduct)Container.DataItem).getPeripheralProductPrice())%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody> </table>
            </FooterTemplate>
        </eStore:Repeater>
    </ItemTemplate>
</eStore:Repeater>
<eStore:Repeater ID="rpRelatedProducts" runat="server">
    <HeaderTemplate>
        <div class="DarkBlueHeader">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Accessory_Products)%>
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
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="tablecolwidth145">
                <span id='<%# Eval("VProductID") %>' name='<%# Eval("VProductID") %>' class="jTipProductDetailWithoutImage">
                    <%# Eval("SProductID")%></span>
                <asp:hiddenfield id="hSProductID" runat="server" value='<%# Eval("SProductID") %> ' />
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
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody> </table>
    </FooterTemplate>
</eStore:Repeater>
<br />
<h3 id="h3CatAttribute" class="h3Title" runat="server">
    Product Spec</h3>
<asp:gridview id="gvSpec" width="100%" runat="server" autogeneratecolumns="False"
    cssclass="estoretable">
    <columns>
        <asp:TemplateField HeaderText="CatName">
            <ItemTemplate>
                <%#Server.HtmlDecode(Eval("LocalCatName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="AttributeName">
            <ItemTemplate>
                <%# Server.HtmlDecode(Eval("LocalAttributeName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="ValueName">
            <ItemTemplate>
                <%#Eval("LocalValueName")%>
            </ItemTemplate>
        </asp:TemplateField>
    </columns>
</asp:gridview>