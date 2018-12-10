using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;

namespace eStore.BusinessModules.FreightFix
{
    public interface IFreightEx
    {
        void CheckFreight(Cart cart, Address storeAddress, ShippingMethod shippingmethod);
    }
}
