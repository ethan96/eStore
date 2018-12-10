using eStore.Proxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for SAPProxyTest and is intended
    ///to contain all SAPProxyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SAPProxyTest
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
        ///A test for getMultiATP
        ///</summary>
        [TestMethod()]
        public void getMultiATPTest()
        {
            SAPProxy target = new SAPProxy(); // TODO: Initialize to an appropriate value
            string SAPDeliveryPlant = "USH1"; // TODO: Initialize to an appropriate value
            DateTime requestDate = DateTime.Now; // TODO: Initialize to an appropriate value
            Hashtable partnos = null; // TODO: Initialize to an appropriate value

            partnos = new Hashtable();
            partnos.Add("AIMB-780QG2-00A1E", 1);
            partnos.Add("96D3-1G1066NN-TR", 1);

            List<ProductAvailability> actual;
            actual = target.getMultiATP(SAPDeliveryPlant, requestDate, partnos);
            Assert.IsNotNull(actual);
            int qtyQtyATP=0;

            Console.WriteLine(target.getAvailability("AIMB-780QG2-00A1E", ref qtyQtyATP, actual));
           Console.WriteLine(target.getAvailability("96D3-1G1066NN-TR", ref qtyQtyATP, actual));
            

            foreach (ProductAvailability key in actual) {
                Console.WriteLine("PartNO" + key.PartNO);
                Console.WriteLine("QtyFulfill" + key.QtyFulfill);
                Console.WriteLine("RequestDate" + key.RequestDate);
                Console.WriteLine("RequestQty"+ key.RequestQty);


            }

            
        }

        /// <summary>
        ///A test for getAvailability
        ///</summary>
        [TestMethod()]
        public void getAvailabilityTest()
        {
            SAPProxy target = new SAPProxy(); // TODO: Initialize to an appropriate value
            string PartNO = string.Empty; // TODO: Initialize to an appropriate value
            int qtyQtyATP = 0; // TODO: Initialize to an appropriate value
            int qtyQtyATPExpected = 0; // TODO: Initialize to an appropriate value
            List<ProductAvailability> productAV = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getAvailability(PartNO, ref qtyQtyATP, productAV);
            Assert.AreEqual(qtyQtyATPExpected, qtyQtyATP);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
