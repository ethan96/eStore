using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.ECO
{
    public partial class FindPartner : Presentation.eStoreBaseControls.eStoreBasePage
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
            ECOPartnerList1.DataSource = getECOPartnerSource();
            if (!IsPostBack)
            {
                bindBaseInfor();
            }
            BindScript("url", "EOCutlity", "EOCutlity.js");
        }

        protected void bindBaseInfor()
        {
            ddlEOCSpecialty.Items.Clear();
            ddlEOCSpecialty.Items.Add(new ListItem("[Select Specialty]", ""));
            foreach (ECOSpecialty ecop in eStore.Presentation.eStoreContext.Current.Store.getAllECOSpecialty())
            {
                ListItem li = new ListItem() { Text = ecop.SpecialtyName, Value = ecop.SpecialtyName };
                ddlEOCSpecialty.Items.Add(li);
            }

            ddlECOIndustry.Items.Clear();
            ddlECOIndustry.Items.Add(new ListItem("[Select Industry]", ""));
            foreach (ECOIndustry ecoin in eStore.Presentation.eStoreContext.Current.Store.getAllECOIndustry())
            {
                ListItem li = new ListItem() { Text = ecoin.IndustryName, Value = ecoin.IndustryName };
                ddlECOIndustry.Items.Add(li);
            }
        }

        protected void btEcoSearch_Click(object sender, EventArgs e)
        {

        }

        private List<ECOPartner> getECOPartnerSource()
        {
            List<ECOPartner> ls = new List<ECOPartner>();
            if(string.IsNullOrEmpty(ddlEOCSpecialty.SelectedValue) && string.IsNullOrEmpty(CountrySelector1.Country) && string.IsNullOrEmpty(ddlECOIndustry.SelectedValue))
                ls = eStore.Presentation.eStoreContext.Current.Store.getAllECOPartner();
            else
                ls = eStore.Presentation.eStoreContext.Current.Store.getECOPartnerByBaseInfor(
                            new List<string>() { ddlEOCSpecialty.SelectedValue }, CountrySelector1.Country, CountrySelector1.State, ddlECOIndustry.SelectedValue);
            return ls;
        }

    }
}