using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StoreTest and is intended
    ///to contain all StoreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreTest
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
        ///A test for getAddressByCountry
        ///</summary>
        [TestMethod()]
        public void getAddressByCountryTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
  
            eStore.BusinessModules.Store target = eSolution.getStore("AUS"); // TODO: Initialize to an appropriate value
            string countryName = "US"; // TODO: Initialize to an appropriate value
            eStore.POCOS.Store.BusinessGroup businessGroup =  eStore.POCOS.Store.BusinessGroup.eP; // TODO: Initialize to an appropriate value
            Address expected = null; // TODO: Initialize to an appropriate value
            Address actual;
            actual = target.getAddressByCountry(countryName, businessGroup);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getAddressByCountry
        ///</summary>
        [TestMethod()]
        public void getFastDeliveryProductsTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
  
            eStore.BusinessModules.Store target = eSolution.getStore("AUS"); // TODO: Initialize to an appropriate value
            
            System.Collections.Generic.List<string> product = target.getFastDeliveryProducts();
        }
    }
}
