<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftSideMenus.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.LeftSideMenus" %>
<ul class="epaps-listMenu">
    <li class="epaps-listFirst"><a href="<%=ResolveUrl("~/CertifiedPeripherals/Default.aspx") %>">
        Certified Peripherals</a></li>
    <!--
    <li><a href="">Procurement Service</a></li>
    -->
    <li>
        <a href="">Products</a>
    </li>    
    <eStore:Repeater ID="rpPeripherals" runat="server">
        <ItemTemplate>
            <li class="epaps-child"><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                <%# Eval("DisplayName") %></a></li>
        </ItemTemplate>
    </eStore:Repeater>
    
 <%--   <li><a href="/CertifiedPeripherals/solutions/RAID.aspx">Peripherals Solutions</a></li>--%>
</ul>
