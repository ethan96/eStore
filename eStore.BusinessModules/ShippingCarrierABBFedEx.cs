using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;
using System.IO;
using System.Xml.Serialization;
using eStore.Utilities;
using System.Xml;
using System.Net;
using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using eStore.POCOS.DAL;
using eStore.BusinessModules;
using eStore.BusinessModules.FedexRateWebServiceNew;

namespace eStore.BusinessModules
{
    public class ShippingCarrierABBFedEx : Carrier
    {
        private const int maximumNumerberOfPackages = 50;
        private readonly string _licenseNumber;
        private readonly string _password;
        private readonly Hashtable _serviceCodes = new Hashtable(12);
        private readonly string _serviceDescription;
        private readonly string _shipperNumber;
        private readonly string _userId;




        public ShippingCarrierABBFedEx(POCOS.Store store, ShippingCarier shippingCarrier) : base(store, shippingCarrier)
        {
            _licenseNumber = shippingCarrier.AccessKey;
            _userId = shippingCarrier.LoginAccount;
            _password = shippingCarrier.Password;
            _shipperNumber = shippingCarrier.ShipperAccount;
            _serviceDescription = "";

        }


        /// <summary>
        /// This function is to get freight estimation and return to  ShippingManager
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="shipFromAddress"></param>
        /// <returns>ArrayList of ShippingMethod</returns>
        public override List<ShippingMethod> getFreightEstimation(Cart cart, eStore.POCOS.Address shipFromAddress)
        {
            //int dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.Country);
            decimal dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.countryCodeX);

            //Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            PackingList packingList = packingManager.getPackingList(cart, dimensionalWeightBase);

            if (packingList == null || packingList.getItemCount() == 0)
                return null;

            //Step 2. send packing list to shipping carrier, shipping carrier return list of shipping methods
            List<ShippingMethod> _fedExShippingMethods = getFedExrates(cart, packingList);

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, packingList, _fedExShippingMethods);

