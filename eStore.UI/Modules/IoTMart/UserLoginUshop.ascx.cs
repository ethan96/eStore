using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;

namespace eStore.UI.Modules.IoTMart
{
    public partial class UserLoginUshop : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Button btLogin = LoginUser.FindControl("LoginButton") as Button;
            btLogin.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_In);
            RegularExpressionValidator UserNameRequired = LoginUser.FindControl("RegularUserNameEmail") as RegularExpressionValidator;
            UserNameRequired.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Customer_email);
            UserNameRequired.ErrorMessage = "请输入邮箱地址";
            RequiredFieldValidator PasswordRequired = LoginUser.FindControl("PasswordRequired") as RequiredFieldValidator;
            PasswordRequired.ErrorMessage = PasswordRequired.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Password_is_required);
            Label lbShow = LoginUser.FindControl("lbShow") as Label;
            lbShow.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_For_your_best_experience);
            if (Request["needlogin"] != null)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "showlogindialog", "$(document).ready(function(){$('#ushoplogin').click();});", true);
            }

        }
        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            string username = ((System.Web.UI.WebControls.Login)sender).UserName; // 取登录名
            string password = ((System.Web.UI.WebControls.Login)sender).Password; //取登录密码
            string userHostAddress = Presentation.eStoreContext.Current.getUserIP();
            int timezoneOffset = 0;
            int.TryParse(Request["timezoneOffset"] ?? "0", out timezoneOffset);
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }
            string seletedcountry = Presentation.eStoreContext.Current.CurrentCountry.CountryName;

            Presentation.VModles.Member.VLoginDialog LogInfor = new Presentation.VModles.Member.VLoginDialog
            {
                member = new Presentation.VModles.Member.Member
                {
                    UserId = username,
                    PassWord = password,
                    Ip = userHostAddress,
                    TimezoneOffset = timezoneOffset,
                    RememberMeSet = this.LoginUser.RememberMeSet
                },
                logInfor = Presentation.eStoreIoc.Resolve<Presentation.VModles.Member.LogInfor>()
            };
            if (Presentation.eStoreUserAccount.TrySignIn(LogInfor)) // 尝试登录
            {
                if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
                    Presentation.eStoreContext.Current.Store.savePopularModelLogByLogin(Session.SessionID, eStore.Presentation.eStoreContext.Current.User.UserID);

                if (continueSenderEvent() == false)
                {
                    string returnUrl = HttpUtility.UrlDecode(Request.QueryString["returnUrl"]);
                    if (string.IsNullOrEmpty(returnUrl))
                        returnUrl = CurrentPagePath;
                    if (returnUrl.Contains("?"))
                        Response.Redirect(returnUrl + "&country=" + seletedcountry);
                    else
                        Response.Redirect(returnUrl + "?country=" + seletedcountry);
                }
            }
            else if (Request["needlogin"] == null)
            {
                popLoginDialog(sender);
            }

        }

        private bool continueSenderEvent()
        {
            string senderID = Request.Form["loginTrigger"];
            bool result = false;
            //find Sender
            if (string.IsNullOrWhiteSpace(senderID) == false)
            {
                if (this.Page is eStoreBasePage)
                {
                    result=((eStoreBasePage)this.Page).Loggedin(senderID);

                }
            }
            return result;
        }

        protected void btsendforgetemail_Click(object sender, EventArgs e)
        {

            string email = txsendforgetemail.Text;
            if (!string.IsNullOrEmpty(email))
            {
                var result = eStore.Presentation.eStoreContext.Current.Store.sso.RequestForgetPassword(email, "ESTORE_CN_USHOP", Presentation.eStoreContext.Current.Store.profile.StoreLangID
                        , eStore.Presentation.eStoreContext.Current.Store.getCurrStoreUrl(Presentation.eStoreContext.Current.Store.profile, Presentation.eStoreContext.Current.MiniSite) + "/accountForm.aspx");
                if (result)
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "showSendSuccessPsw", "$(document).ready(function(){$('#ito_ushop_sendpswSuccess').click();});", true);
                }
            }
        }
        
    }
}