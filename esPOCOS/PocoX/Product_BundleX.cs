using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;



namespace eStore.POCOS
{
    public partial class Product_Bundle : Product
    {
        private Boolean? _valid;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public Product_Bundle()
        {
        }

        /// <summary>
        /// This constructor is used to load a dynamic bundle product with existing bundle setting.
        /// It's mainly will be used for revert product_bundle from shopping cart item.
        /// </summary>
        /// <param name="initBundle"></param>
        public Product_Bundle(Bundle initBundle, int sourceTemplate) : this(sourceTemplate)
        {
            this.bundle = initBundle;
        }

        /// <summary>
        /// This constructor shall be used for creating dynamic product bundle instance from 
        /// Bundle template
        /// </summary>
        /// <param name="sourceTemplate"></param>
        public Product_Bundle(int sourceTemplate)
        {
            this.sourceTemplateID = sourceTemplate;
        }

        public Product_Bundle clone(int sourceTemplate)
        {
            Product_Bundle productBundle = new Product_Bundle(sourceTemplate);
            productBundle.StoreID = this.StoreID;
            productBundle.SProductID = this.SProductID;
            productBundle.DisplayPartno = this.name;
            productBundle.ProductDesc = this.productDescX;
            productBundle.ProductType = this.ProductType;
            productBundle.publishStatus = PUBLISHSTATUS.PUBLISHED;
            productBundle.status = PRODUCTSTATUS.GENERAL;
            foreach (PeripheralAddOnBundleItem item in this.PeripheralAddOnBundleItems)
            {
                if (item.Part != null)
                    productBundle.addProductBundleItem(item.Part, item.Qty.GetValueOrDefault(), item.Sequence.GetValueOrDefault());
            }

            return productBundle;
        }
        public Product_Bundle clone(Bundle initBundle, int sourceTemplate)
        {
            Product_Bundle productBundle = new Product_Bundle(initBundle,sourceTemplate);
            productBundle.StoreID = this.StoreID;
            productBundle.SProductID = this.SProductID;
            productBundle.DisplayPartno = this.name;
            productBundle.ProductDesc = this.productDescX;
            productBundle.ProductType = this.ProductType;
            productBundle.publishStatus = PUBLISHSTATUS.PUBLISHED;
            productBundle.status = PRODUCTSTATUS.GENERAL;
            productBundle.PublishStatus = this.PublishStatus;
            productBundle.ShowPrice = this.ShowPrice;
           //this.bundle = initBundle;
            return productBundle;
        }
        public override PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment)
        {
            if (priceBeforeAdjustment == null)
                priceBeforeAdjustment = new Price();

            PRICINGMODE mode = new PRICINGMODE();
            if (this.sourceTemplateID.HasValue)
            {
                //get listing price from bundle items
                if (isValid() && bundle!=null)
                {
                    listingPrice.value = bundle.adjustedPrice;
                    priceBeforeAdjustment.value = bundle.originalPrice;
                    if (listingPrice.value != priceBeforeAdjustment.value)
                        mode = PRICINGMODE.SPECIAL;
                    else
                        mode = PRICINGMODE.REGULAR;
                }
                else
                {
                    listingPrice.value = 0m;
                    priceBeforeAdjustment.value = 0m;
                    mode = PRICINGMODE.NOTAVAILABLE;
                }
            }
            else
            {
                mode = base.getListingPrice(listingPrice, priceBeforeAdjustment);

                if (isValid() == false)    //CTOS has setting problem and is not legible for order
                    mode = PRICINGMODE.NOTAVAILABLE;
            }
            /*
            //get CTOS default listing price first            
            if (isValid() && bundle!=null)
            {
                listingPrice.value = bundle.adjustedPrice;
                priceBeforeAdjustment.value = bundle.originalPrice;
                if (listingPrice.value != priceBeforeAdjustment.value)
                    mode = PRICINGMODE.SPECIAL;
                else
                    mode = PRICINGMODE.REGULAR;
            }
            else
            {
                listingPrice.value = 0m;
                priceBeforeAdjustment.value = 0m;
                mode = PRICINGMODE.NOTAVAILABLE;
            }
             */

            return mode;
        }

        private Boolean isValid()
        {
            if (isInited() == false)
                initialize();
            return _valid.GetValueOrDefault();
        }

        /// <summary>
        /// This method to initialize complete Product_Bundle components.
        /// </summary>
        public void initialize()
        {
            lock (this)
            {
                if (this.isInited() == false)
                    validateAndInit();
            }
        }

