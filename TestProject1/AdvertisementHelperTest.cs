using eStore.POCOS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using eStore.POCOS;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for AdvertisementHelperTest and is intended
    ///to contain all AdvertisementHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdvertisementHelperTest
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
        ///A test for getDefaultAdv
        ///</summary>
        [TestMethod()]
        public void getDefaultAdvTest()
        {
            AdvertisementHelper target = new AdvertisementHelper(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            List<Advertisement> actual;
            actual = target.getDefaultAdv(storeid);

            foreach (Advertisement ad in actual) {
                 
                Console.WriteLine(ad.AdType + ":" + ad.Title );
            }

        }

        /// <summary>
        ///A test for getAdvByStore
        ///</summary>
        [TestMethod()]
        public void getAdvByStoreTest()
        {
            AdvertisementHelper target = new AdvertisementHelper(); // TODO: Initialize to an appropriate value
            Store store = new StoreHelper().getStorebyStoreid("AUS");
            
            List<Advertisement> actual;
            actual = target.getAdvByStore(store);

            Advertisement a = (from xx in actual select xx).FirstOrDefault();
            MenuAdvertisement m = (from x in a.MenuAdvertisements select x).FirstOrDefault();
            a.MenuAdvertisements.Remove(m);
            a.save();           

        }
    }
}
