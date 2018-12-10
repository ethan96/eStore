using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class EDMCampaignSchedulerHelper : Helper
    {
         public EDMCampaignScheduler getEdmCampaignSchedulerByID(int edmCampiagnId)
         {
             if (string.IsNullOrEmpty(edmCampiagnId.ToString()))
                 return null;
             try
             {
                 var result = (from c in context.EDMCampaignSchedulers
                               where c.id == edmCampiagnId
                               select c).FirstOrDefault();
                 if (result != null)
                     result.helper = this;
                 return result;
             }
             catch (Exception ex)
             {
                 eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                 return null;
             }
         }

         /// <summary>
         /// 获取当前store下所有的campains
         /// </summary>
         /// <param name="storeid"></param>
         /// <param name="todayTime"></param>
         /// <param name="isCanShow"></param>
         /// <returns></returns>
         public List<EDMCampaignScheduler> getAllEDMCampaignSchedulerByStoreID(string storeid, DateTime startDate, DateTime endDate)
         {
             if (string.IsNullOrEmpty(storeid))
                 return null;
             try
             {
                 var _result = (from c in context.EDMCampaignSchedulers
                                where c.StoreID == storeid & (c.LaunchDate >= startDate && c.LaunchDate <= endDate)
                                orderby c.LaunchDate ascending
                                select c).ToList();

                 List<EDMCampaignScheduler> campaigns = null;
                 if (_result != null)
                     campaigns = _result;
                 else
                     return new List<EDMCampaignScheduler>();

                 foreach (EDMCampaignScheduler c in campaigns)
                     c.helper = this;

                 return campaigns;
             }
             catch (Exception ex)
             {
                 eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                 return new List<EDMCampaignScheduler>();
             }

         }
        #region CRUD

        public int save(EDMCampaignScheduler  _campaign)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_campaign == null || _campaign.validate() == false) return 1;
            //Try to retrieve object from DB             

            EDMCampaignScheduler _exist_camp = getEdmCampaignSchedulerByID(_campaign.id);
            try
            {
                if (_campaign.helper != null && _campaign.helper.context != null)
                    context = _campaign.helper.context;


                if (_exist_camp == null)  //object not exist 
                {
                    context.EDMCampaignSchedulers.AddObject(_campaign); //state=added.                            
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.EDMCampaignSchedulers.ApplyCurrentValues(_campaign);
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


        public int delete(EDMCampaignScheduler _edmcampaign)
        {

            if (_edmcampaign == null || _edmcampaign.validate() == false) return 1;

            //Campaign will not be permenently removed.  Instead it'll be marked as deleted
            
            try
            {
                context = _edmcampaign.helper.context;
                context.DeleteObject(_edmcampaign);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        #endregion
    }
}