using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules.Reward
{
    public class RewardPointMgt
    {
        private POCOS.Store _store;
        private List<RewardActivity> _rewardActivities;
        public RewardPointMgt(POCOS.Store store, List<RewardActivity> rewardActivities)
        {
            this._store = store;
            this._rewardActivities = rewardActivities;
        }

        public RewardPointMgt(POCOS.Store store)
        {
            this._store = store;
            _rewardActivities = (new RewardActivityHelper()).getActivity_RewardActivityByStoreid(store.StoreID);
        }

        public IRewardPoint dal
        {
            get
            {
                if (_store.StoreID == "AKR")
                    return new RewardPointFromUnica() { rewardActivities = this._rewardActivities, store = this._store };
                else
                    return new RewardPointFromDB() { rewardActivities = this._rewardActivities, store = this._store };
            }
        }
    }
}
