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

namespace eStore.BusinessModules
{
    public class ShippingCarrierABBUPS : Carrier
    {
        //Dimenisonal weigth base is used to calculate packing box dimensional weight
        //this rule is only applicable to US so far.
        //Dimensional Weight is L*W*H / dimensionalWeightBase
        private static decimal USDomesticDWB = 166;
        private static decimal USInternalDWB = 139;
        private const int maximumNumerberOfPackages = 50;



        private const int DEFAULT_TIMEOUT = 20000;
        private const string DEVELOPMENT_RATES_URL = "https://wwwcie.ups.com/ups.app/xml/Rate";
        private const string PRODUCTION_RATES_URL = "https://onlinetools.ups.com/ups.app/xml/Rate";
        private bool _useNegotiatedRates = false;
        private bool _useRetailRates = false;
        private bool _useProduction = true;
        private readonly string _licenseNumber;
        private readonly string _password;
        private readonly Hashtable _serviceCodes = new Hashtable(12);
        private readonly string _serviceDescription;
        private readonly string _shipperNumber;
        private readonly int _timeout;
        private readonly string _userId;

        private string RatesUrl
        {
            get { return UseProduction ? PRODUCTION_RATES_URL : DEVELOPMENT_RATES_URL; }
        }

        public bool UseRetailRates
        {
            get { return _useRetailRates; }
            set { _useRetailRates = value; }
        }

        public bool UseNegotiatedRates
        {
            get { return _useNegotiatedRates; }
            set { _useNegotiatedRates = value; }
        }

        public bool UseProduction
        {
            get { return _useProduction; }
            set { _useProduction = value; }
        }


        private static Boolean getNegotiationRate = false;

        public ShippingCarrierABBUPS(POCOS.Store store, ShippingCarier shippingCarrier) : base(store, shippingCarrier)
        {
            _licenseNumber = shippingCarrier.AccessKey;
            _userId = shippingCarrier.LoginAccount;
            _password = shippingCarrier.Password;
            _shipperNumber = shippingCarrier.ShipperAccount;
            _timeout = DEFAULT_TIMEOUT;
            _serviceDescription = "";

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
            List<ShippingMethod> _upsShippingMethods = getUPSrates(cart, packingList);

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, packingList, _upsShippingMethods);

            return _upsShippingMethods;
        }


