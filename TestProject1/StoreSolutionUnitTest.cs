using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using eStore.POCOS;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for StoreSolutionUnitTest and is intended
    ///to contain all StoreSolutionUnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoreSolutionUnitTest
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
        ///A test for getStore
        ///</summary>
        [TestMethod()]
        public void getStoreTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();

            eStore.BusinessModules.Store store = eSolution.getStore("AUS");
            Assert.IsNotNull(store);

            store = eSolution.getStore("buy.advantech.com");
            Assert.IsNotNull(store);

            store = eSolution.getStore("buy.advantech.eu");
            Assert.IsNotNull(store);
        }


        /// <summary>
        ///Unit test for getting country list
        ///</summary>
        [TestMethod()]
        public void getCountriesTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();

            List<Country> countries = eSolution.countries;
            Assert.IsNotNull(countries);

            foreach (Country cntry in countries)
                Console.WriteLine("{0} : {1}", cntry.CountryID, cntry.CountryName);
        }

        /// <summary>
        ///Unit test for locating store feature
        ///</summary>
        [TestMethod()]
        public void locateStoreTest()
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();

            eStore.BusinessModules.Store aus = eSolution.getStore("AUS");
            eStore.BusinessModules.Store store = null;

            store = eSolution.locateStore(aus, "217.146.186.221");   //UK IP
            Console.WriteLine("Can US store handle UK traffic, if not which store shall {0}", store.storeID);
            Assert.IsTrue(store.storeID.Equals("AEU"));
            store = eSolution.locateStore(null, "217.146.186.221");
            Console.WriteLine("Store for UK IP is {0}", store.storeID);
            Assert.IsTrue(store.storeID.Equals("AEU"));
            store = eSolution.locateStore(aus, "219.90.3.201");   //TW IP
            Console.WriteLine("Can US store handle TW traffic, if not which store shall {0}", store.storeID);
            Assert.IsTrue(store.storeID.Equals("AUS"));
            store = eSolution.locateStore(null, "219.90.3.201");   //TW IP
            Console.WriteLine("Store for TW IP is {0}", store.storeID);
            Assert.IsTrue(store.storeID.Equals("ATW"));
        }
    }
}
