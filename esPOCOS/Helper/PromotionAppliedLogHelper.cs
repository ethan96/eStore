using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class PromotionAppliedLogHelper : Helper
    {

        #region Business Read
        public PromotionAppliedLog getExistLog(string storeid, string no, string type,int strategyID)
        {
            var _log = (from plog in context.PromotionAppliedLogs
                        where plog.QuoteOrderNo == no && plog.CampaignStrategyID == strategyID && type.ToLower() == type.ToLower()
                       && plog.StoreID == storeid
                       select plog).FirstOrDefault();

              return _log;
        
        }


        public int getUsedTime(string storeid, string promotioncode)
        {
            var _log = (from plog in context.PromotionAppliedLogs
                        where  plog.PromotionCode == promotioncode &&  plog.StoreID == storeid
                        select plog).Count();
            return _log;

        }

        public List<PromotionAppliedLog> getPromotionAppliedLogsbyStrategyID(string storeid, int strategyID)
        {
            var _logs = (from plog in context.PromotionAppliedLogs
                         where plog.CampaignStrategyID == strategyID && plog.StoreID == storeid
                         select plog).ToList();
            return _logs;
        }

        public List<PromotionAppliedLog> getPromotionAppliedLogsbyStrategyID(string storeid, int strategyID, string promotionCode)
        {
            var _logs = (from plog in context.PromotionAppliedLogs
                         where plog.CampaignStrategyID == strategyID && plog.StoreID == storeid && plog.PromotionCode == promotionCode
                         select plog).ToList();
            return _logs;
        }

        #endregion

        #region Creat Update Delete
        public int save(PromotionAppliedLog  _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;
            //Try to retrieve object from DB

            PromotionAppliedLog _exist_log = getExistLog(_log.StoreID,_log.QuoteOrderNo,_log.Type,_log.CampaignStrategyID);
            try
            {
                if (_exist_log == null)  //object not exist 
                {
                    //Insert
                    context.PromotionAppliedLogs .AddObject(_log);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PromotionAppliedLogs.ApplyCurrentValues(_log);
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

        public int delete(PromotionAppliedLog _log)
        {

            if (_log == null || _log.validate() == false) return 1;
            try
            {
                context.DeleteObject(_log);
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

        #region Others

        private static string myclassname()
        {
            return typeof(PromotionAppliedLog).ToString();
        }
        #endregion
    }
}