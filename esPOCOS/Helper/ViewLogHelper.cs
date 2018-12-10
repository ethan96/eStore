using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ViewLogHelper : Helper
    {
        #region Creat Update Delete
        public int save(ViewLog  _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;
            //Try to retrieve object from DB

         
            try
            {
                
                    //Insert                  
                    context.ViewLogs .AddObject(_log);
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