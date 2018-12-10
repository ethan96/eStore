<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Popup.aspx.cs" Inherits="eStore.UI.Popup" %>
<%@ Register Src="~/Modules/ChannelPartner.ascx" TagName="T_ChannelPartner" TagPrefix="eStore" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        *{font-size:12px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:PlaceHolder ID="phPopup" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
