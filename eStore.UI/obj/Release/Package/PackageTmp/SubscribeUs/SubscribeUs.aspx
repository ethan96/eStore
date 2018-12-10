<%@ Page Title="Subscribe Us" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="SubscribeUs.aspx.cs" Inherits="eStore.UI.SubscribeUs.SubscribeUs" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
        <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/SubscribeUsCSS")%>
        <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/font-awesome")%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
	<div class="subscribeUsBody">
		<h1>Subscribe to Us!</h1>
        	
        <!--intro-->
        <div class="intro">
    	    <p><i class="fa fa-check-square" aria-hidden="true"></i> &nbsp; Sign up only for the subjects you prefer.</p>
    	    <p><i class="fa fa-envelope" aria-hidden="true"></i></i> &nbsp; Receive e-mail directly to your inbox with the latest technology news, products or events.</p>
    	    <p><i class="fa fa-times-circle" aria-hidden="true"></i> &nbsp; Unsubscribe or modify your preferences at any time!</p>
        </div>
         <!--end of intro-->

         <!--company info-->
        <div class="section">    
   	       <h2 class="h2">Contact Information</h2>     
        </div>
 
        <div class="company">
	        <label>First Name<span class="required">*</span></label>
            <asp:TextBox runat="server" id="FirstNameTexBox" CssClass="cannotNull"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="FirstNameTexBox" ErrorMessage="Required fields" ForeColor="#e74c3c" SetFocusOnError="True"></asp:RequiredFieldValidator>
            <br>
            
            <label class="phone">Last Name<span class="required">*</span></label>
            <asp:TextBox runat="server" id="LastNameTexBox" CssClass="cannotNull"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="LastNameTexBox" ErrorMessage="Required fields" ForeColor="#e74c3c" SetFocusOnError="True"></asp:RequiredFieldValidator>
            <br>

            <label class="phone">Country<span class="required">*</span></label>
            <asp:DropDownList runat="server" ID="CountryDrp"></asp:DropDownList><br>
            
            <label class="hours">Email<span class="required">*</span></label>
            <asp:TextBox runat="server" id="EmailTextBox" CssClass="cannotNull"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="EmailTextBox" ErrorMessage="Required fields" ForeColor="#e74c3c" SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="EmailTextBox" Display="Dynamic" ErrorMessage="Invalid email address" ForeColor="#e74c3c" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            <br>

            <label class="job">Job Title</label>
            <asp:TextBox runat="server" id="JobTitleTextBox" CssClass="cannotNull"></asp:TextBox><br>

            <label class="c-name">Company Name</label>
            <asp:TextBox runat="server" id="CompanyNameTextBox" CssClass="cannotNull"></asp:TextBox><br>


	        <p class="small"><span class="required">*Required fields</span></p>       
        </div>
        <h3 class="h3" style="border-bottom: none;">Please select areas of your interest:</h3>
 
 
        <div class="container">
            <asp:CheckBoxList runat="server" ID="BAACbl" RepeatDirection="Vertical" RepeatColumns="3" Width="1000px"></asp:CheckBoxList>
         </div>

        <div class=container2>
            <div class="right">
                <asp:RadioButtonList ID="rblReceiveMessage" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="YES" Value="Yes" Selected="True" />
                    <asp:ListItem Text="NO" Value="No" />
                </asp:RadioButtonList>
            </div>
            <div class="left">
                <p>By submitting your data, you consent to receive electronic messages from Advantech.</p>
            </div>
        </div>

        <div class="register_area">
            <asp:button id="submitSubscribe" runat="server" OnClick="submitSubscribe_Click" CssClass="register"  Text="Sign me Up!" UseSubmitBehavior="False"  OnClientClick='if (checkSubmit() == false) { return false; } ; this.disabled = true; ShowWaitMessage();' />
        </div>        
    </div>




    <script type="text/javascript">

        function checkSubmit() {
            if (Page_ClientValidate()) {
                return true;
            }
            else {
                return false;
            }
        }

        function ShowWaitMessage() {
            document.getElementById('<%=submitSubscribe.ClientID %>').value = "Please wait...";
        } 
    </script>
</asp:Content>




