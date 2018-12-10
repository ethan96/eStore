<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductLiterature.ascx.cs"
    Inherits="eStore.UI.Modules.ProductLiterature" %>
<script type="text/javascript">


    $(function () {
        var frameHeight=0;
        if ($(".frameUl").length>0) {
            $("#Recource").tabs({
                select: function (event, ui) {
                    $("#images img").addClass("ui-tabs-hide");
                    $("#images img:eq(" + ui.index + ")").removeClass("ui-tabs-hide");
                }
            });
            frameHeight=$(".frameUl").height();
        }
        if ($(".productLargeImages").length>0) {
            frameHeight=$(".productLargeImages").height();
        }
        var imageh = $(".divLargePicture").height();
        var featureh=$(".ProductFeature").height();
        if(imageh+frameHeight>featureh)
        {
            $(".ProductFeature").animate({ 
                height: imageh+frameHeight 
                }, 0 );
        }
        else
        {
            $(".divLargePicture").animate({ 
                height: featureh-frameHeight
                }, 0 );
        }
    });
 
     function showProductSpecsDialog(productid) {
        $("<div></div>")
        .load("/proc/html.aspx", { type: "ProductSpecList", ProductID: productid })
        .dialog({
             height: 600, width: 400, title: 'Extended Specifications',
             modal: true
        });
        return false;
    }
     function showProduct3DModelDialog(productid) {
        $("<div></div>")
        .load("/proc/html.aspx", { type: "Product3DModel", ProductID: productid })
        .dialog({
            height: 285, width: 495, title: 'DownLoad 3D Model',
            modal: true
        });
        return false;
    }

</script>
<div id="ProductLiterature" class="ui-tabs">
    <div id="Recource">
        <asp:Literal ID="lLiterature" runat="server"></asp:Literal>
    </div>
</div>
