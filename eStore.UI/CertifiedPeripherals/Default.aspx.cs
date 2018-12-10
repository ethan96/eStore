using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using esUtilities;
using eStore.Presentation.SearchConfiguration;

namespace eStore.UI.CertifiedPeripherals
{
    public partial class Default : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Presentation.eStoreContext.Current.SearchConfiguration = new PStoreSearchConfiguration();
        }
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
            set
            {
                base.OverwriteMasterPageFile = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            POCOS.ProductCategory cp = Presentation.eStoreContext.Current.Store.getProductCategory("CP");
            if (cp != null)
                Response.Redirect(Presentation.UrlRewriting.MappingUrl.getMappingUrl(cp));
            else
            {
                Response.Redirect("~/");
            }

            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
           
            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");

            BindScript("url", "jquery.simplePagination.js", "jquery.simplePagination.js");
            string pagestyle = CommonHelper.GetStoreLocation() + "Styles/simplePagination.css";
            AddStyleSheet(pagestyle);

            BindScript("url", "CertifiedPeripherals", "CertifiedPeripherals.js");
            POCOS.MiniSite miniSite = Presentation.eStoreContext.Current.Store.profile.MiniSites.FirstOrDefault(x => x.SiteName == "CertifiedPeripherals");
            this.eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.getHomeBanners(miniSite);
            Presentation.eStoreContext.Current.keywords.Add("KeyWords", "CertifiedPeripheralsHomepage");

            this.isExistsPageMeta = this.setPageMeta(pstore.pstore.MetaTitle, pstore.pstore.MetaDescription, pstore.pstore.MetaKeyword);

        }
        protected override void OnPreRender(EventArgs e)
        {
            string style = CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);
    
            base.OnPreRender(e);
        }
      
    }
}