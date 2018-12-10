using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Web;
using esUtilities;

namespace eStore.BusinessModules
{
    public class StoreSolution
    {
        //attributes
        private static StoreSolution _solution = new StoreSolution();
        private StoreManager _storeManager = StoreManager.getInstance();
        private List<Country> _countries = null;
        
        //make StoreSolution to be singleton and provid access method to get instance
        private StoreSolution() { }

        //methods
        public static StoreSolution getInstance()
        {
            return _solution;
        }

        /// <summary>
        /// The input name can be either StoreID likes AUS, AEU or others, or hostnames like localhost, buy.advantech.com and so. 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public Store getStore(String storeId)
        {
            storeId = storeId.ToUpper();
            StoreHelper storeHelper = new StoreHelper();
            Store store = null;
            if (storeId.Equals("LOCALHOST") || storeId.IndexOf('.') >= 0)    //hostname
                store = _storeManager.getStoreByHostname(storeId, storeHelper);
            else
                store = _storeManager.getStoreByID(storeId, storeHelper);

            return store;
        }
        public List<POCOS.Store> stores
        {
            get
            {
                StoreHelper helper = new StoreHelper();
                List<POCOS.Store> storeProfiles = helper.getStores();
                return storeProfiles;
            }
        }
        /// <summary>
        /// This property return country list coverred by current StoreSolution instance.
        /// </summary>
        public List<Country> countries
        {
            get
            {
                if (_countries == null) //need initialization
                {
                    List<Country> countries = new List<Country>();
                    try
                    {
                        StoreHelper helper = new StoreHelper();
                        List<POCOS.Store> storeProfiles = helper.getStores();

                        foreach (eStore.POCOS.Store store in storeProfiles)
                        {
                            foreach (Country cntry in store.Countries)
                            {
                                if (!countries.Contains(cntry))
                                    countries.Add(cntry);
                            }
                        }

                        _countries = (from cntry in countries
                                      orderby cntry.CountryName
                                      select cntry).ToList();
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Fatal("Exception at retrieving solution country list", "", "", "", ex);
                    }
                }

                return _countries;
            }
        }

        /// <summary>
        /// The method is to locate proper store for the user from the input IP.  If targetStore is not null, it will use
        /// targetStore as default store and would only find alternative store if the IP is from the blocked territory of
        /// the target store.  If targetStore parameter is null, it finds the default store for the input IP.
        /// </summary>
        /// <param name="targetStore"></param>
        /// <param name="sourceIP"></param>
        /// <returns></returns>
        public Store locateStore(Store targetStore, String sourceIP)
        {
            String countryCode = null;
            if (sourceIP.Contains(".") || sourceIP.Equals("localhost", StringComparison.CurrentCultureIgnoreCase))  //make sure it's IP
                countryCode = getCountryCodeByIp(sourceIP);
            else
                countryCode = sourceIP;

            Store candidateStore = null;
            if (!countryCode.Equals("XX"))   //non-internal IP
                candidateStore = locateStore(countryCode);

            if (targetStore != null)
            {
                //IP from european countries shall only be served by AEU store
               // if (candidateStore != null && (candidateStore.storeID.Equals("AEU") || candidateStore.storeID.Equals("ACN") || candidateStore.storeID.Equals("ATH")))   //EU ， CN ， TH store will return local store
                if (candidateStore != null && candidateStore.profile.getBooleanSetting("OpenMarket", true) == false)   //EU ， CN ， TH store will return local store
                    targetStore = candidateStore;
            }
            else
                targetStore = candidateStore;

            return targetStore;
        }

        /// <summary>
        /// This method is to locate store based on input country.  The input country can be in full name or its abbreviation.
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="returnNullForNoMatch"></param>
        /// <returns></returns>
        public Store locateStore(String countryName, Boolean returnNullForNoMatch=true)
        {
            Store candidateStore = null;
            try
            {
                Country country =getCountrybyCodeOrName(countryName);            
                if (country != null)
                    candidateStore = getStore(country.StoreID);
            }
            catch (Exception)
            {
            }

            if (returnNullForNoMatch==false && candidateStore == null) //if no store matches, use US for default.
                candidateStore = getStore("AUS");

            return candidateStore;
        }

        //get countryCode by ip
        public String getCountryCodeByIp(string sourceIP)
        {
            CachePool cache = CachePool.getInstance();
            String countryCode = cache.getIPNation(sourceIP);

            //find country code, if it's not available in cache
            if (String.IsNullOrEmpty(countryCode))
            {
                countryCode = CommonHelper.IPtoNation(sourceIP);
                if (!String.IsNullOrEmpty(countryCode))
                    cache.cacheIPNation(sourceIP, countryCode); //cache for 1 hour
            }
            return countryCode;
        }

        //get country by country name or short code
        public Country  getCountrybyCodeOrName(string countryName)
        {
            Country country = null;
            try
            {
                  country = (from item in countries
                             where item.Shorts.Equals(countryName, StringComparison.OrdinalIgnoreCase) || item.CountryName.Equals(countryName, StringComparison.OrdinalIgnoreCase)
                                   select item).FirstOrDefault();
               
            }
            catch (Exception)
            {
            }
            return country;
        }
        public Country getCountrybyCodeOrName(string storeid, string countryName)
        {
            Country country = null;
            try
            {
                country = (from item in countries
                           where item.Shorts.Equals(countryName, StringComparison.OrdinalIgnoreCase) || item.CountryName.Equals(countryName, StringComparison.OrdinalIgnoreCase)
                           && item.StoreID.Equals(storeid, StringComparison.OrdinalIgnoreCase)
                           select item).FirstOrDefault();

            }
            catch (Exception)
            {
            }
            return country;
        }
        List<String> _robotAgents = null;
        /// <summary>
        /// This method returns list of known robot search agents
        /// </summary>
        /// <returns></returns>
        public List<String> getRobotSearchAgents()
        {
            if (_robotAgents == null)
            {
                _robotAgents = System.Configuration.ConfigurationManager.AppSettings["SearchEngineHTTP_USER_AGENT"].ToString().ToLower().Split('|').ToList(); ;
            }

            return _robotAgents;
        }

        private List<String> _searchEngineIPs = null;
        /// <summary>
        /// This method return list of known search engine IPs
        /// </summary>
        /// <returns></returns>
        public List<String> getSearchEngineIPList()
        {
            if (_searchEngineIPs == null)
            {
                try
                {
                    _searchEngineIPs = System.Configuration.ConfigurationManager.AppSettings["SearchEngineIPList"].ToString().Split('|').ToList();
                }
                catch (Exception)
                {
                    _searchEngineIPs = new List<String>();
                }
            }

            return _searchEngineIPs;
        }

        /// <summary>
        /// This method is to match input IP with known search engine IPs
        /// </summary>
        /// <param name="agentIP"></param>
        /// <returns></returns>
        public Boolean isIPFromKnowSearchEngine(String agentIP)
        {
            List<String> knownIPList = getSearchEngineIPList();
            var matched = knownIPList.FirstOrDefault(s => agentIP.StartsWith(s));
            if (matched == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// This method is to determine if the input search agent identity matches to any known robot agent
        /// </summary>
        /// <param name="searchAgent"></param>
        /// <returns></returns>
        public Boolean isRobotAgent(String searchAgent)
        {
            List<String> robotAgents = getRobotSearchAgents();
            var matched = robotAgents.FirstOrDefault(s => searchAgent.Contains(s.ToLower()));
            if (matched == null)
                return false;
            else
                return true;
        }
    }
}
