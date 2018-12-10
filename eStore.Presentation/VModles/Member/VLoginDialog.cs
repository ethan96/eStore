using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.VModles.Member
{
    public class VLoginDialog
    {
        public enum LogInType { SSO, PSD }

        private LogInType loginType = LogInType.PSD;

        public LogInfor logInfor { get; set; }

        public Member member { get; set; }

        public VLoginDialog()
        { }

        public VLoginDialog(string userid, string password, string ip)
        {
            logInfor = eStoreIoc.Resolve<VModles.Member.LogInfor>();
            member = new Member
            {
                UserId = userid,
                PassWord = password,
                Ip = ip
            };
        }

        public POCOS.User TryLogin()
        {
            POCOS.User user = null;
            var iinfor = ((ILogInfor)logInfor);
            switch (loginType)
            {
                case LogInType.SSO:
                    user = iinfor.SSOLogin(member);
                    break;
                case LogInType.PSD:
                default:
                    user = iinfor.TryLogin(member);
                    break;

            }
            return user;
        }

        public void SetLoginType(LogInType type)
        {
            loginType = type;
        }
        public void SetLoginType(string type)
        {
            switch (type.ToLower())
            {
                case "sso":
                    loginType = LogInType.SSO;
                    break;
                default:
                    loginType = LogInType.PSD;
                    break;
            }
        }
    }
}
