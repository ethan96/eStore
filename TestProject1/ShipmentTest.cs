using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS.DAL;
using eStore.BusinessModules;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ShipmentTest and is intended
    ///to contain all ShipmentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ShipmentTest
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
        ///A test for Shipment Constructor
        ///</summary>
        [TestMethod()]
        public void ShipmentConstructorTest()
        {
            string hostname = "AUS"; // TODO: Initialize to an appropriate value
            eStore.POCOS.Store estore = null; // TODO: Initialize to an appropriate value
            StoreHelper shelper = new StoreHelper();
            estore = shelper.getStorebyStoreid(hostname);

            User user = null;
            user = new UserHelper().getUserbyID("jimmy.xiao@advantech.com.tw");
            Cart cart = new CartHelper().getCartMastersbyUserID("jimmy.xiao@advantech.com.tw", "AUS");
            PackingManager pm = new PackingManager(estore);
            string shippingCarrier = "UPS_US"; // TODO: Initialize to an appropriate value
            string shippingMethod = "UPS Three-Day Select®"; // TODO: Initialize to an appropriate value
            Decimal shippingRate = new Decimal(125.5); // TODO: Initialize to an appropriate value
            PackingList packingList = pm.getPackingList(cart,139) ; // TODO: Initialize to an appropriate value

            //Constructor
            Shipment target = new Shipment(shippingCarrier, shippingMethod, shippingRate, packingList);
            
            
            Console.WriteLine("Shipment.ShippingCarrier:" + target.ShippingCarrier);
            Console.WriteLine("Shipment.ShippingMethod" + target.ShippingMethod);
            Console.WriteLine("Shipment.ShippingRate. " + target.ShippingRate);

            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
