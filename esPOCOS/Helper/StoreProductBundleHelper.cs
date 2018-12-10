using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreProductBundleHelper : Helper { 
        #region Business Read
        public StoreProductBundle get(int id)
        {
            return context.StoreProductBundles.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreProductBundles.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreProductBundle _storeproductbundle)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storeproductbundle == null || _storeproductbundle.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storeproductbundle.Id ==0 || !isExists(_storeproductbundle.Id))  //object not exist 
                {
                    //Insert
                    context.StoreProductBundles.AddObject( _storeproductbundle);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreProductBundles.ApplyCurrentValues( _storeproductbundle);
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
        public int delete(StoreProductBundle _storeproductbundle)
        {
       
            if (_storeproductbundle == null || _storeproductbundle.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeproductbundle);
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