<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HomeMedialContent.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.HomeMedialContent" %>
<%@ Register Src="liveperson.ascx" TagName="liveperson" TagPrefix="eStore" %>
<div class="homecatalogs">
    <ul class="homeitems">
        <li class="catalogs ui-helper-reset ui-corner-all"> 
            <asp:ImageButton ID="imgProduct" runat="server" /> 
            <hr />
            <asp:Repeater ID="rpProductCategory" runat="server">
                <HeaderTemplate>
                    <ul class="eStoreList">
                </HeaderTemplate>
                <ItemTemplate>
                    <li id="category<%# Eval("CategoryID")%>"><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a>
                        <asp:Repeater ID="rpsubcategories" DataSource='<%#  Eval("childCategoriesX") %>'
                            runat="server">
                            <HeaderTemplate>
                                <ul class="hidden">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="homepopup">
                                    <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
                                    ?string.Empty
                                    :string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"15px\" /></a> "
                                    , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                    , eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL")
                                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem))%>
                                    <b><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                                <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a></b>
                                    <br />
                                    <%# getShotDescription(Container.DataItem)%>
                                </div>
                                <div class="clear">
                                </div>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <hr  style="margin:10px 0px;"/>
                            </SeparatorTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </asp:Repeater>
            <asp:HyperLink ID="hlMoreProducts" runat="server" CssClass="moreitems" NavigateUrl="~/Product/AllProduct.aspx?type=standard">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%>
            </asp:HyperLink>
        </li>
        <li class="catalogs ui-helper-reset ui-corner-all"> 
            <asp:ImageButton ID="imgSystem" runat="server" /> 
            <hr />
            <asp:Repeater ID="rpSystems" runat="server">
                <HeaderTemplate>
                    <ul class="eStoreList">
                </HeaderTemplate>
                <ItemTemplate>
                    <li id="category<%# Eval("CategoryID")%>"><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a>
                        <asp:Repeater ID="rpsubcategories" DataSource='<%#  Eval("childCategoriesX") %>'
                            runat="server">
                            <HeaderTemplate>
                                <ul class="hidden">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="homepopup">
                                    <%# (Eval("ImageURL") == null|| string.IsNullOrEmpty(Eval("ImageURL").ToString()))
                                    ?string.Empty
                                    :string.Format("<a href=\"{0}\"><img src=\"{1}\"  alt=\"{2}\" width=\"15px\" /></a> "
                                    , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))
                                    , eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + Eval("ImageURL")
                                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem))%><b>
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                                <%# eStore.Presentation.eStoreGlobalResource.getLocalCategoryName((eStore.POCOS.ProductCategory)Container.DataItem)%></a></b>
                                    <br />
                                    <%# getShotDescription(Container.DataItem)%>
                                </div>
                                <div class="clear">
                                </div>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <hr  style=" margin:10px 0px;"/>
                            </SeparatorTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </asp:Repeater>
            <asp:HyperLink ID="hlMoreSystems" runat="server" CssClass="moreitems" NavigateUrl="~/Product/AllProduct.aspx?type=system">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%>
            </asp:HyperLink>
        </li>
        <li class="catalogs ui-helper-reset ui-corner-all">
            <asp:Image ID="imgSolution" runat="server" />
            <hr />
            <asp:Repeater ID="rpSolutions" runat="server">
                <HeaderTemplate>
                    <ul class="eStoreList">
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
                        target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>">
                        <%# Regex.Replace(Eval("MenuName").ToString(), "</?[a-zA-Z][a-zA-Z0-9]*[^<>]*>", " ")%></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </asp:Repeater>
            <asp:HyperLink ID="hlMoreSolutions" runat="server" CssClass="moreitems" NavigateUrl="~/Product/allsolution.aspx">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)%>
            </asp:HyperLink>
        </li>
        <li class="helpers">
            <eStore:liveperson ID="liveperson1" runat="server" UserLargerImage="true" />
            <asp:HyperLink ID="hlAdamForum" runat="server"></asp:HyperLink>
            <eStore:Repeater ID="rptTodaysDeals" runat="server">
                <HeaderTemplate>
                    <ul class="todaysDeals">
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="<%# ResolveUrl(Eval("Hyperlink").ToString())%>" title='<%# Eval("Title")%>'
                        target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>">
                        <asp:Image ID="Image1" runat="server" ImageUrl='<%# Eval("imageFileHost") %>' />
                    </a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </eStore:Repeater>
            <eStore:Repeater ID="rpEducation" runat="server">
                <HeaderTemplate>
                    <div class="titlebar  ui-helper-reset ui-corner-all">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Education_Columns)%>
                    </div>
                    <ul class="eStoreList">
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href="<%# esUtilities.CommonHelper.ConvertToAppVirtualPath(Eval("Hyperlink").ToString())%>" target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>">
                        <%# Eval("Title")%></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </eStore:Repeater>
        </li>
    </ul>
</div>
<div class="clear">
</div>
<div class="homecatalogs storeBottomAdsHeader hiddenitem">
    <div class="titlebar  ui-helper-reset ui-corner-all">
        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Today_Highlights)%>
    </div>
</div>
<div class="clear">
</div>
<div class="list_carousel">
    <div id="storeBottomAds">
    </div>
</div>
<div class="clear">
</div>
    <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("PopupSubCategoriesAtHomePage"))
       {%>
<script type="text/javascript" language='javascript'>

    $(document).ready(
    function () {
        $(".eStoreList:lt(2) li")
		   .hover(function () {
		       var sender = $(this);
		       clearTimeout(sender.data("timer"));
		       sender.data("timer", setTimeout(function () {
		           sender.data("isactive", true);
		           popsubcategories($(sender).attr("id"));
		       }, 300));
		   }, function () {
		       var sender = $(this);
		       clearTimeout(sender.data("timer"));
		       sender.data("timer", setTimeout(function () {
		           if (sender.data("isactive") == true) {
		               sender.removeClass("homepagepopuptriger");
		               sender.data("isactive", false);
		               $('#JT').remove();
		               
		           }
		       }, 300));
		   });
    }
    );

    function popsubcategories(senderid) {
        {
            var sender = $("#" + senderid);
            var content = $(sender).find("ul").html();
            if (content.trim() == "") {
                clearTimeout(sender.data("timer"));
                sender.data("isactive", false);
                $('#JT').remove();
                return false;
            }
            sender.addClass("homepagepopuptriger");
            JT_showContent(content, senderid, $(sender).find("a").first().text());
            
            $('#JT').hover(function () {
                clearTimeout(sender.data("timer"));
            }, function () {
                sender.removeClass("homepagepopuptriger");
                clearTimeout(sender.data("timer"));
                sender.data("isactive", false);
                $('#JT').remove();
            });
        }
    }
</script>
    <%} %>
