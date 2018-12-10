<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="SubCategory.aspx.cs" Inherits="eStore.UI.Product.SubCategory" %>

<%@ Register Src="../Modules/CategoryWithSubCategoryAndProductsV2.ascx" TagName="CategoryWithSubCategoryAndProductsV2"
    TagPrefix="eStore" %>
<%@ Register src="../Modules/eStoreLiquidSlider.ascx" tagname="eStoreLiquidSlider" tagprefix="eStore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:ProductCategory ID="ProductCategory1" runat="server" />
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <eStore:YouAreHere ID="YouAreHere1" runat="server" />
    <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
        dynamicArrows="false" dynamicTabs="true" showDescription="false"  MinHeight="120" />
    <h1>
        <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal>
    </h1>
    <div class="CategoryDescription">
        <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
    </div>
    <div class="clear">
    </div>
    <asp:DataList ID="dlCategory" ClientIDMode="Static" runat="server" RepeatColumns="2"
        Width="100%" ItemStyle-VerticalAlign="Top" OnItemDataBound="dlCategory_ItemDataBound">
        <ItemTemplate>
            <asp:PlaceHolder ID="phCategory" runat="server"></asp:PlaceHolder>
        </ItemTemplate>
        <ItemStyle CssClass="SubCategoryWithProducts LightSteelBlue" Width="372" />
        <AlternatingItemStyle CssClass="SubCategoryWithProducts DarkGray" Width="372" />
        <SeparatorTemplate>
        </SeparatorTemplate>
        <SeparatorStyle CssClass="dotlinemidial" />
    </asp:DataList>
    <div class="SubCategoryWithProducts MediumSlateBlue" runat="server" id="singlecategories">
        <eStore:CategoryWithSubCategoryAndProducts ID="CategoryWithSubCategoryAndProducts1"
            runat="server" />
    </div>
    <eStore:CategoryWithSubCategoryAndProductsV2 ID="CategoryWithSubCategoryAndProductsV21"
        runat="server" Visible="false" />
    <div class="clear">
    </div>
    <asp:DataList ID="dlRootCategorylist" ClientIDMode="Static" runat="server" RepeatColumns="3"
        ShowFooter="true" ItemStyle-VerticalAlign="Top" EnableViewState="false" OnItemDataBound="dlRootCategorylist_ItemDataBound">
        <ItemTemplate>
            <asp:PlaceHolder ID="phCategory" runat="server"></asp:PlaceHolder>
        </ItemTemplate>
        <ItemStyle CssClass="SubCategory" />
        <SeparatorTemplate>
        </SeparatorTemplate>
        <SeparatorStyle CssClass="dotlinemidial" />
        <FooterTemplate>
            <script type="text/javascript" language="javascript">
                $(document).ready(function () {
                    $.each($("#dlRootCategorylist tr"), function () {
                        $(this).find("p").equalHeight();
                    });

                    $.each($(".CategoryMinPrice span"), function () {
                        var pricepanel = $(this);
                        $.ajaxSetup({ cache: false });
                        $.getJSON("/proc/do.aspx",
                             { func: "<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.LowestPrice %>"
                             , id: $(pricepanel).attr("id")
                             },
                             function (data) {
                                 $(pricepanel).html(data.LowestPrice);
                             });
                    });
                });
 
            </script>
        </FooterTemplate>
    </asp:DataList>
</asp:Content>
