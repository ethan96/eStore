using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOSpecialtyHelper : Helper
    {
        //search specialty by id
        public ECOSpecialty getECOSpecialtyById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _ecoSpecialty = (from p in context.ECOSpecialties
                                   where p.SpecialtyId == id
                                   select p).FirstOrDefault();
                if (_ecoSpecialty != null)
                    _ecoSpecialty.helper = this;
                return _ecoSpecialty;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        //查询所有
        public List<ECOSpecialty> getAllECOSpecialty(bool isPublish = false)
        {
            List<ECOSpecialty> _ecoSpecialtyList = new List<ECOSpecialty>();
            if (isPublish)
                _ecoSpecialtyList = context.ECOSpecialties.Where(p => p.PublishStatus == true).ToList();
            else
                _ecoSpecialtyList = context.ECOSpecialties.ToList();
            if (_ecoSpecialtyList != null)
            {
                foreach (var item in _ecoSpecialtyList)
                {
                    item.helper = this;
                }
            }
            else
                _ecoSpecialtyList = new List<ECOSpecialty>();
            return _ecoSpecialtyList;
        }

        public int save(ECOSpecialty ecoSpecialty)
        {
            if (ecoSpecialty == null || !ecoSpecialty.validate())
                return 1;
            try
            {
                var _exitobj = getECOSpecialtyById(ecoSpecialty.SpecialtyId);
                if (_exitobj != null)
                    context.ECOSpecialties.ApplyCurrentValues(ecoSpecialty);
                else
                    context.ECOSpecialties.AddObject(ecoSpecialty);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }

        public int delete(ECOSpecialty ecoSpecialty)
        {
            if (ecoSpecialty == null)
                return 1;
            try
            {
                context.ECOSpecialties.DeleteObject(ecoSpecialty);
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
