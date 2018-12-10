<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeEnhancedService.ascx.cs" Inherits="eStore.UI.Modules.HomeEnhancedService" %>
<asp:Repeater ID="rpThemeBanners" runat="server" Visible="true">
    <HeaderTemplate>
        <div class="eStore_index_Highlight noBG">
                <h1 class="eStore_index_solutionTitle">Enhanced Services & Partnerships</h1>
                <ol class="eStore_index_otherType">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="block"><a href="<%# (Eval("HyperLink") != null && !string.IsNullOrEmpty(Eval("HyperLink").ToString())) ? Eval("HyperLink") : ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl((Container.DataItem as eStore.POCOS.KitTheme).KitThemeTypes.OrderBy(c=>c.Seq).FirstOrDefault())) %>" target="<%#Eval("Target") %>">
            <div class="img"><img alt="<%#Eval("AlterText") %>" src="<%#Eval("ImageFileX") %>" border="0" /></div> 
            <%#Eval("Title") %></a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ol>
        </div>
    </FooterTemplate>
</asp:Repeater>