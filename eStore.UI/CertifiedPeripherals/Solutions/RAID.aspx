<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true"
    CodeBehind="RAID.aspx.cs" Inherits="eStore.UI.CertifiedPeripherals.Solutions.RAID" %>

<%@ Register Src="~/Modules/CertifiedPeripherals/LeftSideMenus.ascx" TagName="LeftSideMenus"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <div class="eStore_container eStore_block980">
        <div class="epaps-bannerRow">
            <div class="epaps-bannerBg">
                <img src="/images/980220_3.jpg" width="980" height="220" /></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <div class="master-wrapper-side">
        <uc1:LeftSideMenus ID="LeftSideMenus1" runat="server" />
        <div id="subcategories">
            <eStore:Repeater ID="rpsubcategories" runat="server">
                <HeaderTemplate>
                    <div class="epaps-subcategory">
                        <div class="title">
                            <asp:Literal ID="subcategorytitle" runat="server"></asp:Literal></div>
                        <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="#" category="<%# Eval("Id") %>">
                        <%# Eval("DisplayName")%></a></li></ItemTemplate>
                <FooterTemplate>
                    </ul> </div></FooterTemplate>
            </eStore:Repeater>
        </div>
        <div id="productfilters">
        </div>
        <div id="storeSideAds">
        </div>
        <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="master-wrapper-center">
        <div class="master-wrapper-cph">
            <div class="epaps-row780">
                <div class="epaps-title-bgGray-borderLeft">
                    <h1>
                        What is RAID?</h1>
                </div>
                <div class="epaps-SolutionTxt">
                    <div class="epaps-SolutionContent">
                        <b>RAID</b> (Redundant Array of Independent Disks) is a data storage technology
                        that allows multiple hard drives to be combined into a single storage unit. Data
                        is distributed across the drives in one of several ways or RAID levels," depending
                        on the level of redundancy and performance required. RAID levels are not ratings,
                        but rather classifications of functionality. Different RAID levels offer dramatic
                        differences in performance, data availability, and data integrity depending on the
                        specific I/O environment. There is no single RAID level that is perfect for all
                        users.</div>
                    <div class="epaps-SolutionLink">
                        <a href="http://www.advantech.com/certified-peripherals/Solution/raid.aspx">Help me
                            select RAID</a></div>
                </div>
            </div>
            <div class="epaps-row780">
                <div class="epaps-title-bgGray-borderLeft">
                    <h1>
                        RAID Solutions</h1>
                </div>
                <eStore:Repeater ID="rpBundles" runat="server">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="epaps-SolutionCol">
                            <h4>
                                <%# Eval("GroupName")%></h4>
                            <div class="epaps-SolutionCol-block">
                                <eStore:Repeater ID="rpBundle" runat="server" DataSource='<%# Eval("GroupItems") %>'>
                                    <ItemTemplate>
                                        <div class="epaps-SolutionCol-item">
                                            <div class="epaps-SolutionCol-itemTop">
                                                <eStore:Repeater ID="rpBundleItems" runat="server" DataSource='<%# Eval("Items") %>'>
                                                    <ItemTemplate>
                                                        <div class="epaps-SolutionCol-itemProduct">
                                                            <div class="epaps-SolutionCol-itemProductImg">
                                                                <img src="<%# Eval("thumbnailImageX")%>" width="166" height="166" border="0" /></div>
                                                            <div class="epaps-SolutionCol-itemProductTxt">
                                                                <div class="epaps-SolutionCol-itemProductLink">
                                                                    <a href="<%# Eval("Url")%>">
                                                                        <%# Eval("name")%></a></div>
                                                                <div class="epaps-SolutionCol-itemProductMsg">
                                                                    <%# Eval("productDescX")%></div>
                                                                <div class="epaps-SolutionCol-itemProductQty">
                                                                    Qty：<%# Eval("QuantityPerSet")%></div>
                                                            </div>
                                                        </div>
                                                        <!--epaps-SolutionCol-itemProduct-->
                                                    </ItemTemplate>
                                                </eStore:Repeater>
                                            </div>
                                            <!--epaps-SolutionCol-item-->
                                            <div class="epaps-SolutionCol-itemBottom">
                                                <div class="epaps-SolutionCol-itemPrice">
                                                    Price：<span><%# eStore.Presentation.Product.ProductPrice.FormartPriceWithCurrency( (decimal)Eval("TotalPrice"))%></span></div>
                                                <div class="epaps-btn pull-right">
                                                    <asp:LinkButton ID="lbAdd2Cart" runat="server" CommandArgument='<%# Eval("BundleId")+"|"+Eval("GroupName")%>'
                                                        OnClick="lbAdd2Cart_Click" CssClass="needlogin">Add to Cart</asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </eStore:Repeater>
                                <!--epaps-SolutionCol-itemBottom-->
                            </div>
                            <!--epaps-SolutionCol-block-->
                            <div class="epaps-ViewMore epaps-ViewIcon">
                                View More</div>
                            <!--epaps-SolutionCol-->
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </eStore:Repeater>
            </div>
            <div class="clearfix">
            </div>
        </div>
    </div>
</asp:Content>
