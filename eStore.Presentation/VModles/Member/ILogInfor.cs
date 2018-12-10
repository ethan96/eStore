using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.VModles.Member
{
    public interface ILogInfor
    {
        POCOS.User TryLogin(Member member);

        POCOS.User SSOLogin(Member member);
    }
}
