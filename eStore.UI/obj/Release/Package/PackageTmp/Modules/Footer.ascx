<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="eStore.UI.Modules.Footer"
     EnableViewState="false" %>
<%@ OutputCache Duration="120" VaryByParam="None" %>
<%@ Register Src="FooterContent.ascx" TagName="FooterContent" TagPrefix="eStore" %>
<div class="footerleft">
</div>
<div style="float: left;" class="footercenterbg fixLiveHelp">
    <div align="center" id="footerchat" runat="server">
        <div class="padding-top2px">
                <asp:Image ID="imgLiveHelp" runat="server" CssClass="imgLiveHelp" />
            <asp:Literal ID="ltLiveHelpTitle" runat="server"></asp:Literal>
            </div>
            <div class="clear"></div>
            <div id="footerLiveperson" class="footerLiveperson">
            </div>
            <div id="footerCallme" class="footerCallme">
                <asp:Literal ID="ltFooterCallme" runat="server"></asp:Literal>
            </div>
        <script type="text/javascript" language="javascript">
            $("#footerLiveperson").hover(
              function () {
                  $(this).removeClass("footerLiveperson");
                  $(this).addClass("footerLivepersonhover");
              },
              function () {
                  $(this).addClass("footerLiveperson");
                  $(this).removeClass("footerLivepersonhover");
              }
            );
            $("#footerLiveperson").click(function () {
                $("#_lpChatBtn").click();
            });
            $("#footerCallme").click(function () {
                if ($("#_lpLiveCallBtn[href*='ContactUS']").length > 0) {
                    document.location = $("#_lpLiveCallBtn").attr("href");
                }
                else {
                    $("#_lpLiveCallBtn").click();
                }
            });
            $("#footerCallme").hover(
  function () {
      $(this).removeClass("footerCallme");
      $(this).addClass("footerCallmehover");
  },
  function () {
      $(this).addClass("footerCallme");
      $(this).removeClass("footerCallmehover");
  }
);
        </script>
    </div>
    <div align="center">
        <asp:HyperLink ID="hlJPContact" runat="server" Visible="false" />
    </div>
</div>
<div class="footerline">
</div>
<eStore:FooterContent ID="FooterContent1" runat="server" />
<div class="footerline">
</div>
<eStore:FooterContent ID="FooterContent2" runat="server" />
<div class="footerline">
</div>
<eStore:FooterContent ID="FooterContent3" runat="server" />
<div class="footerright">
</div>
<div style="clear: both;">
</div>
<div class="footerbg">
    <div class="floatleft">
        <img src='<%= ResolveUrl("~/App_Themes/Default/icon02.gif")%>'  width="11" height="11" hspace="5" alt="" />
        <a class="footer" href='<%= ResolveUrl("~/ContactUS.aspx?tabs=general-inquiries")%>' >
            <asp:Literal ID="Literal_Feedback" runat="server" EnableViewState="False">Website User Feedback</asp:Literal></a></div>
    <div class="floatright">
        <asp:Literal ID="Literal_Copyright" runat="server" EnableViewState="False"></asp:Literal>
    </div>
</div>
<asp:Repeater ID="rpLanding" runat="server">
    <HeaderTemplate>
        <div class="footerkeywords">
            <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li><a href="<%#ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(Container.DataItem))%>"
            target="<%# Eval("Target")!=null&& Eval("Target")!=""? Eval("Target"):"_self" %>"><%# Eval("MenuName").ToString() %></a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul> </div>
    </FooterTemplate>
</asp:Repeater>
