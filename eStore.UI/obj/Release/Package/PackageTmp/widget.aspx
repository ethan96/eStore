<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" EnableViewState="false"
    AutoEventWireup="true" CodeBehind="widget.aspx.cs" Inherits="eStore.UI.widget" %>

<%@ Register Src="Modules/Widget.ascx" TagName="Widget" TagPrefix="eStore" %>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <%if (!isFullSize)
        { %>
    <div class="eStore_container eStore_block980">
        <div class="master-wrapper-center">
            <div id="widgetContext">
                <%} %>
                <eStore:Widget ID="Widget1" runat="server" />
                <%if (!isFullSize)
                    { %>
            </div>

        </div>

        <div class="master-wrapper-side">
            <div id="storeSideAds">
            </div>
            <eStore:SuggestingProductsAds ID="SuggestingProductsAds1" runat="server" />
        </div>
        <eStore:Advertisement ID="Advertisement1" runat="server" />
    </div>
    <%} %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="eStoreRightContent" runat="server">
</asp:Content>
