<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartContentPreview.ascx.cs"
    Inherits="eStore.UI.Modules.CartContentPreview" %>
<eStore:Repeater ID="dlCartContent" runat="server" OnItemDataBound="dlCartContent_ItemDataBound">
    <HeaderTemplate>
        <table width="100%" border="0" cellpadding="0" cellspacing="0" class="eStore_table_order eStore_orderItem eStore_orderStep3">
            <tr class="<%if (showATP){%> normalTh<%}%>">
                <th>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_ItemIndex)%>
                </th>
                <th>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PartNumber)%>
                </th>
                <th class="tbldesc">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Description)%>
                </th>
                <%if (showATP)
                  { %>
                <th class="adminonly forAuthor">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableABC)%>
                </th>
                <th class="adminonly forAuthor">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_AvailableDate)%>
                </th>
                <th class="adminonly forAuthor">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Available_Qty)%>
                </th>
                <%} %>
                <th>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty)%>
                </th>
                <th>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_UnitPrice)%>
                </th>
                <th>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Sub_Total)%>
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr <%# Container.ItemIndex % 2 == 0 ? "" : "class='odd'" %>>
            <td>
                <span class="fontBold"><%# Container.ItemIndex +1 %></span>
            </td>
            <td class="left">
                <%# Eval("partX.name")%>
            </td>
            <td class="left">
                <%# Eval("Description") %><br />
                <span class="colorRed">
                    <%# Eval("CustomerMessage")%>
                    <%#Eval("PromotionMessage")==null|| string.IsNullOrEmpty(Eval("PromotionMessage").ToString()) 
                         ? string.Empty
                         : (Eval("CustomerMessage") == null || string.IsNullOrEmpty(Eval("CustomerMessage").ToString())? string.Empty:"<br />")
                         + Eval("PromotionMessage").ToString()%>
                </span>
                <%#Eval("PackageTrackingNo") == null || string.IsNullOrEmpty(Eval("PackageTrackingNo").ToString()) ? string.Empty : string.Format("<p class=\"fontBlue\">Tracking No: {0} {1}</p>", Eval("PackageTrackingNo"),
                                                                                                                       Eval("PackageTrackingStatus") == null|| string.IsNullOrEmpty(Eval("PackageTrackingStatus").ToString()) ? string.Empty : string.Format("Status: {0}", Eval("PackageTrackingStatus"))
                        )%>
            </td>
            <%if (showATP)
              { %>
            <td class="adminonly forAuthor">
                <%#Eval("ABCInd")%>
            </td>
            <td class="adminonly forAuthor">
                <%#eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
            </td>
            <td class="adminonly forAuthor">
                <%#Eval("atp.availableQty")%>
            </td>
            <%} %>
            <td>
                <%# Eval("Qty") %>
            </td>
            <td class="right">
                <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("UnitPrice"), Eval("currencySign").ToString())%>
            </td>
            <td class="right">
                <%#eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("AdjustedPrice"), Eval("currencySign").ToString())%>
                <%#Eval("DiscountAmount") == null ? string.Empty : string.Format("<br/><span class=\"colorRed\">-{0}</span>", eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("DiscountAmount"), Eval("currencySign").ToString()))%>
            </td>
        </tr>
        <tr id="warrrantyItem" runat="server">
            <td>
                <span class="paddingleft15">1</span>
            </td>
            <td class="left">
                <asp:Literal ID="lproductNameX" runat="server"></asp:Literal>
            </td>
            <td class="left">
                <asp:Literal ID="lDescription" runat="server"></asp:Literal>
            </td>
            <td class="adminonly forAuthor">
                &nbsp;
            </td>
            <td class="adminonly forAuthor">
                <asp:Literal ID="lavailableDate" runat="server"></asp:Literal>
            </td>
            <td class="adminonly forAuthor">
                <asp:Literal ID="lavailableQty" runat="server"></asp:Literal>
            </td>
            <td>
                <asp:Literal ID="lQty" runat="server"></asp:Literal>
            </td>
            <td class="right">
                <asp:Literal ID="lUnitPrice" runat="server"></asp:Literal>
            </td>
            <td class="right">
                <asp:Literal ID="lAdjustedPrice" runat="server"></asp:Literal>
            </td>
        </tr>
        <eStore:Repeater ID="rpBTOSConfig" runat="server">
            <ItemTemplate>
                <tr class="eStore_orderSystemList">
                    <%if (showATP)
                      { %>
                    <td>
                        &nbsp;
                    </td>
                    <td class="left">
                        <span class="fontBold"><%# Eval("CategoryComponentDesc")%></span>
                        <%#((bool)Eval("isBuildin"))? "built-in":string.Empty %>
                        <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("BTOSConfigDetails") %>'>
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="paddingleft0">
                                    <%# Eval("SProductID")%></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </eStore:Repeater>
                    </td>
                    <td class="left">
                        <%# Eval("OptionComponentDesc")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : Eval("ABCInd")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#(bool)Eval("isBuildin") ? string.Empty : Eval("atp.availableQty")%>
                    </td>
                    <td>
                        <%# Eval("Qty")%>
                    </td>
                    <td class="right">
                        <%# Eval("AdjustedPrice") %>
                    </td>
                    <td class="right">
                        <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("subtotal"),Eval("currencySign").ToString())%>
                    </td>
                    <%}
                      else
                      { %>
                    <td>
                        &nbsp;
                    </td>
                    <td class="left">
                        <%# Eval("CategoryComponentDesc")%>
                        <%#((bool)Eval("isBuildin"))? "built-in":string.Empty %>
                    </td>
                    <td colspan="5" class="left">
                        <%# Eval("OptionComponentDesc")%>
                    </td>
                    <%}%>
                </tr>
            </ItemTemplate>
        </eStore:Repeater>
        <eStore:Repeater ID="rpBundleItem" runat="server" OnItemDataBound="rpBundleItem_ItemDataBound">
            <ItemTemplate>
                <tr class="eStore_orderSystemList">
                    <td>
                        <span class="paddingleft15"><%# Container.ItemIndex+1 %></span>
                    </td>
                    <td class="left">
                        <%# Eval("part.name")%>
                    </td>
                    <%if (showATP)
                      { %>
                    <td class="left">
                        <%# Eval("part.productDescX")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#Eval("part.ABCInd")%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#eStore.Presentation.eStoreLocalization.Date(Eval("part.atp.availableDate"))%>
                    </td>
                    <td class="adminonly forAuthor">
                        <%#Eval("part.atp.availableQty")%>
                    </td>
                    <td>
                        <%#((eStore.POCOS.CartItem)((System.Web.UI.WebControls.RepeaterItem)Container.Parent.Parent).DataItem).Qty * (int)Eval("Qty")%>
                    </td>
                    <td class="right">
                        <%#Eval("AdjustedPrice", "{0:n2}")%>
                    </td>
                    <td class="right">
                        <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("AdjustedPrice") * ((eStore.POCOS.CartItem)((System.Web.UI.WebControls.RepeaterItem)Container.Parent.Parent).DataItem).Qty * (int)Eval("Qty")
                                                                , ((eStore.POCOS.CartItem)((System.Web.UI.WebControls.RepeaterItem)Container.Parent.Parent).DataItem).currencySign)%>
                    </td>
                    <%}
                      else
                      {%>
                    <td colspan="4" class="left">
                        <%# Eval("part.productDescX")%>
                    </td>
                    <%}%>
                </tr>
                <eStore:Repeater ID="rpBTOSConfig" runat="server">
                    <ItemTemplate>
                        <tr>
                            <%if (showATP)
                              { %>
                            <td>
                                &nbsp;
                            </td>
                            <td class="left">
                                <%# Eval("CategoryComponentDesc")%>
                                <%#((bool)Eval("isBuildin"))? "built-in":string.Empty %>
                                <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("BTOSConfigDetails") %>'>
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <%# Eval("SProductID")%></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul></FooterTemplate>
                                </eStore:Repeater>
                            </td>
                            <td class="left">
                                <%# Eval("OptionComponentDesc")%>
                            </td>
                            <td class="adminonly forAuthor">
                                <%#(bool)Eval("isBuildin") ? string.Empty : Eval("ABCInd")%>
                            </td>
                            <td class="adminonly forAuthor">
                                <%#(bool)Eval("isBuildin") ? string.Empty : eStore.Presentation.eStoreLocalization.Date(Eval("atp.availableDate"))%>
                            </td>
                            <td class="adminonly forAuthor">
                                <%#(bool)Eval("isBuildin") ? string.Empty : Eval("atp.availableQty")%>
                            </td>
                            <td>
                                <%# Eval("Qty")%>
                            </td>
                            <td class="right">
                                <%# Eval("AdjustedPrice") %>
                            </td>
                            <td class="right">
                                <%# eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)Eval("subtotal"),Eval("currencySign").ToString())%>
                            </td>
                            <%}
                              else
                              { %>
                            <td>
                                &nbsp;
                            </td>
                            <td  class="left">
                                <%# Eval("CategoryComponentDesc")%>
                                <%#((bool)Eval("isBuildin"))? "built-in":string.Empty %>
                            </td>
                            <td colspan="5"  class="left">
                                <%# Eval("OptionComponentDesc")%>
                            </td>
                            <%}%>
                        </tr>
                    </ItemTemplate>
                </eStore:Repeater>
            </ItemTemplate>
        </eStore:Repeater>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</eStore:Repeater>
