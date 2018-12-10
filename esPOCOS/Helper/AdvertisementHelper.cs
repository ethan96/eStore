using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.POCOS.DAL
{

    public partial class AdvertisementHelper : Helper
    {
        #region Business Read

        /// <summary>
        /// Return all Ads assigned to the specified product and the categories which this product belongs to
        /// </summary>
        /// <param name="_prod"></param>
        /// <returns></returns>
        public List<Advertisement> getAds(Product product, Advertisement.AdvertisementType adType = Advertisement.AdvertisementType.NotSpecified) 
        {
            List<Advertisement> prodAds = new List<Advertisement>();

            if (product != null)
            {
                prodAds = (from mad in context.MenuAdvertisements
                                 from advs in context.Advertisements
                                 where mad.storeid == product.StoreID 
                                 && mad.AdID == advs.id 
                                 && advs.Publish == true
                                 && advs.StoreID == mad.storeid 
                                 && mad.TreeID.ToUpper().Equals(product.SProductID)
                                 && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                                 select advs).ToList();
                //Perform further filtering if targeting adType is specified
                if (prodAds != null && adType != Advertisement.AdvertisementType.NotSpecified)
                {
                    prodAds = (from ad in prodAds
                                where ad.segmentType == adType
                                select ad).ToList();
                }
            }

            return prodAds;
        }
        /// <summary>
        /// Return all Ads assigned to the specified product and the categories which this product belongs to
        /// </summary>
        /// <param name="_prod"></param>
        /// <returns></returns>
        public List<Advertisement> getAds(Product product, List<Advertisement.AdvertisementType> adtypes)
        {
            List<Advertisement> prodAds = new List<Advertisement>();

            if (product != null)
            {
                prodAds = (from mad in context.MenuAdvertisements
                           from advs in context.Advertisements
                           where mad.storeid == product.StoreID
                           && mad.AdID == advs.id
                           && advs.Publish == true
                           && advs.StoreID == mad.storeid
                           && mad.TreeID.ToUpper().Equals(product.SProductID)
                           && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                           select advs).ToList();
                //Perform further filtering if targeting adType is specified
                if (prodAds != null && adtypes != null && adtypes.Count > 0)
                {
                    prodAds = (from ad in prodAds
                               where adtypes.Contains(ad.segmentType)
                               select ad).ToList();
                }
            }

            return prodAds;
        }
        /// <summary>
        /// This method is to search Ads per keyword.  The implementation is not qualified and QA yet.  Suggest qualifying 
        /// this method before use it.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="storeid"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public List<Advertisement> getAds(string keyword, string storeid, bool publish = true)
        {
            if (string.IsNullOrEmpty(keyword) == true && string.IsNullOrEmpty(storeid) == true) return null;

            List<string> keylist = new List<string>();
            //Regex reg = new Regex(@"(?i)\b(?!['-])[a-z'-]+(?<!['-])\b");

            keylist = keyword.Split(',').Where(x=>!string.IsNullOrWhiteSpace (x)).ToList();

            //reg.Replace(keyword, delegate(Match m) { if (!keylist.Contains(m.Value)) keylist.Add(m.Value); return ""; });

            var _defaultadvs = (from mad in context.MenuAdvertisements
                                from advs in context.Advertisements
                                from li in keylist
                                where mad.storeid == storeid && mad.AdID == advs.id && mad.TreeID.Contains(li) && advs.Publish == publish
                                && advs.StoreID == mad.storeid && mad.type == "KeyWord"
                                    && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                                select advs).Distinct().ToList();

            return _defaultadvs.OrderByDescending(ad => ad.weight).ToList();
        }

        /// <summary>
        /// This method is to retrieve Ads associating to the specified product category
        /// </summary>
        /// <param name="_pc"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public List<Advertisement> getAds(ProductCategory _pc, Advertisement.AdvertisementType adType = Advertisement.AdvertisementType.NotSpecified)
        {
            List<Advertisement> categoryAds = new List<Advertisement>();
            if (_pc==null) 
                return categoryAds;

            try
            {
                categoryAds = (from mad in context.MenuAdvertisements
                                    from advs in context.Advertisements
                                    where mad.storeid == _pc.Storeid  
                                    && mad.AdID == advs.id 
                                    && mad.TreeID == _pc.CategoryPath 
                                    && advs.Publish==true
                                   && advs.StoreID == mad.storeid
                                    && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now                                    
                                    select advs).ToList();

                //Perform further filtering if targeting adType is specified
                if (adType != Advertisement.AdvertisementType.NotSpecified)
                {
                    categoryAds = (from ad in categoryAds
                                where ad.segmentType == adType
                                select ad).ToList();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }

            return categoryAds;
        }
        public List<Advertisement> getAds(ProductCategory _pc, List<Advertisement.AdvertisementType> adtypes)
        {
            List<Advertisement> categoryAds = new List<Advertisement>();
            if (_pc == null)
                return categoryAds;

            try
            {
                categoryAds = (from mad in context.MenuAdvertisements
                               from advs in context.Advertisements
                               where mad.storeid == _pc.Storeid
                               && mad.AdID == advs.id
                               && mad.TreeID == _pc.CategoryPath
                               && advs.Publish == true
                              && advs.StoreID == mad.storeid
                               && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                               select advs).ToList();

                //Perform further filtering if targeting adType is specified
                if (adtypes != null && adtypes.Count>0)
                {
                    categoryAds = (from ad in categoryAds
                                   where adtypes.Contains(ad.segmentType)
                                   select ad).ToList();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }

            return categoryAds;
        }
        /// <summary>
        /// This method is to retrieve Ads specified as default Ads
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Advertisement> getDefaultAdv(string storeid, Advertisement.AdvertisementType adType = Advertisement.AdvertisementType.NotSpecified)
        {
            List<Advertisement> defaultAds = new List<Advertisement>();
            if (String.IsNullOrEmpty(storeid)) 
                return defaultAds;

            try
            {
                defaultAds = (from mad in context.MenuAdvertisements
                                    from advs in context.Advertisements
                                    where mad.storeid == storeid 
                                    && mad.AdID == advs.id 
                                    && mad.type.ToUpper() == "DEFAULT" 
                                    && advs.Publish==true
                                    && advs.StoreID == mad.storeid
                                    && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                                    orderby advs.AdType, advs.Seq ascending
                                    select advs).ToList();

                //Perform further filtering if targeting adType is specified
                if (adType != Advertisement.AdvertisementType.NotSpecified)
                {
                    defaultAds = (from ad in defaultAds
                                   where ad.segmentType == adType
                                   select ad).ToList();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Advertisement>();
            }
            return defaultAds;
        }

        /// <summary>
        /// This method will return Advertisement per store and advertisement type. 
        /// Available Advertisement types are defined in Advertisement.AdvertisementType
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="adType"></param>
        /// <returns></returns>
        public List<Advertisement> getAdsByAdType(String storeID, Advertisement.AdvertisementType adType,string location="")
        {
            String searchAdType = adType.ToString().ToUpper();
            var ads = (from mad in context.MenuAdvertisements
                                from advs in context.Advertisements
                                where mad.storeid == storeID 
                                && mad.AdID == advs.id 
                                && advs.AdType.ToUpper() == searchAdType 
                                && advs.Publish == true
                                && advs.StoreID == mad.storeid
                                &&(string.IsNullOrEmpty(location)||(!string.IsNullOrEmpty(location)&& location.ToUpper()==mad.type.ToUpper()))
                                && advs.StartDate <= DateTime.Now && advs.EndDate >= DateTime.Now
                                select advs).ToList();

            Dictionary<int, Advertisement> adList = new Dictionary<int, Advertisement>();
            mergeAdList(adList, ads);

            return adList.Values.OrderByDescending(ad => ad.weight).ToList();
        }

        /// <summary>
        /// This method is to merge new Advertisement items to existing list.  
        /// The return from this method will contain unique advertisement item list.
        /// </summary>
        /// <param name="adList"></param>
        /// <param name="newList"></param>
        private void mergeAdList(Dictionary<int, Advertisement> adList, List<Advertisement> newList, int weightedPoint = 1)
        {
            if (adList != null && newList != null)
            {
                foreach (Advertisement ad in newList)
                {
                    if (!adList.ContainsKey(ad.id))
                        adList.Add(ad.id, ad);
                }
            }
        }


        /// <summary>
        /// This function will return all Advertisements targeting to home page including TodayHighLight, AdamForum and others
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        public List<Advertisement> getHomeAdv(string storeid, bool publish = true)
        {
            if (String.IsNullOrEmpty(storeid)) return null;

            try
            {
                var _defaultadvs = (from mad in context.MenuAdvertisements
                                    from advs in context.Advertisements
                                    where mad.storeid == storeid 
                                    && mad.AdID == advs.id 
                                    && advs.Publish==publish
                                     && mad.type.ToUpper() == "HOMEPAGE"
                                    && advs.StoreID == mad.storeid
                                    && advs.StartDate<=DateTime.Now && advs.EndDate>=DateTime.Now
                                    orderby advs.AdType, advs.Seq ascending
                                    select advs).ToList();

                if (_defaultadvs != null)
                {
                    foreach (var c in _defaultadvs)
                        c.helper = this;
                    return _defaultadvs;
                }
                else
                    return getDefaultAdv(storeid);                
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return getDefaultAdv(storeid);
            }
        }

        /// <summary>
        /// Get the Advertisement by key, advid
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="advid"></param>
        /// <returns></returns>
        public  Advertisement  getAdvByID(string storeid,int advid)
        {
            if (String.IsNullOrEmpty(storeid)) return null;

            try
            {

                var _adv = (from mad in context.Advertisements
                                    where mad.StoreID  == storeid && mad.id  == advid
                            select mad).FirstOrDefault();

                if (_adv != null)
                    _adv.helper = this;

                return _adv;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// For OM used, return publish/unpublish advs.
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="advid"></param>
        /// <returns></returns>

        public List<Advertisement> getAdvByStore(Store store)
        {
            if (store == null) return null;

            try
            {

                var _adv = (from mad in context.Advertisements
                            where mad.StoreID == store.StoreID
                            orderby mad.UpdateDate descending 
                            select mad);

                if (_adv != null)
                {
                    foreach (Advertisement ad in _adv.ToList())
                        ad.helper = this;

                    return _adv.ToList();
                }
                else
                    return new List<Advertisement>();
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        //根据adv type 获取所有发布的adv
        public List<Advertisement> getAdvByStoreAndType(Store store, Advertisement.AdvertisementType adType = Advertisement.AdvertisementType.NotSpecified)
        {
            if (store == null) return null;
            try
            {
                String searchAdType = adType.ToString().ToUpper();
                var _adv = (from mad in context.Advertisements
                            where mad.StoreID == store.StoreID && mad.Publish == true
                            && mad.AdType.ToUpper() == searchAdType
                            orderby mad.UpdateDate descending
                            select mad).ToList();

                if (_adv != null)
                {
                    foreach (Advertisement ad in _adv)
                        ad.helper = this;
                    return _adv;
                }
                else
                    return new List<Advertisement>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据 menuAdvertisement 获取广告
        /// </summary>
        /// <param name="treeids"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Advertisement> getAdvByMenuTreeIds(List<string> treeids, string storeid)
        {
            var _advs = (from adv in context.Advertisements
                        from madvs in context.MenuAdvertisements
                        where adv.id == madvs.AdID && adv.StoreID == storeid
                        && adv.Publish && adv.StartDate <= DateTime.Now && adv.EndDate >= DateTime.Now && treeids.Contains(madvs.TreeID)
                        select adv).Distinct().ToList();
            return _advs;
        }

        /// <summary>
        /// Get category related advertisements
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="type"></param>
        /// <param name="treeID"></param>
        /// <returns></returns>
        public List<Advertisement> getRelatedAdvertisement(string storeID, string type, string treeID)
        {
            try
            {
                var results = from adv in context.Advertisements
                              join menu in context.MenuAdvertisements on adv.id equals menu.AdID
                              where adv.StoreID == storeID &&
                              menu.type == type &&
                              menu.TreeID == treeID
                              orderby adv.Publish descending, adv.AdType, adv.EndDate descending
                              select adv;
                return results.ToList();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", storeID, ex);
            }
            return null;
        }
        #endregion

        #region Creat Update Delete


        public int save(Advertisement  _adv)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_adv == null || _adv.validate() == false) return 1;
            //Try to retrieve object from DB             

            Advertisement _exist_adv =  getAdvByID(_adv.StoreID ,_adv.id);
            try
            {

                if (_exist_adv == null)  //object not exist 
                {
                    context.Advertisements .AddObject(_adv); //state=added.                            
                    context.SaveChanges();                  
                    return 0;
                }
                else
                {
                   //Update
                    context = _adv.helper.context;  
                    context.Advertisements.ApplyCurrentValues(_adv);               
                    context.SaveChanges();                
                    return 0;
                }

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                
                return -5000;
            }

        }


       
        public int delete(Advertisement  _adv)
        {

            if (_adv == null || _adv.validate() == false) return 1;

            try
            {
                context.DeleteObject(_adv);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(CartHelper).ToString();
        }
        #endregion
    }
}