using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class Events : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bindEvents();
            string Name = "Events";
            this.isExistsPageMeta = setPageMeta(
               $"{Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");

        }

        protected void bindEvents()
        {
            gvEvents.DataSource = eStore.Presentation.eStoreContext.Current.Store.getEvents();
            gvEvents.DataBind();
        }

        protected void gvEvents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#FFF0F0'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='White'");
            }
            
        }
    }
}