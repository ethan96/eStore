using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class CartContactTemplate : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.CartContact cartContact
        {
            set
            {
                if (value == null)
                {
                    this.Visible = false;
                    return;
                }
                this.lAttention.Text = Presentation.eStoreContext.Current.Store.getCultureFullName(value.FirstName, value.LastName);
                this.lTelNo.Text = (string.IsNullOrEmpty(value.TelNo) ? string.Empty : value.TelNo) + (string.IsNullOrEmpty(value.TelExt) ? string.Empty : "-" + value.TelExt);
                this.lAddress1.Text = string.IsNullOrEmpty(value.Address1) ? string.Empty : value.Address1;
                this.lAddress2.Text = string.IsNullOrEmpty(value.Address2) ? string.Empty : string.Format("<p><label>{0}</label></p>", value.Address2);
                this.lState.Text = string.IsNullOrEmpty(value.stateNameX) ? string.Empty : value.stateNameX;
                this.lCity.Text = string.IsNullOrEmpty(value.City) ? string.Empty : value.City;
                this.lCountry.Text = string.IsNullOrEmpty(value.Country) ? string.Empty : value.Country;
                this.lZipCode.Text = string.IsNullOrEmpty(value.ZipCode) ? string.Empty : value.ZipCode;
                this.ltCompany.Text = string.IsNullOrEmpty(value.AttCompanyName) ? string.Empty : value.AttCompanyName;
                if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
                {
                    this.ltLegalForm.Text = string.IsNullOrEmpty(value.LegalForm) ? string.Empty : value.LegalForm;
                    this.pLegalForm.Visible = true;
                    this.lvat.Text = string.IsNullOrEmpty(value.VATNumbe) ? string.Empty : value.VATNumbe;
                    this.pvat.Visible = true;
                }
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}