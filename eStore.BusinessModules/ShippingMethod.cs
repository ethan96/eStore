using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace eStore.BusinessModules
{
    /// <summary>
    /// Shipping methods presents a shipping way including shipping carrier,  
    /// the description of shipping carrier, freight of the shipment and provide packing list. 
    /// </summary>
    public class ShippingMethod
    {
        #region Attributes
        //Carrier company
        private string shippingCarrier = "";
        public string ShippingCarrier
        {
            get
            {
                return shippingCarrier;
            }
            set
            {
                shippingCarrier = value;
            }
        }

        //Service code
        private string serviceCode = "";
        public string ServiceCode
        {
            get
            {
                return serviceCode;
            }
            set
            {
                serviceCode = value;
            }
        }

        //Short description of the shipping method by particular carrier
        private string shippingMethodDescription = "";
        public string ShippingMethodDescription
        {
            get
            {
                return shippingMethodDescription;
            }
            set
            {
                shippingMethodDescription = value;
            }
        }

        private string _SpecialShippingMethodDescription = "";
        public string SpecialShippingMethodDescription
        {
            get 
            {
                if (ShippingMethodDescription == "TNT Express (Delivery time: 1-2 days)")
                    _SpecialShippingMethodDescription = "TNT Global";
                else if (ShippingMethodDescription == "TNT Economy (Delivery time: 2-5 days)")
                    _SpecialShippingMethodDescription = "TNT Economy";
                else
                    _SpecialShippingMethodDescription = ShippingMethodDescription;
                return _SpecialShippingMethodDescription; 
            }
            set 
            { 
                _SpecialShippingMethodDescription = value; 
            }
        }
        

        /// <summary>
        /// This runtime property is to indicate the preference priority of this shipping method carrier
        /// </summary>
        public int carrierPriority
        {
            get;
            set;
        }

        //Insured charge
        private float insuredCharge = 0f;
        public float InsuredCharge
        {
            get
            {
                return insuredCharge;
            }
            set
            {
                insuredCharge = value;
            }
        }

        //Publish rate
        private float publishRate = 0f;
        public float PublishRate
        {
            get
            {
                return publishRate;
            }
            set
            {
                publishRate = value;
            }
        }

        //Negotiated rate
        private float negotiatedRate = 0f;
        public float NegotiatedRate
        {
            get
            {
                return negotiatedRate;
            }
            set
            {
                negotiatedRate = value;
            }
        }

        //Discount
        private float discount = 0f;
        public float Discount
        {
            get
            {
                return discount;
            }
            set
            {
                discount = value;
            }
        }

        // Published rate surcharge
        private float publishRateSurcharge = 0f;
        public float PublishRateSurcharge
        {
            get
            {
                return publishRateSurcharge;
            }
            set
            {
                publishRateSurcharge = value;
            }
        }

        // Discount rate surcharge
        private float negotiatedRateSurcharge = 0f;
        public float NegotiatedRateSurcharge
        {
            get
            {
                return negotiatedRateSurcharge;
            }
            set
            {
                negotiatedRateSurcharge = value;
            }
        }

        //Total shipping cost with published rate
        private float shippingCostWithPublishedRate = 0f;
        public float ShippingCostWithPublishedRate
        {
            get
            {
                return shippingCostWithPublishedRate;
            }
            set
            {
                shippingCostWithPublishedRate = value;
            }
        }

        //Total shipping cose wieh negotiated rate
        private float shippingCostWithNegotiatedRate = 0f;
        public float ShippingCostWithNegotiatedRate
        {
            get
            {
                return shippingCostWithNegotiatedRate;
            }
            set
            {
                shippingCostWithNegotiatedRate = value;
            }
        }

        //Currency unit of freight
        private string unitOfCurrency;
        public string UnitOfCurrency
        {
            get
            {
                return unitOfCurrency;
            }
            set
            {
                unitOfCurrency = value;
            }
        }
        
        //Error code
        private ShippingMethodError error;
        public ShippingMethodError Error
        {
            get { return error; }
            set { error = value; }
        }
        


        private eStore.POCOS.PackingList packingList;
        public eStore.POCOS.PackingList PackingList
        {
            get
            {
                return packingList;
            }
            set
            {
                packingList = value;
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public ShippingMethod()
        { }
        #endregion

        public POCOS.Address StoreAddress { get; set; }

        public string FreightExType { get; set; }
        
        public string AlertMessage { get; set; }

        public void CheckFreight(POCOS.Cart cart)
        {
            if (!string.IsNullOrEmpty(FreightExType))
            {
                try
                {
                    var ex = (FreightFix.IFreightEx)Assembly.Load("eStore.BusinessModules")
                                    .CreateInstance("eStore.BusinessModules.FreightFix."+ FreightExType); //FreightQTY
                    ex.CheckFreight(cart, StoreAddress, this);
                }
                catch (Exception ex)
                {
                    
                }
            }
            
        }


    }

    
    /// <summary>
    /// Describe error in shipping method 
    /// </summary>
    public class ShippingMethodError
    {
        /// <summary>
        /// If error level is System, then return error item to presentation layer
        /// If error level is EndUser, then error item/msg will return to end user
        /// </summary>
        public enum Type { System, EndUser };

        private Type _errorLevelType;
        public Type ErrorLevelType
        {
            get { return _errorLevelType; }
            set { _errorLevelType = value; }
        }

        public enum ErrorCode
        {
            //WebService Issue
            UPSWSTimeOut,
            FedEXWSTimeOut,
            FedEXServerCommunicationError,
            TDSFailed,
            USPSWSTimeOut,
            //Account Issue
            InvalidApiAccessKey,
            MissingOrInvalidShippingCarrierAccount,
            MissingOrInvalidShipperInformation,
            MissingShipperAddress,
            //Ship To Address Issue(End User)
            MissingOrInvalidShipToAddress,
            MissingOrInvalidShipToZipcode,
            MissingOrInvalidShipToStateProvinceCode,
            MissingOrInvalidShipToCountryCode,
            //Ship From Address Issue
            MissingShipFromAddress,
            MissingOrInvalidShipFromZipcode,
            MissingOrInvalidShipFromStateProvinceCode,
            MissingOrInvalidShipFromCountryCode,
            //Postalcode, State/Province, CountryIssue
            ZipcodeStateMismatch,
            //Package Issue
            NoPackagesInShipment,
            ExcessMaximumPackageNumber,
            MissingOrInvalidWeight,
            MissingOrInvalidWeightUnitOfMeasurementCode,
            MissingOrInvalidDimension,
            MissingOrInvalidLengthUnitOfMeasurementCode,
            InvalidMeasurementSystemForSelectedCountry,
            //Other Issue
            UnkonwError
        };

        private ErrorCode _code;
        public ErrorCode Code
        {
            get { return _code; }
            set{ _code = value;}
        }


        // Error description, will show in front-end
        private string _errorDescription;
        public string ErrorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }

        /// <summary>
        /// Constructor, setup status result
        /// </summary>
        /// <param name="result"></param>
        public ShippingMethodError()
        {
        }
    
    }
}
