<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryHeader.ascx.cs"
    Inherits="eStore.UI.Modules.CategoryHeader" %>
<%@ Register Src="liveperson.ascx" TagName="liveperson" TagPrefix="eStore" %>
<div id="MainProdArea">
    <div id="AreaTitle">
        <p>
            <asp:Literal ID="lCategoryName" runat="server" EnableViewState="false"></asp:Literal>
        </p>
    </div>
    <div id="info">
        <div id="photo">
            <asp:Image ID="imgCategory" runat="server" Width="200" Height="200" BorderWidth="0" />
        </div>
        <div id="descArea">
            <div id="feature">
                <u>Main Features</u></div>
            <div id="list">
                <asp:Literal ID="lCategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
            </div>
        </div>
    </div>
</div>
<div style="width: 208px; float:right;">
    <eStore:liveperson ID="liveperson1" runat="server" UserLargerImage="false" />
</div>
