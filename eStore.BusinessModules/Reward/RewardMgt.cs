using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules.Reward
{
    public class RewardMgt
    {
        private string conditionType = "";
        private RewardActivity _activity = null;


        public RewardMgt(RewardActivity activity)
        {
            if (activity == null)
                return;
            _activity = activity;
            RewardActivityHelper helper = new RewardActivityHelper();
            var rewardCondition = activity.RewardConditions.FirstOrDefault();
            conditionType = rewardCondition == null ? "Rate" : rewardCondition.RewardType;
        }


        public IReward getRewardHelper()
        {
            var reward = (IReward)Assembly.Load("eStore.BusinessModules").CreateInstance("eStore.BusinessModules.Reward.Reward" + conditionType);
            if (reward != null && reward is RewardBase)
                (reward as RewardBase).rewardActivity = _activity;
            return reward;
        }

        public void grantNewRegisterPoint(User user)
        {
            if (user == null)
                return;
            if (_activity.NewRegisterPoint.HasValue)
            {
                decimal userTotalPoint = (new RewardLogHelper()).getUserAllPoint(_activity.StoreId, user.UserID, _activity.Id);
                if (userTotalPoint == 0)
                {
                    POCOS.RewardLog currentRewardLog = new POCOS.RewardLog();
                    currentRewardLog.StoreId = _activity.StoreId;
                    currentRewardLog.UserId = user.UserID;
                    currentRewardLog.TransactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Approved;
                    currentRewardLog.OrderNo = "New Register";
                    currentRewardLog.RewardPoint = _activity.NewRegisterPoint.Value;
                    currentRewardLog.UpdateDate = DateTime.Now;
                    currentRewardLog.UpdateBy = "NewRegister";
                    currentRewardLog.ActivityId = _activity.Id;
                    currentRewardLog.save();
                }
            }
        }

    }
}
