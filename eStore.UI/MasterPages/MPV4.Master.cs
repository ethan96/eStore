using eStore.Presentation.eStoreBaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.MasterPages
{
    public partial class MPV4 : eStoreBaseMasterPage
    {
        public string Tel
        {
            get
            {
                return Header1.Tel;
            }
            set
            {
                Header1.Tel = value;
            }
        }
    }
}