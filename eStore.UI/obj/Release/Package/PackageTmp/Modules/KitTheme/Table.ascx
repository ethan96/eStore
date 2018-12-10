<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Table.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.Table" %>


<div style="text-align: center; border-bottom: 1px solid #eaeaea;">
    <div class="cs-filter-wrap" style="margin-left: 0!important;">
        <div class="cs-filter-drop cs-application">Select Suitable Area for You</div>

        <div class="application-tag">
        </div>
    </div>
    <div class="clearfix"></div>
</div>
<div class="row-fluid">
    <div class="filter-menu application-menu hide">
        <span class="filter-close">
            <img src="/images/orderlistTable_close.png" width="15" height="15"></span>
        <div class="all-selection" style="margin-top: 15px;"><a class="selected selectall">All Applications</a></div>
        <div class="filter-desc">You can choose more than one business applications</div>
        <div class="clear"></div>
        <div class="menu-group">
            <ul class="menu-item">
                <asp:Repeater ID="rpfilter" runat="server">
                    <ItemTemplate>
                        <li><a data-type="app" data-id="<%#Eval("id") %>"><%#Eval("name") %></a></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div class="clear"></div>
    </div>
</div>
<ul class="AEU-container SuccessStories col-parent cols-4 s-cols-2 xs-cols-full col-height-equal clearboth readmoreTable">
    <asp:Repeater ID="rpcmsls" runat="server">
        <ItemTemplate>
            <li class="col equal-block hiddenitem" data-gp="<%# JoinTags(Eval("CmsID")) %>">
                <div class="col-border">
                    <div class="imgRadius">
                        <div class="fullImgBG height130" style="background-image: url('<%# (Eval("ImageUrl") != null && !string.IsNullOrEmpty(Eval("ImageUrl").ToString())) ? Eval("ImageUrl") : "/images/ss-default.jpg"%>');"></div>
                        </div>
                    <div class="bottomContent">
                        <div class="date"><%# Eval("CreatedDateShort") %></div>
                        <a href="/CMS/CmsDetail.aspx?CMSID=<%# Eval("CmsID") %>" target="_blank" class="titleLink"><%#Eval("Title") %></a>
                        <div class="filterLabel-Block">
                            <%# ShowTags(Eval("CmsID")) %>
                        </div>
                    </div>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            <div class="clearfix"></div>
            <li><span class="readymoreTable-btn">Read more</span></li>
        </FooterTemplate>
    </asp:Repeater>
</ul>
