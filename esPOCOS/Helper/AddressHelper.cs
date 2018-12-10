using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class AddressHelper : Helper { 

    #region Business Read

    public Address getAddressById(int addressid)
    {
        var address = context.Addresses.FirstOrDefault(c => c.AddressID == addressid);
        if (address != null)
            address.helper = this;
        return address;
    }

    #endregion 

    #region Creat Update Delete 
     
    #endregion 
 
    #region Others

	private static string myclassname()
    {
        return typeof(AddressHelper).ToString();
    } 

    #endregion 
	
    public int save(Address address)
    {
        if (address == null || address.validate() == false)
            return 1;
        try
        {
            var exitAddress = getAddressById(address.AddressID);
            if (exitAddress == null)
            {
                context.Addresses.AddObject(address);
                context.SaveChanges();
                return 0;
            }
            else
            {
                context = address.helper.context;
                context.Addresses.ApplyCurrentValues(address);
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

    internal int delete(Address address)
    {
        throw new NotImplementedException();
    }
} 
 }