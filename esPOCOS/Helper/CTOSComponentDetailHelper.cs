using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class CTOSComponentDetailHelper : Helper {

#region Business Read

    public CTOSComponentDetail getbyID(int ccid, string sproductid,string storeid) {
        var _cc = (from c in context.CTOSComponentDetails
                   where c.ComponentID == ccid && c.SProductID == sproductid && c.StoreID == storeid
                   select c).FirstOrDefault();
        return _cc;
    }

#endregion 


 #region Creat Update Delete 
     public  int save(CTOSComponentDetail _ctoscomponentdetail) 
        
     {
          //if parameter is null or validation is false, then return  -1 
      if (_ctoscomponentdetail == null || _ctoscomponentdetail.validate() == false) return 1;
          //Try to retrieve object from DB
      CTOSComponentDetail _exist_ctoscomponentdetail = getbyID(_ctoscomponentdetail.ComponentID , _ctoscomponentdetail.SProductID,_ctoscomponentdetail.StoreID);
          try
          {
          if ( _exist_ctoscomponentdetail == null)  //object not exist 
              {
                   //Insert
                context.CTOSComponentDetails.AddObject( _ctoscomponentdetail);
                 context.SaveChanges();
                 return 0;
             }
             else  
  
             {
                 //Update
                 context.CTOSComponentDetails.ApplyCurrentValues( _ctoscomponentdetail);
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

     public   int delete(CTOSComponentDetail _ctoscomponentdetail) 
{
         if(_ctoscomponentdetail.helper!=null)
            context = _ctoscomponentdetail.helper.context;

         if (_ctoscomponentdetail == null || _ctoscomponentdetail.validate() == false) return 1;
  try    {

      CTOSComponentDetail ccd = getbyID(_ctoscomponentdetail.ComponentID, _ctoscomponentdetail.SProductID, _ctoscomponentdetail.StoreID);
            context.DeleteObject(ccd); 
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
           return typeof(CTOSComponentDetailHelper).ToString();
       } 
#endregion 
	} 
 }