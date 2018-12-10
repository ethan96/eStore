using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOPartnerIndustryHelper:Helper
    {

        internal int delete(ECOPartnerIndustry eCOPartnerIndustry)
        {
            if (eCOPartnerIndustry == null)
                return 1;
            try
            {
                var _exitobj = getECOPartnerIndustryById(eCOPartnerIndustry.Id);
                if (_exitobj == null)
                    return 1;
                context.ECOPartnerIndustries.DeleteObject(_exitobj);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }

        private ECOPartnerIndustry getECOPartnerIndustryById(int p)
        {
            return context.ECOPartnerIndustries.FirstOrDefault(c => c.Id == p);
        }
    }
}
