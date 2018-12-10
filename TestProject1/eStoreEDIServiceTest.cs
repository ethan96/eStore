using eStore.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using System.Linq;
namespace TestProject1
{


    /// <summary>
    ///This is a test class for eStoreEDIServiceTest and is intended
    ///to contain all eStoreEDIServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class eStoreEDIServiceTest
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
        ///A test for getProduct
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("D:\\MyProjects\\eStore3C\\eStore.UI", "/")]
        [UrlToTest("http://localhost:1888/")]
        public void getProductTest()
        {
            eStoreEDIService target = new eStoreEDIService(); // TODO: Initialize to an appropriate value
            string productid = "ADAM-5510M-A2E"; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getProduct(productid);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getProducts
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("D:\\MyProjects\\eStore3C\\eStore.UI", "/")]
        [UrlToTest("http://localhost:1888/")]
        public void getProductsTest()
        {
            eStoreEDIService target = new eStoreEDIService(); // TODO: Initialize to an appropriate value
            string[] productids = { "ADAM-3011-AE", "ADAM-3114-AE", "ADAM-3112-AE", "ADAM-3016-AE", "ADAM-3014-AE" };// TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getProducts(productids.ToList());
            Console.Write(actual);
           
        }
 
    }
}
