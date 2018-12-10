using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.IoTMart
{
    public partial class Footer : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindFooter();
            }
        }
        protected void bindFooter()
        {
            List<POCOS.Menu> ls = Presentation.eStoreContext.Current.Store.getFooterLinks(Presentation.eStoreContext.Current.MiniSite).Take(4).ToList();
            rpMainFooter.DataSource = ls;
            rpMainFooter.DataBind();
        }
    }
}