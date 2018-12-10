using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace eStore.POCOS
{
    public class CachePool
    {
        private static CachePool _self = new CachePool();
        private static MemoryCache _cacheContainer = new MemoryCache("eStore Cache Pool");
        private static TimeSpan _24_hours = new TimeSpan(24, 0, 0); //24 hours
        private static TimeSpan _12_hours = new TimeSpan(12, 0, 0); //6 hours
        private static TimeSpan _6_hours = new TimeSpan(6, 0, 0); //6 hours
        private static TimeSpan _2_hours = new TimeSpan(2, 0, 0); //2 hours
        private static TimeSpan _1_hours = new TimeSpan(1, 0, 0); //1 hours
        private static TimeSpan _30_minutes = new TimeSpan(0, 30, 0);    //30 minutes
        private static TimeSpan _10_minutes = new TimeSpan(0, 10, 0);   //10 minutes
        private static TimeSpan _5_minutes = new TimeSpan(0, 5, 0);     // 5 minutes
        private static TimeSpan _1_minute = new TimeSpan(0, 1, 0);    // 1 minute

        //to prevent third party instantiation
        private CachePool() {}

        public enum CacheOption { Hour24, Hour12, Hour6, Hour2, Hour1, Minute30, Minute10, Minute5, Minute1};

        public static CachePool getInstance()  { return _self; }

        /// <summary>
        /// The index naming conversion is storeID.Part.SProductID.  The default cache time is 2 hours
        /// </summary>
        /// <param name="part"></param>
        public void cachePart(Part part, CacheOption option = CacheOption.Hour1) 
        {
            if (part != null)
            {
                String key = part.StoreID + ".Part." + part.SProductID.ToUpper();
                cacheObject(key, part, option);
            }
        }

        /// <summary>
        /// This method return matched Part.  null value will returns if there is no match.
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public Part getPart(String storeID, String productID)
        {
            String key = storeID + ".Part." + productID.ToUpper();
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Part)value;
        }

        /// <summary>
        /// The index naming conversion is storeID.ProductCategory.categoryID & storeID.ProductCategory.categoryPath.  
        /// The default cache time is 2 hours
        /// </summary>
        /// <param name="part"></param>
        public void cacheProductCategory(ProductCategory productCategory, CacheOption option = CacheOption.Hour1)
        {
            if (productCategory != null)
            {
                String key1 = productCategory.Storeid + ".ProductCategory." + productCategory.CategoryPath.ToUpper();
                String key2 = productCategory.Storeid + ".ProductCategory." + productCategory.CategoryID;
                cacheObject(key1, productCategory, option);
                cacheObject(key2, productCategory, option);
            }
        }

        /// <summary>
        /// This method return matched product category.  null value will returns if there is no match.  
        /// The match key can be either categoryId or categoryPath
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public ProductCategory getProductCategory(String storeID, String categoryId)
        {
            String key = storeID + ".ProductCategory." + categoryId.ToUpper();
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (ProductCategory)value;
        }

        //缓存 Intel 所有的category
        public void cacheIntelProductCategory(List<ProductCategory> pcList,String storeID, CacheOption option = CacheOption.Hour24)
        {
            string key = storeID + ".IntelProductCategory";
            cacheObject(key, pcList, option);
        }

        public List<ProductCategory> getIntelProductCategory(String storeID)
        {
            string key = storeID + ".IntelProductCategory";
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (List<ProductCategory>)value;
        }

        /// <summary>
        /// The index naming conversion is storeID.Store.StoreID & storeID.Store.StoreURL.  
        /// The default cache time is 24 hours
        /// </summary>
        /// <param name="part"></param>
        public void cacheStore(Store store, CacheOption option = CacheOption.Hour1)
        {
            if (store != null)
            {
                String key1 = store.StoreID + ".Store." + store.StoreID.ToUpper();
                String key2 = store.StoreURL + ".Store." + store.StoreURL.ToUpper();
                cacheObject(key1, store, option);
                cacheObject(key2, store, option);
                if (store.MiniSites!=null && store.MiniSites.Any(x=>!string.IsNullOrEmpty(x.StoreURL)))
                {
                    foreach (MiniSite ms in store.MiniSites.Where(x => !string.IsNullOrEmpty(x.StoreURL)))
                    {
                        String mskey = ms.StoreURL + ".Store." + ms.StoreURL.ToUpper();
                        cacheObject(mskey, store, option);
                    }
                
                }
            }
        }

        /// <summary>
        /// This method return matched store.  null value will returns if there is no match.  
        /// The match key can be either storeId or storeURL
        /// </summary>
        /// <param name="storeID">StoreID can be either ID or URL</param>
        /// <returns></returns>
        public Store getStore(String storeID)
        {
            String key = storeID + ".Store." + storeID;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Store)value;
        }

        /// <summary>
        /// The index naming conversion is storeID.StoreCampaignEntity
        /// The default cache time is 2 hours
        /// </summary>
        /// <param name="campaign"></param>
        public void cacheStoreCampaignEntity(CampaignManager.StoreCampaignsEntity storeCampaignEntity, CacheOption option = CacheOption.Hour1)
        {
            if (storeCampaignEntity != null)
            {
                String key = storeCampaignEntity.storeID + ".StoreCampaignEntity";
                cacheObject(key, storeCampaignEntity, option);
            }
        }

        /// <summary>
        /// This method return matched campaign.  null value will returns if there is no match.  
        /// The match key can be storeID + promotionCode
        /// </summary>
        /// <param name="storeID">StoreID can be either ID or URL</param>
        /// <returns></returns>
        public CampaignManager.StoreCampaignsEntity getStoreCampaignEntity(String storeID)
        {
            String key = storeID + ".StoreCampaignEntity";
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (CampaignManager.StoreCampaignsEntity)value;
        }


        /// <summary>
        /// The index naming conversion is storeID.Cart.CartID
        /// The default cache time is 2 hours
        /// </summary>
        /// <param name="cart"></param>
        public void cacheCart(Cart cart, CacheOption option = CacheOption.Hour1)
        {
            if (cart != null)
            {
                String key = cart.StoreID + ".Cart." + cart.CartID;
                cacheObject(key, cart, option);
            }
        }

        /// <summary>
        /// This method return matched shopping cart.  null value will returns if there is no match.  
        /// The match key is storeID.Cart.cartId
        /// </summary>
        /// <param name="storeID">StoreID can be either ID or URL</param>
        /// <returns></returns>
        public Cart getCart(String storeID, String cartID)
        {
            String key = storeID + ".Cart." + cartID;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Cart)value;
        }


        /// <summary>
        /// The index naming conversion is storeID.SAPCompany.CompanyID
        /// The default cache time is 10 minutes
        /// </summary>
        /// <param name="cart"></param>
        public void cacheSAPCompany(String storeId, VSAPCompany sapCompany, CacheOption option = CacheOption.Minute10)
        {
            if (sapCompany != null)
            {
                String key = storeId + ".SAPCompany." + sapCompany.CompanyID;
                cacheObject(key, sapCompany, option);
            }
        }

        /// <summary>
        /// This method return matched SAPCompany.  null value will be returned if there is no match.  
        /// The match key is storeID.SAPCompany.CompanyID
        /// </summary>
        /// <returns></returns>
        public VSAPCompany getSAPCompany(String storeID, String companyID)
        {
            String key = storeID + ".SAPCompany." + companyID;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (VSAPCompany)value;
        }


        /// <summary>
        /// The index naming conversion is storeID.Quotation.QuotationNo
        /// The default cache time is 2 hours
        /// </summary>
        /// <param name="cart"></param>
        public void cacheQuotation(Quotation quote, CacheOption option = CacheOption.Hour1)
        {
            if (quote != null)
            {
                String key = quote.StoreID + ".Quotation." + quote.QuotationNumber;
                cacheObject(key, quote, option);
            }
        }

        /// <summary>
        /// This method return matched quotation.  null value will returns if there is no match.  
        /// The match key is storeID.Quotation.quotationNumber
        /// </summary>
        /// <param name="storeID">StoreID can be either ID or URL</param>
        /// <returns></returns>
        public Quotation getQuotation(String storeID, String quotationNo)
        {
            String key = storeID + ".Quotation." + quotationNo;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Quotation)value;
        }

        /// <summary>
        /// The index naming conversion is StoreSolution.Order.OrderNo
        /// The default cache time is 30 minutes
        /// </summary>
        /// <param name="cart"></param>
        public void cacheOrder(Order order, CacheOption option = CacheOption.Minute30)
        {
            if (order != null)
            {
                String key = "StoreSolution.Order." + order.OrderNo;
                cacheObject(key, order, option);
            }
        }

        /// <summary>
        /// This method return matched quotation.  null value will returns if there is no match.  
        /// The match key is StoreSolution.Order.OrderNo
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public Order getOrder(String orderNo)
        {
            String key = "StoreSolution.Order." + orderNo;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Order)value;
        }

        public List<String> getStoreCacheList(String storeId)
        {
            var matches = from item in _cacheContainer
                          where item.Key.IndexOf(storeId) == 0 || item.Key.IndexOf("Solution.") ==0
                          orderby item.Key
                          select item.Key;
            if (matches != null)
                return matches.ToList();
            else
                return new List<String>();
        }
        public List<String> getCacheList()
        {
            var matches = from item in _cacheContainer
                          //where item.Key.IndexOf(storeId) == 0 || item.Key.IndexOf("Solution.") == 0
                          orderby item.Key
                          select item.Key;
            if (matches != null)
                return matches.ToList();
            else
                return new List<String>();
        }
        public void releaseCacheItem(String key)
        {            
            _cacheContainer.Remove(key);
        }

        public void releaseCacheItems(List<String> items)
        {
            foreach (String item in items)
                _cacheContainer.Remove(item);
        }

        public void releaseStoreCacheItems(String storeID)
        {
            List<String> cacheItems = getStoreCacheList(storeID);
            releaseCacheItems(cacheItems);
        }

        public void releaseStoreCacheProducts(String storeID)
        {
            String keyPrefix = storeID + ".Product.";
            releaseCacheItemsInPattern(keyPrefix);
        }
        public void releaseStoreCacheProduct(String storeID, String SProductID)
        {
            String key1 = storeID + ".Part." + SProductID;
            String key2 = storeID + ".Product." + SProductID;
            String key3 = storeID + ".Model." + SProductID;
            releaseCacheItem(key1);
            releaseCacheItem(key2);
            releaseCacheItem(key3);
        }
        public void releaseStoreCacheStore(String ServerStoreID, string storeID, String storeURL)
        {
            String key1 = ServerStoreID + ".Store." + storeID;
            String key2 = ServerStoreID + ".Store." + storeURL;
            releaseCacheItem(key1);
            releaseCacheItem(key2);
        }
        public void releaseStoreCacheProductCategories(String storeID)
        {
            String keyPrefix = storeID + ".ProductCategory.";
            releaseCacheItemsInPattern(keyPrefix);
        }
        public void releaseStoreCacheProductCategory(String storeID, String CategoryPath, string CategoryID=null)
        {
            String key1 = storeID + ".ProductCategory." +  CategoryPath.ToUpper();
            releaseCacheItem(key1);
            if (string.IsNullOrEmpty(CategoryID))
            {
                String key2 = storeID + ".ProductCategory." + CategoryID;
                releaseCacheItem(key2);
            }
        }
        public void releaseStoreCacheCampaign(String storeID)
        {
            String keyPrefix = storeID + ".Campaign.";
            releaseCacheItemsInPattern(keyPrefix);
        }

        public void releaseStoreCacheCart(String storeID)
        {
            String keyPrefix = storeID + ".Cart.";
            releaseCacheItemsInPattern(keyPrefix);
        }

        public void releaseStoreCacheQuotation(String storeID)
        {
            String keyPrefix = storeID + ".Quotation.";
            releaseCacheItemsInPattern(keyPrefix);
        }

        public void releaseCacheItemsInPattern(String pattern)
        {
            List<String> cacheItems = getStoreCacheList(pattern);
            releaseCacheItems(cacheItems);
        }

        public void cacheObject(String key, Object obj, CacheOption option = CacheOption.Hour1)
        {
            try
            {
                if (obj != null)
                {
                    DateTime expiredTime = DateTime.Now.Add(getTimeSpan(option));
                    DateTimeOffset timeOffset = new DateTimeOffset(expiredTime);
                    if (_cacheContainer.Contains(key))
                        _cacheContainer.Remove(key);
                    _cacheContainer.Add(key, obj, timeOffset);
                }
            }
            catch (Exception)
            {
            }
        }

        public Object getObject(String key)
        {
            return _cacheContainer.Get(key);
        }

        public void cacheIPNation(String IP, String nation, CacheOption option = CacheOption.Hour1)
        {
            if (! String.IsNullOrEmpty(IP))
            {
                String key = "StoreSolution.IPToNation." + IP;
                cacheObject(key, nation, option);
            }
        }

        public String getIPNation(String IP)
        {
            String key = "StoreSolution.IPToNation." + IP;
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (String)value;
        }

        public void reset()
        {
            foreach (KeyValuePair<String, Object> item in _cacheContainer.ToList())
                _cacheContainer.Remove(item.Key);
            //_cacheContainer.Dispose();
        }
       
        /// <summary>
        /// This method is to get corresponding timespan
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private TimeSpan getTimeSpan(CacheOption option)
        {
            TimeSpan timeSpan = _24_hours;  //default
            switch (option)
            {
                case CacheOption.Hour2:
                    timeSpan = _2_hours;
                    break;
                case CacheOption.Hour1:
                    timeSpan = _1_hours;
                    break;
                case CacheOption.Minute30:
                    timeSpan = _30_minutes;
                    break;
                case CacheOption.Minute10:
                    timeSpan = _10_minutes;
                    break;
                case CacheOption.Minute5:
                    timeSpan = _5_minutes;
                    break;
                case CacheOption.Minute1:
                    timeSpan = _1_minute;
                    break;
                case CacheOption.Hour12:
                    timeSpan = _12_hours;
                    break;
                case CacheOption.Hour6:
                    timeSpan = _6_hours;
                    break;
                default:
                    timeSpan = _24_hours;
                    break;
            }

            return timeSpan;
        }

        /// <summary>
        /// This method return WWWCMS help
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        public eStore.POCOS.DAL.WWWCMSHelper getWWWCMS(String storeID)
        {
            string key = storeID + ".CMS";
            object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (eStore.POCOS.DAL.WWWCMSHelper)value;
        }

        public void cacheWWWCMS(eStore.POCOS.DAL.WWWCMSHelper cms, string storeID)
        {
            if (cms != null)
            {
                string key = storeID + ".CMS";
                cacheObject(key, cms, CacheOption.Hour24);
            }
        }

        public Dictionary<string, List<POCOS.Product>> getAppendixSpecCategory(int appendixID)
        {
            string key = "AppendixSpecCategory." + appendixID.ToString();
            object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Dictionary<string, List<POCOS.Product>>)value;
        }

        public void cacheAppendixSpecCategory(Dictionary<string, List<POCOS.Product>> dictionary, int appendixID)
        {
            if (dictionary != null)
            {
                string key = "AppendixSpecCategory." + appendixID.ToString();
                cacheObject(key, dictionary, CacheOption.Hour2);
            }
        }

        public eStore.POCOS.Spec_Category getSpecCategory(int categoryID)
        {
            string key = "SpecCategoryRoot." + categoryID.ToString();
            object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (eStore.POCOS.Spec_Category)value;
        }
        public void cacheSpecCategory(eStore.POCOS.Spec_Category sc, int categoryID)
        {
            if (sc != null)
            {
                string key = "SpecCategoryRoot." + categoryID.ToString();
                cacheObject(key, sc, CacheOption.Hour2);
            }
        }
        public List<eStore.POCOS.Part_Spec_V3> getPartSpecV3List()
        {
            string key = "PartSpecV3";
            object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (List<eStore.POCOS.Part_Spec_V3>)value;
        }
        public void cachePartSpecV3(List<eStore.POCOS.Part_Spec_V3> v3)
        {
            if (v3 != null)
            {
                string key = "PartSpecV3";
                cacheObject(key, v3, CacheOption.Hour2);
            }
        }

        /// <summary>
        /// cache Advertisement for 24 hour
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="option"></param>
        public void cacheAdvertisement(Advertisement adv, CacheOption option = CacheOption.Hour24)
        {
            if (adv != null)
            {
                String key = adv.StoreID + ".Advertisement." + adv.id.ToString().ToUpper();
                cacheObject(key, adv, option);
            }
        }

        public Advertisement getAdvertisement(String storeID, int advid)
        {
            String key = storeID + ".Advertisement." + advid.ToString().ToUpper();
            Object value = _cacheContainer.Get(key);
            if (value == null)
                return null;
            else
                return (Advertisement)value;
        }
    }
}
