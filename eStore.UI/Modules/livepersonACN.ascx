<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="livepersonACN.ascx.cs"
    Inherits="eStore.UI.Modules.livepersonACN" %>
<div id="Banners">
    <div id="Banner_1">
        <asp:ImageMap ID="imglivepersonACN" Width="208" Height="180" AlternateText="Shopping choices"
            runat="Server">
        </asp:ImageMap>
    </div>
    <div id="Banner_2">
        <asp:Image ID="imgwebaccess" runat="server" />
    </div>
</div>
<script type="text/javascript">
    function showQQAPI()
    { window.open('http://b.qq.com/webc.htm?new=0&sid=8008100345&eid=218808P8z8p8Q8K8z8p80&o=http://buy.advantech.com.cn/&q=7&ref=' + document.location, '_blank', 'height=544, width=644,toolbar=no,scrollbars=no,menubar=no,status=no'); }
</script>