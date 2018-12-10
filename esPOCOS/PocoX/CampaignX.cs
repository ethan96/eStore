using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Campaign
    {
        public List<CampaignStrategy> strategiesX
        {
            get
            {
                var _strategies = (from strategy in CampaignStrategies
                                                where (strategy.isDeleted==false)
                                              select strategy).ToList();
                if (_strategies != null)
                {
                    if(this.PromotionType == "System")
                    {
                        foreach(var s in _strategies)
                            s.PromotionCode = this.PromotionCode;
                    }
                    return _strategies;
                }
                else
                    return null; 
            }
        }

        public void removeStrategy(CampaignStrategy strategy)
        {
            strategy.Status = "DELETED";
        }

        public CampaignStrategy getStrategy(int strategyID)
        {
            return (from item in CampaignStrategies
                                         where item.ID == strategyID
                                         select item).FirstOrDefault();
        }

        public void removeStrategy(int strategyID)
        {
            CampaignStrategy strategy = getStrategy(strategyID);
            if (strategy != null)
                removeStrategy(strategy);
        }

        public Boolean isDeleted
        {
            get
            {
                if (!String.IsNullOrEmpty(Status) && Status.ToUpper().Equals("DELETED"))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// This method return the latest campaign code record if there is any.  Only the campaigns that have CampaignType as "System" has campaign codes.
        /// This feature is mainly for campaigns that have multiple coupon codes.
        /// </summary>
        public CampaignCode newestCampaignCode
        {
            get
            {
                if (_newestCampaignCode == null)
                    _newestCampaignCode = campaignCodesX.OrderByDescending(c => c.CreatedDate).FirstOrDefault();

                return _newestCampaignCode;
            }

            set { _newestCampaignCode = value;   }
        }
        private CampaignCode _newestCampaignCode = null;

        /// <summary>
        /// System 类型的 Campaign 生成新的 CampaignCode
        /// </summary>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        public CampaignCode createNewCampaignCode(int expiredDate)
        {
            if (this.PromotionType.Equals("System", StringComparison.OrdinalIgnoreCase))
            {
                string newPromotionCode;
                //create new code
                while (true)
                {
                    var guid = Guid.NewGuid().ToString("N").ToUpper().Substring((new Random()).Next(1, 9), 6);
                    newPromotionCode = (string.IsNullOrEmpty(this.CouponPreix) ? "XX" : this.CouponPreix) + guid;
                    var _exitCamCode = (new CampaignCodeHelper()).getCampaignCode(this.CampaignID, this.StoreID, newPromotionCode);
                    if (_exitCamCode == null)
                        break;
                }
                DateTime now = DateTime.Now;
                var todayPromotion = new CampaignCode()
                {
                    CampaignID = this.CampaignID,
                    CreatedDate = now,
                    EffectiveDate = now.Date,
                    ExpiredDate = now.AddDays(expiredDate).Date, 
                    StoreId = this.StoreID,
                    PromotionCode = newPromotionCode
                };
                if (todayPromotion.save() == 0)
                {
                    this.newestCampaignCode = todayPromotion;
                    return todayPromotion;
                }
            }
            return null;
        }

        private List<CampaignCode> _campaignCodesX;
        public List<CampaignCode> campaignCodesX
        {
            get {
                if (_campaignCodesX == null)
                    _campaignCodesX = new CampaignCodeHelper().getCampaignCodeListByCampaignId(this.StoreID, this.CampaignID);
                return _campaignCodesX;
            }
        }
        

        public bool IsCanUse
        {
            get 
            {
                if (isDeleted || EffectiveDate > DateTime.Now || ExpiredDate < DateTime.Now)
                    return false;

                return true;
            }
        }
        
    }
}
