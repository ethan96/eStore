using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class CampaignCodeHelper : Helper
    {
        public int save(CampaignCode camCode)
        {
            //if parameter is null or validation is false, then return  -1 
            if (camCode == null || camCode.validate() == false) return 1;

            //Try to retrieve object from DB  
            CampaignCode _exist_camCode = getCampaignCode(camCode);
            try
            {
                if (camCode.helper != null)
                    context = camCode.helper.context;
                if (_exist_camCode == null)  //Add 
                {
                    context.CampaignCodes.AddObject(camCode); //state=added.
                    context.SaveChanges();
                    return 0;
                }
                else //Update 
                {
                    if (camCode.helper.context != null)
                        context = camCode.helper.context;

                    context.CampaignCodes.ApplyCurrentValues(camCode); //Even applycurrent value, cartmaster state still in unchanged.                 
                    context.SaveChanges();
                }
                return 0;
            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }

        public CampaignCode getCampaignCode(CampaignCode camCode)
        {
            var camc = (from c in context.CampaignCodes
                        where c.StoreId == camCode.StoreId && c.PromotionCode == camCode.PromotionCode
                            && c.CampaignID == camCode.CampaignID
                        select c).FirstOrDefault();
            return camc;
        }

        public CampaignCode getCampaignCode(int camId,string storeid,string promcode)
        {
            var camc = (from c in context.CampaignCodes
                        where c.StoreId == storeid && c.PromotionCode == promcode
                            && c.CampaignID == camId
                        select c).FirstOrDefault();
            return camc;
        }

        //根据CampaignId获取CampaignCode
        public List<CampaignCode> getCampaignCodeListByCampaignId(String storeId,int campaignId)
        {
            DateTime currentTime = DateTime.Now.Date;
            var campaingnCodeList = (from c in context.CampaignCodes
                                 where c.StoreId == storeId && c.CampaignID == campaignId
                                 && c.EffectiveDate <= currentTime && currentTime <= c.ExpiredDate
                                 select c).ToList();
            return campaingnCodeList;
        }
    }
}
