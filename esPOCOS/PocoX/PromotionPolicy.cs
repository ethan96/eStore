using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// This class defines properties and methods of Promotion policy.  The policy is a derived instance 
    /// from Campaign and CampaignStrategy settings.  Instance of this class will usually be created by CampaignManager
    /// </summary>
    public class PromotionPolicy
    {
        private int _campaignID;
        private string _promotionCode;
        private int _strategyID;
        private DateTime _expirationTime = DateTime.Now.AddMinutes(10); //good for 10 minutes only
        public enum DiscountValueType { FixValue, Rate };

        public PromotionPolicy(int campaignID, int strategyID)
        {
            _campaignID = campaignID;
            _strategyID = strategyID;
        }

        public Boolean isExpired()
        {
            if (DateTime.Now > _expirationTime)
                return true;
            else
                return false;
        }

        public int campaignID { get { return _campaignID; } }

        public string PromotionCode 
        { 
            get { return _promotionCode; }
            set { _promotionCode = value; }
        }

        public int strategyID { get { return _strategyID; } }

        public DiscountValueType discountValueType { get; set; }

        public string promoteMessage { get; set; }

        public Decimal discountPrice { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }
}
