using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    class ShippingCarrierFixRate : Carrier
    {
        private ICollection<StoreFreightRate> freightRates = null;
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShippingCarrierFixRate(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
            freightRates = store.StoreFreightRates;
        }

        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            Cart _cart = cart;
            Address _shipFromAddress = shipFromAddress;

            // Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            PackingList packingList = packingManager.getPackingList(cart, 0);
            List<ShippingMethod> shippingMethodsFixedRate = new List<ShippingMethod>();
            
            ShippingMethod sm = new ShippingMethod();

            //部分产品免运费 -- 其他计算方式需要更改 boxlist
            //目前免运费产品仅仅单独出售，不会包含到 ctos 配件和bundle 配件中。
            //免运产品价格计算的免运总价的规则中。(新的改动，价格计算到免运总价中)
            decimal AmountFreeFreight = 0m;
            int FreeFreightCount = 0;
            foreach (var item in cart.cartItemsX)
            {
                if ("freightfree".Equals(item.partX.InventoryProvider, StringComparison.OrdinalIgnoreCase))
                {
                    AmountFreeFreight += item.AdjustedPrice;
                    FreeFreightCount++;
                }                    
            }
            var TotalAmount = cart.TotalAmount;

            var fixedrate = (from rate in freightRates
                            where (rate.StartAmount <= TotalAmount) && (TotalAmount < rate.EndAmount)
                            select rate).FirstOrDefault();
              Decimal freight=0;
              if (fixedrate ==null || FreeFreightCount == cart.cartItemsX.Count)
                  freight = 0;
              else
                  freight = fixedrate.Freight;
 
            sm.ShippingCostWithPublishedRate = (float)freight;
            sm.PublishRate = (float)freight;
            sm.PackingList = packingList;
            sm.ShippingCarrier = CarrierName;
            sm.ShippingMethodDescription = _store.StoreID + CarrierName;
            sm.UnitOfCurrency = store.defaultCurrency.CurrencyID;
            sm.ServiceCode = CarrierName;
            
            shippingMethodsFixedRate.Add(sm);

            return shippingMethodsFixedRate;
        }

    }
}
