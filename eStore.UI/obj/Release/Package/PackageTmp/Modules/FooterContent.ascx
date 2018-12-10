<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FooterContent.ascx.cs"  ViewStateMode="Inherit"
    Inherits="eStore.UI.Modules.FooterContent" %>
<div class="footercenterbg fixFootercenter">
    <h5 class="tb12">
        <span id="eStoreFooter_lbaboutus" class="tb12">
            <asp:literal id="ltCategoryName" runat="server"></asp:literal></span></h5>
    <ul>
        <asp:repeater id="rtFooterLeft" runat="server">
            <itemtemplate>
                <li>
                    <a href='<%# esUtilities.CommonHelper.ConvertToAppVirtualPath(Eval("URL").ToString()) %>'  target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>"><%# Eval("MenuName") %></a>
                </li>
            </itemtemplate>
        </asp:repeater>
    </ul>
    <ul>
        <asp:repeater id="rtFooterRight" runat="server">
            <itemtemplate>
                <li>
                    <a href='<%# esUtilities.CommonHelper.ConvertToAppVirtualPath(Eval("URL").ToString()) %>'  target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>"><%# Eval("MenuName") %></a>
                </li>
            </itemtemplate>
        </asp:repeater>
    </ul>
</div>
