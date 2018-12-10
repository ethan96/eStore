using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class Transportation_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Transportation");
            base.setPageMeta("Transportation systems", "Transportation systems aim at providing communications and technology that allow safe, convenient, comfortable, efficient, and environmentally friendly travel for all commuters. Many cities worldwide are in the midst of improving their transportation system infrastructures, and Advantech is there to provide a helping hand, offering advanced product solutions for the transportation market. These include not only intelligent infrastructures, but also intelligent vehicles, and marine & railway management platforms.", "Transportation ");

        }
    }
}