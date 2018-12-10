using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    /// <summary>
    /// Carrier is the parent class of all ShippingCarrier
    /// </summary>
    public abstract class Carrier
    {
        #region Properties and Fields
        protected string _carrierName;
        protected eStore.POCOS.Store _store = null;
        protected ShippingCarier _shippingCarrier = null;

        /// <summary>
        /// This property will be initialized in constructor
        /// </summary>
        protected virtual String dimensionUnit
        {
            get;
            set;
        }

        /// <summary>
        /// This property will be initialized in constructor
        /// </summary>
        protected virtual String weightUnit
        {
            set;
            get;
        }

        private MeasureUnit.UnitType _measureUnitType = MeasureUnit.UnitType.IMPERIAL;
        public MeasureUnit.UnitType MeasureUnitType
        {
            get
            {
                return _measureUnitType;
            }
            set
            {
                _measureUnitType = value;
            }
        }

        /// <summary>
        /// This property will be initialized in constructor
        /// </summary>
        protected virtual String currencyCode
        {
            get;
            set;
        }

        public virtual string CarrierName
        {
            get { return _carrierName; }
            set { _carrierName = value; }
        }

        //Carrier web service timeout, time unit - milliseconds
        protected int webServiceTimeout = 20000;    //12 seconds
        public virtual int WebServiceTimeout
        {
            get { return webServiceTimeout; }
        }

        /// <summary>
        /// This is a run time property from StoreCarrier setting.  It indicates the preferernce of this carrier to a store.
        /// </summary>
        public int priority
        {
            get;
            set;
        }
    
        protected ShippingCarier shippingCarrier
        {
            get { return _shippingCarrier; }
        }

        protected eStore.POCOS.Store store
        {
            get { return _store; }
        }

        protected virtual decimal getDimensionWeightBase(String shipToCountry)
        {
            return 0;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name=""></param>
        public Carrier(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
        {
            _store = store;
            _shippingCarrier = shippingCarrier;
            _carrierName = _shippingCarrier.CarierName;

            if (shippingCarrier.Measurement.Equals("METRICS"))
            {
                dimensionUnit = "CM";
                weightUnit = "KGS";
                MeasureUnitType = MeasureUnit.UnitType.METRICS;
            }
            else
            {
                dimensionUnit = "IN";
                weightUnit = "LBS";
                MeasureUnitType = MeasureUnit.UnitType.IMPERIAL;
            }

            currencyCode = store.defaultCurrency.CurrencyID;
        }


        /// <summary>
        /// Shipping charge estimation, implement in all available carriers
        /// </summary>
        /// <param name="cart"></param>
        /// <returns>A list of available shipping method and its freight</returns>
        public abstract List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress);

        /// <summary>
        /// This method will save shipping request/reponse content to XML file, and save to C:\eStoreResource3C\Shipping.
        /// Each log should contain po no( or quote no),  package box dimension, weight, cart items, ship from, ship to, create date, response result(public price, nego. price) 
        /// </summary>
        protected XmlDocument getXMLlog(Cart cart, Address shipFromAddress, PackingList packingList, List<ShippingMethod> shippingMethod)
        {
            XmlDocument doc = new XmlDocument();
            //Root element
            XmlElement elementRoot = doc.CreateElement("ShippingLog");
            doc.AppendChild(elementRoot);

            string _cartID = (string.IsNullOrEmpty(cart.CartID)) ? "?" : cart.CartID;
            XmlElement cartElement = doc.CreateElement("Cart");
            cartElement.SetAttribute("Cart", _cartID);
            cartElement.SetAttribute("Creater", cart.UserID);
            cartElement.SetAttribute("CreatedDate", cart.CreatedDate.ToString());
            elementRoot.AppendChild(cartElement);

            string _storeID = (string.IsNullOrEmpty(cart.StoreID)) ? "?" : cart.StoreID;
            XmlElement storeElement = doc.CreateElement("Store");
            storeElement.SetAttribute("StoreID", _storeID);
            elementRoot.AppendChild(storeElement);

            string _shipFromCountry = (string.IsNullOrEmpty(shipFromAddress.Country) ? "" : shipFromAddress.Country);
            string _shipFromState = (string.IsNullOrEmpty(shipFromAddress.State) ? "" : shipFromAddress.State);
            string _shipFromZipCode = (string.IsNullOrEmpty(shipFromAddress.ZipCode) ? "" : shipFromAddress.ZipCode);
            XmlElement shipFromAddr = doc.CreateElement("ShipFromAddress");
            shipFromAddr.SetAttribute("Country", _shipFromCountry);
            shipFromAddr.SetAttribute("State", _shipFromState);
            shipFromAddr.SetAttribute("ZipCode", _shipFromZipCode);
            elementRoot.AppendChild(shipFromAddr);

            string _shipToCountry = (string.IsNullOrEmpty(cart.ShipToContact.countryCodeX)) ? "" : cart.ShipToContact.countryCodeX;
            string _shipToState = (string.IsNullOrEmpty(cart.ShipToContact.State)) ? "" : cart.ShipToContact.State;
            string _shipToZipCode = (string.IsNullOrEmpty(cart.ShipToContact.ZipCode)) ? "" : cart.ShipToContact.ZipCode;
            XmlElement shipToAddr = doc.CreateElement("ShipToAddress");
            shipToAddr.SetAttribute("Country", _shipToCountry);
            shipToAddr.SetAttribute("State", _shipToState);
            shipToAddr.SetAttribute("ZipCode", _shipToZipCode);
            elementRoot.AppendChild(shipToAddr);

            //Packaging boxes
            XmlElement packages = doc.CreateElement("PackagesList");
            int boxCount = 1;
            decimal totalWeight = 0m;
            decimal TDSAdjustedTotalWeight = 0m;
            foreach (PackagingBox pb in packingList.PackagingBoxes)
            {
                XmlElement box = null;
                box = doc.CreateElement("Box" + boxCount++);
                string _length = pb.Length.ToString();
                string _width = pb.Width.ToString();
                string _height = pb.Height.ToString();
                string _lengthUnit = pb.DimensionUnit;
                string _weight = pb.Weight.ToString();
                string _weightUnit = pb.WeightUnit;

                XmlElement length = doc.CreateElement("Length");
                length.InnerText = _length;
                box.AppendChild(length);
                XmlElement width = doc.CreateElement("Width");
                width.InnerText = _width;
                box.AppendChild(width);
                XmlElement height = doc.CreateElement("Height");
                height.InnerText = _height;
                box.AppendChild(height);
                XmlElement lengthUnit = doc.CreateElement("LengthUnit");
                lengthUnit.InnerText = _lengthUnit;
                box.AppendChild(lengthUnit);
                XmlElement weight = doc.CreateElement("Weight");
                weight.InnerText = _weight;
                if (pb.Weight < ShippingCarrierTDS._minChargeWeight)
                {
                    weight.SetAttribute("TDSAdjustedWeight", ShippingCarrierTDS._minChargeWeight.ToString());
                    TDSAdjustedTotalWeight += ShippingCarrierTDS._minChargeWeight;
                }
                else 
                {
                    TDSAdjustedTotalWeight += pb.Weight;
                }
                box.AppendChild(weight);
                totalWeight += pb.Weight;
                XmlElement weightUnit = doc.CreateElement("WeightUnit");
                weightUnit.InnerText = _weightUnit;
                box.AppendChild(weightUnit);

                //Append box to packages(PackagesList)
                packages.AppendChild(box);
            }
            packages.SetAttribute("CountBoxes", (boxCount-1).ToString());

            elementRoot.AppendChild(packages);

            if (shippingMethod.Count > 0)
            {
                XmlElement shippingMethods = doc.CreateElement("ShippingMethods");
                foreach (ShippingMethod sm in shippingMethod)
                {
                    XmlElement method = doc.CreateElement("Method");
                    method.SetAttribute("Carrier", sm.ShippingCarrier);
                    method.SetAttribute("Description", sm.ShippingMethodDescription);
                    XmlElement PublishRate = doc.CreateElement("PublishRate");
                    PublishRate.SetAttribute("Currency", sm.UnitOfCurrency);
                    PublishRate.InnerText = sm.ShippingCostWithPublishedRate.ToString();
                    method.AppendChild(PublishRate);
                    XmlElement NegotiateRate = doc.CreateElement("NegotiatedRate");
                    NegotiateRate.SetAttribute("Currency", sm.UnitOfCurrency);
                    NegotiateRate.InnerText = (sm.ShippingCostWithNegotiatedRate > 0) ? sm.ShippingCostWithNegotiatedRate.ToString() : "0";
                    method.AppendChild(NegotiateRate);
                    shippingMethods.AppendChild(method);

                    //Add TDS adjusted total weight to packages list node
                    if (sm.ShippingCarrier == "TDS")
                        packages.SetAttribute("TDSAdjuestedTotalWeight", TDSAdjustedTotalWeight.ToString());

                }
                elementRoot.AppendChild(shippingMethods);
            }

            return doc;
        }

        /// <summary>
        /// Edward filter out those shipping methods which are not supported in the ship to country
        /// </summary>
        /// <param name="shiptocountry"></param>
        /// <param name="shipmethods"></param>

        protected List<ShippingMethod> filterShippingMethod(string shiptocountry, List<ShippingMethod> shipmethods)
        {

            StoreHelper helper = new StoreHelper();
            List<RateServiceName> rates = helper.getSupportedShipping(shiptocountry, _store.StoreID);

            List<ShippingMethod> newshipmethods = new List<ShippingMethod>();

             foreach (RateServiceName rs in rates) {

             foreach (ShippingMethod sm in shipmethods.ToList()) {
                    if (sm.ServiceCode.ToUpper().Equals(rs.MessageCode.ToUpper()))
                    {
                        sm.FreightExType = rs.FreigthExType;
                        newshipmethods.Add(sm);
                    }
                }
            }
             return newshipmethods;
        }

        protected void fixShippingMethod(List<ShippingMethod> shipmethods, string shipType, CartContact packlist)
        {
            if (packlist.CountryCode.ToUpper() == "CA")
            {
                string[] upsFilter = new string[]{};
                string shipCode = "";
                if (shipType == "UPS")
                {
                    upsFilter = new string[] { "Y", "X" };
                    shipCode = "03";
                }
                else if (shipType.ToUpper() == "FEDEX")
                {
                    upsFilter = new string[] { "D", "F", "O", "Q", "U", "V", "W", "Z", "G5U", "J9D", "J9F", "K0I", "K7U", "K8I", "T0D", "T8O", "T9F" };
                    shipCode = "FEDEX_GROUND";
                }
                removeCannotToShippingMethod(shipmethods, upsFilter, shipCode, packlist.ZipCode);
            }
        }

        protected void removeCannotToShippingMethod(List<ShippingMethod> shipmethods, string[] ears, string shipCode,string zipCode)
        {
            if (shipmethods == null || shipmethods.Count <= 0 || ears == null || ears.Length <= 0 || string.IsNullOrEmpty(shipCode) || string.IsNullOrEmpty(zipCode))
                return;
            else
            {
                if (ears.FirstOrDefault(x => zipCode.ToUpper().StartsWith(x)) != null)
                {
                    ShippingMethod sp = shipmethods.FirstOrDefault(m => m.ServiceCode == shipCode);
                    if (sp != null)
                        shipmethods.Remove(sp);
                }
            }
        }

        #endregion
    }
}
