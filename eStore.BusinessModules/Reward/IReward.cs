using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules.Reward
{
    public interface IReward
    {
        void calculateOrderReward(Order currentOrder);
    }
}
