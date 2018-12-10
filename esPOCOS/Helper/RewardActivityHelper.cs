using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.POCOS.DAL
{
    public class RewardActivityHelper : Helper
    {
        public List<RewardActivity> getActivity_RewardActivityByStoreid(string storeid, MiniSite minisite = null)
        {
            if(minisite == null)
                return context.RewardActivities.Where(c => c.StoreId.Equals(storeid)
                        && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now
                        && c.Publish == true).ToList();
            else
                return context.RewardActivities.Where(c => c.StoreId.Equals(storeid)
                        && c.Minisite.Equals(minisite.ID)
                        && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now
                        && c.Publish == true).ToList();
        }
    }
}
