<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Advertisement.ascx.cs"
    Inherits="eStore.UI.Modules.Advertisement" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=<%=(int)eStore.Presentation.eStoreContext.Current.SearchConfiguration.AdvertisementAJAXFunctionType%><%=eStore.Presentation.eStoreContext.Current.keywordString%>",
           function (data) {
               $.eStoreAD(data);
               <%= OnHtmlLoaded %>
           });
       });
</script>
