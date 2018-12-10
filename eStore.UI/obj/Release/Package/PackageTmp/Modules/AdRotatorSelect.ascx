<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdRotatorSelect.ascx.cs" Inherits="eStore.UI.Modules.AdRotatorSelect"  EnableViewState="false"%>
<asp:Panel id="plBanner" runat="server">
    <div id="homeBannerheader">
      <div class="wrap">
        <div id="slide-holder">
          <div id="slide-runner"> 
            <asp:Literal ID="ltBannerContent" runat="server"></asp:Literal>
            <div id="slide-controls">
              <p id="slide-nav"></p>
            </div>
          </div>
        </div>
        <asp:Literal ID="ltBannerJS" runat="server"></asp:Literal>
      </div>
    </div>
</asp:Panel>
<!--//幻灯片结束-->