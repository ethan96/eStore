using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    /// <summary>
    /// This class is a singleton class which provides vendor instance management functions. 
    /// Developers should use this management utility for vendor instance management
    /// </summary>
    public class StoreManager
    {
        private static StoreManager _manager = new StoreManager();
        //private Hashtable _stores = new Hashtable();
        private CachePool _cachedStores = CachePool.getInstance();

        /// <summary>
        /// No default constructor. This will prevent developers to directly instantiate this class
        /// </summary>
        private StoreManager()
        {
        }

        public static StoreManager getInstance()
        {
            return _manager;
        }

        /// <summary>
        /// To retrieve store by store ID
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>
        ///     Store instance if found
        ///     null if not exist
        /// </returns>
        public Store getStoreByID(String storeId, StoreHelper helper)
        {
            Store store = null;

            lock (_cachedStores)  //to prevent simultanous access
            {
                store = getStore(storeId);
                if (store == null)  //not cached yet
                {
                    eStore.POCOS.Store profile = null;

                    //implement dummy store here
                    if (storeId.Equals("DummyStore"))
                    {
                        profile = new eStore.POCOS.Store();
                        profile.StoreID = storeId;
                        profile.StoreName = "Dummy store";
                    }
                    else
                    {
                        //retrieve POCO store instance first, then use it for eStore store construction
                        profile = helper.getStorebyStoreid(storeId);
                    }

                    if (profile != null)    //find store definition in eStore DB
                    {
                        //instantiate eStore store instance
                        store = createStoreInstance(profile);
                    }
                    else
                    {
                        //log warning message here, the only instance this can happen is for OM to register new store        
                    }
                }
            }

            return store;
        }

        public Store getStoreByHostname(String hostname, StoreHelper helper)
        {
            Store store = null;

            lock (_cachedStores)
            {
                if (hostname.ToUpper().Equals("LOCALHOST"))
                    return getStoreByID("AUS", helper);

                store = getStore(hostname);
                if (store == null)
                {
                    //retrieve POCO store instance first, then use it for eStore store construction
                    eStore.POCOS.Store profile = helper.getStorebyHostname(hostname);
                    if (profile != null)    //find store definition in eStore DB
                    {
                        //instantiate eStore store instance
                        store = createStoreInstance(profile);
                    }
                    else
                    {
                        //log warning message here, the only instance this can happen is for OM to register new store        
                        if (hostname.ToUpper().Equals("LOCALHOST"))
                        {
                            profile = helper.getStorebyStoreid("AUS");
                            if (profile != null)    //find store definition in eStore DB
                            {
                                //instantiate eStore store instance
                                store = createStoreInstance(profile);
                            }
                        }
                    }
                }
            }

            return store;
        }

        public Store getStoreByprofile(eStore.POCOS.Store profile)
        {
            return createStoreInstance(profile);
        }

        /// <summary>
        /// This storeID can be either true storeID or storeURL.  It returns null if there is nothing found
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private Store getStore(String storeId)
        {
            String key = "Solution.eStore." + storeId.ToUpper();
            Object store = _cachedStores.getObject(key);
            if (store != null)
                return (Store)store;
            else
                return null;
        }

        private Store createStoreInstance(eStore.POCOS.Store profile)
        {
            //instantiate eStore store instance
            Store store = new Store(profile);

            lock (_cachedStores)
            {
                var headerMenu = profile.getStringSetting("eStore_HeaderMenu");
                store.HeaderMenu = string.IsNullOrEmpty(headerMenu) ? "HeaderMenu" : headerMenu;

                //cache store instance for better performance
                _cachedStores.cacheObject("Solution.eStore." + profile.StoreID.ToUpper(), store);
                _cachedStores.cacheObject("Solution.eStore." + profile.StoreURL.ToUpper(), store);
                if (profile.MiniSites != null && profile.MiniSites.Any(x => !string.IsNullOrEmpty(x.StoreURL)))
                {
                    foreach (MiniSite ms in profile.MiniSites.Where(x => !string.IsNullOrEmpty(x.StoreURL)))
                    {
                        String mskey = "Solution.eStore." + ms.StoreURL.ToUpper();
                        _cachedStores.cacheObject(mskey, store);
                    }

                }
            }

            //set store rounding unit
            String roundingUnit = profile.Settings.ContainsKey("roundingUnit") ? profile.Settings["roundingUnit"] : "";
            if (!String.IsNullOrEmpty(roundingUnit))
                Converter.setRoundingUnit(profile.StoreID, roundingUnit);

            //set store CartPriceRoundingUnit
            String CartPriceRoundingUnit = profile.Settings.ContainsKey("CartPriceRoundingUnit") ? profile.Settings["CartPriceRoundingUnit"] : "";
            if (!String.IsNullOrEmpty(CartPriceRoundingUnit))
                Converter.setCartPriceRoundingUnit(profile.StoreID, CartPriceRoundingUnit);

            return store;
        }
    }
}
