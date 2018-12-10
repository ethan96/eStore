using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class PackingListHelper : Helper { 

#region Business Read
#endregion 

 #region Creat Update Delete 
     public   int save(PackingList _packinglist) 

     {
          //if parameter is null or validation is false, then return  -1 
      if (_packinglist == null || _packinglist.validate() == false) return 1;
          //Try to retrieve object from DB
          PackingList  _exist_packinglist=null;
          try
          {
          if ( _exist_packinglist == null)  //object not exist 
              {
                  //Insert
               //  context.PackingLists.AddObject( _packinglist);
                 context.SaveChanges();
                 return 0;
             }
             else  
  
             {
                 //Update
               // context.PackingLists.ApplyCurrentValues( _packinglist);
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

     public   int delete(PackingList _packinglist) 
{ 
 
      if (_packinglist == null || _packinglist.validate() == false) return 1;
  try    {  
           context.DeleteObject(_packinglist); 
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
           return typeof(PackingListHelper).ToString();
       } 
#endregion 
	} 
 }