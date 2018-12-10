using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.BusinessModules;

namespace eStore.Presentation
{
    public static class ProductDeliveryBss
    {
        public static ProductDelivery getDeliveryInfor(POCOS.Product.PRODUCTMARKETINGSTATUS status)
        { 
            var delivery = new ProductDelivery(status,eStoreContext.Current.Store.storeID);
            delivery.EndDeliveryTime = Presentation.eStoreContext.Current.Store.getLocalDateTime(delivery.EndDeliveryTime);
            delivery.Message = Presentation.eStoreContext.Current.Store.Tanslation(status.ToString());
            return delivery;
        }
    }

}