        /// <summary>
        /// This method indicates whether this CTOS system is initi and validated yet.
        /// </summary>
        /// <returns></returns>
        public Boolean isInited()
        {
            return _valid.HasValue;
        }
        public override bool isPStoreProduct()
        {
            if (this.isInited() == false)
                validateAndInit();
            if (bundle != null && bundle.BundleItems.Any())
            {
                if (bundle.BundleItems.Count(x => x.part.isPStoreProduct()) == bundle.BundleItems.Count())
                { return true; }
                else
                { return false; }
            }
            else
                return false;
        }
        /// <summary>
        /// validateAndInit function will first validate through all items listed in ProductBundleItem and compose a default bundle instance
        /// if all items in ProductBundleItem list are valid.  It also returns validation result.
        /// </summary>
        /// <returns></returns>
        private Boolean validateAndInit()
        {
            try
            {
                if (ProductBundleItems != null && ProductBundleItems.Count() > 0)
                {
                    _valid = true;
                    _bundle = new Bundle(this);
                    PartHelper parthelper = new PartHelper();
                    parthelper.prefetchPartList(StoreID, this.ProductBundleItems.Select(x => x.ItemSProductID).ToList());
                    foreach (ProductBundleItem bi in this.ProductBundleItems)
                    {
                        Part part = parthelper.getPart(bi.ItemSProductID, StoreID);
                        if (part != null && part.isOrderable() && part.getNetPrice(false).value > 0)
                        {
                            _bundle.addItem(part, bi.Qty, bi.Sequence.Value);
                            if (bi.Assembly.GetValueOrDefault())
                            {
                                AssemblyProduct = part;
                                _peripheralCompatiblesX = part.PeripheralCompatibles;
                            }
                        }
                        else
                        {
                            //part in the bundle does not exist
                            _valid = false;
                        }
                    }

                    //adjust price if there is price different between selling price and total default price of all items.
                    Price listingPrice = new Price();
                    Price priceBeforeAdjustment = new Price();
                    PRICINGMODE mode = base.getListingPrice(listingPrice, priceBeforeAdjustment);
                    Decimal bundleDefaultPrice = bundle.adjustedPrice; ;

                    if (isValid() && listingPrice.value >0 && listingPrice.value != bundleDefaultPrice)
                        bundle.updatePrice(listingPrice.value);
                }
            }
            catch (Exception ex)
            {
                _valid = false;
                String errorMessage = String.Format("Bundle Product {0} validateAndInit",this.SProductID);
          
                eStoreLoger.Error(errorMessage, "","", StoreID,ex);
            }

            return _valid.GetValueOrDefault();
        }
        private Part _AssemblyProduct;
        public Part AssemblyProduct { get {
            return _AssemblyProduct;
        }
            set {
                _AssemblyProduct = value;
            }
        }
        /// <summary>
        /// will get from assembly item when init
        /// </summary>
        private ICollection<PeripheralCompatible> _peripheralCompatiblesX;
        public  ICollection<PeripheralCompatible> peripheralCompatiblesX
        {
            get
            {
                if (this.PeripheralCompatibles.Count > 0)
                    return this.PeripheralCompatibles;
                else
                {
                    if (_peripheralCompatiblesX == null)
                        _peripheralCompatiblesX = new List<PeripheralCompatible>();
                    return _peripheralCompatiblesX;
                }
            }
            set
            {
                _peripheralCompatiblesX = value;
            }
        }

        private Bundle _bundle = null;
        public Bundle bundle
        {
            get
            {
                if (_bundle == null)    //not inited yet
                {
                    //perform initialization
                    initialize();


                }

                return _bundle;
            }

            set { _bundle = value; }
        }

        /// <summary>
        /// This is runtime property shall only be used for dynamic product bundle instance.
        /// Some of product bundles provide only empty shells. Their bundle item list will be dynamic
        /// composed on the run per need.   When dynamic product bundle is about to be created
        /// this property shall be inited and used to associate with which template compose this dynamic
        /// product bundle instance.
        /// </summary>
        public int ? sourceTemplateID;

        /// <summary>
        /// This method will create a new ProductBundleItem based on input values and add this new item
        /// to ProductBundleItems list.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        public ProductBundleItem addProductBundleItem(Part part, int qty, int seq = 1)
        {
            ProductBundleItem item = new ProductBundleItem(this, part, qty, seq);
            this.ProductBundleItems.Add(item);

            return item;
        }

        List<Part>_defaultSpecSources = null;
        public List<Part> specSources
        {
            get
            {
                if (_defaultSpecSources == null)
                {
                    _defaultSpecSources = new List<Part>();

                    StringBuilder partList = null;
                    foreach (ProductBundleItem bi in this.ProductBundleItems)
                    {
                        if (partList == null)
                        {
                            partList = new StringBuilder();
                            partList.Append(bi.ItemSProductID);
                        }
                        else
                            partList.Append(","+bi.ItemSProductID);
                    }
                    if (partList != null)
                    {
                        PartHelper helper = this.parthelper;
                        if (helper == null) //this condition shall not exist
                            helper = new PartHelper();

                        var parts = helper.prefetchPartList(this.StoreID, partList.ToString());
                        foreach (Part part in parts)
                        {
                            if (part != null && part.isMainStream())
                                _defaultSpecSources.Add(part);
                        }
                    }
                }
                return _defaultSpecSources;
            }
        }

        //重新计算 cost
        public decimal FixCost()
        {
            _costX = 0;
            foreach (ProductBundleItem bItem in this.ProductBundleItems)
            {
                PartHelper helper = this.parthelper;
                if (helper == null)
                    helper = new PartHelper();
                POCOS.Part partBundleItem = helper.getPart(bItem.ItemSProductID, bItem.StoreID, true);
                if (partBundleItem != null)
                {
                    _costX += bItem.Qty * ((partBundleItem.Cost.HasValue && partBundleItem.Cost.Value > 0) ? partBundleItem.Cost.Value : 0);
                }
            }
            return _costX;
        }

        private decimal _costX;
        public override decimal CostX
        {
            get
            {
                if (_costX == 0)
                    FixCost();
                return _costX;
            }
            set
            {
                _costX = value;
            }
        }

        public override bool isBelowCost
        {
            get
            {
                return CostX > getListingPrice().value;
            }
        }


        public override string dataSheetX
        {
            get
            {
                if (String.IsNullOrWhiteSpace(DataSheet))
                {
                    if (specSources != null && specSources.Count > 0)
                    {
                        DataSheet = specSources[0].dataSheetX;
                    }
                }
                return DataSheet;
            }
        }

        public void regetBundleItem()
        {
            this.ProductBundleItems.Clear();
            BundelItemHelper helper = new BundelItemHelper();
            this.ProductBundleItems = helper.getBundleItems(this.StoreID, this.SProductID);
        }
    }
}
