using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{

    public partial class Product_Ctos
    {
        #region Extension Methods
        //the following will be runtime attributes only and will not be stored in DB
        private List<CTOSBOM> _components = new List<CTOSBOM>();
        private List<CTOSBOM> _defaultOptions = null;
        private List<Part> _defaultSpecSources = null;
        private Boolean? _valid;
        private Dictionary<String, Part> _warrantyList = null;
        private ArrayList _errorMessages = null;
        public ArrayList ErrorMessages
        {
            get
            {
                if (_errorMessages == null)
                    _errorMessages = new ArrayList();
                return _errorMessages;
            }
            set { _errorMessages = value; }
        }
        private List<ChangeLog> _changeLogs;
        public List<ChangeLog> changeLogs
        {
            get
            {
                if (_changeLogs == null)
                    _changeLogs = new List<ChangeLog>();
                return _changeLogs;
            }
        }
        private decimal diffByAlternativeItem = 0m;

        /// <summary>
        /// This function is to qualify CTOS current available options. It removes problematic options that have parts 
        /// either not existing or 0 price and only offer qualfied options to user. If the default option is invalid, 
        /// this method will assign another option that has the lowest price to be default option.  
        /// This method usually is invoked once only when the product instance is first loaded.  However this method can 
        /// be invoked at any time per need.
        /// Change to private to prevent creating duplicate records
        /// </summary>
        /// <returns></returns>
        private Boolean validateAndInit()
        {
            try
            {
                //if (_components.Count() > 0)
                //    return _valid;

                //prefetch parts for performance boost

                StringBuilder parts = null;
                foreach (CTOSBOM component in CTOSBOMs)
                {
                    if (component.Show && component.type == CTOSBOM.COMPONENTTYPE.OPTION)
                    {
                        String optionParts = component.getPartList();
                        if (optionParts != null && optionParts.Length > 0)
                        {
                            if (parts == null)  //first part list
                            {
                                parts = new StringBuilder();
                                parts.Append(optionParts);
                            }
                            else
                                parts.Append(",").Append(optionParts);
                        }
                    }
                }

                (new PartHelper()).prefetchPartList(StoreID, parts.ToString());

                List<CTOSBOM> items = new List<CTOSBOM>();
                bool forceRecalculateCTOSDefaultPrice = false;

                //pick up category components and validate options at the same time
                foreach (CTOSBOM component in CTOSBOMs)
                {
                    if (component.Show == false || !component.isCategory())
                        continue;
                    if ( !component.isExtentedFromProductCategory())
                    {
                        CTOSBOM.VALIDSTATE CATEGRORYvalidate = component.validateAndInit();
                        if (CATEGRORYvalidate != CTOSBOM.VALIDSTATE.INVALID)
                        { 
                            items.Add(component);
                            if (CATEGRORYvalidate == CTOSBOM.VALIDSTATE.VALID_WITH_ALTERNATIVE)
                            {
                                foreach (var log in component.changeLogs)
                                {
                                    log.StoreID = this.StoreID;
                                    log.DocID = this.SProductID;
                                    this.changeLogs.Add(log);
                                }
                                forceRecalculateCTOSDefaultPrice = true;
                                diffByAlternativeItem += component.diffByAlternativeItem;
                            }
                        }
                        else
                        {
                            items.Add(component);
                            //critical error
                            _valid = false;
                            String errorMessage = String.Format("CTOS {0} {1} has problematic component {2} -- {3}", SProductID, this.DisplayPartno, component.ComponentID, component.desc);
                            ErrorMessages.Add(errorMessage);
                            //eStoreLoger.Error(errorMessage, "", StoreID);
                        }
                        if (component.ErrorMessages != null && component.ErrorMessages.Count > 0)
                            ErrorMessages.AddRange(component.ErrorMessages);
                    }
                    else
                    {
                        if (component.validateExtendedCategory() != CTOSBOM.VALIDSTATE.INVALID)
                            items.Add(component);
                        else
                        {
                            String errorMessage = String.Format("CTOS {0} {1} has problematic EXTENDEDCATEGORY {2} -- {3}", SProductID, this.DisplayPartno, component.ComponentID, component.desc);
                            ErrorMessages.Add(errorMessage);
                            //eStoreLoger.Error(errorMessage, "", StoreID);
                        }
                    }
                }

                //sort the component order by its sequence number
                var q = from item in items
                        orderby item.Seq
                        select item;
                _components = q.ToList<CTOSBOM>();
                if (!_valid.HasValue)
                    _valid = true;
                if (forceRecalculateCTOSDefaultPrice)
                {
                    Decimal ctosRoundingUnit = this.storeX != null ? this.storeX.ctosRoundingUnit : 1m;
                    updateProductNetPrice(recalculateCTOSDefaultPrice(ctosRoundingUnit));
                }

            }
            catch (Exception ex)
            {
                ErrorMessages.Add("Exception at CTOS validation:" + ex.Message);
                //eStoreLoger.Error("Exception at CTOS validation", "", "", "", ex);
                _valid = false;
            }
            if (ErrorMessages != null && ErrorMessages.Count > 0)
            {
                ErrorMessages.Insert(0, string.Format("Product {0}({1}) validateAndInit Failed", this.name, this.SProductID));
                eStoreLoger.Fatal(string.Join("|", ErrorMessages.ToArray()), "", "", StoreID);
            }

            return _valid.GetValueOrDefault();
        }

        public override bool notAvailable
        {
            get
            {
                return (this.status == PRODUCTSTATUS.INACTIVE || this.status == PRODUCTSTATUS.DELETED ||
                            this.status == PRODUCTSTATUS.INACTIVE_AUTO || this.status == PRODUCTSTATUS.TOBEREVIEW || this.status == PRODUCTSTATUS.PHASED_OUT);
            }
        }

        // ctos phased out status will can not order dable
        public override bool isOrderable(bool individual = false)
        {
            if (this.status == PRODUCTSTATUS.PHASED_OUT)
                return false;
            return base.isOrderable(individual);
        }

        /// <summary>
        /// This method is CTOS particular. It return the valid status of present CTOS.  CTOS can become invalid when
        /// some parts in it are either phased-out or have 0 price and can not find any replacing item.  If CTOS has been validated yet
        /// , this method initiate its validation.
        /// </summary>
        /// <returns></returns>
        public Boolean isValid()
        {
            if (this.isInited() == false)
                initialize();

            return _valid.GetValueOrDefault();
        }

        /// <summary>
        /// This property is to retrieve sorted list of CTOS category components sorted by Seq. Caller shall be able to 
        /// retrieve options associating with each category component
        /// </summary>
        public IList components
        {
            get
            {
                if (this.isInited() == false)
                    initialize();

                return _components;
            }
        }

        /// <summary>
        /// This method to initialize complete CTOS components.
        /// </summary>
        public void initialize()
        {
            lock (_components)
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

        /// <summary>
        /// This method is used to find match option
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public CTOSBOM findOption(int componentId, int optionId)
        {
            foreach (CTOSBOM component in components)
            {
                if (component.ID == componentId)
                {
                    foreach (CTOSBOM option in component.options)
                    {
                        if (option.ParentID == componentId && option.ID == optionId)  //find match
                            return option;
                    }
                }
            }

            //no match
            return null;
        }

        /// <summary>
        /// This method will return match category component.  It return null if there is no match found.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public CTOSBOM findComponent(int componentId)
        {
            foreach (CTOSBOM component in components)
            {
                if (component.ID == componentId)
                    return component;
            }

            return null;
        }

        /// <summary>
        /// This method will create a new BTOS based on CTOS default configuration
        /// </summary>
        public BTOSystem getDefaultBTOS()
        {
            BTOSystem btos = new BTOSystem(StoreID, nextBTOSId, BTONo);

            //SBC CTOS has no existing template for configuration. Thus its default BTO system is empty
            //if (isSBCCTOS())
            //    return btos;

            //compose BTOSystem with default selection by iterating through category components
            foreach (CTOSBOM component in components)
            {
                //pick up default options from each category
                foreach (CTOSBOM option in component.options)
                {
                    if (option.Defaults)
                        btos.addItem(component, option, option.quantity);
                }
            }

            //SBC CTOS has no pre-assigned price, its default price shall be from its default items
            if (!isSBCCTOS())
                updateBTOSPrice(btos);

            return btos;
        }

        /// <summary>
        /// default sample is used only for CTOS internal reference purpose.  To cache this defaultBTOS
        /// is for performance consideration.
        /// </summary>
        private BTOSystem _defaultSample = null;
        private BTOSystem defaultSample
        {
            get
            {
                if (_defaultSample == null)
                    _defaultSample = getDefaultBTOS();

                return _defaultSample;
            }
        }

        /// <summary>
        /// This method returns user selected options in specified category
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public List<BTOSConfig> getSelectedComponentOptions(BTOSystem btos, int componentId)
        {
            CTOSBOM component = findComponent(componentId);
            return getSelectedComponentOptions(btos, component);
        }

        /// <summary>
        /// This method returns user selected options based on the specified category in
        /// BTOS configuration
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public List<BTOSConfig> getSelectedComponentOptions(BTOSystem btos, CTOSBOM component)
        {
            //find match BTOS category(component) config item
            List<BTOSConfig> selected = btos.getComponentConfigs(component);

            //return seledted option list in specified category(component)
            return component.match(selected);
        }


        /// <summary>
        /// This method is to recalculate BTOS price based on CTOS default listing price and BTOS current configuration.
        /// </summary>
        /// <param name="btos"></param>
        /// <param name="newListingPrice">An output parameter -- new BTOS listing price will reflect in this variable.</param>
        /// <param name="newPriceBeforeAdjustment">An output parameter -- new BTOS cross-out price will reflect in this variable.</param>
        /// <returns>the returnign pricing mode indicate whether this BTOS product has special price or not</returns>
        public PRICINGMODE updateBTOSPrice(BTOSystem btos, Price newListingPrice, Price newPriceBeforeAdjustment = null)
        {
            //first calculate total price of all BTOS config items with current product net price
            Decimal btosItemsPrice = getBTOSConfigTotal(btos);

            //then calculate total price of default items
            Decimal defaultItemsPrice = getDefaultItemsTotal();

            Decimal priceDiff = 0m;
            if (newPriceBeforeAdjustment != null)
                priceDiff = newPriceBeforeAdjustment.value - newListingPrice.value;

            //At the end using CTOS default listing price + the price difference 
            //between BTOS config and default items (the price delta between default and user selections)
            PRICINGMODE pricingMode = getListingPrice(newListingPrice, newPriceBeforeAdjustment);
            btos.updatePrice(newListingPrice.value + (btosItemsPrice - defaultItemsPrice));
            newListingPrice.value = btos.Price;
            if (newPriceBeforeAdjustment != null)
                newPriceBeforeAdjustment.value = newListingPrice.value + priceDiff;

            return pricingMode;
        }

        /// <summary>
        /// This method is to recalculate BTOS price based on CTOS listing price and BTOS current configuration.
        /// </summary>
        /// <param name="btos"></param>
        /// <returns></returns>
        public Price updateBTOSPrice(BTOSystem btos)
        {
            Price price = new Price();

            updateBTOSPrice(btos, price);
            return price;
        }

        public bool hasMainComponent(BTOSystem btos)
        {
            bool rlt = false;
            foreach (CTOSBOM  bom in components)
            {
                if (bom.MainPart.GetValueOrDefault())
                {
                    if (btos.BTOSConfigs.Any(x => x.CategoryComponentID == bom.ComponentID))
                    {
                        rlt = true;
                        break;
                    }
                }
            }
            return rlt;
        }

        public Dictionary<int, decimal> getWarrantyItemsPrice(CTOSBOM warrantycomponent, BTOSystem btos)
        {
            Dictionary<int, decimal> warrantyitemsprice = new Dictionary<int, decimal>();
            if (warrantycomponent == null || btos == null)
                return warrantyitemsprice;

            decimal warrantablePrice = btos.getWarrantableTotal();
            foreach (CTOSBOM option in warrantycomponent.options)
            {
                if (warrantyitemsprice.ContainsKey(option.ID) == false)
                    warrantyitemsprice.Add(option.ID, Converter.round(warrantablePrice * option.priceX * option.quantity / 100, this.StoreID));
            }
            return warrantyitemsprice;
        }


        /// <summary>
        /// This method will try to print out CTO System content to Console for validation purpose.
        /// </summary>
        /// <param name="detailLevel">
        ///     1 : default options only
        ///     2 : Configurable components, available options and user selected options
        /// </param>
        public void print(int detailLevel)
        {
            StringBuilder content = new StringBuilder();
            switch (detailLevel)
            {
                case 2:
                    printConfigurableContent();
                    printDefaultSelections();
                    break;
                case 1:
                default:
                    printDefaultSelections();
                    break;
            }
        }

        /// <summary>
        /// This property return a list of parts providing CTOS spec literatures 
        /// (usually renderred in CTOS product detail page). The part list provides here
        /// is the parts assigned in default major components.
        /// </summary>
        public List<Part> specSources
        {
            get
            {
                try
                {
                    if (_defaultSpecSources == null)
                    {
                        _defaultSpecSources = new List<Part>();

                        //identify default categories first
                        StringBuilder partList = null;
                        foreach (CTOSBOM component in components)
                        {
                            if (component.isMainComponent()) //if this component is one of main spec sources
                            {
                                foreach (CTOSBOM option in component.options)
                                {
                                    //get parts from default options
                                    if (option.Defaults)
                                    {
                                        String optionParts = option.getPartList();
                                        if (!String.IsNullOrEmpty(optionParts))
                                        {
                                            if (partList == null)  //first part list
                                            {
                                                partList = new StringBuilder();
                                                partList.Append(optionParts);
                                            }
                                            else
                                                partList.Append(",").Append(optionParts);
                                        }
                                    }
                                }
                            }
                        }

                        //gather spec sourcing parts
                        if (partList != null)
                        {
                            PartHelper helper = this.parthelper;
                            if (helper == null) //this condition shall not exist
                                helper = new PartHelper();

                            String[] partIds = partList.ToString().Split(',');
                            Part part = null;
                            foreach (String partId in partIds)
                            {
                                part = helper.getPart(partId, this.StoreID);
                                if (part != null && part.isMainStream())
                                    _defaultSpecSources.Add(part);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add("Fails at gathering defautl spec parts:" + ex.Message);
                    //eStoreLoger.Error("Fails at gathering defautl spec parts", this.DisplayPartno, this.SProductID, this.StoreID, ex);
                }

                return _defaultSpecSources;
            }
        }

        /// <summary>
        /// There is a special type of CTOS called SBC-BTO. This type of CTOS is to compose
        /// SBC-BTO system.  SBC-BTO is composed in different way from regular BTO system since
        /// there is no existing CTO system configuration as template for building up BTOS.
        /// SBC-BTO is composed directly from putting together a list of specified products.
        /// </summary>
        /// <returns></returns>
        public Boolean isSBCCTOS()
        {
            if (SProductID.ToUpper().StartsWith("SBC-BTO"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// This methos is a overloaded function for querying listingPrice based on the latest BTOS configuration
        /// instead of default BTOS.
        /// </summary>
        /// <param name="listingPrice"></param>
        /// <param name="priceBeforeAdjustment"></param>
        /// <param name="btos"></param>
        /// <returns></returns>
        public PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment, BTOSystem btos)
        {
            //get CTOS default listing price first            
            PRICINGMODE mode = getListingPrice(listingPrice, priceBeforeAdjustment);
            if (isValid() == false)    //CTOS has setting problem and is not legible for order
                mode = PRICINGMODE.NOTAVAILABLE;

            switch (mode)
            {
                case PRICINGMODE.SPECIAL:    //has special promotion
                    Decimal priceDelta = priceBeforeAdjustment.value - listingPrice.value;
                    listingPrice.value = btos.Price;
                    priceBeforeAdjustment.value = listingPrice.value + priceDelta;
                    break;
                case PRICINGMODE.REGULAR:
                    listingPrice.value = btos.Price;
                    break;
                default:
                    listingPrice.value = 0m;
                    priceBeforeAdjustment.value = 0m;
                    break;
            }

            if (btos.getCost() > listingPrice.value)
                listingPrice.value = 0;

            return mode;
        }


        /// <summary>
        /// This methos is a override function for querying listingPrice based on the default BTOS.
        /// </summary>
        /// <param name="listingPrice"></param>
        /// <param name="priceBeforeAdjustment">This parameter can be null if caller doesn't care about price before adjustment</param>
        /// <returns></returns>
        public new PRICINGMODE getListingPrice(Price listingPrice, Price priceBeforeAdjustment)
        {
            //get CTOS default listing price first            
            PRICINGMODE mode = PRICINGMODE.NOTAVAILABLE;
            if (isValid() == false)    //CTOS has setting problem and is not legible for order
            {
                mode = PRICINGMODE.NOTAVAILABLE;
                listingPrice.value = 0m;
                if (priceBeforeAdjustment != null)
                    priceBeforeAdjustment.value = 0m;
            }
            else
            {
                //through IsValid checkup, CTOS price will be recalculated if some of its defualt selected components are phased-out and are replaced
                //by alternative components.
                mode = base.getListingPrice(listingPrice, priceBeforeAdjustment);
            }

            return mode;
        }


        public override String thumbnailImageX
        {
            get
            {
                if (String.IsNullOrWhiteSpace(base.thumbnailImageX))
                {
                    //find thumbnail image
                    if (specSources != null && specSources.Count > 0)
                        this.ImageURL = specSources[0].thumbnailImageX;

                    return base.thumbnailImageX;
                }
                else
                    return base.thumbnailImageX;
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

        /// <summary>
        /// get ctos main part 
        /// </summary>
        /// <returns></returns>
        public POCOS.Part mainPartX()
        {
            if (specSources != null && specSources.Any())
                return specSources[0];
            else
                return null;
        }


        private Store.BusinessGroup _businessGroup = Store.BusinessGroup.None;
        /// <summary>
        /// This property indicates which contact group shall the CTOS belongs to
        /// </summary>
        public override Store.BusinessGroup businessGroup
        {
            get
            {
                if (this.StoreID == "AEU" || this.StoreID == "AUS")
                    return base.businessGroup;

                if (_businessGroup == POCOS.Store.BusinessGroup.None)
                {
                    /*  too heavy
                    int eAPartCount = 0;
                    int ePPartCount = 0;

                    foreach (CTOSBOM option in getDefaultOptions())
                    {
                        if (option.isMainComponent())
                        {
                            foreach (Part part in option.parts)
                            {
                                //only count Advantech main stream product
                                if (part != null && part.isMainStream())
                                {
                                    if (part.isEAProduct())
                                        eAPartCount += option.quantity;
                                    else
                                        ePPartCount += option.quantity;
                                }
                            }
                        }
                    }

                    if (ePPartCount >= eAPartCount)
                        _businessGroup = Store.BusinessGroup.eP;
                    else
                        _businessGroup = Store.BusinessGroup.eA;
                    */

                    switch (this.CTOSProductLine)
                    {
                        case "Embedded Automation":
                        case "Touch Panel PC (TPC)":
                        case "Workstations (AWS)":
                        case "IPPC":
                            _businessGroup = Store.BusinessGroup.eA;
                            break;
                        default:
                            _businessGroup = Store.BusinessGroup.eP;
                            break;
                    }
                }

                return _businessGroup;
            }
        }

        /// <summary>
        /// This method is to generate BTOS Id
        /// </summary>
        private String nextBTOSId
        {
            get
            {
                String id = SProductID + DateTime.Now.ToString(".yyyyMMdd_HHmmss_fff");
                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void printConfigurableContent()
        {
            StringBuilder content = new StringBuilder();

            //print configurable components and their options
            content.AppendLine("\n\n*** System configurable selection list ***");
            foreach (CTOSBOM component in components)
            {
                content.AppendLine(String.Format("-------- {0} ----- {1} ---------------", component.ID, component.name));
                content.AppendLine("  DefaultItem  Price     OptionID     OptionName");
                foreach (CTOSBOM option in component.options)
                    content.AppendLine(String.Format("{0}        {1:#####.##}        {2}   {3}   ", (option.Defaults) ? 'Y' : 'N', option.Price.GetValueOrDefault(), option.ID, option.name));
            }

            content.AppendLine("*** End of System configurable selection list *** \n\n");
            Console.WriteLine(content);
        }

        /// <summary>
        /// 
        /// </summary>
        private void printDefaultSelections()
        {
            StringBuilder content = new StringBuilder();

            //print user selected options
            content.AppendLine("\n\n*** User selected options ***");
            content.AppendLine(" Price   OptionID   Name             ");
            foreach (CTOSBOM option in getDefaultOptions())
                content.AppendLine(String.Format("{0:#####.##}       {1} {2} : ", option.Price.GetValueOrDefault(), option.ID, option.name));
            content.AppendLine("*** End of User selected options ***\n\n");

            Console.WriteLine(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<CTOSBOM> getDefaultOptions()
        {
            if (_defaultOptions == null)
            {
                _defaultOptions = new List<CTOSBOM>();

                //initialize _defaultOptions
                foreach (CTOSBOM component in components)
                {
                    //pick up default options from each category
                    foreach (CTOSBOM option in component.options)
                    {
                        if (option.Defaults)
                            _defaultOptions.Add(option);
                    }
                }
            }

            return _defaultOptions;
        }

        /// <summary>
        /// The method will sum up total price of all CTOS default items. The total amount excludes warranty cost.
        /// </summary>
        /// <returns></returns>
        private Decimal getDefaultItemsTotal()
        {
            Decimal total = 0m;
            IDictionary<String, Part> warrantyList = getWarrantyList();

            foreach (CTOSBOM item in getDefaultOptions())
            {
                if (!item.isWarrantyType()) //exclude warranty cost
                    total += item.Price.GetValueOrDefault();
            }

            return total;
        }
        
        /// <summary>
        /// This method is to look up CTOS options to find a match option with BTOS config item.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private CTOSBOM findMatchedOption(BTOSConfig config)
        {
            //will consider to replace this logic with LINQ later
            foreach (CTOSBOM item in CTOSBOMs)
            {
                if (item.type == CTOSBOM.COMPONENTTYPE.OPTION && item.ComponentID == config.OptionComponentID)
                    return item;
            }

            return null;    //no match
        }

        /// <summary>
        /// The method will sum up total price of all BTOS configuration items.  The total amount excludes warranty cost.
        /// </summary>
        /// <returns></returns>
        private Decimal getBTOSConfigTotal(BTOSystem btos)
        {
            Decimal total = 0m;
            IDictionary<String, Part> warrantyList = getWarrantyList();

            foreach (BTOSConfig item in btos.BTOSConfigs)
            {
                if (!item.isWarrantyConfig())   //exclude warranty cost
                {
                    CTOSBOM option = findMatchedOption(item);
                    if (option != null)
                        total += option.Price.GetValueOrDefault() * item.Qty;
                    else if (item.CategoryComponentID == 999999)
                    {
                        total += item.adjustedPrice * item.Qty;
                    }
                }
            }

            return total;
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

                foreach (CTOSBOM component in components)
                {
                    //extended warranty category name shall contain words of "extended warranty" 
                    if ((component != null) && component.isWarrantyType())
                    {
                        foreach (CTOSBOM option in component.options)
                        {
                            foreach (CTOSComponentDetail detail in option.CTOSComponent.CTOSComponentDetails)
                            {
                                Part part = helper.getPart(detail.SProductID, StoreID);
                                if ((part != null) && (!_warrantyList.ContainsKey(part.SProductID)))
                                    _warrantyList.Add(part.SProductID, part);
                            }
                        }
                    }
                }
            }

            return _warrantyList;
        }

        /// <summary>
        /// this function will get BTOSystem btos Cost
        /// </summary>
        /// <param name="btos">if user change comb will use this btos , else will use default btos</param>
        /// <returns></returns>
        public bool checkIsBelowCost(BTOSystem btos = null)
        {
            BTOSystem _btos = btos;
            if (_btos == null)
                _btos = defaultSample;

            POCOS.Price listprice = new Price(), markupprice = new Price();
            getListingPrice(listprice, markupprice, btos);

            return btos.getCost() > listprice.value;
        }

        /// <summary>
        /// 虚料号是否通过了验证
        /// </summary>
        /// <returns></returns>
        public bool noAssemblyVerify()
        {
            if (this.Assembly.HasValue && !this.Assembly.GetValueOrDefault())
            {
                var _mainpart = mainPartX();
                if (_mainpart != null)
                    return _mainpart.SProductID == this.BTONo;
                else
                    return true;
            }
            else
                return true;
        }

        //获取ctos下面mainpart的Certificates
        public override List<Certificates> Certificates
        {
            get
            {
                if (_certificates == null)
                {
                    _certificates = new List<Certificates>();
                    if (specSources != null && specSources.Any())
                    {
                        foreach (var partItem in specSources)
                        {
                            List<Certificates> partCertificate = partItem.Certificates;
                            if (partCertificate != null && partCertificate.Count > 0)
                            {
                                foreach (var certificatesItem in partCertificate)
                                {
                                    Certificates existsItem = _certificates.FirstOrDefault(p => p.CertificateName == certificatesItem.CertificateName
                                                                                            && p.CertificateImagePath == certificatesItem.CertificateImagePath);
                                    if (existsItem == null)
                                        _certificates.Add(certificatesItem);
                                }
                            }
                        }
                    }
                }
                return _certificates;
            }
        }


        public CTOSComponent getComponentBySprocutid(string sproductid)
        {
            foreach (CTOSBOM component in components)
            {
                if (component.isMainComponent()) //if this component is one of main spec sources
                {
                    foreach (CTOSBOM option in component.options)
                    {
                        //get parts from default options
                        if (option.Defaults)
                        {
                            foreach (CTOSComponentDetail item in option.CTOSComponent.CTOSComponentDetails)
                            {
                                if (item.SProductID == sproductid)
                                    return option.CTOSComponent;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public CTOSComponent getRootComponentBySprocutid(string sproductid)
        { 
            var component = getComponentBySprocutid(sproductid);
            if (component != null)
                return component.rootComponent;
            else
                return null;

        }
        
        #endregion

        #region OM_Methods
        /// <summary>
        /// This method will release default options cache and force CTOS to rebuild its default option list
        /// </summary>
        public void resetDefaultOptions()
        {
            _defaultOptions = null;
        }

        /// <summary>
        /// This method will try to recalculate CTOS default price based on its current default options.
        /// It'll update CTOS price scheme and return the final price to the caller.
        /// </summary>
        /// <param name="roundingUnit"></param>
        /// <returns></returns>
        public Price recalculateCTOSDefaultPrice(Decimal roundingUnit = -1.0m)
        {
            Price price = new Price();
            price.value = getDefaultItemsTotal();

            if (!isValid())
                price.value = 0m;

            if (price.value > 0)
            {
                this.SystemEstimateSum = price.value;

                //performan store pricing rounding
                //The logic here will be hard coded for US at this moment.  The rounding unit need to be defined in DB for all regions
                if (roundingUnit < 0)
                    roundingUnit = 5;   //US default rounding unit
                price.value = Utilities.Converter.round(price.value, roundingUnit);
                //overwrite local store price only when the price source is specified as "Vendor".  When price is from Vendor
                // CTOS price will always been automatically updated.

                if (diffByAlternativeItem != 0m && hasSpecialPrice())
                {

                    ChangeLog log = new eStore.POCOS.ChangeLog("mike.liu@advantech.com.cn", POCOS.ChangeLog.ModuleType.SyncJob.ToString(),
                                                                                                        this.SProductID, POCOS.ChangeLog.ActiveType.ChnagePromotionPrice.ToString(),
                                                                                                         this.PromotePrice.ToString(),
                                                                                                        (this.PromotePrice + diffByAlternativeItem).ToString(), this.StoreID);

                    this.changeLogs.Add(log);

                    this.PromotePrice = PromotePrice + diffByAlternativeItem;
                    this.PromoteMarkup = PromoteMarkup + diffByAlternativeItem;
                    diffByAlternativeItem = 0m;
                }
            }

            if (this.priceSourceX == PRICESOURCE.VENDOR)
                this.StorePrice = price.value;
            this.VendorSuggestedPrice = price.value;

            return price;

        }

        public override bool isUseLimiteResource
        {
            get
            {
                //switch (this.StoreID)
                //    {
                //        case "AEU":
                //            return this.SProductID == "21653";
                //        default:
                //            return this.DisplayPartno.ToUpper().StartsWith("SYS-");

                //    }
                if (this.LimitResourceIsChecked == true) // LimitResourceIsChecked field ㄓ耞
                    return true;
                else
                    return false;

            }
        }

        /// <summary>
        /// get All Product LimitedResource for the system
        /// </summary>
        /// <param name="defaultOnly">if defaultOnly is true, then will only list default items' Limited Resource</param>
        /// <returns></returns>
        public List<ProductLimitedResource> getAllProductLimitedResource(bool defaultOnly = false)
        {
            List<ProductLimitedResource> allSeletedResources = new List<ProductLimitedResource>();

            foreach (CTOSBOM com in this.components)
            {
                foreach (CTOSBOM option in com.options)
                {

                    if (defaultOnly)
                    {
                        if (option.Defaults == true)
                        {
                            foreach (var item in option.limitedResources)
                                item.IsDefaultItem = 1;
                            allSeletedResources.AddRange(option.limitedResources);
                        }
                    }
                    else
                    {
                        if (option.Defaults && option.limitedResources != null && option.limitedResources.Count > 0)
                        {
                            foreach (var item in option.limitedResources)
                                item.IsDefaultItem = 1;
                        }
                        allSeletedResources.AddRange(option.limitedResources);
                    }

                }

            }            
            return allSeletedResources;
        }

        //public Tuple<Dictionary<CTOSBOM, Dictionary<ProductLimitedResource, bool>>, bool> VerifyLimitedResourceSettings()
        //{
        //    var  verifyLimitedResourceSettingsDetail = VerifyLimitedResourceSettingsDetail();
        //    bool result = true;
        //    foreach(var option in verifyLimitedResourceSettingsDetail)
        //    {
        //        foreach (var resourceSetting in option.Value)
        //        {
        //            if (resourceSetting.Value == false)
        //                result = false;
        //        }
        //    }
        //    return new  Tuple<Dictionary<CTOSBOM, Dictionary<ProductLimitedResource, bool>>, bool>(verifyLimitedResourceSettingsDetail, result);
        //}


        public List<ProductLimitedResource> GetProdcutLimitedResourceSettingsWithValidateResult(bool defaultOnly = false)
        {
            var limitedResources = getAllProductLimitedResource(defaultOnly);

            foreach (var resource in limitedResources)
            {
                if (resource.AvaiableQty > 0 && resource.ConsumingQty == 0)// available resource
                {
                    resource.IsValid = true;
                    resource.ValidateMessage = "Valid";
                }
                else if (resource.ConsumingQty > 0 && resource.AvaiableQty == 0)// ocuppied resource
                {
                    var IsValid = false;
                    //var InValidMessage = "";
                    foreach (var oResourceItems in resource.LimitedResource.Name.Split(','))
                    {

                        if (limitedResources
                        .Any(x => x.AvaiableQty > 0
                        && x.LimitedResource.Name.Split(',').Contains(oResourceItems)
                        && x.AvaiableQty >= resource.ConsumingQty))
                        {
                            IsValid = true;
                            break;
                        }
                        //else
                        //{
                        //    //IsValid = false;
                        //    //InValidMessage += oResourceItems + "'s AvaiableQty < ConsumingQty" + "<br>";
                        //    //break;
                        //}

                    }

                    if (IsValid)
                        resource.ValidateMessage = "Valid";
                    else
                        resource.ValidateMessage = "AvaiableQty < ConsumingQty" + "<br>"; ;
                    resource.IsValid = IsValid;
                    
                }
                else if(resource.ConsumingQty == 0 && resource.AvaiableQty == 0)
                {
                    resource.IsValid = false;
                    resource.ValidateMessage = "Both avaiableQty and consumingQty are eqal zero.";
                }
                else if (resource.ConsumingQty > 0 && resource.AvaiableQty > 0)
                {
                    resource.IsValid = false;
                    resource.ValidateMessage = "Both avaiableQty and consumingQty are larger than zero.";
                }
                else
                {
                    resource.IsValid = false;
                    resource.ValidateMessage = "Invalid";
                }
            }

            return limitedResources;
        }


        //public Dictionary<CTOSBOM, Dictionary<ProductLimitedResource, bool>> VerifyLimitedResourceSettingsDetail()
        //{
        //    List<CTOSBOM> Occupancy = new List<CTOSBOM>();
        //    List<CTOSBOM> Avaiable = new List<CTOSBOM>();
        //    //bool result = true;
        //    //get all Avaiable and Occupancy
        //    foreach (CTOSBOM com in this.components)
        //    {
        //        foreach (CTOSBOM option in com.options)
        //        {

        //            if (option.limitedResources != null && option.limitedResources.Count > 0)
        //            {
        //                if (option.limitedResources.Any(x => x.AvaiableQty > 0))
        //                {
        //                    Avaiable.Add(option);
        //                }

        //                if (option.limitedResources.Any(x => x.ConsumingQty > 0))
        //                {
        //                    Occupancy.Add(option);
        //                }

        //            }
        //        }

        //    }

        //    var limitedResourceSettingsResult = new Dictionary<CTOSBOM, Dictionary<ProductLimitedResource, bool>>();

        //    //checking all Occupancy option has 
        //    foreach (var oOptions in Occupancy)
        //    {
        //        var productLimitResourceResult = new Dictionary<ProductLimitedResource, bool>();
        //        foreach (var oResource in oOptions.limitedResources.Where(x => x.ConsumingQty > 0))
        //        {
        //            bool oResourceItemsResult = false;
        //            foreach (var oResourceItems in oResource.LimitedResource.Name.Split(','))
        //            {
        //                if (Avaiable.Any(option => option.limitedResources
        //                .Any(x => x.AvaiableQty > 0
        //                && x.LimitedResource.Name.Split(',').Contains(oResourceItems)
        //                && x.AvaiableQty >= oResource.ConsumingQty)))
        //                    oResourceItemsResult = true;
        //                else
        //                {
        //                    oResourceItemsResult = false;
        //                    break;
        //                }

        //            }

        //            productLimitResourceResult.Add(oResource, oResourceItemsResult);

        //            //if (oResourceItemsResult == false)
        //            //{
        //            //    result = false;
        //            //    break;
        //            //}

        //        }
        //        limitedResourceSettingsResult.Add(oOptions, productLimitResourceResult);
        //    }

        //    return limitedResourceSettingsResult;
        //}

        /// <summary>
        /// validate Limited Resource for default items 
        /// </summary>
        /// <returns></returns>
        public bool validateLimitedResource()
        {
            bool result = false;
            if (this.isValid())
            {
                result = true;
                List<ProductLimitedResource> allSeletedResources = getAllProductLimitedResource(true);
                if (allSeletedResources.Count > 0)// have resource setting
                {
                    //checking left Avaiable Qty after Complete Mateched(without ,)
                    var afterCompleteMateched = (from r in allSeletedResources
                                                 group r by r.LimitedResource into g
                                                 where g.Key.Name.Contains(",") == false
                                                 select new ProductLimitedResource
                                                 {
                                                     LimitedResource = g.Key
                                                     ,
                                                     AvaiableQty = g.Sum(x => x.AvaiableQty) - g.Sum(x => x.ConsumingQty)
                                                 }
                                       ).ToList();

                    //can't find available resouce or haven't enough available resouce 
                    if (afterCompleteMateched == null || afterCompleteMateched.Where(atpX => atpX.AvaiableQty < 0).Count() > 0)
                    {
                        result = false;
                    }
                    else//checking Name with ','
                    {
                        foreach (ProductLimitedResource plr in allSeletedResources.Where(x => x.LimitedResource.Name.Contains(',')))
                        {
                            var match = afterCompleteMateched.FirstOrDefault(x => plr.LimitedResource.Name.Split(',').Contains(x.LimitedResource.Name));
                            if (match != null && match.AvaiableQty >= plr.ConsumingQty)
                            {
                                match.AvaiableQty = match.AvaiableQty - plr.ConsumingQty;
                            }
                            else
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
                else//no resource setting, ignore
                    result = true;
            }
            return result;
        }

        /// <summary>
        /// validate Limited Resource for default items 
        /// </summary>
        /// <returns></returns>
        public bool LimitedResourceIsValidated(bool defaultOnly = false)
        {
            bool result = true;
            var productLimitResourceSetting = this.GetProdcutLimitedResourceSettingsWithValidateResult(defaultOnly);
            
            if (this.isValid())
            {
                if ((productLimitResourceSetting.Count() > 0))
                {
                    if (productLimitResourceSetting.Any(p => p.IsValid == false))
                        result = false;
                }                 
            }
            else
                result = false;
            return result;
        }

        /// <summary>
        /// 过滤ProductLimitedResource重复数据。
        /// </summary>
        private class ProductLimitedResourceComparer : IEqualityComparer<ProductLimitedResource>
        {
            public bool Equals(ProductLimitedResource x, ProductLimitedResource y)
            {
                if (x == null || y == null)
                    return false;
                if (x.StoreID == y.StoreID && x.SProductID == y.SProductID && x.ResourceID == y.ResourceID)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(ProductLimitedResource obj)
            {
                return obj.GetType().GetHashCode();
            }
        }

        #endregion

        #region Unit Test
        public CTOSBOM createDummyComponent(String name, String desc, int seq)
        {
            CTOSBOM component = new CTOSBOM();
            component.ID = _components.Count + 1;
            component.CreatedDate = DateTime.Now;
            component.StoreID = StoreID;
            component.Show = true;
            component.SProductID = SProductID;
            component.Seq = seq;

            //create dummy CTOSComponent for dummy category component need
            CTOSComponent categoryComp = CTOSComponentHelper.createDummyCTOSComponent(name, desc, 0);
            component.CTOSComponent = categoryComp;
            component.ComponentID = categoryComp.ComponentID;

            return component;
        }

        public void addDummyComponent(CTOSBOM component)
        {
            //_components.Add(component);

            //populate options in this category to CTOSBOMs
            CTOSBOMs.Add(component);
            foreach (CTOSBOM option in component.options)
                CTOSBOMs.Add(option);
        }

        public static int unitTest()
        {
            return 0;
        }
        #endregion //Unit Test
    }   //class
}   //name space