        private List<ShippingMethod> getUPSrates(Cart cart, PackingList packlist)
        {
            List<ShippingMethod> _upsShippingMethods = new List<ShippingMethod>();

            XElement xeRequest;
            XElement xeResponse;
            XElement xeUPSRequest = GetUPSRequest(packlist);

            xeRequest = new XElement("RootTagJustForDebugging");
            xeRequest.Add(AccessRequestElement); // api key, user id , password
            xeRequest.Add(xeUPSRequest);

            try
            {

                xeResponse = SendRequest(AccessRequestElement, xeUPSRequest);

                try
                {
                    saveRequest(cart, xeRequest, xeResponse);
                }
                catch (Exception)
                {


                }


                var accountValid = false;

                if (xeResponse.HasElements)
                {
                    if (xeResponse.Descendants("RatedShipment").Count() > 0)
                    {
                        //if (!String.IsNullOrWhiteSpace(crt.cartHeader.CarrierAccount))
                        //{
                        //    // need to check response messages to see if the account was valid
                        //    //<RatedShipmentWarning>Missing / Invalid Shipper Number. Returned rates are Retail Rates.</RatedShipmentWarning>

                        //    bool foundWarning = false;

                        //    foreach (XElement xeWarning in xeResponse.Descendants("RatedShipmentWarning"))
                        //    {
                        //        if (xeWarning.Value.Equals("Missing / Invalid Shipper Number. Returned rates are Retail Rates.", StringComparison.OrdinalIgnoreCase))
                        //        {
                        //            foundWarning = true;
                        //        }
                        //    }

                        //    if (!foundWarning)
                        //    {
                        //        accountValid = true;
                        //    }
                        //}


                        //List<String> preventMethods = Common.AppSettingList("UPSPreventMethods", ",");

                        foreach (XElement xe in xeResponse.Descendants("RatedShipment"))
                        {

                            String serviceCode = xe.Descendants("Code").First().Value;
                            string methodName = shippingCarrier.getServiceName(serviceCode);


                            //String methodName = "";
                            //UPSShippingMethods.TryGetValue(serviceCode, out methodName);

                            //if (methodName == null)
                            //{
                            //    methodName = serviceCode;
                            //}

                            //String forMethodMatch = methodName.Replace("<sup>&reg;</sup>", "");

                            // if we have a valid carrier account number / shipping number, then we aren't going to show any rates at all
                            if (accountValid)
                            {
                                //if (!preventMethods.Contains(forMethodMatch + " Collect"))
                                //{
                                //    lstMethods.Add(new ShippingMethod("UPS", methodName, Decimal.Zero));
                                //}
                            }
                            else
                            {


                                Decimal rate = System.Decimal.Zero;
                                //Decimal negotiatedRate = System.Decimal.Zero;
                                Decimal.TryParse(xe.Descendants("TotalCharges").First().Element("MonetaryValue").Value, out rate);
                                //Decimal.TryParse(xe.Descendants("TotalCharges").First().Element("MonetaryValue").Value, out negotiatedRate);

                                Decimal InsuredCharge = System.Decimal.Zero;
                                Decimal.TryParse(xe.Descendants("ServiceOptionsCharges").First().Element("MonetaryValue").Value, out InsuredCharge);

                                string currencyCode = xe.Descendants("TotalCharges").First().Element("CurrencyCode").Value;

                                Decimal markup = 7.0m;

                                //if (markup > Decimal.Zero)
                                //{
                                //    //bool amountDiscount = Common.AppSetting("US_UPS_MarkupType").Equals("Amount", StringComparison.OrdinalIgnoreCase);
                                //    bool amountDiscount = true;

                                //    if (amountDiscount)
                                //    {
                                //        rate += markup;
                                //    }
                                //    else
                                //    {
                                //        rate = rate * ((100.00M + markup) / 100.00M);
                                //    }
                                //}

                                //lstMethods.Add(new ShippingMethod("UPS", methodName, rate));

                                //get rate response from UPS
                                ShippingMethod _sm = new ShippingMethod();
                                _sm.PackingList = packlist;
                                _sm.ServiceCode = serviceCode;
                                _sm.ShippingCarrier = CarrierName;
                                _sm.ShippingMethodDescription = methodName;
                                _sm.PublishRate = (float)Convert.ToDouble(rate);
                                //_sm.NegotiatedRate = (negotiatedRate == null) ? 0f : (float)Convert.ToDouble(a.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                                _sm.NegotiatedRate = 0f;
                                _sm.InsuredCharge = (float)Convert.ToDouble(InsuredCharge);
                                _sm.ShippingCostWithPublishedRate = (float)Math.Round(rate + markup, 2);
                                //_sm.ShippingCostWithNegotiatedRate = (a.NegotiatedRateCharges == null) ? 0f : (float)Math.Round(_sm.NegotiatedRate, 0);
                                _sm.ShippingCostWithNegotiatedRate = 0f;
                                _sm.UnitOfCurrency = currencyCode;

                                if (getNegotiationRate)
                                    _sm.ShippingCostWithPublishedRate = _sm.ShippingCostWithNegotiatedRate;

                                _sm.Error = null;
                                _upsShippingMethods.Add(_sm);

                            }

                        }
                        _upsShippingMethods = fixUPSShippingMethod(_upsShippingMethods);

                        //filter out those unsupported shipping method
                        List<ShippingMethod> filteredshipmethods = filterShippingMethod(packlist.ShipTo.countryCodeX, _upsShippingMethods);
                        fixShippingMethod(filteredshipmethods, "UPS", packlist.ShipTo);
                        _upsShippingMethods = filteredshipmethods;
                    }


                    if (xeResponse.Descendants("Error").Count() > 0)
                    {
                        ShippingMethod _sm = new ShippingMethod();
                        _sm.PackingList = packlist;
                        foreach (XElement xe in xeResponse.Descendants("Error"))
                        {
                            _sm.Error = getShippingMethodError(xe.Descendants("ErrorCode").First().Value);
                            string message = xe.Descendants("ErrorDescription").First().Value;
                            _sm.ShippingMethodDescription = "UPS XML Request Error: " + message; 
                            _upsShippingMethods.Add(_sm);
                            eStoreLoger.Error("UPS XML Request Error: " + message, "", "", "");
                            break;
                        }
                    }

                }
                else
                    throw new Exception("Failed to get UPS rate response.");          

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
                //_sm.ShippingMethodDescription = "General Error";
                _sm.ShippingMethodDescription = "UPS XML Request Error: " + ex.Message;
                _upsShippingMethods.Add(_sm);
                eStoreLoger.Error("UPS XML Request Error: " + ex.Message, "", "", "", ex);
            }

            return _upsShippingMethods;
        }

