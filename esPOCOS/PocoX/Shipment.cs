using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// Shipment class includes used shipping method and packing list info. 
    /// When customer confirms an order or  a quotation, Shipment obj should be added to cart.
    /// </summary>
    public class Shipment
    {
        #region Attribute
        // Refer to ShippingMethod class->ShippingCarrier attribute
        private string _shippingCarrier;
        public string ShippingCarrier
        {
            get { return _shippingCarrier; }
        }

        // Refer to ShippingMethod class-> ShippingMethodDescription
        private string _shippingMethod;
        public string ShippingMethod
        {
            get { return _shippingMethod; }
        }

        // Actually adopted shipping rate
        private decimal _shippingRate;
        public decimal ShippingRate
        {
            get { return _shippingRate; }
        }

        private PackingList _packingList = null;
        public PackingList PackingList
        {
            get { return _packingList;}
        }
        #endregion


        #region Method
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="shippingMethod"></param>
        /// <param name="packingList"></param>
        public Shipment(string shippingCarrier, string shippingMethod, decimal shippingRate, PackingList packingList)
        {
            _shippingCarrier = shippingCarrier;
            _shippingMethod = shippingMethod;
            _shippingRate = shippingRate;
            _packingList = packingList;
        }
        #endregion
    }
}
