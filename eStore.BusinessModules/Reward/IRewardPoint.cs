using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules.Reward
{
    public interface IRewardPoint
    {
        Dictionary<RewardActivity, decimal> getUserTotalPoint(string userId, MiniSite minisite = null);
        Dictionary<RewardActivity, decimal> getUserAllPoint(string userId, MiniSite minisite = null);
        List<RewardLog> getRewardLogByRewardApproval(string startdate = "", string enddate = "", string orderNo = "", string userId = "");
        List<RewardLog> getAllRewardLogByCondition(string startdate = "", string enddate = "", string userId = "");
    }
}
