using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    /// <summary>
    /// This class implements properties and methods for campaign and campaign strategy management
    /// </summary>
    class CampaignManager
    {
#region regular implementation
        private String _storeID;
        private List<Campaign> _campaigns = new List<Campaign>();
        private DateTime _lastUpdate = DateTime.MinValue;

        //for singleton control
        private CampaignManager() {}

        public CampaignManager(String storeID)
        {
            //at the initial time, build up strategy list
            _storeID = storeID;
        }

        /// <summary>
        /// This method will assign PromotionPolicy to Product instance if there is any matched one
        /// and return flag indicating whether there is at least a match found or not
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Boolean getMostApplicablePromotionPolicy(Product product)
        {
            List<CampaignStrategy> strategies = new List<CampaignStrategy>();
            foreach (Campaign campaign in campaigns)
            {
                if (campaign.PromotionCode.Length == 0) //no promotion code
                {
                    foreach (CampaignStrategy strategy in campaign.CampaignStrategies)
                    {
                        if (strategy.DiscountType.ToUpper().Equals("PRODUCTS")) //only promotion at product is applicable here.
                        {
                            //check business group scope
                            if (matchBusinessGroup(strategy.BusinessGroup, product.ProductGroup) &&
                                 matchProductGroup(strategy.ProductType, product) &&
                                 matchProductLevel(strategy, product))
                            {
                                strategy.startDate = campaign.EffectiveDate;
                                strategy.endDate = campaign.ExpiredDate;
                                strategies.Add(strategy);
                            }
                        }
                    }
                }
            }

            //sort applicable campaign strategies on their priority and pick up the highest priority one as promotion item
            //If later eStore allow multiple promotions at a single product, the logic has to be modified here.
            if (strategies.Count > 0)
            {
                CampaignStrategy candidate = (from j in strategies
                                              orderby j.Priority
                                              select j).FirstOrDefault<CampaignStrategy>();
                PromotionPolicy promotion = new PromotionPolicy(candidate.PromotionCode, candidate.ID);
                promotion.discountPrice = candidate.DiscountValue;
                //product promotion can be only in rate **** when we intruduce rate and fixvalue fields, this part
                //needs to be modified.
                promotion.discountValueType = PromotionPolicy.DiscountValueType.Rate;
                promotion.endDate = candidate.endDate;
                promotion.startDate = candidate.startDate;
                product.promotionPolicy = promotion;

                return true;
            }
            else
                return false;
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
            if (businessGroup.Equals("ALL") || businessGroup.Equals("EA,EP"))   //not business group requirement
                return true;
            else
            {
                productBGroup = productBGroup.ToUpper();
                if (! productBGroup.Equals(businessGroup))
                    return false;
            }

            return true;
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

        /// <summary>
        /// In this method, only "ALL" and "CATEGORY" are handled.  "STARTWITH" will be implented later when how
        /// Category ID works is clear.  ********
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private Boolean matchProductLevel(CampaignStrategy strategy, Product product)
        {
            //Execute exception checkup
            String[] partNo = strategy.ExceptionPNList.Split('|');
            if (partNo.Contains(product.SProductID))    //in exceptional list
                return false;   //product is in the promotion exceptional list

            Boolean match = false;
            //match product level
            String level = strategy.ItemType.ToUpper();
            switch (level)
            {
                case "ALL":
                    //open to all product level, thus there is validation needed here.
                    match = true;
                    break;
                case "CATEGORY":
                    //try to match product category in the category id list
                    //category id list is a "|" seperated string
                    if (strategy.ItemType.ToUpper().Equals("STARTWITH"))    //matching rule
                    {
                        //not implemented yet
                    }
                    else  //category list match
                    {
                        String[] categoryIds = strategy.CategoryIDList.Split('|');
                        foreach (String categoryId in categoryIds)
                        {
                            if (product.belongProductCategory(categoryId))
                            {
                                match = true;
                                break;
                            }
                        }
                    }

                    break;
                default:
                    //no other option needing to be handled here yet.
                    break;
            }

            return match;
        }

        /// <summary>
        /// This property return up-to-date campaign strategies.  Campaign strategies by default will be hold for a day unless
        /// there is notification from 3rd party to request for reload
        /// </summary>
        private List<Campaign> campaigns
        {
            get
            {
                DateTime now = DateTime.Now;
                if (_lastUpdate.DayOfYear == now.DayOfYear && _lastUpdate.Year == now.Year)    //last update is one hour ago
                    return _campaigns;
                else
                {
                    _campaigns.Clear();

                    //Reload campaignn setting from DB and 
                    //qualify campaigns to keep only validate campaigns and campaign strategies
                    DateTime currentTime = DateTime.Now;
                    foreach (Campaign campaign in CampaignHelper.getCampaigns(_storeID))
                    {
                        if (campaign.EffectiveDate <= currentTime && currentTime <= campaign.ExpiredDate)   //valid campaign
                            _campaigns.Add(campaign);
                    }
                }

                return _campaigns;
            }
        }
#endregion //regular implementation

#region Unit test
        //public void addDummyCampaign(
#endregion

    }
}
