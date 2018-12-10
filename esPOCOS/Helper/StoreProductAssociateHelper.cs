using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class StoreProductAssociateHelper : Helper { 
        #region Business Read
        public StoreProductAssociate get(int id)
        {
            return context.StoreProductAssociates.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.StoreProductAssociates.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(StoreProductAssociate _storeproductassociate)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_storeproductassociate == null || _storeproductassociate.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_storeproductassociate.Id ==0 || !isExists(_storeproductassociate.Id))  //object not exist 
                {
                    //Insert
                    context.StoreProductAssociates.AddObject( _storeproductassociate);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.StoreProductAssociates.ApplyCurrentValues( _storeproductassociate);
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
        public int delete(StoreProductAssociate _storeproductassociate)
        {
       
            if (_storeproductassociate == null || _storeproductassociate.validate() == false) return 1;
            try
            {
                context.DeleteObject(_storeproductassociate);
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