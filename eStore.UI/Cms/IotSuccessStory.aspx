<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/TwoColumnIoT.Master" AutoEventWireup="true" CodeBehind="IotSuccessStory.aspx.cs" Inherits="eStore.UI.Cms.IotSuccessStory" %>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreMainContent" runat="server">
<script type="text/javascript">
    $(function () {
        showgrefresh();
    });
    function showgrefresh() {
        /*瀑布流效果*/
        var vg = $(".iot-storiesContentBlock").vgrid({
            easing: "easeOutCirc",
            useLoadImageEvent: true,
            time: 650,
            delay: 0,
            fadeIn: {
                time: 800,
                delay: 50
            }
        });
        $(window).load(function (e) {
            vg.vgrefresh();
        });
    }
</script>
<div class="iot-content">
			<div class="iot-block">
            	<div class="iot_bannerTop msZhengHeiFont">
                    <img src="<%=esUtilities.CommonHelper.GetStoreLocation()%>images/iot_bannerTop-stories.jpg" />
                    <div class="iot_bannerTop-txt iot_bannerTop-Stories"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_SuccessStories)%></div>
                </div>
                <div class="iot-breadcrumb">
                <a href="/">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)%></a> > 
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_SuccessStories)%></div>
            </div><!--iot-block-->
            <div class="iot-block iot-font15">
            	<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Sort_by)%>:
                <div class="iot-storiesSelectBlock">
                    <div class="iot-storiesSelect">
                        <asp:DropDownList ID="ddlCmsAppType" runat="server" CssClass="styled">
                        </asp:DropDownList>
                    </div>
                    <div class="iot-storiesSelect">
                        <asp:DropDownList ID="ddlIotCategories" runat="server" CssClass="styled">
                        </asp:DropDownList>
                    </div>
                </div><!--iot-storiesSelectBlock-->  
                <div class="iot-storiesContentBlock">
                <!--瀑布流 start-->
                    <asp:Repeater ID="rpCmsList" runat="server">
                    <ItemTemplate>
                        <div class="iot-storiesContent">
                    	    <div class="iot-storiesCategoryBG"><span class="cmsType"><%#Eval("cmsAppsStr")%></span>│<span class="cateType"><%# megerCate(Container.DataItem)%></span></div>
                            <div class="iot-storiesTxt">
                                <div class="iot-storiesTxt-Top">
                                    <%# showImag(Eval("ImageUrl"))%>
                                    <a href='CmsDetail.aspx?cmsid=<%#Eval("RECORD_ID") %>&app=<%#Eval("cmsAppsStr")%>&cate=<%# megerCate(Container.DataItem)%>'><%# Eval("Title")%></a>
                                </div>
                                <div class="iot-storiesTxt-Bottom"><%# Eval("Context") %> 
                                <a href='CmsDetail.aspx?cmsid=<%#Eval("RECORD_ID") %>&app=<%#Eval("cmsAppsStr")%>&cate=<%# megerCate(Container.DataItem)%>'>More...</a>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    </asp:Repeater>
                    
                <!--瀑布流 end-->
                </div><!--iot-storiesContentBlock-->  
            </div><!--iot-block-->  
        </div>
        <script type="text/javascript">
            $(document).ready(function () {
                $(".styled").change(function () {
                    mergarIfor($("#<%=ddlCmsAppType.ClientID%>").val(), $("#<%=ddlIotCategories.ClientID%>").val());
                    showgrefresh();
                });
            });
            function mergarIfor(app,cate)
            {
                $(".iot-storiesContentBlock .iot-storiesContent").each(function () {
                    var isShow = false;
                    var tt = $(this).find(".cmsType").text().split(",");
                    for (var t = 0; t < tt.length; t++) {
                        if (app == "all" || tt[t] == app) {
                            var nn = $(this).find(".cateType").text().split(",");
                            for (var n = 0; n < nn.length; n++) {
                                if (cate == "all" || nn[n] == cate) {
                                    isShow = true;
                                }
                            }
                        }
                    }
                    if (isShow) {
                        $(this).show();
                    }
                    else {
                        $(this).hide();
                    }
                });
            }
        </script>
</asp:Content>

