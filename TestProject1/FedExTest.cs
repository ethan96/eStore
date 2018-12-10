using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Web.Services.Protocols;
using eStore.BusinessModules.FedExWebService;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ShippingCarrierFedExTest and is intended
    ///to contain all ShippingCarrierFedExTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FedExTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CreateRateRequest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("eStore.BusinessModules.dll")]
        public void CreateRateRequestTest()
        {
            /* New Production account */
            //string key = "aZHhFpmDB478xtsz";
            //string account = "229182811";
            //string password = "aJS59qY5yg5Ezufe6vV4vUg74";
            //string meternumber = "102174336";

            /* Production account */
            string key = "usvAyuZI2Hbz2QFv";
            string account = "229182811";
            string password = "YmqDX8cnYSMdMEEg0AGyQhKNe";
            string meternumber = "102086661";

            ///* Testing account */
            //string key = "dH6mAmHx9xTfL6OS";
            //string account = "510087682";
            //string password = "bXQ6mCfkKGP5JVrvQTsEIFMdk";
            //string meternumber = "100025628";

            
            RateRequest request = CreateRateRequest(key, account, password, meternumber);
            //
            RateService service = new RateService(); // Initialize the service
            try
            {
                // This is the call to the web service passing in a RateRequest and returning a RateReply
                RateReply reply = service.getRates(request);

                if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
                {
                    ShowRateReply(reply);
                }
                else
                {
                    Console.WriteLine(reply.Notifications[0].Message);
                }
            }
            catch (SoapException e)
            {
                Console.WriteLine(e.Detail.InnerText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static void ShowRateReply(RateReply reply)
        {
            Console.WriteLine("RateReply details:");
            foreach (RateReplyDetail rateDetail in reply.RateReplyDetails)
            {
                Console.WriteLine("ServiceType : " + rateDetail.ServiceType);
                Console.WriteLine("**********************************************************");
                foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                {
                    ShowPackageRateDetails(shipmentDetail);
                }
                ShowDeliveryDetails(rateDetail);
            }
        }

        private static void ShowPackageRateDetails(RatedShipmentDetail shipmentDetail)
        {
            Console.WriteLine("RateType : " + shipmentDetail.ShipmentRateDetail.RateType);
            Console.WriteLine("Total Billing Weight : " + shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Value);
            Console.WriteLine("Total Base Charge : " + shipmentDetail.ShipmentRateDetail.TotalBaseCharge.Amount);
            Console.WriteLine("Total Discount : " + shipmentDetail.ShipmentRateDetail.TotalFreightDiscounts.Amount);
            Console.WriteLine("Total Surcharges : " + shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount);
            Console.WriteLine("Net Charge : " + shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount);
            Console.WriteLine("**********************************************************");
        }

        private static void ShowDeliveryDetails(RateReplyDetail rateDetail)
        {
            if (rateDetail.DeliveryTimestampSpecified)
            {
                Console.WriteLine("Delivery timestamp " + rateDetail.DeliveryTimestamp.ToString());
            }
            Console.WriteLine("Transit Time: " + rateDetail.TransitTime);
        }

        private static RateRequest CreateRateRequest(string key, string account, string password, string meternumber)
        {
            // Build the RateRequest
            RateRequest request = new RateRequest();
            //
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = key;
            request.WebAuthenticationDetail.UserCredential.Password = password;  // Replace "XXX" with the Password
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = account;   // Replace "XXX" with client's account number
            request.ClientDetail.MeterNumber = meternumber; // Replace "XXX" with client's meter number
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = ""; // This is a reference field for the customer.  Any value can be used and will be provided in the response.
            //
            request.Version = new VersionId(); // WSDL version information, value is automatically set from wsdl            
            // 
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[2];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;
            request.CarrierCodes[1] = CarrierCodeType.FDXG;
            //
            SetShipmentDetails(request);
            //
            SetOrigin(request);
            //
            SetDestination(request);
            //
            SetPayment(request, account);
            //
            SetIndividualPackageLineItems(request);
            //
            return request;
        }

        private static void SetShipmentDetails(RateRequest request)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            //request.RequestedShipment.ServiceType = ServiceType.INTERNATIONAL_PRIORITY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
            //request.RequestedShipment.ServiceTypeSpecified = true;
            request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
            request.RequestedShipment.PackagingTypeSpecified = true;
            //
            request.RequestedShipment.TotalInsuredValue = new Money();
            request.RequestedShipment.TotalInsuredValue.Amount = 100;
            request.RequestedShipment.TotalInsuredValue.Currency = "USD";
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            request.RequestedShipment.PackageDetailSpecified = true;
        }

        private static void SetOrigin(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new eStore.BusinessModules.FedExWebService.Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "Sender Address Line 1" };
            request.RequestedShipment.Shipper.Address.City = "Milpitas";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "CA";
            request.RequestedShipment.Shipper.Address.PostalCode = "95035";
            request.RequestedShipment.Shipper.Address.CountryCode = "US";
        }

        private static void SetDestination(RateRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new eStore.BusinessModules.FedExWebService.Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "Recipient Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = "NewYork";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "NY";
            request.RequestedShipment.Recipient.Address.PostalCode = "12345";
            request.RequestedShipment.Recipient.Address.CountryCode = "US";
        }

        private static void SetPayment(RateRequest request, string account)
        {
            request.RequestedShipment.ShippingChargesPayment = new eStore.BusinessModules.FedExWebService.Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = account; // Replace "XXX" with client's account number
        }

        private static void SetIndividualPackageLineItems(RateRequest request)
        {
            // ------------------------------------------
            // Passing individual pieces rate request
            // ------------------------------------------
            request.RequestedShipment.PackageCount = "2";
            //
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[2];
            request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
            request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = "1"; // package sequence number
            //
            request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight(); // package weight
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.LB;
            request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = 0M;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions(); // package dimensions
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "50";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "13";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "4";
            request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money(); // insured value
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Amount = 100;
            request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = "USD";
            //
            request.RequestedShipment.RequestedPackageLineItems[1] = new RequestedPackageLineItem();
            request.RequestedShipment.RequestedPackageLineItems[1].SequenceNumber = "2"; // package sequence number
            //
            request.RequestedShipment.RequestedPackageLineItems[1].Weight = new Weight(); // package weight
            request.RequestedShipment.RequestedPackageLineItems[1].Weight.Units = WeightUnits.LB;
            request.RequestedShipment.RequestedPackageLineItems[1].Weight.Value = 0M;
            //
            request.RequestedShipment.RequestedPackageLineItems[1].Dimensions = new Dimensions(); // package dimensions
            request.RequestedShipment.RequestedPackageLineItems[1].Dimensions.Length = "20";
            request.RequestedShipment.RequestedPackageLineItems[1].Dimensions.Width = "13";
            request.RequestedShipment.RequestedPackageLineItems[1].Dimensions.Height = "4";
            request.RequestedShipment.RequestedPackageLineItems[1].Dimensions.Units = LinearUnits.IN;
            //
            request.RequestedShipment.RequestedPackageLineItems[1].InsuredValue = new Money(); // insured value
            request.RequestedShipment.RequestedPackageLineItems[1].InsuredValue.Amount = 500;
            request.RequestedShipment.RequestedPackageLineItems[1].InsuredValue.Currency = "USD";
        }
    }
}
