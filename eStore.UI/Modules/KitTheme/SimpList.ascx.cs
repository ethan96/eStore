using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.KitTheme
{
    public partial class SimpList : BaseTheme
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CmsModels != null)
            {
                ltTitle.Text = Title;
                rpCmsls.DataSource = CmsModels;
                rpCmsls.DataBind();
            }
        }
    }
}