            return _fedExShippingMethods;
        }


        public List<ShippingMethod> getFedExrates(Cart cart, PackingList packingList)
        {

            List<ShippingMethod> fedExShippingMethods = new List<ShippingMethod>();

            //FedEx Express & FedEx Ground   - If programmer needs to add more FedEx shipping ways, please refer  
            //List<String> lstcodes = new List<string>() { "FDXE, FDXG" }; 
        
            var accountValid = false;

            // all we can really do here is verify that it matches the format for a fedex account number...they no longer provide rates or validation of an account
            // number through their web API, and only allow PaymentType.SENDER as a payment assignment even though PaymentType.RECIPIENT and PaymentType.THIRDPARTY
            // are listed in documentation

            //if (!String.IsNullOrWhiteSpace(crt.cartHeader.CarrierAccount))
            //{
            //    String shipAccountRegex = "^\\d{9}$";

            //    if (Regex.IsMatch(crt.cartHeader.CarrierAccount, shipAccountRegex))
            //    {
            //        accountValid = true;

            //        // now determine which methods to allow the user to select from
            //        bool useDomesticRates = crt.cartHeader.B_Country == "US" && crt.cartHeader.S_Country == "US";

            //        if (useDomesticRates)
            //        {
            //            ShippingMethod smGround = new ShippingMethod("FEDEX", "FedEx Ground", Decimal.Zero);
            //            ShippingMethod smTwoDay = new ShippingMethod("FEDEX", "FedEx 2 Day", Decimal.Zero);
            //            ShippingMethod smPriorityOvernight = new ShippingMethod("FEDEX", "FedEx Priority Overnight", Decimal.Zero);

            //            lstMethods.Add(smGround);
            //            lstMethods.Add(smTwoDay);
            //            lstMethods.Add(smPriorityOvernight);
            //        }
            //        else
            //        {
            //            ShippingMethod smEconomy = new ShippingMethod("FEDEX", "FedEx International Economy", Decimal.Zero);
            //            ShippingMethod smPriority = new ShippingMethod("FEDEX", "FedEx International Priority", Decimal.Zero);

            //            lstMethods.Add(smEconomy);
            //            lstMethods.Add(smPriority);
            //        }
            //    }
            //}


            if (!accountValid)
            {
                RateRequest request = GetFedExRequest(packingList);

                RateService service = new RateService();
                service.Timeout = 20000;
                try
                {
                    //ServicePointManager.SecurityProtocol =
                    //SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                    //SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    RateReply reply = service.getRates(request);
                    Decimal handlingFee = 7;
                    fedExShippingMethods = parseFedExReply(reply, packingList, handlingFee);
                    try
                    {
                        saveRequest(cart, request, reply);
                    }
                    catch (Exception)
                    {


                    }
                }
                catch (Exception ex)
                {
                    fedExShippingMethods = parseFedExException(ex.Message, packingList);
                    eStoreLoger.Warn("FedEx WebService Error: " + ex.Message);
                }
            }
            ////if shipfrom country <> shipto country, method display FedEx International Ground
            //if (packingList.ShipTo.CountryCode != packingList.ShipFrom.CountryCode)
            //{
            //    ShippingMethod fedexGround = fedExShippingMethods.FirstOrDefault(f => f.ServiceCode.ToUpper() == "FEDEX_GROUND");
            //    if (fedexGround != null)
            //        fedexGround.ShippingMethodDescription = "FedEx International Ground";
            //}

            return fedExShippingMethods;
        }



        private RateRequest GetFedExRequest(PackingList packlist)
        {
            RateRequest rrq = new RateRequest();

            rrq.WebAuthenticationDetail = new WebAuthenticationDetail();
            rrq.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            rrq.WebAuthenticationDetail.UserCredential.Key = _licenseNumber;
            rrq.WebAuthenticationDetail.UserCredential.Password = _password;

            rrq.ClientDetail = new ClientDetail();
            rrq.ClientDetail.AccountNumber = _userId;
            rrq.ClientDetail.MeterNumber = _shipperNumber;

            rrq.Version = new VersionId();

            rrq.ReturnTransitAndCommit = true;
            rrq.ReturnTransitAndCommitSpecified = true;

            //rrq.CarrierCodes = new CarrierCodeType[1];
            //rrq.CarrierCodes[0] = cct;

            // If programmer needs to add more FedEx shipping ways, please refer
            rrq.CarrierCodes = new CarrierCodeType[2];
            rrq.CarrierCodes[0] = CarrierCodeType.FDXE;         //FedEx Express
            rrq.CarrierCodes[1] = CarrierCodeType.FDXG;         //FedEx Ground

            SetShipmentDetails(rrq, packlist);

            return rrq;
        }

        private void SetShipmentDetails(RateRequest rrq, PackingList packlist)
        {
            rrq.RequestedShipment = new RequestedShipment();
            rrq.RequestedShipment.ShipTimestamp = DateTime.Now;
            rrq.RequestedShipment.ShipTimestampSpecified = true;
            rrq.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;

            rrq.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING;
            rrq.RequestedShipment.PackagingTypeSpecified = true;

            SetSender(rrq, packlist.ShipFrom);

            SetRecipient(rrq, packlist.ShipTo);

            SetPackageLineItems(rrq, packlist);

            rrq.RequestedShipment.RateRequestTypes = new RateRequestType[1];
            rrq.RequestedShipment.RateRequestTypes[0] = RateRequestType.LIST;

            int packageCount = rrq.RequestedShipment.RequestedPackageLineItems.Length;// crt.cartDetails.Count;

            rrq.RequestedShipment.PackageCount = packageCount.ToString();
        }

        private void SetSender(RateRequest rrq, CartContact contact)
        {
            String address1 = String.Empty;
            String city = String.Empty;
            String stateprovince = String.Empty;
            String zip = String.Empty;
            String country = String.Empty;


            address1 = String.IsNullOrEmpty(contact.Address1) ? "" : contact.Address1.Trim();
            city = String.IsNullOrEmpty(contact.City) ? "" : contact.City.Trim();
            stateprovince = String.IsNullOrEmpty(contact.State) ? "" : contact.State.Trim();
            zip = String.IsNullOrEmpty(contact.ZipCode) ? "" : contact.ZipCode.Trim();
            country = contact.countryCodeX;

            //if (!BS.CountryIsEMEA(crt.cartHeader.B_Country))
            //{
            //    address1 = Common.AppSetting("OriginAddress1_US");
            //    city = Common.AppSetting("OriginCity_US");
            //    stateprovince = Common.AppSetting("OriginState_US");
            //    zip = Common.AppSetting("OriginZip_US");
            //    country = Common.AppSetting("OriginCountry_US");
            //}
            //else
            //{
            //    address1 = Common.AppSetting("OriginAddress1_EMEA");
            //    city = Common.AppSetting("OriginCity_EMEA");
            //    stateprovince = Common.AppSetting("OriginState_EMEA");
            //    zip = Common.AppSetting("OriginZip_EMEA");
            //    country = Common.AppSetting("OriginCountry_EMEA");
            //}

            rrq.RequestedShipment.Shipper = new Party();
            rrq.RequestedShipment.Shipper.Address = new FedexRateWebServiceNew.Address();
            rrq.RequestedShipment.Shipper.Address.StreetLines = new string[1] { address1 };
            rrq.RequestedShipment.Shipper.Address.City = city;
            rrq.RequestedShipment.Shipper.Address.StateOrProvinceCode = stateprovince;
            rrq.RequestedShipment.Shipper.Address.PostalCode = zip;
            rrq.RequestedShipment.Shipper.Address.CountryCode = country;
        }


        private void SetRecipient(RateRequest rrq, CartContact contact)
        {
            var address1 = String.IsNullOrEmpty(contact.Address1) ? "" : contact.Address1.Trim();
            var city = String.IsNullOrEmpty(contact.City) ? "" : contact.City.Trim();
            var stateprovince = String.IsNullOrEmpty(contact.State) ? "" : contact.State.Trim();
            var zip = String.IsNullOrEmpty(contact.ZipCode) ? "" : contact.ZipCode.Trim();
            var country = contact.countryCodeX;

            rrq.RequestedShipment.Recipient = new Party();
            rrq.RequestedShipment.Recipient.Address = new FedexRateWebServiceNew.Address();
            rrq.RequestedShipment.Recipient.Address.StreetLines = new string[1] { address1 };
            rrq.RequestedShipment.Recipient.Address.City = city;
            rrq.RequestedShipment.Recipient.Address.StateOrProvinceCode = stateprovince;
            rrq.RequestedShipment.Recipient.Address.PostalCode = zip;
            rrq.RequestedShipment.Recipient.Address.CountryCode = country;
        }

        internal void SetPackageLineItems(RateRequest rrq, PackingList packlist)
        {
            //bool useKG = crt.cartHeader.B_Country != "US";

            //String wghtUnits = useKG ? "KG" : "LB";

            //Decimal maxWeight = 100;
            Decimal minWeight = 2.0m;

            //if (useKG)
            //{
            //    maxWeight = Common.PoundsToKilograms(maxWeight);
            //}

            if (minWeight == Decimal.Zero)
            {
                minWeight = 0.5M;
            }


            rrq.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[packlist.PackagingBoxes.Count];
            int currentPackage = 0;

            //BB只用Weight 不使用dimension
            foreach (var fedexpckg in packlist.PackagingBoxes)
            {
                if (fedexpckg.Weight < minWeight)
                {
                    fedexpckg.Weight = minWeight;
                }

                RequestedPackageLineItem rpli = new RequestedPackageLineItem();
                rpli.GroupPackageCount = "1";
                rpli.SequenceNumber = (currentPackage + 1).ToString();

                Weight wght = new Weight();
                wght.Units = (this.MeasureUnitType == MeasureUnit.UnitType.METRICS) ? WeightUnits.KG : WeightUnits.LB;
                wght.UnitsSpecified = true;
                wght.Value = fedexpckg.Weight;
                wght.ValueSpecified = true;
                rpli.Weight = wght;

                //Insured value, 但AUS/ABB似乎都不使用帶0, 但可先預留
                rpli.InsuredValue = new Money();
                //FedEx金额不能超过5w
                rpli.InsuredValue.Amount = fedexpckg.InsuredValue > 50000 ? 50000 : fedexpckg.InsuredValue;
                rpli.InsuredValue.Currency = fedexpckg.InsuredCurrency;

                rrq.RequestedShipment.RequestedPackageLineItems[currentPackage] = rpli;
                currentPackage++;
            }
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
                    _sm.ShippingMethodDescription = shippingCarrier.getServiceName(_sm.ServiceCode);

                    foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                    {
                        if (shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_ACCOUNT_PACKAGE" ||
                            shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_ACCOUNT_SHIPMENT")
                        {
                            // Discount rate              
                            _sm.NegotiatedRateSurcharge = (float)Math.Ceiling((float)shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount);
                            _sm.ShippingCostWithNegotiatedRate = (float)Math.Ceiling((float)shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Amount);
                            _sm.NegotiatedRate = _sm.ShippingCostWithNegotiatedRate - _sm.NegotiatedRateSurcharge;

                            _sm.ShippingCostWithPublishedRate = _sm.ShippingCostWithNegotiatedRate * 1.4f;
                            _sm.PublishRate = _sm.NegotiatedRate * 1.4f;
                        }
                        else if (shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_LIST_PACKAGE" ||
                                    shipmentDetail.ShipmentRateDetail.RateType.ToString() == "PAYOR_LIST_SHIPMENT")
                        {
                            // Public rate

                            if (_sm.ServiceCode == "FIRST_OVERNIGHT")
                            {
                                _sm.PublishRateSurcharge = (float)shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount;
                                _sm.ShippingCostWithPublishedRate = (float)shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Amount;
                                _sm.PublishRate = _sm.ShippingCostWithPublishedRate - _sm.PublishRateSurcharge;

                                _sm.ShippingCostWithPublishedRate += 50;
                                _sm.PublishRate += 50;
                            }
                            else
                            {
                                _sm.PublishRateSurcharge = (float)shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount;
                                _sm.ShippingCostWithPublishedRate = (float)shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Amount;
                                _sm.PublishRate = _sm.ShippingCostWithPublishedRate - _sm.PublishRateSurcharge;
                            }
                        }
                        else
                        {
                            //other type which is not supported by eStore at this moment
                        }

                        _sm.UnitOfCurrency = shipmentDetail.ShipmentRateDetail.TotalNetFedExCharge.Currency;

                    }
                    _sm.Error = null;
                    _sm.PackingList = packingList;
                    //if (shippingCarrier.CarierName.Equals("FedEx_US"))
                    //{
                    //    _sm.ShippingCostWithPublishedRate += (float)markupprice;
                    //}
                    _sm.ShippingCostWithPublishedRate += (float)markupprice;
                    shippingMethods.Add(_sm);
                }

                //filter out those unsupported shipping method
                shippingMethods = filterShippingMethod(packingList.ShipTo.CountryCode, shippingMethods);
                fixShippingMethod(shippingMethods, "FEDEX", packingList.ShipTo);

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
            else if (errMsg.Contains("9999"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.FedEXServerCommunicationError;
            }
            else if (errMsg.Contains("Object reference not set "))
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



        private List<ShippingMethod> parseFedExException(string ex, PackingList packingList)
        {
            List<ShippingMethod> shippingMethods = new List<ShippingMethod>();
            ShippingMethod _sm = new ShippingMethod();
            //_sm.ServiceCode = rateDetail.ServiceType.ToString();
            _sm.ShippingCarrier = CarrierName;
            _sm.ShippingMethodDescription = shippingCarrier.getServiceName(_sm.ServiceCode);
            _sm.Error = getShippingMethodError(ex);
            _sm.ShippingMethodDescription = "FedEx WebService Error: " + ex;
            _sm.PackingList = packingList;
            shippingMethods.Add(_sm);

            return shippingMethods;
        }





    }
}
