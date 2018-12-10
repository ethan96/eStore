using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class BTOSConfigHelper : Helper
    {

        #region Business Read


        #endregion

        #region Creat Update Delete
        public   int save(BTOSConfig _btosconfig)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_btosconfig == null || _btosconfig.validate() == false) return 1;
            //Try to retrieve object from DB
            BTOSConfig _exist_btosconfig = null;
            try
            {
                if (_exist_btosconfig == null)  //object not exist 
                {
                    //Insert
                    context.BTOSConfigs.AddObject(_btosconfig);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.BTOSConfigs.ApplyCurrentValues(_btosconfig);
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

        public   int delete(BTOSConfig _btosconfig)
        {

            if (_btosconfig == null || _btosconfig.validate() == false) return 1;
            try
            {
                context.DeleteObject(_btosconfig);
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
            return typeof(BTOSConfigHelper).ToString();
        }
        #endregion
    }
}