using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using eStore.BusinessModules;
using System.Web.Security;
using eStore.POCOS.DAL;
using eStore.Presentation.SearchConfiguration;

namespace eStore.Presentation
{
    public partial class eStoreContext : System.Web.SessionState.IRequiresSessionState
    {
        #region Fields

        private Store _store;

        private POCOS.User _eStoreUser;
        private HttpContext _context = HttpContext.Current;
        private POCOS.Cart _UserShoppingCart;

        private POCOS.Currency _WorkingCurrency;
        private POCOS.Store.BusinessGroup _BusinessGroup = POCOS.Store.BusinessGroup.eP;
        POCOS.Quotation _quotation;
        POCOS.Order _Order;
        Dictionary<String, String> _keywords;

        private string _sessionID;
        public string SessionID
        {
            get 
            {
                if (_sessionID == null)
                    _sessionID = _context.Session.SessionID;
                return _sessionID; 
            }
            set { _sessionID = value; }
        }

        //如果url有errorPath  就移除  (排除域名意外的url)
        public string CurrentPagePath
        {
            get { return ripErrorPath(_context.Request.RawUrl);  }
        }

        public string ripErrorPath(string origPath)
        {
            int errorPathIndex = origPath.ToLower().IndexOf("?aspxerrorpath=");
            if (errorPathIndex > 0) //has error path
                return origPath.Substring(0, errorPathIndex);
            else
                return origPath;
        }

        #endregion

        #region constructor


        /// <summary>
        /// Creates a new instance of the eStoreContext class
        /// </summary>
        private eStoreContext()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            string _storeid;
            if (!isTestMode() || System.Configuration.ConfigurationManager.AppSettings["UseHostname"] == "true")
            {
                _storeid = _context.Request.ServerVariables["SERVER_NAME"];
                _store = eSolution.getStore(_storeid);
            }            
            else
            {
                if (!string.IsNullOrEmpty(_context.Request["storeid"]))
                {
                    _storeid = _context.Request["storeid"];
                    if (_storeid.Contains(','))     //This logic is to resovle duplicated storeid caused continuously changing country
                    {
                        _storeid = _storeid.Split(',').Last();
                    }
                    if (_context.Session != null)
                        _context.Session.Clear();
                    _context.Session["CurrentStore"] = _storeid;
                }
                else
                    _storeid = this.getStoreIDFromSession();

                //_storeid = this.getStoreIDFromCookie();
                if (string.IsNullOrEmpty(_storeid))
                {
                    _storeid = this.getStoreIDFromConfiguration();
                }
                _store = eSolution.getStore(_storeid);
            }

