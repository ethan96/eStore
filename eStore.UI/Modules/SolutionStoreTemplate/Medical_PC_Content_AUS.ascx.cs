using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Medical_PC_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Medical Computing");
            base.setPageMeta("Medical Computing", "Advantech medical platforms match tough industry standards such as UL-60601-1 and EN60601-1 medical safety approvals and IPX1 dust and water drip-proof enclosure certification. With Advantech’s unparalleled support, consider these solutions the front line of today’s medical information systems programs such as EMR and PACs integration.", "Medical Computing");

        }
    }
}