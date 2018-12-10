<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuildSystem.ascx.cs"
    EnableViewState="false" Inherits="eStore.UI.Modules.BuildSystem" %>
<eStore:Repeater ID="rpBuildSystem" runat="server">
    <HeaderTemplate>
        <div class="BuildSystem">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="BuildSystemItem">
            <h4>
                <a href="<%# Eval("Titlelink")%>">
                    <%# Eval("Title")%></a>
            </h4>
            <img src='<%# Eval("Picturepath") %>' alt='<%# Eval("Title") %>' />
            <eStore:Repeater ID="rpBuildSystemItems" runat="server" DataSource='<%# Eval("BuildSystemDetails")%>'>
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li><a href='<%# Eval("SubtitleLink") %>'>
                        <%# Eval("Subtitle")%></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </eStore:Repeater>
            <a href='<%# Eval("Titlelink")%>' class="ItemLink"></a>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</eStore:Repeater>
