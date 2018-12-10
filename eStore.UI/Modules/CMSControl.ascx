<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CMSControl.ascx.cs" Inherits="eStore.UI.Modules.CMSControl" %>
<asp:Panel ID="pbaa"  runat="server"  class="css-Cmsbba">
    <h3 style="color:#666"><span class="textsize16"><asp:Literal ID="ltcmstype" runat="server"></asp:Literal></span> &nbsp; <asp:Literal ID="ltbaa" runat="server"></asp:Literal></h3></asp:Panel>
<asp:Repeater ID="repeaterCMS" runat="server" 
    onitemdatabound="repeaterCMS_ItemDataBound">
    <ItemTemplate>

        <div id="CMS-ContextAll">
          <div id="CMS-DateAre"><%# esUtilities.StringUtility.subDate(Eval("LASTUPDATED")) %></div>
          <div id="CMS-Contex">
  	        <h3 class="htitle">
                  <%# Eval("TITLE")%>  <%# getUrl(Eval("RECORD_ID"), Eval("cmsTypeX"), Eval("TITLE"))%></h3>
            <div class="pcontext"><asp:Literal ID="ltCmsDetailContext" runat="server"></asp:Literal></div>
            <div class="cmsView">
                <%# getCmsView(Eval("HYPER_LINK"))%>
            </div>
          </div>
          <div class="clear"></div>
        </div>
    </ItemTemplate>
</asp:Repeater>