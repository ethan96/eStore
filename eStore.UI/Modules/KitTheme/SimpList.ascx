<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpList.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.SimpList" %>

<div class="titleBlock">
    <asp:Literal ID="ltTitle" runat="server"></asp:Literal></div>
<ul class="AEU-container OnlineResources-WhitePapers col-parent cols-2 s-cols-full xs-cols-full col-height-equal clearboth-t">
    <asp:Repeater ID="rpCmsls" runat="server">
        <ItemTemplate>
            <li class="col equal-block clearboth-t">				
				<span class="date"><%# Eval("CreatedDateShort") %></span>
                <% if (this.TitleX == TitleExt.White_Papers)
                    { %>
                <a href='https://member.advantech.com/yourcontactinformation.aspx?formid=7379e3b5-11fb-4d47-b962-0de7b8df2a32&CMSID=<%# Eval("CmsID") %>&CallBackurl=<%# Eval("URL") %>' target="_blank" class="titleLink"><%#Eval("Title") %></a>
                <%}
                else
                { %>
                <a href='/CMS/CmsDetail.aspx?CMSID=<%# Eval("CmsID") %>' target="_blank" class="titleLink"><%#Eval("Title") %></a>
                <%} %>
			</li>
        </ItemTemplate>
    </asp:Repeater>
</ul>