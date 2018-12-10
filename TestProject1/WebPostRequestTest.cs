using eStore.BusinessModules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Security;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for WebPostRequestTest and is intended
    ///to contain all WebPostRequestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WebPostRequestTest
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
        ///A test for WebPostRequest Constructor
        ///</summary>
        [TestMethod()]
        public void WebPostRequestConstructorTest()
        {
            string url = string.Empty; // TODO: Initialize to an appropriate value
            WebPostRequest target = new WebPostRequest(url);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            string url = string.Empty; // TODO: Initialize to an appropriate value
            WebPostRequest target = new WebPostRequest(url); // TODO: Initialize to an appropriate value
            string key = string.Empty; // TODO: Initialize to an appropriate value
            string value = string.Empty; // TODO: Initialize to an appropriate value
            target.Add(key, value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        /// Direct Link implementation for AEU Payment.
        ///A test for GetResponse
        ///</summary>
        [TestMethod()]
        public void GetResponseTest()
        {
            string url = "https://internetkassa.abnamro.nl/ncol/prod/orderdirect.asp"; // TODO: Initialize to an appropriate value
            WebPostRequest target = new WebPostRequest(url); // TODO: Initialize to an appropriate value
            string key ="1qa2ws3ed";
            decimal amount = 1;
            string cardno = "5178058727084223";
            string currency = "EUR";
            string operation = "RES";
            string orderid = "OEU100000";
            string pspid="Advantech";
            string pswd = "YQHTHA20"; 
            string cvc ="389";
            string ed= "09/13";
          
            string shaout = orderid + (amount * 100).ToString() + currency +cardno +  pspid + operation +  key;                                   
            string SHAOut= FormsAuthentication.HashPasswordForStoringInConfigFile(shaout, "SHA1");

            Console.WriteLine(shaout);

            target.Add("AMOUNT", (amount * 100).ToString());  // amount   
            target.Add("CARDNO", cardno);  // card no
            target.Add("Currency ", currency);  // Currency
            target.Add("CVC",cvc );  //  security code. 
            target.Add("ED",ed);  // expired date
            target.Add("operation", operation);  //  security code.
            target.Add("orderID", orderid);  // orderID   
            target.Add("PSPID", pspid);  // Affiliation name 
            target.Add("PSWD", pswd);  // password   
            target.Add("PM", ed);  // payment method   
            target.Add("USERID", "estoreAdvantech");  // USERID
           
            target.Add("SHASign", SHAOut);  //  security code.                     
            string expected = string.Empty; // TODO: Initialize to an appropriate value

            string actual;
            actual = target.GetResponse();

            XmlSerializer xmls = new XmlSerializer(typeof(ncresponse));
            StringReader sr = new StringReader(actual);
            XmlTextReader xr = new XmlTextReader(sr);

            ncresponse response = (ncresponse) xmls.Deserialize(xr);

            Console.WriteLine(response.NCERROR);
           
        }
    }
}
