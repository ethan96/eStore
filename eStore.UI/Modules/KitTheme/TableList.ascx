<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TableList.ascx.cs" Inherits="eStore.UI.Modules.KitTheme.TableList" %>

<div class="titleBlock">
    <asp:Literal ID="ltTitle" runat="server"></asp:Literal></div>
<ul class="AEU-container OnlineResources-OnlineCatalogs col-parent cols-2 s-cols-full xs-cols-full clearboth-t">
    <asp:Repeater ID="rpCmsls" runat="server" OnItemDataBound="rpCmsls_ItemDataBound">
        <ItemTemplate>
            <li class="col clearboth-t">
                <div class="leftImg brochureBlock">
                    <div class="brochureIcon">
                        <a href="<%# Eval("URL") %>" target="_blank"><img src="/images/AEU_brochureIcon.png" class="brochureDownload" /></a></div>
                    <img src="<%# Eval("ImageUrl") %>" class="img brochureImg" />
                </div>
                <div class="rightContent">
                    <a href="/CMS/CmsDetail.aspx?CMSID=<%# Eval("CmsID") %>" target="_blank" class="titleLink"><%#Eval("Title") %></a>
                    <span class="date"><%# Eval("CreatedDateShort") %></span>
                    <div class="content">
                        <asp:Literal ID="ltContext" runat="server"></asp:Literal>
                    </div>
                </div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
