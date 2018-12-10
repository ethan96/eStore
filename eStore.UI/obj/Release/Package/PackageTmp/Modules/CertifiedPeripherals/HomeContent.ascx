<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeContent.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.HomeContent" %>
<%@ Register Src="ProductList.ascx" TagName="ProductList" TagPrefix="uc1" %>
 
<eStore:Repeater ID="rpPromotionPStoreProducts" runat="server" OnItemDataBound="rpPromotionPStoreProducts_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="epaps-row780" group ="<%#Eval("Key") %>">
            <div class="epaps-title-bgGray-borderLeft">
                <h1>
                    <%#eStore.Presentation.eStoreLocalization.Tanslation("PStore.PromotionType." + Eval("Key").ToString())%>
                </h1>
                <a href="#" class="epaps-linkTxt viewallproducts">View All</a> <a href="#" class="epaps-linkTxt backalllist hidden">
                    Back</a>
            </div>
            <div class="epaps-productRow2" id="<%# Eval("Key") %>panel">
                <uc1:ProductList ID="ProductList1" runat="server"   ShowBorder="true" ShowCompareCheckbox="false"/>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</eStore:Repeater>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $(".epaps-productRow2")
        .each(function (i, n) {
            $(n).find("ul li:gt(7)").hide();
        });

        $(".viewallproducts").click(function () {
            $(this).siblings(".backalllist").show();
            $(this).hide();
            $mypanel = $(this).closest(".epaps-row780");
            $mypanel.find(".epaps-productRow2 ul li").show();
            $mypanel.siblings(".epaps-row780").each(function (i, n) {
                $(n).hide();
            });
            location.hash = null;
            location.hash = "#HeaderBanner";
            return false;
        });

        $(".backalllist").click(function () {
            $(this).siblings(".viewallproducts").show();
            $(this).hide();
            $mypanel = $(this).closest(".epaps-row780");
            $mypanel.find("ul li:gt(7)").hide();
            $mypanel.siblings(".epaps-row780").show();
            return false;
        });
    });
</script>