            if (_store == null)
            {

                try
                {

                    System.Net.IPAddress serverip = new System.Net.IPAddress(0);
                    //estore doesn't support IP access
                    if (System.Net.IPAddress.TryParse(_storeid, out serverip))
                    {
                        var ipstore = BusinessModules.StoreSolution.getInstance().locateStore(null, _storeid);
                        if (ipstore != null)
                        {
                            _context.Response.Redirect($"{(ipstore.profile. getBooleanSetting("IsSecureEntireSite", false) ? "https": "http")}://{ipstore.profile.StoreURL}");
                        }
                        else
                        {
                            Utilities.eStoreLoger.Info(string.Format("fake request for {0}", _storeid));
                            _context.Response.Redirect("/alive.html");
                        }
                    }
                    else
                    {
                        throw new Exception("init request failed.");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("init request failed.");
                }

            }

            _miniSite = _store.profile.getMiniSiteByApplicationPath(_context.Request.ApplicationPath, _context.Request.ServerVariables["SERVER_NAME"]);


            if (getBooleanSettingFromDB("LocateStoreByIP", this._currentCountry,  true) == false)
            {
                _currentCountry = _store.profile.Country;
                return;
            }


            //if not first access, _context.Session["seletedcountry"] is exists
            if (string.IsNullOrWhiteSpace(_context.Request["country"]) == false)
            {
                string country = _context.Request["country"];
                if (country.Contains(','))  //This logic is to resovle duplicated storeid caused continuously changing country
                {
                    country = country.Split(',').Last();
                }
                POCOS.Country selectedcountry = _store.profile.getCountry(country);
                if (selectedcountry != null)
                    _currentCountry = selectedcountry;
            }
            else if (_context.Session["seletedcountry"] != null)
                _currentCountry = eSolution.getCountrybyCodeOrName((string)_context.Session["seletedcountry"]);
            else if (_context.Request.Cookies["country"] != null)
            {
                //if session lost, try to get from cookies
                _currentCountry = eSolution.getCountrybyCodeOrName(_context.Request.Cookies["country"].Value);
            }
            if (!string.IsNullOrWhiteSpace(_context.Request["f"]))
            {
                _context.Session["fromStore"] = _context.Request["f"];
            }

            //first time
            if (_currentCountry == null)
            {
                POCOS.Country iplocatedcountry = null;
                string _countryCode = string.Empty;
                _countryCode = eSolution.getCountryCodeByIp(getUserIP());
                iplocatedcountry = eSolution.getCountrybyCodeOrName(_countryCode);

                var isLocalIp = esUtilities.IPUtility.IpIsWithinBoliviaRange(getUserIP(),
                          Store.profile.LocationIps.Select(c => c.IPAtrrs).ToList());
                // is local ip will not chech ip and country.


                //only redirect traffic if the request is from live person.  Allow robot crawlers to index any targeting store.
                if (iplocatedcountry != null && isRequestFromSearchEngine() == false && !isLocalIp)
                {
                    bool isFromOthereStore = false;
                    if (string.IsNullOrWhiteSpace(_context.Request["token"]) == false)
                    {
                        string token = _context.Request["token"];
                        if (token.Contains(','))    //This logic is to resovle duplicated storeid caused continuously changing country
                        {
                            token = token.Split(',').Last();
                        }
                        long timespan = 0;
                        long.TryParse(token, out timespan);
                        isFromOthereStore = DateTime.Compare(DateTime.Now, new DateTime().AddTicks(timespan).AddMinutes(1)) < 1;
                    }
                    if (iplocatedcountry.StoreID != _store.storeID && !isFromOthereStore)
                    {
                        //redirct by set CurrentCountry
                        CurrentCountry = iplocatedcountry;
                    }
                    else if (iplocatedcountry.StoreID == _store.storeID)
                    {
                        _currentCountry = iplocatedcountry;
                    }
                    else
                    {
                        Store targetStore = eSolution.locateStore(_store, iplocatedcountry.Shorts);
                        if (targetStore.storeID != _store.storeID)
                        {
                            _currentCountry = iplocatedcountry;
                        }
                    }
                }
                else
                    _currentCountry = _store.profile.Country;
            }


            CurrentCountry = _currentCountry;
            
        }
        #endregion 

        #region Methods

        /// <summary>
        /// isRequestFromSearchEngine is to determine whether current HTTP request is from robot search agent or
        /// from live person.  True means the search is from robot agent.  False means the search is made by live person.
        /// </summary>
        /// <returns></returns>
        public Boolean isRequestFromSearchEngine()
        {
            String userAgent = _context.Request.UserAgent;
            if (String.IsNullOrWhiteSpace(userAgent))
                userAgent = "N/A";
            userAgent = userAgent.ToLower();

            StoreSolution solution = StoreSolution.getInstance();
            Boolean isSearchAgent = solution.isRobotAgent(userAgent);
            if (isSearchAgent == false && _context.Request.Browser.Type.StartsWith("Unknown") == false)
                return solution.isIPFromKnowSearchEngine(getUserIP());
            else
                return true;
        }

