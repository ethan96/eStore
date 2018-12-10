using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    /// <summary>
    /// This class is a singleton class which provides vendor instance management functions.
    /// Developers should use this management utility for vendor instance management
    /// </summary>
    public class VendorManager
    {
        private static VendorManager _manager = new VendorManager();
        private Hashtable _vendors = new Hashtable();

        /// <summary>
        /// No default constructor. This will prevent developers to directly instantiate this class
        /// </summary>
        private VendorManager()
        {
        }

        public static VendorManager getInstance()
        {
                return _manager;
        }

        /// <summary>
        /// To retrieve vendor by vendor ID
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns>
        ///     Vendor instance if found
        ///     null if not found
        /// </returns>
        public Vendor getVendor(String vendorId)
        {
            Vendor vendor = null;

            if (_vendors[vendorId] != null)    //check vendor existance at eStore cache
                vendor = (Vendor)_vendors[vendorId];    //vendor has already loaded in eStore
            else                
            {
                //retrieve POCO vendor instance first, then use it for eStore vendor construction
                eStore.POCOS.Vendor profile = new VendorHelper().getVendorbyID(vendorId);

                if (profile != null)    //find vendor definition in eStore DB
                {
                    //instantiate eStore vendor instance
                    vendor = new Vendor(profile);

                    //cache vendor instance to hashtable for better performance
                    _vendors.Add(vendorId, vendor);
                }
                else
                {
                    //log warning message here, the only instance this can happen is for OM to register new vendor        
                }
            }

            return vendor;
        }
    }
}
