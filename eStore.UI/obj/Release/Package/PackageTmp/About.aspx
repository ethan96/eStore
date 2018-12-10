<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.Master"
    CodeBehind="About.aspx.cs" Inherits="eStore.UI.About" %>

<%@ Register TagPrefix="eStore" Namespace="eStore.Presentation.eStoreControls" Assembly="eStore.Presentation" %>
<%@ Register src="Modules/StoreSearch.ascx" tagname="StoreSearch" tagprefix="uc1" %>
<asp:Content ID="eStoreContent" runat="server" ContentPlaceHolderID="eStoreMainContent">
   <div id="states"></div>
    <script>

        $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=5&id=US", function (data) {
            $.each(data, function (i, item) {
                $("<div></div>").html(item.state).appendTo("#states");
            });
        });
        $(function () {
            $("<div id='homepopAd'></div>")
            .append( $("<img></img>").attr("src",""))

        });

    </script>
   
</asp:Content>
