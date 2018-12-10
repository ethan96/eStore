using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using eStore.POCOS.DAL;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ABN_AMRODirectLinkTest and is intended
    ///to contain all ABN_AMRODirectLinkTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ABN_AMRODirectLinkTest
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
        ///A test for sendDirectLinkPayment
        ///</summary>
        [TestMethod()]
        public void sendDirectLinkPaymentTest()
        {
            ABN_AMRODirectLink target = new ABN_AMRODirectLink(); // TODO: Initialize to an appropriate value
            Order order = new OrderHelper().getOrderbyOrderno("OEU020028");
            Payment paymentInfo = order.getLastPayment();

            paymentInfo.cardNo="5178058727084xxx";
            paymentInfo.CardExpiredDate = "09/13";
            paymentInfo.SecurityCode = "389";

            bool simulation = true; // TODO: Initialize to an appropriate value             
            Payment actual;
            actual = target.sendDirectLinkPayment(order, paymentInfo, simulation);

            Console.WriteLine(actual.Status);
        }


        ///A test for sendDirectLinkPayment
        ///</summary>
        [TestMethod()]
        public void OgoneDirectLink()
        {
            OgoneDirectLink target = new OgoneDirectLink(); // TODO: Initialize to an appropriate value
            Order order = new OrderHelper().getOrderbyOrderno("OEU021917");
            Payment paymentInfo = order.getLastPayment();
            if (paymentInfo == null)
                paymentInfo = new Payment();
            paymentInfo.cardNo = "4111111111111111";
            paymentInfo.CardExpiredDate = "0913";
            paymentInfo.SecurityCode = "389";

            bool simulation = true; // TODO: Initialize to an appropriate value             
            Payment actual;
            actual = target.sendDirectLinkPayment(order, paymentInfo, simulation);

            Console.WriteLine(actual.Status);
        }
    }
}
