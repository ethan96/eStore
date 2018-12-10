using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI
{
    public partial class IoTMart : Presentation.eStoreBaseControls.eStoreBasePage
    {/// <summary>
        /// can not over write the master page because used the special holder eStoreConfigurableRightContent,
        /// if this place hodler has controls, it will not apply the setting 
        /// </summary>
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            BindScript("url", "easing", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "carousel", "iot.carousel.js");

            List<POCOS.Advertisement> Advertisements = Presentation.eStoreContext.Current.Store.getiServiceHomeBanners(Presentation.eStoreContext.Current.MiniSite);
            this.AdRotatorSelect1.BannerList = Advertisements.Where(x => x.segmentType == eStore.POCOS.Advertisement.AdvertisementType.HomeBanner).ToList();

            eStore.Presentation.OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new eStore.Presentation.OpenGraphProtocolAdapter("HomePage");
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);

            rpBestSeller.DataSource = getBestSellers(8);
            rpBestSeller.DataBind();

            rpNewArrivals.DataSource = getNewArrivals(8);
            rpNewArrivals.DataBind();
        }


        #region Data Source

        

        /// <summary>
        /// get best seller products
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<POCOS.Product> getBestSellers(int count)
        {
            var hotProduts = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.HOT
                , Presentation.eStoreContext.Current.MiniSite, count, true).OrderByDescending(p => p.ProductLastUpdated).ToList();

            return hotProduts;
        }


        /// <summary>
        /// get New Arrivals products
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<POCOS.Product> getNewArrivals(int count)
        {
            var newProduts = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.NEW
                , Presentation.eStoreContext.Current.MiniSite, count, true).OrderByDescending(p => p.ProductLastUpdated).ToList();

            return newProduts;
        }

        #endregion

        
    }
}