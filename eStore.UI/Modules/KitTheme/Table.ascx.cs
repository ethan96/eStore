using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.KitTheme
{
    public partial class Table : BaseTheme
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CmsModels != null)
            {
                rpfilter.DataSource = Tags.Select(c => new { id = c, name = c }).OrderBy(c=>c.name).ToList();
                rpfilter.DataBind();

                rpcmsls.DataSource = CmsModels;
                rpcmsls.DataBind();
            }
        }
    }
}