        /// <summary>
        /// Sends a request to UPS and retrieves the response.
        /// </summary>
        /// <param name="xeRequest">The request XML to send to UPS.</param>
        /// <returns>The response returned from UPS.</returns>
        internal XElement SendRequest(XElement xeAccess, XElement xeRequest)
        {
            // get the URL from the web.config
            //bool isProductionMode = Common.AppSettingBool("UPSProductionMode");

            //String upsURL = isProductionMode ? Common.AppSetting("UPSProductionServer") : Common.AppSetting("UPSTestServer");

            XElement xeShipResponse = new XElement("RootTagJustForDebugging");

            try
            {
                // create the request
                HttpWebRequest shipRequest = WebRequest.Create(RatesUrl) as HttpWebRequest;

                String bytestring = xeAccess.ToString(SaveOptions.DisableFormatting) + xeRequest.ToString(SaveOptions.DisableFormatting);

                // create encoding to convert the request xml to a byte array
                ASCIIEncoding aseno = new ASCIIEncoding();
                byte[] reqBytes = aseno.GetBytes(bytestring);

                // set the request properties
                shipRequest.ContentType = "application/x-www-form-urlencoded";
                shipRequest.ContentLength = reqBytes.Length;
                shipRequest.Method = "POST";
                shipRequest.ServicePoint.ConnectionLimit = 200;
                shipRequest.Timeout = DEFAULT_TIMEOUT;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                // get the request stream
                using (Stream reqStream = shipRequest.GetRequestStream())
                {
                    // write the byte array into the stream
                    reqStream.Write(reqBytes, 0, reqBytes.Length);

                    // close the stream...will be disposed by using statement
                    reqStream.Close();
                }

                // get the response from the request
                using (HttpWebResponse shipResponse = shipRequest.GetResponse() as HttpWebResponse)
                {
                    // get the response stream from the response
                    using (Stream shipResponseStream = shipResponse.GetResponseStream())
                    {
                        // create a stream reader from the response stream
                        using (StreamReader shipResponseReader = new StreamReader(shipResponseStream, Encoding.UTF8))
                        {
                            // read the response xml from the stream reader
                            //xeShipResponse.Add(XElement.Parse(shipResponseReader.ReadToEnd()));
                            xeShipResponse.Add(XElement.Load(shipResponseReader));

                            // close the stream reader...will be disposed by using statement
                            shipResponseReader.Close();
                        }

                        // close the stream...will be disposed by using statement
                        shipResponseStream.Close();
                    }

                    // close the httpwebresponse...will be disposed by using statement
                    shipResponse.Close();
                }
            }
            catch (Exception ex)
            {
                // Add the exception message to the response xml
                xeShipResponse = XElement.Parse("<Message>" + ex.Message + "</Message>");
            }

            // return the response
            return xeShipResponse;
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

        private void saveRequest(Cart cart, XElement req, XElement resp)
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
            TextWriter textWriter = new StreamWriter(string.Format("{0}\\{1}-request.xml", filePath.ToString(), cart.CartID));
            req.Save(textWriter);
            textWriter.Close();

            TextWriter textWriter2 = new StreamWriter(string.Format("{0}\\{1}-response.xml", filePath.ToString(), cart.CartID));
            resp.Save(textWriter2);
            //serializer2.Serialize(textWriter2, resp);
            textWriter2.Close();
        }

