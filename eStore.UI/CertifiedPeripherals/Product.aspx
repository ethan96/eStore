<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="Product.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.Product" %>

<%@ Register Src="~/Modules/CertifiedPeripherals/LeftSideMenus.ascx" TagName="LeftSideMenus"
    TagPrefix="uc1" %>
<%@ Register Src="../Modules/CertifiedPeripherals/CarouFredProductList.ascx" TagName="CarouFredProductList"
    TagPrefix="uc3" %>
<%@ Register Src="../Modules/CertifiedPeripherals/BundleWith.ascx" TagName="BundleWith"
    TagPrefix="uc2" %>
<%@ Register Src="../Modules/CertifiedPeripherals/BundleOnly.ascx" TagName="BundleOnly"
    TagPrefix="uc4" %>
<%@ Register Src="../Modules/CertifiedPeripherals/YouAreHere.ascx" TagName="YouAreHere"
    TagPrefix="uc5" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <div class="master-wrapper-side">
        <uc1:LeftSideMenus ID="LeftSideMenus1" runat="server" />
        <div id="storeSideAds">
        </div>
         <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="master-wrapper-center">
        <div class="master-wrapper-cph">
            <div class="eStore_breadcrumb">
                <uc5:YouAreHere ID="YouAreHere1" runat="server" />
            </div>
            <div class="epaps-row780">
                <div class="epaps-productCol-img">
                    <div class="epaps-mainImg">
                        <asp:Image ID="imgProductImage" runat="server" Width="166" Height="166" />
                        <div class="tooltip epaps-Longevity" runat="server" id="iconLongevity">
                            <span class="classic">Products marked with the Longevity icon have a longer life than
                                products without the longevity icon. </span>
                        </div>
                    </div>
                    <eStore:Repeater ID="rpThumbnails" runat="server">
                        <HeaderTemplate>
                            <div class="epaps-listImg">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <a href="<%# Eval("ResourceURL")  %>">
                                <img src="<%# Eval("ResourceURL")  %>" width="166" height="166" alt="" <%#  Container.ItemIndex==0?"class='onShowe'":""%>></a>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </eStore:Repeater>
                </div>
                <div class="epaps-productCol-msg">
                    <div class="epaps-title_line">
                        <h3>
                            <asp:Literal ID="ldescript" runat="server"></asp:Literal>
                        </h3>
                    </div>
                    <div class="epaps-productCol-left">
                        <asp:Literal ID="lfeatures" runat="server"></asp:Literal>
                        <div class="clearfix">
                        </div>
                        <eStore:Repeater ID="rpvideo" runat="server">
                            <HeaderTemplate>
                                <ul class="epaps-productCol-video">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li><a href="<%# Eval("Hyperlink") %>" target="<%# Eval("Target") %>">
                                    <img src="<%# Eval("image") %>" /></a> </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </eStore:Repeater>
                    </div>
                    <div class="epaps-productCol-right text-right" id="actionpanel" runat="server">
                        <div class="epaps-productprice">
                            <div class="epaps-productprice">
                                <asp:Literal ID="lPrice" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div id="lIsFreeShipping" class="epaps-productFreeShipping" runat="server">
                            <img src='<%= ResolveUrl("~/images/AUS/eStore_icon_shipping.png") %>' /></div>
                        <div class="epaps-btn">
                            <asp:LinkButton ID="lAdd2Cart" runat="server" OnClick="lAdd2Cart_Click" CssClass="needlogin"></asp:LinkButton>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="LimitedQuantity">
                            <asp:Label ID="lLimitedQuantity" runat="server"></asp:Label>
                        </div>
                         <asp:HyperLink ID="hlSpecialOrder" runat="server"  CssClass="SpecialOrder" Text ="Special Order"></asp:HyperLink>
                        <div class="SpecialShipment">
                            <asp:Label ID="lSpecialShipment" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <uc4:BundleOnly ID="BundleOnly1" runat="server" />
            <uc2:BundleWith ID="BundleWith1" runat="server" />
            <uc3:CarouFredProductList ID="CompatibleRecommendlist" runat="server" ShowBorder="false"
                ShowActions="true" ShowCompareCheckbox="false" />
            <uc3:CarouFredProductList ID="AssociateProductslist" runat="server" ShowBorder="false"
                ShowActions="true" ShowCompareCheckbox="false" />
            <uc3:CarouFredProductList ID="YouMayAlsoBuylist" runat="server" ShowBorder="false"
                ShowActions="true" ShowCompareCheckbox="false" />
            <uc3:CarouFredProductList ID="YourRecentlyViewedItemslist" runat="server" ShowActions="true"
                ShowCompareCheckbox="false" />
            <div class="clearfix">
            </div>
            <div class="epaps-row780" id="specpanle" runat="server">
                <div class="epaps-title-bgGray">
                    <h2>
                        Product Spec</h2>
                </div>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="epaps-table epaps-ProductSpec">
                    <tr>
                        <th>
                            Description
                        </th>
                        <th>
                            <asp:Literal ID="lspecdesc" runat="server"></asp:Literal>
                        </th>
                    </tr>
                    <eStore:Repeater ID="rpSpec" runat="server">
                        <ItemTemplate>
                            <tr>
                                <th>
                                    <%# Eval("LocalAttributeName")%>
                                </th>
                                <td>
                                    <%# Eval("LocalValueName")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </eStore:Repeater>
                </table>
                <div class="epaps-ViewAll epaps-ViewIcon">
                    View Complete Specification</div>
            </div>
        </div>
    </div>
</asp:Content>
