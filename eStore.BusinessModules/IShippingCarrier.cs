using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    /// <summary>
    /// IShippingCarrier define standard interface for different ShippingCarrier provider to implement.
    /// In eStore it instantiate multiple ShippingCarrier instances to fulfill various shipping methods
    /// </summary>
    interface IShippingCarrier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxes"></param>
        /// <param name="productToBePacked"></param>
        /// <param name="shipFrom"></param>
        /// <param name="shipTo"></param>
        /// <param name="shippignMethods"></param>
        /// <returns>Postive number to indicate normal return, negative number to indicate funcation call failure</returns>
        Int32 estimateShippingCharge(List<PackagingBox> boxes, List<Product> productToBePacked, ContactInfo shipFrom, ContactInfo shipTo, out List<ShippingMethod> shippignMethods);
    }
}
