<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eStore.UI.Modules.Header" %>
<%@ Register Src="HeaderMenu.ascx" TagName="HeaderMenu" TagPrefix="eStore" %>
<%@ Register Src="UserLogin.ascx" TagName="UserLogin" TagPrefix="eStore" %>
<%@ Register Src="ChangeRegion.ascx" TagName="ChangeRegion" TagPrefix="eStore" %>
<div class="headerwrapper">
    <div class="header" runat="server" id="headerdivision">
        <div>
            <a class="eStoreLogo" href="<%= ResolveUrl("~/") %>"></a>
        </div>
        <ul class="HeaderList">
            <li><a href="<%= ResolveUrl("~/Compare.aspx") %>">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Compare_List)%>
            </a></li>
            <li><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_View_Cart)%></a><span>|</span></li>
            <li><a href="<%= ResolveUrl("~/Quotation/myquotation.aspx") %>" class="needlogin">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote)%></a><span>|</span></li>
            <%if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false))
            { %>
                <li><a href="<%= ResolveUrl("~/Reward/CreditReward.aspx") %>" class="needlogin">
                        <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.My_Credit_Reward)%>
                    </a><span>|</span></li>
            <%} %>
            <li><a href="<%= ResolveUrl("~/Cart/myorders.aspx") %>" class="needlogin">
                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order)%></a><span>|</span></li>

            <li>
                <div class="langueTitle languageTypeEvent">
                    <asp:Literal Text="" runat="server" ID="ltlangue" />
                    <input type="hidden" id="hdChangeLanguages" />    
                </div>
                <div class="langueContext languageTypeEvent hiddenitem">
                    <asp:Repeater runat="server" ID="rpLangues">
                        <ItemTemplate>
                        <span onclick='changeLnaguageRegion(this)' code='<%# Eval("Language.Code")%>'><%# Eval("Language.Name")%></span>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="clear"></div>
                </div>
            </li>
            <li>
                <eStore:ChangeRegion ID="ChangeRegion1" runat="server" />
            </li>
        </ul>
        <div class="OrderByPartNOPanel">
            <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableOrderByPartNO"))
               {%>
            <a class="OrderByPartNO StoreButton needlogin" href="<%= ResolveUrl("~/Product/OrderbyPartNO.aspx") %>">
                <span>
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Order_By_Part_Number)%>
                </span></a>
            <script type="text/javascript" language="javascript">
                $(".OrderByPartNOPanel a").click(function () {
                    if (typeof (_wmx) != "undefined") {
                        BtnTrack("Order by Part Number", "4", "");
                    }
                });
            </script>
            <%}
               else
               { %>
               &nbsp;
            <%} %>
        </div>
        <asp:Literal ID="ltServingIndustrial" runat="server" EnableViewState="false"></asp:Literal>
        <eStore:HeaderMenu ID="HeaderMenu1" runat="server" />
        <div class="HeaderNav">
            <div class="HeaderContactUs">
                <asp:Literal ID="lStoreContact" runat="server" EnableViewState="false"></asp:Literal></div>
            <div id="centerpopupreopen">
            </div>
            <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="false">
                <AnonymousTemplate>
                    <ul>
                        <li><a href="<%= string.Format("https://member.advantech.com/forgetpassword.aspx?pass={0}&lang={1}&CallBackURLName={2}&callbackurl={3}&group={4}&RegPurpose=HeaderRegisterLink"
                    , eStore.Presentation.eStoreContext.Current.storeMembershippass
                    , eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    ,"Go eStore"
                    , CurrentUrlEncodePath
                    ,eStore.Presentation.eStoreContext.Current.BusinessGroup) %>">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Forgot_password)%>
                        </a></li>
                        <li><a id="RegisterHereLink" href="<%= string.Format("https://member.advantech.com/profile.aspx?pass={0}&lang={1}&CallBackURLName={2}&callbackurl={3}&RegPurpose=HeaderRegisterLink. tracking: {4}&group={5}"
                    , eStore.Presentation.eStoreContext.Current.storeMembershippass
                    , eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    ,"Go eStore"
                    , CurrentUrlEncodePath
                    ,HttpUtility.UrlEncode(string.Format("http://buy.advantech.com/admin/SessionHistory.aspx?sessionID={0}",eStore.Presentation.eStoreContext.Current.SessionID
                   )) ,eStore.Presentation.eStoreContext.Current.BusinessGroup)%>">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Register_Here)%>
                        </a></li>
                        <li><a href="#" id="HeadLoginStatus" runat="server" class="needlogin">
                            <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Login)%>
                        </a></li>
                    </ul>
                    <eStore:UserLogin ID="UserLogin1" runat="server" />
                </AnonymousTemplate>
                <RoleGroups>
                    <asp:RoleGroup Roles="Employee">
                        <ContentTemplate>
                            <ul>
                                <li>
                                    <asp:LinkButton ID="lbtnEditProfile" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Edit_Profile)%></asp:LinkButton>
                                    <asp:DropDownList ID="ddlEmployeeRoles" runat="server" OnSelectedIndexChanged="ddlEmployeeRoles_SelectedIndexChanged"
                                        Visible="false" AutoPostBack="true">
                                        <asp:ListItem Text="Employee" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Customer"></asp:ListItem>
                                    </asp:DropDownList>
                                </li>
                                <li class="myAccountList">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account)%>
                                 
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
                                    
                                </li>
                                <li>
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Welcome)%>
                                    <span>
                                        <asp:Literal ID="lUserName" runat="server"></asp:Literal></span>! [
                                    <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                        LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" />
                                    ]
                                    <asp:Literal ID="lSwitchUser" runat="server"></asp:Literal></li>
                            </ul>
                        </ContentTemplate>
                    </asp:RoleGroup>
                    <asp:RoleGroup Roles="Customer">
                        <ContentTemplate>
                            <ul>
                                <li>
                                    <asp:LinkButton ID="lbtnEditProfile" runat="server">Edit Profile</asp:LinkButton>
                                    <asp:DropDownList ID="ddlEmployeeRoles" runat="server" OnSelectedIndexChanged="ddlEmployeeRoles_SelectedIndexChanged"
                                        AutoPostBack="true">
                                        <asp:ListItem Text="Employee"></asp:ListItem>
                                        <asp:ListItem Text="Customer" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </li>
                                <li>Welcome <span>
                                    <asp:Literal ID="lUserName" runat="server"></asp:Literal></span>! [
                                    <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                        LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" />
                                    ] </li>
                            </ul>
                        </ContentTemplate>
                    </asp:RoleGroup>
                </RoleGroups>
            </asp:LoginView>
        </div>
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

    <asp:Button ID="btChangeLanguages" runat="server" Text="Change Language" Visible="false" 
            onclick="btChangeLangue_Click" />
</div>
<script type="text/javascript">
    function changeLnaguageRegion(th) {
        __doPostBack('<%= btChangeLanguages.UniqueID %>', $(th).attr("code"));
    }

    $(function () {
        $(".languageTypeEvent").hover(
          function () {
              $(".langueContext").show();
          },
          function () {
            $(".langueContext").hide();
          }
        );
    });

</script>
