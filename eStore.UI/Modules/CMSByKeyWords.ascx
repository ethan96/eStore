<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CMSByKeyWords.ascx.cs"
    Inherits="eStore.UI.Modules.CMSByKeyWords" %>
<% if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("DisplayProductRelatedInfo"))
   { %>
<div id="cmsEventList" class="cmsLiCss">
</div>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $.getJSON("<%=esUtilities.CommonHelper.GetStoreLocation()%>proc/do.aspx?func=14",
            { keyword: "<%=eStore.Presentation.eStoreContext.Current.keyWordsStringWithBar%>" },
            function (data) {
                if (data.length != 0) {
                    $("div#cmsEventList").append("<div id='CMSContectList' class='CMSContectHeader'><div class='titlebar ui-corner-all'><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Product_Related_Info)%></div><ul></ul></div>");
                    $(data).each(function (i, item) {
                        if (item.value.trim() == "")
                            $("div#cmsEventList ul").append("<li>" + item.key + "</li>");
                        else
                            $("div#cmsEventList ul").append("<li><a href='" + item.value + "' target='_blank'>" + item.key + "</a></li>");
                    });
                }
            });
    });
</script>
<%} %>