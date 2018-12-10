using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TrackingLogHelper : Helper
    {
        /// <summary>
        /// Return TrackingLogs.
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TrackingLog> getLogs(string storeid, TrackingLog.TrackType type, string trackingno ) { 
            string tracktype = type.ToString();
            
            var _log = (from l in context.TrackingLogs 
                   where l.StoreID == storeid && l.TrackingType == tracktype 
                        &&  l.TrackingNo == trackingno
                        orderby l.LastUpdated descending
                   select l);

            if (_log != null)
                return _log.ToList();
            else
                return new List<TrackingLog>();
        
        }


        public List<TrackingLog> getLogs(TrackingLog.TrackType type, string trackingno)
        {
            string tracktype = type.ToString();

            var _log = (from l in context.TrackingLogs
                        where l.TrackingType == tracktype
                             && l.TrackingNo == trackingno
                        orderby l.LastUpdated descending
                        select l);

            if (_log != null)
                return _log.ToList();
            else
                return new List<TrackingLog>();

        }
 
        
        #region Creat Update Delete
        public int save(TrackingLog   _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;
            //Try to retrieve object from DB
                     
            try
            {               
                    //Insert                  
                    context.TrackingLogs.AddObject(_log);
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
            return typeof(ViewLogHelper).ToString();
        }
        #endregion
    }
}