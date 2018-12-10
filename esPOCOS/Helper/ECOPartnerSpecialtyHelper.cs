using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOPartnerSpecialtyHelper : Helper
    {
        public ECOPartnerSpecialty getPartnerSpecialtyById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _ecoPartnerSpecialty = (from p in context.ECOPartnerSpecialties
                                    where p.Id == id
                                    select p).FirstOrDefault();
                if (_ecoPartnerSpecialty != null)
                    _ecoPartnerSpecialty.helper = this;
                return _ecoPartnerSpecialty;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public int delete(ECOPartnerSpecialty ecoPartnerSpecialty)
        {
            if (ecoPartnerSpecialty == null)
                return 1;
            try
            {
                var _exitobj = getPartnerSpecialtyById(ecoPartnerSpecialty.Id);
                if (_exitobj == null)
                    return 1;
                context.ECOPartnerSpecialties.DeleteObject(_exitobj);
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
