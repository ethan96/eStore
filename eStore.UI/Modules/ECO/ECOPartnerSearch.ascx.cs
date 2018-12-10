using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules.ECO
{
    public partial class ECOPartnerSearch : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private bool _isShowECOSearchInfor = true;
        public bool IsShowECOSearchInfor
        {
            get { return _isShowECOSearchInfor; }
            set { _isShowECOSearchInfor = value; }
        }

        private bool _isShowECOInterestedPartner = false;

        public bool IsShowECOInterestedPartner
        {
            get { return _isShowECOInterestedPartner; }
            set { _isShowECOInterestedPartner = value; }
        }

        


        private EcoInfor _inFor;
        public EcoInfor InFor
        {
            get 
            {
                if (_inFor == null)
                {
                    var _from = Request.Form;
                    var countryCode = _from.AllKeys.FirstOrDefault(c => c.EndsWith("ECOPartnerSearch1$CountrySelector1$ddl_Country"));
                    var stateCode = _from.AllKeys.FirstOrDefault(c => c.EndsWith("ECOPartnerSearch1$CountrySelector1$ddl_State"));
                    var eOCSpecialtyName = _from.AllKeys.FirstOrDefault(c => c.EndsWith("ECOPartnerSearch1$ddlEOCSpecialty"));
                    var eCOIndustryName = _from.AllKeys.FirstOrDefault(c => c.EndsWith("ECOPartnerSearch1$ddlECOIndustry"));
                    CountrySelector1.BindCountry();
                    CountrySelector1.State = Request[stateCode];
                    CountrySelector1.CountryCode = Request[countryCode];
                    _inFor = new EcoInfor();
                    _inFor.Country = CountrySelector1.Country ?? "";
                    _inFor.State = Request[stateCode] ?? "";
                    _inFor.StateName = CountrySelector1.StateName;
                    _inFor.Industry = Request[eCOIndustryName] ?? "";
                    _inFor.Specialties.Add(Request[eOCSpecialtyName] ?? "");
                }
                return _inFor; 
            }
        }

        public event EventHandler eSearch;

        protected override void OnInit(EventArgs e)
        {
            //regetInfor();
            this.Visible = Presentation.eStoreContext.Current.getBooleanSetting("EnableECOSystem",false);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pecoSearchInfor.Visible = IsShowECOSearchInfor;
            if (eSearch != null)
                btEcoSearch.Click += eSearch;
            BindScript("url", "EOCutlity", "EOCutlity.js");
            if (!IsPostBack)
            {
                bindFonts();
                bindInInfor();
                if (eStore.Presentation.eStoreContext.Current.User != null)
                {
                    var user = eStore.Presentation.eStoreContext.Current.User;
                    txtPa_FirstName.Text = user.FirstName;
                    txtPa_LastName.Text = user.LastName;
                    txtPa_Email.Text = user.UserID;
                    txtPa_Tel.Text = user.TelNo;
                    txtPa_Company.Text = user.CompanyName;
                    if (user.mainContact != null)
                    {
                        var contact = user.mainContact;
                        txtPa_Address.Text = contact.Address1 + " " + contact.Address2;
                        txtPa_Zip.Text = contact.ZipCode;
                        txtPa_Tel.Text = contact.TelNo;
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_inFor == null)
                bindInfor();
            else
                bindInfor(_inFor);
            base.OnPreRender(e);
        }

        protected void btIn_Send_Click(object sender, EventArgs e)
        {
            POCOS.ECOPartner partner = new POCOS.ECOPartner()
            {
                Address = txtPa_Address.Text.Trim(),
                CompanyName = txtPa_Company.Text.Trim(),
                ContactTime = ServerTime1.StartDate + " - " + ServerTime1.EndDate,
                Description = txtPa_Des.Text.Trim(),
                Email = txtPa_Email.Text.Trim(),
                FirstName = txtPa_FirstName.Text.Trim(),
                LastName = txtPa_LastName.Text.Trim(),
                Status = "ApplyFor",
                TelNo = txtPa_Tel.Text.Trim(),
                WebSiteUrl = txtPa_WebSite.Text.Trim(),
                ZipCode = txtPa_Zip.Text.Trim(),
                CreatedDate = DateTime.Now,
                CountryName = SelectCountry2.Country,
                State = SelectCountry2.StateName
            };
            if (ddlInECOIndustry.SelectedItem.Text.Equals("Other"))
                partner.Description = (string.IsNullOrEmpty(txtOtherIndustry.Text.Trim())?"":
                    string.Format("Industry:Other --> {0} </br>", txtOtherIndustry.Text.Trim())) + partner.Description;

            partner.ECOPartnerIndustries.Add(new ECOPartnerIndustry() { IndustryId = int.Parse(ddlInECOIndustry.SelectedValue), CreatedDate = DateTime.Now });
            foreach (ListItem item in ckbInEOCSpecialty.Items)
            {
                if (item.Selected)
                    partner.ECOPartnerSpecialties.Add(new ECOPartnerSpecialty() { SpecialtyId = int.Parse(item.Value), CreatedDate = DateTime.Now });
            }
            if (partner.save() == 0) //success
            {
                eStore.BusinessModules.EmailECOPartner emailHelper = new BusinessModules.EmailECOPartner(eStore.Presentation.eStoreContext.Current.Store, partner);
                var result = emailHelper.sendECOPartner(eStore.Presentation.eStoreContext.Current.User);
                if (result.ErrCode == esUtilities.EMailReponse.ErrorCode.NoError)
                    alertMessage("Email send success!");
                else
                    alertMessage(result.ErrCode.ToString());
            }
            else
                alertMessage("Save information fail!");
        }

        private void bindInfor(EcoInfor searchEcoInfor = null)
        {
            ddlEOCSpecialty.Items.Clear();
            ddlEOCSpecialty.Items.Add(new ListItem("[Select Specialty]", ""));
            foreach (ECOSpecialty ecop in eStore.Presentation.eStoreContext.Current.Store.getAllECOSpecialty())
            {
                ListItem li = new ListItem(){ Text = ecop.SpecialtyName, Value = ecop.SpecialtyName };
                if (searchEcoInfor != null && searchEcoInfor.Specialties.Contains(ecop.SpecialtyName))
                    li.Selected = true;
                ddlEOCSpecialty.Items.Add(li);
            }

            ddlECOIndustry.Items.Clear();
            ddlECOIndustry.Items.Add(new ListItem("[Select Industry]", ""));
            foreach (ECOIndustry ecoin in eStore.Presentation.eStoreContext.Current.Store.getAllECOIndustry())
            {
                ListItem li = new ListItem() { Text = ecoin.IndustryName, Value = ecoin.IndustryName };
                if (searchEcoInfor != null && searchEcoInfor.Industry == ecoin.IndustryName)
                    li.Selected = true;
                ddlECOIndustry.Items.Add(li);
            }

            if (searchEcoInfor != null)
            {
                CountrySelector1.Country = searchEcoInfor.Country;
                bindState(searchEcoInfor.State);
            }
        }

        private void bindInInfor()
        {
            ckbInEOCSpecialty.Items.Clear();
            foreach (ECOSpecialty ecop in eStore.Presentation.eStoreContext.Current.Store.getAllECOSpecialty())
            {
                ListItem li = new ListItem() { Text = ecop.SpecialtyName, Value = ecop.SpecialtyId.ToString() };
                ckbInEOCSpecialty.Items.Add(li);
            }
            ddlInECOIndustry.Items.Clear();
            ddlInECOIndustry.Items.Add(new ListItem("-- select --", ""));
            foreach (ECOIndustry ecoin in eStore.Presentation.eStoreContext.Current.Store.getAllECOIndustry())
            {
                ListItem li = new ListItem() { Text = ecoin.IndustryName, Value = ecoin.IndustryId.ToString() };
                ddlInECOIndustry.Items.Add(li);
            }
        }

        private void bindState(string state)
        {
            if (!string.IsNullOrEmpty(state))
                CountrySelector1.State = state;
        }
        
        protected void bindFonts()
        {
            SelectCountry2.CountryLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country) + ":";
            SelectCountry2.StateLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_State_Province) + ":";
        }
    }

    public class EcoInfor
    {
        private List<string> _specialties = new List<string>();

        public List<string> Specialties
        {
            get { return _specialties; }
            set { _specialties = value; }
        }

        private string _country;

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        private string _state;

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        private string _stateName;

        public string StateName
        {
            get { return _stateName; }
            set { _stateName = value; }
        }


        private string _industry;

        public string Industry
        {
            get { return _industry; }
            set { _industry = value; }
        }

        

    }
}