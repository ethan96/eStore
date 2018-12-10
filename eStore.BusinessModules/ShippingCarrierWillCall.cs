using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class ShippingCarrierWillCall
    {
        public string CarrierName
        {
            get
            {
                return CarrierName;
            }
            set
            {
                CarrierName = value;
            }
        }
        
        private POCOS.Store _store;

        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="carrierName"></param>
        /// <param name="store"></param>
        public ShippingCarrierWillCall(eStore.POCOS.Store store)
        {
            _store = store;
        }


        /// <summary>
        /// Get freight estimation
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns></returns>
        public List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            Cart _cart = cart;
            List<ShippingMethod> _willCallShippingMethod = new List<ShippingMethod>();

            try
            {
                // Step 1. Shipping carrier must get the packing list via packing manager
                //PackingManager packingMgr = new PackingManager(_store);
                //PackingList packingList = packingMgr.getPackingList(cart, 0);
                PackingList packingList = new PackingList(cart);
                _willCallShippingMethod.Add(getWillCallrate(packingList));
                return _willCallShippingMethod;
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Shipping Carrier Will Call error", "", "", cart.StoreID, ex);
                return _willCallShippingMethod;
            }

        }

        /// <summary>
        /// Get WillCall shipping method object
        /// </summary>
        /// <param name="packingList"></param>
        /// <returns></returns>
        private ShippingMethod getWillCallrate(PackingList packingList)
        {
            PackingList _packingList = packingList;
            ShippingMethod _willCallShippingMethod;

            _willCallShippingMethod = new ShippingMethod();
            _willCallShippingMethod.PackingList = _packingList;
            _willCallShippingMethod.ShippingCostWithPublishedRate = 0;
            _willCallShippingMethod.ShippingCarrier = "WillCall";
            _willCallShippingMethod.ShippingMethodDescription = "Will Call";
            
            return _willCallShippingMethod;
        }


        #endregion

    }
}
