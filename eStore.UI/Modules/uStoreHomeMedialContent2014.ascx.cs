using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class uStoreHomeMedialContent2014 : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected string sitename = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            //int itemsCount = 5;
            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLeveluStoreCategories(eStoreContext.Current.MiniSite);
            this.rpCategories.DataSource = spc.OrderBy(c => c.Sequence).ThenBy(p => p.LocalCategoryName);
            this.rpCategories.DataBind();
            if (eStoreContext.Current.MiniSite != null && !string.IsNullOrEmpty(eStoreContext.Current.MiniSite.SiteName))
            {
                sitename = eStoreContext.Current.MiniSite.SiteName;
            }
            List<POCOS.Application> apps = eStoreContext.Current.Store.getTopLevelApplications(eStoreContext.Current.MiniSite);
            rpApplications.DataSource = apps.OrderBy(c => c.Sequence).ThenBy(p => p.LocalCategoryName);
            rpApplications.DataBind();

            rpBanners.DataSource = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite, false);
            rpBanners.DataBind();

            if (Presentation.eStoreContext.Current.getBooleanSetting("hasECOPartner",false))
            {
                rpEcoPartnership.DataSource = Presentation.eStoreContext.Current.Store.getAllECOPartner().Take(6);
                rpEcoPartnership.DataBind();
            }
            this.BindScript("url", "iservices.js", "iservices.js");

        }
    }
}