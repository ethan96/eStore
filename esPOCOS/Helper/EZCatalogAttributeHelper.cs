using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class EZCatalogAttributeHelper : Helper { 
        #region Business Read
        public EZCatalogAttribute get(int id)
        {
            return context.EZCatalogAttributes.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.EZCatalogAttributes.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(EZCatalogAttribute _ezcatalogattribute)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_ezcatalogattribute == null || _ezcatalogattribute.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_ezcatalogattribute.Id ==0 || !isExists(_ezcatalogattribute.Id))  //object not exist 
                {
                    //Insert
                    context.EZCatalogAttributes.AddObject( _ezcatalogattribute);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.EZCatalogAttributes.ApplyCurrentValues( _ezcatalogattribute);
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
        public int delete(EZCatalogAttribute _ezcatalogattribute)
        {
       
            if (_ezcatalogattribute == null || _ezcatalogattribute.validate() == false) return 1;
            try
            {
                context.DeleteObject(_ezcatalogattribute);
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