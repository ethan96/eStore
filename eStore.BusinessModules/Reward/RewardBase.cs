using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace eStore.BusinessModules.Reward
{
    public class RewardBase
    {
        public RewardActivity rewardActivity = new RewardActivity();

        //计算用户成功消费总金额(default one year)
        protected decimal getUserTotalAmountByYear(string userId, string storeid, DateTime? start = null, DateTime? end = null)
        {
            return (new OrderHelper()).getUserTotalAmountByYear(storeid, userId, start, end);
        }
    }
}
