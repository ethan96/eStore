using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class DHLRateHelper  { 

#region Business Read
   
   

    public static DHLRate getDHLRate(string countryShort, decimal totalweight)
    {
        if (string.IsNullOrEmpty(countryShort)) return null;

        try
        {
            using (eStore3Entities6 context = new eStore3Entities6())
            {
                var _dhlrate = (from dhlr in context.DHLRates 
                                where (dhlr.Shorts == countryShort && (dhlr.StartKG < totalweight && dhlr.EndKG >= totalweight))
                                select dhlr).FirstOrDefault();

                return _dhlrate;
            }
        
        }
        catch (Exception ex)
        {

            eStoreLoger.Fatal(ex.Message, "", "", "", ex);
             
            return null;
        
    }

        
    }

   
   
#endregion 
     
 
#region Others

	private static string myclassname()
        {
           return typeof(StoreHelper).ToString();
       } 
#endregion 
	} 
 }