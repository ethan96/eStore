<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eStore.UI.Sitemap.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="eStoreMainContent" runat="server">
<style>
    .ParentCategoryUl{ list-style:none;}
    h2{border-bottom:1px solid #999;}
</style>
    <div id="sitemap" style=" background-color:#fff; width:100%; height:auto; margin-top:10px; overflow:hidden;">
        <div style="float:left;width:310px; height:auto; border:0px solid red; margin-right:10px;overflow:hidden;">
            <h2>
                <asp:HyperLink ID="hlStandardCategory" runat="server" Target="_blank">Products</asp:HyperLink>
            </h2>
            <ul class="ParentCategoryUl">
                <asp:Repeater ID="rptProductCategory" runat="server" OnItemDataBound="rptProductCategory_ItemDataBound">
                    <ItemTemplate>
                        <li>
                            <h5>
                                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>" target="_blank">
                                    <%# Eval("LocalCategoryName")%>
                                </a>
                            </h5>
                            <asp:Literal ID="ltAnyCategory" runat="server"></asp:Literal>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                <li runat="server" id="liCertifiedPeripheral">
                    <h5>
                        <asp:HyperLink ID="hlCertifiedPeripheral" runat="server" Target="_blank" NavigateUrl=""></asp:HyperLink>
                    </h5>
                    <ul class="eStoreList">
                        <asp:Repeater ID="rptCertifiedPeripheral" runat="server">
                            <ItemTemplate>
                                <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                                    target="_blank">
                                    <%# Eval("DisplayName")%>
                                </a></li>
                            </ItemTemplate>
                        </asp:Repeater>
                </li>
            <%--pstore category--%>

            </ul>
        </div>

        <div style="float:left;width:310px; height:auto; border:0px solid red;margin-right:10px;overflow:hidden;">
            <%--System category--%>
            <h2>
                <asp:HyperLink ID="hlSystemsCategory" runat="server" Target="_blank">Systems</asp:HyperLink>
            </h2>
            <ul class="ParentCategoryUl">
                <asp:Repeater ID="rptSystemCategory" runat="server" OnItemDataBound="rptSystemCategory_ItemDataBound">
                    <ItemTemplate>
                        <li>
                            <h5>
                                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>" target="_blank">
                                    <%# Eval("LocalCategoryName")%>
                                </a>
                            </h5>
                            <asp:Literal ID="ltAnyCategory" runat="server"></asp:Literal>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <%--Solutions 和 Services--%>
            <asp:Repeater ID="rptSolutionServiceMenu" runat="server" OnItemDataBound="rptSolutionServiceMenu_ItemDataBound">
                <ItemTemplate>
                    <h2>
                        <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>" target="_blank">
                            <%# HttpUtility.HtmlEncode(Eval("MenuName"))%>
                        </a>
                    </h2>
                    <ul class="eStoreList">
                        <asp:Repeater ID="rptSubmenu" runat="server">
                            <ItemTemplate>
                                <li>
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>" target="_blank">
                                        <%# HttpUtility.HtmlEncode(Eval("MenuName"))%>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div style="float:left;width:310px; height:auto; border:0px solid red;margin-right:10px;overflow:hidden;">
            <%--FooterMenu--%>
            <asp:Repeater ID="rptFooterMenu" runat="server">
                <ItemTemplate>
                    <h2><%# HttpUtility.HtmlEncode(Eval("MenuName"))%></h2>
                    <ul class="eStoreList">
                        <asp:Repeater ID="rptSubmenu" runat="server" DataSource="<%#((eStore.POCOS.Menu) Container.DataItem).subMenusX %>">
                            <ItemTemplate>
                                <li>
                                    <a href='<%# Eval("URL") %>' target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>">
                                        <%# Eval("MenuName")%>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
