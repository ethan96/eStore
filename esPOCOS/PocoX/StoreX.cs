using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

 namespace eStore.POCOS{ 

public partial class Store {

    public StoreHelper helper;

    private Dictionary<string, string> _settings;
    public Dictionary<string, string> Settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (StoreParameter sp in this.StoreParameters)
                {
                    _settings.Add(sp.SiteParameter, sp.ParaValue);
                }

            }
            return _settings;
        }
        set
        {
            _settings = value;
        }
    }
    public Dictionary<string, string> ErrorCodes;
    public Dictionary<string, string> Localization;
    public enum BusinessGroup { None, eA, eP
    , IAG, ISG, ACG, ECG,EIOT,SIOT
    };
    public enum StoreType { eStore, uStore };

    public List<LocationIp> LocationIps;

#region Extension Methods 
     
    private Currency _defaultCurrency = null;
    private  Object _lock = new Object();
    /// <summary>
    /// This property returns the default currency of current store
    /// </summary>
    public Currency defaultCurrency
    {
        get
        {
            if (_defaultCurrency == null)
            {
                lock (_lock)
                {
                    try
                    {
                      
                        foreach (StoreCurrency currency in StoreCurrencies)
                        {
                            if (currency.Default.GetValueOrDefault())
                                _defaultCurrency = currency.Currency;
                        }
                    }
                    catch (Exception ex)
                    {
                        eStore.Utilities.eStoreLoger.Error("get defaultCurrency", "", "", "", ex);
                    }
                    
                }
            }

            return _defaultCurrency;
        }
    }

    public virtual String nextQuotationNumber
    {
        get
        {
            String prefix = "Q" + StoreID.Substring(1);
            String nextId = "";
            String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
            try
            {
                nextId = String.Format("{0:D6}", Convert.ToInt32(nextIdCandidate));
            }
            catch (Exception)
            {
                nextId = nextIdCandidate;
            }

            return prefix + nextId;
        }
    }

    public virtual String nextOrderNumber
    {
        get
        {
            String prefix = "O" + StoreID.Substring(1);
            String nextId = "";
            String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
            try
            {
                nextId = String.Format("{0:D6}", Convert.ToInt32(nextIdCandidate));
            }
            catch (Exception)
            {
                nextId = nextIdCandidate;
            }

            return prefix + nextId;
        }
    }


    public virtual String getNextQuotationNumber(string minorPrefix)
    {
        if (string.IsNullOrEmpty(minorPrefix) || minorPrefix.Length > 4)
        {
            throw new Exception("Minor Prefix is empty or over 3 characters.");
        }
        String prefix = "Q" + StoreID.Substring(1) + minorPrefix.ToUpper();
        String nextId = "";
        String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
        try
        {
            nextId = String.Format("{0:D" + (6 - minorPrefix.Length).ToString() + "}", Convert.ToInt32(nextIdCandidate));
        }
        catch (Exception)
        {
            nextId = nextIdCandidate;
        }

        return prefix + nextId;
    }

    public virtual String getNextOrderNumber(string minorPrefix)
    {
        if (string.IsNullOrEmpty(minorPrefix) || minorPrefix.Length > 4)
        {
            throw new Exception("Minor Prefix is empty or over 3 characters.");
        }
        String prefix = "O" + StoreID.Substring(1) + minorPrefix.ToUpper();
        String nextId = "";
        String nextIdCandidate = AllSequenceHelper.GetNewSeq(this, prefix);
        try
        {
            nextId = String.Format("{0:D" + (6 - minorPrefix.Length).ToString() + "}", Convert.ToInt32(nextIdCandidate));
        }
        catch (Exception)
        {
            nextId = nextIdCandidate;
        }

        return prefix + nextId;

    }
    private StoreAddress _eAStoreAddress = null;
    private StoreAddress _ePStoreAddress = null;
    public Address getStoreAddress(BusinessGroup businessGroup)
    {
        Address storeAddress = null;
        storeAddress = (from sa in this.StoreAddresses
                        where sa.Division.Equals(businessGroup.ToString(), StringComparison.OrdinalIgnoreCase)
                        select sa.Address).FirstOrDefault();

        if (storeAddress == null)
        {
            storeAddress = (from sa in this.StoreAddresses
                            where sa.Division.Equals("EP", StringComparison.OrdinalIgnoreCase)
                            select sa.Address).FirstOrDefault();
        }

        if (storeAddress != null && String.IsNullOrWhiteSpace(storeAddress.BankInformation))
            storeAddress.BankInformation = this.getLocalizedValue("eStore_Bank_Information_Context");

        return storeAddress;
    }

    private List<ProductBox> _StoreAvailableBoxs;
    public List<ProductBox> getStoreAvailableBox()
    {
        if (_StoreAvailableBoxs == null)
        {
            _StoreAvailableBoxs = (new ProductBoxHelper()).getStoreAvailableBox(this.StoreID);
        }
        return _StoreAvailableBoxs;
    }
    private ProductBox defaultBox;
    public ProductBox getDefaultBox()
    {
        if (defaultBox == null)
        {
            defaultBox = (new ProductBoxHelper()).getDefaultBox(this);
            if (defaultBox == null)
            {
                defaultBox = (new ProductBoxHelper()).getDefaultBox("AUS");
            }
        }
        return defaultBox;
    
    }

    private ProductBox _maximumBox;
    public ProductBox getMaximumBox()
    {
        if (_maximumBox == null)
        {
            _maximumBox = getStoreAvailableBox().OrderByDescending(x => x.volumn).FirstOrDefault();
        }
        return _maximumBox;

    }
    /// <summary>
    /// This method is to retrieve localization string from store localization string list
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public String getLocalizedValue(String keyword)
    {
        if (Localization.ContainsKey(keyword))
            return Localization[keyword];
        else
            return "";
    }

    public Country getCountry(String countryName)
    {
        Country country = null;
        countryName = countryName.ToLower();
        if (!String.IsNullOrWhiteSpace(countryName))
        {
            try
            {
                country = Countries.FirstOrDefault(c => c.CountryName.ToLower() == countryName
                             || c.Shorts.ToLower() == countryName);
            }
            catch (Exception)
            {
            }
        }

        return country;
    }

    private Decimal _roundingUnit = 0m;
    public Decimal roundingUnit
    {
        get
        {
            if (_roundingUnit == 0m)
            {
                try
                {
                    _roundingUnit = Convert.ToDecimal(Settings["RoundingUnit"]);
                }
                catch (Exception)
                {
                    _roundingUnit = 1m;
                }
            }

            return _roundingUnit;
        }
    }

    private Decimal _ctosRoundingUnit = 0m;
    public Decimal ctosRoundingUnit
    {
        get
        {
            if (_ctosRoundingUnit == 0m)
            {
                try
                {
                    _ctosRoundingUnit = Convert.ToDecimal(Settings["CtosRoundingUnit"]);
                }
                catch (Exception)
                {
                    _ctosRoundingUnit = 1m;
                }
            }

            return _ctosRoundingUnit;
        }
    }

    private decimal _productRoundingUnit = 0m;
    public decimal productRoundingUnit
    {
        get
        {
            if (_productRoundingUnit == 0m)
            {
                try
                {
                    _productRoundingUnit = Convert.ToDecimal(Settings["ProductRoundingUnit"]);
                }
                catch (Exception)
                {
                    _productRoundingUnit = 1m;
                }
            }

            return _productRoundingUnit;
        }
    }


    /// <summary>
    /// This method is to retrieve store setting and return in String format
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string getStringSetting(string key)
    {
        string rlt = string.Empty;
        if (Settings.ContainsKey(key))
        {
            rlt = Settings[key];
        }
        else
        {
            //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
        }
        return rlt;
    }

    /// <summary>
    /// This method is to retrieve store setting and return in Boolean format
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Boolean getBooleanSetting(string key,Boolean defaultValue=false)
    {
      
        if (Settings.ContainsKey(key))
        {
            Boolean.TryParse(Settings[key], out defaultValue);
        }
        else
        {
            //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
        }
        return defaultValue;
    }

    /// <summary>
    /// This method is to retrieve store setting and return in Integer format
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int getIntegerSetting(string key, int defaultValue = 0)
    {
        int rlt = 0;
        if (Settings.ContainsKey(key))
        {
            int.TryParse(Settings[key], out rlt);
        }
        else
        {
            rlt = defaultValue;
            //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
        }
        return rlt;
    }


    /// <summary>
    /// This method is to retrieve store setting and return in decimal format
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public decimal getDecimalSetting(string key,decimal defaultvalue=0)
    {
        decimal rlt = defaultvalue;
        if (Settings.ContainsKey(key))
        {
            decimal.TryParse(Settings[key], out rlt);
        }
        else
        {
            //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
        }
        return rlt;
    }

    private eStore.POCOS.PocoX.ChannelPartner _storeChannelAccount;
    /// <summary>
    /// To differentiate store itself from other channel partners, store may have its own channel ID assigned if 
    /// channel partner referral is supported.
    /// </summary>
    public eStore.POCOS.PocoX.ChannelPartner storeChannelAccount
    {
        get
        {
            if (_storeChannelAccount == null)
            {
                int channelId = getIntegerSetting("Store_Self_Channel_ID");
                try
                {
                    _storeChannelAccount = ChannelPartnerHelper.getChannelPartner(channelId);
                }
                catch (Exception)
                {
                }

                if (_storeChannelAccount == null)   //not defined
                {
                    //create a default Channel Partner instance for store itself if it's not defnitied
                    _storeChannelAccount = new PocoX.ChannelPartner();
                    _storeChannelAccount.Channelid = channelId;
                    //use eP as default address
                    Address storeAddress = this.getStoreAddress(BusinessGroup.eP);
                    //if eP address is not defined, use eA as secondary option
                    if (storeAddress == null)
                        storeAddress = this.getStoreAddress(BusinessGroup.eA);
                    //if none of eA or eP address is available, create a default address instance
                    if (storeAddress == null)
                    {
                        storeAddress = new Address();
                        storeAddress.Address1 = "";
                        storeAddress.AddressID = 0;
                        storeAddress.City = "";
                        storeAddress.Country = "";
                        storeAddress.State = "";
                        storeAddress.Tel = "";
                        storeAddress.ZipCode = "";
                    }

                    _storeChannelAccount.Address = storeAddress.Address1;
                    _storeChannelAccount.City = storeAddress.City;
                    _storeChannelAccount.Company = "ADVANTECH";
                    _storeChannelAccount.Country = storeAddress.Country;
                    _storeChannelAccount.Display = "ADVANTECH";
                    _storeChannelAccount.Email = this.OrderDeptEmail;
                    _storeChannelAccount.Phone = storeAddress.Tel;
                    _storeChannelAccount.Zip = storeAddress.ZipCode;
                }
            }

            return _storeChannelAccount;
        }

        set { _storeChannelAccount = value; }
    }

    /// <summary>
    /// This property indicates whether current store refers order to channel partner or not
    /// </summary>
    public Boolean channelPartnerReferralEnabled
    {
        get { return getBooleanSetting("Support_Channel_Partner"); }
    }

    private String[] _orderablePartStates = null;
    public String[] getOrderablePartStates()
    {
        if (_orderablePartStates == null)
        {
            string validproductstatus = getStringSetting("ProductLogisticsStatus");
            if (string.IsNullOrEmpty(validproductstatus) == false)
            {
                validproductstatus = validproductstatus.ToUpper();
            }
            else
            {
                //set default if it's not set
                validproductstatus = "A,N,S,H,O";
            }
            _orderablePartStates = validproductstatus.Split(',');
        }

        return _orderablePartStates;
    }

    /// <summary>
    /// format contact address string
    /// </summary>
    /// <param name="contact"></param>
    /// <returns></returns>
    public string formatContactAddress(CartContact contact)
    {
        string addressStr = "";
        var formatAddress = this.getStringSetting("ContactAddressFormat");
        if (string.IsNullOrEmpty(formatAddress))
            formatAddress = "{0}, {1}  {2}  {3}  {4} ,{5}";
        addressStr = string.Format(formatAddress, (string.IsNullOrEmpty(contact.Address2)) ? contact.Address1 : contact.Address1 + "<br /> " + contact.Address2,
            (string.IsNullOrEmpty(contact.County)) ? "" : contact.County, contact.City, (string.IsNullOrEmpty(contact.State)) ? "" : contact.State, contact.ZipCode, contact.Country);
        return addressStr;
    }

    /// <summary>
    /// format contact phone string
    /// </summary>
    /// <param name="contact"></param>
    /// <returns></returns>
    public string formatContactPhone(CartContact contact)
    {
        string phoneStr = "";
        var formatPhone = this.getStringSetting("ContactPhoneFormat");
        if (string.IsNullOrEmpty(formatPhone))
            formatPhone = "Phone: {0}-{1}";
        phoneStr = string.Format(formatPhone, contact.TelNo, contact.TelExt);
        return phoneStr;
    }

    /// <summary>
    /// Special Produc list
    /// </summary>
    private Dictionary<string, List<SpecialProductPrice>> _specialProductList;
    public Dictionary<string, List<SpecialProductPrice>> SpecialProductList
    {
        get 
        {
            if (_specialProductList == null)
            {
                if (SpecialProductPrices != null)
                {
                    Dictionary<string, List<SpecialProductPrice>> ls = new Dictionary<string, List<SpecialProductPrice>>();
                    var cc = from c in SpecialProductPrices
                             orderby c.ruleType
                             group c by c.RuleType into g
                             select new
                             {
                                 key = g.Key,
                                 value = g
                             };
                    foreach (var c in cc.ToList())
                    {
                        ls.Add(c.key, c.value.ToList());
                    }
                    _specialProductList = ls;
                }
                else
                    _specialProductList = new Dictionary<string, List<SpecialProductPrice>>();
            }
            return _specialProductList; 
        }
        set { _specialProductList = value; }
    }

    /// <summary>
    /// fix some Special Product Price
    /// </summary>
    /// <param name="part"></param>
    public void fixSpecialProductPrice(Part part, ref bool PriceChanged)
    {
        SpecialProductPrice spp = null;
        foreach (var c in SpecialProductList)
        {
            if (spp != null)
                break;
            switch (c.Key)
            {
                case "Equals":
                    spp = c.Value.FirstOrDefault(v => v.Rule.Equals(part.SProductID));
                    break;
                case "StartsWith":
                    spp = (from v in c.Value
                           where part.SProductID.StartsWith(v.Rule)
                           select v).FirstOrDefault();
                    break;
                case "EndsWith":
                    spp = (from v in c.Value
                           where part.SProductID.EndsWith(v.Rule)
                           select v).FirstOrDefault();
                    break;
                default:
                    break;
            }
        }
        if (spp != null)
        {
            part.VendorSuggestedPrice = part.VendorSuggestedPrice * spp.FixRate;
            PriceChanged = true;
        }
    }
        private List<POCOS.UserGrade> _userGradeX;
        public List<POCOS.UserGrade> userGradeX
        {
            get
            {
                if (_userGradeX == null)
                {
                    _userGradeX = new List<UserGrade>();
                    _userGradeX = helper.getUserGrade(this);
                }
                return _userGradeX;
            }
        }

        /// <summary>
        /// This method is to convert the source value in store currency to user preferred currency.  If the user specified currency
        /// is not supported in current store, it returns 0.  Otherwise, it returns the exchanged value.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="targetCurrency"></param>
        /// <returns></returns>
        public Decimal getCurrencyExchangeValue(Decimal sourceValue, Currency targetCurrency)
    {
        if (targetCurrency != null)
            return getCurrencyExchangeValue(sourceValue, defaultCurrency, targetCurrency);
        else
            return 0;   //not valid
    }

    /// <summary>
    /// This method is to convert the source value in source currency to target currency.  If the user specified currency
    /// is not found, it returns 0.  Otherwise, it returns the exchanged value.
    /// </summary>
    /// <param name="sourceValue"></param>
    /// <param name="targetCurrency"></param>
    /// <returns></returns>
    public Decimal getCurrencyExchangeValue(Decimal sourceValue, Currency sourceCurrency, Currency targetCurrency)
    {
        if (sourceCurrency == null || targetCurrency == null)
            return 0;

        Decimal converted = (sourceValue * sourceCurrency.ToUSDRate.GetValueOrDefault()) / targetCurrency.ToUSDRate.GetValueOrDefault();
        return Converter.round(converted, this.StoreID);
    }


    /// <summary>
    /// This method is a overloaded method which calculate the exchange value from source currency to target currency
    /// </summary>
    /// <param name="sourceValue"></param>
    /// <param name="sourceCurrencyID"></param>
    /// <param name="targetCurrencyID"></param>
    /// <returns></returns>
    public Decimal getCurrencyExchangeValue(Decimal sourceValue, String sourceCurrencyID, String targetCurrencyID)
    {
        if (String.IsNullOrEmpty(sourceCurrencyID) || String.IsNullOrEmpty(targetCurrencyID))
            return 0;

        Currency sourceCurrency = getCurrency(sourceCurrencyID);
        Currency targetCurrency = getCurrency(targetCurrencyID);

        return getCurrencyExchangeValue(sourceValue, sourceCurrency, targetCurrency);
    }

    /// <summary>
    /// This method is to look up currency setting and exchange rate.  If there is nothing matched, it returns null
    /// </summary>
    /// <param name="currencyID"></param>
    /// <returns></returns>
    private Currency getCurrency(String currencyID)
    {
        if (currencies.ContainsKey(currencyID))
            return _currencies[currencyID];
        else
            return null;
    }
    private Dictionary<String, Currency> _currencies = null;
    protected Dictionary<String, Currency> currencies
    {
        get
        {
            if (_currencies == null)
            {
                //initialize and build up currencies
                _currencies = new Dictionary<String, Currency>();
                CurrencyHelper helper = new CurrencyHelper();
                List<Currency> currencies = helper.getAllCurrency();
                foreach (Currency currency in currencies)
                {
                    if (_currencies.ContainsKey(currency.CurrencyID) == false)
                        _currencies.Add(currency.CurrencyID, currency);
                }
            }
            return _currencies;
        }
    }

    public List<Currency> getCurrencyBySign(String currencySign)
    {
        return currencies.Values.Where(c => c.CurrencySign.Equals(currencySign, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public POCOS.MiniSite getMiniSiteByApplicationPath(string appPath,string domain="")
    {
        if (this.MiniSites == null)
            return null;
        try
        {
            return this.MiniSites.FirstOrDefault(x => x.ApplicationPath.Equals(appPath,StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrEmpty(domain) &&!string.IsNullOrEmpty(x.StoreURL)&& x.StoreURL.Equals(domain, StringComparison.OrdinalIgnoreCase))
                );
        }
        catch (Exception)
        {
            return null;
        }
       
    }

    public POCOS.MiniSite getMiniSiteBySiteName(string name)
    {
        if (this.MiniSites == null)
            return null;
        try
        {
            return this.MiniSites.FirstOrDefault(x => x.SiteName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception)
        {
            return null;
        }

    }

    private Language _defaultlanguage;
    public Language defaultlanguage
    {
        get 
        {
            if (_defaultlanguage == null)
                _defaultlanguage = (new LanguageHelper()).getLangByCode(this.StoreLangID);
            return _defaultlanguage; 
        }
        private set { _defaultlanguage = value; }
    }

    private List<Shortcut> _shortcuts;
    public List<Shortcut> shortcuts
    {
        get
        {
            if (_shortcuts == null)
            {
                _shortcuts = new ShortcutHelper().getShortcutList(this.StoreID).ToList();
                if (_shortcuts == null)
                    _shortcuts = new List<Shortcut>();
            }
            return _shortcuts;
        }
    }
    private List<Translation> _translationX;
    public List<Translation> translationX
    {
        get {
            if (_translationX == null)
            {
                _translationX = (new DAL.TranslationHelper()).getTranslationByStore(this.StoreID);
            }
            return _translationX;
        }
    }
    public string getShortShippingMethodDescription(string shipDes)
    {
        string _SpecialShippingMethodDescription = shipDes;
        if (StoreID == "AEU")
        {
            if (shipDes == "TNT Express (Delivery time: 1-2 days)")
                _SpecialShippingMethodDescription = "TNT Global";
            else if (shipDes == "TNT Economy (Delivery time: 2-5 days)")
                _SpecialShippingMethodDescription = "TNT Economy";
        }
        return _SpecialShippingMethodDescription;
    }
#endregion 

#region OM_Only
    public StoreAddress getStoreAddress(int addressID)
    {
        return this.StoreAddresses.FirstOrDefault(p => p.AddressID == addressID);
    }

    /// <summary>
    /// There are only two business groups available, EA and EP
    /// </summary>
    /// <param name="businessGroup"></param>
    /// <returns></returns>
    public StoreAddress getStoreAddress(String businessGroup)
    {
        return StoreAddresses.FirstOrDefault(sAddr => sAddr.Division == businessGroup.ToUpper());
    }

#endregion

    #region for ustore overwrite
    public virtual List<string> MenuTypeList
    {
        get
        {
            return Enum.GetNames(typeof(POCOS.Menu.MenuPosition)).ToList();
        }
    }

    public virtual string templateSubFolder
    {
        get {
            return string.Empty;
        }
    }

    #endregion
} 
 }