using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    public class DeliveryCategoryAJP : IDeliveryCategory
    {
        public List<string> getFastDeliveryProductIds(POCOS.ProductCategory pc)
        {
            List<string> _fastDeliveryProducts = new List<string>();
            _fastDeliveryProducts.Add("SYS-2U2320-7A82");
            _fastDeliveryProducts.Add("SYS-7W7220-7A82");
            _fastDeliveryProducts.Add("SYS-8W6608-7S27");
            _fastDeliveryProducts.Add("SYS-4U4320-7A82Q");
            _fastDeliveryProducts.Add("SYS-4U4320-7S27Q");
            return _fastDeliveryProducts;
        }
    }
}
