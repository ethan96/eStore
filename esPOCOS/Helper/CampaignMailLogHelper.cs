using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class CampaignMailLogHelper : Helper
    {
        public CampaignMailLog getCampaignMailLog(CampaignMailLog _camLog)
        {
            if (!_camLog.CampaignID.HasValue)
                return null;
            var camLog = (from c in context.CampaignMailLogs
                          where c.StoreID == _camLog.StoreID && c.CampaignID == _camLog.CampaignID.Value
                          select c).FirstOrDefault();
            return camLog;
        }

        /// <summary>
        /// This method is to look up user mail activity per campaign. It returns null if user does not
        /// receive email of specified campaign within the past balckout days.  Otherwise it returns
        /// user's last email activity.
        /// </summary>
        /// <param name="cartid"></param>
        /// <param name="campaignID"></param>
        /// <param name="blackoutDays"></param>
        /// <returns></returns>
        public CampaignMailLog getCampaignMailLog(string cartid, int campaignID, int blackoutDays = 365)
        {
            DateTime blackoutPeriod = DateTime.Now.AddDays(-blackoutDays);
            var cam = (from c in context.CampaignMailLogs
                       where c.UserId == cartid && c.CampaignID == campaignID
                         && c.CreateDate > blackoutPeriod
                       orderby c.CreateDate descending
                       select c).FirstOrDefault();

            return cam;
        }

        public List<CampaignMailLog> getCampaignMailLogList(DateTime startDate, DateTime endDate, string email = "",string status ="",string storeId = "AUS")
        {
            endDate = endDate.AddDays(1);
            bool isApplied = true;
            if (!string.IsNullOrEmpty(status))
                isApplied = bool.Parse(status);

            List<CampaignMailLog> cmlList = new List<CampaignMailLog>();
            var cmlData = (from c in context.CampaignMailLogs.Include("Campaigns")
                           join pal in context.PromotionAppliedLogs on new { id = c.CampaignID.Value, email = c.UserId, store = c.StoreID } equals
                                                                     new { id = pal.CampaignID, email = pal.UserID, store = pal.StoreID } into palItem
                           from palData in palItem.DefaultIfEmpty()
                           where (string.IsNullOrEmpty(email) ? c.CreateDate > startDate && c.CreateDate < endDate : c.UserId.ToLower().Contains(email))
                           && c.StoreID == storeId && (string.IsNullOrEmpty(status) ? true : (palData != null) == isApplied)
                           select new
                           {
                               Obj = c,
                               IsApplied = palData != null
                           }
                        ).Distinct().ToList();
            foreach (var item in cmlData)
            {
                item.Obj.IsApplied = item.IsApplied;
                cmlList.Add(item.Obj);
            }

            foreach (CampaignMailLog item in cmlList)
            {
                item.helper = this;
            }
            return cmlList;
        }

        public int save(CampaignMailLog _camLog)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_camLog == null || _camLog.validate() == false) return 1;

            try
            {

                context.CampaignMailLogs.AddObject(_camLog); //state=added.
                context.SaveChanges();
                return 0;
            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }

        /// <summary>
        /// This method returns the campaign's latest email activity time
        /// </summary>
        /// <param name="campaignID"></param>
        /// <returns></returns>
        public DateTime getCampaignLastFollowupTime(int campaignID)
        {
            DateTime? mailLog = (from c in context.CampaignMailLogs
                                 where c.CampaignID == campaignID
                       orderby c.CreateDate descending
                       select c.CreateDate).FirstOrDefault();

            if (mailLog == null)
                return DateTime.MinValue;
            else
                return mailLog.GetValueOrDefault();
        }
    }
}
