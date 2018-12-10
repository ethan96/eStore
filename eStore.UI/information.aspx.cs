using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class information : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public string sInformation { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                eStore.BusinessModules.Store.TEMPLATE_TYPE type;
                if (Enum.TryParse(Request["type"], true, out type))
                {
                    sInformation = eStore.Presentation.eStoreContext.Current.Store.getTemplate(type);
                }
                else
                {
                    sInformation = eStore.Presentation.eStoreContext.Current.Store.getTemplateByStr(Request["type"]);
                }
                this.isExistsPageMeta = setPageMeta(
          $"{Request["type"]} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{Request["type"]} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");
            }
        }
    }
}