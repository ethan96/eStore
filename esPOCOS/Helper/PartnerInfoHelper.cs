using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class PartnerInfoHelper : Helper
    {

        public List<PartnersInfo> getAllPartnersInfoByStoreId(string storeID)
        {
            List<PartnersInfo> partnersInfoList = new List<PartnersInfo>();
            //partnersInfoList = (from ep in context.PartnersInfoes
            //                      select ep).Distinct().OrderBy(p=>p.Country).ToList();
            partnersInfoList = context.PartnersInfoes.Where(p => p.StoreID == storeID && p.Latitude == null).Distinct().OrderBy(p => p.Country).ToList();
            if (partnersInfoList != null)
            {
                foreach (var item in partnersInfoList)
                {
                    item.helper = this;
                }
            }
            return partnersInfoList;
        }

        public List<PartnersInfo> getPartnersInfoByCountryAndStoreId(string country, string storeID)
        {
            List<PartnersInfo> partnersInfoList = new List<PartnersInfo>();

            partnersInfoList = context.PartnersInfoes.Where(p => p.Country == country && p.StoreID == storeID).ToList();

            if (partnersInfoList != null)
            {
                foreach (var item in partnersInfoList)
                {
                    item.helper = this;
                }
            }
            return partnersInfoList;
        }

        public PartnersInfo getPartInfoById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _partnerInfo= (from p in context.PartnersInfoes
                                   where p.ID == id
                                   select p).FirstOrDefault();
                if (_partnerInfo != null)
                    _partnerInfo.helper = this;
                return _partnerInfo;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        public int save(PartnersInfo partnerInfo)
        {
            if (partnerInfo == null || !partnerInfo.validate())
                return 1;
            try
            {
                var _exitobj = getPartInfoById(partnerInfo.ID);
                if (_exitobj != null)
                    context.PartnersInfoes.ApplyCurrentValues(partnerInfo);
                else
                    context.PartnersInfoes.AddObject(partnerInfo);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }



        public int delete(PartnersInfo partnerInfo)
        {
            if (partnerInfo == null)
                return 1;
            try
            {
                context.PartnersInfoes.DeleteObject(partnerInfo);
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
