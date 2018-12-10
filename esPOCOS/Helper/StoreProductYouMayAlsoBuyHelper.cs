using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreProductYouMayAlsoBuyHelper : Helper { 
        #region Business Read
        public StoreProductYouMayAlsoBuy get(int id)
        {
            return context.StoreProductYouMayAlsoBuys.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreProductYouMayAlsoBuys.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreProductYouMayAlsoBuy _storeproductyoumayalsobuy)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storeproductyoumayalsobuy == null || _storeproductyoumayalsobuy.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storeproductyoumayalsobuy.Id ==0 || !isExists(_storeproductyoumayalsobuy.Id))  //object not exist 
                {
                    //Insert
                    context.StoreProductYouMayAlsoBuys.AddObject( _storeproductyoumayalsobuy);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreProductYouMayAlsoBuys.ApplyCurrentValues( _storeproductyoumayalsobuy);
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
        public int delete(StoreProductYouMayAlsoBuy _storeproductyoumayalsobuy)
        {
       
            if (_storeproductyoumayalsobuy == null || _storeproductyoumayalsobuy.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeproductyoumayalsobuy);
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