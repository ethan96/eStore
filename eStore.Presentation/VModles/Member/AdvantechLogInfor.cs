using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;

namespace eStore.Presentation.VModles.Member
{
    public class AdvantechLogInfor : LogInfor, ILogInfor
    {
        public AdvantechLogInfor()
        {
            this.ForgetLink = "https://member.advantech.com/forgetpassword.aspx?pass={0}&lang={1}&CallBackURLName={2}&CallBackURL={3}&group={4}&RegPurpose=";
            this.EditLink = "https://member.advantech.com/profile.aspx?pass={0}&id={1}&lang={2}&tempid={3}&CallBackURLName={4}&callbackurl={5}?sso=y&group={6}";
            this.RegisterLink = "https://member.advantech.com/profile.aspx?pass={0}&lang={1}&CallBackURLName={2}&CallBackURL={3}&group={4}";
        }

        public User SSOLogin(Member member)
        {
            return eStoreContext.Current.Store.ssoLogin(member.Ip, member.PassWord, member.UserId);
        }

        public User TryLogin(Member member)
        {
            return eStoreContext.Current.Store.login(member.UserId, member.PassWord, member.Ip);
        }
    }
}
