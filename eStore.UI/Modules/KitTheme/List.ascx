<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.List" %>


<!-- filter -->
<ul class="AEU-container IndustryFocus listStyle-block readmore">
    
    <asp:Repeater ID="cmslist" runat="server">
        <ItemTemplate>
            <li class="col hiddenitem" <%# IsFelter ? string.Format("data-gp='{0}'", JoinTags(Eval("CmsID"))) : "" %>>
            <a href='/CMS/CmsDetail.aspx?CMSID=<%# Eval("CmsID") %>' target="_blank" class="titleLink"><%# Eval("Title") %></a>
                <%# ShowImage(Eval("ImageUrl")) %>
            <div class="rightContent">
                <span class="date"><%# Eval("CreatedDateShort") %></span>
                <div class="content"><%# esUtilities.StringUtility.ReplaceHtmlTag(Eval("Abstract").ToString(), 200, "...") %><a href="/CMS/CmsDetail.aspx?CMSID=<%# Eval("CmsID") %>" target="_blank">>>More</a></div>
                <div class="filterLabel-Block">
                    <%# ShowTags(Eval("CmsID")) %>                     
                </div>
            </div>
            <div class="clearfix"></div>
                    </li>
        </ItemTemplate>
        <FooterTemplate>
            <li><span class="readymore-btn">Read more</span></li>
        </FooterTemplate>
    </asp:Repeater>

</ul>
