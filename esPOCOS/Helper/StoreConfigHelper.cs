using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class StoreConfigHelper : Helper
    {
        #region Business Read

        public StoreConfig getStoreConfigbyKey(string key)
        {
            var _sc = (from sc in context.StoreConfigs
                       where sc.aKey == key
                       select sc).FirstOrDefault();

            return _sc;

        }

        #endregion

        #region Creat Update Delete
        public int save(StoreConfig _storeconfig)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_storeconfig == null || _storeconfig.validate() == false) return 1;
            //Try to retrieve object from DB
            StoreConfig _exist_storeconfig = getStoreConfigbyKey(_storeconfig.aKey);
            try
            {
                if (_exist_storeconfig == null)  //object not exist 
                {
                    //Insert
                    context.StoreConfigs.AddObject(_storeconfig);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreConfigs.ApplyCurrentValues(_storeconfig);
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

        public int delete(StoreConfig _storeconfig)
        {

            if (_storeconfig == null || _storeconfig.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeconfig);
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
            return typeof(StoreConfigHelper).ToString();
        }
        #endregion
    }
}