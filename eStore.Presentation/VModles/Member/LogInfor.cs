using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.VModles.Member
{
    public abstract class LogInfor
    {
        //忘记密码链接地址
        public string ForgetLink { get; set; }
        
        //注册地址
        public string RegisterLink { get; set; }

        //编辑地址
        public string EditLink { get; set; }
    }
}
