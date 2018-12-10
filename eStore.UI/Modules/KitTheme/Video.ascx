<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Video.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.Video" %>

<ul class="AEU-container Videos col-parent cols-4 s-cols-2 xs-cols-full col-height-equal clearboth">
    <asp:Repeater ID="rpCmsls" runat="server">
        <ItemTemplate>
            <li class="col equal-block">
                <div class="col-border">
                    <a href="<%# Eval("URL") %>" target="_blank">
						<div class="imgRadius videoPic">
                            <div class="videoIcon">
                                <img src="/images/AEU_videoIcon.png" class="videoPlay" /></div>
                            <img src="<%# Eval("ImageUrl") %>" class="img" />
                        </div>
                        <div class="bottomContent">
                            <div class="titleLink"><%# Eval("Title") %></div>
                        </div>
                    </a>
                </div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
