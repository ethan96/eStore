using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for VendorHelperTest and is intended
    ///to contain all VendorHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VendorHelperTest
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
        ///A test for save
        ///</summary>
        [TestMethod()]
        public void saveTest()
        {

            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;

            Vendor _vendor = new Vendor(); // TODO: Initialize to an appropriate value
            _vendor.VendorId = "Advantech001";
            _vendor.VendorName = "Advantech";
            _vendor.VendorAccountEmail = "edward.keh@advantech.com.tw";
            _vendor.VendorCountry ="USA";
            _vendor.VendorCity ="Milpitas";
            _vendor.VendorState = "CA";
            _vendor.VendorAddress ="380 Fairview way";

          
            actual = new VendorHelper().save(_vendor);
            Assert.AreEqual(expected, actual);

            //actual = VendorHelper.delete(_vendor);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for save
        ///</summary>
        [TestMethod()]
        public void saveTest1()
        {
            Vendor _vendor = null; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = new VendorHelper().save(_vendor);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
