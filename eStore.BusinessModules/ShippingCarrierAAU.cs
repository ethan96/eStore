using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class ShippingCarrierAAU: Carrier
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShippingCarrierAAU(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            List<ShippingMethod> shippingMethods = new List<ShippingMethod>();

            if (cart.ShipToContact.countryCodeX != "AU")
            {
                ShippingMethod _sm = new ShippingMethod();
                ShippingMethodError err = new ShippingMethodError();
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToCountryCode;
                _sm.PackingList = null;
                _sm.Error = err; ;
                _sm.ShippingMethodDescription = "General Error";
                shippingMethods.Add(_sm);
            }
            else
            {

                PackingManager packingManager = new PackingManager(_store);
                PackingList packingList = packingManager.getPackingList(cart, 0);
                StateZip _stateZip = new StateZip();
                _stateZip = new StateZipHelper().getStatebyZip(cart.ShipToContact.countryCodeX, cart.ShipToContact.ZipCode);
                ShippingMethod _aauShippingMethod = new ShippingMethod();
                decimal _totalPackagingBoxesWeight = 0;
                foreach (PackagingBox pb in packingList.PackagingBoxes)
                {
                    MeasureUnit measure = pb.Measure;
                    measure.Convert(this.MeasureUnitType);
                    pb.Measure = measure; 
                    _totalPackagingBoxesWeight += pb.Weight;
                }
                if (_stateZip == null)
                {
                    eStoreLoger.Error("Can not find AAU state " + cart.ShipToContact.State, cart.ShipToContact.Country, cart.ShipToContact.ZipCode);
                    if (_totalPackagingBoxesWeight <= 5)
                    {
                        _aauShippingMethod.PublishRate = 15;
                        _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                    }
                    else
                    {
                        _aauShippingMethod.PublishRate =15+ (float)(_totalPackagingBoxesWeight - 5) * 2.6f;
                        _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                    }
                }
                else
                {
                    string _aauState = _stateZip.State;
                    _aauShippingMethod.ShippingCarrier = CarrierName;
                    _aauShippingMethod.PackingList = packingList;
                    _aauShippingMethod.ShippingMethodDescription = _store.StoreID + "LocalShippingCarrier";

               

                    _aauShippingMethod.UnitOfCurrency = store.defaultCurrency.CurrencyID;

                    switch (_aauState)
                    {
                        case "NSW":
                        case "S.A":
                        case "QLD":
                        case "ACT":
                            if (_totalPackagingBoxesWeight <= 5)
                            {
                                _aauShippingMethod.PublishRate = 15;
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            else
                            {
                                _aauShippingMethod.PublishRate =(float) eStore.Utilities.Converter.CartPriceRound((decimal)(15 + (float)(_totalPackagingBoxesWeight - 5) * 1.2f), store.StoreID);
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            break;

                        case "WA":
                        case "NT":
                        case "TAS":
                            if (_totalPackagingBoxesWeight <= 5)
                            {
                                _aauShippingMethod.PublishRate = 15;
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            else
                            {
                                _aauShippingMethod.PublishRate = (float)eStore.Utilities.Converter.CartPriceRound((decimal)(15 + (float)(_totalPackagingBoxesWeight - 5) * 2.6f), store.StoreID);
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            break;

                        case "VIC":
                            if (_totalPackagingBoxesWeight <= 20)
                            {
                                _aauShippingMethod.PublishRate = 15;
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            else
                            {
                                _aauShippingMethod.PublishRate = 30;
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            break;

                        default:
                            eStoreLoger.Error("Can not find AAU state " + _aauState, cart.ShipToContact.Country, cart.ShipToContact.ZipCode);
                            if (_totalPackagingBoxesWeight <= 5)
                            {
                                _aauShippingMethod.PublishRate = 15;
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            else
                            {
                                _aauShippingMethod.PublishRate = (float)eStore.Utilities.Converter.CartPriceRound((decimal)(15 + (float)(_totalPackagingBoxesWeight - 5) * 2.6f), store.StoreID);
                                _aauShippingMethod.ShippingCostWithPublishedRate = _aauShippingMethod.PublishRate;
                            }
                            break;

                    }
                    shippingMethods.Add(_aauShippingMethod);
                }
               
            }
            return shippingMethods;
        }
    }
}
