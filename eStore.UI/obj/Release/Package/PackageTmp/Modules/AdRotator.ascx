<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdRotator.ascx.cs" Inherits="eStore.UI.Modules.AdRotator"
    ViewStateMode="Disabled" %>
<%@ OutputCache Duration="120" VaryByParam="None" %>
<eStore:Repeater ID="rpAdRotator" runat="server">
    <HeaderTemplate>
        <script type="text/javascript">
            $().ready(function () {
                $().ready(function () {
                    $('#coda-slider-2').codaSlider({
                        autoSlide: true,
                        autoSlideInterval: 10000,
                        autoSlideStopWhenClicked: false, dynamicArrows: false, dynamicTabs: false
                    , slideEaseFunction: "easeInOutExpo"

                    });
                });
            });
        </script>
        <div class="coda-slider-wrapper coda-slider-no-js">
            <div class="coda-slider preload" id="coda-slider-2">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="panel">
            <div class="panel-wrapper">
                <a href="<%# ResolveUrl(Eval("Hyperlink").ToString())%>" target="<%# Eval("Target") %>">
                    <img src="<%#"/resource"+ Eval("Imagefile")%>" alt="<%# Eval("Title")%>" /></a>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div> </div>
    </FooterTemplate>
</eStore:Repeater>
