<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecialProducts.ascx.cs"
    ViewStateMode="Disabled" Inherits="eStore.UI.Modules.SpecialProducts" %>
<div id="SpecialProductsTab">
    <eStore:Repeater ID="rpSpecialProductsTab" runat="server">
        <HeaderTemplate>
            <ul id="sptabs">
        </HeaderTemplate>
        <ItemTemplate>
            <li><a href='<%# "#sptab_" + Eval("TabName") %>'>
                <%# Eval("TabName")%></a></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul></FooterTemplate>
    </eStore:Repeater>
    <eStore:Repeater ID="rpSpecialProductsItems" runat="server">
        <ItemTemplate>
            <div id="<%# "sptab_" + Eval("TabName") %>" class="SpecialProductsItem">
                <asp:DataList ID="rpSpecialProductsItem" runat="server" DataSource='<%# Eval("TabProducts")%>'
                    RepeatColumns="5" ItemStyle-CssClass="spItem">
                    <ItemTemplate>
                        <div class="spItem">
                            <h4>
                                <a href="<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                    <%# Eval("name")%></a>
                            </h4>
                            <p>
                                <%# Eval("productDescX")%></p>
                            <a href="<%#  ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                <img src='<%# Eval("thumbnailImageX")%>' alt='<%# Eval("name")%>' /></a>
                            <%# eStore.Presentation.Product.ProductPrice.getPrice((eStore.POCOS.Product)Container.DataItem)%>
                            <a class="ItemLink" href='<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>'>
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </ItemTemplate>
    </eStore:Repeater>
</div>
<script type="text/ecmascript" language="javascript">
    $(function () {
        $("#SpecialProductsTab").tabs();
        $(".spItem").hover(
              function () {
                  $(this).addClass("spItemhover");
              },
              function () {
                  $(this).removeClass("spItemhover");
              });
    });  



</script>
