using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using System.Web.Security;
using System.Security.Principal;

namespace eStore.UI.Modules.IoTMart
{
    public partial class HeaderIoT : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string LocalCSS = "";

        public string logoimage
        {
            get
            {
                return string.Format("{0}images/{1}/{2}", esUtilities.CommonHelper.GetStoreLocation(),eStore.Presentation.eStoreContext.Current.Store.storeID
                    , (eStore.Presentation.eStoreContext.Current.MiniSite == null ? "iot_top.jpg" : eStore.Presentation.eStoreContext.Current.MiniSite.SiteName + "-logo.jpg"));
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            PlaceHolder phLogPlace = (PlaceHolder)this.HeadLoginView.FindControl("phLogPlace");
            if (phLogPlace != null && phLogPlace.Controls.Count <= 0)
            {
                if (eStore.Presentation.eStoreContext.Current.MiniSite != null && eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop)
                {
                    UserLoginUshop uc = this.LoadControl("~/Modules/IoTMart/UserLoginUshop.ascx") as UserLoginUshop;
                    uc.ID = "UserLogin1";
                    phLogPlace.Controls.Add(uc);
                }
                else
                {
                    UserLogin uc = this.LoadControl("~/Modules/UserLogin.ascx") as UserLogin;
                    uc.ID = "UserLogin1";
                    phLogPlace.Controls.Add(uc);
                }
            }
            ChangeRegion1.ImageAttr.Add("align", "absmiddle");
            bindFont();
            if (!IsPostBack)
            {
                if (eStore.Presentation.eStoreContext.Current.MiniSite!=null  && Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.IotMart)
                    ltgotoestore.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.go_to_eStore);
            }
            hyHeadMessage.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Tui_shou);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                NoneedRebindLoginView = BindData();
            }
            catch (Exception)
            {


            }
            base.OnPreRender(e);
        }
        private bool BindData()
        {
            if (eStoreContext.Current.User != null)
            {
                //string Callbackurl = esUtilities.CommonHelper.GetStoreHost(false) + Request.RawUrl;
                LinkButton lUserName = (LinkButton)this.HeadLoginView.FindControl("lUserName");
                if (lUserName == null)
                    return false;
                lUserName.Text = eStoreContext.Current.Store.getCultureFullName(eStoreContext.Current.User.FirstName, eStoreContext.Current.User.LastName);
                lUserName.Attributes.Add("style", "color: #ff8400;");
                if (Presentation.eStoreContext.Current.MiniSite != null && Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop)
                {
                    lUserName.PostBackUrl = esUtilities.CommonHelper.GetStoreLocation() + "accountForm.aspx";
                }
                else
                {
                    lUserName.PostBackUrl = string.Format("https://member.advantech.com/profile.aspx?pass={0}&id={1}&lang={2}&tempid={3}&CallBackURLName={4}&callbackurl={5}&group={6}"
                        , eStoreContext.Current.storeMembershippass
                        , eStoreContext.Current.User.UserID
                        , eStoreContext.Current.Store.profile.StoreLangID
                        , eStoreContext.Current.User.authKey
                        , "Go eStore"
                        , CurrentUrlEncodePath
                        , eStoreContext.Current.BusinessGroup
                        );
                }

                LoginStatus ls = (LoginStatus)this.HeadLoginView.FindControl("HeadLoginStatus");
                ls.LoginText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_In);
                ls.LogoutText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_Out);

                if (eStoreContext.Current.User.hasRight(POCOS.User.ACToken.SwitchRole))
                {
                    //DropDownList ddlEmployeeRoles = (DropDownList)this.HeadLoginView.FindControl("ddlEmployeeRoles");
                    //ddlEmployeeRoles.SelectedValue = Enum.GetName(typeof(POCOS.User.Role), eStoreContext.Current.User.actingRole);
                    //ddlEmployeeRoles.Visible = true;
                    if (eStoreContext.Current.User.actingRole == POCOS.User.Role.Employee)
                    {
                        if (eStoreContext.Current.User.UserID == eStoreContext.Current.User.actingUser.UserID)
                        {
                            TextBox txtSwitchUser = (TextBox)this.HeadLoginView.FindControl("txtSwitchUser");
                            txtSwitchUser.Visible = true;
                            Button btnSwitchUser = (Button)this.HeadLoginView.FindControl("btnSwitchUser");
                            btnSwitchUser.Visible = true;
                            Literal lSwitchUser = (Literal)this.HeadLoginView.FindControl("lSwitchUser");
                            lSwitchUser.Visible = false;
                            Button btnSwitchBack = (Button)this.HeadLoginView.FindControl("btnSwitchBack");
                            btnSwitchBack.Visible = false;
                        }
                        else
                        {
                            TextBox txtSwitchUser = (TextBox)this.HeadLoginView.FindControl("txtSwitchUser");
                            txtSwitchUser.Visible = false;
                            Button btnSwitchUser = (Button)this.HeadLoginView.FindControl("btnSwitchUser");
                            btnSwitchUser.Visible = false;
                            Literal lSwitchUser = (Literal)this.HeadLoginView.FindControl("lSwitchUser");
                            lSwitchUser.Visible = true;
                            lSwitchUser.Text = "Run as " + string.Format(@"{0} {1}", eStoreContext.Current.User.actingUser.FirstName, eStoreContext.Current.User.actingUser.LastName);
                            Button btnSwitchBack = (Button)this.HeadLoginView.FindControl("btnSwitchBack");
                            btnSwitchBack.Visible = true;
                        }
                    }
                }
                else
                {
                    DropDownList ddlEmployeeRoles = (DropDownList)this.HeadLoginView.FindControl("ddlEmployeeRoles");
                    ddlEmployeeRoles.Visible = false;
                }
            }
            else if (eStoreContext.Current.User == null && Request["needlogin"] != null)
            {
                string url = Request.Url.PathAndQuery;
                url = url.Substring(0, url.IndexOf("="));
                if (url == "/product/system.aspx?productid")//控制ctos部分的登录
                {
                    BindScript("script", "autopopLogin", "$(function() {return popLoginDialog(307,450,'" + Request["purpose"] + "');});");
                }
                else
                {
                    BindScript("script", "autopopLogin", "$(function() {return popLoginDialog(270,450,'" + Request["purpose"] + "');});");
                }
            }
            return true;
        }

        protected void HeadLoginStatus_LoggedOut(object sender, EventArgs e)
        {
            if (eStoreContext.Current.User != null)
            {
                eStoreContext.Current.Store.logout(eStoreContext.Current.User);
                POCOS.Country selectedcountry = Presentation.eStoreContext.Current.CurrentCountry;
                HttpContext.Current.Session.Clear();
                Presentation.eStoreContext.Current.CurrentCountry = selectedcountry;
            }

        }

        protected void ddlEmployeeRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id =
                            (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;


                        try
                        {
                            string[] roles = new string[1];
                            roles[0] = ((DropDownList)sender).SelectedItem.Text;
                            string[] loginInfo = ticket.UserData.Split(';');
                            string userdata = string.Format("{0};{1};{2};{3}", loginInfo[0], loginInfo[1], roles[0], loginInfo[3]);

                            //HttpContext.Current.Session.Abandon();
                            HttpContext.Current.Session.Clear();
                            FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, ticket.Name,
                                DateTime.Now, DateTime.Now.AddMonths(3),
                                ticket.IsPersistent, userdata,
                                FormsAuthentication.FormsCookiePath);
                            string encTicket = FormsAuthentication.Encrypt(t);
                            HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                            if (ticket.IsPersistent)
                            {
                                HttpContext.Current.Response.Cookies.Clear();
                                c.Expires = t.Expiration;
                            }
                            HttpContext.Current.Response.Cookies.Add(c);


                            FormsIdentity NewID = new FormsIdentity(t);
                            GenericPrincipal principal = new GenericPrincipal(NewID, roles);
                            POCOS.User.Role activeRole = (POCOS.User.Role)Enum.Parse(typeof(POCOS.User.Role), roles[0]);
                            eStoreContext.Current.User.switchRole(activeRole);
                            Response.Redirect("~/");
                        }
                        catch { }

                    }
                }
            }
        }

        protected void btnSwitchUser_Click(object sender, EventArgs e)
        {

            TextBox txtSwitchUser = (TextBox)this.HeadLoginView.FindControl("txtSwitchUser");
            if (eStoreContext.Current.User.switchRole(POCOS.User.Role.Employee, txtSwitchUser.Text))
            {
                eStoreContext.Current.UserShoppingCart = null;
                eStoreContext.Current.Quotation = null;
                eStoreContext.Current.Order = null;

                Response.Redirect("~/");
            }
        }

        protected void btnSwitchBack_Click(object sender, EventArgs e)
        {
            eStoreContext.Current.User.switchRole(POCOS.User.Role.Employee, eStoreContext.Current.User.UserID);

            //release session cached instances
            eStoreContext.Current.UserShoppingCart = null;
            eStoreContext.Current.Quotation = null;
            eStoreContext.Current.Order = null;

            Response.Redirect("~/");
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/IotSearch.aspx?skey=" + Request["storekeyworddispay"]);
        }


        /// <summary>
        /// if below problem occured, ViewChanged will be triggered
        /// 
        /// In short, the User object is set earlier in the ASP.NET pipeline, 
        /// long before the requested ASP.NET page's code is executed. Now,
        /// on the subsequent visit, the ASP.NET runtime will see the forms authentication ticket and User.Identity.IsAuthenticated will be true,
        /// but not on this request. 
        /// </summary>
        private bool NoneedRebindLoginView = true;


        protected void HeadLoginView_ViewChanged(object sender, EventArgs e)
        {
            try
            {
                if (!NoneedRebindLoginView)
                    BindData();
            }
            catch (Exception)
            {

            }
        }

        protected void bindFont()
        {
            btSearch.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search);
        }
    }
}