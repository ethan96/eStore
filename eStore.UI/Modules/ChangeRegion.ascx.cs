using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class ChangeRegion : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string ImageCss
        {
            set { imgCountry.CssClass = value; }
        }
        Dictionary<string, string> _ImageAttr = new Dictionary<string, string>();
        public Dictionary<string,string> ImageAttr
        {
            get { return _ImageAttr; }
        }

        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.setSelectedCountry();
                base.ChildControlsCreated = true;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var v in _ImageAttr)
                    imgCountry.Attributes.Add(v.Key, v.Value);
        }
        void setSelectedCountry()
        {
            string seletedcountry = Presentation.eStoreContext.Current.CurrentCountry.CountryName;            
            setCountryFlag(seletedcountry);
        }
        void setCountryFlag(string CountryName)
        {
            imgCountry.ToolTip = CountryName;
            if (System.IO.File.Exists(MapPath(ResolveUrl("~/images/flags/" + CountryName + ".png"))))
            {
                imgCountry.ImageUrl = ResolveUrl("~/images/flags/" + CountryName + ".png");
            }
            else
            {
                imgCountry.ImageUrl = ResolveUrl("~/images/flags/hidden.png");
                lblChangeCountryRegion.Text = CountryName;
                lblChangeCountryRegion.ToolTip = CountryName;
            }
        }

        protected void btnChangeRegion_Click(object sender, EventArgs e)
        {
            string countryName = Request.Form["__EVENTARGUMENT"];
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            List<POCOS.Country> countries = eSolution.countries.ToList();
            POCOS.Country country = countries.FirstOrDefault(c => c.CountryName == countryName);
           Presentation.eStoreContext.Current.CurrentCountry=country;
        }
    }
}