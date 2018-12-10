using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules.Reward
{
    public class RewardRate : RewardBase, IReward
    {

        //根据金额 获取 等级信息
        private RewardCondition getRewardConditionByRevenue(decimal totalAmount, string storeid)
        {
            return (new RewardConditionHelper()).getRewardConditionByRevenue(storeid, totalAmount, rewardActivity.Id);
        }
        
        //或者用户的最后一个 等级信息
        private MemberClassChangeLog getUserMemberClassLog(string userId, string storeid)
        {
            return (new MemberClassChangeLogHelper()).getUserMemberClassLog(storeid, userId);
        }

        public void calculateOrderReward(Order currentOrder)
        {
            if (currentOrder != null)
            {
                //一年的总金额
                decimal userTotalAmount = getUserTotalAmountByYear(currentOrder.UserID, currentOrder.StoreID);
                //根据一年的总金额 获取对应的等级信息
                POCOS.RewardCondition currentCondition = getRewardConditionByRevenue(userTotalAmount, currentOrder.StoreID);
                if (currentCondition != null)
                {
                    //获取用户 最后一次等级日志
                    POCOS.MemberClassChangeLog currentMemeberClass = getUserMemberClassLog(currentOrder.UserID, currentOrder.StoreID);
                    //没有等级日志   或者   等级日志 !=  最新会员等级信息
                    if (currentMemeberClass == null || (currentMemeberClass != null && currentMemeberClass.ConditionID != currentCondition.ConditionID))
                    {
                        currentMemeberClass = new MemberClassChangeLog();
                        currentMemeberClass.StoreId = currentOrder.StoreID;
                        currentMemeberClass.UserID = currentOrder.UserID;
                        currentMemeberClass.ConditionID = currentCondition.ConditionID;
                        currentMemeberClass.EffectDate = DateTime.Now;
                        currentMemeberClass.save();
                    }

                    if (currentMemeberClass != null)
                    {
                        //创建一个 待审核的log
                        POCOS.RewardLog currentRewardLog = new POCOS.RewardLog();
                        currentRewardLog.StoreId = currentOrder.StoreID;
                        currentRewardLog.UserId = currentOrder.UserID;
                        currentRewardLog.TransactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Pending;
                        currentRewardLog.OrderNo = currentOrder.OrderNo;
                        currentRewardLog.RewardPoint = currentOrder.TotalAmount * currentCondition.RewardRate;
                        currentRewardLog.UpdateDate = DateTime.Now;
                        currentRewardLog.UpdateBy = currentOrder.UserID;
                        currentRewardLog.ActivityId = rewardActivity.Id;
                        currentRewardLog.save();
                    }
                }
            }
        }
    }
}
