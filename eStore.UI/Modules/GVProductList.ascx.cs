using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class GVProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string target = "";
        public bool isToeStore = false;

        private string _ResultsLocationType = "Both";
        public string ResultsLocationType
        {
            get { return _ResultsLocationType; }
            set { _ResultsLocationType = value; }
        }
        

        private List<POCOS.Product> _productList;
        public List<POCOS.Product> productList
        {
            get { return _productList; }
            set { _productList = value; }
        }

        public bool KeepOriginalSequence { get; set; }

        private int _pagesize = 20;

        public int pageSize
        {
            get { return _pagesize; }
            set { _pagesize = value; }
        }


        protected override void Render(HtmlTextWriter writer)
        {
            if (this.CollectionPager1.Visible)
                bottomPager.Text = CollectionPager1.RenderedHtml;
            else
                bottomPager.Text = string.Empty;

            switch (_ResultsLocationType)
            { 
                case "Top":
                    bottomPager.Visible = false;
                    break;
                case "Bottom":
                    CollectionPager1.Visible = false;
                    break;
                default:
                    break;
            }
            base.Render(writer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindGV();
        }

        public List<string> getStrSlectProductNo()
        {
            List<string> ls = new List<string>();
            string requestproducts = Request["cbproduct"];
            if (!string.IsNullOrEmpty(requestproducts))
            {
                ls = requestproducts.Split(',').ToList();
            }
            return ls;
        }

        /// <summary>
        /// get all select product
        /// </summary>
        /// <returns></returns>
        public List<POCOS.Product> getSlectProductNo()
        {
            List<POCOS.Product> ls = new List<POCOS.Product>();

            string requestproducts = Request["cbproduct"];
            if (!string.IsNullOrEmpty(requestproducts))
            {
                foreach (string pid in requestproducts.Split(','))
                {
                    POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(pid);
                    if (product != null)
                        ls.Add(product);
                }
            }
            return ls;
        }

        protected void BindGV()
        {
            if (productList != null && productList.Count() > 0)
            {
                var products = from p in productList
                               select new
                               {
                                   SProductID = p.SProductID,
                                   name = p.DisplayPartno,
                                   desc = p.productDescX,
                                   img = p.thumbnailImageX,
                                   InCompareList = Presentation.Product.ProductCompareManagement.GetCompareProductsIds().Exists(x => x == p.SProductID),
                                   price = Presentation.Product.ProductPrice.getAJAXProductPrice(p),
                                   link = ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(p)),
                                   statusseq = ((int)p.status < (int)POCOS.Product.PRODUCTSTATUS.PHASED_OUT) ? 0 : 1,
                                   listingOrder = (p.isOrderable(true) && p.getListingPrice().value > 0) ? 0 : 1,
                                   status = "<img class=\"icoStyle\" src='" + esUtilities.CommonHelper.GetStoreLocation(false) + "images/" + p.status + ".gif'/>",
                                   phasedOut = p.notAvailable,
                                   DisableAddtoCart = (p.notAvailable || p.ShowPrice == false),
                                   ReplaceProducts = p.ReplaceProductsX,
                                   ProductType = p.ProductType,
                                   MininumnOrderQty = p.MininumnOrderQty
                               };

                bindCollectionPagerFonts();
                if (KeepOriginalSequence)
                    CollectionPager1.DataSource = products.ToList();
                else
                    CollectionPager1.DataSource = products.OrderBy(o => o.listingOrder).ThenBy(o => o.statusseq).ThenByDescending(o => o.name).ToList();
                CollectionPager1.PageSize = _pagesize;
                CollectionPager1.BindToControl = gvProduct;

                gvProduct.DataSource = CollectionPager1.DataSourcePaged;
                gvProduct.DataBind();
            }
        }
        protected void bindCollectionPagerFonts()
        {
            CollectionPager1.ResultsFormat = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Displaying_results);
            CollectionPager1.NextText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Next);
            CollectionPager1.BackText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Previous);
            CollectionPager1.FirstText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_First);
            CollectionPager1.LastText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Last);
        }

        //GridView Sort
        protected void gvProduct_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";

            if (sortExpression == gvProduct.Attributes["SortExpression"])
            {
                sortDirection = gvProduct.Attributes["SortDirection"] == sortDirection ? "Desc" : "ASC";
            }
            gvProduct.Attributes["SortExpression"] = sortExpression;
            gvProduct.Attributes["SortDirection"] = sortDirection;

            BindGV(); 
        }

        protected string binPhaseOutProduct(object pPhaseOut, object pProductType, object plink, object pSProductID, object pName, object pStatus)
        {
            string linkurl = plink.ToString();
            string storeurl = "";
            if (isToeStore && eStore.Presentation.eStoreContext.Current.MiniSite != null 
                    && !string.IsNullOrEmpty(eStore.Presentation.eStoreContext.Current.MiniSite.ApplicationPath)
                    && linkurl.StartsWith(eStore.Presentation.eStoreContext.Current.MiniSite.ApplicationPath,StringComparison.OrdinalIgnoreCase))
            {
                linkurl = linkurl.ToUpper().Replace(eStore.Presentation.eStoreContext.Current.MiniSite.ApplicationPath.ToUpper(), "");
            }
            
            if (isToeStore)
                storeurl = "http://" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreURL;

            bool phaseOut = (bool)pPhaseOut;
            if (phaseOut)
                return string.Format("<a href='{0}' id='{1}' name='{2}'class=\"jTipProductDetail\"{4}>{2}</a>&nbsp;{3}<br /><span class='colorRed'>Phase out</span>"
                    , storeurl + linkurl, this.ClientID + pSProductID, pName, pStatus, (string.IsNullOrEmpty(target) ? "" : " target=" + target + ""));
            else
                return string.Format("<a href='{0}' id='{1}' name='{2}'class=\"jTipProductDetail\"{4}>{2}</a>&nbsp;{3}"
                    , storeurl + linkurl, this.ClientID + pSProductID, pName, pStatus, (string.IsNullOrEmpty(target) ? "" : " target=" + target + ""));
        }
    }
}