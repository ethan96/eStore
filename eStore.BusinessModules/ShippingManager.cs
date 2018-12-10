using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using System.Threading;
using System.Threading.Tasks;


namespace eStore.BusinessModules
{
    /// <summary>
    /// ShippingManager can help to get shipping methods and freights.
    /// </summary>
#region ShippingManager Class
    public class ShippingManager
    {
        private eStore.POCOS.Store store;
        private Int32 _defaultTimeout = 10000;      // Default time out for multithreading.
        private List<Carrier> _regularCarriers = null;     // A list of available carriers for regular shipment
        private List<Carrier> _dropShipmentCarriers = null;     // A list of available carriers for drop-off shipment

        #region ShippingManager Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShippingManager(eStore.POCOS.Store storeProfile)
        {
            store = storeProfile;
        }

        /// <summary>
        /// If store doesn't have shipping carrier specified, this store will be treated as no shipping service provided.
        /// </summary>
        public Boolean offerShippingService
        {
            get
            {
                if (availableRegularCarriers.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public Boolean onlyLocalFixRate
        {
            get
            {
                if (availableRegularCarriers.Count == 1 && availableRegularCarriers.FirstOrDefault(rc => rc.CarrierName == "LocalFixRate") != null)
                    return true;
                else
                    return false;


            }
        }

        public Boolean offerDropShipmentService
        {
            get
            {
                if (availableDropShipmentCarriers.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// This is a read-only property returning available carrier in current store. 
        /// </summary>
        public IList<Carrier> availableRegularCarriers
        {
            get
            {
                if (_regularCarriers == null)
                    assignStoreCarriers();  //initialize

                return _regularCarriers;
            }
        }

        /// <summary>
        /// This is a read-only property returning available carrier in current store. 
        /// </summary>
        public IList<Carrier> availableDropShipmentCarriers
        {
            get
            {
                if (_dropShipmentCarriers == null)
                    assignStoreCarriers();  //initialize

                return _dropShipmentCarriers;
            }
        }

        /// <summary>
        /// This method is the assign or reassign store carrier setting from DB settings
        /// </summary>
        private void assignStoreCarriers()
        {
            _regularCarriers = new List<Carrier>();
            _dropShipmentCarriers = new List<Carrier>();

            Carrier carrier = null;
            //initialize and build up available carrier list
            foreach (StoreCarier sc in store.StoreCariers)
            {
                carrier = getCarrier(sc.ShippingCarier);
                if (carrier == null)
                    eStoreLoger.Error("Shipping carrier is not supported", sc.CarierName, "", store.StoreID);
                else
                {
                    carrier.priority = sc.Priority;
                    if (carrier.priority > 100) //drop off carrier
                    {
                        if (!_dropShipmentCarriers.Contains(carrier))
                            _dropShipmentCarriers.Add(carrier);
                    }
                    else   //regular carrier
                    {
                        if (!_regularCarriers.Contains(carrier))
                            _regularCarriers.Add(carrier);
                    }
                }
            }
        }

        /// <summary>
        /// Get shipping method thru shipping manager
        /// </summary>
        /// <param name="cart">shipping items</param>
        /// <param name="forDropShipment">Is for drop shipping from different place other than current inventory</param>
        /// <returns>List of ShippingMethod</returns>
        public List<ShippingMethod> getAvailableShippingMethods(Cart cart, Boolean forDropShipment = false)
        {
            List<WaitHandle> waitHandles = new List<WaitHandle>();

            // Check the shopping
            validateCart(cart);

            List<ShippingMethod> shippingMethods = new List<ShippingMethod>();
            DateTime methodStart = DateTime.Now;
            IList<Carrier> carriers = null;
            if (forDropShipment)
                carriers = availableDropShipmentCarriers;
            else
                carriers = availableRegularCarriers;
           
            foreach (Carrier carrier in carriers)
            {
                //get freight estimation from each shipping carrier
                try
                {
                    AutoResetEvent waitHandle = new AutoResetEvent(false);
                    waitHandles.Add(waitHandle);

                    // Muitithreading here
                    TaskInfo taskInfo = new TaskInfo(carrier, cart, store.ShipFromAddress, shippingMethods, waitHandle);
                    ThreadPool.QueueUserWorkItem(this.threadGetFreightEstimation, taskInfo);
                }
                catch (Exception ex)
                {
                    eStoreLoger.Warn("Exception at getAvailableShippingMethod", cart.UserID, carrier.CarrierName, "", ex);
                }
            }

            //If ship to address is in particular region, then allow user to use will call shipping method.
            if (allowUserWillCall(cart))
            {
                ShippingCarrierWillCall _willCallCarrier = null;
                List<ShippingMethod> _willCallShippingMethod = null;

                _willCallCarrier = new ShippingCarrierWillCall(store);
                _willCallShippingMethod = _willCallCarrier.getFreightEstimation(cart, cart.storeX.ShipFromAddress);
                foreach (ShippingMethod method in _willCallShippingMethod)
                    method.carrierPriority = 999;   //least preference

                shippingMethods.AddRange(_willCallShippingMethod);
            }

            if (waitHandles.Count > 0)
                WaitHandle.WaitAll(waitHandles.ToArray<WaitHandle>(), _defaultTimeout);

            if (shippingMethods.Count == 0)
                eStoreLoger.Error("Shipping manager can not get any shipping method !!!");
            else
            {
                float ShippingRateDiscount = 1;
                if (store.Settings.ContainsKey("ShippingRateDiscount"))
                {
                    float.TryParse(store.Settings["ShippingRateDiscount"], out ShippingRateDiscount);
                }

                shippingMethods = (from method in shippingMethods
                                   orderby method.carrierPriority, method.PublishRate
                                   select new ShippingMethod
                                   {
                                       carrierPriority = method.carrierPriority,
                                       Discount = method.Discount,
                                       Error = method.Error,
                                       InsuredCharge = method.InsuredCharge,
                                       NegotiatedRate = method.NegotiatedRate * ShippingRateDiscount,
                                       NegotiatedRateSurcharge = method.NegotiatedRateSurcharge,
                                       PackingList = method.PackingList,
                                       PublishRate = method.PublishRate * ShippingRateDiscount,
                                       PublishRateSurcharge=method.PublishRateSurcharge,
                                       ServiceCode=method.ServiceCode,
                                       ShippingCarrier=method.ShippingCarrier,
                                       ShippingCostWithNegotiatedRate = method.ShippingCostWithNegotiatedRate * ShippingRateDiscount,
                                       ShippingCostWithPublishedRate = method.ShippingCostWithPublishedRate * ShippingRateDiscount,
                                       ShippingMethodDescription=method.ShippingMethodDescription,
                                       UnitOfCurrency=method.UnitOfCurrency,
                                       FreightExType = method.FreightExType
                                   }).ToList<ShippingMethod>(); 
            }

            return shippingMethods;
        }

        /// <summary>
        /// The thread to get freight estmation
        /// </summary>
        /// <param name="info"></param>
        public void threadGetFreightEstimation(Object info)
        {
            TaskInfo taskInfo = (TaskInfo)info;

            try
            {
                List<ShippingMethod> shippingMethodsResult = taskInfo.carrier.getFreightEstimation(taskInfo.cart, taskInfo.shipFrom);

                if (shippingMethodsResult != null)
                {
                    //assign carrier priority preference for presentation purpose
                    foreach (ShippingMethod method in shippingMethodsResult)
                        method.carrierPriority = taskInfo.carrier.priority;

                    //add available shipping method to available shipping method result set
                    lock (taskInfo.shippingMethods) //prevent simultaneous access across different threads
                    {
                        taskInfo.shippingMethods.AddRange(shippingMethodsResult);
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at getting freight estimation", "", "", "", ex);
            }

            taskInfo.taskEvent.Set();   //signal the complete to task execution
        }

        /// <summary>
        /// Get carrier instance by carrier name
        /// </summary>
        /// <param name="carrierName"></param>
        /// <returns>carrier instance</returns>
        private Carrier getCarrier(ShippingCarier shippingCarrier)
        {
            Carrier _carrier;
            String carrierName = shippingCarrier.CarierName;
            switch (carrierName)
            {
                case "UPS_US":
                    _carrier = new ShippingCarrierUPS(store, shippingCarrier);
                    break;
                case "UPS_US_ABB":
                    _carrier = new ShippingCarrierABBUPS(store, shippingCarrier);
                    break;

                case "UPS_EU":
                    _carrier = new ShippingCarrierUPS(store, shippingCarrier);
                    break;

                case "FedEx_US":
                    _carrier = new ShippingCarrierFedEx(store, shippingCarrier);
                    break;

                case "FedEx_US_ABB":
                    _carrier = new ShippingCarrierABBFedEx(store, shippingCarrier);
                    break;

                case "TDS":                             //Taiwan drop ship cost, ship from Taiwan via DHL
                    _carrier = new ShippingCarrierTDS(store, shippingCarrier);
                    break;

                case "LocalFixRate":           //Local carrier, charge fix rate shipping cost
                    _carrier = new ShippingCarrierFixRate(store, shippingCarrier);
                    break;
                
                case "AAUCarier":
                    _carrier = new ShippingCarrierAAU(store, shippingCarrier);
                    break;

                case "AEUUPSCarier": //AEU store ups
                    _carrier = new ShippingCarrierAEUUPS(store, shippingCarrier);
                    break;
                case "USPS":
                    _carrier = new ShippingCarrierUSPS(store, shippingCarrier);
                    break;
                case "USPS_ABB":
                    _carrier = new ShippingCarrierABBUSPS(store, shippingCarrier);
                    break;

                default:
                    _carrier = null;
                    break;
            }
            return _carrier;
        }

        /// <summary>
        /// This method is to validate required fields using in shipping estimation provided in the specifying cart.
        /// It returns true if the cart has all required fields.  Otherwise it return false.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns>boolean</returns>
        public bool validateCart(Cart cart)
        {
            Cart _cart = cart;
            StringBuilder errors = new StringBuilder();

            try
            {
                if (String.IsNullOrEmpty(_cart.storeX.ShipFromAddress.Country) || String.IsNullOrEmpty(_cart.storeX.ShipFromAddress.ZipCode))
                    errors.AppendLine("Incomplete ship from information!");
                
                if (_cart.CartItems.Count == 0)
                    errors.AppendLine("Empty cart!");

                //if (String.IsNullOrEmpty(_cart.ShipToContact.Country) || String.IsNullOrEmpty(_cart.ShipToContact.ZipCode))
                if (String.IsNullOrEmpty(_cart.ShipToContact.countryCodeX) || String.IsNullOrEmpty(_cart.ShipToContact.ZipCode))
                    errors.AppendLine("Incomplete ship to information!");

                //if (String.IsNullOrEmpty(_cart.BillToContact.Country) || String.IsNullOrEmpty(_cart.BillToContact.ZipCode))
                if (String.IsNullOrEmpty(_cart.BillToContact.countryCodeX) || String.IsNullOrEmpty(_cart.BillToContact.ZipCode))
                    errors.AppendLine("Incomplete bill to information!");
            }
            catch (Exception ex)
            {
                errors.AppendLine("Exception at cart validation");
                eStoreLoger.Error(errors.ToString(), cart.UserID, cart.CartID, store.StoreID, ex);
            }

            if (errors.Length == 0) //no error
                return true;
            else
                return false;
        }

        /// <summary>
        /// This function is used to check user can use will call method or not. It's according to user's ship to address or bill to address
        /// Following regions are allowed to use will call shipping method - 
        /// USA, California
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public bool allowUserWillCall(Cart cart)
        {
            string storeId = cart.storeX.StoreID.ToUpper();
            bool allowToWillCall = false;
            if (!string.IsNullOrEmpty(storeId))
            {
                switch (storeId)
                {
                    case "AUS":
                        if (cart.ShipToContact.State.ToUpper() == "CA")
                            allowToWillCall = true;
                        break;

                    default:
                        allowToWillCall = false;
                        break;
                }
            }
            return allowToWillCall;
        }


    }
        #endregion
#endregion


/// <summary>
/// TaskInfo class is used for multithreading
/// </summary>
    #region TaskInfo Class
    public class TaskInfo
    {
        private List<ShippingMethod> _shippingMethods = null;
        private Cart _cart = null;
        private Address _shipFrom = null;
        private AutoResetEvent _event = null;
        private Carrier _carrier;


        public TaskInfo(Carrier carrier, Cart cart, Address shipFrom, List<ShippingMethod> shippingMethods, AutoResetEvent ev)
        {
            _carrier = carrier;
            _cart = cart;
            _shipFrom = shipFrom;
            _shippingMethods = shippingMethods;
            _event = ev;
        }

        public Carrier carrier
        {
            get { return _carrier; }
        }

        public Cart cart
        {
            get { return _cart; }
        }

        public Address shipFrom
        {
            get { return _shipFrom; }
        }

        public List<ShippingMethod> shippingMethods
        {
            get {return _shippingMethods;}
        }

        public AutoResetEvent taskEvent
        {
            get { return _event;}
        }
        

    }
#endregion
}