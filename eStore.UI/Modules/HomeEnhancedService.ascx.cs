using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class HomeEnhancedService : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var themebanners = eStore.Presentation.eStoreContext.Current.Store.GetKitThemeLs();
            rpThemeBanners.DataSource = themebanners;
            rpThemeBanners.DataBind();
            rpThemeBanners.Visible = themebanners.Any();
        }
    }
}