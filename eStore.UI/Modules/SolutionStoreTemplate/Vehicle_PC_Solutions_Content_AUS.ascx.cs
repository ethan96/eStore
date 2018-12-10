using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Vehicle_PC_Solutions_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Vehicle Computing");
            base.setPageMeta("Vehicle Computing", "Advantech Vehicle & Fleet Management solutions provide industrial-grade vehicle mounted computers, portable computers, and Automatic Vehicle Locator modules, whether your needs are for fleet management, law enforcement, or logistics & warehousing applications."
                , "Vehicle Computing ");

        }
    }
}