using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ECOIndustryHelper : Helper
    {
        public ECOIndustry getIndustryById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _ecoIndustry = (from p in context.ECOIndustries
                                    where p.IndustryId == id
                                    select p).FirstOrDefault();
                if (_ecoIndustry != null)
                    _ecoIndustry.helper = this;
                return _ecoIndustry;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public ECOIndustry getIndustryByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            try
            {
                var _ecoIndustry = (from p in context.ECOIndustries
                                    where p.IndustryName == name
                                    select p).FirstOrDefault();
                if (_ecoIndustry != null)
                    _ecoIndustry.helper = this;
                return _ecoIndustry;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<ECOIndustry> getAllECOIndustry(bool isPublish = false)
        {
            List<ECOIndustry> _ecoIndustryList = new List<ECOIndustry>();
            if (isPublish)
                _ecoIndustryList = context.ECOIndustries.Where(p => p.PublishStatus == true).ToList();
            else
                _ecoIndustryList = context.ECOIndustries.ToList();
            if (_ecoIndustryList != null)
            {
                foreach (var item in _ecoIndustryList)
                {
                    item.helper = this;
                }
            }
            else
                _ecoIndustryList = new List<ECOIndustry>();
            return _ecoIndustryList;
        }

        public int save(ECOIndustry ecoIndustry)
        {
            if (ecoIndustry == null || !ecoIndustry.validate())
                return 1;
            try
            {
                var _exitobj = getIndustryById(ecoIndustry.IndustryId);
                if (_exitobj != null)
                    context.ECOIndustries.ApplyCurrentValues(ecoIndustry);
                else
                    context.ECOIndustries.AddObject(ecoIndustry);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }

        public int delete(ECOIndustry ecoIndustry)
        {
            if (ecoIndustry == null)
                return 1;
            try
            {
                context.ECOIndustries.DeleteObject(ecoIndustry);
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
