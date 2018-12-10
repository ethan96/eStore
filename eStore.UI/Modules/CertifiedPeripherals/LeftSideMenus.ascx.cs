using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class LeftSideMenus : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
                rpPeripherals.DataSource = pstore.getTopLevelPStoreCategory();
                rpPeripherals.DataBind();
            }
        }
    }
}