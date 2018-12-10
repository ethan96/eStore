<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="ProductLineV2.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.ProductLineV2" %>

<%@ Register Src="~/Modules/CertifiedPeripherals/LeftSideMenus.ascx" TagName="LeftSideMenus"
    TagPrefix="uc1" %>
<%@ Register Src="../Modules/CertifiedPeripherals/CarouFredProductList.ascx" TagName="CarouFredProductList"
    TagPrefix="uc3" %>
<%@ Register Src="../Modules/CertifiedPeripherals/YouAreHere.ascx" TagName="YouAreHere"
    TagPrefix="uc2" %>
<%@ Register Src="../Modules/CertifiedPeripherals/PaginationProductList.ascx" TagName="PaginationProductList"
    TagPrefix="uc4" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <div class="master-wrapper-side">
        <eStore:Repeater ID="rpsubcategories" runat="server">
            <HeaderTemplate>
                <div id="subcategories">
                    <div class="epaps-subcategory">
                        <div class="title">
                            <asp:Literal ID="subcategorytitle" runat="server"></asp:Literal></div>
                        <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                    <%# Eval("DisplayName") %></a></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul> </div> </div>
            </FooterTemplate>
        </eStore:Repeater>
        <div class="epaps-specfilter" id="specfilter" runat="server" visible="false">
            <div class="epaps-title">
                <asp:Literal ID="subcategorytitleforspec" runat="server"></asp:Literal>
            </div>
            <eStore:Repeater ID="rpFilter" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="epaps-selectRow">
                        <div class="epaps-selectTitle epaps-openBox">
                            <%# Eval("AttributeName")%></div>
                        <eStore:Repeater ID="rpFilterValues" runat="server" DataSource='<%# Eval("Values") %>'>
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <input type="checkbox" products="<%# Eval("Products") %>"><span class="AttributeValueName"><%# Eval("Name")%></span>
                                    (<span class="matchedcount"><%# Eval("Count")%></span>)</li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </eStore:Repeater>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </eStore:Repeater>
        </div>
        <div id="storeSideAds">
        </div>
        <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="master-wrapper-center">
        <div class="master-wrapper-cph">
            <div class="eStore_breadcrumb">
            <uc2:YouAreHere ID="YouAreHere1" runat="server" /></div>
            <div class="epaps-row780">
                <div class="epaps-bannerRow">
                    <div class="epaps-bannerTop">
                        <div class="epaps-bannerTopLeft">
                            <asp:Image ID="imgCategoryBanner" runat="server" Width="335" Height="176" />
                        </div>
                        <div class="epaps-bannerTopRight" id="headerbanner" runat="server">
                            <div class="epaps-bannerImg" id="bannerImg" runat="server">
                            </div>
                            <div class="epaps-bannerMsg">
                                <div class="title">
                                    <asp:Literal ID="bannertitle" runat="server"></asp:Literal>
                                </div>
                                <div class="content">
                                    <asp:Literal ID="bannercontent" runat="server"></asp:Literal>
                                </div>
                                <div class="link">
                                    <asp:HyperLink ID="bannerlink" runat="server">More</asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="epaps-bannerBottom">
                        <asp:Literal ID="lname" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
            <div class="epaps-row780" id="PromotionProductsPanel" runat="server" clientidmode="Static"
                visible="false">
                <div class="epaps-title-bgGray-borderLeft">
                    <h1>
                        Promotion Products</h1>
                    <div class="clearfix">
                    </div>
                </div>
                <uc3:CarouFredProductList ID="CarouFredProductList1" runat="server" />
                <div class="clearfix">
                </div>
            </div>
            <div class="epaps-row780 epaps-SelectedBox">
            </div>
            <div class="epaps-row780">
                <div class="epaps-title-bgGray-borderLeft">
                    <h1>
                        Products</h1>
                    <%if (this.PaginationProductList1.ShowCompareCheckbox)
                      { %>
                    <div class="caps-CurrentSelections pull-right">
                        <div class="caps-title_add epaps-openBox">
                            Current Selections：(<span class="CurrentSelectionsCount">0</span>)
                        </div>
                        <ul class="caps-content_add">
                        </ul>
                    </div>
                    <%} %>
                    <div class="clearfix">
                    </div>
                </div>
                <!-- ************* epaps-productRow2 ************* -->
                <uc4:PaginationProductList ID="PaginationProductList1" runat="server" />
                <div class="clearfix">
                </div>
            </div>
        </div>
    </div>
</asp:Content>
