<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="eStore.UI.Modules.IoTMart.Footer" %>
<div class="iot-footer">
    <asp:Repeater runat="server" ID="rpMainFooter">
        <ItemTemplate>
            <div class="iot-footerBlock">
                <div class="iot-title">
                <a href='<%# Eval("URL") ==null || Eval("URL")== "" ?"#":esUtilities.CommonHelper.ConvertToAppVirtualPath(Eval("URL").ToString()) %>'
                            target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>" 
                            onclick="<%# Eval("URL") ==null || Eval("URL")== "" ? "return false" : ""%>"  
                            style = "<%# Eval("URL") ==null || Eval("URL")== "" ? "cursor:text;" : ""%>">
                    <%# Eval("MenuName") %></a></div>
                <asp:Repeater runat="server" ID="rpSubFooter" DataSource='<%#Eval("subMenusX") %>'>
                    <ItemTemplate>
                        <a href='<%# Eval("URL")==null?"#":esUtilities.CommonHelper.ConvertToAppVirtualPath(Eval("URL").ToString()) %>'
                            target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>"
                             onclick="<%# Eval("URL") ==null || Eval("URL")== "" ? "return false" : ""%>">
                            <%# Eval("MenuName") %></a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
