using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Signage_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Digital Signage");
            base.setPageMeta("Digital Signage", "These network-manageable players support Window®-based digital signage software and offer top performance and  flexibility. ", "Digital Signage");

        }
    }
}