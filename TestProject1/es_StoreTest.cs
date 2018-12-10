using eStore.POCOS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for es_StoreTest and is intended
    ///to contain all es_StoreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class es_StoreTest
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
        ///A test for getInstance
        ///</summary>
        [TestMethod()]
        public void getInstanceTest()
        {
            
        }

        [TestMethod()]
        public void TestIPisInteranlOrNot()
        {
            Store _store = new eStore.POCOS.DAL.StoreHelper().getStorebyStoreid("AUS");

            eStore.POCOS.Country iplocatedcountry = null;
            string _countryCode = string.Empty;
            _countryCode = eStore.BusinessModules.StoreSolution.getInstance().getCountryCodeByIp("219.90.3.101");
           iplocatedcountry =  eStore.BusinessModules.StoreSolution.getInstance().getCountrybyCodeOrName(_countryCode);
            
            var rr = esUtilities.IPUtility.IpIsWithinBoliviaRange("219.90.3.101",
                          _store.LocationIps.Select(c => c.IPAtrrs).ToList());

            string s = "End";
        }
    }
}
