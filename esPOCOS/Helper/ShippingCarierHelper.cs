using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class ShippingCarierHelper { 

#region Business Read
    
   


    public static  ShippingCarier  getShippingCarrier(string shippingcariername){
    
        if (string.IsNullOrEmpty(shippingcariername)) return null;

        try
        {
            using (eStore3Entities6 context = new eStore3Entities6())
            {
                var _store = (from s in context.ShippingCariers.Include("RateServiceNames")
                              where (s.CarierName == shippingcariername)
                              select s).FirstOrDefault();
                return _store;
            }
            
        }
        catch (Exception ex)
        {

            eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            return null;
        }
    
    }


#endregion 

 #region Creat Update Delete 
     public static  int save(ShippingCarier _shippingcarier) 

     {
          //if parameter is null or validation is false, then return  -1 
      if (_shippingcarier == null || _shippingcarier.validate() == false) return 1;
          //Try to retrieve object from DB
          ShippingCarier  _exist_shippingcarier= getShippingCarrier(_shippingcarier.CarierName );
          try
          {
          if ( _exist_shippingcarier == null)  //object not exist 
              {
                  //Insert
                  using (eStore3Entities6 context = new eStore3Entities6())
                  {
                      context.ShippingCariers.AddObject(_shippingcarier);
                      context.SaveChanges();
                      return 0;
                  }
             }
             else  
  
             {
                 //Update
                 using (eStore3Entities6 context = new eStore3Entities6())
                 {
                     context.ShippingCariers.Attach(_exist_shippingcarier);
                     context.ShippingCariers.ApplyCurrentValues(_shippingcarier);
                     context.SaveChanges();
                     return 0;
                 }
           }
      }
       catch (Exception ex)
      {
          eStoreLoger.Fatal(ex.Message, "", "", "", ex);
         return -5000;
     }
   }

     public static int delete(ShippingCarier _shippingcarier) 
{ 
 
      if (_shippingcarier == null || _shippingcarier.validate() == false) return 1;

         try    {
          using (eStore3Entities6 context = new eStore3Entities6())
             {
                context.DeleteObject(_shippingcarier);
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
#endregion 
 
#region Others

	private   static string myclassname()
        {
           return typeof(ShippingCarierHelper).ToString();
       } 
#endregion 
	} 
 }