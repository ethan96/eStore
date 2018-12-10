using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;
using eStore.BusinessModules.FedExWebService;
using System.Web.Services;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    partial class ShippingCarrierFedEx:Carrier
    {
        private const int maximumNumberOfPackages = 50;
        private static decimal USMarkUpPrice = 8.5m;

        #region Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public ShippingCarrierFedEx(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        /// <summary>
        /// This function is to get freight estimation and return to  ShippingManager
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns>ArrayList of ShippingMethod</returns>
        public override List<ShippingMethod> getFreightEstimation(Cart cart, eStore.POCOS.Address shipFromAddress)
        {
            Cart _cart = cart;
            //int dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.Country);
            decimal dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.countryCodeX);

            //Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            PackingList packingList = packingManager.getPackingList(cart, dimensionalWeightBase);

            if (packingList == null || packingList.getItemCount() == 0)
                return null;

            //Step 2. send packing list to shipping carrier, shipping carrier return list of shipping methods
            List<ShippingMethod> _fedExShippingMethods = getFedExrates(cart,packingList);

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, packingList, _fedExShippingMethods);

            return _fedExShippingMethods;
        }

        /// <summary>
        /// This method is to retrieve the conversion base for converting Box volumn to DimensionWeight
        /// </summary>
        /// <param name="shipToCountry"></param>
        /// <returns></returns>
        protected override decimal getDimensionWeightBase(String shipToCountry)
        {
            if (shippingCarrier.CarierName.Equals("FedEx_US"))
            {
                //Calculate dimension weight
                //For shipments originateing in the United States, Puerto Rico, the dimension weight base is 194, otherwise all other country is 186
                string _originatingCountry = "US|PR";
                Regex r = new Regex(_originatingCountry);
                Match m = r.Match(shipToCountry);
                if (m.Success)
                    return 194m;
                else
                    return 186m;
            }
            else
                return 0m;  //not applicable
        }
        #endregion


        #region FedEx Service
        private List<ShippingMethod> getFedExrates(Cart cart, PackingList packingList)
        {
            List<ShippingMethod> fedExShippingMethods = new List<ShippingMethod>();

            //set up currency unit for FedEx service
            RateRequest request = CreateRateRequest(packingList);

            //Initialize the service
            RateService service = new RateService();
            service.Timeout = WebServiceTimeout;       //milliseconds
            
            try
            {
                //This is the call to the web service passing in a RateRequest and returning a RateReply
                RateReply reply = service.getRates(request);
                /*
                XmlSerializer serializer = new
                XmlSerializer(typeof(RateReply));
                FileStream fs = new FileStream(@"J:\temp\paps shipping trakcing\AUS.Cart.20150921_094019_211-response.xml", FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);
                RateReply reply = (RateReply)serializer.Deserialize(reader);
                */
                decimal markupprice = getUSMarkUpPrice(cart.TotalAmount, cart.isPStoreOrder());
                fedExShippingMethods = parseFedExReply(reply, packingList, markupprice);
                try
                {
                    saveRequest(cart, request, reply);
                }
                catch (Exception)
                {


                }
                //if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
                //{
                //    fedExShippingMethods = parseFedExReply(reply, packingList);
                //}
                //else
                //{
                //    //Console.WriteLine(reply.Notifications[0].Message);
                //    //Console.WriteLine(reply.Notifications[0].Code + "           " + reply.Notifications[0].Message);
                //    fedExShippingMethods = parseFedExReply(reply, packingList);
                //    eStoreLoger.Error("FedEx WebService Error: " + reply.Notifications[0].Message + "(Code:" + reply.Notifications[0].Code + ")");
                //}
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                fedExShippingMethods = parseFedExException(ex.Message, packingList);
                eStoreLoger.Warn("FedEx WebService Error: " + ex.Message);
                //throw ex;
            }

            //filter out those unsupported shipping method
            var shipmethods = filterShippingMethod(packingList.ShipTo.CountryCode, fedExShippingMethods);
            fixShippingMethod(shipmethods, "FEDEX", packingList.ShipTo);
            //if shipfrom country <> shipto country, method display FedEx International Ground
            if (packingList.ShipTo.CountryCode != packingList.ShipFrom.CountryCode)
            {
                ShippingMethod fedexGround = shipmethods.FirstOrDefault(f => f.ServiceCode.ToUpper() == "FEDEX_GROUND");
                if (fedexGround != null)
                    fedexGround.ShippingMethodDescription = "FedEx International Ground";
            }
            return shipmethods;
        }

        private RateRequest CreateRateRequest(PackingList packingList)
        {
            //Build the RateRequest
            RateRequest request = new RateRequest();

            //Web authentication detail
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = shippingCarrier.AccessKey;
            request.WebAuthenticationDetail.UserCredential.Password = shippingCarrier.Password;

            //Client detail
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = shippingCarrier.LoginAccount;
            request.ClientDetail.MeterNumber = shippingCarrier.ShipperAccount;

            //Transaction detail
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "Reserved in use.";

            request.Version = new VersionId();      //WSDL version information, value is automatically set from wsdl

            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;

            // If programmer needs to add more FedEx shipping ways, please refer
            request.CarrierCodes = new CarrierCodeType[2];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;         //FedEx Express
            request.CarrierCodes[1] = CarrierCodeType.FDXG;         //FedEx Ground

            //set shipment details
            decimal totalInvaluedValue = getTotalInsuredValue(packingList.PackagingBoxes);
            SetShipmentDetails(request, totalInvaluedValue);  //Need to change as cost instead of list price

            //set ship from info
            SetShipFrom(request, packingList.ShipFrom);

            //set ship to info
            SetShipTo(request, packingList.ShipTo);

            //set payment
            SetPayment(request);

            //set group package line items
            SetGroupPackage(request, packingList);

            return request;
        }

        private void SetShipmentDetails(RateRequest request, decimal totalInsuredValue)
        {
            request.RequestedShipment = new RequestedShipment();
            //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            request.RequestedShipment.TotalInsuredValue = new Money();
            request.RequestedShipment.TotalInsuredValue.Amount = totalInsuredValue;
            request.RequestedShipment.TotalInsuredValue.Currency = currencyCode;
            // Shipping date and time
            request.RequestedShipment.ShipTimestamp = DateTime.Now;
            request.RequestedShipment.ShipTimestampSpecified = true;
            //Note: RateRequest automatically returns discount rates.
            //If you include the LIST option, the RateRequest returns both list and discount rates.
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            request.RequestedShipment.PackagingType = new PackagingType();
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING;
            request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            request.RequestedShipment.PackageDetailSpecified = true;
        }

        //Required, The descriptive data for the physical location from which the shipment orginates
        private void SetShipFrom(RateRequest request, CartContact shipFrom)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new eStore.BusinessModules.FedExWebService.Address();
            //request.RequestedShipment.Shipper.Address.StreetLines = new string[2] { _packingList.ShipFrom.Address1, _packingList.ShipFrom.Address2 };
            request.RequestedShipment.Shipper.Address.City = shipFrom.City;
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = shipFrom.State;
            request.RequestedShipment.Shipper.Address.PostalCode = shipFrom.ZipCode;
            //request.RequestedShipment.Shipper.Address.CountryCode = shipFrom.Country;
            request.RequestedShipment.Shipper.Address.CountryCode = shipFrom.countryCodeX;
        }

        //Required, The descriptive data for the physical location to which the shipment is destined
        private void SetShipTo(RateRequest request, CartContact shipTo)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new eStore.BusinessModules.FedExWebService.Address();
            //commented out to avoid error causing by inconsistency
            request.RequestedShipment.Recipient.Address.CountryCode = shipTo.countryCodeX;
            request.RequestedShipment.Recipient.Address.PostalCode = String.IsNullOrEmpty(shipTo.ZipCode) ? "" : shipTo.ZipCode.Trim();

            if ((request.RequestedShipment.Recipient.Address.CountryCode.Equals("US") ||
                request.RequestedShipment.Recipient.Address.CountryCode.Equals("CA")) == false)
            {
                request.RequestedShipment.Recipient.Address.City = String.IsNullOrEmpty(shipTo.City) ? "" : shipTo.City.Trim();
                //request.RequestedShipment.Recipient.Address.StateOrProvinceCode = String.IsNullOrEmpty(shipTo.State) ? "" : shipTo.State.Trim();
            }
        }

        private void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new eStore.BusinessModules.FedExWebService.Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = shippingCarrier.LoginAccount;
        }

        private void SetGroupPackage(RateRequest request, PackingList packingList)
        {
            //Passing individual pieces rate request
            //request.RequestedShipment.PackageCount = _packingList.PackagingBoxes.Count.ToString();
            List<RequestedPackageLineItem> requestedPackageLineItems = new List<RequestedPackageLineItem>();
            int i = 1;
            foreach (PackagingBox pb in packingList.PackagingBoxes)
            {
                MeasureUnit measure = pb.Measure;
                measure.Convert(this.MeasureUnitType);
                pb.Measure = measure; 
                RequestedPackageLineItem _requestedPackageLineItem = new RequestedPackageLineItem();
                _requestedPackageLineItem.SequenceNumber = (i++).ToString();

                //Weight
                _requestedPackageLineItem.Weight = new Weight();
                _requestedPackageLineItem.Weight.Units = (this.MeasureUnitType==MeasureUnit.UnitType.METRICS) ? WeightUnits.KG : WeightUnits.LB;
                _requestedPackageLineItem.Weight.Value = (pb.Weight > 1000) ? 0 : pb.Weight;    //Weight limit is 1k.

                //Dimension, each length limit is 120
                _requestedPackageLineItem.Dimensions = new Dimensions();
                _requestedPackageLineItem.Dimensions.Units = (this.MeasureUnitType==MeasureUnit.UnitType.METRICS) ? LinearUnits.CM : LinearUnits.IN;
                _requestedPackageLineItem.Dimensions.Length = ((Math.Round(pb.Length, 0)) > 120)?"0":(Math.Round(pb.Length,0)).ToString();
                _requestedPackageLineItem.Dimensions.Width = ((Math.Round(pb.Width,0))>120)?"0": (Math.Round(pb.Width, 0)).ToString();
                _requestedPackageLineItem.Dimensions.Height =((Math.Round(pb.Height,0))> 120)?"0": (Math.Round(pb.Height, 0)).ToString();

                //Insured value
                _requestedPackageLineItem.InsuredValue = new Money();
                //FedEx金额不能超过5w
                _requestedPackageLineItem.InsuredValue.Amount = pb.InsuredValue > 50000 ? 50000 : pb.InsuredValue;
                _requestedPackageLineItem.InsuredValue.Currency = pb.InsuredCurrency;

                requestedPackageLineItems.Add(_requestedPackageLineItem);
            }

            request.RequestedShipment.RequestedPackageLineItems = requestedPackageLineItems.ToArray();
        }
        private decimal getUSMarkUpPrice(decimal totalprice, bool isPStoreOrder = false)
        {
           decimal markupprice=USMarkUpPrice;
                if (isPStoreOrder && totalprice < 500)
                    markupprice = 5m;
                return markupprice;
        }

        private List<ShippingMethod> parseFedExReply(RateReply reply, PackingList packingList, decimal markupprice)
        {
            List<ShippingMethod> shippingMethods = new List<ShippingMethod>();
            if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
            {
                foreach (RateReplyDetail rateDetail in reply.RateReplyDetails)
                {
                    //RateReplyDetail rateDetail = reply.RateReplyDetails[0];

                    // The "PAYOR..." rates are expressed in the currency identified in the payor's
                    // rate table(s). The "RATED..." rates are expressed in the currency of the origin country. Former
                    // "...COUNTER..." values have become "...RETAIL..." values, except for PAYOR_COUNTER and
                    // RATED_COUNTER, which have been removed.
                    // Domestic -> Public rate: PAYOR_LIST_PACKAGE, Discount rate: PAYOR_ACCOUNT_PACKAGE
                    // International ->Public rate: PAYOR_LIST_SHIPMENT, Discount rate: PAYOR_ACCOUNT_SHIPMENT

                    ShippingMethod _sm = new ShippingMethod();
                    _sm.ServiceCode = rateDetail.ServiceType.ToString();
                    _sm.ShippingCarrier = CarrierName;
                    //_sm.ShippingMethodDescription = shippingCarrier.getServiceName(_sm.ServiceCode);
                    var _rakeService = shippingCarrier.GetRateServiceByName(_sm.ServiceCode);
                    if (_rakeService != null)
                    {
                        _sm.ShippingMethodDescription = _rakeService.DefaultServiceName;
                        _sm.FreightExType = _rakeService.FreigthExType;
                    }
                    else
                        _sm.ShippingMethodDescription = "";

                    foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                    {
                        if (shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_ACCOUNT_PACKAGE" ||
                            shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_ACCOUNT_SHIPMENT")
                        {
                            // Discount rate              
                            _sm.NegotiatedRateSurcharge =(float)Math.Ceiling((float)shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount);
                            _sm.ShippingCostWithNegotiatedRate = (float)Math.Ceiling((float)shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Amount);
                            _sm.NegotiatedRate = _sm.ShippingCostWithNegotiatedRate - _sm.NegotiatedRateSurcharge;

                            _sm.ShippingCostWithPublishedRate = _sm.ShippingCostWithNegotiatedRate * 1.4f;
                            _sm.PublishRate = _sm.NegotiatedRate*1.4f;
                        }
                        else if (shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_LIST_PACKAGE" ||
                                    shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_LIST_SHIPMENT")
                        {
                            // Public rate
                            _sm.PublishRateSurcharge = (float)shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount;
                            _sm.ShippingCostWithPublishedRate = (float)shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Amount;
                            _sm.PublishRate = _sm.ShippingCostWithPublishedRate - _sm.PublishRateSurcharge;
                        }
                        else
                        {
                            //other type which is not supported by eStore at this moment
                        }

                        _sm.UnitOfCurrency = shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Currency;

                    }
                    _sm.Error = null;
                    _sm.PackingList = packingList;
                    if (shippingCarrier.CarierName.Equals("FedEx_US"))
                    {
                        _sm.ShippingCostWithPublishedRate += (float)markupprice;
                    }
                    shippingMethods.Add(_sm);
                }
            }
            else
            {
                ShippingMethod _sm = new ShippingMethod();
                //_sm.ServiceCode = rateDetail.ServiceType.ToString();
                _sm.ShippingCarrier = CarrierName;
                _sm.ShippingMethodDescription = shippingCarrier.getServiceName(_sm.ServiceCode);
                _sm.Error = getShippingMethodError(reply.Notifications[0].Code);
                _sm.ShippingMethodDescription = reply.Notifications[0].Code;
                _sm.PackingList = packingList;
                shippingMethods.Add(_sm);
            }
            return shippingMethods;
        }

        private List<ShippingMethod> parseFedExException(string ex, PackingList packingList)
        {
            List<ShippingMethod> shippingMethods = new List<ShippingMethod>();
            ShippingMethod _sm = new ShippingMethod();
            //_sm.ServiceCode = rateDetail.ServiceType.ToString();
            _sm.ShippingCarrier = CarrierName;
            _sm.ShippingMethodDescription = shippingCarrier.getServiceName(_sm.ServiceCode);
            _sm.Error = getShippingMethodError(ex);
            _sm.ShippingMethodDescription = ex;
            _sm.PackingList = packingList;
            shippingMethods.Add(_sm);

            return shippingMethods;
        }

        private decimal getTotalInsuredValue(ICollection<PackagingBox> packagingBoxs)
        {
            decimal totalInsureValue = 0;
            foreach (PackagingBox pb in packagingBoxs)
            {
                //FedEx金额不能超过5w
                totalInsureValue += pb.InsuredValue > 50000 ? 50000 : pb.InsuredValue;
            }

            return totalInsureValue;
        }

        private ShippingMethodError getShippingMethodError(string errMsg)
        {
            /** ***************************************************************
             *    Code               Description
             *    =====================================
             * 1000           Authentication Failed
             *         2           Invalid Shipper Account Nbr.
             *    803           Meter number is missing or invalid.
             *    
             *    866           Origin postal code missing or invalid.
             *    839           Origin Postal-State Mismatch.
             *    709           Origin country is not serviced.
             *    
             *    836           Destination Postal-State Mismatch.
             *    521           Destination postal code missing or invalid.
             *    522           Destination country code missing or invalid
             *    
             *    809           Package 1 - Weight is missing or invalid.
             *    858           Package 1 - Invalid dimensions.
             *    
             *  Object reference not set to an instance of an object.
             * ***************************************************************/

            ShippingMethodError err = new ShippingMethodError();

            if (errMsg.Contains("timed out"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.FedEXWSTimeOut;
            }
            else if (errMsg == "1000" || errMsg == "2")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShippingCarrierAccount;
            }
            else if (errMsg == "803")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShipperInformation;
            }
            else if (errMsg == "866" || errMsg == "839")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShipFromZipcode;
            }
            else if (errMsg == "709")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShipFromCountryCode;
            }
            else if (errMsg == "836" || errMsg == "521")
            {
                err.ErrorLevelType = ShippingMethodError.Type.EndUser;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShipToZipcode;
            }
            else if (errMsg == "522")
            {
                err.ErrorLevelType = ShippingMethodError.Type.EndUser;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidShipToCountryCode;
            }
            else if (errMsg == "809")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidWeight;
            }
            else if (errMsg == "858")
            {
                err.ErrorLevelType = ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.MissingOrInvalidDimension;
            }
            else if(errMsg.Contains("Object reference not set "))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.UnkonwError;
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
        private void saveXMLlog(Cart cart, POCOS.Address shipFromAddress, PackingList packingList, List<ShippingMethod> shippingMethod)
        {
            XmlDocument doc = getXMLlog(cart, shipFromAddress, packingList, shippingMethod);

            //Save XML file to C:\eStoreResources3C\Logs\Shipping
            StringBuilder filePath = new StringBuilder();
            string folderName = "FedEx";
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
        private void saveRequest(Cart cart, RateRequest req, RateReply resp)
        {

            StringBuilder filePath = new StringBuilder();
            string folderName = "FedEx";
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
            XmlSerializer serializer2 = new XmlSerializer(typeof(RateReply));
            TextWriter textWriter2 = new StreamWriter(string.Format("{0}\\{1}-response.xml", filePath.ToString(), cart.CartID));
            serializer2.Serialize(textWriter2, resp);
            textWriter2.Close();
        }
        #endregion
    }
}
