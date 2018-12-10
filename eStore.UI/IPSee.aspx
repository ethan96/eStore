<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPSee.aspx.cs" Inherits="eStore.UI.IPSee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Your Ip is:</h3>
            <h4>
                <asp:Literal ID="ltIp" runat="server"></asp:Literal></h4>
            <h4>
                <asp:Literal ID="ltArea" runat="server"></asp:Literal></h4>
        </div>
    </form>
</body>
</html>
