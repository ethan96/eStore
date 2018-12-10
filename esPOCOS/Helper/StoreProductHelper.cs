using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreProductHelper : Helper { 
        #region Business Read
        public StoreProduct get(int id)
        {
            return context.StoreProducts.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreProducts.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreProduct _storeproduct)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storeproduct == null || _storeproduct.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storeproduct.Id ==0 || !isExists(_storeproduct.Id))  //object not exist 
                {
                    //Insert
                    context.StoreProducts.AddObject( _storeproduct);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreProducts.ApplyCurrentValues( _storeproduct);
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
        public int delete(StoreProduct _storeproduct)
        {
       
            if (_storeproduct == null || _storeproduct.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeproduct);
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