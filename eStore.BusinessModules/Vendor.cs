using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    /// <summary>
    /// This class will encapsulate POCOS.Vendor in it.
    /// </summary>

    public class Vendor
    {
        private eStore.POCOS.Vendor _profile; 

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="vendor"></param>
        public Vendor(eStore.POCOS.Vendor vendor)
        {
            _profile = vendor;
        }

        public virtual String vendorId
        {
            get { return _profile.VendorId; }
        }

        public virtual String name
        {
            get { return _profile.VendorName; }
            set { _profile.VendorName = value; }
        }
    }
}
