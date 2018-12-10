using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class GiftActivityHelper : Helper
    {
        public GiftActivity getGiftActivityById(int id)
        {
            return context.GiftActivities.FirstOrDefault(c => c.GiftId == id);
        }

        public List<GiftActivity> getAllGiftActivity(string storeid, int advid, decimal minProbability = 0)
        {
            var ls = from g in context.GiftActivities
                      from a in context.Advertisements
                      where a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now
                        && a.Publish && a.StoreID == storeid && a.id == advid
                        && a.id == g.AdvId && g.Storeid == storeid
                    select g;
            if (minProbability != 0)
                ls = ls.Where(c => c.Probability >= minProbability);
            return ls.ToList();
        }
    }
}
