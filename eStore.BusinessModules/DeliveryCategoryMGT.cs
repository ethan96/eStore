using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    public class DeliveryCategoryMGT
    {
        public IDeliveryCategory Get(string site)
        {
            IDeliveryCategory dc;
            try
            {
                dc = (IDeliveryCategory)Activator.CreateInstance(Type.GetType(string.Format("eStore.BusinessModules.DeliveryCategory{0}", site)));
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error("DeliveryCategoryMGT " + site, "", "", "", ex);
                dc = null;
            }
            return dc;
        }
        
    }
}
