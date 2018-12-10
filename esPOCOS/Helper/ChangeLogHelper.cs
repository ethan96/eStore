using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class ChangeLogHelper : Helper
    {
        #region Business Read

        public List<ChangeLog> getChangelogs(string docid)
        {
            var clog = (from cl in context.ChangeLogs
                        where cl.DocID.ToUpper() == docid.ToUpper()
                        orderby cl.CreatedDate descending
                        select cl).ToList();
            return clog;
        }

        /// <summary>
        /// For OM to query change logs.
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="docid"></param>
        /// <returns></returns>
        public List<ChangeLog> getChangelogs(DateTime startdate, DateTime enddate, string docid ,string storeId = "AUS", bool isCategory = true) {
            try
            {
                enddate = enddate.Date.AddHours(24);
                List<ChangeLog> _logs;
                if (isCategory)
                {
                    List<string> docids = new List<string>();
                    int kt;
                    if (int.TryParse(docid, out kt))
                        docids.Add(kt.ToString());
                    var tc = (from cat in context.ProductCategories
                              where cat.CategoryPath.ToUpper().StartsWith(docid.ToUpper())
                              && cat.Storeid.ToUpper() == storeId.ToUpper()
                              select cat).ToList();
                    if (tc != null && tc.Count > 0)
                        foreach (var c in tc)
                            docids.Add(c.CategoryID.ToString());
                    _logs = (from cl in context.ChangeLogs
                             where cl.CreatedDate >= startdate.Date
                             && cl.CreatedDate <= enddate
                             && docids.Contains(cl.DocID.ToUpper())
                             && cl.StoreID.ToUpper().Equals(storeId.ToUpper())
                             orderby cl.CreatedDate descending
                             select cl).ToList();
                }
                else
                {
                    _logs = (from c in context.ChangeLogs
                            where c.CreatedDate >= startdate.Date
                            && c.CreatedDate <= enddate
                            && c.DocID.ToUpper().StartsWith(docid.ToUpper())
                            && c.StoreID.ToUpper().Equals(storeId.ToUpper())
                            orderby c.CreatedDate descending
                            select c).ToList();
                }
                if (_logs != null)
                    return _logs;
                else
                    return new List<ChangeLog>();
            }
             catch (Exception ex){
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                 return new List<ChangeLog>();
             }
             
        }

        /// <summary>
        /// For OM to download report
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public List<ChangeLog> getChangelogsByActivity(DateTime startdate, DateTime enddate, List<string> activities)
        {
            try
            {
                return (from c in context.ChangeLogs
                        where c.CreatedDate >= startdate.Date
                        && c.CreatedDate <= enddate.Date
                        && activities.Contains(c.Activity)
                        orderby c.StoreID, c.CreatedDate
                        select c).ToList();
            }
            catch
            {
                return new List<ChangeLog>();
            }
        }
        #endregion


        #region Creat Update Delete




        public int save(ChangeLog _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;
            //Try to retrieve object from DB

            try
            {
                //Insert                  
                context.ChangeLogs.AddObject(_log);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }





        public int delete(ChangeLog _log)
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
            return typeof(ChangeLogHelper).ToString();
        }
        #endregion
    }
}