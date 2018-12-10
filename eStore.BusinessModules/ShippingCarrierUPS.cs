using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using eStore.BusinessModules.UPSWebService;
using eStore.Utilities;
using eStore.BusinessModules.UPSLandedCostWS;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace eStore.BusinessModules
{
    public class ShippingCarrierUPS:Carrier
    {
        //Dimenisonal weigth base is used to calculate packing box dimensional weight
        //this rule is only applicable to US so far.
        //Dimensional Weight is L*W*H / dimensionalWeightBase
        private static decimal USDomesticDWB = 166;
        private static decimal USInternalDWB = 139;
        private const int maximumNumerberOfPackages = 50;
        private decimal getUSMarkUpPrice(decimal totalprice, bool isPStoreOrder=false)
        {
            if (store.StoreID == "AUS" || store.StoreID == "ALA")
            {
                decimal markupprice = 8.5m;//Advantech S & H Charge $5 and DC $3.5
                if (isPStoreOrder && totalprice < 500)
                    markupprice = 5m;
                if (totalprice > 300)
                {
                    markupprice += Math.Ceiling(totalprice / 100) * 0.5m;
                }
                else if (totalprice > 100)
                {
                    markupprice += 1.5m;
                }
                return markupprice;
            }
            else
            {
                return 0m;
            }
 
        }
          
        private static Boolean getNegotiationRate = false;

        #region Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShippingCarrierUPS(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        /// <summary>
        /// This function is to get freight estimation and return to  ShippingManager
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns>ArrayList of ShippingMethod</returns>
        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            //int dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.Country);
            decimal dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.countryCodeX);

            // Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            PackingList packingList = packingManager.getPackingList(cart, dimensionalWeightBase);

            if (packingList == null || packingList.getItemCount() == 0)
                return null;

            // Step 2. Send packing list to shipping carrier, shipping carrier return list of shipping methods
            List<ShippingMethod> _upsShippingMethods = getUPSrates(cart,packingList);

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, packingList, _upsShippingMethods);

            return _upsShippingMethods;
        }

        /// <summary>
        /// This method is to retrieve the conversion base for converting Box volumn to DimensionWeight
        /// </summary>
        /// <param name="shipToCountry"></param>
        /// <returns></returns>
        protected override decimal getDimensionWeightBase(String shipToCountry)
        {
            if (shippingCarrier.CarierName.Equals("UPS_US"))
            {
                //Calculate dimension weight
                //For shipments to Canada, the United States, Puerto Rico and the U.S. Virgin Islands, 
                //the dimension weight base is 166, otherwise all other country is 139
                string _originatingCountry = "US|PR|CA|VI";
                Regex r = new Regex(_originatingCountry);
                Match m = r.Match(shipToCountry);
                if (m.Success)
                    return USDomesticDWB;
                else
                    return USInternalDWB;
            }
            else
                return USInternalDWB; ;  //not applicable
        }

        /// <summary>
        /// This method is to get shipping estimation from UPS
        /// </summary>
        /// <param name="packlist"></param>
        /// <returns>List of ShippingMethods</returns>
        private List<ShippingMethod> getUPSrates(Cart cart,PackingList packlist)
        {
            //PackingList _packlist = packlist;
            List<ShippingMethod> _upsShippingMethods = new List<ShippingMethod>();

            //UPS has limited numbers of packages in 1 request, so need to seperate to send request.
            int NumbersOfRequest = 0;
            if (packlist.PackagingBoxes.Count % maximumNumerberOfPackages != 0)
                NumbersOfRequest = (int)(packlist.PackagingBoxes.Count / maximumNumerberOfPackages) + 1;
            else
                NumbersOfRequest = (int)(packlist.PackagingBoxes.Count / maximumNumerberOfPackages);

            RateService rate = new RateService();
            RateRequest rateRequest = new RateRequest();

            UPSSecurity upss = new UPSSecurity();
            UPSSecurityServiceAccessToken upssSvcAccessToken = new UPSSecurityServiceAccessToken();
            upssSvcAccessToken.AccessLicenseNumber = shippingCarrier.AccessKey;
            upss.ServiceAccessToken = upssSvcAccessToken;
            UPSSecurityUsernameToken upssUsrNameToken = new UPSSecurityUsernameToken();
            upssUsrNameToken.Username = shippingCarrier.LoginAccount;
            upssUsrNameToken.Password = shippingCarrier.Password; 
            upss.UsernameToken = upssUsrNameToken;
            rate.UPSSecurityValue = upss;
            rate.Timeout = WebServiceTimeout;

            RequestType request = new RequestType();
            String[] requestOption = { "Shop" };        //don't change it
            request.RequestOption = requestOption;
            rateRequest.Request = request;

            eStore.BusinessModules.UPSWebService.ShipmentType shipment = new eStore.BusinessModules.UPSWebService.ShipmentType();

            ShipperType shipper = new ShipperType();
            shipper.ShipperNumber = shippingCarrier.ShipperAccount;
            //shipper address
            AddressType shipperAddress = new AddressType();
            fillupAddress(shipperAddress, packlist.ShipFrom);
            shipper.Address = shipperAddress;
            shipment.Shipper = shipper;

            //ship from address
            AddressType shipFromAddress = new AddressType();
            fillupAddress(shipFromAddress, packlist.ShipFrom);
            ShipFromType shipFrom = new ShipFromType();
            shipFrom.Address = shipFromAddress;
            shipment.ShipFrom = shipFrom;

            //ship to address
            ShipToAddressType shipToAddress = new ShipToAddressType();
            fillupAddress(shipToAddress, packlist.ShipTo, false);
            ShipToType shipTo = new ShipToType();
            shipTo.Address = shipToAddress;
            shipment.ShipTo = shipTo;

            //to get neogotiate rates, it must  declare NegotiatedRatesIndicator = ""
            ShipmentRatingOptionsType neogotiateRating = new ShipmentRatingOptionsType();
            neogotiateRating.NegotiatedRatesIndicator = "";
            if (this.store.StoreID == "AEU")
            {
                shipment.ShipmentRatingOptions = neogotiateRating;
                getNegotiationRate = true;
            }
            List<PackageType> packages = new List<PackageType>();
            foreach (PackagingBox pb in packlist.PackagingBoxes)
            {
                MeasureUnit measure = pb.Measure;
                measure.Convert(this.MeasureUnitType);
                pb.Measure = measure; 
                //convert eStore packaging box to ups package
                PackageType package = new PackageType();

                //UPS package dimension and weight type
                //prepare UPS package dimension info
                DimensionsType packageDimension = new DimensionsType();
                packageDimension.Length = Math.Round(pb.Measure.Length, 2).ToString();
                packageDimension.Height = Math.Round(pb.Measure.Height, 2).ToString();
                packageDimension.Width = Math.Round(pb.Measure.Width,2).ToString();

                CodeDescriptionType dimensionUnitType = new CodeDescriptionType();
                dimensionUnitType.Code = dimensionUnit;     //dimensionUnit;
                packageDimension.UnitOfMeasurement = dimensionUnitType;
                package.Dimensions = packageDimension;

                //prepare UPS package weight info
                PackageWeightType packageWeight = new PackageWeightType();
                packageWeight.Weight = Math.Round(pb.Weight, 2).ToString();
                CodeDescriptionType weightUnitType = new CodeDescriptionType();
                weightUnitType.Code = weightUnit;    //weightUnit;
                packageWeight.UnitOfMeasurement = weightUnitType;
                package.PackageWeight = packageWeight;
                PackageServiceOptionsType ps = new PackageServiceOptionsType();
                InsuredValueType ivt = new InsuredValueType();
                ivt.MonetaryValue = Math.Round(pb.InsuredValue, 2).ToString();
                ivt.CurrencyCode = currencyCode;
                ps.DeclaredValue = ivt;
                
                CodeDescriptionType packType = new CodeDescriptionType();
                packType.Code = "02";       //don't change it, 02 means that pack type is box
                package.PackagingType = packType;
                package.PackageServiceOptions = ps;
                packages.Add(package);
            }

            


            try
            {
                RateResponse rateResponse = new RateResponse();
                //shipment.Package = packages.ToArray();

                for (int i = 0; i < NumbersOfRequest; i++)
                {
                    shipment.Package = packages.Skip(i * maximumNumerberOfPackages).Take(maximumNumerberOfPackages).ToArray();
                    if (getNegotiationRate)
                    {
                        //this optional parameter is to acquire negotiation rate.  We don't need this at this moment
                        shipment.ShipmentRatingOptions = neogotiateRating;
                    }
                    rateRequest.Shipment = shipment;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                          | SecurityProtocolType.Tls11
                          | SecurityProtocolType.Tls12
                          | SecurityProtocolType.Ssl3;

                    rateResponse = rate.ProcessRate(rateRequest);
                    try
                    {
                        saveRequest(cart, rateRequest, rateResponse);
                    }
                    catch (Exception)
                    {
                        
                      
                    }
                    if (rateResponse == null)
                    {
                        throw new Exception("Failed to get UPS rate response.");
                    }
                    float USMarkUpPrice = (float)getUSMarkUpPrice(cart.TotalAmount, cart.isPStoreOrder());
                    foreach (RatedShipmentType a in rateResponse.RatedShipment)
                    {
                        //get rate response from UPS
                        ShippingMethod _sm = new ShippingMethod();
                        _sm.PackingList = packlist;
                        _sm.ServiceCode = a.Service.Code;
                        _sm.ShippingCarrier = CarrierName;
                        _sm.ShippingMethodDescription = shippingCarrier.getServiceName(a.Service.Code);
                        _sm.PublishRate = (float)Convert.ToDouble(a.TransportationCharges.MonetaryValue);
                        _sm.NegotiatedRate = (a.NegotiatedRateCharges == null) ? 0f : (float)Convert.ToDouble(a.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                        _sm.InsuredCharge = (float)Convert.ToDouble(a.ServiceOptionsCharges.MonetaryValue);
                        _sm.ShippingCostWithPublishedRate = (float)Math.Round(Convert.ToDouble(a.TransportationCharges.MonetaryValue) + USMarkUpPrice, 2);
                        _sm.ShippingCostWithNegotiatedRate = (a.NegotiatedRateCharges == null) ? 0f : (float)Math.Round(_sm.NegotiatedRate, 0);
                        _sm.UnitOfCurrency = a.TotalCharges.CurrencyCode;
                        if (getNegotiationRate)
                            _sm.ShippingCostWithPublishedRate = _sm.ShippingCostWithNegotiatedRate;

                        _sm.Error = null;
                        _upsShippingMethods.Add(_sm);
                    }
                }
                _upsShippingMethods = fixUPSShippingMethod(_upsShippingMethods);

              //LandedCost  landedcost =  getLandedCost(packlist);

              //filter out those unsupported shipping method
              List<ShippingMethod> filteredshipmethods=  filterShippingMethod(shipToAddress.CountryCode, _upsShippingMethods);
              fixShippingMethod(filteredshipmethods, "UPS", packlist.ShipTo);
              _upsShippingMethods = filteredshipmethods;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                //added by Edward and commented by Jay, no reason to throw ex
                //throw ex; 
                //Console.WriteLine(ex.Detail.InnerText);
                //get rate response from UPS
                ShippingMethod _sm = new ShippingMethod();
                _sm.PackingList = packlist;
                _sm.Error = getShippingMethodError(ex.Detail.InnerText);
                _sm.ShippingMethodDescription = ex.Detail.InnerText;
                _upsShippingMethods.Add(_sm);
                eStoreLoger.Error("UPS WebService Error: " + ex.Detail.InnerText, "", "", "", ex);
            }
            catch (Exception ex)
            {
                //added by Edward and commented by Jay, no reason to throw ex
                //throw ex;
                //Console.WriteLine(ex.Message);
                //get rate response from UPS
                ShippingMethod _sm = new ShippingMethod();
                _sm.PackingList = packlist;
                _sm.Error = getShippingMethodError(ex.Message);
                _sm.ShippingMethodDescription = "General Error";
                _upsShippingMethods.Add(_sm);
                eStoreLoger.Error("UPS WebService Error: " + ex.Message, "", "", "", ex);
            }

            return _upsShippingMethods;
        }

        /// <summary>
        /// this fucntion will operation same shippingmethod
        /// </summary>
        /// <param name="upsShippingMethods"></param>
        /// <returns></returns>
        private List<ShippingMethod> fixUPSShippingMethod(List<ShippingMethod> upsShippingMethods)
        {
            List<ShippingMethod> _upsShippingMethods = new List<ShippingMethod>();
            foreach (ShippingMethod a in upsShippingMethods)
            {
                ShippingMethod _sm = _upsShippingMethods.FirstOrDefault(c => c.ServiceCode == a.ServiceCode);
                if (_sm == null)
                    _upsShippingMethods.Add(a);
                else
                {
                    _sm.PublishRate += a.PublishRate;
                    _sm.NegotiatedRate += a.NegotiatedRate;
                    _sm.InsuredCharge += a.InsuredCharge;
                    _sm.ShippingCostWithPublishedRate += a.ShippingCostWithPublishedRate;
                    _sm.ShippingCostWithNegotiatedRate += a.ShippingCostWithNegotiatedRate;
                }
            }
            return _upsShippingMethods;
        }


        /// <summary>
        /// 
        /// </summary>
        private LandedCost getLandedCost(PackingList packlist)
        {
            try
            {
                LC lc = new LC();
                LandedCostRequest lcRequest = new LandedCostRequest();
                QueryRequestType queryRequest = new QueryRequestType();
                eStore.BusinessModules.UPSLandedCostWS.ShipmentType shipType = new eStore.BusinessModules.UPSLandedCostWS.ShipmentType();

                shipType.OriginCountryCode = packlist.ShipFrom.countryCodeX;
                shipType.OriginStateProvinceCode = packlist.ShipFrom.State;

                shipType.DestinationCountryCode = packlist.ShipTo.countryCodeX;
                shipType.DestinationStateProvinceCode = packlist.ShipTo.State;
                shipType.TransportationMode = "1";
                shipType.ResultCurrencyCode = "USD";

                eStore.BusinessModules.UPSLandedCostWS.ChargesType freightChargeType = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                freightChargeType.CurrencyCode = "USD";
                freightChargeType.MonetaryValue = "0";  //??
                shipType.FreightCharges = freightChargeType;

                eStore.BusinessModules.UPSLandedCostWS.ChargesType additionalInsurance = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                additionalInsurance.CurrencyCode = "USD";
                additionalInsurance.MonetaryValue = "0";
                shipType.AdditionalInsurance = additionalInsurance;                          

               
                shipType.Product = getProducts(packlist).ToArray<ProductType>();
                shipType.ResultCurrencyCode = "USD";
                queryRequest.Shipment = shipType;

                RequestTransportType requestTransType = new RequestTransportType();
                requestTransType.RequestAction = "LandedCost";
                lcRequest.Request = requestTransType;
                lcRequest.Item = queryRequest;


                AccessRequest accessRequest = new AccessRequest();
                accessRequest.UserId = "eStore.Advantech";
                accessRequest.Password = "eStoreUPS";
                accessRequest.AccessLicenseNumber = "CC640DFEAA3FFCD8";
                lc.AccessRequestValue = accessRequest;

                System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
                Console.WriteLine(lcRequest);
                LandedCostResponse landedCostResponse = lc.LCRequest(lcRequest);

                //Please note:
                //Below type casting/iteration/extraction of required data is for sample. Type, null and length checks are recommended.
                QueryResponseType queryResponse = (QueryResponseType)landedCostResponse.Item;
                String questionName = queryResponse.Shipment.Product[0].Question[0].Name;
                String questionText = queryResponse.Shipment.Product[0].Question[0].Text;

                string t = queryResponse.TransactionDigest;


                Console.WriteLine("The landedCostResponse : ");
                Console.WriteLine("Question name : " + questionName);
                Console.WriteLine("Question Text: " + questionText);
                LandedCost landedcost = ShipmentEstimate(queryResponse);

                return landedcost;
            }

            catch (System.Web.Services.Protocols.SoapException ex)
            {
                throw ex;
                //eStoreLoger.Error(ex.Message, "", "", "", ex);
                //Console.WriteLine(ex.Message);
                //return null;
            }

            catch (Exception ex)
            {
                throw ex;
                //eStoreLoger.Error(ex.Message, "", "", "", ex);
                //Console.WriteLine(ex.Message);
                //return null;
            }

        }


        private ProductType[] getProducts(PackingList pklist) {

            try
            {
                List<ProductType> producttypes = new List<ProductType>();

                foreach (PackagingBox pbox in pklist.PackagingBoxes)
                {
                    foreach (PackingBoxDetail citem in pbox.PackingBoxDetails)
                    {
                        ProductType prodType = new ProductType();
                        prodType.ProductCountryCodeOfOrigin = pklist.ShipFrom.countryCodeX;
                        TariffInfoType teriffType = new TariffInfoType();

                        //from Part object
                        teriffType.TariffCode = "8471.50.20.000";

                        prodType.TariffInfo = teriffType;
                        eStore.BusinessModules.UPSLandedCostWS.ChargesType unitPrice = new eStore.BusinessModules.UPSLandedCostWS.ChargesType();
                        unitPrice.MonetaryValue = "1000";
                        unitPrice.CurrencyCode = "USD";
                        prodType.UnitPrice = unitPrice;

                        ValueWithUnitsType vwt = new ValueWithUnitsType();
                        ValueWithUnitsTypeUnitOfMeasure unit = new ValueWithUnitsTypeUnitOfMeasure();
                        unit.UnitCode = "kg";
                        vwt.UnitOfMeasure = unit;
                        vwt.Value = "2";

                        prodType.Weight = vwt;
                        ValueWithUnitsType quantity = new ValueWithUnitsType();
                        quantity.Value = citem.Qty.ToString();
                        prodType.Quantity = quantity;

                        producttypes.Add(prodType);
                    }
                }

                return producttypes.ToArray<ProductType>();
            }
            catch (Exception e) {
                throw e;
            }
           
        
        }


        /// <summary>
        ///  This method will return landed cost, extract Duty, Tax , otherTax&Fees and Tax&Fees.
        /// </summary>
        /// <param name="queryResponse"></param>

        private LandedCost ShipmentEstimate(QueryResponseType queryResponse)
        {
            LandedCost landedcost = new LandedCost();
            LC lc = new LC();
            LandedCostRequest lcRequest = new LandedCostRequest();
            EstimateRequestType queryRequest = new EstimateRequestType();
            ShipmentAnswerType shipType = new ShipmentAnswerType();
            ProductAnswerType[] product = new ProductAnswerType[queryResponse.Shipment.Product.Length];

            //Set products information
            for (int i = 0; i < queryResponse.Shipment.Product.Length; i++)
            {
                product[i] = new ProductAnswerType();
                product[i].TariffCode = queryResponse.Shipment.Product[i].TariffCode;
            }
            shipType.Product = product;


            queryRequest.Shipment = shipType;
            queryRequest.TransactionDigest = queryResponse.TransactionDigest;
            RequestTransportType requestTransType = new RequestTransportType();
            requestTransType.RequestAction = "LandedCost";
            lcRequest.Request = requestTransType;
            lcRequest.Item = queryRequest;

            AccessRequest accessRequest = new AccessRequest();
            accessRequest.UserId = "eStore.Advantech";
            accessRequest.Password = "eStoreUPS";
            accessRequest.AccessLicenseNumber = "CC640DFEAA3FFCD8";
            lc.AccessRequestValue = accessRequest;

            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            Console.WriteLine(lcRequest);
            LandedCostResponse landedCostResponse = lc.LCRequest(lcRequest);

            EstimateResponseType estimateResponse = (EstimateResponseType)landedCostResponse.Item;

            //Calcuate Total Tax, Duties, fees  for all products
            decimal duties = 0m;
            decimal vats = 0m;
            decimal OtherTaxFees = 0m;

            foreach (ProductEstimateType p in estimateResponse.ShipmentEstimate.ProductsCharges.Product)
            {
                decimal duty;
                decimal vat;
                decimal taxesfee;
                if (decimal.TryParse(p.Charges.Duties, out duty))
                    duties = duties + duty;

                if (decimal.TryParse(p.Charges.VAT, out vat))
                    vats = vats + vat;

                if (decimal.TryParse(p.Charges.TaxesAndFees, out taxesfee))
                    OtherTaxFees = OtherTaxFees + taxesfee;
            }

            landedcost.Duties = duties;
            landedcost.VAT = vats;
            landedcost.OtherTaxAndFees = OtherTaxFees;
            decimal taxfees = 0m;
            decimal.TryParse(estimateResponse.ShipmentEstimate.ShipmentCharges.TaxesAndFees, out taxfees);
            landedcost.TaxAndFees = taxfees;

            //Console.WriteLine("CurrrencyCode" + estimateResponse.ShipmentEstimate.CurrencyCode);
            Console.WriteLine("Duties" + duties);
            Console.WriteLine("VAT " + vats);
            Console.WriteLine("Other Tax&Fees" + OtherTaxFees);
            Console.WriteLine("TaxesAndFees" + estimateResponse.ShipmentEstimate.ShipmentCharges.TaxesAndFees);

            return landedcost;
        }

        /// <summary>
        /// This method is to fill up UPS address type fields
        /// </summary>
        /// <param name="address"></param>
        /// <param name="contact"></param>
        private void fillupAddress(AddressType address, CartContact contact, Boolean includeAddressLine=true)
        {
            if (includeAddressLine)
            {
                string[] street = { String.IsNullOrEmpty(contact.Address1) ? "" : contact.Address1.Trim(), String.IsNullOrEmpty(contact.Address2) ? "" : contact.Address2.Trim() };
                address.AddressLine = street;
            }

            if (getNegotiationRate)
            {
                address.City = String.IsNullOrEmpty(contact.City) ? "" : contact.City.Trim();
                address.StateProvinceCode = String.IsNullOrEmpty(contact.State) ? "" : contact.State.Trim();
            }

            address.PostalCode = String.IsNullOrEmpty(contact.ZipCode) ? "" : contact.ZipCode.Trim();
            //address.CountryCode = contact.Country;
            address.CountryCode = contact.countryCodeX;
        }

        private ShippingMethodError getShippingMethodError(string errMsg)
        {
            /** ***************************************************************
             *    Code               Description
             *    =====================================
             * 000000           The operation has timed out.
             * 250002           Invalid UserId/Password
             * 250003           Invalid Access License number
             * 
             * 110004           Missing shipper information.
             * 110005           Missing shipper number.
             * 110006           Missing shipper address information.
             * 
             * 110046           Missing Ship From Postal Code
             * 110017           Missing ship from country code.
             * 110016           Missing ship from state province code.
             * 110014           Missing ship from address information.
             * 
             * 110045           Missing Ship To Postal Code.
             * 110013           Missing ship to country code.
             * 110012           Missing ship to state province code.
             * 110010           Missing ship to address information.
             * 110208           Missing/Illegal ShipTo/Address/CountryCode
             * 
             * 110308           Invalid or unsupported origin country code
             * 111285           The postal code is invalid in the state. (Ship to / ship from)
             * 111286           Input is not a valid state for the specified shipment.
             * 
             * 110108           Unknown Error.
             * 
             * 110002           No packages in shipment.
             * 110003           Maximum number of package exceeded(50)
             * 110609           All package dimensions are required and each must be greater than 0 for package 1.
             * 
             * 111030           Packages must weigh more than zero pounds.
             * 111035           The maximum per package weight for the selected service from the selected country is 150 pounds.
             * 111036           The maximum per package weight for the selected service from the selected country is 70 kg.
             * 111031           Packages must weigh more than zero kg.
             * 110023           Missing or invalid package weight unit of measurement code for package 1. Valid values are: KGS or LBS.
             * 110548           A shipment cannot have a KGS/IN or LBS/CM as its unit of measurements
             * 
             * 111057           This measurement system is not valid for the selected country.
             * 
             * 111055           Package exceeds the maximum length constraint of 108 inches. Length is the longest side of a package.
             * 111056           This package exceeds the maximum length constraint of 270 cm. Length is the longest side of a package.
             * 110020           Missing package dimensions unit of measurement information for package 1.
             * 110021           Missing or invalid package dimensions unit of measurement code for package 1. Valid values are: IN or CM.
             * ***************************************************************/
            
            ShippingMethodError err = new ShippingMethodError();
           
            if (errMsg.Contains("timed out"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.UPSWSTimeOut;
            }
            else if (errMsg.Contains("250002"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShippingCarrierAccount;
            }
            else if (errMsg.Contains("250003"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.InvalidApiAccessKey;
            }
            else if (errMsg.Contains("110004") || errMsg.Contains("110005"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipperInformation;
            }
            else if (errMsg.Contains("110006"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingShipperAddress;
            }
            else if (errMsg.Contains("1100046"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipFromZipcode;
            }
            else if (errMsg.Contains("1100017") || errMsg.Contains("110308"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipFromCountryCode;
            }
            else if (errMsg.Contains("1100016"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipFromStateProvinceCode;
            }
            else if (errMsg.Contains("1100014"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingShipFromAddress;
            }
            else if (errMsg.Contains("1100045") || errMsg.Contains("111285"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToZipcode;
            }
            else if (errMsg.Contains("1100013"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToCountryCode;
            }
            else if (errMsg.Contains("1100012") || errMsg.Contains("110286"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToStateProvinceCode;
            }
            else if (errMsg.Contains("1100010") || errMsg.Contains("110208"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToAddress;
            }
            else if (errMsg.Contains("110108"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.UnkonwError;   
            }
            else if (errMsg.Contains("110002"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.NoPackagesInShipment;
            }
            else if (errMsg.Contains("110003"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.ExcessMaximumPackageNumber;
            }
            else if (errMsg.Contains("110609") || errMsg.Contains("111055") || errMsg.Contains("111056"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidDimension;
            }
            else if (errMsg.Contains("111030") || errMsg.Contains("111031") || errMsg.Contains("111035") || errMsg.Contains("111036"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidWeight;
            }
            else if (errMsg.Contains("110023"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidWeightUnitOfMeasurementCode;
            }
            else if (errMsg.Contains("110548") || errMsg.Contains("111057"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.InvalidMeasurementSystemForSelectedCountry;
            }
            else if (errMsg.Contains("110021"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidLengthUnitOfMeasurementCode;
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
            XmlDocument doc = getXMLlog(cart, shipFromAddress, packingList, shippingMethod);

            //Save XML file to C:\eStoreResources3C\Logs\Shipping
            StringBuilder filePath = new StringBuilder();
            string folderName = "UPS";
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

        private void saveRequest(Cart cart, RateRequest req, RateResponse resp)
        {
          
            StringBuilder filePath = new StringBuilder();
            string folderName = "UPS";
            DateTime now = DateTime.Now;
            int year = now.Year;
            int month = now.Month;
            string yearMonth = year.ToString() + "_" + month.ToString();
            filePath.Append(System.Configuration.ConfigurationManager.AppSettings.Get("Log_Path")).Append("/").Append("Shipping").Append("/").Append(yearMonth).Append("/").Append(folderName).Append("/");
            // filename is order number
            string filename = "";
            filename = cart.CartID + "-request.xml";
            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            {
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }

            //Save
            XmlSerializer serializer = new XmlSerializer(typeof(RateRequest));
            TextWriter textWriter = new StreamWriter(string.Format("{0}\\{1}-request.xml", filePath.ToString(), cart.CartID));
            serializer.Serialize(textWriter, req);
            textWriter.Close();
            XmlSerializer serializer2 = new XmlSerializer(typeof(RateResponse));
            TextWriter textWriter2 = new StreamWriter(string.Format("{0}\\{1}-response.xml", filePath.ToString(), cart.CartID));
            serializer2.Serialize(textWriter2, resp);
            textWriter2.Close();
        }

        #endregion
    }
}
