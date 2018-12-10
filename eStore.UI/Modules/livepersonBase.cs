using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Modules
{
    public abstract class livepersonBase : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool UserLargerImage { get; set; }
        public virtual string livepersonStyle
        {
            get
            {
                if (UserLargerImage)
                    return "eStoreLivePerson-LargerImage";
                else
                    return "eStoreLivePerson-SmallImage";
            }
        }
    }
}