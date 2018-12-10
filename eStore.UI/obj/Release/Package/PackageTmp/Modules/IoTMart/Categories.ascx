<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Categories.ascx.cs"
    Inherits="eStore.UI.Modules.IoTMart.Categories" %>
<div class="iot-navBlock">
    <div class="iot-navBlock-title">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Category)%></div>
    <asp:Repeater ID="rpRootCategory" runat="server">
        <ItemTemplate>
            <div class="iot-navBlock-link">
                <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem,eStore.Presentation.eStoreContext.Current.MiniSite))%>"
                    <%# getMenuCss(Container.DataItem) %>>
                    <%# Eval("localCategoryNameX")%></a>
                <eStore:Repeater ID="rpSubCategory" runat="server" DataSource='<%# Eval("childCategoriesX")%>'>
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem,eStore.Presentation.eStoreContext.Current.MiniSite))%>">
                            <%# Eval("localCategoryNameX")%></a> </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul></FooterTemplate>
                </eStore:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div class="iot-navBlock BG-black">
    <a href='<%=ResolveUrl("~/Cms/IotSuccessStory.aspx") %>' class="iot-navBlock-title">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_SuccessStories)%></a>
</div>
<div class="iot-navBlock BG-black">
    <asp:HyperLink ID="hyAboutIot" CssClass="iot-navBlock-title" runat="server">About IoTMart</asp:HyperLink>
</div>
