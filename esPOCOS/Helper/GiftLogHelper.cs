using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class GiftLogHelper : Helper
    {

        internal int save(GiftLog giftLog)
        {
            //if parameter is null or validation is false, then return  -1 
            if (giftLog == null || giftLog.validate() == false) return 1;
            //Try to retrieve object from DB
            GiftLog _exist_module = getGiftLogbyID(giftLog.LogId);
            try
            {
                if (_exist_module == null)  //object not exist 
                {
                    //Insert
                    context.GiftLogs.AddObject(giftLog);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.GiftLogs.ApplyCurrentValues(giftLog);
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

        private GiftLog getGiftLogbyID(Guid guid)
        {
            return context.GiftLogs.FirstOrDefault(c => c.LogId == guid);
        }
        
        public List<GiftLog> getGiftLogByUserid(string userid,string storeid,int advid)
        {
            var ls = (from c in context.GiftLogs
                      where c.UserId == userid && c.Storeid == storeid && c.AdvId == advid
                      select c).ToList();
            foreach (var c in ls)
                c.helper = this;
            return ls;
        }

        internal int delete(GiftLog giftLog) 
        {
            try
            {
                var _exit = getGiftLogbyID(giftLog.LogId);
                if (_exit != null)
                {
                    context.GiftLogs.DeleteObject(_exit);
                    context.SaveChanges();
                    return 0;
                }
                return -1;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
