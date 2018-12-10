using eStore.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for eStoreWebServiceTest and is intended
    ///to contain all eStoreWebServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class eStoreWebServiceTest
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
        ///A test for iseStoreProduct
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void iseStoreProductTest()
        {
            eStoreWebService target = new eStoreWebService(); // TODO: Initialize to an appropriate value
            string PartNo = "IPC-623BP"; // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            string standardProductlink = string.Empty; // TODO: Initialize to an appropriate value
            string standardProductlinkExpected = string.Empty; // TODO: Initialize to an appropriate value
            string buildsystemlink = string.Empty; // TODO: Initialize to an appropriate value
            string buildsystemlinkExpected = string.Empty; // TODO: Initialize to an appropriate value
            target.iseStoreProduct(PartNo, storeid, out standardProductlink, out buildsystemlink);

            Console.WriteLine(standardProductlink);
            Console.WriteLine(buildsystemlink);

        }

        /// <summary>
        ///A test for link2eStore
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("D:\\MyProjects\\eStore3C\\eStore.UI", "/")]
        [UrlToTest("http://localhost:1888/")]
        public void link2eStoreTest()
        {
            eStoreWebService target = new eStoreWebService(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            string ip = "216.2.25.227"; // TODO: Initialize to an appropriate value
            string modelNO = "AIMB-780"; // TODO: Initialize to an appropriate value
            string[] productids = { "AIMB-780QG2-00A1E", "AIMB-780WG2-00A1E" }; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;

            actual = target.link2eStore(storeid, ip, modelNO, productids);
            actual = target.link2eStoreByCountry("Indonesia", modelNO, productids);
            Console.WriteLine(actual);


             modelNO = "IPC-510"; // TODO: Initialize to an appropriate value
             string[] productids2 = {"IPC-510BP-25Z","IPC-510BP-30ZBE"
,"IPC-510BP-30ZE"
,"IPC-510MB-00XBE"
,"IPC-510MB-00XE"
,"IPC-510MB-30ZBE"
,"IPC-510MB-30ZE"};// TODO: Initialize to an appropriate value
             actual = target.link2eStore(storeid, ip, modelNO, productids2);
            Console.WriteLine(actual);

            modelNO = "UNO-3074"; // TODO: Initialize to an appropriate value
             string[] productids3 = {"UNO-3074-C11BE"
,"UNO-3074-C11E"
,"UNO-3074-P11E"
,"UNO-3074-P32BE"
,"UNO-3074-P32CE"
,"UNO-3074-P32E"};// TODO: Initialize to an appropriate value
             actual = target.link2eStore(storeid, ip, modelNO, productids3);
            Console.WriteLine(actual);

        }


        /// <summary>
        ///A test for link2eStore
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("D:\\MyProjects\\eStore3C\\eStore.UI", "/")]
        [UrlToTest("http://localhost:1888/")]
        public void link2eStorebycountryTest()
        {
            eStoreWebService target = new eStoreWebService(); // TODO: Initialize to an appropriate value
            string storeid = "AUS"; // TODO: Initialize to an appropriate value
            string ip = "adam-4520"; // TODO: Initialize to an appropriate value
            string modelNO = "AIMB-780"; // TODO: Initialize to an appropriate value
            string[] productids = { "adam-4520-d2e" }; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;

             
            actual = target.link2eStoreByCountry("us", modelNO, productids);
            Console.WriteLine(actual);
 
        }
    }
}
