<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master"
    AutoEventWireup="true" CodeBehind="IotModel.aspx.cs" Inherits="eStore.UI.Product.IotModel" %>
<%@ Register Src="../Modules/IoTMart/HotDeals.ascx" TagName="HotDeals" TagPrefix="uc1" %>
<%@ Register src="../Modules/IoTMart/IotProductList.ascx" tagname="IotProductList" tagprefix="uc2" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="eStoreLeftContent">
    <div class="iot-navBlock">
        <asp:Literal ID="ltImageSideAdv" runat="server"></asp:Literal></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="iot-block">
        <div class="iot-breadcrumb">
            <eStore:YouAreHere ID="YouAreHere1" runat="server" />
        </div>
    </div>
    <!--iot-block-->
    <div class="iot-block">
        <div class="iot-title">
            <asp:Literal ID="ltCategoryName" runat="server"></asp:Literal>
        </div>
        <div class="iot-iconLinkBlock">
            <asp:Repeater ID="rpSubCateogries" runat="server">
                <ItemTemplate>
                    <a href="<%# pageUrl %>#<%# Eval("CategoryPath")%>">
                        <%#Eval("localCategoryNameX")%></a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="iot-att">
            <asp:Literal ID="ltCateDescription" runat="server"></asp:Literal>
        </div>
    </div>
    <asp:Panel ID="pnHotDeals" runat="server">
        <div class="iot-block">
            <div class="iot-titleBG2">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Featured)%></div>
            <div class="iot-highlightBlock">
                <uc1:HotDeals ID="HotDeals1" runat="server" />
            </div>
        </div>
    </asp:Panel>
    
    <!--iot-block-->
    <div class="IotAds" adtype="oneStore780Ads"></div>
    <!--iot-block-->
    <uc2:IotProductList ID="IotProductList1" runat="server" />
    <!--iot-block-->
        <script type="text/javascript">
            $(document).ready(function () {
                $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=7<%=eStore.Presentation.eStoreContext.Current.keywordString%>",
               function (data) {
                   $(".IotAds[adtype='oneStore780Ads']").IotModelAdBanners(data, 1);
               });

                $(".youtube").colorbox({ iframe: true, innerWidth: 800, innerHeight: 600 });
            });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <asp:Panel ID="pnMostBuy" runat="server">    
    <div class="iot-container">
        <div class="iot-block iot-eStoreTop">
            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_SeeMoreProducts)%>
            <asp:HyperLink ID="hyVisiteNow" runat="server" Target="_blank">Visit Now</asp:HyperLink></div>
        <div id="Iot-MostBuyPro" class="iot-block iot-eStoreBottom">
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#Iot-MostBuyPro").eStoreMostBuyPro(3);
        });
    </script>
    </asp:Panel>
</asp:Content>