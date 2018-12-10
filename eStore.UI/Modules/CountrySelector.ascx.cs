using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class CountrySelector : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private bool _enableState;
        public bool EnableState
        {
            get { return _enableState; }
            set { _enableState = value; }
        }

        public string CountryLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Country);
        public string StateLableName = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_State_Province);
        
        public string guid = Guid.NewGuid().ToString();
        public bool isShowName = true;
        public string ValidationGroup { get; set; }
        private string _CountryCode;
        public string CountryCss { get; set; }
        public string StatesCss { get; set; }
        public string CountryTitleCss = "title";
        private List<string> _countryFilter = new List<string>();
        public string FilterCountry
        {
            get
            { return string.Join(",",_countryFilter); }
            set
            { _countryFilter = value.ToLower().Split(',').ToList(); }
        }
        public int ddlWidth {get;set; }
        public string CountryCode
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ddl_Country.SelectedValue))
                {
                    return this.ddl_Country.SelectedValue;
                }
                else
                    return string.Empty;
            }
            set
            {
                _CountryCode = value;
                try
                {
                    if (this.ddl_Country.Items.Count > 0)
                        this.ddl_Country.SelectedValue = value;
                }
                catch (Exception)
                {

                }
            }
        }
        public string Country
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ddl_Country.SelectedValue))
                {
                    return this.ddl_Country.SelectedItem.Text;
                }
                else
                    return string.Empty;
            }
            set
            { 
                if(!string.IsNullOrEmpty(value))
                {
                    ListItem li = ddl_Country.Items.FindByText(value);
                    if (li != null)
                    {
                        ddl_Country.ClearSelection();
                        li.Selected = true;
                    }
                }
            }
        }
        public string State
        {
            get
            {
                if (!string.IsNullOrEmpty(Request[this.ddl_State.UniqueID]))
                {
                    return Request[this.ddl_State.UniqueID];
                }
                else
                    return string.Empty;
            }
            set
            {
                hf_State.Value = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ListItem li = ddl_State.Items.FindByValue(value);
                    if (li != null)
                    {
                        ddl_State.ClearSelection();
                        li.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// state name
        /// </summary>
        public string StateName
        {
            get
            {
                if (!string.IsNullOrEmpty(State))
                {
                    POCOS.Country country = eStore.Presentation.eStoreContext.Current.Store.getCountrybyCountrynameORCode(Country);
                    if (country != null)
                    {
                        var countrystate = country.CountryStates.FirstOrDefault(c => c.StateShorts == State);
                        if (countrystate != null)
                            return countrystate.StateName;
                    }
                }
                return State;
            }
            set
            {
                hf_State.Value = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ListItem li = ddl_State.Items.FindByText(value);
                    if (li != null)
                    {
                        ddl_State.ClearSelection();
                        li.Selected = true;
                    }
                }
            }
        }

        private bool _isForShipping=false;
        public bool IsForShipping {
            get {
                return _isForShipping;
            }
            set {
                _isForShipping = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            hf_State.Value = State;
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ValidationGroup) == false)
            {
                this.RequiredCountry.ValidationGroup = this.ValidationGroup;
                this.ddl_Country.ValidationGroup = this.ValidationGroup;
                this.RequiredCountry.Enabled = true;
            }
            else
            {
                this.RequiredCountry.Enabled = false;

            }
            if (!IsPostBack || EnableState)
            {
                BindCountry();
                bindState();
                bindFonts();
            }
            lbl_Country.Visible = lbl_State.Visible = isShowName;
            //ddl_Country.Attributes.Add("onChange", "changeCustomerCountry(this.value,'"+ ddl_State.ClientID +"','"+hf_State.ClientID+"')");
            if (!string.IsNullOrEmpty(CountryCss))
                ddl_Country.CssClass = CountryCss;
            if (!string.IsNullOrEmpty(StatesCss))
                ddl_State.CssClass = StatesCss;
            if (ddlWidth > 0)
                ddl_Country.Width = ddl_State.Width = ddlWidth;
        }

        //Bind  Country
        public void BindCountry()
        {
            ddl_Country.Items.Clear();
            if (string.IsNullOrEmpty(_CountryCode))
            {
                if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingUser.mainContact != null)
                    _CountryCode = eStoreContext.Current.User.actingUser.mainContact.countryCodeX;
            }
            if (string.IsNullOrEmpty(_CountryCode))
                _CountryCode = Presentation.eStoreContext.Current.CurrentCountry.CountryName;

            List<POCOS.Country> countries;
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingRole == POCOS.User.Role.Employee)
            {
                eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                countries = eSolution.countries.OrderBy(c => c.CountryName).ToList();
            }
            else
            {
                countries = eStoreContext.Current.Store.profile.Countries.ToList<POCOS.Country>();
                string addtionalShipToCountry = eStore.Presentation.eStoreContext.Current.getStringSetting("Ship_To_Country");
                if (IsForShipping==true && string.IsNullOrEmpty(addtionalShipToCountry) == false)
                {
                    if (countries ==null)
                        countries = new List<POCOS.Country>();
                    eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                    string[] addtionalcountries= addtionalShipToCountry.Split(',');
                    countries.AddRange((from c in eSolution.countries
                                        where addtionalcountries.Contains(c.Shorts)
                                        select c).ToList());
                }
            }
            var isCheckVAT = Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting");
            ddl_Country.Items.Add(new ListItem("[" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select_Country) + "]", ""));
            foreach (POCOS.Country country in countries.OrderBy(x=>x.CountryName))
            {
                if (_countryFilter.Any())
                {
                    if (_countryFilter.Contains(country.CountryName.ToLower()) || _countryFilter.Contains(country.Shorts.ToLower()))
                    { }
                    else
                        continue;
                }
                ListItem item = new ListItem();
                if (!String.IsNullOrEmpty(_CountryCode))
                {
                    if (_CountryCode == country.Shorts)
                        item.Selected = true;
                }
                item.Text = country.CountryName;
                item.Value = country.Shorts;
                if (isCheckVAT)
                    item.Attributes.Add("IsEC", country.IsEC.GetValueOrDefault().ToString());
                ddl_Country.Items.Add(item);
            }
        }

        private void bindState()
        {
            if (string.IsNullOrEmpty(State))
            {
                if(!string.IsNullOrEmpty(hf_State.Value))
                    State = hf_State.Value;
                else if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingUser.mainContact != null)
                    State = eStoreContext.Current.User.actingUser.mainContact.State;
            }
        }

        protected void bindFonts()
        {
            lbl_Country.Text = CountryLableName;
            lbl_State.Text = StateLableName;
            ddl_State.Items.Add(new ListItem("[" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select_State) + "]", ""));
        }

    }
}