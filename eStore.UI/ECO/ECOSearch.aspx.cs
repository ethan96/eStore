using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI
{
    public partial class ECOSearch : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            AddStyleSheet(ResolveUrl("~/Styles/iServicesHomepage.css"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bindResult();
        }

        private void bindResult()
        {
            var ls = eStore.Presentation.eStoreContext.Current.Store.getECOPartnerByBaseInfor(
                ECOPartnerSearch1.InFor.Specialties, ECOPartnerSearch1.InFor.Country, ECOPartnerSearch1.InFor.State, ECOPartnerSearch1.InFor.Industry);
            ECOPartnerList1.DataSource = ls;
            ECOPartnerList1.SearchInfor = ECOPartnerSearch1.InFor;
        }


        
    }
}