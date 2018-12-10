﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using eStore.POCOS;
using System.Linq;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class PStoreProduct
    {
        public class SimpleSpec
        {
            public string AttrCatName { get; set; }
            public string AttrName { get; set; }
            public string AttrValueName { get; set; }
        }

        private eStore.POCOS.StoreDeal.PromotionType _pStorePromotionType = eStore.POCOS.StoreDeal.PromotionType.Other;
        public eStore.POCOS.StoreDeal.PromotionType pStorePromotionType
        {
            get
            {
                if (storeDeal == null)
                    return StoreDeal.PromotionType.Other;
                else
                    return storeDeal.pStorePromotionType;
            }
        }

        private List< StoreDeal> _storeDeals;
        private string locker = "locker";
        public List<StoreDeal> storeDeals
        {
            get
            {
                lock (locker)
                {
                    if (_storeDeals == null)
                    {

                        POCOS.DAL.PStoreProductHelper helper = new DAL.PStoreProductHelper();

                        _storeDeals = helper.getStoreDeal(this.StoreID, this.PSProductId.GetValueOrDefault());
                        if (_storeDeals == null)
                            _storeDeals = new List<StoreDeal>();
                    }
                }
                return _storeDeals;
            }
        }

        private StoreDeal _storeDeal;
    
        public StoreDeal storeDeal
        {
            get
            {

                if (_storeDeal == null && storeDeals != null && storeDeals.Any())
                    {


                        _storeDeal = storeDeals.OrderBy(x => x.pStorePromotionType).FirstOrDefault();
                        if (_storeDeal == null)
                            _storeDeal = new StoreDeal();
                    }
    
                return _storeDeal;
            }
        }

        public new Boolean isOrderable(bool individual = false)
        {
            Boolean orderable = true;
            if (this.notAvailable)
                orderable = false;
            else if (this.StartFrom.Date > DateTime.Now.Date || this.ExpireDate.Date < DateTime.Now.Date)
                orderable = false;

            return orderable;
        }
        public override bool isPStoreProduct()
        {
            return true;
        }

        public bool isLongevity
        {
            get {
                string sLongevity = getMetadataValue("Longevity");
                if (string.IsNullOrEmpty(sLongevity))
                    return false;
                else if (sLongevity.Equals("1", StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            }
        }

        public override String productDescX
        {

            get
            {
                string desc = getMetadataValue("Product Description");
                if (!string.IsNullOrEmpty(desc))
                {
                    return desc;
                }
                else
                    return base.productDescX;
            }
        }

        public override string thumbnailImageX
        {
            get
            {
                return string.Format("https://wfcache.advantech.com/www/certified-peripherals/documents/{0}_Main.jpg", this.SProductID);
            }
        }
        public  string largeImageX
        {
            get
            {
                return string.Format("https://wfcache.advantech.com/www/certified-peripherals/documents/{0}_b.jpg", this.SProductID);
            }
        }
        public override Price getListingPrice()
        {
            Price listingPrice = new Price();

            eStore.POCOS.Product.PRICINGMODE mode = getListingPrice(listingPrice);

            return listingPrice;
        }
        public virtual eStore.POCOS.Product.PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment = null)
        {
            eStore.POCOS.Product.PRICINGMODE priceMode = eStore.POCOS.Product.PRICINGMODE.NOTAVAILABLE; //default as not-available

            if (priceBeforeAdjustment == null)  //caller doesn't care of priceBeforeAdjustment
            {
                //create a local one for calculation purpose
                priceBeforeAdjustment = new Price();
            }
            if (CallForPrice.GetValueOrDefault())
            {
                listingPrice.value = 0;
                return eStore.POCOS.Product.PRICINGMODE.NOTAVAILABLE;
            }
            //check product status and pricing setting
            if (!isOrderable())
            {
                listingPrice.value = 0;
                return eStore.POCOS.Product.PRICINGMODE.NOTAVAILABLE;
            }

            //get regular listing price here
            listingPrice.value = getNetPrice().value;

            if (listingPrice.value <= 0)    //invalid listing price
                return eStore.POCOS.Product.PRICINGMODE.NOTAVAILABLE;  //price not available

            //get promotion price if necessary
            if (this.storeDeal != null && this.storeDeal.PromotionPrice > 0)
            {
                priceMode = eStore.POCOS.Product.PRICINGMODE.SPECIAL;

                priceBeforeAdjustment.value = listingPrice.value;

                //validate pricing strategy to make sure priceBeforeAdjustment is at least 2% higher than listing price
                if (priceBeforeAdjustment.value * 0.98m >= this.storeDeal.PromotionPrice)  //good case
                {
                    listingPrice.value = this.storeDeal.PromotionPrice;
                    priceMode = eStore.POCOS.Product.PRICINGMODE.SPECIAL;
                }
                else
                {
                    priceMode = eStore.POCOS.Product.PRICINGMODE.REGULAR;  //though the product is in promotion mode, but the price strategy is not valid.
                    priceBeforeAdjustment.value = -1;   //price not available
                }
            }
            else
            {
                priceMode = eStore.POCOS.Product.PRICINGMODE.REGULAR;
            }

            if (this.CostX > listingPrice.value) // list price < cost will show call for price
            {
                listingPrice.value = 0;
                return eStore.POCOS.Product.PRICINGMODE.NOTAVAILABLE;
            }
 
            return priceMode;
        }
        public override decimal CostX
        {
            get
            {
                if (this.pStorePromotionType == StoreDeal.PromotionType.HottestDeals)//skip cost checking for HottestDeals
                { 
                    return 0m; 
                }
                else
                {
                    if (this.PTDCost.HasValue)
                        return this.PTDCost.GetValueOrDefault();
                    else
                        return base.CostX;
                }
            }
            set
            {
                this.PTDCost = value;
            }
        }
 

        private object specfilterlocker = new object();
        private bool? _isInitSpecFilter;
        public bool isInitSpecFilter
        {
            get
            {
                return _isInitSpecFilter.HasValue;
            }
        }
        private List<SimpleSpec> _simpleSpec;
        public List<SimpleSpec> simpleSpec
        {
            get
            {
                lock (specfilterlocker)
                {
                    if (isInitSpecFilter == false)
                    {
                        _simpleSpec = (from s in specs
                                       select new SimpleSpec {
                                        AttrCatName=s.AttrCatName,
                                         AttrName=s.AttrName,
                                          AttrValueName=s.AttrValueName
                                       }).ToList();
                        //_isInitSpecFilter = true;
                        return _simpleSpec;
                    }
                    else
                    {
                        return _simpleSpec;
                    }
                }
            }
            set {
                lock (specfilterlocker)
                {
                    _simpleSpec = value;
                    _isInitSpecFilter = true;
                }
            }
        }

        public override IList<VProductMatrix> specs
        {
            get
            {
                return metadata.Where(x => x.isProductSepc).OrderBy(x=>x.seq).ToList();
            }

        }

        /// <summary>
        /// The following three prooperties are for product detail page SEO purpose
        /// </summary>
        public String pageTitle
        {
            get
            {
                string PageTitle = this.MetaTitle;
                if (!String.IsNullOrEmpty(PageTitle))
                    return PageTitle;
                else
                    return this.productDescX;
            }
        }

        /// <summary>
        /// This property is a safe way to product keywords.  When product doesn't have any keyword associated it will
        /// return product's display name and product ID as default product keyword set.
        /// </summary>
        public String keywords
        {
            get
            {
                string Keywords = this.MetaKeyword;
                if (!String.IsNullOrEmpty(Keywords))
                    return Keywords;
                else
                    return SProductID;
            }
        }

        public String metaData
        {
            get
            {
                string PageDescription = this.MetaDescription;
                if (!String.IsNullOrEmpty(PageDescription))
                    return PageDescription;
                else
                    return this.productDescX;
            }
        }


        private IList<VProductMatrix> _metatdata;

        private IList<VProductMatrix> metadata {
            get {
                if (_metatdata == null)
                { 
                        POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
                        _metatdata = helper.getMetadata(this, null); 
                }
                return _metatdata;
            }
        }
        private List<string> _features;

        private string getMetadataValue(string key)
        {
            try
            {
                string mv = metadata.Where(x => x.AttrName == key).Select(x => x.LocalValueName).FirstOrDefault();
                return mv;

            }
            catch (Exception)
            {

                return string.Empty;
            }
          
        }

        public override string productFeatures
        {
            get
            {
                if (_features == null)
                {
                    POCOS.DAL.PStoreProductHelper helper = new POCOS.DAL.PStoreProductHelper();
                    string productfeature = getMetadataValue("Features");
                    if (string.IsNullOrEmpty(productfeature))

                        _features = new List<string>();
                    else
                    {
                        _features = productfeature.Split('•').Where(x=>string.IsNullOrEmpty(x)==false).ToList();
                    }

                }
                return string.Join("", _features.Select(x=>string.Format("<li>{0}</li>",x.Trim())).ToArray());
            }
        }

        public bool IsPrimary { get; set; }
        public bool IsDefault { get; set; }
        public int MaxSet { get; set; } // ex: M/B has two CPU sockets, so MaxSet = 2



        private PStoreProductCategory _category;
        public PStoreProductCategory category
        {
            get
            {
                if (_category == null)
                {
                    PStoreProductCategoryHelper helper = new PStoreProductCategoryHelper();
                    _category = helper.get(this.storeX, this.ProductCategoryId);
                }
                return _category;
            }
        }


        private PStoreProductCategory _maincategory;
        public PStoreProductCategory mainCategory
        {
            get
            {
                if (_maincategory == null)
                {
                    if (category == null)
                    {
                        _maincategory = null;
                    }
                    else
                    {
                        if (category.parentX == null)
                        {
                            _maincategory = category;
                        }
                        else
                        {
                            _maincategory = category;
                            int depth = 5;
                            do
                            {
                                depth--;
                                _maincategory = _maincategory.parentX;
                            }
                            while (_maincategory.parentX != null&&depth>0);
                        }

                    }
                }

                return _maincategory;
            }
        }


 
        private List<PStoreProduct> _youMayAlsoBuy;
        public List<PStoreProduct> youMayAlsoBuy
        {
            get
            {
                if (_youMayAlsoBuy == null)
                {
                    POCOS.DAL.PStoreProductHelper pproducthelper = new PStoreProductHelper();
                    _youMayAlsoBuy= pproducthelper.GetStoreProductYouMayAlsoBuyByStoreProduct(this.StoreID, this.SProductID);
                }
                return _youMayAlsoBuy;
            }
        }

        private List<PStoreProduct> _associate;
        public List<PStoreProduct> associate
        {
            get
            {
                if (_associate == null)
                {
                    POCOS.DAL.PStoreProductHelper pproducthelper = new PStoreProductHelper();
                    _associate= pproducthelper.GetStoreProductAssociateByStoreProductId(this.StoreID, this.SProductID);
                }
                return _associate;
            }
        }
        private List<StoreProductBundleList> _bundleList;
        public List<StoreProductBundleList> BundleList
        {
            get {
                if (_bundleList == null)
                {
                    POCOS.DAL.PStoreProductHelper pproducthelper = new PStoreProductHelper();
                    _bundleList = pproducthelper.getBundleList(this);
                }
                return _bundleList;
            }
        }

        private List<StoreProductBundle> _bundles;
        public List<StoreProductBundle> bundles
        {
            get
            {
                if (_bundles == null)
                {
                    _bundles = new List<StoreProductBundle>();
                    if (BundleList != null && BundleList.Any())
                    {

                        var bg = (from bl in BundleList
                                  group bl by bl.StoreProductBundle into g
                                  select g).ToList();

                        if (bg.Any())
                        {
                            foreach (var g in bg)
                            {
                                StoreProductBundle spb = g.Key;
                                spb.StoreProductBundleLists = g.ToList();
                                _bundles.Add(spb);
                            }
                        }
                    }
                }
                return _bundles;
            }
        }


        /// <summary>
        /// this is for bundle with me
        /// </summary>
        /// <param name="bundleid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public POCOS.Product_Bundle getComboProducts(int bundleid, int itemid)
        {
            if (BundleList == null && !BundleList.Any())
                return null;
            StoreProductBundleList item = BundleList.FirstOrDefault(x => x.StoreProductBundleId == bundleid && x.Id == itemid);
            if (item == null)
                return null;
            var Primaryitem = BundleList.FirstOrDefault(x => x.IsPrimary);

            if (Primaryitem == null)
                return null;

            POCOS.Part virtualBundleProduct = (new PartHelper()).getPart("ePAPS-Bundle", this.StoreID);
            if (virtualBundleProduct == null || !(virtualBundleProduct is POCOS.Product_Bundle))
                return null;
            Bundle initBundle = new Bundle();
            initBundle.StoreID = this.StoreID;
            initBundle.SourceType = "ePAPSBundleWith";
            initBundle.SourceTemplateID = bundleid;
            BundleItem mainItem = new BundleItem(this, 1, 1, Primaryitem.basePrice);
            initBundle.BundleItems.Add(mainItem);

            BundleItem bundleItem = new BundleItem(item.partX, item.QuantityPerSet ?? 1, 2, item.BundlePricePerSet);
            initBundle.BundleItems.Add(bundleItem);
            initBundle.updatePrice(initBundle.originalPrice);

            Product_Bundle productBundle = ((Product_Bundle)virtualBundleProduct).clone(initBundle, bundleid);
            productBundle.DisplayPartno = item.StoreProductBundle.BundlePartNo;
            productBundle.ProductDesc = item.StoreProductBundle.Note;
            productBundle.CostX = -1;
            return productBundle;
        }
    }

}