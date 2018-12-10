using System.Web;
using eStore.POCOS;
using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace eStore.BusinessModules
{
    public class ShippingCarrierABBUSPS: Carrier
    {
        private readonly string _loginAccount;
        private const int DEFAULT_TIMEOUT = 20000;

        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            //int dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.Country);
            decimal dimensionalWeightBase = getDimensionWeightBase(cart.ShipToContact.countryCodeX);
            if(cart.ShipToContact.countryCodeX == "CA")
            {
                return new List<ShippingMethod>();
            }
            //Step 1. Shipping carrier must get the packing list via packing manager
            PackingManager packingManager = new PackingManager(_store);
            PackingList packingList = packingManager.getPackingList(cart, dimensionalWeightBase);

            if (packingList == null || packingList.getItemCount() == 0)
                return null;

            //Step 2. send packing list to shipping carrier, shipping carrier return list of shipping methods
            List<ShippingMethod> _USPSShippingMethods = getUSPSRate(cart, packingList);

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, packingList, _USPSShippingMethods);

            return _USPSShippingMethods;
        }

        public ShippingCarrierABBUSPS(POCOS.Store store, ShippingCarier shippingCarrier) : base(store, shippingCarrier)
        {
            _loginAccount = shippingCarrier.LoginAccount;

        }


        private List<ShippingMethod> getUSPSRate(Cart cart, PackingList packingList)
        {
            var shippingMethods = new List<ShippingMethod>();

            //ThreeDimensionBoxing boxing = new ThreeDimensionBoxing();
            //boxing.AddExtraLengthIsRequired = true;


            //var containers = boxing.Boxing(storeName, boxes);



            XElement xeRequest;
            XElement xeResponse;
            xeRequest = new XElement("RootTagJustForDebugging");
            xeResponse = new XElement("RootTagJustForDebugging");

            bool isInternational = cart.ShipToContact.countryCodeX != "US";

            XElement xeUSPSRequest = GetUSPSRequest(cart, packingList, isInternational);

            xeRequest.Add(xeUSPSRequest);
            try
            {

                xeResponse = SendRequest(xeUSPSRequest, isInternational);
                try
                {
                    saveRequest(cart, xeRequest, xeResponse);
                }
                catch(Exception)
                {

                }

                if (xeResponse.HasElements)
                {
                    if (xeResponse.Descendants("Package").Count() > 0)
                    {
                        //List<String> preventMethods = Common.AppSettingList("USPSPreventMethods", ",");
                        List<String> filterMethods = new List<String>();
                        bool filterAllow = false;

                        //if (Common.AppSetting("USPSFilterMethod").Equals("Allow", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    filterAllow = true;
                        //    filterMethods = Common.AppSettingList("USPSAllowMethods", ",");
                        //}
                        //else
                        //{
                        //    filterAllow = false;
                        //    filterMethods = Common.AppSettingList("USPSPreventMethods", ",");
                        //}

                        String methodPrefix = "U.S. Postal ";

                        foreach (XElement xePackage in xeResponse.Descendants("Package"))
                        {
                            if (isInternational)
                            {
                                foreach (XElement xeService in xePackage.Descendants("Service"))
                                {
                                    String methodName = HttpUtility.HtmlDecode(xeService.Element("SvcDescription").Value);

                                    var serviceCode = methodName.Replace("<sup>&reg;</sup>", "").Replace("<sup>&trade;</sup>", "").Replace("<sup>™</sup>", "").Replace("<sup>®</sup>", "");

                                    Decimal rate = System.Decimal.Zero;
                                    Decimal InsuredCharge = System.Decimal.Zero;
                                    Decimal.TryParse(xeService.Element("Postage").Value, out rate);
                                    Decimal markup = 7.0m;
                                    ShippingMethod _sm = new ShippingMethod();

                                    if (shippingMethods.Exists(sm => sm.ServiceCode.Equals(serviceCode)))
                                    {
                                        shippingMethods.Find(sm => sm.ServiceCode.Equals(serviceCode)).PublishRate += (float)Convert.ToDouble(rate);
                                        shippingMethods.Find(sm => sm.ServiceCode.Equals(serviceCode)).ShippingCostWithPublishedRate += (float)Convert.ToDouble(rate);
                                    }
                                    else
                                    {
                                        _sm.PackingList = packingList;
                                        _sm.ServiceCode = serviceCode;
                                        _sm.ShippingCarrier = CarrierName;
                                        _sm.ShippingMethodDescription = shippingCarrier.getServiceName(serviceCode);
                                        _sm.PublishRate = (float)Convert.ToDouble(rate);
                                        //_sm.NegotiatedRate = (negotiatedRate == null) ? 0f : (float)Convert.ToDouble(a.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                                        _sm.NegotiatedRate = 0f;
                                        _sm.InsuredCharge = (float)Convert.ToDouble(InsuredCharge);
                                        _sm.ShippingCostWithPublishedRate = (float)Math.Round(rate + markup, 2);
                                        //_sm.ShippingCostWithNegotiatedRate = (a.NegotiatedRateCharges == null) ? 0f : (float)Math.Round(_sm.NegotiatedRate, 0);
                                        _sm.ShippingCostWithNegotiatedRate = 0f;
                                        _sm.UnitOfCurrency = currencyCode;


                                        _sm.Error = null;
                                        shippingMethods.Add(_sm);
                                    }
                                }
                            }
                            else
                            {
                                foreach (XElement xePostage in xePackage.Descendants("Postage"))
                                {
                                    String methodName = HttpUtility.HtmlDecode(xePostage.Element("MailService").Value);

                                    var serviceCode = methodName.Replace("<sup>&reg;</sup>", "").Replace("<sup>&trade;</sup>", "").Replace("<sup>™</sup>", "").Replace("<sup>®</sup>", "");

                                    Decimal rate = System.Decimal.Zero;
                                    Decimal.TryParse(xePostage.Element("Rate").Value, out rate);

                                    Decimal InsuredCharge = System.Decimal.Zero;
                                    Decimal markup = 7.0m;
                                    ShippingMethod _sm = new ShippingMethod();

                                    if (shippingMethods.Exists(sm => sm.ServiceCode.Equals(serviceCode)))
                                    {
                                        shippingMethods.Find(sm => sm.ServiceCode.Equals(serviceCode)).PublishRate += (float)Convert.ToDouble(rate);
                                        shippingMethods.Find(sm => sm.ServiceCode.Equals(serviceCode)).ShippingCostWithPublishedRate += (float)Convert.ToDouble(rate);
                                    }
                                    else
                                    {
                                        _sm.PackingList = packingList;
                                        _sm.ServiceCode = serviceCode;
                                        _sm.ShippingCarrier = CarrierName;
                                        _sm.ShippingMethodDescription = shippingCarrier.getServiceName(serviceCode);
                                        _sm.PublishRate = (float)Convert.ToDouble(rate);
                                        //_sm.NegotiatedRate = (negotiatedRate == null) ? 0f : (float)Convert.ToDouble(a.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                                        _sm.NegotiatedRate = 0f;
                                        _sm.InsuredCharge = (float)Convert.ToDouble(InsuredCharge);
                                        _sm.ShippingCostWithPublishedRate = (float)Math.Round(rate + markup, 2);
                                        //_sm.ShippingCostWithNegotiatedRate = (a.NegotiatedRateCharges == null) ? 0f : (float)Math.Round(_sm.NegotiatedRate, 0);
                                        _sm.ShippingCostWithNegotiatedRate = 0f;
                                        _sm.UnitOfCurrency = currencyCode;


                                        _sm.Error = null;
                                        shippingMethods.Add(_sm);

                                    }



                                }
                            }
                        }

                        //filter out those unsupported shipping method
                        List<ShippingMethod> filteredshipmethods = filterShippingMethod(packingList.ShipTo.countryCodeX, shippingMethods);
                        fixShippingMethod(filteredshipmethods, "USPS", packingList.ShipTo);
                        shippingMethods = filteredshipmethods;
                    }

                    if (xeResponse.Descendants("Error").Count() > 0)
                    {
                        xeResponse.Descendants("Error").Descendants("Description");


                        var error = (from er in xeResponse.Descendants("Error")
                                     select new PackageError
                                     {
                                         Number = er.Element("Number") == null ? "" : (string)er.Element("Number"),
                                         Source = er.Element("Source") == null ? "" : (string)er.Element("Source"),
                                         Description = er.Element("Description") == null ? "" : (string)er.Element("Description"),
                                         HelpFile = er.Element("HelpFile") == null ? "" : (string)er.Element("HelpFile"),
                                         HelpContext = er.Element("HelpContext") == null ? "" : (string)er.Element("HelpContext"),
                                     }).FirstOrDefault();

                        ShippingMethod _sm = new ShippingMethod();
                        _sm.PackingList = packingList;
                        _sm.Error = getShippingMethodError(error.Number);
                        _sm.ShippingMethodDescription = "USPS XML Request Error: " + error.Description;
                        shippingMethods.Add(_sm);
                        eStoreLoger.Error("USPS XML Request Error: " + error.Description, "", "", "");

                        //throw new Exception(string.Format(
                        //    "USPS service not available - Error-Number: {0}, Error-Source: {1}, Error-Description: {2}, Error-HelpFile: {3}, Error-HelpContext: {4}, please choose UPS or FeDEX solution",
                        //    error.Number, error.Source, error.Description, error.HelpFile, error.HelpContext));
                    }
                }
                else
                {
                    throw new Exception("Failed to get USPS rate response.");
                }

            }
            catch (Exception ex)
            {
                ShippingMethod _sm = new ShippingMethod();
                _sm.PackingList = packingList;
                _sm.Error = getShippingMethodError(ex.Message);
                _sm.ShippingMethodDescription = "USPS XML Request Error: " + ex.Message;
                shippingMethods.Add(_sm);
                eStoreLoger.Error("USPS XML Request Error: " + ex.Message, "", "", "", ex);
            }

            return shippingMethods;
        }


        internal XElement GetUSPSRequest(Cart crt, PackingList packingList, bool isInternational)
        {
            String uspsUser = _loginAccount;

            XElement xeRequest = new XElement("root");

            if (isInternational)
            {
                xeRequest = new XElement("IntlRateV2Request",
                    new XAttribute("USERID", uspsUser),
                    new XElement("Revision", "2"));
            }
            else
            {
                xeRequest = new XElement("RateV4Request",
                    new XAttribute("USERID", uspsUser),
                    new XElement("Revision", "2"));
            }

            XElement xeCartDetail = GetCartDetail(crt, packingList, isInternational);

            if (xeCartDetail.HasElements)
            {
                foreach (XElement xePackage in xeCartDetail.Descendants("Package"))
                {
                    xeRequest.Add(xePackage);
                }
            }
            //foreach(CartDetail cd in crt.cartDetails)
            //{
            //    xeRequest.Add(GetCartDetail(cd, crt.cartHeader.S_Zip));
            //}

            return xeRequest;
        }

        internal XElement GetCartDetail(Cart crt, PackingList packingList, bool isInternational)
        {
            XElement xePackages = new XElement("Packages");


            Decimal minWeight = 2.0m;

            if (minWeight == Decimal.Zero)
            {
                minWeight = 0.5M;
            }


            foreach (var package in packingList.PackagingBoxes)
            {
                if (package.Weight < minWeight)
                {
                    package.Weight = minWeight;
                }

                int lbs = 0;
                int ozs = 0;

                PoundsAndOuncesFromWeight(package.Weight, out lbs, out ozs);

                if (isInternational)
                {
                    XElement xePackage = new XElement("Package",
                        new XAttribute("ID", package.BoxId),
                        new XElement("Pounds", lbs),
                        new XElement("Ounces", ozs),
                        new XElement("Machinable", "false"),
                        new XElement("MailType", "Package"),
                        new XElement("ValueOfContents", ""),
                        new XElement("Country", packingList.ShipTo.countryX),
                        new XElement("Container", ""),
                        new XElement("Size", "REGULAR"),
                        new XElement("Width", ""),
                        new XElement("Length", ""),
                        new XElement("Height", ""),
                        new XElement("Girth", ""),
                        new XElement("CommercialFlag", "N")
                    );

                    xePackages.Add(xePackage);
                }
                else
                {
                    XElement xePackage = new XElement("Package",
                        new XAttribute("ID", package.BoxId),
                        new XElement("Service", "ALL"),
                        new XElement("ZipOrigination", packingList.ShipFrom.ZipCode),
                        new XElement("ZipDestination", packingList.ShipTo.ZipCode),
                        new XElement("Pounds", lbs),
                        new XElement("Ounces", ozs),
                        new XElement("Container", ""),
                        new XElement("Size", "REGULAR"),
                        new XElement("Machinable", "false")
                    );

                    xePackages.Add(xePackage);
                }
            }

            return xePackages;
        }



        /// <summary>
        /// Sends a request to USPS and retrieves the response.
        /// </summary>
        /// <param name="xeRequest">The request XML to send to USPS.</param>
        /// <returns>The response returned from USPS.</returns>
        internal XElement SendRequest(XElement xeRequest, bool isInternational)
        {
            bool isProductionMode = true;

            String uspsURL = isProductionMode ? "http://production.shippingapis.com/ShippingAPI.dll" : "http://testing.shippingapis.com/ShippingAPITest.dll";

            XElement xeShipResponse = new XElement("RootTagJustForDebugging");

            String apiString = "API=RateV4&XML=";

            if (isInternational)
            {
                apiString = "API=IntlRateV2&XML=";
            }

            try
            {
                //String bytestring = "API=RateV4&XML=" + xeRequest.ToString(SaveOptions.DisableFormatting);
                String bytestring = apiString + xeRequest.ToString(SaveOptions.DisableFormatting);

                // create the request
                HttpWebRequest shipRequest = WebRequest.Create(uspsURL) as HttpWebRequest;

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
                //ServicePointManager.SecurityProtocol =
                //    SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                //    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

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
            string folderName = "USPS";
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
            string folderName = "USPS";
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

        public static void PoundsAndOuncesFromWeight(Decimal weight, out int pounds, out int ounces)
        {
            pounds = int.Parse(Math.Floor(weight).ToString());
            Decimal weightRemain = weight - pounds;
            ounces = int.Parse(Math.Ceiling(16 * weightRemain).ToString());

            // common sense check
            if (ounces >= 16)
            {
                do
                {
                    pounds++;
                    ounces -= 16;
                } while (ounces >= 16);
            }
        }

        private ShippingMethodError getShippingMethodError(string errMsg)
        {

            ShippingMethodError err = new ShippingMethodError();

            if (errMsg.Contains("timed out"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.USPSWSTimeOut;
            }
            else if (errMsg.Contains("-2147219497") || errMsg.Contains("-2147219385"))
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.EndUser;
                err.Code = BusinessModules.ShippingMethodError.ErrorCode.MissingOrInvalidShipToZipcode;
            }
            else
            {
                err.ErrorLevelType = BusinessModules.ShippingMethodError.Type.System;
                err.Code = ShippingMethodError.ErrorCode.UnkonwError;
            }

            return err;
        }
    }
}
