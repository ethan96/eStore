using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CTOSBOM
    {
#region Extension Constructors

#endregion

#region Extension Methods

        //runtime attributes
        private List<CTOSBOM> _options = new List<CTOSBOM>();
        private List<CTOSBOMComponentDetail> _componentDetails = new List<CTOSBOMComponentDetail>();  //this attribute will only have value when it's option
        public enum COMPONENTTYPE
        {
            OPTION = 0,
            CATEGORY = 1,
            ADDONCARDS = 2,
            ADDONMODULES = 3,
            ADDONDEVICES = 4,
            ACCESSORIES = 5,
            UNKNOWN = 9
        };
        public enum VALIDSTATE { INVALID = 0, VALID, VALID_WITH_ALTERNATIVE };
        private int _warrantyType = -1; //-1 mean not initilized yet, 0 mean not a warranty config, 1 mean a warranty config
        private ArrayList _errorMessages;
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

        /// <summary>
        /// This method will init this component and validate options in it.  It removes any problematic option
        /// and only leave eligible option for configuration.
        /// </summary>
        /// <returns></returns>
        public VALIDSTATE validateAndInit()
        {
            VALIDSTATE validStatus = VALIDSTATE.INVALID;

            try
            {
                if (type == COMPONENTTYPE.OPTION)
                {
                    validStatus = validateOption();
                }
                else if (type != COMPONENTTYPE.UNKNOWN)
                {
                    if (isExtentedFromProductCategory())
                    {
                        validStatus = validateExtendedCategory();
                    }
                    else
                    {
                        validStatus = validateComponent();
                    }
                }
                else
                {
                    validStatus = VALIDSTATE.INVALID;
                }
            }
            catch (Exception)
            {
                //eStoreLoger.Error("Exception at CTOSBOM init and validation", "", "", "", ex);
                validStatus = VALIDSTATE.INVALID;
            }

            return validStatus;
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
        public decimal diffByAlternativeItem = 0m;

        /// <summary>
        /// This method will init and validate all options assigned in present component. It filters out invalid options and
        /// reassign default option if there is need.
        /// </summary>
        /// <returns>
        ///     INVALID                 :   No orderable component is available in current CTOSBOM category.  In such case, CTOS which include 
        ///                                 this category shall not be orderable.
        ///     VALID                   :   CTOSBOM default selected component is go and valid.
        ///     VALID_WITH_ALTERNATIVE  :   CTOSBOM default selected component is not available, but an alternative component is available 
        ///                                 and is reassigned to be this CTOSBOM's default component. 
        ///                                 **** To make sure CTOS price reflects to this new change, CTOS default price needs to be recalculated.
        /// </returns>
        private VALIDSTATE validateComponent()
        {
            List<CTOSBOM> items = new List<CTOSBOM>();

            //reset options
            _options.Clear();

            //pick up valid options
            CTOSBOM defaultOption = null;
            foreach (CTOSBOM option in ChildComponents)
            {
                if (option.Show && option.type == COMPONENTTYPE.OPTION)  //it's a option component
                {
                    if (option.validateAndInit() != VALIDSTATE.INVALID)
                        items.Add(option);
                    else
                        ErrorMessages.AddRange(option.ErrorMessages);

                    //identify default option
                    if (option.Defaults == true)
                        defaultOption = option;
                }
            }

            var result = from item in items
                         orderby item.Seq, item.priceX
                         select item;
            _options = result.ToList<CTOSBOM>();

            if (_options.Count == 0)    //invalid component since there is no single option under it
                return VALIDSTATE.INVALID;

            //check if there is at least one default option
            var count = (from item in items
                        where item.Defaults == true
                        select item).Count();

            if (count <= 0 && !(string.IsNullOrEmpty(this.InputType) == false && this.InputType.ToUpper() == "MULTISELECT")) //no deafult item
            {
                //find the option having lowest price
                var list = items.OrderBy(item => item.Price);
                CTOSBOM cb = list.ElementAt(0);
                if (cb != null)
                {
                    //reset default bom -- one component only has one default bom
                    var defBom = ChildComponents.FirstOrDefault(c => c.Defaults);
                    if (defBom != null)
                        defBom.Defaults = false;
                    cb.Defaults = true;
                }

                if (defaultOption != null)
                    ErrorMessages.Add(string.Format("Default option {0} is invalid, default option is assigned to {1}.", defaultOption.CTOSComponent.ComponentName, cb.CTOSComponent.ComponentName));
                else
                    ErrorMessages.Add(string.Format("No default option available, {0} is set to the defalut.", cb.CTOSComponent.ComponentName));

                ChangeLog log = new eStore.POCOS.ChangeLog("mike.liu@advantech.com.cn", POCOS.ChangeLog.ModuleType.SyncJob.ToString(),
                string.Empty, POCOS.ChangeLog.ActiveType.ChnageDefaultOption.ToString(), 
                defaultOption == null ? string.Empty : defaultOption.ComponentID.ToString(),
                cb.ComponentID.ToString(), string.Empty);
                this.changeLogs.Add(log);
                diffByAlternativeItem += (cb.origPriceX - (defaultOption == null ? 0m : defaultOption.origPriceX));
                //need recalculateCTOSDefaultPrice
                return VALIDSTATE.VALID_WITH_ALTERNATIVE;
            }

            return VALIDSTATE.VALID;
        }


        /// <summary>
        /// This method validate current option and recalculate option price.  It return false if current option has problematic
        /// parts.
        /// </summary>
        /// <param name="errors">optional parameter -- if it's provided, this method will return the validation failure reasons</param>
        /// <returns></returns>
        private VALIDSTATE validateOption(List<String> errors = null)
        {
            if (type != COMPONENTTYPE.OPTION)
                return VALIDSTATE.INVALID;   //not an option item

            if (CTOSComponent.CTOSComponentDetails.Count == 0)
            {
                //special option, for instance "None" option or any space holding option
                //that has no physical part associated with.
                return VALIDSTATE.VALID;
            }

            VALIDSTATE validStatus = VALIDSTATE.VALID;
            Price = 0m;  //reset price and recalculate
            _componentDetails = new List<CTOSBOMComponentDetail>(); // 20180206 alex, prevent double add same object
            //regualr option
            foreach (CTOSComponentDetail item in CTOSComponent.CTOSComponentDetails)
            {
                //if part is a warrantee item, the part price will be percentage.  The total charge of this particular option
                //shall be calculated at runtime. 
                Part part = new PartHelper().getPart(item.SProductID, StoreID);
                if (part != null)
                {
                    CTOSBOMComponentDetail componentDetail = new CTOSBOMComponentDetail(StoreID, item.SProductID, part, item.Qty);
                    _componentDetails.Add(componentDetail);   //cache to prevent second round of DB visit
                    if (part.isWarrantyPart() == false)    //not warrantee item
                    {
                        Price price = part.getNetPrice(false);
                        if (price.value > 0)
                            Price += price.value * item.Qty * this.quantity;
                        else
                        {
                            //critical error
                            validStatus = VALIDSTATE.INVALID;
                            String errorMessage = String.Format("Price of {0} is not available at item {1} [Status:{2}]", part.SProductID, item.ComponentID, part.StockStatus);
                            //String errorMessage = String.Format("Price of <a href='{3}/Cbom/ComponentEdit.aspx?compontentID={4}&parentID={5}'>{0}</a> is not available at item {1} [Status:{2}]", part.SProductID, item.ComponentID, part.StockStatus,
                            //    System.Configuration.ConfigurationSettings.AppSettings["eStoreOMURL"],this.ComponentID,this.ParentComponent.ComponentID);
                            ErrorMessages.Add(errorMessage);
                            //eStoreLoger.Warn(errorMessage, "", StoreID);
                            if (errors != null)
                                errors.Add(errorMessage);
                        }
                    }
                }
                else   //part is not available
                {
                    String errorMessage = String.Format("Part {0} is not available / not valid at item {1}", item.SProductID, item.ComponentID);
                    //eStoreLoger.Warn(errorMessage, "", StoreID);
                    if (errors != null)
                        errors.Add(errorMessage);
                    validStatus = VALIDSTATE.INVALID;
                }
            }

            return validStatus;
        }

        private List< ProductCategory> _extendedCategories;
        public List<ProductCategory> extendedCategories
        {
            get
            {
                return _extendedCategories;
            }
        }

        public VALIDSTATE validateExtendedCategory()
        {
            if (!string.IsNullOrEmpty(this.CTOSComponent.SProductID))
            {
                _extendedCategories = new List<ProductCategory>();
                foreach (string categoryid in this.CTOSComponent.SProductID.Split(';'))
                {
                    CachePool cachePool = CachePool.getInstance();
                    ProductCategory _extendedCategory;
                    _extendedCategory = CachePool.getInstance().getProductCategory(StoreID, categoryid);

                    if (_extendedCategory == null)
                    {
                        ProductCategoryHelper helper = new ProductCategoryHelper();
                        _extendedCategory = helper.getProductCategory(categoryid, StoreID);

                        if (_extendedCategory != null)
                            cachePool.cacheProductCategory(_extendedCategory);
                    }
                    if (_extendedCategory != null)
                    {
                        _extendedCategories.Add(_extendedCategory);
                    }
                }
            }
            if (_extendedCategories.Any())
            {
                if (_extendedCategories.Count == 1 && _extendedCategories.First().childCategoriesX.Count > 0)
                {
                    _extendedCategories = _extendedCategories.First().childCategoriesX.ToList();
                }
                return VALIDSTATE.VALID;
            }
            else
                return VALIDSTATE.INVALID;
        }

        public IList options
        {
            get
            {
                if (_options.Count <= 0) //not initialized yet
                {
                    /* _options composition will be performed in init and validation stage
                    List<CTOSBOM> items = new List<CTOSBOM>();

                    //pick up option components
                    foreach (CTOSBOM option in ChildComponents )
                    {
                        if (option.Show && option.type == COMPONENTTYPE.OPTION)  //it's a option component
                        //if (option.type == COMPONENTTYPE.OPTION)  //*****Show is always false at this moment, so we temporary remove the check up
                            items.Add(option);
                    }

                    var result = from item in items
                                 orderby item.Seq,item.priceX
                                 select item;
                    _options = result.ToList<CTOSBOM>();
                     * */

                    if (!isExtentedFromProductCategory())
                        validateAndInit();      //perform validation, in this function, options will be constructed too.
                }
                
                return _options;
            }
        }

        /// <summary>
        /// Type is a readonly property indicating this component type.  There are three possible component types,
        /// CATEGORY, LIST and UNKNOWN.
        /// </summary>
        public COMPONENTTYPE type
        {
            get
            {
                if (_type != COMPONENTTYPE.UNKNOWN)
                    return _type;

                if (CTOSComponent != null)
                {
                    _type = CTOSComponent.type;
                }
                else
                    _type = COMPONENTTYPE.UNKNOWN;

                return _type;
            }
        }

        /// <summary>
        /// this method to check category can be composed to btos or not
        /// used by:
        /// 1. add to cart as single item
        /// 2. if not, add as btos
        /// </summary>
        /// <returns></returns>
        public bool isAddon()
        {
            return type == COMPONENTTYPE.ACCESSORIES || type == COMPONENTTYPE.ADDONDEVICES;
        }

        /// <summary>
        ///  this method to check category is imported from standard product category or not
        /// </summary>
        /// <returns></returns>
        public bool isExtentedFromProductCategory()
        {
            return CTOSComponent != null && CTOSComponent.isExtentedFromProductCategory();
        }

        public bool isCategory()
        {
            return CTOSComponent != null && CTOSComponent.isCategory();
        }

        private COMPONENTTYPE _type = COMPONENTTYPE.UNKNOWN;


        public String name
        {
            get { return CTOSComponent.ComponentName; }
        }

        public String desc
        {
            get { return CTOSComponent.ComponentDesc; }
        }

        /// <summary>
        /// This method is to find option based on option ID. Current way to matching is not efficient, but won't
        /// create too much loading neither, because number of option within a category component won't be too much.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public CTOSBOM findOption(int optionId)
        {
            foreach (CTOSBOM option in options)
            {
                if (option.ID == optionId)
                    return option;
            }

            return null;
        }

        /// <summary>
        /// This method is to identify CTOS options in current category(CTOS component) which match 
        /// the BTOS options specified in input BTOS config item.  Usually this function is
        /// used in presentation layer objects to reverse matching selected CTOS options from
        /// existing BTOS configuration and present those selected options in front end to show
        /// user what BTOS current configuration is.
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        public List<BTOSConfig> match(List<BTOSConfig> configs)
        {
            List<BTOSConfig> items = new List<BTOSConfig>();
            BTOSConfig defaultOption = null;

            foreach (BTOSConfig config in configs)
            {
                foreach (CTOSBOM option in options)
                {
                    if (option.CTOSComponent.ComponentID == config.OptionComponentID)
                    {
                        config.matchedOption = option;
                        items.Add(config);
                        break;  //break inner loop
                    }
                    //else if (option.InputType.Equals("Radio") && option.Defaults)
                    else if (option.Defaults)   //default options don't need to be radio type
                    {
                        //try to find default item
                        defaultOption = new BTOSConfig();
                        defaultOption.matchedOption= option;
                    }
                }
            }

            if (items.Count == 0 && defaultOption != null)
                items.Add(defaultOption);

            return items;
        }

        /// <summary>
        /// This property is intent to replace Quantity property.  Quantity property is a nullable and can be 0 value.
        /// This new property will return at least 1 if Quantity is either 0 or null
        /// </summary>
        public int quantity
        {
            get
            {
                if (Quantity.GetValueOrDefault() == 0)
                    Quantity = 1;

                return Quantity.GetValueOrDefault();
            }

            set { Quantity = value; }
        }

        /// <summary>
        /// this method is used mainly for prefetch Parts before CTOS uses them.  The return will be a comma seperated string
        /// </summary>
        /// <returns></returns>
        public String getPartList()
        {
            if (type != COMPONENTTYPE.OPTION || CTOSComponent.CTOSComponentDetails.Count == 0)
                return null;   //not an option item

            //regualr option
            StringBuilder parts = null;
            foreach (CTOSComponentDetail item in CTOSComponent.CTOSComponentDetails)
            {
                if (parts == null)  //first element
                {
                    parts = new StringBuilder();
                    parts.Append(item.SProductID);
                }
                else
                    parts.Append(",").Append(item.SProductID);
            }

            return parts.ToString();
        }

        /// <summary>
        /// the bom used parts
        /// </summary>
        /// <returns></returns>
        public List<Part> getUsePartList()
        {
            var parts = (new PartHelper()).prefetchPartList(StoreID, getPartList());
            return parts;
        }

        /// <summary>
        /// This method return the indicator of where this category component is one of CTOS spec sources or not
        /// </summary>
        /// <returns></returns>
        public Boolean isMainComponent()
        {
            return MainPart.GetValueOrDefault();
        }

        public Boolean isWarrantyType()
        {
            if (_warrantyType == -1)    //attribute is not initialized yet
            {
                //initialize attribute
                if (this.ComponentID== 2139||(this.ParentComponent!=null &&this.ParentComponent.ComponentID==2139))
                    _warrantyType = 1;
                else
                    _warrantyType = 0;
            }

            if (_warrantyType == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This is a read-only property for retrieving item price.
        /// </summary>
        public Decimal priceX
        {
            get
            {
                Decimal price = 0m;
                if (isWarrantyType())
                {
                    //assumpt warranty item only has single part associated with it
                    if (_componentDetails != null && _componentDetails.Count > 0)
                        price = _componentDetails[0].part.getNetPrice(false).value; //get non-rounded value
                }
                else
                    price = Price.GetValueOrDefault();

                return price;
            }
        }

        private Decimal origPriceX
        {
            get
            {
                Decimal price = 0m;
                if (_componentDetails != null && _componentDetails.Count > 0)
                {
                    foreach (var detail in _componentDetails)
                    {
                        if (detail.part != null)
                            price += detail.part.getOriginalPrice().value * detail.qty * this.quantity; ;
                    }
                }
                return price;
            }
        }

        
        /// <summary>
        /// this is a realonly property providing what parts are included in this option.
        /// </summary>
        public IList<CTOSBOMComponentDetail> componentDetails
        {
            get { return _componentDetails; }
        }

        private List<ProductLimitedResource> _limitedResources;
        /// <summary>
        /// A runtime property provides a list of limited resources used in an CBOM option component
        /// </summary>
        public List<ProductLimitedResource> limitedResources
        {
            get 
            {
                if(this.type == COMPONENTTYPE.OPTION && _limitedResources == null)
                {
                    _limitedResources = new List<ProductLimitedResource>();
                    if (this.componentDetails != null && this.componentDetails.Count > 0)
                    {
                        foreach (var d in this.componentDetails)
                        {
                            if (d != null && d.part != null && d.part.ProductLimitedResources != null && d.part.ProductLimitedResources.Count > 0)
                            {
                                if (d.qty > 1)
                                {
                                    foreach (var pr in d.part.ProductLimitedResources.ToList())
                                    {
                                        ProductLimitedResource resource = _limitedResources.FirstOrDefault(c => c.ResourceID == pr.ResourceID);
                                        if (resource != null)   //already exist
                                        {
                                            resource.AvaiableQty += pr.AvaiableQty * d.qty;
                                            resource.ConsumingQty += pr.ConsumingQty * d.qty;
                                        }
                                        else
                                        {
                                            resource = new ProductLimitedResource()
                                            {
                                                
                                                AvaiableQty = pr.AvaiableQty * d.qty,
                                                ConsumingQty = pr.ConsumingQty * d.qty,
                                                SProductID = pr.SProductID,
                                                LimitedResource = pr.LimitedResource
                                            };
                                            _limitedResources.Add(resource);
                                        }
                                    }
                                }
                                else
                                    _limitedResources.AddRange(d.part.ProductLimitedResources);
                            }
                                
                        }
                    }
                }
                return _limitedResources; 
            }
        }


        private String _limitedResourceExpression = null;
        /// <summary>
        /// get bom limited Resources
        /// PS: 0-1-2 ResourceID为0的Product 含有1个AvaiableQty 含有2个ConsumingQty
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public string limitedResourcesStr()
        {
            if (String.IsNullOrWhiteSpace(_limitedResourceExpression))
            {
                _limitedResourceExpression = String.Empty;
                if (this.limitedResources != null && this.limitedResources.Count > 0)
                {
                    _limitedResourceExpression = string.Join(";", limitedResources.Select(c => c.LimitedResource.Name.ToString() + "|" + c.AvaiableQty.ToString() + "|" + c.ConsumingQty.ToString()).ToArray());
                }
            }

            return _limitedResourceExpression;
        }


#endregion  //Extension Method

#region Unit Test

        /// <summary>
        /// This function is for unit test purpose only. To the maximun, 3 products can be assigned to this dummy
        /// option. The purpose of calling this method is to create a new dummy option in current category component.
        /// </summary>
        /// <param name="asDefault"></param>
        /// <param name="adjustRate"></param>
        /// <param name="item1"></param>
        /// <param name="item2">Optional</param>
        /// <param name="item3">Optional</param>
        public CTOSBOM addDummyOption(Boolean asDefault, Decimal adjustRate, Product item1, Product item2 = null, Product item3 = null)
        {
            //create an option
            CTOSBOM option = new CTOSBOM();

            if (adjustRate == 0)    //not a valid adjust rate
                adjustRate = 1.0m;
            option.AdjustRate = adjustRate;
            option.ID = ID + _options.Count + 1;

            option.Price = item1.getNetPrice().value;
            if (item2 != null)
                option.Price += item2.getNetPrice().value;
            if (item3 != null)
                option.Price += item3.getNetPrice().value;

            option.Defaults = asDefault;
            option.CreatedDate = DateTime.Now;
            option.InputType = "radio";
            option.Maxquantity = 1;
            option.Seq = option.ID;
            option.Show = true;
            option.SProductID = SProductID;
            option.StoreID = StoreID;
            option.ParentID = ID;
            option.Quantity = 1;

            //create dummy CTOSComponent for dummy option need
            CTOSComponent optionComp = CTOSComponentHelper.createDummyCTOSComponent("DummyOption"+option.ID, "DummyOption"+option.ID, 1);
            option.CTOSComponent = optionComp;

            StringBuilder compDesc = new StringBuilder();
            
            //fill up option detail.  Attention!  option can be a combination of product items
            CTOSComponentDetail item = null;

            item = new CTOSComponentDetail();
            compDesc.Append(item1.ProductDesc);
            item.Qty = 1;
            item.SProductID = item1.SProductID;
            optionComp.CTOSComponentDetails.Add(item);

            //additional item
            if (item2 != null)
            {
                item = new CTOSComponentDetail();
                compDesc.Append(item1.ProductDesc);
                item.Qty = 1;
                item.SProductID = item1.SProductID;
                optionComp.CTOSComponentDetails.Add(item);
            }

            if (item3 != null)
            {
                item = new CTOSComponentDetail();
                compDesc.Append(item1.ProductDesc);
                item.Qty = 1;
                item.SProductID = item1.SProductID;
                optionComp.CTOSComponentDetails.Add(item);
            }

            //add new option to current category component
            ChildComponents.Add(option);

            return option;
        }

#endregion
    }
}
