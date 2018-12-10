using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Net;
using System.IO;
using System.Xml.Linq;
using eStore.POCOS.DAL;
using System.Xml;
using System.Xml.Serialization;

namespace eStore.BusinessModules
{
    public class ShippingCarrierUSPS : Carrier
    {
        private List<ShippingInstruction> _shippingInstructions;

        public ShippingCarrierUSPS(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
            : base(store, shippingCarrier)
        {
        }

        public List<ShippingInstruction> getShippingInstructions
        {
            get
            {
                return _shippingInstructions;
            }
        }
        public List<Container> getAvailableUSPSContainers()
        {
            ThreeDimensionBoxing boxing = new ThreeDimensionBoxing();
            var packages = boxing.getAvailableUSPSPackages();
            var uspsContainers = (from p in packages
                                  select new Container
                                  {
                                      ID = p.ID,
                                      PartNo = p.Description,
                                      Width = p.Length,
                                      Depth = p.Width,
                                      Height = p.Depth,
                                      Arrangements = new List<Arrangement>()
                                  }).ToList();
            return uspsContainers;
        }

        public List<ShippingMethod> getFreightEstimationX(string storeName, List<ProductShippingDimension> boxes, string fromZipCode, string toZipCode, out List<ShippingInstruction> shippingInstructions)
        {
            var shippingMethods = new List<ShippingMethod>();
            try
            {
                shippingMethods = getUSPSRate(storeName, boxes, fromZipCode, toZipCode, out shippingInstructions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return shippingMethods;
        }        

        private List<ShippingMethod> getUSPSRate(string storeName, List<ProductShippingDimension> boxes, string fromZipCode, string toZipCode, out List<ShippingInstruction> shippingInstructions)
        {
            shippingInstructions = new List<ShippingInstruction>();
            var shippingMethods = new List<ShippingMethod>();
            ThreeDimensionBoxing boxing = new ThreeDimensionBoxing();
            boxing.AddExtraLengthIsRequired = true;
            if (boxes.Count == 0) return null;

            var containers = boxing.Boxing(storeName, boxes);

            var request = prepareRequest(containers, fromZipCode, toZipCode);

            ePAPSShippingService service = new ePAPSShippingService();
            var shippingCarrier = service.getShippingCarrier(store, "USPS");
            if (shippingCarrier == null) throw new Exception("StoreCarier USPS not defined.");
            var loginAccount = shippingCarrier.LoginAccount;
            //var xmlData = prepareXMLData(request, "730ADVAN5282");
            var xmlData = prepareXMLData(request, loginAccount);
            try
            {
                var shippingMethod = RateV4(xmlData);
                if (shippingMethod != null)
                {
                    shippingMethods.Add(shippingMethod);
                    foreach (var c in containers)
                    {
                        foreach (var a in c.Arrangements)
                        {

                            var shippingInstruction = new ShippingInstruction
                            {
                                ContainerNo = c.PartNo,
                                Level = a.Level + 1,
                                ItemNo = a.ProductShippingDimension.ItemNo,
                                PartNo = a.ProductShippingDimension.PartNo,
                                ShipVia = "USPS PRIORITY MAIL"
                            };
                            shippingInstructions.Add(shippingInstruction);
                        }
                    }
                }
                else
                {
                    throw new Exception("USPS service is not available.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return shippingMethods;
        }

        public List<ShippingInstruction> getUSPSShippingInstructions(string storeName, List<ProductShippingDimension> boxes)
        {
            var shippingInstructions = new List<ShippingInstruction>();
            ThreeDimensionBoxing boxing = new ThreeDimensionBoxing();
            boxing.AddExtraLengthIsRequired = true;
            if (boxes.Count == 0) return null;

            var containers = boxing.Boxing(storeName, boxes);
            foreach (var c in containers)
            {
                foreach (var a in c.Arrangements)
                {

                    var shippingInstruction = new ShippingInstruction
                    {
                        ContainerNo = c.PartNo,
                        Level = a.Level + 1,
                        ItemNo = a.ProductShippingDimension.ItemNo,
                        PartNo = a.ProductShippingDimension.PartNo,
                        ShipVia = "USPS PRIORITY MAIL"
                    };
                    shippingInstructions.Add(shippingInstruction);
                }
            }
            return shippingInstructions;
        }

        private RateV4Request prepareRequest(List<Container> containers, string fromZipCode, string toZipCode)
        {
            var boxing = new ThreeDimensionBoxing();
            var packages = boxing.getAvailablePackages();
            var request = new RateV4Request();
            request = new RateV4Request
            {
                Revision = "2",
                Packages = new List<Package>()
            };
            var id = 1;
            foreach (var c in containers)
            {
                foreach (var b in c.Arrangements)
                {
                    var boxWeightInPounds = b.ProductShippingDimension.ShipWeightKG * 2.20462M;
                    var boxWeightPound = (int)boxWeightInPounds;
                    var boxWeightOunces = (boxWeightInPounds - boxWeightPound) * 16;
                    c.Pounds += boxWeightPound;
                    c.Ounces += boxWeightOunces;
                    while (c.Ounces >= 16)
                    {
                        c.Pounds += 1;
                        c.Ounces -= 16;
                    }
                }
                var lengths = new List<decimal> { c.Width, c.Depth, c.Height };
                var sortedLengths = lengths.OrderBy(x => x).ToArray();
                decimal girth = (sortedLengths[0] + sortedLengths[1]) * 2;
                // retrieve comtainer package info
                var package = packages.FirstOrDefault(x => x.ID == c.ID);

                var size = "REGULAR";
                if (sortedLengths[2] > 12) size = "LARGE";
                if (package == null)
                {
                    request.Packages.Add(new Package
                    {
                        ID = getOrdinalIndicator(id++),
                        Service = "PRIORITY",
                        ZipOrigination = fromZipCode,
                        ZipDestination = toZipCode,
                        Pounds = c.Pounds,
                        Ounces = c.Ounces,
                        Container = c.ContainerName,
                        //Container = "NONRECTANGULAR",
                        Size = size, //"LARGE",
                        Width = c.Width,
                        Length = c.Depth,
                        Height = c.Height,
                        Girth = girth, // length is the longest dimension. girth is the sum of 2nd and 3rd dimension times 2
                        SpecialServices = new List<string>(),
                        //SpecialServices = new List<string> { string.Format("<SpecialService>{0}</SpecialService>", 1) },
                        Machinable = "true",
                    });
                }
                else
                    request.Packages.Add(new Package
                    {
                        ID = getOrdinalIndicator(id++),
                        Service = "PRIORITY",
                        ZipOrigination = fromZipCode,
                        ZipDestination = toZipCode,
                        Pounds = c.Pounds,
                        Ounces = c.Ounces,
                        Container = package.Container, //"NONRECTANGULAR",
                        Size = size, //"LARGE",
                        Width = c.Width,
                        Length = c.Depth,
                        Height = c.Height,
                        Girth = girth, // length is the longest dimension. girth is the sum of 2nd and 3rd dimension times 2
                        SpecialServices = new List<string>(),
                        //SpecialServices = new List<string> { string.Format("<SpecialService>{0}</SpecialService>", 1) },
                        Machinable = "true",
                    });
            }

            return request;
        }

        private string getOrdinalIndicator(int id)
        {
            var ordinalIndicator = string.Empty;
            switch (id % 10)
            {
                case 1:
                    ordinalIndicator = "ST";
                    break;
                case 2:
                    ordinalIndicator = "ND";
                    break;
                case 3:
                    ordinalIndicator = "RD";
                    break;
                default:
                    ordinalIndicator = "TH";
                    break;
            }
            return string.Format("{0}{1}", id, ordinalIndicator);
        }

        private string prepareXMLData(RateV4Request request, string userId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<RateV4Request USERID='{0}'>", userId));
            sb.Append(PrepareXMLDataNode("Revision", request.Revision));
            foreach (var p in request.Packages)
            {
                sb.Append(string.Format("<Package ID='{0}'>", p.ID));
                sb.Append(PrepareXMLDataNode("Service", p.Service));
                if (!string.IsNullOrEmpty(p.FirstClassMailType))
                {
                    sb.Append(PrepareXMLDataNode("FirstClassMailType", p.FirstClassMailType));
                }
                sb.Append(PrepareXMLDataNode("ZipOrigination", p.ZipOrigination));
                sb.Append(PrepareXMLDataNode("ZipDestination", p.ZipDestination));
                if (p.Pounds != null)
                {
                    sb.Append(PrepareXMLDataNode("Pounds", p.Pounds.ToString()));
                }
                if (p.Ounces != null)
                {
                    sb.Append(PrepareXMLDataNode("Ounces", p.Ounces.ToString()));
                }
                if (string.IsNullOrEmpty(p.Container))
                    sb.Append(string.Format("<{0} />", "Container"));
                else
                {
                    sb.Append(PrepareXMLDataNode("Container", p.Container));
                }
                if (!string.IsNullOrEmpty(p.Size))
                {
                    sb.Append(PrepareXMLDataNode("Size", p.Size));
                }
                if (p.Width != null)
                {
                    sb.Append(PrepareXMLDataNode("Width", p.Width.ToString()));
                }
                if (p.Length != null)
                {
                    sb.Append(PrepareXMLDataNode("Length", p.Length.ToString()));
                }
                if (p.Height != null)
                {
                    sb.Append(PrepareXMLDataNode("Height", p.Height.ToString()));
                }
                if (p.Girth != null)
                {
                    sb.Append(PrepareXMLDataNode("Girth", p.Girth.ToString()));
                }
                if (p.Value != null)
                {
                    sb.Append(PrepareXMLDataNode("Value", p.Value.ToString()));
                }
                /* remove signature service
                if (p.SpecialServices != null && p.SpecialServices.Count() > 0)
                {
                    sb.Append("<SpecialServices>");
                    foreach (var s in p.SpecialServices)
                    {
                        sb.Append(string.Format("{0}", s));
                    }
                    sb.Append("</SpecialServices>");
                }
                */ 
                if (!string.IsNullOrEmpty(p.Machinable))
                {
                    sb.Append(PrepareXMLDataNode("Machinable", p.Machinable));
                }
                if (!string.IsNullOrEmpty(p.DropOffTime))
                {
                    sb.Append(PrepareXMLDataNode("DropOffTime", p.DropOffTime));
                }
                if (!string.IsNullOrEmpty(p.ShipDate))
                {
                    sb.Append(PrepareXMLDataNode("ShipDate", p.ShipDate));
                }
                sb.Append("</Package>");
            }

            sb.Append("</RateV4Request>");
            return sb.ToString();
        }

        private string PrepareXMLDataNode(string nodeName, string nodeValue)
        {
            return string.Format("<{0}>{1}</{0}>", nodeName, nodeValue);
        }
        /// <summary>
        /// Estimate freight charge, return false when error 
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private bool TestRateV4(string xmlData)
        {
            bool executeResult = true;
            ePAPSShippingService service = new ePAPSShippingService();
            var shippingCarrier = service.getShippingCarrier(store, "USPS");
            //string url = string.Format("http://production.shippingapis.com/ShippingAPI.dll?API=RateV4&XML={0}", xmlData);
            string url = string.Format(shippingCarrier.WebServiceURL, xmlData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var result = reader.ReadToEnd();
                    XDocument doc = XDocument.Parse(result);

                    var data = (from e in doc.Descendants("Package")
                                select new PackageResponse
                                {
                                    ID = e.Attribute("ID") == null ? "" : (string)e.Attribute("ID").Value,
                                    ZipOrigination = e.Element("ZipOrigination") == null ? "" : (string)e.Element("ZipOrigination"),
                                    ZipDestination = e.Element("ZipDestination") == null ? "" : (string)e.Element("ZipDestination"),
                                    Pounds = e.Element("Pounds") == null ? (decimal?)null : (decimal?)e.Element("Pounds"),
                                    Ounces = (decimal?)e.Element("Ounces"),
                                    FirstClassMailType = e.Element("FirstClassMailType") == null ? "" : (string)e.Element("FirstClassMailType"),
                                    Container = e.Element("Container") == null ? "" : (string)e.Element("Container"),
                                    Size = e.Element("Size") == null ? "" : (string)e.Element("Size"),
                                    Width = (decimal?)e.Element("Width"),
                                    Length = (decimal?)e.Element("Length"),
                                    Height = (decimal?)e.Element("Height"),
                                    Girth = (decimal?)e.Element("Girth"),
                                    Machinable = e.Element("Machinable") == null ? "" : (string)e.Element("Machinable"),
                                    Zone = e.Element("Zone") == null ? -1 : (int)e.Element("Zone"),
                                    Postage = (from p in e.Descendants("Postage")
                                               select new Postage
                                               {
                                                   MailService = (string)p.Element("MailService"),
                                                   Rate = (decimal)p.Element("Rate"),
                                                   SpecialServices = (from s in p.Descendants("SpecialServices").Descendants("SpecialService")
                                                                      select new SpecialService
                                                                      {
                                                                          ServiceID = s.Element("ServiceID") == null ? -1 : (int)s.Element("ServiceID"),
                                                                          ServiceName = s.Element("ServiceName") == null ? "" : (string)s.Element("ServiceName"),
                                                                          Available = s.Element("Available") == null ? "" : (string)s.Element("Available"),
                                                                          Price = s.Element("Price") == null ? 0 : (decimal)s.Element("Price")
                                                                      }).ToList(),
                                                   CommercialRate = (decimal?)p.Element("CommericalRate"),
                                                   CommercialPlusRate = (decimal?)p.Element("CommercialPlusRate"),
                                                   ServiceInformation = p.Element("ServiceInformation") == null ? "" : (string)p.Element("ServiceInformation"),
                                                   MaxDimensions = p.Element("MaxDimensions") == null ? "" : (string)p.Element("MaxDimensions"),
                                                   CommitmentDate = p.Element("CommitmentDate") == null ? "" : (string)p.Element("CommitmentDate"),
                                                   CommitmentName = p.Element("CommitmentName") == null ? "" : (string)p.Element("CommitmentName"),
                                                   CLASSID = p.Attribute("CLASSID") == null ? -1 : int.Parse(p.Attribute("CLASSID").Value),
                                               }).FirstOrDefault(),
                                    Error = (from er in e.Descendants("Error")
                                             select new PackageError
                                             {
                                                 Number = er.Element("Number") == null ? "" : (string)er.Element("Number"),
                                                 Source = er.Element("Source") == null ? "" : (string)er.Element("Source"),
                                                 Description = er.Element("Description") == null ? "" : (string)er.Element("Description"),
                                                 HelpFile = er.Element("HelpFile") == null ? "" : (string)er.Element("HelpFile"),
                                                 HelpContext = er.Element("HelpContext") == null ? "" : (string)er.Element("HelpContext"),
                                             }).FirstOrDefault()
                                }).ToList();
                    foreach (var d in data)
                    {
                        Console.WriteLine("Package ID={0}", d.ID);
                        if (d.Error == null)
                        {
                            Console.WriteLine("ZipOrigination={0}", d.ZipOrigination);
                            Console.WriteLine("ZipDestination={0}", d.ZipDestination);
                            Console.WriteLine("Zon={0}", d.Zone);
                            Console.WriteLine("Width={0}", d.Width);
                            Console.WriteLine("Length={0}", d.Length);
                            Console.WriteLine("Height={0}", d.Height);
                            Console.WriteLine("Pounds={0}", d.Pounds);
                            Console.WriteLine("Ounces={0}", d.Ounces);
                            Console.WriteLine("Girth={0}", d.Girth);
                            Console.WriteLine("MailService={0}", d.Postage.MailService);
                            Console.WriteLine("Postage Rate={0}", d.Postage.Rate);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Error-Number:{0}", d.Error.Number);
                            Console.WriteLine("Error-Source:{0}", d.Error.Source);
                            Console.WriteLine("Error-Description:{0}", d.Error.Description);
                            Console.WriteLine("Error-HelpFile:{0}", d.Error.HelpFile);
                            Console.WriteLine("Error-HelpContext:{0}", d.Error.HelpContext);
                            /// Tell from end USPS service not available, please choose UPS or FeDEX solution
                            throw new
                                Exception(string.Format(
                                "USPS service not available - Error-Number: {0}, Error-Source: {1}, Error-Description: {2}, Error-HelpFile: {3}, Error-HelpContext: {4}, please choose UPS or FeDEX solution",
                                d.Error.Number, d.Error.Source, d.Error.Description, d.Error.HelpFile, d.Error.HelpContext));
                        }
                    }
                    return executeResult;
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                var errorText = string.Empty;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    errorText = reader.ReadToEnd();
                    Console.WriteLine(errorText);
                }
                throw new Exception(errorText);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private ShippingMethod RateV4(string xmlData)
        {
            var shippingMethod = new ShippingMethod();
            string url = string.Format("http://production.shippingapis.com/ShippingAPI.dll?API=RateV4&XML={0}", xmlData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var result = reader.ReadToEnd();
                    XDocument doc = XDocument.Parse(result);
                    saveRequest(xmlData, result);
                    var data = (from e in doc.Descendants("Package")
                                select new PackageResponse
                                {
                                    ID = e.Attribute("ID") == null ? "" : (string)e.Attribute("ID").Value,
                                    ZipOrigination = e.Element("ZipOrigination") == null ? "" : (string)e.Element("ZipOrigination"),
                                    ZipDestination = e.Element("ZipDestination") == null ? "" : (string)e.Element("ZipDestination"),
                                    Pounds = e.Element("Pounds") == null ? (decimal?)null : (decimal?)e.Element("Pounds"),
                                    Ounces = (decimal?)e.Element("Ounces"),
                                    FirstClassMailType = e.Element("FirstClassMailType") == null ? "" : (string)e.Element("FirstClassMailType"),
                                    Container = e.Element("Container") == null ? "" : (string)e.Element("Container"),
                                    Size = e.Element("Size") == null ? "" : (string)e.Element("Size"),
                                    Width = (decimal?)e.Element("Width"),
                                    Length = (decimal?)e.Element("Length"),
                                    Height = (decimal?)e.Element("Height"),
                                    Girth = (decimal?)e.Element("Girth"),
                                    Machinable = e.Element("Machinable") == null ? "" : (string)e.Element("Machinable"),
                                    Zone = e.Element("Zone") == null ? -1 : (int)e.Element("Zone"),
                                    Postage = (from p in e.Descendants("Postage")
                                               select new Postage
                                               {
                                                   MailService = (string)p.Element("MailService"),
                                                   Rate = (decimal)p.Element("Rate"),
                                                   SpecialServices = (from s in p.Descendants("SpecialServices").Descendants("SpecialService")
                                                                      select new SpecialService
                                                                      {
                                                                          ServiceID = s.Element("ServiceID") == null ? -1 : (int)s.Element("ServiceID"),
                                                                          ServiceName = s.Element("ServiceName") == null ? "" : (string)s.Element("ServiceName"),
                                                                          Available = s.Element("Available") == null ? "" : (string)s.Element("Available"),
                                                                          Price = s.Element("Price") == null ? 0 : (decimal)s.Element("Price")
                                                                      }).ToList(),
                                                   CommercialRate = (decimal?)p.Element("CommericalRate"),
                                                   CommercialPlusRate = (decimal?)p.Element("CommercialPlusRate"),
                                                   ServiceInformation = p.Element("ServiceInformation") == null ? "" : (string)p.Element("ServiceInformation"),
                                                   MaxDimensions = p.Element("MaxDimensions") == null ? "" : (string)p.Element("MaxDimensions"),
                                                   CommitmentDate = p.Element("CommitmentDate") == null ? "" : (string)p.Element("CommitmentDate"),
                                                   CommitmentName = p.Element("CommitmentName") == null ? "" : (string)p.Element("CommitmentName"),
                                                   CLASSID = p.Attribute("CLASSID") == null ? -1 : int.Parse(p.Attribute("CLASSID").Value),
                                               }).FirstOrDefault(),
                                    Error = (from er in e.Descendants("Error")
                                             select new PackageError
                                             {
                                                 Number = er.Element("Number") == null ? "" : (string)er.Element("Number"),
                                                 Source = er.Element("Source") == null ? "" : (string)er.Element("Source"),
                                                 Description = er.Element("Description") == null ? "" : (string)er.Element("Description"),
                                                 HelpFile = er.Element("HelpFile") == null ? "" : (string)er.Element("HelpFile"),
                                                 HelpContext = er.Element("HelpContext") == null ? "" : (string)er.Element("HelpContext"),
                                             }).FirstOrDefault()
                                }).ToList();

                    foreach (var d in data)
                    {
                        Console.WriteLine("Package ID={0}", d.ID);
                        if (d.Error == null)
                        {
                            shippingMethod.ShippingCarrier = "USPS";
                            shippingMethod.ServiceCode = d.Postage.MailService;
                            /* remove signature confirmation fee
                            float signatureConfirmationFee = 0;
                            var signatureConfirmationService = d.Postage.SpecialServices.Where(x => x.ServiceID.ToString().Trim() == "156" && x.Available.Trim().ToUpper() == "TRUE").FirstOrDefault();
                            if (signatureConfirmationService != null)
                            {
                                signatureConfirmationFee = (float)signatureConfirmationService.Price;
                            }

                            shippingMethod.ShippingCostWithPublishedRate += (float)d.Postage.Rate + (float)signatureConfirmationFee;
                            */
                            shippingMethod.ShippingCostWithPublishedRate += (float)d.Postage.Rate;
                            Console.WriteLine("ZipOrigination={0}", d.ZipOrigination);
                            Console.WriteLine("ZipDestination={0}", d.ZipDestination);
                            Console.WriteLine("Zon={0}", d.Zone);
                            Console.WriteLine("Width={0}", d.Width);
                            Console.WriteLine("Length={0}", d.Length);
                            Console.WriteLine("Height={0}", d.Height);
                            Console.WriteLine("Pounds={0}", d.Pounds);
                            Console.WriteLine("Ounces={0}", d.Ounces);
                            Console.WriteLine("Girth={0}", d.Girth);
                            Console.WriteLine("Postage Rate={0}", d.Postage.Rate);
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Error-Number:{0}", d.Error.Number);
                            Console.WriteLine("Error-Source:{0}", d.Error.Source);
                            Console.WriteLine("Error-Description:{0}", d.Error.Description);
                            Console.WriteLine("Error-HelpFile:{0}", d.Error.HelpFile);
                            Console.WriteLine("Error-HelpContext:{0}", d.Error.HelpContext);
                            /// Tell from end USPS service not available, please choose UPS or FeDEX solution
                            throw new
                                Exception(string.Format(
                                "USPS service not available - Error-Number: {0}, Error-Source: {1}, Error-Description: {2}, Error-HelpFile: {3}, Error-HelpContext: {4}, please choose UPS or FeDEX solution",
                                d.Error.Number, d.Error.Source, d.Error.Description, d.Error.HelpFile, d.Error.HelpContext));
                        }
                    }
                    return shippingMethod;
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                var errorText = string.Empty;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    errorText = reader.ReadToEnd();
                    Console.WriteLine(errorText);
                }
                throw new Exception(errorText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private POCOS.Cart _cart;
        public override List<ShippingMethod> getFreightEstimation(Cart cart, Address shipFromAddress)
        {
            _cart = cart;
            var shippingMethods = new List<ShippingMethod>();
            var shippingInstructions = new List<ShippingInstruction>();
            List<ProductShippingDimension> psds = new List<ProductShippingDimension>();
            foreach (var ci in cart.cartItemsX)
            {
                ProductShippingDimension psd = new ProductShippingDimension();
                psd.Width = ci.partX.DimensionWidthCM.GetValueOrDefault();
            }
            shippingMethods = getFreightEstimationX(store.StoreID, psds, store.ShipFromAddress.ZipCode, cart.ShipToContact.ZipCode, out shippingInstructions);
            _shippingInstructions = shippingInstructions;

            //Save log as XML to file
            saveXMLlog(cart, shipFromAddress, new PackingList(), shippingMethods);
            return shippingMethods;
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
        private void saveRequest(string req, string resp)
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
            if (_cart == null)
            {
                filename = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            }
            else
                filename = _cart.CartID;
            //Check saving directory existent
            if (!Directory.Exists(@filePath.ToString()))
            {
                //Create default saving folder
                Directory.CreateDirectory(@filePath.ToString());
            }
         
            //Save

            System.IO.File.AppendAllText(string.Format("{0}\\{1}-request.xml", filePath.ToString(), filename), req);
            System.IO.File.AppendAllText(string.Format("{0}\\{1}-response.xml", filePath.ToString(), filename), resp);
        }
    }

    public class RateV4Request
    {
        public string Revision { get; set; }
        public List<Package> Packages { get; set; }
    }

    public class Package
    {
        public string ID { get; set; }
        public string Service { get; set; }
        public string FirstClassMailType { get; set; }
        public string ZipOrigination { get; set; }
        public string ZipDestination { get; set; }
        public decimal? Pounds { get; set; }
        public decimal? Ounces { get; set; }
        public string Container { get; set; }
        public string Size { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public decimal? Girth { get; set; }
        public decimal? Value { get; set; }
        public List<string> SpecialServices { get; set; }
        public string Machinable { get; set; }
        public string DropOffTime { get; set; }
        public string ShipDate { get; set; }
    }

    public class SpecialService
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Available { get; set; }
        public decimal Price { get; set; }
        public string DeclaredValueRequired { get; set; }
        public string DueSenderRequired { get; set; }
    }

    public class RateV4Response
    {
        public List<PackageResponse> MyProperty { get; set; }
    }

    public class PackageResponse
    {
        public string ID { get; set; }
        public string ZipOrigination { get; set; }
        public string ZipDestination { get; set; }
        public decimal? Pounds { get; set; }
        public decimal? Ounces { get; set; }
        public string FirstClassMailType { get; set; }
        public string Container { get; set; }
        public string Size { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public decimal? Girth { get; set; }
        public string Machinable { get; set; }
        public int Zone { get; set; }
        public Postage Postage { get; set; }
        public PackageError Error { get; set; }
    }

    public class Postage
    {
        public int CLASSID { get; set; }
        public string MailService { get; set; }
        public decimal Rate { get; set; }
        public decimal? CommercialRate { get; set; }
        public decimal? CommercialPlusRate { get; set; }
        public string ServiceInformation { get; set; }
        public string MaxDimensions { get; set; }
        public string CommitmentDate { get; set; }
        public string CommitmentName { get; set; }
        public List<SpecialService> SpecialServices { get; set; }
    }

    public class PackageError
    {
        public string Number { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string HelpFile { get; set; }
        public string HelpContext { get; set; }
    }

    [Serializable]
    public class ShippingInstruction
    {
        public int ShippingMethodId { get; set; }
        public string ShippingCarrier { get; set; }
        public string ServiceCode { get; set; }
        public string ShippingMethodDescription { get; set; }
        public float Rate { get; set; }
        public string ContainerNo { get; set; }
        public float ContainerFreight { get; set; }
        public string ShipVia { get; set; }
        public int ItemNo { get; set; }
        public string PartNo { get; set; }
        public int Level { get; set; }
    }

    [Serializable]
    public class ProductShippingDimension
    {
        public int ID { get; set; }
        public string ContainerName { get; set; }
        public int ItemNo { get; set; }
        public string PartNo { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        public decimal Height { get; set; }
        public decimal? ShipWeightKG { get; set; }
        public decimal Surface { get { return Width * Depth; } }
        public string ShipFrom { get; set; }
        public bool? FreeShipping { get; set; }
        public int Quantity { get; set; }
        public int? BundleID { get; set; }
    }

    [Serializable]
    public class Arrangement
    {
        public int Level { get; set; }
        public ProductShippingDimension ProductShippingDimension { get; set; }
    }

    [Serializable]
    public class Container : ProductShippingDimension
    {
        public decimal? Pounds { get; set; }
        public decimal? Ounces { get; set; }
        public List<Arrangement> Arrangements { get; set; }
    }
}
