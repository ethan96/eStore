<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu2013.ascx.cs" Inherits="eStore.UI.Modules.Menu2013" %>
<div id="CategoryArea">
    <asp:Repeater ID="rpCategoryGroup" runat="server">
        <HeaderTemplate>
            <div id="CategoryGroup">
                <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li<%# Container.ItemIndex==0?" class=\"select\"":"" %>>
                 <%# Eval("LocalCategoryName")%>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul> </div>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater ID="rpCategoryItem" runat="server">
        <HeaderTemplate>
            <div id="CategoryItem">
        </HeaderTemplate>
        <ItemTemplate>
            <ul<%# Container.ItemIndex==0?"":" style=\"display: none;\"" %>>
            <asp:Repeater ID="rpProductCategory" runat="server" DataSource='<%# Eval("childCategoriesX") %>'>
          
                <ItemTemplate>
                    <li id="category<%# Eval("CategoryID")%>"><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                        <%# Eval("LocalCategoryName")%></a>
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
                                    ,System.Web.HttpUtility.HtmlEncode( Eval("LocalCategoryName")) )%>
                                    <b><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                        <%# Eval("LocalCategoryName")%></a></b>
                                    <br />
                                    <%# (Eval("descriptionX")==null || string.IsNullOrEmpty( Eval("descriptionX").ToString()))?""
                                                                                : (Eval("descriptionX").ToString().Length > 110
                                                                                ? (Eval("descriptionX").ToString().Substring(0, 110) + "...")
                                        : Eval("descriptionX"))%>
                                </div>
                                <div class="arrow">
                                    <a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>">
                                        <img src="<%= ResolveUrl("~/images/arrow_1.png") %>" border="0" alt="" /></a></div>
                                <div class="clear">
                                </div>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <hr style="margin: 10px 0px;" />
                            </SeparatorTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </li>
                </ItemTemplate>
   
            </asp:Repeater>
            </ul>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</div>
<script language="javascript" type="text/javascript">
    $("#CategoryGroup ul li").click(function () {
        $(this).addClass("select");
        $(this).siblings().removeClass("select");
        var activetabindex = $("#CategoryGroup ul li").index(this);
        $("#CategoryItem>ul").each(function (i, n) {
            if (i == activetabindex)
            { $(n).show(); }
            else
            { $(n).hide(); }
            if ($("#floatbtospanel").length > 0) {
                $('#floatbtos').scrollFollow({ container: 'floatbtospanel' });
            }
        });
    });


    $(document).ready(
    function () {
        $("#CategoryItem>ul li")
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
