using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.IoTMart
{
    public partial class HotDeals : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected string videoCss = "";
        private Dictionary<POCOS.Product, POCOS.Advertisement> videoads = new Dictionary<POCOS.Product, POCOS.Advertisement>();

        public event EventHandler eHaveInfor;

        private POCOS.ProductCategory _category;
        public POCOS.ProductCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (_category != null)
            {
                var ads = eStore.Presentation.eStoreContext.Current.Store.getAds(_category, POCOS.Advertisement.AdvertisementType.StoreAds);
                foreach (var ad in ads)
                {
                    foreach (var menu in ad.MenuAdvertisements)
                    {
                        var pro = _category.productList.FirstOrDefault(c => c.DisplayPartno == menu.TreeID);
                        if (pro != null && ad.htmlContentX.Contains("iot-videoBlock") && !videoads.Keys.Select(c=>c.DisplayPartno).Contains(pro.DisplayPartno))
                            videoads.Add(pro, ad);
                    }
                }
            }
            bool hasObj = false;
            if (videoads.Any())
            {
                videoCss = "iot-highlightBlock iot-highlight2";
                rpHotDeals.DataSource = videoads.Select(c => c.Key).Take(2).ToList();
                rpHotDeals.DataBind();
                hasObj = true;
            }
            else
            {
                videoCss = "iot-highlightBlock";
                var ls = new List<POCOS.Product>();
                if (_category == null)
                    ls = getHotDealProducts(3);
                else
                    ls = getHotDealProducts(_category, 3);
                if (ls.Any())
                {
                    rpHotDeals.DataSource = ls;
                    rpHotDeals.DataBind();
                    hasObj = true;
                }
            }
            if (!hasObj && eHaveInfor != null)
                eHaveInfor(null, null);
        }

        /// <summary>
        /// get hot deal products
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<POCOS.Product> getHotDealProducts(int count)
        {
            var promoProduts = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION
                , Presentation.eStoreContext.Current.MiniSite, count, true).OrderByDescending(p => p.ProductLastUpdated).ToList();
            var cj = count - promoProduts.Count;
            if (cj > 0)
                promoProduts.AddRange(Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.NEW
                    , Presentation.eStoreContext.Current.MiniSite, cj, true).OrderByDescending(p => p.ProductLastUpdated).ToList());
            return promoProduts;
        }

        protected List<POCOS.Product> getHotDealProducts(POCOS.ProductCategory pc, int count) 
        {
            var promoProduts = Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION
                , Presentation.eStoreContext.Current.MiniSite, pc, count);
            var cj = count - promoProduts.Count;
            if (cj > 0)
                promoProduts.AddRange(Presentation.eStoreContext.Current.Store.getProducts(POCOS.Product.PRODUCTMARKETINGSTATUS.NEW
                , Presentation.eStoreContext.Current.MiniSite, pc, cj));
            return promoProduts;
        }

        protected void rpHotDeals_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.Product pro = e.Item.DataItem as POCOS.Product;
                Literal ltProStatus = e.Item.FindControl("ltProStatus") as Literal;
                Literal ltViedo = e.Item.FindControl("ltViedo") as Literal;

                if (videoads.Any())
                {
                    foreach (var v in videoads)
                    {
                        if (v.Key.DisplayPartno == pro.DisplayPartno)
                        {
                            ltViedo.Text = v.Value.htmlContentX;
                            break;
                        }
                    }
                }
                else
                {
                    if (pro.marketingstatus.Any())
                    { 
                        if(pro.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.NEW))
                            ltProStatus.Text = "<span class=\"iot-iconNew\"></span>";
                    }
                    Literal ltProPromotion = e.Item.FindControl("ltProPromotion") as Literal;
                    if (!string.IsNullOrEmpty(pro.PromoteMessage))
                        ltProPromotion.Text = "<div class=\"iot-proDiscount\">" + pro.PromoteMessage + "</div>";
                }
                if (!pro.isOrderable())
                {
                    Panel porderEable = e.Item.FindControl("porderEable") as Panel;
                    porderEable.Visible = false;
                }
            }
        }
        
    }
}