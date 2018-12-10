using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class VendorHelper : Helper
    {
        #region Business Read
        public Vendor getVendorbyID(string vendorid)
        {
            if (String.IsNullOrEmpty(vendorid)) return null;

            try
            {

                var _vendor = (from vd in context.Vendors
                               where (vd.VendorId == vendorid)
                               select vd).FirstOrDefault();
                return _vendor;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public Vendor getVendorbyVendorEmail(String vendoremail)
        {
            if (string.IsNullOrEmpty(vendoremail)) return null;

            try
            {

                var _vendor = (from vd in context.Vendors
                               where (vd.VendorAccountEmail == vendoremail)
                               select vd).FirstOrDefault();
                return _vendor;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        #endregion

        #region Creat Update Delete
        public int save(Vendor _vendor)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_vendor == null || _vendor.validate() == false) return 1;
            //Try to retrieve object from DB
            Vendor _exist_vendor = null;
            try
            {
                if (_exist_vendor == null)  //object not exist 
                {
                    //Insert
                    //  context.Vendors.AddObject( _vendor);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    // context.Vendors.ApplyCurrentValues( _vendor);
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

        public int delete(Vendor _vendor)
        {

            if (_vendor == null || _vendor.validate() == false) return 1;
            try
            {
                context.DeleteObject(_vendor);
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
            return typeof(VendorHelper).ToString();
        }
        #endregion
    }
}