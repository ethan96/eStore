using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    public interface IDeliveryCategory
    {
        List<string> getFastDeliveryProductIds(POCOS.ProductCategory pc);
    }

    public class DeliveryCategoryOther : IDeliveryCategory
    {
        public List<string> getFastDeliveryProductIds(POCOS.ProductCategory pc)
        {
            return null;
        }
    }
}
