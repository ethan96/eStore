<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master"
    AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.Search" %>

<%@ Register Src="~/Modules/CertifiedPeripherals/LeftSideMenus.ascx" TagName="LeftSideMenus"
    TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreConfigurableRightContent"
    runat="server">
    <uc1:LeftSideMenus ID="LeftSideMenus1" runat="server" />
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="epaps-row780">
        <div class="epaps-title-bgGray-borderLeft">
            <h1>
                Search Result</h1>
        </div>
        <eStore:Repeater ID="rpSearchResultCategory" runat="server">
            <HeaderTemplate>
                <div class="epaps-tabs">
                    <ul>
                        <li class="epaps-tabOn">All</li>
            </HeaderTemplate>
            <ItemTemplate>
                <li categoryid="<%# Eval("Id")%>">
                    <%# Eval("DisplayName")%></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul></div>
            </FooterTemplate>
        </eStore:Repeater>
        <eStore:Repeater ID="rpProducts" runat="server">
            <HeaderTemplate>
                <div class="epaps-tabTxt" style="display: block;">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="epaps-searchRow" categoryid="<%# Eval("ProductCategoryId")%>">
                    <div class="epaps-searchImg">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <img src="<%#  Eval("thumbnailImageX")  %>" width="166" height="166" border="0" /></a></div>
                    <div class="epaps-searchTxt">
                        <div class="epaps-searchTitle">
                            <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                <%# Eval("productDescX")%></a></div>
                        <div class="epaps-productprice">
                            <%# eStore.Presentation.Product.ProductPrice.getPrice((eStore.POCOS.PStoreProduct) Container.DataItem)%></div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
                <div class="simple-pagination">
                </div>
            </FooterTemplate>
        </eStore:Repeater>
        <div id="nofoundmessage" runat="server" clientidmode="Static">
            <b>
                <strong>No products were found.</strong></b><div class="clear">
            </div>
            <p>
                Some Search Tips:</p><div class="clear">
            </div>
            <ul style="list-style-type:disc; padding-left:20px;">
                <li>Make sure all words are spelled correctly.</li>
                <li>Try different keywords.</li>
                <li>The product you'd like to search might be phased out and please contact our sales
                    for support.</li>
            </ul>
        </div>
    </div>
    <script type="text/javascript" language="javascript">
        var panelseletor = ".epaps-row780";
        paginationproducts(panelseletor, ".epaps-tabTxt div[categoryid]", false);

        $(".epaps-tabs li").click(function () {
            $(this).siblings().removeClass("epaps-tabOn");
            $(this).addClass("epaps-tabOn");
            var itemsseletor = ".epaps-tabTxt div[categoryid='" + $(this).attr("categoryid") + "']";
            if ($(this).attr("categoryid") == undefined) {
                itemsseletor = ".epaps-tabTxt div[categoryid]";
            }
            else
            { itemsseletor = ".epaps-tabTxt div[categoryid='" + $(this).attr("categoryid") + "']"; }
            paginationproducts(panelseletor, itemsseletor, true);
            return false;
        });
    </script>
    <!-- ************* epaps-row780 ************* -->
</asp:Content>
