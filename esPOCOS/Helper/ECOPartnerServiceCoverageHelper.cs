using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOPartnerServiceCoverageHelper : Helper
    {
        public ECOPartnerServiceCoverage getPartnerServiceCoverageById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _ecoCoverage = (from p in context.ECOPartnerServiceCoverages
                                    where p.Id == id
                                    select p).FirstOrDefault();
                if (_ecoCoverage != null)
                    _ecoCoverage.helper = this;
                return _ecoCoverage;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public int delete(ECOPartnerServiceCoverage ecoCoverage)
        {
            if (ecoCoverage == null)
                return 1;
            try
            {
                var _exitobj = getPartnerServiceCoverageById(ecoCoverage.Id);
                if (_exitobj == null)
                    return 1;

                context.ECOPartnerServiceCoverages.DeleteObject(_exitobj);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }
    }
}
