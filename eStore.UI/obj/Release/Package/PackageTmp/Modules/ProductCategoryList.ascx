<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductCategoryList.ascx.cs"
    Inherits="eStore.UI.Modules.ProductCategoryList" %>
<%@ Register Src="eStoreLiquidSlider.ascx" TagName="eStoreLiquidSlider" TagPrefix="eStore" %>
<%@ Register Src="YouAreHere.ascx" TagName="YouAreHere" TagPrefix="eStore" %>
<eStore:YouAreHere ID="YouAreHere1" runat="server" />
<eStore:eStoreLiquidSlider ID="eStoreLiquidSlider1" runat="server" navigationType="Numbers"
    showDescription="false" MinHeight="120" />
<h1>
    <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal>
</h1>
<div class="CategoryDescription">
    <asp:Literal ID="lCategoryDescription" runat="server"  EnableViewState="false"></asp:Literal>
</div>
<div class="clear">
</div>
<asp:DataList ID="dlCategory" ClientIDMode="Static" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
    ItemStyle-VerticalAlign="Top" OnItemDataBound="dlCategory_ItemDataBound"  EnableViewState="false">
    <ItemTemplate>
        <asp:PlaceHolder ID="phCategory" runat="server"></asp:PlaceHolder>
    </ItemTemplate>
    <ItemStyle CssClass="SubCategory" />
    <SeparatorTemplate>
    </SeparatorTemplate>
    <SeparatorStyle CssClass="dotlinemidial" />
</asp:DataList>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $.each($("#dlCategory tr"), function () {
            $(this).find("h5").equalHeight();
            $(this).find("p").equalHeight();
        });

        $.each($(".CategoryMinPrice span"), function () {
            var pricepanel = $(this);
             $.getJSON(GetStoreLocation()+"proc/do.aspx",
             { func: "<%=(int)eStore.Presentation.AJAX.AJAXFunctionType.LowestPrice %>"
             , id: $(pricepanel).attr("id")
             },
             function (data) {
                 $(pricepanel).html(data.LowestPrice);
             });
        });
    });
 
</script>
