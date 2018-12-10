using eStore.POCOS;

namespace eStore.Presentation.VModles.Member
{
    public class eStoreLogInfor : LogInfor, ILogInfor
    {
        public eStoreLogInfor()
        {
            this.ForgetLink = "/Account/Forgetpassword.aspx";
            this.EditLink = "/Account/MyRegister.aspx?edit=";
            this.RegisterLink = "/Account/MyRegister.aspx";
        }

        public User SSOLogin(Member member)
        {
            return TryLogin(member);
        }

        public User TryLogin(Member member)
        {
            var user = Presentation.eStoreContext.Current.Store.getUser(member.UserId);
            if (user != null)
            {
                if (user.LoginPassword != esUtilities.StringUtility.StringEncry(member.PassWord))
                    user = null;
                else
                    user.authKey = user.LoginPassword;
            }
            return user;
        }
    }
}
