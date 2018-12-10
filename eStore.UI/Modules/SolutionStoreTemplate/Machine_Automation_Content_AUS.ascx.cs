using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Machine_Automation_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Machine Automation");
            base.setPageMeta("Machine Automation", "Advantech offers flexible PAC solutions based on IEC 61131-3 SoftLogic, PLCopen motion control function blocks and remote diagnostic utilities, providing solutions in both centralized and decentralized motion control architectures. These solutions are aimed at satisfying the demanding requirements of various industrial applications, including solar cell manufacturing, electronics manufacturing and packaging. Advantech’s new PAC-based solutions provide integrated operation, runtime and downtime information into MES/CIM systems to improve the production yield rate and optimized usage of all the facilities. ", "Machine Automation");

        }
    }
}