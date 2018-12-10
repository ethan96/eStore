using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.KitTheme
{
    public partial class TableList : BaseTheme
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

        protected void rpCmsls_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                BusinessModules.AdvantechCmsModel item = e.Item.DataItem as BusinessModules.AdvantechCmsModel;
                Literal ltContext = e.Item.FindControl("ltContext") as Literal;
                if (item.Abstract.Length > 100)
                    ltContext.Text = $"<div class=\"moreBlock\"><span class=\"pointMore\">...</span><a href=\"/CMS/CmsDetail.aspx?CMSID={item.CmsID}\" target=\"_blank\" class=\"moreBtn\">>>More</a></div><span class=\"txt\">{item.Abstract}<a href=\"#\" class=\"closeBtn\">&gt;&gt;Close</a></span>";
                else
                    ltContext.Text = $"<span class=\"txt\">{item.Abstract}</span>";
            }
        }
    }
}