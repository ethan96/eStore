<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdPopularProduct.ascx.cs" Inherits="eStore.UI.Modules.AdPopularProduct" %>
<% if (isShowPopularModel())
 {%>
<div id="MostProductContainer" runat="server" clientidmode="Static" class="RecentPopularProduct RecentPopularProduct_Blue">
    <div class="Title">
        <div class="Close"></div>
        &nbsp;&nbsp;&nbsp;Most popular product
    </div>
    <div id="RecentPopularMainContent" class="RecentPopularMainContent">
        <div class="ProductImg">
            <img id="RecentImg" src="http://downloadt.advantech.com/download/downloadlit.aspx?LIT_ID=25d1f275-93be-44f6-a4a8-95f56bd2901c" width="125" />
        </div>

        <div class="ProductMessage">
            <h3 id="RecentSproductId">ADAM-3937-BE</h3>
            <p id="RecentDescription" class="ProductDescription">Advantech ADAM Protocol for OPC Server</p>
            <div class="PriceContent">
                <div id="RecentPrice" class="Price">$45.00</div>
                <div class="ProductMore"><a id="aRecentPopularProduct" href="/OPC+Server/PCLS+OPC+ADM30+AE/model-PCLS-OPC/ADM30-AE.htm" style="font-size:10px;">more</a></div>
            </div>
        </div>
    </div>
</div>
<script>
    $(function () {
        $("#aRecentPopularProduct").button();

        $("#MostProductContainer .Close").toggle(function () {
            $("#RecentPopularMainContent").hide();
            $("#MostProductContainer").css("width", "250px");
            $("#MostProductContainer .Close").css("background-position", "left 0px");
        }, function () {
            $("#RecentPopularMainContent").show(300);
            $("#MostProductContainer").css("width", "350px");
            $("#MostProductContainer .Close").css("background-position", "left -18px");
        });

        MoveWindow();

        function MoveWindow() {
            setTimeout(function () {
                $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.getPopularProduct %><%=KeyWord %>&tid" + new Date(),
                    function (data) {
                        if (data != null && data != "" && data != undefined) {
                            $("#RecentImg").attr("src", data.TumbnailImage);
                            $("#RecentSproductId").text(data.SproductId);
                            $("#RecentDescription").text(data.Description);
                            $("#RecentPrice").text(data.Price);
                            $("#aRecentPopularProduct").attr("href", data.Link);
                            $("#MostProductContainer").show("clip");
                        }
                    }
                );
            }, 5000);
        }
    });
</script>
<% } %>