        private POCOS.Country _currentCountry;
        public POCOS.Country CurrentCountry
        {
            get
            {
                return _currentCountry;
            }

            set
            {
                if (value == null)
                {
                    //_currentCountry = this.Store.profile.Country;
                    //_context.Session["seletedcountry"] = _currentCountry;
                }
                else if (value.StoreID != Store.storeID)
                {                    
                    string oldurlpara = "";
                    Dictionary<string, string> keyvalues = new Dictionary<string, string>();
                    var querystring = _context.Request.QueryString;
                    var allkeys = querystring.AllKeys;
                    if (allkeys != null && allkeys.Any())
                    {
                        if (allkeys.Contains("storeid", StringComparer.OrdinalIgnoreCase))
                            keyvalues.Add("storeid", querystring["storeid"]);
                        if (allkeys.Contains("country", StringComparer.OrdinalIgnoreCase))
                            keyvalues.Add("country", querystring["country"]);
                        if (allkeys.Contains("token", StringComparer.OrdinalIgnoreCase))
                            keyvalues.Add("token", querystring["token"]);
                        if (allkeys.Contains("f", StringComparer.OrdinalIgnoreCase))
                            keyvalues.Add("f", querystring["f"]);
                    }
                    foreach (var c in keyvalues)
                    {
                        if (string.IsNullOrEmpty(oldurlpara))
                            oldurlpara = c.Key + "=" + c.Value;
                        else
                            oldurlpara += "&" + c.Key + "=" + c.Value;
                    }

                    string newurlpara = string.Format(isTestMode() ? "storeid={0}&country={1}&token={2}&f={3}" : "country={1}&token={2}&f={3}"
                                , value.StoreID
                                , value.CountryName
                                , DateTime.Now.Ticks.ToString()
                                , Store.storeID);
                    string page = CurrentPagePath;

                    //cannot find target's same minisite, go to store page
                    if (MiniSite != null && value.storeX.getMiniSiteBySiteName(MiniSite.SiteName) == null)
                    {
                        //avoid exception: Index and count must refer to a location within the string
                        //if (!string.IsNullOrEmpty(MiniSite.ApplicationPath) && CurrentPagePath.StartsWith(MiniSite.ApplicationPath))
                        //    page = CurrentPagePath.Remove(0, MiniSite.ApplicationPath.Length);

                        page = "";
                        //本机测试没有用
                        if (isTestMode())
                            page = string.Format("http://{0}", _context.Request.Url.Authority);
                    }


                    string url = !string.IsNullOrEmpty(oldurlpara) ? page.Replace(oldurlpara, newurlpara) :
                                        (page + (page.Contains('?') ? "&" + newurlpara : "?" + newurlpara));

                    if (isTestMode())
                    {
                        _context.Session.Clear();
                        _context.Session["CurrentStore"] = value.StoreID;
                        _context.Response.Redirect(url);
                    }
                    else
                    {
                        //如果有对应minisite，切换到对应domain
                        string domain = value.storeX.StoreURL;
                        if (MiniSite != null)
                        {
                            var targetminisite = value.storeX.getMiniSiteBySiteName(MiniSite.SiteName);
                            if (targetminisite != null && !string.IsNullOrEmpty(targetminisite.StoreURL))
                            {
                                domain = targetminisite.StoreURL;
                            }
                        }

                        _context.Response.Redirect(string.Format("{0}://{1}{2}"
                                                , value.storeX.getBooleanSetting("IsSecureEntireSite", false) ? "https" : "http"
                                                , domain
                                                , url
                                                ));
                    }
                }
                else if (CurrentCountry == null)
                {
                    _currentCountry = value;
                    _context.Session["seletedcountry"] = value.Shorts;
                }
                else if (value.StoreID == Store.storeID && CurrentCountry.CountryID != value.CountryID)
                {
                    _currentCountry = value;
                    _context.Session["seletedcountry"] = value.Shorts;
                    _context.Response.Redirect("~/");
                }
                else
                {
                    _currentCountry = value;
                    _context.Session["seletedcountry"] = value.Shorts;
                    
                }

                if (_currentCountry != null && _currentCountry.CountryCurrencies.Any())
                {
                    var cu = _currentCountry.CountryCurrencies.FirstOrDefault(c => c.Default.GetValueOrDefault());
                    if (cu != null && cu.Currency != null)
                        CurrentCurrency = cu.Currency;
                }
                else
                    CurrentCurrency = Store.profile.defaultCurrency;

            }
        }

