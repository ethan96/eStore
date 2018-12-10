<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListWithFilter.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.ListWithFilter" %>
<%@ Register src="List.ascx" tagname="List" tagprefix="uc1" %>

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

<uc1:List ID="List1" runat="server" />
