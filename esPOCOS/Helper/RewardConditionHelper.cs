using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class RewardConditionHelper : Helper
    {
        #region Business Read
        //根据金额 获取 等级信息
        public RewardCondition getRewardConditionByRevenue(string storeId,decimal totalAmount, int activityid)
        {
            var rewardLogItem = (from r in context.RewardConditions
                                 where r.StoreId == storeId && r.ActivityId == activityid && r.RevenueMin <= totalAmount 
                                 && r.RevenueMax > totalAmount 
                                select r).FirstOrDefault();
            if (rewardLogItem == null)
            {
                rewardLogItem = (from r in context.RewardConditions
                                 where r.StoreId == storeId && r.ActivityId == activityid
                                select r).FirstOrDefault();
            }
            return rewardLogItem;
        }

        #endregion

        #region Creat Update Delete

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(RewardConditionHelper).ToString();
        }
        #endregion
    }
}