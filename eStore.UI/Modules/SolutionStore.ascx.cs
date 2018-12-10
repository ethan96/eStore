using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class SolutionStore : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            this.rpSolutionStoreTab.DataSource = Presentation.eStoreContext.Current.Store.profile.SolutionStoreTabs;
            this.rpSolutionStoreTab.DataBind();
            this.rpSolutionStoreItems.DataSource = Presentation.eStoreContext.Current.Store.profile.SolutionStoreTabs;
            this.rpSolutionStoreItems.DataBind();
        }
    }
}