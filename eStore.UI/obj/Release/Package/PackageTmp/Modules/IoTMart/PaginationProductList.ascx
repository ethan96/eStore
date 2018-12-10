<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaginationProductList.ascx.cs"
    Inherits="eStore.UI.Modules.IoTMart.PaginationProductList" %>
<div class="epaps-productRow2" id="<%=newid %>">
    <asp:Repeater runat="server" ID="rpProList">
        <HeaderTemplate>
            <table class="estoretable fontbold clear">
                <tr>
                    <th scope="col">
                    <%if (!eStore.Presentation.eStoreContext.Current.getBooleanSetting("isHideProListSelect"))
                    { %>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Select)%>
                    <%} %>  
                    </th>
                    <th scope="col">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_Number)%>
                    </th>
                    <th scope="col">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Description)%>
                    </th>
                    <th scope="col">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price)%>
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr category="Cate<%=newid %>">
                <td>
                    <input type="checkbox" id='chk_<%# Eval("SProductID") %>' name="cbproduct" <%# (Eval("MininumnOrderQty")==null)?"":string.Format("MOQ='{0}'",Eval("MininumnOrderQty")) %>
                        <%# (bool)Eval("DisableAddtoCart") ?"disabled=\"disabled\"":""%> value='<%# Eval("SProductID") %>' />
                </td>
                <td class="nowrap">
                    <%# binPhaseOutProduct(Eval("phasedOut"), Eval("ProductType"), Eval("link"), Eval("SProductID"), Eval("name"), Eval("status"))%>
                </td>
                <td>
                    <%#HttpUtility.HtmlDecode(Eval("desc").ToString())%>
                    <eStore:Repeater ID="rpBTOSConfigDetails" runat="server" DataSource='<%# Eval("ReplaceProducts") %>'
                        Visible='<%# (bool)Eval("phasedOut") %>'>
                        <HeaderTemplate>
                            <br />
                            <%= eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Advantech_recommends_the_following_replacement)%>:
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                                id='<%# Eval("SProductID") %>' name='<%# Eval("name")  %> '>
                                <%# Eval("name")%></a> </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul></FooterTemplate>
                    </eStore:Repeater>
                </td>
                <td class="right">
                    <%#Eval("price")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="clearfix">
    </div>
</div>
<div id="<%=newid %>_paging">
</div>
<%if(isShowPagination){ %>
<script type="text/javascript">
    $("#<%=newid %>_paging").bindPagination($("#<%=newid %> tr[category='Cate<%=newid %>']"),<%=count %>);
</script>
<%} %>
