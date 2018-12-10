<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SolutionStore.ascx.cs"
    ViewStateMode="Disabled" Inherits="eStore.UI.Modules.SolutionStore" %>
<eStore:Repeater ID="rpSolutionStoreTab" runat="server">
    <HeaderTemplate>
        <div id="SolutionStoreTab">
            <ul id="sstabs">
    </HeaderTemplate>
    <ItemTemplate>
        <li><a href='<%# "#sstab_" + Eval("TabID") %>'>
            <%# Eval("TabDisplayName")%></a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul></FooterTemplate>
</eStore:Repeater>
<eStore:Repeater ID="rpSolutionStoreItems" runat="server">
    <ItemTemplate>
        <div id="<%# "sstab_" + Eval("TabID") %>" class="SolutionStoreItem">
            <div class="sssummary" style="font-size: 12px;">
                <h4>
                    <%# Eval("TabDisplayName")%>
                </h4>
                <img src='<%# Eval("Picture","http://advantech.vo.llnwd.net/o35{0}")%>' alt='<%# Eval("TabDisplayName")%>' />
                <a href='<%# Eval("GoToStoreLink")%>'></a>
            </div>
            <eStore:Repeater ID="rpSolutionStoreItem" runat="server" DataSource='<%# Eval("SolutionStoreItems")%>'>
                <ItemTemplate>
                    <div class="ssitem">
                        <a href='<%# Eval("ItemLink")%>'>
                            <img src='<%# Eval("ItemPicture","http://advantech.vo.llnwd.net/o35{0}")%>' alt='<%# Eval("ItemDescription")%>' /></a>
                        <div class="sssublink">
                            <a href='<%# Eval("ItemLink")%>'>
                                <%# Eval("ItemTitle")%></a>
                        </div>
                        <h4>
                            <a href='<%# Eval("ItemLink")%>'>
                                <%# Eval("ItemDescription")%></a></h4>
                        <ul>
                            <%# Eval("ItemContent")%></ul>
                        <a class="ItemLink" href='<%# Eval("ItemLink")%>'></a>
                    </div>
                </ItemTemplate>
            </eStore:Repeater>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div>
        <script type="text/ecmascript" language="javascript">
            $(function () {
                $("#SolutionStoreTab").tabs({
                    event: 'mouseover'
                });

            });
        </script>
    </FooterTemplate>
</eStore:Repeater>
