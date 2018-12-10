using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class StoreConfigKeyHelper : Helper
    {
        #region Business Read

        #endregion

        #region Creat Update Delete
        public int save(StoreConfigKey _storeconfigkey)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_storeconfigkey == null || _storeconfigkey.validate() == false) return 1;
            //Try to retrieve object from DB
            StoreConfigKey _exist_storeconfigkey = null;
            try
            {
                if (_exist_storeconfigkey == null)  //object not exist 
                {
                    //Insert
                    context.StoreConfigKeys.AddObject( _storeconfigkey);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreConfigKeys.ApplyCurrentValues( _storeconfigkey);
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

        public   int delete(StoreConfigKey _storeconfigkey)
        {

            if (_storeconfigkey == null || _storeconfigkey.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeconfigkey);
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
            return typeof(StoreConfigKeyHelper).ToString();
        }
        #endregion
    }
}