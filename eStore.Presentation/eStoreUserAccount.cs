using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace eStore.Presentation
{
    public class eStoreUserAccount
    {
        public static bool TrySignIn(Presentation.VModles.Member.VLoginDialog vlogdialog)
        {
            POCOS.User user = vlogdialog.TryLogin();
            if (user != null)
            {
                string[] roles = new string[1];
                if (user.hasRight(POCOS.User.ACToken.SwitchRole))
                { roles[0] = "Employee"; }
                else
                { roles[0] = "Customer"; }

                string userdata = string.Format("password;{0};{1};{2}", vlogdialog.member.PassWord, roles[0], vlogdialog.member.TimezoneOffset.ToString());

                //HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
                FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, vlogdialog.member.UserId,
                    DateTime.Now, DateTime.Now.AddMonths(3),
                    vlogdialog.member.RememberMeSet, userdata,
                    FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(t);
                HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                if (vlogdialog.member.RememberMeSet)
                {
                    HttpContext.Current.Response.Cookies.Clear();
                    c.Expires = t.Expiration;
                }
                HttpContext.Current.Response.Cookies.Add(c);
                if (HttpContext.Current.Request.Cookies["WMX_Account"] == null)
                {
                    HttpCookie migoCookie = new HttpCookie("WMX_Account", user.UserID);
                    migoCookie.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Current.Response.Cookies.Add(migoCookie);
                }

                FormsIdentity id = new FormsIdentity(t);
                GenericPrincipal principal = new GenericPrincipal(id, roles);
                HttpContext.Current.User = principal;//存到HttpContext.User中
                HttpContext.Current.Session.Clear();
                if (user.hasRight(POCOS.User.ACToken.SwitchRole))
                    user.switchRole(POCOS.User.Role.Employee);
                TimeSpan ts = new TimeSpan(vlogdialog.member.TimezoneOffset / 60, vlogdialog.member.TimezoneOffset % 60, 0);
                user.timeSpan = ts;
                user.UserLanguages = HttpContext.Current.Request.UserLanguages;
                Presentation.eStoreContext.Current.User = user;
                if(user.newUser)
                    Presentation.eStoreContext.Current.AffiliateTracking(Presentation.eStoreContext.Current.User);

                //
                if (Presentation.eStoreContext.Current.getBooleanSetting("iAblePointClub", false) == true)
                    Presentation.eStoreContext.Current.Store.GetiAblePoint(Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.User.authKey);

                return true;
            }
            else
                return false;
        }
        public static bool TrySignInFromCookies(HttpCookie authCookie, string ip)
        {
            POCOS.User user = null;
            FormsAuthenticationTicket authTicket = null;
            if (authCookie == null)
            {
                user = null;
            }
            else
            {
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (Exception)
                { user = null; }
                if (authTicket == null)
                {
                    user = null;
                }
                else
                {
                    try
                    {
                        string[] loginInfo = authTicket.UserData.Split(';');
                        string logintype = loginInfo[0];
                        string userrole = loginInfo[2];
                        VModles.Member.VLoginDialog logindialog = new VModles.Member.VLoginDialog(authTicket.Name, loginInfo[1], ip);
                        logindialog.member.TimezoneOffset = int.Parse(loginInfo[3]);
                        if (logintype == "password" || logintype == "sso")
                        {
                            logindialog.SetLoginType(loginInfo[0]); // sso or password
                            user = logindialog.TryLogin();
                        }
                        else
                        {
                            authCookie.Expires = DateTime.Now.AddDays(-30);
                            HttpContext.Current.Response.Cookies.Add(authCookie);
                            FormsAuthentication.SignOut();
                            user = null;
                        }

                        user.switchRole((POCOS.User.Role)Enum.Parse(typeof(POCOS.User.Role), userrole));

                        TimeSpan ts = new TimeSpan(logindialog.member.TimezoneOffset / 60, logindialog.member.TimezoneOffset % 60, 0);
                        user.timeSpan = ts;
                        user.UserLanguages = HttpContext.Current.Request.UserLanguages;
                        if (HttpContext.Current.Request.Cookies["WMX_Account"] == null)
                        {
                            HttpCookie migoCookie = new HttpCookie("WMX_Account", user.UserID);
                            migoCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Current.Response.Cookies.Add(migoCookie);
                        }
                    }
                    catch (Exception)
                    {
                        authCookie.Expires = DateTime.Now.AddDays(-30);
                        HttpContext.Current.Response.Cookies.Add(authCookie);
                        FormsAuthentication.SignOut();
                        user = null;
                    }
                }
            }
            Presentation.eStoreContext.Current.User = user;

            if (user != null)
            {
                if (user.newUser)
                    Presentation.eStoreContext.Current.AffiliateTracking(Presentation.eStoreContext.Current.User);
                return true;
            }
            else
                return false;
        }
        public static bool TrySSOSignIn(String userHostIP, String authKey, String userId)
        {
            VModles.Member.VLoginDialog logindialog = new VModles.Member.VLoginDialog(userId, authKey, userHostIP);
            logindialog.SetLoginType(VModles.Member.VLoginDialog.LogInType.SSO);
            POCOS.User user = logindialog.TryLogin();
            if (user != null)
            {
                string[] roles = new string[1];
                if (user.hasRight(POCOS.User.ACToken.SwitchRole))
                { roles[0] = "Employee"; }
                else
                { roles[0] = "Customer"; }
                string userdata = string.Format("sso;{0};{1};0", authKey,roles[0]);

                //HttpContext.Current.Session.Abandon();
                HttpContext.Current.Session.Clear();
                FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, user.UserID,
                    DateTime.Now, DateTime.Now.AddMonths(3),
                    false, userdata,
                    FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(t);

                HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                HttpContext.Current.Response.Cookies.Add(c);

                if (HttpContext.Current.Request.Cookies["WMX_Account"] == null)
                {
                    HttpCookie migoCookie = new HttpCookie("WMX_Account", user.UserID);
                    migoCookie.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Current.Response.Cookies.Add(migoCookie);
                }

                FormsIdentity id = new FormsIdentity(t);
                GenericPrincipal principal = new GenericPrincipal(id, roles);
                HttpContext.Current.User = principal;//存到HttpContext.User中
                HttpContext.Current.Session.Clear();
                if (user.hasRight(POCOS.User.ACToken.SwitchRole))
                    user.switchRole(POCOS.User.Role.Employee);
                Presentation.eStoreContext.Current.User = user;
                if (user.newUser)
                    Presentation.eStoreContext.Current.AffiliateTracking(Presentation.eStoreContext.Current.User);
                return true;
            }
            else
                return false;
        }
        public static bool RegisterUser(VModles.Member.VRegiesterUser vuser)
        {
            var user = vuser.GetUser();
            bool result = (user.save() == 0);
            if (result)
            {
                esUtilities.EMailReponse response = null;
                eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                response = mailTemplate.SendRegisterEmail(user
                    , eStore.Presentation.eStoreLocalization.Tanslation("eStore_Register_Subjec")
                    , eStore.Presentation.eStoreContext.Current.Store
                    , eStore.Presentation.eStoreContext.Current.CurrentLanguage
                    , eStore.Presentation.eStoreContext.Current.MiniSite);
                if (response != null && response.ErrCode == esUtilities.EMailReponse.ErrorCode.NoError)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Register_Success"));
                    //Response.Redirect("/");
                }
                else
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Can_not_send_email_to"), user.UserID));
                }
            }
            return result;
        }

        public static bool UpdateUser(VModles.Member.VRegiesterUser vuser, POCOS.User currentUser)
        {
            var user = vuser.GetUser(currentUser);
            return user.save() == 0;
        }

    }
}
