using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class StateZipHelper : Helper
    {
        #region Business Read

        public StateZip getStatebyZip(string country, string zipcode)
        {


            if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(zipcode)) return null;

            try
            {
                var _zip = (from szip in context.StateZips
                            where (szip.Country == country && szip.Zipcode == zipcode)
                            select szip).FirstOrDefault();

                 return _zip;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);

                return null;
            }


        }


        #endregion

        #region Creat Update Delete

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(StoreHelper).ToString();
        }
        #endregion
    }
}