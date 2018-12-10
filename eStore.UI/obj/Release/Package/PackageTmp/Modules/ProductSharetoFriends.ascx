<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductSharetoFriends.ascx.cs" 
    Inherits="eStore.UI.Modules.ProductSharetoFriends" %>
<style type="text/css">
#ProS2FDialog {
	height: 250px;
	width: 420px;
	font-family: Arial, Helvetica, sans-serif;
	font-size: 12px;
	padding-left: 10px;
}
#ProS2FDialog p,#ProS2FDialog ul,#ProS2FDialog li{
	padding: 0px;
	list-style-image: none;
	list-style-type: none;
	margin-top: 3px;
	margin-right: 0px;
	margin-bottom: 0px;
	margin-left: 0px;
}
#ProS2FDialog ul{
	clear: both;	
}
#ProS2FDialog .bold{
	font-weight: bold;	
}
#ProS2FDialog .clear{
	clear: both;	
}
#ProS2FDialog .ProS2FDialogleft{
	float: left;
	width: 150px;
	padding-right: 10px;
	text-align: right;
}
#ProS2FDialog .ProS2FDialogright{
	width: 260px;
	float: left;
}
</style>
<div id="ProS2FDialog">
    <p class="actionblock">
        Please enter the following information, and click the Submit button.</p>
    <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_product" runat="server" CssClass="labels">Product</asp:Label></li>
        <li class="ProS2FDialogright">
            <span class="bold">
                <asp:Label ID="lbl_model_name" runat="server" CssClass="modelname"></asp:Label>
            </span></li>
    </ul>
    <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_first_name" runat="server" CssClass="labels" Text="First Name"></asp:Label></li>
        <li class="ProS2FDialogright"><eStore:TextBox ID="txt_first_name" runat="server" Columns="36"></eStore:TextBox>
            <label class="requiredStar">*</label><asp:RequiredFieldValidator ID="RFV_first_name" runat="server" ErrorMessage="Please enter your first name."  ValidationGroup="vProductSharetoFriends"
            SetFocusOnError="True" ControlToValidate="txt_first_name" Display="None"></asp:RequiredFieldValidator>
        </li>
    </ul>
    <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_last_name" runat="server" CssClass="labels" Text="Last Name"></asp:Label></li>
    <li class="ProS2FDialogright">
      <eStore:TextBox ID="txt_last_name" runat="server" Columns="36"></eStore:TextBox>
    </li>
    </ul>
    <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_your_email" runat="server" CssClass="labels" Text="Your Email Address"></asp:Label></li>
      <li class="ProS2FDialogright">
        <eStore:TextBox ID="txt_your_email" runat="server" Columns="36"></eStore:TextBox>
        <label class="requiredStar">*</label>
        <asp:RequiredFieldValidator ID="RFV_your_email" runat="server" ErrorMessage="Please enter your email address."  ValidationGroup="vProductSharetoFriends"
            SetFocusOnError="True" ControlToValidate="txt_your_email" Display="None"></asp:RequiredFieldValidator>
          <asp:RegularExpressionValidator ID="REV_your_email" runat="server" 
              ControlToValidate="txt_your_email" Display="None"
              ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
              ErrorMessage="Your email is illegal email address" SetFocusOnError="True" 
              ValidationGroup="vProductSharetoFriends"></asp:RegularExpressionValidator>
      </li>
    </ul>
    <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_friends_email" runat="server" CssClass="labels" Text="Email of Your Friend(s)"></asp:Label></li>
        <li class="ProS2FDialogright">
        <eStore:TextBox ID="txt_friends_email" runat="server" Columns="36"></eStore:TextBox>
        <label class="requiredStar">*</label>
        <asp:RequiredFieldValidator ID="RFV_friends_email" runat="server" ControlToValidate="txt_friends_email"
            Display="None" EnableViewState="False" ErrorMessage="Please enter your friend(s) email." ValidationGroup="vProductSharetoFriends"
            SetFocusOnError="True"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="REV_friedns_email" runat="server" 
              ControlToValidate="txt_friends_email" Display="None"
              ValidationExpression="(\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\;?)+" 
              ErrorMessage="Your friend(s) mail is illegal email address" SetFocusOnError="True" 
              ValidationGroup="vProductSharetoFriends"></asp:RegularExpressionValidator>
        </li>
  	</ul>
        <ul>
   	  <li class="ProS2FDialogleft">&nbsp;</li>
        <li class="ProS2FDialogright"><span class="notes">Please use " ; " to divide each email address.</span></li>
  </ul>
  <ul>
    	<li class="ProS2FDialogleft"><asp:Label ID="lbl_comments" runat="server" CssClass="labels" Text="Your Comments"></asp:Label></li>
        <li class="ProS2FDialogright">
        <eStore:TextBox ID="txt_comments" runat="server" Width="245px" Rows="8" 
                TextMode="MultiLine" Height="81px"></eStore:TextBox>
      </li>
    </ul>
        <asp:Button ID="btn_submit" runat="server" CssClass="ProductSharetoFriends"  Text="Submit" OnClick="btn_submit_Click"  ValidationGroup="vProductSharetoFriends"/>
    <asp:ValidationSummary ID="vts_ProductSharetoFriends" runat="server" 
    ShowMessageBox="True" ShowSummary="False" 
    ValidationGroup="vProductSharetoFriends" />
</div>

