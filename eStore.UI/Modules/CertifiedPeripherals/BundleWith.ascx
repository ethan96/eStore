<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BundleWith.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.BundleWith" %>
<%@ Register Src="CarouFredProductList.ascx" TagName="CarouFredProductList" TagPrefix="uc1" %>
<div class="epaps-row780">
    <h5>
        Bundled With Me</h5>
    <eStore:Repeater ID="rpbwm" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <div class="epaps-productRow-bgBlue  epaps-comparecontainer">
                <div class="epaps-title_line">
                    <h3>
                        <%# Eval("GroupName") %>
                    </h3>
                </div>
                <div class="epaps-carousel" id="groupcarousel" runat="server">
                    <div class="caroufredsel_wrapper">
                        <eStore:Repeater ID="rpProducts" runat="server" OnItemDataBound="rpMB_ItemDataBound"
                            DataSource='<%# Eval("Products") %>'>
                            <HeaderTemplate>
                                <ul class="productlist">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="epaps-productCol" bundleid="<%# Eval("StoreProductBundleId") %>">
                                    <div class="epaps-productImg epaps-productImgwithborder">
                                        <asp:Literal ID="lpstorefeatures" runat="server"></asp:Literal>   <asp:HyperLink ID="hlproductimg" runat="server"></asp:HyperLink>
                                    </div>
                                    <div class="epaps-productContent">
                                        <div class="epaps-productLink">
                                            <asp:Literal ID="lproductDescX" runat="server"></asp:Literal>
                                        </div>
 
                                        <div class="epaps-productprice">
                                            <div class="epaps-productprice-now">
                                                <asp:Literal ID="lBundleListPrice" runat="server"></asp:Literal>
                                            </div>
                                            <div class="epaps-productprice-ago">
                                                <asp:Literal ID="lBundleMarkupPrice" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <div class="epaps-rowBottom text-center">
                                        <asp:Literal ID="lFreeShipping" runat=server></asp:Literal>
                                            <div class="epaps-btn">
                                              <asp:LinkButton ID="lAdd2Cart" runat="server" onclick="lAdd2Cart_Click"  CommandArgument='<%# Eval("StoreProductBundleId")+"|"+Eval("Id")%>'  CssClass="needlogin"></asp:LinkButton></div>
                                        </div>
                                    </div>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </eStore:Repeater>
                        <div class="carousel-control">
                            <a class="epaps-arrow1" href="#"></a><span id="Span1" class="pager"></span><a class="epaps-arrow2"
                                href="#"></a>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <div class="clearfix">
            </div>
        </FooterTemplate>
    </eStore:Repeater>
</div>
