using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class AutomationHomeContent :  Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // liveperson1.LivePersonType = "livepersonACN";
            liveperson1.UserLargerImage = true;
            rpHotProduct.DataSource = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.HOT, Presentation.eStoreContext.Current.MiniSite, 6,true).OrderByDescending(p=>p.ProductLastUpdated).ToList();
            rpHotProduct.DataBind();
            rpNewProduct.DataSource = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.NEW, Presentation.eStoreContext.Current.MiniSite, 6, true).OrderByDescending(p => p.ProductLastUpdated).ToList();
            rpNewProduct.DataBind();

            eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite);

            if (Presentation.eStoreContext.Current.getBooleanSetting("EnableTodaysDeals"))
            {
                if (eStoreContext.Current.Store.todaysDealsColumns != null)
                {
                    this.rptTodaysDeals.DataSource = eStoreContext.Current.Store.todaysDealsColumns.Where(x=>x.MiniSite==Presentation.eStoreContext.Current.MiniSite);
                    this.rptTodaysDeals.DataBind();
                }
            }

            elsRewardsAds.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner("RewardsAdsSliderBanner").Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).ToList();

        }
    }
}