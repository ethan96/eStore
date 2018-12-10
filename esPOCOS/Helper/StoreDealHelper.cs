using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreDealHelper : Helper { 
        #region Business Read
        public StoreDeal get(int id)
        {
            return context.StoreDeals.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreDeals.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreDeal _storedeal)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storedeal == null || _storedeal.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storedeal.Id ==0 || !isExists(_storedeal.Id))  //object not exist 
                {
                    //Insert
                    context.StoreDeals.AddObject( _storedeal);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreDeals.ApplyCurrentValues( _storedeal);
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
        public int delete(StoreDeal _storedeal)
        {
       
            if (_storedeal == null || _storedeal.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storedeal);
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