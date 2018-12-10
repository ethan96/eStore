using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Environment_Facility_mgt_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Environmental & Facility");
            base.setPageMeta("Environmental & Facility", "Advantech is a proven leader in Environmental Monitoring applications (EMS) and Facility Management Systems (FMS). Now, Advantech’s EFMS combines both to provide value-added systems and solutions through high-volume SCADA and advanced web-based technology. This process now allows users to monitor and operate processes online anytime and any location.", "Environmental,Facility");

        }
    }
}