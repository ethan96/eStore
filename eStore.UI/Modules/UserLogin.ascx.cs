using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;

namespace eStore.UI.Modules
{
    public partial class UserLogin : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        //登录页面的初始信息
        private Presentation.VModles.Member.VLoginDialog LogInfor = new Presentation.VModles.Member.VLoginDialog
        {
            logInfor = Presentation.eStoreIoc.Resolve<Presentation.VModles.Member.LogInfor>()
        };
        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox UserName = LoginUser.FindControl("UserName") as TextBox;
            UserName.Attributes.Add("placeholder", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Email_address));
            TextBox Password = LoginUser.FindControl("Password") as TextBox;
            Password.Attributes.Add("placeholder", eStore.Presentation.eStoreLocalization.Tanslation("Password"));
            LinkButton btLogin = LoginUser.FindControl("LoginButton") as LinkButton;
            btLogin.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Log_In);
            RequiredFieldValidator UserNameRequired = LoginUser.FindControl("UserNameRequired") as RequiredFieldValidator;
            UserNameRequired.ErrorMessage = UserNameRequired.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Name_is_required);
            RequiredFieldValidator PasswordRequired = LoginUser.FindControl("PasswordRequired") as RequiredFieldValidator;
            PasswordRequired.ErrorMessage = PasswordRequired.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Password_is_required);
            Literal lbShow = LoginUser.FindControl("lbShow") as Literal;
            lbShow.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_For_your_best_experience);
            Literal llogin = LoginUser.FindControl("llogin") as Literal;
            llogin.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Tilte);
            HyperLink hlforgotpassword = LoginUser.FindControl("hlforgotpassword") as HyperLink;
            hlforgotpassword.NavigateUrl = string.Format(LogInfor.logInfor.ForgetLink
                    , eStore.Presentation.eStoreContext.Current.storeMembershippass
                    , eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Go_eStore)
                    , CurrentUrlEncodePath
                    , eStore.Presentation.eStoreContext.Current.BusinessGroup);
            hlforgotpassword.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Forget_Password);
            Label RememberMeLabel = LoginUser.FindControl("RememberMeLabel") as Label;
            RememberMeLabel.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Keep_me_logged_in);
            HyperLink hlSignup = LoginUser.FindControl("hlSignup") as HyperLink;
            hlSignup.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Register_Now);
            hlSignup.NavigateUrl = string.Format(LogInfor.logInfor.RegisterLink
                    , eStore.Presentation.eStoreContext.Current.storeMembershippass
                    , eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_User_Login_Go_eStore)
                    , CurrentUrlEncodePath
                    , eStore.Presentation.eStoreContext.Current.BusinessGroup
                    );
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
            LogInfor.member = new Presentation.VModles.Member.Member
            {
                UserId = username,
                PassWord = password,
                Ip = userHostAddress,
                TimezoneOffset = timezoneOffset,
                RememberMeSet = this.LoginUser.RememberMeSet
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
                    result = ((eStoreBasePage)this.Page).Loggedin(senderID);

                }
            }
            return result;
        }
    }
}