using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules.Reward
{
    public class RewardNumber : RewardBase, IReward
    {
        public void calculateOrderReward(Order currentOrder)
        {
            decimal userTotalAmount = getUserTotalAmountByYear(currentOrder.UserID, currentOrder.StoreID, rewardActivity.StartDate, rewardActivity.EndDate);
            //用户总点数，需要扣除新注册用户的点数。
            decimal userAllPoint = (new RewardLogHelper()).getUserAllPoint(currentOrder.StoreID, currentOrder.UserID, rewardActivity.Id);
            //Check this user's ID is new register
            RewardLog registerRecord = (new RewardLogHelper()).getRewardLogByUserId(currentOrder.StoreID, currentOrder.UserID, rewardActivity.Id)
                .Where(x => x.UpdateBy.Equals("NewRegister", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            //Maximum point is 180
            if (userAllPoint >= 180)
                return;

            decimal newRegisterPoint = 0;//Save Register point
            if (rewardActivity.NewRegisterPoint.HasValue && registerRecord != null)
            {
                userAllPoint -= rewardActivity.NewRegisterPoint.GetValueOrDefault();
                newRegisterPoint = rewardActivity.NewRegisterPoint.GetValueOrDefault();
            }

            var dondition = rewardActivity.RewardConditions.FirstOrDefault();
            if (dondition != null && dondition.RewardRate != 0 && dondition.RevenueMin == dondition.RevenueMax)
            {
                var cont = Math.Floor(userTotalAmount / dondition.RevenueMin.GetValueOrDefault(1000000000)) - Math.Floor(userAllPoint / dondition.RewardRate.GetValueOrDefault(1000000000));
                if(cont > 0)
                {
                    //Reward point cannot over than 180 points.
                    decimal rewardPoint = dondition.RewardRate.Value * cont;
                    if (rewardPoint + userAllPoint + newRegisterPoint > 180)
                        rewardPoint = 180 - userAllPoint - newRegisterPoint;

                    //创建一个已审核的log
                    POCOS.RewardLog currentRewardLog = new POCOS.RewardLog();
                    currentRewardLog.StoreId = currentOrder.StoreID;
                    currentRewardLog.UserId = currentOrder.UserID;
                    currentRewardLog.TransactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Approved;
                    currentRewardLog.OrderNo = currentOrder.OrderNo;
                    currentRewardLog.RewardPoint = rewardPoint;
                    currentRewardLog.UpdateDate = DateTime.Now;
                    currentRewardLog.UpdateBy = currentOrder.UserID;
                    currentRewardLog.ActivityId = rewardActivity.Id;
                    currentRewardLog.save();
                }
            }
        }
    }
}