        private POCOS.Language _currentLanguage;
        public POCOS.Language CurrentLanguage
        {
            get
            {
                if (_currentLanguage == null)
                {
                    HttpCookie storeLangeCookie = HttpContext.Current.Request.Cookies["StoreLanguageCode"];
                    if (_context.Session["seletedLanguage"] != null && this.Store.profile.StoreLanguages.FirstOrDefault(x => x.Language.Code.Equals((string)_context.Session["seletedLanguage"], StringComparison.OrdinalIgnoreCase)) != null)
                    {
                        _currentLanguage = this.Store.profile.StoreLanguages.FirstOrDefault(x => x.Language.Code.Equals((string)_context.Session["seletedLanguage"], StringComparison.OrdinalIgnoreCase)).Language;
                    }
                    else if (storeLangeCookie != null && !string.IsNullOrEmpty(storeLangeCookie.Value)
                                && this.Store.profile.StoreLanguages.FirstOrDefault(c => c.Language.Code.Equals(storeLangeCookie.Value, StringComparison.OrdinalIgnoreCase)) != null)
                        _currentLanguage = (new LanguageHelper()).getLangByCode(storeLangeCookie.Value);
                    else if (CurrentCountry != null && CurrentCountry.CountryLanguages.Any(x => x.IsActive.GetValueOrDefault() && x.IsDefault.GetValueOrDefault()))
                        _currentLanguage = CurrentCountry.CountryLanguages.First(x => x.IsActive.GetValueOrDefault() && x.IsDefault.GetValueOrDefault()).Language;
                    else if (this.Store.profile.StoreLanguages.Any(x => x.IsActive.GetValueOrDefault() && x.IsDefault.GetValueOrDefault()))
                        _currentLanguage = this.Store.profile.StoreLanguages.First(x => x.IsActive.GetValueOrDefault() && x.IsDefault.GetValueOrDefault()).Language;
                    else
                        _currentLanguage = this.Store.profile.defaultlanguage;
                }
                return _currentLanguage;
            }
            set
            {
                _currentLanguage = value;
                _context.Session["seletedLanguage"] = (value == null ? "" : value.Code);
                HttpCookie storeLangeCookie = HttpContext.Current.Request.Cookies["StoreLanguageCode"];
                if (storeLangeCookie == null)
                    storeLangeCookie = new HttpCookie("StoreLanguageCode");
                storeLangeCookie.Value = value == null ? "" : value.Code;
                storeLangeCookie.Expires = DateTime.Now.AddDays(15);
                HttpContext.Current.Response.AppendCookie(storeLangeCookie);
            }
        }


        private POCOS.Currency _currentCurrency;
        public POCOS.Currency CurrentCurrency
        {
            get
            {
                if(_currentCurrency == null)
                    _currentCurrency = Store.profile.defaultCurrency;
                return _currentCurrency;
            }
            set
            {
                _currentCurrency = value;
            }
        }

        public POCOS.Address CurrentAddress
        {
            get
            {
                return Store.getAddressByCountry(CurrentCountry, Current.BusinessGroup);
            }
        }

        //for ads
        public Dictionary<String, String> keywords
        {
            get
            {

                if (_keywords == null)
                    _keywords = new Dictionary<string, string>();
                return _keywords;
            }
        }
        /// <summary>
        /// 获取广告关键字
        /// </summary>
        public string keywordString
        {
            get
            {
                StringBuilder sbkeyword = new StringBuilder();
                foreach (var item in keywords)
                {
                    sbkeyword.AppendFormat("&{0}={1}", item.Key, System.Web.HttpUtility.UrlEncode(replaceSpecialCharacters(item.Value)));
                }
                return sbkeyword.ToString();
            }
        }

