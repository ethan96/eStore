<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumn.Master" AutoEventWireup="true"
    CodeBehind="ProductCategoryV2.aspx.cs" Inherits="eStore.UI.Product.ProductCategoryV2" %>

<%@ Register Src="../Modules/Category/CategoryRepeater.ascx" TagName="CategoryRepeater"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreRightContent" runat="server">
    <eStore:ProductCategory ID="ProductCategory1" runat="server" />
    <div id="storeSideAds">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div class="medical-container">
        <div class="medical-content-left">
            <!--page-banner-->
            <div class="medical-content">
              
                <eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
                    showDescription="false" MinHeight="120" />
                <h1>
                    <asp:Literal ID="ltCategoryName" runat="server"></asp:Literal></h1>
                <asp:Literal ID="ltCategoryDescription" runat="server"></asp:Literal>
            </div>
            <uc1:CategoryRepeater ID="CategoryRepeater1" runat="server" />
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $.each($("span.CategoryMinPrice"), function () {
                var pricepanel = $(this);
                $.getJSON(GetStoreLocation() + "proc/do.aspx",
             { func: "<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.LowestPrice %>"
             , id: $(pricepanel).attr("id")
             },
             function (data) {
                 $(pricepanel).html(data.LowestPrice);
             });
            });
        });
    </script>
</asp:Content>
