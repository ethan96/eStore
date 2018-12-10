<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="livepersonAUS.ascx.cs" Inherits="eStore.UI.Modules.livepersonAUS" %>
<asp:Panel ID="PNAskanExpert" runat="server">
    <div class="new-<%=livepersonStyle %>">
        <ul class="iplanet_online_chat">
            <li class="askexpert-title">
                <asp:Literal ID="laskanexpert" runat="server"></asp:Literal>
            </li>
            <li class="askexpert-chat">
                <a id="_lpChatBtn" runat="server" clientidmode="Static">Chat Online Now</a>
            </li>
            <li class="askexpert-quote">
                <a id="_lpLiveCallBtn" runat="server" clientidmode="Static">Request Call Back</a>
            </li>
            <li class="askexpert-more"><a id="_lpContactUs" runat="server" clientidmode="Static">More Contacts</a></li>
            <li class="askexpert-tollfree">Toll Free</li>
            <li>
                <asp:Literal ID="ltAddressTel" runat="server"></asp:Literal></li>
        </ul>
    </div>
    <script type="text/javascript" language="javascript">
        $("#_lpChatBtn").click(function () {
            if (typeof (_wmx) != "undefined") {
                var wmx_model = GetMetaContent("wmx_model");
                BtnTrack(wmx_model, "4", "A");
            }
        });
        $("#_lpLiveCallBtn").click(function () {
            if (typeof (_wmx) != "undefined") {
                var wmx_model = GetMetaContent("wmx_model");
                BtnTrack(wmx_model, "4", "B");
            }
        });
    </script>
</asp:Panel>