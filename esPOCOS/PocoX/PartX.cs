using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Proxy;
using System.Collections;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.POCOS
{

    public partial class Part
    {
        public Part()
        { }

        public Part(SAPProduct sappro,Store store)
        {
            this.StoreID = store.StoreID;
            this.SProductID = sappro.PART_NO;
            this.StockStatus = sappro.STATUS;
            this.ModelNo = sappro.MODEL_NO;
            this.VendorProductLine = sappro.PRODUCT_LINE;
            this.VendorProductGroup = sappro.PRODUCT_GROUP;
            this.ShipWeightKG = sappro.SHIP_WEIGHT;
            this.NetWeightKG = sappro.NET_WEIGHT;
            this.ProductSite = sappro.ORG_ID;
            this.Dimension = sappro.Dimension;
            setDimension();
        }


        public void setDimension()
        {
            if (!string.IsNullOrWhiteSpace(this.Dimension))
            {
                Match matchResults = null;
                decimal width, length, height;
                try
                {
                    Regex regexObj = new Regex(@"^([0-9.]*)[CMWHLX*]+([0-9.]*)[CMWHLX*]+([0-9.]*)H?\(?([CM]*)\)?$");
                    matchResults = regexObj.Match(this.Dimension.Replace(" ","").Trim().ToUpper());
                    if (matchResults.Success)
                    {
                        if (decimal.TryParse(matchResults.Groups[1].Value, out length) &&
                           decimal.TryParse(matchResults.Groups[2].Value, out width) &&
                           decimal.TryParse(matchResults.Groups[3].Value, out height))
                        {
                           if(this.Dimension.ToUpper().Contains("CM"))
                            {
                                this.DimensionHeightCM = height;
                                this.DimensionLengthCM = length;
                                this.DimensionWidthCM = width;
                           }
                           else if ((width > 100 || length > 100 || height > 100)
                               ||this.Dimension.ToUpper().Contains("MM"))
                            {
                                this.DimensionHeightCM = height / 10;
                                this.DimensionLengthCM = length / 10;
                                this.DimensionWidthCM = width / 10;
                            }
                            else
                            {
                                this.DimensionHeightCM = height;
                                this.DimensionLengthCM = length;
                                this.DimensionWidthCM = width;
                            }
                        }
                    }

                }
                catch (ArgumentException)
                {
                    // Syntax error in the regular expression
                }
            }

        }

        public PartHelper parthelper;

        #region Extesion Properties
        /// <summary>
        /// Tag description
        /// 1. AssignedBoxPack - Assigned a box for a cart item
        /// 2. IndividualPack - 1 product in 1 box
        /// 3. SingleProductTypeMergePack - 
        /// 4. MultipleProductTypeMergePack     /** Reserved **/
        /// 5. WeightPack - If product weight is not existed, 
        /// 6. SpecialSizePack - Special
        /// </summary>
        public enum PackingTag
        {
            IndividualPack,
            SingleProductTypeMergePack,
            WeightPack,
            MultipleProductTypeMergePack,
            CtosPack
        };
        private PackingTag packTag;
        public PackingTag PackTag
        {
            get
            {
                return packTag;
            }
            set
            {
                packTag = value;
            }
        }

        public bool hasProductPriceChanged = false;
        public bool hasProductStockStatusChanged = false;
        public bool hasProductInfoChanged = false;
        public bool isSAPParts = true;
        // this property will serve for recording whether part content is updated or not.  If this flag is turned on,
        // part shall be saved to assure DB information is up to date.
        public Boolean isUpdated = false;

        private List<VProductMatrix> specList = null;
        private Store _storeX = null;
        protected Store storeX
        {
            get
            {
                if (_storeX == null)
                    _storeX = new StoreHelper().getStorebyStoreid(this.StoreID);

                return _storeX;
            }

            set { _storeX = value; }
        }

        #endregion

#region Extension Methods
        // ProductStatus shall be defined in es_StoreProduct in POCOS layer
        // This is just a temporary holder
        public enum PARTSTATUS { /*valid for ordre statues*/ ACTIVE, TO_BE_PHASED_OUT, HOLD_SHIPMENT, SALES_LIMIT, 
                                                        /*invalid for order status*/ PHASED_OUT, INACTIVE, DELETED, OTHERS };
        public enum PRICESOURCE { LOCAL, VENDOR };  //NA means "not to show price"
        

        public virtual int getInventoryCount()
        {
            //depending on inventory retrieval method, here it shall call vendor inventory check up function
            return CurrentStockQty.GetValueOrDefault();
        }


        /// <summary>
        /// To set or retrieve product name
        /// </summary>
        public virtual String name
        {
            get 
            {
                if (dummyName == null)
                    return VendorProductName??SProductID;
                else
                    return dummyName;
            }
            set { dummyName = value; }
        }

        /// <summary>
        /// This property is only applicable to unit test
        /// </summary>
        private String dummyName
        {
            get;
            set;
        }

        /// <summary>
        /// Price source property indicates where the part price shall be defined.  It can be either from SAP or from local price
        /// </summary>
        public virtual PRICESOURCE priceSource
        {
            get
            {
                PRICESOURCE source;

                    switch (PriceSourceProvider)
                    {
                        case "LOCAL":
                            source = PRICESOURCE.LOCAL;
                            break;
                        default:
                            source = PRICESOURCE.VENDOR;
                            break;
                    }

                return source;
            }

            set
            {
                switch (value)
                {
                    case PRICESOURCE.VENDOR:
                        PriceSourceProvider = "SAP";
                        break;
                    case PRICESOURCE.LOCAL:
                        PriceSourceProvider = "";
                        break;
                }
            }
        }

        private List<RelatedProduct> _RelatedProductsX;
        public List<RelatedProduct> RelatedProductsX
        {
            get 
            {
                if (_RelatedProductsX == null)
                    _RelatedProductsX = RelatedProducts.Where(c => c.IseStoreSetting != false).ToList();
                return _RelatedProductsX; 
            }
        }
        

        /// <summary>
        /// Atp contains availability to promise information. It's value will differ according to request quantity.
        /// ATP information shall be momentary value and shall be expired within 5 minutes
        /// </summary>
        private ATP _atp = null;
 
        private DateTime _atpLastUpdated = DateTime.MinValue;
        public ATP atp
        {
            get
            {
                if (needRetrieveATP())
                {
                    refreshATP(1);
                }

                return _atp;
            }

            set
            {
                _atp = value;
                _atpLastUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// if never initialized or last update time is longer than 5 minutes ago, return true else return false
        /// </summary>
        /// <returns></returns>
        public bool needRetrieveATP()
        {
            if (_atp == null ||     //never initialized
                       DateTime.Now.Subtract(_atpLastUpdated).TotalMinutes > 5)  //last update time is longer than 5 minutes ago
                return true;
            else
                return false;
        }
        public ATP atpX
        {
            get {
                if (_atp == null ||     //never initialized
                    DateTime.Now.Subtract(_atpLastUpdated).TotalMinutes > 5)  //last update time is longer than 5 minutes ago
                {
                    _atp = new ATP(DateTime.MaxValue,0);
                    _atpLastUpdated = DateTime.Now.AddMinutes(-5) ;
                }

                return _atp;
            }
        }

        /// <summary>
        /// This method is to force eStore to acquire ATP again regardlessly.
        /// </summary>
        /// <returns></returns>
        public ATP refreshATP(int requestQty)
        {
            _atp = getATP(requestQty);
            _atpLastUpdated = DateTime.Now;
            return _atp;
        }

        public virtual Price getOriginalPrice()
        {
            Price price = new Price();
            switch (this.priceSource)
            {
                case PRICESOURCE.LOCAL:
                    price.value = LocalPrice.GetValueOrDefault();
                    break;
                case PRICESOURCE.VENDOR:    //SAP for Advantech
                default:
                    price.value = VendorSuggestedPrice.GetValueOrDefault();
                    break;
            }
            return price;
        }
        /// <summary>
        /// This function is used to retrive part raw price without considering any promotion setting.  Usually caller use this method to retrive part-only price.
        /// </summary>
        /// <param name="rounding">an optinal parameter, it will be used very rarely when the caller doesn't want the price to be rounded</param>
        /// <returns></returns>
        public virtual Price getNetPrice(Boolean rounding = true)
        {
            Price price = new Price();

            if (notAvailable)
                price.value = 0;
            else
            {
                //get regular listing price here
                price = getOriginalPrice();
            }

            if (rounding)
                price.value = Converter.round(price.value, this.StoreID);
            else
                price.value = Math.Round(price.value, 2);   //keep two decimal digits in default

            return price;
        }
        public virtual Price getNetPrice( POCOS.UserGrade userGrade, Boolean rounding)
        {
            Price price = new Price();

            if (notAvailable)
                price.value = 0;
            else
            {
                if (userGrade == null || userGrade.Grade == "GA")
                {
                    price = getNetPrice(rounding);
                }
                else
                {

                    if (this.partGradePricesX != null && this.partGradePricesX.Any(x=>x.PriceGrade==userGrade.PriceGrade))
                    {
                        price = new POCOS.Price() { value= this.partGradePricesX.First(x => x.PriceGrade == userGrade.PriceGrade).Price };
                    }
                    else
                    {
                        price = getNetPrice();
                    }
                }
                //get regular listing price here
               
            }

            if (rounding)
                price.value = Converter.round(price.value, this.StoreID);
            else
                price.value = Math.Round(price.value, 2);   //keep two decimal digits in default

            return price;
        }

        private List<PartGradePrice> _partGradePricesX;
        public List<PartGradePrice> partGradePricesX
        {
            get {
                if (_partGradePricesX == null)
                {
                    PartHelper helper = new PartHelper();
                    _partGradePricesX = helper.getPartGradePrices(this);
                    if (_partGradePricesX == null)
                        _partGradePricesX = new List<PartGradePrice>();
                }
                return _partGradePricesX;
            }
        }
        /// <summary>
        /// This method will have override implementation at Prduct class. eStore UI shall use this method only for listing
        /// Product or Part price.  The function shall be not be used to retrieve CTOS part price.
        /// </summary>
        /// <returns></returns>
        public virtual Price getListingPrice()
        {
            Price price = getNetPrice();

            //every store may have its owner minimum price restriction.  Hereby price may need to be adjusted before return
            makeMinPriceAdjustment(price);

            if (price.value < CostX) // list price < cost will show call for price
                price.value = 0;

            return price;
        }
        public virtual Price getListingPrice(POCOS.UserGrade userGrade)
        {
            Price price = getNetPrice(userGrade,true);
            if (price.value < CostX) // list price < cost will show call for price
                price.value = 0;

            return price;
        }
        /// <summary>
        /// isOrderable method will only check part status without price validation.  If price is needed, use getListingPrice method
        /// instead.  This method also take parameter.  When parameter value is true, it will validate if this part can be sold alone.
        /// Some part can only be sold with CTOS system and can not be sold individually.
        /// </summary>
        /// <param name="individual">if individual is true, HDD can't be orderable singly</param>
        /// <returns></returns>
        public virtual Boolean isOrderable(bool individual = false)
        {
            Boolean orderable = true;
            //HDD can't be orderable singly
            //if (individual && !string.IsNullOrWhiteSpace(this.VendorProductLine)&& this.VendorProductLine.ToUpper() == "HDD")
            //    orderable = false;

            if (this.notAvailable)
                orderable = false;

            return orderable;
        }

        public virtual Boolean isOrderableBase(bool individual = false)
        {
            return this.isOrderable(individual);
        }

        public virtual bool isTOrPParts()
        {
            return !string.IsNullOrEmpty(ABCInd) && (ABCInd.ToUpper() == "T" || ABCInd.ToUpper() == "P");
        }


        public Boolean getOrderablePrice(ref Price _price)
        {
            Boolean isOrderable = true;
            _price = new Price();
            if (this is Product_Ctos)
            {
                Product_Ctos ctosItem = (Product_Ctos)this;
                _price = ctosItem.getListingPrice();
                isOrderable = ctosItem.isOrderable();
            }
            else if (this is Product)
            {
                Product productItem = (Product)this;
                _price = productItem.getListingPrice();
                isOrderable = productItem.isOrderable();
            }
            else
            {
                _price = this.getListingPrice();
                isOrderable = this.isOrderable();
            }
            return isOrderable;
        }

        public string autoAppliedCouponCode
        {
            get {
                if (System.Web.HttpContext.Current != null
                    && System.Web.HttpContext.Current.Session!=null
                    && System.Web.HttpContext.Current.Session["autoAppliedCouponCode"] != null 
                    && string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session["autoAppliedCouponCode"].ToString()) == false)
                {
                    return System.Web.HttpContext.Current.Session["autoAppliedCouponCode"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Override method implemented in both Part and Product.  This property is to replace VendorFeature acquisition
        /// </summary>
        public virtual String productFeatures
        {
            get { return esUtilities.CommonHelper.replaceSpecialSymbols(VendorFeatures); }
        }

        /// <summary>
        /// This method is to check up if this product is a mainstream product that may be used for price
        /// adjustment purpose.  By default, Advantech products will be the mainstream products and AGS
        /// products are not.  Product ID starts with "96", "19", "20", "17", "OPTION", "AGS" is not 
        /// mainstream product.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isMainStream()
        {
            if (SProductID.StartsWith("96"))
            {
                //this special logics is to handle 3rd party products that John's group carries and treat them
                //as Advantech item.  So if this part is included in CTOS, it can be assigned as main CTOS part
                //, so CTOS can include its image in CTOS image list.
                //For example, HD media player 4GB HDD 
                if (this.VendorProductLine == "SYS")
                    return true;
                else
                    return false;
            }
            else if (isEPAPS())
            {
                return false;
            }
            else if (SProductID.StartsWith("19") ||
                SProductID.StartsWith("20") || SProductID.StartsWith("17") ||
                SProductID.StartsWith("OPTIONS") || SProductID.StartsWith("AGS") || SProductID.StartsWith("SQF"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Part can be a warrantee item specifying the warranty charging price and can be used
        /// in CTOS warranty option
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isWarrantyPart()
        {
            if (SProductID.StartsWith("AGS-EW"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Some parts can not be included in CTOS warranty contract, for example OS. Hereby this method is mainly to return
        /// the indicator of whether this part can be included in warranty contract or not.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isWarrantable()
        {
            //AEU 除了AGS-EW延保,其他都有warrant
            if (StoreID == "AEU")
                return !isWarrantyPart();
            else
            {
                if (isSoftware())
                    return false;
                else if (SProductID.StartsWith("AGS") || SProductID.StartsWith("OPTION"))  //assembly or special option
                    return false;
                else if (VendorProductLine != null && VendorProductLine == "ASS#")  //assembly charges
                    return false;
                else
                    return true;
            }
            /*
            if (SProductID.StartsWith("96SW") || SProductID.StartsWith("AGS") ||
                SProductID.StartsWith("OPTION") ||
                SProductID.StartsWith("98MQ"))
                return false;
            else if (VendorProductLine != null && (VendorProductLine == "EPCS" || VendorProductLine == "EDOS" ||
                                                                            VendorProductLine == "ASS#" || VendorProductLine == "WCOM" ||
                                                                            VendorProductLine == "WAUT" || VendorProductLine == "DAAS"))
                return false;
            else
                return true;
             * */
        }

        /// <summary>
        /// This method will determine whether this product is an OS software or image or not.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isOS()
        {
            //if (SProductID.StartsWith("968Q") || SProductID.StartsWith("96SW") || SProductID.StartsWith("20700"))
            if (SProductID.StartsWith("968Q") || SProductID.StartsWith("98MQ") ||
                (SProductID.StartsWith("96SW") && (!SProductID.StartsWith("96SW-ANT") || !SProductID.StartsWith("96SW-GHOST") || !SProductID.StartsWith("96SW-OFF"))) || 
                    SProductID.StartsWith("20700"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method will determine whether this product is an software product or not.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean isSoftware()
        {
            if (SProductID.StartsWith("96SW") || SProductID.StartsWith("98MQ") || SProductID.StartsWith("968Q") )
                return true;
            else if (VendorProductLine != null && (VendorProductLine == "EPCS" || VendorProductLine == "EDOS" ||
                                                                            VendorProductLine == "WCOM" ||
                                                                            VendorProductLine == "WAUT" || VendorProductLine == "DAAS"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method is used to determine which product group this product belongs to
        /// </summary>
        /// <returns></returns>
        public Boolean isEAProduct()
        {
            if (string.IsNullOrEmpty(this.ProductDivision))
                return false;
            else if (this.ProductDivision.ToUpper().Equals("EAUT") ||
                this.ProductDivision.ToUpper().Equals("EAIO") ||
                this.ProductDivision.ToUpper().Equals("HMIA") ||
                this.ProductDivision.ToUpper().Equals("ICOM"))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This method indicates whether it's a P-Trade part or no
        /// </summary>
        /// <returns></returns>
        public Boolean isPTradePart()
        {
            if (String.IsNullOrWhiteSpace(this.VendorProductGroup))
            {
                if (SProductID.StartsWith("96") && (isTenDigitPart() == false))
                    return true;
                else
                    return false;
            }
            else
            {
                if (this.VendorProductGroup.Equals("PAPS"))
                    return true;
                else
                    return false;
            }
        }

        public bool isEPAPS()
        {
            //Product_Division:AGSG & Product_Group:PAPS
            return
              !string.IsNullOrEmpty(this.ProductDivision)
                && !string.IsNullOrEmpty(this.VendorProductGroup)
                && this.ProductDivision == "AGSG"
                && this.VendorProductGroup == "PAPS";
        }

        public virtual bool isPStoreProduct()
        {
            return false;
        }

        /// <summary>
        /// This property is a overridable prooperty and will have a override implementation in Product
        /// </summary>
        public virtual String productDescX
        {
            get { return esUtilities.CommonHelper.replaceSpecialSymbols(VendorProductDesc); }
        }

        public IEnumerable<RelatedProduct> getAccessories(int requestQty = 1)
        {
            IEnumerable<RelatedProduct> accessories = new List<RelatedProduct>();

            try
            {
                //acquire ATP value of relatedProducts
                Dictionary<Part, int> updatingList = new Dictionary<Part, int>();
                foreach (RelatedProduct rp in RelatedProductsX)
                    updatingList.Add(rp.RelatedPart, requestQty);

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                //Store store = new StoreHelper().getStorebyStoreid(this.StoreID);
                //parthelper.setATPs(store, updatingList);
                parthelper.setATPs(storeX, updatingList);

                accessories = from part in this.RelatedProductsX
                                      where part.RelatedPart.atp.availableQty >= requestQty
                                      select part;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at getAccessories", SProductID, "", StoreID, ex);
            }

            return accessories;
        }


        public IEnumerable<PeripheralCompatible> getCompatiblePeripherals(int requestQty = 1)
        {
            IEnumerable<PeripheralCompatible> peripherals = new List<PeripheralCompatible>();

            try
            {
                //acquire ATP value of relatedProducts
                Dictionary<Part, int> updatingList = new Dictionary<Part, int>();
                foreach (PeripheralCompatible rp in PeripheralCompatibles)
                    updatingList.Add(rp.Part , requestQty);

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                //Store store = new StoreHelper().getStorebyStoreid(this.StoreID);
                parthelper.setATPs(storeX, updatingList);

                peripherals = from part in this.PeripheralCompatibles
                                  where part.Part.atp.availableQty >= requestQty
                                  select part;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at getCompatiblePeripherals", SProductID, "", StoreID, ex);
            }

            return peripherals;
        }

        public string fillupPartnoasSAP()
        {

            string strzero = "";

            if (isnumberic(this.SProductID))
            {
                int zerolen = 18 - SProductID.Length;

                for (int i = 0; i < zerolen; i++)
                {
                    strzero = strzero + "0";
                }


            }

            return strzero + SProductID;

        }
        private bool isnumberic(string partno)
        {

            int i = 0;
            foreach (char c in partno.ToCharArray())
            {
                if (char.IsNumber(c) == false)
                {
                    return false;
                }
                else
                {
                    i++;
                }


            }

            return true;

        }

        private Dictionary<string,int> suggestingProducts;
        public Dictionary<string, int> SuggestingProductsDictionary
        {

            get
            {
                if (suggestingProducts != null)
                    return suggestingProducts;

                try
                {
                    MyDataMining.MyDataMining myDataMining = new MyDataMining.MyDataMining();
                    suggestingProducts = new Dictionary<string, int>();
                    System.Data.DataTable dt = myDataMining.GetBasketAnalysis(this.SProductID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            if(suggestingProducts.ContainsKey(row[0].ToString())==false)
                            suggestingProducts.Add(row[0].ToString(),int.Parse(row[1].ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Exception at getting Suggesting Products", "", "", "", ex);
                }



                return suggestingProducts;
            }
        }

        /// <summary>
        /// This method return 
        /// </summary>
        /// <returns></returns>
        public int getQuantityInStock()
        {
            ATP availableEstimation = getATP();
            if (availableEstimation != null &&
                availableEstimation.availableQty > 0 &&
                availableEstimation.availableDate.CompareTo(DateTime.Now) < 0)
            {
                return availableEstimation.availableQty;
            }
            else
                return 0;
        }

        public virtual String thumbnailImageX
        {
            get { return TumbnailImageID; }
        }

        private String _datasheetX = null;
        public virtual string dataSheetX
        {
            get
            {
                if (_datasheetX == null)
                {
                    if (this.ProductResources != null && this.ProductResources.Count > 0)
                    {
                        POCOS.ProductResource prDataSheet = this.ProductResources.FirstOrDefault(_pr => _pr.ResourceType == "Datasheet");
                        if (prDataSheet != null)
                            _datasheetX = prDataSheet.ResourceURL;
                        else
                            _datasheetX = "";
                    }
                    else
                        _datasheetX = "";
                }
                return _datasheetX;
            }
        }

        private object speclocker = new object();
        /// <summary>
        /// This property contains part / product / CTOS product spec values
        /// 
        /// set用于批量从db获取资料以提高速度
        /// </summary>
        public virtual IList<VProductMatrix> specs
        {
            get
            {
                lock (speclocker)
                {
                    if (specList != null)
                        return specList;


                    if (this is Product)
                    {
                        if (parthelper == null)
                            parthelper = new PartHelper();
                        // spec 不再区分 paps 和 estore
                        //if (isEPAPS())
                        //{
                        //    specList = parthelper.getProductEPAPSSpec((Product)this, this.StoreID);
                        //}
                        //else
                        specList = parthelper.getProductSpec((Product)this, this.StoreID);
                    }
                    else
                        specList = new List<VProductMatrix>();

                    return specList;
                }
            }
            set {

                List<VProductMatrix> retspecs = new List<VProductMatrix>();
               
                    var _specs = (from sp in  value.ToList() 
                                  select new
                                  {
                                      sp.LocalCatName,
                                      sp.AttrCatName,
                                      sp.AttrName,
                                      sp.AttrValueName,
                                      sp.LocalAttributeName,
                                      sp.LocalValueName,
                                      sp.seq,
                                      sp.AttrID,
                                      sp.AttrValueID,
                                      sp.CatID,
                                      sp.ProductNo
                                  }).Distinct().ToList();

                    foreach (var s in _specs)
                    {
                        VProductMatrix v = new VProductMatrix();
                        v.AttrCatName = s.AttrCatName;
                        v.AttrName = s.AttrName;
                        v.AttrValueName = s.AttrValueName;
                        v.LocalCatName = s.LocalCatName;
                        v.LocalAttributeName = s.LocalAttributeName;
                        v.LocalValueName = s.LocalValueName;
                        v.AttrID = s.AttrID;
                        v.AttrValueID = s.AttrValueID;
                        v.CatID = s.CatID;
                        v.ProductNo = s.ProductNo;

                        retspecs.Add(v);
                    }
                
                specList = value.ToList();
            }
        }

        /// <summary>
        /// This property indicates whether current part state is in store-wise orderable state
        /// </summary>
        public virtual Boolean notAvailable
        {
            get
            {
                if (String.IsNullOrEmpty(StockStatus))
                    return false;    //when status is null, most of the case it's active part

                if (storeX.getOrderablePartStates().Contains(this.StockStatus))
                {
                    if (this.StockStatus.ToUpper().Equals("O"))
                    {
                        if (hasInventoryIn60Days())
                            return false;
                        else
                            return true;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// This method is to check if there will be enough inventory with 60 days
        /// </summary>
        /// <param name="quantity">Optional! Its default value will be 1 if no value is provided</param>
        /// <returns></returns>
        protected Boolean hasInventoryIn60Days(int quantity=1)
        {
            if (atp.availableDate < DateTime.Now.AddDays(60) && atp.availableQty >= quantity)
                return true;
            else
                return false;
        }

        //every store might have minimum product / part price restriction in order
        protected void makeMinPriceAdjustment(Price price)
        {
            //the following logic is hardcoded per US need.  We need to review this logic and make it to be configurable *****
            if (price != null && price.value > 0)
            {
                //Store store = new StoreHelper().getStorebyStoreid(this.StoreID);
                if (storeX.Settings.ContainsKey("minimumprice"))
                {
                    decimal minimumprice = 0m;
                    if (decimal.TryParse(storeX.Settings["minimumprice"], out minimumprice))
                    {
                        if (minimumprice>price.value)
                            price.value = minimumprice;
                    }
                }
            }
        }

        /// <summary>
        /// When the sourcingQty value is 0, it only check current available quantity. It returns
        /// NULL if there is nothing available.
        /// Whent the sourcingQty value is greater than 0, it returns ATP with a list of delivery schedules
        /// </summary>
        /// <param name="requestQty"></param>
        /// <returns></returns>
        private ATP getATP(int requestQty=0)
        {
            try
            {   
                //Store store = new StoreHelper().getStorebyStoreid(this.StoreID);
                if (!storeX.getBooleanSetting("getATPFromSAP"))
                    return new ATP(DateTime.MaxValue, 0);
                SAPProxy sp = new SAPProxy();
                Hashtable parts = new Hashtable();
                parts.Add(SProductID, requestQty);
                List<ProductAvailability> lpv = sp.getMultiATP(storeX.Settings["SAPDefaultPlant"], DateTime.Now, parts);
                var atpTemp = sp.GetAvailability(SProductID, lpv);

                return new ATP(atpTemp.RequestDate, atpTemp.QtyATP) { Message = atpTemp.Message };
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("getATP exception", "", "", "", ex);
                return new ATP(DateTime.MaxValue, 0) { Message = "Exception at getATP" };
            }
        }

        /// <summary>
        /// To determine if the part is a 10 digit parts (spare part)
        /// As long as the leading 10 digits are numeric, it's a spare part
        /// </summary>
        /// <returns></returns>
        public bool isTenDigitPart()
        {
            if (_isTenDigitPart.HasValue)
                return _isTenDigitPart.GetValueOrDefault();
            else
            {
                int i = 0;
                foreach (char c in SProductID.ToCharArray())
                {
                    if (char.IsNumber(c) == false)
                    {
                        _isTenDigitPart = false;
                        break;
                    }
                    else
                    {
                        i++;
                        if (i >= 10)    //first 10 digits are numeric
                            break;
                    }
                }

                if (i == 10)
                    _isTenDigitPart = true;
                else
                    _isTenDigitPart = false;
            }

            return _isTenDigitPart.GetValueOrDefault();
        }
        private bool? _isTenDigitPart;

        public virtual bool isUseLimiteResource //是否验证limited Resource
        {
            get
            {
                return true;
            }
        }

        // get pat limitedResources string
        private string _limitedResourcesStr;
        public string limitedResourcesStr
        {
            get 
            {
                if (_limitedResourcesStr == null)
                {
                    if (this != null && this.ProductLimitedResources != null && this.ProductLimitedResources.Count > 0)
                        _limitedResourcesStr = string.Join(",", ProductLimitedResources.Select(c => c.ResourceID.ToString() 
                            + "-" + (c.AvaiableQty.HasValue ? c.AvaiableQty.GetValueOrDefault() : 0).ToString() 
                            + "-" + (c.ConsumingQty.HasValue ? c.ConsumingQty.GetValueOrDefault() : 0).ToString()).ToArray());
                }
                return _limitedResourcesStr; 
            }
        }

        private Dictionary<string, int> _availableResource;
        public Dictionary<string, int> availableResource
        {
            get
            {
                if (_availableResource == null)
                    initResource();
                return _availableResource;
            }
        }

        private Dictionary<string, int> _consumingResource;
        public Dictionary<string, int> consumingResource
        {
            get
            {
                if (_consumingResource == null)
                    initResource();
                return _consumingResource;
            }
        }

        private bool initResource()
        {
            bool valid = true;
            _availableResource = new Dictionary<string, int>();
            _consumingResource = new Dictionary<string, int>();
            if (this.ProductLimitedResources != null && this.ProductLimitedResources.Count > 0)
            {
                foreach (ProductLimitedResource resource in this.ProductLimitedResources)
                {
                    if (resource.AvaiableQty.HasValue && resource.AvaiableQty.GetValueOrDefault() > 0)
                    {
                        if (_availableResource.ContainsKey(resource.LimitedResource.Name))
                            _availableResource[resource.LimitedResource.Name] += resource.AvaiableQty.GetValueOrDefault();
                        else
                            _availableResource.Add(resource.LimitedResource.Name, resource.AvaiableQty.GetValueOrDefault());
                    }
                    if (resource.ConsumingQty.HasValue && resource.ConsumingQty.GetValueOrDefault() > 0)
                    {
                        if (_consumingResource.ContainsKey(resource.LimitedResource.Name))
                            _consumingResource[resource.LimitedResource.Name] += resource.ConsumingQty.GetValueOrDefault();
                        else
                            _consumingResource.Add(resource.LimitedResource.Name, resource.ConsumingQty.GetValueOrDefault());
                    }
                }
            
            }
            return valid;
        }

        // this is get part dependency parts
        private List<Part> _dependentPartsX;
        public List<Part> dependentPartsX
        {
            get
            {
                if (_dependentPartsX == null)
                {
                    _dependentPartsX = new List<Part>();
                    if (this.DependencytoProducts != null && this.DependencytoProducts.Count > 0) // get its dependency Products
                    {
                        foreach (var d in this.DependencytoProducts)
                        {
                            if (d.PartDependency != null)
                            {
                                var _exitPart = _dependentPartsX.FirstOrDefault(c => c.SProductID == d.SProductID);
                                if (_exitPart == null)
                                    _dependentPartsX.Add(d.PartDependency);
                            }
                        }
                    }
                }
                return _dependentPartsX;
            }
        }

        // 拼合 dependency part id for ui
        private string _dependencyPartStr = null;
        public string dependencyPartStr
        {
            get 
            {
                if (_dependencyPartStr == null && dependentPartsX.Count > 0)
                    _dependencyPartStr = string.Join("|", dependentPartsX.Select(c => c.SProductID).ToArray());
                if (string.IsNullOrEmpty(_dependencyPartStr))
                    _dependencyPartStr = string.Empty;
                return _dependencyPartStr; 
            }
        }

        /// <summary>
        /// 产品是否含有 BoardFlash 如果有 则此产品可以安装OS
        /// </summary>
        /// <returns></returns>
        public bool hasBuiltInStorage()
        {
            return this.SProductID.ToUpper().StartsWith("PCM-3362");
        }

        public virtual decimal CostX 
        {
            get
            {
                if (LocalCost.HasValue && LocalCost.GetValueOrDefault() > 0)
                    return LocalCost.GetValueOrDefault();
                return this.Cost.GetValueOrDefault();
            }
            set
            {
                this.Cost = value;
            }
        }

        public virtual bool isBelowCost 
        {
            get
            {
                return CostX > getListingPrice().value;
            }
        }

        public bool isInactive
        {
            get 
            {
                if (!String.IsNullOrWhiteSpace(StockStatus) && StockStatus.Equals('I') && StockStatus.Equals("TBR"))
                    return true;
                else
                    return false;
            }
        }
        
        public Sync.SAP_PRODUCT SAP_PRODUCT
        {
            get
            {
                if (parthelper == null)
                    parthelper = new PartHelper();
                return parthelper.getSAPProductFromPIS(this.SProductID);
            }
        }

        public bool isBBproduct(string materialgroup, string productline)
        {
            if (CheckMaterialGroup(materialgroup) == true)
                return CheckProductLine(productline);
            return false;
        }

        public bool CheckMaterialGroup(string materialgroup)
        {
            if (!string.IsNullOrEmpty(materialgroup))
            {
                List<string> list = materialgroup.Split('|').ToList();
                var sp = this.SAP_PRODUCT;
                if (sp != null)
                    return list.Contains(sp.MATERIAL_GROUP);
            }
            return false;
        }

        public bool CheckProductLine(string productline)
        {
            if (!string.IsNullOrEmpty(productline))
            {
                List<string> list = productline.Split('|').ToList();
                var sp = this.SAP_PRODUCT;
                if (sp != null)
                    return list.Contains(sp.PRODUCT_LINE);
            }
            return false;
        }

        #endregion

        #region OM_Extension
        /// <summary>
        /// This property indicates if current product has all mandatory data for publish to eStore. 
        /// It's mainly used in OM to check product information readiness.
        /// The validation criteria is as following
        /// 1. DisplayPartno, Status, ProductDesc, ProductFeatures
        /// 2. ImageURL, CreatedBy, DisplayPartNo, (DimensionHeight/Width/Length commented out)
        /// 3. Price and Product status
        /// 4. Product features and data sheets
        /// </summary>
        public virtual Boolean validForPublish
        {
            get
            {
                //perform the most basic validation
                //DisplayPartno, Status, ProductDesc, ProductFeatures
                validate();

                //additional validation
                if (String.IsNullOrEmpty(this.TumbnailImageID))
                    error_message.Add(new PocoX.ErrorMessage("ImageURL", "Missing product image information"));
                if (String.IsNullOrEmpty(this.CreatedBy))
                    error_message.Add(new PocoX.ErrorMessage("CreatedBy", "Need to record who enroll this product to eStore"));
                if (String.IsNullOrEmpty(this.VendorProductName))
                    error_message.Add(new PocoX.ErrorMessage("DisplayPartno", "Product display name can not be empty"));
                /*
                if (this.DimensionHeightCM.GetValueOrDefault() == 0 || 
                    this.DimensionLengthCM.GetValueOrDefault() == 0 ||
                    this.DimensionWidthCM.GetValueOrDefault() == 0)
                    error_message.Add(new PocoX.ErrorMessage("Product dimension height, width and length", "Missing product dimension info"));
                 */
                if (isMainStream()==true && String.IsNullOrEmpty(this.ModelNo) )
                    error_message.Add(new PocoX.ErrorMessage("ModelNo", "Product Model no can not be empty"));
                //validing price setting
                if (this.priceSource == PRICESOURCE.LOCAL && this.LocalPrice == 0)
                    error_message.Add(new PocoX.ErrorMessage("StorePrice", "Local price can not be 0 when price source is set to local"));
                if (this.priceSource == PRICESOURCE.VENDOR && this.VendorSuggestedPrice == 0)
                    error_message.Add(new PocoX.ErrorMessage("VendorSuggestedPrice", "Vendor suggestted price can not be 0 when price source is set to vendor"));
                if (this.notAvailable)
                    error_message.Add(new PocoX.ErrorMessage("Status", "Product status is not eligible to add to eStore"));

                if (String.IsNullOrEmpty(this.productFeatures) && !isPTradePart())
                    error_message.Add(new PocoX.ErrorMessage("ProductFeatures", "Product features can not be empty"));
                //validate product data sheet information
                var count = (from resource in ProductResources
                             where resource.ResourceType.ToUpper().Equals("DATASHEET")
                             select resource).DefaultIfEmpty().Count();
                if (count <= 0)
                    error_message.Add(new PocoX.ErrorMessage("Product data sheets", "Missing product data sheet information"));
                if (MininumnOrderQty != null && this.MininumnOrderQty.HasValue && this.MininumnOrderQty.Value > 0)
                    error_message.Add(new PocoX.ErrorMessage("Part MOQ", "MOQ restriction"));
                if (error_message.Count > 0)
                    return false;
                else
                    return true;
            }
        }

#endregion

        #region Unit Test
        /// <summary>
        /// This is only run-time attribute and will only be used for unit test purpose
        /// </summary>
        public Boolean dummy
        {
            get;
            set;
        }

#endregion //Unit Test


        protected List<Certificates> _certificates;
        public virtual List<Certificates> Certificates
        {
            get 
            {
                if (_certificates == null)
                {
                    _certificates = this.productResourcesX.Where(c => c.ResourceType == "Certificates")
                            .Select(c => new Certificates { CertificateName = c.ResourceName, CertificateImagePath = c.ResourceURL }).ToList();
                }
                return _certificates; 
            }
            set { _certificates = value; }
        }

        private List<ProductResource> _productResourcesX;
        public virtual List<ProductResource> productResourcesX
        {
            get
            {
                if (_productResourcesX == null)
                {
                    _productResourcesX = new List<ProductResource>();
                    //priority LocalResource then pis, last update latest, resorcetype a-z
                    foreach (POCOS.ProductResource pr in this.ProductResources.Where(c=>c.ResourceStatus != "delete").OrderByDescending(x => x.IsLocalResource).ThenBy(x => x.ResourceType).ThenByDescending(x => x.ResourceID))
                    {
                        //LargeImages will merge local resoure, other use according the priority
                        //Add Certificates & eStoreLocalMainImage types in product resources
                        switch (pr.ResourceType)
                        {
                            case "LargeImages":
                            case "Certificates":
                                _productResourcesX.Add(pr);
                                break;
                            case "video":
                                if(pr.isActivity())
                                    _productResourcesX.Add(pr);
                                break;
                            default:
                                if (_productResourcesX.Any(x => x.ResourceType == pr.ResourceType) == false)
                                    _productResourcesX.Add(pr);
                                break;
                        }
                    }
                }
                return _productResourcesX;
            }
        }
        //获取eStoreLocalMainImage
        public string geteStoreLocalMainImage(bool keepOriginalResource = true)
        {
            if (!keepOriginalResource && this.productResourcesX.Any())
            {
                if (this.productResourcesX.Any(x => x.ResourceType == "eStoreLocalMainImage"))
                {
                    return this.productResourcesX.First(x => x.ResourceType == "eStoreLocalMainImage").ResourceURL;
                }
                else
                    return string.Empty;
            }
            else
                return string.Empty;

        }

        public string replaceSpecialResource(POCOS.ProductResource resource, bool keepOriginalResource)
        {
            if (resource == null)
                return string.Empty;
            if (!keepOriginalResource && this.productResourcesX.Any())
            {
                if (this.productResourcesX.Any(x => x.ResourceType == resource.ResourceType))
                {
                    return this.productResourcesX.First(x => x.ResourceType == resource.ResourceType).ResourceURL;
                }
                else
                    return resource.ResourceURL;
            }
            else
                return resource.ResourceURL;

        }
    }
}