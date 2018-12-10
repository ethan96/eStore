using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class CountryAddressHelper : Helper 
    {
        internal int delete(CountryAddress countryAddress)
        {
            if (countryAddress == null)
                return 1;
            try
            {
                var _exitobj = context.CountryAddresses.FirstOrDefault(c => c.Division == countryAddress.Division
                        && c.CountryName == countryAddress.CountryName && c.AddressID == countryAddress.AddressID);
                if (_exitobj != null)
                {
                    context.DeleteObject(_exitobj);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return 1;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }            
        }
    }
}
