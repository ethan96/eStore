using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    partial class ShippingCarrierDHL:Carrier
    {
        public ShippingCarrierDHL(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            throw new NotImplementedException();
        }
    }
}
