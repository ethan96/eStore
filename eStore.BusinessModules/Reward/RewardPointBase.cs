using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules.Reward
{
    public class RewardPointBase
    {
        public POCOS.Store store;
        public List<RewardActivity> rewardActivities = new List<RewardActivity>();

        //获取用户可用的积分
        public virtual Dictionary<RewardActivity, decimal> getUserTotalPoint(string userId, MiniSite minisite = null)
        {
            Dictionary<RewardActivity, decimal> ls = new Dictionary<RewardActivity, decimal>();
            foreach (var i in rewardActivities)
                ls.Add(i, (new RewardLogHelper()).getUserTotalPoint(store.StoreID, userId, i.Id));
            return ls;
        }

        //获取用户所有的积分
        public virtual Dictionary<RewardActivity, decimal> getUserAllPoint(string userId, MiniSite minisite = null)
        {
            Dictionary<RewardActivity, decimal> ls = new Dictionary<RewardActivity, decimal>();
            foreach (var i in rewardActivities)
                ls.Add(i, (new RewardLogHelper()).getUserAllPoint(store.StoreID, userId, i.Id));
            return ls;
        }

        public virtual List<POCOS.RewardLog> getRewardLogByRewardApproval(string startdate = "", string enddate = "", string orderNo = "", string userId = "")
        {
            return new RewardLogHelper().getRewardLogByRewardApproval(store.StoreID, startdate, enddate, orderNo, userId);
        }

        public virtual List<POCOS.RewardLog> getAllRewardLogByCondition(string startdate = "", string enddate = "", string userId = "")
        {
            return new RewardLogHelper().getAllRewardLogByCondition(store.StoreID, startdate, enddate, userId);
        }
    }
}
