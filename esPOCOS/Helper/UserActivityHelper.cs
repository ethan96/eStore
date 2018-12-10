using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class UserActivityHelper : Helper
    {
        #region Creat Update Delete

        public int save(UserActivityLog _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;
            //Try to retrieve object from DB

            try
            {
                //Insert                  
                context.UserActivityLogs.AddObject(_log);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(UserActivityLog _log)
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

        /// <summary>
        /// get user activity log summery by date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<UserActivityLog> getUserActivitySummery(DateTime startDate, DateTime endDate, string storeid)
        {
            var ss = (from log in context.UserActivityLogs
                      let logids = from lo in context.UserActivityLogs
                                   where lo.StoreID.Equals(storeid) && lo.CreatedDate >= startDate && lo.CreatedDate <= endDate
                                    && !string.IsNullOrEmpty(lo.RemoteAddr) && !lo.RemoteAddr.StartsWith("172.")
                                    && !lo.RemoteAddr.StartsWith("127.") && !lo.RemoteAddr.StartsWith("10.")
                                    && (string.IsNullOrEmpty(lo.UserId) || !lo.UserId.ToLower().EndsWith("@damez.com"))
                                   group lo by lo.UserId into g
                                   select g.Max(c => c.ID)
                      where logids.Contains(log.ID)
                      select log).ToList();

            return ss;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<UserActivityLog> getUserActivityLogByDate(string userid ,DateTime startDate, DateTime endDate, string storeid)
        {
            var logs = (from log in context.UserActivityLogs
                        where log.StoreID.Equals(storeid) && log.CreatedDate >= startDate && log.CreatedDate <= endDate && log.UserId == userid
                        select log).ToList();
            return logs;
        }

        /// <summary>
        /// get user activity log bu session id
        /// </summary>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        public List<UserActivityLog> getAllUserActivityLog(string sessionid)
        {
            List<UserActivityLog> userActivityLog = (from p in context.UserActivityLogs
                                                     where p.SessionID == sessionid
                                                     select p).ToList();
            if (userActivityLog != null)
                return userActivityLog;
            else
                return new List<UserActivityLog>();

        }

        public List<string> getUserActivityLogByProductID(DateTime startdate, DateTime enddate, string storeId, string productNo)
        {
            List<UserActivityLog> logs = new List<UserActivityLog>();
            List<string> userLogs = new List<string>();
            if (storeId.ToUpper() == "ALL")
            {
                logs = (from log in context.UserActivityLogs
                        where log.ProductID == productNo && log.CreatedDate >= startdate && log.CreatedDate <= enddate && log.CategoryType.ToUpper() != "SEARCH"
                        && !log.UserId.Contains("@advantech.") && !log.UserId.Contains("@damez.com")
                        select log).Distinct().ToList();
                userLogs = (from l in logs
                            group l by l.UserId into user
                            orderby user.Key
                            select user.Key).ToList();
                                
            }
            else
            {
                logs = (from log in context.UserActivityLogs
                        where log.ProductID == productNo && log.CreatedDate >= startdate && log.CreatedDate <= enddate && log.CategoryType.ToUpper() != "SEARCH"
                        && !log.UserId.Contains("@advantech.") && !log.UserId.Contains("@damez.com") && log.StoreID == storeId.ToUpper()
                        select log).Distinct().ToList();
                userLogs = (from l in logs
                            group l by l.UserId into user
                            orderby user.Key
                            select user.Key).ToList();
            }
            return userLogs;
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(UserActivityHelper).ToString();
        }
        #endregion
    }
}