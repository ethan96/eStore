<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MPV4.Master" AutoEventWireup="true" CodeBehind="FindPartner.aspx.cs" Inherits="eStore.UI.ECO.FindPartner" %>
<%@ Register src="../Modules/CountrySelector.ascx" tagname="CountrySelector" tagprefix="uc1" %>
<%@ Register src="../Modules/ECO/ECOPartnerList.ascx" tagname="ECOPartnerList" tagprefix="uc2" %>

 <asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
  <style>
      
/* start ECO Search  */
#ecoSearchInfor {
	width: 183px;
	font-size:12px;
	margin-left: auto;
	margin-right: auto;
	margin-bottom: 7px;
}
#ecoSearchInfor #ecotitle
{
    background-image: url('ecotitle-bg.jpg');
    background-repeat: repeat-x;
    height: 28px;
font-weight:normal; font-size:12px;
    color: #FFF;
    line-height: 25px;
    text-align: center;
}

#ecoSearchInfor #ecoContext {
	padding: 5px;
	border-right-width: 1px;
	border-bottom-width: 1px;
	border-left-width: 1px;
	border-right-style: solid;
	border-bottom-style: solid;
	border-left-style: solid;
	border-right-color: #CCC;
	border-bottom-color: #CCC;
	border-left-color: #CCC;
}
#ecoSearchInfor .eco-subtitle{ padding:5px 0px; font-weight:bold;}
#ecoSearchInfor #ecoContext #ecoBTSearch {
	text-align: right;
	padding-right: 20px;
	margin-top: 5px;
    margin-bottom: 5px;
}
#ecoSearchInfor #ecoContext #ecoInterested {
	-webkit-text-size-adjust:none;
	font-size: 10px;
	color: #666;
	text-decoration: underline;
}
#ecoParterList #ecoParterSearchResult {
	padding-top: 10px;
}
#ecoParterList .ecoParter-List {
	margin-top: 5px;
	margin-bottom: 8px;
	border: 1px dashed #CCC;
}
#ecoParterList #ecoParterList-title {
 
 
	font-weight: bold;
	color: #666;
	font-size: 24px;
	padding-bottom:5px;
 
}
#ecoParterList .ecoParter-List ul li {
	margin-left: 15px;
	line-height: 25px;
	margin-right: 10px;
}
#ecoParterList .ecoMtitle {
	float: left;
	font-weight: bold;
}
#ecoParterList .ecoMDes {
	float: right;
	width: 660px;
}
#ecoParterList p, #ecoParterList ul, #ecoParterList li, #ecoParterList h3, #ecoParterList h4 {
	list-style-image: none;
	list-style-type: none;
}
#ecoParter-logo
{
	padding-top: 5px;
	padding-right: 5px;
	padding-bottom: 5px;
	padding-left: 5px;
}
.ecoButton-Below {
	text-align: right;
	padding-right: 70px;
	padding-top: 2px;
	padding-bottom: 7px;
}
#ecoInterestedPartner p, #ecoInterestedPartner ul, #ecoInterestedPartner li, #ecoInterestedPartner h3, #ecoInterestedPartner h4 {
	list-style-image: none;
	list-style-type: none;
}
#ecoInterestedPartner {
	width: 550px;
}
#ecoInterestedPartner .ecoInPartner-title {
	float: left;
	width: 113px;
}
#ecoInterestedPartner .ecoInPartner-title2 {
	float: left;
	width: 110px;
}
#ecoInterestedPartner .normalLineHeigh
{
    line-height: normal;
}
.paddingTop10
{
    padding-top: 10px;
}
.ecoInPartnerfloatLable label {
	width: 108px;
	display: block;
	float: left;
	font-weight:bold;
	margin-right: 5px;
}
#ecoInterestedPartner .ecoInPartner-titleWith200 {
	width:150px;
	float: left;
}
#ecoInterestedPartner ul li {
	line-height: 25px;
}
#ecoInterestedPartner .ecoInPartner-twocolumn {
	float: left;
	width: 160px;
}
#ecoInterestedPartner .ecoInPartner-onecolumn {
	float: left;
	width: 380px;
}
#ecoInterestedPartner .ecoInPartner-company {
	padding-left: 50px;
}
#ecoEmailFrinds {
	width: 350px;
}
#ecoEmailFrinds p {
	line-height: 25px;
}
#ecoEmailFrinds .ecoEmailFrinds-title {
	float: left;
	width: 130px;
}
#ecoRequestAssistance p, #ecoRequestAssistance ul, #ecoRequestAssistance li, #ecoRequestAssistance h3, #ecoRequestAssistance h4 {
	list-style-image: none;
	list-style-type: none;
}
#ecoRequestAssistance {
	width: 560px;
}
#ecoRequestAssistance .ecoInPartner-title {
	float: left;
	width: 120px;
}
#ecoRequestAssistance .ecoInPartner-title100 {
	float: left;
	width: 90px;
}
#ecoRequestAssistance ul label {
	width: 120px;
	display: block;
	float: left;
	font-weight:bold;
}
#ecoRequestAssistance .ecoInPartner-titleWith200 {
	width:150px;
	float: left;
}
#ecoRequestAssistance ul li {
	line-height: 25px; clear:left; padding-bottom:5PX;
}
#ecoRequestAssistance .ecoInPartner-twocolumn {
	float: left;
	width: 160px;
}
#ecoRequestAssistance .ecoInPartner-onecolumn {
	float: left;
	width: 380px;
}
#ecoRequestAssistance .ecoInPartner-company {
	padding-left: 50px;
}
#ecoSearchInfor #ecoContext .eco-desContext {
	margin-top: 3px;
	margin-bottom: 5px;
}
#ecoSearchInfor #ecoContext .eco-checklable{
    padding-left: 5px;
}
#ecoSearchInfor #ecoContext .eco-checklable label {
	padding-left: 3px;
}#ecoSearchInfor #ecoContext .eco-ddlList { padding: 5px 0px;}
#ecoSearchInfor #ecoContext .eco-ddlList p {
	padding-top: 3px;
	padding-bottom: 2px;
}
#ecoSearchInfor #ecoContext .eco-ddlList p select {
	margin: 0px;
	padding: 0px;
}
#ecoPartnerTitle {
	height: 28px;
	line-height: 28px;
	padding-left: 10px;
	margin-bottom: 5px; background-image:url(ecotitle-bg.jpg);font-weight:normal; font-size:12px;
}
.country-traverse
{
    float:left;
    clear:left;
     padding-bottom:7px;}
