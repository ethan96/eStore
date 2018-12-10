<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="eStore.UI.Modules.V4.Header" %>
<%@ Register Src="HeaderMenu.ascx" TagName="HeaderMenu" TagPrefix="eStoreV4" %>

<%@ Register src="HeaderMenuV2.ascx" tagname="HeaderMenuV2" tagprefix="uc1" %>


<div class="eStore_header">
    <div class="eStore_headerTop">
        <div class="eStore_block980">
            <div class="eStore_logo">
                <asp:HyperLink ID="logo" runat="server" NavigateUrl="/" EnableViewState="false">
                    <asp:Image ID="logoImage" runat="server"  AlternateText="Advantech eStore Logo"/>
<%--                    <img src='<%=ResolveUrl("~/images/eStore_logoS.png") %>' alt="<%= eStore.Presentation.eStoreContext.Current.Store.profile.StoreName %>"   />--%>
                </asp:HyperLink>
            </div>
            <div class="eStore_mobile">
                <asp:Image ID="imgicon_chat" runat="server" ImageUrl="~/images/icon_chat.png" AlternateText="Contact Advantech" CssClass="eStore_chat" />
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/eStore_iconSearch.png" AlternateText="Search"
                    CssClass="eStore_search" />
                <asp:Image ID="Image4" runat="server" ImageUrl="~/images/eStore_iconShopping.png" AlternateText="Shopping"
                    CssClass="eStore_shoppingCart needlogin mobileNotShow" />
                <asp:Image ID="Image3"
                    runat="server" ImageUrl="~/images/eStore_iconList.png" AlternateText="list" CssClass="eStore_seeMore" />
            </div>
        </div>
    </div>
    <!--headerTop-->
    <div class="eStore_mobileBox">
        <div class="eStore_searchBox">
            <input type="text" title="Search key" name="mobilekeysearch" class="InputValidation storekeyworddispay"
                value="<%= Request["storekeyworddispay"] %>" placeholder="<%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_search_box)%>" /><a href="#"><span></span></a>
        </div>
        <div class="eStore_ShoppingBox">
        </div>
    </div>
    <div class="eStore_headerBottom">
        <div class="eStore_block980">
            <div class="eStore_menuLink">
                <% if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("eStore_search_new_icon", false))
                    { %>
                    <img class="img-search-new" src="<%= ResolveUrl("~/images/eStore_icon_new.png") %>">
                <%} %>
                <div class="eStore_menuLink_searchBlock float-right">
                   <eStore:StoreSearch ID="StoreSearch2" runat="server" /><a href="#" class="submit"><span></span></a>
                    <small class="close" style="display: none;">X</small>
                </div>
                <asp:PlaceHolder ID="phmenu" runat="server"></asp:PlaceHolder>
            </div>
            <!--menuLink-->
            <div class="eStore_topMsg">
                <ol>
                    <li>
                        <asp:DropDownList ID="ddlLanguages" runat="server" CssClass="" AutoPostBack ="true"
                             OnSelectedIndexChanged="ddlLanguages_SelectedIndexChanged">
                        </asp:DropDownList>
                    </li>
                    <li class="toplivechatIcon">
                       <%= ChatStr %>
                    </li>
                    <asp:Literal ID="lStoreContact" runat="server" EnableViewState="false"></asp:Literal>
                    <asp:LoginView ID="HeadLoginView" runat="server" EnableViewState="false">
                        <AnonymousTemplate>
                            
                                <li><a href="#eStore_LogIn_input" class="eStore_MyAccount fancybox">
                                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Login)%>
                                </a>  <eStore:UserLogin ID="UserLogin1" runat="server" /></li>
                            
                          
                        </AnonymousTemplate>
                        <RoleGroups>
                            <asp:RoleGroup Roles="Employee">
                                <ContentTemplate>
                                    <li><a href='<%= ResolveUrl("~/Account/MyAccount.aspx") %>'><span class="eStore_MyAccount">
                                        Hi,
                                        <asp:Literal ID="lUserName" runat="server"></asp:Literal>
                                        <asp:Literal ID="lSwitchUser" runat="server"></asp:Literal>
                                        <asp:Literal ID="ltiAblePoint" runat="server"></asp:Literal></a>
                                        <div class="eStore_login">
                                            <span></span>
                                            <ul>
                                               <li><a href='<%= ResolveUrl("~/Account/MyAccount.aspx") %>'><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account)%></a></li>
                                            <li>
                                                <asp:LinkButton ID="lbtnOM" PostBackUrl="http://buy.advantech.com/admin/" runat="server"><%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Online_manager)%></asp:LinkButton></li>
                                            <li><a href="<%= ResolveUrl("~/Account/ReleaseCache.aspx") %>">
                                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Release_Website_Cache)%></a></li>
                                            <% if (!eStore.Presentation.eStoreContext.Current.getBooleanSetting("EnableOrderByPartNO"))
                                               {%>
                                            <li><a href="<%= ResolveUrl("~/Product/OrderbyPartNO.aspx") %>">
                                                <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Order_By_Part_Number)%>
                                            </a></li>
                                            <%} %>
                                            <li>
                                                <eStore:TextBox ID="txtSwitchUser" CssClass="textEntry" runat="server"></eStore:TextBox>
                                                <asp:Button ID="btnSwitchUser" runat="server" Text="Switch" OnClick="btnSwitchUser_Click" />
                                                <asp:Button ID="btnSwitchBack" runat="server" Text="Switch to Me" OnClick="btnSwitchBack_Click" />
                                            </li>
                                        </ul> </div>
                                    </li>
                                    <li>
                                        <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                            LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" />
                                    </li>
                                    <li class="eStore_shoppingCart mobileNotShow"><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
                        (<span><asp:Literal ID="lCartItemCount" runat="server">0</asp:Literal></span>)</a></li>
                                </ContentTemplate>
                            </asp:RoleGroup>
                            <asp:RoleGroup Roles="Customer">
                                <ContentTemplate>
                                    <li><a href='<%= ResolveUrl("~/Account/MyAccount.aspx") %>' class="mobileHref"><span>Hi,
                                        <asp:Literal ID="lUserName" runat="server"></asp:Literal><asp:Literal ID="ltiAblePoint" runat="server"></asp:Literal></span> </a>
                                    </li><li>
                                        <asp:LoginStatus ID="HeadLoginStatus" runat="server" LogoutAction="Redirect" LogoutText="Log Out"
                                            LogoutPageUrl="~/" OnLoggedOut="HeadLoginStatus_LoggedOut" />
                                    </li>
                                    <li class="eStore_shoppingCart"><a href="<%= ResolveUrl("~/Cart/Cart.aspx") %>" class="needlogin">
                                        (<span><asp:Literal ID="lCartItemCount" runat="server">0</asp:Literal></span>)</a></li>
                                </ContentTemplate>
                            </asp:RoleGroup>
                        </RoleGroups>
                    </asp:LoginView>
               
                    
                </ol>
                <% if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnableOrderByPartNO"))
                   {%>
                <a href="<%= ResolveUrl("~/Product/OrderbyPartNO.aspx") %>" class="eStore_btn needlogin">
                    <%=eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Order_By_Part_Number)%></a>
                <%} %>
            </div>
            <!--eStore_topMsg-->
        </div>
    </div>
    <!--headerBottom-->
</div>
