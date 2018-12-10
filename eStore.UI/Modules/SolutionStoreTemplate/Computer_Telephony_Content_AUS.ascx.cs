using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Computer_Telephony_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Computer Telephony Integration");
            base.setPageMeta("Computer Telephony Integration Solutions", "Add industrial-grade reliability to your CTI network with Advantech¡¯s new generation of intelligent PBX flexible and reliable CTI solutions in a cost effective package, and come optimized to your specific application below.", "Computer Telephony Integration");
        }
    }
}