<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BundleOnly.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.BundleOnly" %>
<div class="epaps-row780">
    <h5>
        Select To Bundle
    </h5>
    <div class="epaps-productRow-bgBlue">
        <div class="epaps-title_line">
            <h3>
                Motherboard</h3>
            <a href="3.2.html" target="_blank">Compare</a><a class="epaps-lightBox" href="#epaps-lightBox_select">Help
                me select motherboard</a></div>
        <div class="epaps-carousel" id="mbcarousel">
            <div class="caroufredsel_wrapper">
                <eStore:Repeater ID="rpMB" runat="server" OnItemDataBound="rpMB_ItemDataBound">
                    <HeaderTemplate>
                        <ul class="productlist">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="epaps-productCol" bundleid="<%# Eval("StoreProductBundleId") %>">
                            <asp:Literal ID="lpstorefeatures" runat="server"></asp:Literal>
                            <div class="epaps-productImg epaps-popproductdetails">
                                <asp:HyperLink ID="hlproductimg" runat="server"></asp:HyperLink>
                            </div>
                            <div class="epaps-productContent">
                                <div class="epaps-productLink epaps-immediate">
                                    <asp:HyperLink ID="hlproductname" runat="server"></asp:HyperLink>
                                </div>
                                <div class="epaps-productTxt">
                                    <asp:Literal ID="lproductDescX" runat="server"></asp:Literal>
                                </div>
                                <div class="epaps-feature">
                                    <ul>
                                        <asp:Literal ID="lproductFeatures" runat="server"></asp:Literal></ul>
                                </div>
                            </div>
                            <asp:Literal ID="lcomparecheckbox" runat="server"></asp:Literal>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </eStore:Repeater>
                <div class="carousel-control">
                    <a class="epaps-arrow1" href="#"></a><span id="pager" class="pager"></span><a class="epaps-arrow2"
                        href="#"></a>
                </div>
            </div>
        </div>
        <div class="epaps-title_line">
            <h3>
                <asp:Literal ID="lbundlegourpname" runat="server"></asp:Literal>
            </h3>
            <a href="3.2.html" target="_blank">Compare</a></div>
        <div class="epaps-carousel" id="groupcarousel">
            <div class="caroufredsel_wrapper">
                <eStore:Repeater ID="rpProducts" runat="server" OnItemDataBound="rpMB_ItemDataBound">
                    <HeaderTemplate>
                        <ul class="productlistorig">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="epaps-productCol" bundleid="<%# Eval("StoreProductBundleId") %>">
                            <asp:Literal ID="lpstorefeatures" runat="server"></asp:Literal>
                            <div class="epaps-productImg epaps-productImgwithborder">
                                <asp:HyperLink ID="hlproductimg" runat="server"></asp:HyperLink>
                            </div>
                            <div class="epaps-productContent">
                                <div class="epaps-productLink epaps-immediate">
                                    <asp:HyperLink ID="hlproductname" runat="server"></asp:HyperLink>
                                </div>
                                <div class="epaps-productTxt">
                                    <asp:Literal ID="lproductDescX" runat="server"></asp:Literal>
                                </div>
                                <div class="epaps-feature hidden">
                                    <ul style="display: none">
                                        <asp:Literal ID="lproductFeatures" runat="server" Visible="false"></asp:Literal></ul>
                                </div>
                            </div>
                            <asp:Literal ID="lcomparecheckbox" runat="server"></asp:Literal>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </eStore:Repeater>
                <ul class="productlist">
                </ul>
                <div class="carousel-control">
                    <a class="epaps-arrow1" href="#"></a><span id="Span1" class="pager"></span><a class="epaps-arrow2"
                        href="#"></a>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="epaps-productCol-hidden">
</div>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        var bundleid = $("#mbcarousel ul.productlist>li:first").attr("bundleid");
        refreshgroupcarouselitmes(bundleid);
    });

    $("#mbcarousel ul.productlist>li").click(
    function () {
        var bundleid = $(this).attr("bundleid");
        refreshgroupcarouselitmes(bundleid);
        return false;
    }
    );

    function refreshgroupcarouselitmes(bundleid) {
        $("#groupcarousel ul.productlist").empty();
        $( "#groupcarousel ul.productlistorig>li[bundleid='" + bundleid + "']").clone().each(function(index,item){
            $("#groupcarousel ul.productlist").trigger("insertItem", item.outerHTML);
        });
       
    }
</script>
