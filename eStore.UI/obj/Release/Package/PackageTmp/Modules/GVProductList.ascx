<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GVProductList.ascx.cs"
    Inherits="eStore.UI.Modules.GVProductList" %>
<div class="ProductListTopPager">
    <eStore:CollectionPager ID="CollectionPager1" runat="server" PageSize="20" PagingMode="PostBack"
        EnableViewState="false">
    </eStore:CollectionPager>
</div>
<div class="clear">
</div>
<asp:GridView ID="gvProduct" runat="server" AutoGenerateColumns="False" CssClass="estoretable fontbold clear" Width="100%"
    BorderWidth="0px" GridLines="None" EnableViewState="false" CellPadding="0" CellSpacing="1"
    DataKeyNames="SProductID" AllowSorting="true" OnSorting="gvProduct_Sorting">
    <Columns>
        <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <input type="checkbox" id='chk_<%# Eval("SProductID") %>' name="cbproduct" <%# (Eval("MininumnOrderQty")==null)?"":string.Format("MOQ='{0}'",Eval("MininumnOrderQty")) %>
                    <%# (bool)Eval("DisableAddtoCart") ?"disabled=\"disabled\"":""%> value='<%# Eval("SProductID") %>' />
            </ItemTemplate>
            <HeaderTemplate>
            <%if (!eStore.Presentation.eStoreContext.Current.getBooleanSetting("isHideProListSelect"))
              { %>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Select)%>
            <%} %>    
            </HeaderTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Product Number" SortExpression="name" ItemStyle-CssClass="nowrap">
            <ItemTemplate>
                <%# binPhaseOutProduct(Eval("phasedOut"), Eval("ProductType"), Eval("link"), Eval("SProductID"), Eval("name"), Eval("status"))%>
            </ItemTemplate>
            <HeaderTemplate>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_Number)%></HeaderTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Description">
            <ItemTemplate>
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
            </ItemTemplate>
            <HeaderTemplate>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Description)%></HeaderTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Price" SortExpression="price" ItemStyle-CssClass="right">
            <ItemTemplate>
                <%#Eval("price")%>
            </ItemTemplate>
            <HeaderTemplate>
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price)%></HeaderTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<div class="ProductListBottomPager">
    <asp:Literal ID="bottomPager" runat="server"></asp:Literal>
</div>