        internal XElement RequestElement
        {
            get
            {
                XElement xeRequest = new XElement("Request",
                    new XElement("TransactionReference",
                        new XElement("CustomerContext", "Rating and Service"),
                        new XElement("XpciVersion", "1.0")),
                    new XElement("RequestAction", "Shop"),
                    new XElement("RequestOption", "Shop"));

                return xeRequest;
            }
        }

        internal XElement PickupTypeElement
        {
            get
            {
                String upsPickupTypeCode = "06";
                String upsPickupTypeDescription = "";

                switch (upsPickupTypeCode)
                {
                    case "20":
                        upsPickupTypeDescription = "Air Service Center";
                        break;
                    case "19":
                        upsPickupTypeDescription = "Letter Center";
                        break;
                    case "07":
                        upsPickupTypeDescription = "On Call Air";
                        break;
                    case "06":
                        upsPickupTypeDescription = "One Time Pickup";
                        break;
                    case "03":
                        upsPickupTypeDescription = "Customer Counter";
                        break;
                    case "01":
                    default:
                        upsPickupTypeDescription = "Daily Pickup";
                        break;
                }

                XElement xePickupType = new XElement("PickupType",
                    new XElement("Code", upsPickupTypeCode),
                    new XElement("Description", upsPickupTypeDescription));

                return xePickupType;
            }
        }


        internal XElement CustomerClassificationElement
        {
            get
            {
                XElement xeCustClass = new XElement("CustomerClassification",
                    new XElement("Code", "00"));

                return xeCustClass;
            }
        }

        internal XElement AccessRequestElement
        {
            get
            {
                String upsLicense = _licenseNumber;
                String upsUser = _userId;
                String upsPassword = _password;

                XElement xeAccessRequest = new XElement("AccessRequest",
                    new XElement("AccessLicenseNumber", upsLicense),
                    new XElement("UserId", upsUser),
                    new XElement("Password", upsPassword));

                return xeAccessRequest;
            }
        }

        internal XElement ShipperElement(CartContact contact)
        {
            XElement xeShipper = new XElement("Shipper");

            String shipperNumber = shippingCarrier.ShipperAccount;

            if (!String.IsNullOrWhiteSpace(shipperNumber))
            {
                xeShipper.Add(new XElement("ShipperNumber", shipperNumber));

                //xeShipper.Add(MakeAddressElement(crt.cartHeader.B_Address, crt.cartHeader.B_Address2, "", crt.cartHeader.B_City, crt.cartHeader.B_State, crt.cartHeader.B_Zip, crt.cartHeader.B_Country));
            }
            else
            {
                String address1 = String.Empty;
                String address2 = String.Empty;
                String city = String.Empty;
                String stateprovince = String.Empty;
                String zip = String.Empty;
                String country = String.Empty;

                //if (!BS.CountryIsEMEA(crt.cartHeader.B_Country))
                if (true)
                {
                    address1 = String.IsNullOrEmpty(contact.Address1) ? "" : contact.Address1.Trim();
                    address2 = String.IsNullOrEmpty(contact.Address2) ? "" : contact.Address2.Trim();


                    city = String.IsNullOrEmpty(contact.City) ? "" : contact.City.Trim();
                    stateprovince = String.IsNullOrEmpty(contact.State) ? "" : contact.State.Trim();
                    zip = String.IsNullOrEmpty(contact.ZipCode) ? "" : contact.ZipCode.Trim();
                    country = contact.countryCodeX;
                }
                //else
                //{


                //    address1 = Common.AppSetting("OriginAddress1_EMEA");
                //    city = Common.AppSetting("OriginCity_EMEA");
                //    stateprovince = Common.AppSetting("OriginState_EMEA");
                //    zip = Common.AppSetting("OriginZip_EMEA");
                //    country = Common.AppSetting("OriginCountry_EMEA");
                //}

                xeShipper.Add(MakeAddressElement(address1, address1, "", city, stateprovince, zip, country));
            }

            return xeShipper;
        }

