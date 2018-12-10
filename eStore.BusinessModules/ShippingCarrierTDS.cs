using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class ShippingCarrierTDS : Carrier
    {
        //In DHL Taiwan contract and rules, it mentioned that measure unit is cm/kg, and price is according to weight(kg).
        //Dimension weight = W(cm) x L(cm) x H(cm) / 5000;
        //Dimensional Weight base is defined 5000 by DHL, measure unit is cm/kg
        //5000 convert to in/lb unit = 138.4
        //6000 convert to in/lb unit = 166.08
        private const decimal _dimensionalWeightBase = 139m;
        //surcharge rate will need to revise each season, it provided by OP devision. Contact with Polar.Yu@advantech.com.tw
        private const decimal _surchargeRate = 1.4m;
        private const decimal _surchargeRate_US = 1.4m;
        //If total boxes weight is less than minChargeWeight, then use minChargeWeight as calculated weight. It provided by OPdevision. 
        public const decimal _minChargeWeight = 2.0m;

        public override string CarrierName
        {
            get
            {
                return base.CarrierName;
            }
            set
            {
                base.CarrierName = value;
            }
        }


        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="carrierName"></param>
        /// <param name="store"></param>
        public ShippingCarrierTDS(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        /// <summary>
        /// Get freight estimation
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns></returns>
        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            Cart _cart = cart;
            
            //TDS is used only for shipment from Taiwan
            StoreHelper helper = new StoreHelper();
            POCOS.Store atwStore = helper.getStorebyStoreid("ATW");
            String shipFromCountry = atwStore.ShipFromAddress.Country;

            decimal dimensionalWeightBase = getDimensionWeightBase(shipFromCountry);

            // Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            packingManager.packingType = PackingManager.PackingType.PackingRule_TDS;
            PackingList packingList = packingManager.getPackingList(cart, _dimensionalWeightBase);

            //Re-write ship from address as ACL 
            packingList.ShipFrom.Address1 = atwStore.ShipFromAddress.Address1;
            packingList.ShipFrom.Address2 = atwStore.ShipFromAddress.Address2;
            packingList.ShipFrom.City = atwStore.ShipFromAddress.City;
            packingList.ShipFrom.ZipCode = atwStore.ShipFromAddress.ZipCode;
            packingList.ShipFrom.State = atwStore.ShipFromAddress.State;
            packingList.ShipFrom.Country = "Taiwan";
            packingList.ShipFrom.countryCodeX = shipFromCountry;

            //ShippingCarrierDHL _dhlTDS = new ShippingCarrierDHL("TDS",_store);       // DS means drop shipment
            List<ShippingMethod> _tdsShippingMethods = new List<ShippingMethod>();

            _tdsShippingMethods.Add(getDHLTDSrate(cart.storeX,packingList));

            Address shipFrom = new Address();
            shipFrom.Country = packingList.ShipFrom.countryCodeX;
            shipFrom.State = packingList.ShipFrom.City;
            shipFrom.ZipCode = packingList.ShipFrom.ZipCode;

            //Save log as XML to file
            saveXMLlog(cart, shipFrom, packingList, _tdsShippingMethods);

            return _tdsShippingMethods;
        }

        /// <summary>
        /// Get TDS shipping method object
        /// </summary>
        /// <param name="packingList"></param>
        /// <returns></returns>
        private ShippingMethod getDHLTDSrate(POCOS.Store store,PackingList packingList)
        {
            PackingList _packingList = packingList;
            ShippingMethod _dhlShippingMethod = new ShippingMethod();
            //_dhlShippingMethod = null;
            decimal _totalPackagesWeight = 0;
            
            //If a packaging box is less then 2.5kg, then set it as 2.5kg
            decimal calWeight = 0;
            foreach (PackagingBox pb in _packingList.PackagingBoxes)
            {
                MeasureUnit measure = pb.Measure;
                measure.Convert(this.MeasureUnitType);
                pb.Measure = measure;   //recalculate measure and reassign the result
                calWeight = Math.Round((pb.WeightUnit == "KGS") ? pb.Weight : MeasureUnit.convertLB2KG(pb.Weight), 2);

                _totalPackagesWeight += calWeight;
            }

            try
            {
                DHLRate _dhlRate;
                Currency _tdsCurrency;
                float dhlRateUS = 0f;

                //if total weight is less or equal to 0, make it to be 0.5 in default
                if (_totalPackagesWeight <= 0)
                    _totalPackagesWeight = 0.5m;

                // Ceiling to fit rate table in db
                if (_totalPackagesWeight >= 30)
                    _totalPackagesWeight = Math.Ceiling(_totalPackagesWeight);

                //_dhlRate = DHLRateHelper.getDHLRate(_packingList.ShipTo.Country, _totalPackagesWeight);
                _dhlRate = DHLRateHelper.getDHLRate(_packingList.ShipTo.countryCodeX, _totalPackagesWeight);
                _dhlShippingMethod.ShippingCarrier = "TDS";
                _dhlShippingMethod.ShippingMethodDescription = "DHL Worldwide Package Express";
                _dhlShippingMethod.PackingList = _packingList;

                //if (_dhlRate == null && string.IsNullOrEmpty(_packingList.ShipTo.Country))
                if (_dhlRate == null && string.IsNullOrEmpty(_packingList.ShipTo.countryCodeX))
                {
                    string errorMsg = "ShipToCountryIsNullOrEmpty";
                    _dhlShippingMethod.Error = getShippingMethodError(errorMsg);
                    throw new Exception(errorMsg);
                }
                else if (_dhlRate == null)
                {
                    string errorMsg = "DHLRateHelper is failed";
                    _dhlShippingMethod.Error = getShippingMethodError(errorMsg);
                    throw new Exception(errorMsg);
                }
                else
                {
                    _tdsCurrency = CurrencyHelper.getCurrencybyID("NTD");
                    _dhlShippingMethod.UnitOfCurrency = store.defaultCurrency.CurrencyID;
                    // If total packages weight is more than 20kg, it returns the unit price each kg.
                    if (_totalPackagesWeight > 30)
                        dhlRateUS = (float)Math.Round((float)(_dhlRate.Price.GetValueOrDefault() * _tdsCurrency.ToUSDRate / store.defaultCurrency.ToUSDRate), 2) * (float)_totalPackagesWeight;
                    else
                        dhlRateUS = (float)Math.Round((float)(_dhlRate.Price.GetValueOrDefault() * _tdsCurrency.ToUSDRate / store.defaultCurrency.ToUSDRate), 2);

                    //DHL net price need to add surcharge and tax
                    if (_packingList.ShipTo.countryCodeX == "US")
                    {
                        dhlRateUS = dhlRateUS * (float)_surchargeRate_US;
                    }
                    else
                    {
                        dhlRateUS = dhlRateUS * (float)_surchargeRate;
                    }


                    //DHL charge price in eStore = DHL contract price * multiplier
                    dhlRateUS *= (float)_dhlRate.Multiplier;
                    dhlRateUS = (float)Math.Round(dhlRateUS, 0);
                    //SAP那边, DHL少一块钱, 现在所有的都默认+1
                    dhlRateUS += 1;
                    _dhlShippingMethod.PublishRate = dhlRateUS;
                    _dhlShippingMethod.ShippingCostWithPublishedRate = dhlRateUS;
                    
                    _dhlShippingMethod.Error = null;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Fail to get TDS rate.", "", "", "", ex);
            }

            return _dhlShippingMethod;
        }

        /// <summary>
        /// Get error object of shipping method
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns>ShippingMethodError</returns>
        private ShippingMethodError getShippingMethodError(string errMsg)
        {
            ShippingMethodError err = new ShippingMethodError();

            if (errMsg.Contains("ShipToCountryIsNullOrEmpty"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToCountryCode;
            }
            else if (errMsg == "DHLRateHelper is failed")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.TDSFailed;
            }
            else
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.UnkonwError;
            }

            return err;
        }

        /// <summary>
        /// This method will get shipping methods log as XML then save to C:\eStoreResources3C\Logs\Shipping.
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <param name="packingList"></param>
        /// <param name="shippingMethod"></param>
        private void saveXMLlog(Cart cart, Address shipFromAddress, PackingList packingList, List<ShippingMethod> shippingMethod)
        {
            //If TDS shipping method is enable, then revise packing list as ShippingMethod.PackingList. 
            //The purpose is in order to get revised weight of package
            packingList = (shippingMethod.Count > 0) ? shippingMethod[0].PackingList : packingList;

            XmlDocument doc = getXMLlog(cart, shipFromAddress, packingList, shippingMethod);

            //Save XML file to C:\eStoreResources3C\Logs\Shipping
            StringBuilder filePath = new StringBuilder();
            string folderName = "TDS";
            DateTime now = DateTime.Now;
            int year = now.Year; 
            int month = now.Month;
            string yearMonth = year.ToString() + "_" + month.ToString();
            filePath.Append(System.Configuration.ConfigurationManager.AppSettings.Get("Log_Path")).Append("/").Append("Shipping").Append("/").Append(yearMonth).Append("/").Append(folderName).Append("/");
            // filename is order number
            string filename = "";
            filename = cart.CartID + ".xml";

            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            {
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }

            //Save
            doc.Save(filePath + filename);
        }
        #endregion
    }
}
