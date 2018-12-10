<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn2013.Master" AutoEventWireup="true" CodeBehind="ChinaServicePartner.aspx.cs" Inherits="eStore.UI.ChinaServicePartner" %>
<%@ Register Src="~/Modules/liveperson.ascx" TagName="liveperson" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
<%--<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:Repeater ID="rpPromotionProduct" runat="server" OnItemDataBound="rpPromotionProduct_ItemDataBound">
        <HeaderTemplate>
            <div id="SaleArea">
                <div id="SaleHeader">
                    特卖专区</div>
                <div id="SaleProduct">
                    <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <span class="title">
                    <asp:HyperLink ID="hlcategory" runat="server"></asp:HyperLink>
                </span>
                <div class="photo">
                    <%# string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"88px\" height=\"88px\" /></a> "
, ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                              , Eval("thumbnailImageX")
, Eval("name") )%></div>
                <div class="info">
                    <div class="sno">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            <%# Eval("name")%></a></div>
                    <div class="desc">
                        <%# Eval("productDescX")%></div>
                  
                        <%#eStore.Presentation.Product.ProductPrice.getAJAXProductPrice(Container.DataItem as eStore.POCOS.Product, eStore.Presentation.Product.PriceStyle.productprice)%>
                    <div class="bushop">
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                            SHOP NOW</a></div>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul> </div> </div></FooterTemplate>
    </eStore:Repeater>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div id="PromoArea">
        <div id="SliderArea">
            <div id="AreaTitle"><p class="smallP">服务商据点信息</p></div>            <div id="MapInfo">
                <div id="flashDiv"></div>
                <div id="boxContainer">
	                <div id="maskDiv"></div>
    	            <div id="boxDiv">
                        <div id="BU-Close">X</div>
                        <div id="Title"><span class="colorRed">◎</span> <span id="stateTitle">加载中..</span></div>
                        <div id="DetailDiv"></div>
                    </div>
                </div>
            </div>
        </div>
        <div id="Banner">
            <eStore:liveperson ID="liveperson1" runat="server" UserLargerImage="true" />
        </div>
    </div>
    <script>
        $(document).ready(function () {
            var flashvars = {};
            var params = {
                scale: "exactfit",
                menu: "false",
                wmode: "transparent"
            };
            var attributes = {};
            swfobject.embedSWF("swf/chinaMap.swf", "flashDiv", "562", "534", "9.0.0", "swf/expressInstall.swf", flashvars, params, attributes);
        });
        function getData(stateCode) {
            $('#SliderArea #MapInfo #boxContainer #boxDiv #stateTitle').text("加载中..");
            $('#SliderArea #MapInfo #boxContainer #boxDiv #DetailDiv').empty();
            
            var partnerContent = "";
            eStore.UI.eStoreScripts.getPartnerList("China", stateCode, "eStore", function (resource) {
                if (resource != null && resource != "") {
                    for (var i = 0; i < resource.length; i++) {
                        if (i == 0)
                            $('#SliderArea #MapInfo #boxContainer #boxDiv #stateTitle').text(resource[i].STATE);
                        var partnerEmail = "sales@advantech.com.cn";//默认
                        if (resource[i].Main_Contact_Email != null && resource[i].Main_Contact_Email != undefined && resource[i].Main_Contact_Email != "")
                            partnerEmail = resource[i].Main_Contact_Email;
                        partnerContent += '<div class="DetailList"><div class="LeftBlock">' + resource[i].COMPANY_NAME + '</div><div class="RightBlock">' + resource[i].COMPANY_ADDRESS + '<br />' + resource[i].TEL_NO +
                                        '<br /><a href="mailto:' + partnerEmail + '">' + partnerEmail + '</a></div><div class="clear"></div></div>';
                    }
                }
                else
                    partnerContent = "<br>很抱歉，您搜索的省份目前无适合的服务商，请点击其他邻近省份.";
                $('#SliderArea #MapInfo #boxContainer #boxDiv #DetailDiv').append(partnerContent);
            });

            $('#SliderArea #MapInfo #boxContainer').show();

            $('#SliderArea #MapInfo #boxContainer #BU-Close').click(function () {
                $('#SliderArea #MapInfo #boxContainer').hide();
            });
            return false;
        }
    </script>
</asp:Content>