        /// <summary>
        /// For cms 
        /// 目前仅用在Product detail 页面和 Ctos detail 页面，
        /// 暂时只根据 product 的 sproductid 和 ctos 的display no 来获取 cms
        /// </summary>
        public string keyWordsStringWithBar
        {
            get
            {
                if (keywords != null && keywords.Count > 0)
                    return System.Web.HttpUtility.UrlEncode(replaceSpecialCharacters(string.Join(",", keywords.Values).Replace("，", ",").Replace("|", ",")));
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 去除一些特殊符号
        /// </summary>
        /// <param name="oldStr"></param>
        /// <returns></returns>
        public string replaceSpecialCharacters(string oldStr)
        {
            if (!string.IsNullOrEmpty(oldStr))
                return System.Text.RegularExpressions.Regex.Replace(oldStr, @"[\""®™&%$#*!@^~～！@#￥%……&×]|(<.*?>)", "");
            else
                return "";
        }
        private string _storeMembershippass;
        public string storeMembershippass
        {
            get {
                if (string.IsNullOrEmpty(_storeMembershippass))
                {
                    if (MiniSite == null)
                    {
                        _storeMembershippass = this.Store.profile.StoreMembershippass;
                    }
                    else
                    { 
                        _storeMembershippass =string.Format("{0}_{1}", this.Store.profile.StoreMembershippass,this.MiniSite.SiteName.ToLower()); 
                    }
                }
                return _storeMembershippass;
            }
        }
        public POCOS.Store.BusinessGroup BusinessGroup
        {
            get { return _BusinessGroup; }
            set { _BusinessGroup = value; }
        }
        /// <summary>
        /// Gets or sets the current Store
        /// </summary>
        public Store Store
        {
            get {   return this._store;         }
            set {    this._store = value;      }
        }

        private POCOS.MiniSite _miniSite;
        public POCOS.MiniSite MiniSite
        {
            get { return _miniSite; }
            set {
                _miniSite = value;
            }
    }

        /// <summary>
        /// This method is to retrieve user request IP address.  If solution is in testing mode, it will try to read static IP from file.  
        /// If there is no specific static testing IP, it return IP in user request
        /// </summary>
        /// <returns></returns>
        public String getUserIP()
        {
            string ip = _context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = string.IsNullOrEmpty(_context.Request.UserHostAddress) ? "" : _context.Request.UserHostAddress;
            else if (ip.Contains(","))
            {
                ip = ip.Split(',').First().Trim();
            }
            return ip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string getIPFromFile()
        {
            string ip = _context.Request.UserHostAddress;
            string filename = "c:\\ip.txt";
            //打开文件并显示其内容 
            //119.75.23.81
            //213.152.172.181

            System.IO.StreamReader reader = null;
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    reader = new System.IO.StreamReader(filename);
                    //for(string line=reader.ReadLine();line!=null;line=reader.ReadLine()) 
                    ip = reader.ReadLine();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                ip = getUserIP();
            }
            return ip;
        }

    
        public Boolean isTestMode()
        {
            if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["TestingMode"]))
                return false;
            else
                return System.Configuration.ConfigurationManager.AppSettings["TestingMode"] == "true";
        }
        public POCOS.User User
        {
            get
            {

                if (_eStoreUser != null)
                    return _eStoreUser;
                else if (!string.IsNullOrEmpty(_context.Request["tempid"]) && !string.IsNullOrEmpty(_context.Request["id"]))//check from sso
                {
                    if (!Presentation.eStoreUserAccount.TrySSOSignIn(getUserIP(), _context.Request["tempid"], _context.Request["id"]))
                    {
                        return null;
                    }
                    else
                    {
                        if (getBooleanSetting("EnableRewardSystem", false))
                        {
                            foreach (var r in Store.getRewardActivities(MiniSite).Where(c => c.NewRegisterPoint.GetValueOrDefault() > 0))
                            {
                                eStore.BusinessModules.Reward.RewardMgt helper = new eStore.BusinessModules.Reward.RewardMgt(r);
                                helper.grantNewRegisterPoint(_eStoreUser);
                            }
                        }
                    }
                }
                else if (string.IsNullOrEmpty(_context.User.Identity.Name))//check if not Identity
                {
                    return null;
                }
                else if (_context.Session["User"] == null)
                {
                    string cookieName = FormsAuthentication.FormsCookieName;
                    HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];

                    if (!Presentation.eStoreUserAccount.TrySignInFromCookies(authCookie, getUserIP()))
                    {
                        return null;
                    }
                }
                else
                {
                    _eStoreUser = (eStore.POCOS.User)_context.Session["User"];
                    if (_eStoreUser != null
                     && _eStoreUser.timeSpan.Hours == 0 && _eStoreUser.timeSpan.Seconds == 0
                     && !string.IsNullOrEmpty(_context.Request["timezoneOffset"]))
                    {
                        int timezoneOffset = 0;
                        int.TryParse(_context.Request["timezoneOffset"] ?? "0", out timezoneOffset);
                        if (timezoneOffset != 0)
                        {
                            TimeSpan ts = new TimeSpan(timezoneOffset / 60, timezoneOffset % 60, 0);
                            _eStoreUser.timeSpan = ts;
                        }
                    }
                }
                return this._eStoreUser;
            }
            set
            {
                _eStoreUser = value;
                _context.Session["User"] = value;
            }
        }

        private string getStoreIDFromConfiguration()
        {
            string storeID = null;
            try
            {
                storeID = System.Configuration.ConfigurationManager.AppSettings["DefaultStore"];

            }
            catch (Exception)
            {

                storeID = "AUS";
            }
            if (string.IsNullOrEmpty(storeID))
                storeID = "AUS";

            return storeID;
        }

