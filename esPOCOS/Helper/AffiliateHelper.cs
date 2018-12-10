using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class AffiliateHelper : Helper
    {

        public Affiliate getAffiliateBySiteID(string siteid)
        {
            return context.Affiliates.FirstOrDefault(c=>c.SiteID == siteid);
        }

        internal int save(Affiliate affiliate)
        {
            if (affiliate == null || affiliate.validate() == false)
                return 1;
            try
            {
                var exitAffiliate = getAffiliateBySiteID(affiliate.SiteID);
                if (exitAffiliate == null)
                {
                    affiliate.CreateDate = DateTime.Now;
                    context.Affiliates.AddObject(affiliate);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = affiliate.helper.context;
                    context.Affiliates.ApplyCurrentValues(affiliate);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }       
        }

        internal int delete(Affiliate affiliate)
        {
            throw new NotImplementedException();
        }
    }
}
