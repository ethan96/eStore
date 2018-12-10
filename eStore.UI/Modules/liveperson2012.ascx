<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="liveperson2012.ascx.cs" Inherits="eStore.UI.Modules.liveperson2012"  EnableViewState="false"%>
<%@ Register Src="CallMeNow.ascx" TagName="CallMeNow" TagPrefix="eStore" %>
<asp:Panel ID="PNAskanExpert" runat="server">
    <div class="<%=livepersonStyle %>">
<div class="at-expert-box-2">
    <div class="iplanet_online_chat">
          <asp:Literal ID="laskanexpert" runat="server" Text=""></asp:Literal>
        <table border="0" cellpadding="0" cellspacing="0" class="iplanet_online_call02">
            <tr>
                <td>
                 <asp:Image ID="Image2" ImageUrl="~/images/chat_icon.jpg"  runat="server" />
                </td>
                <td width="5">
                </td>
                <td>  <a id="_lpChatBtn" runat="server" clientidmode="Static">Chat Online Now</a>  
                   
                </td>
            </tr>
            <tr>
                <td colspan="3" height="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Image ID="Image3" ImageUrl="~/images/call_icon_02.jpg"  runat="server" />
                </td>
                <td width="5">
                </td>
                <td>
                    <a id="_lpLiveCallBtn" runat="server" clientidmode="Static">Request Call Back</a>
                </td>
            </tr>
        </table>
    </div>
</div>
</div>
<eStore:CallMeNow ID="CallMeNow1" runat="server" />
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
</script></asp:Panel>
<asp:Literal ID="ltCNQQ" runat="server"></asp:Literal>