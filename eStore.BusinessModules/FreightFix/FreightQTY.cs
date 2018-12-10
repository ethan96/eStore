using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;

namespace eStore.BusinessModules.FreightFix
{
    public class FreightQTY : IFreightEx
    {
        public void CheckFreight(Cart cart, Address storeAddress, ShippingMethod shippingmethod)
        {
            var t = cart.storeX.translationX.FirstOrDefault(c => c.Key.Equals("eStore_FreightEx_Message", StringComparison.OrdinalIgnoreCase));
            if (t != null && !string.IsNullOrEmpty(t.Value))
            {
                bool isSuccess = false; // have aty = 0
                foreach (var item in cart.cartItemsX)
                {
                    if (item.BTOSystem != null)
                        isSuccess = item.BTOSystem.BTOSConfigsWithoutNoneItems.Any(c => (c.atp.availableQty <= 0 || c.atp.availableQty < item.Qty)
                        && (c.atp.availableDate.Date > DateTime.Now.Date)
                        && string.IsNullOrEmpty(c.atp.Message));
                    else
                        isSuccess = ((item.atp.availableQty <= 0 || item.atp.availableQty < item.Qty)
                            &&(item.atp.availableDate.Date>DateTime.Now.Date)
                            && string.IsNullOrEmpty(item.atp.Message));
                    if (isSuccess)
                    {
                        //"One or more of the items in your cart are on back order and are not available to ship priority.  Please call our toll free AOnline Inside Sales team at 888-576-9668 ext 183 for additional help";
                        shippingmethod.AlertMessage = string.Format(t.Value, storeAddress.Tel);
                        return;
                    }
                }
            }       
        }
    }
}
