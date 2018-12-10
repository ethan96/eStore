using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.POCOS
{
    /// <summary>
    /// BTOSystem is a single unit containing the configuration of BTO system.
    /// Ideally BTOSystem instance shall be either created by CTOSystem after user selection or
    /// shall be created through reloading BTO configuration from DB.  Thus the ideallest candidate
    /// for generating BTO system no is at CTOSystem.
    /// </summary>
    public partial class BTOSystem
    {
#region Business Logic Extension
        private String _storeId = null;
        private Decimal _price = 0m;
        private Dictionary<String, Part> _warrantyList = null;

        /// <summary>
        /// Default constructor used by Entity framework at constructing BTOSystem from DB.  Using this constructor, the caller
        /// need to set storeId property immediately after BTOSystem is constructured.
        /// </summary>
        public BTOSystem()
        {
            //<Note> ******
            //make sure to set storeId property immediately after calling this methhod.  
            //Otherwise parts property may return empty set.

            //In Entity framework, it assigns storeId property after construction
        }

        /// <summary>
        /// Default constructor for eStore applicaiton
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="systemNo"></param>
        public BTOSystem(String storeId, String systemNo, String modelNo)
        {
            if (BTOSConfigs == null)
                BTOSConfigs = new List<BTOSConfig>();
            _storeId = storeId;

            BTOConfigID = systemNo;
            BTONo = modelNo;
        }

        /// <summary>
        /// This consturctor will take a list of BTO component item and encapsulate them in it.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="systemNo"></param>
        /// <param name="configs"></param>
        public BTOSystem(String storeId, String systemNo, String modelNo, List<BTOSConfig> configs)
        {
            if (configs != null)
                BTOSConfigs = configs;

            _storeId = storeId;
            BTOConfigID = systemNo;
            BTONo = modelNo;
        }

        /// <summary>
        /// Call this method to add BTO component to compose BTO system
        /// </summary>
        /// <param name="component"></param>
        /// <param name="option"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public BTOSConfig addItem(CTOSBOM component, CTOSBOM option, int qty)
        {
            //if (isSBCBTOS())
            //    return null;    //SBC BTOS is composed with CTOS components

            if (component == null || option == null)
                return null;

            Boolean success = true;
            BTOSConfig config = new BTOSConfig();
            CTOSComponent optionItem = option.CTOSComponent;

            config.CategoryComponentID = component.CTOSComponent.ComponentID;
            config.CategoryComponentDesc = component.CTOSComponent.ComponentDesc;
            config.OptionComponentID = optionItem.ComponentID;
            config.OptionComponentDesc = optionItem.ComponentDesc;
            config.Qty = qty;
            config.Price = 0m;
            config.BTOConfigID = this.BTOConfigID;

            //fill up option detail.  Attention!  option can be a combination of product items
            BTOSConfigDetail item = null;
            Price netPrice = null;
            Part part;
            PartHelper helper = new PartHelper();

            foreach (CTOSComponentDetail partItem in optionItem.CTOSComponentDetails)
            {
                part = helper.getPart(partItem.SProductID, storeID);
                if (part != null)
                {
                    if (this.isSBCBTOS())
                    {
                        //SBC-BTO allows default items to be specified in DB as mandatory components
                        addNoneCTOSItem(part, partItem.Qty, 0,1, null, component.name);
                        success = false;    //don't add config item to BTOS
                    }
                    else
                    {
                        netPrice = part.getNetPrice(false);
                        if (netPrice.value > 0)
                        {
                            //the following prices are before adjustment. When BTOS price is updated, all item prices
                            //will be readjusted o reflect BTOS total charge.
                            item = new BTOSConfigDetail();
                            item.Description = partItem.ComponentDesc;
                            item.Qty = partItem.Qty;
                            item.NetPrice = Math.Round(netPrice.value, 2);
                            item.AdjustedPrice = item.NetPrice;
                            item.SProductID = part.SProductID;
                            item.MainProduct = part.isMainStream();
                            item.Warrantable = part.isWarrantable();
                            item.BTOConfigID = this.BTOConfigID;

                            //add config detail to BTOConfig
                            config.BTOSConfigDetails.Add(item);
                            config.Price += netPrice.value * item.Qty;
                            config.AdjustedPrice = config.Price;
                        }
                        else
                        {
                            success = false;
                            //fatal error since product doesn't have price associated, this shall be a invalid product
                            String errorMessage = String.Format("Price is not available at product {0} assigned in CTOS {1} option {2}",
                                                                partItem.SProductID, config.CategoryComponentID, config.OptionComponentID);
                            eStoreLoger.Error(errorMessage, "", "", _storeId);
                        }
                    }
                }
                else
                {
                    //can not find product assigned in CTOS component option
                    success = false;
                    //This is a fatal error and shall be noted *****
                    String errorMessage = String.Format("Failed at finding product {0} assigned in CTOS {1} option {2}", 
                                                        partItem.SProductID, config.CategoryComponentID, config.OptionComponentID);
                    eStoreLoger.Error(errorMessage, "", "", _storeId);
                }
            }

            if (success)  //problematic config
            {
                    BTOSConfigs.Add(config);
                    _warrantyList = null;
            }
            else
                config = null;

            return config;
        }

        /// <summary>
        /// This method is to empty BTOS configuration. Usually this method is used for complete reconstruction from scratch.
        /// </summary>
        public void clear()
        {
            BTOSConfigs.Clear();
            _warrantyList = null;
        }

        /// <summary>
        /// This method is to remove particular option from BTOS when user change their BTOS configuration
        /// </summary>
        /// <param name="option"></param>
        public void removeItem(CTOSBOM component, CTOSBOM option)
        {
            if (isSBCBTOS())
                return;    //SBC BTOS is composed with CTOS components

            if (option.type == CTOSBOM.COMPONENTTYPE.OPTION)    //make sure the input is an option
            {
                removeItem(component.CTOSComponent.ComponentID, option.CTOSComponent.ComponentID);
                _warrantyList = null;
            }
        }

        private void removeItem(int componentId, int optionId)
        {
            if (isSBCBTOS())
                return;    //SBC BTOS is composed with CTOS components

            BTOSConfig item = findItem(componentId, optionId);
            if (item != null)
            {
                BTOSConfigs.Remove(item);
                _warrantyList = null;
            }
        }

        public void changeItemQuantity(CTOSBOM component, CTOSBOM option, int qty)
        {
            if (isSBCBTOS())
                return;    //SBC BTOS is composed with CTOS components

            if (option.type == CTOSBOM.COMPONENTTYPE.OPTION)    //make sure the input is an option
                changeItemQuantity(component.CTOSComponent.ComponentID, option.CTOSComponent.ComponentID, qty);
        }

        private void changeItemQuantity(int componentId, int optionId, int qty)
        {
            BTOSConfig item = findItem(componentId, optionId);
            if (item != null)
                item.Qty = qty;
        }

        public List<BTOSConfig> getComponentConfigs(CTOSBOM component)
        {
            if (isSBCBTOS())
                return null;    //SBC BTOS is composed with CTOS components

            List<BTOSConfig> items = new List<BTOSConfig>();

            foreach (BTOSConfig config in BTOSConfigs)
            {
                if (config.CategoryComponentID == component.CTOSComponent.ComponentID)
                    items.Add(config);
            }

            return items;
        }

        public List<BTOSConfig> BTOSConfigsWithoutNoneItems
        {

            get {
                var bcs = from b in BTOSConfigs
                          where !b.isNoneItem()
                          select b;
                return bcs.ToList();
            }
        }
        /// <summary>
        /// This method will return matched option item.  It returns null if there is no match found.
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="optionId"></param>
        /// <returns></returns>
        private BTOSConfig findItem(int componentId, int optionId)
        {
            if (isSBCBTOS())
                return null;    //SBC BTOS is composed with CTOS components

            foreach (BTOSConfig config in BTOSConfigs)
            {
                if (config.CategoryComponentID == componentId && config.OptionComponentID == optionId)
                    return config;
            }

            return null;
        }


        /// <summary>
        /// Price is a read-only property. Any party interests of knowing BTOS price can use this property
        /// </summary>
        public Decimal Price
        {
            get
            {
                if (_price == 0)        //this condition ideally shall not exist, the reason it implements here is for double assurance.
                {
                    //sum up config items price
                    foreach (BTOSConfig config in BTOSConfigs)
                        _price += config.AdjustedPrice.GetValueOrDefault() * config.Qty;
                }

                _price = Math.Round(_price, 2);
                return _price;
            }
        }

        /// <summary>
        /// Whenever BTOS content items are changed, updatePrice method has to be called at the same time to make sure
        /// BTOS price is up-to-date and the warranty cost is recalculated.
        /// When BTOS price is updated, system will readjust BTOS item price to obsorb the price difference and warranty
        /// cost will be recalculated to reflect the latest change
        /// The price assigned here shall have been adjusted after promotion adjustment
        /// </summary>
        /// <param name="price"></param>
        /// <param name="performAdjust">An optional parameter indicating whether BTO item pricing shall be readjusted or not</param>
        public void updatePrice(Decimal price, Boolean performAdjust = true)
        {
            _price = Converter.round(price, this.storeID);

            if (performAdjust)  //only perform price adjustment when specified. In default this condition is true
            {
                //recalculate BTOS item price
                adjustBTOSItemPrice();
            }

            //recalculate BTOS warranty cost if user selects one
            Decimal warrantyCost = calculateWarranty();
            if (warrantyCost > 0)
                warrantyCost = Converter.round(calculateWarranty(), this.storeID);

            _price += warrantyCost;
        }

        
        /// <summary>
        /// This method is to adjust BTOS item price to distribute price difference between 
        /// CTOS total price (after rounding and discount) and 
        /// the total price of net prices of all CTOS items to eStore main products in BTOS item list. 
        /// </summary>
        private void adjustBTOSItemPrice()
        {
            List<BTOSConfigDetail> mainItems = new List<BTOSConfigDetail>();
            Decimal netTotal = 0m, mainItemTotal = 0m,  additionalPrice = 0m;

            foreach (BTOSConfig config in BTOSConfigs)
            {
                if (!config.isWarrantyConfig() && !config.isNoneCTOS())    //exclude warranty and none CTOS config
                {
                    foreach (BTOSConfigDetail item in config.BTOSConfigDetails)
                    {
                        Decimal itemNetPrice = item.NetPrice.GetValueOrDefault() * item.Qty * config.Qty;
                        netTotal += itemNetPrice;

                        if (item.MainProduct == true)
                        {
                            mainItems.Add(item);
                            mainItemTotal += itemNetPrice;
                        }

                    }
                }
                //else if (config.isWarrantyConfig())
                //{
                //    warrantyPrice += Converter.round(this.getWarrantableTotal() * config.netPrice * config.Qty / 100, this.storeID);
                //}
                else if (config.isNoneCTOS())
                {
                    additionalPrice += config.netPrice * config.Qty;
                }
            }

            if (mainItemTotal <= 0) //no main item found
            {
                String errorMessage = String.Format("No main item found for price adjustment BTOS {0}", BTOConfigID);
                eStoreLoger.Error(errorMessage, "", "", _storeId);
            }
            else
            {
                //calculate adjustRate
                Decimal priceDiff = netTotal - (_price - additionalPrice);
                Decimal newMainItemsTotal = mainItemTotal - priceDiff;
                Decimal adjustRate = newMainItemsTotal / mainItemTotal;

                //adjust main item unit selling price
                Decimal adjustedMainItemTotal = 0;
                foreach (BTOSConfigDetail item in mainItems)
                {
                    //item.AdjustedPrice = Math.Round(item.NetPrice.GetValueOrDefault() * adjustRate, 2);
                    item.AdjustedPrice = Converter.round(item.NetPrice.GetValueOrDefault() * adjustRate, storeID);
                    adjustedMainItemTotal += item.AdjustedPrice.GetValueOrDefault() * item.BTOSConfig.Qty * item.Qty;
                }

                //final adjustment after rounding, add the delta to the first adjustable item
                if (mainItems.Count() > 0 && (adjustedMainItemTotal != newMainItemsTotal))
                {
                    BTOSConfigDetail adjustingitem = mainItems.OrderBy(x => x.BTOSConfig.Qty).ThenByDescending(x=>x.AdjustedPrice).First();
                    adjustingitem.AdjustedPrice = adjustingitem.AdjustedPrice.GetValueOrDefault() + (newMainItemsTotal - adjustedMainItemTotal) / adjustingitem.BTOSConfig.Qty;
                }
            }

            //adjust and reassign config item AdjustedPrice
            foreach (BTOSConfig config in BTOSConfigs)
                config.AdjustedPrice = config.adjustedPrice;
        }

        private Decimal calculateWarranty()
        {
            Decimal warrantyPrice = 0m;

            if (getWarrantyList().Count != 0)
            {
                //calculate warrantable item total cost
                Decimal warrantableCost = getWarrantableTotal();

                //find warranty items
                warrantyPrice = updateWarrantyItemPrice(warrantableCost);
            }

            return warrantyPrice;
        }

        /// <summary>
        /// this method returns the total cost of BTOS warrantable items
        /// </summary>
        /// <returns></returns>
        public Decimal getWarrantableTotal()
        {
            Decimal warranty = 0m;

            foreach (BTOSConfig config in BTOSConfigs)
            {
                foreach (BTOSConfigDetail item in config.BTOSConfigDetails)
                {
                    if (item.Warrantable.GetValueOrDefault())
                        warranty += item.AdjustedPrice.GetValueOrDefault() * item.Qty * config.Qty;
                }
            }

            return warranty;
        }

        /// <summary>
        /// This method return matched warranty BTOSConfigDetail item associated with specifying part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private Decimal updateWarrantyItemPrice(Decimal warrantablePrice)
        {
            Decimal warrantyPrice = 0m;
            IDictionary<String, Part> warrantyList = getWarrantyList();

            Part warrantyItem = null;
            foreach (BTOSConfig config in BTOSConfigs)
            {
                //extended warranty category name shall contain words of "extended warranty" 
                if (config.isWarrantyConfig())
                {
                    Decimal configWarranty = 0m;
                    foreach (BTOSConfigDetail item in config.BTOSConfigDetails)
                    {
                        warrantyItem = warrantyList[item.SProductID];
                        if (warrantyItem != null)    //match
                        {
                            //item.NetPrice = Math.Round(warrantablePrice * warrantyItem.getNetPrice().value * item.Qty / 100, 2);
                            item.NetPrice = Converter.CartPriceRound(warrantablePrice * warrantyItem.getNetPrice(false).value * item.Qty / 100, storeID);
                            item.AdjustedPrice = item.NetPrice;
                            configWarranty += item.NetPrice.GetValueOrDefault();
                        }
                    }

                    config.Price = configWarranty;  //warranty cost for this BTOS config
                    config.AdjustedPrice = config.Price;

                    warrantyPrice += configWarranty;    //total warranty cost
                }
            }

            return warrantyPrice;
        }

        /// <summary>
        /// This method is provided for testing purpose. To find BTOS price, please use Price property.
        /// </summary>
        /// <returns></returns>
        public Decimal getSum()
        {
            Decimal sum = 0m;
            foreach (BTOSConfig config in BTOSConfigs)
                sum += config.adjustedPrice * config.Qty;

            return Math.Round(sum, 2);
        }


        public String storeID
        {
            get { return _storeId; }
            set { _storeId = value; }
        }

        /// <summary>
        /// This method is to clone itself to a new BTOSystem
        /// </summary>
        /// <returns></returns>
        public BTOSystem clone()
        {
            BTOSystem newBTOS = new BTOSystem();

            newBTOS.storeID = storeID;
            newBTOS.BTOConfigID = getHierarchyBTOConfigID(BTOConfigID);
            newBTOS.BTONo = BTONo;
            //copy BTOS Configs here
            foreach (BTOSConfig config in BTOSConfigs)
            {
                BTOSConfig newConfig = new BTOSConfig();
                newConfig.BTOConfigID = newBTOS.BTOConfigID;
                config.copyTo(newConfig);
                newBTOS.BTOSConfigs.Add(newConfig);
            }
            newBTOS.updatePrice(Price,false);
            var parts = newBTOS.parts;
            return newBTOS;
        }

        /*
 21486.20160524_061030_834.0803185807
 21486.20160524_061030_834.0803185807.0803185808
 21486.20160524_061030_834.0803185807.0803185808.0803185809
 21486.20160524_061030_834.0803185807.0803185808.0803185809.0803185810
 21486.20160524_061030_834.0803185807.0803185808.0803185809.0803185810.0803185811
 21486.20160524_061030_834.0803185807.0803185808.0803185809.0803185810.0803185811.0803185812
 21486.20160524_061030_834.V0803185812.0803185813
         */
        private string getHierarchyBTOConfigID(string previousId)
        {
            string BTOConfigID;
            if (!string.IsNullOrEmpty(previousId) && previousId.Length > 89)
            {
                BTOConfigID = "";

                List<string> resultList = new List<string>();
                try
                {
                    Regex regexObj = new Regex(@"(?<origID>.*?)(\.+V?(?<versionID>\d{10})+)");
                    Match matchResult = regexObj.Match(previousId);
                    string origID=string.Empty;
                    while (matchResult.Success)
                    {
                        if (string.IsNullOrEmpty(origID))
                            origID = matchResult.Groups["origID"].Value;
                        resultList.Add(matchResult.Groups["versionID"].Value);
                        matchResult = matchResult.NextMatch();
                    }
                    BTOConfigID = string.Format("{0}.V{1}{2}", origID, resultList.Last(), DateTime.Now.ToString(".MMddHHmmss"));
                }
                catch (ArgumentException ex)
                {
                    BTOConfigID = previousId + DateTime.Now.ToString(".MMddHHmmss");
                }

            }
            else
            {
                BTOConfigID = previousId + DateTime.Now.ToString(".MMddHHmmss");
            }

            return BTOConfigID;
        }
        /// <summary>
        /// This property returns a list of parts with quantity used in BTOS composition.
        /// This method is usually used by any party that is interested at knowing what parts are used in
        /// the BTOSystem, for example shipping calculator.
        /// </summary>
        public Dictionary<Part, int> parts
        {
            get
            {
                Dictionary<Part, int> partItems = new Dictionary<Part, int>();
                int qty = 0;

                foreach (BTOSConfig config in BTOSConfigs)
                {
                    foreach (KeyValuePair<Part, int> partItem in getConfigParts(config))
                    {
                        qty = partItem.Value * config.Qty;

                        if (partItems.ContainsKey(partItem.Key))
                            partItems[partItem.Key] = partItems[partItem.Key] + qty;
                        else
                            partItems.Add(partItem.Key, qty);
                    }
                }

                return partItems;
            }
        }

        //返回btos下面 产品的价钱
        public Dictionary<string, decimal> prices
        {
            get
            {
                Dictionary<string, decimal> configProductPrice = new Dictionary<string, decimal>();
                foreach (BTOSConfig configItem in BTOSConfigs)
                {
                    foreach (BTOSConfigDetail detailItem in configItem.BTOSConfigDetails)
                    {
                        decimal configPrice = configItem.Qty * detailItem.Qty * detailItem.AdjustedPrice.GetValueOrDefault();
                        if (configProductPrice.ContainsKey(detailItem.SProductID))
                            configProductPrice[detailItem.SProductID] = configProductPrice[detailItem.SProductID] + configPrice;
                        else
                            configProductPrice.Add(detailItem.SProductID, configPrice);
                    }
                }

                return configProductPrice;
            }
        }

        /// <summary>
        /// This method shall be invokes before using any parts method in BTOSystem or BTOSConfig items in it
        /// </summary>
        public void initPartReferences(bool includeATP=false)
        {
            IDictionary<Part, int> init = parts;
            if (includeATP)
            {
                Store store = new StoreHelper().getStorebyStoreid(this.storeID);
                new PartHelper().setATPs(store, parts);
            }
        }

        /// <summary>
        /// SBC-BTO products shall be named in the format of SBC-BTO-xxxxx
        /// </summary>
        /// <returns></returns>
        public Boolean isSBCBTOS()
        {
            if (!String.IsNullOrEmpty(BTONo) && (this.BTONo.ToUpper().StartsWith("SBC-BTO")))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Call this method to add part directly to BTO system. 
        /// It's only used to add none CTOS items to BTO system when it's needed.
        /// Its main usage is to compose SBC BTOS
        /// </summary>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public void addNoneCTOSBundle(List<BundleItem> bundle, int qty)
        {
            if (bundle==null || qty<=0)
                return;

            List<BundleItem> existsBundleList = new List<BundleItem>();
            foreach (BundleItem bundleItem in bundle)
            {
                if (!existsBundleList.Contains(bundleItem))
                {
                    if (bundleItem != null && bundleItem.part != null)
                    {
                        if (string.IsNullOrEmpty(bundleItem.peripheralProducts))
                            addNoneCTOSItem(bundleItem.part, qty, bundleItem.adjustedPrice, bundleItem.quantity,null,bundleItem.peripheralCategoryName);
                        else
                        {
                            List<BundleItem> keyBundle = (from p in bundle
                                                          where p.peripheralProducts == bundleItem.peripheralProducts && p.part != null
                                                          select p).ToList();
                            int keyBundleQty = 1;
                            string peripheralCategoryName = string.Empty;
                            Dictionary<Part, decimal> keyPart = new Dictionary<Part, decimal>();

                            foreach (var itemBundle in keyBundle)
                            {
                                if (string.IsNullOrEmpty(peripheralCategoryName))
                                    peripheralCategoryName = itemBundle.peripheralCategoryName;
                                keyPart.Add(itemBundle.part, itemBundle.adjustedPrice);
                                if (itemBundle.quantity > keyBundleQty)
                                    keyBundleQty = itemBundle.quantity;

                                //已经添加过
                                existsBundleList.Add(itemBundle);
                            }
                            addNoneCTOSItem(keyPart, qty, keyBundleQty, null, peripheralCategoryName);
                        }
                    }
                    else
                    {
                        //can not find product assigned in CTOS component option
                        //This is a fatal error and shall be noted *****
                        eStoreLoger.Error("null input in SBC BTO bundle", "", _storeId);
                    }
                }
                
            }
            //BTOSConfigs = BTOSConfigs.OrderBy(X => X.CategoryComponentDesc.StartsWith("AGS") ? 1 : 0).ToList();
            BTOSConfigs = BTOSConfigs.OrderBy(X => X.BTOSConfigDetails != null && X.BTOSConfigDetails.FirstOrDefault().SProductID.StartsWith("AGS") ? 1 : 0).ToList();
        }


        /// <summary>
        /// This method is to allow caller to add single part / product into BTOS as none CTOS item
        /// </summary>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public BTOSConfig addNoneCTOSItem(Part part, int qty, Decimal fixPrice = 0m, int configdetailqty = 1, POCOS.ProductCategory category = null, string categoryComponentDesc = "")
        {
            if (part == null || qty <= 0)
                return null;
            
            BTOSConfig config = new BTOSConfig(true, part.isWarrantyPart() ? "EXTENDED WARRANTY" : (string.IsNullOrEmpty(categoryComponentDesc) ? part.SProductID : categoryComponentDesc), part.productDescX);   //create special BTO config
            if (category != null)
            {
                config.OptionComponentID = category.CategoryID;
                config.CategoryComponentDesc = category.LocalCategoryName;
            }
            config.Qty = qty;
            config.Price = 0m;
            config.BTOConfigID = this.BTOConfigID;
            //添加configDetail to config
            addDetailToCTOSConfig(config, part, fixPrice, configdetailqty);

            return config;
        }

        private BTOSConfig addNoneCTOSItem(Dictionary<Part, decimal> partList, int qty, int configdetailqty = 1, POCOS.ProductCategory category = null, string categoryComponentDesc = "")
        {
            if (partList == null || partList.Count == 0 || qty <= 0)
                return null;

            bool isWarrantyPart = false;
            string configCategoryDesc = string.Empty;
            string optionComponentDesc = string.Empty;
            foreach (var item in partList)
            {
                if (item.Key.isWarrantyPart())
                    isWarrantyPart = true;
                if (!string.IsNullOrEmpty(configCategoryDesc))
                {
                    configCategoryDesc += ";";
                    optionComponentDesc += "<br>";
                }
                configCategoryDesc += item.Key.SProductID;
                optionComponentDesc += item.Key.productDescX;
            }
            if (optionComponentDesc.Length > 500)
                optionComponentDesc = optionComponentDesc.Substring(0, 499);

            BTOSConfig config = new BTOSConfig(true, isWarrantyPart ? "EXTENDED WARRANTY" : (string.IsNullOrEmpty(categoryComponentDesc) ? configCategoryDesc : categoryComponentDesc), optionComponentDesc);   //create special BTO config
            if (category != null)
            {
                config.OptionComponentID = category.CategoryID;
                config.CategoryComponentDesc = category.LocalCategoryName;
            }
            config.Qty = qty;
            config.Price = 0m;
            config.BTOConfigID = this.BTOConfigID;

            foreach (var itemPart in partList)
            {
                if (config != null)
                    //添加configDetail to config
                    addDetailToCTOSConfig(config, itemPart.Key, itemPart.Value, configdetailqty);
            }

            return config;
        }

        //添加configDetail to config
        private void addDetailToCTOSConfig(BTOSConfig config, Part part, decimal fixPrice = 0m, int configdetailqty = 1)
        {
            if (part != null)
            {
                //fill up option detail.  Attention! option can be a combination of product items
                BTOSConfigDetail item = null;
                Price netPrice = null;
                if (fixPrice == 0m)
                    netPrice = part.getListingPrice();
                else
                    netPrice = new Price(fixPrice, null);

                if (netPrice.value > 0)
                {
                    item = new BTOSConfigDetail();
                    item.Qty = configdetailqty;
                    item.NetPrice = Math.Round(netPrice.value, 2);
                    item.Description = part.productDescX;
                    item.AdjustedPrice = item.NetPrice;
                    item.SProductID = part.SProductID;
                    item.MainProduct = part.isMainStream();
                    item.Warrantable = part.isWarrantable();
                    item.partX = part;
                    //add config detail to BTOConfig
                    config.BTOSConfigDetails.Add(item);
                    config.Price += netPrice.value * item.Qty;
                    config.AdjustedPrice = config.Price;

                    BTOSConfigs.Add(config);
                    _warrantyList = null;
                }
                else
                {
                    config = null;
                    //fatal error since product doesn't have price associated, this shall be a invalid product
                    String errorMessage = String.Format("Price is not available at product %s", part.SProductID, "", "");
                    eStoreLoger.Error(errorMessage, "", "", _storeId);
                }
            }
        }

        public void removeNoneCTOSItem(int configId)
        {
            BTOSConfig config = findItem(configId);

            if (config!=null)
            {
                BTOSConfigs.Remove(config);
                _warrantyList = null;
            }
        }

        public void changeNoneCTOSItemQuantity(int configId, int qty)
        {
            BTOSConfig item = findItem(configId);

            if (item != null)
                item.Qty = qty;
        }

        private BTOSConfig findItem(int configId)
        {
            BTOSConfig config = (from item in BTOSConfigs
                                where item.ConfigID.Equals(configId)
                                select item).FirstOrDefault();
            return config;
        }


        /// <summary>
        /// This method is to retrieve parts associated in BTOSConfig item.  The return list will contain
        /// only physical parts. Parts like warranty will not be included.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private Dictionary<Part, int> getConfigParts(BTOSConfig config)
        {
            PartHelper helper = new PartHelper();

            if (config.parts == null)   //not initialized yet
            {
                Dictionary<Part, int> parts = new Dictionary<Part, int>();

                //fill up option detail.  Attention!  option can be a combination of product items
                Part part = null;

                if (config.BTOSConfigDetails != null)
                {
                    foreach (BTOSConfigDetail partItem in config.BTOSConfigDetails)
                    {
                        if (partItem.partX == null)
                        {
                            part = helper.getPart(partItem.SProductID, storeID);
                            if (part != null)
                            {
                                partItem.partX = part;
                            }
                        }
                        else
                        {
                            part = partItem.partX;
                        }
                        
                        if (part != null && part.isWarrantyPart() == false)    //not warranty item
                        {
                            if (parts.ContainsKey(part))
                                parts[part] = parts[part] + partItem.Qty;
                            else
                                parts.Add(part, partItem.Qty);

                           // partItem.partX = part;
                        }
                    }
                }

                //associate runtime property to config instance
                config.parts = parts;
            }

            return config.parts;
        }

        /// <summary>
        /// This method retrieves parts specifying warranty rate and warranty details in BTOS
        /// </summary>
        /// <returns></returns>
        private IDictionary<String, Part> getWarrantyList()
        {
            if (_warrantyList == null)
            {
                _warrantyList = new Dictionary<String, Part>();
                PartHelper helper = new PartHelper();

                foreach (BTOSConfig config in BTOSConfigs)
                {
                    //extended warranty category name shall contain words of "extended warranty" 
                    if (config!=null && config.isWarrantyConfig())
                    {
                        foreach (BTOSConfigDetail item in config.BTOSConfigDetails)
                        {
                            Part part = helper.getPart(item.SProductID, storeID);
                            if (part != null)
                                _warrantyList.Add(part.SProductID, part);
                        }
                    }
                }
            }

            return _warrantyList;
        }

        /// <summary>
        /// this function is get CTOS BTOSystem cost
        /// </summary>
        /// <param name="btos">BTOSystem , if btos is null will get ctos default BTOSystem</param>
        /// <returns></returns>
        public decimal getCost()
        {
            Decimal totalCost = 0m;
            foreach (var item in parts)
                totalCost += item.Key.CostX * item.Value;

            return totalCost;
        }



#endregion //Business Logic Extension

        #region OM_Only
        /// <summary>
        /// This method is an expensive method. When it's invoked, it will trigger a process to complete reconcile and recalculate
        /// entire BTOS price.  This method shall only be used in OM
        /// </summary>
        public void reconcile()
        {
            //reconcile cart item changes
            //get BTOS price without warranty]
            Decimal sum = 0m;
            foreach (BTOSConfig config in this.BTOSConfigs)
            {
                if (!config.isWarrantyConfig())
                {
                    config.AdjustedPrice = config.adjustedPrice;
                    sum += config.adjustedPrice * config.Qty;
                }
            }

            //get BTOS price without warranty]
            /*
            Decimal sum = 0m;
            foreach (BTOSConfig config in BTOSConfigs)
            {
                sum += config.adjustedPrice * config.Qty;
            }
             * */

            //reconcile warranty price as well
            this.updatePrice(sum, false);   //not to adjust BTOS config items
        }

        //获取btos下面所有产品是否都是 可用的。
        public Boolean isOrderable(string storeId)
        {
            PartHelper parthelper = new PartHelper();
            Boolean isOrderable = true;
            foreach (var configItem in BTOSConfigsWithoutNoneItems)
            {
                if (!configItem.isWarrantyConfig())
                {
                    foreach (var detailItem in configItem.BTOSConfigDetails.ToList())
                    {
                        Part partItem = detailItem.partX;
                        if (partItem == null)
                            partItem = parthelper.getPart(detailItem.SProductID, storeId, true);
                        if (partItem == null || !partItem.isOrderable())
                        {
                            isOrderable = false;
                            break;
                        }
                    }
                }

                if (!isOrderable)
                    break;
            }
            return isOrderable;
        }

        public bool? refresh()
        {
            bool? result = null ;
            PartHelper parthelper = new PartHelper();
            foreach (var configItem in BTOSConfigsWithoutNoneItems)
            {
                if (!configItem.isWarrantyConfig())
                {
                    bool itempricechanged = false;
                    foreach (var detailItem in configItem.BTOSConfigDetails.ToList())
                    {
                        Part partItem = detailItem.partX;
                        if (partItem == null)
                            partItem = parthelper.getPart(detailItem.SProductID, this.storeID, true);
                        if (partItem == null || !partItem.isOrderableBase())
                        {
                            result = false;
                            return result;
                            //break;
                        }
                        else
                        {
                            if (partItem.getNetPrice().value != detailItem.NetPrice)
                            {
                                itempricechanged = true;
                                detailItem.NetPrice = partItem.getNetPrice().value;
                                detailItem.AdjustedPrice = detailItem.NetPrice;
                            }
                        }
                    }
                    if (itempricechanged)
                    {
                        configItem.Price = configItem.BTOSConfigDetails.Sum(x => x.NetPrice * x.Qty).GetValueOrDefault();
                        configItem.AdjustedPrice = configItem.Price;
                        result = true;
                    }
                }
            }
            return result;
        }


        #endregion

        #region Unit Test
        /// <summary>
        /// This methos is only for unit test purpose.  When there is only one product input, it create a single product item.
        /// When there are more than one product involved, it create combo product item.
        /// </summary>
        /// <param name="mainItem"></param>
        /// <param name="qty"></param>
        /// <param name="combox"></param>
        /// <param name="comboxx"></param>
        public void addDummyItem(Product mainItem, int qty, Product combox = null, Product comboxx = null)
        {
            BTOSConfig config = new BTOSConfig();

            config.CategoryComponentID = 50001 + BTOSConfigs.Count;
            config.CategoryComponentDesc = "Dummy category";
            config.OptionComponentID = 60001 + BTOSConfigs.Count;
            config.OptionComponentDesc = "Dummy option";
            config.Qty = qty;

            //fill up option detail.  Attention!  option can be a combination of product items
            BTOSConfigDetail item = null;
            Price netPrice = null;

            netPrice = mainItem.getNetPrice();
            item = new BTOSConfigDetail();
            item.Description = mainItem.ProductDesc;
            item.Qty = 1;
            item.NetPrice = netPrice.value;
            item.AdjustedPrice = item.NetPrice;
            item.SProductID = mainItem.SProductID;
            item.MainProduct = mainItem.isMainStream();
            config.BTOSConfigDetails.Add(item);

            //additional item
            if (combox != null)
            {
                netPrice = combox.getNetPrice();
                item = new BTOSConfigDetail();
                item.Description = combox.ProductDesc;
                item.Qty = 1;
                item.NetPrice = netPrice.value;
                item.AdjustedPrice = item.NetPrice;
                item.SProductID = combox.SProductID;
                item.MainProduct = combox.isMainStream();

                //add config detail to BTOConfig
                config.BTOSConfigDetails.Add(item);
            }

            if (comboxx != null)
            {
                netPrice = comboxx.getNetPrice();
                item = new BTOSConfigDetail();
                item.Description = comboxx.ProductDesc;
                item.Qty = 1;
                item.NetPrice = netPrice.value;
                item.AdjustedPrice = item.NetPrice;
                item.SProductID = comboxx.SProductID;
                item.MainProduct = comboxx.isMainStream();

                //add config detail to BTOConfig
                config.BTOSConfigDetails.Add(item);
            }

            BTOSConfigs.Add(config);
        }

        /// <summary>
        /// This method will print out entire BTOS configuration to Console.  It's mainly for testing purpose.
        /// </summary>
        public void print()
        {
            StringBuilder content = new StringBuilder();

            //print user selected options
            content.AppendLine("\n\n*** BTOS configuration detail : " + BTOConfigID);
            content.AppendLine("NetPrice   AdjPrice   OrderQty     Component           Option  ");
            foreach (BTOSConfig config in BTOSConfigs)
                content.AppendLine(String.Format("{0:####.##}      {1:####.##}       {2:###}         {3}       {4}", config.netPrice, config.adjustedPrice, config.Qty, config.CategoryComponentDesc, config.OptionComponentDesc));

            content.AppendLine(String.Format("\n BTOS total is {0}\n", Price));
            content.AppendLine("*** End of BTOS configuration detail ***\n\n");

            Console.WriteLine(content);
        }

        /// <summary>
        /// This method is to print out parts used in BTOS composition
        /// </summary>
        public void printPartList()
        {
            StringBuilder content = new StringBuilder();

            content.AppendLine("\n\n*** Part List : \n" + BTOConfigID);
            foreach (KeyValuePair<Part, int> partItem in parts)
                content.AppendLine(String.Format("{0}    {1}    {2}", partItem.Key.SProductID, partItem.Key.name, partItem.Value));
            content.AppendLine("*** End of Part List\n");

            Console.WriteLine(content);
        }

#endregion //Unit Test
    }
}
