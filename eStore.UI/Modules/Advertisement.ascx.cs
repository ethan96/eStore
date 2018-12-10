using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class Advertisement : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string OnHtmlLoaded { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OnHtmlLoaded))
            { }
        }
    }
}