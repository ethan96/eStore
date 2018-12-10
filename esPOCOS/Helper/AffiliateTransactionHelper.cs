using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class AffiliateTransactionHelper : Helper
    {

        internal int save(AffiliateTransaction affiliateTransaction)
        {
            if (affiliateTransaction == null || affiliateTransaction.validate() == false)
                return 1;
            try
            {
                var exitAffiliateTran = getAffiliateTranByOrderNo(affiliateTransaction);
                if (exitAffiliateTran == null)
                {
                    affiliateTransaction.CreateDate = DateTime.Now;
                    context.AffiliateTransactions.AddObject(affiliateTransaction);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = affiliateTransaction.helper.context;
                    context.AffiliateTransactions.ApplyCurrentValues(affiliateTransaction);
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

        private AffiliateTransaction getAffiliateTranByOrderNo(AffiliateTransaction p)
        {
            return context.AffiliateTransactions.FirstOrDefault(c => c.OrderNO == p.OrderNO && c.TrackingID == p.TrackingID && c.SiteID == p.SiteID);
        }

        internal int delete(AffiliateTransaction affiliateTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
