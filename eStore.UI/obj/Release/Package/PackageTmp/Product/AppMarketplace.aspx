<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="AppMarketplace.aspx.cs" Inherits="eStore.UI.Product.AppMarketplace" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/compare")%>

    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/compare")%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <div class="eStore_container eStore_block980 product">
        <eStore:YouAreHereMutli ID="YouAreHereMutli1" runat="server" />
        <div class="productPic">

            <asp:Image ID="imgLargePicture" runat="server" />
            <div class="clearfix"></div>
        </div>

        <div class="productDescription">
            <h2>
                <asp:Literal ID="lProductName" runat="server" /><span class="icon">
                    <asp:Literal ID="ltproductStatus" runat="server"></asp:Literal></span> <span class="remind font18">
                        <asp:Literal ID="ltPhaseOut" runat="server" Visible="false"></asp:Literal></span>
            </h2>
            <p>
                <asp:Literal ID="lShortDescription" runat="server" />
            </p>
            <asp:HyperLink ID="hlmanual" runat="server" CssClass="manual" Text="Download Manual" Visible="false"></asp:HyperLink>

            <div class="marketplace-info">
                <div class="subscriptionPrice">
                    <asp:Literal ID="lPrice" runat="server" />
                </div>

            </div>
            <div class="buttons">
                <div class="cart"><asp:HyperLink ClientIDMode="Static" runat="server" id="SelectyourSystem" class="eStore_btn">Select your System</asp:HyperLink></div>
                <div class="ghost">
                    <asp:HyperLink ID="hlDemo" runat="server" Text="Watch Demo" Visible="false" CssClass="youtube eStore_btn1"></asp:HyperLink>
                </div>
            </div>
        </div>

        <div id="ProductWidget"></div>

        <div class="clearfix"></div>
        <a id="SelectyourSystemspan"></a>
        <eStore.V4:ProductsComparison ID="ProductsComparison1" runat="server" Title="Select your Systems"></eStore.V4:ProductsComparison>

    </div>

    <script>
        function openTab(evt, cityName) {
            // Declare all variables
            var i, tabcontent, tablinks;

            // Get all elements with class="tabcontent" and hide them
            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }

            // Get all elements with class="tablinks" and remove the class "active"
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }

            // Show the current tab, and add an "active" class to the button that opened the tab
            document.getElementById(cityName).style.display = "block";
            evt.currentTarget.className += " active";
        }
        var addParams = function( url, data )
        {
            if ( ! $.isEmptyObject(data) )
            {
                url += ( url.indexOf('?') >= 0 ? '&' : '?' ) + $.param(data);
            }

            return url;
        }
        var productWidgetId = <%=productWidgetId %>;
        $(document).ready(function () {
            if(productWidgetId != 0){
                $.get('ProWidget.ashx', { WidgetID: productWidgetId }, function(result){
                    $("#ProductWidget").html(result);
                    compareHeight();
                    if(isExitsFunction("reLoadWidgetPageFunction")) {
                        reLoadWidgetPageFunction();
                    }
                }, 'html');
            }
            $.each(   $(".productlist a[href *='/system-']"), function(i, n){
                $(n).prop("href", addParams($(n).prop("href"),{options:<%=componentid%>}));
            });
            
        });
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