        internal XElement ShipToElement(CartContact contact)
        {
            var city = String.IsNullOrEmpty(contact.City) ? "" : contact.City.Trim();
            var state = String.IsNullOrEmpty(contact.State) ? "" : contact.State.Trim();
            var zip = String.IsNullOrEmpty(contact.ZipCode) ? "" : contact.ZipCode.Trim();


            XElement xeShipTo = new XElement("ShipTo",
                new XElement("CompanyName", contact.AttCompanyName),
                new XElement("AttentionName", ""),
                new XElement("PhoneNumber", contact.TelNo),
                MakeAddressElement(contact.Address1, "", "", city, state, zip, contact.countryCodeX));

            return xeShipTo;
        }

        internal XElement ShipFromElement(CartContact contact)
        {
            XElement xeShipFrom = new XElement("ShipFrom",
                new XElement("CompanyName", ""),
                new XElement("AttentionName", ""),
                new XElement("PhoneNumber", ""));

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

            xeShipFrom.Add(MakeAddressElement(address1, "", "", city, stateprovince, zip, country));

            return xeShipFrom;
        }

        internal XElement MakeAddressElement(String address1, String address2, String address3, String city, String stateProvince, String zip, String country)
        {
            XElement xeAddress = new XElement("Address",
                new XElement("AddressLine1", address1),
                new XElement("AddressLine2", address2),
                new XElement("AddressLine3", address3),
                new XElement("City", city));

            if (!String.IsNullOrWhiteSpace(stateProvince))
            {
                if (country == "US" || country == "USA" || country.Equals("United States", StringComparison.OrdinalIgnoreCase))
                {
                    xeAddress.Add(new XElement("StateProvinceCode", stateProvince));
                }
            }

            xeAddress.Add(new XElement("PostalCode", zip), new XElement("CountryCode", country));

            return xeAddress;
        }

        internal XElement MakePackageElements(PackingList packlist)
        {
            //bool useKG = BS.CountryIsEMEA(c.cartHeader.B_Country);

            //String weightUnits = useKG ? "KGS" : "LBS";

            XElement xePackages = new XElement("Temp");

            Decimal minWeight = 2.0m;

            //if (useKG)
            //{
            //    maxWeight = Common.PoundsToKilograms(maxWeight);
            //}

            if (minWeight == Decimal.Zero)
            {
                minWeight = 0.5M;
            }


            foreach (var upspckg in packlist.PackagingBoxes)
            {
                if (upspckg.Weight < minWeight)
                {
                    upspckg.Weight = minWeight;
                }

                XElement xePackage = new XElement("Package",
                    new XElement("PackagingType",
                        new XElement("Code", "02"),
                        new XElement("Description", "")),
                        new XElement("PackageWeight",
                        new XElement("UnitOfMeasurement",
                            new XElement("Code", weightUnit)),
                        new XElement("Weight", upspckg.Weight)));

                xePackages.Add(xePackage);
            }

            return xePackages;
        }


        public XElement GetUPSRequest(PackingList packlist)
        {
            XElement xeRequest = new XElement("RatingServiceSelectionRequest",
                            RequestElement,
                            PickupTypeElement,
                            CustomerClassificationElement,
                            new XElement("Shipment",
                                new XElement("Description", "Rate Shopping - Domestic"),
                                ShipperElement(packlist.ShipFrom),
                                ShipToElement(packlist.ShipTo),
                                ShipFromElement(packlist.ShipFrom),
                                MakePackageElements(packlist).Elements(),
                                new XElement("ShipmentServiceOptions")));

            return xeRequest;
        }

    }

}