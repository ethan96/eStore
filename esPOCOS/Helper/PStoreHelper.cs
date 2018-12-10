using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class PStoreHelper : Helper { 
        #region Business Read
        public PStore get(int id)
        {
            return context.PStores.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.PStores.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(PStore _pstore)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_pstore == null || _pstore.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_pstore.Id ==0 || !isExists(_pstore.Id))  //object not exist 
                {
                    //Insert
                    context.PStores.AddObject(_pstore);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PStores.ApplyCurrentValues(_pstore);
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
        public int delete(PStore _pstore)
        {
       
            if (_pstore == null || _pstore.validate() == false) return 1;
            try
            {
                context.DeleteObject(_pstore);
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

        public PStore getPStoreByStore(POCOS.Store store)
        {
            if (store == null)
                return null;
            PStore pstore = context.PStores.FirstOrDefault(x => x.Name == store.StoreID);
            if (pstore != null)
                pstore.storeX = store;
            return pstore;
        }
	} 
 }