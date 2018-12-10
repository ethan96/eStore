using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using System.Web.Security;
using System.Security.Principal;

namespace eStore.UI.Modules.V4
{    public partial class Header : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {

        public string ChatStr {
            get {
                return eStore.Presentation.eStoreContext.Current.GetTopChat();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            bindLangue();
            Control menu = Page.LoadControl($"~/Modules/V4/{ eStore.Presentation.eStoreContext.Current.Store.HeaderMenu }.ascx");
            phmenu.Controls.Add(menu);
            
        }

        //protected string MemberBadge {
        //    get {
        //        if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.actingUser.userGradeX != null)
        //        {
        //            return $" MemberBadge_{Presentation.eStoreContext.Current.User.actingUser.userGradeX.Grade}";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //}
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                logoImage.ImageUrl = Presentation.eStoreContext.Current.getStringSetting("StoreLogoUrl", "/images/eStore_logo_top.png");
                logoImage.Attributes.Add("data-logoPath", logoImage.ImageUrl);
                logoImage.Attributes.Add("data-slogoPath",
                    Presentation.eStoreContext.Current.getStringSetting("StoreLogoSUrl", "/images/eStore_logoS.png"));

                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("ShowTel", true))
                {
                    POCOS.Address storeAddress = Presentation.eStoreContext.Current.CurrentAddress;
                    
                    if (storeAddress != null)
                    {
                        if (string.IsNullOrEmpty(lStoreContact.Text))
                        {
                            lStoreContact.Text = string.Format("<li><b id='top_phonenumber'>{0}</b></li>"
                            , string.IsNullOrEmpty(storeAddress.Tel) ? string.Empty : storeAddress.Tel
                            , string.IsNullOrEmpty(storeAddress.ServiceTime) ? string.Empty : storeAddress.ServiceTime);
                        }

                        //lContactMobile.Text = string.Format("     <h3>{0}</h3><h4>{1}</h4>"
                        //                 , string.IsNullOrEmpty(storeAddress.Tel) ? string.Empty : storeAddress.Tel
                        //                 , string.IsNullOrEmpty(storeAddress.ServiceTime) ? string.Empty : storeAddress.ServiceTime);


                        //lContactMobile.Visible = true;
                        lStoreContact.Visible = true;
                    }
                    else
                    {
                        lStoreContact.Text = string.Empty;
                        //lContactMobile.Text = string.Empty;
                        //lContactMobile.Visible = false;
                        lStoreContact.Visible = false;
                    }
                }
                else if (eStore.Presentation.eStoreContext.Current.Store.storeID.Equals("EMT"))
                {
                    lStoreContact.Visible = true;
                    lStoreContact.Text = string.Format("<li><a href=\"mailto:{0}\">{0}</a></li>", "inquiry.intercon@advantech.com.tw");
                }
                

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
                Literal lUserName = (Literal)this.HeadLoginView.FindControl("lUserName");
                if (lUserName == null)
                    return false;
                lUserName.Text = eStore.Presentation.eStoreContext.Current.Store.FormatHonorific(eStoreContext.Current.Store.getCultureFullName(eStoreContext.Current.User.FirstName, eStoreContext.Current.User.LastName)
                    , eStore.Presentation.eStoreContext.Current.getStringSetting("Honorific"));
                LoginStatus ls = (LoginStatus)this.HeadLoginView.FindControl("HeadLoginStatus");
                ls.LoginText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_In);
                ls.LogoutText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_Out);
                
                if (Presentation.eStoreContext.Current.getBooleanSetting("iAblePointClub", false) == true)
                {
                    Literal ltiAblePoint = (Literal)this.HeadLoginView.FindControl("ltiAblePoint");
                    string iAblePoint = Presentation.eStoreContext.Current.getStringSetting("iAblePoint");
                    if (!string.IsNullOrEmpty(iAblePoint))
                        ltiAblePoint.Text = string.Format(iAblePoint, Presentation.eStoreContext.Current.User.IABLEpoint);
                }

                Literal lCartItemCount = (Literal)this.HeadLoginView.FindControl("lCartItemCount");
                lCartItemCount.Text = Presentation.eStoreContext.Current.UserShoppingCart.cartItemsX.Count.ToString();
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
                    if(ddlEmployeeRoles != null)
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

        void bindLangue()
        {
            if (eStore.Presentation.eStoreContext.Current.Store.profile.StoreLanguages.Any())
            {

                List<POCOS.StoreLanguage> list = new List<POCOS.StoreLanguage>();
                list.Add(new POCOS.StoreLanguage() { Language = eStore.Presentation.eStoreContext.Current.Store.profile.defaultlanguage });
                list.AddRange(eStore.Presentation.eStoreContext.Current.Store.profile.StoreLanguages.Where(c => c.IsActive.GetValueOrDefault()));
                var data = list.Select(x => new { Name = x.Language.Name, Language =x.Language.Code});
                ddlLanguages.DataTextField = "Name";
                ddlLanguages.DataValueField = "Language";
                ddlLanguages.DataSource = data;
                ddlLanguages.DataBind();
                ddlLanguages.SelectedValue = eStoreContext.Current.CurrentLanguage == null ?
                    eStore.Presentation.eStoreContext.Current.Store.profile.defaultlanguage.Code : eStoreContext.Current.CurrentLanguage.Code;
            }
            else
            {
                ddlLanguages.Visible = false;
            }
        }
        protected void ddlLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var slang = eStore.Presentation.eStoreContext.Current.Store.profile.StoreLanguages.FirstOrDefault(c => c.Language.Code.Equals(ddlLanguages.SelectedValue));
            eStoreContext.Current.CurrentLanguage = slang == null ? null : slang.Language;
            Response.Redirect(Request.RawUrl);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(Presentation.eStoreContext.Current.SearchConfiguration.ResultPageUrl + "?skey=" + Request["storekeyworddispay"]);
        }

        public string Tel
        {
            get
            {
                return lStoreContact.Text;
            }
            set
            {
                lStoreContact.Text = value;
            }
        }
    }
}