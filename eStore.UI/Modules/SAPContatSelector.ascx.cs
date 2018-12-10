using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class SAPContatSelector : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public POCOS.VSAPCompany  getSAPCompany()
        {
            string SAPCompanyID = this.Request["storeSAPCompanies"];
            if (!string.IsNullOrEmpty(SAPCompanyID))
            {
                return Presentation.eStoreContext.Current.Store.findSAPContact(SAPCompanyID);
            }
            return null;
        }
    }
}