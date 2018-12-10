using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class CTOSBOMHelper : Helper {

#region Business Read

    public  CTOSBOM  getCTOSBOMbySPID(string sproductid, Store _store)
    {
        if (String.IsNullOrEmpty(sproductid) || _store == null) return null;

        try
        {

            var _carts = (from cm in context.CTOSBOMs
                          where cm.SProductID == sproductid && cm.StoreID==_store.StoreID
                          select cm).FirstOrDefault();
            return _carts;
        }
        catch (Exception ex)
        {

            eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            return null;
        }
    }

    /// <summary>
    /// Get Ctosbom by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CTOSBOM getCtosbomByKey(int id,string sproductid,string storeID) {
        var _ctos = (from c in context.CTOSBOMs
                    where c.ComponentID == id && c.SProductID ==sproductid && c.StoreID == storeID
                    select c).FirstOrDefault();
        return _ctos;
    }

#endregion 

 #region Creat Update Delete 
     public  int save(CTOSBOM _ctosbom) 

     {
          //if parameter is null or validation is false, then return  -1 
      if (_ctosbom == null || _ctosbom.validate() == false) return 1;
          //Try to retrieve object from DB
      CTOSBOM _exist_ctosbom = getCtosbomByKey(_ctosbom.ComponentID ,_ctosbom .SProductID,_ctosbom.StoreID );

   
          try
          {
          if ( _exist_ctosbom == null)  //object not exist 
              {
                  //Insert
                 context.CTOSBOMs.AddObject( _ctosbom);
                 context.SaveChanges();
                 return 0;
             }
             else  
  
             {
                 context = _ctosbom.Product_Ctos.parthelper.getContext();
                 //Update
                context.CTOSBOMs.ApplyCurrentValues( _ctosbom);
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

    public  int delete(CTOSBOM _ctosbom) 
    { 
      if (_ctosbom == null || _ctosbom.validate() == false) return 1;
      try    
      {
          if (_ctosbom.ChildComponents != null && _ctosbom.ChildComponents.Any())
          {
              var ids = _ctosbom.ChildComponents.Select(c => c.ID).ToList();
              foreach (int id in ids) //É¾³ýcbom µÄ option
              {
                  var item = _ctosbom.ChildComponents.FirstOrDefault(c => c.ID == id);
                  item.delete();
              }
          }
          context = _ctosbom.Product_Ctos.parthelper.getContext();
          context.CTOSBOMs.DeleteObject(_ctosbom); 
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
           return typeof(CTOSBOMHelper).ToString();
       } 
#endregion 
	} 
 }