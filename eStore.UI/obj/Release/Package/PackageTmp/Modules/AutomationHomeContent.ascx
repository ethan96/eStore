<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutomationHomeContent.ascx.cs"
    Inherits="eStore.UI.Modules.AutomationHomeContent" %>
<%@ Register Src="liveperson.ascx" TagName="liveperson" TagPrefix="eStore" %>
<%@ Register Src="eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
<div id="PromoArea">
    <div id="SliderArea">
        <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Tabs"
            MinHeight="280" showDescription="false" />
    </div>
    <div id="Banner">
        <eStore:liveperson ID="liveperson1" runat="server" UserLargerImage="true" />
        <eStore:Repeater ID="rptTodaysDeals" runat="server">
            <HeaderTemplate>
                <ul class="todaysDeals">
            </HeaderTemplate>
            <ItemTemplate>
                <li><a href="<%# ResolveUrl(Eval("Hyperlink").ToString())%>" title='<%# Eval("Title")%>'
                    target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>">
                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# Eval("imageFileHost") %>' />
                </a></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </eStore:Repeater>
    </div>
</div>
<div id="WizardArea">
    <%--  <div id="icon">
    </div>
    <div id="QArea">
        <div id="imgQ">
        </div>
        <div id="textQ">
            How Do You Want The Data Acquisition Modules To Integrate with Your System ?</div>
        <div id="busearch">
        <asp:HyperLink ID="hlDAQ" runat="server" NavigateUrl="~/DAQ.aspx">
            立即搜寻体验</asp:HyperLink></div>
    </div>--%>
    <%--  <asp:ImageButton ID="imgbtnCredit" runat="server" ImageUrl="~/images/hongli.jpg" PostBackUrl="~/%e7%a7%af%e5%88%86%e4%b8%93%e5%8c%ba/%e7%a7%af%e5%88%86%e4%b8%93%e5%8c%ba/dhtml-708.htm" />--%>
    <eStore:eStoreLiquidSlider ID="elsRewardsAds" runat="server" navigationType="Numbers"
        MinHeight="106" showDescription="false" />
</div>
<div id="ProductArea">
    <eStore:Repeater ID="rpHotProduct" runat="server">
        <HeaderTemplate>
            <div id="HotArea">
                <div class="AreaTitle">
                    <p>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.iautomation_store_Hot_Products)%></p>
                </div>
                <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <div class="prodimg">
                    <%# string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"158px\" height=\"158px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                              , Eval("thumbnailImageX")
, Eval("name") )%></div>
                <div class="info">
                    <div class="sno">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("name")%></a></div>
                    <div class="desc">
                        <%# Eval("productDescX")%></div>
                    <div class="bushop">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            SHOP NOW</a></div>
                </div>
                <div class="price">
                    <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productpriceLarge)%></div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul> </div></FooterTemplate>
    </eStore:Repeater>
    <eStore:Repeater ID="rpNewProduct" runat="server">
        <HeaderTemplate>
            <div id="NewArea">
                <div class="AreaTitle">
                    <p>
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.iautomation_store_New_Products)%></p>
                </div>
                <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <div class="prodimg">
                    <%# string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"158px\" height=\"158px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                              , Eval("thumbnailImageX")
, Eval("name") )%></div>
                <div class="info">
                    <div class="sno">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("name")%></a></div>
                    <div class="desc">
                        <%# Eval("productDescX")%></div>
                    <div class="bushop">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            SHOP NOW</a></div>
                </div>
                <div class="price">
                    <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productpriceLarge)%></div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul> </div></FooterTemplate>
    </eStore:Repeater>
</div>
