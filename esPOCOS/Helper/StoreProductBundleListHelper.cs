using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreProductBundleListHelper : Helper { 
        #region Business Read
        public StoreProductBundleList get(int id)
        {
            return context.StoreProductBundleLists.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreProductBundleLists.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreProductBundleList _storeproductbundlelist)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storeproductbundlelist == null || _storeproductbundlelist.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storeproductbundlelist.Id ==0 || !isExists(_storeproductbundlelist.Id))  //object not exist 
                {
                    //Insert
                    context.StoreProductBundleLists.AddObject( _storeproductbundlelist);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreProductBundleLists.ApplyCurrentValues( _storeproductbundlelist);
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
        public int delete(StoreProductBundleList _storeproductbundlelist)
        {
       
            if (_storeproductbundlelist == null || _storeproductbundlelist.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeproductbundlelist);
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
	} 
 }