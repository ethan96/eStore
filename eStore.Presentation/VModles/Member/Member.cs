using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.VModles.Member
{
    public class Member
    {
        //User email
        public string UserId { get; set; }
        
        //用户密码
        public string PassWord { get; set; }
        
        //ip地址
        public string Ip { get; set; }
        
        public int TimezoneOffset { get; set; }
        
        //是否保持登录状态
        public bool RememberMeSet { get; set; }
    }
}
