using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    public class DeliveryCategoryAUS : IDeliveryCategory
    {
        public List<string> getFastDeliveryProductIds(POCOS.ProductCategory pc)
        {
            List<string> _fastDeliveryProducts = new List<string>();
            POCOS.Order saporder = new POCOS.Order();
            saporder.StoreID = pc.Storeid;
            saporder.OrderNo = "UCPP000002";
            saporder.UserID = "";

            DateTime now = DateTime.Now;
            DateTime then = new DateTime(2014, 1, 1);
            TimeSpan diff = then - now;
            int days = diff.Days;
            //because UCPP000001 created at 10/1/2012, make sure we can get the order by ws
            SAPOrderTracking SAPOrderTracking = new SAPOrderTracking(saporder, days);

            if (SAPOrderTracking.SAPOrder != null)
            {
                _fastDeliveryProducts = SAPOrderTracking.SAPOrder.getTopLevelItems().Select(x => x.PartNO).ToList();
            }

            return _fastDeliveryProducts;
        }
    }
}
