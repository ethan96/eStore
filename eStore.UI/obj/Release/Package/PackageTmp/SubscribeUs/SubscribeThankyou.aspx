<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="SubscribeThankyou.aspx.cs" Inherits="eStore.UI.SubscribeUs.SubscribeThankyou" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
            <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/SubscribeUsCSS")%>
        <%= System.Web.Optimization.Styles.Render("~/App_Themes/V4/font-awesome")%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="eStoreMainContent" runat="server">
     <div class="subscribeUsBody">
<%--    <div class="body">--%>
	    <h1 class="h1">Thank you for subscribing!</h1>
	    <h3 class="h3">You are now subscribed to our email! </h3>
	    <p class="large">Stay Connected with us:</p>
	    <ul>
		    <li><a href="https://www.youtube.com/c/AdvantecheStore"><img src="../images/Youtube.png"></a></li>
		    <li><a href="https://www.linkedin.com/company/advantech-digital-signage"><img src="../images/LinkedIn.png"></a></li>
		    <li><a href="https://plus.google.com/+AdvantecheStore"><img src="../images/Google.png"></a></li>
		    <li><a href="https://twitter.com/advantechE"><img src="../images/Twitter.png"></a></li>
	    </ul>
<%--	</div>--%>
    </div>
</asp:Content>
