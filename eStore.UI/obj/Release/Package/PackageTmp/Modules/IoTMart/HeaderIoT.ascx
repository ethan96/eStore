<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderIoT.ascx.cs" Inherits="eStore.UI.Modules.IoTMart.HeaderIoT" %>
<%@ Register Src="~/Modules/UserLogin.ascx" TagName="UserLogin" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/IoTMart/UserLoginUshop.ascx" TagName="UserLoginUshop" TagPrefix="eStore" %>
<%@ Register Src="~/Modules/ChangeRegion.ascx" TagName="ChangeRegion" TagPrefix="eStore" %>

<link href="<%=esUtilities.CommonHelper.GetStoreLocation() %>Styles/jquery.fancybox-1.3.4.css" rel="stylesheet" type="text/css" />
<script src="<%=esUtilities.CommonHelper.GetStoreLocation() %>Scripts/jquery.fancybox-1.3.4.js" type="text/javascript"></script>

<div class="iot-header">
	<div class="iot-logo">
        <a href='<%= ResolveUrl("~/") %>'><img src="<%=logoimage %>" /></a></div>
        <asp:HyperLink ID = "hyHeadMessage" runat="server"></asp:HyperLink>
    <div class="iot-topMsg">
    	<div class="iot-topLink">
        <dl style="float:right">
            <dt class="floatLeft">
            <eStore:ChangeRegion ID="ChangeRegion1" ImageCss="showCountryRegion mOut" runat="server" /></dt>
            
            <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="false">
                <AnonymousTemplate>
                    <dt class="floatLeft"><a href="#" id="HeadLoginStatus" runat="server" class="needlogin">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Login)%>
                        </a></dt>
                    <dt class="floatLeft">
                   <%if (eStore.Presentation.eStoreContext.Current.MiniSite != null && eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == eStore.POCOS.MiniSite.SiteType.UShop)
                     {  %>
                        <a id="RegisterHereLink" href="<%=esUtilities.CommonHelper.GetStoreLocation() %>RegisterForm.aspx"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Register_Here)%></a>
                   <%}
                     else
                     { %>
                    <a id="RegisterHereLink" href="<%= string.Format("https://member.advantech.com/profile.aspx?pass={0}&lang={1}&CallBackURLName={2}&callbackurl={3}&RegPurpose=HeaderRegisterLink. tracking: {4}&group={5}"
                    , eStore.Presentation.eStoreContext.Current.storeMembershippass
                    , eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    ,"Go eStore"
                    , CurrentUrlEncodePath
                    ,HttpUtility.UrlEncode(string.Format("http://buy.advantech.com/admin/SessionHistory.aspx?sessionID={0}",eStore.Presentation.eStoreContext.Current.SessionID
                   )) ,eStore.Presentation.eStoreContext.Current.BusinessGroup)%>">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Register_Here)%>
                        </a>
                        <%} %>
                    </dt>
                    <asp:PlaceHolder ID="phLogPlace" runat="server"></asp:PlaceHolder>
                </AnonymousTemplate>
                <RoleGroups>
                    <asp:RoleGroup Roles="Employee">
                        <ContentTemplate>
                            <dt class="myAccountList floatLeft">
                                <asp:LinkButton ID="lUserName" runat="server"></asp:LinkButton>
                                <ul>
                                            <li>
                                                <asp:LinkButton ID="lbtnOM" PostBackUrl="http://buy.advantech.com/admin/" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Online_manager)%></asp:LinkButton></li>
                                            <li><a href="<%= ResolveUrl("~/Account/ReleaseCache.aspx") %>">
                                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Release_Website_Cache)%></a></li>
                                            <% if (!eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableOrderByPartNO"))
                                               {%>
                                            <li><a href="<%= ResolveUrl("~/Product/OrderbyPartNO.aspx") %>"><span>
                                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Order_By_Part_Number)%>
                                            </span></a></li>
                                            <%} %>
                                            <li>
                                                <eStore:TextBox ID="txtSwitchUser" CssClass="textEntry" runat="server"></eStore:TextBox>
                                                <asp:Button ID="btnSwitchUser" runat="server" Text="Switch" OnClick="btnSwitchUser_Click" />
                                                <asp:Button ID="btnSwitchBack" runat="server" Text="Switch to Me" OnClick="btnSwitchBack_Click" />
                                            </li>
                                        </ul>   

                            </dt>
                            <dt class="floatLeft">[ <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                        LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" CssClass="margin0" /> ]
                                    <asp:Literal ID="lSwitchUser" runat="server"></asp:Literal>

                                    <asp:DropDownList ID="ddlEmployeeRoles" runat="server" OnSelectedIndexChanged="ddlEmployeeRoles_SelectedIndexChanged"
                                        Visible="false" AutoPostBack="true">
                                        <asp:ListItem Text="Employee" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Customer"></asp:ListItem>
                                    </asp:DropDownList></dt> 
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Cart)%></a>
                             </dt>
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Quotation/myquotation.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a>
                             </dt>
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Cart/myorders.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a>
                             </dt>

                        </ContentTemplate>
                    </asp:RoleGroup>
                    <asp:RoleGroup Roles="Customer">
                        <ContentTemplate>
                            <dt class="floatLeft"><span>
                                    <asp:LinkButton ID="lUserName" runat="server"></asp:LinkButton></span></dt>
                            <dt class="floatLeft">[ <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                        LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" CssClass="margin0" /> ] </dt> 
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Cart)%></a>
                             </dt>
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Quotation/myquotation.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a>
                             </dt>
                            <dt class="floatLeft"><a href="<%= ResolveUrl("~/Cart/myorders.aspx") %>" class="needlogin">
                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a>
                             </dt>
                        </ContentTemplate>
                    </asp:RoleGroup>
                </RoleGroups>
            </asp:LoginView>
            <dt class="floatLeft">|<a href="<%= ResolveUrl("~/Compare.aspx") %>">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Compare_List)%></a>|
            </dt>
            <dt class="floatLeft"><a href="http://<%= eStore.Presentation.eStoreContext.Current.Store.profile.StoreURL %>" target="_blank">
                <asp:Literal ID="ltgotoestore" runat="server"></asp:Literal></a>
            </dt>
            </dl>
            </div>
        <div class="clear"></div>
        <div class="iot-topSearch">
        <eStore:StoreSearch ID="StoreSearch1" runat="server" />
        <asp:Button ID="btSearch" runat="server" Text="SEARCH" 
            onclick="btSearch_Click" CssClass="btnSearch" /></div>
    </div>

        <div id="changeCountryRegion" class="changeCountryRegion mOut">
        <div id="changeCountryRegionLeft" class="mOut">
        </div>
        <div id="changeCountryRegionRight" class="mOut">
            <div class="title mOut">
                This site in other countries/regions:</div>
            <ul id="countryRegionUl" class="mOut">
            </ul>
        </div>
    </div>
    <div class="clear">
    </div>
</div>



        
