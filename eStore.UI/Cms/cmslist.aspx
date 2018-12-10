<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="cmslist.aspx.cs" Inherits="eStore.UI.Cms.cmslist" %>


<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
<link href="../Styles/CMS.css" rel="stylesheet" type="text/css" />

<div class="cms-context">
<ul>
    <dl>
    	<dt class="cms-title center rightline"><%=eStore.Presentation.eStoreLocalization.Tanslation("estore_Industry_news")%></dt>
        <dt class="center right"><%=eStore.Presentation.eStoreLocalization.Tanslation("estore_source")%></dt>
        <dt class="center rightline right"><%=eStore.Presentation.eStoreLocalization.Tanslation("estore_News_date")%></dt>
        <dd class="clear"></dd>
    </dl>
    <asp:Repeater ID="rpcmsls" runat="server">
        <ItemTemplate>
            <li>
    	        <dt class="cms-title">
        	        <h3><a href='/CMS/CmsDetail.aspx?CMSID=<%#Eval("CmsID")%>' target="_blank"><%#Eval("Title")%></a></h3>
                    <p><%#Eval("Abstract")%></p>
                </dt>
                <dt class="center right top4"><%#Eval("Source")%></dt>
                <dt class="center right top4"><%#Eval("CreatedDateX")%></dt>
                <dd class="clear"></dd>
            </li>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <li class="tdbg">
    	        <dt class="cms-title">
        	        <h3><a href='/CMS/CmsDetail.aspx?CMSID=<%#Eval("CmsID")%>' target="_blank"><%#Eval("Title")%></a></h3>
                    <p><%#Eval("Abstract")%></p>
                </dt>
                <dt class="center right top4"><%#Eval("Source")%></dt>
                <dt class="center right top4"><%#Eval("CreatedDateX")%></dt>
                <dd class="clear"></dd>
            </li>
        </AlternatingItemTemplate>
    </asp:Repeater>

</ul>
</div>
</asp:Content>
