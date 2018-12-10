using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.BusinessModules;
using System.Collections.Generic;
using System.Linq;
namespace TestProject1
{


    /// <summary>
    ///This is a test class for ShippingCarierHelperTest and is intended
    ///to contain all ShippingCarierHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ShippingCarierHelperTest
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
        ///A test for getShippingCarrier
        ///</summary>
        [TestMethod()]
        public void getShippingCarrierTest()
        {
            string shippingcariername = "UPS_US"; // TODO: Initialize to an appropriate value

            ShippingCarier actual;
            actual = ShippingCarierHelper.getShippingCarrier(shippingcariername);

            foreach (RateServiceName rsm in actual.RateServiceNames)
            {

                Console.WriteLine(rsm.MessageCode + ":" + rsm.DefaultServiceName);

            }

            Assert.IsTrue(actual != null);

        }

        /// <summary>
        ///A test for getShippingCarrier
        ///</summary>
        [TestMethod()]
        public void shippingRateValidation()
        {

            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore("AUS");
            foreach (string id in getOrders())

            //     eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore("AEU");
            //foreach (string id in geteuorders())
            {
                try
                {
                    Order order = store.getOrder(id);
                    Carrier carrier = null;

                    foreach (StoreCarier sc in store.profile.StoreCariers)
                    {
                        if (string.IsNullOrWhiteSpace(order.Courier))
                        {
                            if (order.StoreID == "AEU")
                            {

                                order.Courier = "AEUUPSCarier";
                            }
                            else if (order.ShippingMethod.StartsWith("UPS"))
                            {
                                order.Courier = "UPS_US";
                            }
                            else
                            {
                                order.Courier = "FedEx_US";
                            }
                        }
                        if (order.Courier == "FEDEX_2_DAY")
                            order.Courier = "FedEx_US";

                        if (order.Courier == sc.CarierName)
                        {
                            carrier = getCarrier(store.profile, sc.ShippingCarier);
                            break;
                        }

                    }
                  
                    System.Collections.Generic.List<ShippingMethod> methods = carrier.getFreightEstimation(order.cartX, store.profile.ShipFromAddress);

                    foreach (eStore.BusinessModules.ShippingMethod method in methods)
                    {
                        if (method.ShippingMethodDescription == order.ShippingMethod)
                        {
                           List<string> boxes=new List<string>();
                           if (method.PackingList != null)
                               foreach (PackagingBox box in method.PackingList.PackagingBoxes)
                               {
                                   boxes.Add(string.Format("1BX @{3}LBS {0} X {1} X {2} @{4}", box.Width, box.Length, box.Height, box.Weight, box.InsuredValue));
                               }

                            Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}"
                                , order.OrderNo
                                , order.OrderDate
                                , method.ShippingMethodDescription
                                , method.ShippingCostWithPublishedRate
                                , string.Join(" / ", boxes)
                                )
                                );
                       
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(id + "  " + ex.Message);
                }
            }
           
           
        }
        private Carrier getCarrier(eStore.POCOS.Store store, ShippingCarier shippingCarrier)
        {
            Carrier _carrier;
            String carrierName = shippingCarrier.CarierName;
            switch (carrierName)
            {
                case "UPS_US":
                case "UPS_EU":
                    _carrier = new ShippingCarrierUPS(store, shippingCarrier);
                    break;

                case "FedEx_US":
                    _carrier = new ShippingCarrierFedEx(store, shippingCarrier);
                    break;

                case "TDS":                             //Taiwan drop ship cost, ship from Taiwan via DHL
                    _carrier = new ShippingCarrierTDS(store, shippingCarrier);
                    break;

                case "LocalFixRate":           //Local carrier, charge fix rate shipping cost
                    _carrier = new ShippingCarrierFixRate(store, shippingCarrier);
                    break;

                case "AAUCarier":
                    _carrier = new ShippingCarrierAAU(store, shippingCarrier);
                    break;

                case "AEUUPSCarier": //AEU store ups
                    _carrier = new ShippingCarrierAEUUPS(store, shippingCarrier);
                    break;

                default:
                    _carrier = null;
                    break;
            }
            return _carrier;
        }

        public string[] getOrders()
        {
            string[] orderids = { "OUS026740"
,"OUS026775"
,"OUS026803"
,"OUS026817"
,"OUS026818"
,"OUS026819"
,"OUS026825"
,"OUS026854"
,"OUS026856"
,"OUS026863"
,"OUS026865"
,"OUS026866"
,"OUS026868"
,"OUS026880"
,"OUS026897"
,"OUS026906"
,"OUS026910"
,"OUS026923"
,"OUS026926"
,"OUS026935"
,"OUS026940"
,"OUS026944"
,"OUS026945"
,"OUS026949"
,"OUS026950"
,"OUS026951"
,"OUS026955"
,"OUS026959"
,"OUS026962"
,"OUS026965"
,"OUS026967"
,"OUS026979"
,"OUS026993"
,"OUS026996"
,"OUS026997"
,"OUS027002"
,"OUS027008"
,"OUS027014"
,"OUS027017"
,"OUS027018"
,"OUS027025"
,"OUS027026"
,"OUS027031"
,"OUS027034"
,"OUS027039"
,"OUS027040"
,"OUS027042"
,"OUS027044"
,"OUS027051"
,"OUS027069"
,"OUS027071"
,"OUS027074"
,"OUS027078"
,"OUS027079"
,"OUS027080"
,"OUS027087"
,"OUS027090"
,"OUS027091"
,"OUS027091"
,"OUS027095"
,"OUS027096"
,"OUS027097"
,"OUS027098"
,"OUS027100"
,"OUS027104"
,"OUS027106"
,"OUS027107"
,"OUS027108"
,"OUS027118"
,"OUS027125"
,"OUS027126"
,"OUS027130"
,"OUS027135"
,"OUS027136"
,"OUS027140"
,"OUS027142"
,"OUS027144"
,"OUS027146"
,"OUS027148"
,"OUS027158"
,"OUS027163"
,"OUS027164"
,"OUS027169"
,"OUS027173"
,"OUS027174"
,"OUS027175"
,"OUS027178"
,"OUS027179"
,"OUS027189"
,"OUS027191"
,"OUS027200"
,"OUS027202"
,"OUS027204"
,"OUS027207"
,"OUS027209"
,"OUS027211"
,"OUS027215"
,"OUS027216"
,"OUS027226"
,"OUS027228"
,"OUS027229"
,"OUS027230"
,"OUS027236"
,"OUS027238"
,"OUS027240"
,"OUS027243"
,"OUS027244"
,"OUS027245"
,"OUS027251"
,"OUS027255"
,"OUS027269"
,"OUS027273"
,"OUS027276"
,"OUS027277"
,"OUS027279"
,"OUS027285"
,"OUS027290"
,"OUS027296"
,"OUS027298"
,"OUS027300"
,"OUS027301"
,"OUS027302"
,"OUS027304"
,"OUS027317"
,"OUS027318"
,"OUS027322"
,"OUS027331"
,"OUS027341"
,"OUS027344"
,"OUS027348"
,"OUS027350"
,"OUS027354"
,"OUS027365"
,"OUS027366"
,"OUS027378"
,"OUS027381"
,"OUS027402"
,"OUS027409"
,"OUS027180"
,"OUS027315"
,"OUS027382"
,"OUS027389"
,"OUS027394"
,"OUS027395"
,"OUS027320"
,"OUS027316"
,"OUS027346"
,"OUS027342"
,"OUS026908"
,"OUS027414"
,"OUS027429"
,"OUS027433"
,"OUS027424"
,"OUS026796"
,"OUS026966"
,"OUS027015"
,"OUS027457"
,"OUS027283"
,"OUS027448"
,"OUS027375"
,"OUS026713"
,"OUS027468"
,"OUS027488"


 };
            return orderids;
        }


        public string[] geteuorders()
        {
            string[] orderids = { "OEU021442"
,"OEU021441"
,"OEU021440"
,"OEU021439"
,"OEU021437"
,"OEU021435"
,"OEU021433"
,"OEU021431"
,"OEU021429"
,"OEU021426"
,"OEU021425"
,"OEU021418"
,"OEU021402"
,"OEU021397"
,"OEU021396"
,"OEU021394"
,"OEU021393"
,"OEU021390"
,"OEU021388"
,"OEU021380"
,"OEU021378"
,"OEU021376"
,"OEU021375"
,"OEU021374"
,"OEU021373"
,"OEU021371"
,"OEU021369"
,"OEU021368"
,"OEU021367"
,"OEU021361"
,"OEU021360"
,"OEU021357"
,"OEU021356"
,"OEU021355"
,"OEU021354"
,"OEU021352"
,"OEU021351"
,"OEU021349"
,"OEU021348"
,"OEU021346"
};
            return orderids;
        }

        [TestMethod()]
        public void getReplaceBox()
        {
            eStore.BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore("AUS");

            int[] needreplacingboxes = { 12,13,19,22,39};
            PackingRule_AUS rule = new PackingRule_AUS();
            foreach (ProductBox box in store.profile.getStoreAvailableBox().Where(x => needreplacingboxes.Contains(x.BoxID)))
            {
                ProductBox replacedbox = getClosestStoreBox(store.profile, box);
                if (replacedbox != null)
                {
                    Console.WriteLine("{0} - {1}", box.BoxID, replacedbox.BoxID);
                }
            }
        
        }

        private ProductBox getClosestStoreBox(eStore.POCOS.Store store, ProductBox box)
        {
            try
            {
                List<ProductBox> _StoreAvailableBoxs = store.getStoreAvailableBox();
                var _ProductBox = (from s in _StoreAvailableBoxs
                                   where box.CompareTo(s) <0
                                   orderby s.volumn
                                   select s).FirstOrDefault();
                return _ProductBox;
            }
            catch (Exception)
            {
             
                return null;
            }
        }

    }
}
