using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS
{
    /// <summary>
    /// This class implements properties and methods for campaign and campaign strategy management
    /// </summary>
    public class CampaignManager
    {
        #region regular implementation
        //private Dictionary<String, StoreCampaignsEntity> _storeCampaigns = new Dictionary<string,StoreCampaignsEntity>();
        private static CampaignManager _manager = null;
        public enum PromotionCodeStatus { Valid, Expired, ExceedUserLimit, ExceedCampaignLimit, LessThanMinimumRequirement, NotApplicable, Invalid, Applied, IsBelowCost };

        //for singleton control
        private CampaignManager() { }

        public static CampaignManager getInstance()
        {
            if (_manager == null)
                _manager = new CampaignManager();
            return _manager;
        }

        /// <summary>
        /// This method return up-to-date campaign strategies of a particular store.  
        /// Campaign strategies by default will be hold for an hour unless
        /// there is notification from 3rd party to request for reload
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private IList<Campaign> getStoreCampaigns(String storeId, String couponCode)
        {
            IList<Campaign> campaigns = new List<Campaign>(); ;

            try
            {
                //move this cache to cache pool
                CachePool cachePool = CachePool.getInstance();
                StoreCampaignsEntity storeCampaignEntity = cachePool.getStoreCampaignEntity(storeId);

                //need to reload store campaigns
                if (storeCampaignEntity == null)
                {
                    DateTime now = DateTime.Now;
                    List<Campaign> storeCampaigns = new List<Campaign>();

                    //Reload campaignn setting from DB and 
                    //qualify campaigns to keep only validate campaigns and campaign strategies
                    DateTime currentTime = DateTime.Now;
                    CampaignHelper chelper = new CampaignHelper();

                    foreach (Campaign campaign in chelper.getCampaigns(storeId))
                    {
                        if (campaign.EffectiveDate <= currentTime && currentTime <= campaign.ExpiredDate)   //valid campaign
                            storeCampaigns.Add(campaign);
                    }

                    storeCampaignEntity = new StoreCampaignsEntity(storeId, storeCampaigns);
                    cachePool.cacheStoreCampaignEntity(storeCampaignEntity);
                }

                if (storeCampaignEntity != null)
                {
                        //如果在 缓存池中,  根据code获取不到Campaign, 去DB检查一次
                        campaigns = storeCampaignEntity.getCampaigns(couponCode);
                        if (campaigns.Count == 0 && !String.IsNullOrEmpty(couponCode))  //search coupon code 
                        {
                            //根据 couponCode查找 对应的System Campagin
                            Campaign newCampagin = new CampaignHelper().getCampaignsByCodes(storeId, couponCode);
                            if (newCampagin != null)
                            {
                                storeCampaignEntity.replaceCampagin(newCampagin);
                                campaigns.Add(newCampagin);
                            }
                        }
                }                
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at getStoreCampaigns", "", "", "", ex);
            }
            if (campaigns.Any()) //重写 PromotionType == "System" 的 promotioncode
            {
                foreach (var c in campaigns)
                {
                    if (c.PromotionType == "System")
                        c.PromotionCode = couponCode;
                }
            }

            return campaigns;
        }

        /// <summary>
        /// This method will assign PromotionPolicy to Product instance if there is any matched one
        /// and return flag indicating whether there is at least a match found or not
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public PromotionPolicy getMostApplicableProductPromotionPolicy(Product product)
        {
            List<CampaignStrategy> strategies = new List<CampaignStrategy>();
            String storeId = product.StoreID;
            try
            {
                foreach (Campaign campaign in getStoreCampaigns(storeId, product.autoAppliedCouponCode))
                {
                    if (campaign.PromotionCode.Length == 0 || campaign.PromotionCode.ToUpper() == product.autoAppliedCouponCode.ToUpper()) //no promotion code
                    {
                        //foreach (CampaignStrategy strategy in campaign.CampaignStrategies)
                        foreach (CampaignStrategy strategy in campaign.strategiesX)
                        {
                            if (isProductApplicableForPromotion(strategy, product, CampaignStrategy.CouponType.Product))
                            {
                                strategy.startDate = campaign.EffectiveDate;
                                strategy.endDate = campaign.ExpiredDate;
                                strategies.Add(strategy);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at getMostApplicablePromotion", "", "", "", ex);
            }

            //sort applicable campaign strategies on their priority and pick up the highest priority one as promotion item
            //If later eStore allow multiple promotions at a single product, the logic has to be modified here.
            if (strategies.Count > 0)
            {
                CampaignStrategy candidate = (from j in strategies
                                              orderby j.Priority
                                              select j).FirstOrDefault<CampaignStrategy>();
                return newPromotionPolicy(candidate);
            }
            else
                return null;
        }

        /// <summary>
        /// This method provides functionality for user to apply promotion code against a particular quotation. The caller
        /// will receive promotion code status as return.  If the promotion code is a valid one, the discount amount will be
        /// applied to quotation in the process.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="quotation"></param>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        public IList<CampaignStrategy> getApplicableCampaignStrategies(User user, Cart cart, String promotionCode)
        {
            CampaignManager.PromotionCodeStatus status = CampaignManager.PromotionCodeStatus.Invalid;
            String storeId = cart.StoreID;
            List<CampaignStrategy> strategies = new List<CampaignStrategy>();

            //collecting valid campaign strategy candidates
            List<Campaign> cartcampaigns = (List<Campaign>)getStoreCampaigns(storeId, promotionCode); //如果 PromotionType = 'System' 是否考虑重置promotionCode
            if (!string.IsNullOrEmpty(promotionCode))
            {
                cartcampaigns.AddRange((List<Campaign>)getStoreCampaigns(storeId, string.Empty));
            }
            foreach (Campaign campaign in cartcampaigns)
            {
                //foreach (CampaignStrategy strategy in campaign.CampaignStrategies)
                foreach (CampaignStrategy strategy in campaign.strategiesX)
                {
                    //restriction checkup
                    status = checkRestrictions(strategy, cart, user, cart.TotalAmount);
                    if (status == PromotionCodeStatus.Valid)
                        strategies.Add(strategy);
                }
            }

            //pick up the strategy having the highest priority

            return (from strategy in strategies orderby strategy.Priority.GetValueOrDefault() descending select strategy).ToList();
        }

        public bool addPromotionAppliedLogs(PromotionAppliedLog log, string couponCode)
        {
            try
            {
                Campaign campaign = (from Campaign c in getStoreCampaigns(log.StoreID, couponCode)
                                     where c.CampaignID == log.CampaignID
                                     select c).FirstOrDefault();
                if (campaign != null)
                {
                    CampaignStrategy strategy = (from s in campaign.CampaignStrategies
                                                 where s.ID == log.CampaignStrategyID
                                                 select s
                                                             ).FirstOrDefault();


                    if (strategy != null)
                        strategy.promotionAppliedLogs.Add(log);
                }

            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at addPromotionAppliedLogs", "", "", "", ex);

            }

            return true;

        }

        /// <summary>
        /// This method provides functionality for user to apply promotion code against a particular order. The caller
        /// will receive promotion code status as return.  If the promotion code is a valid one, the discount amount will be
        /// applied to order in the process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        public PromotionCodeStatus applyPromotionCode(Order order, String promotionCode)
        {
            CampaignManager.PromotionCodeStatus status = CampaignManager.PromotionCodeStatus.Valid;

            return status;
        }

        /// <summary>
        /// Freight promotion only tigth with purchasingAmount
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="purchasingAmount"></param>
        /// <returns></returns>
        public PromotionPolicy getApplicableFreightPromotionPolicy(CampaignStrategy strategy, Decimal purchasingAmount)
        {
            PromotionPolicy promotion = null;
            if (purchasingAmount >= strategy.MinAmount.GetValueOrDefault())
                promotion = newPromotionPolicy(strategy);

            return promotion;
        }


        /// <summary>
        /// Freight promotion only tigth with purchasingAmount
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="purchasingAmount"></param>
        /// <returns></returns>
        public PromotionPolicy getApplicableProductPromotionPolicy(CampaignStrategy strategy, Part part, CampaignStrategy.CouponType coupontype = CampaignStrategy.CouponType.Product)
        {
            PromotionPolicy promotion = null;
            if (isProductApplicableForPromotion(strategy, part, coupontype))
                promotion = newPromotionPolicy(strategy);

            return promotion;
        }

        public PromotionPolicy getApplicableProductPromotionPolicy(CampaignStrategy strategy, CartItem cartitem, CampaignStrategy.CouponType coupontype = CampaignStrategy.CouponType.Product)
        {
            PromotionPolicy promotion = null;
            if ( isProductApplicableForPromotion(strategy,  cartitem.partX, coupontype))
                if (string.IsNullOrEmpty(strategy.XRule))
                    promotion = newPromotionPolicy(strategy);
                else
                {
                    if (cartitem.btosX != null && matchXRole(strategy, cartitem.btosX.parts))
                        promotion = newPromotionPolicy(strategy);
                    //else if (cartitem.bundlex)
                    //{ }
                }
            return promotion;
        }

        public PromotionPolicy getApplicableCartPromotionPolicy(CampaignStrategy strategy, Cart cart)
        {
            PromotionPolicy promotion = null;
            if (isCartApplicableForPromotion(strategy, cart))
                promotion = newPromotionPolicy(strategy);

            return promotion;
        }

        /// <summary>
        /// This method is to valid whether the input product is applicable to the specified campaign strategy
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private Boolean isProductApplicableForPromotion(CampaignStrategy strategy, Part part, CampaignStrategy.CouponType coupontype)
        {
            Boolean applicable = false;
            if (strategy.promotionType == coupontype) //only promotion at product is applicable here.
            {
                //check business group scope
                if (part is Product)
                {
                    Product product = (Product)part;
                    if (matchBusinessGroup(strategy.BusinessGroup, product.businessGroup.ToString()) &&
                    matchProductGroup(strategy.ProductType, product) &&
                    matchProductLevelAndCriteria(strategy, product.name))
                    {
                        applicable = true;
                    }
                }
                else if (part is PStoreProduct)
                {
                    PStoreProduct product = (PStoreProduct)part;
                    if (coupontype == CampaignStrategy.CouponType.FreeGroundShipping)
                    {
                        if (product.FreeShipping.GetValueOrDefault())
                        {
                            applicable = true;
                        }
                    }
                    else
                    {
                        if (matchProductGroup(strategy.ProductType, product) &&
                         matchProductLevelAndCriteria(strategy, product.name))
                        {
                            applicable = true;
                        }
                    }
                }
               
            }

            return applicable;
        }


        private Boolean isCartApplicableForPromotion(CampaignStrategy strategy, Cart cart)
        {
            Boolean applicable = false;
            if (strategy.promotionType == CampaignStrategy.CouponType.Cart) //only promotion at Cart is applicable here.
            {
                decimal minAmountThreshold = strategy.MinAmount.GetValueOrDefault();
                int minQtyThreshold = strategy.MinQTY.GetValueOrDefault();

                //int maxQtyLimited = strategy.MaxQtyPerUser.GetValueOrDefault();
                if (minAmountThreshold > 0 || minQtyThreshold > 0)    //has minimum qualification
                {
                    decimal minAmount = 0;
                    int minQty = 0;
                    foreach (CartItem item in cart.CartItems)
                    {
                        if (item.partX is Product)
                        {
                            if (isProductApplicableForPromotion(strategy, item.partX, CampaignStrategy.CouponType.Cart))
                            {
                                minAmount += item.AdjustedPrice;
                                minQty += item.Qty;
                            }
                        }
                    }

                    //check to see if there is minimum amount qualification
                    if (minAmountThreshold > 0 && minAmount >= minAmountThreshold)
                        applicable = true;
                    //If it's not qualified to minAmount checkup check if there is minimum order quality qualification
                    if (minQtyThreshold > 0 && minQty >= minQtyThreshold)
                        applicable = true;
                }
                else
                    applicable = true;
            }

            return applicable;
        }

        /// <summary>
        /// This method is to cover CampaignStrategy to PromotionPolicy
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        private PromotionPolicy newPromotionPolicy(CampaignStrategy strategy)
        {
            PromotionPolicy promotion = new PromotionPolicy(strategy.CampaignID, strategy.ID);
            promotion.discountPrice = strategy.DiscountValue;
            if (strategy.DiscountValueType == "Rate")
                promotion.discountValueType = PromotionPolicy.DiscountValueType.Rate;
            else if (strategy.DiscountValueType == "FixValue")
                promotion.discountValueType = PromotionPolicy.DiscountValueType.FixValue;
            else if (strategy.DiscountValue < 1m)
                promotion.discountValueType = PromotionPolicy.DiscountValueType.Rate;
            else
                promotion.discountValueType = PromotionPolicy.DiscountValueType.FixValue;

            promotion.PromotionCode = strategy.PromotionCode;
            promotion.promoteMessage = strategy.PromoteDesc;
            promotion.endDate = strategy.endDate;
            promotion.startDate = strategy.startDate;
            return promotion;
        }


        /// <summary>
        /// This is the match product business group with targeting business group in campaign strategy
        /// </summary>
        /// <param name="businessGroup"></param>
        /// <param name="productBusinessGroup"></param>
        /// <returns></returns>
        private Boolean matchBusinessGroup(String businessGroup, String productBGroup)
        {
            //check business group scope
            businessGroup = businessGroup.ToUpper();
            productBGroup = productBGroup.ToUpper();
            try
            {
                if (businessGroup.Equals("BOTH") || businessGroup.Equals("ALL") || businessGroup.IndexOf(productBGroup) != -1)   //not business group requirement
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Boolean matchXRole(CampaignStrategy strategy, Dictionary<POCOS.Part, int> parts)
        {
            Boolean match = false;
            if (string.IsNullOrEmpty(strategy.XRule))
                match = true;
            else
            {
                string[] xrole = strategy.XRule.Split('|');
                int matchCount = (from r in xrole
                                  from p in parts
                                  where p.Key.SProductID.StartsWith(r, true, null)
                                  select p
                                    ).Count();
                match = matchCount > 0;

            }
            return match;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private Boolean matchProductGroup(String targetGroup, Product product)
        {
            targetGroup = targetGroup.ToUpper();
            Boolean match = false;

            switch (targetGroup)
            {
                case "ALL":
                case "BOTH":
                    match = true;
                    break;
                case "CTOS":
                    if (product.productType == Product.PRODUCTTYPE.CTOS)
                        match = true;
                    break;
                case "COMPONENT":
                    if (product.productType == Product.PRODUCTTYPE.STANDARD)
                        match = true;
                    break;
            }

            return match;
        }
        private Boolean matchProductGroup(String targetGroup, PStoreProduct product)
        {
            targetGroup = targetGroup.ToUpper();
            Boolean match = false;

            switch (targetGroup)
            {
                case "EPAPS":
                        match = true;
                    break;
            }

            return match;
        }
        /// <summary>
        /// In this method, only "ALL" and "CATEGORY" are handled.  "STARTWITH" will be implented later when how
        /// Category ID works is clear.  ********
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private Boolean matchProductLevelAndCriteria(CampaignStrategy strategy, String sProductId)
        {
            //Execute exception checkup
            if (!String.IsNullOrEmpty(strategy.ExceptionPNList))
            {
                String[] partNo = strategy.ExceptionPNList.Split('|');
                if (partNo.Contains(sProductId))    //in exceptional list
                    return false;   //product is in the promotion exceptional list
            }

            MATCHING_METHOD matchMethod = MATCHING_METHOD.EXACT;
            if (!String.IsNullOrWhiteSpace(strategy.MatchCriteria))
            {
                switch (strategy.MatchCriteria.ToUpper())
                {
                    case "LIST":    //exact match
                        matchMethod = MATCHING_METHOD.EXACT;
                        break;
                    case "STARTWITH":
                        matchMethod = MATCHING_METHOD.STARTWITH_TBC;
                        break;
                    case "NONE":
                        matchMethod = MATCHING_METHOD.NONE;
                        break;
                }
            }

            if (matchMethod == MATCHING_METHOD.NONE)
                return true;

            Boolean match = false;

            try
            {
                //match product level
                String level = strategy.ItemType.ToUpper();
                List<String> categoryIds = null;

                switch (level)
                {
                    case "ALL":
                        //open to all product level, thus there is validation needed here.
                        match = true;
                        break;
                    case "CATEGORY":
                        match = strategy.applicableProductIDS.Contains(sProductId);
                        break;
                    case "PRODUCT":
                    case "STARTWITH":
                        if (matchMethod == MATCHING_METHOD.STARTWITH_TBC)
                            matchMethod = MATCHING_METHOD.TARGET_STARTWITH;

                        categoryIds = String.IsNullOrWhiteSpace(strategy.CategoryIDList) ? new List<String>() : strategy.CategoryIDList.Split('|').ToList();
                        //    categoryIds = strategy.CategoryIDList.Split('|');

                        match = matchNamingCriteria(categoryIds, matchMethod, sProductId);
                        /*
                        if (categoryIds.Length > 0)
                        {
                            match = categoryIds.FirstOrDefault(ids => sProductId.StartsWith(ids)) != null;
                        }
                        else
                        {
                            match = false;
                        }
                         * */

                        break;
                    default:
                        //no other option needing to be handled here yet.
                        break;
                }
            }
            catch (Exception) { match = false; }

            return match;
        }


        private enum MATCHING_METHOD
        {
            EXACT, STARTWITH_TBC, NONE /* this value is an intermedium value and need to be further classfied*/,
            CRITERIA_STARTWITH, TARGET_STARTWITH
        };
        private Boolean matchNamingCriteria(List<String> criteria, MATCHING_METHOD method, String target)
        {
            if (criteria == null || criteria.Count == 0)
                return true;

            Boolean match = false;
            int matchCount = 0;
            switch (method)
            {
                case MATCHING_METHOD.EXACT:
                    match = criteria.Contains(target);
                    break;

                case MATCHING_METHOD.CRITERIA_STARTWITH:
                    matchCount = (from source in criteria
                                  where source.StartsWith(target)
                                  select source).Count();
                    match = (matchCount > 0) ? true : false;
                    break;

                case MATCHING_METHOD.TARGET_STARTWITH:
                    matchCount = (from source in criteria
                                  where target.StartsWith(source)
                                  select source).Count();
                    match = (matchCount > 0) ? true : false;
                    break;
            }

            return match;
        }

        /// <summary>
        /// This method is to qualify a particular campaign strategy with User and purchasing items. There are several restriction criteria may
        /// involved.
        /// 1. Promotion code is only applicable for a group of specified users
        /// 2. Promotion code has minimum purchasing amount
        /// 3. Promotion code has maximum usage time limitation per promotion period
        /// 4. Promotion code has maximum usage time limitation per user
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="user"></param>
        /// <param name="purchasingAmount"></param>
        /// <returns></returns>
        private PromotionCodeStatus checkRestrictions(CampaignStrategy strategy, Cart cart, User user, Decimal purchasingAmount)
        {

            //check if campaign is opening for current user
            if (!String.IsNullOrEmpty(strategy.CustomerType) && !strategy.CustomerType.ToUpper().Equals("ALL_CUSTOMERS"))  //only apply to particular users
            {
                if (strategy.UserIDList == "inviter")
                {
                    UserHelper helper = new UserHelper();
                    if (helper.getInvitees(user.actingUser.UserID).Any() == false)
                        return PromotionCodeStatus.NotApplicable;
                }
                else if (strategy.UserIDList == "invitee")
                {
                    UserHelper helper = new UserHelper();
                    if (!helper.isInvitee(user.actingUser.UserID))
                        return PromotionCodeStatus.NotApplicable;
                }
                else if (!String.IsNullOrEmpty(strategy.UserIDList))
                {
                    String[] userIdList = strategy.UserIDList.Split('|');
                    int count = (from userId in userIdList
                                 where userId.Equals(user.UserID, StringComparison.OrdinalIgnoreCase)
                                 select userId).Count();
                    if (count == 0)
                    {
                        foreach (var id in userIdList) //模糊匹配用户是否符合。
                        {
                            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(id);
                            if (reg.IsMatch(user.UserID))
                                return PromotionCodeStatus.Valid;
                        }
                        return PromotionCodeStatus.NotApplicable;    //user is not in the applicable list
                    }
                }
                else if (strategy.xrule != null && strategy.xrule is eStore.POCOS.PocoX.XRule4UserRestriction)
                {
                    if (strategy.xrule.isApplicable(user) == false)
                    {
                        return PromotionCodeStatus.NotApplicable;
                    }
                }
            }

            //check minimum purchasing amount
            if (strategy.MinAmount > 0 && purchasingAmount < strategy.MinAmount)
                return PromotionCodeStatus.LessThanMinimumRequirement;

            //check maximum usage limitation
            if (strategy.MaxTotalAppliedTimes > 0 && strategy.getTotalAppliedTimes() >= strategy.MaxTotalAppliedTimes)  //has usage limitation
                return PromotionCodeStatus.ExceedCampaignLimit;

            if (strategy.MaxApplicableTimesPerUser > 0 && strategy.getTotalAppliedTimes(user.UserID) >= strategy.MaxApplicableTimesPerUser)  //has usage limitation by user
                return PromotionCodeStatus.ExceedCampaignLimit;

            if (strategy.MaxQtyPerUser.GetValueOrDefault() > 0)
            {

                if (strategy.getTotalAppliedQty(user.UserID) >= strategy.MaxQtyPerUser.GetValueOrDefault())
                {
                    return PromotionCodeStatus.ExceedUserLimit;
                }
            }
            if (strategy.ItemType.ToUpper() == "CATEGORY")
            {
                foreach (var item in cart.cartItemsX)
                {
                    //var ismath = matchProductLevelAndCriteria(strategy, item.partX.SProductID); //如果产品有一个落在promotion的category中即判定为可用
                    var ismath = isProductApplicableForPromotion(strategy, item.partX, strategy.promotionType);
                    if (ismath)
                        return PromotionCodeStatus.Valid;
                }
                return PromotionCodeStatus.Invalid;
            }
            else if (strategy.ItemType.ToUpper() == "PRODUCT")
            {
                foreach (var item in cart.cartItemsX)
                {
                    if (item.partX is POCOS.Product)
                    {
                        var ismath = matchProductGroup(strategy.ProductType, (Product)item.partX);
                        if (ismath)
                            return PromotionCodeStatus.Valid;
                    }
                   
                }
                return PromotionCodeStatus.Invalid;
            }
            return PromotionCodeStatus.Valid;
        }

        #endregion //regular implementation

        /// <summary>
        /// This inner class is for caching store campaign setting
        /// </summary>
        public class StoreCampaignsEntity
        {
            DateTime _lastUpdate = DateTime.MinValue;

            List<Campaign> _campaignsWithCouponCode = null;
            List<Campaign> _campaignsNoCouponCode = null;

            public StoreCampaignsEntity(String storeID, List<Campaign> campaigns)
            {
                this.storeID = storeID;
                _campaignsWithCouponCode = new List<Campaign>();
                _campaignsNoCouponCode = new List<Campaign>();

                //Reload campaignn setting from DB and 
                //qualify campaigns to keep only validate campaigns and campaign strategies
                DateTime currentTime = DateTime.Now;
                foreach (Campaign campaign in campaigns)
                {
                    if (campaign.PromotionCode.Length == 0) //no promotion code
                        _campaignsNoCouponCode.Add(campaign);
                    else
                    {
                        campaign.PromotionCode = campaign.PromotionCode.ToUpper();
                        _campaignsWithCouponCode.Add(campaign);
                    }
                }

                _lastUpdate = DateTime.Now;
            }

            public String storeID
            {
                get;
                set;
            }

            /// <summary>
            /// The cache entry will be only good for one hour
            /// </summary>
            /// <returns></returns>
            public Boolean isExpired()
            {
                DateTime now = DateTime.Now;
                if (DateTime.Now.Subtract(_lastUpdate).TotalHours > 1)    //last update is one hour ago or before
                    return true;
                else
                    return false;
            }

            public IList<Campaign> getCampaigns(String couponCode)
            {
                if (String.IsNullOrEmpty(couponCode))
                    return _campaignsNoCouponCode;
                else
                {
                    List<Campaign> campaigns = new List<Campaign>();
                    DateTime currentTime = DateTime.Now; 
                    foreach (Campaign campaign in _campaignsWithCouponCode)
                    {

                        if (campaign.PromotionCode.Equals(couponCode.ToUpper()) || campaign.campaignCodesX.Any(x => x.PromotionCode.Equals(couponCode, StringComparison.OrdinalIgnoreCase) && x.EffectiveDate <= currentTime && currentTime <= x.ExpiredDate)) campaigns.Add(campaign);
                    }

                    return campaigns;
                }
            }

            //如果Campaign的CampaignCodes不一样,替换旧的 Campaign
            public void replaceCampagin(Campaign newCampaign)
            {
                Campaign oldCampaign = _campaignsWithCouponCode.FirstOrDefault(p => p.CampaignID == newCampaign.CampaignID);
                if (oldCampaign != null)
                {
                    if (oldCampaign.campaignCodesX.Count != newCampaign.campaignCodesX.Count)
                    {
                        _campaignsWithCouponCode.Remove(oldCampaign);
                        _campaignsWithCouponCode.Add(newCampaign);
                    }
                }
            }
        }

        #region Unit test
        //public void addDummyCampaign(
        #endregion

    }

}
