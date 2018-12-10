<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="Theme.aspx.cs" Inherits="eStore.UI.Kit.Theme" EnableViewState="false" %>

<%@ Register Src="~/Modules/KitTheme/List.ascx" TagPrefix="eStore" TagName="List" %>
<%@ Register Src="~/Modules/KitTheme/Table.ascx" TagPrefix="eStore" TagName="Table" %>
<%@ Register Src="~/Modules/KitTheme/SimpList.ascx" TagPrefix="eStore" TagName="SimpList" %>
<%@ Register Src="~/Modules/KitTheme/TableList.ascx" TagPrefix="eStore" TagName="TableList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/KitTheme")%>
    <%= System.Web.Optimization.Scripts.Render("~/Scripts/V4/KitThemeJs")%>
</asp:Content>
<asp:Content ID="banner" ContentPlaceHolderID="eStoreHeaderFullSizeContent" runat="server">
    <eStore.V4:eStoreCycle2Slider ID="eStoreCycle2Slider1" runat="server" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div id="site-AEU">
        <div class="site-container">
            <ul class="AEU-menu col-parent cols-6 s-cols-3 xs-cols-2 clearboth">
                <asp:Repeater ID="rpTypes" runat="server">
                    <ItemTemplate>
                        <li class="col"><a href="<%# ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem)) %>" <%# Eval("Id").ToString() == themType.Id.ToString() ? "class='on'" : "" %>><%# Eval("Title") %></a></li>
                        <li></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <asp:PlaceHolder ID="phTheme" runat="server"></asp:PlaceHolder>
        </div>
        <!-- siteContainer -->
    </div>
</asp:Content>
