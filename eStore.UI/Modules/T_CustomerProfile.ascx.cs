using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class T_CustomerProfile : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetUserValue();
                bindFonts();
            }

        }

        //Load Current Message
        private void GetUserValue()
        {
            if (Presentation.eStoreContext.Current.User != null)
            {
                this.CultureFullName1.FirstName = eStoreContext.Current.User.FirstName;
                this.CultureFullName1.LastName = eStoreContext.Current.User.LastName;
                txt_email.Text = eStoreContext.Current.User.UserID;
                int phoneNumber = eStoreContext.Current.User.mainContact.TelNo.IndexOf("-");
                if (phoneNumber > 0)
                {
                    txt_Phone1.Text = eStoreContext.Current.User.mainContact.TelNo.Substring(0, phoneNumber);
                    txt_Phone2.Text = eStoreContext.Current.User.mainContact.TelNo.Substring(phoneNumber + 1);
                }
                else
                {
                    txt_Phone1.Text = "";
                    txt_Phone2.Text = eStoreContext.Current.User.mainContact.TelNo;
                }
                txt_Phone_Ext.Text = eStoreContext.Current.User.mainContact.TelExt;
                txt_company.Text = eStoreContext.Current.User.CompanyName;
                txt_department.Text = eStoreContext.Current.User.Department;


                txt_city.Text = eStoreContext.Current.User.mainContact.City;
                txt_zip.Text = eStoreContext.Current.User.mainContact.ZipCode;
                txt_address.Text = Presentation.eStoreContext.Current.User.mainContact.Address1 + "," + Presentation.eStoreContext.Current.User.mainContact.City + ","
                                           + Presentation.eStoreContext.Current.User.mainContact.State;
            }
        }
        //Get Country
        public string GetCountry()
        {
            return CountrySelector1.Country; ;
        }
        //Get Country State
        public string GetCountryState()
        {
            return CountrySelector1.State;
        }

        public string GetZipCode()
        {
            return txt_zip.Text.Trim();
        }

        public string GetCity()
        {
            return txt_city.Text.Trim();
        }

        public POCOS.UserRequest getUserRequest(eStore.POCOS.UserRequest.ReqType reqType = POCOS.UserRequest.ReqType.ContactUs)
        {
            POCOS.UserRequest requestContactUs = new POCOS.UserRequest(Presentation.eStoreContext.Current.Store.profile, reqType);
            requestContactUs.FirstName = this.CultureFullName1.FirstName;
            requestContactUs.LastName = this.CultureFullName1.LastName;
            requestContactUs.Email = string.IsNullOrEmpty(txt_email.Text) ? string.Empty : txt_email.Text;
            requestContactUs.Telephone = string.Format("{0}-{1}-{2}"
                , string.IsNullOrEmpty(txt_Phone1.Text) ? string.Empty : txt_Phone1.Text
                , string.IsNullOrEmpty(txt_Phone2.Text) ? string.Empty : txt_Phone2.Text
                , string.IsNullOrEmpty(txt_Phone_Ext.Text) ? string.Empty : txt_Phone_Ext.Text);

            requestContactUs.Company = string.IsNullOrEmpty(txt_company.Text) ? string.Empty : txt_company.Text;
            requestContactUs.Address = string.Format("{0} {1} {2} {3} {4}"
                , string.IsNullOrEmpty(this.CountrySelector1.Country) ? string.Empty : this.CountrySelector1.Country
                , string.IsNullOrEmpty(txt_department.Text) ? string.Empty : txt_department.Text
                , string.IsNullOrEmpty(txt_zip.Text) ? string.Empty : txt_zip.Text
                , string.IsNullOrEmpty(txt_address.Text) ? string.Empty : txt_address.Text
                , string.IsNullOrEmpty(txt_city.Text) ? string.Empty : txt_city.Text
              );

            requestContactUs.Country = this.CountrySelector1.Country;
            requestContactUs.State = this.CountrySelector1.State;

            return requestContactUs;
        }

        protected void bindFonts()
        {
            lbl_Email.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_eMail);
            lbl_Phone.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Phone);
            lbl_Phone_Ext.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Ext);
            lbl_Company.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Company);
            lbl_Department.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Department);
            lbl_City.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_City);
            lbl_ZipCode.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_ZipCode);
            lbl_Address.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Address);

        }


    }
}