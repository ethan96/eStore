<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CultureFullName.ascx.cs"
    Inherits="eStore.UI.Modules.CultureFullName" %>
<td>
    <label class="<%=currentCss %>">
        <asp:Literal ID="lleft" runat="server">First Name</asp:Literal>
        <span class="eStore_redStar">*</span>:
    </label>
</td>
<td>
    <eStore:TextBox ID="txtlfet" runat="server" ToolTip="First Name" CssClass="cssFirstName require">
    </eStore:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        ErrorMessage="First Name cannot be empty." Display="None" ControlToValidate="txtlfet"
        ValidationGroup="CustomerProfile" InitialValue="First Name"></asp:RequiredFieldValidator>
</td>
<td>
    <label class="<%=currentCss %>">
        <asp:Literal ID="lright" runat="server">Last Name</asp:Literal>
        <span class="eStore_redStar">*</span>:
    </label>
</td>
<td>
    <eStore:TextBox ID="txtright" runat="server" ToolTip="Last Name" CssClass="cssLastName require">
    </eStore:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
        ErrorMessage="Last Name cannot be empty." ValidationGroup="CustomerProfile" InitialValue="Last Name"
        ControlToValidate="txtright" Display="None"></asp:RequiredFieldValidator>
</td>