.country-traverse p {
	padding: 0px;
	float: left;
	margin-top: 0px;
	margin-right: 15px;
	margin-bottom: 0px;
	margin-left: 0px;
}
.eco-FPSearch-context {
	background-color: #CCC;
	padding-top: 7px;
	padding-right: 7px;
	padding-bottom: 7px;
	padding-left: 0px;
	margin-top: 5px;
}
.eco-FPSearch-context label{
	width: 115px;
	display: block;
	float: left;
	line-height: 24px;
	padding-left:20px;
}
.eco-FPSearch-context select{ float:left; background-color:White;}
.eco-FPSearch-context p{ height:24px;}
#findparter-context img{ width:100%;}
#ecoFPSearch{
	padding-left: 230px;
}
#ecoFindPartnerContext{margin-bottom: 5px;}
/* end ECO Search  */

  </style>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="eStoreMainContent" runat="server">
    <div id="findparter-context">
        <img src="../images/AUS/findpartner.jpg" alt="become a partner" />
        <br />     <br />
    <h3 class="colorBlue fontbold">Complete your Solution</h3>
  Advantech’s interest is to supply our computing platforms to all IoT applications as part of our global strategy in building an Intelligent and Connected Planet. Through this partner marketplace, you are able to find referrals for compatible software, hardware, and integrators to help further the development of your projects.<br />
    </div>
    <br /><br />
    <div id="ecoFindPartnerContext">
        <p class="fontbold">
        Begin by searching with our Ecosystem Partner toolbar.</p>

        <div class="eco-FPSearch-context">
            <p>
            <label>Service Provider</label><asp:DropDownList ID="ddlEOCSpecialty" runat="server" Width="150px">
        </asp:DropDownList></p>
            <div class="country-traverse">
            <uc1:CountrySelector ID="CountrySelector1" runat="server" isShowName="true" ddlWidth="150" FilterCountry="USA,Canada" /></div>
            <div class="clear"></div>
            <p>
            <label>Select by Industry</label><asp:DropDownList ID="ddlECOIndustry" runat="server" Width="150px" CssClass="ecoSearchInput">
            </asp:DropDownList>
        </p>
        <div id="ecoFPSearch">
            <asp:Button ID="btEcoSearch" runat="server" Text="Search" 
                onclick="btEcoSearch_Click" />
        </div>
        </div>
        
    </div>
    <uc2:ECOPartnerList ID="ECOPartnerList1" runat="server"  EnableViewState ="false"/>
</asp:Content>