        private string getStoreIDFromCookie()
        {
            string storeID = null;
            try
            {
                storeID = _context.Request.Cookies["CurrentStore"].Value;

            }
            catch (Exception)
            {

                storeID = null;
            }


            return storeID;
        }
        private string getStoreIDFromSession()
        {
            string storeID = null;
            try
            {
                storeID = _context.Session["CurrentStore"] != null ? _context.Session["CurrentStore"].ToString() : string.Empty;

            }
            catch (Exception)
            {

                storeID = null;
            }


            return storeID;
        }
        public static eStoreContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.Items["eStoreContext"] == null)
                {
                    eStoreContext _eStoreContext = new eStoreContext();                
                    HttpContext.Current.Items.Add("eStoreContext", _eStoreContext);
                    return _eStoreContext;
                }
                return (eStoreContext)HttpContext.Current.Items["eStoreContext"];
            }
        }
        
        public POCOS.Cart UserShoppingCart
        {
            get
            {
                if (_UserShoppingCart != null)
                {
                    _UserShoppingCart.fixCartCurrency(CurrentCurrency);
                    return _UserShoppingCart;
                }
                else if (_context.Session["UserShoppingCart"] == null)
                {
                    if (this.User == null)
                    {
                        _UserShoppingCart = null;
                    }
                    else
                    {

                        _UserShoppingCart = eStoreContext.Current.User.actingUser.shoppingCart;
                        BusinessGroup = _UserShoppingCart.businessGroup;
                        _UserShoppingCart.fixCartCurrency(CurrentCurrency);
                        _context.Session["UserShoppingCart"] = _UserShoppingCart;

                        //cart更新时间超过24小时, 查看里面产品的status和价格是否有变化
                        _UserShoppingCart.refresh();
                    }
                }
                else
                {
                    _UserShoppingCart = (POCOS.Cart)_context.Session["UserShoppingCart"];
                    BusinessGroup = _UserShoppingCart.businessGroup;
                    _UserShoppingCart.fixCartCurrency(CurrentCurrency);
                }

                return _UserShoppingCart;
            }
            set
            {
                _UserShoppingCart = value;
                _context.Session["UserShoppingCart"] = value;
            }
        }
        public POCOS.Quotation Quotation
        {
            get
            {
                if (_quotation != null)
                {
                    _quotation.cartX.fixCartCurrency(CurrentCurrency);
                    _quotation.fixCurrency();
                    return _quotation;
                }

                else if (_context.Session["Quotation"] == null)
                {
                    if (this.User == null)
                    {
                        _quotation = null;
                    }
                    else
                    {
                        _quotation = eStoreContext.Current.User.actingUser.openingQuote;
                        BusinessGroup = _quotation.cartX.businessGroup;
                        _quotation.ResellerID = this.User.ResellerID;
                        _quotation.cartX.fixCartCurrency(CurrentCurrency);
                        _quotation.fixCurrency();
                        _context.Session["Quotation"] = _quotation;

                        //cart更新时间超过24小时, 查看里面产品的status和价格是否有变化
                        _quotation.cartX.refresh();
                    }
                }
                else
                {
                    _quotation = (POCOS.Quotation)_context.Session["Quotation"];
                    BusinessGroup = _quotation.cartX.businessGroup;
                    _quotation.cartX.fixCartCurrency(CurrentCurrency);
                    _quotation.fixCurrency();
                }

                if (_quotation.cartX.SoldToContact == null)
                {
                    if (_quotation != null && User.actingUser.mainContact != null)
                        _quotation.cartX.setSoldTo(User.actingUser.mainContact);
                }
                return _quotation;
            }
            set
            {
                _quotation = value;
                _context.Session["Quotation"] = value;
            }
        }
        public POCOS.Order Order
        {
            get
            {
                if (_Order != null)
                    return _Order;

                else if (_context.Session["Order"] == null)
                {
                    _Order = null;
                }
                else
                {
                    _Order = (POCOS.Order)_context.Session["Order"];
                    BusinessGroup = _Order.cartX.businessGroup;
                }
                return _Order;
            }
            set
            {
                _Order = value;
                _context.Session["Order"] = _Order;
            }
        }
        private List<POCOS.StoreErrorCode> _StoreErrorCode;
        public List<POCOS.StoreErrorCode> StoreErrorCode
        {
            get
            {
                if (_StoreErrorCode == null)
                    _StoreErrorCode = new List<POCOS.StoreErrorCode>();
                return _StoreErrorCode;
            }

        }
        public void AddStoreErrorCode(string ErrorCode, object[] args = null, bool gotoErrorPage = false, int StatusCode=0)
        {
            if (gotoErrorPage)
            {
                if (StatusCode != 0)
                    _context.Response.StatusCode = StatusCode;
                if (args != null)
                    _context.Server.Transfer("~/ErrorPage.aspx?message=" + HttpUtility.UrlEncode(string.Format(ErrorCode, args)));
                else
                    _context.Server.Transfer("~/ErrorPage.aspx?message=" + HttpUtility.UrlEncode(ErrorCode));

            }
            POCOS.StoreErrorCode storeErrorCode = Presentation.eStoreContext.Current.Store.profile.StoreErrorCodes.FirstOrDefault(error => error.ErrorCode == ErrorCode);
            if (storeErrorCode != null)
                AddStoreErrorCode(storeErrorCode);
            else if (args != null)
            {
                POCOS.StoreErrorCode tmpErrorCode = new POCOS.StoreErrorCode();
                tmpErrorCode.StoreID = Store.storeID;
                tmpErrorCode.ErrorCode = ErrorCode;
                tmpErrorCode.UserActionMessage = string.Format(ErrorCode, args);
                tmpErrorCode.DefaultMessage = string.Format(ErrorCode, args);
                AddStoreErrorCode(tmpErrorCode);
            }
            else
            {
                POCOS.StoreErrorCode tmpErrorCode = new POCOS.StoreErrorCode();
                tmpErrorCode.StoreID = Store.storeID;
                tmpErrorCode.ErrorCode = ErrorCode;
                tmpErrorCode.UserActionMessage = ErrorCode;
                tmpErrorCode.DefaultMessage = ErrorCode;
                AddStoreErrorCode(tmpErrorCode);
                Utilities.eStoreLoger.Fatal(string.Format("not available error code {1} for {0}", Store.storeID, ErrorCode));
            }
        }
        private void AddStoreErrorCode(POCOS.StoreErrorCode _storeErrorCode)
        {
            if (!StoreErrorCode.Exists(se => se.ErrorCode == _storeErrorCode.ErrorCode))
            {
                StoreErrorCode.Add(_storeErrorCode);
            }

        }

        private ISearchConfiguration _searchConfiguration;
        public ISearchConfiguration SearchConfiguration
        {
            get
            {
                if (_searchConfiguration == null)
                    _searchConfiguration = new eStoreSearchConfiguration();
                return _searchConfiguration;
            }
            set { _searchConfiguration = value; }
        }

        private Dictionary<string, string> _settings;
        public Dictionary<string, string> Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = this.Store.getSettings(this.CurrentCountry);

                }
                return _settings;
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
            if (MiniSite != null && MiniSite.Settings.ContainsKey(key))
            {
                rlt = MiniSite.Settings[key];
            
            }
            else if (Settings.ContainsKey(key))
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
        /// This method is to retrieve store setting and return in String format
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getStringSetting(string key, string defaultValue = "")
        {
            string rlt = string.Empty;
            if (MiniSite != null && MiniSite.Settings.ContainsKey(key))
            {
                rlt = MiniSite.Settings[key];

            }
            else if (Settings.ContainsKey(key))
            {
                rlt = Settings[key];
            }
            else
            {
                //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
            }
            return rlt == string.Empty ? defaultValue : rlt;
        }

        /// <summary>
        /// This method is to retrieve store setting and return in Boolean format
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean getBooleanSetting(string key, Boolean defaultValue = false)
        {

            if (MiniSite != null && MiniSite.Settings.ContainsKey(key))
            {
                Boolean.TryParse( MiniSite.Settings[key], out defaultValue); 

            }
            else if (Settings.ContainsKey(key))
            {
                Boolean.TryParse(Settings[key], out defaultValue);
            }
            else
            {
                //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
            }
            return defaultValue;
        }

        public Boolean getBooleanSettingFromDB(string key, POCOS.Country country = null, Boolean defaultValue = false)
        {
            if (MiniSite != null && MiniSite.Settings.ContainsKey(key))
                Boolean.TryParse(MiniSite.Settings[key], out defaultValue);
            else
            {
                var setting = this.Store.getSettings(country ?? this.CurrentCountry);
                if (setting.ContainsKey(key))
                    Boolean.TryParse(setting[key], out defaultValue);
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
            int rlt = defaultValue;
            if (MiniSite != null && MiniSite.Settings.ContainsKey(key))
            {
                int.TryParse(MiniSite.Settings[key], out rlt);  

            }
            else if (Settings.ContainsKey(key))
            {
                int.TryParse(Settings[key], out rlt);
            }
            else
            {
                //eStore.Utilities.eStoreLoger.Info(string.Format("Settings {0} is missing in {1}", key, StoreID));
            }
            return rlt;
        }

        /// <summary>
        /// Gets or sets an object item in the context by the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key]
        {
            get
            {
                if (this._context == null)
                {
                    return null;
                }

                if (this._context.Items[key] != null)
                {
                    return this._context.Items[key];
                }
                return null;
            }
            set
            {
                if (this._context != null)
                {
                    this._context.Items.Remove(key);
                    this._context.Items.Add(key, value);

                }
            }
        }

        public DateTime getSafeTime(string dateStr)
        {
            DateTime dt = DateTime.Now;
            if (!string.IsNullOrEmpty(dateStr))
            {
                if (DateTime.TryParse(dateStr, out dt))
                    return dt;
                else
                    return DateTime.Now;
            }
            else
                return DateTime.Now;
        }

        public string verificationCode
        {
            get
            {
                string cacheKey = "CAPTCHA";
                Dictionary<string, string> CAPTCHADict =( Dictionary<string, string>) POCOS.CachePool.getInstance().getObject(cacheKey);
                if(CAPTCHADict!=null && CAPTCHADict.ContainsKey(_context.Session.SessionID))
                    return CAPTCHADict[_context.Session.SessionID];
                else
                    return DateTime.Now.ToString();
            }
            set
            {
                string cacheKey = "CAPTCHA";
                Dictionary<string, string> CAPTCHADict = (Dictionary<string, string>)POCOS.CachePool.getInstance().getObject(cacheKey);
                if (CAPTCHADict == null)
                {
                    CAPTCHADict = new Dictionary<string, string>();
                    POCOS.CachePool.getInstance().cacheObject(cacheKey, CAPTCHADict,POCOS.CachePool.CacheOption.Minute30);
                }
                CAPTCHADict[_context.Session.SessionID] = value;
            }
        }

        public System.Collections.Specialized.NameValueCollection SiteID
        {
            get 
            {
                if (_context.Session["UserSiteID"] == null)
                {
                    HttpCookie UserSiteIDCookie = HttpContext.Current.Request.Cookies["UserSiteID"];
                    if (UserSiteIDCookie != null)
                    {
                        _context.Session["UserSiteID"] = UserSiteIDCookie.Values;
                        return UserSiteIDCookie.Values;
                    }
                    else
                        return null;
                }
                else
                    return _context.Session["UserSiteID"] as System.Collections.Specialized.NameValueCollection;
            }
            set
            {
                HttpCookie UserSiteIDCookie = HttpContext.Current.Request.Cookies["UserSiteID"];
                if (UserSiteIDCookie == null)
                {
                    UserSiteIDCookie = new HttpCookie("UserSiteID");
                    UserSiteIDCookie.Values.Add("SiteID", value["SiteID"]);
                    UserSiteIDCookie.Values.Add("TrackingID", value["TrackingID"]);
                    UserSiteIDCookie.Expires = DateTime.Now.AddDays(15);
                    HttpContext.Current.Response.AppendCookie(UserSiteIDCookie);
                }
                _context.Session["UserSiteID"] = value;                
            }
        }

        public void AffiliateTracking(object data, string sessionID = "")
        {
            if (Presentation.eStoreContext.Current.SiteID != null)
            {
                POCOS.Affiliate aff = new POCOS.Affiliate() { SiteID = Presentation.eStoreContext.Current.SiteID["SiteID"] };
                if (aff.isExistInStore())
                {
                    string trackingurl = string.Empty;
                    if (data is POCOS.Order)
                    {
                        trackingurl = aff.TrackingOrder((POCOS.Order)data, Presentation.eStoreContext.Current.SiteID["TrackingID"]);
                    }
                    else if (data is POCOS.User)
                    {
                        trackingurl = aff.TrackingUser((POCOS.User)data, Presentation.eStoreContext.Current.SiteID["TrackingID"]);
                    }
                    else if (data is POCOS.Quotation)
                    {
                        trackingurl = aff.TrackingQuotation((POCOS.Quotation)data, Presentation.eStoreContext.Current.SiteID["TrackingID"]);
                    }

                    if (!string.IsNullOrEmpty(trackingurl))
                    {
                        Store.postTrackingData(trackingurl);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(sessionID))
            {
                POCOS.Affiliate aff = new POCOS.Affiliate() { SiteID = sessionID };
                if (aff.isExistInStore() && data is POCOS.Order)
                    aff.TrackingOrder((POCOS.Order)data, string.Empty);
            }
        }

        // estore head top chat 
        public string GetTopChat()
        {
            string chattype = getStringSetting("eStore_TopChat");
            try
            { // 多线程或许有问题                
                if (!Store.Chats.Keys.Contains(chattype))
                    Store.Chats.Add(chattype, Chats.ChatMgt.GetChat(chattype).GetStoreChats(Store.profile));
            }
            catch (Exception)
            { }
            
            return Store.Chats[chattype]; 
        }
                
        #endregion
    }
}
