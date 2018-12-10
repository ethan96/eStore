using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{

    public partial class Product
    {

        #region Extension Methods
        // ProductStatus shall be defined in es_StoreProduct in POCOS layer
        // This is just a temporary holder
        public enum PRODUCTSTATUS { GENERAL, BUNDLE, SOLUTION_ONLY, PHASED_OUT, INACTIVE, DELETED, INACTIVE_AUTO, COMING_SOON, TOBEREVIEW  /*this state will be specified by sync job*/};
        public enum PRODUCTMARKETINGSTATUS 
        {
            NEW = 1,
            HOT = 2,
            PROMOTION = 3,
            CLEARANCE = 4,
            TOBEPHASEOUT = 5,
            FEATURE = 6,
            Inquire = 7,

            EXPRESS = 16,
            TwoDaysFastDelivery = 17,
            TwoWeeksFastDelivery = 18,
            ThreeDayFastDelivery = 19,
            FiveDayFastDelivery = 20,
            TenDayFastDelivery = 21

        }
        public enum PUBLISHSTATUS { PUBLISHED, NOT_PUBLISHED };
        public enum PRODUCTTYPE { STANDARD, BUNDLE, CTOS, WARRANTY, APPMARKETPLACE };
        public enum PRICINGMODE { SPECIAL, REGULAR, NOTAVAILABLE };
        private List<ProductCategory> _productCategories = null;
        private List<String> _categoryIDList = null;
        //private PromotionPolicy _promotionPolicy = null;
        private Store _storeX = null;   //this attribute will be only initialize per need


        /// <summary>
        /// default constructor
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// This construction will populate input part properties to itself
        /// </summary>
        /// <param name="part"></param>
        public Product(Part part)
        {
            if (part != null)
            {
                // copy base class properties.
                foreach (PropertyInfo prop in part.GetType().GetProperties())
                {
                    if (prop.CanWrite)
                    {
                        PropertyInfo target = this.GetType().GetProperty(prop.Name);
                        Object obj = prop.GetValue(part, null);
                        target.SetValue(this, obj, null);
                    }
                }
            }
        }

        //attributes
        /// <summary>
        /// publishStatus is to reflect where this product is visible at eStore front end or not.  
        /// PUBLISHED means the product is visible and searchable at eStore
        /// NOT_PUBLISHED means the product is still in review stage and will not be available at eStore
        /// </summary>
        public virtual PUBLISHSTATUS publishStatus
        {
            get
            {
                if (PublishStatus)
                    return PUBLISHSTATUS.PUBLISHED;
                else
                    return PUBLISHSTATUS.NOT_PUBLISHED;
            }

            set
            {
                if (value == PUBLISHSTATUS.PUBLISHED)
                    this.PublishStatus = true;
                else
                    this.PublishStatus = false;
            }
        }


        /// <summary>
        /// This property allows use to get and set last order status
        /// </summary>
        public virtual PRODUCTSTATUS status
        {
            get
            {
                if (stringDictionary.ContainsKey(this.Status))
                    return (PRODUCTSTATUS)stringDictionary[this.Status];
                else
                    return PRODUCTSTATUS.INACTIVE;
            }

            set { this.Status = enumDictionary[value]; }
        }

        public virtual List<PRODUCTMARKETINGSTATUS> marketingstatus
        {
            get {
                if (this.MarketingStatus == 0)
                    return new List<PRODUCTMARKETINGSTATUS>();
                else
                {
                    List<PRODUCTMARKETINGSTATUS> mss = new List<PRODUCTMARKETINGSTATUS>();
                    foreach (PRODUCTMARKETINGSTATUS ms in Enum.GetValues(typeof(PRODUCTMARKETINGSTATUS)))
                    {
                        if (isBitOn(MarketingStatus, (int)ms))
                            mss.Add(ms);
                    }
                    return mss;
                }
            }
            set
            {
                int markstatus = 0;
                foreach (PRODUCTMARKETINGSTATUS ms in value)
                {
                    setBitOn(ref markstatus, (int)ms, true);
                }
                MarketingStatus = markstatus;
            }
        }
        public bool isBitOn(int value, int bitIndex)
        {
            int mask = 1;
            if (bitIndex > 0)
                mask = mask << bitIndex;
            return ((value & mask) == 0) ? false : true;
        }
        protected void setBitOn(ref int value, int bitIndex, bool bOn)
        {
            int mask = 1;
            if (bitIndex > 0)
                mask = mask << bitIndex;
            if (bOn)    //use OR
                value = value | mask;
            else   //use AND NOT
                value = value & ~mask;
        }

        public bool isIncludeSatus(PRODUCTMARKETINGSTATUS item)
        {
            int markSt = (int)Math.Pow(2, (int)item);
            return (MarketingStatus & markSt) == markSt;
        }

        public void addMarketingstatus(PRODUCTMARKETINGSTATUS item)
        {
            var ls = marketingstatus;
            ls.Add(item);
            marketingstatus = ls;
        }

        public List<PRODUCTMARKETINGSTATUS> salesMarketings
        { 
            get
            {
                var ls = new List<PRODUCTMARKETINGSTATUS>();
                foreach (var item in marketingstatus)
                {
                    if ((int)item < 16)
                        ls.Add(item);
                }
                return ls;
            }
        }

        public List<PRODUCTMARKETINGSTATUS> deliveryMarketings
        {
            get
            {
                var ls = new List<PRODUCTMARKETINGSTATUS>();
                foreach (var item in (PRODUCTMARKETINGSTATUS[])Enum.GetValues(typeof(PRODUCTMARKETINGSTATUS)))
                { 
                    if((int)item > 15 && isIncludeSatus(item))
                        ls.Add(item);
                }
                return ls;
            }
        }

        public List<ProductBatchStatusMapping> GetSimpleMarketingStatusMapping(bool withAfterBegin = false)
        {
            var helper = new ProductBatchStatusMappingHelper();
            List<ProductBatchStatusMapping> ls;
            if(withAfterBegin)
                ls = helper.GetBatchStatusMappingWithAfterBegin(this);
            else
                ls = helper.GetBatchStatusMappingByProduct(this);
            foreach (var item in ls)
            {
                if (item.BatchId.HasValue)
                {
                    item.StartDate = item.ProductBatchStatu.StartDate;
                    item.EndDate = item.ProductBatchStatu.EndDate;
                    item.MarketingStatus = item.ProductBatchStatu.MarketingStatus;
                }
            }
            return ls;
        }

        public void UpdateProductBatchStatus()
        {
            var promappings = GetSimpleMarketingStatusMapping();
            var statustep = new List<eStore.POCOS.Product.PRODUCTMARKETINGSTATUS>();
            foreach (var mapp in promappings)
            {
                Product protem = new Product { MarketingStatus = mapp.MarketingStatus.GetValueOrDefault() };
                foreach (var satuse in protem.marketingstatus)
                {
                    if (!statustep.Contains(satuse))
                        statustep.Add(satuse);
                }
            }
            this.marketingstatus = statustep;
        }

        /// <summary>
        /// To set or retrieve product name
        /// </summary>
        public virtual String name
        {
            get { return this.DisplayPartno; }
            set { this.DisplayPartno = value; }
        }

        public virtual PRODUCTTYPE productType
        {
            get
            {
                PRODUCTTYPE type;

                switch (ProductType)
                {
                    case "CTOS":
                        type = PRODUCTTYPE.CTOS;
                        break;
                    case "BUNDLE":
                        type=PRODUCTTYPE.BUNDLE;
                        break;
                    case "APPMARKETPLACE":
                        type = PRODUCTTYPE.APPMARKETPLACE;
                        break;

                    case "Virtual":
                        if (this is Product_Bundle)
                            type = PRODUCTTYPE.BUNDLE;
                        else if (this is Product_Ctos)
                            type = PRODUCTTYPE.CTOS;
                        else

                            type = PRODUCTTYPE.STANDARD;

                        break;
                    case "FG":
                    case "Simple":
                    case "STANDARD":
                    default:
                        type = PRODUCTTYPE.STANDARD;
                        break;
                }

                return type;
            }

            set
            {
                switch (value)
                {
                    case PRODUCTTYPE.CTOS:
                        ProductType = "CTOS";
                        break;
                    case PRODUCTTYPE.BUNDLE:
                        ProductType = "BUNDLE";
                        break;
                    case PRODUCTTYPE.APPMARKETPLACE:
                        ProductType = "APPMARKETPLACE";
                        break;
                    case PRODUCTTYPE.STANDARD:
                    default:
                        ProductType = "STANDARD";
                        break;
                }
            }

        }

        /// <summary>
        /// To check if product price shall be available for end user or not.
        /// </summary>
        private Boolean priceEnable
        {
            get { return ShowPrice; }
            set { this.ShowPrice = value; }
        }



        public virtual Decimal promotionPrice
        {
            get { return PromotePrice.GetValueOrDefault(); }
            set { this.PromotePrice = value; }
        }

        public virtual Decimal promotionMarkUpPrice
        {
            get { return PromoteMarkup.GetValueOrDefault(); }
            set { this.PromoteMarkup = value; }
        }

        public virtual Nullable<DateTime> promotionStart
        {
            get { return this.PromoteStart; }
            set { this.PromoteStart = value; }
        }

        public virtual Nullable<DateTime> promotionEnd
        {
            get { return this.PromoteEnd; }
            set { this.PromoteEnd = value; }
        }

        public virtual int clearanceThreshValue
        {
            get { return ClearanceThreshold.GetValueOrDefault(); }
            set { this.ClearanceThreshold = value; }
        }


        /// <summary>
        /// This function return a boolean value to indicate if this product is offering special pricing.
        /// When the return value is true, caller can get the original price from priceBeforeAdjustment.
        /// eStore front end application shall use this method as main method of pricing checking
        /// 
        /// listingPrice
        /// </summary>
        /// <param name="listingPrice">
        ///     If product offers special pricing (in either promotion or clearance stage)
        ///         listingPrice will be the lower price of promotion price and regular listing price
        ///     otherwise
        ///         listingPrice will be the regular listing price
        ///         
        ///     ** regular listing price can be either store local price or vendor suggesting price depending
        ///     ** on priceSource setting
        /// </param>.
        /// <param name="priceBeforeAdjustment">
        ///     This is a optional parameter.  If caller passes in this parameter, its value will be filled up
        ///     if the product offers special pricing.
        ///     The return value will be higher price of promotion markup price and regular listing price
        /// </param>
        /// <returns>
        ///     Pricing mode : special price, regular price or price not available
        /// </returns>
        public virtual PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment = null)
        {
            PRICINGMODE priceMode = PRICINGMODE.NOTAVAILABLE; //default as not-available

            if (priceBeforeAdjustment == null)  //caller doesn't care of priceBeforeAdjustment
            {
                //create a local one for calculation purpose
                priceBeforeAdjustment = new Price();
            }

            //check product status and pricing setting
            if (!this.priceEnable || !isOrderable())
            {
                listingPrice.value = 0;
                return PRICINGMODE.NOTAVAILABLE;
            }

            //get regular listing price here
            listingPrice.value = getProductNetPrice().value;

            if (listingPrice.value <= 0)    //invalid listing price
                return PRICINGMODE.NOTAVAILABLE;  //price not available

            //get promotion price if necessary
            if (hasSpecialPrice())
            {
                priceMode = PRICINGMODE.SPECIAL;
                if (PromoteMarkup.GetValueOrDefault() > listingPrice.value)
                    priceBeforeAdjustment.value = PromoteMarkup.GetValueOrDefault();
                else
                    priceBeforeAdjustment.value = listingPrice.value;

                //validate pricing strategy to make sure priceBeforeAdjustment is at least 2% higher than listing price
                if (priceBeforeAdjustment.value * 0.98m >= PromotePrice.GetValueOrDefault())  //good case
                {
                    listingPrice.value = PromotePrice.GetValueOrDefault();
                    priceMode = PRICINGMODE.SPECIAL;
                }
                else
                {
                    priceMode = PRICINGMODE.REGULAR;  //though the product is in promotion mode, but the price strategy is not valid.
                    priceBeforeAdjustment.value = -1;   //price not available
                }
            }
            else
            {
                PromotionPolicy promotionPolicy = getPromotionPolicy();
                if (promotionPolicy != null)
                {
                    priceMode = PRICINGMODE.SPECIAL;
                    priceBeforeAdjustment.value = listingPrice.value;
                    switch (promotionPolicy.discountValueType)
                    {
                        case PromotionPolicy.DiscountValueType.Rate:
                        case PromotionPolicy.DiscountValueType.FixValue:
                            listingPrice.value = listingPrice.value - calculateDiscount(promotionPolicy, listingPrice.value);
                            break;
                        default:
                            priceMode = PRICINGMODE.REGULAR;  //though the product is in promotion mode, but the price strategy is not valid.
                            priceBeforeAdjustment.value = -1;   //price not available
                            break;  //do nothing
                    }
                }
                else
                    priceMode = PRICINGMODE.REGULAR;
            }

            listingPrice.value = Converter.round(listingPrice.value, this.StoreID);
            priceBeforeAdjustment.value = Converter.round(priceBeforeAdjustment.value, this.StoreID);

            //if (this.CostX > listingPrice.value) // list price < cost will show call for price
            //    listingPrice.value = 0;

            if (this.CostX > listingPrice.value)
            {
                if (this.isIncludeSatus(PRODUCTMARKETINGSTATUS.CLEARANCE))
                {
                    //if CLEARANCE do nothing, keep promotion price
                }
                else if (this.isIncludeSatus(PRODUCTMARKETINGSTATUS.PROMOTION))
                {
                    listingPrice.value = getProductNetPrice().value;
                    if (listingPrice.value == 0)
                    {
                        priceMode = PRICINGMODE.NOTAVAILABLE;
                    }
                    else
                        priceMode = PRICINGMODE.REGULAR;
                }
                else
                {
                    priceMode = PRICINGMODE.NOTAVAILABLE;
                    listingPrice.value = 0;
                }
            }


            return priceMode;
        }

        public virtual PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment = null, UserGrade userGrade=null)
        {
           if(userGrade == null)
                return getListingPrice(listingPrice, priceBeforeAdjustment);

            if (this.partGradePricesX.Any())
            {
                var pgp = this.partGradePricesX.FirstOrDefault(x => x.PriceGrade == userGrade.PriceGrade && x.PriceGroup == userGrade.PriceGroup);
                if (pgp == null)
                {
                    return getListingPrice(listingPrice, priceBeforeAdjustment);
                }
                else
                {
                    return getListingPrice(listingPrice, priceBeforeAdjustment);
                }
            }
            else
            {
                return getListingPrice(listingPrice, priceBeforeAdjustment);
            }

        }
        private Decimal calculateDiscount(PromotionPolicy policy, Decimal amount)
        {
            Decimal discount = 0m;

            if (policy.discountValueType == PromotionPolicy.DiscountValueType.FixValue)
                discount = (policy.discountPrice > amount) ? amount : policy.discountPrice;
            else
            {
                discount = amount * (1.0m - policy.discountPrice);
                discount = (discount > amount) ? amount : discount;
            }

            return discount;
        }
        /// <summary>
        /// This method is to provide a convenient way for eStore presentation to get product or part price.
        /// This is a override method to replace the method implemented in Part.
        /// </summary>
        /// <returns></returns>
        public override Price getListingPrice()
        {
            Price listingPrice = new Price();

            PRICINGMODE mode = getListingPrice(listingPrice,null,null);

            return listingPrice;
        }

        public new Price getOriginalPrice()
        {
            Price price = new Price();
            //get regular listing price here
            switch (this.priceSourceX)
            {
                case PRICESOURCE.VENDOR:    //SAP for Advantech
                    price.value = applyPriceMargin(base.getOriginalPrice().value);
                    break;
                case PRICESOURCE.LOCAL:
                default:
                    if (this is Product_Ctos)   //only CTOS needs to apply margin on top of store price
                        price.value = applyPriceMargin(StorePrice);
                    else
                        price.value = StorePrice;
                    break;
            }
            return price;
        }

        private Price _price = null;
        /// <summary>
        /// This function is used to retrive product raw price without considering any promotion setting.
        /// This method is not available to other classes.
        /// </summary>
        /// <returns></returns>
        private Price getProductNetPrice()
        {
            if (_price == null)
            {
                _price = new Price();

                //get regular listing price here
                _price = getOriginalPrice();

                //every store may have its owner minimum price restriction.  Hereby price may need to be adjusted before return
                makeMinPriceAdjustment(_price);

                _price.value = Converter.round(_price.value, this.StoreID);
            }

            return _price;
        }


        /// <summary>
        /// This method is for CTOS to update runtime price after price recalculation when there are phased-out components and are replaced by their alternative components.
        /// </summary>
        /// <param name="newBasePrice"></param>
        /// <returns></returns>
        protected Price updateProductNetPrice(Price newBasePrice)
        {
            if (_price == null)
                _price = new Price();

            //get new listing price here
            _price.value = applyPriceMargin(newBasePrice.value);

            //every store may have its owner minimum price restriction.  Hereby price may need to be adjusted before return
            makeMinPriceAdjustment(_price);

            _price.value = Converter.round(_price.value, this.StoreID);

            return _price;
        }

        /// <summary>
        /// Different store may add margin on top of vendor suggesting price.  This function is to apply eStore margin rule on top
        /// vendor suggesting price.
        /// </summary>
        private Decimal applyPriceMargin(Decimal origPrice)
        {
            Decimal adjustedProductPrice = origPrice;

            //has margin adjustment rules
            if (priceAdjustRulesX.Count > 0)
            {
                //apply the first applicable rule
                Decimal adjustingRate = 0m;
                List<ProductCategory> categories = null;
                foreach (PriceAdjustRule rule in priceAdjustRulesX)
                {
                    switch (rule.MatchField)
                    {
                        case "DisplayPartno":
                            switch (rule.MatchType)
                            {
                                case "StartWith":
                                    if (this.DisplayPartno.StartsWith(rule.TypeID))
                                        adjustingRate = rule.Rate.GetValueOrDefault();
                                    break;
                                case "Contain":
                                    if (this.DisplayPartno.Contains(rule.TypeID))
                                        adjustingRate = rule.Rate.GetValueOrDefault();
                                    break;
                                case "Exact":
                                    if (this.DisplayPartno.Equals(rule.TypeID))
                                        adjustingRate = rule.Rate.GetValueOrDefault();
                                    break;
                            }
                            break;
                        case "CategoryPath":
                            //for standard product, it covers deep category match.  For CTOS product, it covers only shadow 1-level match
                            if (categories == null)
                                categories = (new ProductCategoryHelper()).getProductCategoryByPartno(this, StoreID);
                            if (hasApplicableCategory(rule, categories))
                                adjustingRate = rule.Rate.GetValueOrDefault();
                            break;
                        default:
                            break;  //do nothing
                    }

                    //to prevent mistakenly adjust product price to lower than our cost
                    if (adjustingRate > 0.7m)
                    {
                        adjustedProductPrice *= adjustingRate;
                        break;  //exist from loop when first adjusting rule is found
                    }
                }
            }

            return adjustedProductPrice;
        }

        /// <summary>
        /// This method 
        /// </summary>
        /// <returns></returns>
        private Boolean hasApplicableCategory(PriceAdjustRule rule, List<ProductCategory> categories)
        {
            Boolean applicable = false;
            try
            {
                int count = 0;
                switch (rule.MatchType)
                {
                    case "Exact":
                        count = (from category in categories
                                 where category.CategoryPath.Equals(rule.TypeID)
                                 select category).Count();
                        break;
                    case "StartWith":
                        count = (from category in categories
                                 where category.CategoryPath.StartsWith(rule.TypeID)
                                 select category).Count();
                        break;
                    case "Contain":
                        count = (from category in categories
                                 where category.CategoryPath.Contains(rule.TypeID)
                                 select category).Count();
                        break;
                }

                if (count > 0)
                    applicable = true;
            }
            catch (Exception)
            {
            }

            return applicable;
        }

        List<PriceAdjustRule> _priceAdjustRulesX = null;
        /// <summary>
        /// This run time property return filtered price adjusting rule if there is any
        /// </summary>
        private List<PriceAdjustRule> priceAdjustRulesX
        {
            get
            {
                if (_priceAdjustRulesX == null)
                {
                    if (storeX.PriceAdjustRules != null && storeX.PriceAdjustRules.Count > 0)
                    {
                        String typeValue = "STANDARD";
                        if (this is Product_Ctos)
                            typeValue = "CTOS";

                        try
                        {
                            _priceAdjustRulesX = (from rule in storeX.PriceAdjustRules
                                                  where rule.IsActive == 1 && rule.ProductType.Equals(typeValue)
                                                  orderby rule.Rate descending
                                                  select rule).ToList();
                        }
                        catch (Exception)
                        {
                            ;   //no matching rules, do nothing here
                        }
                    }

                    //create an empty list if there is no matched price adjusting rules
                    if (_priceAdjustRulesX == null)
                        _priceAdjustRulesX = new List<PriceAdjustRule>();
                }

                return _priceAdjustRulesX;
            }
        }

        /// <summary>
        /// Product has its store price other than the local price defined in Part.   The price source setting in Product and Part service for different purpose
        /// , though their function is similar.   Product local price will be from StorePrice and Part local price will be from LocalPrice.
        /// </summary>
        public PRICESOURCE priceSourceX
        {
            get
            {
                PRICESOURCE source = PRICESOURCE.VENDOR;
                if (String.IsNullOrEmpty(this.PriceSource))
                    source = PRICESOURCE.LOCAL;
                else
                {
                    switch (this.PriceSource.ToUpper())
                    {
                        case "VENDOR":
                            source = PRICESOURCE.VENDOR;
                            break;
                        case "LOCAL":
                        default:
                            source = PRICESOURCE.LOCAL;
                            break;
                    }
                }

                return source;
            }
        }

        public virtual Boolean hasSpecialPrice()
        {
            Boolean inSpecial = false;

            if ((isIncludeSatus(PRODUCTMARKETINGSTATUS.CLEARANCE) || isIncludeSatus(PRODUCTMARKETINGSTATUS.PROMOTION)) &&
                (PromoteStart.GetValueOrDefault() <= DateTime.Now && PromoteEnd.GetValueOrDefault() >= DateTime.Now) &&
                (PromotePrice.GetValueOrDefault() > 0))
            {
                //check inventory to make sure there are goods for sale
                if (ClearanceThreshold.GetValueOrDefault() > 0)
                {
                    if (getInventoryCount() >= ClearanceThreshold.GetValueOrDefault())
                        inSpecial = true;
                    else
                        inSpecial = false;
                }
                else   //offering special price without inventory concern
                    inSpecial = true;
            }

            return inSpecial;
        }

        /// <summary>
        /// This method is used to check up if product is assigned to a particular product category. A product can belong to multiple
        /// product categories depending on how PM associate them with Product categories
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public virtual Boolean belongProductCategory(String categoryId)
        {
            Boolean belong = false;

            try
            {
                if (categoryIDList.Contains(categoryId))
                    belong = true;
            }
            catch (Exception)
            {
                //nothing important be handled here
            }

            return belong;
        }


        public virtual PromotionPolicy getPromotionPolicy()
        {

            PromotionPolicy _promotionPolicy;
            //check if there is any no-coupon-code-required promotion
            _promotionPolicy = CampaignManager.getInstance().getMostApplicableProductPromotionPolicy(this);

            if (_promotionPolicy == null)
            {
                //create a temporary hold for preventing performance hit
            }

            return _promotionPolicy;
        }

        /// <summary>
        /// This property is an overridable property. It returns more informative description if eStore owner enter it
        /// </summary>
        public override string productDescX
        {
            get
            {
                String desc = null;

                if (String.IsNullOrWhiteSpace(ProductDesc))
                    desc = base.productDescX;
                else
                    desc = esUtilities.CommonHelper.replaceSpecialSymbols(ProductDesc);

                return desc;
            }
        }

        /// <summary>
        /// This property is to replace VendorFeatures and ProductFeatures
        /// </summary>
        public override string productFeatures
        {
            get
            {
                if (String.IsNullOrEmpty(ProductFeatures))
                {
                    ProductFeatures = base.productFeatures;
                    return ProductFeatures;
                }
                else
                    return esUtilities.CommonHelper.replaceSpecialSymbols(ProductFeatures);
            }
        }

        /// <summary>
        /// This method is to provide presentation layer which contact group this product
        /// shall belongs to.
        /// </summary>
        public virtual Store.BusinessGroup businessGroup
        {
            get
            {
                if (this.StoreID == "AEU" || this.StoreID == "AUS")
                {
                    if (this.productCategories != null && this.productCategories.Any(x => x.MiniSite == null))
                    {
                        return this.productCategories.First(x => x.MiniSite == null).businessGroup;
                    }
                    else
                    {
                        if (isEAProduct())
                            return Store.BusinessGroup.eA;
                        else
                            return Store.BusinessGroup.eP;
                    }

                }
                else
                {
                    if (isEAProduct())
                        return Store.BusinessGroup.eA;
                    else
                        return Store.BusinessGroup.eP;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<String> categoryIDList
        {
            get
            {
                if (_categoryIDList == null) //not initialized yet
                {
                    if (productCategories.Any()==false)
                    {
                        _categoryIDList = new List<String>();
                    }
                }

                return _categoryIDList;
            }
        }

        /// <summary>
        /// productCategories is a runtime property i
        /// set用于批量从db获取资料以提高速度
        /// </summary>
        public List<ProductCategory> productCategories
        {
            get
            {
                if (_productCategories == null) //not initialized yet
                {
                    List<ProductCategory> categoryCandidates = (new ProductCategoryHelper()).getProductCategoryByPartno(this, StoreID);

                    _productCategories = getdeepestCategories(categoryCandidates);
                }
                return _productCategories;
            }
            set {
                _productCategories = getdeepestCategories(value);
            }
        }

        public List<ProductCategory> getdeepestCategories(List<ProductCategory> categoryCandidates)
        {
            List<ProductCategory> _productCategories = new List<ProductCategory>();
            _categoryIDList = new List<string>();
            if (categoryCandidates != null) //select only the categories that have unique and deepest category path.
            {
                //filter duplicated categories based on full category path
                List<String> paths = new List<String>();
                foreach (ProductCategory category in categoryCandidates)
                {
                    ProductCategory explorer = category;
                    String fullCategoryPath = "";
                    //the fullCategoryPath pattern will be xxxx.yyyy.zzzz.
                    foreach (ProductCategory current in category.categoryHierarchy)
                    {
                        if (!_categoryIDList.Any(x => x == current.CategoryPath))
                            _categoryIDList.Add(current.CategoryPath);

                        fullCategoryPath += current.CategoryPath + ".";
                    }

                    paths.Add(fullCategoryPath);
                }

                //order by path length
                paths = (from path in paths
                         orderby path.Length
                         select path).ToList();

                for (int loop1 = 0; loop1 < paths.Count(); loop1++)
                {
                    Boolean shortPath = false;
                    String path = paths[loop1];
                    for (int loop2 = loop1 + 1; loop2 < paths.Count; loop2++)
                    {
                        if (paths[loop2].StartsWith(path))
                        {
                            shortPath = true;
                            break;
                        }
                    }
                    if (shortPath)
                    {
                        //not the deepest category path
                        paths[loop1] = null;    //remove it from the candidate list
                    }
                }

                //find the categories having deepest category path here
                foreach (String path in paths)
                {
                    if (!String.IsNullOrEmpty(path))
                    {
                        try
                        {
                            String[] categoryIDs = path.Split('.');
                            String categoryID = categoryIDs[categoryIDs.Count() - 2];
                            ProductCategory category = (from item in categoryCandidates
                                                        where item.CategoryPath.Equals(categoryID)
                                                        select item).FirstOrDefault();
                            if (category != null)
                                _productCategories.Add(category);
                        }
                        catch (Exception ex)
                        {
                            //unexpected issue happen
                            eStoreLoger.Error("Exception at getting product's category list", "", "", "", ex);
                        }
                    }
                }
            }


            return _productCategories;
        }

        public void reSetProductCategories()
        {
            _productCategories = null;
        }

        public override String thumbnailImageX
        {
            get
            {
                if (String.IsNullOrWhiteSpace(ImageURL) || VendorID == "Local")
                    return base.thumbnailImageX;
                else
                    return this.ImageURL;
            }
        }

        public override string dataSheetX
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.DataSheet))
                    this.DataSheet = base.dataSheetX;
                return this.DataSheet;
            }
        }

        private int? _defaultWarrantyYear;
        public virtual int? defaultWarrantyYear
        {
            get
            {
                if (_defaultWarrantyYear.HasValue == false)
                {
                    if (this.WarrantyYear.HasValue && this.WarrantyYear.GetValueOrDefault()>=0)
                    {
                        _defaultWarrantyYear = this.WarrantyYear.GetValueOrDefault();
                    }
                    else if (this.productType==PRODUCTTYPE.STANDARD && this.categoryIDList.Any(x => x.Equals("MstrCATE_EAPRO_RSCOMM",StringComparison.OrdinalIgnoreCase)))
                    {
                        _defaultWarrantyYear = 5;
                    }
                    else
                    {
                        _defaultWarrantyYear = 2;
                    }
                }
                return _defaultWarrantyYear;
            }
        }



        /// <summary>
        /// The following three prooperties are for product detail page SEO purpose
        /// </summary>
        public String pageTitle
        {
            get
            {
                if (!String.IsNullOrEmpty(PageTitle))
                    return PageTitle;
                else
                    return this.DisplayPartno + "-" + this.ProductDesc;
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
                if (!String.IsNullOrEmpty(Keywords))
                    return Keywords;
                else if (!string.IsNullOrEmpty(DisplayPartno))
                    return DisplayPartno + "," + SProductID;
                else
                    return SProductID;
            }
        }

        public String metaData
        {
            get
            {
                if (!String.IsNullOrEmpty(PageDescription))
                    return PageDescription;
                else
                    return this.productDescX;
            }
        }


        public ProductCategory GetDefalutCategory(MiniSite minisite)
        {
            ProductCategory category;
            var mappingls = (new POCOS.DAL.ProductCategroyMappingHelper()).GetMappingByProduct(this);
            var defaultmapping = mappingls.FirstOrDefault(c => c.Default == true && c.ProductCategory.MiniSite == minisite);
            if (defaultmapping != null)
                category = defaultmapping.ProductCategory;
            else
                category = this.productCategories.FirstOrDefault(x => x.MiniSite == minisite);
            return category;
        }


        private bool? _hasInitSEOPath;
        private void initSEOPath(MiniSite minisite)
        {
            try
            {
                ProductCategory category = GetDefalutCategory(minisite);
                if (category!=null )
                {
                    seopath1 = category.SEOPath1;
                    seopath2 = category.SEOPath2;
                }
                else
                {
                    seopath1 = this.keywords;
                    seopath2 = this.name;
                }

            }
            catch (Exception)
            {
                
                throw;
            }
            _hasInitSEOPath = true;
        }
        private string seopath1 = string.Empty;
        private string seopath2 = string.Empty;
        public string SEOPath1(MiniSite minisite)
        {

            if (_hasInitSEOPath.HasValue == false)
            {
                initSEOPath(minisite);
            }
            return seopath1;

        }
        public string SEOPath2(MiniSite minisite)
        {

            if (_hasInitSEOPath.HasValue == false)
            {
                initSEOPath(minisite);
            }
            return seopath2;

        }


        /// <summary>
        /// Actually product has a navigation property, Store, already, but this property is not an efficient way of getting Store instance.
        /// The reason is Store property will reconstruct Store instance for every product instance.  Not to overload memory usage and
        /// to have better system performance, a new property, storeX, is introduced.  This runtime property will retrieve Store instance
        /// from cache if it's already loaded.  Otherwise, it will retrieve store instance from DB as usual.
        /// </summary>
        public Store storeX
        {
            get
            {
                if (_storeX == null)
                    _storeX = (new StoreHelper()).getStorebyStoreid(this.StoreID);

                return _storeX;
            }
        }

        private static Dictionary<String, PRODUCTSTATUS> _stringDictionary = null;
        private static Dictionary<PRODUCTSTATUS, String> _enumDictionary = null;
        private static String _mutex = "MUTEX";
        private Dictionary<PRODUCTSTATUS, String> enumDictionary
        {
            get
            {
                if (_enumDictionary == null)
                    initDictionary();

                return _enumDictionary;
            }
        }

        private Dictionary<String, PRODUCTSTATUS> stringDictionary
        {
            get
            {
                if (_stringDictionary == null)
                    initDictionary();

                return _stringDictionary;
            }
        }

        private void initDictionary()
        {
            lock (_mutex)
            {
                try
                {
                    if (_enumDictionary == null)
                        _enumDictionary = new Dictionary<PRODUCTSTATUS, string>();
                    if (_stringDictionary == null)
                        _stringDictionary = new Dictionary<string, PRODUCTSTATUS>();

                    foreach (int value in Enum.GetValues(typeof(PRODUCTSTATUS)))
                    {
                        PRODUCTSTATUS status = (PRODUCTSTATUS)value;
                        String name = Enum.GetName(typeof(PRODUCTSTATUS), status);
                        if (!_enumDictionary.ContainsKey(status))
                            _enumDictionary.Add(status, name);
                        if (!_stringDictionary.ContainsKey(name))
                            _stringDictionary.Add(name, status);
                    }
                }
                catch (Exception)
                {
                    _enumDictionary = null;
                    _stringDictionary = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="individual">if individual is true, HDD can't be orderable singly</param>
        /// <returns></returns>
        public override Boolean isOrderable(bool individual = false)
        {
            Boolean orderable = true;

            if (this.PublishStatus == false)
                orderable = false;
            else if (!base.isOrderable(individual))
            {
                return false;
            }
            else
            {
                switch (status)
                {
                    case PRODUCTSTATUS.DELETED:
                    case PRODUCTSTATUS.INACTIVE:
                    case PRODUCTSTATUS.INACTIVE_AUTO:
                    case PRODUCTSTATUS.COMING_SOON:
                    case PRODUCTSTATUS.TOBEREVIEW:
                        orderable = false;
                        break;
                    case PRODUCTSTATUS.PHASED_OUT:
                        orderable = !notAvailable;
                        break;
                    default:
                        orderable = true;
                        break;
                }

           }

            return orderable;
        }

        public override bool isOrderableBase(bool individual = false)
        {
            if (isBelowCost)
                return false;
            return base.isOrderable(individual);
        }

        public override bool isTOrPParts()
        {
            return false;
        }
        private List<POCOS.Part> suggestingProducts;



        public List<POCOS.Part> SuggestingProducts
        {

            get
            {
                if (suggestingProducts != null)
                    return suggestingProducts;

                try
                {
                    MyDataMining.MyDataMining myDataMining = new MyDataMining.MyDataMining();

                    System.Data.DataTable dt = myDataMining.GetBasketAnalysis(this.SProductID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        suggestingProducts = new List<Part>();
                        PartHelper helper = this.parthelper;
                        if (helper == null) //this condition shall not exist
                            helper = new PartHelper();
                        Part part = null;

                        //prefetch parts for performance boost
                        StringBuilder parts = null;
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            if (parts == null)  //first part list
                            {
                                parts = new StringBuilder();
                                parts.Append(row[0].ToString());
                            }
                            else
                                parts.Append(",").Append(row[0].ToString());
                        }

                        helper.prefetchPartList(this.StoreID, parts.ToString());

                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            part = helper.getPart(row[0].ToString(), this.StoreID);
                            if (part != null && part is POCOS.Product)
                                suggestingProducts.Add(part);
                        }

                        //pick up only the top 5 products
                        suggestingProducts = (from product in suggestingProducts
                                              orderby product.getListingPrice().value descending
                                              select product).Take(5).ToList<Part>();
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Exception at getting Suggesting Products", "", "", "", ex);
                }

                return suggestingProducts;
            }
        }

        private List<Product> _replaceProductsX;
        public List<Product> ReplaceProductsX
        {
            get
            {
                if (_replaceProductsX == null)
                {
                    if (this.ReplaceProducts != null)
                    {
                        _replaceProductsX = (from p in ReplaceProducts
                                             where p.Status.ToUpper() == "ACTIVE"
                                             orderby p.CreatedDate descending
                                             select p.ReplacedProduct).ToList();
                    }

                    if (_replaceProductsX == null)
                        _replaceProductsX = new List<POCOS.Product>();
                }

                return _replaceProductsX;
            }
        }

        private List<Product> _replaceProductsXX;
        public List<Product> ReplaceProductsXX
        {
            get
            {
                if (_replaceProductsXX == null)
                {
                    _replaceProductsXX = new List<POCOS.Product>();
                    List<string> repalaceItemList = new List<string>();
                    repalaceItemList.Add(this.name);
                    foreach (Product replaceItem in ReplaceProductsX)
                        generateRepalceProduct(replaceItem, ref _replaceProductsXX, ref repalaceItemList);
                }
                return _replaceProductsXX;
            }
        }

        //foreach repalce product
        private void generateRepalceProduct(Product repalceProduct,ref List<Product> replaceProductList,ref List<string> replaceItemList)
        {
            //var existsProduct = replaceProductList.FirstOrDefault(p => p.name == repalceProduct.name);
            Boolean isExists = replaceItemList.Contains(repalceProduct.name);
            if (!repalceProduct.isOrderable() && repalceProduct.ReplaceProductsX.Count > 0 && !isExists)
            {
                replaceItemList.Add(repalceProduct.name);
                foreach (POCOS.Product item in repalceProduct.ReplaceProductsX)
                {
                    if (!item.isOrderable() && item.ReplaceProductsX.Count > 0)
                        generateRepalceProduct(item, ref replaceProductList,ref replaceItemList);
                    else
                    {
                        if (!replaceItemList.Contains(item.name))
                        {
                            replaceProductList.Add(item);
                            replaceItemList.Add(item.name);
                        }
                    }
                }
            }
            else if (!isExists)
            {
                replaceProductList.Add(repalceProduct);
                replaceItemList.Add(repalceProduct.name);
            }
        }

        private List<Part> _allDependencyProduct;
        public List<Part> allDependencyProduct
        {
            get
            {
                if (_allDependencyProduct == null)
                {
                    _allDependencyProduct = new List<Part>();
                    if (this.dependentPartsX.Count > 0) // add its own dependency parts
                        _allDependencyProduct.AddRange(this.dependentPartsX);

                    // add its peripheral product dependency parts
                    if (this.PeripheralCompatibles != null && this.PeripheralCompatibles.Count > 0)
                    {
                        foreach (var p in this.PeripheralCompatibles)
                        {
                            if (!string.IsNullOrEmpty(p.PeripheralProduct.SProductID))
                            {
                                foreach (Part peripheralPart in p.PeripheralProduct.partsX)
                                {
                                    foreach (Part dependPartm in peripheralPart.dependentPartsX)
                                    {
                                        Part _exitPart = _allDependencyProduct.FirstOrDefault(c => c.SProductID == dependPartm.SProductID);
                                        if (_exitPart == null)
                                            _allDependencyProduct.Add(dependPartm);
                                    }
                                }
                            }
                        }
                    }

                    // add its Related Products dependency parts 
                    if (this.RelatedProductsX != null && this.RelatedProductsX.Count > 0)
                    {
                        foreach (var r in this.RelatedProductsX)
                        {
                            foreach (var rc in r.RelatedPart.dependentPartsX)
                            {
                                var _exitPart = _allDependencyProduct.FirstOrDefault(c => c.SProductID == rc.SProductID);
                                if (_exitPart == null)
                                    _allDependencyProduct.Add(rc);
                            }
                            
                        }
                    }
                }
                return _allDependencyProduct;
            }
        }

        /// <summary>
        /// add produt price whit warranty price
        /// </summary>
        /// <param name="_waranty"></param>
        /// <returns></returns>
        public decimal getPriceWhitWarantyPrice(Part _waranty)
        {
            if (isWarrantable())
                return this.getProductNetPrice().value + getWarantyPrice(_waranty);
            else
                return Converter.round(this.getProductNetPrice().value, this.StoreID);
        }

        /// <summary>
        /// get product waranty price do not with product price
        /// </summary>
        /// <param name="_waranty">waranty part</param>
        /// <returns></returns>
        public decimal getWarantyPrice(Part _waranty)
        {
            if (isWarrantable())
                return Converter.round(this.getProductNetPrice().value * _waranty.getNetPrice(false).value / 100, this.StoreID);
            else
                return 0;
        }


        private List<ProductWidgetMapping> _widgetPages = null;
        /// <summary>
        /// product mapping widget page
        /// </summary>
        public List<ProductWidgetMapping> widgetPagesX
        {
            get 
            {
                if (_widgetPages == null)
                {
                    ProductWidgetMappingHelper helper = new ProductWidgetMappingHelper();
                    _widgetPages = helper.getMappingListBySproductId(this.SProductID, this.StoreID);
                }
                return _widgetPages;
            }
        }


        private int _categorySeq = 99;
        //在category中的排序
        public int CategorySeq
        {
            get { return _categorySeq; }
            set { _categorySeq = value; }
        }
        

        #endregion

        #region OM Extension Methods

        /// <summary>
        /// This property indicates if current product has all mandatory data for publish to eStore. 
        /// It's mainly used in OM to check product information readiness.
        /// The validation criteria is as following
        /// 1. DisplayPartno, Status, ProductDesc, ProductFeatures
        /// 2. ImageURL, CreatedBy, DisplayPartNo, (DimensionHeight/Width/Length commented out)
        /// 3. Price and Product status
        /// 4. Product features and data sheets
        /// </summary>
        public override Boolean validForPublish
        {
            get
            {
                //perform the most basic validation
                //DisplayPartno, Status, ProductDesc, ProductFeatures
                validate();

                //additional validation
                if (String.IsNullOrEmpty(this.TumbnailImageID) && string.IsNullOrEmpty(this.ImageURL))
                    error_message.Add(new PocoX.ErrorMessage("ImageURL", "Missing product image information"));
                if (String.IsNullOrEmpty(this.CreatedBy))
                    error_message.Add(new PocoX.ErrorMessage("CreatedBy", "Need to record who enroll this product to eStore"));
                if (String.IsNullOrEmpty(this.DisplayPartno))
                    error_message.Add(new PocoX.ErrorMessage("DisplayPartno", "Product display name can not be empty"));
                /*
                if (this.DimensionHeightCM.GetValueOrDefault() == 0 || 
                    this.DimensionLengthCM.GetValueOrDefault() == 0 ||
                    this.DimensionWidthCM.GetValueOrDefault() == 0)
                    error_message.Add(new PocoX.ErrorMessage("Product dimension height, width and length", "Missing product dimension info"));
                 */
                if (isMainStream() == true && String.IsNullOrEmpty(this.ModelNo) && this.VendorID != "Local")
                    error_message.Add(new PocoX.ErrorMessage("ModelNo", "Product Model no can not be empty"));
                ////validing price setting 
                // allow to set local price 0
                //if (this.priceSourceX == PRICESOURCE.LOCAL && this.StorePrice == 0)
                //    error_message.Add(new PocoX.ErrorMessage("StorePrice", "Store price can not be 0 when price source is set to local"));
                if (this.priceSourceX == PRICESOURCE.VENDOR && this.VendorSuggestedPrice == 0)
                    error_message.Add(new PocoX.ErrorMessage("VendorSuggestedPrice", "Vendor suggestted price can not be 0 when price source is set to vendor"));
                if (this.status == PRODUCTSTATUS.DELETED ||
                    this.status == PRODUCTSTATUS.INACTIVE ||
                    this.status == PRODUCTSTATUS.INACTIVE_AUTO ||
                    this.status == PRODUCTSTATUS.PHASED_OUT ||
                    this.status == PRODUCTSTATUS.TOBEREVIEW)
                    error_message.Add(new PocoX.ErrorMessage("Status", "Product status is not eligible to add to eStore"));

                if (String.IsNullOrEmpty(this.productFeatures))
                    error_message.Add(new PocoX.ErrorMessage("ProductFeatures", "Product features can not be empty"));
                //validate product data sheet information
                var count = (from resource in ProductResources
                             where resource.ResourceType.ToUpper().Equals("DATASHEET")
                             select resource).DefaultIfEmpty().Count();
                if (count <= 0)
                    error_message.Add(new PocoX.ErrorMessage("Product data sheets", "Missing product data sheet information"));

                if (error_message.Count > 0)
                    return false;
                else
                    return true;
            }
        }

        public bool validateComingSoon()
        {
            error_message = new List<PocoX.ErrorMessage>();
            if (string.IsNullOrEmpty(DisplayPartno))
                error_message.Add(new PocoX.ErrorMessage("DisplayPartno", "DisplayPartno can not be Null "));
            if (string.IsNullOrEmpty(Status))
                error_message.Add(new PocoX.ErrorMessage("Status", "Status can not be Null "));
            if (string.IsNullOrEmpty(ProductDesc))
                error_message.Add(new PocoX.ErrorMessage("ProductDesc", "ProductDesc can not be Null "));
            if (string.IsNullOrEmpty(ProductFeatures))
                error_message.Add(new PocoX.ErrorMessage("ProductFeatures", "ProductFeatures can not be Null "));


            if (error_message.Count > 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// if product status is phased out will get part notAvailable
        /// </summary>
        public override bool notAvailable
        {
            get
            {
                //try to check part status first
                if (base.notAvailable)
                    return true;    //when part status is not available, then product shall also be unavailable

                //then check product status
                if (string.IsNullOrEmpty(this.Status))
                    return false;
                else
                {
                    if (this.Status == "O" && this.isIncludeSatus(PRODUCTMARKETINGSTATUS.TOBEPHASEOUT)) // 不能修改 part的StockStatus 但要做 StockStatus = O 的逻辑
                    {
                        if (hasInventoryIn60Days())
                            return false;
                        else
                            return true;
                    }

                    return false;   //This product shall be available
                }
            }
        }


        private Dictionary<Product, int> _categoryCrossSellProducts;
        public Dictionary<Product, int> categoryCrossSellProducts
        {
            get 
            {
                if (_categoryCrossSellProducts == null)
                {
                    //not init yet, start initialization
                    _categoryCrossSellProducts = new Dictionary<Product, int>();

                    //weight cross sell based on matching level
                    if (productCategories!= null && productCategories.Any() /* has element*/)
                    {
                        Dictionary<String, ProductCategory> categories = new Dictionary<String, ProductCategory>();
                        int maxLevel = productCategories.Max(c => c.categoryHierarchy.Count);

                        //loop through the categories this product belongs to collect complete category 
                        //list it directly or in-directly belongs to
                        foreach (ProductCategory category in productCategories)
                        {
                            //this logic is kind of tricky.  When there are more than 1 category the product directly belongs to
                            //and their hierarchy levels are different.  To assure evenly weights match level, the weight level may
                            //not always start from 1.  For example product A belong to category F and category M
                            //Case 1
                            //The category tree of CDEF is like F -> E -> D -> C and another category tree is like M -> K
                            //The weight level of each category will be like F(4) -> E(3) -> D(2) -> C(1) and M(4) -> K(3)
                            //Case 2
                            //If the category tree is like F -> E -> D -> C and M -> D
                            //The weight level will be F(4) -> E(3) -> D(2) -> C(1) and M(4) -> D(2)
                            int currentLevel = maxLevel - category.categoryHierarchy.Count + 1;
                            //loop through category hierarchy to collect indirect categories that product belongs to
                            foreach (ProductCategory innerCategory in category.categoryHierarchy)
                            {
                                String categoryPath = innerCategory.CategoryPath;
                                if (categories.ContainsKey(categoryPath))   //exist in current dictionary
                                {
                                    //compare weighting level value and assign the smaller value to match category
                                    if (categories[categoryPath].weightingLevel > currentLevel)
                                        categories[categoryPath].weightingLevel = currentLevel;
                                }
                                else
                                {
                                    innerCategory.weightingLevel = currentLevel;
                                    categories.Add(categoryPath, innerCategory); //add new entry to dictionary
                                }

                                currentLevel++;
                            }
                        }

                        //Hereby we are ready to build up weighted cross-sell product list
                        Dictionary<String, CrossSellItem> crossSellItems = new Dictionary<string,CrossSellItem>();
                        foreach (ProductCategory category in categories.Values)
                        {
                            if (category != null && category.CrossSellProducts != null)
                            {
                                foreach (CrossSellProduct crossSellProduct in category.CrossSellProducts)
                                {
                                    try
                                    {
                                        CrossSellItem item = null;
                                        int newWeight = (int)(crossSellProduct.Product.getListingPrice().value * category.weightingLevel);
                                        if (crossSellItems.ContainsKey(crossSellProduct.SProductID))
                                        {
                                            item = crossSellItems[crossSellProduct.SProductID];
                                            if (item.weight < newWeight)
                                                item.weight = newWeight;
                                        }
                                        else
                                        {
                                            item = new CrossSellItem(crossSellProduct.Product, newWeight);
                                            crossSellItems.Add(crossSellProduct.SProductID, item);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        //ignore this exception and move on
                                    }
                                }
                            }
                        }

                        //convert CrossSellItem to Cross sell product dictionary
                        foreach (CrossSellItem item in crossSellItems.Values)
                        {
                            _categoryCrossSellProducts.Add(item.product, item.weight);
                        }
                    }
                }

                return _categoryCrossSellProducts;
            }
        }


        public override bool isBelowCost
        {
            get
            {
                return CostX > getListingPrice().value;
            }
        }

        public virtual bool IsLocalProduct
        {
            get
            {
                return this.VendorID == null || this.VendorID.ToLower() == "local";
            }
        }


        #endregion

        #region Unit Test
        #endregion //Unit Test

        /// <summary>
        /// This inner class will only be used in Product class
        /// </summary>
        protected class CrossSellItem
        {
            public CrossSellItem(Product crossSellProduct, int weight)
            {
                this.product = crossSellProduct;
                this.weight = weight;
            }

            public Product product
            {
                get;
                set;
            }

            public int weight
            {
                get;
                set;
            }
        }

    }

}