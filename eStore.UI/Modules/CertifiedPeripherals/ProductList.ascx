<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductList.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.ProductList" %>
<eStore:Repeater ID="rpProducts" runat="server">
    <HeaderTemplate>
        <ul class="productlist">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="epaps-productCol" productid="<%# Eval("SProductID") %>">
            <div class="epaps-productImg epaps-productImgwithborder">
                <%# string.Format("<span class=\"epaps-{0}\"></span>", Eval("pStorePromotionType") ) %>
                <div class="epaps-productlogo<%# string.IsNullOrEmpty(Eval("Manufacturer").ToString())?" hiddenitem":""%>">
                    <img src="https://wfcache.advantech.com/www/certified-peripherals/documents/LOGO/<%# Eval("Manufacturer") %>.png" /></div>
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <img src="<%#  Eval("thumbnailImageX")  %>" width="166" height="166" />
                </a>
                <div class="tooltip epaps-Longevity<%# ((bool)Eval("isLongevity"))?"":" hiddenitem"%>">
                    <span class="classic">Products marked with the Longevity icon have a longer life than
                        products without the longevity icon. </span>
                </div>
            </div>
            <div class="epaps-productContent">
                <div class="epaps-productLink epaps-immediate">
                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("productDescX")%></a></div>
                <div>
                    
                    <div class="epaps-productListPrice" id="pricepanle" runat="server" visible='<%#ShowPrice%>'>
                    <div class="epaps-productprice" visible='<%#!ShowPrice%>'>
                        <div id="Div1" class="epaps-productListFreeShipping" runat="server" visible='<%# Eval("freeShipping") %>'><img src='<%= ResolveUrl("~/images/AUS/eStore_icon_shipping.png") %>' /></div>
                        <%# eStore.Presentation.Product.ProductPrice.getPrice((eStore.POCOS.PStoreProduct) Container.DataItem)%>
                    </div>                    
                </div>
                </div>
                
                <div class="epaps-rowBottom text-center" id="actionpanel" runat="server" visible='<%#ShowActions%>'>
                    <div class="epaps-btn">
                        <asp:LinkButton ID="lAdd2Cart" runat="server" OnClick="lAdd2Cart_Click" CommandArgument='<%# Eval("SProductID")%>'
                            CssClass="needlogin" Text="Add to Cart"></asp:LinkButton></div>
                </div>
            </div>
            <%# ShowCompareCheckbox ? string.Format("<div class=\"epaps-compareBlock\"><input type=\"checkbox\" name=\"parts\" id=\"ckbcompare_{1}\"  value=\"{0}\"><label for=\"ckbcompare\">Compare Product</label></div>", Eval("SProductId"), Eval("PProductId")) : ""%>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</eStore:Repeater>
